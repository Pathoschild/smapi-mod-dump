/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using Fishnets.Data;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile.Dimensions;

namespace Fishnets
{
    internal static class Patches
    {
        internal static IMonitor IMonitor => ModEntry.IMonitor;

        internal static readonly Fishnet _instance = new();

        internal static void Patch(IModHelper helper)
        {
            Harmony harmony = new(helper.ModRegistry.ModID);

            harmony.Patch(
                original: AccessTools.Method(typeof(Object), nameof(Object.placementAction)),
                prefix: new(typeof(Patches), nameof(placementActionPrefix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Object), nameof(Object.canBePlacedInWater)),
                prefix: new(typeof(Patches), nameof(canBePlacedInWaterPrefix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Object), nameof(Object.canBePlacedHere)),
                prefix: new(typeof(Patches), nameof(canBePlacedHerePrefix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Object), nameof(Object.isPlaceable)),
                prefix: new(typeof(Patches), nameof(isPlaceablePrefix))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(CraftingPage), "layoutRecipes"),
                postfix: new(typeof(Patches), nameof(layoutRecipesPostfix))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(CraftingRecipe), nameof(CraftingRecipe.drawMenuView)),
                prefix: new(typeof(Patches), nameof(drawMenuViewPrefix))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Object), nameof(Object.drawInMenu), new[] { typeof(SpriteBatch), typeof(Vector2), typeof(float), typeof(float), typeof(float), typeof(StackDrawType), typeof(Color), typeof(bool) }),
                prefix: new(typeof(Patches), nameof(drawInMenuPrefix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Object), nameof(Object.drawWhenHeld)),
                prefix: new(typeof(Patches), nameof(drawWhenHeldPrefix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Object), nameof(Object.drawPlacementBounds)),
                postfix: new(typeof(Patches), nameof(drawPlacementBoundsPostfix))
            );
        }

        internal static bool placementActionPrefix(Object __instance, ref bool __result, GameLocation location, int x, int y, Farmer who = null)
        {
            try
            {
                if (__instance.ParentSheetIndex == ModEntry.ObjectInfo.Id)
                {
                    Point tile = new((int)Math.Floor(x / 64f), (int)Math.Floor(y / 64f));
                    if (!Fishnet.IsValidPlacementLocation(location, tile.X, tile.Y))
                        return false;
                    __result = new Fishnet(new(tile.X, tile.Y)).placementAction(location, x, y, who);
                    if (__result && __instance.Stack <= 0)
                        Game1.player.removeItemFromInventory(__instance);
                    return false;
                }
                return true;
            }
            catch (Exception ex) { return handleError($"Object.{nameof(Object.placementAction)}", ex, __instance is null || __instance.ParentSheetIndex != ModEntry.ObjectInfo.Id); }
        }

        internal static bool canBePlacedInWaterPrefix(Object __instance, ref bool __result)
        {
            try
            {
                if (__instance.ParentSheetIndex == ModEntry.ObjectInfo.Id)
                {
                    __result = _instance.canBePlacedInWater();
                    return false;
                }
                return true;
            }
            catch (Exception ex) { return handleError($"Object.{nameof(Object.canBePlacedInWater)}", ex, true); }
        }

        internal static bool canBePlacedHerePrefix(Object __instance, ref bool __result, GameLocation l, Vector2 tile)
        {
            try
            {
                if (__instance.ParentSheetIndex == ModEntry.ObjectInfo.Id)
                {
                    __result = _instance.canBePlacedHere(l, tile);
                    return false;
                }
                return true;
            }
            catch (Exception ex) { return handleError($"Object.{nameof(Object.canBePlacedHere)}", ex, true); }
        }

        internal static bool isPlaceablePrefix(Object __instance, ref bool __result)
        {
            try
            {
                if (__instance.ParentSheetIndex == ModEntry.ObjectInfo.Id)
                {
                    __result = _instance.isPlaceable();
                    return false;
                }
                return true;
            }
            catch (Exception ex) { return handleError($"Object.{nameof(Object.isPlaceable)}", ex, true); }
        }

        internal static void layoutRecipesPostfix(CraftingPage __instance)
        {
            try
            {
                var recipeList = __instance.pagesOfCraftingRecipes;
                int pageIndex = -1;
                int recipeIndex = -1;
                foreach (var page in recipeList)
                {
                    bool exit = false;
                    foreach (var recipe in page)
                    {
                        if (recipe.Value.name == "Fish Net")
                        {
                            pageIndex = recipeList.IndexOf(page);
                            recipeIndex = page.ToList().IndexOf(recipe);
                            exit = true;
                            break;
                        }
                    }
                    if (exit)
                        break;
                }
                if (pageIndex != -1 && recipeIndex != -1)
                {
                    var originalDict = recipeList[pageIndex].ToList();
                    originalDict[recipeIndex] = new(rewriteCraftingComponent(originalDict[recipeIndex].Key), originalDict[recipeIndex].Value);
                    __instance.pagesOfCraftingRecipes[pageIndex] = originalDict.ToDictionary(x => x.Key, x => x.Value);
                }
            }
            catch (Exception ex) { handleError("CraftingPage.layoutRecipes", ex, false); }
        }

        internal static bool drawMenuViewPrefix(CraftingRecipe __instance, SpriteBatch b, int x, int y, float layerDepth = 0.88f, bool shadow = true)
        {
            try
            {
                if (__instance.name != "Fish Net")
                    return true;
                Utility.drawWithShadow(b, Fishnet.Texture, new(x, y), Fishnet.SourceRect, Color.White, 0f, Vector2.Zero, 4f, false, layerDepth);
                return false;
            }
            catch (Exception ex) { return handleError($"CraftingRecipe.{nameof(CraftingRecipe.drawMenuView)}", ex, __instance?.name != "Fish Net"); }
        }

        internal static bool drawInMenuPrefix(Object __instance, SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
        {
            try
            {
                if (__instance.ParentSheetIndex != ModEntry.ObjectInfo.Id)
                    return true;
                _instance.Stack = __instance.Stack;
                _instance.Quality = __instance.Quality;
                _instance.drawInMenu(spriteBatch, location, scaleSize, transparency, layerDepth, drawStackNumber, color, drawShadow);
                return false;
            }
            catch (Exception ex) { return handleError($"Object.{nameof(Object.drawInMenu)}", ex, __instance?.ParentSheetIndex != ModEntry.ObjectInfo.Id); }
        }

        internal static bool drawWhenHeldPrefix(Object __instance, SpriteBatch spriteBatch, Vector2 objectPosition, Farmer f)
        {
            try
            {
                if (__instance.ParentSheetIndex != ModEntry.ObjectInfo.Id)
                    return true;
                _instance.drawWhenHeld(spriteBatch, objectPosition, f);
                return false;
            }
            catch (Exception ex) { return handleError($"Object.{nameof(Object.drawWhenHeld)}", ex, __instance?.ParentSheetIndex != ModEntry.ObjectInfo.Id); }
        }

        internal static void drawPlacementBoundsPostfix(Object __instance, SpriteBatch spriteBatch, GameLocation location)
        {
            try
            {
                if (__instance.ParentSheetIndex != ModEntry.ObjectInfo.Id)
                    return;
                int X = (int)Game1.GetPlacementGrabTile().X * 64;
                int Y = (int)Game1.GetPlacementGrabTile().Y * 64;
                Game1.isCheckingNonMousePlacement = !Game1.IsPerformingMousePlacement();
                if (Game1.isCheckingNonMousePlacement)
                {
                    Vector2 nearbyValidPlacementPosition = Utility.GetNearbyValidPlacementPosition(Game1.player, location, _instance, X, Y);
                    X = (int)nearbyValidPlacementPosition.X;
                    Y = (int)nearbyValidPlacementPosition.Y;
                }
                _instance.draw(spriteBatch, X / 64, Y / 64, 0.5f);
            }
            catch (Exception ex) { handleError($"Object.{nameof(Object.drawWhenHeld)}", ex, false); }
        }

        private static ClickableTextureComponent rewriteCraftingComponent(ClickableTextureComponent original)
        {
            return new ClickableTextureComponent(original.name, original.bounds, original.label, original.hoverText, Fishnet.Texture, Fishnet.SourceRect, original.scale, original.drawShadow)
            {
                myID = original.myID,
                rightNeighborID = original.rightNeighborID,
                leftNeighborID = original.leftNeighborID,
                upNeighborID = original.upNeighborID,
                downNeighborID = original.downNeighborID,
                fullyImmutable = original.fullyImmutable,
                region = original.region
            };
        }

        private static bool handleError(string source, Exception ex, bool result)
        {
            IMonitor.Log($"Faild patching {source}", LogLevel.Error);
            IMonitor.Log($"{ex.Message}\n{ex.StackTrace}");
            return result;
        }
    }
}
