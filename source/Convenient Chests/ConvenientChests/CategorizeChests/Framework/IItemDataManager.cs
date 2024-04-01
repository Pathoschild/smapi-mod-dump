/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aEnigmatic/StardewValley
**
*************************************************/

using System.Collections.Generic;

namespace ConvenientChests.CategorizeChests.Framework {
    /// <summary>
    /// A repository of item data that maps item keys to representative items
    /// and vice versa.
    /// </summary>
    internal interface IItemDataManager {
        Dictionary<string, IList<ItemKey>> Categories { get; }
    }
}