/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/PrimmR/Turbo
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using StardewValley;

namespace Turbo
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {
        /*********
        ** Attributes
        *********/

        /*** Public ***/
        public const long SPF = 166667;

        public static double speed = 1;

        /*** Private ***/
        internal static ModConfig Config;

        internal static long elapsedTicks = 0;

        internal static long nextFrame = 0;

        internal static int change = 0; // 0 Up, 1 Down, 2 Reset

        internal static long clockTicks = 0; // To fix very small issue with linear clock mode


        /// <summary>The mod entry point method.</summary>
        public override void Entry(IModHelper helper)
        {
            LoadConfig();

            // Harmony patches
            var harmony = new Harmony(this.ModManifest.UniqueID);
            harmony.Patch(AccessTools.Method(typeof(Game1), nameof(Game1.Instance_Update)),
                new HarmonyMethod(typeof(UpdatePatcher), nameof(UpdatePatcher.UpdateGame)));
            harmony.Patch(original: AccessTools.Method(typeof(Game1), nameof(Game1.UpdateGameClock)),
                new HarmonyMethod(typeof(UpdatePatcher), nameof(UpdatePatcher.UpdateClock)));
            harmony.Patch(original: AccessTools.Method(typeof(HUDMessage), nameof(HUDMessage.draw)),
                finalizer: new HarmonyMethod(typeof(HUDPatcher), nameof(HUDPatcher.HUDDraw_Final)));

            // Inputs
            helper.Events.Input.ButtonPressed += IO.OnButtonPressed;
            helper.Events.GameLoop.SaveLoaded += IO.OnSaveLoaded;
            helper.Events.GameLoop.DayStarted += IO.OnDayStarted;

            helper.ConsoleCommands.Add("set_speed", "Sets the game speed.\n\nUsage: set_speed <value>\n- value: the speed multiplier.", IO.SetSpeedCmd);
            helper.ConsoleCommands.Add("set_clock_mode", "Sets the behaviour of the in-game clock.\n\nUsage: set_clock_mode <value>\n- value: the clock mode (0, 1, or 2):\n   0: the clock increments proportionally with game speed\n   1: the clock increments at a constant rate, regardless of game speed \n   2: the clock is frozen", IO.SetClockModeCmd);

            IO.Initialise(this.Monitor, this.Helper);
            UpdatePatcher.Initialise(this.Monitor);
            HUDPatcher.Initialise(this.Monitor);
        }

        /// <summary>Resets Turbo's internal timer to 0.</summary>
        public static void ResetClock()
        {
            elapsedTicks = 0;
            nextFrame = 0;
        }

        /// <summary>Loads the mod's configuration data.</summary>
        private void LoadConfig()
        {
            Config = this.Helper.ReadConfig<ModConfig>();
            if (Config.clockMode < 0 || Config.clockMode > 2)
            {
                Config.clockMode = 0;
                Helper.WriteConfig(ModEntry.Config);
                Monitor.Log("Clock mode set to invalid state - Initialised to 0 (regular)", LogLevel.Error);
            }
        }
    }
}