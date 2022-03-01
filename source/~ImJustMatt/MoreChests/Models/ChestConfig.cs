/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace MoreChests.Models;

using System.Collections.Generic;

internal class ChestConfig
{
    public ChestConfig(int capacity, HashSet<string> enabledFeatures)
    {
        this.Capacity = capacity;
        this.EnabledFeatures = enabledFeatures;
    }

    protected ChestConfig()
    {
    }

    public int Capacity { get; set; }

    public HashSet<string> EnabledFeatures { get; set; }
}