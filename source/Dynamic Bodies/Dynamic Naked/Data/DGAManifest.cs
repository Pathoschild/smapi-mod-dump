/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ribeena/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using StardewModdingAPI;

namespace DynamicBodies.Data
{

    /// <summary>A manifest which describes a mod for SMAPI/DGA.
    /// A simplified version of https://github.com/Pathoschild/SMAPI/blob/d4ff9f3f5c108493452879938aa224adb556b7c3/src/SMAPI.Toolkit/Serialization/Models/Manifest.cs
    /// </summary>
    public class DGAManifest : IManifest
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The mod name.</summary>
        public string Name { get; }

        /// <summary>A brief description of the mod.</summary>
        public string Description { get; }

        /// <summary>The mod author's name.</summary>
        public string Author { get; }

        /// <summary>The mod version.</summary>
        public ISemanticVersion Version { get; }

        /// <summary>The minimum SMAPI version required by this mod, if any.</summary>
        public ISemanticVersion? MinimumApiVersion { get; }


        /// <summary>The mod which will read this as a content pack. Mutually exclusive with <see cref="Manifest.EntryDll"/>.</summary>
        public IManifestContentPackFor? ContentPackFor { get; }

        /// <summary>The other mods that must be loaded before this mod.</summary>
        public IManifestDependency[] Dependencies { get; }

        /// <summary>The unique mod ID.</summary>
        public string UniqueID { get; }

        public string EntryDll => throw new NotImplementedException();

        public string[] UpdateKeys => throw new NotImplementedException();

        /// <summary>Any manifest fields which didn't match a valid field.</summary>
        [JsonExtensionData]
        public IDictionary<string, object> ExtraFields { get; } = new Dictionary<string, object>();

        public DGAManifest(IManifest manifest)
        {
            Name = manifest.Name;
            Description = manifest.Description;
            Author = manifest.Author;
            Version = manifest.Version;
            MinimumApiVersion = manifest.MinimumApiVersion;
            Dependencies = manifest.Dependencies;
            //Hard coded information for DGA
            ContentPackFor = new DGAManifestContentPackFor("spacechase0.DynamicGameAssets", new SemanticVersion("1.0.0"));
            UniqueID = "DGA."+manifest.UniqueID;
            ExtraFields["DGA.FormatVersion"] = 2;
            ExtraFields["DGA.ConditionsFormatVersion"] = new SemanticVersion("1.23.0");
        }
    }

    class DGAManifestContentPackFor : IManifestContentPackFor
    {

        public string UniqueID { get; }
        public ISemanticVersion? MinimumVersion { get; }

        public DGAManifestContentPackFor(string uniqueId, ISemanticVersion? minimumVersion)
        {
            this.UniqueID = uniqueId;
            this.MinimumVersion = minimumVersion;
        }
    }
}
