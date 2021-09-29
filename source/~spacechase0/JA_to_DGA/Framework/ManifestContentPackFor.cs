/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;

namespace JA_to_DGA.Framework
{
    internal class ManifestContentPackFor : IManifestContentPackFor
    {
        public string UniqueID { get; set; }

        public ISemanticVersion MinimumVersion { get; set; }
    }
}
