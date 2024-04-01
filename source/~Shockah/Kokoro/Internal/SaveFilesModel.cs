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
using System.Collections.Generic;

namespace Shockah.Kokoro;

internal sealed class SaveFilesModel
{
	[JsonProperty] public ISemanticVersion Version { get; internal set; } = ModEntry.Instance.ModManifest.Version;
	[JsonProperty] public IList<SaveFileEntry> Entries { get; internal set; } = new List<SaveFileEntry>();

	public sealed record SaveFileEntry(
		long PlayerID,
		SaveFileDescriptor Descriptor
	);
}