/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tstaples/GiftTasteHelper
**
*************************************************/

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
}
