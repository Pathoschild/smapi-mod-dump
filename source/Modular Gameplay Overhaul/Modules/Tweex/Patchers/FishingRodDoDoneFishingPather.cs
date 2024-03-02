/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
**
*************************************************/

namespace DaLion.Overhaul.Modules.Tweex.Patchers;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class FishingRodDoDoneFishingPather : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FishingRodDoDoneFishingPather"/> class.</summary>
    internal FishingRodDoDoneFishingPather()
    {
        this.Target = this.RequireMethod<FishingRod>("doDoneFishing");
    }

    #region harmony patches

    [HarmonyPrefix]
    private static void FishingRodDoDoneFishingPrefix(bool ___lastCatchWasJunk, ref bool consumeBaitAndTackle)
    {
        if (TweexModule.Config.TrashDoesNotConsumeBait && ___lastCatchWasJunk)
        {
            consumeBaitAndTackle = false;
        }
    }

    #endregion harmony patches
}
