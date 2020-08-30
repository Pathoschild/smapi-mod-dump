using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthStaminaRegen.Config.ConfigOptions
{
    class HealthConfig
    {
        public bool Enabled { get; set; } = true;
        public int HealthPerRegenRate { get; set; } = 2;
        public uint RegenRateInSeconds { get; set; } = 1;
        public int SecondsUntilRegenWhenTakenDamage { get; set; } = 3;
        public bool DontCheckConditions { get; set; } = false;
    }
}
