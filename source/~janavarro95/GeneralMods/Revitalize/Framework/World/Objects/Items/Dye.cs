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
using Revitalize.Framework.Illuminate;
using StardewValley;

namespace Revitalize.Framework.World.Objects.Items
{
    public class Dye : CustomObject
    {

        public NamedColor dyeColor;

        public Dye() { }

        public Dye(NamedColor Color)
        {

        }

        public override Item getOne()
        {
            Dye component = new Dye(this.dyeColor);
            return component;
        }
    }
}
