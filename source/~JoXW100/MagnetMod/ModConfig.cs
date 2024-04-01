/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JoXW100/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagnetMod
{
    class ModConfig
    {
        public bool EnableMod { get; set; } = true;
        public float MagnetRangeMult { get; set; } = -1;
        public int MagnetSpeedMult { get; set; } = 2;
        public bool NoLootBounce { get; set; } = false;
        public bool NoLootWave { get; set; } = false;
    }
}
