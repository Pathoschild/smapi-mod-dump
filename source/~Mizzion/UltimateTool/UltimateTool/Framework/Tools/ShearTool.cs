/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using SFarmer = StardewValley.Farmer;
using SObject = StardewValley.Object;

namespace UltimateTool.Framework.Tools
{
    internal class ShearTool : BaseTool
    {
        
        public ShearTool()
        {
            
        }

        public override bool IsEnabled(SFarmer who, Tool tool, Item item, GameLocation location)
        {
            return tool is Shears;
        }

        public override bool Apply(Vector2 tile, SObject tileObj, TerrainFeature tileFeature, SFarmer who, Tool tool, Item item, GameLocation location)
        {
            return UseToolOnTile(tool, tile);
        }
        /*
        private bool BeginUsing()
        {
            Rectangle rectangle = new Rectangle((int)tile.X - 32, (int)tile.Y - 32, 64, 64);
            if (location is Farm)
            {
                foreach (FarmAnimal farmAnimal in (location as Farm).animals.Values)
                {
                    if (farmAnimal.GetBoundingBox().Intersects(rectangle))
                    {
                        this.animal = farmAnimal;
                        break;
                    }
                }
            }
            else if (location is AnimalHouse)
            {
                foreach (FarmAnimal farmAnimal in (location as AnimalHouse).animals.Values)
                {
                    if (farmAnimal.GetBoundingBox().Intersects(rectangle))
                    {
                        this.animal = farmAnimal;
                        break;
                    }
                }
            }
        }*/
    }
}
