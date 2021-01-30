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
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using System.Collections.Generic;


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

        public static PlayerData PlayerData { get; private set; } = new PlayerData();
        public static PlayerDataToWrite PlayerDataToWrite { get; private set; } = new PlayerDataToWrite();
        internal static TrackerCommand TrackerCommand { get; private set; } = new TrackerCommand();
        internal static LocationTokens LocationTokens { get; private set; } = new LocationTokens();
        internal static DeathAndExhaustionTokens DeathAndExhaustionTokens { get; private set; } = new DeathAndExhaustionTokens();
        internal static MarriageTokens MarriageTokens { get; private set; } = new MarriageTokens();
        internal static QuestData QuestData { get; private set; } = new QuestData();

        private static readonly PerScreen<PlayerData> perScreen = new PerScreen<PlayerData>(createNewState: () => PlayerData);

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            // Add required event helpers
            helper.Events.Player.Warped += this.LocationChange;
            helper.Events.GameLoop.GameLaunched += this.GameLaunched;
            helper.Events.GameLoop.UpdateTicked += this.UpdateTicked;
            helper.Events.GameLoop.Saved += this.Saved;
            helper.Events.GameLoop.ReturnedToTitle += this.Title;
            helper.Events.GameLoop.DayStarted += this.DayStarted;

            // Read the mod config for values and create one if one does not currently exist
            this.config = this.Helper.ReadConfig<ModConfig>();

            // Add debug command if AllowDebugging is true
            if (this.config.AllowDebugging == true)
            {
                helper.ConsoleCommands.Add("tracker", "Displays the current tracked values", this.Tracker);
            }
        }

        /// <summary>Raised after the game is launched, right before the first update tick.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // Access Content Patcher API
            var api = this.Helper.ModRegistry.GetApi<IContentPatcherAPI>("Pathoschild.ContentPatcher");

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
                       ? PlayerDataToWrite.DeathCountMarried
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
                       ? PlayerDataToWrite.DeathCountMarried + 1
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
                       var currentpassoutcount = PlayerDataToWrite.PassOutCount;

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
                       /*
                       Some previously completed quests still need to be added manually (6, 16, 128 or 129 and 130)
                       */

                       // Create array with the length of the QuestsCompleted array list
                       string[] questsdone = new string[ModEntry.perScreen.Value.QuestsCompleted.Count];

                       // Set each value in new array to be the same as in QuestCompleted
                       foreach(var quest in ModEntry.perScreen.Value.QuestsCompleted)
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

        /// <summary>The method called after a new day starts.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void DayStarted(object sender, DayStartedEventArgs e)
        {
            // Read JSON file and create if needed
            PlayerDataToWrite = this.Helper.Data.ReadJsonFile<PlayerDataToWrite>($"data\\{Constants.SaveFolderName}.json") ?? new PlayerDataToWrite();

            ModEntry.perScreen.Value.DeepestMineLevel = Game1.player.deepestMineLevel;

            DeathAndExhaustionTokens.updatepassout = true;
            DeathAndExhaustionTokens.updatedeath = true;
            this.Monitor.Log($"Trackers set to update");

            MarriageTokens.UpdateMarriageTokens(this.Monitor, ModEntry.perScreen, ModEntry.PlayerDataToWrite, this.config);
           
            QuestData.AddCompletedQuests(ModEntry.perScreen, ModEntry.PlayerDataToWrite);
            this.Monitor.Log("Determining previously completed quests... As best as I can");

            // Save any data to JSON file
            this.Monitor.Log("Writing data to JSON file");
            this.Helper.Data.WriteJsonFile<PlayerDataToWrite>($"data\\{Constants.SaveFolderName}.json", ModEntry.PlayerDataToWrite);
        }

        /// <summary>Raised after the current player moves to a new location.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void LocationChange(object sender, WarpedEventArgs e)
        {
            LocationTokens.UpdateLoactionTokens(this.Monitor, ModEntry.perScreen);
        }

        /// <summary>Raised when the tracker command is entered into the SMAPI console.</summary>
        /// <param name="command">The command name (tracker)</param>
        /// <param name="args">The command arguments</param>
        private void Tracker(string command, string[] args)
        {
            TrackerCommand.DisplayInfo(this.Monitor, ModEntry.perScreen, ModEntry.PlayerDataToWrite);
        }

        /// <summary>Raised after the game state is updated (≈60 times per second).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void UpdateTicked(object sender, UpdateTickedEventArgs e)
        {          
            DeathAndExhaustionTokens.UpdateDeathAndExhaustionTokens(this.Helper, this.Monitor, ModEntry.PlayerDataToWrite, this.config);

            // Check for new added quests
            QuestData.UpdateQuestLog();
            // Check if any quests have been completed
            QuestData.CheckForCompletedQuests(ModEntry.perScreen, ModEntry.PlayerDataToWrite, this.Monitor);
            // Check if any special orders have been completed
            QuestData.CheckForCompletedSpecialOrders(ModEntry.perScreen, this.Monitor);

        }

        /// <summary>Raised before/after the game writes data to save file (except the initial save creation). 
        /// This is also raised for farmhands in multiplayer.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void Saved(object sender, SavedEventArgs e)
        {
            // Update old tracker
            PlayerDataToWrite.DeathCountMarriedOld = PlayerDataToWrite.DeathCountMarried;
            this.Monitor.Log("Old death tracker updated for new day");

            // Save any data to JSON file
            this.Monitor.Log("Writing data to JSON file");
            this.Helper.Data.WriteJsonFile<PlayerDataToWrite>($"data\\{Constants.SaveFolderName}.json", ModEntry.PlayerDataToWrite);
        }

        private void Title(object sender, ReturnedToTitleEventArgs e)
        {
            if (ModEntry.perScreen.Value.SpecialOrdersCompleted.Count != 0)
            {
                ModEntry.perScreen.Value.SpecialOrdersCompleted.Clear();
                this.Monitor.Log("Clearing Special Order data, ready for new save");
            }

            QuestData.QuestlogidsNew.Clear();
            QuestData.QuestlogidsOld.Clear();

            if (ModEntry.perScreen.Value.QuestsCompleted.Count != 0)
            {
                ModEntry.perScreen.Value.QuestsCompleted.Clear();               
                this.Monitor.Log("Clearing Quest data, ready for new save");
            }
        }
    }
}