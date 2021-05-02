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
using Newtonsoft.Json;
using PyTK.CustomElementHandler;
using Revitalize.Framework.Objects.InformationFiles;
using Revitalize.Framework.Objects.InformationFiles.Furniture;

namespace Revitalize.Framework.Objects.Furniture
{
    public class FurnitureTileComponent:MultiTiledComponent
    {

        [JsonIgnore]
        public int framesUntilNextRotation = 0;

        public FurnitureTileComponent():base()
        {

        }

        public FurnitureTileComponent(CustomObjectData PyTKData,BasicItemInformation Info):base(PyTKData,Info)
        {
            this.Price = Info.price;
        }

        public FurnitureTileComponent(CustomObjectData PyTKData, BasicItemInformation Info,Vector2 TileLocation) : base(PyTKData,Info,TileLocation)
        {
            this.Price = Info.price;
        }

    }
}
