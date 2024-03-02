/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Chikakoo/stardew-valley-randomizer
**
*************************************************/

using System.Collections.Generic;

namespace Randomizer
{
    /// <summary>
    /// Contains data about monsters
    /// </summary>
    public class MonsterData
	{
		private static Dictionary<string, string> DefaultMonsterData;

        /// <summary>
        /// Initialize the data from the XNB file, but make the following changes:
        /// - The value -4 is changed to coal
        /// - The value -6 is changed to gold ore
        /// 
        /// These changes were based on the source code
        /// See the constructor for "Debrs" in Debris.cs
        /// </summary>
        private static void Initialize()
		{
            DefaultMonsterData = Globals.ModRef.Helper.GameContent
				.Load<Dictionary<string, string>>("Data/Monsters");

			foreach(KeyValuePair<string, string> data in DefaultMonsterData)
			{
				string key = data.Key;
				string[] dataParts = data.Value.Split("/");

				// Note that parsing this will replace -4 and -6 with the appropriate items
				List<ItemDrop> itemDrops = ItemDrop.ParseString(dataParts[(int)MonsterFields.ItemDrops]);

				// Additional squid ink is added to the pool for balance purposes, since there's barely
				// anything that drops it, and it may be required for recipes
				switch (key)
				{
                    // Add two squid ink drops before the one that already exists
                    // Should result in: 814 .9 814 .5 814 .2 (.2 already exists)
                    case "Squid Kid":
						const int SquidKidInkIndex = 1;
						itemDrops.Insert(SquidKidInkIndex, new ItemDrop(ObjectIndexes.SquidInk, 0.5));
                        itemDrops.Insert(SquidKidInkIndex, new ItemDrop(ObjectIndexes.SquidInk, 0.9));
                        break;
					// Add one squid innk drop at the start of the list
					case "Shadow Brute":
                        const int ShadowBruteInkIndex = 0;
                        itemDrops.Insert(ShadowBruteInkIndex, new ItemDrop(ObjectIndexes.SquidInk, 0.75));
						break;
                }

				// Actually replace the item drops
				dataParts[(int)MonsterFields.ItemDrops] = string.Join(" ", itemDrops);

				DefaultMonsterData[key] = string.Join("/", dataParts);
			}
		}

		public enum MonsterFields
		{
			HP,
			Damage,
			MinCoins,
			MaxCoins,
			IsGlider,
			RandomMovementDuration,
			ItemDrops,
			Resiliance,
			Jitteriness,
			MoveTowardPlayer,
			Speed,
			MissChance,
			IsMinesMonster,
			Experience,
			Name
		};

		/// <summary>
		/// Parses the xnb string into a Monster object
		/// </summary>
		/// <param name="data">The xnb string</param>
		/// <returns />
		public static Monster ParseMonster(string data)
		{
			string[] fields = data.Split('/');
			if (fields.Length < 15)
			{
				Globals.ConsoleError($"Incorrect number of fields when parsing monster with input: {data}");
				return null;
			}

			int hp = ParseIntField(fields[(int)MonsterFields.HP], data, "HP");
			int damage = ParseIntField(fields[(int)MonsterFields.Damage], data, "Damage");
			int minCoins = ParseIntField(fields[(int)MonsterFields.MinCoins], data, "Min Coins");
			int maxCoins = ParseIntField(fields[(int)MonsterFields.MaxCoins], data, "Max Coins");
			bool isGlider = ParseBooleanField(fields[(int)MonsterFields.IsGlider], data, "Is Glider");
			int randomMovementDuration = ParseIntField(fields[(int)MonsterFields.RandomMovementDuration], data, "Random Movement Duration");
			string itemDrops = fields[(int)MonsterFields.ItemDrops];
			int resilience = ParseIntField(fields[(int)MonsterFields.Resiliance], data, "Resilience");
			double jitteriness = ParseDoubleField(fields[(int)MonsterFields.Jitteriness], data, "Jitteriness");
			int movesTowardPlayer = ParseIntField(fields[(int)MonsterFields.MoveTowardPlayer], data, "Moves Toward Player");
			int speed = ParseIntField(fields[(int)MonsterFields.Speed], data, "Speed");
			double missChance = ParseDoubleField(fields[(int)MonsterFields.MissChance], data, "Miss Chance");
			bool isMinesMonster = ParseBooleanField(fields[(int)MonsterFields.IsMinesMonster], data, "Is Mines Monster");
			int experience = ParseIntField(fields[(int)MonsterFields.Experience], data, "Experience");
			string name = fields[(int)MonsterFields.Name];

			return new Monster(
				hp,
				damage,
				minCoins,
				maxCoins,
				isGlider,
				randomMovementDuration,
				itemDrops,
				resilience,
				jitteriness,
				movesTowardPlayer,
				speed,
				missChance,
				isMinesMonster,
				experience,
				name
			);
		}

		/// <summary>
		/// Parse an integer field
		/// </summary>
		/// <param name="intString">The integer to parse</param>
		/// <param name="data">The data</param>
		/// <param name="fieldName">The name of the field to report back</param>
		/// <returns />
		private static int ParseIntField(string intString, string data, string fieldName)
		{
			if (!int.TryParse(intString, out int result))
			{
				Globals.ConsoleError($"Failed to parse monster field {fieldName} with input: {data}");
				return 0;
			}

			return result;
		}

		/// <summary>
		/// Parse an double field
		/// </summary>
		/// <param name="boolString">The double to parse</param>
		/// <param name="data">The data</param>
		/// <param name="fieldName">The name of the field to report back</param>
		/// <returns />
		private static double ParseDoubleField(string doubleString, string data, string fieldName)
		{
			if (!double.TryParse(doubleString, out double result))
			{
				Globals.ConsoleError($"Failed to parse monster field {fieldName} with input: {data}");
				return 0;
			}

			return result;
		}

		/// <summary>
		/// Parse an boolean field
		/// </summary>
		/// <param name="boolString">The boolean to parse</param>
		/// <param name="data">The data</param>
		/// <param name="fieldName">The name of the field to report back</param>
		/// <returns />
		private static bool ParseBooleanField(string boolString, string data, string fieldName)
		{
			if (boolString == "true")
			{
				return true;
			}

			else if (boolString == "false")
			{
				return false;
			}

			Globals.ConsoleError($"Failed to parse monster field {fieldName} with input: {data}");
			return false;
		}

		/// <summary>
		/// The weapon items in dictionary form - data taken from Data/Monsters.xnb
		/// </summary>
		/// <returns />
		public static List<Monster> GetAllMonsters()
		{
			if (DefaultMonsterData == null)
			{
                Initialize();
            }

			List<Monster> monsters = new();
			foreach (string data in DefaultMonsterData.Values)
			{
				Monster monster = ParseMonster(data);
				if (monster != null && monster.IsMinesMonster)
				{
					monsters.Add(monster);
				}
			}

			// count should be 45
			return monsters;
		}
	}
}