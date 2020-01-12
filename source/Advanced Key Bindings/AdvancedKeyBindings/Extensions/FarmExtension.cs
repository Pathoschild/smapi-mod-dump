using System.Collections.Generic;
using StardewValley;
using StardewValley.Buildings;

namespace AdvancedKeyBindings.Extensions
{
    public static class FarmExtension
    {
        public static List<Building> GetDemolishableBuildings(this Farm farm)
        {
            var demolishableBuildings = new List<Building>();
            foreach (var building in farm.buildings)
            {
                if (building.CanDemolish())
                {
                    demolishableBuildings.Add(building);
                }
            }

            return demolishableBuildings;
        }
    }
}