/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jasisco5/UncannyValleyMod
**
*************************************************/

using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewValley.Monsters;
using SpaceCore.UI;
using Microsoft.Xna.Framework;
using Force.DeepCloner;
using StardewValley.Network;

namespace UncannyValleyMod
{
    internal class ModQuests
    {
        IModHelper helper;
        IMonitor monitor;
        private ModWeapon modWeapon;
        public ModSaveData saveModel { get; set; }
        Dictionary<int, bool> questsObtained = new Dictionary<int, bool>();
        bool mansionMonstersSpawned = false;
        bool isBasementOpen = false;

        // Save Methods
        [EventPriority(EventPriority.Low)]
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            // Load Save Data
            if (saveModel.questsObtained.ContainsKey(2051901))
            {
                questsObtained = saveModel.questsObtained.DeepClone();
            } 
            else
            {
                saveModel.questsObtained = questsObtained.DeepClone();
            }
            isBasementOpen = saveModel.isBasementOpen.DeepClone();
            // Make sure Save data matches current version
            if(!questsObtained.ContainsKey(2051906)) { FixSave(); }
            // Save dependent functions
            SpawnBasementDoor();
            if (isBasementOpen && Game1.player.hasQuest("2051905"))
            {
                helper.Events.GameLoop.UpdateTicking += TriggerBasementEvent;
            }
        }
        private void FixSave()
        {
            for (int i = 2051901; i <= 2051910; i++)
            {
                if(!questsObtained.ContainsKey(i)) { questsObtained[i] = false; }
            }
        }
        [EventPriority(EventPriority.High)]
        private void OnSaving(object sender, SavingEventArgs e)
        {
            saveModel.isBasementOpen = isBasementOpen.DeepClone();
            saveModel.questsObtained = questsObtained.DeepClone();
        }

        private void initQuestObtained()
        {
            questsObtained.Clear();
            questsObtained[2051901] = false;
            questsObtained[2051902] = false;
            questsObtained[2051903] = false;
            questsObtained[2051904] = false;
            questsObtained[2051905] = false;
            questsObtained[2051906] = false;
            questsObtained[2051907] = false;
            questsObtained[2051908] = false;
            questsObtained[2051909] = false;
            questsObtained[2051910] = false;
        }
        private void SpawnBasementDoor()
        {
            //this.monitor.Log($"Spawning the Basement Door", LogLevel.Debug);
            if (isBasementOpen)
            {
                Game1.getLocationFromName("Custom_Mansion_Interior").setTileProperty(54, 24, "Buildings", "Action", "Warp 39 14 Custom_Mansion_Basement");
            } else
            {
                Game1.getLocationFromName("Custom_Mansion_Interior").setTileProperty(54, 24, "Buildings", "Action", "");
            }
        }
        public ModQuests(IModHelper _helper, IMonitor _monitor, ModWeapon _modWeapon)
        {
            monitor = _monitor;
            helper = _helper;
            modWeapon = _modWeapon;
            //SpaceCore.Api scApi;
            initQuestObtained();
            helper.Events.GameLoop.OneSecondUpdateTicking += this.OnQuestActivity;
            helper.Events.Player.Warped += this.OnWarped;
            helper.Events.GameLoop.DayStarted += (object sender, DayStartedEventArgs e) => 
            { 
                mansionMonstersSpawned = false; 
            };
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            helper.Events.GameLoop.Saving += this.OnSaving;

            // Start quest code
            SpaceCore.Events.SpaceEvents.OnEventFinished += this.OnEventFinished;
            // OnItemEaten - Check player.itemToEat for what they just ate.
            // BombExploded - When a bomb explodes in a location. Useful for zelda-like puzzle walls
        }

        

        private void OnWarped(object sender, WarpedEventArgs e)
        {
            if (Game1.getLocationFromName("Custom_Mansion_Interior") == e.NewLocation)
            {
                // On Slay Monster Quest
                if (questsObtained[2051904] == true && questsObtained[2051905] == false)
                {
                    // No need to respawn monsters since they are saved by default
                    //SpawnMansionMonsters();
                }
            }
        }

