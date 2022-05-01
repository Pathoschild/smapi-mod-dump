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

using System.Collections.Generic;

namespace ContentPatcher.Framework.Tokens
{
    /// <summary>The parsed input arguments for a token.</summary>
    internal interface IInputArguments
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The underlying tokenised string.</summary>
        ITokenString TokenString { get; }

        /// <summary>The positional input arguments.</summary>
        string[] PositionalArgs { get; }

        /// <summary>The named input arguments, excluding <see cref="ReservedArgs"/>.</summary>
        IDictionary<string, IInputArgumentValue> NamedArgs { get; }

        /// <summary>The named input arguments handled by Content Patcher. Tokens should generally ignore these.</summary>
        IDictionary<string, IInputArgumentValue> ReservedArgs { get; }

        /// <summary>An ordered list of the <see cref="ReservedArgs"/>, including duplicate args.</summary>
        KeyValuePair<string, IInputArgumentValue>[] ReservedArgsList { get; }

        /// <summary>Whether any <see cref="NamedArgs"/> were provided.</summary>
        bool HasNamedArgs { get; }

        /// <summary>Whether any <see cref="PositionalArgs"/> were provided.</summary>
        bool HasPositionalArgs { get; }

        /// <summary>Whether the input arguments contain tokens that may change depending on the context.</summary>
        bool IsMutable { get; }

        /// <summary>Whether the instance is valid for the current context.</summary>
        bool IsReady { get; }


        /*********
        ** Public methods
        *********/
        /// <summary>Get the first positional argument value, if any.</summary>
        string GetFirstPositionalArg();

        /// <summary>Get the raw value for a named argument, if any.</summary>
        /// <param name="key">The argument name.</param>
        string GetRawArgumentValue(string key);

        /// <summary>Get the raw input argument segment containing positional arguments, after parsing tokens but before splitting into individual arguments.</summary>
        string GetPositionalSegment();
    }
}
