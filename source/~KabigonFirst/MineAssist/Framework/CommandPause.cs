using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineAssist.Framework {
    class CommandPause : Command {
        public static string name = "Pause";

        public override void exec(Dictionary<string, string> par) {
            StardewWrap.pause();
            isFinish = true;
        }
    }
}
