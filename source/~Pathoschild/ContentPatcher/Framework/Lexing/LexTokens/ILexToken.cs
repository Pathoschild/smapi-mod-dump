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

namespace ContentPatcher.Framework.Lexing.LexTokens
{
    /// <summary>A lexical token within a string, which combines one or more <see cref="LexBit"/> patterns into a cohesive part.</summary>
    internal interface ILexToken
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The lexical token type.</summary>
        LexTokenType Type { get; }


        /*********
        ** Public methods
        *********/
        /// <summary>Get a text representation of the lexical token.</summary>
        string ToString();
    }
}
