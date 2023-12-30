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
using System.Linq;
using Archipelago.Gifting.Net.Gifts;
using Archipelago.Gifting.Net.Traits;
using Microsoft.Xna.Framework;
using StardewArchipelago.Stardew;
using StardewValley;
using Object = StardewValley.Object;

namespace StardewArchipelago.Archipelago.Gifting
{
    public class GiftGenerator
    {
        private const double DEFAULT_BUFF_DURATION = 120;

        private StardewItemManager _itemManager;

        public GiftGenerator(StardewItemManager itemManager)
        {
            _itemManager = itemManager;
        }

        public bool TryCreateGiftItem(Object giftObject, bool isTrap, out GiftItem giftItem, out GiftTrait[] traits, out string failureMessage)
        {
            giftItem = null;
            traits = null;
            var giftOrTrap = isTrap ? "trap" : "gift";
            if (giftObject == null)
            {
                failureMessage = $"You must hold an item in your hand to {giftOrTrap} it";
                return false;
            }

            var name = _itemManager.NormalizeName(giftObject.ParentSheetIndex, giftObject.Name);

            if (!_itemManager.ObjectExists(name) || giftObject.questItem.Value)
            {
                failureMessage = $"{name} cannot be sent to other players";
                return false;
            }


            giftItem = new GiftItem(name, giftObject.Stack, giftObject.salePrice() * BankHandler.EXCHANGE_RATE);
            traits = GenerateGiftTraits(giftObject, isTrap);
            failureMessage = $"";
            return true;
        }

        private GiftTrait[] GenerateGiftTraits(Item giftObject, bool isTrap)
        {
            var traits = new List<GiftTrait>();

            if (isTrap)
            {
                traits.Add(new GiftTrait(GiftFlag.Trap, 1, 1));
            }

            if (!Game1.objectInformation.ContainsKey(giftObject.ParentSheetIndex))
            {
                return traits.ToArray();
            }

            var objectInfo = Game1.objectInformation[giftObject.ParentSheetIndex].Split('/');
            var edibility = objectInfo[2];
            if (Convert.ToInt32(edibility) > 0)
            {
                traits.AddRange(GetConsumableTraits(objectInfo));
            }

            traits.AddRange(GetCategoryTraits(objectInfo));

            return traits.ToArray();
        }

        private IEnumerable<GiftTrait> GetConsumableTraits(string[] objectInfo)
        {
            yield return CreateTrait(GiftFlag.Consumable);
            if (objectInfo.Length < 7)
            {
                yield break;
            }

            var foodOrDrink = objectInfo[6];
            if (foodOrDrink.Equals("food", StringComparison.InvariantCultureIgnoreCase))
            {
                yield return CreateTrait(GiftFlag.Food);
            }

            if (foodOrDrink.Equals("drink", StringComparison.InvariantCultureIgnoreCase))
            {
                yield return CreateTrait(GiftFlag.Drink);
            }

            if (objectInfo.Length < 8)
            {
                yield break;
            }

            var buffs = objectInfo[7].Split(' ').Select(int.Parse).ToArray();
            var buffDuration = (objectInfo.Length > 8 ? double.Parse(objectInfo[8]) : 120) / DEFAULT_BUFF_DURATION;

            foreach (var buffTrait in GetBuffTraits(buffs, buffDuration)) yield return buffTrait;
        }

        private IEnumerable<GiftTrait> GetBuffTraits(int[] buffs, double buffDuration)
        {
            if (buffDuration <= 0)
            {
                yield break;
            }

            for (var i = 0; i < buffs.Length; i++)
            {
                if (!_buffFlags.ContainsKey(i) || buffs[i] <= 0)
                {
                    continue;
                }

                foreach (var buffFlag in _buffFlags[i])
                {
                    yield return CreateTrait(buffFlag, buffDuration, buffs[i]);
                }
            }
        }

        private IEnumerable<GiftTrait> GetCategoryTraits(string[] objectInfo)
        {
            if (objectInfo.Length < 4)
            {
                yield break;
            }

            var typeAndCategory = objectInfo[3].Split(" ");

            if (typeAndCategory.Length < 1)
            {
                yield break;
            }

            var categoryNames = Array.Empty<string>();
            if (typeAndCategory.Length > 1)
            {
                var category = int.Parse(typeAndCategory[1]);
                if (_categoryFlags.ContainsKey(category))
                {
                    categoryNames = _categoryFlags[category];
                    foreach (var categoryName in categoryNames)
                    {
                        if (!string.IsNullOrWhiteSpace(categoryName))
                        {
                            yield return CreateTrait(categoryName);
                        }
                    }
                }
            }

            var type = typeAndCategory[0];
            if (ReplaceFlags.ContainsKey(type))
            {
                type = ReplaceFlags[type];
            }

            if (categoryNames.Contains(type) || string.IsNullOrWhiteSpace(type))
            {
                yield break;
            }

            if (!string.IsNullOrWhiteSpace(type))
            {
                yield return CreateTrait(type);
            }
        }

