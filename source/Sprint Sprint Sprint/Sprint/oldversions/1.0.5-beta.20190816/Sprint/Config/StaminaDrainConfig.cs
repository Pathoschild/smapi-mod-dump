using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sprint.Config
{
    class StaminaDrainConfig
    {
        /// <summary> If player will lose stamina when sprinting. </summary>
        public bool DrainStamina { get; set; } = true;

        /// <summary> The amount of stamina lost per second</summary>
        public float StaminaDrainCost { get; set; } = 0.25f;
    }
}
