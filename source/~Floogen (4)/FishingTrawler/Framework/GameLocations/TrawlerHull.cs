/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/FishingTrawler
**
*************************************************/

using FishingTrawler.Framework.GameLocations;
using FishingTrawler.Framework.Objects.Items.Resources;
using FishingTrawler.Messages;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using xTile.Dimensions;
using xTile.Layers;
using xTile.Tiles;

namespace FishingTrawler.GameLocations
{
    internal class TrawlerHull : TrawlerLocation
    {
        private List<Location> _hullHoleLocations;
        private List<Location> _coalLocations;
        private List<Location> _engineRefillLocations;

        private const int TRAWLER_TILESHEET_INDEX = 3;
        private const float MINIMUM_WATER_LEVEL_FOR_FLOOR = 5f;
        private const float MINIMUM_WATER_LEVEL_FOR_ITEMS = 20f;
        private const string FLOOD_WATER_LAYER = "FloodWater";
        private const string FLOOD_ITEMS_LAYER = "FloodItems";
        private const string WATER_SPLASH_LAYER = "WaterSplash";

        private static int _waterLevel;
        private static int _fuelLevel;

        internal bool areLeaksEnabled;
        internal bool hasWeakHull;
        internal int fuelConsumptionIncrement;

        public TrawlerHull()
        {

        }

        internal TrawlerHull(string mapPath, string name) : base(mapPath, name)
        {
            _waterLevel = 0;
            areLeaksEnabled = true;
            hasWeakHull = false;
            fuelConsumptionIncrement = -10;

            _hullHoleLocations = new List<Location>();
            _coalLocations = new List<Location>();
            _engineRefillLocations = new List<Location>();

            Layer buildingsLayer = map.GetLayer("Buildings");
            for (int x = 0; x < buildingsLayer.LayerWidth; x++)
            {
                for (int y = 0; y < buildingsLayer.LayerHeight; y++)
                {
                    Tile tile = buildingsLayer.Tiles[x, y];
                    if (tile is null)
                    {
                        continue;
                    }

                    if (tile.Properties.ContainsKey("CustomAction"))
                    {
                        if (tile.Properties["CustomAction"] == "HullHole")
                        {
                            _hullHoleLocations.Add(new Location(x, y));
                        }
                        else if (tile.Properties["CustomAction"] == "GetCoal")
                        {
                            _coalLocations.Add(new Location(x, y));
                        }
                        else if (tile.Properties["CustomAction"] == "RefillEngine")
                        {
                            _engineRefillLocations.Add(new Location(x, y));
                        }
                    }
                }
            }
        }

        internal override void Reset()
        {
            // Set the fuel level back to default
            _fuelLevel = 100;

            // Fix all leaks and set the flood level to 0
            foreach (Location hullHoleLocation in _hullHoleLocations.Where(loc => IsHoleLeaking(loc.X, loc.Y)))
            {
                AttemptPlugLeak(hullHoleLocation.X, hullHoleLocation.Y, Game1.player, true);
            }

            RecalculateWaterLevel(0);
        }

        protected override void resetLocalState()
        {
            base.resetLocalState();

            // Add water / brook sounds
            AmbientLocationSounds.addSound(new Vector2(7f, 0f), AmbientLocationSounds.sound_babblingBrook);
            AmbientLocationSounds.addSound(new Vector2(13f, 0f), AmbientLocationSounds.sound_babblingBrook);

            // Add engine shake and sound
            if (GetFuelLevel() > 0)
            {
                AmbientLocationSounds.addSound(new Vector2(1.5f, 5.5f), AmbientLocationSounds.sound_engine);
                base.temporarySprites.Add(new TemporaryAnimatedSprite(Path.Combine(FishingTrawler.assetManager.assetFolderPath, "Maps", "TrawlerHull.png"), new Microsoft.Xna.Framework.Rectangle(32, 192, 16, 16), 7000 - Game1.gameTimeInterval, 1, 1, new Vector2(1.45f, 5.45f) * 64f, flicker: false, flipped: false, 0.5188f, 0f, Color.White, 4f, 0f, 0f, 0f)
                {
                    shakeIntensity = 1f
                });
            }
        }

