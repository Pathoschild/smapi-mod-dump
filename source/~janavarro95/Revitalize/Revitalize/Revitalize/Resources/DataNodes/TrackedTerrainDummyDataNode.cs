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
    class TrackedTerrainDummyDataNode
    {
        
        public Vector2 position;
        public string location;

        public TrackedTerrainDummyDataNode(string loc, Vector2 tile)
        {
            location = loc;          
            position = tile;

        }

    }
}
