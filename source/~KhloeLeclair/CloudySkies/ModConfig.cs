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

namespace Leclair.Stardew.CloudySkies;

public class ModConfig {

	public bool AllowShaders { get; set; } = true;

	public HashSet<string> DisabledShaders { get; set; } = new();

	public bool ReplaceTVMenu { get; set; } = true;

	public bool ShowDebugTiming { get; set; } = false;

	public bool RecompileShaders { get; set; } = false;

	public bool ShowWeatherTooltip { get; set; } = true;

	public bool UseCustomGingerIsleArt { get; set; } = true;

}
