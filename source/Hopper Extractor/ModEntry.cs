/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/BPavol/stardew-valley-mods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using HarmonyLib;
using HopperExtractor.Patches;

namespace HopperExtractor
{
    internal sealed class ModEntry: Mod
    {
       /*********
       ** Public methods
       *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            var harmony = new Harmony(this.ModManifest.UniqueID);

            ObjectPatches.Initialize(this.Monitor);
            harmony.Patch(
               original: AccessTools.Method(typeof(StardewValley.Object), nameof(StardewValley.Object.minutesElapsed)),
               prefix: new HarmonyMethod(typeof(ObjectPatches), nameof(ObjectPatches.After_MinutesElapsed))
            );
        }
    }
}
