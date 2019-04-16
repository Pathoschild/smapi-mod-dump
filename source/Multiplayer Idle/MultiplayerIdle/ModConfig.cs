using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerIdle {
    class ModConfig {
        public string IdleMethod { get; set; }
        public int IdleCheckSeconds { get; set; }
        public ModConfig() {
            IdleMethod = "SINGLE";
            IdleCheckSeconds = 3;
        }
    }
}
