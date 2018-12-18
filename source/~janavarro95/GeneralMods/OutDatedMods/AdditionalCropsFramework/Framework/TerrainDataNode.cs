using StardewValley;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdditionalCropsFramework.Framework
{
   public class TerrainDataNode
    {
       public GameLocation location;
       public int tileX;
       public int tileY;
       public HoeDirt terrainFeature;

        public TerrainDataNode(GameLocation loc, int Xtile, int Ytile, HoeDirt TerrainFeature)
        {
            location = loc;
            tileX = Xtile;
            tileY = Ytile;
            terrainFeature = TerrainFeature;
        }

    }
}
