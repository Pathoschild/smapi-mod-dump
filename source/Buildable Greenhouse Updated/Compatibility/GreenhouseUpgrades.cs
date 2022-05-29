/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Yariazen/BuildableGreenhouse
**
*************************************************/

using StardewValley;
using StardewValley.Buildings;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System.Linq;

namespace BuildableGreenhouse.Compatibility
{
    partial class ModCompatibility
    {
        public static void applyGreenhouseUpgradesCompatibility()
        {
            if (Helper.ModRegistry.IsLoaded("Cecidelus.GreenhouseUpgrades"))
            {
                Monitor.Log("Applying GreenhouseUpgrades Compatibility");

                GreenhouseBuilding greenhouse = Game1.getFarm().buildings.OfType<GreenhouseBuilding>().FirstOrDefault();

                string s;
                greenhouse.modData.TryGetValue("Cecidelus.GreenhouseUpgrades/upgrade-level", out s);
                if (s == null)
                    return;
                int upgradeLevel = int.Parse(s);

                if(upgradeLevel == 2)
                    waterGreenhouse();
            }
        }

        private static void waterGreenhouse()
        {
            if (Greenhouses == null)
                return;

            foreach (Building building in Greenhouses.Values)
            {
                foreach (TerrainFeature terrainFeature in building.indoors.Value.terrainFeatures.Values)
                    if (terrainFeature is HoeDirt hoeDirt)
                        hoeDirt.state.Value = 1;
                foreach (IndoorPot indoorPot in building.indoors.Value.objects.OfType<IndoorPot>())
                    indoorPot.hoeDirt.Value.state.Value = 1;
            }
        }
    }
}