        /// <summary>
        /// Events called when an event/cutscene ends
        /// </summary>
        private void OnEventFinished(object sender, EventArgs e)
        {
            //this.monitor.Log($"Event id = {Game1.CurrentEvent.id}", LogLevel.Debug);
            // Jodi Event
            if (Game1.CurrentEvent.id == "2051901")
            {
                //this.monitor.Log($"Starting Act1", LogLevel.Debug);
                Game1.player.addQuest("2051901");
            }
            // Meet Apprentice
            if (Game1.CurrentEvent.id == "2051902")
            {
                //this.monitor.Log($"Starting Act1_2", LogLevel.Debug);
                Game1.player.addQuest("2051902");
                
                // Move player to where they are in the event
                void teleport(object sender, UpdateTickingEventArgs e)
                {
                    if(Game1.eventUp) { return; }
                    NetPosition position = Game1.player.position;
                    Warp mansionWarp = new Warp((int)(position.X / 64), (int)(position.Y / 64), "Custom_Mansion_Exterior", 23, 37, false);
                    Game1.player.warpFarmer(mansionWarp, 0);

                    helper.Events.GameLoop.UpdateTicking -= teleport;
                }
                helper.Events.GameLoop.UpdateTicking += teleport;
            }
            // Meet Butler
            if (Game1.CurrentEvent.id == "2051903")
            {
                //this.monitor.Log($"Starting Act2_3", LogLevel.Debug);
                Game1.player.addQuest("2051903");
            }

            // Breaking the Totem
            if (Game1.CurrentEvent.id == "2051904")
            {
                // Complete Quest
                Game1.player.completeQuest("2051905");
                Game1.player.addQuest("2051906");
                // Get Weapon
                if (!saveModel.weaponObtained) { modWeapon.AddWeaponToInv(); }
                // Start Chase Scene
                //this.monitor.Log($"Starting Chase Scene", LogLevel.Debug);
                void teleport(object sender, UpdateTickingEventArgs e)
                {
                    if (Game1.eventUp) { return; }
                    NetPosition position = Game1.player.position;
                    Warp mansionWarp = new Warp((int)(position.X / 64), (int)(position.Y / 64), "Custom_Mansion_Basement_Chase", 78, 71, false);
                    Game1.player.warpFarmer(mansionWarp, 0);

                    helper.Events.GameLoop.UpdateTicking -= teleport;
                }
                helper.Events.GameLoop.UpdateTicking += teleport;
            }
        }

        /// <summary>
        /// Events called when the player first obtains a quest.
        /// Called once every second.
        /// </summary>
        private void OnQuestActivity(object sender, OneSecondUpdateTickingEventArgs e)
        {
            // True when the quest icon is glowing
            if (Game1.player.hasNewQuestActivity())
            {
                // First Obtained the inital quest
                if (!questsObtained[2051901] && Game1.player.hasQuest("2051901"))
                {
                    questsObtained[2051901] = true;
                    //this.monitor.Log($"{Game1.player.Name} obtained quest Act1", LogLevel.Debug);
                }
                // Obtained Act2_1 (Go to Mansion)
                if (!questsObtained[2051902] && Game1.player.hasQuest("2051902"))
                {
                    questsObtained[2051902] = true;
                    //this.monitor.Log($"{Game1.player.Name} obtained quest Act2_1", LogLevel.Debug);
                }
                // Obtained Act2_2 (Speak to the Butler)
                if (!questsObtained[2051903] && Game1.player.hasQuest("2051903"))
                {
                    questsObtained[2051903] = true;
                    //this.monitor.Log($"{Game1.player.Name} obtained quest Act2_2", LogLevel.Debug);

                    helper.Events.GameLoop.OneSecondUpdateTicking += this.Act2_2;
                }
                // Obtained Act2_3 (Slay Skeletons)
                if (!questsObtained[2051904] && Game1.player.hasQuest("2051904"))
                {
                    questsObtained[2051904] = true;
                    //this.monitor.Log($"{Game1.player.Name} obtained quest Act2_3", LogLevel.Debug);

                    SpawnMansionMonsters();
                }
                // Obtained Act2_4 (Break Totem)
                if (!questsObtained[2051905] && Game1.player.hasQuest("2051905"))
                {
                    questsObtained[2051905] = true;
                    // Open Basement
                    //this.monitor.Log($"{Game1.player.Name} finished quest Act2_3", LogLevel.Debug);
                    Game1.addHUDMessage(HUDMessage.ForCornerTextbox($"Got the key to the Mansion's Basement."));
                    isBasementOpen = true;
                    SpawnBasementDoor();
                    // Add totem logic
                    //this.monitor.Log($"{Game1.player.Name} obtained quest Act2_4", LogLevel.Debug);


                    helper.Events.GameLoop.UpdateTicking += TriggerBasementEvent;
                    //helper.Events.Input.ButtonPressed += this.AttackTotem;
                }

            }
        }

