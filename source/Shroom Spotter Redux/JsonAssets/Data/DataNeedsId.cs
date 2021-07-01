/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

namespace JsonAssets.Data
{
    public abstract class DataNeedsId
    {
        public string Name { get; set; }

        public string EnableWithMod { get; set; }
        public string DisableWithMod { get; set; }

        internal int id = -1;
    }
}
