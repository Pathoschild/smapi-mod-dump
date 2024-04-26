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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;

using HarmonyLib;

using Leclair.Stardew.Common;
using Leclair.Stardew.Common.Events;
using Leclair.Stardew.MoreNightlyEvents.Events;
using Leclair.Stardew.MoreNightlyEvents.Models;
using Leclair.Stardew.MoreNightlyEvents.Patches;

using Microsoft.Xna.Framework.Content;

using Netcode;

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;

using StardewValley;
using StardewValley.Delegates;
using StardewValley.Events;
using StardewValley.Locations;
using StardewValley.TokenizableStrings;
using StardewValley.Triggers;

namespace Leclair.Stardew.MoreNightlyEvents;

public class ModEntry : ModSubscriber {

#if DEBUG
	public static readonly LogLevel DEBUG_LEVEL = LogLevel.Debug;
#else
	public static readonly LogLevel DEBUG_LEVEL = LogLevel.Trace;
#endif

	public const string EVENTS_PATH = @"Mods/leclair.morenightlyevents/Events";

	public const string IGNORE_SEASON_DATA = @"leclair.morenightlyevents/AlwaysInSeason";

#nullable disable

	public static ModEntry Instance { get; private set; }

	public ModConfig Config { get; private set; }

	internal Harmony Harmony { get; private set; }

#nullable enable

	public string? ForcedEvent { get; internal set; }

	internal Dictionary<string, ITranslationHelper>? EventTranslators;

	internal Dictionary<string, BaseEventData>? Events;

	internal List<KeyValuePair<string, BaseEventData>>? SortedEvents;


	public override void Entry(IModHelper helper) {
		base.Entry(helper);

		Instance = this;

		// Harmony
		Harmony = new Harmony(ModManifest.UniqueID);

		Utility_Patches.Patch(this);
		TreeCrop_Patches.Patch(this);

		// Read Config
		Config = Helper.ReadConfig<ModConfig>();

		// Init
		I18n.Init(Helper.Translation);

		CheckRecommendedIntegrations();
	}

	#region Events

	bool ForceEventTrigger(string[] args, TriggerActionContext context, out string? error) {
		string key = string.Join(' ', args).Trim();
		if (key.Equals("clear", StringComparison.OrdinalIgnoreCase)) {
			ForcedEvent = null;

		} else {
			LoadEvents();
			if (string.IsNullOrWhiteSpace(key) || !Events.ContainsKey(key)) {
				error = $"This is no event with the ID '{key}'.";
				return false;
			}

			ForcedEvent = key;
		}

		error = null;
		return true;
	}

	[Subscriber]
	private void OnGameLaunched(object? sender, GameLaunchedEventArgs e) {

		TriggerActionManager.RegisterAction("leclair.morenightlyevents_ForceEvent", ForceEventTrigger);

		// Commands
		Helper.ConsoleCommands.Add("mne_invalidate", "Invalidate the cached event data.", (_, _) => {
			Helper.GameContent.InvalidateCache(EVENTS_PATH);
			Log($"Invalidated cache.", LogLevel.Info);
		});

		Helper.ConsoleCommands.Add("mne_list", "List all available nightly events from MRE.", (_, _) => {
			LoadEvents();

			List<string[]> entries = [];

			foreach(var pair in SortedEvents) {
				var evt = pair.Value;
				entries.Add([
					evt.Id,
					$"{evt.Type}",
					$"{evt.Priority}",
					$"{evt.TargetMap}"
				]);
			}

			if (entries.Count > 0)
				LogTable(["Event Id", "Type", "Priority", "Target Map"], entries, LogLevel.Info);

			Log($"Total Events: {Events.Count}", LogLevel.Info);
		});

		Helper.ConsoleCommands.Add("mne_interrupt", "Interrupt the active event, immediately ending it.", (_, _) => {
			FarmEventInterrupter.Interrupt();
			Log($"Interrupted events.", LogLevel.Info);
		});

		Helper.ConsoleCommands.Add("mne_pick", "Pick an event for the given day.", (_, args) => {
			if (!ArgUtility.TryGetOptionalInt(args, 0, out int day, out string? error, -1)) {
				Log(error, LogLevel.Warn);
				return;
			}

			WorldDate date = WorldDate.Now();
			if (day != -1)
				date.TotalDays = day;

			Log($"Showing event pick for: Year {date.Year} - Season: {date.Season} - Day: {date.DayOfMonth}", LogLevel.Info);
			var evt = SelectEvent(date, LogLevel.Debug, true);
			Log($"Selected Event: {evt?.Id}", LogLevel.Info);
		});

		Helper.ConsoleCommands.Add("mne_test", "Test an event by forcing it to happen the next night.", (_, args) => {
			string key = string.Join(' ', args).Trim();
			if (key.Equals("clear", StringComparison.OrdinalIgnoreCase)) {
				ForcedEvent = null;

			} else if (!string.IsNullOrWhiteSpace(key)) {
				LoadEvents();
				if (!Events.ContainsKey(key)) {
					Log($"There is no event with the ID '{key}'.", LogLevel.Warn);
					return;
				}

				ForcedEvent = key;
			}

			if (string.IsNullOrWhiteSpace(ForcedEvent))
				Log($"There is currently no event scheduled for tonight.", LogLevel.Info);
			else
				Log($"The '{ForcedEvent}' is scheduled to run tonight. Use the command 'mne_test clear' to cancel.", LogLevel.Info);
		});
	}

