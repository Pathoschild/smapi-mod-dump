using System.Collections.Generic;

namespace BFAVToFAVRModConverter.Models
{
    /// <summary>Represents an mod's 'manifest.json' file.</summary>
    public class ModManifest
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The name of the mod.</summary>
        public string Name { get; set; }

        /// <summary>The author of the mod.</summary>
        public string Author { get; set; }

        /// <summary>The version of the mod.</summary>
        public string Version { get; set; }

        /// <summary>The description of the mod.</summary>
        public string Description { get; set; }

        /// <summary>The unique id of the mod.</summary>
        public string UniqueId { get; set; }

        /// <summary>The update keys of a mod.</summary>
        public List<string> UpdateKeys { get; set; }

        /// <summary>The mod requirements for the mod.</summary>
        public Dictionary<string, string> ContentPackFor { get; set; }


        /*********
        ** Public Methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="name">The name of the mod.</param>
        /// <param name="author">The author of the mod.</param>
        /// <param name="version">The version of the mod.</param>
        /// <param name="description">The description of the mod.</param>
        /// <param name="uniqueId">The unique id of the mod.</param>
        /// <param name="contentPackFor">The mod requirements for the mod.</param>
        public ModManifest(string name, string author, string version, string description, string uniqueId, List<string> updateKeys, Dictionary<string, string> contentPackFor)
        {
            Name = name;
            Author = author;
            Version = version;
            Description = description;
            UniqueId = uniqueId;
            UpdateKeys = updateKeys;
            ContentPackFor = contentPackFor;
        }
    }
}
