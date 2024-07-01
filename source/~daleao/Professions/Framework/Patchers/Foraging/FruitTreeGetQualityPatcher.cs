/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Professions.Framework.Patchers.Foraging;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.TerrainFeatures;

#endregion using directives

[UsedImplicitly]
internal sealed class FruitTreeGetQualityPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FruitTreeGetQualityPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal FruitTreeGetQualityPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<FruitTree>(nameof(FruitTree.GetQuality));
    }

    #region harmony patches

    /// <summary>Patch to apply Ecologist perk to shaken fruit trees.</summary>
    [HarmonyPrefix]
    private static void FruitTreeGetQualityPrefix(ref int __result)
    {
        if (Game1.player.HasProfession(Profession.Ecologist))
        {
            __result = Game1.player.GetEcologistForageQuality();
        }
    }

    #endregion harmony patches
}
