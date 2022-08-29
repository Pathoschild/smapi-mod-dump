/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

#region using directives

using HarmonyLib;
using StardewValley.Monsters;
using Ultimates;
using VirtualProperties;

#endregion using directives

[UsedImplicitly]
internal sealed class DustSpiritBehaviorAtGameTickPatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal DustSpiritBehaviorAtGameTickPatch()
    {
        Target = RequireMethod<DustSpirit>(nameof(DustSpirit.behaviorAtGameTick));
    }

    #region harmony patches

    /// <summary>Patch to hide Poacher from Dust Spirits during Ultimate.</summary>
    [HarmonyPostfix]
    private static void DustSpiritBehaviorAtGameTickPostfix(DustSpirit __instance, ref bool ___seenFarmer)
    {
        if (!__instance.Player.IsLocalPlayer || __instance.Player.get_Ultimate() is not
                Ambush { IsActive: true }) return;
        ___seenFarmer = false;
    }

    #endregion harmony patches
}