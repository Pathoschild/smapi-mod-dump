/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Dwayneten/AutoFarmScreenshot
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFarmScreenshot
{
    class ModConfig
    {
        public float ScaleNumber { get; set; }
        public ModConfig()
        {
            this.ScaleNumber = 0.25f;
        }
    }
}
