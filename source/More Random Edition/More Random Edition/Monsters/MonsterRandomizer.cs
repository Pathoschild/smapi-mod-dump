using System;
using System.Collections.Generic;
using System.Linq;

namespace Randomizer
{
	public class MonsterRandomizer
	{
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
			Dictionary<string, string> replacements = new Dictionary<string, string>();

			List<Monster> allMonsters = MonsterData.GetAllMonsters();
			Dictionary<int, int> monsterItemSwaps = GetMonsterDropReplacements(allMonsters);
			Dictionary<string, ItemDrop> extraItemDrops = new Dictionary<string, ItemDrop>();

			foreach (Monster monster in allMonsters)
			{
				monster.HP = Math.Max(Globals.RNGGetIntWithinPercentage(monster.HP, HPVariance), 1);
				monster.Damage = Math.Max(Globals.RNGGetIntWithinPercentage(monster.Damage, DamageVariance), 1);
				monster.RandomMovementDuration = Globals.RNGGetNextBoolean(35) ? 0 : Range.GetRandomValue(1, 3000);
				RandomizeMonsterDrops(monster, monsterItemSwaps, extraItemDrops);
				RandomizeResilience(monster);
				monster.Jitteriness = Range.GetRandomValue(0, 2) / 100d;
				RandomizeMoveTowardPlayerThreshold(monster);
				monster.Speed = Range.GetRandomValue(1, 4);
				monster.MissChance = Range.GetRandomValue(0, 5) / 100d;
				monster.Experience = Math.Max(Globals.RNGGetIntWithinPercentage(monster.Experience, ExperienceVariance), 1);

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
		private static Dictionary<int, int> GetMonsterDropReplacements(List<Monster> allMonsters)
		{
			Dictionary<int, Item> items = new Dictionary<int, Item>();
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

			List<int> uniqueIds = items.Keys.ToList();
			Dictionary<int, int> replacements = new Dictionary<int, int>();
			foreach (int id in items.Keys.ToArray())
			{
				int newId = Globals.RNGGetAndRemoveRandomValueFromList(uniqueIds);
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
			if (Globals.RNGGetNextBoolean())
			{
				item = ItemList.GetRandomItemAtDifficulty(ObtainingDifficulties.NoRequirements);
				probability = Range.GetRandomValue(5, 8) / 100d;
			}

			else if (Globals.RNGGetNextBoolean())
			{
				item = ItemList.GetRandomItemAtDifficulty(ObtainingDifficulties.SmallTimeRequirements);
				probability = Range.GetRandomValue(4, 6) / 100d;
			}

			else if (Globals.RNGGetNextBoolean())
			{
				item = ItemList.GetRandomItemAtDifficulty(ObtainingDifficulties.MediumTimeRequirements);
				probability = Range.GetRandomValue(3, 5) / 100d;
			}

			else if (Globals.RNGGetNextBoolean())
			{
				item = ItemList.GetRandomItemAtDifficulty(ObtainingDifficulties.LargeTimeRequirements);
				probability = Range.GetRandomValue(2, 4) / 100d;
			}

			else if (Globals.RNGGetNextBoolean())
			{
				item = ItemList.GetRandomItemAtDifficulty(ObtainingDifficulties.UncommonItem);
				probability = Range.GetRandomValue(1, 2) / 100d;
			}

			else
			{
				item = ItemList.GetRandomItemAtDifficulty(ObtainingDifficulties.RareItem);
				probability = Range.GetRandomValue(1, 5) / 1000d;
			}

			return new ItemDrop(item.Id, probability);
		}

		/// <summary>
		/// Swaps around monster drops and adds the extra drop to each monster
		/// </summary>
		/// <param name="monster">The monster</param>
		/// <param name="swaps">The swaps</param>
		private static void RandomizeMonsterDrops(Monster monster, Dictionary<int, int> swaps, Dictionary<string, ItemDrop> extras)
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
		private static void SwapMonsterItemDrops(Monster monster, Dictionary<int, int> swaps)
		{
			if (!Globals.Config.SwapUniqueMonsterDrops_Needs_Above_Setting_On) { return; }

			foreach (ItemDrop itemDrop in monster.ItemDrops)
			{
				int id = itemDrop.ItemToDrop.Id;
				if (swaps.ContainsKey(id))
				{
					itemDrop.ItemToDrop = ItemList.Items[swaps[id]];
				}
			}
		}

		/// <summary>
		/// Randomizes the resilience of a monster
		/// </summary>
		/// <param name="monster"></param>
		private static void RandomizeResilience(Monster monster)
		{
			if (monster.Resilience == 0)
			{
				monster.Resilience = Globals.RNGGetNextBoolean(ResilienceVariance) ? 1 : 0;
			}

			else
			{
				monster.Resilience = Globals.RNGGetIntWithinPercentage(monster.Resilience, ResilienceVariance);
			}
		}

		/// <summary>
		/// Randomizes the threshold that the monster must hit in order to move toward the player
		/// - 5% chance of it being a large number
		/// </summary>
		/// <param name="monster">The monster to set the value of</param>
		private static void RandomizeMoveTowardPlayerThreshold(Monster monster)
		{
			if (Globals.RNGGetNextBoolean(5))
			{
				monster.MovesTowardPlayerThreshold = Range.GetRandomValue(8, 12);
			}

			else
			{
				monster.MovesTowardPlayerThreshold = Range.GetRandomValue(0, 4);
			}
		}

		/// <summary>
		/// Writes the monster info to the spoiler log
		/// </summary>
		/// <param name="allMonsters">The monster</param>
		/// <param name="monsterItemSwaps">The item swaps performed</param>
		/// <param name="extraItemDrops">The map of monsters to their extra item drop</param>
		private static void WriteToSpoilerLog(
			List<Monster> allMonsters,
			Dictionary<int, int> monsterItemSwaps,
			Dictionary<string, ItemDrop> extraItemDrops)
		{
			WriteSwapInfoToSpoilerLog(monsterItemSwaps);
			WriteMonsterInfoToSpoilerLog(allMonsters, extraItemDrops);
		}

		/// <summary>
		/// Writes the monster info to the spoiler log
		/// </summary>
		/// <param name="monsterItemSwaps">The item swaps performed</param>
		private static void WriteSwapInfoToSpoilerLog(Dictionary<int, int> monsterItemSwaps)
		{
			if (!Globals.Config.RandomizeMonsters) { return; }

			Globals.SpoilerWrite("===== MONSTERS =====");
			Globals.SpoilerWrite("");
			Globals.SpoilerWrite("> Major monster drop swaps");

			foreach (int originalId in monsterItemSwaps.Keys)
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
			if (!Globals.Config.RandomizeMonsters) { return; }

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
