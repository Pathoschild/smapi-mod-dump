/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System.Diagnostics;

namespace JsonAssets.Data
{
    [DebuggerDisplay("name = {Name}, id = {Id}")]
    public abstract class DataNeedsId
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The default item name.</summary>
        public string Name { get; set; }

        public string EnableWithMod { get; set; }
        public string DisableWithMod { get; set; }

        internal int Id { get; set; } = -1;
    }
}
