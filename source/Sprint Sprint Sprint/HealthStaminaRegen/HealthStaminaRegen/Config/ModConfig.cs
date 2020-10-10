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
using HealthStaminaRegen.Config.ConfigOptions;

namespace HealthStaminaRegen.Config
{
    class ModConfig
    {
        public HealthConfig Health { get; set; } = new HealthConfig();
        public StaminaConfig Stamina { get; set; } = new StaminaConfig();
        public bool IgnorePause { get; set; } = false;
    }
}
