/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pet-Slime/StardewValley
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Text;
using HarmonyLib;
using StardewValley;
using StardewValley.Mods;

namespace MoonShared
{
    public static class TypeExtensions
    {

        /// <summary>
        /// Apparently, in .NET Core, a hash code for a given string will be different between runs.
        /// https://andrewlock.net/why-is-string-gethashcode-different-each-time-i-run-my-program-in-net-core/
        /// This gets one that will be the same.
        /// </summary>
        /// <param name="str">The string to get the hash code of.</param>
        /// <returns>The deterministic hash code.</returns>
        public static int GetDeterministicHashCode(this string str)
        {
            unchecked
            {
                int hash1 = (5381 << 16) + 5381;
                int hash2 = hash1;

                for (int i = 0; i < str.Length; i += 2)
                {
                    hash1 = ((hash1 << 5) + hash1) ^ str[i];
                    if (i == str.Length - 1)
                        break;
                    hash2 = ((hash2 << 5) + hash2) ^ str[i + 1];
                }

                return hash1 + (hash2 * 1566083941);
            }
        }


        /// <summary>Gets a method and asserts that it was found.</summary>
        /// <param name="type">The <see cref="Type"/>.</param>
        /// <param name="name">The method name.</param>
        /// <param name="parameters">The method parameter types, or <c>null</c> if it's not overloaded.</param>
        /// <returns>The corresponding <see cref="MethodInfo"/>, if found.</returns>
        /// <exception cref="MissingMethodException">If a matching method is not found.</exception>
        [DebuggerStepThrough]
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        public static MethodInfo RequireMethod(this Type type, string name, Type[]? parameters)
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        {
            return AccessTools.Method(type, name, parameters);
        }

        /// <summary>Get a value from an array if it's in range.</summary>
        /// <typeparam name="T">The value type.</typeparam>
        /// <param name="array">The array to search.</param>
        /// <param name="index">The index of the value within the array to find.</param>
        /// <param name="value">The value at the given index, if found.</param>
        /// <returns>Returns whether the index was within the array bounds.</returns>
        public static bool TryGetIndex<T>(this T[] array, int index, out T value)
        {
            if (array == null || index < 0 || index >= array.Length)
            {
                value = default;
                return false;
            }

            value = array[index];
            return true;
        }

        /// <summary>Get a value from an array if it's in range, else get the default value.</summary>
        /// <typeparam name="T">The value type.</typeparam>
        /// <param name="array">The array to search.</param>
        /// <param name="index">The index of the value within the array to find.</param>
        /// <param name="defaultValue">The default value if the value isn't in range.</param>
        public static T GetOrDefault<T>(this T[] array, int index, T defaultValue = default)
        {
            return array.TryGetIndex(index, out T value)
                ? value
                : defaultValue;
        }

        /// <summary>Get a value from a list if it's in range, else get the default value.</summary>
        /// <typeparam name="T">The value type.</typeparam>
        /// <param name="list">The list to search.</param>
        /// <param name="index">The index of the value within the array to find.</param>
        /// <param name="defaultValue">The default value if the value isn't in range.</param>
        public static T GetOrDefault<T>(this IList<T> list, int index, T defaultValue = default)
        {
            return list.TryGetIndex(index, out T value)
                ? value
                : defaultValue;
        }

        /// <summary>Get a value from a list if it's in range.</summary>
        /// <typeparam name="T">The value type.</typeparam>
        /// <param name="list">The list to search.</param>
        /// <param name="index">The index of the value within the array to find.</param>
        /// <param name="value">The value at the given index, if found.</param>
        /// <returns>Returns whether the index was within the array bounds.</returns>
        public static bool TryGetIndex<T>(this IList<T> list, int index, out T value)
        {
            if (list == null || index < 0 || index >= list.Count)
            {
                value = default;
                return false;
            }

            value = list[index];
            return true;
        }

        /// <summary>Get a value from a list if it's in range, else get the default value.</summary>
        /// <typeparam name="T">The value type.</typeparam>
        /// <param name="list">The list to search.</param>
        /// <param name="random">the random seed needed.</param>
        /// <param name="defaultValue">The default value if the value isn't in range.</param>
        public static T RandomChoose<T>(this IList<T> list, Random random, T defaultValue = default)
        {
            if (list == null || list.Count <= 0)
            {
                return defaultValue;
            }

            return list[random.Next(list.Count)];
        }



