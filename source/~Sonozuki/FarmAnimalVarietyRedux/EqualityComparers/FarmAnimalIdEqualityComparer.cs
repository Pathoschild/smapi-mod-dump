/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sonozuki/StardewMods
**
*************************************************/

using StardewValley;
using System.Collections.Generic;

namespace FarmAnimalVarietyRedux.EqualityComparers
{
    /// <summary>Defines how two <see cref="FarmAnimal"/>s should be compared.</summary>
    /// <remarks>This only uses the <see cref="FarmAnimal.myId"/> and disregards every other property such as ownerId, etc.</remarks>
    internal class FarmAnimalIdEqualityComparer : IEqualityComparer<FarmAnimal>
    {
        /*********
        ** Public Methods
        *********/
        /// <inheritdoc/>
        public bool Equals(FarmAnimal x, FarmAnimal y) => x?.myID == y?.myID;

        /// <inheritdoc/>
        public int GetHashCode(FarmAnimal obj) => (obj?.myID).GetHashCode();
    }
}
