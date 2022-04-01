/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/


namespace Leclair.Stardew.Almanac.Pages {
	public interface IRightFlowMargins {

		// Right Flow
		int RightMarginTop { get; }
		int RightMarginLeft { get; }
		int RightMarginRight { get; }
		int RightMarginBottom { get; }

		int RightScrollMarginTop { get; }
		int RightScrollMarginBottom { get; }
	}
}
