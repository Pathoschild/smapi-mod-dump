/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sonozuki/StardewMods
**
*************************************************/

using System.Collections.Generic;

namespace FarmAnimalVarietyRedux.ContentPatcherTokens
{
    /// <summary>Represents the "GetSourceX" token for Content Patcher.</summary>
    /// <remarks>Used to get the X component of the source rectangle of a shop icon.</remarks>
    public class GetSourceXToken
    {
        /*********
        ** Fields
        *********/
        /// <summary>The old value of <see cref="ModEntry.ContentPacksLoaded"/>.</summary>
        /// <remarks>Used to determine if the content packs were loading during the context update.</remarks>
        private bool PreviousContentPacksLoaded;


        /*********
        ** Public Methods
        *********/
        /// <summary>Gets whether the token allows input arguments.</summary>
        /// <returns><see langword="true"/>, meaning it allows arguments.</returns>
        public bool AllowsInput() => true;

        /// <summary>Gets whether input arguments are required.</summary>
        /// <returns><see langword="true"/>, meaning arguments are required for the token to work.</returns>
        public bool RequiresInput() => true;

        /// <summary>Gets whether the token may return multiple values for the given input.</summary>
        /// <param name="input">The input arguments.</param>
        /// <returns><see langword="false"/>, meaning an input can't have multiple values.</returns>
        public bool CanHaveMultipleValues(string input = null) => false;

        /// <summary>Gets whether the token always returns a value within a numeric range.</summary>
        /// <param name="input">The input arguments.</param>
        /// <param name="min">The minimum value this token may return.</param>
        /// <param name="max">The maximum value this token may return.</param>
        /// <returns><see langword="true"/>, meaning the token returns a numeric value.</returns>
        public bool HasBoundedRangeValues(string input, out int min, out int max)
        {
            min = 0;
            max = 4096;
            return true;
        }

        /// <summary>Validates that the provided input arguments are valid.</summary>
        /// <param name="input">The input arguments.</param>
        /// <param name="error">The validation error, if any.</param>
        /// <returns><see langword="true"/> if <paramref name="input"/> is valid; otherwise, <see langword="false"/>.</returns>
        public bool TryValidateInput(string input, out string error)
        {
            error = "";

            var splitInput = input.Split(',');
            if (splitInput.Length == 1)
                return true;

            error = "Invalid number of input arguments, pass one argument containing the internal animal name to get the source rectangle of the shop icon";

            return false;
        }

        /// <summary>Gets the current values.</summary>
        /// <param name="input">The input arguments.</param>
        /// <returns>The value for <paramref name="input"/>.</returns>
        public IEnumerable<string> GetValues(string input) => new[] { ModEntry.Instance.AssetManager.GetShopIconSourceRectangle(input).X.ToString() };

        /// <summary>Determines if the values has changed when the context changes.</summary>
        /// <returns><see langword="true"/> if the value has updated; otherwise, <see langword="false"/>.</returns>
        public bool UpdateContext()
        {
            if (PreviousContentPacksLoaded != ModEntry.Instance.ContentPacksLoaded)
            {
                PreviousContentPacksLoaded = ModEntry.Instance.ContentPacksLoaded;
                return true;
            }

            return false;
        }
    }
}
