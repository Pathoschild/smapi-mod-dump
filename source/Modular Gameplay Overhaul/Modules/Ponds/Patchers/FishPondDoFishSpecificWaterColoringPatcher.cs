/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
**
*************************************************/

namespace DaLion.Overhaul.Modules.Ponds.Patchers;

#region using directives

using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Extensions.Xna;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Buildings;

#endregion using directives

[UsedImplicitly]
internal sealed class FishPondDoFishSpecificWaterColoringPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FishPondDoFishSpecificWaterColoringPatcher"/> class.</summary>
    internal FishPondDoFishSpecificWaterColoringPatcher()
    {
        this.Target = this.RequireMethod<FishPond>("doFishSpecificWaterColoring");
    }

    #region harmony patches

    /// <summary>Recolor for algae/seaweed.</summary>
    [HarmonyPostfix]
    private static void FishPondDoFishSpecificWaterColoringPostfix(FishPond __instance)
    {
        if (__instance.fishType.Value.IsAlgaeIndex())
        {
            var shift = -(5 + (3 * __instance.FishCount));
            __instance.overrideWaterColor.Value = new Color(60, 126, 150).ShiftHue(shift);
        }
        else if (__instance.GetFishObject().Name.ContainsAnyOf("Mutant", "Radioactive"))
        {
            __instance.overrideWaterColor.Value = new Color(40, 255, 40);
        }
    }

    #endregion harmony patches
}
