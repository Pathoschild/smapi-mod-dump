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

namespace MoreBuildings.Buildings.MiniSpa
{
    [XmlType("Mods_spacechase0_MiniSpaBuilding")]
    public class MiniSpaBuilding : Building
    {
        private static readonly BluePrint Blueprint = new("MiniSpa");

        public MiniSpaBuilding()
            : base(MiniSpaBuilding.Blueprint, Vector2.Zero) { }

        protected override GameLocation getIndoors(string nameOfIndoorsWithoutUnique)
        {
            return new MiniSpaLocation();
        }
    }
}
