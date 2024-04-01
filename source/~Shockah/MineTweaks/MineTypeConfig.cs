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

namespace Shockah.MineTweaks;

public sealed class MineTypeConfig
{
	[JsonProperty] public float StoneChanceMultiplier { get; internal set; } = 1f;
	[JsonProperty] public float MonsterChanceMultiplier { get; internal set; } = 1f;
	[JsonProperty] public float ItemChanceMultiplier { get; internal set; } = 1f;
	[JsonProperty] public float GemStoneChanceMultiplier { get; internal set; } = 1f;
	[JsonProperty] public float MonsterMuskChanceMultiplier { get; internal set; } = 2f;
}