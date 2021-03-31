/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/cantorsdust/StardewMods
**
*************************************************/

namespace TimeSpeed.Framework
{
    /// <summary>Represents a general location type relative to <see cref="StardewValley.GameLocation.IsOutdoors"/>.</summary>
    internal enum LocationType
    {
        /// <summary>The location is inside a building.</summary>
        Indoors,

        /// <summary>The location is outside.</summary>
        Outdoors,

        /// <summary>The generated locations from the Deep Woods mod.</summary>
        DeepWoods,

        /// <summary>The normal mines.</summary>
        Mine,

        /// <summary>The skull cavern.</summary>
        SkullCavern,

        /// <summary>The volcano dungeon.</summary>
        VolcanoDungeon
    }
}