	[Subscriber]
	private void OnAssetInvalidated(object? sender, AssetsInvalidatedEventArgs e) {
		foreach (var name in e.Names) {
			if (name.IsEquivalentTo(EVENTS_PATH)) {
				Events = null;
				SortedEvents = null;
			}
		}
	}

	[Subscriber]
	private void OnAssetRequested(object? sender, AssetRequestedEventArgs e) {
		if (e.Name.IsEquivalentTo(EVENTS_PATH))
			e.LoadFrom(LoadAssetEvents, AssetLoadPriority.Exclusive);
	}

	[Subscriber]
	private void OnDayStarted(object? sender, DayStartedEventArgs e) {
		ForcedEvent = null;
	}

	#endregion

	#region Event Data

	[return: NotNullIfNotNull(nameof(input))]
	public string? TokenizeFromEvent(string eventId, string? input, Farmer? who = null, Random? rnd = null) {
		if (string.IsNullOrWhiteSpace(input) || EventTranslators is null || ! EventTranslators.TryGetValue(eventId, out var translator))
			return input;

		bool ParseToken(string[] query, out string? replacement, Random? random, Farmer? player) {
			if (!ArgUtility.TryGet(query, 0, out string? cmd, out string? error))
				return TokenParser.LogTokenError(query, error, out replacement);

			if (cmd is null || ! cmd.Equals("LocalizedText")) {
				replacement = null;
				return false;
			}

			if (!ArgUtility.TryGet(query, 1, out string? key, out error))
				return TokenParser.LogTokenError(query, error, out replacement);

			var tl = translator.Get(key);
			if (!tl.HasValue()) { 
				replacement = null;
				return false;
			}

			Dictionary<int, string> replacements;
			if (query.Length > 2) {
				replacements = new();
				for (int i = 2; i < query.Length; i++) {
					replacements[i - 2] = query[i];
				}

			} else
				replacements = [];

			replacement = tl.Tokens(replacements).ToString();
			return true;
		}

		return TokenParser.ParseText(input, rnd, ParseToken, who);
	}


	public bool TryGetEvent<T>(string key, out T? value) where T : BaseEventData {
		LoadEvents();
		if (Events.TryGetValue(key, out var val) && val is T tval) {
			value = tval;
			return true;
		}

		value = default;
		return false;
	}

