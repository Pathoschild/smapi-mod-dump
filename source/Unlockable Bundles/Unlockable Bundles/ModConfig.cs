/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley-unlockable-bundles
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unlockable_Bundles {
    public class ModConfig
    {
        public int ScrollDelay { get; set; }
        public int ScrollCharacterLength { get; set; }

        public ModConfig()
        {
            ScrollDelay = 25;
            ScrollCharacterLength = 10;
        }
    }
}
