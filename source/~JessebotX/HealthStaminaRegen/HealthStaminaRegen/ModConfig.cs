using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthStaminaRegen
{
    class ModConfig
    {
        public int HealthRegenRate { get; set; } = 2;
        public int StaminaRegenRate { get; set; } = 2;
        public int SecondsUntilHealthRegen { get; set; } = 2;
        public int SecondsUntilStaminaRegen { get; set; } = 1;
    }
}
