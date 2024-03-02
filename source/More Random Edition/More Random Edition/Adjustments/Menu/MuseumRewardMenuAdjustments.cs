/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Chikakoo/stardew-valley-randomizer
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;
using System.Collections.Generic;
using System.Linq;
using SVItem = StardewValley.Item;
using SVObject = StardewValley.Object;

namespace Randomizer
{
    /// <summary>
    /// Randomizes the rewards the player gets from completing the museum
    /// This sets them to be items in a similar category
    /// Does not include important items: Dwarvish Translation Manual, Ancient Seeds, Stardrop
    /// </summary>
    public class MuseumRewardMenuAdjustments
    {
        /// <summary>
        /// A map of the new id, to the old item so we can quickly look up
        /// what value to pass the OnMuseumRewardGrabbed event
        /// </summary>
        private static Dictionary<string, SVItem> _museumMapNewIdToOldItem;

        /// <summary>
        /// A map of the old id, to the new item so we can quickly look up the new reward
        /// </summary>
        private static Dictionary<string, SVItem> _museumMapOldIdToNewItem;

        /// <summary>
        /// Populates the reward map - this is expected to be called when the save is loaded
        /// Note that this will be called before the menu is ever opened, so we DO want to use the
        /// RNG from before the file initializes
        /// </summary>
        public static void PopulateRewardMap()
        {
            // Reassign these dictionaries so duplicate keys don't get added if a farm is loaded multiple times
            _museumMapNewIdToOldItem = new();
            _museumMapOldIdToNewItem = new();

            WriteToSpoilerLog("==== MUSEUM REWARDS ====");

            var seedPool = ItemList.Items.Values
                .Where(item => 
                    item is SeedItem seedItem && 
                    item.Id != (int)ObjectIndexes.AncientSeeds) // This would clash with the ancient seed artifact reward!
                .Cast<SeedItem>()
                .ToList();

            var springSeedId = Globals.RNGGetRandomValueFromList(seedPool
                .Where(seed => seed.GrowingSeasons.Contains(Seasons.Spring))
                .Select(seed => seed.Id)
                .ToList());
            var summerSeedId = Globals.RNGGetRandomValueFromList(seedPool
                .Where(seed => 
                    seed.Id != springSeedId &&
                    seed.GrowingSeasons.Contains(Seasons.Summer))
                .Select(seed => seed.Id)
                .ToList());
            var fallSeedId = Globals.RNGGetRandomValueFromList(seedPool
                .Where(seed => 
                    seed.Id != springSeedId &&
                    seed.Id != summerSeedId &&
                    seed.GrowingSeasons.Contains(Seasons.Fall))
                .Select(seed => seed.Id)
                .ToList());
            var anySeedId = Globals.RNGGetRandomValueFromList(seedPool
                .Where(seed =>
                    seed.Id != springSeedId &&
                    seed.Id != summerSeedId &&
                    seed.Id != fallSeedId)
                .Select(seed => seed.Id)
                .ToList());

            var springSeed = new SVObject(springSeedId, Range.GetRandomValue(5, 15));
            var summerSeed = new SVObject(summerSeedId, Range.GetRandomValue(5, 15));
            var fallSeed = new SVObject(fallSeedId, Range.GetRandomValue(5, 15));
            var anySeed = new SVObject(anySeedId, Range.GetRandomValue(3, 8));

            var foodItems = Globals.RNGGetRandomValuesFromList(ItemList.GetCookedItems(), 2);
            var food1 = new SVObject(foodItems[0].Id, Range.GetRandomValue(1, 10));
            var food2 = new SVObject(foodItems[1].Id, Range.GetRandomValue(1, 10));

            var bigCraftables = ItemList.GetRandomBigCraftables(4);
            var randomFurniture = ItemList.GetRandomFurniture(16);

            var fIndex = 0;
            var bcIndex = 0;

            var randomTotem = GetSVItem(ItemList.GetRandomTotem().Id);
            var starDrop = GetSVItem((int)ObjectIndexes.Stardrop);

            // Rewards from total donations
            // 5, 10, 15, 20, 25, 30, 35, 40, 50, 60 (skipped), 70, 80, 90, 95 (skipped)
            MapItems(springSeed, GetSVItem((int)ObjectIndexes.CauliflowerSeeds));
            MapItems(summerSeed, GetSVItem((int)ObjectIndexes.MelonSeeds));
            MapItems(anySeed, GetSVItem((int)ObjectIndexes.StarfruitSeeds));
            MapItems(randomFurniture[fIndex++], GetFurnitureItem((int)FurnitureIndexes.ANightOnEcoHill));
            MapItems(randomFurniture[fIndex++], GetFurnitureItem((int)FurnitureIndexes.JadeHills));
            MapItems(randomFurniture[fIndex++], GetFurnitureItem((int)FurnitureIndexes.LgFutanBear));
            MapItems(fallSeed, GetSVItem((int)ObjectIndexes.PumpkinSeeds));
            MapItems(bigCraftables[bcIndex++], GetBigCraftableItem((int)BigCraftableIndexes.Rarecrow8));
            MapItems(randomFurniture[fIndex++], GetFurnitureItem((int)FurnitureIndexes.BearStatue));
            // The 60 reward is the key that's given on the next day
            MapItems(food1, GetSVItem((int)ObjectIndexes.TripleShotEspresso));
            MapItems(randomTotem, GetSVItem((int)ObjectIndexes.WarpTotemFarm));
            MapItems(food2, GetSVItem((int)ObjectIndexes.MagicRockCandy));
            MapItems(starDrop, starDrop, skipLog: true); // We DO NOT want to overwrite the stardrop reward!

            // Rewards from minerals donated
            // 11, 21, 31, 41, 50
            MapItems(randomFurniture[fIndex++], GetFurnitureItem((int)FurnitureIndexes.StandingGeode));
            MapItems(bigCraftables[bcIndex++], GetBigCraftableItem((int)BigCraftableIndexes.SingingStone));
            MapItems(randomFurniture[fIndex++], GetFurnitureItem((int)FurnitureIndexes.ObsidianVase));
            MapItems(randomFurniture[fIndex++], GetFurnitureItem((int)FurnitureIndexes.CrystalChair));
            MapItems(bigCraftables[bcIndex++], GetBigCraftableItem((int)BigCraftableIndexes.Crystalarium));

            // Rewards from artifacts donated
            // 11 (with rare disc/dwarf gadget), 15, 20
            MapItems(randomFurniture[fIndex++], GetFurnitureItem((int)FurnitureIndexes.BurntOffering));
            MapItems(randomFurniture[fIndex++], GetFurnitureItem((int)FurnitureIndexes.Skeleton));
            MapItems(bigCraftables[bcIndex++], GetBigCraftableItem((int)BigCraftableIndexes.Rarecrow7));

            // 3 with ancient drum, and then with bone flute (yes ,these are not actually furniture objects)
            MapItems(randomFurniture[fIndex++], GetSVItem((int)ObjectIndexes.DrumBlock));
            MapItems(randomFurniture[fIndex++], GetSVItem((int)ObjectIndexes.FluteBlock));

            // skip the dwarvish guide since it's important
            var dwarvishTranslationGuide = GetSVItem(326); // We don't track this item, so we'll hard-code it
            MapItems(dwarvishTranslationGuide, dwarvishTranslationGuide, skipLog: true);

            // 5 with chicken statue (the artifact, different from the reward here)
            MapItems(randomFurniture[fIndex++], GetFurnitureItem((int)FurnitureIndexes.ChickenStatue));

            // the three sets of skeleton artifactcs
            MapItems(randomFurniture[fIndex++], GetFurnitureItem((int)FurnitureIndexes.SlothSkeletonL));
            MapItems(randomFurniture[fIndex++], GetFurnitureItem((int)FurnitureIndexes.SlothSkeletonM));
            MapItems(randomFurniture[fIndex++], GetFurnitureItem((int)FurnitureIndexes.SlothSkeletonR));

            WriteToSpoilerLog("");
        }


