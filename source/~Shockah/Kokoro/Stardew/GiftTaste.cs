/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using StardewValley;
using System;

namespace Shockah.Kokoro.Stardew;

public enum GiftTaste
{
	Hate = -2,
	Dislike = -1,
	Neutral = 0,
	Like = 1,
	Love = 2
}

public static class GiftTasteExt
{
	public static GiftTaste From(int taste)
	{
		return taste switch
		{
			NPC.gift_taste_hate => GiftTaste.Hate,
			NPC.gift_taste_dislike => GiftTaste.Dislike,
			NPC.gift_taste_neutral => GiftTaste.Neutral,
			NPC.gift_taste_like => GiftTaste.Like,
			NPC.gift_taste_love => GiftTaste.Love,
			_ => throw new ArgumentException($"{nameof(taste)} has an invalid value."),
		};
	}

	public static int GetStardewValue(this GiftTaste self)
	{
		return self switch
		{
			GiftTaste.Hate => NPC.gift_taste_hate,
			GiftTaste.Dislike => NPC.gift_taste_dislike,
			GiftTaste.Neutral => NPC.gift_taste_neutral,
			GiftTaste.Like => NPC.gift_taste_like,
			GiftTaste.Love => NPC.gift_taste_love,
			_ => throw new ArgumentException($"{nameof(GiftTaste)} has an invalid value."),
		};
	}

	public static GiftTaste GetModified(this GiftTaste self, int levels)
	{
		int newValue = (int)self + levels;
		return (GiftTaste)Math.Clamp(newValue, (int)GiftTaste.Hate, (int)GiftTaste.Love);
	}
}