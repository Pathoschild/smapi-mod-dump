using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthStaminaRegen.Config.ConfigOptions
{
    class StaminaConfig
    {
        public bool Enabled { get; set; } = true;
        public float StaminaPerRegenRate { get; set; } = 2f;
        public int RegenRateInSeconds { get; set; } = 1;
        public int SecondsUntilRegenWhenUsedStamina { get; set; } = 3;
        public bool DontCheckConditions { get; set; } = false;
    }
}
