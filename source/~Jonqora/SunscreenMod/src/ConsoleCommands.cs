using System;
using StardewModdingAPI;
using StardewValley;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using StardewModdingAPI.Utilities;

namespace SunscreenMod
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

            Helper.ConsoleCommands.Add("playSound",
                "Plays a soundbank sound for testing purposes." + NL + "Usage: playSound <soundName> [pitch]",
                cmdPlaySound);
            Helper.ConsoleCommands.Add("checkTime",
                "Print the current game time." + NL + "Usage: checkTime",
                checkTime);
            Helper.ConsoleCommands.Add("getUV",
                "Print the UV value (before index scaling) for a certain date." + NL + "Usage: getUV <daysSinceStart>",
                getUV);
            Helper.ConsoleCommands.Add("setSunburn",
                "Manually set sunburn level from 0-3." + NL + "Usage: setSunburn <level>",
                setSunburn);
            Helper.ConsoleCommands.Add("setNewBurn",
                "Manually set new burn damage level from 0-3." + NL + "Usage: setNewBurn <level>",
                setNewBurn);
            Helper.ConsoleCommands.Add("checkBurn",
                "Check sunburn and new burn damage levels." + NL + "Usage: checkBurn",
                checkBurn);
        }


        /*********
        ** Private methods
        *********/
        private static void checkTime(string _command, string[] _args)
        {
            try
            {
                Monitor.Log($"Current game time: {Game1.timeOfDay}", LogLevel.Info);
            }
            catch (Exception ex)
            {
                Monitor.Log($"Command checkTime failed:\n{ex}", LogLevel.Warn);
            }
        }
        private static void getUV(string _command, string[] _args)
        {
            try
            {
                int days = int.Parse(_args[0]);
                SDate date = SDate.FromDaysSinceStart(days);
                int dailyMaxUV = UVIndex.DailyMaxUV(days);
                int uvIndex = Convert.ToInt32((double)dailyMaxUV / 25);
                Monitor.Log($"Forecasted max UV value: {dailyMaxUV}\n" +
                    $"Forecasted UV Index: {uvIndex}", LogLevel.Info);
            }
            catch (Exception ex)
            {
                Monitor.Log($"Command getUV failed:\n{ex}", LogLevel.Warn);
            }
        }

        private static void setSunburn(string _command, string[] _args)
        {
            try
            {
                ModEntry.Instance.Burn.SunburnLevel = int.Parse(_args[0]);
            }
            catch (Exception ex)
            {
                Monitor.Log($"Command setSunburn failed:\n{ex}", LogLevel.Warn);
            }
        }
        private static void setNewBurn(string _command, string[] _args)
        {
            try
            {
                ModEntry.Instance.Burn.NewBurnDamageLevel = int.Parse(_args[0]);
            }
            catch (Exception ex)
            {
                Monitor.Log($"Command setNewBurn failed:\n{ex}", LogLevel.Warn);
            }
        }
        private static void checkBurn(string _command, string[] _args)
        {
            try
            {
                Monitor.Log($"Current sunburn level: {ModEntry.Instance.Burn.SunburnLevel}\n" +
                    $"Current new burn damage level: {ModEntry.Instance.Burn.NewBurnDamageLevel}\n" +
                    $"Current sun damage count: {ModEntry.Instance.Burn.SunDamageCounter}", LogLevel.Info);
            }
            catch (Exception ex)
            {
                Monitor.Log($"Command checkBurn failed:\n{ex}", LogLevel.Warn);
            }
        }


        /// <summary>Invokes the playSound console commands (first argument specifies the sound name).</summary>
        /// <param name="command">The name of the command invoked.</param>
        /// <param name="args">The arguments received by the command. Each word after the command name is a separate argument.</param>
        private static void cmdPlaySound(string _command, string[] _args)
        {
            try
            {
                if (_args.Length > 0)
                {
                    if (_args.Length == 1)
                    {
                        Game1.playSound(_args[0]);
                        Monitor.Log($"Success! Played sound <{_args[0]}>", LogLevel.Info);
                    }
                    else
                    {
                        if (Int32.TryParse(_args[1], out int pitch))
                        {
                            Game1.playSoundPitched(_args[0], pitch);
                            Monitor.Log($"Success! Played sound <{_args[0]}> with pitch <{pitch}>", LogLevel.Info);
                        }
                        else
                        {
                            Monitor.Log($"ERROR: The optional second argument must be an integer (used for pitched sounds).\n" +
                                $"Usage: playSound <soundname> [pitch]", LogLevel.Info);
                        }
                    }
                }
                else
                {
                    Monitor.Log($"You must enter a sound name with this command. The integer [pitch] argument is optional.\n" +
                        $"Usage: playSound <soundname> [pitch]", LogLevel.Info);
                }
            }
            catch (Exception ex)
            {
                Monitor.Log($"Command playSound failed:\n{ex}", LogLevel.Warn);
            }
        }
    }
}