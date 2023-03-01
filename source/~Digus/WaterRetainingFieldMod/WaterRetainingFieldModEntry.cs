/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.TerrainFeatures;

namespace WaterRetainingFieldMod
{
    /// <summary>The mod entry class loaded by SMAPI.</summary>
    public class WaterRetainingFieldModEntry : Mod
    {
        internal static DataLoader DataLoader;
        internal static IManifest Manifest;


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Manifest = this.ModManifest;

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.DayStarted += OnDayStarted;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the game is launched, right before the first update tick. This happens once per game session (unrelated to loading saves). All mods are loaded and initialized at this point, so this is a good time to set up mod integrations.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            DataLoader = new DataLoader(Helper);

            var harmony = new Harmony("Digus.WaterRetainingFieldMod");

            harmony.Patch(
                original: AccessTools.Method(typeof(HoeDirt), nameof(HoeDirt.dayUpdate)),
                prefix: new HarmonyMethod(typeof(HoeDirtOverrides), nameof(HoeDirtOverrides.DayUpdatePrefix)) { priority = Priority.First },
                postfix: new HarmonyMethod(typeof(HoeDirtOverrides), nameof(HoeDirtOverrides.DayUpdatePostfix)) { priority = Priority.First }
            );
        }

        /// <summary>Raised after the game begins a new day (including when the player loads a save).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            HoeDirtOverrides.TileLocationState.Clear();
        }
    }
}
