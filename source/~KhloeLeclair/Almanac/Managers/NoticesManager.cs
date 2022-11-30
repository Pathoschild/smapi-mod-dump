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
using StardewModdingAPI.Utilities;

using StardewValley;
using StardewValley.TerrainFeatures;

using Leclair.Stardew.Almanac.Models;
using StardewValley.GameData.Movies;

namespace Leclair.Stardew.Almanac.Managers;

public class NoticesManager : BaseManager {

	// This is our bush.
	// There are many like it, but this is ours.
	private Bush? bush;

	// Mods
	private readonly Dictionary<IManifest, Func<int, WorldDate, IEnumerable<Tuple<string, Texture2D, Rectangle?, Item>>>> ModHooks = new();
	private readonly Dictionary<IManifest, Func<int, WorldDate, IEnumerable<IRichEvent>>> InterfaceHooks = new();

	private Dictionary<string, LocalNotice>? DataEvents;
	private bool Loaded = false;

	public NoticesManager(ModEntry mod) : base(mod) { }

	public void Invalidate() {
		Loaded = false;
		DataEvents = null;
		Mod.Helper.GameContent.InvalidateCache(AssetManager.LocalNoticesPath);
		Mod.Helper.GameContent.InvalidateCache(AssetManager.NPCOverridesPath);
	}

	#region Event Handlers

	[Subscriber]
	private void OnAssetInvalidated(object? sender, AssetsInvalidatedEventArgs e) {
		foreach(var name in e.Names)
			if (name.IsEquivalentTo(AssetManager.LocalNoticesPath)) {
				Loaded = false;
				DataEvents = null;
			}
	}

