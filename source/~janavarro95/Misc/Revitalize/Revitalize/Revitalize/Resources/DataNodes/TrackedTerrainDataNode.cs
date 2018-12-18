using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revitalize.Resources.DataNodes
{
    class TrackedTerrainDataNode
    {
      public  HoeDirt terrainFeature;
      public  Vector2 position;
      public  GameLocation location;

        public TrackedTerrainDataNode(GameLocation loc, HoeDirt terrain, Vector2 tile)
        {
            location = loc;
            terrainFeature = terrain;
            position = tile;

        }

    }
}
