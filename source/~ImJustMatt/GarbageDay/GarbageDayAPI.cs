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
using ImJustMatt.GarbageDay.API;

namespace ImJustMatt.GarbageDay
{
    public class GarbageDayAPI : IGarbageDayAPI
    {
        private readonly Dictionary<string, Dictionary<string, double>> _loot;

        internal GarbageDayAPI(Dictionary<string, Dictionary<string, double>> loot)
        {
            _loot = loot;
        }

        public void AddLoot(string whichCan, IDictionary<string, double> lootTable)
        {
            if (!_loot.TryGetValue(whichCan, out var loot))
            {
                loot = new Dictionary<string, double>();
                _loot.Add(whichCan, loot);
            }

            foreach (var lootItem in lootTable)
            {
                loot[lootItem.Key] = lootItem.Value;
            }
        }
    }
}