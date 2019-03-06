namespace GiftTasteHelper.Framework
{
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
