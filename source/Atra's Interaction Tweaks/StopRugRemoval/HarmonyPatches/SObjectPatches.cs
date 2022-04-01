/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StopRugRemoval
**
*************************************************/

using AtraShared.Utils.Extensions;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Objects;
using StopRugRemoval.Configuration;
using StopRugRemoval.HarmonyPatches.BombHandling;

namespace StopRugRemoval.HarmonyPatches;

/// <summary>
/// Patches against SObject.
/// </summary>
[HarmonyPatch(typeof(SObject))]
internal static class SObjectPatches
{
    /// <summary>
    /// Prefix to prevent planting of wild trees on rugs.
    /// </summary>
    /// <param name="location">Game location.</param>
    /// <param name="tile">Tile to look at.</param>
    /// <param name="__result">the replacement result.</param>
    /// <returns>True to continue to vanilla function, false otherwise.</returns>
    [HarmonyPrefix]
    [HarmonyPatch("canPlaceWildTreeSeed")]
    [SuppressMessage("StyleCop", "SA1313", Justification = "Style prefered by Harmony")]
    private static bool PrefixWildTrees(GameLocation location, Vector2 tile, ref bool __result)
    {
        try
        {
            if (!ModEntry.Config.PreventPlantingOnRugs)
            {
                return true;
            }
            (int posX, int posY) = ((tile * 64f) + new Vector2(32f, 32f)).ToPoint();
            foreach (Furniture f in location.furniture)
            {
                if (f.furniture_type.Value == Furniture.rug && f.getBoundingBox(f.TileLocation).Contains(posX, posY))
                {
                    __result = false;
                    return false;
                }
            }
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Errored while trying to prevent tree growth\n\n{ex}.", LogLevel.Error);
        }
        return true;
    }

    /// <summary>
    /// Prefix on placement to prevent planting of fruit trees and tea saplings on rugs, hopefully.
    /// </summary>
    /// <param name="__instance">SObject instance to check.</param>
    /// <param name="location">Gamelocation being placed in.</param>
    /// <param name="x">X placement location in pixel coordinates.</param>
    /// <param name="y">Y placement location in pixel coordinates.</param>
    /// <param name="__result">Result of the function.</param>
    /// <returns>True to continue to vanilla function, false otherwise.</returns>
    [HarmonyPrefix]
    [HarmonyPatch(nameof(SObject.placementAction))]
    [SuppressMessage("StyleCop", "SA1313", Justification = "Style prefered by Harmony")]
    private static bool PrefixPlacementAction(SObject __instance, GameLocation location, int x, int y, ref bool __result)
    {
        try
        {
            if (ModEntry.Config.PreventPlantingOnRugs && __instance.isSapling())
            {
                foreach (Furniture f in location.furniture)
                {
                    if (f.getBoundingBox(f.TileLocation).Contains(x, y))
                    {
                        Game1.showRedMessage(I18n.RugPlantingMessage());
                        __result = false;
                        return false;
                    }
                }
            }
            if (!ConfirmBomb.HaveConfirmed.Value
                && !__instance.bigCraftable.Value && __instance is not Furniture
                && __instance.ParentSheetIndex is 286 or 287 or 288
                && (IsLocationConsideredDangerous(location) ? ModEntry.Config.InDangerousAreas : ModEntry.Config.InSafeAreas)
                    .HasFlag(Context.IsMultiplayer ? ConfirmBombEnum.InMultiplayerOnly : ConfirmBombEnum.NotInMultiplayer))
            {
                // handle the case where a bomb has already been placed?
                Vector2 loc = new(x, y);
                foreach (TemporaryAnimatedSprite tas in location.temporarySprites)
                {
                    if (tas.position.Equals(loc))
                    {
                        __result = false;
                        return false;
                    }
                }

                Response[] responses = new Response[]
                {
                    new Response("BombsNo", I18n.No()),
                    new Response("BombsYes", I18n.Yes()),
                    new Response("BombsArea", I18n.YesArea()),
                };

                location.createQuestionDialogue(I18n.ConfirmBombs(), responses, "atravitaInteractionTweaksBombs");
                ConfirmBomb.BombLocation.Value = loc;
                ConfirmBomb.WhichBomb.Value = __instance.ParentSheetIndex;
                __result = false;
                return false;
            }
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Mod failed while trying to prevent tree planting\n\n{ex}", LogLevel.Error);
        }
        return true;
    }

    private static bool IsLocationConsideredDangerous(GameLocation location)
    {
        return ModEntry.Config.SafeLocationMap.TryGetValue(location.NameOrUniqueName, out IsSafeLocationEnum val)
            ? (val == IsSafeLocationEnum.Dangerous) || (val == IsSafeLocationEnum.Dynamic && location.IsDangerousLocation())
            : location.IsDangerousLocation();
    }
}