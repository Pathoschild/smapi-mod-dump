/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TheMightyAmondee/CustomDeathPenaltyPlus
**
*************************************************/

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using System.Linq;
using StardewValley.Locations;
using StardewValley.Menus;

namespace CustomDeathPenaltyPlus
{
    /// <summary>
    /// Extensions for the PassOutPenaltyChanges class
    /// </summary>
    internal static class PassOutPenaltyChangesExtensions
    {     
        public static void Reconcile(this ModConfig.PassOutPenaltyChanges changes, IMonitor monitor)
        {
            // Reconcile MoneytoRestorePercentage if it's value is outside the useable range
            if (false
                || changes.MoneytoRestorePercentage > 1
                || changes.MoneytoRestorePercentage < 0)
            {
                monitor.Log($"MoneytoRestorePercentage in PassOutPenalty is invalid, default value will be used instead... {changes.MoneytoRestorePercentage} isn't a decimal between 0 and 1", LogLevel.Warn);
                changes.MoneytoRestorePercentage = 0.95;
            }

            // Reconcile MoneyLossCap if it's value is -ve
            if (changes.MoneyLossCap < 0)
            {
                monitor.Log("MoneyLossCap in PassOutPenalty is invalid, default value will be used instead... Using a negative number won't add money, nice try though", LogLevel.Warn);
                changes.MoneyLossCap = 500;
            }

            // Reconcile EnergytoRestorePercentage if it's value is ouside the useable range
            if (false
                || changes.EnergytoRestorePercentage > 1
                || changes.EnergytoRestorePercentage < 0)
            {
                monitor.Log($"EnergytoRestorePercentage in PassOutPenalty is invalid, default value will be used instead... {changes.EnergytoRestorePercentage} isn't a decimal between 0 and 1", LogLevel.Warn);
                changes.EnergytoRestorePercentage = 0.50;
            }
        }
    }

    /// <summary>
    /// Extensions for the DeathPenaltyChanges class
    /// </summary>
    internal static class DeathPenaltyChangesExtensions
    {
        public static void Reconcile(this ModConfig.DeathPenaltyChanges changes, IMonitor monitor)
        {
            // Reconcile MoneytoRestorePercentage if it's value is ouside the useable range
            if (false
                || changes.MoneytoRestorePercentage > 1
                || changes.MoneytoRestorePercentage < 0)
            {
                monitor.Log($"MoneytoRestorePercentage in DeathPenalty is invalid, default value will be used instead... {changes.MoneytoRestorePercentage} isn't a decimal between 0 and 1", LogLevel.Warn);
                changes.MoneytoRestorePercentage = 0.95;
            }

            // Reconcile MoneyLossCap if the value is -ve
            if (changes.MoneyLossCap < 0)
            {
                monitor.Log("MoneyLossCap in DeathPenalty is invalid, default value will be used instead... Using a negative number won't add money, nice try though", LogLevel.Warn);
                changes.MoneyLossCap = 500;
            }

            // Reconcile EnergytoRestorePercentage if it's value is ouside the useable range
            if (false
                || changes.EnergytoRestorePercentage > 1
                || changes.EnergytoRestorePercentage < 0)
            {
                monitor.Log($"EnergytoRestorePercentage in DeathPenalty is invalid, default value will be used instead... {changes.EnergytoRestorePercentage} isn't a decimal between 0 and 1", LogLevel.Warn);
                changes.EnergytoRestorePercentage = 0.10;
            }

            // Reconcile HealthtoRestorePercentage if it's value is ouside the useable range
            if (false
                || changes.HealthtoRestorePercentage > 1
                || changes.HealthtoRestorePercentage <= 0)
            {
                monitor.Log($"HealthtoRestorePercentage in DeathPenalty is invalid, default value will be used instead... {changes.HealthtoRestorePercentage} isn't a decimal between 0 and 1, excluding 0", LogLevel.Warn);
                changes.HealthtoRestorePercentage = 0.50;
            }
        }
    }

