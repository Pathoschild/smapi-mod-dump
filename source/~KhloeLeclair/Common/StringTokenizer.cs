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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using StardewValley;
using StardewValley.BellsAndWhistles;

using StardewModdingAPI;

namespace Leclair.Stardew.Common;

public static class StringTokenizer {

	public delegate bool HandleTokenDelegate(string input, GameStateQuery.GameState state, out string result);

	#region Storage

	public static readonly Dictionary<string, HandleTokenDelegate> Tokens = new();

	#endregion

	#region Registration

	public static void RegisterToken(string key, HandleTokenDelegate handler) {
		lock((Tokens as ICollection).SyncRoot) {
			if (!Tokens.ContainsKey(key))
				Tokens.Add(key, handler);
		}
	}

	#endregion

	#region Built-in Stuff

	[SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "Applies to children of class, not class itself. Don't be dumb VS")]
	[SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Standardized API with Reflection for Discovery")]
	public static class Builtins {

		public static bool Handle_ArticleFor(string input, GameStateQuery.GameState state, out string result) {
			result = Lexicon.getProperArticleForWord(input);
			return true;
		}

		public static bool Handle_CharacterName(string input, GameStateQuery.GameState state, out string result) {
			NPC who = Game1.getCharacterFromName(input);
			result = who?.displayName ?? string.Empty;
			return who != null;
		}

		public static bool Handle_DataString(string input, GameStateQuery.GameState state, out string result) {
			try {
				string[] args = input.Split(' ');
				if (args.Length < 3) {
					result = string.Empty;
					return false;
				}

				Dictionary<string, string> data = Game1.content.Load<Dictionary<string, string>>(args[0]);
				if (data.TryGetValue(args[1], out string? entry) && entry != null) {
					int index = int.Parse(args[2]);
					string[] bits = entry.Split('/');

					if (index >= 0 && index < bits.Length) {
						result = bits[index];
						return true;
					}
				}

			} catch { }

			result = string.Empty;
			return false;
		}

		public static bool Handle_DayOfMonth(string input, GameStateQuery.GameState state, out string result) {
			result = state.date.DayOfMonth.ToString();
			return true;
		}

		public static bool Handle_EscapedText(string input, GameStateQuery.GameState state, out string result) {
			result = input.Replace(' ', '\u00a0');
			return true;
		}

		public static bool Handle_FarmerUniqueID(string input, GameStateQuery.GameState state, out string result) {
			if (state.farmer == null) {
				result = string.Empty;
				return false;
			}

			result = state.farmer.UniqueMultiplayerID.ToString();
			return true;
		}

		public static bool Handle_FarmName(string input, GameStateQuery.GameState state, out string result) {
			if (state.farmer == null) {
				result = string.Empty;
				return false;
			}

			result = state.farmer.farmName.Value;
			return true;
		}

		public static bool Handle_GenderedText(string input, GameStateQuery.GameState state, out string result) {
			if (state.farmer == null) {
				result = string.Empty;
				return false;
			}

			string[] args = input.Split(' ');
			result = state.farmer.IsMale || args.Length < 2 ? args[0] : args[1];
			return true;
		}

		public static bool Handle_ItemCount(string input, GameStateQuery.GameState state, out string result) {
			if (state.item == null) {
				result = string.Empty;
				return false;
			}

			result = state.item.Stack.ToString();
			return true;
		}

		public static bool Handle_LocalizedText(string input, GameStateQuery.GameState state, out string result) {
			string[] args = input.Split(' ');
			if (args.Length > 1)
				result = Game1.content.LoadString(args[0], args[1..]);
			else
				result = Game1.content.LoadString(args[0]);

			return true;
		}

		public static bool Handle_LocationName(string input, GameStateQuery.GameState state, out string result) {
			GameLocation loc = Game1.getLocationFromName(input);
			if (loc == null) {
				result = string.Empty;
				return false;
			}

			// TODO: A way to look-up display names in 1.5.
			result = loc.Name;
			return true;
		}

		public static bool Handle_PositiveAdjective(string input, GameStateQuery.GameState state, out string result) {
			result = Lexicon.getRandomPositiveAdjectiveForEventOrPerson();
			return true;
		}

		public static bool Handle_Season(string input, GameStateQuery.GameState state, out string result) {
			result = state.date.Season;
			return true;
		}

		public static bool Handle_SpouseFarmerText(string input, GameStateQuery.GameState state, out string result) {
			if (state.farmer != null) {
				string[] args = input.Split(' ');
				if (state.farmer.team.GetSpouse(state.farmer.UniqueMultiplayerID).HasValue) {
					result = args[0];
					return true;
				}

				if (state.farmer.getSpouse() != null) {
					result = args[1];
					return true;
				}
			}

			result = string.Empty;
			return false;
		}

		public static bool Handle_SpouseGenderedText(string input, GameStateQuery.GameState state, out string result) {
			if (state.farmer != null) {
				string[] args = input.Split(' ');

				long? spouse = state.farmer.team.GetSpouse(state.farmer.UniqueMultiplayerID);
				if (spouse.HasValue && Game1.getFarmerMaybeOffline(spouse.Value) is Farmer who) {
					result = who.IsMale ?
						args[0] : args[1];
					return true;
				}

				if (state.farmer.getSpouse() is NPC npc) {
					result = npc.Gender == 0 ?
						args[0] : args[1];
					return true;
				}
			}

			result = string.Empty;
			return false;
		}

		public static bool Handle_SuggestedItem(string input, GameStateQuery.GameState state, out string result) {
			if (state.item == null) {
				result = string.Empty;
				return false;
			}

			result = state.item.DisplayName;
			return true;
		}
	}

	#endregion

	#region Initialization

	private static bool Initialized = false;

	private static void Initialize() {
		if (Initialized)
			return;

		Initialized = true;

		foreach (var method in typeof(Builtins).GetMethods(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)) {
			if (!method.Name.StartsWith("Handle_"))
				continue;

			HandleTokenDelegate @delegate = (HandleTokenDelegate) Delegate.CreateDelegate(typeof(HandleTokenDelegate), method);
			if (@delegate != null)
				RegisterToken(method.Name[7..], @delegate);
		}
	}

	#endregion

	#region Parsing

	public static string ParseString(string input, Random? rnd = null, WorldDate? date = null, int? time = null, int? tick = null, double? picked = null, Farmer? who = null, GameLocation? location = null, Item? item = null, IMonitor? monitor = null, bool trace = false) {
		rnd ??= Game1.random;

		return ParseString(input, new GameStateQuery.GameState(
			rnd: rnd,
			date: date ?? Game1.Date,
			timeOfDay: time ?? Game1.timeOfDay,
			ticks: tick ?? Game1.ticks,
			pickedValue: picked ?? rnd.NextDouble(),
			farmer: who ?? Game1.player,
			location: location,
			item: item,
			monitor: monitor,
			trace: trace
		));
	}

	public static string ParseString(string input, GameStateQuery.GameState state) {

		Initialize();

		int idx = input.IndexOf('[');

		while(idx != -1) {
			idx = HandleToken(idx, ref input, state);
			if (idx != -1)
				idx = idx + 1 >= input.Length ? -1 : input.IndexOf('[', idx + 1);
		}

		return input.Replace('\u00a0', ' ');
	}

	private static int HandleToken(int start, ref string input, GameStateQuery.GameState state) {

		int next = input.IndexOfAny(new[] { '[', ']' }, start + 1);

		while(next != -1) {
			char c = input[next];
			if (c == '[')
				next = HandleToken(next, ref input, state);

			else if (c == ']') {
				string token = input.Substring(start + 1, next - start - 1);
				if (state.trace)
					state.monitor?.Log($"[StringTokenizer] Token: {token}");
				int space = token.IndexOfWhitespace();

				string name;
				string trail;

				if (space == -1) {
					name = token;
					trail = string.Empty;
				} else {
					name = token[..space];
					trail = token[(space + 1)..];
				}

				if (Tokens.TryGetValue(name, out var handler)) {
					if (handler(trail, state, out string result)) {
						if (state.trace)
							state.monitor?.Log($"[StringTokenizer]   -> {result}", LogLevel.Trace);

						input = input.Remove(start, next - start + 1);
						input = input.Insert(start, result);
						return start + result.Length;
					}

				} else {
					state.monitor?.Log($"[StringTokenizer] No Such Token: {name}", LogLevel.Warn);
				}

				return next;
			}

			if (next != -1)
				next = next + 1 >= input.Length ? -1 : input.IndexOfAny(new[] { '[', ']' }, next + 1);
		}

		return -1;
	}

	#endregion

}
