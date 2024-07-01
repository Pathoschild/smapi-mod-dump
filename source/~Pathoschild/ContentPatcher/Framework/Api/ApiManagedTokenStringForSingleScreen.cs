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
using System.Diagnostics.CodeAnalysis;
using ContentPatcher.Framework.Tokens;
using IApiManagedTokenString = ContentPatcher.IManagedTokenString;

namespace ContentPatcher.Framework.Api
{
    /// <summary>A parsed string which may contain Content Patcher tokens matched against Content Patcher's internal context for an API consumer. This implementation assume it's always run on the same screen.</summary>
    internal class ApiManagedTokenStringForSingleScreen : IApiManagedTokenString
    {
        /*********
        ** Fields
        *********/
        /// <summary>The underlying token string.</summary>
        private readonly IManagedTokenString TokenString;

        /// <summary>The context with which to update the token string.</summary>
        private readonly IContext Context;

        /// <summary>The context update tick when the token string was last updated.</summary>
        private int LastUpdateTick = -1;


        /*********
        ** Accessors
        *********/
        /// <inheritdoc />
        [MemberNotNullWhen(false, nameof(ApiManagedTokenStringForSingleScreen.ValidationError))]
        public bool IsValid { get; }

        /// <inheritdoc />
        public string? ValidationError { get; }

        /// <inheritdoc />
        public bool IsReady => this.TokenString.IsReady;

        /// <inheritdoc />
        public string? Value { get; private set; }


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="tokenString">The underlying token string.</param>
        /// <param name="context">The context with which to update the token string.</param>
        /// <param name="isValid">Whether the token string was parsed successfully (regardless of whether its tokens are in scope currently).</param>
        /// <param name="validationError">If <paramref name="isValid"/> is false, an error phrase indicating why the token string failed to parse.</param>
        public ApiManagedTokenStringForSingleScreen(IManagedTokenString tokenString, IContext context, bool isValid = true, string? validationError = null)
        {
            this.TokenString = tokenString;
            this.Context = context;
            this.IsValid = isValid;
            this.ValidationError = validationError;
        }

        /// <inheritdoc />
        public IEnumerable<int> UpdateContext()
        {
            // skip unneeded updates
            if (!this.ShouldUpdate())
                return [];
            this.LastUpdateTick = this.Context.UpdateTick;

            // update context
            string? oldValue = this.Value;
            if (this.IsValid)
            {
                this.TokenString.UpdateContext(this.Context);
                this.Value = this.IsReady ? this.TokenString.Value : null;
            }

            // return screen ID if it changed
            return this.Value != oldValue
                ? new[] { StardewModdingAPI.Context.ScreenId }
                : [];
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Get whether the token string needs to be updated for the current context.</summary>
        private bool ShouldUpdate()
        {
            return
                this.TokenString.ShouldUpdate()
                && this.LastUpdateTick < this.Context.UpdateTick;
        }
    }
}
