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
    public class ModConfig
    {
        /// <summary>
        /// Whether animals' happiness should increase when inside after 6pm.
        /// </summary>
        public NightFixConfig NightFix = new NightFixConfig();

        public ProfessionFixConfig ProfessionFix = new ProfessionFixConfig();
    }
}
