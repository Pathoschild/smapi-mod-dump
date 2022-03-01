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
using ContentPatcher.Framework.Conditions;

namespace ContentPatcher.Framework.Tokens.ValueProviders
{
    /// <summary>A value provider which gets the internal asset key for a content pack file, to allow loading it directly through a content manager.</summary>
    internal class InternalAssetKeyValueProvider : BaseValueProvider
    {
        /*********
        ** Fields
        *********/
        /// <summary>Get the internal asset key for a relative path.</summary>
        private readonly Func<string, string> GetInternalAssetKey;


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="getInternalAssetKey">Get the internal asset key for a relative path.</param>
        public InternalAssetKeyValueProvider(Func<string, string> getInternalAssetKey)
            : base(ConditionType.InternalAssetKey, mayReturnMultipleValuesForRoot: false)
        {
            this.GetInternalAssetKey = getInternalAssetKey;

            this.EnableInputArguments(required: true, mayReturnMultipleValues: false, maxPositionalArgs: null);
        }

        /// <inheritdoc />
        public override bool UpdateContext(IContext context)
        {
            bool changed = !this.IsReady;
            this.MarkReady(true);
            return changed;
        }

        /// <inheritdoc />
        public override IEnumerable<string> GetValues(IInputArguments input)
        {
            this.AssertInput(input);

            string path = input.GetPositionalSegment();

            if (!string.IsNullOrWhiteSpace(path))
                yield return this.GetInternalAssetKey(path);
        }
    }
}
