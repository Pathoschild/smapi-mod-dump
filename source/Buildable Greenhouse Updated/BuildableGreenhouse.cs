/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Yariazen/BuildableGreenhouse
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;
using System.Xml.Serialization;

namespace BuildableGreenhouse
{
    [XmlType("Mods_Yariazen_BuildableGreenhouseBuilding")]
    public class BuildableGreenhouseBuilding : Building
    {
        private static readonly BluePrint Blueprint = new("BuildableGreenhouse");

        public BuildableGreenhouseBuilding()
            : base(Blueprint, Vector2.Zero) { }

        public BuildableGreenhouseBuilding(BluePrint blueprint, Vector2 tileLocation)
            : base(blueprint, tileLocation) { }

        protected override GameLocation getIndoors(string nameOfIndoorsWithoutUnique)
        {
            return new BuildableGreenhouseLocation();
        }
    }

    [XmlType("Mods_Yariazen_BuildableGreenhouseLocation")]
    public class BuildableGreenhouseLocation : GameLocation
    {
        public BuildableGreenhouseLocation()
            : base("Maps\\Greenhouse", "BuildableGreenhouse")
        { 
            isGreenhouse.Value = true;
        }
    }
}