namespace GiftTasteHelper.Framework
{
    internal class GiftConfig
    {
        /// <summary>If ShowOnlyKnownGifts is enabled, then hide the tooltip if no gifts are known for that NPC.</summary>
        public bool HideTooltipWhenNoGiftsKnown { get; set; } = false;

        /// <summary>The maximum number of gifts to display on the tooltip (or 0 for unlimited).</summary>
        public int MaxGiftsToDisplay { get; set; } = 0;

        /// <summary>Should universal gifts be shown on the tooltip.</summary>
        public bool ShowUniversalGifts { get; set; } = true;

        /// <summary>Should the names of universal gifts be a different color.</summary>
        public bool ColorizeUniversalGiftNames { get; set; } = false;

        /// <summary>Should gift info for NPCs that haven't been met be displayed.</summary>
        public bool ShowGiftsForUnmetNPCs { get; set; } = false;
    }

    internal class ModConfig : GiftConfig
    {
        /// <summary>Whether the tooltip should be displayed on the calendar.</summary>
        public bool ShowOnCalendar { get; set; } = true;

        /// <summary>Whether the tooltip should be displayed on the social page.</summary>
        public bool ShowOnSocialPage { get; set; } = true;

        /// <summary>Show only favourite gifts that you haven given to that NPC instead of all of them.</summary>
        public bool ShowOnlyKnownGifts { get; set; } = false;

        /// <summary>Should your gift progress be saved across all saves, or unique per save.</summary>
        public bool ShareKnownGiftsWithAllSaves { get; set; } = true;
    }
}
