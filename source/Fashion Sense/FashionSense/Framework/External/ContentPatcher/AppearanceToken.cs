/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/FashionSense
**
*************************************************/

using FashionSense.Framework.Managers;
using FashionSense.Framework.Utilities;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionSense.Framework.External.ContentPatcher
{
    class AppearanceToken
    {
        private readonly List<string> _validInputs = new List<string>()
        {
            "Hairstyle",
            "Accessory",
            "AccessorySecondary",
            "AccessoryTertiary",
            "Hat",
            "Shirt",
            "Pants",
            "Sleeves"
        };

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
            return _validInputs;
        }

        /// <summary>Validate that the provided input arguments are valid.</summary>
        /// <param name="input">The input arguments, if any.</param>
        /// <param name="error">The validation error, if any.</param>
        /// <returns>Returns whether validation succeeded.</returns>
        /// <remarks>Default true.</remarks>
        public bool TryValidateInput(string input, out string error)
        {
            error = String.Empty;

            if (!_validInputs.Contains(input, StringComparer.OrdinalIgnoreCase))
            {
                error = $"No matching appearance node under the name of: {input}";
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
            return Context.IsWorldReady;
        }

        /// <summary>Get the current values.</summary>
        /// <param name="input">The input arguments, if any.</param>
        public IEnumerable<string> GetValues(string input)
        {
            if (!IsReady() || !_validInputs.Contains(input, StringComparer.OrdinalIgnoreCase))
                yield break;

            string targetKey = null;
            switch (input.ToLower())
            {
                case "hairstyle":
                    targetKey = ModDataKeys.CUSTOM_HAIR_ID;
                    break;
                case "accessory":
                    targetKey = ModDataKeys.CUSTOM_ACCESSORY_ID;
                    break;
                case "accessorysecondary":
                    targetKey = ModDataKeys.CUSTOM_ACCESSORY_SECONDARY_ID;
                    break;
                case "accessorytertiary":
                    targetKey = ModDataKeys.CUSTOM_ACCESSORY_TERTIARY_ID;
                    break;
                case "hat":
                    targetKey = ModDataKeys.CUSTOM_HAT_ID;
                    break;
                case "shirt":
                    targetKey = ModDataKeys.CUSTOM_SHIRT_ID;
                    break;
                case "pants":
                    targetKey = ModDataKeys.CUSTOM_PANTS_ID;
                    break;
                case "sleeves":
                    targetKey = ModDataKeys.CUSTOM_SLEEVES_ID;
                    break;
            }

            if (targetKey is null || !Game1.player.modData.ContainsKey(targetKey))
                yield break;

            yield return Game1.player.modData[targetKey];
        }
    }
}
