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
using Harmony;
using MultiFertilizer.Framework;
using Spacechase.Shared.Patching;
using SpaceShared;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Characters;
using StardewValley.TerrainFeatures;

namespace MultiFertilizer.Patches
{
    /// <summary>Applies Harmony patches to <see cref="Crop"/>.</summary>
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = DiagnosticMessages.NamedForHarmony)]
    internal class CropPatcher : BasePatcher
    {
        /*********
        ** Public methods
        *********/
        /// <inheritdoc />
        public override void Apply(HarmonyInstance harmony, IMonitor monitor)
        {
            harmony.Patch(
                original: this.RequireMethod<Crop>(nameof(Crop.harvest)),
                prefix: this.GetHarmonyMethod(nameof(Before_Harvest)),
                postfix: this.GetHarmonyMethod(nameof(After_Harvest))
            );
        }


        /*********
        ** Private methods
        *********/
        /// <summary>The method to call before <see cref="Crop.harvest"/>.</summary>
        private static void Before_Harvest(Crop __instance, int xTile, int yTile, HoeDirt soil, JunimoHarvester junimoHarvester)
        {
            if (!soil.TryGetFertilizer(Mod.KeyFert, out FertilizerData fertilizer))
                return;

            soil.fertilizer.Value = fertilizer.Id;
        }

        /// <summary>The method to call after <see cref="Crop.harvest"/>.</summary>
        private static void After_Harvest(Crop __instance, int xTile, int yTile, HoeDirt soil, JunimoHarvester junimoHarvester)
        {
            soil.fertilizer.Value = 0;
        }
    }
}
