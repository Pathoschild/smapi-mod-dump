/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TheMightyAmondee/CustomTokens
**
*************************************************/

using System;
using System.Collections;
using System.Text;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using System.Collections.Generic;
using HarmonyLib;


namespace CustomTokens
{
    public interface IContentPatcherAPI
    {
        // Basic api
        void RegisterToken(IManifest mod, string name, Func<IEnumerable<string>> getValue);

        // Advanced api, not currently used
        void RegisterToken(IManifest mod, string name, object token);
    }

    public class ModEntry 
        : Mod
    {
        internal ModConfig config;
        internal static LocationTokens LocationTokens { get; private set; } = new LocationTokens();
        internal static DeathAndExhaustionTokens DeathAndExhaustionTokens { get; private set; } = new DeathAndExhaustionTokens();
        internal static QuestData QuestData { get; private set; } = new QuestData();

        public static readonly PerScreen<PlayerData> perScreen = new PerScreen<PlayerData>(createNewState: () => new PlayerData());

        private static readonly string[] tokens = { "DeathCountMarried", "PassOutCount", "QuestsCompleted" };

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            // Add required event helpers
            helper.Events.Player.Warped += this.LocationChange;
            helper.Events.GameLoop.GameLaunched += this.GameLaunched;
            helper.Events.GameLoop.UpdateTicked += this.UpdateTicked;
            helper.Events.GameLoop.Saving += this.Saving;
            helper.Events.GameLoop.ReturnedToTitle += this.Title;
            helper.Events.GameLoop.DayStarted += this.DayStarted;

            // Read the mod config for values and create one if one does not currently exist
            this.config = this.Helper.ReadConfig<ModConfig>();

            // Add debug command if AllowDebugging is true
            if (this.config.AllowDebugging == true)
            {
                helper.ConsoleCommands.Add("tracker", "Displays the current tracked values", this.DisplayInfo);
            }

            var harmony = new Harmony(this.ModManifest.UniqueID);
            QuestData.Hook(harmony, this.Monitor);
        }

        /// <summary>Raised after the game is launched, right before the first update tick.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // Access Content Patcher API
            var api = this.Helper.ModRegistry.GetApi<IContentPatcherAPI>("Pathoschild.ContentPatcher");

