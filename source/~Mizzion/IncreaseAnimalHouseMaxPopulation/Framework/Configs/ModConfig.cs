/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

using StardewModdingAPI;

namespace IncreaseAnimalHouseMaxPopulation.Framework.Configs
{
    public class ModConfig
    {
        public MainSettings MainSettings { get; set; } = new MainSettings();
        public BuildingSettings BuildingSettings { get; set; } = new BuildingSettings();
        public Cheats Cheats { get; set; } = new Cheats();

    }
    
    public class MainSettings
    {
        public bool EnableDebugMode { get; set; } = false;
        public bool EnableBuildingMapReplacements { get; set; } = true;
        public bool EnableHoverTip { get; set; } = false;
        public SButton RefreshConfigButton { get; set; } = SButton.F5;
    }
    
    public class BuildingSettings
    {
        public int MaxBarnPopulation { get; set; } = 20;
        public int MaxCoopPopulation { get; set; } = 20;
        public int CostPerPopulationIncrease { get; set; } = 10000;
        public bool AutoFeedExtraAnimals { get; set; } = false;
    }
    
    public class Cheats
    {
        public bool EnableFree { get; set; } = false;

    }
}