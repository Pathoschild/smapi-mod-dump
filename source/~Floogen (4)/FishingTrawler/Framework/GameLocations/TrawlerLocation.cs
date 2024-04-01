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
using StardewValley;
using StardewValley.BellsAndWhistles;
using System;
using System.Collections.Generic;
using System.Linq;
using xTile.Dimensions;

namespace FishingTrawler.Framework.GameLocations
{
    internal abstract class TrawlerLocation : GameLocation
    {
        private List<Location> _recentlyRepairedLocations;

        public TrawlerLocation()
        {

        }

        internal TrawlerLocation(string mapPath, string name) : base(mapPath, name)
        {
            _recentlyRepairedLocations = new List<Location>();

            FishingTrawler.assetManager.HandleMapAssets(this);
        }

        internal abstract void Reset();

        internal bool IsWithinRangeOfTile(int tileX, int tileY, int xDistance, int yDistance, Farmer who)
        {
            if (Enumerable.Range((int)(who.Tile.X - xDistance), (xDistance * 2) + 1).Contains(tileX))
            {
                if (Enumerable.Range((int)(who.Tile.Y - yDistance), (yDistance * 2) + 1).Contains(tileY))
                {
                    return true;
                }
            }

            return false;
        }

        internal bool IsMessageAlreadyDisplayed(string message)
        {
            return Game1.hudMessages.Any(m => m.message == Game1.parseText(message, Game1.dialogueFont, 384));
        }

        internal bool WasTileRepairedRecently(int x, int y)
        {
            return _recentlyRepairedLocations.Any(l => l.X == x && l.Y == y);
        }

        internal void AddRepairedTile(int x, int y)
        {
            _recentlyRepairedLocations.Add(new Location(x, y));
        }

        internal void ClearAllRepairedTiles()
        {
            _recentlyRepairedLocations.Clear();
        }

        protected override void resetLocalState()
        {
            base.resetLocalState();
            critters = new List<Critter>();

            if (string.IsNullOrEmpty(miniJukeboxTrack.Value))
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

        public override bool CanItemBePlacedHere(Vector2 tile, bool itemIsPassable = false, CollisionMask collisionMask = CollisionMask.All, CollisionMask ignorePassables = CollisionMask.Buildings | CollisionMask.Characters | CollisionMask.Farmers | CollisionMask.Flooring | CollisionMask.Furniture | CollisionMask.TerrainFeatures | CollisionMask.LocationSpecific, bool useFarmerTile = false, bool ignorePassablesExactly = false)
        {
            // Preventing player from placing items here
            return false;
        }
    }
}
