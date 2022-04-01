/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;

using Leclair.Stardew.Common.Enums;


namespace Leclair.Stardew.Almanac.Models {

	public enum NoticeIconType {
		Item,
		Texture
	}

	public record struct DateRange(int Start, int End, int[] Valid = null);

	public class LocalNotice {

		// When
		public TimeScale Period { get; set; }
		public DateRange[] Ranges { get; set; }

		public int FirstYear { get; set; } = 1;
		public int LastYear { get; set; } = int.MaxValue;
		public int[] ValidYears { get; set; } = null;

		public Season[] ValidSeasons { get; set; } = new[] {
			Season.Spring,
			Season.Summer,
			Season.Fall,
			Season.Winter
		};


		// Text
		public bool ShowEveryDay { get; set; } = false;

		public string Description { get; set; }

		// Icon
		public NoticeIconType IconType { get; set; }

		// Item
		public string Item { get; set; }

		// Texture
		public GameTexture? IconSource { get; set; } = null;
		public string IconPath { get; set; }
		public Rectangle? IconSourceRect { get; set; }
	}
}
