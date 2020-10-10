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

namespace ConvenientChests.CategorizeChests.Framework.Persistence
{
    class SaveData
    {
        /// <summary>
        /// The mod version that produced this save data.
        /// </summary>
        public string Version;

        /// <summary>
        /// A list of chest addresses and the chest data associated with them.
        /// </summary>
        public IEnumerable<ChestEntry> ChestEntries;
    }
}