/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sonozuki/StardewMods
**
*************************************************/

namespace BfavToFavrModConverter.Favr
{
    /// <summary>The harvest methods for animal produce.</summary>
    public enum HarvestType
    {
        /// <summary>The item is spawned in the building at the start of the morning for the player to pick up.</summary>
        Lay,

        /// <summary>The item requires the player to use a tool on the animal.</summary>
        Tool,

        /// <summary>The item is spawned outside the building for the player to pick up.</summary>
        Forage
    }
}
