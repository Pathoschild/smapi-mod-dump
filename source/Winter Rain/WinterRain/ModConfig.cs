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
        public bool UseSnowDuration { get; set; }
        public int[] SnowDuration { get; set; }
        public double ChanceToRain_duration { get; set; }
        public double ChanceToSnow_duration { get; set; }
        public double ChanceToRain_notDuration {  get; set; }
        public double ChanceToSnow_notDuration { get; set; }



        public ModConfig()
        {
            UseSnowDuration = true;
            SnowDuration = new int[2] {8, 21};
            ChanceToRain_duration = 0.183;
            ChanceToSnow_duration = 0.63;
            ChanceToRain_notDuration = 0.183;
            ChanceToSnow_notDuration = 0.447; // 0.63-0.183
        }
    }
}
