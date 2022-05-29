/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/AlternativeTextures
**
*************************************************/

using AlternativeTextures.Framework.Managers;
using AlternativeTextures.Framework.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlternativeTextures.Framework.External.ContentPatcher
{
    internal class TextureToken
    {
        private TextureManager _textureManager;
        private AssetManager _assetManager;

        public TextureToken(TextureManager textureManager, AssetManager assetManager)
        {
            _textureManager = textureManager;
            _assetManager = assetManager;
        }

        /// <summary>Get whether the token allows input arguments (e.g. an NPC name for a relationship token).</summary>
        /// <remarks>Default false.</remarks>
        public bool AllowsInput() { return true; }

        /// <summary>Whether the token requires input arguments to work, and does not provide values without it (see <see cref="AllowsInput"/>).</summary>
        /// <remarks>Default false.</remarks>
        public bool RequiresInput() { return true; }

        /// <summary>Whether the token may return multiple values for the given input.</summary>
        /// <param name="input">The input arguments, if any.</param>
        /// <remarks>Default true.</remarks>
        public bool CanHaveMultipleValues(string input = null) { return false; }

        /// <summary>Get the set of valid input arguments if restricted, or an empty collection if unrestricted.</summary>
        /// <remarks>Default unrestricted.</remarks>
        public IEnumerable<string> GetValidInputs()
        {
            return _textureManager.GetValidTextureNamesWithSeason();
        }

        /// <summary>Validate that the provided input arguments are valid.</summary>
        /// <param name="input">The input arguments, if any.</param>
        /// <param name="error">The validation error, if any.</param>
        /// <returns>Returns whether validation succeeded.</returns>
        /// <remarks>Default true.</remarks>
        public bool TryValidateInput(string input, out string error)
        {
            error = String.Empty;

            if (_textureManager.GetTextureByToken($"{AlternativeTextures.TEXTURE_TOKEN_HEADER}{input}") is null)
            {
                if (_textureManager.GetModelByToken($"{AlternativeTextures.TEXTURE_TOKEN_HEADER}{input}") is TokenModel model && model is not null)
                {
                    _textureManager.UpdateTokenCache(model.Id);
                    return true;
                }

                error = $"No matching texture found for the given UNIQUE_ID.TEXTURE_NAME: {input}";
                return false;
            }

            return true;
        }

        /// <summary>Update the values when the context changes.</summary>
        /// <returns>Returns whether the value changed, which may trigger patch updates.</returns>
        public bool UpdateContext()
        {
            return true;
        }

        /// <summary>Get whether the token is available for use.</summary>
        public bool IsReady()
        {
            return _textureManager.GetValidTextureNamesWithSeason().Count > 0;
        }

        /// <summary>Get the current values.</summary>
        /// <param name="input">The input arguments, if any.</param>
        public IEnumerable<string> GetValues(string input)
        {
            if (!IsReady() || !_textureManager.GetValidTextureNamesWithSeason().Any(name => String.Equals(name, input, StringComparison.OrdinalIgnoreCase)))
                yield break;

            yield return $"{AlternativeTextures.TEXTURE_TOKEN_HEADER}{input}";
        }
    }
}
