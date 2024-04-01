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

namespace Shockah.JunimoWarp;

public class ModConfig : IVersioned.Modifiable
{
	[JsonProperty(NullValueHandling = NullValueHandling.Ignore)] public ISemanticVersion? Version { get; set; }
	[JsonProperty] public bool RequiredEmptyChest { get; internal set; } = true;
}