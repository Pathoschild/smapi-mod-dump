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
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile.Dimensions;
using xTile.Layers;
using xTile.Tiles;

namespace FishingTrawler.GameLocations
{
    internal class TrawlerCabin : GameLocation
    {
        private List<Location> _cabinPipeLocations;
        private const int CABIN_TILESHEET_INDEX = 2;

        public TrawlerCabin()
        {

        }

        internal TrawlerCabin(string mapPath, string name) : base(mapPath, name)
        {
            _cabinPipeLocations = new List<Location>();

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

                    if (tile.Properties.ContainsKey("CustomAction") && tile.Properties["CustomAction"] == "RustyPipe")
                    {
                        _cabinPipeLocations.Add(new Location(x, y));
                    }
                }
            }
        }

        internal void Reset()
        {
            foreach (Location pipeLocation in _cabinPipeLocations.Where(loc => IsPipeLeaking(loc.X, loc.Y)))
            {
                AttemptPlugLeak(pipeLocation.X, pipeLocation.Y, Game1.player, true);
            }
        }

        protected override void resetLocalState()
        {
            base.resetLocalState();

            AmbientLocationSounds.addSound(new Vector2(4f, 3f), 2);

            if (this.miniJukeboxTrack.Value is null)
            {
                Game1.changeMusicTrack("fieldofficeTentMusic"); // Suggested tracks: Snail's Radio, Jumio Kart (Gem), Pirate Theme
            }
        }

        public override void UpdateWhenCurrentLocation(GameTime time)
        {
            base.UpdateWhenCurrentLocation(time);
        }

        public override void cleanupBeforePlayerExit()
        {
            if (Game1.startedJukeboxMusic)
            {
                ModEntry.SetTrawlerTheme(Game1.getMusicTrackName());
            }
            else if (String.IsNullOrEmpty(this.miniJukeboxTrack.Value) && !String.IsNullOrEmpty(ModEntry.trawlerThemeSong))
            {
                ModEntry.SetTrawlerTheme(null);
            }

            base.cleanupBeforePlayerExit();
        }

        public override void checkForMusic(GameTime time)
        {
            base.checkForMusic(time);
        }

        public override bool isTileOccupiedForPlacement(Vector2 tileLocation, StardewValley.Object toPlace = null)
        {
            // Preventing player from placing items here
            return true;
        }

        public override bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
        {
            Tile tile = this.map.GetLayer("Buildings").PickTile(new Location(tileLocation.X * 64, tileLocation.Y * 64), viewport.Size);
            if (tile != null && tile.Properties.ContainsKey("CustomAction"))
            {
                if (tile.Properties["CustomAction"] == "PathosCat")
                {
                    Game1.drawObjectDialogue(ModEntry.i18n.Get("game_message.pathos_cat"));
                    return true;
                }
            }

            return base.checkAction(tileLocation, viewport, who);
        }

        public override bool isActionableTile(int xTile, int yTile, Farmer who)
        {
            string actionProperty = this.doesTileHaveProperty(xTile, yTile, "CustomAction", "Buildings");
            if (actionProperty != null && actionProperty == "RustyPipe")
            {
                if (!IsWithinRangeOfLeak(xTile, yTile, who))
                {
                    Game1.mouseCursorTransparency = 0.5f;
                }

                return true;
            }
            if (actionProperty != null && actionProperty == "PathosCat")
            {
                return Enumerable.Range(who.getTileX(), 1).Contains(xTile);
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

        private bool IsPipeLeaking(int tileX, int tileY)
        {
            Tile hole = this.map.GetLayer("Buildings").Tiles[tileX, tileY];
            if (hole != null && this.doesTileHaveProperty(tileX, tileY, "CustomAction", "Buildings") == "RustyPipe")
            {
                return bool.Parse(hole.Properties["IsLeaking"]);
            }

            ModEntry.monitor.Log("Called [IsHoleLeaking] on tile that doesn't have IsLeaking property on Buildings layer, returning false!", LogLevel.Trace);
            return false;
        }

        public bool AttemptPlugLeak(int tileX, int tileY, Farmer who, bool forceRepair = false)
        {
            AnimatedTile firstTile = this.map.GetLayer("Buildings").Tiles[tileX, tileY] as AnimatedTile;
            //ModEntry.monitor.Log($"({tileX}, {tileY}) | {isActionableTile(tileX, tileY, who)}", LogLevel.Debug);

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

            if (firstTile.Properties["CustomAction"] == "RustyPipe" && bool.Parse(firstTile.Properties["IsLeaking"]) is true)
            {
                // Stop the leak
                firstTile.Properties["IsLeaking"] = false;

                // Patch up the net
                this.setMapTile(tileX, tileY, 81, "Buildings", null, CABIN_TILESHEET_INDEX);
                this.setMapTileIndex(tileX, tileY - 1, -1, "Buildings");

                // Add the custom properties for tracking
                this.map.GetLayer("Buildings").Tiles[tileX, tileY].Properties.CopyFrom(firstTile.Properties);

                this.playSound("hammer");
            }

            return true;
        }

        private int[] GetPipeLeakingIndexes(int startingIndex)
        {
            List<int> indexes = new List<int>();
            for (int offset = 0; offset < 3; offset++)
            {
                indexes.Add(startingIndex + offset);
            }

            return indexes.ToArray();
        }

        public bool AttemptCreatePipeLeak(int tileX = -1, int tileY = -1)
        {
            List<Location> validPipeLocations = _cabinPipeLocations.Where(loc => !IsPipeLeaking(loc.X, loc.Y)).ToList();

            if (validPipeLocations.Count() == 0)
            {
                return false;
            }

            // Pick a random valid spot to rip
            Location pipeLocation = validPipeLocations.ElementAt(Game1.random.Next(0, validPipeLocations.Count()));
            if (tileX != -1 && tileY != -1)
            {
                if (!_cabinPipeLocations.Any(loc => !IsPipeLeaking(loc.X, loc.Y) && loc.X == tileX && loc.Y == tileY))
                {
                    return false;
                }

                pipeLocation = _cabinPipeLocations.FirstOrDefault(loc => !IsPipeLeaking(loc.X, loc.Y) && loc.X == tileX && loc.Y == tileY);
            }

            // Set the net as ripped
            Tile firstTile = this.map.GetLayer("Buildings").Tiles[pipeLocation.X, pipeLocation.Y];
            firstTile.Properties["IsLeaking"] = true;

            this.setAnimatedMapTile(pipeLocation.X, pipeLocation.Y, GetPipeLeakingIndexes(152), 90, "Buildings", null, CABIN_TILESHEET_INDEX);
            this.setAnimatedMapTile(pipeLocation.X, pipeLocation.Y - 1, GetPipeLeakingIndexes(134), 90, "Buildings", null, CABIN_TILESHEET_INDEX);

            // Copy over the old properties
            this.map.GetLayer("Buildings").Tiles[pipeLocation.X, pipeLocation.Y].Properties.CopyFrom(firstTile.Properties);

            this.playSound("flameSpell");

            return true;
        }

        public bool AreAnyPipesLeaking()
        {
            return _cabinPipeLocations.Any(loc => IsPipeLeaking(loc.X, loc.Y));
        }

        public bool AreAllPipesLeaking()
        {
            return _cabinPipeLocations.Count(loc => IsPipeLeaking(loc.X, loc.Y)) == _cabinPipeLocations.Count();
        }

        public int GetLeakingPipesCount()
        {
            return _cabinPipeLocations.Count(loc => IsPipeLeaking(loc.X, loc.Y));
        }

        public Location GetRandomWorkingPipe()
        {
            List<Location> validPipeLocations = _cabinPipeLocations.Where(loc => !IsPipeLeaking(loc.X, loc.Y)).ToList();

            // Pick a random valid spot to burst
            return _cabinPipeLocations.Where(loc => !IsPipeLeaking(loc.X, loc.Y)).ElementAt(Game1.random.Next(0, validPipeLocations.Count()));
        }
    }
}
