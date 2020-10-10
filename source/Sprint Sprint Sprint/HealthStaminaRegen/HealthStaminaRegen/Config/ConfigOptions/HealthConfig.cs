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

namespace HealthStaminaRegen.Config.ConfigOptions
{
    class HealthConfig
    {
        public bool Enabled { get; set; } = true;
        public int HealthPerRegenRate { get; set; } = 2;
        public int RegenRateInSeconds { get; set; } = 1;
        public int SecondsUntilRegenWhenTakenDamage { get; set; } = 3;
        public bool DontCheckConditions { get; set; } = false;
    }
}
