/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Bouhm/stardew-valley-mods
**
*************************************************/

namespace NPCMapLocations.Framework
{
    /// <summary>The base data for an NPC marker on the map synchronized from the host player.</summary>
    public class SyncedNpcMarker
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The NPC's translated display name.</summary>
        public string DisplayName { get; set; }

        /// <summary>The name of the location containing the NPC.</summary>
        public string LocationName { get; set; }

        /// <summary>The NPC's marker position on the world map.</summary>
        public WorldMapPosition WorldMapPosition { get; set; }

        /// <summary>Whether the NPC's birthday is today.</summary>
        public bool IsBirthday { get; set; }

        /// <summary>The NPC's character type.</summary>
        public CharacterType Type { get; set; } = CharacterType.Villager;
    }
}
