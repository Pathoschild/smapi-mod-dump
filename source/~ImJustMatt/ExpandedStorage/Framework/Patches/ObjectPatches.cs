/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using ExpandedStorage.Framework.Controllers;
using HarmonyLib;
using XSAutomate.Common.Patches;
using ExpandedStorage.Framework.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;

// ReSharper disable InconsistentNaming

namespace ExpandedStorage.Framework.Patches
{
    internal class ObjectPatches : BasePatch<ExpandedStorage>
    {
        private static readonly HashSet<string> ExcludeModDataKeys = new();

        public ObjectPatches(IMod mod, Harmony harmony) : base(mod, harmony)
        {
            harmony.Patch(
                AccessTools.Method(typeof(Object), nameof(Object.checkForAction)),
                new HarmonyMethod(GetType(), nameof(CheckForActionPrefix))
            );

            harmony.Patch(
                AccessTools.Method(typeof(Object), nameof(Object.draw), new[] {typeof(SpriteBatch), typeof(int), typeof(int), typeof(float)}),
                new HarmonyMethod(GetType(), nameof(DrawPrefix))
            );

            harmony.Patch(
                AccessTools.Method(typeof(Object), nameof(Object.drawWhenHeld)),
                new HarmonyMethod(GetType(), nameof(DrawWhenHeldPrefix))
            );

            harmony.Patch(
                AccessTools.Method(typeof(Object), nameof(Object.drawPlacementBounds)),
                new HarmonyMethod(GetType(), nameof(DrawPlacementBoundsPrefix))
            );

            harmony.Patch(
                AccessTools.Method(typeof(Object), nameof(Object.getOne)),
                postfix: new HarmonyMethod(GetType(), nameof(GetOnePostfix))
            );

            harmony.Patch(
                AccessTools.Method(typeof(Object), nameof(Object.maximumStackSize)),
                postfix: new HarmonyMethod(GetType(), nameof(MaximumStackSizePostfix))
            );

            harmony.Patch(
                AccessTools.Method(typeof(Object), nameof(Object.placementAction)),
                new HarmonyMethod(GetType(), nameof(PlacementActionPrefix))
            );
        }

        internal static void AddExclusion(string modDataKey)
        {
            ExcludeModDataKeys.Add(modDataKey);
        }

        /// <summary>Trigger primary chest check for action</summary>
        private static bool CheckForActionPrefix(Object __instance, ref bool __result, Farmer who, bool justCheckingForActivity)
        {
            if (justCheckingForActivity || !Game1.didPlayerJustRightClick(true)) return true;
            if (!Mod.AssetController.TryGetStorage(__instance, out _)) return true;
            var x = __instance.modData.TryGetValue("furyx639.ExpandedStorage/X", out var xStr) ? int.Parse(xStr) : 0;
            var y = __instance.modData.TryGetValue("furyx639.ExpandedStorage/Y", out var yStr) ? int.Parse(yStr) : 0;
            if (!Game1.currentLocation.Objects.TryGetValue(new Vector2(x, y), out var obj) || obj is not Chest chest) return true;
            __result = chest.checkForAction(who);
            return false;
        }

        /// <summary>Do not draw object extensions of bigger expanded storages.</summary>
        private static bool DrawPrefix(Object __instance, SpriteBatch spriteBatch, int x, int y, float alpha)
        {
            if (__instance.modData.Keys.Any(ExcludeModDataKeys.Contains)
                || !Mod.AssetController.TryGetStorage(__instance, out var storage)) return true;
            return storage.StorageSprite is not { } spriteSheet
                   || spriteSheet.TileWidth <= 1 && spriteSheet.TileHeight <= 1
                   || (int) __instance.TileLocation.X == x && (int) __instance.TileLocation.Y == y;
        }

        private static bool DrawWhenHeldPrefix(Object __instance, SpriteBatch spriteBatch, Vector2 objectPosition, Farmer f)
        {
            if (__instance.modData.Keys.Any(ExcludeModDataKeys.Contains)
                || !Mod.AssetController.TryGetStorage(__instance, out var storage)
                || __instance is not Chest chest) return true;

            if (storage.StorageSprite is {Texture: { }} spriteSheet)
            {
                objectPosition.X -= spriteSheet.Width * 2f - 32;
                objectPosition.Y -= spriteSheet.Height * 2f - 64 - (storage.IsPlaceable ? 0 : spriteSheet.Height / 2f);
            }
            else if (!storage.IsPlaceable)
            {
                objectPosition.Y += 16;
            }

            var currentFrame = Mod.Helper.Reflection.GetField<int>(chest, "_shippingBinFrameCounter").GetValue();
            chest.Draw(currentFrame, storage, spriteBatch, objectPosition, Vector2.Zero);
            return false;
        }

