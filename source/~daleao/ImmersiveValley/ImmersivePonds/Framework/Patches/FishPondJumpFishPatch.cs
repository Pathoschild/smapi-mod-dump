/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Stardew.Ponds.Framework.Patches;

#region using directives

using Extensions;
using HarmonyLib;
using StardewValley.Buildings;

#endregion using directives

[UsedImplicitly]
internal sealed class FishPondJumpFishPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal FishPondJumpFishPatch()
    {
        Target = RequireMethod<FishPond>(nameof(FishPond.JumpFish));
    }

    #region harmony patches

    /// <summary>Prevent un-immersive jumping algae.</summary>
    [HarmonyPrefix]
    private static bool FishPondJumpFishPrefix(FishPond __instance, ref bool __result)
    {
        if (!__instance.fishType.Value.IsAlgaeIndex()) return true; // run original logic

        __result = false;
        return false; // don't run original logic
    }

    #endregion harmony patches
}