/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Caraxi/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotReallyHostedMultiplayer
{
    class ModConfig
    {
        public bool Enabled { get; set; } = true;
        public bool AutoLoad { get; set; } = false;
        public string AutoLoadFile { get; set; } = "";
        public int SleepCountdownTimer { get; set; } = 3;
    }
}
