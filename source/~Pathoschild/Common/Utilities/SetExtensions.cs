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

namespace Pathoschild.Stardew.Common.Utilities
{
    /// <summary>Provides extension methods for <see cref="ISet{T}"/>.</summary>
    internal static class SetExtensions
    {
        /*********
        ** Public methods
        *********/
        /// <summary>Add multiple elements to the current set.</summary>
        /// <typeparam name="T">The value type.</typeparam>
        /// <param name="set">The set to update.</param>
        /// <param name="values">The values to add.</param>
        /// <returns>Returns whether any of the values were successfully added.</returns>
        public static bool AddMany<T>(this ISet<T> set, IEnumerable<T> values)
        {
            bool anyAdded = false;

            foreach (T value in values)
                anyAdded |= set.Add(value);

            return anyAdded;
        }

        /// <summary>Add multiple elements to the current set.</summary>
        /// <typeparam name="T">The value type.</typeparam>
        /// <param name="set">The set to update.</param>
        /// <param name="values">The values to add.</param>
        /// <returns>Returns whether any of the values were successfully added.</returns>
        public static bool AddMany<T>(this ISet<T> set, params T[] values)
        {
            return set.AddMany((IEnumerable<T>)values);
        }
    }
}