        /// <summary>
        /// Act2_2 : Speaking to 'Butler' completes the quest
        /// </summary>
        private void Act2_2(object sender, OneSecondUpdateTickingEventArgs e)
        {
            if(Game1.currentSpeaker != null)
            {
                //this.monitor.Log($"Current Speaker = {Game1.currentSpeaker.Name}.", LogLevel.Debug);
                if (Game1.currentSpeaker.Name == "Butler")
                {
                    Game1.player.completeQuest("2051903");
                    helper.Events.GameLoop.OneSecondUpdateTicking -= this.Act2_2;
                    Game1.player.addQuest("2051904");
                }
            }
        }

        /// <summary>
        /// Act2_4 : Attack the Totem with the Spectral Sabre
        /// </summary>
        /*
        private void AttackTotem(object sender, ButtonPressedEventArgs e)
        {
            if (Game1.currentLocation == null) { return; }
            if (Game1.currentLocation == Game1.getLocationFromName("Custom_Mansion_Basement") && e.Button.IsUseToolButton())
            {
                Vector2 position = e.Cursor.GrabTile;
                this.monitor.Log($"Clicked Tile = {position}.", LogLevel.Debug);
                if (position.Equals(new Vector2(78, 72)))
                {
                    this.monitor.Log($"Totem Tile Clicked", LogLevel.Debug);
                    if (Game1.player.CurrentTool.hasEnchantmentOfType<ReapingEnchantment>())
                    {
                        this.monitor.Log($"Spectral Saber Used", LogLevel.Debug);
                        Game1.player.completeQuest("2051905");
                        Game1.addHUDMessage(HUDMessage.ForCornerTextbox($"Reached the end of what is currently available."));
                        helper.Events.Input.ButtonPressed -= this.AttackTotem;
                    }
                }
            }
        }
        */

