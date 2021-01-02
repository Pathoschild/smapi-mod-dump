/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/ExpandedStorage
**
*************************************************/

namespace ExpandedStorage.Framework.Models
{
    public class ExpandedStorageData
    {
        /// <summary>Storage Name must match the name from Json Assets.</summary>
        public string StorageName { get; set; }
        
        /// <summary>Modded capacity allows storing more/less than vanilla (36).</summary>
        public int Capacity { get; set; } = 36;
        
        /// <summary>The ParentSheetIndex as provided by Json Assets.</summary>
        internal int ParentSheetIndex { get; set; }
    }
}