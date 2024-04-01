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
using Newtonsoft.Json.Linq;
using Shockah.Kokoro;
using StardewModdingAPI;
using System.Collections.Generic;

namespace Shockah.DontStopMeNow;

public class ModConfig : IVersioned.Modifiable
{
	[JsonProperty(NullValueHandling = NullValueHandling.Ignore)] public ISemanticVersion? Version { get; set; }
	[JsonProperty] public bool OverrideMoveSpeed { get; set; } = true;
	[JsonProperty] public float MoveSpeed { get; set; } = 0.5f;
	[JsonProperty] public bool MoveWhileSwingingTools { get; set; } = false;
	[JsonProperty] public bool MoveWhileSwingingMeleeWeapons { get; set; } = true;
	[JsonProperty] public bool MoveWhileSpecial { get; set; } = true;
	[JsonProperty] public bool MoveWhileAimingSlingshot { get; set; } = true;
	[JsonProperty] public bool MoveWhileChargingTools { get; set; } = false;
	[JsonProperty] public bool FixToolFacing { get; set; } = true;
	[JsonProperty] public bool FixMeleeWeaponFacing { get; set; } = true;
	[JsonProperty] public bool FixChargingToolFacing { get; set; } = true;
	[JsonProperty] public bool FixFishingRodFacing { get; set; } = true;
	[JsonProperty] public bool FixFacingOnMouse { get; set; } = true;
	[JsonProperty] public bool FixFacingOnController { get; set; } = false;
	[JsonExtensionData] internal IDictionary<string, JToken> ExtensionData { get; set; } = new Dictionary<string, JToken>();
}