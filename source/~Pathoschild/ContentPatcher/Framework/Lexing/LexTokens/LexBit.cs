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
    /// <summary>A low-level character pattern within a string/</summary>
    /// <param name="Type">The lexical character pattern type.</param>
    /// <param name="Text">The raw matched text.</param>
    internal readonly record struct LexBit(LexBitType Type, string Text);
}
