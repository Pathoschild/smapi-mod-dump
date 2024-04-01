/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ChroniclerCherry/stardew-valley-mods
**
*************************************************/

namespace MultiYieldCrops.Framework
{
    class Rule
    {
        public string ExtraYieldItemType = "Object";
        public string ItemName;
        public int minHarvest = 1;
        public int maxHarvest = 1;
        public float maxHarvestIncreasePerFarmingLevel = 0;
        public string[] disableWithMods = null;
    }
}
