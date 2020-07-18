using System;
using StardewModdingAPI;
using StardewValley;
using System.Collections.Generic;
using System.Linq;

namespace AngryGrandpa
{
    /// <summary>Class containing the mod's console commands.</summary>
    public class ConsoleCommands
    {
        /*********
        ** Accessors
        *********/
        protected static IModHelper Helper => ModEntry.Instance.Helper;
        protected static IMonitor Monitor => ModEntry.Instance.Monitor;
        protected static ModConfig Config => ModConfig.Instance;


        /*********
        ** Fields
        *********/
        protected static ITranslationHelper i18n = Helper.Translation;


        /*********
        ** Public methods
        *********/
        /// <summary>
        /// Use the Mod Helper to register the commands in this class.
        /// </summary>
        public static void Apply()
        {
            string NL = Environment.NewLine;

            Helper.ConsoleCommands.Add("reset_evaluation",
                i18n.Get("reset_evaluation.description") + NL + "Usage: reset_evaluation",
                cmdResetEvaluation);
            Helper.ConsoleCommands.Add("grandpa_config",
                i18n.Get("grandpa_config.description") + NL + "Usage: grandpa_config",
                cmdGrandpaConfig);
            Helper.ConsoleCommands.Add("grandpa_score",
                i18n.Get("grandpa_score.description") + NL + "Usage: grandpa_score",
                cmdGrandpaScore);
            Helper.ConsoleCommands.Add("grandpa_debug",
                i18n.Get("grandpa_debug.description") + NL + "Usage: grandpa_debug",
                cmdGrandpaDebug);
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Gives a farm evaluation in console output when the 'grandpa_score' command is invoked.</summary>
        /// <param name="command">The name of the command invoked.</param>
        /// <param name="args">The arguments received by the command. Each word after the command name is a separate argument.</param>
        private static void cmdGrandpaScore(string _command, string[] _args)
        {
            try
            {
                if (!Context.IsWorldReady)
                {
                    throw new Exception("An active save is required.");
                }
                int grandpaScore = Utility.getGrandpaScore();
                int maxScore = Config.GetMaxScore();
                int candles = Utility.getGrandpaCandlesFromScore(grandpaScore);
                Monitor.Log($"SCORE ESTIMATE\n" +
                    $"    Grandpa's Score: {grandpaScore} of {maxScore} Great Honors\n" +
                    $"    Number of candles earned: {candles}\n" +
                    $"    Scoring system: \"{Config.ScoringSystem}\"\n" +
                    $"    Candle score thresholds: [{Config.GetScoreForCandles(1)}, {Config.GetScoreForCandles(2)}, {Config.GetScoreForCandles(3)}, {Config.GetScoreForCandles(4)}]",
                    LogLevel.Info);
            }
            catch (Exception ex)
            {
                Monitor.Log($"Command grandpa_score failed:\n{ex}", LogLevel.Warn);
            }
        }

        /// <summary>Resets all event flags related to grandpa's evaluation(s) when the 'reset_evaluation' command is invoked.</summary>
        /// <param name="command">The name of the command invoked.</param>
        /// <param name="args">The arguments received by the command. Each word after the command name is a separate argument.</param>
        private static void cmdResetEvaluation(string _command, string[] _args)
        {
            try
            {
                if (!Context.IsWorldReady)
                {
                    throw new Exception("An active save is required.");
                }
                var eventsToRemove = new List<int>
                {
                    558291, 558292, 321777 // Initial eval, Re-eval, and Evaluation request
                };
                foreach (int e in eventsToRemove)
                {
                    while (Game1.player.eventsSeen.Contains(e)) { Game1.player.eventsSeen.Remove(e); }
                }
                // Game1.player.eventsSeen.Remove(2146991); // Candles (removed instead by command_grandpaEvaluation postfix)
                Game1.getFarm().hasSeenGrandpaNote = false; // Seen the note on the shrine
                while (Game1.player.mailReceived.Contains("grandpaPerfect")) // Received the statue of perfection
                { 
                    Game1.player.mailReceived.Remove("grandpaPerfect"); 
                } 
                Game1.getFarm().grandpaScore.Value = 0; // Reset grandpaScore
                FarmPatches.RemoveCandlesticks(Game1.getFarm()); // Removes all candlesticks (not flames).
                Game1.getFarm().removeTemporarySpritesWithIDLocal(6666f); // Removes candle flames.

                // Remove flags added by this mod
                var flagsToRemove = new List<string> 
                {
                    "6324bonusRewardsEnabled", "6324reward2candles", "6324reward3candles", // Old, outdated flags
                    "6324grandpaNoteMail", "6324reward1candle", "6324reward2candle", "6324reward3candle", "6324reward4candle", "6324hasDoneModdedEvaluation", // Current used flags
                };
                foreach (string flag in flagsToRemove)
                {
                    while (Game1.player.mailReceived.Contains(flag)) { Game1.player.mailReceived.Remove(flag); }
                }

                if (!Game1.player.eventsSeen.Contains(2146991))
                {
                    Game1.player.eventsSeen.Add(2146991); // Make sure they can't see candle event before the next evaluation.
                }

                Monitor.Log($"Reset grandpaScore and associated event and mail flags for all evaluations.", LogLevel.Info);
            }
            catch (Exception ex)
            {
                Monitor.Log($"Command reset_evaluation failed:\n{ex}", LogLevel.Warn);
            }
        }
        
        /// <summary>Prints the active Angry Grandpa config settings to the console.</summary>
        /// <param name="command">The name of the command invoked.</param>
        /// <param name="args">The arguments received by the command. Each word after the command name is a separate argument.</param>
        private static void cmdGrandpaConfig(string _command, string[] _args)
        {
            try
            {
                ModConfig.Print(); // Print config values to console
            }
            catch (Exception ex)
            {
                Monitor.Log($"Command grandpa_config failed:\n{ex}",
                    LogLevel.Error);
            }
        }

        /// <summary>Prints config and score data with some extra debugging info.</summary>
        /// <param name="command">The name of the command invoked.</param>
        /// <param name="args">The arguments received by the command. Each word after the command name is a separate argument.</param>
        private static void cmdGrandpaDebug(string _command, string[] _args)
        {
            cmdGrandpaConfig("grandpa_config", null);
            cmdGrandpaScore("grandpa_score", null);

            try
            {
                if (!Context.IsWorldReady)
                {
                    throw new Exception("An active save is required.");
                }

                List<int> eventsAG = new List<int> { 558291, 558292, 2146991, 321777 };
                List<string> mailAG = new List<string> { "6324grandpaNoteMail", "6324reward1candle", "6324reward2candle", "6324reward3candle", "6324reward4candle", "6324bonusRewardsEnabled", "6324hasDoneModdedEvaluation" };

                Monitor.Log($"DEBUG\n" +
                    $"    Actual current Farm.grandpaScore value: {Game1.getFarm().grandpaScore.Value}\n" +
                    $"    Actual current Farm.hasSeenGrandpaNote value: {Game1.getFarm().hasSeenGrandpaNote}\n" +
                    $"    List of eventsSeen entries: {string.Join(", ", eventsAG.Where(Game1.player.eventsSeen.Contains).ToList())}\n" +
                    $"    List of mailReceived entries: {string.Join(", ", mailAG.Where(Game1.player.mailReceived.Contains).ToList())}", LogLevel.Debug);
            }
            catch (Exception ex)
            {
                Monitor.Log($"Command grandpa_debug failed:\n{ex}",
                    LogLevel.Error);
            }
        }
    }
}