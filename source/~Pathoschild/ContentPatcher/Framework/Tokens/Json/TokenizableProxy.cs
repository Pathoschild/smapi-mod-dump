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
using Pathoschild.Stardew.Common.Utilities;

namespace ContentPatcher.Framework.Tokens.Json
{
    /// <summary>Tracks a instance whose value is set by a tokenizable <see cref="TokenString"/>.</summary>
    internal class TokenizableProxy : IContextual
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The token string which provides the field value.</summary>
        public IManagedTokenString TokenString { get; }

        /// <summary>Set the instance value.</summary>
        public Action<string> SetValue { get; }

        /// <inheritdoc />
        public bool IsMutable => this.TokenString.IsMutable;

        /// <inheritdoc />
        public bool IsReady => this.TokenString.IsReady;


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="tokenString">The token string which provides the field value.</param>
        /// <param name="setValue">Set the instance value.</param>
        public TokenizableProxy(IManagedTokenString tokenString, Action<string> setValue)
        {
            this.TokenString = tokenString;
            this.SetValue = setValue;
        }

        /// <inheritdoc />
        public bool UpdateContext(IContext context)
        {
            bool changed = this.TokenString.UpdateContext(context);

            if (this.IsReady)
                this.SetValue(this.TokenString.Value!);

            return changed;
        }

        /// <inheritdoc />
        public IInvariantSet GetTokensUsed()
        {
            return this.TokenString.GetTokensUsed();
        }

        /// <inheritdoc />
        public IContextualState GetDiagnosticState()
        {
            return this.TokenString.GetDiagnosticState();
        }
    }
}
