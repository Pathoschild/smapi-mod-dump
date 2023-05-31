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
using Shockah.Kokoro.UI;
using System.Collections.Generic;

namespace Shockah.XPDisplay
{
	public sealed class ModConfig
	{
		[JsonProperty] public Orientation SmallBarOrientation { get; internal set; } = Orientation.Vertical;
		[JsonProperty] public Orientation BigBarOrientation { get; internal set; } = Orientation.Horizontal;
		[JsonProperty] public float Alpha { get; internal set; } = 0.6f;
		[JsonProperty] public string? LevelUpSoundName { get; internal set; } = "crystal";
		[JsonProperty] public ToolbarSkillBarConfig ToolbarSkillBar { get; internal set; } = new();
		[JsonProperty] public ISet<string> SkillsToExcludeFromToolbarOnXPGain { get; internal set; } = new HashSet<string> { "Achtuur.Travelling" };
	}

	public sealed class ToolbarSkillBarConfig
	{
		[JsonProperty] public bool IsEnabled { get; internal set; } = true;
		[JsonProperty] public float Scale { get; internal set; } = 4f;
		[JsonProperty] public float SpacingFromToolbar { get; internal set; } = 24f;
		[JsonProperty] public bool ShowIcon { get; internal set; } = true;
		[JsonProperty] public bool ShowLevelNumber { get; internal set; } = true;
		[JsonProperty] public bool AlwaysShowCurrentTool { get; internal set; } = false;
		[JsonProperty] public float ToolSwitchDurationInSeconds { get; internal set; } = 3f;
		[JsonProperty] public float ToolUseDurationInSeconds { get; internal set; } = 3f;
		[JsonProperty] public float XPChangedDurationInSeconds { get; internal set; } = 3f;
		[JsonProperty] public float LevelChangedDurationInSeconds { get; internal set; } = 5f;
	}
}