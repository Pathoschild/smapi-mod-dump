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
	public class MonsterRandomizer
	{
		private static RNG Rng { get; set; }

		private const int HPVariance = 25;
		private const int DamageVariance = 25;
		private const int ResilienceVariance = 25;
		private const int ExperienceVariance = 50;

		/// <summary>
		/// Randomizes monster stats/drops/etc.
		/// - Skips coin range randomization - not sure what it's for
		/// - Skips whether a monster is a glider, as flying monster spawns are hard-coded... don't want to
		///   accidently spawn a non-flying monster somewhere it can't move
		/// </summary>
		/// <returns />
		public static Dictionary<string, string> Randomize()
		{
			Rng = RNG.GetFarmRNG(nameof(MonsterRandomizer));
			Dictionary<string, string> replacements = new();

			List<Monster> allMonsters = MonsterData.GetAllMonsters();
			Dictionary<string, string> monsterItemSwaps = GetMonsterDropReplacements(allMonsters);
			Dictionary<string, ItemDrop> extraItemDrops = new();

			foreach (Monster monster in allMonsters)
			{
				monster.HP = Math.Max(Rng.NextIntWithinPercentage(monster.HP, HPVariance), 1);
				monster.Damage = Math.Max(Rng.NextIntWithinPercentage(monster.Damage, DamageVariance), 1);
				monster.RandomMovementDuration = Rng.NextBoolean(35) ? 0 : Rng.NextIntWithinRange(1, 3000);
				RandomizeMonsterDrops(monster, monsterItemSwaps, extraItemDrops);
				RandomizeResilience(monster);
				monster.Jitteriness = Rng.NextIntWithinRange(0, 2) / 100d;
				RandomizeMoveTowardPlayerThreshold(monster);
				monster.Speed = Rng.NextIntWithinRange(1, 4);
				monster.MissChance = Rng.NextIntWithinRange(0, 5) / 100d;
				monster.Experience = Math.Max(Rng.NextIntWithinPercentage(monster.Experience, ExperienceVariance), 1);

				replacements.Add(monster.Name, monster.ToString());
			}

			WriteToSpoilerLog(allMonsters, monsterItemSwaps, extraItemDrops);

			return replacements;
		}

		/// <summary>
		/// Gets a mapping of all monster items to some other random monster item
		/// Also swaps the item obtainability and crafting amounts 
		/// </summary>
		/// <param name="allMonsters">The monster info</param>
		/// <returns />
		private static Dictionary<string, string> GetMonsterDropReplacements(List<Monster> allMonsters)
		{
			Dictionary<string, Item> items = new();
			foreach (Monster monster in allMonsters)
			{
				foreach (ItemDrop itemDrop in monster.ItemDrops)
				{
					Item itemToDrop = itemDrop.ItemToDrop;
					if (!items.ContainsKey(itemToDrop.Id) && itemToDrop.IsMonsterItem)
					{
						items.Add(itemToDrop.Id,
							new Item(itemToDrop.Id)
							{
								DifficultyToObtain = itemToDrop.DifficultyToObtain,
								ItemsRequiredForRecipe = itemToDrop.ItemsRequiredForRecipe
							}
						);
					}
				}
			}

			List<string> uniqueIds = items.Keys.ToList();
			Dictionary<string, string> replacements = new();
			foreach (string id in items.Keys.ToArray())
			{
				string newId = Rng.GetAndRemoveRandomValueFromList(uniqueIds);
				replacements.Add(id, newId);

				Item newItem = ItemList.Items[newId];
				Item oldItemInfo = items[id];
				newItem.DifficultyToObtain = oldItemInfo.DifficultyToObtain;
				newItem.ItemsRequiredForRecipe = oldItemInfo.ItemsRequiredForRecipe;
			}

			return replacements;
		}

		/// <summary>
		/// Gets a random item drop - starts at noreqs and has a 50% chance to continue down the rarity chain
		/// - No req = 1/2, 5-8% drop rate
		/// - Small time req = 1/4, 4-6% drop rate
		/// - Med time req = 1/8, 3-5% drop rate
		/// - Large time req = 1/16, 2-4% drop rate
		/// - Uncommon = 1/32, 1-2% drop rate
		/// - Rare = 1/64, 0.1-0.5% drop rate
		/// </summary>
		/// <returns>The random item drop</returns>
		private static ItemDrop GetRandomItemDrop()
		{
			double probability = 0;
			Item item = null;
			if (Rng.NextBoolean())
			{
				item = ItemList.GetRandomItemAtDifficulty(Rng, ObtainingDifficulties.NoRequirements);
				probability = Rng.NextIntWithinRange(5, 8) / 100d;
			}

			else if (Rng.NextBoolean())
			{
				item = ItemList.GetRandomItemAtDifficulty(Rng, ObtainingDifficulties.SmallTimeRequirements);
				probability = Rng.NextIntWithinRange(4, 6) / 100d;
			}

			else if (Rng.NextBoolean())
			{
				item = ItemList.GetRandomItemAtDifficulty(Rng, ObtainingDifficulties.MediumTimeRequirements);
				probability = Rng.NextIntWithinRange(3, 5) / 100d;
			}

			else if (Rng.NextBoolean())
			{
				item = ItemList.GetRandomItemAtDifficulty(Rng, ObtainingDifficulties.LargeTimeRequirements);
				probability = Rng.NextIntWithinRange(2, 4) / 100d;
			}

			else if (Rng.NextBoolean())
			{
				item = ItemList.GetRandomItemAtDifficulty(Rng, ObtainingDifficulties.UncommonItem);
				probability = Rng.NextIntWithinRange(1, 2) / 100d;
			}

			else
			{
				item = ItemList.GetRandomItemAtDifficulty(Rng, ObtainingDifficulties.RareItem);
				probability = Rng.NextIntWithinRange(1, 5) / 1000d;
			}

			return new ItemDrop(item.ObjectIndex, probability);
		}

		/// <summary>
		/// Swaps around monster drops and adds the extra drop to each monster
		/// </summary>
		/// <param name="monster">The monster</param>
		/// <param name="swaps">The swaps</param>
		private static void RandomizeMonsterDrops(
			Monster monster, 
			Dictionary<string, string> swaps, 
			Dictionary<string, ItemDrop> extras)
		{
			SwapMonsterItemDrops(monster, swaps);

			ItemDrop extraDrop = GetRandomItemDrop();
			extras[monster.Name] = extraDrop;
			monster.ItemDrops.Add(extraDrop);
		}

		/// <summary>
		/// Swaps the monster item drops
		/// </summary>
		/// <param name="monster">The monster</param>
		/// <param name="swaps">The swaps</param>
		private static void SwapMonsterItemDrops(Monster monster, Dictionary<string, string> swaps)
		{
			if (!Globals.Config.Monsters.Randomize || !Globals.Config.Monsters.SwapUniqueDrops) { return; }

			foreach (ItemDrop itemDrop in monster.ItemDrops)
			{
				string id = itemDrop.ItemToDrop.Id;
				if (swaps.ContainsKey(id))
				{
					itemDrop.ItemToDrop = ItemList.Items[swaps[id]];
				}
			}
		}

		/// <summary>
		/// Randomizes the resilience of a monster
		/// - If already 0, 50% chance to set to 1
		/// - If not 0, then vary it by the variance value
		/// </summary>
		/// <param name="monster"></param>
		private static void RandomizeResilience(Monster monster)
		{
			monster.Resilience = monster.Resilience == 0
				? Rng.NextBoolean(ResilienceVariance) ? 1 : 0
				: Rng.NextIntWithinPercentage(monster.Resilience, ResilienceVariance);
		}

		/// <summary>
		/// Randomizes the threshold that the monster must hit in order to move toward the player
		/// - 5% chance of it being a large number
		/// </summary>
		/// <param name="monster">The monster to set the value of</param>
		private static void RandomizeMoveTowardPlayerThreshold(Monster monster)
		{
			monster.MovesTowardPlayerThreshold = Rng.NextBoolean(5)
				? Rng.NextIntWithinRange(8, 12)
				: Rng.NextIntWithinRange(0, 4);
		}

		/// <summary>
		/// Writes the monster info to the spoiler log
		/// </summary>
		/// <param name="allMonsters">The monster</param>
		/// <param name="monsterItemSwaps">The item swaps performed</param>
		/// <param name="extraItemDrops">The map of monsters to their extra item drop</param>
		private static void WriteToSpoilerLog(
			List<Monster> allMonsters,
			Dictionary<string, string> monsterItemSwaps,
			Dictionary<string, ItemDrop> extraItemDrops)
		{
			WriteSwapInfoToSpoilerLog(monsterItemSwaps);
			WriteMonsterInfoToSpoilerLog(allMonsters, extraItemDrops);
		}

		/// <summary>
		/// Writes the monster info to the spoiler log
		/// </summary>
		/// <param name="monsterItemSwaps">The item swaps performed</param>
		private static void WriteSwapInfoToSpoilerLog(Dictionary<string, string> monsterItemSwaps)
		{
			if (!Globals.Config.Monsters.Randomize) { return; }

			Globals.SpoilerWrite("===== MONSTERS =====");
			Globals.SpoilerWrite("");
			Globals.SpoilerWrite("> Major monster drop swaps");

			foreach (string originalId in monsterItemSwaps.Keys)
			{
				Globals.SpoilerWrite($"{ItemList.Items[originalId].Name} -> {ItemList.Items[monsterItemSwaps[originalId]].Name}");
			}
		}

		/// <summary>
		/// Writes the monster info to the spoiler log
		/// </summary>
		/// <param name="allMonsters">The monster</param>
		/// <param name="extraItemDrops">The map of monsters to their extra item drop</param>
		private static void WriteMonsterInfoToSpoilerLog(List<Monster> allMonsters, Dictionary<string, ItemDrop> extraItemDrops)
		{
			if (!Globals.Config.Monsters.Randomize) { return; }

			Globals.SpoilerWrite("");
			Globals.SpoilerWrite("> Monster stats");

			foreach (Monster monster in allMonsters)
			{
				ItemDrop extraDrop = extraItemDrops[monster.Name];

				Globals.SpoilerWrite("---");
				Globals.SpoilerWrite(monster.Name);
				Globals.SpoilerWrite($"{monster.HP} HP; {monster.Damage} Damage; {monster.Resilience} Resilience; {monster.Speed} Speed");
				Globals.SpoilerWrite($"Miss chance: {monster.MissChance * 100}%");
				Globals.SpoilerWrite($"Experience: {monster.Experience}");
				Globals.SpoilerWrite($"Moves randomly for {monster.RandomMovementDuration / 1000d} seconds");
				Globals.SpoilerWrite($"Jitteriness value: {monster.Jitteriness}");
				Globals.SpoilerWrite($"Threshold until the monster moves towards players: {monster.MovesTowardPlayerThreshold}");
				Globals.SpoilerWrite($"Extra drop: {extraDrop.ItemToDrop.Name} at {extraDrop.Probability * 100}%");
			}

			Globals.SpoilerWrite("");
		}
	}
}
