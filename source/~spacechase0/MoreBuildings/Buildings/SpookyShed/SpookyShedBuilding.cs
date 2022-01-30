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

namespace MoreBuildings.Buildings.SpookyShed
{
    [XmlType("Mods_spacechase0_SpookyShedBuilding")]
    public class SpookyShedBuilding : Building
    {
        private static readonly BluePrint Blueprint = new("SpookyShed");

        public SpookyShedBuilding()
            : base(SpookyShedBuilding.Blueprint, Vector2.Zero) { }

        protected override GameLocation getIndoors(string nameOfIndoorsWithoutUnique)
        {
            return new SpookyShedLocation();
        }
    }
}
