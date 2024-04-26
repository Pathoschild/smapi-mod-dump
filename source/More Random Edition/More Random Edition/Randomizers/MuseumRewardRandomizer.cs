/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Chikakoo/stardew-valley-randomizer
**
*************************************************/

using Force.DeepCloner;
using StardewValley;
using StardewValley.GameData.Museum;
using System.Collections.Generic;
using System.Linq;

namespace Randomizer
{
    /// <summary>
    /// Randomizes the rewards the player gets from completing the museum
    /// This sets them to be items in a similar category
    /// Does not include important items: Dwarvish Translation Manual, Ancient Seeds, Stardrop
    /// </summary>
    public class MuseumRewardRandomizer
    {
        private static RNG Rng { get; set; }

        /// <summary>
        /// A list of used reward Ids - these must be unique because it will result in Duplicate rewards
        /// being glitchy if you only claim one of them
        /// </summary>
        private static List<string> UsedRewardIds { get; } = new();

        /// <summary>
        /// Randomize museum rewards to similar rewards
        /// - Seeds -> different seeds from the same season
        /// - Furniture/BigCraftables -> random item of the same kind
        /// 
        /// Does NOT randomize the ancient seeds, Dwarven Translation Manual, or the stardrop
        /// </summary>
        /// <returns>The dictionary of replacements to replace the asset with</returns>
        public static Dictionary<string, MuseumRewards> RandomizeMuseumRewards()
        {
            Dictionary<string, MuseumRewards> museumRewardReplacements = new();
            if (!Globals.Config.RandomizeMuseumRewards)
            {
                return museumRewardReplacements;
            }

            Rng = RNG.GetFarmRNG(nameof(MuseumRewardRandomizer));
            UsedRewardIds.Clear();

            Dictionary<string, MuseumRewards> museumRewards = 
                DataLoader.MuseumRewards(Game1.content);

            Globals.SpoilerWrite("==== MUSEUM REWARDS ====");

            foreach (var museumRewardsData in museumRewards)
            {
                string rewardKey = museumRewardsData.Key;
                MuseumRewards oldRewardData = museumRewardsData.Value;

                if (SkipRandomizingReward(oldRewardData))
                {
                    continue;
                }

                MuseumRewards repalcementData = oldRewardData.DeepClone();
                ReplaceReward(repalcementData);
                UsedRewardIds.Add(repalcementData.RewardItemId);

                museumRewardReplacements[rewardKey] = repalcementData;

                string oldRewardString = GetSpoilerLogRewardString(oldRewardData.RewardItemId);
                string newRewardString = GetSpoilerLogRewardString(repalcementData.RewardItemId);
                Globals.SpoilerWrite($"{oldRewardString} was changed to {newRewardString}");
            }

            Globals.SpoilerWrite("");
            return museumRewardReplacements;
        }

        /// <summary>
        /// Returns whether we should skip randomizing the given reward - we skip if:
        /// - There's no reward id (this one is the skull key)
        /// - The reward is the DwarvishTranslationManual, Ancient Seeds, or Stardop
        /// - The reward is a recipe (currently is only the Ancient Seeds)
        /// </summary>
        /// <param name="rewardData">The reward data</param>
        /// <returns>True if we should skip changing the reward, false otherwise</returns>
        private static bool SkipRandomizingReward(MuseumRewards rewardData)
        {
            // This one is the skull key
            if (rewardData.RewardItemId == null)
            {
                return true;
            }

            if (Item.IsQualifiedIdForObject(rewardData.RewardItemId))
            {
                const string DwarvshTraslationManualId = "(O)326";

                Item matchingItem = ItemList.GetItemFromStringId(rewardData.RewardItemId);
                if (matchingItem == null ||
                    rewardData.RewardItemId == DwarvshTraslationManualId ||
                    matchingItem.ObjectIndex == ObjectIndexes.AncientSeeds ||
                    matchingItem.ObjectIndex == ObjectIndexes.Stardrop)
                {
                    return true;
                }
            }

            return rewardData.RewardItemIsRecipe;
        }