            if(api != null)
            {
                // Register "MineLevel" token
                api.RegisterToken(
                    this.ModManifest,
                    "MineLevel",
                    () =>
                    {
                        if (Context.IsWorldReady)
                        {
                            var currentMineLevel = ModEntry.perScreen.Value.CurrentMineLevel;

                            return new[]
                            {
                            currentMineLevel.ToString()
                            };
                        }

                        return null;
                    });

                // Register "DeepestMineLevel" token
                api.RegisterToken(
                    this.ModManifest,
                    "DeepestMineLevel",
                    () =>
                    {
                        if (Context.IsWorldReady)
                        {
                            var deepestMineLevel = ModEntry.perScreen.Value.DeepestMineLevel;

                            return new[]
                            {
                            deepestMineLevel.ToString()
                            };
                        }

                        return null;
                    });

                // Register "VolcanoFloor" token
                api.RegisterToken(
                    this.ModManifest,
                    "VolcanoFloor",
                    () =>
                    {
                        if (Context.IsWorldReady)
                        {
                            var currentVolcanoFloor = ModEntry.perScreen.Value.CurrentVolcanoFloor;

                            return new[]
                            {
                            currentVolcanoFloor.ToString()
                            };
                        }

                        return null;
                    });

                // Register "AnniversaryDay" token
                api.RegisterToken(
                    this.ModManifest,
                    "AnniversaryDay",
                    () =>
                    {
                        if (Context.IsWorldReady)
                        {
                            var AnniversaryDay = ModEntry.perScreen.Value.AnniversaryDay;

                            return new[]
                            {
                            AnniversaryDay.ToString()
                            };
                        }

                        return null;
                    });

                // Register "AnniversarySeason" token
                api.RegisterToken(
                    this.ModManifest,
                    "AnniversarySeason",
                    () =>
                    {
                        if (Context.IsWorldReady)
                        {
                            var AnniversarySeason = ModEntry.perScreen.Value.AnniversarySeason;

                            return new[]
                            {
                            AnniversarySeason
                            };
                        }

                        return null;
                    });

                // Register "YearsMarried" token
                api.RegisterToken(
                   this.ModManifest,
                   "YearsMarried",
                   () =>
                   {
                       if (Context.IsWorldReady)
                       {
                           var currentYearsMarried = ModEntry.perScreen.Value.CurrentYearsMarried;

                           return new[]
                           {
                            currentYearsMarried.ToString()
                           };
                       }

                       return null;
                   });

                // Register "DeathCount" token
                api.RegisterToken(
                   this.ModManifest,
                   "DeathCount",
                   () =>
                   {
                       if (Context.IsWorldReady)
                       {
                           var currentdeathcount = (int)Game1.stats.timesUnconscious;

                           return new[]
                           {
                            currentdeathcount.ToString()
                           };
                       }

                       return null;
                   });

                // Register "DeathCountMarried" token
                api.RegisterToken(
                   this.ModManifest,
                   "DeathCountMarried",
                   () =>
                   {
                       if (Context.IsWorldReady)
                       {
                           var currentdeathcountmarried = Game1.player.isMarried()
                           ? ModEntry.perScreen.Value.DeathCountMarried
                           : 0;

                           return new[]
                           {
                            currentdeathcountmarried.ToString()
                           };

                       }

                       return null;
                   });

                // Register "DeathCountPK" token
                api.RegisterToken(
                   this.ModManifest,
                   "DeathCountPK",
                   () =>
                   {
                   /* 
                   CP won't use correct value of DeathCountMarried token during the PlayerKilled event as the token is updated outside of the useable update rates, 
                   Adding 1 to the value of that token ensures token value is correct when content is loaded for event
                   */

                       if (Context.IsWorldReady)
                       {
                           var currentdeathcount = (int)Game1.stats.timesUnconscious + 1;

                           return new[]
                           {
                            currentdeathcount.ToString()
                           };
                       }

                       return null;
                   });

                // Register "DeathCountMarriedPK" token
                api.RegisterToken(
                   this.ModManifest,
                   "DeathCountMarriedPK",
                   () =>
                   {
                       if (Context.IsWorldReady)
                       {
                       /* 
                       CP won't use correct value of DeathCountMarried token during the PlayerKilled event as the token is updated outside of the useable update rates, 
                       Adding 1 to the value of that token ensures token value is correct when content is loaded for event
                       */
                           var currentdeathcountmarried = Game1.player.isMarried()
                           ? ModEntry.perScreen.Value.DeathCountMarried + 1
                           : 0;

                           return new[]
                           {
                            currentdeathcountmarried.ToString()
                           };

                       }

                       return null;
                   });

                // Register "PassOutCount" token
                api.RegisterToken(
                   this.ModManifest,
                   "PassOutCount",
                   () =>
                   {
                       if (Context.IsWorldReady)
                       {
                           var currentpassoutcount = ModEntry.perScreen.Value.PassOutCount;

                           return new[]
                           {
                            currentpassoutcount.ToString()
                           };

                       }

                       return null;
                   });

                // Register "TotalQuestsCompleted" token
                api.RegisterToken(
                   this.ModManifest,
                   "QuestsCompleted",
                   () =>
                   {
                       if (Context.IsWorldReady)
                       {
                           var currentquestsdone = Game1.stats.questsCompleted;

                           return new[]
                           {
                            currentquestsdone.ToString()
                           };

                       }

                       return null;
                   });

                // Register "QuestIDsCompleted" token
                api.RegisterToken(
                   this.ModManifest,
                   "QuestIDsCompleted",
                   () =>
                   {
                       if (Context.IsWorldReady)
                       {                      
                            // Create array with the length of the QuestsCompleted array list
                            string[] questsdone = new string[ModEntry.perScreen.Value.QuestsCompleted.Count];

                            // Set each value in new array to be the same as in QuestCompleted
                            foreach (var quest in ModEntry.perScreen.Value.QuestsCompleted)
                            {
                                questsdone.SetValue(quest.ToString(), ModEntry.perScreen.Value.QuestsCompleted.IndexOf(quest));
                            }

                           return questsdone;
                       }

                       return null;
                   });

                // Register "SOIDsCompleted" token
                api.RegisterToken(
                   this.ModManifest,
                   "SOIDsCompleted",
                   () =>
                   {
                       if (Context.IsWorldReady)
                       {

                       // Create array with the length of the SpecialOrdersCompleted array list
                       string[] ordersdone = new string[ModEntry.perScreen.Value.SpecialOrdersCompleted.Count];

                       // Set each value in new array to be the same as in SpecialOrdersCompleted
                       foreach (var order in ModEntry.perScreen.Value.SpecialOrdersCompleted)
                           {
                               ordersdone.SetValue(order, ModEntry.perScreen.Value.SpecialOrdersCompleted.IndexOf(order));
                           }

                           return ordersdone;

                       }

                       return null;
                   });

                // Register "SOCompleted" token
                api.RegisterToken(
                   this.ModManifest,
                   "SOCompleted",
                   () =>
                   {
                       if (Context.IsWorldReady)
                       {
                           var totalspecialorderscompleted = ModEntry.perScreen.Value.SpecialOrdersCompleted.Count;

                           return new[]
                           {
                           totalspecialorderscompleted.ToString()
                           };

                       }

                       return null;
                   });
            }
            else
            {
                this.Monitor.Log("Content Patcher API not found, I'm not going to do anything useful", LogLevel.Warn);
            }
          
        }

        /// <summary>The method called after a new day starts.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void DayStarted(object sender, DayStartedEventArgs e)
        {
            // Add mod data to save file if needed
            foreach(var token in tokens)
            {
                if (!Game1.player.modData.ContainsKey($"{this.ModManifest.UniqueID}.{token}"))
                {
                    Game1.player.modData.Add($"{this.ModManifest.UniqueID}.{token}", "");
                    this.Monitor.Log($"Added save data {this.ModManifest.UniqueID}.{token}");
                }
            }
           
            string[] QuestsComplete = Game1.player.modData[$"{this.ModManifest.UniqueID}.QuestsCompleted"].Split('/');

            // Add recorded completed quests from mod data in save file to player data
            foreach(string questid in QuestsComplete)
            {
                if (questid != "" && ModEntry.perScreen.Value.QuestsCompleted.Contains(int.Parse(questid)) == false)
                {
                    ModEntry.perScreen.Value.QuestsCompleted.Add(int.Parse(questid));
                }
            }

            ModEntry.perScreen.Value.DeepestMineLevel = Game1.player.deepestMineLevel;
            ModEntry.perScreen.Value.DeathCountMarried = Game1.player.modData[$"{this.ModManifest.UniqueID}.DeathCountMarried"] != "" 
                ? int.Parse(Game1.player.modData[$"{this.ModManifest.UniqueID}.DeathCountMarried"]) 
                : 0;
            ModEntry.perScreen.Value.PassOutCount = Game1.player.modData[$"{this.ModManifest.UniqueID}.PassOutCount"] != "" 
                ? int.Parse(Game1.player.modData[$"{this.ModManifest.UniqueID}.PassOutCount"])
                : 0;

            // Reset booleans for new day
            DeathAndExhaustionTokens.updatepassout = true;
            DeathAndExhaustionTokens.updatedeath = true;
            this.Monitor.Log($"Trackers set to update");
           
            // Get days married
            int DaysMarried = Game1.player.GetDaysMarried();
            float Years = DaysMarried / 112;
            // Get years married
            double YearsMarried = Math.Floor(Years);
            // Get Anniversary date
            var anniversary = SDate.Now().AddDays(-(DaysMarried - 1));

            // Set tokens for the start of the day
            ModEntry.perScreen.Value.CurrentYearsMarried = Game1.player.isMarried() == true ? YearsMarried : 0;

            ModEntry.perScreen.Value.AnniversarySeason = Game1.player.isMarried() == true ? anniversary.Season : "No season";

            ModEntry.perScreen.Value.AnniversaryDay = Game1.player.isMarried() == true ? anniversary.Day : 0;

            // Test if player is married
            if (Game1.player.isMarried() is false)
            {
                // No, relevant trackers will use their default values

                this.Monitor.Log($"{Game1.player.Name} is not married");

                if (config.ResetDeathCountMarriedWhenDivorced == true && ModEntry.perScreen.Value.DeathCountMarried != 0)
                {
                    // Reset tracker if player is no longer married
                    ModEntry.perScreen.Value.DeathCountMarried = 0;
                }
            }

            // Yes, tokens exist
            else
            {
                this.Monitor.Log($"{Game1.player.Name} has been married for {YearsMarried} year(s)");

                this.Monitor.Log($"Anniversary is the {anniversary.Day} of {anniversary.Season}");
            }
        }

        /// <summary>Raised after the current player moves to a new location.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void LocationChange(object sender, WarpedEventArgs e)
        {
            // Update location tokens if needed
            LocationTokens.UpdateLocationTokens(this.Monitor, ModEntry.perScreen);
        }

        /// <summary>Raised after the game state is updated (≈60 times per second).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void UpdateTicked(object sender, UpdateTickedEventArgs e)
        {    
            // Update death or pass out tokens if needed
            DeathAndExhaustionTokens.UpdateDeathAndExhaustionTokens(this.Helper, this.Monitor, ModEntry.perScreen, this.config);
            // Check if any special orders have been completed
            QuestData.CheckForCompletedSpecialOrders(ModEntry.perScreen, this.Monitor);

        }

        /// <summary>Raised before/after the game writes data to save file (except the initial save creation). 
        /// This is also raised for farmhands in multiplayer.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void Saving(object sender, SavingEventArgs e)
        {
            string[] QuestsComplete = Game1.player.modData[$"{this.ModManifest.UniqueID}.QuestsCompleted"].Split('/');

            // Remove quests already in mod data so they aren't added again
            foreach(string questid in QuestsComplete)
            {
                if(questid != "")
                {
                    ModEntry.perScreen.Value.QuestsCompleted.Remove(int.Parse(questid));
                }
            }

            // Add any newly completed quests to mod data
            foreach (int questid in ModEntry.perScreen.Value.QuestsCompleted)
            {
                Game1.player.modData[$"{this.ModManifest.UniqueID}.QuestsCompleted"] = Game1.player.modData[$"{this.ModManifest.UniqueID}.QuestsCompleted"] + $"{questid}/";
            }

            // Clear quest data for new day
            ModEntry.perScreen.Value.QuestsCompleted.Clear();

            // Update old tracker
            Game1.player.modData[$"{this.ModManifest.UniqueID}.DeathCountMarried"] = ModEntry.perScreen.Value.DeathCountMarried.ToString();
            Game1.player.modData[$"{this.ModManifest.UniqueID}.PassOutCount"] = ModEntry.perScreen.Value.PassOutCount.ToString();
            this.Monitor.Log("Trackers updated for new day");
        }

        /// <summary>
        /// Raised after the game returns to the title screen.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void Title(object sender, ReturnedToTitleEventArgs e)
        {
            if (ModEntry.perScreen.Value.SpecialOrdersCompleted.Count != 0)
            {
                ModEntry.perScreen.Value.SpecialOrdersCompleted.Clear();
                this.Monitor.Log("Clearing Special Order data, ready for new save");
            }

            if (ModEntry.perScreen.Value.QuestsCompleted.Count != 0)
            {
                ModEntry.perScreen.Value.QuestsCompleted.Clear();               
                this.Monitor.Log("Clearing Quest data, ready for new save");
            }
        }

        /// <summary>Raised when the tracker command is entered into the SMAPI console.</summary>
        /// <param name="command">The command name (tracker)</param>
        /// <param name="args">The command arguments</param>
        private void DisplayInfo(string command, string[] args)
        {

            string Quests(ArrayList collection)
            {
                StringBuilder questsasstring = new StringBuilder("None");

                // Remove default string if array isn't empty
                if (collection.Count > 0)
                {
                    questsasstring.Remove(0, 4);
                }

                // Add each quest id to string
                foreach (var quest in collection)
                {
                    questsasstring.Append($", {quest}");

                    // Remove whitespace and comma if id is the first in the array
                    if (collection.IndexOf(quest) == 0)
                    {
                        questsasstring.Remove(0, 2);
                    }
                }

                return questsasstring.ToString();
            }

            try
            {
                // Display information in SMAPI console
                this.Monitor.Log($"\n\nMineLevel: {ModEntry.perScreen.Value.CurrentMineLevel}" +
                    $"\nVolcanoFloor: {ModEntry.perScreen.Value.CurrentVolcanoFloor}" +
                    $"\nDeepestMineLevel: {ModEntry.perScreen.Value.DeepestMineLevel}" +
                    $"\nYearsMarried: {ModEntry.perScreen.Value.CurrentYearsMarried}" +
                    $"\nAnniversaryDay: {ModEntry.perScreen.Value.AnniversaryDay}" +
                    $"\nAnniversarySeason: {ModEntry.perScreen.Value.AnniversarySeason}" +
                    $"\nQuestIDsCompleted: {Quests(ModEntry.perScreen.Value.QuestsCompleted)}" +
                    $"\nSOIDsCompleted: {Quests(ModEntry.perScreen.Value.SpecialOrdersCompleted)}" +
                    $"\nSOCompleted: {ModEntry.perScreen.Value.SpecialOrdersCompleted.Count}" +
                    $"\nQuestsCompleted: {Game1.stats.questsCompleted}" +
                    $"\nDeathCount: {Game1.stats.timesUnconscious}" +
                    $"\nDeathCountMarried: {ModEntry.perScreen.Value.DeathCountMarried}" +
                    $"\nDeathCountPK: {(Game1.player.isMarried() ? Game1.stats.timesUnconscious + 1 : 0)}" +
                    $"\nDeathCountMarriedPK: {(Game1.player.isMarried() ? ModEntry.perScreen.Value.DeathCountMarried + 1 : 0)}" +
                    $"\nPassOutCount: {ModEntry.perScreen.Value.PassOutCount}", LogLevel.Info);
            }
            catch (Exception ex)
            {
                // Throw an exception if command failed to execute
                throw new Exception("Command failed somehow", ex);
            }

        }
    }
}