        /// <summary>
        /// Adjusts the menu rewards - replaces the rewards that would have shown up with ones that
        /// we've remapped
        /// </summary>
        /// <param name="itemGrabMenu">The menu that we'll be adjusting</param>
        public static void AdjustMenu(ItemGrabMenu itemGrabMenu)
        {
            if (!Globals.Config.RandomizeMuseumRewards)
            {
                return;
            }

            // Setting this will cause the menu to execute our code instead when an item is grabbed
            itemGrabMenu.behaviorOnItemGrab = new ItemGrabMenu.behaviorOnItemSelect(OnMuseumRewardGrabbed);

            // This is the UI for the menu
            var itemGrabMenuUI = itemGrabMenu.ItemsToGrabMenu;

            // Get a list of all the items we actually want to be rewards
            List<SVItem> rewardItems = new();
            itemGrabMenuUI.actualInventory.ToList().ForEach(item =>
            {
                // Ancient seeds and their recipe are two separate items here, so we'll just let it add the current reward back
                if (item.ParentSheetIndex == (int)ObjectIndexes.AncientSeeds)
                {
                    rewardItems.Add(item);
                    return;
                }

                var itemKey = GetUniqueItemKey(item);
                if (_museumMapOldIdToNewItem.TryGetValue(itemKey, out SVItem newItem))
                {
                    rewardItems.Add(newItem);
                } 
                else
                {
                    // If the above fails to add the replacement, we'll add the old reward as a backup
                    rewardItems.Add(item);
                }
            });

            // Clear the rewards out and add our own
            itemGrabMenuUI.actualInventory.Clear();
            rewardItems.ForEach(item => itemGrabMenuUI.actualInventory.Add(item));
        }

