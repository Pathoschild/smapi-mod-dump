/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

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
