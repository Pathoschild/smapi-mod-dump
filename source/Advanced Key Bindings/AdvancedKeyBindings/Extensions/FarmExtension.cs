/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Drachenkaetzchen/AdvancedKeyBindings
**
*************************************************/

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
        
        public static List<Building> GetMovableBuildings(this Farm farm)
        {
            var movableBuildings = new List<Building>();
            foreach (var building in farm.buildings)
            {
                if (building.CanMove())
                {
                    movableBuildings.Add(building);
                }
            }

            return movableBuildings;
        }
    }
}