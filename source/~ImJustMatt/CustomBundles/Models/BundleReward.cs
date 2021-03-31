/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace ImJustMatt.CustomBundles.Models
{
    public class BundleReward
    {
        /// <summary>The type of object rewarded</summary>
        public string ObjectType { get; set; }

        /// <summary>ParentSheetIndex of object rewarded</summary>
        public int ObjectId { get; set; }

        /// <summary>The number of items rewarded</summary>
        public int NumberGiven { get; set; }
    }
}