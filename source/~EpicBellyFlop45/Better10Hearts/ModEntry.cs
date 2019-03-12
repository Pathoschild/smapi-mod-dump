using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Locations;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static StardewValley.Network.OverlaidDictionary;

namespace Better10Hearts
{
    class ModEntry : Mod, IAssetEditor
    {
        public IDictionary<string, bool> npcEnergyGeneration = new Dictionary<string, bool>();
        public ModConfig Config;
        public static bool HasPassoutBeenHandled = false;

        /// <summary>
        /// Called when the mod is first loaded up, want to add all event handlers and load the mail list to add
        /// </summary>
        /// <param name="helper"></param>
        public override void Entry(IModHelper helper)
        {
            this.Helper.Events.GameLoop.DayStarted += Events_DayStarted;
            this.Helper.Events.GameLoop.TimeChanged += Events_TimeChanged;
            this.Helper.Events.GameLoop.UpdateTicked += Events_UpdateTicked;
            
            Config = this.Helper.ReadConfig<ModConfig>();

            // Create a new Harmony instance for patching source code
            HarmonyInstance harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);

            // Get the method we want to patch
            MethodInfo targetMethod = AccessTools.Method(typeof(Farmer), nameof(Farmer.passOutFromTired));

            // Get the patch that was created
            MethodInfo prefix = AccessTools.Method(typeof(ModEntry), nameof(ModEntry.Prefix));

            // Apply the patch
            harmony.Patch(targetMethod, prefix: new HarmonyMethod(prefix));
        }
        
        /// <summary>
        /// Used to patch 
        /// </summary>
        /// <returns></returns>
        private static bool Prefix(Farmer who)
        {
            if (!who.IsLocalPlayer)
                return false;
            if (who.isRidingHorse())
                who.mount.dismount();
            if (Game1.activeClickableMenu != null)
            {
                Game1.activeClickableMenu.emergencyShutDown();
                Game1.exitActiveMenu();
            }
            who.completelyStopAnimatingOrDoingAction();
            if (who.bathingClothes.Value)
                who.changeOutOfSwimSuit();
            who.swimming.Value = false;
            who.CanMove = false;
            GameLocation passOutLocation = Game1.currentLocation;
            Vector2 bed = Utility.PointToVector2(Utility.getHomeOfFarmer(Game1.player).getBedSpot()) * 64f;
            bed.X -= 64f;
            LocationRequest.Callback callback = (LocationRequest.Callback)(() =>
            {
                who.Position = bed;
                who.currentLocation.lastTouchActionLocation = bed;
                if (!Game1.IsMultiplayer || Game1.timeOfDay >= 2600)
                    Game1.PassOutNewDay();
                Game1.changeMusicTrack("none");
                if (passOutLocation is FarmHouse || passOutLocation is Cellar)
                    return;

                // Check if an NPC has piked up the player or if the game should handle the passout
                if (!ModEntry.HasPassoutBeenHandled)
                {
                    int num = Math.Min(1000, who.Money / 10);
                    who.Money -= num;
                    who.mailForTomorrow.Add("passedOut " + (object)num);
                }
            });
            if (!who.isInBed.Value)
            {
                LocationRequest locationRequest = Game1.getLocationRequest(who.homeLocation.Value, false);
                Game1.warpFarmer(locationRequest, (int)bed.X / 64, (int)bed.Y / 64, 2);
                locationRequest.OnWarp += callback;
                who.FarmerSprite.setCurrentSingleFrame(5, (short)3000, false, false);
                who.FarmerSprite.PauseForSingleAnimation = true;
            }
            else
                callback();

            return false;
        }

        /// <summary>
        /// Check every second if the player has spoken to any NPC since the previous second
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Events_UpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            // Make sure a save is loaded and to only run code once every second
            if (!Context.IsWorldReady || !e.IsOneSecond)
            {
                return;
            }

            List<string> npcNamesToChange = new List<string>();

