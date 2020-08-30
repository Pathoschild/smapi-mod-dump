using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthStaminaRegen.Config.ConfigOptions
{
    class RegenDelayConfig
    {
        public bool Enabled { get; set; } = true;
        public int SecondsUntilRegen { get; set; } = 2;
    }
}