	/// <summary>
	/// Load 
	/// </summary>
	[MemberNotNull(nameof(Events))]
	[MemberNotNull(nameof(SortedEvents))]
	[MemberNotNull(nameof(EventTranslators))]
	private void LoadEvents() {
		Events ??= Helper.GameContent.Load<Dictionary<string, BaseEventData>>(EVENTS_PATH);

		foreach(var evt in Events.Values) {
			evt.Conditions ??= new();
			foreach(var cnd in evt.Conditions) {
				if (cnd.Chance < 0 || cnd.Chance > 1) {
					Log($"Ignoring condition of event '{evt.Id}' with invalid Chance '{cnd.Chance}': chance must be number in range 0 to 1 (inclusive)", LogLevel.Warn);
					cnd.Chance = 0;
				}
			}
		}

		EventTranslators ??= new();

		SortedEvents = Events.ToList();
		SortedEvents.Sort((a, b) => {
			if (a.Value.Priority < b.Value.Priority) return 1;
			if (a.Value.Priority > b.Value.Priority) return -1;

			return a.Key.CompareTo(b.Key);
		});
	}

	/// <summary>
	/// Load all the events defined in assets / content packs.
	/// </summary>
	private Dictionary<string, BaseEventData> LoadAssetEvents() {
		var result = new Dictionary<string, BaseEventData>();

		Dictionary<string, BaseEventData>? data = null;
		EventTranslators = new();

		try {
			data = Helper.ModContent.Load<Dictionary<string, BaseEventData>>("assets/events.json");
		} catch (Exception ex) {
			try {
				var dlist = Helper.ModContent.Load<List<BaseEventData>>("assets/events.json");
				if (dlist is not null) {
					data = new();
					foreach (var entry in dlist) {
						if (!string.IsNullOrEmpty(entry.Id) && !data.ContainsKey(entry.Id))
							data.Add(entry.Id, entry);
					}
				}
			} catch (Exception) {
				/* no op */
			}

			if (data is null)
				Log($"The event.json asset file is invalid.", LogLevel.Error, ex);
		}

		if (data is not null)
			foreach (var entry in data) {
				entry.Value.Id = entry.Key;
				if (result.TryAdd(entry.Key, entry.Value))
					EventTranslators.TryAdd(entry.Key, Helper.Translation);
			}

		foreach (var cp in Helper.ContentPacks.GetOwned()) {
			if (!cp.HasFile("events.json"))
				continue;

			data = null;

			try {
				data = cp.ReadJsonFile<Dictionary<string, BaseEventData>>("events.json");
			} catch(Exception ex) {
				try {
					var dlist = cp.ReadJsonFile<List<BaseEventData>>("events.json");
					if (dlist is not null) {
						data = new();
						foreach(var entry in dlist) {
							if (!string.IsNullOrEmpty(entry.Id) && !data.ContainsKey(entry.Id))
								data.Add(entry.Id, entry);
						}
					}
				} catch(Exception) {
					/* no op */
				}

				if (data is null)
					Log($"The event.json file of {cp.Manifest.Name} is invalid.", LogLevel.Error, ex);
			}

			if (data is not null)
				foreach(var entry in data) {
					entry.Value.Id = entry.Key;
					if (result.TryAdd(entry.Key, entry.Value))
						EventTranslators.TryAdd(entry.Key, cp.Translation);
				}
		}

		return result;
	}

	#endregion

	#region Farm Event Picker

	/// <summary>
	/// Determine whether or not we can replace an existing <see cref="FarmEvent"/>
	/// with a new instance. This logic is used to avoid overriding important events
	/// from the base game, such as the earthquake event. It also preserves the
	/// vanilla behavior or not having a night event the night before a wedding.
	/// </summary>
	/// <param name="evt">The event we want to replace.</param>
	/// <returns>Whether or not the event can be replaced.</returns>
	public bool CanReplaceEvent(FarmEvent? evt) {
		if (evt is null) {
			if (Game1.weddingToday)
				return false;

			foreach(Farmer who in Game1.getOnlineFarmers()) {
				Friendship spouse = who.GetSpouseFriendship();
				if (spouse is not null && spouse.IsMarried() && spouse.WeddingDate == Game1.Date)
					return false;
			}

			return true;
		}

		if (evt is FairyEvent)
			return true;
		if (evt is WitchEvent)
			return true;
		if (evt is SoundInTheNightEvent) {
			var behavior = Helper.Reflection.GetField<NetInt>(evt, "behavior", false)?.GetValue();
			if (behavior is not null && behavior.Value != 4 && behavior.Value != 5)
				return true;
		}

		return false;
	}