        public override void UpdateWhenCurrentLocation(GameTime time)
        {
            // Handle playing splash sound and animation if the hull is flooded enough
            Vector2 playerStandingPosition = new Vector2(Game1.player.getStandingX() / 64, Game1.player.getStandingY() / 64);
            if (lastTouchActionLocation.Equals(Vector2.Zero) && map.GetLayer(FLOOD_WATER_LAYER).Properties["@Opacity"] > 0f)
            {
                string touchActionProperty = doesTileHaveProperty((int)playerStandingPosition.X, (int)playerStandingPosition.Y, "CustomTouchAction", FLOOD_WATER_LAYER);
                lastTouchActionLocation = new Vector2(Game1.player.getStandingX() / 64, Game1.player.getStandingY() / 64);

                if (touchActionProperty != null)
                {
                    if (touchActionProperty == "PlaySound")
                    {
                        string soundName = doesTileHaveProperty((int)playerStandingPosition.X, (int)playerStandingPosition.Y, "PlaySound", FLOOD_WATER_LAYER);
                        if (string.IsNullOrEmpty(soundName))
                        {
                            FishingTrawler.monitor.LogOnce($"Tile at {playerStandingPosition} is missing PlaySound property on FloodWater layer!", LogLevel.Trace);
                            return;
                        }

                        TemporaryAnimatedSprite sprite2 = new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(0, 0, 64, 64), 50f, 9, 1, Game1.player.Position, flicker: false, flipped: false, 0f, 0.025f, Color.White, 1f, 0f, 0f, 0f);
                        sprite2.acceleration = new Vector2(Game1.player.xVelocity, Game1.player.yVelocity);
                        FishingTrawler.multiplayer.broadcastSprites(this, sprite2);
                        playSound(soundName);
                    }
                }
            }

            // Add engine shake
            if (GetFuelLevel() > 0)
            {
                if (base.temporarySprites.Any(s => s.initialPosition == new Vector2(1.45f, 5.45f)) is false)
                {
                    base.temporarySprites.Add(new TemporaryAnimatedSprite(Path.Combine(FishingTrawler.assetManager.assetFolderPath, "Maps", "TrawlerHull.png"), new Microsoft.Xna.Framework.Rectangle(32, 192, 16, 16), 7000 - Game1.gameTimeInterval, 1, 1, new Vector2(1.45f, 5.45f) * 64f, flicker: false, flipped: false, 0.5188f, 0f, Color.White, 4f, 0f, 0f, 0f)
                    {
                        shakeIntensity = 1f
                    });

                    AmbientLocationSounds.addSound(new Vector2(1.5f, 5.5f), AmbientLocationSounds.sound_engine);
                }
                setMapTileIndex(2, 5, -1, "AlwaysFront", TRAWLER_TILESHEET_INDEX);
            }
            else
            {
                AmbientLocationSounds.removeSound(new Vector2(1.5f, 5.5f));

                setAnimatedMapTile(2, 5, new int[] { 24, 25, 26, 27, 28 }, 90, "AlwaysFront", null, TRAWLER_TILESHEET_INDEX);
            }

            base.UpdateWhenCurrentLocation(time);
        }

        public override bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
        {
            string actionProperty = doesTileHaveProperty(tileLocation.X, tileLocation.Y, "CustomAction", "Buildings");

            if (String.IsNullOrEmpty(actionProperty) is false)
            {
                // Check if the tile is a coal gathering spot
                if (actionProperty == "GetCoal" && base.IsWithinRangeOfTile(tileLocation.X, tileLocation.Y, 1, 1, who) is true)
                {
                    // Give player coal, if they don't already have max stack
                    var currentCoalItem = who.Items.FirstOrDefault(i => CoalClump.IsValid(i));
                    if (currentCoalItem is null)
                    {
                        currentCoalItem = CoalClump.CreateInstance(1);
                        who.addItemToInventory(currentCoalItem);

                        base.playSound("coin");
                    }
                    else if (CoalClump.IncrementSize(currentCoalItem, 1))
                    {
                        base.playSound("coin");
                    }
                    else
                    {
                        base.playSound("cancel");
                    }
                }
                else if (actionProperty == "RefillEngine" && base.IsWithinRangeOfTile(tileLocation.X, tileLocation.Y, 1, 1, who) is true)
                {
                    // Attempt to get the player's fuel stack
                    var fuelStack = who.Items.FirstOrDefault(CoalClump.IsValid);
                    if (fuelStack is not null)
                    {
                        int fuelSize = CoalClump.GetSize(fuelStack);

                        if (GetFuelLevel() == 100)
                        {
                            Game1.addHUDMessage(new HUDMessage(FishingTrawler.i18n.Get("game_message.coal_clump.engine_full"), 3) { timeLeft = 1000f });
                            return true;
                        }

                        int restoreAmount = (5 * fuelSize) + (fuelSize == 3 ? 5 : 0);
                        AdjustFuelLevel(restoreAmount);
                        FishingTrawler.SyncTrawler(SyncType.Fuel, restoreAmount, FishingTrawler.GetFarmersOnTrawler());

                        who.removeItemFromInventory(fuelStack);

                        base.playSound("furnace");
                    }
                    else
                    {
                        Game1.addHUDMessage(new HUDMessage(FishingTrawler.i18n.Get("game_message.coal_clump.must_be_holding"), 3) { timeLeft = 1000f });
                    }
                }

                return true;
            }

            // Check to see if player is standing in front of stairs before using  
            if (String.IsNullOrEmpty(doesTileHaveProperty(tileLocation.X, tileLocation.Y, "Action", "Buildings")) is false)
            {
                if (who.getTileX() != 9 || (who.getTileY() != 6 && who.getTileY() != 5))
                {
                    return false;
                }
            }

            return base.checkAction(tileLocation, viewport, who);
        }

