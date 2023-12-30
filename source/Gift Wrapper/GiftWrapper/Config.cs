/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/GiftWrapper
**
*************************************************/

namespace GiftWrapper
{
	public class Config
	{
		public enum Themes
		{
			Red = 1,
			Blue = 2,
			Green = 3
		}

		public Themes Theme { get; set; } = (Themes)(-1);
		public bool PlayAnimations { get; set; } = true;
		public bool AlwaysAvailable { get; set; } = false;
		public bool GiftPreviewEnabled { get; set; } = true;
		public int GiftPreviewTileRange { get; set; } = 5;
		public int GiftPreviewFadeSpeed { get; set; } = 10;
	}
}