        /// <summary>
        /// Fires when a reward is grabbed out of the museum reward inventory
        /// We will use our map to fire the original event for this, passing in the original reward
        /// </summary>
        /// <param name="item">The item that was grabbed</param>
        /// <param name="who">The farmer who grabbed it</param>
        private static void OnMuseumRewardGrabbed(SVItem item, Farmer who)
        {
            // Set the item to send to the base game's event
            // - the passed in one if we didn't map it
            // - or the new item that it was mapped to
            var rewardToSendToBaseGame = item;
            var itemKey = GetUniqueItemKey(item);
            if (_museumMapNewIdToOldItem.TryGetValue(itemKey, out SVItem oldItem))
            {
                rewardToSendToBaseGame = oldItem;
            }

            // Send the base game the info so it can handle it as normal so that items are not duped
            (Game1.currentLocation as LibraryMuseum).collectedReward(rewardToSendToBaseGame, who);
        }

        /// <summary>
        /// Takes in the items and constructs both maps for them
        /// </summary>
        /// <param name="newItem">The new item to get as a reward</param>
        /// <param name="oldItem">The old item that was the reward</param>
        /// <param name="skipLog">Whether to not log this item (true if we're not actually remapping this one)</param>
        private static void MapItems(SVItem newItem, SVItem oldItem, bool skipLog = false)
        {
            var newItemKey = GetUniqueItemKey(newItem);
            var oldItemKey = GetUniqueItemKey(oldItem);
            if (!_museumMapNewIdToOldItem.ContainsKey(newItemKey) && !_museumMapOldIdToNewItem.ContainsKey(oldItemKey))
            {
                _museumMapNewIdToOldItem.Add(GetUniqueItemKey(newItem), oldItem);
                _museumMapOldIdToNewItem.Add(GetUniqueItemKey(oldItem), newItem);

                if (!skipLog)
                {
                    WriteToSpoilerLog($"Museum reward: \"{oldItem.Name}\" was changed to \"{newItem.Name}\"");
                }
            }

            else
            {
                Globals.ConsoleError($"Error while randomizing museum: duplicate key found when mapping {oldItem.Name} to {newItem.Name}. Skipping this entry.");
            }
        }
        /// <summary>
        /// Shortcut to the StardewValley.Object constructor
        /// </summary>
        /// <param name="id">The Id</param>
        /// <returns />
        private static SVObject GetSVItem(int id)
        {
            return new SVObject(id, initialStack: 1);
        }

        /// <summary>
        /// Shortcut to the Furniture factory function
        /// </summary>
        /// <param name="id">The Id</param>
        /// <returns />
        private static Furniture GetFurnitureItem(int id)
        {
            return Furniture.GetFurnitureInstance(id);
        }

        /// <summary>
        /// Shortcut to the BigCraftable constructor
        /// </summary>
        /// <param name="id">The Id</param>
        /// <returns />
        private static SVObject GetBigCraftableItem(int id)
        {
            return new SVObject(Vector2.Zero, id);
        }

        /// <summary>
        /// Used to get dictionary keys
        /// This is from Stardew Valley's utility class
        /// </summary>
        /// <param name="item">The item</param>
        /// <returns></returns>
        private static string GetUniqueItemKey(SVItem item)
        {
            // Hard code the stack to 1 becuase we really don't care and it will cause problems looking up the original items
            return Utility.getStandardDescriptionFromItem(item, 1);
        }

        /// <summary>
        /// Writes to the spoiler log if the setting allows it
        /// </summary>
        private static void WriteToSpoilerLog(string textToWrite)
        {
            if (Globals.Config.RandomizeMuseumRewards)
            {
                Globals.SpoilerWrite(textToWrite);
            }
        }
    }
}