    /// <summary>Holds the booleans used by the mod to know what to load and when.</summary>
    internal class Toggles
    {
        internal bool warptoinvisiblelocation = false;

        internal bool loadstate = false;

        internal bool shouldtogglepassoutdata = true;
    }

    /// <summary>The mod entry point.</summary>
    public class ModEntry
        : Mod
    {
        private ModConfig config;

        private static readonly PerScreen<Toggles> togglesperscreen = new PerScreen<Toggles>(createNewState: () => new Toggles());

        public static string location;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
            helper.Events.GameLoop.GameLaunched += this.GameLaunched;
            helper.Events.GameLoop.Saving += this.Saving;
            helper.Events.GameLoop.DayStarted += this.DayStarted;
            helper.Events.GameLoop.DayEnding += this.DayEnding;
            helper.Events.Multiplayer.ModMessageReceived += this.MessageReceived;

            // Read the mod config for values and create one if one does not currently exist
            this.config = this.Helper.ReadConfig<ModConfig>();

            // Add console command
            helper.ConsoleCommands.Add("configinfo", "Displays the current config settings", this.Info);

            // Allow other classes to use the ModConfig
            PlayerStateRestorer.SetConfig(this.config);
            AssetEditor.SetConfig(this.config, this.ModManifest);
        }

        /// <summary>Raised after the game is launched, right before the first game tick</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // Reconcile config values
            this.config.PassOutPenalty.Reconcile(this.Monitor);
            this.config.DeathPenalty.Reconcile(this.Monitor);

            if (this.config.OtherPenalties.WakeupNextDayinClinic == true && this.config.OtherPenalties.MoreRealisticWarps == true)
            {
                this.config.OtherPenalties.MoreRealisticWarps = false;
                this.Monitor.Log("WakeupNextDayinClinic and MoreRealisticWarps cannot both be true as they can conflict.\nSetting MoreRealisticWarps to false...", LogLevel.Warn);
            }

            // Is WakeupNextDayinClinic true or is FriendshipPenalty greater than 0?
            if (this.config.OtherPenalties.WakeupNextDayinClinic == true || this.config.OtherPenalties.HarveyFriendshipChange != 0)
            {
                // Yes, edit some events

                //Edit MineEvents
                this.Helper.Content.AssetEditors.Add(new AssetEditor.MineEventFixes(Helper));
                //Edit IslandSouthEvents
                this.Helper.Content.AssetEditors.Add(new AssetEditor.IslandSouthEventFixes(Helper));
                //Edit HospitalEvents
                this.Helper.Content.AssetEditors.Add(new AssetEditor.HospitalEventFixes(Helper));
            }

            // Is MoreRealisticWarps true?
            else if (this.config.OtherPenalties.MoreRealisticWarps == true)
            {
                // Yes, edit an event

                //Edit HospitalEvents
                this.Helper.Content.AssetEditors.Add(new AssetEditor.HospitalEventFixes(Helper));
            }

            // Edit strings
            this.Helper.Content.AssetEditors.Add(new AssetEditor.StringsFromCSFilesFixes(Helper));
            
