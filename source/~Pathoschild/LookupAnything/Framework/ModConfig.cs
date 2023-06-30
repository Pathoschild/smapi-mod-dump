/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

namespace Pathoschild.Stardew.LookupAnything.Framework
{
    /// <summary>The parsed mod configuration.</summary>
    internal class ModConfig
    {
        /*********
        ** Accessors
        *********/
        /****
        ** Common settings
        ****/
        /// <summary>The key bindings.</summary>
        public ModConfigKeys Controls { get; set; } = new();

        /// <summary>Whether to hide content until the player has discovered it in-game (where applicable).</summary>
        public bool ProgressionMode { get; set; }

        /// <summary>Whether to highlight item gift tastes which haven't been revealed in the NPC profile.</summary>
        public bool HighlightUnrevealedGiftTastes { get; set; }

        /// <summary>Which gift taste levels to show in NPC and item lookups.</summary>
        public ModGiftTasteConfig ShowGiftTastes { get; set; } = new();

        /// <summary>Whether to show gift tastes that the player doesn't own somewhere in the world.</summary>
        public bool ShowUnownedGifts { get; set; } = true;

        /// <summary>Whether to close the lookup UI when the lookup key is released.</summary>
        public bool HideOnKeyUp { get; set; }

        /****
        ** Advanced settings
        ****/
        /// <summary>Whether to look up the original entity when the game spawns a temporary copy.</summary>
        /// <remarks>In some cases the game spawns a temporary entity to represent another one. For example, Abigail in the mines is actually a temporary NPC with the name 'AbigailMine', so looking her up there won't show Abigail's real info. With this option enabled, Lookup Anything will look up the original Abigail instead.</remarks>
        public bool EnableTargetRedirection { get; set; } = true;

        /// <summary>Whether to include map tiles as lookup targets.</summary>
        public bool EnableTileLookups { get; set; }

        /// <summary>Whether the menu should always be full-screen, instead of centered in the window.</summary>
        public bool ForceFullScreen { get; set; } = false;

        /// <summary>The number of pixels to shift content on each up/down scroll.</summary>
        public int ScrollAmount { get; set; } = 160;

        /// <summary>Whether to show advanced data mining fields.</summary>
        public bool ShowDataMiningFields { get; set; }
    }
}
