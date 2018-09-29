using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TehPers.Stardew.SCCL.Configs {
    public class ModConfig {

        public bool ModEnabled { get; set; } = true;
        
        public List<string> LoadOrder = new List<string>();

        public HashSet<string> DisabledMods = new HashSet<string>();
    }
}
