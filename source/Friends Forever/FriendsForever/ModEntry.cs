using System;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;

namespace FriendsForever
{
    public class Config {
        /// <summary>Whether spouses should be prevented from having friendship decay.</summary>
        public bool AffectSpouses = false;
        /// <summary>Whether dates should be prevented from having friendship decay. Does nothing if married.</summary>
        public bool AffectDates = true;
        /// <summary>Whether everyone else should be prevented from having friendship decay. Effects everyone
        /// but the spouse if they are married.</summary>
        public bool AffectEveryoneElse = true;
        /// <summary>Whether animals should be prevented from having friendship decay. Due to SMAPI limitations you have
        /// to sleep in the bed for this to work.</summary>
        public bool AffectAnimals = true;
    }

    public class ModEntry : Mod {
        internal Config Config;
        internal Dictionary<FarmAnimal, int> AnimalFriendships = new Dictionary<FarmAnimal, int>();

        /// <summary>Mod entry point. Reads the config and adds the listeners.</summary>
        /// <param name="helper">Helper object for various mod functions (such as loading config files).</param>
        public override void Entry(IModHelper helper) {//is Menus.SaveGameMenu
            Config = helper.ReadConfig<Config>();
            helper.Events.GameLoop.DayStarted += StartDay;
            helper.Events.Display.MenuChanged += MenuClosed;
            helper.Events.GameLoop.Saving += BeforeSave;

            if (!(Config.AffectDates || Config.AffectDates || Config.AffectEveryoneElse || Config.AffectAnimals)) {
                Monitor.Log("This mod can be removed, all features currently disabled.", LogLevel.Warn);
            }
        }

        /// <summary>Start the day out by 'talking' to every NPC that we don't want friendship decay for.</summary>
        private void StartDay(object sender, DayStartedEventArgs e) {
            //This is a host-only mod:
            if (!Context.IsMainPlayer)
                return;
            
            AnimalFriendships.Clear();
        
            var farmers = Game1.getAllFarmers();
            var npcs = Utility.getAllCharacters();

            var farmerArray = farmers as Farmer[] ?? farmers.ToArray();
            foreach (NPC character in npcs) {
                if (!character.isVillager() && !character.IsMonster)
                    continue;

                foreach (Farmer farmer in farmerArray) {
                    if (!farmer.friendshipData.ContainsKey(character.getName()))
                        continue;
                    
                    var friendship = farmer.friendshipData[character.getName()];

                    if (farmer.spouse == character.Name && !Config.AffectSpouses) {
                        continue;
                    //If the they are 'dating' and we're not to affect dates, skip them. The exception to this is if
                    //the farmer has a spouse, in which case we want to treat them like everyone else:
                    } else if (friendship.Status == FriendshipStatus.Dating && !Config.AffectDates && farmer.spouse == null) {
                        continue;
                    } else if (!Config.AffectEveryoneElse) {
                        continue;
                    }

                    

                    //Set the flag for having talked to that character, but don't add any points.
                    //The player can talk to the person themselves and still get the 20 points.
                    friendship.TalkedToToday = true;
                }
            }
        }
        
        /// <summary>When a menu is closed, we want to see if the player is trying to sleep in bed. If so we need to
        /// save an animal's friendship toward a player if the animal has not been pet. This is a work around since
        /// SMAPI has no way to know whether an animal was pet or not at the end of the day.</summary>
        private void MenuClosed(object sender, MenuChangedEventArgs e) {
            //Player can't move if they select yes to the sleep dialog:
            if (Context.CanPlayerMove)
                return;

            //The sleep dialog box is of DialogBox in singleplayer but ReadyCheckDialog in multiplayer:
            if (!(e.OldMenu is DialogueBox || e.OldMenu is ReadyCheckDialog))
                return;
                
            var player = Game1.player;
            //To prevent error messages during the launch screen:
            if (player.currentLocation == null)
                return;

            Microsoft.Xna.Framework.Point bedLocation;
            //Of course the bed is in the FarmHouse location:
            if (player.currentLocation.Name == "FarmHouse") {
                bedLocation = ((FarmHouse) player.currentLocation).getBedSpot();
            } else if (player.currentLocation.Name == "Cabin") {
                bedLocation = ((Cabin) player.currentLocation).getBedSpot();
            } else {
                return;
            }
            
            //The bed location is in number of tiles where-as player position is pixels. 
            if (Math.Abs(bedLocation.X * Game1.tileSize - player.position.X) < Game1.tileSize
                && Math.Abs(bedLocation.Y * Game1.tileSize - player.position.Y) < Game1.tileSize)
            {
                AnimalFriendships.Clear();
                var animals = Game1.getFarm().getAllFarmAnimals().Distinct();
                foreach (var animal in animals) {
                    if (!animal.wasPet.Value)
                        AnimalFriendships[animal] = animal.friendshipTowardFarmer.Value;
                }
            }
        }
        
        /// <summary>Before the save, we want to add any lost friendship to animals.</summary>
        private void BeforeSave(object sender, SavingEventArgs e) {
            if (!(Config.AffectAnimals && Context.IsMainPlayer))
                return;
        
            var animals = Game1.getFarm().getAllFarmAnimals().Distinct();
            foreach (var animal in animals) {
                if (!AnimalFriendships.ContainsKey(animal))
                    continue;
                
                animal.friendshipTowardFarmer.Set(animal.friendshipTowardFarmer.Get() + (10 - AnimalFriendships[animal] / 200));
            }
        }
    }
}