            // Edit mail
            this.Helper.Content.AssetEditors.Add(new AssetEditor.MailDataFixes(Helper));
        }

        /// <summary>Raised after the game state is updated</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            // Reload events if needed
            if (true
                // Player has died, before killscreen is true
                && Game1.player.health <= 0 
                // death state has not been saved
                && PlayerStateRestorer.statedeathps.Value == null
                // location of death hasn't been saved
                && location == null 
                // MoreRealisticWarps is true
                && this.config.OtherPenalties.MoreRealisticWarps == true)
            {
                // Save location of death
                location = Game1.currentLocation.NameOrUniqueName;
                // Reload events
                this.Helper.Content.InvalidateCache("Data\\Events\\Hospital");
            }

            // Check if player died each half second
            if (e.IsMultipleOf(30))
            {
                if (true
                    // Has player died?
                    && Game1.killScreen == true
                    // Has the players death state been saved?
                    && PlayerStateRestorer.statedeathps.Value == null)
                {
                    // Save playerstate using DeathPenalty values
                    PlayerStateRestorer.SaveStateDeath();

                    // Reload asset upon death to reflect amount lost
                    this.Helper.Content.InvalidateCache("Strings\\StringsFromCSFiles");

                    // Will a new day be loaded in multiplayer after death?
                    if (true
                        // It is multiplayer
                        && Context.IsMultiplayer == true 
                        // WakeupNextDayinClinic is true
                        && this.config.OtherPenalties.WakeupNextDayinClinic == true)
                    {
                        // Set warptoinvisiblelocation to true
                        togglesperscreen.Value.warptoinvisiblelocation = true;
                    }
                }
            }

            if(true
                // Player death state has been saved
                && PlayerStateRestorer.statedeathps.Value != null
                // An event is in progress, this would be the PlayerKilled event
                && Game1.CurrentEvent != null)
            {
                // Set loadstate to true so state will be loaded after event
                togglesperscreen.Value.loadstate = true;

                // Current active menu can be downcast to an itemlistmenu
                if (Game1.activeClickableMenu as ItemListMenu != null)
                {
                    // Will items be restored?
                    if (this.config.DeathPenalty.RestoreItems == true)
                    {
                        // Yes, we don't want that menu, so close it and end the event

                        // Close the menu
                        Game1.activeClickableMenu.exitThisMenuNoSound();
                        // End the event
                        Game1.CurrentEvent.exitEvent();
                    }

                    // Load state earlier in multiplayer
                    if (Context.IsMultiplayer == true)
                    {
                        // Restore playerstate using DeathPenalty values
                        PlayerStateRestorer.LoadStateDeath();

                        // Clear state if WakeupNextDayinClinic is false, other stuff needs to be done if it's true
                        if (this.config.OtherPenalties.WakeupNextDayinClinic == false)
                        {
                            // Reset PlayerStateRestorer class with the statedeath field
                            PlayerStateRestorer.statedeathps.Value = null;

                            // State already loaded and cleared, set loadstate to false
                            togglesperscreen.Value.loadstate = false;
                        }
                    }

                    // Should the player be warped where they can't be seen?
                    if (togglesperscreen.Value.warptoinvisiblelocation == true)
                    {
                        // Yes, warp player to an invisible location

                        Game1.warpFarmer(Game1.currentLocation.NameOrUniqueName, 1000, 1000, false);
                        // Set warptoinvisiblelocation to false to stop endless warp loop
                        togglesperscreen.Value.warptoinvisiblelocation = false;
                    }

                    // Set player exit location for event
                    else if (true 
                        && this.config.OtherPenalties.MoreRealisticWarps == true  
                        && location != null)
                    {
                        if (location == "SkullCave" 
                            || (location.StartsWith("UndergroundMine") == true 
                            && Game1.currentLocation.NameOrUniqueName != "Mine"))
                        {
                            Game1.CurrentEvent.setExitLocation("SkullCave", 3, 5);
                        }

                        else if (true 
                            && (false 
                            || ModEntry.location.StartsWith("Farm") == true 
                            || Game1.getLocationFromName(ModEntry.location) as FarmHouse != null) 
                            && location.StartsWith("IslandFarm") == false)
                        {
                            int tileX = 12;
                            int tileY = 18;
                            switch (Game1.player.houseUpgradeLevel)
                            {
                                case 0:
                                    tileX = 3;
                                    tileY = 9;
                                    break;
                                case 1:
                                    tileX = 9;
                                    tileY = 8;
                                    break;
                                default:
                                    break;
                            }
                            Game1.CurrentEvent.setExitLocation(Game1.player.homeLocation.Value, tileX, tileY);
                        }
                        location = null;
                    }
                }
            }

            if(true
                // Player death state has been saved
                && PlayerStateRestorer.statedeathps.Value != null
                // Player isn't warping
                && Game1.isWarping == false
                // No events are running
                && Game1.CurrentEvent == null
                // state should be loaded
                && togglesperscreen.Value.loadstate == true)
            {
                // Set loadstate to false
                togglesperscreen.Value.loadstate = false;

                // Start new day if necessary
                if (this.config.OtherPenalties.WakeupNextDayinClinic == true)
                {
                    // Save necessary data to data model
                    Game1.player.modData[$"{this.ModManifest.UniqueID}.DidPlayerWakeupinClinic"] = "true";


                    // Is the game multiplayer?
                    if (Context.IsMultiplayer == false)
                    {
                        // No, load new day immediately

                        Game1.NewDay(1.1f);
                    }

                    else
                    {
                        // Yes, inform other players you're ready for a new day

                        Game1.player.team.SetLocalReady("sleep", true);

                        // Ensures new day will load, will become false after new day is loaded
                        Game1.player.passedOut = true;

                        // Create class instance to hold player's name
                        Multiplayer multiplayer = new Multiplayer
                        {
                            PlayerWhoDied = Game1.player.Name
                        };
                                             
                        // Send data from class instance to other players, message type is IDied
                        this.Helper.Multiplayer.SendMessage(multiplayer, "IDied", modIDs: new[] { this.ModManifest.UniqueID });

                        // Bring up a new menu that will launch a new day when all player's are ready
                        Game1.activeClickableMenu = (IClickableMenu)new ReadyCheckDialog("sleep", false, (ConfirmationDialog.behavior)(_ => Game1.NewDay(1.1f)));

                        // Reset PlayerStateRestorer class with the statedeath field
                        PlayerStateRestorer.statedeathps.Value = null;

                        // Add player to list of ready farmers if needed
                        if (Game1.player.team.announcedSleepingFarmers.Contains(Game1.player) == true) return;
                        Game1.player.team.announcedSleepingFarmers.Add(Game1.player);
                    }
                }

                // Restore state after PlayerKilled event ends if new day hasn't been loaded
                else
                {
                    // Restore Player state using DeathPenalty values
                    PlayerStateRestorer.LoadStateDeath();

                    // Reset PlayerStateRestorer class with the statedeath field
                    PlayerStateRestorer.statedeathps.Value = null;
                }
            }

            // Check if time is 2am or the player has passed out
            if (Game1.timeOfDay == 2600 || Game1.player.stamina <= -15)
            {
                // Set DidPlayerPassOutYesterday to true and DidPlayerWakeupinClinic to false in data model
                Game1.player.modData[$"{this.ModManifest.UniqueID}.DidPlayerPassOutYesterday"] = "true";
                Game1.player.modData[$"{this.ModManifest.UniqueID}.DidPlayerWakeupinClinic"] = "false";

                if (true
                    // Player is not in FarmHouse
                    && Game1.player.currentLocation as FarmHouse == null
                    // Player is not in Cellar
                    && Game1.player.currentLocation as Cellar == null
                    // Player pass out state has not been saved
                    && PlayerStateRestorer.statepassoutps.Value == null)
                {
                    // Save playerstate using PassOutPenalty values
                    PlayerStateRestorer.SaveStatePassout();
                    // Save amount lost to data model
                    Game1.player.modData[$"{this.ModManifest.UniqueID}.MoneyLostLastPassOut"] = $"{(int)Math.Round(PlayerStateRestorer.statepassoutps.Value.moneylost)}";
                }
            }

            // Load state earlier if it is multiplayer and it isn't 2AM or later
            if (Game1.timeOfDay < 2600 
                && Game1.player.canMove == true 
                && Context.IsMultiplayer == true 
                && PlayerStateRestorer.statepassoutps.Value != null)
            {
                // Load state and fix stamina
                PlayerStateRestorer.LoadStatePassout();
                Game1.player.stamina = (int)(Game1.player.maxStamina * this.config.PassOutPenalty.EnergytoRestorePercentage);

                // Reset state
                PlayerStateRestorer.statepassoutps.Value = null;

                // Set shouldtogglepassoutdata to false, this prevents DidPlayerPassOutYesterday from becoming false when player goes to bed
                togglesperscreen.Value.shouldtogglepassoutdata = false;
            }

            // If player can stay up past 2am, discard saved values and reset changed properties in data model
            if (Game1.timeOfDay == 2610 && PlayerStateRestorer.statepassoutps.Value != null)
            {
                Game1.player.modData[$"{this.ModManifest.UniqueID}.DidPlayerPassOutYesterday"] = "false";
                Game1.player.modData[$"{this.ModManifest.UniqueID}.MoneyLostLastPassOut"] = "0";                
                PlayerStateRestorer.statepassoutps.Value = null;
            }
        }

        /// <summary>Raised before the game ends the current day.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void DayEnding(object sender, DayEndingEventArgs e)
        {
            // Has the pass out state been saved after passing out?
            if (PlayerStateRestorer.statepassoutps.Value != null)
            {
                //Yes, reload the state

                // Restore playerstate using PassOutPenalty values
                PlayerStateRestorer.LoadStatePassout();

                // Reset PlayerStateRestorer class with the statepassout field
                PlayerStateRestorer.statepassoutps.Value = null;
            }

            // Is the day ending because player died?
            else if (PlayerStateRestorer.statedeathps.Value != null)
            {
                //Yes, reload the state

                // Restore playerstate using DeathPenalty values
                PlayerStateRestorer.LoadStateDeath();

                // Reset PlayerStateRestorer class with the statedeath field
                PlayerStateRestorer.statedeathps.Value = null;
            }
        }

        /// <summary>Raised before the game begins writing data to the save file</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void Saving(object sender, SavingEventArgs e)
        {
            // Has player not passed out but DidPlayerPassOutYesterday property is true?
            if (Game1.player.modData[$"{this.ModManifest.UniqueID}.DidPlayerPassOutYesterday"] == "true" 
                && (Game1.player.isInBed.Value == true 
                || Game1.player.modData[$"{this.ModManifest.UniqueID}.DidPlayerWakeupinClinic"] == "true") 
                && togglesperscreen.Value.shouldtogglepassoutdata == true)
            {
                // Yes, fix this so the new day will load correctly

                // Change DidPlayerPassOutYesterday field to false
                Game1.player.modData[$"{this.ModManifest.UniqueID}.DidPlayerPassOutYesterday"] = "false";
            }

            // Is DidPlayerWakeupinClinic true?
            if (Game1.player.modData[$"{this.ModManifest.UniqueID}.DidPlayerWakeupinClinic"] == "true")
            {               
                //Is player in bed or has player passed out? (player has not died)
                if (Game1.player.isInBed.Value == true || Game1.player.modData[$"{this.ModManifest.UniqueID}.DidPlayerPassOutYesterday"] == "true")
                {
                    // Yes, fix this so the new day will load correctly

                    // Change field to false
                    Game1.player.modData[$"{this.ModManifest.UniqueID}.DidPlayerWakeupinClinic"] = "false";
                }
            }
          
            // Set shouldtogglepassoutdata if needed so DidPlayerPassOutYesterday will toggle normally again
            togglesperscreen.Value.shouldtogglepassoutdata = true;
        }

        /// <summary>Raised after the game begins a new day</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void DayStarted(object sender, DayStartedEventArgs e)
        {

            if (!Game1.player.modData.ContainsKey($"{this.ModManifest.UniqueID}.DidPlayerPassOutYesterday"))
            {
                Game1.player.modData.Add($"{this.ModManifest.UniqueID}.DidPlayerPassOutYesterday", "false");
            }

            if (!Game1.player.modData.ContainsKey($"{this.ModManifest.UniqueID}.MoneyLostLastPassOut"))
            {
                Game1.player.modData.Add($"{this.ModManifest.UniqueID}.MoneyLostLastPassOut", "0");
            }

            if (!Game1.player.modData.ContainsKey($"{this.ModManifest.UniqueID}.DidPlayerWakeupinClinic"))
            {
                Game1.player.modData.Add($"{this.ModManifest.UniqueID}.DidPlayerWakeupinClinic", "false");
            }

            // Did player pass out yesterday?
            if (Game1.player.modData[$"{this.ModManifest.UniqueID}.DidPlayerPassOutYesterday"] == "true")
            {
                // Yes, fix player state

                // Change stamina to the amount restored by the config values
                Game1.player.stamina = (int)(Game1.player.maxStamina * this.config.PassOutPenalty.EnergytoRestorePercentage);

                // Invalidate cached mail data, this allows it to reload with correct values
                Helper.Content.InvalidateCache("Data\\mail");
            }

            // Did player wake up in clinic?
            if(Game1.player.modData[$"{this.ModManifest.UniqueID}.DidPlayerWakeupinClinic"] == "true")
            {
                // Yes, fix player state

                if(Game1.currentLocation.NameOrUniqueName != "Hospital" || Context.IsMultiplayer == true)
                {
                    // Warp player to clinic
                    Game1.warpFarmer("Hospital", 20, 12, false);
                }

                // Change health and stamina to the amount restored by the config values
                Game1.player.stamina = (int)(Game1.player.maxStamina * this.config.DeathPenalty.EnergytoRestorePercentage);
                Game1.player.health = Math.Max((int)(Game1.player.maxHealth * this.config.DeathPenalty.HealthtoRestorePercentage), 1);
            }
        }

        /// <summary>
        /// Raised after a mod message is received over the network.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void MessageReceived(object sender, ModMessageReceivedEventArgs e)
        {
            // Has a message been received from CustomDeathPenaltyPlus of the type IDied?
            if(e.FromModID == this.ModManifest.UniqueID && e.Type == "IDied")
            {
                // Read data from message into a new class instance of Multiplayer
                Multiplayer multiplayer = e.ReadAs<Multiplayer>();
                // Display a new HUD message to say that the dead player needs a new day to be started
                Game1.addHUDMessage(new HUDMessage($"{multiplayer.PlayerWhoDied} will need the rest of the day to recover.", null));
            }
        }

        // Define console command
        private void Info(string command, string[] args)
        {
            try
            {
                this.Monitor.Log($"Current config settings:" +
                        $"\n\nDeathPenalty" +
                        $"\n\nRestoreItems: {config.DeathPenalty.RestoreItems.ToString().ToLower()}" +
                        $"\nMoneyLossCap: {config.DeathPenalty.MoneyLossCap}" +
                        $"\nMoneytoRestorePercentage: {config.DeathPenalty.MoneytoRestorePercentage}" +
                        $"\nEnergytoRestorePercentage: {config.DeathPenalty.EnergytoRestorePercentage}" +
                        $"\nHealthtoRestorePercentage: {config.DeathPenalty.HealthtoRestorePercentage}" +
                        $"\n\nPassOutPenalty" +
                        $"\n\nMoneyLossCap: {config.PassOutPenalty.MoneyLossCap}" +
                        $"\nMoneytoRestorePercentage: {config.PassOutPenalty.MoneytoRestorePercentage}" +
                        $"\nEnergytoRestorePercentage: {config.PassOutPenalty.EnergytoRestorePercentage}" +
                        $"\n\nOtherPenalties" +
                        $"\n\nWakeupNextDayinClinic: { config.OtherPenalties.WakeupNextDayinClinic.ToString().ToLower()}" +
                        $"\nHarveyFriendshipChange: {config.OtherPenalties.HarveyFriendshipChange}" +
                        $"\nMaruFriendshipChange: {config.OtherPenalties.MaruFriendshipChange}" +
                        $"\nMoreRealisticWarps: {config.OtherPenalties.MoreRealisticWarps.ToString().ToLower()}" +
                        $"\nDebuffonDeath: {config.OtherPenalties.DebuffonDeath.ToString().ToLower()}",
                        LogLevel.Info);
            }
            catch (IndexOutOfRangeException)
            {
                this.Monitor.Log("Incorrect command format used.\nRequired format: configinfo", LogLevel.Error);
            }
        }
    }
}
