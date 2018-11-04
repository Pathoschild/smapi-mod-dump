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