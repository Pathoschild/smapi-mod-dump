using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BattleRoyale
{
    class Chests
	{
		private static List<TileLocation> chestLocations;

		private const float minFractionToSpawn = 0.4f;
		private const float maxFractionToSpawn = 0.9f;
		private const int numberOfPlayersForMaxFraction = 15;

		public static void LoadChestsSpawns(IModHelper helper)
		{
			var data = helper.Data.ReadJsonFile<ChestsSpawnsDataModel>("chests.json");
			if (data == null)
			{
				data = new ChestsSpawnsDataModel();
				helper.Data.WriteJsonFile("chests.json", data);
			}
			chestLocations = data.ChestsSpawns;

			foreach (var chest in chestLocations)
			{
				Console.WriteLine($"Loaded chest spawn ({chest.locationName}, {chest.tileX}, {chest.tileY})");
			}
		}
        
        private readonly List<int>[] weaponsLoot = new List<int>[3] {
            new List<int>() { 5, 14, 45, 33 },//A grade : 20% 
			new List<int>() { 2, 7, 10, 51, 49 },//B grade: 50%
			new List<int>() { 3, 26}//C grade: 30%
		};

        private readonly int[][] foodLoot = new int[][]
		{
			new int[]{ 222, 221, 607, 608 },//A grade : found in C grade chests
			new int[]{ 202, 216, 456, 198, 224, 227 }//B grade : found in B grade chests
		};
		/* private readonly List<int>[] foodLoot = new List<int>[2]
		{
			new List<int>() { 222, 221, 607, 608 },//A grade : found in C grade chests
			new List<int>() { 202, 216, 456, 198, 224, 227 }//B grade : found in B grade chests
		};*/
		
		public void SpawnAndFillChests()
		{
			//(Chest chest, GameLocation location, Vector2 tile)
			var generatedChests = new List<Tuple<Chest, GameLocation, Vector2>>();

			var gameLocationNames = new Dictionary<string, GameLocation>();
			foreach (GameLocation gameLocation in Game1.locations)
				gameLocationNames.Add(gameLocation.Name, gameLocation);

			int seed = Game1.random.Next(int.MaxValue / 2);

			Shuffle(chestLocations);

			float fractionToSpawn = minFractionToSpawn + (maxFractionToSpawn - minFractionToSpawn) * Math.Min(1, Game1.getOnlineFarmers().Count / numberOfPlayersForMaxFraction);

			foreach (TileLocation tileLocation in chestLocations.Skip((int)(chestLocations.Count * (1 - fractionToSpawn))))
			{
				Random random = new Random(seed);
				seed++;

				if (gameLocationNames.TryGetValue(tileLocation.locationName, out GameLocation gameLocation))
				{
					var tileVector = tileLocation.CreateVector2();

					var items = new List<Item>();

					//Chest grade
					int grade = 2;//0,1,2 : A,B,C
					double random1 = random.NextDouble();
					if (random1 < 0.2)
						grade = 0;
					else if (random1 < 0.2 + 0.5)
						grade = 1;

					//Weapons
					int itemID = weaponsLoot[grade][random.Next(weaponsLoot[grade].Count)];
					items.Add(itemID == 32 || itemID == 33 ? (Item)new StardewValley.Tools.Slingshot(itemID) : new StardewValley.Tools.MeleeWeapon(itemID));

					//Add slingshot ammo if a slingshot was added
					if (items.Any ( x => x is StardewValley.Tools.Slingshot))
					{
						//stones
						items.Add(new StardewValley.Object(390, 8 + Game1.random.Next(4+1)));
						
						items.Add(new StardewValley.Object(441, 2));//explosive ammo
					}

					//Food
					//C grade chests should have A grade food, and B chest have B food (A grade has none)
					if (grade == 1)//B
					{
						items.Add(new StardewValley.Object(foodLoot[1][random.Next(foodLoot[1].Length)], 1 + random.Next(3)));
					}
					else if (grade == 2)//C
					{
						items.Add(new StardewValley.Object(foodLoot[0][random.Next(foodLoot[0].Length)], 1));
					}

					//Hats: 0 to 39
					if (random.NextDouble() < 0.01)
					{
						int x = random.Next(0, 39 + 1 - 1);//39 hats in total. Don't allow chicken hat(10)
						items.Add(new Hat(x >= 10 ? x + 1 : x));
					}

					//Cherry bomb
					if (random.NextDouble() < 0.15)
					{
						items.Add(new StardewValley.Object(286, 2+random.Next(2)));
					}

					//Spawn the chest
					var chest = new Chest(true);
					chest.items.AddRange(items);

					Color chestColor;
					switch(grade)
					{
						case 0: chestColor = Color.Yellow; break;
						case 1: chestColor = Color.Blue; break;
						case 2:
						default: chestColor = Color.White; break;
					}
					chest.playerChoiceColor.Set(chestColor);

					gameLocation.setObject(tileVector, chest);
					
					generatedChests.Add(Tuple.Create(chest, gameLocation, tileVector));
				}
			}

			var horses = new Horses(generatedChests);
			ModEntry.Events.GameLoop.UpdateTicked += horses.Update;
		}
		
		private static void Shuffle<T>(IList<T> list)
		{
			int n = list.Count;
			while (n > 1)
			{
				n--;
				int k = Game1.random.Next(n + 1);
				T value = list[k];
				list[k] = list[n];
				list[n] = value;
			}
		}
	}

	class ChestsSpawnsDataModel
	{
		public List<TileLocation> ChestsSpawns { get; set; } = new List<TileLocation>()
		{
			new TileLocation("BusStop", 26,13),
			new TileLocation("Town", 18,52),
			new TileLocation("Town", 29,67),
			new TileLocation("Saloon", 33,6),
			new TileLocation("SamHouse", 11,23),
			new TileLocation("HaleyHouse", 5,4),
			new TileLocation("HaleyHouse", 13,4),
			new TileLocation("Town", 59,99),
			new TileLocation("ManorHouse", 19,4),
			new TileLocation("Town", 72,90),
			new TileLocation("ArchaeologyHouse", 42,4),
			new TileLocation("Town", 102,69),
			new TileLocation("JojaMart", 10,27),
			new TileLocation("JojaMart", 6,27),
			new TileLocation("Town", 98,4),
			new TileLocation("Town", 39,13),
			new TileLocation("Town", 69,11),
			new TileLocation("SebastianRoom", 9,11),
			new TileLocation("ScienceHouse", 9,7),
			new TileLocation("Mountain", 31,5),
			new TileLocation("BathHouse_MensLocker", 6,24),
			new TileLocation("BathHouse_WomensLocker", 11,22),
			new TileLocation("Railroad", 32,40),
			new TileLocation("Mine", 22,6),
			new TileLocation("Backwoods", 17,11),
			new TileLocation("FarmCave", 5,9),
			new TileLocation("Beach", 10,39),
			new TileLocation("LeahHouse", 13,5),
			new TileLocation("Forest", 71,48),
			new TileLocation("Forest", 69,84),
			new TileLocation("Forest", 36,82),
			new TileLocation("Forest", 16,76),
			new TileLocation("WizardHouse", 2,7),
			new TileLocation("Forest", 3,4),
			new TileLocation("Woods", 12,7),
			new TileLocation("Farm", 71, 15),
			new TileLocation("Backwoods", 45, 16),
			new TileLocation("Mountain", 60, 7),
			new TileLocation("Mountain", 89, 33),
			new TileLocation("ScienceHouse", 30, 12),
			new TileLocation("Town", 88, 3),
			new TileLocation("Blacksmith", 12, 14),
			new TileLocation("Town", 91, 104),
			new TileLocation("JoshHouse", 3, 5),
			new TileLocation("Town", 3, 65),
			new TileLocation("Farm", 8, 8),
			new TileLocation("Forest", 35, 25),
			new TileLocation("AnimalShop", 8, 6),
			new TileLocation("Sewer", 16, 28),
			new TileLocation("BugLand", 48, 46),
			new TileLocation("BugLand", 17, 29),
			new TileLocation("BugLand", 31, 5),
			new TileLocation("Beach", 83, 38),
			new TileLocation("SandyHouse", 4, 4),
			new TileLocation("SkullCave", 3, 5),
			new TileLocation("Desert", 6, 35),
			new TileLocation("Backwoods", 27, 27),
			new TileLocation("SeedShop", 37, 18),
			new TileLocation("Forest", 40, 6)
		};
	}
}
