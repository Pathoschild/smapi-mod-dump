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
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using StardewValley;
using StardewValley.Locations;

using StardewModdingAPI;
using StardewValley.Characters;
using StardewValley.Network;

namespace Leclair.Stardew.Common;

/*

/// <summary>
/// Represents a snapshot of game state, used for evaluating game state queries
/// or tokenizable strings. A game state may represent the current game state,
/// or the state of the game at some other point.
/// </summary>
public interface IGameState {
	/// <summary>
	/// The <see cref="System.Random"/> instance to use for generating
	/// random numbers.
	/// </summary>
	Random Random { get; }

	/// <summary>
	/// The in-game date. Use this rather than <see cref="Game1.Date"/> in case
	/// the game state represents a different game state.
	/// </summary>
	WorldDate Date { get; }

	/// <summary>
	/// The in-game time of day. Use this rather than <see cref="Game1.timeOfDay"/>
	/// in case the game state represents a different game state.
	/// </summary>
	int TimeOfDay { get; }

	/// <summary>
	/// The number of ellapsed ticks. Use this rather than <see cref="Game1.ticks"/>
	/// in case the game state represents a different game state.
	/// </summary>
	int Ticks { get; }

	/// <summary>
	/// The player for which this game state is being evaluated. This may be
	/// <see cref="Game1.player"/> but is not required to be.
	/// </summary>
	Farmer Farmer { get; }

	/// <summary>
	/// The game location for which this game state is being evaluated. This
	/// may be <see cref="Game1.currentLocation"/> but is not required to be.
	/// </summary>
	GameLocation? Location { get; }

	/// <summary>
	/// An item for which this game state is being evaluated, if there is a
	/// relevant item.
	/// </summary>
	Item? Item { get; }

	/// <summary>
	/// A monitor that can be used for logging feedback when evaluating with
	/// this game state. This may be null.
	/// </summary>
	IMonitor? Monitor { get; }

	/// <summary>
	/// Whether or not we want trace-level logging when evaluating with this
	/// game state.
	/// </summary>
	bool DoTrace { get; }
}



public interface IParsedQuery {
	/// <summary>
	/// Evaluate a parsed game state query.
	/// </summary>
	/// <param name="state">The game state to use.</param>
	bool Evaluate(IGameState state);
}


public interface IGameStateQueryApi {

	/// <summary>
	/// Called when evaluating a game state query. This method is responsible
	/// for actually checking if the game state matches the query condition.
	/// </summary>
	/// <param name="state">The game state being evaluated.</param>
	delegate bool EvaluationDelegate(IGameState state);

	/// <summary>
	/// Called when parsing a game state query from an input string. This
	/// method is responsible for returning an evaluation delegate that
	/// will be called when the query is evaulated.
	/// </summary>
	/// <param name="args">An array of arguments passed to the condition.
	/// This is a list of values that were separated by spaces.</param>
	delegate EvaluationDelegate ConditionDelegate(string[] args);

	/// <summary>
	/// Called when using <see cref="CheckMatchingPlayers(string, IGameState, EvaluateFarmerDelegate)"/>
	/// </summary>
	/// <param name="farmer">The targeted player to evaluate.</param>
	delegate bool EvaluateFarmerDelegate(Farmer farmer);

	/// <summary>
	/// Register a new condition that can be checked using game state queries.
	/// </summary>
	/// <param name="key">The condition name.</param>
	/// <param name="method">The method for handling the condition.</param>
	void RegisterCondition(string key, ConditionDelegate method);

	#region Condition Helpers

	/// <summary>
	/// Check a condition against one or more players depending on the provided
	/// argument, where <paramref name="who"/> is one of <c>Any</c>, <c>All</c>,
	/// <c>Current</c>, <c>Host</c>, <c>Target</c>, or a unique multiplayer id.
	/// </summary>
	/// <param name="who">The player(s) to target.</param>
	/// <param name="state">The game state being evaulated.</param>
	/// <param name="method">A method to run for each targeted player. If
	/// <paramref name="who"/> is <c>Any</c> any this method ever returns
	/// true, then CheckMatchingPlayers will immediately return true. If
	/// who is <c>All</c>, then CheckMatchingPlayers will only return true
	/// if this method returns true for every targeted player.</param>
	bool CheckMatchingPlayers(string who, IGameState state, EvaluateFarmerDelegate method);

	/// <summary>
	/// Get a specific <see cref="GameLocation"/> depending on the provided
	/// argument.
	/// </summary>
	/// <param name="name"><c>Here</c>, <c>Target</c>, or a location's
	/// internal name.</param>
	/// <param name="state">The game state being evaluated.</param>
	/// <returns></returns>
	GameLocation GetLocation(string name, IGameState state);

	#endregion

	#region Execute Queries

	/// <summary>
	/// Construct a new <see cref="IGameState"/> instance that can be used when
	/// evaluating a game state query.
	/// </summary>
	/// <param name="random"></param>
	/// <param name="date"></param>
	/// <param name="timeOfDay"></param>
	/// <param name="ticks"></param>
	/// <param name="farmer"></param>
	/// <param name="location"></param>
	/// <param name="item"></param>
	/// <param name="monitor"></param>
	/// <param name="doTrace"></param>
	/// <returns></returns>
	IGameState BuildState(
		Random? random = null,
		WorldDate? date = null,
		int? timeOfDay = null,
		int? ticks = null,
		Farmer? farmer = null,
		GameLocation? location = null,
		Item? item = null,
		IMonitor? monitor = null,
		bool doTrace = false
	);

	/// <summary>
	/// Parse and evaluate a game state query. This is less efficient than calling
	/// <see cref="ParseConditions(string, bool, bool, IMonitor?)"/> and then
	/// calling <see cref="IParsedQuery.Evaluate(IGameState)"/>. However, if you
	/// aren't using a query frequently it shouldn't be an issue.
	/// </summary>
	/// <param name="query">The game state query to evaluate.</param>
	/// <param name="state">The game state to use.</param>
	bool CheckConditions(string query, IGameState state);

	/// <summary>
	/// Parse a game state query, returning a parsed object that can be used
	/// to evaluate the query.
	/// </summary>
	/// <param name="query">The game state query to parse.</param>
	/// <param name="skip_unknown">Whether or not unknown conditions should be
	/// skipped. When skipping an unknown condition, a <c>FALSE</c> condition
	/// will be added to the parsed conditions list, preventing the query from
	/// ever evaluating to true.</param>
	/// <param name="skip_error">Whether or not to skip errors when parsing
	/// conditions. When skipping an error, a <c>FALSE</c> condition will be
	/// added to the parsed conditions list, preventing the query from ever
	/// evaluating to true.</param>
	/// <param name="monitor">An <see cref="IMonitor"/> to log errors to when
	/// parsing conditions.</param>
	IParsedQuery ParseConditions(string query, bool skip_unknown = false, bool skip_error = false, IMonitor? monitor = null);

	#endregion

}


/// <summary>
/// A reimplementation of Stardew Valley 1.6's GameStateQuery system. This
/// implementation is based on the <see href="https://stardewvalleywiki.com/Modding:Migrate_to_Stardew_Valley_1.6#Game_state_queries">1.6 migration documentation</see>
/// available on the modding wiki, and has been lightly tested against known
/// GameStateQueries used in 1.6 to ensure they evaluate and return sane
/// results. It's probably got some bugs and/or differences in behavior from
/// the behavior of the official GameStateQuery class in 1.6.
/// </summary>
public static class GameStateQuery {

	#region Storage

	public static readonly Dictionary<string, IGameStateQueryApi.ConditionDelegate> Conditions = new();

	#endregion

	#region Registration

	public static void RegisterCondition(string key, Func<int, IGameState, bool> method) {
		RegisterCondition(key, args => {
			int value = int.Parse(args[0]);
			return state => method(value, state);
		});
	}

	public static void RegisterCondition(string key, Func<int, int, IGameState, bool> method) {
		RegisterCondition(key, args => {
			int item1 = int.Parse(args[0]);
			int item2 = int.Parse(args[1]);
			return state => method(item1, item2, state);
		});
	}

	public static void RegisterCondition(string key, Func<string, IGameState, bool> method) {
		RegisterCondition(key, args => {
			string joined = string.Join(' ', args);
			return state => method(joined, state);
		});
	}

	public static void RegisterCondition(string key, Func<Farmer, string, IGameState, bool> method) {
		RegisterCondition(key, args => {
			string joined = string.Join(' ', args[1..]);
			return state => CheckMatchingPlayers(args[0], state, farmer => method(farmer, joined, state));
		});
	}

	public static void RegisterCondition(string key, Func<Farmer, int, IGameState, bool> method) {
		RegisterCondition(key, args => {
			int value = int.Parse(args[1]);
			return state => CheckMatchingPlayers(args[0], state, farmer => method(farmer, value, state));
		});
	}

	public static void RegisterCondition(string key, IGameStateQueryApi.ConditionDelegate method) {
		lock ((Conditions as ICollection).SyncRoot) {
			if (!Conditions.ContainsKey(key))
				Conditions.Add(key, method);
		}
	}

	#endregion

	#region Helpers

	// Taken directly from 1.6 code
	public static bool TryParseDayOfWeek(string? day, out DayOfWeek result) {
		if (int.TryParse(day, out int num)) {
			result = (DayOfWeek) (num % 7);
			return true;
		}

		switch(day?.ToLower()) {
			case "mon":
			case "monday":
				result = DayOfWeek.Monday;
				return true;
			case "tue":
			case "tuesday":
				result = DayOfWeek.Tuesday;
				return true;
			case "wed":
			case "wednesday":
				result = DayOfWeek.Wednesday;
				return true;
			case "thu":
			case "thursday":
				result = DayOfWeek.Thursday;
				return true;
			case "fri":
			case "friday":
				result = DayOfWeek.Friday;
				return true;
			case "sat":
			case "saturday":
				result = DayOfWeek.Saturday;
				return true;
			case "sun":
			case "sunday":
				result = DayOfWeek.Sunday;
				return true;
			default:
				result = DayOfWeek.Sunday;
				return false;
		}
	}

	// Taken directly from 1.6 code
	public static int GetDeterministicHashCode(string value) {
		int count = value.Length;
		int hash1 = 352654597;
		int hash2 = hash1;
		int i;
		for (i = 0; i < count; i++) {
			int c = value[i];
			hash1 = ((hash1 << 5) + hash1) ^ c;
			if (++i >= count) {
				break;
			}
			c = value[i];
			hash2 = ((hash2 << 5) + hash2) ^ c;
		}
		return hash1 + hash2 * 1566083941;
	}

	public static IGameStateQueryApi.EvaluationDelegate ChoiceImpl(string[] args, Func<IGameState, int, Random> getRandom) {
		if (args.Length < 4)
			throw new ArgumentException("must specify one or more values to match");

		string key = args[0];
		int min = int.Parse(args[1]);
		int max = int.Parse(args[2]);

		if (string.IsNullOrWhiteSpace(key))
			throw new ArgumentException("must specify a seed key");

		if (min < 0 || max < 0)
			throw new ArgumentException("values must be greater than or equal to zero");

		int[] choices = new int[args.Length - 3];
		for (int i = 0; i < choices.Length; i++) {
			choices[i] = int.Parse(args[i + 3]);
		}

		int ikey = GetDeterministicHashCode(key);

		return state => {
			Random rnd = getRandom(state, ikey);
			int value = rnd.Next(min, max + 1);
			foreach (int val in choices)
				if (value == val)
					return true;
			return false;
		};
	}

	public static IGameStateQueryApi.EvaluationDelegate RandomImpl(int skipArgs, string[] args, Func<IGameState, double> getValue) {
		float chance = float.Parse(args[skipArgs]);
		bool addDailyLuck = false;
		for (int i = skipArgs; i < args.Length; i++) {
			if (string.Equals(args[i], "@addDailyLuck", StringComparison.OrdinalIgnoreCase)) {
				addDailyLuck = true;
				break;
			}
		}
		return state => getValue(state) < (addDailyLuck ? chance + (float) Game1.player.DailyLuck : chance);
	}

	public static Random CreateRandom(long seedA, long seedB) {
		return new Random((int) ((long) (int) seedA + (long) (int) seedB));
	}

	#endregion

	#region Initialization

	private static bool Initialized = false;

	private static void Initialize() {
		if (Initialized)
			return;

		Initialized = true;

		RegisterCondition("SEASON", args => {
			if (args.Length > 1)
				return state => {
					foreach (string season in args) {
						if (string.Equals(state.Date.SeasonKey, season, StringComparison.OrdinalIgnoreCase))
							return true;
					}
					return false;
				};

			string season = args[0];
			return state => string.Equals(state.Date.SeasonKey, season, StringComparison.OrdinalIgnoreCase);
		});

		RegisterCondition("YEAR", (value, state) =>
			state.Date.Year >= value
		);

		RegisterCondition("RANDOM", args => RandomImpl(0, args, state =>
			state.Random.NextDouble()
		));

		RegisterCondition("SYNCED_DAY_CHOICE", args => ChoiceImpl(args, (state, seed) =>
			CreateRandom(state.Date.TotalDays + 1, seed)
		));

		RegisterCondition("SYNCED_TICK_CHOICE", args => ChoiceImpl(args, (state, seed) =>
			CreateRandom(state.Ticks, seed)
		));

		RegisterCondition("SYNCED_DAY_RANDOM", args => {
			int key = GetDeterministicHashCode(args[0]);

			return RandomImpl(1, args, state =>
				CreateRandom(state.Date.TotalDays + 1, key).NextDouble()
			);
		});

		RegisterCondition("SYNCED_TICK_RANDOM", args => {
			int key = GetDeterministicHashCode(args[0]);

			return RandomImpl(1, args, state =>
				CreateRandom(state.Ticks, key).NextDouble()
			);
		});

		RegisterCondition("SYNCED_SUMMER_RAIN_RANDOM", args => {
			int key = GetDeterministicHashCode("summer_rain_chance");
			float chance = float.Parse(args[0]);
			float modifier = float.Parse(args[1]);

			return state =>
				CreateRandom(state.Date.TotalDays + 1, key).Next() <=
				chance + state.Date.DayOfMonth * modifier;
		});

		RegisterCondition("TIME", (start, end, state) => {
			if (start >= 0 && state.TimeOfDay < start)
				return false;
			if (end >= 0 && state.TimeOfDay > end)
				return false;
			return true;
		});

		RegisterCondition("DAY_OF_WEEK", args => {
			if (!TryParseDayOfWeek(args[0], out var day))
				throw new ArgumentException($"'{args[0]}' is not a valid day of week.");

			return state => (DayOfWeek)(state.Date.DayOfMonth % 7) == day;
		});

		RegisterCondition("DAY_OF_MONTH", args => {
			if (string.Equals("even", args[0], StringComparison.OrdinalIgnoreCase))
				return state => state.Date.DayOfMonth % 2 == 0;
			else if (string.Equals("odd", args[0], StringComparison.OrdinalIgnoreCase))
				return state => state.Date.DayOfMonth % 2 == 1;

			int day = int.Parse(args[0]);
			return state => state.Date.DayOfMonth == day;
		});

		RegisterCondition("DAYS_PLAYED", (value, state) =>
			state.Date.TotalDays + 1 >= value
		);

		RegisterCondition("PLAYER_STAT", args => {
			string playerKey = args[0];
			string statName = args[1];
			if (string.IsNullOrEmpty(playerKey))
				throw new ArgumentException("must specify player key");
			if (string.IsNullOrEmpty(statName))
				throw new ArgumentException("must specify stat name");

			if (!uint.TryParse(args[2], out uint min))
				throw new ArgumentException("invalid minimum value '" + args[2] + "', must be an integer");

			return state => CheckMatchingPlayers(playerKey, state, farmer => {
				Stats stats = farmer.stats;
				if (stats.Values.TryGetValue(statName, out uint val))
					return val >= min;
				return stats.GetType().GetField(statName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public)?.GetValue(stats) is uint v && v >= min;
			});
		});

		RegisterCondition("FOUND_ALL_LOST_BOOKS", args => state =>
			Game1.netWorldState.Value.LostBooksFound >= 21
		);

		RegisterCondition("PLAYER_COMBAT_LEVEL", (farmer, level, _) =>
			farmer.CombatLevel >= level
		);

		RegisterCondition("PLAYER_FARMING_LEVEL", (farmer, level, _) =>
			farmer.FarmingLevel >= level
		);

		RegisterCondition("PLAYER_FISHING_LEVEL", (farmer, level, _) =>
			farmer.FishingLevel >= level
		);

		RegisterCondition("PLAYER_FORAGING_LEVEL", (farmer, level, _) =>
			farmer.ForagingLevel >= level
		);

		RegisterCondition("PLAYER_MINING_LEVEL", (farmer, level, _) =>
			farmer.MiningLevel >= level
		);

		RegisterCondition("PLAYER_HAS_CRAFTING_RECIPE", args => {
			string who = args[0];
			string recipe = string.Join(' ', args, 1, args.Length - 1);
			return state => CheckMatchingPlayers(who, state, farmer =>
				farmer.craftingRecipes.ContainsKey(recipe)
			);
		});

		RegisterCondition("PLAYER_HAS_COOKING_RECIPE", args => {
			string who = args[0];
			string recipe = string.Join(' ', args, 1, args.Length - 1);
			return state => CheckMatchingPlayers(who, state, farmer =>
				farmer.cookingRecipes.ContainsKey(recipe)
			);
		});

		RegisterCondition("PLAYER_HAS_TOWN_KEY", args => {
			string who = args[0];
			return state => CheckMatchingPlayers(who, state, farmer =>
				farmer.HasTownKey
			);
		});

		RegisterCondition("PLAYER_HEARTS", args => {
			string who = args[0];
			string target = args[1];
			int level = int.Parse(args[2]);
			return state => CheckMatchingPlayers(who, state, farmer => {
				if (string.Equals("Any", target, StringComparison.OrdinalIgnoreCase))
					return farmer.hasAFriendWithHeartLevel(level, false);
				if (string.Equals("AnyDateable", target, StringComparison.OrdinalIgnoreCase))
					return farmer.hasAFriendWithHeartLevel(level, true);

				return farmer.getFriendshipHeartLevelForNPC(target) >= level;
			});
		});

		RegisterCondition("PLAYER_HAS_ALL_ACHIEVEMENTS", args => {
			Dictionary<int, string> achievements = Game1.content.Load<Dictionary<int, string>>(@"Data\Achievements");
			int[] keys = achievements.Keys.ToArray();

			return state => CheckMatchingPlayers(args[0], state, farmer => {
				foreach (int key in keys) {
					if (!farmer.achievements.Contains(key))
						return false;
				}

				return true;
			});
		});

		RegisterCondition("PLAYER_HAS_TRASH_CAN_LEVEL", args => {
			string who = args[0];
			int value = int.Parse(args[1]);
			return state => CheckMatchingPlayers(who, state, farmer =>
				farmer.trashCanLevel == value
			);
		});

		RegisterCondition("ITEM_STACK", (stack, state) =>
			(state.Item?.Stack ?? 0) >= stack
		);

		RegisterCondition("ITEM_QUALITY", (quality, state) =>
			state.Item is SObject sobj && sobj.Quality >= quality
		);

		RegisterCondition("ITEM_ID", args => {
			int id = int.Parse(args[0]);
			return state => state.Item?.ParentSheetIndex == id;
		});

		RegisterCondition("ITEM_HAS_TAG", args => state => {
			if (state.Item is null)
				return false;

			foreach (string tag in args)
				if (!state.Item.HasContextTag(tag))
					return false;

			return true;
		});

		RegisterCondition("PLAYER_HAS_PROFESSION", (farmer, profession, _) =>
			farmer.professions.Contains(profession)
		);

		RegisterCondition("PLAYER_HAS_ACHIEVEMENT", (farmer, id, _) =>
			farmer.achievements.Contains(id)
		);

		RegisterCondition("PLAYER_LOCATION_NAME", (farmer, name, _) =>
			string.Equals(farmer.currentLocation?.Name, name, StringComparison.OrdinalIgnoreCase)
		);

		RegisterCondition("PLAYER_LOCATION_CONTEXT", args => state =>
			CheckMatchingPlayers(args[0], state, farmer => {
				var loc = farmer.currentLocation;
				string name;

				if (loc is null)
					return false;
				else if (loc.InDesertContext())
					name = "Desert";
				else if (loc.InValleyContext())
					name = "Default";
				else if (loc.InIslandContext())
					name = "Island";
				else
					return false;

				return string.Equals(name, args[1], StringComparison.OrdinalIgnoreCase);
			})
		);

		RegisterCondition("PLAYER_LOCATION_UNIQUE_NAME", (farmer, name, _) =>
			string.Equals(farmer.currentLocation?.NameOrUniqueName, name, StringComparison.OrdinalIgnoreCase)
		);

		RegisterCondition("PLAYER_HAS_MET", (farmer, name, _) =>
			farmer.friendshipData.ContainsKey(name)
		);

		RegisterCondition("PLAYER_IS_UNMET", (farmer, name, _) =>
			! farmer.friendshipData.ContainsKey(name)
		);

		RegisterCondition("PLAYER_IS_DATING", (farmer, name, _) => {
			if (string.Equals(name, "Any", StringComparison.OrdinalIgnoreCase)) {
				foreach (var value in farmer.friendshipData.Values)
					if (value.IsDating())
						return true;

			} else if (farmer.friendshipData.TryGetValue(name, out var val))
				return val.IsDating();

			return false;
		});

		RegisterCondition("PLAYER_IS_MARRIED", (farmer, name, _) => {
			if (string.Equals(name, "Any", StringComparison.OrdinalIgnoreCase))
				return farmer.isMarriedOrRoommates();
			if (string.Equals(name, "Player", StringComparison.OrdinalIgnoreCase))
				return farmer.team.GetSpouse(farmer.UniqueMultiplayerID).HasValue;

			return farmer.friendshipData.TryGetValue(name, out var val) && val.IsMarried();
		});

		RegisterCondition("PLAYER_IS_ROOMMATE", (farmer, name, _) => {
			if (string.Equals(name, "Any", StringComparison.OrdinalIgnoreCase))
				return farmer.hasRoommate();
			if (string.Equals(name, "Player", StringComparison.OrdinalIgnoreCase))
				return false;

			return farmer.friendshipData.TryGetValue(name, out var val) && val.IsRoommate();
		});

		RegisterCondition("PLAYER_IS_ENGAGED", (farmer, name, _) => {
			if (string.Equals(name, "Any", StringComparison.OrdinalIgnoreCase))
				return farmer.isEngaged();
			if (string.Equals(name, "Player", StringComparison.OrdinalIgnoreCase))
				return farmer.team.IsEngaged(farmer.UniqueMultiplayerID);

			return farmer.friendshipData.TryGetValue(name, out var val) && val.IsEngaged();
		});

		RegisterCondition("IS_COMMUNITY_CENTER_COMPLETE", _ => _ =>
			Game1.MasterPlayer.hasCompletedCommunityCenter() &&
			!Game1.MasterPlayer.mailReceived.Contains("JojaMember")
		);

		RegisterCondition("IS_ISLAND_NORTH_BRIDGE_FIXED", _ => _ =>
			Game1.getLocationFromName("IslandNorth") is IslandNorth isle && isle.bridgeFixed.Value
		);

		RegisterCondition("IS_JOJA_MART_COMPLETE", _ => _ =>
			Game1.MasterPlayer.hasCompletedCommunityCenter() &&
			Game1.MasterPlayer.mailReceived.Contains("JojaMember")
		);

		RegisterCondition("MUSEUM_DONATIONS", args => {
			if (!int.TryParse(args[0], out int min))
				throw new ArgumentException("must specify minimum number required");

			bool filtered = args.Length > 1;
			if (!filtered)
				return _ => Game1.netWorldState.Value.MuseumPieces.Count() >= min;

			return _ => {
				int count = 0;
				foreach (string item in Game1.netWorldState.Value.MuseumPieces.Values) {
					var obj = new SObject(item, 1);
					if (obj.Type is null)
						continue;
					for (int i = 1; i < args.Length; i++) {
						if (obj.Type == args[i]) {
							count++;
							break;
						}
					}
				}

				return count >= min;
			};
		});

		// Note to self: This is deterministic, can predict.
		RegisterCondition("IS_VISITING_ISLAND", args => {
			string who = args[0];
			return state => Game1.IsVisitingIslandToday(who);
		});

		RegisterCondition("PLAYER_IS_DIVORCED", (farmer, name, _) => {
			if (string.Equals(name, "Any", StringComparison.OrdinalIgnoreCase))
				return farmer.isDivorced();

			return farmer.friendshipData.TryGetValue(name, out var val) && val.IsDivorced();
		});

		RegisterCondition("PLAYER_HAS_CHILDREN", (farmer, min, _) =>
			farmer.getChildrenCount() >= min
		);

		RegisterCondition("PLAYER_FARMHOUSE_UPGRADE", (farmer, level, _) =>
			farmer.HouseUpgradeLevel >= level
		);

		RegisterCondition("PLAYER_HAS_SECRET_NOTE", (farmer, note, _) =>
			farmer.secretNotesSeen.Contains(note)
		);

		RegisterCondition("PLAYER_HAS_PET", args => state =>
			CheckMatchingPlayers(args[0], state, farmer => farmer.hasPet())
		);

		RegisterCondition("IS_PASSIVE_FESTIVAL_OPEN", (name, state) => {
			if (name != "NightMarket")
				return false;

			return state.Date.SeasonKey == "winter" && state.Date.DayOfMonth >= 15 && state.Date.DayOfMonth <= 17 && state.TimeOfDay >= 1700;
		});

		RegisterCondition("IS_PASSIVE_FESTIVAL_TODAY", (name, state) => {
			if (name != "NightMarket")
				return false;

			return state.Date.SeasonKey == "winter" && state.Date.DayOfMonth >= 15 && state.Date.DayOfMonth <= 17;
		});

		RegisterCondition("IS_FESTIVAL_DAY", (offset, state) => {
			WorldDate date;
			if (offset == 0)
				date = state.Date;
			else {
				date = new WorldDate(state.Date);
				date.TotalDays += offset;
			}

			return Utility.isFestivalDay(date.DayOfMonth, date.Season);
		});

		RegisterCondition("PLAYER_PREFERRED_PET", args => {
			string who = args[0];
			bool cat = string.Equals(args[1], "Cat", StringComparison.OrdinalIgnoreCase);
			return state =>
				CheckMatchingPlayers(who, state, farmer => farmer.catPerson == cat);
		});

		RegisterCondition("PLAYER_GENDER", args => {
			string who = args[0];
			bool male = string.Equals(args[1], "Male", StringComparison.OrdinalIgnoreCase);
			return state =>
				CheckMatchingPlayers(who, state, farmer => farmer.IsMale == male);
		});

		RegisterCondition("PLAYER_MONEY_EARNED", (farmer, amount, _) =>
			farmer.totalMoneyEarned >= amount
		);

		RegisterCondition("PLAYER_CURRENT_MONEY", (farmer, amount, _) =>
			farmer.Money >= amount
		);

		RegisterCondition("PLAYER_MOD_DATA", args => {
			string who = args[0];
			string key = args[1];
			string value = args[2];

			return state => CheckMatchingPlayers(who, state, farmer =>
				farmer.modData.TryGetValue(key, out string? data) &&
				string.Equals(data, value, StringComparison.OrdinalIgnoreCase)
			);
		});

		RegisterCondition("PLAYER_HAS_READ_LETTER", (farmer, flag, _) =>
			farmer.mailReceived.Contains(flag)
		);

		RegisterCondition("PLAYER_HAS_FLAG", (farmer, flag, _) =>
			farmer.hasOrWillReceiveMail(flag)
		);

		RegisterCondition("MINE_LOWEST_LEVEL_REACHED", (level, _) =>
			MineShaft.lowestLevelReached >= level
		);

		RegisterCondition("PLAYER_SPECIAL_ORDER_ACTIVE", args => {
			string who = args[0];
			string order = args[1];

			return state => CheckMatchingPlayers(who, state, farmer =>
				farmer.team.SpecialOrderActive(order)
			);
		});

		RegisterCondition("PLAYER_SPECIAL_ORDER_RULE_ACTIVE", args => {
			string who = args[0];
			string rule = args[1];

			return state => CheckMatchingPlayers(who, state, farmer =>
				farmer.team.SpecialOrderRuleActive(rule)
			);
		});

		RegisterCondition("PLAYER_HAS_SEEN_EVENT", (farmer, eventId, _) =>
			farmer.eventsSeen.Contains(eventId)
		);

		RegisterCondition("PLAYER_HAS_ITEM", (farmer, id, _) =>
			farmer.Items.Contains(ItemRegistry.Create(id, 1))
		);

		RegisterCondition("PLAYER_HAS_CAUGHT_FISH", (farmer, fish, _) =>
			farmer.fishCaught.ContainsKey(fish)
		);

		RegisterCondition("PLAYER_HAS_CONVERSATION_TOPIC", args => {
			string who = args[0];
			string topic = args[1];

			return state => CheckMatchingPlayers(who, state, farmer =>
				farmer.activeDialogueEvents.ContainsKey(topic)
			);
		});

		RegisterCondition("PLAYER_HAS_DIALOGUE_ANSWER", (farmer, id, _) =>
			farmer.DialogueQuestionsAnswered.Contains(id)
		);

		RegisterCondition("WEATHER", args => {
			int weather = args[1].ToLower() switch {
				"sun" => 0,
				"rain" => 1,
				"wind" => 2,
				"storm" => 3,
				"festival" => 4,
				"snow" => 5,
				"wedding" => 6,
				_ => -1
			};

			string where = args[0];

			return state => {
				var loc = GetLocation(where, state);
				if (loc == null)
					return false;

				// TODO: Handle Desert

				var ctx = loc.GetLocationContext();
				var wtr = Game1.netWorldState.Value.GetWeatherForLocation(loc.GetLocationContextId());

				bool wind = wtr.isDebrisWeather.Value;
				bool storm = wtr.isLightning.Value;
				bool rain = wtr.isRaining.Value;
				bool snow = wtr.isSnowing.Value;

				switch (weather) {
					case 0: // Sun
						return !wind && !storm && !rain && !snow;
					case 1: // Rain
						return rain && !storm;
					case 2: // Wind
						return wind;
					case 3: // Storm
						return storm;
					case 4: // Festival
						return ctx == Game1.locationContextData["Default"] && Utility.isFestivalDay(state.Date.DayOfMonth, state.Date.Season);
					case 5: // Snow
						return snow;
					case 6: // Wedding
						return ctx == Game1.locationContextData["Default"] && Game1.weddingToday;
				}

				return false;
			};
		});

		RegisterCondition("LOCATION_SEASON", args => state =>
			state.Date.SeasonKey == args[1]
		);

		RegisterCondition("LOCATION_IS_MINES", args => state =>
			GetLocation(args[0], state) is MineShaft // shaft && (shaft.mineLevel < 121 || shaft.mineLevel == 77377)
		);

		RegisterCondition("LOCATION_IS_SKULL_CAVE", args => state =>
			GetLocation(args[0], state) is MineShaft shaft && shaft.mineLevel >= 121 && shaft.mineLevel != 77377
		);

		RegisterCondition("LOCATION_CONTEXT", args => {
			string where = args[0];
			string name = args[1];

			return state => {
				var loc = GetLocation(where, state);
				string ctx;
				if (loc is null)
					return false;
				if (loc is Desert)
					ctx = "Desert";
				else if (loc.GetLocationContext() == Game1.locationContextData["Default"])
					ctx = "Default";
				else if (loc.GetLocationContext() == Game1.locationContextData["Island"])
					ctx = "Island";
				else
					return false;
				return string.Equals(ctx, name, StringComparison.OrdinalIgnoreCase);
			};
		});

		RegisterCondition("LOCATION_ACCESSIBLE", args => _ =>
			Game1.isLocationAccessible(args[0])
		);

		RegisterCondition("FARM_NAME", args => {
			string name = string.Join(' ', args);
			return state =>
				string.Equals(state.Farmer.farmName.Value, name, StringComparison.OrdinalIgnoreCase);
		});

		RegisterCondition("FARM_TYPE", args => {
			string type = args[0];
			return _ => {
				if (string.Equals(Game1.GetFarmTypeID(), type, StringComparison.OrdinalIgnoreCase))
					return true;

				string? name = Game1.whichFarm switch {
					0 => "Default",
					1 => "Riverlands",
					2 => "Forest",
					3 => "Mountains",
					4 => "Combat",
					5 => "Fourcorners",
					6 => "Beach",
					_ => null
				};

				return name is not null && string.Equals(name, type, StringComparison.OrdinalIgnoreCase);
			};
		});

		RegisterCondition("FARM_CAVE", args => {
			int val;
			if (string.Equals(args[0], "Bats", StringComparison.OrdinalIgnoreCase))
				val = 1;
			else if (string.Equals(args[0], "Mushrooms", StringComparison.OrdinalIgnoreCase))
				val = 2;
			else if (string.Equals(args[0], "None", StringComparison.OrdinalIgnoreCase))
				val = 0;
			else
				return _ => false;

			return _ => Game1.MasterPlayer.caveChoice.Value == val;
		});

		RegisterCondition("IS_CUSTOM_FARM_TYPE", _ => _ =>
			Game1.whichFarm == 7
		);

		RegisterCondition("IS_HOST", _ => _ =>
			Game1.IsMasterGame
		);

		RegisterCondition("IS_MULTIPLAYER", _ => _ =>
			Game1.IsMultiplayer
		);

		RegisterCondition("WORLD_STATE_ID", args => {
			string key = args[0];
			return _ => NetWorldState.checkAnywhereForWorldStateID(key);
		});

		RegisterCondition("WORLD_STATE_FIELD", args => {
			string name = args[0];
			string value = args[1];

			PropertyInfo? prop = typeof(NetWorldState).GetProperty(name, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
			if (prop is null)
				return _ => false;

			if (prop.PropertyType == typeof(int)) {
				int val = int.Parse(value);
				return _ =>
					prop.GetValue(Game1.netWorldState.Value) is int i &&
					i == val;
			}

			if (prop.PropertyType == typeof(bool)) {
				bool val = bool.Parse(value);
				return _ =>
					prop.GetValue(Game1.netWorldState.Value) is bool b &&
					b == val;
			}

			return _ => {
				object? val = prop.GetValue(Game1.netWorldState.Value);
				string? str = val is null ? "null" : val is string s ? s : val.ToString();
				return string.Equals(str, value, StringComparison.OrdinalIgnoreCase);
			};
		});

		RegisterCondition("CAN_BUILD_CABIN", _ => _ => {
			if (!Game1.IsMasterGame)
				return false;
			return Game1.getFarm().getNumberBuildingsConstructed("Cabin") < Game1.CurrentPlayerLimit - 1;
		});

		RegisterCondition("CAN_BUILD_FOR_CABINS", args => _ =>
			Game1.getFarm().getNumberBuildingsConstructed(args[0]) < (Game1.getFarm().getNumberBuildingsConstructed("Cabin") + 1)
		);

		RegisterCondition("TRUE", _ => _ => true);
		RegisterCondition("FALSE", _ => _ => false);


	}

	#endregion

	#region Helper Methods

	public static bool CheckMatchingPlayers(string who, IGameState state, IGameStateQueryApi.EvaluateFarmerDelegate func) {
		if (who == "Current")
			return func(Game1.player);
		if (who == "Host")
			return func(Game1.MasterPlayer);
		if (who == "Target")
			return func(state.Farmer);
		if (who == "Any" || who == "All") {
			bool all = who == "All";
			foreach(var fmr in Game1.getAllFarmers()) {
				if (func(fmr)) {
					if (!all) {
						if (state.DoTrace)
							state.Monitor?.Log($"[GameStateQuery]    Player: {fmr.Name}", LogLevel.Trace);
						return true;
					}
				} else if (all) {
					if (state.DoTrace)
						state.Monitor?.Log($"[GameStateQuery]    Player: {fmr.Name}", LogLevel.Trace);
					return false;
				}
			}

			return all;
		}

		if (long.TryParse(who, out long pid)) {
			Farmer farmer = Game1.getFarmerMaybeOffline(pid);
			if (farmer is not null)
				return func(farmer);
		}

		return false;
	}

	public static GameLocation GetLocation(string name, IGameState state) {
		if (name == "Here")
			return Game1.currentLocation;
		if (name == "Target")
			return state?.Location ?? Game1.currentLocation;
		return Game1.getLocationFromName(name);
	}

	#endregion

	#region Parsing

	public static ParsedQuery ParseConditions(string query, bool skip_unknown = false, bool skip_error = false, IMonitor? monitor = null) {
		if (string.IsNullOrEmpty(query))
			return ParsedQuery.EMPTY;

		List<ParsedCondition> results = new();

		if (!Initialized)
			Initialize();

		foreach(string condition in query.Split(',')) {
			string trimmed = condition.Trim();
			if (trimmed.Length == 0)
				continue;

			string[] parts = trimmed.Split(' ');
			string key = parts[0];
			bool inverted = key.StartsWith('!');
			if (inverted)
				key = key[1..];

			if (Conditions.TryGetValue(key, out var cond)) {
				IGameStateQueryApi.EvaluationDelegate method;
				try {
					method = cond.Invoke(parts[1..]);
				} catch (Exception ex) {
					if (skip_error) {
						monitor?.Log($"[GameStateQuery] An error occurred in condition builder for \"{trimmed}\":\n{ex}", LogLevel.Warn);
						results.Add(new ParsedCondition(trimmed, false, _ => false));
						continue;
					} else
						throw;
				}

				results.Add(new ParsedCondition(trimmed, inverted, method));

			} else if (skip_unknown) {
				monitor?.Log($"[GameStateQuery] Unknown Condition: {key}", LogLevel.Warn);
				results.Add(new ParsedCondition(trimmed, false, _ => false));

			} else
				throw new ArgumentException($"[GameStateQuery] Unknown Condition: {key}");
		}

		if (results.Count == 0)
			return ParsedQuery.EMPTY;

		return new ParsedQuery(results.ToArray());
	}

	#endregion

	#region API Compatibility

	public static bool CheckConditions(string query, Random? rnd = null, WorldDate? date = null, int? time = null, int? tick = null, Farmer? who = null, GameLocation? location = null, Item? item = null, IMonitor? monitor = null, bool trace = false) {
		rnd ??= Game1.random;

		return CheckConditions(query, new GameState(
			Random: rnd,
			Date: date ?? Game1.Date,
			TimeOfDay: time ?? Game1.timeOfDay,
			Ticks: tick ?? Game1.ticks,
			Farmer: who ?? Game1.player,
			Location: location,
			Item: item,
			Monitor: monitor,
			DoTrace: trace
		));
	}

	public static bool CheckConditions(string query, IGameState state) {
		return ParseConditions(
			query: query,
			skip_unknown: true,
			skip_error: true,
			monitor: state.Monitor
		).Evaluate(state);
	}

	#endregion

	#region Data Types

	/// <summary>
	/// GSQState represents a snapshot of game state against which conditions
	/// can be checked.
	/// </summary>
	/// <param name="rnd">The <see cref="System.Random"/> instance to use for Random number generation.</param>
	/// <param name="date">A <see cref="WorldDate"/> instance representing the in-game date being checked.</param>
	/// <param name="timeOfDay">The time of day, such that 600 is 6:00am, 1330 is 1:30pm, and 2600 is 2:00am.</param>
	/// <param name="ticks">The number of elapsed game ticks.</param>
	/// <param name="farmer">The player being checked.</param>
	/// <param name="location">The location being checked.</param>
	/// <param name="item">The item being checked.</param>
	/// <param name="monitor">An optional <see cref="IMonitor"/> to use for logging.</param>
	/// <param name="trace">Whether or not trace level logging should be performed.</param>
	public readonly record struct GameState(
		Random Random,
		WorldDate Date,
		int TimeOfDay,
		int Ticks,
		Farmer Farmer,
		GameLocation? Location,
		Item? Item,
		IMonitor? Monitor,
		bool DoTrace
	) : IGameState;

	/// <summary>
	/// A singular parsed condition.
	/// </summary>
	/// <param name="input">The string that was parsed.</param>
	/// <param name="inverted">Whether or not the result should be inverted.</param>
	/// <param name="method">The method to call to check this condition.</param>
	public readonly record struct ParsedCondition(
		string input,
		bool inverted,
		IGameStateQueryApi.EvaluationDelegate method
	);

	/// <summary>
	/// A parsed query, made up of multiple conditions.
	/// </summary>
	public readonly struct ParsedQuery : IParsedQuery {

		/// <summary>
		/// An empty query with no conditions. Always evaluates to true.
		/// </summary>
		public static readonly ParsedQuery EMPTY = new(null);

		/// <summary>
		/// An array of this query's conditions.
		/// </summary>
		public readonly ParsedCondition[]? Conditions;

		public ParsedQuery(ParsedCondition[]? conditions) {
			Conditions = conditions;
		}

		/// <summary>
		/// Evaluate the query with the given <paramref name="state"/>.
		/// </summary>
		/// <param name="state">The game state we're checking against.</param>
		/// <returns>Whether or not the conditions match the provided game state.</returns>
		public bool Evaluate(IGameState state) {
			if (Conditions == null || Conditions.Length == 0)
				return true;

			foreach (var condition in Conditions) {
				try {
					if (state.DoTrace)
						state.Monitor?.Log($"[GameStateQuery] Condition: {condition.input}", LogLevel.Trace);

					bool result = condition.method(state);
					if (condition.inverted)
						result = !result;

					if (state.DoTrace)
						state.Monitor?.Log($"[GameStateQuery]    Result: {result}", LogLevel.Trace);

					if (!result)
						return false;

				} catch (Exception ex) {
					state.Monitor?.Log($"[GameStateQuery] An error occurred in condition handler for \"{condition.input}\":\n{ex}", LogLevel.Error);
					return false;
				}
			}

			return true;
		}

		public bool Evaluate(Random? rnd = null, WorldDate? date = null, int? time = null, int? tick = null, Farmer? who = null, GameLocation? location = null, Item? item = null, IMonitor? monitor = null, bool trace = false) {
			rnd ??= Game1.random;

			return Evaluate(new GameState(
				Random: rnd,
				Date: date ?? Game1.Date,
				TimeOfDay: time ?? Game1.timeOfDay,
				Ticks: tick ?? Game1.ticks,
				Farmer: who ?? Game1.player,
				Location: location,
				Item: item,
				Monitor: monitor,
				DoTrace: trace
			));
		}
	};

	#endregion
}
*/
