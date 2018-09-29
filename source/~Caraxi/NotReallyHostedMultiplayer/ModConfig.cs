using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotReallyHostedMultiplayer
{
    class ModConfig
    {
        public bool Enabled { get; set; } = true;
        public bool AutoLoad { get; set; } = false;
        public string AutoLoadFile { get; set; } = "";
        public int SleepCountdownTimer { get; set; } = 3;
    }
}
