/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/SlivaStari/BuildableForge
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildableForge
{
    // Based on Industrial Forge's DataClasses
    public class BlueprintData
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string BlueprintType { get; set; }
        public string NameOfBuildingToUpgrade { get; set; }
        public string MaxOccupants { get; set; }
        public string Width { get; set; }
        public string Height { get; set; }
        public string HumanDoorX { get; set; }
        public string HumanDoorY { get; set; }
        public string AnimalDoorX { get; set; }
        public string AnimalDoorY { get; set; }
        public string MapToWarpTo { get; set; }
        public string SourceRectForMenuViewX { get; set; }
        public string SourceRectForMenuViewY { get; set; }
        public string ActionBehaviour { get; set; }
        public string NamesOfBuildingLocations { get; set; }
        public string Magical { get; set; }
        public string DaysToBuild { get; set; }
        public string MoneyRequired { get; set; }
        public List<RequiredItem> ItemsRequired { get; set; }


        public BlueprintData()
        {
            ItemsRequired = new List<RequiredItem>();
        }

        /// <summary>Convert the blueprint data to a string stardew valley understands, and replace the name and description with the current language variants, if present.</summary>
        /// <returns>The blueprint in the format Stardew Valley understands</returns>
        public string ToBlueprintString()
        {
            string s;

            string items = String.Join(" ", ItemsRequired);

            s = String.Join("/", new string[] {items, Width, Height, HumanDoorX, HumanDoorY, AnimalDoorX, AnimalDoorY, MapToWarpTo, Name, Description,
                BlueprintType, NameOfBuildingToUpgrade, SourceRectForMenuViewX, SourceRectForMenuViewY, MaxOccupants, ActionBehaviour, NamesOfBuildingLocations, MoneyRequired, Magical});
            if (DaysToBuild != "0" && DaysToBuild != "-1")
            {
                s += "/" + DaysToBuild;
            }

            return s;
        }
    }
    /// <summary>
    /// The data class for blueprint's item requirements.
    /// </summary>
    public class RequiredItem
    {
        public string ItemName { get; set; }
        public int ItemAmount { get; set; }
        public int ItemID { get; set; }


        public override string ToString()
        {
            return ItemID + " " + ItemAmount;
        }
    }
}