/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/StardewValleyMods
**
*************************************************/

using System.Diagnostics.CodeAnalysis;
using Harmony;
using Spacechase.Shared.Harmony;
using SpaceShared;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;

namespace BuildableLocationsFramework.Patches
{
    /// <summary>Applies Harmony patches to <see cref="Building"/>.</summary>
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = DiagnosticMessages.NamedForHarmony)]
    internal class BuildingPatcher : BasePatcher
    {
        /*********
        ** Public methods
        *********/
        /// <inheritdoc />
        public override void Apply(HarmonyInstance harmony, IMonitor monitor)
        {
            harmony.Patch(
                original: this.RequireMethod<Building>(nameof(Building.updateInteriorWarps)),
                postfix: this.GetHarmonyMethod(nameof(After_UpdateInteriorWarps))
            );
        }


        /*********
        ** Private methods
        *********/
        /// <summary>The method to call before <see cref="Building.updateInteriorWarps"/>.</summary>
        private static void After_UpdateInteriorWarps(Building __instance, GameLocation interior)
        {
            string targetName = Mod.FindOutdoorsOf(__instance)?.Name;
            if (targetName == null)
                return;

            interior ??= __instance.indoors.Value;
            if (interior == null)
                return;
            foreach (Warp warp in interior.warps)
            {
                warp.TargetName = targetName;
            }
        }
    }
}
