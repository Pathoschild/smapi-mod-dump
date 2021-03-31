/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Harmony;
using ImJustMatt.Common.Patches;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;

namespace ImJustMatt.ExpandedStorage.Framework.Patches
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal class UtilityPatches : BasePatch<ExpandedStorage>
    {
        public UtilityPatches(IMod mod, HarmonyInstance harmony) : base(mod, harmony)
        {
            harmony.Patch(
                AccessTools.Method(typeof(Utility), nameof(Utility.playerCanPlaceItemHere)),
                postfix: new HarmonyMethod(GetType(), nameof(PlayerCanPlaceItemHerePostfix))
            );
        }

        private static void PlayerCanPlaceItemHerePostfix(ref bool __result, GameLocation location, Item item, int x, int y, Farmer f)
        {
            if (!Mod.AssetController.TryGetStorage(item, out var storage) || storage.StorageSprite is not {Texture: { }} spriteSheet)
                return;

            x = 64 * (x / 64);
            y = 64 * (y / 64);

            if (Utility.isPlacementForbiddenHere(location) || item == null || Game1.eventUp || f.bathingClothes.Value || f.onBridge.Value)
            {
                __result = false;
                return;
            }

            // Is Within Tile With Leeway
            if (!Utility.withinRadiusOfPlayer(x, y, Math.Max(spriteSheet.TileWidth, spriteSheet.TileHeight), f))
            {
                __result = false;
                return;
            }

            // Position intersects with farmer
            var rect = new Rectangle(x, y, spriteSheet.TileWidth * 64, spriteSheet.TileHeight * 64);
            if (location.farmers.Any(farmer => farmer.GetBoundingBox().Intersects(rect)))
            {
                __result = false;
                return;
            }

            // Is Close Enough to Farmer
            rect.Inflate(32, 32);
            if (!rect.Intersects(f.GetBoundingBox()))
            {
                __result = false;
                return;
            }

            for (var i = 0; i < spriteSheet.TileWidth; i++)
            {
                for (var j = 0; j < spriteSheet.TileHeight; j++)
                {
                    var tileLocation = new Vector2(x / 64 + i, y / 64 + j);
                    if (item.canBePlacedHere(location, tileLocation)
                        && location.getObjectAtTile((int) tileLocation.X, (int) tileLocation.Y) == null
                        && location.isTilePlaceable(tileLocation, item))
                        continue;

                    // Item cannot be placed here
                    __result = false;
                    return;
                }
            }

            __result = true;
        }
    }
}