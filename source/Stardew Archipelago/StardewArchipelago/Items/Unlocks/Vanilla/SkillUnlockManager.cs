/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewArchipelago.Archipelago;
using StardewArchipelago.Extensions;
using StardewArchipelago.Items.Mail;
using StardewArchipelago.Locations.CodeInjections.Vanilla;
using StardewValley;
using StardewValley.Constants;

namespace StardewArchipelago.Items.Unlocks.Vanilla
{
    public class SkillUnlockManager : IUnlockManager
    {
        private ArchipelagoClient _archipelago;

        public SkillUnlockManager(ArchipelagoClient archipelago)
        {
            _archipelago = archipelago;
        }

        public void RegisterUnlocks(IDictionary<string, Func<ReceivedItem, LetterAttachment>> unlocks)
        {
            RegisterPlayerSkills(unlocks);
        }

        private void RegisterPlayerSkills(IDictionary<string, Func<ReceivedItem, LetterAttachment>> unlocks)
        {
            unlocks.Add($"{Skill.Farming} Level", SendProgressiveFarmingLevel);
            unlocks.Add($"{Skill.Fishing} Level", SendProgressiveFishingLevel);
            unlocks.Add($"{Skill.Foraging} Level", SendProgressiveForagingLevel);
            unlocks.Add($"{Skill.Mining} Level", SendProgressiveMiningLevel);
            unlocks.Add($"{Skill.Combat} Level", SendProgressiveCombatLevel);
            RegisterPlayerMasteries(unlocks);
        }

        private void RegisterPlayerMasteries(IDictionary<string, Func<ReceivedItem, LetterAttachment>> unlocks)
        {
            unlocks.Add($"{Skill.Farming} Mastery", SendFarmingMastery);
            unlocks.Add($"{Skill.Fishing} Mastery", SendFishingMastery);
            unlocks.Add($"{Skill.Foraging} Mastery", SendForagingMastery);
            unlocks.Add($"{Skill.Mining} Mastery", SendMiningMastery);
            unlocks.Add($"{Skill.Combat} Mastery", SendCombatMastery);
        }

        private LetterAttachment SendProgressiveFarmingLevel(ReceivedItem receivedItem)
        {
            const Skill skill = Skill.Farming;
            var apItem = $"{skill} Level";
            var receivedLevels = _archipelago.GetReceivedItemCount(apItem);
            foreach (var farmer in Game1.getAllFarmers())
            {
                var currentLevel = farmer.farmingLevel.Value;
                var newLevel = currentLevel + 1;
                newLevel = Math.Max(0, Math.Min(receivedLevels, newLevel));
                if (newLevel <= currentLevel)
                {
                    continue;
                }

                GiveExperienceToNextLevel(farmer, skill);
                farmer.farmingLevel.Value = newLevel;
                farmer.newLevels.Add(new Point((int)skill, newLevel));
            }
            return new LetterInformationAttachment(receivedItem);
        }

        private LetterAttachment SendProgressiveFishingLevel(ReceivedItem receivedItem)
        {
            const Skill skill = Skill.Fishing;
            var apItem = $"{skill} Level";
            var receivedLevels = _archipelago.GetReceivedItemCount(apItem);
            foreach (var farmer in Game1.getAllFarmers())
            {
                var currentLevel = farmer.fishingLevel.Value;
                var newLevel = currentLevel + 1;
                newLevel = Math.Max(0, Math.Min(receivedLevels, newLevel));
                if (newLevel <= currentLevel)
                {
                    continue;
                }

                GiveExperienceToNextLevel(farmer, skill);
                farmer.fishingLevel.Value = newLevel;
                farmer.newLevels.Add(new Point((int)skill, newLevel));
            }
            return new LetterInformationAttachment(receivedItem);
        }

        private LetterAttachment SendProgressiveForagingLevel(ReceivedItem receivedItem)
        {
            const Skill skill = Skill.Foraging;
            var apItem = $"{skill} Level";
            var receivedLevels = _archipelago.GetReceivedItemCount(apItem);
            foreach (var farmer in Game1.getAllFarmers())
            {
                var currentLevel = farmer.foragingLevel.Value;
                var newLevel = currentLevel + 1;
                newLevel = Math.Max(0, Math.Min(receivedLevels, newLevel));
                if (newLevel <= currentLevel)
                {
                    continue;
                }

                GiveExperienceToNextLevel(farmer, skill);
                farmer.foragingLevel.Value = newLevel;
                farmer.newLevels.Add(new Point((int)skill, newLevel));
            }
            return new LetterInformationAttachment(receivedItem);
        }

