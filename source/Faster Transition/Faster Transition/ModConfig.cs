/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DotSharkTeeth/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FasterTransition
{
    public sealed class ModConfig
    {
        public bool Enable { get; set; } = true;
        public bool NoTransition { get; set; } = false;
        public float Speed { get; set; } = 0.003f;
    }
}
