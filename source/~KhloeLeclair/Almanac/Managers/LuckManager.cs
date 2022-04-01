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
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Leclair.Stardew.Common;

using StardewModdingAPI;

using StardewValley;
using StardewValley.Objects;

using SObject = StardewValley.Object;

using Leclair.Stardew.Almanac.Models;

namespace Leclair.Stardew.Almanac.Managers {
	public class LuckManager : BaseManager {

		#region Stuff (Sprites, Strings)

		public static class LuckSprites {
			public static readonly Rectangle UNLUCKY_MAX = new(592, 346, 52, 13);
			public static readonly Rectangle UNLUCKY = new(540, 346, 52, 13);
			public static readonly Rectangle LUCKY = new(540, 333, 52, 13);
			public static readonly Rectangle LUCKY_2 = new(592, 333, 52, 13);
			public static readonly Rectangle LUCKY_MAX = new(644, 333, 52, 13);
		}

		public SpriteInfo GetLuckSprite(double luck) {
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

		public string GetLuckText(double luck) {
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

		#endregion


		// Mods
		private readonly Dictionary<IManifest, Func<int, WorldDate, IEnumerable<Tuple<bool, string, Texture2D, Rectangle?, Item>>>> ModHooks = new();
		private readonly Dictionary<IManifest, Func<int, WorldDate, IEnumerable<Tuple<bool, IRichEvent>>>> InterfaceHooks = new();

		public LuckManager(ModEntry mod) : base(mod) { }

		#region Event Handlers



		#endregion

		#region Mod Management

		public void ClearHook(IManifest mod) {
			if (InterfaceHooks.ContainsKey(mod))
				InterfaceHooks.Remove(mod);

			if (ModHooks.ContainsKey(mod))
				ModHooks.Remove(mod);
		}

		public void RegisterHook(IManifest mod, Func<int, WorldDate, IEnumerable<Tuple<bool, string, Texture2D, Rectangle?, Item>>> hook) {
			if (InterfaceHooks.ContainsKey(mod))
				InterfaceHooks.Remove(mod);

			if (hook == null && ModHooks.ContainsKey(mod))
				ModHooks.Remove(mod);
			else if (hook != null)
				ModHooks[mod] = hook;
		}

		public void RegisterHook(IManifest mod, Func<int, WorldDate, IEnumerable<Tuple<bool, IRichEvent>>> hook) {
			if (ModHooks.ContainsKey(mod))
				ModHooks.Remove(mod);

			if (hook == null && InterfaceHooks.ContainsKey(mod))
				InterfaceHooks.Remove(mod);
			else if (hook != null)
				InterfaceHooks[mod] = hook;
		}

		#endregion

		#region Luck Lookup

		public double GetLuckForDate(int seed, WorldDate date) {
			Random rnd = new(date.TotalDays + (seed / 2));

			return Math.Min(0.100000001490116, (double) rnd.Next(-100, 101) / 1000.0);
		}

		public double GetModifiedLuckForDate(int seed, WorldDate date) {
			double result = GetLuckForDate(seed, date);

			// Luck Skill
			if (Mod.intLS.IsLoaded) {
				foreach (var who in Game1.getAllFarmers()) {
					// Fortunate players add 0.01 to their team's sharedDailyLuck.
					if (Mod.intLS.HasFortunate(who))
						result += 0.01;

					// Lucky players have a 20% chance of maximum daily luck.
					if (Mod.intLS.HasLucky(who)) {
						Random rnd = new(seed + date.TotalDays * 3);
						if (rnd.NextDouble() <= 0.20)
							result = 0.12;
					}

					// UnUnLucky
					if (Mod.intLS.HasUnUnlucky(who) & result < 0)
						result = 0;
				}
			}

			return result;
		}

		#endregion

		#region Events

		public IEnumerable<IRichEvent> GetEventsForDate(int seed, WorldDate date) {

			bool do_vanilla = true;

			foreach (var ihook in InterfaceHooks.Values) {
				if (ihook != null)
					foreach (var entry in ihook(seed, date)) {
						if (entry == null)
							continue;

						do_vanilla &= entry.Item1;

						if (entry.Item2 != null)
							yield return entry.Item2;
					}
			}

			foreach (var hook in ModHooks.Values) {
				if (hook != null)
					foreach (var entry in hook(seed, date)) {
						if (entry == null)
							continue;

						do_vanilla &= entry.Item1;

						if (string.IsNullOrEmpty(entry.Item2))
							continue;

						SpriteInfo sprite;

						if (entry.Item4.HasValue && entry.Item4.Value == Rectangle.Empty)
							sprite = null;
						else if (entry.Item3 != null)
							sprite = new(
								entry.Item3,
								entry.Item4 ?? entry.Item3.Bounds
							);
						else if (entry.Item5 != null)
							sprite = SpriteHelper.GetSprite(entry.Item5);
						else
							sprite = null;


						yield return new RichEvent(
							entry.Item2,
							null,
							sprite,
							entry.Item5
						);
					}
			}

			var evt = do_vanilla ? GetVanillaEventForDate(seed, date) : null;
			if (evt != null)
				yield return evt;

			if (evt == null && Mod.intLS.IsLoaded) {
				// Do any players have the Luck Skill mod profession "Shooting Star"?
				bool shooting = false;

				foreach (var farmer in Game1.getAllFarmers()) {
					if (Mod.intLS.HasShootingStar(farmer)) {
						shooting = true;
						break;
					}
				}

				evt = shooting ? GetLuckSkillEventForDate(seed, date) : null;
				if (evt != null)
					yield return evt;
			}

			evt = GetTrashEvent(seed, date);
			if (evt != null)
				yield return evt;
		}

		#endregion

		#region Vanilla Events

		public static IRichEvent GetTrashEvent(int seed, WorldDate date) {
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
				SpriteInfo sprite = SpriteHelper.GetSprite(item);

				return new RichEvent(
					I18n.Page_Fortune_GarbageHat(),
					null,
					sprite
				);
			}

			return null;
		}

		private static IRichEvent GetVanillaEventForDate(int seed, WorldDate date) {
			int days = date.TotalDays + 1;

			if (days == 31)
				return null;

			Random rnd = new(days + (int) (seed / 2));

			// Don't track any of the Community Center / Joja events because
			// those all rely on game state and are not random based on the
			// date they happen on. Same with weddings preventing events.

			if (rnd.NextDouble() < 0.01 && !date.Season.Equals("winter"))
				return new RichEvent(
					I18n.Page_Fortune_Event_Fairy(),
					null,
					new SpriteInfo(
						SpriteHelper.GetTexture(Common.Enums.GameTexture.MouseCursors),
						new Rectangle(16, 592, 16, 16)
					)
				);

			if (rnd.NextDouble() < 0.01)
				return new RichEvent(
					I18n.Page_Fortune_Event_Witch(),
					null,
					new SpriteInfo(
						SpriteHelper.GetTexture(Common.Enums.GameTexture.MouseCursors),
						new Rectangle(277, 1886, 34, 29)
					)
				);

			if (rnd.NextDouble() < 0.01)
				return new RichEvent(
					I18n.Page_Fortune_Event_Meteorite(),
					null,
					new SpriteInfo(
						SpriteHelper.GetTexture(Common.Enums.GameTexture.Object),
						new Rectangle(352, 400, 32, 32)
					)
				);

			if (rnd.NextDouble() < 0.005)
				return new RichEvent(
					I18n.Page_Fortune_Event_Owl(),
					null,
					SpriteHelper.GetSprite(new SObject(Vector2.Zero, 95))
				);

			// Don't track Strange Capsule, because that relies on whether
			// or not the player has already seen it.
			/*if (rnd.NextDouble() < 0.008 && date.Year > 1 && ! Game1.MasterPlayer.mailReceived.Contains("Got_Capsule"))
				return new RichEvent(
					I18n.Page_Fortune_Event_Ufo(),
					null,
					SpriteHelper.GetSprite(new SObject(Vector2.Zero, 96))
				);*/

			return null;
		}

		private static IRichEvent GetLuckSkillEventForDate(int seed, WorldDate date) {
			int days = date.TotalDays + 1 + 999999;

			if (days == 31)
				return null;

			Random rnd = new(days + (int) (seed / 2));

			// Don't track any of the Community Center / Joja events because
			// those all rely on game state and are not random based on the
			// date they happen on. Same with weddings preventing events.

			if (rnd.NextDouble() < 0.01 && !date.Season.Equals("winter"))
				return new RichEvent(
					I18n.Page_Fortune_Event_Fairy(),
					null,
					new SpriteInfo(
						SpriteHelper.GetTexture(Common.Enums.GameTexture.MouseCursors),
						new Rectangle(16, 592, 16, 16)
					)
				);

			if (rnd.NextDouble() < 0.01)
				return new RichEvent(
					I18n.Page_Fortune_Event_Witch(),
					null,
					new SpriteInfo(
						SpriteHelper.GetTexture(Common.Enums.GameTexture.MouseCursors),
						new Rectangle(277, 1886, 34, 29)
					)
				);

			if (rnd.NextDouble() < 0.01)
				return new RichEvent(
					I18n.Page_Fortune_Event_Meteorite(),
					null,
					new SpriteInfo(
						SpriteHelper.GetTexture(Common.Enums.GameTexture.Object),
						new Rectangle(352, 400, 32, 32)
					)
				);

			if (rnd.NextDouble() < 0.01 && date.Year > 1)
				return new RichEvent(
					I18n.Page_Fortune_Event_Ufo(),
					null,
					SpriteHelper.GetSprite(new SObject(Vector2.Zero, 96))
				);

			if (rnd.NextDouble() < 0.005)
				return new RichEvent(
					I18n.Page_Fortune_Event_Owl(),
					null,
					SpriteHelper.GetSprite(new SObject(Vector2.Zero, 95))
				);

			// Don't track Strange Capsule, because that relies on whether
			// or not the player has already seen it.

			return null;
		}

		#endregion

	}
}
