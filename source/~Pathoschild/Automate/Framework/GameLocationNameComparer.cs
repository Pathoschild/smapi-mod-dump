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
using StardewValley;

namespace Pathoschild.Stardew.Automate.Framework
{
    /// <summary>A comparer which considers two locations equal if they have the same name.</summary>
    internal class GameLocationNameComparer : IEqualityComparer<GameLocation>
    {
        /*********
        ** Public methods
        *********/
        /// <summary>Determines whether the specified objects are equal.</summary>
        /// <returns>true if the specified objects are equal; otherwise, false.</returns>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        public bool Equals(GameLocation? left, GameLocation? right)
        {
            string? leftName = left?.NameOrUniqueName;
            string? rightName = right?.NameOrUniqueName;

            if (leftName is null)
                return rightName is null;

            if (rightName is null)
                return false;

            return leftName == rightName;
        }

        /// <summary>Get a hash code for the specified object.</summary>
        /// <param name="obj">The value.</param>
        public int GetHashCode(GameLocation obj)
        {
            return (obj.NameOrUniqueName ?? string.Empty).GetHashCode();
        }
    }
}
