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
using System.Linq;
using Pathoschild.Stardew.Common.Utilities;

namespace ContentPatcher.Framework.Tokens.ValueProviders
{
    /// <summary>A value provider whose values don't change after it's initialized.</summary>
    internal class ImmutableValueProvider : BaseValueProvider
    {
        /*********
        ** Fields
        *********/
        /// <summary>The allowed root values (or <c>null</c> if any value is allowed).</summary>
        private readonly IInvariantSet? AllowedRootValues;

        /// <summary>The current token values.</summary>
        private readonly IInvariantSet Values;


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="name">The value provider name.</param>
        /// <param name="values">Get the current token values.</param>
        /// <param name="allowedValues">The allowed values (or <c>null</c> if any value is allowed).</param>
        /// <param name="canHaveMultipleValues">Whether the root may contain multiple values (or <c>null</c> to set it based on the given values).</param>
        /// <param name="isMutable">Whether to mark the value provider as mutable. The value provider will be immutable regardless, but this avoids optimizations in cases where the value provider may be replaced later.</param>
        public ImmutableValueProvider(string name, IInvariantSet? values, IInvariantSet? allowedValues = null, bool? canHaveMultipleValues = null, bool isMutable = false)
            : base(name, mayReturnMultipleValuesForRoot: false)
        {
            this.Values = values ?? InvariantSets.Empty;
            this.AllowedRootValues = allowedValues?.Any() == true ? allowedValues : null;
            this.MayReturnMultipleValuesForRoot = canHaveMultipleValues ?? (this.Values.Count > 1 || this.AllowedRootValues == null || this.AllowedRootValues.Count > 1);
            this.IsMutable = isMutable;
        }

        /// <inheritdoc />
        public override bool HasBoundedValues(IInputArguments input, [NotNullWhen(true)] out IInvariantSet? allowedValues)
        {
            allowedValues = this.AllowedRootValues;
            return allowedValues != null;
        }

        /// <inheritdoc />
        public override IEnumerable<string> GetValues(IInputArguments input)
        {
            this.AssertInput(input);

            return this.Values;
        }
    }
}
