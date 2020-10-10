/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

namespace ContentPatcher.Framework.Lexing.LexTokens
{
    /// <summary>A lexical token type.</summary>
    public enum LexTokenType
    {
        /// <summary>A literal string.</summary>
        Literal,

        /// <summary>A Content Patcher token.</summary>
        Token,

        /// <summary>The input arguments to a Content Patcher token.</summary>
        TokenInput
    }
}
