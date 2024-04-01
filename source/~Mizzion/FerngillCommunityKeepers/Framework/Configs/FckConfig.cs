/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FerngillCommunityKeepers.Framework.Configs
{
    internal class FckConfig
    {
        public bool EnableFreeMode { get; set; } = false;

        public int HourlyRateForWorkers { get; set; } = 10;

        //TreeConfig
        public TreeConfig TreeSettings {get;set;} = new();

        //CropConfig
        public CropConfig CropSettings { get; set; } = new();

        //AnimalConfig
        public AnimalConfig AnimalSettings { get; set; } = new();
    }
}
