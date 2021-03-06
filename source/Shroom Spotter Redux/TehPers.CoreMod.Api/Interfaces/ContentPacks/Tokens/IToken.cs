/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using System;

namespace TehPers.CoreMod.Api.ContentPacks.Tokens {
    public interface IToken : IContextSpecific {
        /// <summary>Get the value of the token, optionally with an argument.</summary>
        /// <param name="helper">The token helper.</param>
        /// <param name="arguments">The arguments passed to the token.</param>
        /// <returns>The string value of the token.</returns>
        /// <exception cref="ArgumentException"><paramref name="arguments"/> is invalid.</exception>
        string GetValue(ITokenHelper helper, string[] arguments);

        /// <summary>Invoked whenever the value is updated.</summary>
        event EventHandler Changed;
    }
}