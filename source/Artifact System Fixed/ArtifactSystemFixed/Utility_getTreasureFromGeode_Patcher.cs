/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ilyaki/ArtifactSystemFixed
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using static ArtifactSystemFixed.Utils;

namespace ArtifactSystemFixed
{
	class Utility_getTreasureFromGeode_Patcher : Patch
	{
		public static ModConfig Config { private get; set; } = null;

		protected override PatchDescriptor GetPatchDescriptor() => new PatchDescriptor(new StardewValley.Utility().GetType(), "getTreasureFromGeode");
		
		public static bool Prefix() => false;

		public static StardewValley.Object Postfix(StardewValley.Object o, Item geode)
		{
			Random random = new Random((int)Game1.stats.GeodesCracked + (int)Game1.uniqueIDForThisGame / 2);
			int whichGeode = (geode as StardewValley.Object).ParentSheetIndex;

			if (random.NextDouble() < 0.5)
			{
				#region Choose ore/basic object (unchanged from vanilla)
				int amount = random.Next(3) * 2 + 1;
				if (random.NextDouble() < 0.1)
					amount = 10;
				if (random.NextDouble() < 0.01)
					amount = 20;


				if (random.NextDouble() < 0.5)
				{
					switch (random.Next(4))
					{
						case 0: case 1: return new StardewValley.Object(390, amount, false, -1, 0);//Stone

						case 2: return new StardewValley.Object(330, 1, false, -1, 0);//Clay

						case 3:
							{
								int parentSheetIndex;
								switch (whichGeode)
								{
									case 749://Omni geode
										parentSheetIndex = 82 + random.Next(3) * 2;// 82/84/86 : FireQuartz/FrozenTear/EarthCrystal
										break;
									case 536://Frozen Geode
										parentSheetIndex = 84;//Frozen Tear
										break;
									case 535://Basic geode
										parentSheetIndex = 86;//Earth Crystal
										break;
									default://Logically only Magma Geode
										parentSheetIndex = 82; //Fire Quartz
										break;
								}
								return new StardewValley.Object(parentSheetIndex, 1, false, -1, 0);
							}
					}
				}
				else
				{
					#region Choose ore
					switch (whichGeode)
					{
						case 535://Basic geode
							switch (random.Next(3))
							{
								case 0:
									return new StardewValley.Object(378, amount, false, -1, 0);//Copper ore
								case 1:
									return new StardewValley.Object((Game1.player.deepestMineLevel > 25) ? 380 : 378, amount, false, -1, 0);// Iron or Copper ore
								case 2:
									return new StardewValley.Object(382, amount, false, -1, 0);//Coal
							}
							break;
						case 536://Frozen geode
							switch (random.Next(4))
							{
								case 0:
									return new StardewValley.Object(378, amount, false, -1, 0);//Copper ore
								case 1:
									return new StardewValley.Object(380, amount, false, -1, 0);//Iron ore
								case 2:
									return new StardewValley.Object(382, amount, false, -1, 0);//Coal
								case 3:
									return new StardewValley.Object((Game1.player.deepestMineLevel > 75) ? 384 : 380, amount, false, -1, 0);//Gold or Iron ore
							}
							break;
						default://Omni or magma geode
							switch (random.Next(5))
							{
								case 0:
									return new StardewValley.Object(378, amount, false, -1, 0);//Copper ore
								case 1:
									return new StardewValley.Object(380, amount, false, -1, 0);//Iron ore
								case 2:
									return new StardewValley.Object(382, amount, false, -1, 0);//Coal
								case 3:
									return new StardewValley.Object(384, amount, false, -1, 0);//Gold ore
								case 4:
									return new StardewValley.Object(386, amount / 2 + 1, false, -1, 0);//Iridium ore
							}
							break;
					}
					#endregion
				}
				return new StardewValley.Object(Vector2.Zero, 390, 1);//Stone
				#endregion
			}
			else
			{
				//Choose treasure

				if (Config.Geode_AlreadyFoundMultiplier != 0)
				{
					#region Weighted probability
					Console.WriteLine("Choosing geode with weighted probability");

					string[] treasures = Game1.objectInformation[whichGeode].Split('/')[6].Split(' ');//e.g. 539 540 543 547 553 554 562 563 565 570 575 578 122

					var treasuresToWeight = new Dictionary<int, double>();
					foreach (string treasureString in treasures)
					{
						if (int.TryParse(treasureString, out int treasure))
						{
							int timesFound = 0;
							string type = Game1.objectInformation[treasure].Split('/')[3];

							if (type.Contains("Mineral"))
								timesFound = GetNumberOfMineralFound(treasure);
							else if (type.Contains("Arch"))
								timesFound = GetNumberOfArtifactFound(treasure);
							
							double weight = Math.Pow(Config.Geode_AlreadyFoundMultiplier, timesFound);

							treasuresToWeight.Add(treasure, weight);
						}
					}

					int index = ChooseWeightedProbability(treasuresToWeight, random, 390);//It is possible to have nothing to choose if weight multiplier is 0
					Console.WriteLine($"Weighted probability chose {index}");

					if (whichGeode == 749 && random.NextDouble() < 0.008 && (int)Game1.stats.GeodesCracked > 15)
						index = 74;//Prismatic shard

					return new StardewValley.Object(index, 1, false, -1, 0);
					#endregion
				}
				else
				{
					#region Vanilla selection
					string[] treasures = Game1.objectInformation[whichGeode].Split('/')[6].Split(' ');//e.g. 539 540 543 547 553 554 562 563 565 570 575 578 122


					int index = Convert.ToInt32(treasures[random.Next(treasures.Length)]);

					if (whichGeode == 749 && random.NextDouble() < 0.008 && (int)Game1.stats.GeodesCracked > 15)
					{
						index = 74;//Prismatic shard
					}

					return new StardewValley.Object(index, 1, false, -1, 0);
					#endregion
				}
			}
		}
	}
}
