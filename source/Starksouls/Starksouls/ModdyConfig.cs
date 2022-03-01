/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/OhWellMikell/Starksouls
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starksouls
{
    class ModdyConfig
    {
        public int HealthRegenPercent { get; set; } = 20;
        public int HealthAfterDeathPercent { get; set; } = 20;
        public int StaminaRegenPercent { get; set; } = 20;
        public int StaminaAfterDeathPercent { get; set; } = 20;

    }
}
