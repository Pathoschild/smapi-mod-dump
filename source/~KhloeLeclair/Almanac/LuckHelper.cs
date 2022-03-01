/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Leclair.Stardew.Common;
using Leclair.Stardew.Common.UI;
using Leclair.Stardew.Common.UI.FlowNode;

using StardewValley;
using StardewValley.Objects;

using SObject = StardewValley.Object;

namespace Leclair.Stardew.Almanac {
	public static class LuckHelper {

		public interface IHoroscopeEvent {
			string SimpleLabel { get; }
			IEnumerable<IFlowNode> AdvancedLabel { get; }
			SpriteInfo Sprite { get; }
		}

		public struct HoroscopeEvent : IHoroscopeEvent {
			public string SimpleLabel { get; }
			public IEnumerable<IFlowNode> AdvancedLabel { get; }
			public SpriteInfo Sprite { get; }

			public HoroscopeEvent(string simpleLabel, IEnumerable<IFlowNode> advancedLabel, SpriteInfo sprite) {
				SimpleLabel = simpleLabel;
				AdvancedLabel = advancedLabel;
				Sprite = sprite;
			}
		}

		public static class LuckSprites {
			public static readonly Rectangle UNLUCKY_MAX = new(592, 346, 52, 13);
			public static readonly Rectangle UNLUCKY = new(540, 346, 52, 13);
			public static readonly Rectangle LUCKY = new(540, 333, 52, 13);
			public static readonly Rectangle LUCKY_2 = new(592, 333, 52, 13);
			public static readonly Rectangle LUCKY_MAX = new(644, 333, 52, 13);
		}

		public static SpriteInfo GetLuckSprite(double luck) {
			Rectangle source;
			if (luck >= 0.07)
				source = LuckSprites.LUCKY_MAX;
			else if (luck >= 0.02)
				source = LuckSprites.LUCKY_2;
			else if (luck >= 0)
				source = LuckSprites.LUCKY;
			else if (luck >= -0.07)
				source = LuckSprites.UNLUCKY;
			else
				source = LuckSprites.UNLUCKY_MAX;

			return new SpriteInfo(
				texture: SpriteHelper.GetTexture(Common.Enums.GameTexture.MouseCursors),
				baseSource: source,
				baseFrames: 4
			);
		}

		public static string GetLuckText(double luck) {
			if (luck >= 0.07)
				return I18n.Page_Fortune_LuckGreat();
			if (luck >= 0.02)
				return I18n.Page_Fortune_LuckGood();
			if (luck >= 0)
				return I18n.Page_Fortune_LuckNeutral();
			if (luck >= -0.07)
				return I18n.Page_Fortune_LuckBad();

			return I18n.Page_Fortune_LuckAwful();
		}

		public static double GetLuckForDate(int seed, WorldDate date) {
			Random rnd = new(date.TotalDays + (seed / 2));

			return Math.Min(0.100000001490116, (double) rnd.Next(-100, 101) / 1000.0);
		}

		public static IHoroscopeEvent GetTrashEvent(int seed, WorldDate date) {
			for (int i = 0; i < 8; i++) {
				Random rnd = new((date.TotalDays + 1) + (seed / 2) + 777 + i * 77);

				int prewarm = rnd.Next(0, 100);
				for (int j = 0; j < prewarm; j++)
					rnd.NextDouble();

				prewarm = rnd.Next(0, 100);
				for (int j = 0; j < prewarm; j++)
					rnd.NextDouble();

				rnd.NextDouble();

				if (rnd.NextDouble() >= 0.002)
					continue;

				Item item = (Item) new Hat(66);
				SpriteInfo sprite = SpriteHelper.GetSprite(item, ModEntry.instance.Helper);

				return new HoroscopeEvent(
					I18n.Page_Fortune_GarbageHat(),
					null,
					sprite
				);
			}

			return null;
		}

		public static IHoroscopeEvent GetEventForDate(int seed, WorldDate date) {

			// TODO: Some sort of hook that could be extended with the API
			// so that we can put in random night events from other mods.

			int days = date.TotalDays + 1;

			if (days == 31)
				return null;

			Random rnd = new(days + (int) (seed / 2));

			// Don't track any of the Community Center / Joja events because
			// those all rely on game state and are not random based on the
			// date they happen on. Same with weddings preventing events.

			if (rnd.NextDouble() < 0.01 && !date.Season.Equals("winter"))
				return new HoroscopeEvent(
					I18n.Page_Fortune_Event_Fairy(),
					null,
					new SpriteInfo(
						SpriteHelper.GetTexture(Common.Enums.GameTexture.MouseCursors),
						new Rectangle(16, 592, 16, 16)
					)
				);

			if (rnd.NextDouble() < 0.01)
				return new HoroscopeEvent(
					I18n.Page_Fortune_Event_Witch(),
					null,
					new SpriteInfo(
						SpriteHelper.GetTexture(Common.Enums.GameTexture.MouseCursors),
						new Rectangle(277, 1886, 34, 29)
					)
				);

			if (rnd.NextDouble() < 0.01)
				return new HoroscopeEvent(
					I18n.Page_Fortune_Event_Meteorite(),
					null,
					new SpriteInfo(
						SpriteHelper.GetTexture(Common.Enums.GameTexture.Object),
						new Rectangle(352, 400, 32, 32)
					)
				);

			if (rnd.NextDouble() < 0.005)
				return new HoroscopeEvent(
					I18n.Page_Fortune_Event_Owl(),
					null,
					SpriteHelper.GetSprite(new SObject(Vector2.Zero, 95), ModEntry.instance.Helper)
				);

			// Don't track Strange Capsule, because that relies on whether
			// or not the player has already seen it.

			return null;
		}

	}
}
