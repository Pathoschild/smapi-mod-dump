/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/FishingTrawler
**
*************************************************/

using FishingTrawler.Objects.Rewards;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile.Dimensions;
using xTile.Layers;
using xTile.Tiles;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace FishingTrawler.GameLocations
{
    internal class TrawlerSurface : GameLocation
    {
        // Source rectangles for drawing fluff
        private Rectangle _smallCloudSource = new Rectangle(0, 64, 48, 48);
        private Rectangle _mediumCloudSource = new Rectangle(0, 160, 96, 48);
        private Rectangle _largeCloudSource = new Rectangle(0, 208, 144, 96);
        private Rectangle _longCloudSource = new Rectangle(0, 112, 96, 48);

        private Rectangle _rockPillarSource = new Rectangle(0, 0, 40, 53);
        private Rectangle _rockWithTreeSource = new Rectangle(48, 16, 96, 96);

        // Mini-game stat related
        internal int fishCaughtQuantity;
        internal int fishCaughtMultiplier;
        private List<Location> _netRipLocations;

        // Speed related offsets
        private float _slowOffset = -5f;
        private float _fastOffset = -7f;

        private float _nextSmoke = 0f;
        private const int CLOUD_ID = 1010101;
        private const int GROUND_ID = 2020202;
        private const int FLAGS_TILESHEET_INDEX = 2;
        private const int TRAWLER_TILESHEET_INDEX = 3;

        public TrawlerSurface()
        {

        }

        internal TrawlerSurface(string mapPath, string name) : base(mapPath, name)
        {
            base.ignoreDebrisWeather.Value = true;
            base.critters = new List<Critter>();

            fishCaughtQuantity = 0;
            fishCaughtMultiplier = 1;
            _netRipLocations = new List<Location>();

            Layer alwaysFrontLayer = this.map.GetLayer("AlwaysFront");
            for (int x = 0; x < alwaysFrontLayer.LayerWidth; x++)
            {
                for (int y = 0; y < alwaysFrontLayer.LayerHeight; y++)
                {
                    Tile tile = alwaysFrontLayer.Tiles[x, y];
                    if (tile is null)
                    {
                        continue;
                    }

                    if (tile.Properties.ContainsKey("CustomAction") && tile.Properties["CustomAction"] == "RippedNet")
                    {
                        _netRipLocations.Add(new Location(x, y));
                    }
                }
            }
        }

        internal void Reset()
        {
            foreach (Location netRippedLocation in _netRipLocations.Where(loc => IsNetRipped(loc.X, loc.Y)))
            {
                AttemptFixNet(netRippedLocation.X, netRippedLocation.Y, Game1.player, true);
            }

            UpdateFishCaught(fishCaughtOverride: 0);

            // Clear out the TemporaryAnimatedSprite we preserved
            base.resetLocalState();
        }

        protected override void resetLocalState()
        {
            base.critters = new List<Critter>();

            List<TemporaryAnimatedSprite> preservedSprites = new List<TemporaryAnimatedSprite>();
            foreach (TemporaryAnimatedSprite sprite in base.temporarySprites)
            {
                preservedSprites.Add(sprite);
            }

            base.resetLocalState();
            base.temporarySprites = preservedSprites;

            AmbientLocationSounds.addSound(new Vector2(44f, 23f), 2);
        }

        public override void checkForMusic(GameTime time)
        {
            if (Game1.random.NextDouble() < 0.006 && !(Game1.isSnowing || Game1.isRaining))
            {
                base.localSound("seagulls");
            }

            if (String.IsNullOrEmpty(this.miniJukeboxTrack.Value) && Game1.getMusicTrackName() != "fieldofficeTentMusic")
            {
                Game1.changeMusicTrack("fieldofficeTentMusic"); // Suggested tracks: Snail's Radio, Jumio Kart (Gem), Pirate Theme
            }
        }

        public override void cleanupBeforePlayerExit()
        {
            base.cleanupBeforePlayerExit();
        }

        public override void tryToAddCritters(bool onlyIfOnScreen = false)
        {
            // Overidden to hide birds, but also hides vanilla clouds (which works in our favor)
        }

        internal Rectangle PickRandomCloud()
        {
            switch (Game1.random.Next(1, 5))
            {
                case 1:
                    return _smallCloudSource;
                case 2:
                    return _mediumCloudSource;
                case 3:
                    return _largeCloudSource;
                default:
                    return _longCloudSource;
            }
        }

        internal Rectangle PickRandomDecoration()
        {
            switch (Game1.random.Next(1, 3))
            {
                case 1:
                    return _rockPillarSource;
                default:
                    return _rockWithTreeSource;
            }
        }

        internal Vector2 PickSpawnPosition(bool isCloud)
        {
            int reservedLowerYPosition = 15;
            int reservedUpperYPosition = 33;

            int reservedLowerXPosition = 70;
            int reservedUpperXPosition = 80;

            if (isCloud)
            {
                return new Vector2(Game1.random.Next(reservedLowerXPosition, reservedUpperXPosition), Game1.random.Next(13, 40)) * 64f;
            }

            if (Game1.random.Next(0, 2) == 0)
            {
                return new Vector2(Game1.random.Next(reservedLowerXPosition, reservedUpperXPosition), Game1.random.Next(13, reservedLowerYPosition)) * 64f;
            }
            else
            {
                return new Vector2(Game1.random.Next(reservedLowerXPosition, reservedUpperXPosition), Game1.random.Next(reservedUpperYPosition, 40)) * 64f;
            }
        }

        public override void UpdateWhenCurrentLocation(GameTime time)
        {
            base.UpdateWhenCurrentLocation(time);

            if (this._nextSmoke > 0f)
            {
                this._nextSmoke -= (float)time.ElapsedGameTime.TotalSeconds;
            }
            else
            {
                Vector2 smokePosition = new Vector2(43.5f, 19.5f) * 64f;

                TemporaryAnimatedSprite sprite = new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(0, 1600, 64, 128), 200f, 9, 1, smokePosition, flicker: false, flipped: false, 1f, 0.015f, Color.Gray, 1f, 0.025f, 0f, 0f);
                sprite.acceleration = new Vector2(-0.30f, -0.15f);
                base.temporarySprites.Add(sprite);

                this._nextSmoke = 0.2f;
            }

            if (!Game1.isSnowing && !Game1.isRaining)
            {
                if (!base.temporarySprites.Any(t => t.id == CLOUD_ID) || (Game1.random.NextDouble() < 0.05 && base.temporarySprites.Where(t => t.id == CLOUD_ID).Count() < 10))
                {
                    string assetPath = ModResources.assetFolderPath;

                    // Fill up with some clouds
                    for (int x = 0; x < Game1.random.Next(1, 5); x++)
                    {
                        float randomScale = Game1.random.Next(3, 13);
                        bool randomFlipped = Game1.random.Next(0, 2) == 0 ? true : false;

                        TemporaryAnimatedSprite cloud = new TemporaryAnimatedSprite(Path.Combine(assetPath, "Maps", "BellsAndWhistles.png"), PickRandomCloud(), 200f, 1, 9999, PickSpawnPosition(true), flicker: false, flipped: randomFlipped, 1f, 0f, Color.White, randomScale, 0f, 0f, 0f);
                        cloud.motion = new Vector2(_slowOffset, 0f);
                        cloud.drawAboveAlwaysFront = true;
                        cloud.id = CLOUD_ID;

                        base.temporarySprites.Add(cloud);
                    }
                }
            }

            if (!base.temporarySprites.Any(t => t.id == GROUND_ID) && Game1.random.NextDouble() < 0.10)
            {
                string assetPath = ModResources.assetFolderPath;
                bool randomFlipped = Game1.random.Next(0, 2) == 0 ? true : false;

                TemporaryAnimatedSprite decoration = new TemporaryAnimatedSprite(Path.Combine(assetPath, "Maps", "BellsAndWhistles.png"), PickRandomDecoration(), 200f, 1, 9999, PickSpawnPosition(false), flicker: false, flipped: randomFlipped, 1f, 0f, Color.White, 4f, 0f, 0f, 0f);
                decoration.motion = new Vector2(_fastOffset, 0f);
                decoration.id = GROUND_ID;

                base.temporarySprites.Add(decoration);
            }
        }

        public override bool isTileOccupiedForPlacement(Vector2 tileLocation, StardewValley.Object toPlace = null)
        {
            // Preventing player from placing items here
            return true;
        }

        public override bool isActionableTile(int xTile, int yTile, Farmer who)
        {
            string actionProperty = this.doesTileHaveProperty(xTile, yTile, "CustomAction", "AlwaysFront");
            if (actionProperty != null && actionProperty == "RippedNet")
            {
                if (!IsWithinRangeOfNet(who))
                {
                    Game1.mouseCursorTransparency = 0.5f;
                }

                return true;
            }

            return base.isActionableTile(xTile, yTile, who);
        }

        private bool IsWithinRangeOfNet(Farmer who)
        {
            int playerX = (int)(who.Position.X / 64f);
            int playerY = (int)(who.Position.Y / 64f);
            //ModEntry.monitor.Log($"({who.Position.X / 64}, {who.Position.Y / 64})", LogLevel.Debug);

            if (Enumerable.Range(34, 3).Contains(playerX) && Enumerable.Range(24, 2).Contains(playerY))
            {
                return true;
            }

            return false;
        }

        private int[] GetNetRippedTileIndexes(int startingIndex)
        {
            List<int> indexes = new List<int>();
            for (int offset = 0; offset < 5; offset++)
            {
                indexes.Add(startingIndex + offset);
            }

            return indexes.ToArray();
        }

        private bool IsNetRipped(int tileX, int tileY)
        {
            Tile hole = this.map.GetLayer("AlwaysFront").Tiles[tileX, tileY];
            if (hole != null && this.doesTileHaveProperty(tileX, tileY, "CustomAction", "AlwaysFront") == "RippedNet")
            {
                return bool.Parse(hole.Properties["IsRipped"]);
            }

            ModEntry.monitor.Log("Called [IsNetRipped] on tile that doesn't have IsRipped property on AlwaysFront layer, returning false!", LogLevel.Trace);
            return false;
        }

        private int[] GetFlagTileIndexes(int startingIndex)
        {
            List<int> indexes = new List<int>();
            for (int offset = 0; offset < 8; offset++)
            {
                indexes.Add(startingIndex + 20 * offset);
            }

            return indexes.ToArray();
        }

        public void SetFlagTexture(FlagType flagType)
        {
            if (flagType == FlagType.Unknown)
            {
                // Clear the flag
                this.setMapTileIndex(39, 21, -1, "Flags");
                this.setMapTileIndex(40, 21 - 1, -1, "Flags");
                return;
            }

            this.setAnimatedMapTile(39, 21, GetFlagTileIndexes(2 * (int)flagType), 60, "Flags", null, FLAGS_TILESHEET_INDEX);
            this.setAnimatedMapTile(40, 21, GetFlagTileIndexes(2 * (int)flagType + 1), 60, "Flags", null, FLAGS_TILESHEET_INDEX);
        }

        public bool AttemptCreateNetRip(int tileX = -1, int tileY = -1)
        {
            //ModEntry.monitor.Log("Attempting to create net rip...", LogLevel.Trace);

            List<Location> validNetLocations = _netRipLocations.Where(loc => !IsNetRipped(loc.X, loc.Y)).ToList();

            if (validNetLocations.Count() == 0)
            {
                return false;
            }

            // Pick a random valid spot to rip
            Location netLocation = validNetLocations.ElementAt(Game1.random.Next(0, validNetLocations.Count()));
            if (tileX != -1 && tileY != -1)
            {
                if (!_netRipLocations.Any(loc => !IsNetRipped(loc.X, loc.Y) && loc.X == tileX && loc.Y == tileY))
                {
                    return false;
                }

                netLocation = _netRipLocations.FirstOrDefault(loc => !IsNetRipped(loc.X, loc.Y) && loc.X == tileX && loc.Y == tileY);
            }

            // Set the net as ripped
            Tile firstTile = this.map.GetLayer("AlwaysFront").Tiles[netLocation.X, netLocation.Y];
            firstTile.Properties["IsRipped"] = true;

            this.setAnimatedMapTile(netLocation.X, netLocation.Y, GetNetRippedTileIndexes(530), 90, "AlwaysFront", null, TRAWLER_TILESHEET_INDEX);
            this.setAnimatedMapTile(netLocation.X, netLocation.Y - 1, GetNetRippedTileIndexes(506), 90, "AlwaysFront", null, TRAWLER_TILESHEET_INDEX);

            // Copy over the old properties
            this.map.GetLayer("AlwaysFront").Tiles[netLocation.X, netLocation.Y].Properties.CopyFrom(firstTile.Properties);

            this.playSound("crit");

            return true;
        }

        public bool AttemptFixNet(int tileX, int tileY, Farmer who, bool forceRepair = false)
        {
            AnimatedTile firstTile = this.map.GetLayer("AlwaysFront").Tiles[tileX, tileY] as AnimatedTile;
            //ModEntry.monitor.Log($"({tileX}, {tileY}) | {isActionableTile(tileX, tileY, who)}", LogLevel.Debug);

            if (firstTile is null)
            {
                return false;
            }

            if (!forceRepair && !(isActionableTile(tileX, tileY, who) && IsWithinRangeOfNet(who)))
            {
                return false;
            }

            if (!firstTile.Properties.ContainsKey("CustomAction") || !firstTile.Properties.ContainsKey("IsRipped"))
            {
                return false;
            }

            if (firstTile.Properties["CustomAction"] == "RippedNet" && bool.Parse(firstTile.Properties["IsRipped"]) is true)
            {
                // Stop the rip
                firstTile.Properties["IsRipped"] = false;

                // Patch up the net
                this.setMapTile(tileX, tileY, 435, "AlwaysFront", null, TRAWLER_TILESHEET_INDEX);
                this.setMapTile(tileX, tileY - 1, 436, "AlwaysFront", null, TRAWLER_TILESHEET_INDEX);

                // Add the custom properties for tracking
                this.map.GetLayer("AlwaysFront").Tiles[tileX, tileY].Properties.CopyFrom(firstTile.Properties);

                this.playSound("harvest");
            }


            return false;
        }

        public void UpdateFishCaught(bool isEngineFailing = false, int fishCaughtOverride = -1)
        {
            if (fishCaughtOverride > -1)
            {
                fishCaughtQuantity = fishCaughtOverride;
            }
            else
            {
                // If the engine is failing, then offset is negative (meaning player can lose fish if both nets are broken too)
                fishCaughtQuantity += (int)((_netRipLocations.Where(loc => !IsNetRipped(loc.X, loc.Y)).Count() * ModEntry.config.fishPerNet * fishCaughtMultiplier) + (isEngineFailing ? -1 : ModEntry.config.engineFishBonus));
                if (fishCaughtQuantity < 0)
                {
                    fishCaughtQuantity = 0;
                }
            }

            //ModEntry.monitor.Log($"Fish caught: {fishCaughtQuantity}", LogLevel.Debug);
        }

        public bool IsPlayerByBoatEdge(Farmer who)
        {
            int playerX = who.getStandingX() / 64;
            int playerY = who.getStandingY() / 64;

            string actionProperty = this.doesTileHaveProperty(playerX, playerY, "CustomAction", "Back");
            if (actionProperty != null && actionProperty == "EmptyBucketSpot")
            {
                return true;
            }

            return false;
        }

        public bool AreAnyNetsRipped()
        {
            return _netRipLocations.Any(loc => IsNetRipped(loc.X, loc.Y));
        }

        public bool AreAllNetsRipped()
        {
            return _netRipLocations.Count(loc => IsNetRipped(loc.X, loc.Y)) == _netRipLocations.Count();
        }

        public int GetRippedNetsCount()
        {
            return _netRipLocations.Count(loc => IsNetRipped(loc.X, loc.Y));
        }

        public Location GetRandomWorkingNet()
        {
            List<Location> validNetLocations = _netRipLocations.Where(loc => !IsNetRipped(loc.X, loc.Y)).ToList();

            // Pick a random valid spot to leak
            return _netRipLocations.Where(loc => !IsNetRipped(loc.X, loc.Y)).ElementAt(Game1.random.Next(0, validNetLocations.Count()));
        }
    }
}
