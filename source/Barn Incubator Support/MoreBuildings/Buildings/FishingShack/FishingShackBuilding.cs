/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using PyTK.CustomElementHandler;
using StardewValley;
using StardewValley.Buildings;

namespace MoreBuildings.Buildings.FishingShack
{
    public class FishingShackBuilding : Building, ISaveElement
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

        public object getReplacement()
        {
            Mill building = new Mill(new BluePrint("Mill"), new Vector2(this.tileX.Value, this.tileY.Value));
            building.indoors.Value = this.indoors.Value;
            building.daysOfConstructionLeft.Value = this.daysOfConstructionLeft.Value;
            building.tileX.Value = this.tileX.Value;
            building.tileY.Value = this.tileY.Value;
            return building;
        }

        public Dictionary<string, string> getAdditionalSaveData()
        {
            return new();
        }

        public void rebuild(Dictionary<string, string> additionalSaveData, object replacement)
        {
            Mill building = (Mill)replacement;
            this.indoors.Value = building.indoors.Value;
            this.daysOfConstructionLeft.Value = building.daysOfConstructionLeft.Value;
            this.tileX.Value = building.tileX.Value;
            this.tileY.Value = building.tileY.Value;

            this.indoors.Value.map = Game1.content.Load<xTile.Map>("Maps\\FishShack");
            this.indoors.Value.updateWarps();
            this.updateInteriorWarps();
        }
    }
}