        private GiftTrait CreateTrait(string trait, double duration = 1.0, double quality = 1.0)
        {
            return new GiftTrait(trait, duration, quality);
        }

        private static readonly Dictionary<int, string[]> _buffFlags = new()
        {
            // { ConsumableBuff.FARMING, new[] {GiftFlag.Farming}},
            { ConsumableBuff.FISHING, new[] { GiftFlag.Fish } },
            { ConsumableBuff.MINING, new[] { GiftFlag.Tool } },
            { ConsumableBuff.DIGGING, new[] { GiftFlag.Tool } },
            // { ConsumableBuff.LUCK, new[] {GiftFlag.Luck}},
            // { ConsumableBuff.FORAGING, new[] {GiftFlag.Foraging}},
            // { ConsumableBuff.CRAFTING, new[] {GiftFlag.Crafting}},
            { ConsumableBuff.MAX_ENERGY, new[] { GiftFlag.Mana } },
            // { ConsumableBuff.MAGNETISM, new[] {GiftFlag.Magnetism}},
            { ConsumableBuff.SPEED, new[] { GiftFlag.Speed } },
            { ConsumableBuff.DEFENSE, new[] { GiftFlag.Armor } },
            { ConsumableBuff.ATTACK, new[] { GiftFlag.Weapon } },
        };

        private static readonly Dictionary<int, string[]> _categoryFlags = new()
        {
            { Category.GEM, new[] { "Gem" } },
            { Category.FISH, new[] { GiftFlag.Fish } },
            { Category.EGG, new[] { GiftFlag.Egg } },
            //{ Category.MILK, new[] {GiftFlag.Milk}},
            { Category.COOKING, new[] { "Cooking" } },
            //{ Category.CRAFTING, new[] {GiftFlag.Crafting}},
            //{ Category.BIG_CRAFTABLE, new[] {GiftFlag.BigCraftable}},
            { Category.MINERAL, new[] { "Mineral" } },
            { Category.MEAT, new[] { GiftFlag.Meat } },
            { Category.METAL, new[] { GiftFlag.Metal } },
            { Category.BUILDING, new[] { GiftFlag.Material } },
            //{ Category.SELL_AT_PIERRE, new[] {GiftFlag.SellAtPierre}},
            //{ Category.SELL_AT_PIERRE_AND_MARNIE, new[] {GiftFlag.SellAtPierreAndMarnie}},
            //{ Category.FERTILIZER, new[] {GiftFlag.Fertilizer}},
            //{ Category.TRASH, new[] {GiftFlag.Trash}},
            //{ Category.BAIT, new[] {GiftFlag.Bait}},
            //{ Category.TACKLE, new[] {GiftFlag.Tackle}},
            //{ Category.SELL_AT_FISH_SHOP, new[] {GiftFlag.SellAtFishShop}},
            //{ Category.FURNITURE, new[] {GiftFlag.Furniture}},
            //{ Category.INGREDIENT, new[] {GiftFlag.Ingredient}},
            //{ Category.ARTISAN_GOOD, new[] {GiftFlag.ArtisanGood}},
            //{ Category.SYRUP, new[] {GiftFlag.Syrup}},
            { Category.MONSTER_LOOT, new[] { GiftFlag.Monster } },
            { Category.EQUIPMENT, new[] { GiftFlag.Armor } },
            { Category.SEED, new[] { GiftFlag.Seed } },
            { Category.VEGETABLE, new[] { GiftFlag.Vegetable } },
            { Category.FRUIT, new[] { GiftFlag.Fruit } },
            { Category.FLOWER, new[] { "Flower" } },
            //{ Category.FORAGE, new[] {GiftFlag.Forage}},
            { Category.HAT, new[] { "Cosmetic", "Hat" } },
            { Category.RING, new[] { GiftFlag.Armor } },
            { Category.WEAPON, new[] { GiftFlag.Weapon } },
            { Category.TOOL, new[] { GiftFlag.Tool } },
        };

        private static readonly Dictionary<string, string> ReplaceFlags = new()
        {
            { "Arch", "Artifact" },
            { "Basic", "" },
            { "Minerals", "Mineral" },
            { "Seeds", GiftFlag.Seed },
        };
    }
}
