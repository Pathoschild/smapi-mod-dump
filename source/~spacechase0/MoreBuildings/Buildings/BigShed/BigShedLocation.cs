/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using StardewValley.Locations;

namespace MoreBuildings.Buildings.BigShed
{
    [XmlType("Mods_spacechase0_BigShedLocation")]
    public class BigShedLocation : DecoratableLocation
    {
        public BigShedLocation()
            : base("Maps\\Shed2_", "Shed2") { }

        public override List<Rectangle> getFloors()
        {
            return new() { new Rectangle(1, 3, 21, 20) };
        }

        public override List<Rectangle> getWalls()
        {
            return new() { new Rectangle(1, 1, 21, 3) };
        }
    }
}
