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
using StardewValley.GameData.GarbageCans;
using System.Collections.Generic;
using System.Linq;
using static StardewValley.GameData.QuantityModifier;

namespace Randomizer
{
    public class GarbageCanRandomizer
    {
        /// <summary>
        /// A map of garbage can keys to which NPC probably use them
        /// </summary>
        private static readonly Dictionary<string, List<GiftableNPCIndexes>> GarbageCanMap = new()
        {
            ["Blacksmith"] = new List<GiftableNPCIndexes>() { GiftableNPCIndexes.Clint },
            ["EmilyAndHaley"] = new List<GiftableNPCIndexes>()
            {
                GiftableNPCIndexes.Emily,
                GiftableNPCIndexes.Haley
            },
            ["Evelyn"] = new List<GiftableNPCIndexes>()
            {
                GiftableNPCIndexes.Pierre,
                GiftableNPCIndexes.Evelyn,
                GiftableNPCIndexes.George,
                GiftableNPCIndexes.Alex
            },
            ["JodiAndKent"] = new List<GiftableNPCIndexes>()
            {
                GiftableNPCIndexes.Jodi,
                GiftableNPCIndexes.Kent,
                GiftableNPCIndexes.Sam,
                GiftableNPCIndexes.Vincent
            },
            ["JojaMart"] = new List<GiftableNPCIndexes>()
            {
                GiftableNPCIndexes.Pam,
                GiftableNPCIndexes.Shane,
                GiftableNPCIndexes.Sam
            },
            ["Mayor"] = new List<GiftableNPCIndexes>()
            {
                GiftableNPCIndexes.Lewis,
                GiftableNPCIndexes.Marnie
            },
            ["Museum"] = new List<GiftableNPCIndexes>()
            {
                GiftableNPCIndexes.Penny,
                GiftableNPCIndexes.Jas,
                GiftableNPCIndexes.Vincent
            },
            ["Saloon"] = new List<GiftableNPCIndexes>() { GiftableNPCIndexes.Gus },

            // Potentially include Sandy here, but it seems that this has a 100% chance
            // of giving a CalicoEgg, so we maybe don't want to modify this
            ["DesertFestival"] = new() 
        };

        /// <summary>
        /// Randomizes garbage cans by making it possible for them to drop the
        /// disliked/hated items of npcs that are likely to use them
        /// </summary>
        /// <returns>The replacement data</returns>
        public static GarbageCanData Randomize()
        {
            if (!Globals.Config.RandomizeGarbageCans)
            {
                return null;
            }

            GarbageCanData replacementData = 
                DataLoader.GarbageCans(Game1.content).DeepClone();

            foreach (var garbageCanData in replacementData.GarbageCans)
            {
                string garbageCanKey = garbageCanData.Key;
                GarbageCanEntryData garbageCanEntryData = garbageCanData.Value;

                if (!GarbageCanMap.TryGetValue(garbageCanKey, out var npcIndexes))
                {
#if DEBUG
                    // This message is NOT useful for anyone but developers, so only show in debug mode
					Globals.ConsoleWarn($"Garbage can not mapped: {garbageCanKey}");
#endif
                    continue;
                }

                const double BaseChance = 0.10;
                var dislikedItemIds = 
                    GetNpcGarbageItems(npcIndexes, NPCGiftTasteIndexes.Dislikes);
                var hatedItemIds =
                    GetNpcGarbageItems(npcIndexes, NPCGiftTasteIndexes.Hates);

                if (dislikedItemIds.Any())
                {
                    garbageCanEntryData.Items.Add(
                        CreateGarbageCanItemData(
                            "DislikedItems",
                            dislikedItemIds,
                            chance: BaseChance,
                            isMegaSuccess: true));
                }

                if (hatedItemIds.Any())
                {
                    garbageCanEntryData.Items.Add(
                        CreateGarbageCanItemData(
                            "HatedItems",
                            hatedItemIds,
                            chance: BaseChance,
                            isDoubleMegaSuccess: true));
                }
            }

            return replacementData;
        }

        /// <summary>
        /// Gets a list of qualified ids of all the given npcs for the given taste
        /// Will remove duplicate values
        /// </summary>
        /// <param name="npcs">The npcs to look up gift tatstes for</param>
        /// <param name="npcTaste">The taste</param>
        /// <returns>The list of qualifid ids</returns>
        private static List<string> GetNpcGarbageItems(
            List<GiftableNPCIndexes> npcs, 
            NPCGiftTasteIndexes npcTaste)
        {
            // For each npc, grab all preferences into a list of list
            // Flatten the list, then select only the qualified ids out of it
            return npcs
                .Select(npc => PreferenceRandomizer.GetIndividualPreferences(npc, npcTaste))
                .SelectMany(itemList => itemList)
                .Select(item => item.QualifiedId)
                .Distinct()
                .ToList();
        }

        /// <summary>
        /// Creates a new garbage can item data object
        /// </summary>
        /// <param name="uniqueId">The unique id - gets the mod id preprended to it</param>
        /// <param name="randomItemIds">A list of random item ids that could spawn</param>
        /// <param name="chance">The base chance of the item spawning</param>
        /// <param name="isMegaSuccess">Show the graphical effects for a mega success</param>
        /// <param name="isDoubleMegaSuccess">Show the graphical effects for a double mega success</param>
        /// <returns>The new garbage can data object</returns>
        public static GarbageCanItemData CreateGarbageCanItemData(
            string uniqueId,
            List<string> randomItemIds,
            double chance,
            bool isMegaSuccess = false,
            bool isDoubleMegaSuccess = false)
        {
            return new GarbageCanItemData()
            {
                IgnoreBaseChance = true,
                IsMegaSuccess = isMegaSuccess,
                IsDoubleMegaSuccess = isDoubleMegaSuccess,
                AddToInventoryDirectly = false,
                CreateMultipleDebris = false,
                Condition = $"RANDOM {chance:0.##} @addDailyLuck",
                Id = $"{Globals.ModRef.ModManifest.UniqueID}-{uniqueId}",
                ItemId = null,
                RandomItemId = randomItemIds,
                MaxItems = null,
                MinStack = -1,
                MaxStack = -1,
                Quality = -1,
                ObjectInternalName = null,
                ObjectDisplayName = null,
                ToolUpgradeLevel = -1,
                IsRecipe = false,
                StackModifiers = null,
                StackModifierMode = QuantityModifierMode.Stack,
                QualityModifiers = null,
                QualityModifierMode = QuantityModifierMode.Stack,
                PerItemCondition = null
            };
        }
    }
}
