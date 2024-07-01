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
using ContentPatcher.Framework.Conditions;
using StardewModdingAPI;

namespace ContentPatcher.Framework.Migrations
{
    /// <summary>Migrates patches to format version 2.2.</summary>
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Named for clarity.")]
    internal partial class Migration_2_2 : BaseMigration
    {
        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        public Migration_2_2()
            : base(new SemanticVersion(2, 2, 0))
        {
            this.AddedTokens = new(
                nameof(ConditionType.FarmMapAsset),
                nameof(ConditionType.HasVisitedLocation)
            );
        }
    }
}
