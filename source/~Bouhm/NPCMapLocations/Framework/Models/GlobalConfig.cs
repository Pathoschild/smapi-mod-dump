/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Bouhm/stardew-valley-mods
**
*************************************************/

using System.Collections.Generic;

namespace NPCMapLocations.Framework.Models
{
    /// <summary>The model for global config options.</summary>
    public class GlobalConfig
    {
        /*********
        ** Accessors
        *********/
        /// <summary>Whether to enable debug features.</summary>
        public bool DebugMode { get; set; } = false;

        /// <summary>The key binding to open the options menu when on the map view.</summary>
        public string MenuKey { get; set; } = "Tab";

        /// <summary>The key binding to cycle the tooltip position when on the map view.</summary>
        public string TooltipKey { get; set; } = "Space";

        /// <summary>The tooltip position when pointing at something on the map.</summary>
        public int NameTooltipMode { get; set; } = 1;

        /// <summary>Whether to only show villagers in the same location as the player.</summary>
        public bool OnlySameLocation { get; set; } = false;

        /// <summary>Whether to show villagers that would normally be hidden. </summary>
        public bool ShowHiddenVillagers { get; set; } = false;

        /// <summary>Whether to mark NPCs with quests or birthdays today.</summary>
        public bool ShowQuests { get; set; } = true;

        /// <summary>Whether to show the Traveling Merchant when she's in the forest.</summary>
        public bool ShowTravelingMerchant { get; set; } = true;

        /// <summary>Whether to show the floating minimap.</summary>
        public bool ShowMinimap { get; set; } = false;

        /// <summary>Whether to show farm buildings on the map.</summary>
        public bool ShowFarmBuildings { get; set; } = true;

        /// <summary>The key binding to toggle the floating minimap.</summary>
        public string MinimapToggleKey { get; set; } = "OemPipe";

        /// <summary>The minimap's pixel X position on screen.</summary>
        public int MinimapX { get; set; } = 12;

        /// <summary>The minimap's pixel Y position on screen.</summary>
        public int MinimapY { get; set; } = 12;

        /// <summary>The minimap's pixel width on screen.</summary>
        public int MinimapWidth { get; set; } = 75;

        /// <summary>The minimap's pixel height on screen.</summary>
        public int MinimapHeight { get; set; } = 45;

        /// <summary>Location names in which the minimap should be disabled.</summary>
        public HashSet<string> MinimapExclusions { get; set; } = new();

        /// <summary>Whether to show seasonal variations of the map.</summary>
        public bool UseSeasonalMaps { get; set; } = true;

        /// <summary>Whether to replace the stylized Ginger Island in the bottom-right corner of the map with a more detailed and accurate map.</summary>
        public bool UseDetailedIsland { get; set; } = false;

        /// <summary>Whether to show player children on the map.</summary>
        public bool ShowChildren { get; set; } = false;

        /// <summary>Whether to show horses on the map.</summary>
        public bool ShowHorse { get; set; } = true;

        /// <summary>NPC names to hide from the map.</summary>
        public HashSet<string> NpcExclusions { get; set; } = new();

        /// <summary>Custom offsets when drawing vanilla NPCs.</summary>
        public Dictionary<string, int> NpcMarkerOffsets { get; set; } = new();
    }
}
