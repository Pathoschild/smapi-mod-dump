/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
**
*************************************************/

namespace DaLion.Overhaul.Modules.Tools.Patchers;

#region using directives

using DaLion.Shared.Harmony;
using StardewValley.Locations;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationCanRefillWateringCanOnTilePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationCanRefillWateringCanOnTilePatcher"/> class.</summary>
    internal GameLocationCanRefillWateringCanOnTilePatcher()
    {
        this.Target = this.RequireMethod<GameLocation>(nameof(GameLocation.CanRefillWateringCanOnTile));
    }

    #region harmony patches

    private static void GameLocationCanRefillWateringCanOnTilePostfix(
        GameLocation __instance,
        ref bool __result,
        int tileX,
        int tileY)
    {
        if (__result && (__instance is Beach || __instance.catchOceanCrabPotFishFromThisSpot(tileX, tileY)) &&
            ToolsModule.Config.Can.PreventRefillWithSaltWater)
        {
            __result = false;
        }
    }

    #endregion harmony patches
}
