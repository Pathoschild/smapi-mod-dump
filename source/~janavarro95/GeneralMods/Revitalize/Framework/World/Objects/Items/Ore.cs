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
using StardewValley;
using System.Xml.Serialization;
using Omegasis.Revitalize.Framework.World.Objects.InformationFiles;

namespace Omegasis.Revitalize.Framework.World.Objects.Items
{
    [XmlType("Mods_Revitalize.Framework.World.Objects.Items.Ore")]
    public class Ore : CustomObject
    {
        public Ore()
        {

        }

        public Ore(BasicItemInformation BasicItemInfo) : base(BasicItemInfo)
        {
            this.basicItemInformation = BasicItemInfo;

        }

        public Ore(BasicItemInformation BasicItemInfo, int StackSize) : base(BasicItemInfo, StackSize)
        {
            this.basicItemInformation = BasicItemInfo;
        }

        public Ore(BasicItemInformation BasicItemInfo, Vector2 TilePosition) : base(BasicItemInfo, TilePosition)
        {
            this.basicItemInformation = BasicItemInfo;

        }

        public Ore(BasicItemInformation BasicItemInfo, Vector2 TilePosition, int StackSize = 1) : base(BasicItemInfo, TilePosition, StackSize)
        {
            this.basicItemInformation = BasicItemInfo;

        }

        public override Item getOne()
        {
            Ore component = new Ore(this.basicItemInformation.Copy(), Vector2.Zero, 1);
            return component;
        }

    }
}
