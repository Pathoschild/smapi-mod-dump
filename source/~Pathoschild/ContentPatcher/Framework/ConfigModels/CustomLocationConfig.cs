/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

#nullable disable

using System;
using System.Runtime.Serialization;

namespace ContentPatcher.Framework.ConfigModels
{
    /// <summary>A custom location to add to the game.</summary>
    internal class CustomLocationConfig
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The unique location name.</summary>
        public string Name { get; set; }

        /// <summary>The initial map file to load.</summary>
        public string FromMapFile { get; set; }

        /// <summary>The fallback location names to migrate if no location is found matching <see cref="Name"/>.</summary>
        public string[] MigrateLegacyNames { get; set; }


        /*********
        ** Public methods
        *********/
        /// <summary>Normalize the model after it's deserialized.</summary>
        /// <param name="context">The deserialization context.</param>
        [OnDeserialized]
        public void OnDeserialized(StreamingContext context)
        {
            this.MigrateLegacyNames ??= Array.Empty<string>();
        }
    }
}
