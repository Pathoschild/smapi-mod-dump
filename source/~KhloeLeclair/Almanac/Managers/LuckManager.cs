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

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Leclair.Stardew.Common;
using Leclair.Stardew.Common.Enums;
using Leclair.Stardew.Common.Events;
using Leclair.Stardew.Common.UI;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewValley;

using SObject = StardewValley.Object;

using Leclair.Stardew.Almanac.Models;

namespace Leclair.Stardew.Almanac.Managers;

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
			texture: Game1.mouseCursors,
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
	private readonly Dictionary<IManifest, Func<ulong, WorldDate, IEnumerable<Tuple<bool, string, Texture2D, Rectangle?, Item>>>> ModHooks = new();
	private readonly Dictionary<IManifest, Func<ulong, WorldDate, IEnumerable<Tuple<bool, IRichEvent>>>> InterfaceHooks = new();

	private Dictionary<string, LocalNotice>? DataEvents;
	private bool Loaded = false;

	public LuckManager(ModEntry mod) : base(mod) { }

	public void Invalidate() {
		Loaded = false;
		DataEvents = null;
		Mod.Helper.GameContent.InvalidateCache(AssetManager.FortuneEventsPath);
	}

	#region Event Handlers

	[Subscriber]
	private void OnAssetInvalidated(object? sender, AssetsInvalidatedEventArgs e) {
		foreach (var name in e.Names)
			if (name.IsEquivalentTo(AssetManager.FortuneEventsPath)) {
				Loaded = false;
				DataEvents = null;
			}
	}

	[Subscriber]
	private void OnAssetRequested(object? sender, AssetRequestedEventArgs e) {
		if (e.Name.IsEquivalentTo(AssetManager.FortuneEventsPath))
			e.LoadFrom(
				() => LoadEvents(),
				priority: AssetLoadPriority.Low
			);
	}

	#endregion

	#region Loading

	private Dictionary<string, LocalNotice> LoadEvents() {
		Dictionary<string, LocalNotice> events = new();

		foreach (var cp in Mod.Helper.ContentPacks.GetOwned()) {
			if (!cp.HasFile("fortune_events.json"))
				continue;

			Dictionary<string, LocalNotice>? data;
			try {
				data = cp.ReadJsonFile<Dictionary<string, LocalNotice>>("fortune_events.json");
				if (data == null)
					throw new ArgumentNullException();
			} catch (Exception ex) {
				Log($"Invalid or empty fortune_events.json for content pack '{cp.Manifest.Name}'", LogLevel.Error, ex);
				continue;
			}

			foreach (var entry in data) {
				entry.Value.Translation = cp.Translation;
				entry.Value.ModContent = cp.ModContent;
				if (!events.ContainsKey(entry.Key))
					events[entry.Key] = entry.Value;
			}
		}

		return events;
	}

	[MemberNotNull(nameof(DataEvents))]
	private void Load() {
		if (Loaded && DataEvents != null)
			return;

		DataEvents = Mod.Helper.GameContent.Load<Dictionary<string, LocalNotice>>(AssetManager.FortuneEventsPath);
		Loaded = true;
	}

	#endregion

	#region Mod Management

	public void ClearHook(IManifest mod) {
		if (InterfaceHooks.ContainsKey(mod))
			InterfaceHooks.Remove(mod);

		if (ModHooks.ContainsKey(mod))
			ModHooks.Remove(mod);
	}

	public void RegisterHook(IManifest mod, Func<ulong, WorldDate, IEnumerable<Tuple<bool, string, Texture2D, Rectangle?, Item>>> hook) {
		if (InterfaceHooks.ContainsKey(mod))
			InterfaceHooks.Remove(mod);

		if (hook == null && ModHooks.ContainsKey(mod))
			ModHooks.Remove(mod);
		else if (hook != null)
			ModHooks[mod] = hook;
	}

	public void RegisterHook(IManifest mod, Func<ulong, WorldDate, IEnumerable<Tuple<bool, IRichEvent>>> hook) {
		if (ModHooks.ContainsKey(mod))
			ModHooks.Remove(mod);

		if (hook == null && InterfaceHooks.ContainsKey(mod))
			InterfaceHooks.Remove(mod);
		else if (hook != null)
			InterfaceHooks[mod] = hook;
	}

	#endregion

	#region Luck Lookup

	public double GetLuckForDate(ulong seed, WorldDate date) {
		Random rnd = new(date.TotalDays + (int)seed / 2);

		int prewarm = rnd.Next(0, 100);
		for (int j = 0; j < prewarm; j++)
			rnd.NextDouble();

		prewarm = rnd.Next(0, 100);
		for (int j = 0; j < prewarm; j++)
			rnd.NextDouble();

		rnd.NextDouble();

		return Math.Min(0.100000001490116, (double) rnd.Next(-100, 101) / 1000.0);
	}

	public double GetModifiedLuckForDate(ulong seed, WorldDate date) {
		double result = GetLuckForDate(seed, date);

		// Luck Skill
		if (Mod.intLS != null && Mod.intLS.IsLoaded) {
			foreach (var who in Game1.getAllFarmers()) {
				// Fortunate players add 0.01 to their team's sharedDailyLuck.
				if (Mod.intLS.HasFortunate(who))
					result += 0.01;

				// Lucky players have a 20% chance of maximum daily luck.
				if (Mod.intLS.HasLucky(who)) {
					// Don't use the seed, since we don't control that mod's seed.
					Random rnd = new((int)Game1.uniqueIDForThisGame + date.TotalDays * 3);
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

	public IRichEvent? HydrateEvent(LocalNotice notice, WorldDate date, GameStateQuery.GameState state, string? key = null) {
		if (notice == null)
			return null;

		// Year Validation
		if (notice.FirstYear > date.Year || notice.LastYear < date.Year)
			return null;

		if (notice.ValidYears != null && !notice.ValidYears.Contains(date.Year))
			return null;

		// Season Validation
		if (notice.ValidSeasons != null && !notice.ValidSeasons.Contains(Season.All) && !notice.ValidSeasons.Contains((Season) date.SeasonIndex))
			return null;

		// Date Range Validation
		int day;

		bool first = true;

		switch (notice.Period) {
			case TimeScale.Year:
				day = date.TotalDays % (WorldDate.MonthsPerYear * ModEntry.DaysPerMonth);
				break;
			case TimeScale.Season:
				day = date.DayOfMonth;
				break;
			case TimeScale.Week:
				day = date.DayOfMonth % 7;
				break;
			default:
				day = -1;
				break;
		}

		if (notice.Ranges != null) {
			bool matched = false;
			first = false;
			foreach (var range in notice.Ranges) {
				if (range.Start <= day && range.End >= day && (range.Valid == null || range.Valid.Contains(day))) {
					if (range.Start == day)
						first = true;
					matched = true;
				}
			}

			if (!matched)
				return null;
		}

		// Condition Validation
		if (!string.IsNullOrEmpty(notice.Condition) && !GameStateQuery.CheckConditions(notice.Condition, state))
			return null;

		// Get icon
		Item? item = null;
		SpriteInfo? sprite;

		// Try parsing the item.
		// This will change in 1.6
		if (!string.IsNullOrEmpty(notice.Item)) {
			try {
				item = InventoryHelper.CreateItemById(notice.Item, 1);
			} catch (Exception ex) {
				Log($"Unable to get item instance for: {notice.Item}", LogLevel.Warn, ex);
				item = null;
			}
		}

		if (notice.IconType == NoticeIconType.Item) {
			sprite = item == null ? null : SpriteHelper.GetSprite(item);

		} else if (notice.IconType == NoticeIconType.ModTexture) {
			Texture2D? tex;
			if (!string.IsNullOrEmpty(notice.IconPath) && notice.ModContent != null)
				tex = notice.ModContent.Load<Texture2D>(notice.IconPath);
			else
				tex = null;

			sprite = tex == null ? null : new SpriteInfo(
				tex,
				notice.IconSourceRect ?? tex.Bounds
			);

		} else if (notice.IconType == NoticeIconType.Texture) {
			Texture2D? tex;
			if (!string.IsNullOrEmpty(notice.IconPath))
				tex = Mod.Helper.GameContent.Load<Texture2D>(notice.IconPath);
			else if (notice.IconSource.HasValue)
				tex = SpriteHelper.GetTexture(notice.IconSource.Value);
			else
				tex = null;

			sprite = tex == null ? null : new SpriteInfo(
				tex,
				notice.IconSourceRect ?? tex.Bounds
			);

		} else {
			item = null;
			sprite = null;
		}

		if (notice.Translation != null && !string.IsNullOrEmpty(notice.I18nKey))
			notice.Description = notice.Translation.Get(notice.I18nKey).ToString();

		string? desc = string.IsNullOrEmpty(notice.Description) ? null : StringTokenizer.ParseString(notice.Description, state);
		if (desc != null && Mod.Config.DebugMode && !string.IsNullOrEmpty(key))
			desc = $"{desc} @C@c@h(#{key})";

		return new RichEvent(
			(first || notice.ShowEveryDay) ? desc : null,
			null,
			sprite,
			item
		);
	}

	public IEnumerable<IRichEvent> GetEventsForDate(ulong seed, WorldDate date) {

		bool do_vanilla = true;

		Load();

		var state = new GameStateQuery.GameState(
			rnd: Game1.random,
			date: date,
			timeOfDay: 600,
			ticks: 0,
			pickedValue: Game1.random.NextDouble(),
			farmer: Game1.player,
			location: null,
			item: null,
			monitor: Mod.Monitor,
			trace: false
		);

		if (DataEvents != null)
			foreach (var entry in DataEvents) {
				IRichEvent? hydrated = HydrateEvent(entry.Value, date, state, entry.Key);
				if (hydrated != null)
					yield return hydrated;
			}

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

					SpriteInfo? sprite;

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

		if (evt == null && Mod.intLS != null && Mod.intLS.IsLoaded) {
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

	public static IRichEvent? GetTrashEvent(ulong seed, WorldDate date) {
		for (int i = 0; i < 8; i++) {
			Random rnd = new((date.TotalDays + 1) + ((int)seed / 2) + 777 + i * 77);

			int prewarm = rnd.Next(0, 100);
			for (int j = 0; j < prewarm; j++)
				rnd.NextDouble();

			prewarm = rnd.Next(0, 100);
			for (int j = 0; j < prewarm; j++)
				rnd.NextDouble();

			rnd.NextDouble();

			if (rnd.NextDouble() >= 0.002)
				continue;

			Item? item = InventoryHelper.CreateItemById("(H)66", 1);
			SpriteInfo? sprite = SpriteHelper.GetSprite(item);

			return new RichEvent(
				I18n.Page_Fortune_GarbageHat(),
				null,
				sprite
			);
		}

		return null;
	}

	private static IRichEvent? GetVanillaEventForDate(ulong seed, WorldDate date) {
		int days = date.TotalDays + 1;

		if (days == 31)
			return null;

		Random rnd = new(days + (int)seed / 2);

		// Don't track any of the Community Center / Joja events because
		// those all rely on game state and are not random based on the
		// date they happen on. Same with weddings preventing events.

		if (rnd.NextDouble() < 0.01 && !date.Season.Equals("winter"))
			return new RichEvent(
				I18n.Page_Fortune_Event_Fairy(),
				null,
				new SpriteInfo(
					Game1.mouseCursors,
					new Rectangle(16, 592, 16, 16)
				)
			);

		if (rnd.NextDouble() < 0.01)
			return new RichEvent(
				I18n.Page_Fortune_Event_Witch(),
				null,
				new SpriteInfo(
					Game1.mouseCursors,
					new Rectangle(277, 1886, 34, 29)
				)
			);

		if (rnd.NextDouble() < 0.01)
			return new RichEvent(
				I18n.Page_Fortune_Event_Meteorite(),
				null,
				new SpriteInfo(
					Game1.objectSpriteSheet,
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

	private static IRichEvent? GetLuckSkillEventForDate(ulong seed, WorldDate date) {
		int days = date.TotalDays + 1 + 999999;

		if (days == 31)
			return null;

		Random rnd = new(days + (int)seed / 2);

		// Don't track any of the Community Center / Joja events because
		// those all rely on game state and are not random based on the
		// date they happen on. Same with weddings preventing events.

		if (rnd.NextDouble() < 0.01 && !date.Season.Equals("winter"))
			return new RichEvent(
				I18n.Page_Fortune_Event_Fairy(),
				null,
				new SpriteInfo(
					Game1.mouseCursors,
					new Rectangle(16, 592, 16, 16)
				)
			);

		if (rnd.NextDouble() < 0.01)
			return new RichEvent(
				I18n.Page_Fortune_Event_Witch(),
				null,
				new SpriteInfo(
					Game1.mouseCursors,
					new Rectangle(277, 1886, 34, 29)
				)
			);

		if (rnd.NextDouble() < 0.01)
			return new RichEvent(
				I18n.Page_Fortune_Event_Meteorite(),
				null,
				new SpriteInfo(
					Game1.objectSpriteSheet,
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
