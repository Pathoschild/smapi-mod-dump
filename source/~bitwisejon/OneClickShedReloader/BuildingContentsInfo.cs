/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/bitwisejon/StardewValleyMods
**
*************************************************/

using StardewValley.Buildings;
using System.Collections.Generic;
using System.Linq;

namespace BitwiseJonMods
{
    class BuildingContentsInfo
    {
        public int NumberOfContainers { get; set; }
        public int NumberReadyToHarvest { get; set; }
        public int NumberReadyToLoad { get; set; }

        public IEnumerable<StardewValley.Object> Containers { get; set; }
        public IEnumerable<StardewValley.Object> ReadyToHarvestContainers { get; set; }
        public IEnumerable<StardewValley.Object> ReadyToLoadContainers { get; set; }


        public BuildingContentsInfo(Building building, List<string> supportedContainerTypes)
        {
            if (building == null || building.indoors.Value == null)
            {
                Containers = null;
                NumberOfContainers = 0;
                NumberReadyToHarvest = 0;
                NumberReadyToLoad = 0;
            }
            else
            {
                var indoors = building.indoors.Value;
                var objects = indoors.objects.Values;
                Containers = objects.Where(o => supportedContainerTypes.Any(c => o.Name == c)).Select(o => o);
                ReadyToHarvestContainers = Containers.Where(c => c.heldObject.Value != null && c.readyForHarvest.Value == true);
                ReadyToLoadContainers = Containers.Where(c => c.heldObject.Value == null && c.readyForHarvest.Value == false);

                NumberOfContainers = Containers.Count();
                NumberReadyToHarvest = ReadyToHarvestContainers.Count();
                NumberReadyToLoad = NumberReadyToHarvest + ReadyToLoadContainers.Count();
            }
        }
    }
}
