/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/YonKuma/MoodGuard
**
*************************************************/

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
