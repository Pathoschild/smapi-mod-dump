/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JessebotX/StardewMods
**
*************************************************/

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
