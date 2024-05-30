/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Professions.Framework.Patchers.Mining;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class UtilityGetTreasureFromGeodePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="UtilityGetTreasureFromGeodePatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal UtilityGetTreasureFromGeodePatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<Utility>(nameof(Utility.getTreasureFromGeode));
    }

    #region harmony patches

    /// <summary>Patch to record minerals collected for Gemologist.</summary>
    [HarmonyPostfix]
    private static void UtilityGetTreasureFromGeodePostfix(Item __result)
    {
        Data.AppendToGemologistMineralsCollected(__result.ItemId, Game1.player);
    }

    #endregion harmony patches
}
