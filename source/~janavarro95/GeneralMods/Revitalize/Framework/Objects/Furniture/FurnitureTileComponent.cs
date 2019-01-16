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
