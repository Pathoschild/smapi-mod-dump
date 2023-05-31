/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Ponds.Patchers;

#region using directives

using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Buildings;

#endregion using directives

[UsedImplicitly]
internal sealed class FishPondJumpFishPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FishPondJumpFishPatcher"/> class.</summary>
    internal FishPondJumpFishPatcher()
    {
        this.Target = this.RequireMethod<FishPond>(nameof(FishPond.JumpFish));
    }

    #region harmony patches

    /// <summary>Prevent un-immersive jumping algae.</summary>
    [HarmonyPrefix]
    private static bool FishPondJumpFishPrefix(FishPond __instance, ref bool __result)
    {
        if (!__instance.fishType.Value.IsAlgaeIndex())
        {
            return true; // run original logic
        }

        __result = false;
        return false; // don't run original logic
    }

    #endregion harmony patches
}
