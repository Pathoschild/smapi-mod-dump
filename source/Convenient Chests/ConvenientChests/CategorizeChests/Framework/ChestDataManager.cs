/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aEnigmatic/StardewValley
**
*************************************************/

using System.Runtime.CompilerServices;
using StardewValley.Objects;

namespace ConvenientChests.CategorizeChests.Framework
{
    class ChestDataManager : IChestDataManager
    {
        private readonly ConditionalWeakTable<Chest, ChestData> _table = new ConditionalWeakTable<Chest, ChestData>();

        public ChestData GetChestData(Chest chest) => _table.GetValue(chest, c => new ChestData(c));
    }
}