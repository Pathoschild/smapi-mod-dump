/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/EpicBellyFlop45/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterCrabPots.Config
{
    class ConfigItem
    {
        public List<Item> WhatCanBeFound { get; set; }
        public List<Item> WhatTrashCanBeFound { get; set; }

        public ConfigItem(List<Item> whatCanBeFound, List<Item> whatTrashCanBeFound)
        {
            WhatCanBeFound = whatCanBeFound;
            WhatTrashCanBeFound = whatTrashCanBeFound;
        }
    }
}
