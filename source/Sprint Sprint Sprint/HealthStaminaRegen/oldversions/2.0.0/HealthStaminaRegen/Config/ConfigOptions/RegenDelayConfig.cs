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
    class RegenDelayConfig
    {
        public bool Enabled { get; set; } = true;
        public int SecondsUntilRegen { get; set; } = 2;
    }
}
