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

namespace Leclair.Stardew.MoreNightlyEvents.Models;

public class SideEffectData {

	public string Id { get; set; } = string.Empty;

	public string? Condition { get; set; }

	public bool HostOnly { get; set; }

	public List<string>? Actions { get; set; }

}
