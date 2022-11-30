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
using Leclair.Stardew.Common.Types;

namespace Leclair.Stardew.ThemeManager.Models;

public class PatchData {

	public CaseInsensitiveDictionary<Dictionary<string, string>>? Colors { get; set; }

	public CaseInsensitiveDictionary<Dictionary<string, string>>? RawColors { get; set; }

	public CaseInsensitiveDictionary<Dictionary<string, string>>? Fields { get; set; }

	public Dictionary<string, string[]>? RedToGreenLerp { get; set; }

}
