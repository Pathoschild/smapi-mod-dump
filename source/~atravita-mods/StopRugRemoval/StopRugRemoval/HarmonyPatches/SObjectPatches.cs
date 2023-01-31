/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraShared.Menuing;
using AtraShared.Utils;
using AtraShared.Utils.Extensions;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI.Utilities;
using StardewValley.Objects;
using StardewValley.Tools;

using StopRugRemoval.Configuration;

namespace StopRugRemoval.HarmonyPatches;

/// <summary>
/// Patches against SObject.
/// </summary>
[HarmonyPatch(typeof(SObject))]
internal static class SObjectPatches
{
    /// <summary>
    /// Whether or not bombs have been confirmed.
    /// </summary>
    internal static readonly PerScreen<bool> HaveConfirmedBomb = new(createNewState: () => false);

    /// <summary>
    /// Prefix to prevent planting of wild trees on rugs.
    /// </summary>
    /// <param name="location">Game location.</param>
    /// <param name="tile">Tile to look at.</param>
    /// <param name="__result">the replacement result.</param>
    /// <returns>True to continue to vanilla function, false otherwise.</returns>
    [HarmonyPrefix]
    [HarmonyPatch("canPlaceWildTreeSeed")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "HarmonyConvention")]
    private static bool PrefixWildTrees(GameLocation location, Vector2 tile, ref bool __result)
    {
        try
        {
            if (!ModEntry.Config.PreventPlantingOnRugs)
            {
                return true;
            }

            int posX = ((int)tile.X * 64) + 32;
            int posY = ((int)tile.Y * 64) + 32;
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

    [HarmonyPrefix]
    [HarmonyPatch(nameof(SObject.onExplosion))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "HarmonyConvention")]
    private static void PrefixOnExplosion(SObject __instance, Farmer who, GameLocation location)
    {
        try
        {
            if (__instance.IsSpawnedObject && ModEntry.Config.SaveBombedForage && ModEntry.Config.Enabled)
            {
                // The SObject does not have its location anymore. Just spawn near the farmer, I guess?
                location.debris.Add(new Debris(__instance, who.Position + new Vector2(Game1.random.Next(-128, 128), Game1.random.Next(-128, 128))));
                ModEntry.ModMonitor.DebugOnlyLog(__instance.DisplayName + ' ' + __instance.TileLocation.ToString(), LogLevel.Warn);
            }
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Error while creating debris.{ex}", LogLevel.Error);
        }
    }

    /// <summary>
    /// Prevent hoes from lifting up the scarecrows.
    /// </summary>
    /// <param name="__instance">SObject instance.</param>
    /// <param name="t">tool used.</param>
    [HarmonyPrefix]
    [HarmonyPatch(nameof(SObject.performToolAction))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "HarmonyConvention")]
    private static bool PrefixPerformToolAction(SObject __instance, Tool t, ref bool __result)
    {
        __result = t is not Hoe || !__instance.IsScarecrow();
        return __result;
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
            if (!HaveConfirmedBomb.Value && ModEntry.Config.Enabled
                && !__instance.bigCraftable.Value && __instance is not Furniture
                && __instance.ParentSheetIndex is 286 or 287 or 288
                && (IsLocationConsideredDangerous(location) ? ModEntry.Config.BombsInDangerousAreas : ModEntry.Config.BombsInSafeAreas)
                    .HasFlag(Context.IsMultiplayer ? ConfirmationEnum.InMultiplayerOnly : ConfirmationEnum.NotInMultiplayer))
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

                List<Response> responses = new()
                {
                    new Response("BombsYes", I18n.YesOne()).SetHotKey(Keys.Y),
                    new Response("BombsArea", I18n.YesArea()),
                    new Response("BombsNo", I18n.No()).SetHotKey(Keys.Escape),
                };

                List<Action?> actions = new()
                {
                    () =>
                    {
                        Game1.player.reduceActiveItemByOne();
                        GameLocationUtils.ExplodeBomb(Game1.player.currentLocation, __instance.ParentSheetIndex, loc, ModEntry.Multiplayer());
                    },
                    () =>
                    {
                        HaveConfirmedBomb.Value = true;
                        Game1.player.reduceActiveItemByOne();
                        GameLocationUtils.ExplodeBomb(Game1.player.currentLocation, __instance.ParentSheetIndex, loc, ModEntry.Multiplayer());
                    },
                };

                __result = false;

                Game1.activeClickableMenu = new DialogueAndAction(I18n.ConfirmBombs(), responses, actions, ModEntry.InputHelper);
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
        => ModEntry.Config.SafeLocationMap.TryGetValue(location.NameOrUniqueName, out IsSafeLocationEnum val)
            ? (val == IsSafeLocationEnum.Dangerous) || (val == IsSafeLocationEnum.Dynamic && location.IsDangerousLocation())
            : location.IsDangerousLocation();
}