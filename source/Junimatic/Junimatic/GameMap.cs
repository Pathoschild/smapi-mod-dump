/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NermNermNerm/Junimatic
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Objects;

namespace NermNermNerm.Junimatic
{
    internal class GameMap
    {
        private readonly GameLocation location;

        public GameMap(GameLocation location)
        {
            this.location = location;
        }

        /// <summary>
        ///   Given a point, try and see what's there.  One or none of the output parameters will be set.
        ///   If none, it means that the point contains nothing and is impassible.
        /// </summary>
        /// <param name="tileToCheck">The location to test.</param>
        /// <param name="reachableTile">An adjacent point that a juniumo can reach.</param>
        /// <param name="validFloors">The set of floortiles that we can traverse.</param>
        /// <param name="isWalkable">True if the tile is empty, but a Junimo would be allowed to walk there.</param>
        /// <param name="machine">If the tile contains a machine that a junimo could interact with, this is set.</param>
        /// <param name="storage">If the tile contains a storage machine that the junimo could use, this is set.</param>
        public void GetThingAt(Point tileToCheck, Point reachableTile, FlooringSet validFloors, out bool isWalkable, out GameMachine? machine, out GameStorage? storage)
        {
            var item = this.location.getObjectAtTile(tileToCheck.X, tileToCheck.Y);
            if (item is not null)
            {
                isWalkable = false;
                machine = ObjectMachine.TryCreate(item, reachableTile);
                storage = GameStorage.TryCreate(item, reachableTile);
                return;
            }

            if (validFloors.IsTileWalkable(this.location, tileToCheck))
            {
                isWalkable = true;
                machine = null;
                storage = null;
                return;
            }

            var building = this.location.getBuildingAt(tileToCheck.ToVector2());
            if (building != null)
            {
                isWalkable = false;
                machine = BuildingMachine.TryCreate(building, reachableTile);
                storage = null;
                return;
            }

            isWalkable = false;
            machine = null;
            storage = null;
            return;

        }

        public void GetCrabPotAt(Point tileToCheck, Point reachableTile, out GameMachine? machine)
        {
            var item = this.location.getObjectAtTile(tileToCheck.X, tileToCheck.Y);
            machine = item is CrabPot pot ? new CrabPotMachine(pot, reachableTile) : null;
        }

        private static readonly Point[] walkableDirections = [new Point(-1, 0), new Point(1, 0), new Point(0, -1), new Point(0, 1)];

        public void GetStartingInfo(StardewValley.Object portal, out List<Point> adjacentTiles, out FlooringSet validFlooringTiles)
        {
            var floorIds = new HashSet<string>();
            var tilesWithFloors = new List<Point>();
            foreach (var direction in walkableDirections)
            {
                var targetPoint = direction + portal.TileLocation.ToPoint();
                string? floorIdAt = FlooringSet.getFlooringId(this.location, targetPoint);

                if (floorIdAt is not null)
                {
                    tilesWithFloors.Add(targetPoint);
                    floorIds.Add(floorIdAt);
                }
            }

            validFlooringTiles = new FlooringSet(floorIds);
            adjacentTiles = tilesWithFloors;
        }

        /// <summary>
        ///   Returns a list of all the points in the map's location that have a junimo portal on them.
        /// </summary>
        public IEnumerable<Point> GetPortalTiles()
        {
            return this.GetPortals().Select(o => o.TileLocation.ToPoint());
        }

        public IEnumerable<StardewValley.Object> GetPortals()
        {
            return this.location.objects.Values.Where(o => o.ItemId == UnlockPortal.JunimoPortal);
        }
    }
}
