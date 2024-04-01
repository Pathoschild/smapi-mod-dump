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
using StardewModdingAPI;
using StardewValley;
using System.Collections.Generic;

namespace Shockah.Kokoro;

internal sealed class SaveFileDescriptor
{
	[JsonProperty] public string GameVersion { get; internal set; } = Game1.version;
	[JsonProperty] public ISemanticVersion SmapiVersion { get; internal set; } = Constants.ApiVersion;
	[JsonProperty] public Dictionary<string, ModDescriptor> Mods { get; internal set; } = [];

	public sealed record ModDescriptor(
		string Name,
		string Author,
		ISemanticVersion Version
	);

	public static SaveFileDescriptor CreateFromCurrentState()
		=> new() { Mods = GetModDictionaryFromCurrentState() };

	public static Dictionary<string, ModDescriptor> GetModDictionaryFromCurrentState()
	{
		Dictionary<string, ModDescriptor> result = [];
		foreach (var mod in ModEntry.Instance.Helper.ModRegistry.GetAll())
			result[mod.Manifest.UniqueID] = new(mod.Manifest.Name, mod.Manifest.Author, mod.Manifest.Version);
		return result;
	}
}