/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

using StardewValley;

namespace Pathoschild.Stardew.LookupAnything.Framework.Constants
{
    /// <summary>Indicates how much an NPC likes a particular gift.</summary>
    internal enum GiftTaste
    {
        Hate = NPC.gift_taste_hate,
        Dislike = NPC.gift_taste_dislike,
        Neutral = NPC.gift_taste_neutral,
        Like = NPC.gift_taste_like,
        Love = NPC.gift_taste_love
    }
}
