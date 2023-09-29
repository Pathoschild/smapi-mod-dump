/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/SinZ163/StardewMods
**
*************************************************/

using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SinZationalSnakeSupport
{
    internal class PythonCompatManifest : IManifest
    {
        public PythonCompatManifest(IManifest original) 
        {
            this.UniqueID = original.UniqueID;
            this.Name = original.Name;
            this.Description = original.Description;
            this.Author = original.Author;
            this.Version = original.Version;
            this.MinimumApiVersion = original.MinimumApiVersion;
            this.Dependencies = original.Dependencies;
            this.UpdateKeys = original.UpdateKeys;
            this.ExtraFields = original.ExtraFields;
        }

        public string Name { get; internal init; }

        public string Description { get; internal init; }

        public string Author { get; internal init; }

        public ISemanticVersion Version { get; internal init; }

        public ISemanticVersion MinimumApiVersion { get; internal init; }
        public string UniqueID { get; internal init; }

        public string EntryDll => "SinZ.SnakeSupportHandler";

        public IManifestContentPackFor ContentPackFor => null;

        public IManifestDependency[] Dependencies { get; internal init; }

        public string[] UpdateKeys { get; internal init; }

        public IDictionary<string, object> ExtraFields { get; internal init; }
    }
}
