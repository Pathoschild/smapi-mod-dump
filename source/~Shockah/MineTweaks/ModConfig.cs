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

namespace Shockah.MineTweaks
{
	public sealed class ModConfig
	{
		[JsonProperty] public MineTypeConfig Mine { get; internal set; } = new();
		[JsonProperty] public MineTypeConfig SkullCavern { get; internal set; } = new();
		[JsonProperty] public MineTypeConfig Volcano { get; internal set; } = new();
	}
}