/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/emurphy42/PredictiveMods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using SObject = StardewValley.Object;

namespace PredictiveCore
{
	public static class Geodes
	{
		public enum GeodeType
		{
			Regular,
			Frozen,
			Magma,
			Omni,
			Trove,
			Coconut,
		}

		public class Treasure
		{
			public static readonly List<string> UndonatableTreasures = new ()
			{
				"Coal",
				"Clay",
				"Stone",
				"Copper Ore",
				"Iron Ore",
				"Gold Ore",
				"Iridium Ore",
				"Golden Pumpkin",
				"Treasure Chest",
				"Pearl",
				"Banana Sapling",
				"Mango Sapling",
				"Pineapple Seeds",
				"Taro Tuber",
				"Mahogany Seed",
				"Fossilized Skull",
			};

			private static readonly Dictionary<GeodeType, int> GeodeObjects = new ()
			{
				{ GeodeType.Regular, 535 },
				{ GeodeType.Frozen, 536 },
				{ GeodeType.Magma, 537 },
				{ GeodeType.Omni, 749 },
				{ GeodeType.Trove, 275 },
				{ GeodeType.Coconut, 791 },
			};

			internal Treasure (uint geodeNumber, GeodeType geodeType)
			{
				this.geodeNumber = geodeNumber;
				this.geodeType = geodeType;
				geodeObject = new SObject (GeodeObjects[geodeType].ToString(), 1);

				uint originalNumber = Game1.player.stats.GeodesCracked;
				try
				{
					Game1.player.stats.GeodesCracked = geodeNumber;
					item = Utility.getTreasureFromGeode (geodeObject);
				}
				finally
				{
					Game1.player.stats.GeodesCracked = originalNumber;
				}
			}

			public readonly uint geodeNumber;
			public readonly GeodeType geodeType;
			public readonly SObject geodeObject;

			public readonly Item item;
			public int stack => item.Stack;
			public string displayName => item.DisplayName;

			// Currently, the only non-object treasure item is a rare hat.
			public bool valuable => item is not SObject @object ||
				@object.Stack * @object.Price > 75;

			public bool needDonation => !UndonatableTreasures.Contains (item.Name) &&
				!LibraryMuseum.HasDonatedArtifact(item.ParentSheetIndex.ToString());
		}

		public class Prediction
		{
			public readonly uint number;
			public readonly SortedDictionary<GeodeType, Treasure> treasures;

			private static readonly GeodeType[] Types =
				(GeodeType[]) Enum.GetValues (typeof (GeodeType));

			internal Prediction (uint number)
			{
				this.number = number;
				treasures = new SortedDictionary<GeodeType, Treasure> ();
				foreach (GeodeType type in Types)
					treasures.Add (type, new Treasure (number, type));
			}
		}

		// Whether this module should be available for player use.
		public static bool IsAvailable =>
			Game1.player.stats.GeodesCracked > 0 &&
			(Utilities.Config.InaccuratePredictions || Utilities.SupportedVersion);

		// Whether future progress by the player could alter the treasures found
		// when cracking geodes.
		public static bool IsProgressDependent =>
			Game1.player.deepestMineLevel <= 75;

		// Lists the treasures in the next several geodes to be cracked on or
		// after the given geode number, up to the given limit.
		public static List<Prediction> ListTreasures (uint fromNumber, uint limit)
		{
			Utilities.CheckWorldReady ();
			if (!IsAvailable)
				throw new UnavailableException ("geodes");

			List<Prediction> predictions = new ();

			for (uint number = fromNumber; number < fromNumber + limit; ++number)
				predictions.Add (new Prediction (number));

			return predictions;
		}

		internal static void Initialize (bool addConsoleCommands)
		{
			if (!addConsoleCommands)
				return;
			Utilities.Helper.ConsoleCommands.Add ("predict_geodes",
				"Predicts the treasures in the next several geodes to be cracked on or after a given geode, or the next geodes by default.\n\nUsage: predict_geodes [<limit> [<number>]]\n- limit: number of treasures to predict (default 10)\n- number: the number of geodes cracked after the first treasure to predict (a number starting from 1).",
				(_command, args) => ConsoleCommand (args.ToList ()));
		}

		private static void ConsoleCommand (List<string> args)
		{
			try
			{
				Utilities.CheckWorldReady ();

				uint limit = 10;
				if (args.Count > 0)
				{
					if (!uint.TryParse (args[0], out limit) || limit < 1)
						throw new ArgumentException ($"Invalid limit '{args[0]}', must be a number 1 or higher.");
					args.RemoveAt (0);
				}

				uint number = Game1.player.stats.GeodesCracked + 1;
				if (args.Count > 0)
				{
					if (!uint.TryParse (args[0], out number) || number < 1)
						throw new ArgumentException ($"Invalid geode number '{args[0]}', must be a number 1 or higher.");
					args.RemoveAt (0);
				}

				List<Prediction> predictions = ListTreasures (number, limit);
				Utilities.Monitor.Log ($"Next {limit} treasure(s) starting with geode {number}:",
					LogLevel.Info);
				Utilities.Monitor.Log ("  (*: museum donation needed; $: valuable)",
					LogLevel.Info);
				Utilities.Monitor.Log ("Number | Geode Type | Treasure                | Special",
					LogLevel.Info);
				Utilities.Monitor.Log ("-------|------------|-------------------------|--------",
					LogLevel.Info);
				foreach (Prediction prediction in predictions)
				{
					foreach (var kvp in prediction.treasures)
					{
						string entry = string.Format (kvp.Key == prediction.treasures.First ().Key
								? "{0,5:D}. | {1,-10} | {2,2:D} {3,-20} |  {4}   {5}"
								: "       | {1,-10} | {2,2:D} {3,-20} |  {4}   {5}",
							prediction.number,
							kvp.Key,
							kvp.Value.stack, kvp.Value.displayName,
							kvp.Value.needDonation ? "*" : " ",
							kvp.Value.valuable ? "$" : " ");
						Utilities.Monitor.Log (entry, LogLevel.Info);
					}
				}
			}
			catch (Exception e)
			{
				Utilities.Monitor.Log (e.Message, LogLevel.Error);
			}
		}
	}
}
