/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

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
