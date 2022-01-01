/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

using HarmonyLib;
using JetBrains.Annotations;
using StardewValley.Monsters;

namespace TheLion.Stardew.Professions.Framework.Patches;

[UsedImplicitly]
internal class DustSpiritBehaviorAtGameTickPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal DustSpiritBehaviorAtGameTickPatch()
    {
        Original = RequireMethod<DustSpirit>(nameof(DustSpirit.behaviorAtGameTick));
    }

    #region harmony patches

    /// <summary>Patch to hide Poacher from Dust Spirits during Super Mode.</summary>
    [HarmonyPostfix]
    private static void DustSpiritBehaviorAtGameTickPostfix(DustSpirit __instance, ref bool ___seenFarmer)
    {
        if (!__instance.Player.IsLocalPlayer || !ModState.IsSuperModeActive ||
            ModState.SuperModeIndex != Utility.Professions.IndexOf("Poacher")) return;
        ___seenFarmer = false;
    }

    #endregion harmony patches
}