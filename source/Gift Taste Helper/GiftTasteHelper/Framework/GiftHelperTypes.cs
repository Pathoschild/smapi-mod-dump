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
