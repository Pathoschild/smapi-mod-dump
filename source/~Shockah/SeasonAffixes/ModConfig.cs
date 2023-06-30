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
using System.Collections.Generic;

namespace Shockah.SeasonAffixes;

public partial class ModConfig : IVersioned.Modifiable
{
	[JsonProperty(NullValueHandling = NullValueHandling.Ignore)] public ISemanticVersion? Version { get; set; }

	[JsonProperty] public AffixSetChoicePeriod ChoicePeriod { get; internal set; } = AffixSetChoicePeriod.Season;
	[JsonProperty] public bool ChoiceOnYear1Spring2 { get; internal set; } = false;
	[JsonProperty] public bool Incremental { get; internal set; } = false;
	[JsonProperty] public int Choices { get; internal set; } = 2;
	//[JsonProperty] public int RerollsPerSeason { get; internal set; } = 1;
	[JsonProperty] public IDictionary<string, double> AffixWeights { get; internal set; } = new Dictionary<string, double>();
	[JsonProperty] public ISet<string> PermanentAffixes { get; internal set; } = new HashSet<string>();

	[JsonProperty] public IList<AffixSetEntry> AffixSetEntries { get; internal set; } = new List<AffixSetEntry>() { new(1, 0, 2.0), new(1, 1, 8.0), new(1, 2, 5.0), new(2, 1, 3.0), new(2, 2, 2.0) };
	[JsonProperty] public int AffixRepeatPeriod { get; internal set; } = 2;
	[JsonProperty] public int AffixSetRepeatPeriod { get; internal set; } = 4;

	[JsonProperty] public bool WinterCrops { get; internal set; } = false;
}