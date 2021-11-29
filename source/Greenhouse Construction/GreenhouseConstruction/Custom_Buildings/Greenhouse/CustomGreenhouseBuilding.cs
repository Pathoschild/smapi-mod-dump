/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/derslayr10/GreenhouseConstruction
**
*************************************************/

using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using PyTK.CustomElementHandler;
using StardewValley;
using StardewValley.Buildings;

namespace GreenhouseConstruction.Custom_Buildings.Greenhouse
{

    [XmlType("Mods_Derslayr_CustomGreenhouseBuilding")]

    public class CustomGreenhouseBuilding : Building, ISaveElement
    {

        private static readonly BluePrint BluePrint = new BluePrint("GreenhouseConstruction_SpecialGreenhouse");

        public CustomGreenhouseBuilding() : base(CustomGreenhouseBuilding.BluePrint, Vector2.Zero) {
        

        
        }

        protected override GameLocation getIndoors(string nameOfIndoorsWithoutUnique)
        {
            return new CustomGreenhouseLocation();
        }

        public Dictionary<string, string> getAdditionalSaveData()
        {
            return new Dictionary<string, string>();
        }

        public object getReplacement()
        {

            Mill building = new Mill(new BluePrint("GreenhouseConstruction_SpecialGreenhouse"), new Vector2(this.tileX.Value, this.tileY.Value));
            building.indoors.Value = this.indoors.Value;
            building.daysOfConstructionLeft.Value = this.daysOfConstructionLeft.Value;
            building.tileX.Value = this.tileX.Value;
            building.tileY.Value = this.tileY.Value;
            return building;

        }

        public void rebuild(Dictionary<string, string> additionalSaveData, object replacement)
        {

            Mill building = (Mill)replacement;

            this.indoors.Value = building.indoors.Value;
            this.daysOfConstructionLeft.Value = building.daysOfConstructionLeft.Value;
            this.tileX.Value = building.tileX.Value;
            this.tileY.Value = building.tileY.Value;

            this.indoors.Value.map = Game1.content.Load<xTile.Map>("Maps\\GreenhouseInterior");
            this.indoors.Value.updateWarps();
            this.updateInteriorWarps();

        }
    }
}
