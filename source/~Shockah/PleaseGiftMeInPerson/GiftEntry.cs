/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using Newtonsoft.Json;
using Shockah.CommonModCode.Stardew;
using StardewValley;
using System;

namespace Shockah.PleaseGiftMeInPerson
{
	public enum GiftPreference
	{
		Hates = -5,
		DislikesAndHatesFrequent = -4,
		HatesFrequent = -3,
		Dislikes = -2,
		DislikesFrequent = -1,
		Neutral = 0,
		LikesInfrequentButDislikesFrequent = 1,
		LikesInfrequent = 2,
		Likes = 3,
		LovesInfrequent = 4,
		LikesAndLovesInfrequent = 5,
		Loves = 6
	}

	internal enum GiftMethod { InPerson, ByMail }
	
	internal struct GiftEntry: IEquatable<GiftEntry>
	{
		public int Year { get; set; }
		public int SeasonIndex { get; set; }
		public int DayOfMonth { get; set; }
		public GiftTaste GiftTaste { get; set; }
		public GiftMethod GiftMethod { get; set; }

		[JsonIgnore]
		public Season Season
			=> (Season)SeasonIndex;

		[JsonIgnore]
		public WorldDate Date
			=> new(Year, Enum.GetName(Season)?.ToLower(), DayOfMonth);

		public GiftEntry(int year, int seasonIndex, int dayOfMonth, GiftTaste giftTaste, GiftMethod giftMethod)
		{
			this.Year = year;
			this.SeasonIndex = seasonIndex;
			this.DayOfMonth = dayOfMonth;
			this.GiftTaste = giftTaste;
			this.GiftMethod = giftMethod;
		}

		public GiftEntry(WorldDate date, GiftTaste giftTaste, GiftMethod giftMethod)
			: this(date.Year, date.SeasonIndex, date.DayOfMonth, giftTaste, giftMethod)
		{
		}

		public bool Equals(GiftEntry other)
			=> Year == other.Year && SeasonIndex == other.SeasonIndex && DayOfMonth == other.DayOfMonth && GiftTaste == other.GiftTaste && GiftMethod == other.GiftMethod;

		public override bool Equals(object? obj)
			=> obj is GiftEntry entry && Equals(entry);

		public override int GetHashCode()
			=> (Year, SeasonIndex, DayOfMonth, GiftTaste, GiftMethod).GetHashCode();
	}
}