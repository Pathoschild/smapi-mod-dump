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
using System.Linq;

namespace Randomizer
{
	public class Monster
	{
		public int HP { get; set; }
		public int Damage { get; set; }
		public Range Coins { get; set; }
		public bool IsGlider { get; set; }
		public int RandomMovementDuration { get; set; }
		public List<ItemDrop> ItemDrops { get; set; }
		public int Resilience { get; set; }
		public double Jitteriness { get; set; }
		public int MovesTowardPlayerThreshold { get; set; }
		public int Speed { get; set; }
		public double MissChance { get; set; }
		public bool IsMinesMonster { get; set; }
		public int Experience { get; set; }

		/// <summary>
		/// This is the key in the Monsters data file
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// This is the display name in the Monsters data file (the last property)
		/// </summary>
		public string DisplayName { get; set; }

		/// <summary>
		/// Constructor specifically for mines monsters
		/// </summary>
		public Monster(
			int hp,
			int damage,
			int minCoins,
			int maxCoins,
			bool isGlider,
			int randomMovementDuration,
			string itemDropString,
			int resilience,
			double jitteriness,
			int moveTowardPlayer,
			int speed,
			double missChance,
			bool isMinesMonster,
			int experience,
			string name,
			string displayName)
		{
			HP = hp;
			Damage = damage;
			Coins = new Range(minCoins, maxCoins);
			IsGlider = isGlider;
			RandomMovementDuration = randomMovementDuration;
			ItemDrops = ItemDrop.ParseString(itemDropString);
			Resilience = resilience;
			Jitteriness = jitteriness;
			MovesTowardPlayerThreshold = moveTowardPlayer;
			Speed = speed;
			MissChance = missChance;
			IsMinesMonster = isMinesMonster;
			Experience = experience;
			Name = name;
			DisplayName = displayName;
		}

		/// <summary>
		/// Returns the string to use for the assets
		/// </summary>
		/// <returns />
		public override string ToString()
		{
			string itemString = string.Join(" ", ItemDrops.Select(x => x.ToString()));
			string gliderString = IsGlider ? "true" : "false";
			string mineMonsterString = IsMinesMonster ? "true" : "false";

			return $"{HP}/{Damage}/{Coins.MinValue}/{Coins.MaxValue}/{gliderString}/{RandomMovementDuration}/{itemString}/{Resilience}/{Jitteriness}/{MovesTowardPlayerThreshold}/{Speed}/{MissChance}/{mineMonsterString}/{Experience}/{Name}";
		}
	}
}
