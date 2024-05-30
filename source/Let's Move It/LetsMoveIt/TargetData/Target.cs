/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Exblosis/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using SObject = StardewValley.Object;

namespace LetsMoveIt.TargetData
{
    internal partial class Target
    {
        private static ModConfig Config = null!;
        private static IModHelper Helper = null!;
        private static IMonitor Monitor = null!;

        public static string? Name;
        //public static string? Index;
        //public static Guid? Guid;
        public static object? TargetObject;
        public static GameLocation TargetLocation = null!;
        public static Vector2 TilePosition;
        public static Vector2 TileOffset;
        private static readonly HashSet<Vector2> BoundingBoxTile = [];

        /// <summary>Only for set values</summary>
        public static void Init(ModConfig config, IModHelper helper, IMonitor monitor)
        {
            Config = config;
            Helper = helper;
            Monitor = monitor;
        }

        public static void ButtonAction(GameLocation location, Vector2 tile)
        {
            if (Config.ModKey == SButton.None)
                return;
            if (Helper.Input.IsDown(Config.ModKey))
            {
                Get(location, tile, Mod1.GetGlobalMousePosition());
                return;
            }
            if (TargetObject is not null)
            {
                Helper.Input.Suppress(Config.MoveKey);
                bool overwriteTile = Helper.Input.IsDown(Config.OverwriteKey);
                if (IsOccupied(location, tile) && !overwriteTile)
                {
                    Game1.playSound("cancel");
                    return;
                }
                if (Config.CopyMode)
                {
                    CopyTo(location, tile, overwriteTile);
                }
                else
                {
                    MoveTo(location, tile, overwriteTile);
                }
            }
        }

        private static bool IsOccupied(GameLocation location, Vector2 tile)
        {
            bool occupied = false;
            if (!location.isTilePassable(tile) || !location.isTileOnMap(tile) || location.isTileHoeDirt(tile) || location.isCropAtTile((int)tile.X, (int)tile.Y) || location.IsTileBlockedBy(tile, ignorePassables: CollisionMask.All))
            {
                if (TargetObject is Crop && location.isTileHoeDirt(tile))
                {
                    occupied = false;
                }
                else if (TargetObject is SObject sObject && sObject.IsTapper())
                {
                    if (location.terrainFeatures.TryGetValue(tile, out var tf) && tf is Tree)
                    {
                        occupied = false;
                    }
                    else
                    {
                        occupied = true;
                    }
                }
                else
                {
                    occupied = true;
                }
            }
            if (BoundingBoxTile.Count != 0)
            {
                BoundingBoxTile.ToList().ForEach(t =>
                {
                    if (!location.isTilePassable(t) || !location.isTileOnMap(t) || location.isTileHoeDirt(t) || location.isCropAtTile((int)t.X, (int)t.Y) || location.IsTileBlockedBy(t, ignorePassables: CollisionMask.All))
                    {
                        if (BoundingBoxTile.Count == 1)
                        {
                            if (TargetObject is Bush bush && bush.size.Value == 3 && location.getObjectAtTile((int)tile.X, (int)tile.Y) is IndoorPot)
                            {
                                occupied = false;
                            }
                            else
                            {
                                occupied = true;
                            }
                        }
                        else
                        {
                            occupied = true;
                        }
                    }
                });
            }
            return occupied;
        }

        public static void PlaySound()
        {
            if (!string.IsNullOrEmpty(Config.Sound))
                Game1.playSound(Config.Sound);
        }
    }
}
