using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoodGuard
{
    [Serializable]
    public class NightFixConfig
    {
        /// <summary>
        /// Whether to enable the guard against nighttime happiness decreases.
        /// </summary>
        public bool Enabled = true;

        /// <summary>
        /// The operating mode for the night fix. Valid options are:
        /// Standard - Happiness is not lost if animals are inside after 6pm
        /// Increased - Happiness is increased if animals are inside after 6pm
        /// Maximized - Happiness is always maximized for all animals
        /// </summary>
        public String Mode = "Standard";
    }
}
