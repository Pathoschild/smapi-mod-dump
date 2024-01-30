/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jamespfluger/Stardew-ModCollection
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckyLeprechaunBoots.Utils
{
    public class ModConfig
    {
        /// <summary>
        /// Sets whether this mod is enabled, defaults to true
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Sets how much to multiply the daily luck value by, defaults to 1.0 (none)
        /// </summary>
        public double DailyLuckMultiplier { get; set; } = 1.0;

        /// <summary>
        /// Sets how much to multiply the daily luck value by, defaults to 0.075
        /// </summary>
        public double DailyLuckToAdd { get; set; } = 0.075;
    }
}
