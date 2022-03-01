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

namespace Starksouls.Config.Config_Options
{
    internal class HealthConfig
    {
        public bool Enabled { get; set; } = true;
        public int HealthRegenPercent { get; set; } = 20;
        public int HealthAfterDeathPercent { get; set; } = 20;
    }
}