            foreach (var key in npcEnergyGeneration.Keys)
            {
                NPC npc = Game1.getCharacterFromName(key);

                bool previousHasSpokenToday = npcEnergyGeneration[npc.Name];
                bool newHasSpokenToday = Game1.player.hasTalkedToFriendToday(npc.Name);

                // Check if there the player has spoken to NPC
                if (previousHasSpokenToday != newHasSpokenToday)
                {
                    var npcSpouse = npc.getSpouse();

                    // This is to prevent a NullException error if the NPC doesn't have a spouse
                    if (npcSpouse != null)
                    {
                        // Check if NPC is player's spouse or not
                        if (npcSpouse.Name == Game1.player.Name)
                        {
                            // Increase the player's stamina by the config value for the spouse. Make sure not to go above the max and it is increased and not decreased
                            Game1.player.Stamina = Math.Min(Game1.player.MaxStamina, Game1.player.Stamina + Math.Max(0, Config.SpouseStaminaIncrease));
                        }
                        else
                        {
                            // Increase the player's stamina by the config value for the NPC. Make sure not to go above the max and it is increased and not decreased
                            Game1.player.Stamina = Math.Min(Game1.player.MaxStamina, Game1.player.Stamina + Math.Max(0, Config.NPCStaminaIncrease));
                        }
                    }
                    else
                    {
                        // Increase the player's stamina by the config value for the NPC. Make sure not to go above the max and it is increased and not decreased
                        Game1.player.Stamina = Math.Min(Game1.player.MaxStamina, Game1.player.Stamina + Math.Max(0, Config.NPCStaminaIncrease));
                    }

                    // Add the names to a list so later on we can set them to true. We can't change it now as the list that needs changing is being iterated
                    npcNamesToChange.Add(npc.Name);
                }
            }

            // Iternate through that list that the names were added to, make the changes
            if (npcNamesToChange.Count > 0)
            {
                foreach (var name in npcNamesToChange)
                {
                    npcEnergyGeneration[name] = true;
                }
            }
        }

        /// <summary>
        /// Will check everytime the time changes for checking where the player has passed out at and if there are any NPC in the area
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Events_TimeChanged(object sender, TimeChangedEventArgs e)
        {
            // Get to current location to check the NPCs against
            var location = Game1.currentLocation;

            // Make sure an NPC is in the area passed out in and that the time is 2am +
            if (location.characters.Count == 0 || Game1.timeOfDay < 2600)
            {
                return;
            }

            // Make sure to set the passout as being handled so the game doesn't handle it
            HasPassoutBeenHandled = true;

            // Pick a random NPC that is in that location and set as the mail sender
            NPC letterSender = location.characters[new Random().Next(location.characters.Count)];

            // Check if the player is in the NPC's home or not
            if (letterSender.DefaultMap == location.Name)
            {
                // Send the player mail from that NPC as if they are at home
                Game1.mailbox.Add("better10Hearts_" + letterSender.Name + "_Home");
            }
            else
            {
                // Send the player mail from that NPC as if they are away from home
                Game1.mailbox.Add("better10Hearts_" + letterSender.Name + "_Away");
            }
        }

        /// <summary>
        /// Will check at the begining of each day, it will go through each NPC and check if it's their birthday, if so if they have enough friendship points to give max luck
        /// It will also add all NPCs that have 10hearts, so when the player talks to them the list can be compared to see if the player should receive energy
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Events_DayStarted(object sender, DayStartedEventArgs e)
        {
            // Mark passout handled as false for the new day
            HasPassoutBeenHandled = false;

            // Remove all the mail added by the mod from the mailbox so it can be resent each day
            foreach (var mail in Game1.player.mailReceived)
            {
                if (mail.Contains("better10Hearts"))
                {
                    Game1.player.mailReceived.Remove(mail);
                }
            }

            // Empty the list, this means it's regenerated every day. Meaning if an NPC goes below 10, they no longer will be on the list
            npcEnergyGeneration.Clear();

            foreach (NPC npc in Utility.getAllCharacters())
            {
                // Check the player has friendship data with the NPC
                if (Game1.player.friendshipData.ContainsKey(npc.Name))
                {
                    // Retrieve that frienship data and calculate how many hearts they have
                    var friendship = Game1.player.friendshipData[npc.Name];

                    float hearts = friendship.Points / NPC.friendshipPointsPerHeartLevel;

                    // Check if friendship is at 10 hearts
                    if (hearts >= 10)
                    {
                        // Add the npc to a list to check if they have spoken today
                        npcEnergyGeneration.Add(npc.Name, false);

                        // Check if it's the NPC's birthday
                        if (npc.isBirthday(Game1.currentSeason, Game1.dayOfMonth))
                        {
                            // Set max luck 
                            Game1.dailyLuck = 0.12;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Will call when going though each asset, if the mail asset is passed in, return true as we want to edit this
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="asset"></param>
        /// <returns></returns>
        public bool CanEdit<T>(IAssetInfo asset)
        {
            // If the mail asset is passed in, return true so we can edit it
            if (asset.AssetNameEquals("Data/mail"))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Edit the mail asset to add the new mail for when the player passes out
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="asset"></param>
        public void Edit<T>(IAssetData asset)
        {
            // Add new mail items
            if (asset.AssetNameEquals("Data/mail"))
            {
                IDictionary<string, string> newMailList = this.Helper.Content.Load<Dictionary<string, string>>("Assets/NewMail.json", ContentSource.ModFolder);

                IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;

                // Add each new mail item to the mail list
                foreach (var newMail in newMailList)
                {
                    data[newMail.Key] = newMail.Value;
                }
            }
        }
    }
}
