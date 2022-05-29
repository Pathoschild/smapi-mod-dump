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

namespace Pathoschild.Stardew.Common
{
    /// <summary>Provides extension methods for objects.</summary>
    internal static class ObjectExtensions
    {
        /// <summary>Throw an exception if the given value is null.</summary>
        /// <param name="value">The value to test.</param>
        /// <param name="paramName">The name of the parameter being tested.</param>
        /// <returns>Returns the value if non-null.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="value"/> is null.</exception>
        public static T AssertNotNull<T>(this T? value, string? paramName = null)
            where T : class
        {
            if (value is null)
                throw new ArgumentNullException(paramName);

            return value;
        }
    }
}
