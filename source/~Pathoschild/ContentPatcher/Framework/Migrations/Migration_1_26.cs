/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

using System.Diagnostics.CodeAnalysis;
using ContentPatcher.Framework.ConfigModels;
using StardewModdingAPI;

namespace ContentPatcher.Framework.Migrations
{
    /// <summary>Migrates patches to format version 1.26.</summary>
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Named for clarity.")]
    internal class Migration_1_26 : BaseMigration
    {
        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        public Migration_1_26()
            : base(new SemanticVersion(1, 26, 0)) { }

        /// <inheritdoc />
        public override bool TryMigrateMainContent(ContentConfig content, [NotNullWhen(false)] out string? error)
        {
            if (!base.TryMigrateMainContent(content, out error))
                return false;

            // 1.26 adds config sections
            foreach (ConfigSchemaFieldConfig? config in content.ConfigSchema.Values)
            {
                if (!string.IsNullOrWhiteSpace(config?.Section))
                {
                    error = this.GetNounPhraseError($"using {nameof(config.Section)} with a {nameof(content.ConfigSchema)} entry");
                    return false;
                }
            }

            return true;
        }
    }
}
