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

namespace MoreBuildings.Buildings.FishingShack
{
    [XmlType("Mods_spacechase0_FishingShackBuilding")]
    public class FishingShackBuilding : Building
    {
        private static readonly BluePrint Blueprint = new("FishShack");

        public FishingShackBuilding()
            : base(FishingShackBuilding.Blueprint, Vector2.Zero) { }

        public FishingShackBuilding(BluePrint blueprint, Vector2 tileLocation)
            : base(blueprint, tileLocation) { }

        protected override GameLocation getIndoors(string nameOfIndoorsWithoutUnique)
        {
            return new FishingShackLocation();
        }
    }
}
