/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

using System;

namespace ContentPatcher.Framework.Lexing.LexTokens
{
    /// <summary>An exception indicating an incorrect format when lexing tokens.</summary>
    internal class LexFormatException : FormatException
    {
        /// <summary>Construct an instance.</summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public LexFormatException(string message, FormatException innerException = null)
            : base(message, innerException) { }
    }
}
