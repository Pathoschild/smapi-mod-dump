/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

#nullable enable

namespace Leclair.Stardew.Almanac.Pages;

public interface ILeftFlowMargins {

	// Left Flow
	int LeftMarginTop { get; }
	int LeftMarginLeft { get; }
	int LeftMarginRight { get; }
	int LeftMarginBottom { get; }

	int LeftScrollMarginTop { get; }
	int LeftScrollMarginBottom { get; }
}