        /// <summary>Shuffle a List for a random value.</summary>
        /// <typeparam name="T">The value type.</typeparam>
        /// <param name="list">The list to be shuffled.</param>
        /// <param name="random">The RNG to shuffle off of.</param>
        public static void Shuffle<T>(this IList<T> list, Random random)
        {
            for (int n = list.Count - 1; n > 0; --n)
            {
                int k = random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static bool IsPositive(this int number)
        {
            return number > 0;
        }

        public static bool IsNegative(this int number)
        {
            return number < 0;
        }

        public static bool IsZero(this int number)
        {
            return number == 0;
        }



    }

    /// <summary>Provides common utility methods for reading and writing to <see cref="ModDataDictionary"/> fields.</summary>
    public static class ModDataHelper
    {
        /*********
        ** Public methods
        *********/
        /****
        ** Bool
        ****/
        /// <summary>Read a boolean value from the mod data if it exists and is valid, else get the default value.</summary>
        /// <param name="data">The mod data dictionary.</param>
        /// <param name="key">The data key within the <paramref name="data"/>.</param>
        /// <param name="default">The default value if the field is missing or invalid.</param>
        public static bool GetBool(this ModDataDictionary data, string key, bool @default = false)
        {
            return data.TryGetValue(key, out string raw) && bool.TryParse(raw, out bool value)
                ? value
                : @default;
        }

        /// <summary>Write a boolean value into the mod data, or remove it if it matches the <paramref name="default"/>.</summary>
        /// <param name="data">The mod data dictionary.</param>
        /// <param name="key">The data key within the <paramref name="data"/>.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="default">The default value if the field is missing or invalid. If the value matches the default, it won't be written to the data to avoid unneeded serialization and network sync.</param>
        public static void SetBool(this ModDataDictionary data, string key, bool value, bool @default = false)
        {
            if (value == @default)
                data.Remove(key);
            else
                data[key] = value.ToString(CultureInfo.InvariantCulture);
        }

        /****
        ** Float
        ****/
        /// <summary>Read a float value from the mod data if it exists and is valid, else get the default value.</summary>
        /// <param name="data">The mod data dictionary.</param>
        /// <param name="key">The data key within the <paramref name="data"/>.</param>
        /// <param name="default">The default value if the field is missing or invalid.</param>
        /// <param name="min">The minimum value to consider valid, or <c>null</c> to allow any value.</param>
        public static float GetFloat(this ModDataDictionary data, string key, float @default = 0, float? min = null)
        {
            return data.TryGetValue(key, out string raw) && float.TryParse(raw, out float value) && value >= min
                ? value
                : @default;
        }

        /// <summary>Write a float value into the mod data, or remove it if it matches the <paramref name="default"/>.</summary>
        /// <param name="data">The mod data dictionary.</param>
        /// <param name="key">The data key within the <paramref name="data"/>.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="default">The default value if the field is missing or invalid. If the value matches the default, it won't be written to the data to avoid unneeded serialization and network sync.</param>
        /// <param name="min">The minimum value to consider valid, or <c>null</c> to allow any value.</param>
        /// <param name="max">The maximum value to consider valid, or <c>null</c> to allow any value.</param>
        [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator", Justification = "We're comparing to a marker value, so precision loss isn't an issue.")]
        public static void SetFloat(this ModDataDictionary data, string key, float value, float @default = 0, float? min = null, float? max = null)
        {
            if (value < min)
                value = min.Value;
            if (value > max)
                value = max.Value;

            if (value == @default)
                data.Remove(key);
            else
                data[key] = value.ToString(CultureInfo.InvariantCulture);
        }

        /****
        ** Int
        ****/
        /// <summary>Read an integer value from the mod data if it exists and is valid, else get the default value.</summary>
        /// <param name="data">The mod data dictionary.</param>
        /// <param name="key">The data key within the <paramref name="data"/>.</param>
        /// <param name="default">The default value if the field is missing or invalid.</param>
        /// <param name="min">The minimum value to consider valid, or <c>null</c> to allow any value.</param>
        public static int GetInt(this ModDataDictionary data, string key, int @default = 0, int? min = null)
        {
            return data.TryGetValue(key, out string raw) && int.TryParse(raw, out int value) && value >= min
                ? value
                : @default;
        }

        /// <summary>Write an integer value into the mod data, or remove it if it matches the <paramref name="default"/>.</summary>
        /// <param name="data">The mod data dictionary.</param>
        /// <param name="key">The data key within the <paramref name="data"/>.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="default">The default value if the field is missing or invalid. If the value matches the default, it won't be written to the data to avoid unneeded serialization and network sync.</param>
        /// <param name="min">The minimum value to consider valid, or <c>null</c> to allow any value.</param>
        public static void SetInt(this ModDataDictionary data, string key, int value, int @default = 0, int? min = null)
        {
            if (value == @default || value <= min)
                data.Remove(key);
            else
                data[key] = value.ToString(CultureInfo.InvariantCulture);
        }

        /****
        ** Custom
        ****/
        /// <summary>Read a value from the mod data with custom parsing if it exists and can be parsed, else get the default value.</summary>
        /// <typeparam name="T">The value type.</typeparam>
        /// <param name="data">The mod data dictionary.</param>
        /// <param name="key">The data key within the <paramref name="data"/>.</param>
        /// <param name="parse">Parse the raw value.</param>
        /// <param name="default">The default value if the field is missing or invalid.</param>
        /// <param name="suppressError">Whether to return the default value if <paramref name="parse"/> throws an exception; else rethrow it.</param>
        public static T GetCustom<T>(this ModDataDictionary data, string key, Func<string, T> parse, T @default = default, bool suppressError = true)
        {
            if (!data.TryGetValue(key, out string raw))
                return @default;

            try
            {
                return parse(raw);
            }
            catch when (suppressError)
            {
                return @default;
            }
        }

        /// <summary>Write a value into the mod data with custom serialization, or remove it if it serializes to null or an empty string.</summary>
        /// <typeparam name="T">The value type.</typeparam>
        /// <param name="data">The mod data dictionary.</param>
        /// <param name="key">The field key.</param>
        /// <param name="value">The value to save.</param>
        /// <param name="serialize">Serialize the value to its string representation.</param>
        public static void SetCustom<T>(this ModDataDictionary data, string key, T value, Func<T, string> serialize = null)
        {
            string serialized = serialize != null
                ? serialize(value)
                : value?.ToString();

            if (string.IsNullOrWhiteSpace(serialized))
                data.Remove(key);
            else
                data[key] = serialized;
        }
    }
}
