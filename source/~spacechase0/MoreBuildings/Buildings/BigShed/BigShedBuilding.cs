/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;

namespace MoreBuildings.Buildings.BigShed
{
    [XmlType("Mods_spacechase0_BigShedBuilding")]
    public class BigShedBuilding : Building
    {
        private static readonly BluePrint Blueprint = new("Shed2");

        public BigShedBuilding()
            : base(BigShedBuilding.Blueprint, Vector2.Zero) { }

        protected override GameLocation getIndoors(string nameOfIndoorsWithoutUnique)
        {
            return new BigShedLocation();
        }
    }
}
