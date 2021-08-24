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
using ExpandedStorage.API;

namespace ExpandedStorage.Framework.Models
{
    public class StorageConfigModel : IStorageConfig
    {
        public IList<string> Tabs { get; set; } = new List<string>();
        public int Capacity { get; set; }
        public HashSet<string> EnabledFeatures { get; set; } = new() {"CanCarry", "ShowColorPicker", "ShowSearchBar", "ShowTabs"};
        public HashSet<string> DisabledFeatures { get; set; } = new();
    }
}