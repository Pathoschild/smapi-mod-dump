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

        // Advanced api
        void RegisterToken(IManifest mod, string name, object token);
    }

    public class Update
    {
        public bool updatedeath = false;

        public bool updatepassout = false;
    }

    public class ModEntry 
        : Mod
    {
        internal ModConfig config;
        internal static LocationTokens LocationTokens { get; private set; } = new LocationTokens();
        internal static DeathAndExhaustionTokens DeathAndExhaustionTokens { get; private set; } = new DeathAndExhaustionTokens();
        internal static QuestData QuestData { get; private set; } = new QuestData();

        public static readonly PerScreen<PlayerData> perScreenPlayerData = new PerScreen<PlayerData>(createNewState: () => new PlayerData());

        private static readonly string[] tokens = { "DeathCount", "DeathCountMarried", "PassOutCount", "QuestsCompleted", "TotalQuestsCompleted", "DeepestVolcanoFloor" };

        public static Update update = new Update();
        public static int deathcounter;
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
            QuestData.Hook(harmony, this.Monitor, this.Helper);
        }

        /// <summary>Raised after the game is launched, right before the first update tick.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // Access Content Patcher API
            var api = this.Helper.ModRegistry.GetApi<IContentPatcherAPI>("Pathoschild.ContentPatcher");

            if (api != null)
            {
                // Register "MineLevel" token
                api.RegisterToken(
                    this.ModManifest,
                    "MineLevel",
                    () =>
                    {
                        if (Context.IsWorldReady)
                        {
                            var currentMineLevel = ModEntry.perScreenPlayerData.Value.CurrentMineLevel;

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
                            var deepestMineLevel = ModEntry.perScreenPlayerData.Value.DeepestMineLevel;

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
                            var currentVolcanoFloor = ModEntry.perScreenPlayerData.Value.CurrentVolcanoFloor;

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
                            var AnniversaryDay = ModEntry.perScreenPlayerData.Value.AnniversaryDay;

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
                            var AnniversarySeason = ModEntry.perScreenPlayerData.Value.AnniversarySeason;

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
                           var currentYearsMarried = ModEntry.perScreenPlayerData.Value.CurrentYearsMarried;

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
                           var currentdeathcount = Game1.player.stats.TimesUnconscious;

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
                           var currentdeathcountmarried = Game1.player.isMarriedOrRoommates()
                           ? ModEntry.perScreenPlayerData.Value.DeathCountMarried
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
                           var currentdeathcount = Game1.player.stats.TimesUnconscious + 1;

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
                           var currentdeathcountmarried = Game1.player.isMarriedOrRoommates()
                           ? ModEntry.perScreenPlayerData.Value.DeathCountMarried + 1
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
                           var currentpassoutcount = ModEntry.perScreenPlayerData.Value.PassOutCount;

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
                           var currentquestsdone = Game1.player.stats.QuestsCompleted;

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
                            string[] questsdone = new string[ModEntry.perScreenPlayerData.Value.QuestsCompleted.Count];

                            // Set each value in new array to be the same as in QuestCompleted
                            foreach (var quest in ModEntry.perScreenPlayerData.Value.QuestsCompleted)
                            {
                                questsdone.SetValue(quest.ToString(), ModEntry.perScreenPlayerData.Value.QuestsCompleted.IndexOf(quest));
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
                       string[] ordersdone = new string[ModEntry.perScreenPlayerData.Value.SpecialOrdersCompleted.Count];

                       // Set each value in new array to be the same as in SpecialOrdersCompleted
                       foreach (var order in ModEntry.perScreenPlayerData.Value.SpecialOrdersCompleted)
                           {
                               ordersdone.SetValue(order, ModEntry.perScreenPlayerData.Value.SpecialOrdersCompleted.IndexOf(order));
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
                           var totalspecialorderscompleted = ModEntry.perScreenPlayerData.Value.SpecialOrdersCompleted.Count;

                           return new[]
                           {
                           totalspecialorderscompleted.ToString()
                           };

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
                           var totalspecialorderscompleted = ModEntry.perScreenPlayerData.Value.SpecialOrdersCompleted.Count;

                           return new[]
                           {
                           totalspecialorderscompleted.ToString()
                           };

                       }

                       return null;
                   });

                // Register "DeepestNormalMineLevel" token
                api.RegisterToken(
                    this.ModManifest,
                    "DeepestNormalMineLevel",
                    () =>
                    {
                        if (Context.IsWorldReady)
                        {
                            var deepestStandardMineLevel = ModEntry.perScreenPlayerData.Value.DeepestMineLevel < 121 ? ModEntry.perScreenPlayerData.Value.DeepestMineLevel : 120;

                            return new[]
                            {
                            deepestStandardMineLevel.ToString()
                            };
                        }

                        return null;
                    });

                // Register "DeepestSkullCavernMineLevel" token
                api.RegisterToken(
                    this.ModManifest,
                    "DeepestSkullCavernMineLevel",
                    () =>
                    {
                        if (Context.IsWorldReady)
                        {
                            var deepestSkullCavernMineLevel = ModEntry.perScreenPlayerData.Value.DeepestMineLevel > 120 ? ModEntry.perScreenPlayerData.Value.DeepestMineLevel - 120 : 0;

                            return new[]
                            {
                            deepestSkullCavernMineLevel.ToString()
                            };
                        }

                        return null;
                    });

                // Register "DeepestVolcanoFloor" token
                api.RegisterToken(
                    this.ModManifest,
                    "DeepestVolcanoFloor",
                    () =>
                    {
                        if (Context.IsWorldReady)
                        {
                            var deepestVolcanoFloor = ModEntry.perScreenPlayerData.Value.DeepestVolcanoFloor;

                            return new[]
                            {
                            deepestVolcanoFloor.ToString()
                            };
                        }

                        return null;
                    });

                // Register "Child" token
                api.RegisterToken(this.ModManifest, "Child", new ChildTokens());
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
                if (questid != "" && ModEntry.perScreenPlayerData.Value.QuestsCompleted.Contains(questid) == false)
                {
                    ModEntry.perScreenPlayerData.Value.QuestsCompleted.Add(questid);
                }
            }

            ModEntry.perScreenPlayerData.Value.DeepestMineLevel = Game1.player.deepestMineLevel;
            ModEntry.perScreenPlayerData.Value.DeathCountMarried = Game1.player.modData[$"{this.ModManifest.UniqueID}.DeathCountMarried"] != "" 
                ? int.Parse(Game1.player.modData[$"{this.ModManifest.UniqueID}.DeathCountMarried"]) 
                : 0;
            ModEntry.perScreenPlayerData.Value.PassOutCount = Game1.player.modData[$"{this.ModManifest.UniqueID}.PassOutCount"] != "" 
                ? int.Parse(Game1.player.modData[$"{this.ModManifest.UniqueID}.PassOutCount"])
                : 0;
            ModEntry.perScreenPlayerData.Value.DeathCountMarried = Game1.player.modData[$"{this.ModManifest.UniqueID}.DeathCount"] != ""
               ? int.Parse(Game1.player.modData[$"{this.ModManifest.UniqueID}.DeathCount"])
               : 0;
            ModEntry.perScreenPlayerData.Value.PassOutCount = Game1.player.modData[$"{this.ModManifest.UniqueID}.TotalQuestsCompleted"] != ""
                ? int.Parse(Game1.player.modData[$"{this.ModManifest.UniqueID}.TotalQuestsCompleted"])
                : 0;
            ModEntry.perScreenPlayerData.Value.DeepestVolcanoFloor = Game1.player.modData[$"{this.ModManifest.UniqueID}.DeepestVolcanoFloor"] != ""
                ? int.Parse(Game1.player.modData[$"{this.ModManifest.UniqueID}.DeepestVolcanoFloor"])
                : 0;

            // Reset booleans for new day
            update.updatepassout = true;
            update.updatedeath = true;
            this.Monitor.Log($"Trackers set to update");
           
            // Get days married
            int DaysMarried = Game1.player.GetDaysMarried();
            float Years = DaysMarried / 112;
            // Get years married
            double YearsMarried = Math.Floor(Years);
            // Get Anniversary date
            var anniversary = SDate.Now().AddDays(-DaysMarried);

            // Set tokens for the start of the day
            ModEntry.perScreenPlayerData.Value.CurrentYearsMarried = Game1.player.isMarriedOrRoommates() == true ? YearsMarried : 0;

            ModEntry.perScreenPlayerData.Value.AnniversarySeason = Game1.player.isMarriedOrRoommates() == true ? anniversary.Season.ToString() : "No season";

            ModEntry.perScreenPlayerData.Value.AnniversaryDay = Game1.player.isMarriedOrRoommates() == true ? anniversary.Day : 0;

            // Test if player is married
            if (Game1.player.isMarriedOrRoommates() is false)
            {
                // No, relevant trackers will use their default values

                this.Monitor.Log($"{Game1.player.Name} is not married");

                if (config.ResetDeathCountMarriedWhenDivorced == true && ModEntry.perScreenPlayerData.Value.DeathCountMarried != 0)
                {
                    // Reset tracker if player is no longer married
                    ModEntry.perScreenPlayerData.Value.DeathCountMarried = 0;
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
            LocationTokens.UpdateLocationTokens(this.Monitor, ModEntry.perScreenPlayerData);
        }

        /// <summary>Raised after the game state is updated (≈60 times per second).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void UpdateTicked(object sender, UpdateTickedEventArgs e)
        {    
            // Update death or pass out tokens if needed
            DeathAndExhaustionTokens.UpdateDeathAndExhaustionTokens(this.Monitor, ModEntry.perScreenPlayerData, this.config, update);
            // Check if any special orders have been completed
            QuestData.CheckForCompletedSpecialOrders(ModEntry.perScreenPlayerData, this.Monitor);

        }

        /// <summary>Raised before/after the game writes data to save file (except the initial save creation). 
        /// This is also raised for farmhands in multiplayer.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void Saving(object sender, SavingEventArgs e)
        {
            string[] QuestsComplete = Game1.player.modData[$"{this.ModManifest.UniqueID}.QuestsCompleted"].Split('/');

            // Remove quests already in mod data so they aren't added again
            foreach (string questid in QuestsComplete)
            {
                if(questid != "")
                {
                    ModEntry.perScreenPlayerData.Value.QuestsCompleted.Remove(questid);
                }
            }

            // Add any newly completed quests to mod data
            foreach (var questid in ModEntry.perScreenPlayerData.Value.QuestsCompleted)
            {
                Game1.player.modData[$"{this.ModManifest.UniqueID}.QuestsCompleted"] = Game1.player.modData[$"{this.ModManifest.UniqueID}.QuestsCompleted"] + $"{questid}/";
            }

            // Clear quest data for new day
            ModEntry.perScreenPlayerData.Value.QuestsCompleted.Clear();

            // Update old tracker
            Game1.player.modData[$"{this.ModManifest.UniqueID}.DeathCountMarried"] = ModEntry.perScreenPlayerData.Value.DeathCountMarried.ToString();
            Game1.player.modData[$"{this.ModManifest.UniqueID}.PassOutCount"] = ModEntry.perScreenPlayerData.Value.PassOutCount.ToString();
            Game1.player.modData[$"{this.ModManifest.UniqueID}.DeepestVolcanoFloor"] = ModEntry.perScreenPlayerData.Value.DeepestVolcanoFloor.ToString();
            this.Monitor.Log("Trackers updated for new day");
        }

        /// <summary>
        /// Raised after the game returns to the title screen.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void Title(object sender, ReturnedToTitleEventArgs e)
        {
            if (ModEntry.perScreenPlayerData.Value.SpecialOrdersCompleted.Count != 0)
            {
                ModEntry.perScreenPlayerData.Value.SpecialOrdersCompleted.Clear();
                this.Monitor.Log("Clearing Special Order data, ready for new save");
            }

            if (ModEntry.perScreenPlayerData.Value.QuestsCompleted.Count != 0)
            {
                ModEntry.perScreenPlayerData.Value.QuestsCompleted.Clear();               
                this.Monitor.Log("Clearing Quest data, ready for new save");
            }

            if (ModEntry.perScreenPlayerData.Value.QuestsCompleted.Count != 0)
            {
                ModEntry.perScreenPlayerData.Value.QuestsCompleted.Clear();
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
                this.Monitor.Log($"\n\nMineLevel: {ModEntry.perScreenPlayerData.Value.CurrentMineLevel}" +
                    $"\nVolcanoFloor: {ModEntry.perScreenPlayerData.Value.CurrentVolcanoFloor}" +
                    $"\nDeepestNormalMineLevel: { (ModEntry.perScreenPlayerData.Value.DeepestMineLevel < 121 ? ModEntry.perScreenPlayerData.Value.DeepestMineLevel : 120)}" +
                    $"\nDeepestSkullCavernMineLevel: {(ModEntry.perScreenPlayerData.Value.DeepestMineLevel > 120 ? ModEntry.perScreenPlayerData.Value.DeepestMineLevel - 120 : 0)}" +
                    $"\nDeepestVolcanoFloor: {ModEntry.perScreenPlayerData.Value.DeepestVolcanoFloor}" +
                    $"\nDeepestMineLevel: {ModEntry.perScreenPlayerData.Value.DeepestMineLevel}" +
                    $"\nYearsMarried: {ModEntry.perScreenPlayerData.Value.CurrentYearsMarried}" +
                    $"\nAnniversaryDay: {ModEntry.perScreenPlayerData.Value.AnniversaryDay}" +
                    $"\nAnniversarySeason: {ModEntry.perScreenPlayerData.Value.AnniversarySeason}" +
                    $"\nQuestIDsCompleted: {Quests(ModEntry.perScreenPlayerData.Value.QuestsCompleted)}" +
                    $"\nSOIDsCompleted: {Quests(ModEntry.perScreenPlayerData.Value.SpecialOrdersCompleted)}" +
                    $"\nSOCompleted: {ModEntry.perScreenPlayerData.Value.SpecialOrdersCompleted.Count}" +
                    $"\nQuestsCompleted: {Game1.player.stats.QuestsCompleted}" +
                    $"\nDeathCount: {Game1.player.stats.TimesUnconscious}" +
                    $"\nDeathCountMarried: {ModEntry.perScreenPlayerData.Value.DeathCountMarried}" +
                    $"\nDeathCountPK: {(Game1.player.isMarriedOrRoommates() ? Game1.player.stats.TimesUnconscious + 1 : 0)}" +
                    $"\nDeathCountMarriedPK: {(Game1.player.isMarriedOrRoommates() ? ModEntry.perScreenPlayerData.Value.DeathCountMarried + 1 : 0)}" +
                    $"\nPassOutCount: {ModEntry.perScreenPlayerData.Value.PassOutCount}", LogLevel.Info);
            }
            catch (Exception ex)
            {
                // Throw an exception if command failed to execute
                throw new Exception("Command failed somehow", ex);
            }

        }
    }
}