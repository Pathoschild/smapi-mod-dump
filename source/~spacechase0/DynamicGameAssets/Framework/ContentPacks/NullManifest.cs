/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using StardewModdingAPI;

namespace DynamicGameAssets.Framework.ContentPacks
{
    internal class NullManifest : IManifest
    {
        public string Name => "null";

        public string Description => "null";

        public string Author => "null";

        public ISemanticVersion Version => new SemanticVersion("1.0.0");

        public ISemanticVersion MinimumApiVersion => null;

        public string UniqueID => "null";

        public string EntryDll => null;

        public IManifestContentPackFor ContentPackFor => null;

        public IManifestDependency[] Dependencies => null;

        public string[] UpdateKeys => null;

        public IDictionary<string, object> ExtraFields => null;
    }
}
