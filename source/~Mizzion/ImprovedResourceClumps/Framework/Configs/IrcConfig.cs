/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

using ImprovedResourceClumps.Framework.Configs.ClumpConfigs;

namespace ImprovedResourceClumps.Framework.Configs
{
    internal class IrcConfig
    {
        public bool EnableDebugMode { get; set; } = true;
        public HollowLogs HollowLog = new HollowLogs();
        public Stumps Stump = new Stumps();
        public Boulders Boulder = new Boulders();
        public Meteors Meteor = new Meteors();
        /*
        public MineRock1 MineRocks_1 = new MineRock1();
        public MineRock2 MineRocks_2 = new MineRock2();
        public MineRock3 MineRocks_3 = new MineRock3();
        public MineRock4 MineRocks_4 = new MineRock4();*/
    }
}