        public override bool isActionableTile(int xTile, int yTile, Farmer who)
        {
            string actionProperty = doesTileHaveProperty(xTile, yTile, "CustomAction", "Buildings");

            // Check if the tile is a leak
            if (String.IsNullOrEmpty(actionProperty) is false && actionProperty == "HullHole")
            {
                if (!IsWithinRangeOfLeak(xTile, yTile, who))
                {
                    Game1.mouseCursorTransparency = 0.5f;
                }

                return true;
            }

            // Check if the tile is a coal gathering spot
            if (String.IsNullOrEmpty(actionProperty) is false && actionProperty == "GetCoal")
            {
                if (base.IsWithinRangeOfTile(xTile, yTile, 1, 1, who) is false)
                {
                    Game1.mouseCursorTransparency = 0.5f;
                }

                return true;
            }

            // Check to see if player is standing in front of stairs before using            
            if (String.IsNullOrEmpty(doesTileHaveProperty(xTile, yTile, "Action", "Buildings")) is false)
            {
                if (who.getTileX() != 9 || (who.getTileY() != 6 && who.getTileY() != 5))
                {
                    Game1.mouseCursorTransparency = 0.5f;
                }

                return true;
            }

            return base.isActionableTile(xTile, yTile, who);
        }

        #region Fuel event methods
        public void SetFuelLevel(int amount)
        {
            _fuelLevel = amount;

            if (_fuelLevel < 0)
            {
                _fuelLevel = 0;
            }
            else if (_fuelLevel > 100)
            {
                _fuelLevel = 100;
            }
        }

        public void AdjustFuelLevel(int amount)
        {
            SetFuelLevel(_fuelLevel + amount);
        }

        public int GetFuelLevel()
        {
            return _fuelLevel;
        }

        public bool IsEngineFailing()
        {
            return _fuelLevel <= 50;
        }
        #endregion

        #region Boat leak event methods
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
            Tile hole = map.GetLayer("Buildings").Tiles[tileX, tileY];
            if (hole != null && doesTileHaveProperty(tileX, tileY, "CustomAction", "Buildings") == "HullHole")
            {
                return bool.Parse(hole.Properties["IsLeaking"]);
            }