        private LetterAttachment SendProgressiveMiningLevel(ReceivedItem receivedItem)
        {
            const Skill skill = Skill.Mining;
            var apItem = $"{skill} Level";
            var receivedLevels = _archipelago.GetReceivedItemCount(apItem);
            foreach (var farmer in Game1.getAllFarmers())
            {
                var currentLevel = farmer.miningLevel.Value;
                var newLevel = currentLevel + 1;
                newLevel = Math.Max(0, Math.Min(receivedLevels, newLevel));
                if (newLevel <= currentLevel)
                {
                    continue;
                }

                GiveExperienceToNextLevel(farmer, skill);
                farmer.miningLevel.Value = newLevel;
                farmer.newLevels.Add(new Point((int)skill, newLevel));
            }
            return new LetterInformationAttachment(receivedItem);
        }

        private LetterAttachment SendProgressiveCombatLevel(ReceivedItem receivedItem)
        {
            const Skill skill = Skill.Combat;
            var apItem = $"{skill} Level";
            var receivedLevels = _archipelago.GetReceivedItemCount(apItem);
            foreach (var farmer in Game1.getAllFarmers())
            {
                var currentLevel = farmer.combatLevel.Value;
                var newLevel = currentLevel + 1;
                newLevel = Math.Max(0, Math.Min(receivedLevels, newLevel));
                if (newLevel <= currentLevel)
                {
                    continue;
                }

                GiveExperienceToNextLevel(farmer, skill);
                farmer.combatLevel.Value = newLevel;
                farmer.newLevels.Add(new Point((int)skill, newLevel));
            }
            return new LetterInformationAttachment(receivedItem);
        }

        private LetterAttachment SendFarmingMastery(ReceivedItem receivedItem)
        {
            const Skill skill = Skill.Farming;
            foreach (var farmer in Game1.getAllFarmers())
            {
                AddCraftingRecipes(farmer, new[] { "Statue Of Blessings" });
                GiveItemsToFarmer(farmer, "(W)66");
                farmer.stats.Increment(StatKeys.Mastery((int)skill), 1);
            }
            return new LetterInformationAttachment(receivedItem);
        }

        private LetterAttachment SendForagingMastery(ReceivedItem receivedItem)
        {
            const Skill skill = Skill.Foraging;
            foreach (var farmer in Game1.getAllFarmers())
            {
                AddCraftingRecipes(farmer, new[] { "Mystic Tree Seed", "Treasure Totem" });
                farmer.stats.Increment(StatKeys.Mastery((int)skill), 1);
            }
            return new LetterInformationAttachment(receivedItem);
        }

        private LetterAttachment SendFishingMastery(ReceivedItem receivedItem)
        {
            const Skill skill = Skill.Fishing;
            foreach (var farmer in Game1.getAllFarmers())
            {
                AddCraftingRecipes(farmer, new[] { "Challenge Bait" });
                GiveItemsToFarmer(farmer, "(T)AdvancedIridiumRod");
                farmer.stats.Increment(StatKeys.Mastery((int)skill), 1);
            }
            return new LetterInformationAttachment(receivedItem);
        }

        private LetterAttachment SendMiningMastery(ReceivedItem receivedItem)
        {
            const Skill skill = Skill.Mining;
            foreach (var farmer in Game1.getAllFarmers())
            {
                AddCraftingRecipes(farmer, new[] { "Statue Of The Dwarf King", "Heavy Furnace" });
                farmer.stats.Increment(StatKeys.Mastery((int)skill), 1);
            }
            return new LetterInformationAttachment(receivedItem);
        }

        private LetterAttachment SendCombatMastery(ReceivedItem receivedItem)
        {
            const Skill skill = Skill.Combat;
            foreach (var farmer in Game1.getAllFarmers())
            {
                AddCraftingRecipes(farmer, new[] { "Anvil", "Mini-Forge" });
                Game1.player.stats.Set("trinketSlots", 1);
                farmer.stats.Increment(StatKeys.Mastery((int)skill), 1);
            }
            return new LetterInformationAttachment(receivedItem);
        }

        private void AddCraftingRecipes(Farmer farmer, IEnumerable<string> recipes)
        {
            foreach (var recipe in recipes)
            {
                farmer.craftingRecipes.Add(recipe, 0);
            }
        }

        private void GiveItemsToFarmer(Farmer farmer, string itemId)
        {
            if (_archipelago.SlotData.ToolProgression.HasFlag(ToolProgression.Progressive))
            {
                return;
            }

            var item = ItemRegistry.Create(itemId);
            if (!farmer.addItemToInventoryBool(item))
            {
                Game1.createItemDebris(item, Game1.player.getStandingPosition(), 2);
            }
        }

        public void GiveExperienceToNextLevel(Farmer farmer, Skill skill)
        {
            var experienceForLevelUp = farmer.GetExperienceToNextLevel(skill);
            farmer.AddExperience(skill, experienceForLevelUp);
        }
    }
}
