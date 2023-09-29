/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
**
*************************************************/

namespace DaLion.Overhaul.Modules.Combat.Patchers.Enchantments;

#region using directives

using DaLion.Overhaul.Modules.Combat.VirtualProperties;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationIsTileOccupiedForPlacementPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationIsTileOccupiedForPlacementPatcher"/> class.</summary>
    internal GameLocationIsTileOccupiedForPlacementPatcher()
    {
        this.Target = this.RequireMethod<GameLocation>(nameof(GameLocation.isTileOccupiedForPlacement));
    }

    #region harmony patches

    /// <summary>Collide with Wabbajack animals.</summary>
    [HarmonyPostfix]
    private static void GameLocationIsTileOccupiedForPlacementPostfix(GameLocation __instance, ref bool __result, Vector2 tileLocation)
    {
        if (__result)
        {
            return;
        }

        foreach (var animal in __instance.Get_Animals())
        {
            if (!animal.getTileLocation().Equals(tileLocation))
            {
                continue;
            }

            __result = true;
            break;
        }
    }

    #endregion harmony patches
}
