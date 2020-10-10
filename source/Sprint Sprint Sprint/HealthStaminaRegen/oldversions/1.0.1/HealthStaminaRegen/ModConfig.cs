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

namespace HealthStaminaRegen
{
    class ModConfig
    {
        public int HealthRegenRate { get; set; } = 2;
        public float StaminaRegenRate { get; set; } = 2;
        public int SecondsUntilHealthRegen { get; set; } = 2;
        public int SecondsUntilStaminaRegen { get; set; } = 1;
    }
}
