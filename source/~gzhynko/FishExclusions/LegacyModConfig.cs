/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/gzhynko/StardewMods
**
*************************************************/

namespace FishExclusions
{
    /// <summary> The deprecated mod config from version 1.0.0 </summary>
    public class LegacyModConfig
    {
        /// <summary>
        /// The items to exclude.
        /// </summary>
        public int[] ItemsToExclude { get; set; } = { };
        
        /// <summary>
        /// The ID of the item to catch if all possible fish for this water body / season / weather is excluded.
        /// </summary>
        public int ItemToCatchIfAllFishIsExcluded { get; set; } = 168;
        
        /// <summary>
        /// The number of times to retry the 'fish choosing' algorithm before giving up and catching the item specified above.
        /// WARNING: Large numbers can cause a Stack Overflow exception. Use with caution.
        /// </summary>
        public int TimesToRetry { get; set; } = 20;
    }
}