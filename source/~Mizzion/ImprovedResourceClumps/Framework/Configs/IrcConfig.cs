using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