	[Subscriber]
	private void OnAssetRequested(object? sender, AssetRequestedEventArgs e) {
		if (e.Name.IsEquivalentTo(AssetManager.NPCOverridesPath))
			e.LoadFrom(
				() => new Dictionary<string, Models.NPCOverride>(),
				priority: AssetLoadPriority.Low
			);

		if (e.Name.IsEquivalentTo(AssetManager.LocalNoticesPath))
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
			if (!cp.HasFile("notices.json"))
				continue;

			Dictionary<string, LocalNotice>? data;
			try {
				data = cp.ReadJsonFile<Dictionary<string, LocalNotice>>("notices.json");
				if (data == null)
					throw new ArgumentNullException();
			} catch (Exception ex) {
				Log($"Invalid or empty notices.json for content pack '{cp.Manifest.Name}'", LogLevel.Error, ex);
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

		DataEvents = Mod.Helper.GameContent.Load<Dictionary<string, LocalNotice>>(AssetManager.LocalNoticesPath);
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

	public void RegisterHook(IManifest mod, Func<int, WorldDate, IEnumerable<Tuple<string, Texture2D, Rectangle?, Item>>> hook) {
		if (InterfaceHooks.ContainsKey(mod))
			InterfaceHooks.Remove(mod);

		if (hook == null && ModHooks.ContainsKey(mod))
			ModHooks.Remove(mod);
		else if (hook != null)
			ModHooks[mod] = hook;
	}

	public void RegisterHook(IManifest mod, Func<int, WorldDate, IEnumerable<IRichEvent>> hook) {
		if (ModHooks.ContainsKey(mod))
			ModHooks.Remove(mod);

		if (hook == null && InterfaceHooks.ContainsKey(mod))
			InterfaceHooks.Remove(mod);
		else if (hook != null)
			InterfaceHooks[mod] = hook;
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

		switch(notice.Period) {
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
			} catch(Exception ex) {
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

		if (notice.Translation != null && ! string.IsNullOrEmpty(notice.I18nKey))
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

	public IEnumerable<IRichEvent> GetEventsForDate(int seed, WorldDate date) {

		Load();

		var state = new GameStateQuery.GameState(
			Random: Game1.random,
			Date: date,
			TimeOfDay: 600,
			Ticks: 0,
			Farmer: Game1.player,
			Location: null,
			Item: null,
			Monitor: Mod.Monitor,
			DoTrace: false
		);

		if (DataEvents != null)
			foreach(var entry in DataEvents) {
				IRichEvent? hydrated = HydrateEvent(entry.Value, date, state, entry.Key);
				if (hydrated != null)
					yield return hydrated;
			}

		foreach (var ihook in InterfaceHooks.Values) {
			if (ihook != null)
				foreach (var entry in ihook(seed, date)) {
					if (entry == null)
						continue;

					yield return entry;
				}
		}

		foreach (var hook in ModHooks.Values) {
			if (hook != null)
				foreach (var entry in hook(seed, date)) {
					if (entry == null || string.IsNullOrEmpty(entry.Item1))
						continue;

					SpriteInfo? sprite;

					if (entry.Item3.HasValue && entry.Item3.Value == Rectangle.Empty)
						sprite = null;
					else if (entry.Item2 != null)
						sprite = new(
							entry.Item2,
							entry.Item3 ?? entry.Item2.Bounds
						);
					else if (entry.Item4 != null)
						sprite = SpriteHelper.GetSprite(entry.Item4);
					else
						sprite = null;

					yield return new RichEvent(
						entry.Item1,
						null,
						sprite,
						entry.Item4
					);
				}
		}

		foreach (var evt in GetVanillaEventsForDate(date)) {
			if (evt != null)
				yield return evt;
		}
	}

	#endregion

	#region Vanilla Events

	public IEnumerable<IRichEvent> GetVanillaEventsForDate(WorldDate date) {

		// Berry Season
		bool gathering = Mod.Config.NoticesShowGathering;

		if (gathering && bush == null)
			bush = new();

		if (gathering && bush!.inBloom(date.Season, date.DayOfMonth)) {
			Item? berry = null;
			if (date.SeasonIndex == 0)
				berry = InventoryHelper.CreateItemById("(O)296", 1); // Salmonberry

			else if (date.SeasonIndex == 2)
				berry = InventoryHelper.CreateItemById("(O)410", 1); // Blackberry

			if (berry != null) {
				bool first_day = date.DayOfMonth == 1 || !bush.inBloom(date.Season, date.DayOfMonth - 1);
				int last = date.DayOfMonth;

				// If it's the first day, then we also need the last day
				// so we can display a nice string to the user.
				if (first_day)
					for (int d = date.DayOfMonth + 1; d <= ModEntry.DaysPerMonth; d++) {
						if (bush.inBloom(date.Season, d))
							last = d;
						else
							break;
					}

				yield return new RichEvent(
					null,
					first_day ?
						FlowHelper.Translate(
							Mod.Helper.Translation.Get("page.notices.season"),
							new {
								item = berry.DisplayName,
								start = new SDate(date.DayOfMonth, date.Season).ToLocaleString(withYear: false),
								end = new SDate(last, date.Season).ToLocaleString(withYear: false)
							},
							align: Alignment.Middle
						) : null,
					SpriteHelper.GetSprite(berry),
					berry
				);
			}
		}

		// Festivals
		if (Mod.Config.NoticesShowFestivals && Utility.isFestivalDay(date.DayOfMonth, date.Season)) {
			var data = Game1.temporaryContent.Load<Dictionary<string, string>>("Data\\Festivals\\" + date.Season + date.DayOfMonth);
			if (data.ContainsKey("name") && data.ContainsKey("conditions")) {
				string name = data["name"];
				string[] conds = data["conditions"].Split('/');
				string? where = conds.Length >= 1 ? conds[0] : null;

				int start = -1;
				int end = -1;

				if (conds.Length >= 2) {
					string[] bits = conds[1].Split(' ');
					if (bits.Length >= 2) {
						start = Convert.ToInt32(bits[0]);
						end = Convert.ToInt32(bits[1]);
					}
				}

				foreach (GameLocation loc in Game1.locations) {
					if (loc?.Name == where) {
						where = Mod.GetLocationName(loc) ?? where;
						break;
					}
				}

				yield return new RichEvent(
					null,
					FlowHelper.Translate(
						Mod.Helper.Translation.Get("page.notices.festival"),
						new {
							name,
							where,
							start = Mod.FormatTime(start),
							end = Mod.FormatTime(end)
						},
						align: Alignment.Middle
					),
					new SpriteInfo(
						Game1.temporaryContent.Load<Texture2D>("LooseSprites\\Billboard"),
						new Rectangle(
							1, 398,
							84, 12
						),
						baseFrames: 6
					)
				);
			}
		}

		// Weddings / Anniversaries / Children
		foreach (var who in Game1.getAllFarmers()) {
			// Children
			// TODO: This.

			if (!Mod.Config.NoticesShowAnniversaries)
				continue;

			// Player Weddings and Anniversaries
			// TODO: This.

			// NPC Weddings and Anniversaries
			if ((who.isEngaged() || who.isMarried()) && who.friendshipData != null) {
				foreach (var entry in who.friendshipData.Pairs) {
					if (entry.Value == null || entry.Value.WeddingDate == null)
						continue;

					if (entry.Value.IsDivorced())
						continue;

					WorldDate? wedding = entry.Value.WeddingDate;
					if (wedding == null || wedding.SeasonIndex != date.SeasonIndex || wedding.DayOfMonth != date.DayOfMonth)
						continue;

					NPC? spouse = Game1.getCharacterFromName(entry.Key);
					if (spouse == null)
						continue;

					char last = spouse.displayName.Last<char>();

					bool no_s = last == 's' ||
						LocalizedContentManager.CurrentLanguageCode ==
						LocalizedContentManager.LanguageCode.de &&
							(last == 'x' || last == 'ß' || last == 'z');

					var pendant = InventoryHelper.CreateItemById("(O)460", 1); // Mermaid Pendant
					var sprite = SpriteHelper.GetSprite(pendant);

					// Wedding?
					if (date.Year == wedding.Year) {
						yield return new RichEvent(
							null,
							FlowHelper.Translate(
								Mod.Helper.Translation.Get(
									no_s ?
										"page.notices.wedding.no-s"
										: "page.notices.wedding.s"
								),
								new {
									name = who.displayName,
									spouse = spouse.displayName
								},
								align: Alignment.Middle
							),
							sprite
						);
					} else {
						yield return new RichEvent(
							null,
							FlowHelper.Translate(
								Mod.Helper.Translation.Get(
									no_s ?
										"page.notices.anniversary.no-s"
										: "page.notices.anniversary.s"
								),
								new {
									name = who.displayName,
									spouse = spouse.displayName
								},
								align: Alignment.Middle
							),
							sprite
						);
					}
				}
			}
		}

		// Trains
		if (Mod.Config.NoticesShowTrains) {
			int time = TrainHelper.GetTrainTime(date);
			if (time >= 0)
				yield return new RichEvent(
					null,
					FlowHelper.Translate(
						Mod.Helper.Translation.Get("page.notices.train"),
						new {
							time = Mod.FormatTime(time)
						},
						align: Alignment.Middle
					),
					new SpriteInfo(
						Game1.mouseCursors,
						TrainHelper.TRAIN
					)
				);
		}

		// Spring
		if (date.SeasonIndex == 0) {

		}

		// Summer
		else if (date.SeasonIndex == 1) {

			// Extra Foragables
			if (gathering && date.DayOfMonth >= 12 && date.DayOfMonth <= 14) {
				yield return new RichEvent(
					date.DayOfMonth == 12 ?
						I18n.Page_Notices_Summer() : null,
					null,
					SpriteHelper.GetSprite(
						InventoryHelper.CreateItemById("(O)394", 1) // Rainbow Shell
					)
				);
			}
		}

		// Fall
		else if (date.SeasonIndex == 2) {

			if (gathering && date.DayOfMonth >= 15 && date.DayOfMonth <= 28) {
				Item? nut = InventoryHelper.CreateItemById("(O)408", 1);

				// Should never be null but just in case.
				if (nut != null)
					yield return new RichEvent(
						null,
						date.DayOfMonth == 15 ?
						FlowHelper.Translate(
							Mod.Helper.Translation.Get("page.notices.season"),
							new {
								item = nut.DisplayName,
								start = new SDate(15, date.Season).ToLocaleString(withYear: false),
								end = new SDate(28, date.Season).ToLocaleString(withYear: false),
							},
							align: Alignment.Middle
						) : null,
						SpriteHelper.GetSprite(nut),
						nut
					);
			}
		}

		// Winter
		else if (date.SeasonIndex == 3) {

			// Night Market
			if (date.DayOfMonth >= 15 && date.DayOfMonth <= 17) {
				yield return new RichEvent(
					date.DayOfMonth == 15 ?
						I18n.Page_Notices_Market() : null,
					null,
					new SpriteInfo(
						Game1.mouseCursors,
						new Rectangle(346, 392, 8, 8)
					)
				);
			}

		}


		// Traveling Merchant
		if (Mod.Config.NoticesShowMerchant != MerchantMode.Disabled && date.DayOfMonth % 7 % 5 == 0) {
			var sprite = new SpriteInfo(
				Game1.mouseCursors,
				new Rectangle(193, 1412, 18, 18)
			);

			if (Mod.Config.NoticesShowMerchant == MerchantMode.Visit)
				yield return new RichEvent(
					null,
					FlowHelper.Builder()
						.FormatText(I18n.Page_Notices_Merchant(), align: Alignment.Middle)
						.Build(),
					sprite
				);

			else {
				var stock = Utility.getTravelingMerchantStock((int) (Game1.uniqueIDForThisGame + (uint)date.TotalDays + 1));
				if (stock.Count > 0) {
					var builder = FlowHelper.Builder()
						.FormatText(I18n.Page_Notices_Merchant_Stock(), align: Alignment.Middle)
						.Text("\n  ");

					bool first = true;

					foreach (var pair in stock) {
						var item = pair.Key;
						if (item.Stack < 1 && !item.IsInfiniteStock())
							continue;

						if (first)
							first = false;
						else
							builder.Text(", ", shadow: false);

						if (item is SObject sobj)
							builder
								.Sprite(SpriteHelper.GetSprite(sobj), scale: 2, align: Alignment.Middle)
								.Text(" ");

						builder.Text(item.DisplayName, shadow: false);
					}


					yield return new RichEvent(
						null,
						builder.Build(),
						sprite: sprite
					);
				}
			}
		}
	}

	#endregion

}
