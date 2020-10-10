/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/silentoak/StardewMods
**
*************************************************/

using System.Runtime.CompilerServices;
using System.Collections.Generic;

/*** 
 * From Pathoschild.Stardew.Common.Utilities
 * see https://github.com/Pathoschild/StardewMods/blob/595c21818eea6ace20280180b17004153e9dacee/Common/Utilities/ObjectReferenceComparer.cs
 **/
namespace SilentOak.QualityProducts.Utils
{
    /// <summary>A comparer which considers two references equal if they point to the same instance.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    internal class ObjectReferenceComparer<T> : IEqualityComparer<T>
    {
        /*********
        ** Public methods
        *********/
        /// <summary>Determines whether the specified objects are equal.</summary>
        /// <returns>true if the specified objects are equal; otherwise, false.</returns>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        public bool Equals(T x, T y)
        {
            return ReferenceEquals(x, y);
        }

        /// <summary>Get a hash code for the specified object.</summary>
        /// <param name="obj">The value.</param>
        public int GetHashCode(T obj)
        {
            return RuntimeHelpers.GetHashCode(obj);
        }
    }
}
