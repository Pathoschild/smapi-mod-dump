/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Chikakoo/stardew-valley-randomizer
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;

namespace Randomizer
{
	/// <summary>
	/// Randomizes the blueprints - that is, the buildings that Robin can build for you
	/// </summary>
	public class BlueprintRandomizer
	{
        /// <summary>
        /// The data from Data/Blueprints.xnb
        /// </summary>
        public static Dictionary<string, string> BuildingData { get; private set; }

		/// <summary>
		/// Randomize the blueprints
		/// </summary>
		/// <returns>The dictionary to use to replace the assets</returns>
		public static Dictionary<string, string> Randomize()
		{
			// Initialize building data here so that it's reloaded in case of a locale change
			BuildingData = Globals.ModRef.Helper.GameContent
				.Load<Dictionary<string, string>>("Data/Blueprints");
			Dictionary<string, string> blueprintChanges = new();

			List<Buildings> buildings = Enum.GetValues(typeof(Buildings)).Cast<Buildings>().ToList();
			Building currentBuilding = null;
			List<Building> buildingsToAdd = new();

			List<int> idsToDisallowForAnimalBuildings = ItemList.GetAnimalProducts().Select(x => x.Id).ToList();
			idsToDisallowForAnimalBuildings.AddRange(new List<int>
			{
				(int)ObjectIndexes.GreenSlimeEgg,
				(int)ObjectIndexes.BlueSlimeEgg,
				(int)ObjectIndexes.RedSlimeEgg,
				(int)ObjectIndexes.PurpleSlimeEgg
			});

			Item resource1, resource2;
			ItemAndMultiplier itemChoice;
			List<ItemAndMultiplier> listChoice;

            foreach (Buildings buildingType in buildings)
			{
				resource1 = ItemList.GetRandomResourceItem();
				resource2 = ItemList.GetRandomResourceItem(new int[] { resource1.Id });

				switch (buildingType)
				{
					case Buildings.Silo:
						currentBuilding = new Building(
							"Silo",
							new List<ItemAndMultiplier>
							{
								new(resource1, Range.GetRandomValue(2, 3)),
								new(resource2),
								new(ItemList.GetRandomItemAtDifficulty(ObtainingDifficulties.MediumTimeRequirements))
							},
							baseMoneyRequired: 100
						);
						break;
					case Buildings.Mill:
						currentBuilding = new Building(
							"Mill",
							new List<ItemAndMultiplier>
							{
								new(resource1, 3),
								new(resource2, 2),
								new(ItemList.GetRandomItemAtDifficulty(ObtainingDifficulties.MediumTimeRequirements))
							},
							baseMoneyRequired: 2500
						);
						break;
					case Buildings.ShippingBin:
						itemChoice = Globals.RNGGetRandomValueFromList(new List<ItemAndMultiplier>
						{
							new(resource1, 3),
							new(ItemList.GetRandomItemAtDifficulty(ObtainingDifficulties.MediumTimeRequirements))
						});

						currentBuilding = new Building(
							"Shipping Bin",
							new List<ItemAndMultiplier>
							{
								itemChoice
							},
                            baseMoneyRequired: 250
						);
						break;
					case Buildings.Coop:
						itemChoice = Globals.RNGGetRandomValueFromList(new List<ItemAndMultiplier>
						{
							new(resource1, 5),
							new(ItemList.GetRandomItemAtDifficulty(ObtainingDifficulties.MediumTimeRequirements, idsToDisallowForAnimalBuildings.ToArray()))
						});

						currentBuilding = new Building(
							"Coop",
							new List<ItemAndMultiplier>
							{
								itemChoice,
								new(resource2, Range.GetRandomValue(2, 3))
							},
                            baseMoneyRequired: 4000
						);
						break;
					case Buildings.BigCoop:
						itemChoice = Globals.RNGGetRandomValueFromList(new List<ItemAndMultiplier>
						{
							new(resource1, 3),
							new(ItemList.GetRandomItemAtDifficulty(ObtainingDifficulties.MediumTimeRequirements, idsToDisallowForAnimalBuildings.ToArray()))
						});

						currentBuilding = new Building(
							"Big Coop",
							new List<ItemAndMultiplier>
							{
								itemChoice,
								new(resource2, 7)
							},
                            baseMoneyRequired: 10000
						);
						break;
					case Buildings.DeluxeCoop:
						itemChoice = Globals.RNGGetRandomValueFromList(new List<ItemAndMultiplier>
						{
							new(resource1, 9),
							new(ItemList.GetRandomItemAtDifficulty(ObtainingDifficulties.LargeTimeRequirements, idsToDisallowForAnimalBuildings.ToArray()))
						});

						currentBuilding = new Building(
							"Deluxe Coop",
							new List<ItemAndMultiplier>
							{
								itemChoice,
								new(resource2, 4)
							},
                            baseMoneyRequired: 20000
                        );
						break;
					case Buildings.Barn:
						itemChoice = Globals.RNGGetRandomValueFromList(new List<ItemAndMultiplier>
						{
							new(resource1, 5),
							new(ItemList.GetRandomItemAtDifficulty(ObtainingDifficulties.MediumTimeRequirements, idsToDisallowForAnimalBuildings.ToArray()))
						});

						currentBuilding = new Building(
							"Barn",
							new List<ItemAndMultiplier>
							{
								itemChoice,
								new(resource2, Range.GetRandomValue(2, 3))
							},
                            baseMoneyRequired: 6000
                        );
						break;
					case Buildings.BigBarn:
						itemChoice = Globals.RNGGetRandomValueFromList(new List<ItemAndMultiplier>
						{
							new(resource1, 3),
							new(ItemList.GetRandomItemAtDifficulty(ObtainingDifficulties.MediumTimeRequirements, idsToDisallowForAnimalBuildings.ToArray()))
						});

						currentBuilding = new Building(
							"Big Barn",
							new List<ItemAndMultiplier>
							{
								itemChoice,
								new(resource2, 7)
							},
                            baseMoneyRequired: 12000
                        );
						break;
					case Buildings.DeluxeBarn:
						itemChoice = Globals.RNGGetRandomValueFromList(new List<ItemAndMultiplier>
						{
							new(resource1, 9),
							new(ItemList.GetRandomItemAtDifficulty(ObtainingDifficulties.LargeTimeRequirements, idsToDisallowForAnimalBuildings.ToArray()))
						});

						currentBuilding = new Building(
							"Deluxe Barn",
							new List<ItemAndMultiplier>
							{
								itemChoice,
								new(resource2, 4)
							},
                            baseMoneyRequired: 25000
                        );
						break;
					case Buildings.SlimeHutch:
						currentBuilding = new Building(
							"Slime Hutch",
							new List<ItemAndMultiplier>
							{
								new(resource1, 9),
								new(ItemList.GetRandomItemAtDifficulty(ObtainingDifficulties.MediumTimeRequirements), 2),
								new(ItemList.GetRandomItemAtDifficulty(ObtainingDifficulties.LargeTimeRequirements))
							},
                            baseMoneyRequired: 10000
                        );
						break;
					case Buildings.Shed:
						listChoice = Globals.RNGGetRandomValueFromList(new List<List<ItemAndMultiplier>> {
							new() { new ItemAndMultiplier(resource1, 5) },
							new() {
								new ItemAndMultiplier(resource1, 3),
								new ItemAndMultiplier(ItemList.GetRandomItemAtDifficulty(ObtainingDifficulties.MediumTimeRequirements))
							}
						});

						currentBuilding = new Building("Shed", listChoice, baseMoneyRequired: 15000);
						break;
					case Buildings.StoneCabin:
						currentBuilding = new Building("Stone Cabin", GetRequiredItemsForCabin(), baseMoneyRequired: 100);
						break;
					case Buildings.PlankCabin:
						currentBuilding = new Building("Plank Cabin", GetRequiredItemsForCabin(), baseMoneyRequired: 100);
						break;
					case Buildings.LogCabin:
						currentBuilding = new Building("Log Cabin", GetRequiredItemsForCabin(), baseMoneyRequired: 100);
						break;
					case Buildings.Well:
						itemChoice = Globals.RNGGetRandomValueFromList(new List<ItemAndMultiplier>
						{
							new(resource1, 3),
							new(ItemList.GetRandomItemAtDifficulty(ObtainingDifficulties.MediumTimeRequirements))
						});

						currentBuilding = new Building("Well", new() { itemChoice }, baseMoneyRequired: 1000);
						break;
					case Buildings.FishPond:
						currentBuilding = new Building(
							"Fish Pond",
							new List<ItemAndMultiplier>
							{
								new(resource1, 2),
								new(ItemList.GetRandomItemAtDifficulty(ObtainingDifficulties.SmallTimeRequirements), 2),
								new(ItemList.GetRandomItemAtDifficulty(ObtainingDifficulties.SmallTimeRequirements), 2)
							},
                            baseMoneyRequired: 5000
						);
						break;
					case Buildings.Stable:
						currentBuilding = new Building(
							"Stable",
							new List<ItemAndMultiplier>
							{
								new(ItemList.GetRandomItemAtDifficulty(ObtainingDifficulties.MediumTimeRequirements), 2),
								new(resource1, 8),
							},
                            baseMoneyRequired: 10000
                        );
						break;
					default:
						Globals.ConsoleError($"Unhandled building: {buildingType}");
						continue;
				}
				buildingsToAdd.Add(currentBuilding);
			}

			foreach (Building building in buildingsToAdd)
			{
				blueprintChanges.Add(building.Name, building.ToString());
			}

			WriteToSpoilerLog(buildingsToAdd);
			return blueprintChanges;
		}

