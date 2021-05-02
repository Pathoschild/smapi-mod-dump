/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/FishingTrawler
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile.Dimensions;
using xTile.Layers;
using xTile.ObjectModel;
using xTile.Tiles;

namespace FishingTrawler.GameLocations
{
    internal class TrawlerHull : GameLocation
    {
        private List<Location> _hullHoleLocations;
        private const int TRAWLER_TILESHEET_INDEX = 2;
        private const float MINIMUM_WATER_LEVEL_FOR_FLOOR = 5f;
        private const float MINIMUM_WATER_LEVEL_FOR_ITEMS = 20f;

        internal static int waterLevel;
        internal bool areLeaksEnabled;
        internal bool hasWeakHull;

        public TrawlerHull()
        {

        }

        internal TrawlerHull(string mapPath, string name) : base(mapPath, name)
        {
            waterLevel = 0;
            areLeaksEnabled = true;
            hasWeakHull = false;
            _hullHoleLocations = new List<Location>();

            Layer buildingsLayer = this.map.GetLayer("Buildings");
            for (int x = 0; x < buildingsLayer.LayerWidth; x++)
            {
                for (int y = 0; y < buildingsLayer.LayerHeight; y++)
                {
                    Tile tile = buildingsLayer.Tiles[x, y];
                    if (tile is null)
                    {
                        continue;
                    }

                    if (tile.Properties.ContainsKey("CustomAction") && tile.Properties["CustomAction"] == "HullHole")
                    {
                        _hullHoleLocations.Add(new Location(x, y));
                    }
                }
            }
        }

        internal void Reset()
        {
            foreach (Location hullHoleLocation in _hullHoleLocations.Where(loc => IsHoleLeaking(loc.X, loc.Y)))
            {
                AttemptPlugLeak(hullHoleLocation.X, hullHoleLocation.Y, Game1.player, true);
            }

            RecaculateWaterLevel(0);
        }

        protected override void resetLocalState()
        {
            base.resetLocalState();
            base.critters = new List<Critter>();

            AmbientLocationSounds.addSound(new Vector2(7f, 0f), 0);
            AmbientLocationSounds.addSound(new Vector2(13f, 0f), 0);

            if (String.IsNullOrEmpty(this.miniJukeboxTrack.Value))
            {
                Game1.changeMusicTrack("fieldofficeTentMusic"); // Suggested tracks: Snail's Radio, Jumio Kart (Gem), Pirate Theme
            }
        }

        public override void checkForMusic(GameTime time)
        {
            base.checkForMusic(time);
        }

        public override void cleanupBeforePlayerExit()
        {
            //Game1.changeMusicTrack("none");
            base.cleanupBeforePlayerExit();
        }

        public override bool isTileOccupiedForPlacement(Vector2 tileLocation, StardewValley.Object toPlace = null)
        {
            // Preventing player from placing items here
            return true;
        }

        public override void UpdateWhenCurrentLocation(GameTime time)
        {
            Vector2 playerStandingPosition = new Vector2(Game1.player.getStandingX() / 64, Game1.player.getStandingY() / 64);

            if (this.lastTouchActionLocation.Equals(Vector2.Zero) && this.map.GetLayer("FloodWater").Properties["@Opacity"] > 0f)
            {
                string touchActionProperty = this.doesTileHaveProperty((int)playerStandingPosition.X, (int)playerStandingPosition.Y, "CustomTouchAction", "FloodWater");
                this.lastTouchActionLocation = new Vector2(Game1.player.getStandingX() / 64, Game1.player.getStandingY() / 64);
                if (touchActionProperty != null)
                {
                    if (touchActionProperty == "PlaySound")
                    {
                        string soundName = this.doesTileHaveProperty((int)playerStandingPosition.X, (int)playerStandingPosition.Y, "PlaySound", "FloodWater");
                        if (String.IsNullOrEmpty(soundName))
                        {
                            ModEntry.monitor.Log($"Tile at {playerStandingPosition} is missing PlaySound property on FloodWater layer!", LogLevel.Trace);
                            return;
                        }

                        TemporaryAnimatedSprite sprite2 = new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(0, 0, 64, 64), 50f, 9, 1, Game1.player.Position, flicker: false, flipped: false, 0f, 0.025f, Color.White, 1f, 0f, 0f, 0f);
                        sprite2.acceleration = new Vector2(Game1.player.xVelocity, Game1.player.yVelocity);
                        ModEntry.multiplayer.broadcastSprites(this, sprite2);
                        this.playSound(soundName);
                    }
                }
            }

