/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Bpendragon/GreenhouseSprinklers
**
*************************************************/

using StardewModdingAPI.Events;

using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;

using System.Linq;

namespace Bpendragon.GreenhouseSprinklers
{
    partial class ModEntry
    {
        private void OnBuildingListChanged(object sender, BuildingListChangedEventArgs e)
        {
            Monitor.Log("Building list changed");
        }

        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (e.OldMenu is CarpenterMenu)
            {
                var gh = Game1.getFarm().buildings.OfType<GreenhouseBuilding>().FirstOrDefault();
                if (gh.buildingType.Value.StartsWith("GreenhouseSprinklers"))
                {
                    gh.buildingType.Set("Greenhouse");

                    if (Config.ShowVisualUpgrades)
                    {
                        Monitor.Log("Invalidating Texture Cache after leaving Robin's Menu");
                        Helper.GameContent.InvalidateCache("Buildings/Greenhouse");
                    }//invalidate the cache after leaving robin's menu, forcing load of new sprite if applicable.
                }
            }
        }
    }
}
