/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NermNermNerm/Junimatic
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;


using static NermNermNerm.Stardew.LocalizeFromSource.SdvLocalize;


namespace NermNermNerm.Junimatic
{
    internal class FlooringSet
    {
        private readonly IReadOnlySet<string> validFloorIds;

        internal FlooringSet(IReadOnlySet<string> ids)
        {
            this.validFloorIds = ids;
        }

        internal bool IsTileWalkable(GameLocation location, Point tileToCheck)
        {
            string? floorId = getFlooringId(location, tileToCheck);
            return floorId is not null && this.validFloorIds.Contains(floorId);
        }

        /// <summary>
        ///   Gets an identifier for the floor-tile at the coordinate if the tile is indeed on a floor tile.
        /// </summary>
        /// <returns>
        ///   null if there's not a walkable tile at that spot, else an identifier for the type of floor it is.
        ///   The identifier isn't guaranteed to be an itemid or anything, just unique to the floor type.
        /// </returns>
        internal static string? getFlooringId(GameLocation l, Point point)
        {
            Vector2 tile = point.ToVector2();
            l.terrainFeatures.TryGetValue(tile, out var terrainFeatures);
            if (terrainFeatures is Flooring flooring)
            {
                return flooring.whichFloor.Value;
            }
            else if (!l.IsOutdoors && l.isTilePassable(tile) && l.isTilePlaceable(tile) && l.getObjectAtTile(point.X, point.Y) is null)
            {
                return I("#BARE_FLOOR#");
            }
            else
            {
                return null;
            }
        }

    }
}
