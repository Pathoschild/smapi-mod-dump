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
using System.IO;
using ContentPatcher.Framework.Conditions;
using Pathoschild.Stardew.Common.Utilities;
using StardewModdingAPI.Utilities;

namespace ContentPatcher.Framework.Tokens.ValueProviders
{
    /// <summary>A value provider which checks whether a file exists in the content pack's folder.</summary>
    internal class HasFileValueProvider : BaseValueProvider
    {
        /*********
        ** Fields
        *********/
        /// <summary>Get whether a relative file path exists in the content pack.</summary>
        private readonly Func<string, bool> RelativePathExists;


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="relativePathExists">Get whether a relative file path exists in the content pack.</param>
        public HasFileValueProvider(Func<string, bool> relativePathExists)
            : base(ConditionType.HasFile, mayReturnMultipleValuesForRoot: false)
        {
            this.RelativePathExists = relativePathExists;
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
        public override bool HasBoundedValues(IInputArguments input, out IInvariantSet allowedValues)
        {
            allowedValues = InvariantSets.Boolean;
            return true;
        }

        /// <inheritdoc />
        public override IEnumerable<string> GetValues(IInputArguments input)
        {
            this.AssertInput(input);

            return InvariantSets.FromValue(
                this.GetPathExists(input.GetPositionalSegment())
            );
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Get whether the given file path exists.</summary>
        /// <param name="path">The relative file path.</param>
        /// <exception cref="InvalidOperationException">The path is not relative or contains directory climbing (../).</exception>
        private bool GetPathExists(string? path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return false;

            // get normalized path
            path = PathUtilities.NormalizePath(path);

            // validate
            if (Path.IsPathRooted(path))
                return false; // don't throw an error since this is often an empty token like "{{FolderName}}/asset.png"
            if (!PathUtilities.IsSafeRelativePath(path))
                throw new InvalidOperationException($"The {this.Name} token requires a relative path and cannot contain directory climbing (../).");

            // check file existence
            return this.RelativePathExists(path);
        }
    }
}