            base.UpdateWhenCurrentLocation(time);
        }

        public override bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
        {
            return base.checkAction(tileLocation, viewport, who);
        }

        public override bool isActionableTile(int xTile, int yTile, Farmer who)
        {
            string actionProperty = this.doesTileHaveProperty(xTile, yTile, "CustomAction", "Buildings");
            if (actionProperty != null && actionProperty == "HullHole")
            {
                if (!IsWithinRangeOfLeak(xTile, yTile, who))
                {
                    Game1.mouseCursorTransparency = 0.5f;
                }

                return true;
            }

            return base.isActionableTile(xTile, yTile, who);
        }

        private bool IsWithinRangeOfLeak(int tileX, int tileY, Farmer who)
        {
            if (who.getTileY() != 4 || !Enumerable.Range(who.getTileX() - 1, 3).Contains(tileX))
            {
                return false;
            }

            return true;
        }

        private int GetRandomBoardTile()
        {
            return 371 + Game1.random.Next(0, 5);
        }

        private bool IsHoleLeaking(int tileX, int tileY)
        {
            Tile hole = this.map.GetLayer("Buildings").Tiles[tileX, tileY];
            if (hole != null && this.doesTileHaveProperty(tileX, tileY, "CustomAction", "Buildings") == "HullHole")
            {
                return bool.Parse(hole.Properties["IsLeaking"]);
            }

            ModEntry.monitor.Log("Called [IsHoleLeaking] on tile that doesn't have IsLeaking property on Buildings layer, returning false!", LogLevel.Trace);
            return false;
        }

        public bool AttemptPlugLeak(int tileX, int tileY, Farmer who, bool forceRepair = false)
        {
            AnimatedTile firstTile = this.map.GetLayer("Buildings").Tiles[tileX, tileY] as AnimatedTile;
            //ModEntry.monitor.Log($"({tileX}, {tileY}) | {isActionableTile(tileX, tileY, who)}", LogLevel.Trace);

            if (firstTile is null)
            {
                return false;
            }

            if (!forceRepair && !(isActionableTile(tileX, tileY, who) && IsWithinRangeOfLeak(tileX, tileY, who)))
            {
                return false;
            }

            if (!firstTile.Properties.ContainsKey("CustomAction") || !firstTile.Properties.ContainsKey("IsLeaking"))
            {
                return false;
            }

            if (firstTile.Properties["CustomAction"] == "HullHole" && bool.Parse(firstTile.Properties["IsLeaking"]) is true)
            {
                // Stop the leaking
                firstTile.Properties["IsLeaking"] = false;

                // Update the tiles
                bool isFirstTile = true;
                for (int y = tileY; y < 5; y++)
                {
                    if (isFirstTile)
                    {
                        // Board up the hole
                        this.setMapTile(tileX, y, GetRandomBoardTile(), "Buildings", null, TRAWLER_TILESHEET_INDEX);

                        // Add the custom properties for tracking
                        this.map.GetLayer("Buildings").Tiles[tileX, tileY].Properties.CopyFrom(firstTile.Properties);

                        this.playSound("crafting");

                        isFirstTile = false;
                        continue;
                    }

                    string targetLayer = y == 4 ? "Back" : "Buildings";

                    AnimatedTile animatedTile = this.map.GetLayer(targetLayer).Tiles[tileX, y] as AnimatedTile;
                    int tileIndex = animatedTile.TileFrames[0].TileIndex - 1;

                    this.setMapTile(tileX, y, tileIndex, targetLayer, null, TRAWLER_TILESHEET_INDEX);
                }
            }

            return true;
        }

        private int[] GetHullLeakTileIndexes(int startingIndex)
        {
            List<int> indexes = new List<int>();
            for (int offset = 0; offset < 6; offset++)
            {
                indexes.Add(startingIndex + offset);
            }

            return indexes.ToArray();
        }

