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

namespace LocationCleaner.Framework.Config
{
    internal class Objects
    {
        public bool WeedRemoval { get; set; } = true;
        public bool StoneRemoval { get; set; } = true;
        public bool OreRemoval { get; set; } = true;
        public bool GeodeRemoval { get; set; } = true;
        public bool TwigRemoval { get; set; } = true;
    }
}