        /// <summary>
        /// Replaces the reward with our own
        /// - Furniture and big craftables get a random item of the same type
        /// - We treat the flute/drum blocks as furniture, as its the same idea
        /// </summary>
        /// <param name="rewardData">The given reward</param>
        private static void ReplaceReward(MuseumRewards rewardData)
        {
            string oldRewardId = rewardData.RewardItemId;

            bool isDropOrFluteBlock = oldRewardId == ItemList.GetQualifiedId(ObjectIndexes.FluteBlock) ||
                oldRewardId == ItemList.GetQualifiedId(ObjectIndexes.DrumBlock);
            if (FurnitureFunctions.IsQualifiedIdForFurniture(oldRewardId) ||
                isDropOrFluteBlock)
            {
                rewardData.RewardItemId = FurnitureFunctions.GetRandomFurnitureQualifiedId(Rng, UsedRewardIds);
            }

            else if (BigCraftableFunctions.IsQualifiedIdForBigCraftable(oldRewardId))
            {
                rewardData.RewardItemId = BigCraftableFunctions.GetRandomBigCraftableQualifiedId(Rng, UsedRewardIds);
            }

            else if (Item.IsQualifiedIdForObject(oldRewardId))
            {
                ModifyObjectRewardData(rewardData);
            }

            else
            {
                Globals.ConsoleWarn($"Warning: unrecognized museum reward to replace: {rewardData.RewardItemId}");
            }
        }

        /// <summary>
        /// Modifies the given rewardData - this assumes the item you give it is a stardew Valley Object
        /// - If it's a seed item, gets a random seed from the same season
        ///   - The starfruit reward is special in that we want ANY season, but less of it
        /// - It it's a totem, get a random totem
        /// - Else it's a cooked item, so get a random one of those
        /// </summary>
        /// <param name="rewardData">The reward data to modify</param>
        private static void ModifyObjectRewardData(MuseumRewards rewardData)
        {
            Item item = ItemList.GetItemFromStringId(rewardData.RewardItemId);
            if (item is SeedItem seedItem)
            {
                // The starfruit reward is an exception - we want to grab ANY seed, and give less of it
                bool isStarFruitSeedReward = item.ObjectIndex == ObjectIndexes.StarfruitSeeds;
                Seasons? season = isStarFruitSeedReward
                    ? null
                    : seedItem.GrowingSeasons[0];

                var seedPool = ItemList.Items.Values
                    .Where(item => 
                        item is SeedItem seedItem &&
                        !UsedRewardIds.Contains(item.QualifiedId) &&
                        item.ObjectIndex != ObjectIndexes.AncientSeeds && // This would clash with the ancient seed artifact reward!
                        (season == null || seedItem.GrowingSeasons.Contains(season.Value)))
                    .Cast<SeedItem>()
                    .ToList();

                rewardData.RewardItemId = Rng.GetRandomValueFromList(seedPool).QualifiedId;
                rewardData.RewardItemCount = isStarFruitSeedReward
                    ? Rng.NextIntWithinRange(3, 8)
                    : Rng.NextIntWithinRange(5, 15);
            }

            else if (item.IsTotem)
            {
                // Only one totem is possible, so no need to check UsedRewardIds
                rewardData.RewardItemId = ItemList.GetRandomTotem(Rng).QualifiedId;
            }

            else
            {
                // Otherwise, it's a random cooked item
                rewardData.RewardItemId = Rng.GetRandomValueFromList(
                    ItemList.GetCookedItems()
                        .Where(item => !UsedRewardIds.Contains(item.QualifiedId))
                        .ToList()
                    ).QualifiedId;
                rewardData.RewardItemCount = Rng.NextIntWithinRange(3, 10);
            }        
        }

        /// <summary>
        /// Gets the spoiler log string for the given qualified id
        /// </summary>
        /// <param name="rewardId">The qualified id</param>
        /// <returns>The reward string to use for the spoiler log</returns>
        private static string GetSpoilerLogRewardString(string rewardId)
        {
            return $"{rewardId} ({ItemList.GetDisplayNameFromQualifiedId(rewardId)})";
        }
    }
}
