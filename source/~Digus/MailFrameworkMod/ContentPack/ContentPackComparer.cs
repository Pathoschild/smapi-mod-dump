/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using StardewModdingAPI;

namespace MailFrameworkMod.ContentPack
{
    internal class ContentPackComparer : IEqualityComparer<IContentPack>
    {
        public bool Equals(IContentPack x, IContentPack y)
        {
            return y != null && x != null && x.Manifest.UniqueID.Equals(y.Manifest.UniqueID);
        }

        public int GetHashCode(IContentPack obj)
        {
            return obj.Manifest.UniqueID.GetHashCode();
        }
    }
}