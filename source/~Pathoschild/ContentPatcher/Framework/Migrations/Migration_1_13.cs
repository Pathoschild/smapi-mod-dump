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
using System.Linq;
using ContentPatcher.Framework.Conditions;
using ContentPatcher.Framework.ConfigModels;
using Pathoschild.Stardew.Common.Utilities;
using StardewModdingAPI;

namespace ContentPatcher.Framework.Migrations
{
    /// <summary>Migrates patches to format version 1.13.</summary>
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Named for clarity.")]
    internal class Migration_1_13 : BaseMigration
    {
        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        public Migration_1_13()
            : base(new SemanticVersion(1, 13, 0))
        {
            this.AddedTokens = new InvariantSet(
                nameof(ConditionType.Query)
            );
        }

        /// <inheritdoc />
        public override bool TryMigrate(ref PatchConfig[] patches, [NotNullWhen(false)] out string? error)
        {
            if (!base.TryMigrate(ref patches, out error))
                return false;

            foreach (PatchConfig patch in patches)
            {
                // 1.13 adds map tile patches
                if (patch.MapTiles.Any())
                {
                    error = this.GetNounPhraseError($"using {nameof(PatchConfig.MapTiles)}");
                    return false;
                }
            }

            return true;
        }
    }
}
