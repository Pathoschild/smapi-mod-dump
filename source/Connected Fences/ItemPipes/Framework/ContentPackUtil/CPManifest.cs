/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sergiomadd/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StardewModdingAPI;
using System.Threading.Tasks;

namespace ItemPipes.Framework.ContentPackUtil
{
    class CPManifest  : IManifest
    {
        public string Name { get; }

        public string Description { get; }

        public string Author { get; }

        public ISemanticVersion Version { get; }

        public ISemanticVersion MinimumApiVersion { get; }

        public string UniqueID { get; }

        public string EntryDll { get; }

        public IManifestContentPackFor ContentPackFor { get; set; }

        public IManifestDependency[] Dependencies { get; }

        public string[] UpdateKeys { get; }

        public IDictionary<string, object> ExtraFields { get; set; }

        public CPManifest(Manifest manifest)
        {
            this.Name = manifest.Name;
            this.Description = manifest.Description;
            this.Author = manifest.Author;
            this.Version = manifest.Version;
            this.MinimumApiVersion = manifest.MinimumApiVersion;
            this.UniqueID = manifest.UniqueID;
            this.ContentPackFor = manifest.ContentPackFor;
            this.Dependencies = manifest.Dependencies;
            this.UpdateKeys = manifest.UpdateKeys;
            this.ExtraFields = manifest.ExtraFields;
        }
    }
}
