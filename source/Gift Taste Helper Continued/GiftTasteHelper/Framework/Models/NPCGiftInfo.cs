/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Just-Chaldea/GiftTasteHelper
**
*************************************************/

using System.Collections.Generic;

namespace GiftTasteHelper.Framework
{
    internal class NpcGiftInfo
    {
        public Dictionary<GiftTaste, ItemData[]> Gifts = new Dictionary<GiftTaste, ItemData[]>();
        public Dictionary<GiftTaste, ItemData[]> UniversalGifts = new Dictionary<GiftTaste, ItemData[]>();
    }
}
