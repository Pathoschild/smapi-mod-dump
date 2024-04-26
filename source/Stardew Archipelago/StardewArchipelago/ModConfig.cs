/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

namespace StardewArchipelago
{
    public class ModConfig
    { 
        /// <summary>
        /// When enabled, Pierre and Sandy sell limited stocks of seeds.
        /// Jojamart sells cheaper seeds, but forces you to purchase large packs instead of one at a time
        /// </summary>
        public bool EnableSeedShopOverhaul { get; set; } = true;

        /// <summary>
        /// Automatically hides the archipelago letters that are "empty".
        /// A letter is considered empty when it contains neither a physical item, nor a code snippet to run
        /// Examples include NPC Hearts, Seasons, Carpenter building unlocks, etc
        /// </summary>
        public bool HideEmptyArchipelagoLetters { get; set; } = false;

        /// <summary>
        /// When the archipelago icon shows up in-game, two version of the icons are available.
        /// The default are the "flat" icons, that come from Archipelago itself and are used in many games
        /// The Custom ones are made by candycaneannihalator and are intended to ressemble the Stardew Valley style
        /// </summary>
        public bool UseCustomArchipelagoIcons { get; set; } = false;

        /// <summary>
        /// Skips all Zelda-style animations where the character holds an item above their head
        /// </summary>
        public bool SkipHoldUpAnimations { get; set; } = false;

        /// <summary>
        /// Disables decaying of friendship points
        /// On friendsanity, this applies to earned points
        /// Outside of friendsanity, this applies to real points (hearts)
        /// </summary>
        public bool DisableFriendshipDecay { get; set; } = false;
    }
}