            FishingTrawler.monitor.LogOnce("Called [IsHoleLeaking] on tile that doesn't have IsLeaking property on Buildings layer, returning false!", LogLevel.Trace);
            return false;
        }

        public bool AttemptPlugLeak(int tileX, int tileY, Farmer who, bool forceRepair = false)
        {
            AnimatedTile firstTile = map.GetLayer("Buildings").Tiles[tileX, tileY] as AnimatedTile;
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
                base.AddRepairedTile(tileX, tileY);

                // Update the tiles
                bool isFirstTile = true;
                for (int y = tileY; y < 5; y++)
                {
                    if (isFirstTile)
                    {
                        // Board up the hole
                        setMapTile(tileX, y, GetRandomBoardTile(), "Buildings", null, TRAWLER_TILESHEET_INDEX);

                        // Add the custom properties for tracking
                        map.GetLayer("Buildings").Tiles[tileX, tileY].Properties.CopyFrom(firstTile.Properties);

                        playSound("crafting");

                        isFirstTile = false;
                        continue;
                    }

                    string targetLayer = y == 4 ? WATER_SPLASH_LAYER : "Buildings";

                    AnimatedTile animatedTile = map.GetLayer(targetLayer).Tiles[tileX, y] as AnimatedTile;
                    int tileIndex = animatedTile.TileFrames[0].TileIndex - 1;

                    setMapTile(tileX, y, tileIndex, targetLayer, null, TRAWLER_TILESHEET_INDEX);
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
            Tile firstTile = map.GetLayer("Buildings").Tiles[holeLocation.X, holeLocation.Y];
            firstTile.Properties["IsLeaking"] = true;

            bool isFirstTile = true;
            for (int y = holeLocation.Y; y < 5; y++)
            {
                if (isFirstTile)
                {
                    // Break open the hole, copying over the properties
                    setAnimatedMapTile(holeLocation.X, holeLocation.Y, holeLocation.Y == 1 ? GetHullLeakTileIndexes(401) : GetHullLeakTileIndexes(377), 60, "Buildings", null, TRAWLER_TILESHEET_INDEX);
                    map.GetLayer("Buildings").Tiles[holeLocation.X, holeLocation.Y].Properties.CopyFrom(firstTile.Properties);

                    playSound("barrelBreak");

                    isFirstTile = false;
                    continue;
                }

                string targetLayer = y == 4 ? WATER_SPLASH_LAYER : "Buildings";

                int[] animatedHullTileIndexes = GetHullLeakTileIndexes(map.GetLayer(targetLayer).Tiles[holeLocation.X, y].TileIndex + 1);
                setAnimatedMapTile(holeLocation.X, y, animatedHullTileIndexes, 60, targetLayer, null, TRAWLER_TILESHEET_INDEX);
            }

            return true;
        }

        public List<Location> GetAllLeakableLocations()
        {
            return _hullHoleLocations;
        }

        public void RecalculateWaterLevel(int waterLevelOverride = -1)
        {
            // Foreach leak, add 1 to the water level

            if (waterLevelOverride > -1)
            {
                _waterLevel = waterLevelOverride;
            }
            else
            {
                // For each leak, add 1.5 (rounded down) to the water level
                ChangeWaterLevel((int)(_hullHoleLocations.Where(loc => IsHoleLeaking(loc.X, loc.Y)).Count() * 1.5));
            }

            // Using PyTK for these layers and opacity
            map.GetLayer(FLOOD_WATER_LAYER).Properties["@Opacity"] = _waterLevel > MINIMUM_WATER_LEVEL_FOR_FLOOR ? _waterLevel * 0.01f + 0.1f : 0f;
            map.GetLayer(FLOOD_ITEMS_LAYER).Properties["@Opacity"] = _waterLevel > MINIMUM_WATER_LEVEL_FOR_ITEMS ? 1f : 0f;
        }

        public void ChangeWaterLevel(int change)
        {
            _waterLevel += change;

            if (_waterLevel < 0)
            {
                _waterLevel = 0;
            }
            else if (_waterLevel > 100)
            {
                _waterLevel = 100;
            }
        }

        public bool HasLeak()
        {
            return _hullHoleLocations.Any(loc => IsHoleLeaking(loc.X, loc.Y));
        }

        public bool AreAllHolesLeaking()
        {
            return _hullHoleLocations.Count(loc => IsHoleLeaking(loc.X, loc.Y)) == GetLeakingHolesCount();
        }

        public int GetLeakingHolesCount()
        {
            return _hullHoleLocations.Count(loc => IsHoleLeaking(loc.X, loc.Y));
        }

        public int GetPatchedHolesCount()
        {
            return _hullHoleLocations.Where(loc => IsHoleLeaking(loc.X, loc.Y) is false).Count();
        }

        public Location? GetRandomPatchedHullHole()
        {
            List<Location> validHoleLocations = _hullHoleLocations.Where(loc => !IsHoleLeaking(loc.X, loc.Y)).ToList();
            if (validHoleLocations.Count() == 0)
            {
                return null;
            }

            // Pick a random valid spot to leak
            return _hullHoleLocations.Where(loc => !IsHoleLeaking(loc.X, loc.Y)).ElementAt(Game1.random.Next(0, validHoleLocations.Count()));
        }

        public int GetWaterLevel()
        {
            return _waterLevel;
        }

        public bool IsFlooding()
        {
            return map.GetLayer(FLOOD_WATER_LAYER).Properties["@Opacity"] > 0f;
        }

        internal bool HasFlooded()
        {
            return _waterLevel >= 100;
        }
        #endregion
    }
}
