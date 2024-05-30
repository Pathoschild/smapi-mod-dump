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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using StardewModdingAPI.Utilities;
using IApiManagedTokenString = ContentPatcher.IManagedTokenString;

namespace ContentPatcher.Framework.Api
{
    /// <summary>A parsed string which may contain Content Patcher tokens matched against Content Patcher's internal context for an API consumer. This implementation is <strong>per-screen</strong>, so the result depends on the screen that's active when calling the members.</summary>
    internal class ApiManagedTokenString : IApiManagedTokenString
    {
        /*********
        ** Fields
        *********/
        /// <summary>The underlying values.</summary>
        private readonly PerScreen<IApiManagedTokenString> ManagedValues;


        /*********
        ** Accessors
        *********/
        /// <inheritdoc />
        [MemberNotNullWhen(false, nameof(ApiManagedTokenString.ValidationError))]
        public bool IsValid => this.ManagedValues.Value.IsValid;

        /// <inheritdoc />
        public string? ValidationError => this.ManagedValues.Value.ValidationError;

        /// <inheritdoc />
        public bool IsReady => this.ManagedValues.Value.IsReady;

        /// <inheritdoc />
        public string? Value => this.ManagedValues.Value.Value;


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="parse">Get a parsed token string for the currently active screen.</param>
        public ApiManagedTokenString(Func<IApiManagedTokenString> parse)
        {
            this.ManagedValues = new PerScreen<IApiManagedTokenString>(parse);
        }

        /// <inheritdoc />
        public IEnumerable<int> UpdateContext()
        {
            return new HashSet<int>(
                this.ManagedValues
                    .GetActiveValues()
                    .Select(p => p.Value)
                    .SelectMany(p => p.UpdateContext())
            );
        }
    }
}
