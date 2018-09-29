using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdjustableStaminaHealing
{
    public class Config
    {
        public float HealingValuePerSeconds { get; set; } = 0.5f;
        public int SecondsNeededToStartHealing { get; set; } = 3;
        public bool StopHealingWhileGamePaused { get; set; } = true;
        public bool HealHealth { get; set; } = false;
        public Keys IncreaseKey{ get; set; } = Keys.O;
        public Keys DecreaseKey { get; set; } = Keys.P;
    }
}
