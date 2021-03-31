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

namespace ImJustMatt.GarbageDay.API
{
    public interface IContent
    {
        /// <summary>Items to add to a Loot table</summary>
        IDictionary<string, IDictionary<string, double>> Loot { get; set; }
    }
}