	private void ApplyDate(WorldDate date) {
		Game1.dayOfMonth = date.DayOfMonth;
		Game1.season = date.Season;
		Game1.year = date.Year;

		// We also want to set this.
		Game1.stats.DaysPlayed = (uint) (date.TotalDays + 1);
	}

	public BaseEventData? SelectEvent(WorldDate? date = null, LogLevel? useLevel = null, bool extraDebug = false) {
		if (date is null || date == Game1.Date)
			return SelectEventImpl(useLevel, extraDebug);

		// No such luck, we need to temporarily change the date
		// for our game state queries to run correctly.
		var old_date = Game1.Date;
		ApplyDate(date);

		try {
			return SelectEventImpl(useLevel, extraDebug);
		} finally {
			ApplyDate(old_date);
		}
	}

	private BaseEventData? SelectEventImpl(LogLevel? useLevel = null, bool extraDebug = false) { 
		LoadEvents();
		if (ForcedEvent is not null)
			return Events.GetValueOrDefault(ForcedEvent);

		Random rnd = Utility.CreateDaySaveRandom();

		List<(BaseEventData, float)> matchedEvents = new();
		double totalWeight = 0;

		LogLevel level = useLevel ?? DEBUG_LEVEL;
		Log($"Total Events: {Events.Count}", level);

		foreach (var pair in SortedEvents) {
			var evt = pair.Value;
			if (evt?.Conditions is null || evt.Conditions.Count == 0)
				continue;

			double? val = null;

			foreach (var cond in evt.Conditions) {
				if (cond.Chance <= 0 || string.IsNullOrWhiteSpace(cond.Condition))
					continue;

				if (GameStateQuery.CheckConditions(cond.Condition, random: rnd)) {
					if (!val.HasValue)
						val = rnd.NextDouble();

					if (cond.Chance >= 1 || cond.Chance >= val.Value) {
						// If we encounter a passing exclusive event condition, we
						// just pass the first one.
						if (cond.IsExclusive) {
							Log($"Selected exclusive event '{evt.Id}' with condition '{cond.Condition}'", level);
							return evt;

						} else {
							// But if it's not exclusive, we add it to a pool.
							totalWeight += cond.Weight;
							matchedEvents.Add((evt, cond.Weight));
						}

						// Either way, we stop after our first matching condition.
						break;
					}
				}
			}
		}

		// Now, pick a random point somewhere within the total chances.
		double remainingWeight = totalWeight * rnd.NextDouble();

		Log($"Possible Events: {matchedEvents.Count} (Total: {totalWeight}, Random: {remainingWeight})", level);
		List<string[]>? events = extraDebug ? new() : null;

		BaseEventData? selected = null;
		foreach (var pair in matchedEvents) {
			if (selected is null && remainingWeight <= pair.Item2) {
				selected = pair.Item1;
				if (!extraDebug)
					break;
			}

			events?.Add([
				selected == pair.Item1 ? "**" : "",
				pair.Item1.Id,
				$"{pair.Item2}",
				string.Format("{0:0.00}", remainingWeight)
			]);

			remainingWeight -= pair.Item2;
		}

		if (events is not null)
			LogTable(["", "Id", "Weight", "Remaining"], events, level);

		return selected;
	}

	public FarmEvent? PickEvent(FarmEvent? existing) {
		if (!CanReplaceEvent(existing))
			return existing;

		BaseEventData? evt = SelectEvent();

		if (evt is null) 
			return existing;

		Log($"Using Nightly Event: {evt.Id}", LogLevel.Debug);

		if (evt is PlacementEventData ped)
			return new PlacementEvent(evt.Id, ped);

		if (evt is ScriptEventData sed)
			return new ScriptEvent(evt.Id, sed);

		if (evt is GrowthEventData ged)
			return new GrowthEvent(evt.Id, ged);

		Log($"No matching event type. Ignoring event: {evt.Id}", LogLevel.Warn);
		return existing;
	}

#endregion

}
