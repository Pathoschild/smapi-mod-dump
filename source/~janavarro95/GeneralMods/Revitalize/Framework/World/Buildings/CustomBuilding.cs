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
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;

namespace Omegasis.Revitalize.Framework.World.Buildings
{
    [XmlType("Mods_Omegasis.Revitalize.Buildings.CustomBuilding")]
    public class CustomBuilding:Building
    {

        public CustomBuilding()
        {

        }

        public CustomBuilding(BluePrint bluePrint, Vector2 TileLocation):base(bluePrint,TileLocation)
        {

        }


        public virtual bool isInteractingWithBuilding(Vector2 tileLocation, Farmer who)
        {
            Rectangle rect = new Rectangle(this.tileX.Value, this.tileY.Value, this.tilesWide.Value, this.tilesHigh.Value);
            return rect.Contains(tileLocation);
        }
    }
}
