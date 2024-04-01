/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Bouhm/stardew-valley-mods
**
*************************************************/

namespace NPCMapLocations.Framework.Models
{
    /// <summary>Which villagers should be visible on the world map.</summary>
    public enum VillagerVisibility
    {
        /// <summary>Show all villagers on the map.</summary>
        All = 1,

        /// <summary>Show villagers the player has talked to today.</summary>
        TalkedTo,

        /// <summary>Show villagers the player has not talked to today.</summary>
        NotTalkedTo
    }
}
