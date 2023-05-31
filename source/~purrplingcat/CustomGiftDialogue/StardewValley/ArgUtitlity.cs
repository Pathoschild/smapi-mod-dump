/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/StardewMods
**
*************************************************/

using System;
using System.Linq;
using Microsoft.Xna.Framework;

namespace StardewValley
{
    /// <summary>A utility for working with space-delimited or split argument lists.</summary>
    public static class ArgUtility
    {
        /// <summary>Split a list of space-separated arguments (ignoring surrounding whitespace and accidental double spaces).</summary>
        /// <param name="value">The value to split.</param>
        /// <returns>Returns an array of the space-delimited arguments, or an empty array if the <paramref name="value" /> was null or empty.</returns>
        public static string[] SplitBySpace(string value)
        {
            return value?.Split(' ', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();
        }

        /// <summary>Split a list of space-separated arguments (ignoring surrounding whitespace and accidental double spaces).</summary>
        /// <param name="value">The value to split.</param>
        /// <param name="limit">The number of arguments to return. Any remaining arguments by appended to the final argument.</param>
        /// <returns>Returns an array of the space-delimited arguments, or an empty array if the <paramref name="value" /> was null or empty.</returns>
        public static string[] SplitBySpace(string value, int limit)
        {
            return value?.Split(' ', limit, StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();
        }

        /// <summary>Split a list of space-separated arguments (ignoring surrounding whitespace and accidental double spaces) and get a specific argument.</summary>
        /// <param name="value">The value to split.</param>
        /// <param name="index">The index of the value to get.</param>
        /// <param name="defaultValue">The value to return if the <paramref name="index" /> is out of range for the array.</param>
        /// <returns>Returns the value at the given index if the array was non-null and the index is in range, else the <paramref name="defaultValue" />.</returns>
        public static string SplitBySpaceAndGet(string value, int index, string defaultValue = null)
        {
            if (value == null)
            {
                return defaultValue;
            }
            return Get(value.Split(' ', index + 2, StringSplitOptions.RemoveEmptyEntries), index, defaultValue);
        }

        /// <summary>Get whether an index is within the bounds of the array, regardless of what value is at that position.</summary>
        /// <param name="array">The array of arguments to check.</param>
        /// <param name="index">The index to check within the <paramref name="array" />.</param>
        public static bool HasIndex(string[] array, int index)
        {
            if (index >= 0)
            {
                if (array == null)
                {
                    return false;
                }
                return array.Length > index;
            }
            return false;
        }

        /// <summary>Get a string argument by its array index.</summary>
        /// <param name="array">The array of arguments to read.</param>
        /// <param name="index">The index to get within the <paramref name="array" />.</param>
        /// <param name="defaultValue">The value to return if the index is out of bounds or invalid.</param>
        /// <param name="allowBlank">Whether to return the argument even if it's null or whitespace. If false, the <paramref name="defaultValue" /> will be returned in that case.</param>
        /// <returns>Returns the selected argument (if the <paramref name="index" /> is found and valid), else <paramref name="defaultValue" />.</returns>
        public static string Get(string[] array, int index, string defaultValue = null, bool allowBlank = true)
        {
            if (index >= 0 && index < array?.Length)
            {
                string value = array[index];
                if (allowBlank || !string.IsNullOrWhiteSpace(value))
                {
                    return value;
                }
            }
            return defaultValue;
        }

        /// <summary>Get an boolean argument by its array index.</summary>
        /// <inheritdoc cref="M:StardewValley.ArgUtility.Get(System.String[],System.Int32,System.String,System.Boolean)" />
        public static bool GetBool(string[] array, int index, bool defaultValue = false)
        {
            if (!bool.TryParse(Get(array, index), out var value))
            {
                return defaultValue;
            }
            return value;
        }

        /// <summary>Get a float argument by its array index.</summary>
        /// <inheritdoc cref="M:StardewValley.ArgUtility.Get(System.String[],System.Int32,System.String,System.Boolean)" />
        public static float GetFloat(string[] array, int index, float defaultValue = 0f)
        {
            if (!float.TryParse(Get(array, index), out var value))
            {
                return defaultValue;
            }
            return value;
        }

        /// <summary>Get an integer argument by its array index.</summary>
        /// <inheritdoc cref="M:StardewValley.ArgUtility.Get(System.String[],System.Int32,System.String,System.Boolean)" />
        public static int GetInt(string[] array, int index, int defaultValue = 0)
        {
            if (!int.TryParse(Get(array, index), out var value))
            {
                return defaultValue;
            }
            return value;
        }

        /// <summary>Get a string argument by its array index, if it's found and valid.</summary>
        /// <param name="array">The array of arguments to read.</param>
        /// <param name="index">The index to get within the <paramref name="array" />.</param>
        /// <param name="value">The argument value, if found and valid.</param>
        /// <param name="error">An error phrase indicating why getting the argument failed (like 'required index X not found'), if applicable.</param>
        /// <param name="allowBlank">Whether to match the argument even if it's null or whitespace. If false, it will be treated as invalid in that case.</param>
        /// <returns>Returns whether the argument was successfully found and is valid.</returns>
        public static bool TryGet(string[] array, int index, out string value, out string error, bool allowBlank = true)
        {
            if (array == null)
            {
                value = null;
                error = "argument list is null";
                return false;
            }
            if (index < 0 || index >= array.Length)
            {
                value = null;
                error = $"required index {index} not found (list has 0 to {array.Length - 1})";
                return false;
            }
            value = array[index];
            if (!allowBlank && string.IsNullOrWhiteSpace(value))
            {
                value = null;
                error = $"required index {index} has a blank value";
                return false;
            }
            error = null;
            return true;
        }

        /// <summary>Get a boolean argument by its array index, if it's found and valid.</summary>
        /// <inheritdoc cref="M:StardewValley.ArgUtility.TryGet(System.String[],System.Int32,System.String@,System.String@,System.Boolean)" />
        public static bool TryGetBool(string[] array, int index, out bool value, out string error)
        {
            if (!TryGet(array, index, out var raw, out error, allowBlank: false))
            {
                value = false;
                return false;
            }
            if (!bool.TryParse(raw, out value))
            {
                value = false;
                error = $"required index {index} has value '{raw}', which can't be parsed as a boolean";
                return false;
            }
            error = null;
            return true;
        }

        /// <summary>Get a float argument by its array index, if it's found and valid.</summary>
        /// <inheritdoc cref="M:StardewValley.ArgUtility.TryGet(System.String[],System.Int32,System.String@,System.String@,System.Boolean)" />
        public static bool TryGetFloat(string[] array, int index, out float value, out string error)
        {
            if (!TryGet(array, index, out var raw, out error, allowBlank: false))
            {
                value = -1f;
                return false;
            }
            if (!float.TryParse(raw, out value))
            {
                value = -1f;
                error = $"required index {index} has value '{raw}', which can't be parsed as a float";
                return false;
            }
            error = null;
            return true;
        }

        /// <summary>Get an integer argument by its array index, if it's found and valid.</summary>
        /// <inheritdoc cref="M:StardewValley.ArgUtility.TryGet(System.String[],System.Int32,System.String@,System.String@,System.Boolean)" />
        public static bool TryGetInt(string[] array, int index, out int value, out string error)
        {
            if (!TryGet(array, index, out var raw, out error, allowBlank: false))
            {
                value = -1;
                return false;
            }
            if (!int.TryParse(raw, out value))
            {
                value = -1;
                error = $"required index {index} has value '{raw}', which can't be parsed as an integer";
                return false;
            }
            error = null;
            return true;
        }

        /// <summary>Get a point argument by its array index, if it's found and valid. This reads two consecutive values starting from <paramref name="index" /> for the X and Y values.</summary>
        /// <inheritdoc cref="M:StardewValley.ArgUtility.TryGet(System.String[],System.Int32,System.String@,System.String@,System.Boolean)" />
        public static bool TryGetPoint(string[] array, int index, out Point value, out string error)
        {
            if (!TryGetInt(array, index, out var x, out error) || !TryGetInt(array, index + 1, out var y, out error))
            {
                value = Point.Zero;
                return false;
            }
            error = null;
            value = new Point(x, y);
            return true;
        }

        /// <summary>Get a vector argument by its array index, if it's found and valid. This reads two consecutive values starting from <paramref name="index" /> for the X and Y values.</summary>
        /// <param name="array">The array of arguments to read.</param>
        /// <param name="index">The index to get within the <paramref name="array" />.</param>
        /// <param name="value">The argument value, if found and valid.</param>
        /// <param name="error">An error phrase indicating why getting the argument failed (like 'required index X not found'), if applicable.</param>
        /// <param name="integerOnly">Whether the X and Y values must be integers.</param>
        /// <returns>Returns whether the argument was successfully found and is valid.</returns>
        public static bool TryGetVector2(string[] array, int index, out Vector2 value, out string error, bool integerOnly = false)
        {
            float x;
            float y;
            if (integerOnly)
            {
                if (TryGetInt(array, index, out var x2, out error) && TryGetInt(array, index + 1, out var y2, out error))
                {
                    value = new Vector2(x2, y2);
                    return true;
                }
            }
            else if (TryGetFloat(array, index, out x, out error) && TryGetFloat(array, index + 1, out y, out error))
            {
                value = new Vector2(x, y);
                return true;
            }
            value = Vector2.Zero;
            return false;
        }

        /// <summary>Get a rectangle argument by its array index, if it's found and valid. This reads four consecutive values starting from <paramref name="index" /> for the X, Y, width, and height values.</summary>
        /// <inheritdoc cref="M:StardewValley.ArgUtility.TryGet(System.String[],System.Int32,System.String@,System.String@,System.Boolean)" />
        public static bool TryGetRectangle(string[] array, int index, out Rectangle value, out string error)
        {
            if (!TryGetInt(array, index, out var x, out error) || !TryGetInt(array, index + 1, out var y, out error) || !TryGetInt(array, index + 2, out var width, out error) || !TryGetInt(array, index + 3, out var height, out error))
            {
                value = Rectangle.Empty;
                return false;
            }
            error = null;
            value = new Rectangle(x, y, width, height);
            return true;
        }

        /// <summary>Get all arguments starting from the given index as a concatenated string, if the index is found.</summary>
        /// <param name="array">The array of arguments to read.</param>
        /// <param name="index">The index of the first argument to include within the <paramref name="array" />.</param>
        /// <param name="value">The concatenated argument values, if found and valid.</param>
        /// <param name="error">An error phrase indicating why getting the argument failed (like 'required index X not found'), if applicable.</param>
        /// <param name="delimiter">The delimiter with which to concatenate values.</param>
        /// <returns>Returns whether at least one argument was successfully found.</returns>
        public static bool TryGetRemainder(string[] array, int index, out string value, out string error, char delimiter = ' ')
        {
            if (array == null)
            {
                value = null;
                error = "argument list is null";
                return false;
            }
            if (index < 0 || index >= array.Length)
            {
                value = null;
                error = $"required index {index} not found (list has 0 to {array.Length - 1})";
                return false;
            }
            value = string.Join(delimiter, array.Skip(index));
            error = null;
            return true;
        }

        /// <summary>Get a string argument by its array index, or a default value if the argument isn't found.</summary>
        /// <param name="array">The array of arguments to read.</param>
        /// <param name="index">The index to get within the <paramref name="array" />.</param>
        /// <param name="value">The argument value, if found and valid.</param>
        /// <param name="defaultValue">The value to return if the index is out of bounds or invalid.</param>
        /// <param name="allowBlank">Whether to match the argument even if it's null or whitespace. If false, it will be treated as invalid in that case.</param>
        /// <param name="error">An error phrase indicating why getting the argument failed (like 'required index X not found'), if applicable.</param>
        /// <returns>Returns true if either (a) the argument was found and valid, or (b) the argument was not found so the default value was used. Returns false if the argument was found but isn't in a valid format.</returns>
        public static bool TryGetOptional(string[] array, int index, out string value, string defaultValue = null, bool allowBlank = true)
        {
            if (array == null || index < 0 || index >= array.Length)
            {
                value = defaultValue;
                return true;
            }
            value = array[index];
            if (!allowBlank && string.IsNullOrWhiteSpace(value))
            {
                value = defaultValue;
                return false;
            }
            return true;
        }

        /// <summary>Get a boolean argument by its array index, or a default value if the argument isn't found.</summary>
        /// <inheritdoc cref="M:StardewValley.ArgUtility.TryGetOptional(System.String[],System.Int32,System.String@,System.String,System.Boolean)" />
        public static bool TryGetOptionalBool(string[] array, int index, out bool value, out string error, bool defaultValue = false)
        {
            if (array == null || index < 0 || index >= array.Length)
            {
                error = null;
                value = defaultValue;
                return true;
            }
            if (!bool.TryParse(array[index], out value))
            {
                error = $"optional index {index} has value '{array[index]}', which can't be parsed as a boolean";
                value = defaultValue;
                return false;
            }
            error = null;
            return true;
        }

        /// <summary>Get a float argument by its array index, or a default value if the argument isn't found.</summary>
        /// <inheritdoc cref="M:StardewValley.ArgUtility.TryGetOptional(System.String[],System.Int32,System.String@,System.String,System.Boolean)" />
        public static bool TryGetOptionalFloat(string[] array, int index, out float value, out string error, float defaultValue = 0f)
        {
            if (array == null || index < 0 || index >= array.Length)
            {
                error = null;
                value = defaultValue;
                return true;
            }
            if (!float.TryParse(array[index], out value))
            {
                error = $"optional index {index} has value '{array[index]}', which can't be parsed as a float";
                value = defaultValue;
                return false;
            }
            error = null;
            return true;
        }

        /// <summary>Get an int argument by its array index, or a default value if the argument isn't found.</summary>
        /// <inheritdoc cref="M:StardewValley.ArgUtility.TryGetOptional(System.String[],System.Int32,System.String@,System.String,System.Boolean)" />
        public static bool TryGetOptionalInt(string[] array, int index, out int value, out string error, int defaultValue = 0)
        {
            if (array == null || index < 0 || index >= array.Length)
            {
                error = null;
                value = defaultValue;
                return true;
            }
            if (!int.TryParse(array[index], out value))
            {
                error = $"optional index {index} has value '{array[index]}', which can't be parsed as an integer";
                value = defaultValue;
                return false;
            }
            error = null;
            return true;
        }

        /// <summary>Get all arguments starting from the given index as a concatenated string, or a default value if the index isn't in the array.</summary>
        /// <param name="array">The array of arguments to read.</param>
        /// <param name="index">The index of the first argument to include within the <paramref name="array" />.</param>
        /// <param name="value">The concatenated argument values, if found and valid.</param>
        /// <param name="error">An error phrase indicating why getting the argument failed (like 'required index X not found'), if applicable.</param>
        /// <param name="defaultValue">The value to return if the index is out of bounds or invalid.</param>
        /// <param name="delimiter">The delimiter with which to concatenate values.</param>
        /// <returns>Returns whether at least one argument was successfully found.</returns>
        public static bool TryGetOptionalRemainder(string[] array, int index, out string value, out string error, string defaultValue = null, char delimiter = ' ')
        {
            if (array == null || index < 0 || index >= array.Length)
            {
                value = defaultValue;
                error = null;
                return true;
            }
            value = string.Join(delimiter, array.Skip(index));
            error = null;
            return true;
        }
    }
}