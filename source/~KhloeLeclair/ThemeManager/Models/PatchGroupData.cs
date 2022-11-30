/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System.Collections.Generic;
using System.Reflection;

using Leclair.Stardew.Common.Types;

using Newtonsoft.Json;

using StardewModdingAPI;

namespace Leclair.Stardew.ThemeManager.Models;

public class PatchGroupData {

	public string ID { get; set; } = string.Empty;

	[JsonIgnore]
	public bool CanUse { get; set; }

	public RequiredMod[]? RequiredMods { get; set; }

	public CaseInsensitiveDictionary<string>? Variables { get; set; }

	public Dictionary<string, PatchData>? Patches { get; set; }

	[JsonIgnore]
	public Dictionary<string, MethodInfo[]>? Methods { get; set; }

}

public class RequiredMod {

	public string UniqueID { get; set; } = string.Empty;
	public string? MinimumVersion { get; set; }
	public string? MaximumVersion { get; set; }

}
