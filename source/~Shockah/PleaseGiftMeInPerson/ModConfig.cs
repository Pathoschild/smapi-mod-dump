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
using System;
using System.Collections.Generic;

namespace Shockah.PleaseGiftMeInPerson
{
	internal class ModConfig
	{
		public class Entry: IEquatable<Entry>
		{
			public int GiftsToRemember { get; set; }
			public int DaysToRemember { get; set; }
			public GiftPreference InPersonPreference { get; set; }
			public GiftPreference ByMailPreference { get; set; }
			public float InfrequentGiftPercent { get; set; }
			public float FrequentGiftPercent { get; set; }
			public bool EnableModOverrides { get; set; }

			[JsonConstructor]
			public Entry(
				int giftsToRemember,
				int daysToRemember,
				GiftPreference inPersonPreference,
				GiftPreference byMailPreference,
				float infrequentGiftPercent,
				float frequentGiftPercent,
				bool enableModOverrides = true
			)
			{
				this.GiftsToRemember = giftsToRemember;
				this.DaysToRemember = daysToRemember;
				this.InPersonPreference = inPersonPreference;
				this.ByMailPreference = byMailPreference;
				this.InfrequentGiftPercent = infrequentGiftPercent;
				this.FrequentGiftPercent = frequentGiftPercent;
				this.EnableModOverrides = enableModOverrides;
			}

			public Entry(Entry other): this(
				giftsToRemember: other.GiftsToRemember,
				daysToRemember: other.DaysToRemember,
				inPersonPreference: other.InPersonPreference,
				byMailPreference: other.ByMailPreference,
				infrequentGiftPercent: other.InfrequentGiftPercent,
				frequentGiftPercent: other.FrequentGiftPercent,
				enableModOverrides: other.EnableModOverrides
			)
			{
			}

			public void CopyFrom(Entry other)
			{
				this.GiftsToRemember = other.GiftsToRemember;
				this.DaysToRemember = other.DaysToRemember;
				this.InPersonPreference = other.InPersonPreference;
				this.ByMailPreference = other.ByMailPreference;
				this.InfrequentGiftPercent = other.InfrequentGiftPercent;
				this.FrequentGiftPercent = other.FrequentGiftPercent;
				this.EnableModOverrides = other.EnableModOverrides;
			}

			public bool HasSameValues(Entry other)
				=> GiftsToRemember == other.GiftsToRemember
				&& DaysToRemember == other.DaysToRemember
				&& InPersonPreference == other.InPersonPreference
				&& ByMailPreference == other.ByMailPreference
				&& InfrequentGiftPercent == other.InfrequentGiftPercent
				&& FrequentGiftPercent == other.FrequentGiftPercent;

			public bool Equals(Entry? other)
				=> other is not null
				&& HasSameValues(other)
				&& EnableModOverrides == other.EnableModOverrides;

			public override bool Equals(object? obj)
				=> obj is Entry entry && Equals(entry);

			public override int GetHashCode()
				=> (GiftsToRemember, DaysToRemember, InPersonPreference, ByMailPreference, InfrequentGiftPercent, FrequentGiftPercent, EnableModOverrides).GetHashCode();

			public static bool operator ==(Entry lhs, Entry rhs)
				=> lhs.Equals(rhs);

			public static bool operator !=(Entry lhs, Entry rhs)
				=> !lhs.Equals(rhs);
		}

		public Entry Default { get; set; } = new(
			giftsToRemember: 5,
			daysToRemember: 14,
			inPersonPreference: GiftPreference.Neutral,
			byMailPreference: GiftPreference.HatesFrequent,
			infrequentGiftPercent: 0.33f,
			frequentGiftPercent: 0.66f
		);

		public Entry Spouse { get; set; } = new(
			giftsToRemember: 5,
			daysToRemember: 14,
			inPersonPreference: GiftPreference.Neutral,
			byMailPreference: GiftPreference.HatesFrequent,
			infrequentGiftPercent: 0.33f,
			frequentGiftPercent: 0.66f
		);

		public IDictionary<string, Entry> PerNPC { get; set; } = new Dictionary<string, Entry>();

		public bool EnableNPCOverrides { get; set; } = true;
		public bool ConfirmUnlikedInPersonGifts { get; set; } = true;
		public bool ShowCurrentMailModifier { get; set; } = true;

		public ModConfig()
		{
		}

		public ModConfig(ModConfig other): this()
		{
			Default.CopyFrom(other.Default);
			foreach (var (name, entry) in other.PerNPC)
				PerNPC[name] = new(entry);
		}

		public Entry GetForNPC(string npcName)
		{
			if (!PerNPC.TryGetValue(npcName, out var entry))
				entry = Default;
			return entry;
		}
	}
}