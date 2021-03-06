/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

using ContentPatcher.Framework.ConfigModels;
using Pathoschild.Stardew.Common.Utilities;

namespace ContentPatcher.Framework
{
    /// <summary>A loaded content pack.</summary>
    internal class LoadedContentPack : RawContentPack
    {
        /*********
        ** Accessors
        *********/
        /// <summary>Handles reading, normalizing, and saving the configuration for the content pack.</summary>
        public ConfigFileHandler ConfigFileHandler { get; }

        /// <summary>The content pack's configuration.</summary>
        public InvariantDictionary<ConfigField> Config { get; }


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="contentPack">The raw content pack instance.</param>
        /// <param name="configFileHandler">Handles reading, normalizing, and saving the configuration for the content pack.</param>
        /// <param name="config">The content pack's configuration.</param>
        public LoadedContentPack(RawContentPack contentPack, ConfigFileHandler configFileHandler, InvariantDictionary<ConfigField> config)
            : base(contentPack)
        {
            this.ConfigFileHandler = configFileHandler;
            this.Config = config;
        }
    }
}
