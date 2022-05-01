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

namespace ContentPatcher.Framework.ConfigModels
{
    /// <summary>The schema for a field in the <c>config.json</c> file.</summary>
    internal class ConfigSchemaFieldConfig
    {
        /// <summary>The comma-delimited values to allow.</summary>
        public string AllowValues { get; set; }

        /// <summary>The default value if the field is missing or (if <see cref="AllowBlank"/> is <c>false</c>) blank.</summary>
        public string Default { get; set; }

        /// <summary>Whether to allow blank values.</summary>
        public bool AllowBlank { get; set; } = false;

        /// <summary>Whether the player can specify multiple values for this field.</summary>
        public bool AllowMultiple { get; set; } = false;

        /// <summary>An optional explanation of the config field for players.</summary>
        public string Description { get; set; }

        /// <summary>An optional section key to group related fields.</summary>
        public string Section { get; set; }
    }
}
