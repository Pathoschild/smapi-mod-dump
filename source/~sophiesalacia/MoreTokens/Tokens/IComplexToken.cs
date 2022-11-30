/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sophiesalacia/StardewMods
**
*************************************************/

#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Xml;
using HarmonyLib;
using StardewValley;

namespace MoreTokens.Tokens;

internal interface IComplexToken
{
    internal Dictionary<string, string> CurrentArguments { get; set; }

    string GetName();

    /*
     * Metadata
     */
    /// <summary>Get whether the values may change depending on the context.</summary>
    /// <remarks>Default true.</remarks>
    bool IsMutable();

    /// <summary>Get whether the token allows input arguments (e.g. an NPC name for a relationship token).</summary>
    /// <remarks>Default false.</remarks>
    bool AllowsInput();

    /// <summary>Whether the token requires input arguments to work, and does not provide values without it (see <see cref="AllowsInput"/>).</summary>
    /// <remarks>Default false.</remarks>
    bool RequiresInput();

    /// <summary>Whether the token may return multiple values for the given input.</summary>
    /// <param name="input">The input arguments, if any.</param>
    /// <remarks>Default true.</remarks>
    bool CanHaveMultipleValues(string? input = null);

    /// <summary>Get the set of valid input arguments if restricted, or an empty collection if unrestricted.</summary>
    /// <remarks>Default unrestricted.</remarks>
    IEnumerable<string> GetValidInputs();

    /// <summary>Get whether the token always chooses from a set of known values for the given input. Mutually exclusive with <see cref="HasBoundedRangeValues"/>.</summary>
    /// <param name="input">The input arguments, if any.</param>
    /// <param name="allowedValues">The possible values for the input.</param>
    /// <remarks>Default unrestricted.</remarks>
    bool HasBoundedValues(string? input, out IEnumerable<string> allowedValues);

    /// <summary>Get whether the token always returns a value within a bounded numeric range for the given input. Mutually exclusive with <see cref="HasBoundedValues"/>.</summary>
    /// <param name="input">The input arguments, if any.</param>
    /// <param name="min">The minimum value this token may return.</param>
    /// <param name="max">The maximum value this token may return.</param>
    /// <remarks>Default false.</remarks>
    bool HasBoundedRangeValues(string? input, out int min, out int max);

    /// <summary>Validate that the provided input arguments are valid.</summary>
    /// <param name="input">The input arguments, if any.</param>
    /// <param name="error">The validation error, if any.</param>
    /// <returns>Returns whether validation succeeded.</returns>
    /// <remarks>Default true.</remarks>
    bool TryValidateInput(string? input, [NotNullWhen(false)] out string? error);

    /// <summary>Validate that the provided values are valid for the given input arguments (regardless of whether they match).</summary>
    /// <param name="input">The input arguments, if any.</param>
    /// <param name="values">The values to validate.</param>
    /// <param name="error">The validation error, if any.</param>
    /// <returns>Returns whether validation succeeded.</returns>
    /// <remarks>Default true.</remarks>
    bool TryValidateValues(string? input, IEnumerable<string> values, [NotNullWhen(false)] out string? error);

    /// <summary>Normalize a token value so it matches the format expected by the value provider, if needed.</summary>
    /// <param name="value">This receives the raw value, already trimmed and non-empty.</param>
    string NormalizeValue(string value);


    /*
     * State
     */
    /// <summary>Update the values when the context changes.</summary>
    /// <returns>Returns whether the value changed, which may trigger patch updates.</returns>
    bool UpdateContext();

    /// <summary>Get whether the token is available for use.</summary>
    bool IsReady();

    /// <summary>Get the current values.</summary>
    /// <param name="input">The input arguments, if any.</param>
    IEnumerable<string> GetValues(string? input);

    /*
     * Helper
     */
    /// <summary>
    /// Parse the input string to a set of named arguments.
    /// </summary>
    /// <param name="input">The input in raw string form.</param>
    /// <param name="args">The output arguments with key as argument name and value as argument value.</param>
    /// <param name="error">The parsing error, if any.</param>
    /// <returns>Returns whether parsing succeeded.</returns>
    internal bool TryParse(string input, out Dictionary<string, string> args, [NotNullWhen(false)] out string? error)
    {
        args = new Dictionary<string, string>();
        error = null;

        string[] split = input.Split('|');

        foreach (string arg in split)
        {
            string[] argSplit = arg.Split('=', StringSplitOptions.RemoveEmptyEntries);
            
            if (argSplit.Length != 2)
            {
                error = $"Malformed argument provided: '{arg}'";
                return false;
            }

            string argName = argSplit[0].Trim();
            string argValue = argSplit[1].Trim();

            if (argName.Equals("player", StringComparison.OrdinalIgnoreCase) && args.ContainsKey("player"))
            {
                error = "Too many player arguments specified";
                return false;
            }

            args.Add(argName, argValue);
        }

        return true;
    }
}
