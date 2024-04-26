/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/idermailer/NoGreenhouseEntranceTiles
**
*************************************************/

using StardewModdingAPI;
using HarmonyLib;

namespace NoGreenhouseEntranceTiles
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            var harmony = new Harmony(ModManifest.UniqueID);

            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Buildings.GreenhouseBuilding), nameof(StardewValley.Buildings.GreenhouseBuilding.CanDrawEntranceTiles)),
                prefix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.AlwaysFalse))
                );
        }

        private static bool AlwaysFalse(ref bool __result)
        {
            __result = false;
            return false;
        }
    }
}
