/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/PrimmR/Turbo
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewModdingAPI;
using StardewValley.Menus;
using StardewValley;

namespace Turbo
{
    /// <summary>Handles input & output events</summary>
    internal class IO
    {
        private static IMonitor Monitor;
        private static IModHelper Helper;

        internal static void Initialise(IMonitor monitor, IModHelper helper)
        {
            Monitor = monitor;
            Helper = helper;
        }

        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        internal static void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            // Ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;

            // Decrease speed [<]
            if (ModEntry.Config.decrementSpeedKeybind.JustPressed())
            {
                if (ModEntry.speed > 0.25)
                {
                    ModEntry.speed /= 2;
                    ModEntry.speed = CeilLog2(ModEntry.speed);
                    ModEntry.change = 1;
                }
                SpeedMessage();
                Game1.playSound("smallSelect", 810);
            }

            // Increase speed [>]
            if (ModEntry.Config.incrementSpeedKeybind.JustPressed())
            {
                if (ModEntry.speed < 32)
                {
                    ModEntry.speed *= 2;
                    ModEntry.speed = FloorLog2(ModEntry.speed);
                    ModEntry.change = 0;
                }
                SpeedMessage();
                Game1.playSound("smallSelect", 1590);
            }

            // Reset speed [;]
            if (ModEntry.Config.resetSpeedKeybind.JustPressed())
            {
                ModEntry.speed = 1;
                ModEntry.change = 2;
                SpeedMessage();
                Game1.playSound("smallSelect", 1200);
            }

            // Cycle clock mode [']
            if (ModEntry.Config.cycleClockModeKeybind.JustPressed())
            {
                ModEntry.Config.clockMode += 1;
                ModEntry.Config.clockMode %= 3;
                ClockModeMessage();
                Helper.WriteConfig(ModEntry.Config);
                Game1.playSound("smallSelect", 1410);
            }
        }

        /// <summary>Sends a HUD and console message about the current game speed.</summary>
        private static void SpeedMessage()
        {
            // Hud message
            Game1.hudMessages.Clear();
            HUDMessage message = new HUDMessage($"{Helper.Translation.Get("Speed.Change")} {ModEntry.speed}x", 2082);
            message.timeLeft = 3500f * (float)ModEntry.speed;
            Game1.addHUDMessage(message);
            Monitor.Log($"Sent HUD message", LogLevel.Trace);

            // Console message
            Monitor.Log($"Speed changed to: {ModEntry.speed}x", LogLevel.Info);
        }

        /// <summary>Sends a HUD and console message about the current clock mode.</summary>
        private static void ClockModeMessage()
        {
            // Hud message
            Game1.hudMessages.Clear();
            ModEntry.change = 2;

            string str = ModEntry.Config.clockMode switch
            {
                0 => Helper.Translation.Get("ClockMode.Regular"),
                1 => Helper.Translation.Get("ClockMode.Constant"),
                2 => Helper.Translation.Get("ClockMode.Frozen"),
                _ => throw new Exception($"Invalid Clock Mode in {nameof(ClockModeMessage)}")
            };
            HUDMessage message = new HUDMessage($"{Helper.Translation.Get("ClockMode.Change")} {str}", 2082);
            message.timeLeft = 3500f * (float)ModEntry.speed;
            Game1.addHUDMessage(message);
            Monitor.Log($"Sent HUD message", LogLevel.Trace);

            // Console message
            Monitor.Log($"Clock Mode changed to: {str}", LogLevel.Info);
        }

        /// <summary>Calculates the first power of 2 below a certain value.</summary>
        /// <param name="x">The value to floor.</param>
        private static double FloorLog2(double x)
        {
            return Math.Pow(2, Math.Floor(Math.Log2(x)));
        }

        /// <summary>Calculates the first power of 2 above a certain value.</summary>
        /// <param name="x">The value to ceiling.</param>
        private static double CeilLog2(double x)
        {
            return Math.Pow(2, Math.Ceiling(Math.Log2(x)));
        }

        // Commands

        /// <summary>Handles the SMAPI console set_speed command.</summary>
        public static void SetSpeedCmd(string command, string[] args)
        {
            try
            {
                double mult = double.Parse(args[0]);
                if (mult > 0)
                {
                    ModEntry.speed = mult;
                    SpeedMessage();
                }
                else
                {
                    Monitor.Log($"Speed must be greater than 0", LogLevel.Error);
                }
            }
            catch
            {
                Monitor.Log($"Could not parse command", LogLevel.Error);
            }
        }

        /// <summary>Handles the SMAPI console set_clock_mode command.</summary>
        public static void SetClockModeCmd(string command, string[] args)
        {
            try
            {
                int mode = int.Parse(args[0]);
                if (mode >= 0 && mode <= 2)
                {
                    ModEntry.Config.clockMode = mode;
                    ClockModeMessage();
                    Helper.WriteConfig(ModEntry.Config);
                }
                else
                {
                    Monitor.Log($"Mode must be either: 0, 1, or 2\nSee help for more info", LogLevel.Error);
                }
            }
            catch
            {
                Monitor.Log($"Could not parse command", LogLevel.Error);
            }
        }

        // Events
        /// <summary>Called on every new day.</summary>
        internal static void OnDayStarted(object? sender, DayStartedEventArgs e)
        {
            ModEntry.ResetClock();
            Monitor.Log("Reset clock", LogLevel.Trace);
        }

        /// <summary>Called on save load.</summary>
        internal static void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
            ModEntry.speed = 1;
            Monitor.Log("Speed initialised to: 1x", LogLevel.Info);
        }
    }
}