        public bool AttemptCreateHullLeak(int tileX = -1, int tileY = -1)
        {
            //ModEntry.monitor.Log($"[{Game1.player.Name} | MD: {ModEntry.mainDeckhand.Name}] Attempting to create hull leak... [{tileX}, {tileY}]: {IsHoleLeaking(tileX, tileY)}", LogLevel.Debug);

            List<Location> validHoleLocations = _hullHoleLocations.Where(loc => !IsHoleLeaking(loc.X, loc.Y)).ToList();

            if (validHoleLocations.Count() == 0 || !areLeaksEnabled)
            {
                return false;
            }

            // Pick a random valid spot to leak
            Location holeLocation = validHoleLocations.ElementAt(Game1.random.Next(0, validHoleLocations.Count()));
            if (tileX != -1 && tileY != -1)
            {
                if (!_hullHoleLocations.Any(loc => !IsHoleLeaking(loc.X, loc.Y) && loc.X == tileX && loc.Y == tileY))
                {
                    return false;
                }

                holeLocation = _hullHoleLocations.FirstOrDefault(loc => !IsHoleLeaking(loc.X, loc.Y) && loc.X == tileX && loc.Y == tileY);
            }

            // Set the hole as leaking
            Tile firstTile = this.map.GetLayer("Buildings").Tiles[holeLocation.X, holeLocation.Y];
            firstTile.Properties["IsLeaking"] = true;

            bool isFirstTile = true;
            for (int y = holeLocation.Y; y < 5; y++)
            {
                if (isFirstTile)
                {
                    // Break open the hole, copying over the properties
                    this.setAnimatedMapTile(holeLocation.X, holeLocation.Y, holeLocation.Y == 1 ? GetHullLeakTileIndexes(401) : GetHullLeakTileIndexes(377), 60, "Buildings", null, TRAWLER_TILESHEET_INDEX);
                    this.map.GetLayer("Buildings").Tiles[holeLocation.X, holeLocation.Y].Properties.CopyFrom(firstTile.Properties);

                    this.playSound("barrelBreak");

                    isFirstTile = false;
                    continue;
                }

                string targetLayer = y == 4 ? "Back" : "Buildings";

                int[] animatedHullTileIndexes = GetHullLeakTileIndexes(this.map.GetLayer(targetLayer).Tiles[holeLocation.X, y].TileIndex + 1);
                this.setAnimatedMapTile(holeLocation.X, y, animatedHullTileIndexes, 60, targetLayer, null, TRAWLER_TILESHEET_INDEX);
            }

            return true;
        }

        public List<Location> GetAllLeakableLocations()
        {
            return _hullHoleLocations;
        }

        public void RecaculateWaterLevel(int waterLevelOverride = -1)
        {
            // Should be called from ModEntry.OnOneSecondUpdateTicking (at X second interval)
            // Foreach leak, add 1 to the water level

            if (waterLevelOverride > -1)
            {
                waterLevel = waterLevelOverride;
            }
            else
            {
                // For each leak, add 2 to the water level
                ChangeWaterLevel(_hullHoleLocations.Where(loc => IsHoleLeaking(loc.X, loc.Y)).Count() * 2);
            }

            // Using PyTK for these layers and opacity
            this.map.GetLayer("FloodWater").Properties["@Opacity"] = waterLevel > MINIMUM_WATER_LEVEL_FOR_FLOOR ? (waterLevel * 0.01f) + 0.1f : 0f;
            this.map.GetLayer("FloodItems").Properties["@Opacity"] = waterLevel > MINIMUM_WATER_LEVEL_FOR_ITEMS ? 1f : 0f;
        }

        public void ChangeWaterLevel(int change)
        {
            waterLevel += change;

            if (waterLevel < 0)
            {
                waterLevel = 0;
            }
            else if (waterLevel > 100)
            {
                waterLevel = 100;
            }
        }

        public bool HasLeak()
        {
            return _hullHoleLocations.Any(loc => IsHoleLeaking(loc.X, loc.Y));
        }

        public bool AreAllHolesLeaking()
        {
            return _hullHoleLocations.Count(loc => IsHoleLeaking(loc.X, loc.Y)) == _hullHoleLocations.Count();
        }

        public Location GetRandomPatchedHullHole()
        {
            List<Location> validHoleLocations = _hullHoleLocations.Where(loc => !IsHoleLeaking(loc.X, loc.Y)).ToList();

            // Pick a random valid spot to leak
            return _hullHoleLocations.Where(loc => !IsHoleLeaking(loc.X, loc.Y)).ElementAt(Game1.random.Next(0, validHoleLocations.Count()));
        }

        public int GetWaterLevel()
        {
            return waterLevel;
        }

        public bool IsFlooding()
        {
            return this.map.GetLayer("FloodWater").Properties["@Opacity"] > 0f;
        }
    }
}
