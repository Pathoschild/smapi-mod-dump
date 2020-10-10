/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

using GrandpasGift.Framework.Configs;

namespace GrandpasGift.Framework
{
    internal class ModConfig
    {
        //Whether or not the mod is enabled
        public bool ModActive { get; set; } = true;

        //Set up ForestFarmMap config.
        public ForestFarmConfig ForestConfig { get; set; } = new ForestFarmConfig();

        //Set up HillTop Farm config
        public HillTopFarmConfig HillTopConfig { get; set; } = new HillTopFarmConfig();

        //Setup Riverland Farm Config
        public RiverlandFarmConfig RiverlandConfig { get; set; } = new RiverlandFarmConfig();

        //Set up Standard Farm Config
        public StandardFarmConfig StandardConfig { get; set; } = new StandardFarmConfig();

        //Set up Wilderness Farm Config
        public WildernessFarmConfig WildernessConfig { get; set; } = new WildernessFarmConfig();


    }
}
