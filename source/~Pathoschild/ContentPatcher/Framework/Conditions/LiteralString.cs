/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using ContentPatcher.Framework.Lexing.LexTokens;
using ContentPatcher.Framework.Tokens;
using Pathoschild.Stardew.Common.Utilities;

namespace ContentPatcher.Framework.Conditions
{
    /// <summary>A literal string value.</summary>
    internal class LiteralString : IManagedTokenString
    {
        /*********
        ** Accessors
        *********/
        /// <inheritdoc />
        public string? Raw { get; }

        /// <inheritdoc />
        public IEnumerable<ILexToken> LexTokens { get; }

        /// <inheritdoc />
        public bool HasAnyTokens { get; } = false;

        /// <inheritdoc />
        public bool IsMutable { get; } = false;

        /// <inheritdoc />
        public bool IsSingleTokenOnly { get; } = false;

        /// <inheritdoc />
        public string Value { get; }

        /// <inheritdoc />
        public bool IsReady { get; } = true;

        /// <inheritdoc />
        public string Path { get; }


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="value">The literal string value.</param>
        /// <param name="path">The path to the value from the root content file.</param>
        public LiteralString(string? value, LogPathBuilder path)
        {
            this.Raw = value;
            this.Value = value ?? string.Empty;
            this.LexTokens = new ILexToken[] { new LexTokenLiteral(value) };
            this.Path = path.ToString();
        }

        /// <inheritdoc />
        public bool UpdateContext(IContext context)
        {
            return false;
        }

        /// <inheritdoc />
        public IInvariantSet GetTokensUsed()
        {
            return InvariantSets.Empty;
        }

        /// <inheritdoc />
        public IEnumerable<LexTokenToken> GetTokenPlaceholders(bool recursive)
        {
            return Enumerable.Empty<LexTokenToken>();
        }

        /// <inheritdoc />
        public bool UsesToken(ConditionType token)
        {
            return false;
        }

        /// <inheritdoc />
        public bool UsesTokens(IEnumerable<ConditionType> tokens)
        {
            return false;
        }

        /// <inheritdoc />
        public IContextualState GetDiagnosticState()
        {
            return new ContextualState();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return this.Value;
        }
    }
}
