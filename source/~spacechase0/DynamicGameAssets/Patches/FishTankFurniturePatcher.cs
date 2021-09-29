/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System.Diagnostics.CodeAnalysis;
using DynamicGameAssets.Game;
using HarmonyLib;
using Spacechase.Shared.Patching;
using SpaceShared;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using static StardewValley.Objects.FishTankFurniture;

namespace DynamicGameAssets.Patches
{
    /// <summary>Applies Harmony patches to <see cref="FishTankFurniture"/>.</summary>
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = DiagnosticMessages.NamedForHarmony)]
    internal class FishTankFurniturePatcher : BasePatcher
    {
        /*********
        ** Public methods
        *********/
        /// <inheritdoc />
        public override void Apply(Harmony harmony, IMonitor monitor)
        {
            harmony.Patch(
                original: this.RequireMethod<FishTankFurniture>(nameof(FishTankFurniture.GetCapacityForCategory)),
                prefix: this.GetHarmonyMethod(nameof(Before_GetCapacityForCategory))
            );
        }


        /*********
        ** Private methods
        *********/
        /// <summary>The method to call before <see cref="FishTankFurniture.GetCapacityForCategory"/>.</summary>
        /// <returns>Returns whether to run the original method.</returns>
        private static bool Before_GetCapacityForCategory(FishTankFurniture __instance, FishTankCategories category, ref int __result)
        {
            if (__instance is CustomFishTankFurniture fishTank)
            {
                __result = fishTank.GetCapacityForCategory(category);
                return false;
            }

            return true;
        }
    }
}
