using System;
using StardewModdingAPI;
using StardewValley;
using StardewModdingAPI.Utilities;

namespace SunscreenMod
{
    /// <summary>Class containing the mod's console commands.</summary>
    public class ConsoleCommands
    {
        /*********
        ** Accessors
        *********/
        static IModHelper Helper => ModEntry.Instance.Helper;
        static IMonitor Monitor => ModEntry.Instance.Monitor;
        static ModConfig Config => ModConfig.Instance;


        /*********
        ** Public methods
        *********/
        /// <summary>Use the Mod Helper to register the commands in this class.</summary>
        public static void Apply()
        {
            string NL = Environment.NewLine;

            Helper.ConsoleCommands.Add("checkBurn",
                "Check sunburn and new burn damage levels." + NL + "Usage: checkBurn",
                checkBurn);
            Helper.ConsoleCommands.Add("getUV",
                "Print the UV high (and UV index) for a certain date, or today if no date specified." + NL + "Usage: getUV [daysSinceStart]",
                getUV);

            if (Config.DebugMode) //Only register most testing commands while in debug mode (needs a game reset to activate)
            {
                Helper.ConsoleCommands.Add("uvPlaySound",
                    "Plays a soundbank sound for testing purposes." + NL + "Usage: uvPlaySound <soundName> [pitch]",
                    cmdPlaySound);
                Helper.ConsoleCommands.Add("checkTime",
                    "Print the current game time." + NL + "Usage: checkTime",
                    checkTime);
                Helper.ConsoleCommands.Add("setSunburn",
                    "Manually set sunburn level from 0-3." + NL + "Usage: setSunburn <level>",
                    setSunburn);
                Helper.ConsoleCommands.Add("setNewBurn",
                    "Manually set new burn damage level from 0-3." + NL + "Usage: setNewBurn <level>",
                    setNewBurn);
            }
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Print the current game time.</summary>
        /// <param name="_command">The name of the command invoked.</param>
        /// <param name="_args">The arguments received by the command. Each word after the command name is a separate argument.</param>
        private static void checkTime(string _command, string[] _args)
        {
            try
            {
                Monitor.Log($"Current game time: {Game1.timeOfDay}", LogLevel.Debug);
            }
            catch (Exception ex)
            {
                Monitor.Log($"Command checkTime failed:\n{ex}", LogLevel.Warn);
            }
        }
        /// <summary>Print the UV value (no scaling) and UV index high for a specific date (int days since start).</summary>
        /// <param name="_command">The name of the command invoked.</param>
        /// <param name="_args">The arguments received by the command. Each word after the command name is a separate argument.</param>
        private static void getUV(string _command, string[] _args)
        {
            try
            {
                int days;
                int dailyMaxUV;
                int uvIndex;

                if (_args.Length > 0)
                {
                    days = int.Parse(_args[0]);
                    SDate date = SDate.FromDaysSinceStart(days);

                    if (date == SDate.Now().AddDays(1)) //date is tomorrow, can be more accurate with weather
                    {
                        dailyMaxUV = UVIndex.DailyMaxUV(days, Game1.weatherForTomorrow);
                        uvIndex = Convert.ToInt32((double)dailyMaxUV / 25);
                        Monitor.Log($"Tomorrow's max UV value: {dailyMaxUV} | UV Index: {uvIndex}", LogLevel.Debug);
                        return;
                    }
                    else if (date != SDate.Now()) //any other day except today
                    {
                        dailyMaxUV = UVIndex.DailyMaxUV(days);
                        uvIndex = Convert.ToInt32((double)dailyMaxUV / 25);
                        Monitor.Log($"Forecasted max UV value (if sunny): {dailyMaxUV} | UV Index: {uvIndex}", LogLevel.Debug);
                        return;
                    }
                }
                //No date argument provided OR day provided is today
                days = SDate.Now().DaysSinceStart;
                dailyMaxUV = UVIndex.DailyMaxUV(days, UVIndex.GetTodaysWeather());
                uvIndex = Convert.ToInt32((double)dailyMaxUV / 25);
                Monitor.Log($"Today's max UV value: {dailyMaxUV} | UV Index: {uvIndex}", LogLevel.Debug);
            }
            catch (Exception ex)
            {
                Monitor.Log($"Command getUV failed:\n{ex}", LogLevel.Warn);
            }
        }
        /// <summary>Set the current player's sunburn level from 0-3.</summary>
        /// <param name="_command">The name of the command invoked.</param>
        /// <param name="_args">The arguments received by the command. Each word after the command name is a separate argument.</param>
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
        /// <summary>Set the current player's new burn damage level from 0-3.</summary>
        /// <param name="_command">The name of the command invoked.</param>
        /// <param name="_args">The arguments received by the command. Each word after the command name is a separate argument.</param>
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
        /// <summary>Print the player's current sunburn and new burn levels plus sun damage count.</summary>
        /// <param name="_command">The name of the command invoked.</param>
        /// <param name="_args">The arguments received by the command. Each word after the command name is a separate argument.</param>
        private static void checkBurn(string _command, string[] _args)
        {
            try
            {
                string protection = "no";
                if (ModEntry.Instance.Sunscreen.IsProtected())
                {
                    protection = $"until {ModEntry.Instance.Sunscreen.TimeOfExpiry.Get12HourTime()}";
                }
                string usedAloe = "no";
                if (ModEntry.Instance.Lotion.HasAppliedAloeToday) usedAloe = "yes";

                Monitor.Log($"Current sunburn level: {ModEntry.Instance.Burn.SunburnLevel}\n" +
                    $"Current new burn damage level: {ModEntry.Instance.Burn.NewBurnDamageLevel}\n" +
                    $"Current sun damage count: {ModEntry.Instance.Burn.SunDamageCounter}\n" +
                    $"Protected by sunscreen: {protection}\n" +
                    $"Used aloe gel today: {usedAloe}", LogLevel.Debug);
            }
            catch (Exception ex)
            {
                Monitor.Log($"Command checkBurn failed:\n{ex}", LogLevel.Warn);
            }
        }


        /// <summary>Invokes the uvPlaySound console commands (first argument specifies the sound name).</summary>
        /// <param name="_command">The name of the command invoked.</param>
        /// <param name="_args">The arguments received by the command. Each word after the command name is a separate argument.</param>
        private static void cmdPlaySound(string _command, string[] _args)
        {
            try
            {
                if (_args.Length > 0)
                {
                    if (_args.Length == 1)
                    {
                        Game1.playSound(_args[0]);
                        Monitor.Log($"Success! Played sound <{_args[0]}>", LogLevel.Debug);
                    }
                    else
                    {
                        if (Int32.TryParse(_args[1], out int pitch))
                        {
                            Game1.playSoundPitched(_args[0], pitch);
                            Monitor.Log($"Success! Played sound <{_args[0]}> with pitch <{pitch}>", LogLevel.Debug);
                        }
                        else
                        {
                            Monitor.Log($"ERROR: The optional second argument must be an integer (used for pitched sounds).\n" +
                                $"Usage: uvPlaySound <soundname> [pitch]", LogLevel.Debug);
                        }
                    }
                }
                else
                {
                    Monitor.Log($"You must enter a sound name with this command. The integer [pitch] argument is optional.\n" +
                        $"Usage: uvPlaySound <soundname> [pitch]", LogLevel.Debug);
                }
            }
            catch (Exception ex)
            {
                Monitor.Log($"Command uvPlaySound failed:\n{ex}", LogLevel.Warn);
            }
        }
    }
}