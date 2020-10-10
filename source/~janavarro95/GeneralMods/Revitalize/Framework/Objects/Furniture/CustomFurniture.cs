/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Revitalize.Framework.Objects.InformationFiles.Furniture;

namespace Revitalize.Framework.Objects.Furniture
{
    public class CustomFurniture:CustomObject
    {
        public FurnitureInformation furnitureInfo;


        public CustomFurniture() : base()
        {

        }

        public CustomFurniture(BasicItemInformation itemInfo, FurnitureInformation furnitureInfo) : base(itemInfo)
        {
            this.furnitureInfo = furnitureInfo;
        }

        public CustomFurniture(BasicItemInformation itemInfo, Vector2 TileLocation, FurnitureInformation furnitureInfo) : base(itemInfo, TileLocation)
        {
            this.furnitureInfo = furnitureInfo;
        }

    }
}
