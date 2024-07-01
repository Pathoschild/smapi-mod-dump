/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Professions.Framework.Patchers.Fishing;

#region using directives

using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Buildings;

#endregion using directives

[UsedImplicitly]
internal sealed class FishPondIsLegalFishForPondsPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FishPondIsLegalFishForPondsPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal FishPondIsLegalFishForPondsPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<FishPond>("isLegalFishForPonds");
    }

    #region harmony patches

    /// <summary>Patch for Aquarist to raise legendary fish.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.HigherThanNormal)]
    private static bool FishPondIsLegalFishForPondsPrefix(FishPond __instance, ref bool __result, string itemId)
    {
        if (!ItemContextTagManager.HasBaseTag(itemId, "fish_legendary"))
        {
            return true; // run original logic
        }

        __result = __instance.GetOwner().HasProfessionOrLax(Profession.Aquarist);
        return false; // don't run original logic
    }

    #endregion harmony patches
}
