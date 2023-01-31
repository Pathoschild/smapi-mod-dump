/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Yariazen/YariazenMods
**
*************************************************/

using static BuildableGreenhouse.Framework.ModTranslation;

namespace BuildableGreenhouse.Framework
{
    public class ModConfig
    {
        public string _StartWithGreenhouse { get; set; }
        public bool StartWithGreenhouse { set; get; }

        public string _BuildCostDescription { get; set; }
        public int BuildCost { get; set; }

        public string _BuildDaysDescription { get; set; }
        public int BuildDays { get; set; }

        public string _BuildingDifficulty { get; set; }
        public int BuildingDifficulty { get; set; }

        public ModConfig()
        {
            _StartWithGreenhouse = StartWithGreenhouse_Tooltip;
            StartWithGreenhouse = false;
            _BuildCostDescription = BuildCost_Tooltip;
            BuildCost = 100000;
            _BuildDaysDescription = BuildDays_Tooltip;
            BuildDays = 3;
            _BuildingDifficulty = BuildingDifficulty_Tooltip;
            BuildingDifficulty = 2;
        }
    }
}