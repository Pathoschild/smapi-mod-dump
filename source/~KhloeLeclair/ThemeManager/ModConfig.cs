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

using Leclair.Stardew.Common.UI;

namespace Leclair.Stardew.ThemeManager;

public enum ClockAlignMode {
	Default,
	ByTheme,
	Manual
};

internal class ModConfig {

	public bool AlignText { get; set; } = true;

	public ClockAlignMode ClockMode { get; set; } = ClockAlignMode.ByTheme;

	public Alignment? ClockAlignment { get; set; }

	public string StardewTheme { get; set; } = "automatic";

	public Dictionary<string, string> SelectedThemes { get; set; } = new();

}
