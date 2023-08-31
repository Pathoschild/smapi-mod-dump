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

namespace SinZationalSixSupport
{
    internal class BackwardsCompatManifest : IManifest
    {
        public BackwardsCompatManifest(string uniqueID, string name, string description, string author, ISemanticVersion version) 
        {
            this.UniqueID = uniqueID;
            this.Name = name;
            this.Description = description;
            this.Author = author;
            this.Version = version;
        }

        public string Name { get; internal init; }

        public string Description { get; internal init; }

        public string Author { get; internal init; }

        public ISemanticVersion Version { get; internal init; }

        public ISemanticVersion MinimumApiVersion => null;
        public string UniqueID { get; internal init; }

        public string EntryDll => "";

        public IManifestContentPackFor ContentPackFor => null;

        public IManifestDependency[] Dependencies => Array.Empty<IManifestDependency>();

        public string[] UpdateKeys => Array.Empty<string>();

        public IDictionary<string, object> ExtraFields => new Dictionary<string, object>();
    }
}
