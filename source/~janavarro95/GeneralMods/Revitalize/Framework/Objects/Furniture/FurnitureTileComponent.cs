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
using Revitalize.Framework.Objects.InformationFiles;
using Revitalize.Framework.Objects.InformationFiles.Furniture;

namespace Revitalize.Framework.Objects.Furniture
{
    public class FurnitureTileComponent:MultiTiledComponent
    {


        public FurnitureTileComponent():base()
        {

        }

        public FurnitureTileComponent(BasicItemInformation itemInfo):base(itemInfo)
        {
        }

        public FurnitureTileComponent(BasicItemInformation itemInfo,Vector2 TileLocation) : base(itemInfo,TileLocation)
        {

        }

    }
}
