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

using System.Diagnostics.CodeAnalysis;
using ContentPatcher.Framework.Conditions;
using ContentPatcher.Framework.Lexing.LexTokens;
using Pathoschild.Stardew.Common.Utilities;
using StardewModdingAPI;

namespace ContentPatcher.Framework.Migrations
{
    /// <summary>Validates patches for format version 1.15.</summary>
    /// <remarks>This needs to be run before <see cref="Migration_1_15_Rewrites"/> to avoid rewritten token arguments triggering validation errors.</remarks>
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Named for clarity.")]
    internal class Migration_1_15_Prevalidation : BaseMigration
    {
        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        public Migration_1_15_Prevalidation()
            : base(new SemanticVersion(1, 15, 0))
        {
            this.AddedTokens.AddMany(
                ConditionType.HasConversationTopic.ToString()
            );
        }

        /// <inheritdoc />
        public override bool TryMigrate(ref ILexToken lexToken, out string error)
        {
            if (!base.TryMigrate(ref lexToken, out error))
                return false;

            // 1.15 adds named arguments
            if (lexToken is LexTokenToken token && token.HasInputArgs() && token.InputArgs.ToString().Contains("|") && token.InputArgs.ToString().Contains("="))
            {
                error = this.GetNounPhraseError("using named arguments");
                return false;
            }

            return true;
        }
    }
}
