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
		/// Randomize the blueprints
		/// </summary>
		/// <returns>The dictionary to use to replace the assets</returns>
		public static Dictionary<string, string> Randomize()
		{
			Dictionary<string, string> blueprintChanges = new Dictionary<string, string>();

			List<Buildings> buildings = Enum.GetValues(typeof(Buildings)).Cast<Buildings>().ToList();
			Building currentBuilding = null;
			List<Building> buildingsToAdd = new List<Building>();

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
								new ItemAndMultiplier(resource1, Range.GetRandomValue(2, 3)),
								new ItemAndMultiplier(resource2),
								new ItemAndMultiplier(ItemList.GetRandomItemAtDifficulty(ObtainingDifficulties.MediumTimeRequirements))
							},
							100,
							$"3/3/-1/-1/-2/-1/null/{Globals.GetTranslation("Silo-name-and-description")}/Buildings/none/48/128/-1/null/Farm"
						);
						break;
					case Buildings.Mill:
						currentBuilding = new Building(
							"Mill",
							new List<ItemAndMultiplier>
							{
								new ItemAndMultiplier(resource1, 3),
								new ItemAndMultiplier(resource2, 2),
								new ItemAndMultiplier(ItemList.GetRandomItemAtDifficulty(ObtainingDifficulties.MediumTimeRequirements))
							},
							2500,
							$"4/2/-1/-1/-2/-1/null/{Globals.GetTranslation("Mill-name-and-description")}/Buildings/none/64/128/-1/null/Farm"
						);
						break;
					case Buildings.ShippingBin:
						itemChoice = Globals.RNGGetRandomValueFromList(new List<ItemAndMultiplier>
						{
							new ItemAndMultiplier(resource1, 3),
							new ItemAndMultiplier(ItemList.GetRandomItemAtDifficulty(ObtainingDifficulties.MediumTimeRequirements))
						});

						currentBuilding = new Building(
							"Shipping Bin",
							new List<ItemAndMultiplier>
							{
								itemChoice
							},
							250,
							$"2/1/-1/-1/-1/-1/null/{Globals.GetTranslation("Shipping-Bin-name-and-description")}/Buildings/none/48/80/-1/null/Farm",
							"false/0"
						);
						break;
					case Buildings.Coop:
						itemChoice = Globals.RNGGetRandomValueFromList(new List<ItemAndMultiplier>
						{
							new ItemAndMultiplier(resource1, 5),
							new ItemAndMultiplier(ItemList.GetRandomItemAtDifficulty(ObtainingDifficulties.MediumTimeRequirements, idsToDisallowForAnimalBuildings.ToArray()))
						});

						currentBuilding = new Building(
							"Coop",
							new List<ItemAndMultiplier>
							{
								itemChoice,
								new ItemAndMultiplier(resource2, Range.GetRandomValue(2, 3))
							},
							4000,
							$"6/3/1/2/2/2/Coop/{Globals.GetTranslation("Coop-name-and-description")}/Buildings/none/64/96/4/null/Farm"
						);
						break;
					case Buildings.BigCoop:
						itemChoice = Globals.RNGGetRandomValueFromList(new List<ItemAndMultiplier>
						{
							new ItemAndMultiplier(resource1, 3),
							new ItemAndMultiplier(ItemList.GetRandomItemAtDifficulty(ObtainingDifficulties.MediumTimeRequirements, idsToDisallowForAnimalBuildings.ToArray()))
						});

						currentBuilding = new Building(
							"Big Coop",
							new List<ItemAndMultiplier>
							{
								itemChoice,
								new ItemAndMultiplier(resource2, 7)
							},
							10000,
							$"6/3/1/2/2/2/Coop2/{Globals.GetTranslation("Big-Coop-name-and-description")}/Upgrades/Coop/64/96/8/null/Farm"
						);
						break;
					case Buildings.DeluxeCoop:
						itemChoice = Globals.RNGGetRandomValueFromList(new List<ItemAndMultiplier>
						{
							new ItemAndMultiplier(resource1, 9),
							new ItemAndMultiplier(ItemList.GetRandomItemAtDifficulty(ObtainingDifficulties.LargeTimeRequirements, idsToDisallowForAnimalBuildings.ToArray()))
						});

						currentBuilding = new Building(
							"Deluxe Coop",
							new List<ItemAndMultiplier>
							{
								itemChoice,
								new ItemAndMultiplier(resource2, 4)
							},
							20000,
							$"6/3/1/2/2/2/Coop3/{Globals.GetTranslation("Deluxe-Coop-name-and-description")}/Upgrades/Big Coop/64/96/12/null/Farm"
						);
						break;
					case Buildings.Barn:
						itemChoice = Globals.RNGGetRandomValueFromList(new List<ItemAndMultiplier>
						{
							new ItemAndMultiplier(resource1, 5),
							new ItemAndMultiplier(ItemList.GetRandomItemAtDifficulty(ObtainingDifficulties.MediumTimeRequirements, idsToDisallowForAnimalBuildings.ToArray()))
						});

						currentBuilding = new Building(
							"Barn",
							new List<ItemAndMultiplier>
							{
								itemChoice,
								new ItemAndMultiplier(resource2, Range.GetRandomValue(2, 3))
							},
							6000,
							$"7/4/1/3/3/3/Barn/{Globals.GetTranslation("Barn-name-and-description")}/Buildings/none/96/96/4/null/Farm"
						);
						break;
					case Buildings.BigBarn:
						itemChoice = Globals.RNGGetRandomValueFromList(new List<ItemAndMultiplier>
						{
							new ItemAndMultiplier(resource1, 3),
							new ItemAndMultiplier(ItemList.GetRandomItemAtDifficulty(ObtainingDifficulties.MediumTimeRequirements, idsToDisallowForAnimalBuildings.ToArray()))
						});

						currentBuilding = new Building(
							"Big Barn",
							new List<ItemAndMultiplier>
							{
								itemChoice,
								new ItemAndMultiplier(resource2, 7)
							},
							12000,
							$"7/4/1/3/4/3/Barn2/{Globals.GetTranslation("Big-Barn-name-and-description")}/Upgrades/Barn/96/96/8/null/Farm"
						);
						break;
					case Buildings.DeluxeBarn:
						itemChoice = Globals.RNGGetRandomValueFromList(new List<ItemAndMultiplier>
						{
							new ItemAndMultiplier(resource1, 9),
							new ItemAndMultiplier(ItemList.GetRandomItemAtDifficulty(ObtainingDifficulties.LargeTimeRequirements, idsToDisallowForAnimalBuildings.ToArray()))
						});

						currentBuilding = new Building(
							"Deluxe Barn",
							new List<ItemAndMultiplier>
							{
								itemChoice,
								new ItemAndMultiplier(resource2, 4)
							},
							25000,
							$"7/4/1/3/4/3/Barn3/{Globals.GetTranslation("Deluxe-Barn-name-and-description")}/Upgrades/Big Barn/96/96/12/null/Farm"
						);
						break;
					case Buildings.SlimeHutch:
						currentBuilding = new Building(
							"Slime Hutch",
							new List<ItemAndMultiplier>
							{
								new ItemAndMultiplier(resource1, 9),
								new ItemAndMultiplier(ItemList.GetRandomItemAtDifficulty(ObtainingDifficulties.MediumTimeRequirements), 2),
								new ItemAndMultiplier(ItemList.GetRandomItemAtDifficulty(ObtainingDifficulties.LargeTimeRequirements))
							},
							10000,
							$"11/6/5/5/-1/-1/SlimeHutch/{Globals.GetTranslation("Slime-Hutch-name-and-description")}/Buildings/none/96/96/20/null/Farm"
						);
						break;
					case Buildings.Shed:
						listChoice = Globals.RNGGetRandomValueFromList(new List<List<ItemAndMultiplier>> {
							new List<ItemAndMultiplier> { new ItemAndMultiplier(resource1, 5) },
							new List<ItemAndMultiplier>
							{
								new ItemAndMultiplier(resource1, 3),
								new ItemAndMultiplier(ItemList.GetRandomItemAtDifficulty(ObtainingDifficulties.MediumTimeRequirements))
							}
						});

						currentBuilding = new Building(
							"Shed",
							listChoice,
							15000,
							$"7/3/3/2/-1/-1/Shed/{Globals.GetTranslation("Shed-name-and-description")}/Buildings/none/96/96/20/null/Farm"
						);
						break;
					case Buildings.StoneCabin:
						currentBuilding = new Building(
							"Stone Cabin",
							GetRequiredItemsForCabin(),
							100,
							$"5/3/2/2/-1/-1/Cabin/{Globals.GetTranslation("Stone-Cabin-name-and-description")}/Buildings/none/96/96/20/null/Farm",
							"false/0"
						);
						break;
					case Buildings.PlankCabin:
						currentBuilding = new Building(
							"Plank Cabin",
							GetRequiredItemsForCabin(),
							100,
							$"5/3/2/2/-1/-1/Cabin/{Globals.GetTranslation("Plank-Cabin-name-and-description")}/Buildings/none/96/96/20/null/Farm",
							"false/0"
						);
						break;
					case Buildings.LogCabin:
						currentBuilding = new Building(
							"Log Cabin",
							GetRequiredItemsForCabin(),
							100,
							$"5/3/2/2/-1/-1/Cabin/{Globals.GetTranslation("Log-Cabin-name-and-description")}/Buildings/none/96/96/20/null/Farm",
							"false/0"
						);
						break;
					case Buildings.Well:
						itemChoice = Globals.RNGGetRandomValueFromList(new List<ItemAndMultiplier>
						{
							new ItemAndMultiplier(resource1, 3),
							new ItemAndMultiplier(ItemList.GetRandomItemAtDifficulty(ObtainingDifficulties.MediumTimeRequirements))
						});

						currentBuilding = new Building(
							"Well",
							new List<ItemAndMultiplier>
							{
								itemChoice
							},
							1000,
							$"3/3/-1/-1/-1/-1/null/{Globals.GetTranslation("Well-name-and-description")}/Buildings/none/32/32/-1/null/Farm"
						);
						break;
					case Buildings.FishPond:
						currentBuilding = new Building(
							"Fish Pond",
							new List<ItemAndMultiplier>
							{
								new ItemAndMultiplier(resource1, 2),
								new ItemAndMultiplier(ItemList.GetRandomItemAtDifficulty(ObtainingDifficulties.SmallTimeRequirements), 2),
								new ItemAndMultiplier(ItemList.GetRandomItemAtDifficulty(ObtainingDifficulties.SmallTimeRequirements), 2)
							},
							5000,
							$"5/5/-1/-1/-2/-1/null/{Globals.GetTranslation("Fish-Pond-name-and-description")}/Buildings/none/76/78/10/null/Farm",
							"false/2"
						);
						break;
					default:
						Globals.ConsoleError($"Unhandled building: {buildingType.ToString()}");
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
			Item resource = ItemList.GetRandomResourceItem();
			Item easyItem = Globals.RNGGetRandomValueFromList(
				ItemList.GetItemsBelowDifficulty(ObtainingDifficulties.MediumTimeRequirements, new List<int> { resource.Id })
			);

			return Globals.RNGGetRandomValueFromList(new List<List<ItemAndMultiplier>> {
				new List<ItemAndMultiplier> { new ItemAndMultiplier(resource, 2) },
				new List<ItemAndMultiplier>
				{
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