        private void TriggerBasementEvent(object sender, UpdateTickingEventArgs e)
        {
            if (Game1.eventUp) { return; }
            if (Game1.currentLocation == Game1.getLocationFromName("Custom_Mansion_Basement"))
            {
                Vector2 pos = Game1.player.position.Get();
                pos = new Vector2((int)(pos.X / 64), (int)(pos.Y / 64));
                //this.monitor.Log($"Player Position is: {pos.X}, {pos.Y}", LogLevel.Debug);
                //78, 58
                if ( (pos.X == 78 || pos.X == 77)  && pos.Y == 58)
                {
                    helper.Events.GameLoop.UpdateTicking -= TriggerBasementEvent;
                    Game1.PlayEvent("2051904", false, false);
                }
            }
        }
        // Spawn Monsters
        private void SpawnMansionMonsters()
        {
            if (mansionMonstersSpawned) { return; }
            //this.monitor.Log($"Spawning Monsters", LogLevel.Debug);
            mansionMonstersSpawned = true;
            Monster[] slimes = new Monster[20];
            slimes[0] = new GreenSlime(new Microsoft.Xna.Framework.Vector2(70 * 64, 27 * 64), Microsoft.Xna.Framework.Color.BlueViolet);
            slimes[1] = new GreenSlime(new Microsoft.Xna.Framework.Vector2(8 * 64, 27 * 64), Microsoft.Xna.Framework.Color.BlueViolet);
            slimes[2] = new GreenSlime(new Microsoft.Xna.Framework.Vector2(4 * 64, 42 * 64), Microsoft.Xna.Framework.Color.BlueViolet);
            slimes[3] = new GreenSlime(new Microsoft.Xna.Framework.Vector2(4 * 64, 10 * 64), Microsoft.Xna.Framework.Color.DeepSkyBlue);
            slimes[4] = new GreenSlime(new Microsoft.Xna.Framework.Vector2(23 * 64, 8 * 64), Microsoft.Xna.Framework.Color.BlueViolet);
            slimes[5] = new GreenSlime(new Microsoft.Xna.Framework.Vector2(37 * 64, 9 * 64), Microsoft.Xna.Framework.Color.BlueViolet);
            slimes[6] = new GreenSlime(new Microsoft.Xna.Framework.Vector2(49 * 64, 11 * 64), Microsoft.Xna.Framework.Color.DeepSkyBlue);
            slimes[7] = new GreenSlime(new Microsoft.Xna.Framework.Vector2(65 * 64, 10 * 64), Microsoft.Xna.Framework.Color.BlueViolet);
            slimes[8] = new GreenSlime(new Microsoft.Xna.Framework.Vector2(89 * 64, 10 * 64), Microsoft.Xna.Framework.Color.BlueViolet);
            slimes[9] = new GreenSlime(new Microsoft.Xna.Framework.Vector2(69 * 64, 43 * 64), Microsoft.Xna.Framework.Color.DeepSkyBlue);
            slimes[10] = new GreenSlime(new Microsoft.Xna.Framework.Vector2(71 * 64, 27 * 64), Microsoft.Xna.Framework.Color.BlueViolet);
            slimes[11] = new GreenSlime(new Microsoft.Xna.Framework.Vector2(9 * 64, 27 * 64), Microsoft.Xna.Framework.Color.BlueViolet);
            slimes[12] = new GreenSlime(new Microsoft.Xna.Framework.Vector2(5 * 64, 42 * 64), Microsoft.Xna.Framework.Color.BlueViolet);
            slimes[13] = new GreenSlime(new Microsoft.Xna.Framework.Vector2(5 * 64, 10 * 64), Microsoft.Xna.Framework.Color.DeepSkyBlue);
            slimes[14] = new GreenSlime(new Microsoft.Xna.Framework.Vector2(24 * 64, 8 * 64), Microsoft.Xna.Framework.Color.BlueViolet);
            slimes[15] = new GreenSlime(new Microsoft.Xna.Framework.Vector2(38 * 64, 9 * 64), Microsoft.Xna.Framework.Color.BlueViolet);
            slimes[16] = new GreenSlime(new Microsoft.Xna.Framework.Vector2(50 * 64, 11 * 64), Microsoft.Xna.Framework.Color.DeepSkyBlue);
            slimes[17] = new GreenSlime(new Microsoft.Xna.Framework.Vector2(66 * 64, 10 * 64), Microsoft.Xna.Framework.Color.BlueViolet);
            slimes[18] = new GreenSlime(new Microsoft.Xna.Framework.Vector2(90 * 64, 10 * 64), Microsoft.Xna.Framework.Color.BlueViolet);
            slimes[19] = new GreenSlime(new Microsoft.Xna.Framework.Vector2(70 * 64, 43 * 64), Microsoft.Xna.Framework.Color.DeepSkyBlue);
            foreach (var slime in slimes)
            {
                Game1.currentLocation.characters.Add(slime);
            }
            /*
            Monster[] skeletons = new Monster[10];
            skeletons[0] = new Skeleton(new Microsoft.Xna.Framework.Vector2(70 * 64, 27 * 64), false);
            skeletons[1] = new Skeleton(new Microsoft.Xna.Framework.Vector2(8 * 64, 27 * 64),  false);
            skeletons[2] = new Skeleton(new Microsoft.Xna.Framework.Vector2(4 * 64, 42 * 64),  false);
            skeletons[3] = new Skeleton(new Microsoft.Xna.Framework.Vector2(4 * 64, 10 * 64),  false);
            skeletons[4] = new Skeleton(new Microsoft.Xna.Framework.Vector2(23 * 64, 8 * 64),  false);
            skeletons[5] = new Skeleton(new Microsoft.Xna.Framework.Vector2(37 * 64, 9 * 64),  true);
            skeletons[6] = new Skeleton(new Microsoft.Xna.Framework.Vector2(49 * 64, 11 * 64), false);
            skeletons[7] = new Skeleton(new Microsoft.Xna.Framework.Vector2(65 * 64, 10 * 64), false);
            skeletons[8] = new Skeleton(new Microsoft.Xna.Framework.Vector2(89 * 64, 10 * 64), false);
            skeletons[9] = new Skeleton(new Microsoft.Xna.Framework.Vector2(69 * 64, 43 * 64), true);
            foreach (var skeleton in skeletons)
            {
                Game1.currentLocation.characters.Add(skeleton);
                Game1.currentLocation.characters.Add(skeleton);
            }
            */
        }
        void KillTheButler()
        {
            foreach (NPC npc in Game1.currentLocation.characters)
            {
                // Check if the character is the butler before sending them to the shadow realm
                if (npc.Name != "Butler") continue;
                Game1.currentLocation.characters.Remove(npc);
                return; // Stop iterating once the deed is done
            }
        }
    }
}
