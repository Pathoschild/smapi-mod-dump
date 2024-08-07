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
using StardewValley.Extensions;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using SObject = StardewValley.Object;

namespace PredictiveCore
{
	public static class Mining
	{
		public enum Location
		{
			TheMines,
			SkullCavern,
		}

		public enum FloorType
		{
			Mushroom,
			MonsterInfested,
			SlimeInfested,
			Quarry,
			QuarryInfested,
			Treasure,
			PepperRex,
		}

		public struct Prediction
		{
			public Location location;
			public int floor;
			public FloorType type;
			public Item item;
		}

		// Whether this module should be available for player use.
		public static bool IsAvailable =>
			MineShaft.lowestLevelReached >= 1 &&
			(Utilities.Config.InaccuratePredictions || Utilities.SupportedVersion);

		// Whether future progress by the player could alter the special floors
		// in the mines. This intentionally disregards the Skull Cavern, since
		// its existence is a spoiler until discovered and doesn't affect the
		// floors of the regular mines.
		public static bool IsProgressDependent =>
			MineShaft.lowestLevelReached < 120 ||
			!Utility.doesMasterPlayerHaveMailReceivedButNotMailForTomorrow ("ccCraftsRoom");

		// Lists the special floors to be found in mines on the given date.
		public static List<Prediction> ListFloorsForDate (SDate date)
		{
			Utilities.CheckWorldReady ();
			if (!IsAvailable)
				throw new UnavailableException ("mining");

			List<Prediction> predictions = new ();

			// Logic from StardewValley.Locations.MineShaft.chooseLevelType
			// and StardewValley.Locations.MineShaft.loadLevel
			// as implemented in Stardew Predictor by MouseyPounds.

			bool hasQuarry = Utility.doesMasterPlayerHaveMailReceivedButNotMailForTomorrow("ccCraftsRoom")
				&& Game1.MasterPlayer.hasOrWillReceiveMail("VisitedQuarryMine");

			// Look up to 10 floors beyond the last elevator floor reached.
			int floorHorizon = Math.Min (120,
				(MineShaft.lowestLevelReached / 5) * 5 + 10);

			var avoidMonsters = false;
			var difficultMines = (Game1.netWorldState.Value.MinesDifficulty > 0);
            if (!difficultMines)
			{
                foreach (Farmer onlineFarmer in Game1.getOnlineFarmers())
                {
                    if (onlineFarmer.hasBuff(Buff.avoidMonsters))
                    {
						avoidMonsters = true;
                    }
                }
            }

            for (int floor = 1; floor < floorHorizon; ++floor)
			{
				// Check for uncollected treasures.
				if (floor % 10 == 0 && floor != 30 &&
					!Game1.player.chestConsumedMineLevels.ContainsKey (floor))
				{
					predictions.Add (new Prediction
					{
						location = Location.TheMines,
						floor = floor,
						type = FloorType.Treasure,
						item = GetTreasureForFloor (floor),
					});
					continue;
				}

				// Otherwise, skip elevator floors.
				if (floor % 5 == 0)
					continue;

				int mapNumberToLoad = floor % 40; // exceptions are all elevator floors
                
				FloorType? typeToPredict = null;

				// Perform same non-invasive checks as loadLevel(), even if they don't lead to a prediction
				// This is apparently off a bit on quarry levels for some reason
                Random rng = Utility.CreateDaySaveRandom(floor * 100);
                if (
					!avoidMonsters
					    && rng.NextDouble() < 0.044
						&& mapNumberToLoad > 5
					    && mapNumberToLoad < 30
						&& mapNumberToLoad != 19
				)
				{
                    if (rng.NextBool())
					{
						typeToPredict = FloorType.MonsterInfested;
					}
					else
					{
						typeToPredict = FloorType.SlimeInfested;
					}
                }
				else if (
                    rng.NextDouble() < 0.044
                        && hasQuarry
						&& mapNumberToLoad > 1
				)
				{
                    if (rng.NextDouble() < 0.25)
					{
						typeToPredict = FloorType.QuarryInfested;
					}
					else
					{
                        typeToPredict = FloorType.Quarry;
                    }
                }

                // Perform same non-invasive checks as chooseLevelType(), even if they don't lead to a prediction
                rng = Utility.CreateDaySaveRandom(Game1.stats.DaysPlayed, floor, 4 * floor);
				if (difficultMines)
				{
				}
				else if (rng.NextDouble() < 0.3 && floor > 2)
                {
                    if (rng.NextDouble() < 0.3)
                    {
                    }
                }
                if (rng.NextDouble() < 0.15 && floor > 5)
                {
                }
                if (rng.NextDouble() < 0.035 && floor > 80)
                {
					typeToPredict = FloorType.Mushroom;
                }

				if (typeToPredict != null)
				{
                    predictions.Add(new Prediction
                    {
                        location = Location.TheMines,
                        floor = floor,
                        type = (FloorType)typeToPredict
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

                    // Again, perform same non-invasive checks as loadLevel(), even if they don't lead to a prediction
                    // Skip checking for Skull Cavern treasure rooms (uses MineShaft.mineRandom)

                    int mapNumberToLoad = floor % 40;

                    Random rng = Utility.CreateDaySaveRandom(floor * 100);
                    if (
                        rng.NextDouble() < 0.044
							&& floor % 5 != 0
							&& mapNumberToLoad > 5
							&& mapNumberToLoad < 30
							&& mapNumberToLoad != 19
                    )
                    {
                        if (rng.NextBool())
                        {
                        }
                        if (rng.NextBool())
                        {
                            predictions.Add(new Prediction
                            {
                                location = Location.SkullCavern,
                                floor = floor - 120,
                                type = FloorType.PepperRex,
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
				(_command, args) => ConsoleCommand (args.ToList ()));
		}

		private static void ConsoleCommand (List<string> args)
		{
			try
			{
				SDate date = Utilities.ArgsToSDate (args);

				List<Prediction> predictions = ListFloorsForDate (date);
				Utilities.Monitor.Log ($"Special floors in the Mines and Skull Cavern on {date}:",
					LogLevel.Info);
				foreach (Prediction prediction in predictions)
				{
					var item = (prediction.item != null)
						? $": {prediction.item.DisplayName}"
						: "";
					Utilities.Monitor.Log ($"- {prediction.location}, floor {prediction.floor}: {prediction.type}{item}",
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

		private static Item GetTreasureForFloor (int floor)
		{
			List<Item> candidates = floor switch
			{
				10 => new List<Item> ()
				{
					new Boots ("506"),
					new Boots ("507"),
                    new MeleeWeapon ("12"),
                    new MeleeWeapon ("17"),
                    new MeleeWeapon ("22"),
                    new MeleeWeapon ("31"),
                },
				20 => new List<Item> ()
				{
					new MeleeWeapon ("11"),
                    new MeleeWeapon ("24"),
                    new MeleeWeapon ("20"),
                    new Ring ("517"),
                    new Ring ("519"),
                },
				40 => new List<Item> () { new Slingshot () },
				50 => new List<Item> ()
				{
					new Boots ("509"),
                    new Boots ("510"),
                    new Boots ("508"),
                    new MeleeWeapon ("1"),
                    new MeleeWeapon ("43"),
                },
				60 => new List<Item> ()
				{
					new MeleeWeapon ("21"),
                    new MeleeWeapon ("44"),
                    new MeleeWeapon ("6"),
                    new MeleeWeapon ("18"),
                    new MeleeWeapon ("27"),
                },
				70 => new List<Item> () { new Slingshot ("33") },
                80 => new List<Item> ()
				{
					new Boots ("512"),
                    new Boots ("511"),
                    new MeleeWeapon ("10"),
                    new MeleeWeapon ("7"),
                    new MeleeWeapon ("46"),
                    new MeleeWeapon ("19"),
                },
				90 => new List<Item> ()
				{
					new MeleeWeapon ("8"),
                    new MeleeWeapon ("52"),
                    new MeleeWeapon ("45"),
                    new MeleeWeapon ("5"),
                    new MeleeWeapon ("60"),
                },
				100 => new List<Item> () { new SObject ("434", 1) },
                110 => new List<Item> ()
				{
					new Boots ("514"),
                    new Boots ("878"),
                    new MeleeWeapon ("50"),
                    new MeleeWeapon ("28"),
                },
				120 => Game1.player.hasSkullKey
					? new List<Item> () { new SpecialItem (4) }
					: new List<Item> (),
				_ => new List<Item> (),
			};

			if (Game1.netWorldState.Value.ShuffleMineChests == Game1.MineChestType.Remixed)
			{
				Random rng = Utility.CreateRandom((double)Game1.uniqueIDForThisGame * 512.0, floor);
				return rng.ChooseFrom(candidates);
			}
			return candidates.FirstOrDefault ();
		}
	}
}
