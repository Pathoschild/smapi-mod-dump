/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/SinZ163/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Profiler
{
    internal class ModConfig
    {
        public int BigLoopThreshold { get; set; } = 100;
        public double BigLoopInnerThreshold { get; set; } = 0.01d;
        public int EventThreshold { get; set; } = 10;

        public double LoggerDurationOuterThreshold { get; set; } = 5.0;
        public double LoggerDurationInnerThreshold { get; set; } = 0.1;
    }
}
