/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AndyCrocker/StardewMods
**
*************************************************/

using System.Collections.Generic;

namespace BetterCrabPotsConfigUpdater.ModConfigs
{
    /// <summary>An object that contains data about each water body for the new config format.</summary>
    public class ConfigItem
    {
        /// <summary>A list of items that can be found.</summary>
        public List<Item> WhatCanBeFound { get; set; } = new List<Item>();

        /// <summary>A list of items that can be found as trash.</summary>
        public List<Item> WhatTrashCanBeFound { get; set; } = new List<Item>();

        /// <summary>Construct an instance.</summary>
        /// <param name="whatCanBeFound">A list of items that can be found.</param>
        /// <param name="whatTrashCanBeFound">A list of items that can be found as trash.</param>
        public ConfigItem(List<Item> whatCanBeFound, List<Item> whatTrashCanBeFound)
        {
            WhatCanBeFound = whatCanBeFound;
            WhatTrashCanBeFound = whatTrashCanBeFound;
        }
    }
}
