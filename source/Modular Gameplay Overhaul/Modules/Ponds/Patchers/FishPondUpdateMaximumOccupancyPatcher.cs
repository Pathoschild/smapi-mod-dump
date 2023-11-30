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

using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Buildings;

#endregion using directives

[UsedImplicitly]
internal sealed class FishPondUpdateMaximumOccupancyPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FishPondUpdateMaximumOccupancyPatcher"/> class.</summary>
    internal FishPondUpdateMaximumOccupancyPatcher()
    {
        this.Target = this.RequireMethod<FishPond>(nameof(FishPond.UpdateMaximumOccupancy));
        this.Postfix!.after = new[] { OverhaulModule.Professions.Name };
    }

    #region harmony patches

    /// <summary>Set Tui-La pond capacity.</summary>
    [HarmonyPostfix]
    [HarmonyAfter("DaLion.Overhaul.Modules.Professions")]
    private static void FishPondUpdateMaximumOccupancyPostfix(FishPond __instance)
    {
        if (__instance.fishType.Value is 1127 or 1128)
        {
            __instance.maxOccupants.Set(2);
        }
    }

    #endregion harmony patches
}
