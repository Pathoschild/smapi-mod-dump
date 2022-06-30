/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Patches.Prestige;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerCheckForLevelGainPatch : DaLion.Common.Harmony.HarmonyPatch
{
    private const int PRESTIGE_GATE_I = 15000;

    /// <summary>Construct an instance.</summary>
    internal FarmerCheckForLevelGainPatch()
    {
        Target = RequireMethod<Farmer>(nameof(Farmer.checkForLevelGain));
    }

    #region harmony patches

    /// <summary>Patch to allow level increase up to 20.</summary>
    [HarmonyPostfix]
    private static void FarmerCheckForLevelGainPostfix(ref int __result, int oldXP, int newXP)
    {
        if (!ModEntry.Config.EnablePrestige) return;

        for (var i = 1; i <= 10; ++i)
        {
            var requiredExpForThisLevel = PRESTIGE_GATE_I + ModEntry.Config.RequiredExpPerExtendedLevel * i;
            if (oldXP >= requiredExpForThisLevel) continue;
            if (newXP < requiredExpForThisLevel) return;

            __result = i + 10;
        }
    }

    #endregion harmony patches
}