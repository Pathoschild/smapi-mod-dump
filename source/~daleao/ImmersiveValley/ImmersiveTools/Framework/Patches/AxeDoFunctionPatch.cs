/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Stardew.Tools.Framework.Patches;

#region using directives

using HarmonyLib;
using StardewValley.Tools;
using System;

#endregion using directives

[UsedImplicitly]
internal sealed class AxeDoFunctionPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal AxeDoFunctionPatch()
    {
        Target = RequireMethod<Axe>(nameof(Axe.DoFunction));
    }

    #region harmony patches

    /// <summary>Charge shockwave stamina cost.</summary>
    [HarmonyPostfix]
    private static void AxeDoFunctionPostfix(int power, Farmer who)
    {
        if (power > 0)
            who.Stamina -=
                (int)Math.Round(Math.Sqrt(Math.Max(2 * power - who.ForagingLevel * 0.1f, 0.1f) *
                                          (int)Math.Pow(2d * power, 2d))) * ModEntry.Config.StaminaCostMultiplier;
    }

    #endregion harmony patches
}