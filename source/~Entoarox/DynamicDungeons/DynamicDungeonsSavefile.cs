/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Entoarox/StardewMods
**
*************************************************/

using System.Collections.Generic;
using StardewValley.Objects;

namespace Entoarox.DynamicDungeons
{
    internal class DynamicDungeonsSavefile
    {
        /*********
        ** Accessors
        *********/
        public Dictionary<int, int> LoreFound = new Dictionary<int, int>();
        public Chest StorageChest = new Chest();
    }
}
