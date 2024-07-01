/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using StardewArchipelago.Archipelago;

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
        /// All Archipelago letters will not use a very short and concise format
        /// instead of the funny ones full of fluff
        /// </summary>
        public bool DisableLetterTemplates { get; set; } = false;

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
        /// Disables decaying of friendship points.
        /// On friendsanity, this applies to earned points.
        /// Outside of friendsanity, this applies to real points (hearts).
        /// </summary>
        public bool DisableFriendshipDecay { get; set; } = false;

        /// <summary>
        /// Amount of speed gained per movement speed bonus received
        /// Multiplied by 0.05 in-game
        /// </summary>
        public int BonusPerMovementSpeed { get; set; } = 5;

        /// <summary>
        /// Which Item messages should be displayed in the game chatbox
        /// </summary>
        public ChatItemsFilter DisplayItemsInChat { get; set; } = ChatItemsFilter.RelatedToMe;

        /// <summary>
        /// Should the join and leave messages of other players be displayed in the game chatbox
        /// </summary>
        public bool EnableConnectionMessages { get; set; } = true;

        /// <summary>
        /// Should the chat messages of other players be displayed in the game chatbox
        /// </summary>
        public bool EnableChatMessages { get; set; } = true;

        /// <summary>
        /// Whether to display archipelago icons on the calendar for dates where the player has checks to do
        /// </summary>
        public bool ShowCalendarIndicators { get; set; } = true;

        /// <summary>
        /// Whether to display archipelago icons on the mine elevator menu for floors where the player has checks to do
        /// </summary>
        public bool ShowElevatorIndicators { get; set; } = true;

        /// <summary>
        /// Whether to display archipelago icons on inventory items for items that the player can do checks with
        /// </summary>
        public ItemIndicatorPreference ShowItemIndicators { get; set; } = ItemIndicatorPreference.True;
    }

    public enum ItemIndicatorPreference
    {
        False = 0,
        True = 1,
        OnlyShipsanity = 2,
    }
}