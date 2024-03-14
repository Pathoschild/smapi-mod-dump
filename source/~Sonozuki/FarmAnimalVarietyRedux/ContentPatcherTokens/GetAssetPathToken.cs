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
using System.IO;

namespace FarmAnimalVarietyRedux.ContentPatcherTokens
{
    /// <summary>Represents the "GetAssetPath" token for Content Patcher.</summary>
    /// <remarks>Used to get the path of the asset for either a shop icon or animal spritesheet.</remarks>
    public class GetAssetPathToken
    {
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

        /// <summary>Validates that the provided input arguments are valid.</summary>
        /// <param name="input">The input arguments.</param>
        /// <param name="error">The validation error, if any.</param>
        /// <returns><see langword="true"/> if <paramref name="input"/> is valid; otherwise, <see langword="false"/>.</returns>
        public bool TryValidateInput(string input, out string error)
        {
            error = "";

            var splitInput = input.Split(',');
            if (splitInput.Length == 1 || splitInput.Length == 5)
                return true;

            error = "Invalid number of input arguments, either pass one argument containing the internal animal name to get the path to the shop icon, or pass five arguments containing the internal animal name, internal subtype name, isBaby, isHarvested, and season to get the sprite sheet path";

            return false;
        }

        /// <summary>Gets the current values.</summary>
        /// <param name="input">The input arguments.</param>
        /// <returns>The value for <paramref name="input"/>.</returns>
        public IEnumerable<string> GetValues(string input)
        {
            input = input.Replace(", ", ",");
            var splitInput = input.Split(',');

            var suffix = "";
            if (splitInput.Length == 1)
            {
                // check if the texture the shop icon is stored on the cursors spritesheet, if so don't prefix/suffix with favr texture pathing
                var shopIconPath = ModEntry.Instance.AssetManager.GetShopIconPath(input);
                var cursorsPath = Path.Combine("LooseSprites", "Cursors");
                if (shopIconPath.ToLower() == cursorsPath.ToLower())
                {
                    yield return cursorsPath;
                    yield break;
                }

                // it's not a game sprite so prefix/suffix with favr pathing like normal
                suffix = ",shopicon";
            }

            yield return $"favr{input}{suffix}";
        }
    }
}
