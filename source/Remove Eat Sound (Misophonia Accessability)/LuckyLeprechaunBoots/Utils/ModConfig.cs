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