        private static bool DrawPlacementBoundsPrefix(Object __instance, SpriteBatch spriteBatch, GameLocation location)
        {
            if (__instance.modData.Keys.Any(ExcludeModDataKeys.Contains)) return true;
            if (Mod.AssetController.TryGetStorage(__instance, out var storage) && !storage.IsPlaceable) return false;
            if (storage?.StorageSprite is not {Texture: { }} spriteSheet) return true;

            var tile = 64 * Game1.GetPlacementGrabTile();
            var x = (int) tile.X;
            var y = (int) tile.Y;

            Game1.isCheckingNonMousePlacement = !Game1.IsPerformingMousePlacement();
            if (Game1.isCheckingNonMousePlacement)
            {
                var pos = Utility.GetNearbyValidPlacementPosition(Game1.player, location, __instance, x, y);
                x = (int) pos.X;
                y = (int) pos.Y;
            }

            var canPlaceHere = Utility.playerCanPlaceItemHere(location, __instance, x, y, Game1.player)
                               || Utility.isThereAnObjectHereWhichAcceptsThisItem(location, __instance, x, y)
                               && Utility.withinRadiusOfPlayer(x, y, 1, Game1.player);

            Game1.isCheckingNonMousePlacement = false;

            spriteSheet.ForEachPos(x / 64, y / 64, delegate(Vector2 pos)
            {
                spriteBatch.Draw(Game1.mouseCursors,
                    pos * 64 - new Vector2(Game1.viewport.X, Game1.viewport.Y),
                    new Rectangle(canPlaceHere ? 194 : 210, 388, 16, 16),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    4f,
                    SpriteEffects.None,
                    0.01f);
            });

            if (__instance is Chest chest)
            {
                var globalPosition = new Vector2((int) (x / 64f), (int) (y / 64f - storage.Depth / 16f - 1f));
                var currentFrame = Mod.Helper.Reflection.GetField<int>(chest, "_shippingBinFrameCounter").GetValue();
                chest.Draw(currentFrame, storage, spriteBatch, Game1.GlobalToLocal(Game1.viewport, globalPosition * 64), Vector2.Zero, 0.5f);
            }
            else
                __instance.draw(spriteBatch, x / 64, y / 64, 0.5f);

            return false;
        }

        private static void GetOnePostfix(Object __instance, ref Item __result)
        {
            if (Mod.AssetController.TryGetStorage(__instance, out var storage))
            {
                __result = __instance.ToChest(storage);
            }
        }

        /// <summary>Disallow stacking carried chests.</summary>
        private static void MaximumStackSizePostfix(Object __instance, ref int __result)
        {
            if (!__instance.modData.Keys.Any(ExcludeModDataKeys.Contains)
                && __instance is Chest
                || Mod.AssetController.TryGetStorage(__instance, out var storage)
                && (storage.Config.Option("CarryChest", true) != StorageConfigController.Choice.Enable
                    || storage.Config.Option("AccessCarried", true) != StorageConfigController.Choice.Enable))
            {
                __result = -1;
            }
        }

        private static bool PlacementActionPrefix(Object __instance, ref bool __result, GameLocation location, int x, int y, Farmer who)
        {
            if (__instance.modData.Keys.Any(ExcludeModDataKeys.Contains)
                || !Mod.AssetController.TryGetStorage(__instance, out var storage)) return true;

            if (!storage.IsPlaceable)
            {
                __result = false;
                return false;
            }

            // Verify pos is not already occupied
            var pos = new Vector2(x, y) / 64f;
            pos.X = (int) pos.X;
            pos.Y = (int) pos.Y;
            if (location.objects.ContainsKey(pos) || location is MineShaft || location is VolcanoDungeon)
            {
                Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.13053"));
                __result = false;
                return false;
            }

            if (storage.IsFridge)
            {
                if (location is not FarmHouse && location is not IslandFarmHouse)
                {
                    Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.13053"));
                    __result = false;
                    return false;
                }

                if (location is FarmHouse {upgradeLevel: < 1})
                {
                    Game1.showRedMessage(Game1.content.LoadString("Strings\\UI:MiniFridge_NoKitchen"));
                    __result = false;
                    return false;
                }
            }

            // Get instance of object to place
            var chest = __instance.ToChest(storage);
            chest.shakeTimer = 50;
            chest.TileLocation = pos;

            // Place object at location
            location.objects.Add(pos, chest);
            location.localSound(storage.PlaceSound);

            __result = true;
            return false;
        }
    }
}