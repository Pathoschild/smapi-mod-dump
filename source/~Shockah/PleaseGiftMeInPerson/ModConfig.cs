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
using Shockah.Kokoro;
using StardewModdingAPI;
using System;
using System.Collections.Generic;

namespace Shockah.PleaseGiftMeInPerson
{
	public class ModConfig : IVersioned.Modifiable
	{
		public class Entry : IEquatable<Entry>
		{
			[JsonProperty] public int GiftsToRemember { get; internal set; }
			[JsonProperty] public int DaysToRemember { get; internal set; }
			[JsonProperty] public GiftPreference InPersonPreference { get; internal set; }
			[JsonProperty] public GiftPreference ByMailPreference { get; internal set; }
			[JsonProperty] public float InfrequentGiftPercent { get; internal set; }
			[JsonProperty] public float FrequentGiftPercent { get; internal set; }
			[JsonProperty] public bool EnableModOverrides { get; internal set; } = true;

			public Entry()
			{
			}

			public Entry(Entry other)
			{
				GiftsToRemember = other.GiftsToRemember;
				DaysToRemember = other.DaysToRemember;
				InPersonPreference = other.InPersonPreference;
				ByMailPreference = other.ByMailPreference;
				InfrequentGiftPercent = other.InfrequentGiftPercent;
				FrequentGiftPercent = other.FrequentGiftPercent;
				EnableModOverrides = other.EnableModOverrides;
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

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)] public ISemanticVersion? Version { get; set; }

		[JsonProperty]
		public Entry Default { get; internal set; } = new()
		{
			GiftsToRemember = 5,
			DaysToRemember = 14,
			InPersonPreference = GiftPreference.Neutral,
			ByMailPreference = GiftPreference.HatesFrequent,
			InfrequentGiftPercent = 0.33f,
			FrequentGiftPercent = 0.66f
		};

		[JsonProperty]
		public Entry? Spouse { get; internal set; }

		[JsonProperty] public IDictionary<string, Entry> PerNPC { get; internal set; } = new Dictionary<string, Entry>();

		[JsonProperty] public bool EnableNPCOverrides { get; internal set; } = true;
		[JsonProperty] public bool ConfirmUnlikedInPersonGifts { get; internal set; } = true;
		[JsonProperty] public bool ShowCurrentMailModifier { get; internal set; } = true;

		public ModConfig()
		{
		}

		public ModConfig(ModConfig other) : this()
		{
			Default.CopyFrom(other.Default);
			foreach (var (name, entry) in other.PerNPC)
				PerNPC[name] = new(entry);
			EnableNPCOverrides = other.EnableNPCOverrides;
			ConfirmUnlikedInPersonGifts = other.ConfirmUnlikedInPersonGifts;
			ShowCurrentMailModifier = other.ShowCurrentMailModifier;

			if (Spouse is null && other.Spouse is not null)
				Spouse = new(other.Spouse);
			else if (Spouse is not null && other.Spouse is null)
				Spouse = null;
			else if (Spouse is not null && other.Spouse is not null)
				Spouse.CopyFrom(other.Spouse);
		}

		public Entry GetForNPC(string npcName)
		{
			if (!PerNPC.TryGetValue(npcName, out var entry))
				entry = Default;
			return entry;
		}
	}
}