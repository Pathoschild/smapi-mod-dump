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
using Revitalize.Framework.World.Objects.InformationFiles;
using Revitalize.Framework.Objects;
using StardewValley;

namespace Revitalize.Framework.World.Objects.Items
{
    public class Ore:CustomObject
    {
        public Ore()
        {

        }

        public Ore(BasicItemInformation BasicItemInfo): base(BasicItemInfo)
        {


        }

        public Ore(BasicItemInformation BasicItemInfo, int StackSize) : base(BasicItemInfo, StackSize)
        {
           

        }

        public Ore(BasicItemInformation BasicItemInfo, Vector2 TilePosition) : base(BasicItemInfo,TilePosition)
        {


        }

        public Ore(BasicItemInformation BasicItemInfo, Vector2 TilePosition, int StackSize=1) : base(BasicItemInfo,TilePosition,StackSize)
        {


        }

        public override Item getOne()
        {
            Ore component = new Ore(this.basicItemInfo.Copy(),Vector2.Zero,1);
            return component;
        }

    }
}
