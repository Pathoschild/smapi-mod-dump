/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/idermailer/WinterRain
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinterRain
{
    class ModConfig
    {
        public bool useSnowDuration { get; set; }
        public int[] snowDuration { get; set; }
        public double chanceToRain { get; set; }
        public double chanceToSnow { get; set; }



        public ModConfig()
        {
            this.useSnowDuration = true;
            this.snowDuration = new int[2] {8, 21};
            this.chanceToRain = 0.183;
            this.chanceToSnow = 0.63;
        }
    }
}
