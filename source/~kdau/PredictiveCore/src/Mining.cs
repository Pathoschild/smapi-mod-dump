using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Collections.Generic;

namespace PredictiveCore
{
	public enum MineType
	{
		TheMines,
		SkullCavern,
	}

	public enum MineFloorType
	{
		Mushroom,
		MonsterInfested,
		SlimeInfested,
		Quarry,
		QuarryInfested,
		Treasure,
		PepperRex,
	}

	public struct MiningPrediction
	{
		public MineType mine;
		public int floor;
		public MineFloorType type;
	}

	public static class Mining
	{
		// Whether this module should be available for player use.
		public static bool IsAvailable =>
			MineShaft.lowestLevelReached >= 1;

		// Whether future progress by the player could alter the special floors
		// in the mines. This intentionally disregards the Skull Cavern, since
		// its existence is a spoiler until discovered and doesn't affect the
		// floors of the regular mines.
		public static bool IsProgressDependent
		{
			get
			{
				Utilities.CheckWorldReady ();
				return MineShaft.lowestLevelReached < 120 ||
					!Utility.doesMasterPlayerHaveMailReceivedButNotMailForTomorrow ("ccCraftsRoom");
			}
		}

		// Lists the special floors to be found in mines on the given date.
		public static List<MiningPrediction> ListFloorsForDate (WorldDate date)
		{
			Utilities.CheckWorldReady ();
			if (!IsAvailable)
				throw new InvalidOperationException ("The mines have not been reached.");

			List<MiningPrediction> predictions = new List<MiningPrediction> ();

			// Logic from StardewValley.Locations.MineShaft.chooseLevelType()
			// and StardewValley.Locations.MineShaft.loadLevel()
			// as implemented in Stardew Predictor by MouseyPounds.

			int days = date.TotalDays + 1;
			bool hasQuarry = Utility.doesMasterPlayerHaveMailReceivedButNotMailForTomorrow
				("ccCraftsRoom");

			// Look up to 10 floors beyond the last elevator floor reached.
			int floorHorizon = Math.Min (120,
				(MineShaft.lowestLevelReached / 5) * 5 + 10);

			for (int floor = 1; floor < floorHorizon; ++floor)
			{
				// Check for uncollected treasures.
				if (floor % 10 == 0 && floor != 30 &&
					!Game1.player.chestConsumedMineLevels.ContainsKey (floor))
				{
					predictions.Add (new MiningPrediction
					{
						mine = MineType.TheMines,
						floor = floor,
						type = MineFloorType.Treasure,
					});
					continue;
				}

				// Otherwise, skip elevator floors.
				if (floor % 5 == 0)
					continue;

				// Check for monster or slime infestation.
				Random rng = new Random (days + floor * 100 +
					(int) Game1.uniqueIDForThisGame / 2);
				if (rng.NextDouble () < 0.044 && floor % 40 > 5 &&
					floor % 40 < 30 && floor % 40 != 19)
				{
					predictions.Add (new MiningPrediction
					{
						mine = MineType.TheMines,
						floor = floor,
						type = (rng.NextDouble() < 0.5)
							? MineFloorType.MonsterInfested
							: MineFloorType.SlimeInfested,
					});
					continue;
				}
				
				// Check for quarry-style floor, with or without infestation.
				if (rng.NextDouble() < 0.044 && hasQuarry && floor % 40 > 1)
				{
					predictions.Add (new MiningPrediction
					{
						mine = MineType.TheMines,
						floor = floor,
						type = (rng.NextDouble() < 0.25)
							? MineFloorType.QuarryInfested
							: MineFloorType.Quarry,
					});
					continue;
				}

				// Check for a mushroom floor.
				rng = new Random((days * floor) + (4 * floor) +
					(int) Game1.uniqueIDForThisGame / 2);
				if (rng.NextDouble () < 0.3 && floor > 2)
					rng.NextDouble ();
				rng.NextDouble ();
				if (rng.NextDouble() < 0.035 && floor > 80)
				{
					predictions.Add (new MiningPrediction
					{
						mine = MineType.TheMines,
						floor = floor,
						type = MineFloorType.Mushroom,
					});
				}
			}

			// Check the Skull Cavern, if it has been reached, up to floor 99.
			if (MineShaft.lowestLevelReached > 120)
			{
				for (int floor = 127; floor < 220; ++floor)
				{
					// Check for Pepper Rex floors. Since these are precluded
					// by an earlier unpredictable roll for treasure rooms,
					// these are only potential floors.
					Random rng = new Random (days + floor * 100 +
						(int) Game1.uniqueIDForThisGame / 2);
					if (rng.NextDouble () < 0.044)
					{
						rng.NextDouble ();
						if (rng.NextDouble () < 0.5)
						{
							predictions.Add (new MiningPrediction
							{
								mine = MineType.SkullCavern,
								floor = floor - 120,
								type = MineFloorType.PepperRex,
							});
						}
					}
				}
			}

			return predictions;
		}

		internal static void Initialize (bool addConsoleCommands)
		{
			if (!addConsoleCommands)
			{
				return;
			}
			Utilities.Helper.ConsoleCommands.Add ("predict_mine_floors",
				"Predicts the special floors to be found in the Mines and Skull Cavern on a given date, or today by default.\n\nUsage: predict_mine_floors [<year> <season> <day>]\n- year: the target year (a number starting from 1).\n- season: the target season (one of 'spring', 'summer', 'fall', 'winter').\n- day: the target day (a number from 1 to 28).",
				(_command, args) => ConsoleCommand (new List<string> (args)));
		}

		private static void ConsoleCommand (List<string> args)
		{
			try
			{
				WorldDate date = Utilities.ArgsToWorldDate (args);

				List<MiningPrediction> predictions = ListFloorsForDate (date);
				Utilities.Monitor.Log ($"Special floors in the Mines and Skull Cavern on {date}:",
					LogLevel.Info);
				foreach (MiningPrediction prediction in predictions)
				{
					Utilities.Monitor.Log ($"- {prediction.mine}, floor {prediction.floor}: {prediction.type}",
						LogLevel.Info);
				}
				if (predictions.Count == 0)
				{
					Utilities.Monitor.Log ("  (none)", LogLevel.Info);
				}
			}
			catch (Exception e)
			{
				Utilities.Monitor.Log (e.Message, LogLevel.Error);
			}
		}
	}
}
