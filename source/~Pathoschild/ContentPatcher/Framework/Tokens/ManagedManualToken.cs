/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

using ContentPatcher.Framework.Tokens.ValueProviders;

namespace ContentPatcher.Framework.Tokens
{
    /// <summary>Wraps a token whose values are set manually.</summary>
    internal class ManagedManualToken
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The token instance.</summary>
        public IToken Token { get; }

        /// <summary>The value provider for the token.</summary>
        public ManualValueProvider ValueProvider { get; }


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="name">The value provider name.</param>
        /// <param name="scope">The mod namespace in which the token is accessible, or <c>null</c> for any namespace.</param>
        public ManagedManualToken(string name, string scope = null)
        {
            this.ValueProvider = new ManualValueProvider(name);
            this.Token = new Token(this.ValueProvider, scope);
        }
    }
}
