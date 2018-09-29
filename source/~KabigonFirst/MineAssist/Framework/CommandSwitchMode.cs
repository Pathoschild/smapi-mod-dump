using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineAssist.Framework {
    class CommandSwitchMode : Command {
        public static string name = "SwitchMod";
        public new enum Paramter {
            ModeName
        }

        public override void exec(Dictionary<string, string> par) {
            if (par.ContainsKey(Paramter.ModeName.ToString())) {
                ModEntry.m_instance.switchMode(par[Paramter.ModeName.ToString()]);
            }
        }
    }
}
