using System.Collections.Generic;

namespace Randomizer
{
	/// <summary>
	/// Contains data about monsters
	/// </summary>
	public class MonsterData
	{
		/// <summary>
		/// The data from the xnb file. Note the following changes:
		/// - The value -4 is changed to coal
		/// - The value -6 is changed to gold ore
		/// 
		/// These changes were based on the source code
		/// See the constructor for "Debrs" in Debris.cs
		/// </summary>
		public static List<string> DefaultStringData = new List<string>
		{
			"24/5/0/0/false/1000/766 .75 766 .05 153 .1 66 .015 92 .15 96 .005 99 .001/1/.01/4/2/.00/true/3/Green Slime",
			"40/6/0/0/false/1000/382 .5 433 .01 336 .001 84 .02 414 .02 97 .005 99 .001/2/.00/4/3/.00/true/2/Dust Spirit",
			"24/6/0/0/true/1000/767 .9 767 .4 108 .001 287 .02 96 .005 99 .001/1/.01/4/3/.00/true/3/Bat",
			"36/7/0/0/true/1000/767 .9 767 .55 108 .001 287 .02 97 .005 99 .001/1/.01/4/3/.00/true/7/Frost Bat",
			"80/15/0/0/true/1000/767 .9 767 .7 108 .001 287 .02 98 .005 99 .001/1/.01/4/3/.00/true/15/Lava Bat",
			"300/30/0/0/true/1000/386 .9 386 .5 386 .25 386 .1 288 .05 768 .5 773 .05 349 .05 787 .05 337 .008/1/.01/4/3/.00/true/22/Iridium Bat",
			"45/5/0/0/false/1000/390 .9 80 .1 382 .1 380 .1 96 .005 99 .001/5/.01/3/2/.00/true/5/Stone Golem",
			"30/5/0/0/false/1000/771 .9 771 .5 770 .5 382 .1 86 .005 72 .001/1/.01/3/2/.00/true/5/Wilderness Golem",
			"20/4/0/0/false/1000/684 .6 273 .05 273 .05 157 .02 114 .005 96 .005 99 .001/0/.005/3/1/.00/true/2/Grub",
			"22/6/0/0/true/1000/684 .9 157 .02 114 .005 96 .005 99 .001/1/.005/13/2/.0/true/10/Fly",
			"106/7/0/0/false/1000/766 .75 412 .08 70 .02 98 .015 92 .5 97 .005 99 .001/0/.01/4/2/.0/true/6/Frost Jelly",
			$"205/16/0/0/false/1000/766 .8 157 .1 {(int)ObjectIndexes.Coal} .1 72 .01 92 .5 98 .005 99 .001/0/.01/4/2/.0/true/10/Sludge",
			$"125/20/0/0/false/0/769 .75 769 .1 329 .02 337 .002 336 .01 335 .02 334 .04 203 .04 293 .03 108 .003 {(int)ObjectIndexes.Coal} .1 98 .005 99 .001/2/.01/-1/2/.0/true/15/Shadow Guy",
			$"96/10/0/3/false/0/768 .95 768 .1 156 .08 338 .08 {(int)ObjectIndexes.GoldOre} .2 97 .005 99 .001/3/.01/12/4/.0/true/15/Ghost",
			"190/25/0/3/false/0/749 .99 338 .1/3/.01/12/4/.0/true/20/Carbon Ghost",
			"40/6/0/0/false/0/286 .25 535 .25 280 .03 105 .02 86 .1 72 .01 96 .005 99 .001/0/.01/1/2/.0/true/10/Duggy",
			"30/5/0/0/false/0/717 .15 286 .4 96 .005 99 .001/1/0/1/2/.0/true/4/Rock Crab",
			"120/15/0/0/false/0/717 .25 287 .4 98 .005 99 .001/3/0/1/3/.0/true/12/Lava Crab",
			"240/15/0/0/false/0/732 .5 386 .5 386 .5 386 .5/3/0/1/3/.0/true/20/Iridium Crab",
			"1/18/0/0/true/0/768 .75 814 .9 814 .5 814 .2 336 .05 287 .1 288 .05 98 .005 99 .001/2/0/6/3/.0/true/15/Squid Kid", // Added two squid ink drops for balance
			"300/12/1/3/false/0/769 .25 105 .03 106 .03 166 .001 60 .04 232 .04 72 .03 74 .01 97 .005 99 .001/3/0/5/2/.0/true/15/Skeleton Warrior",
			$"160/18/0/0/false/0/814 .75 769 .75 769 .1 337 .002 336 .01 335 .02 334 .04 203 .04 108 .003 {(int)ObjectIndexes.Coal} .1 98 .005 99 .001 74 .0005/2/.01/8/3/.0/true/15/Shadow Brute", // Added squid ink drop for balance
			$"80/17/0/0/false/0/769 .75 769 .2 337 .002 336 .01 335 .02 334 .04 108 .003 {(int)ObjectIndexes.Coal} .1 98 .005 99 .001 74 .0005/2/.01/8/3/.0/true/15/Shadow Shaman",
			"140/10/0/2/false/2000/80 0/1/.01/8/2/.0/true/8/Skeleton",
			"60/5/0/2/false/2000/80 0/1/.01/8/2/.0/true/8/Skeleton Mage",
			"40/15/0/0/false/2000/768 .65 378 .1 378 .1 380 .1 380 .1 382 .1 98 .005 99 .001/8/.01/8/2/.0/true/6/Metal Head",
			"5/5/0/0/false/2000/378 .1 378 .1 380 .1 380 .1 382 .1/4/.01/8/2/.0/true/1/Spiker",
			"1/8/0/0/false/2000/684 .76 157 .02 114 .005 96 .005 99 .001/0/0/-1/2/.0/true/1/Bug",
			"260/30/0/3/false/1000/768 .99 428 .2 428 .05 768 .15 243 .04 99 .001 74 .001/0/.01/8/2/.0/true/20/Mummy",
			"60/5/0/0/false/1000/766 .99 766 .9 766 .4 99 .001/0/.01/5/2/.0/true/7/Big Slime",
			"150/23/0/2/true/1000/769 .99 769 .15 287 .15 226 .06 446 .008 74 .001/0/.01/13/2/.0/true/20/Serpent",
			"300/15/0/0/false/1000/80 0/5/.01/3/2/.0/true/7/Pepper Rex"
		};

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
			if (fields.Length != 15)
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
		/// The weapon items in dictionary form - data taken from Monsters.xnb
		/// </summary>
		/// <returns />
		public static List<Monster> GetAllMonsters()
		{
			List<Monster> monsters = new List<Monster>();
			foreach (string data in DefaultStringData)
			{
				Monster monster = ParseMonster(data);
				if (monster != null)
				{
					monsters.Add(ParseMonster(data));
				}
			}

			return monsters;
		}
	}
}