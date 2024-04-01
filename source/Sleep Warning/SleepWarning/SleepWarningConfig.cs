/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/GlimmerDev/StardewValleyMod_SleepWarning
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleepWarning
{
    internal class SleepWarningConfig
    {
        public int FirstWarnTime { get; set; } = 2300;
        public int SecondWarnTime { get; set; } = 2400;
        public int ThirdWarnTime { get; set; } = 2500;
        public string WarningSound = "crystal";
    }
}
