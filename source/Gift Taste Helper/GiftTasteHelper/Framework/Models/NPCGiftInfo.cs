using System.Collections.Generic;

namespace GiftTasteHelper.Framework
{
    internal class NpcGiftInfo
    {
        public Dictionary<GiftTaste, ItemData[]> Gifts = new Dictionary<GiftTaste, ItemData[]>();
        public Dictionary<GiftTaste, ItemData[]> UniversalGifts = new Dictionary<GiftTaste, ItemData[]>();
    }
}
