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

namespace GarbageDay.API
{
    public interface IGarbageDayAPI
    {
        /// <summary>Adds to loot table</summary>
        /// <param name="whichCan">Unique ID for Garbage Can matching name from Map Action</param>
        /// <param name="lootTable">Loot table of item context tags with their relative probability</param>
        void AddLoot(string whichCan, IDictionary<string, double> lootTable);
    }
}