		/// <summary>
		/// Gets the required items for a cabin - applies to any cabin
		/// </summary>
		/// <returns />
		private static List<ItemAndMultiplier> GetRequiredItemsForCabin()
		{
			Item resource = ItemList.GetRandomResourceItem(new int[(int)ObjectIndexes.Hardwood]);
			Item easyItem = Globals.RNGGetRandomValueFromList(
				ItemList.GetItemsBelowDifficulty(ObtainingDifficulties.MediumTimeRequirements, new List<int> { resource.Id })
			);

			return Globals.RNGGetRandomValueFromList(new List<List<ItemAndMultiplier>> {
				new() { new ItemAndMultiplier(resource, 2) },
				new() {
					new ItemAndMultiplier(resource),
					new ItemAndMultiplier(easyItem)
				}
			});
		}

		/// <summary>
		/// Gets the required items string for use in the spoiler log
		/// </summary>
		/// <param name="building">The building</param>
		/// <returns />
		private static string GetRequiredItemsSpoilerString(Building building)
		{
			string requiredItemsSpoilerString = "";
			foreach (RequiredItem item in building.RequiredItems)
			{
				requiredItemsSpoilerString += $" - {item.Item.Name}: {item.NumberOfItems}";
			}
			return requiredItemsSpoilerString;
		}

		/// <summary>
		/// Writes the buildings to the spoiler log
		/// </summary>
		/// <param name="buildingsToAdd">Info about the changes buildings</param>
		private static void WriteToSpoilerLog(List<Building> buildingsToAdd)
		{
			if (!Globals.Config.RandomizeBuildingCosts) { return; }

			Globals.SpoilerWrite("==== BUILDINGS ====");
			foreach (Building building in buildingsToAdd)
			{
				Globals.SpoilerWrite($"{building.Name} - {building.Price}G");
				Globals.SpoilerWrite(GetRequiredItemsSpoilerString(building));
				Globals.SpoilerWrite("===");
			}
			Globals.SpoilerWrite("");
		}
	}
}
