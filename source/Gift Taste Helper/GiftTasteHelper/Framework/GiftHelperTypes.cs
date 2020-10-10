/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tstaples/GiftTasteHelper
**
*************************************************/

using StardewValley;

namespace GiftTasteHelper.Framework
{
    internal enum GiftHelperType
    {
        Calendar,
        SocialPage
    }

    internal enum GiftTaste : int
    {
        Love    = NPC.gift_taste_love,
        Like    = NPC.gift_taste_like,
        Dislike = NPC.gift_taste_dislike,
        Hate    = NPC.gift_taste_hate,
        Neutral = NPC.gift_taste_neutral,
        MAX
    }
}
