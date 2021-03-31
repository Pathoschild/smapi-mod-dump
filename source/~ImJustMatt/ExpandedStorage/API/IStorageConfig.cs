/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

using System.Collections.Generic;

namespace ImJustMatt.ExpandedStorage.API
{
    public interface IStorageConfig
    {
        /// <summary>Modded capacity allows storing more/less than vanilla (36).</summary>
        public int Capacity { get; set; }

        /// <summary>List of features to toggle on.</summary>
        HashSet<string> EnabledFeatures { get; set; }

        /// <summary>List of features to toggle off.</summary>
        HashSet<string> DisabledFeatures { get; set; }
    }
}