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
using System.Text;
using System.Threading.Tasks;
using Archipelago.Gifting.Net;
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

        public bool TryCreateGiftItem(Object giftObject, out GiftItem giftItem, out GiftTrait[] traits)
        {
            giftItem = null;
            traits = null;
            if (giftObject == null)
            {
                Game1.chatBox?.addMessage($"You must hold an item in your hand to gift it", Color.Gold);
                return false;
            }
            
            if (!_itemManager.ObjectExists(giftObject.Name) || giftObject.questItem.Value)
            {
                Game1.chatBox?.addMessage($"{giftObject.Name} cannot be gifted to other players", Color.Gold);
                return false;
            }

            giftItem = new GiftItem(giftObject.Name, giftObject.Stack, giftObject.salePrice() * BankHandler.EXCHANGE_RATE);
            traits = GenerateGiftTraits(giftObject);
            return true;
        }

        private GiftTrait[] GenerateGiftTraits(Item giftObject)
        {
            var traits = new List<GiftTrait>();

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
                if (_buffFlags.ContainsKey(i) && buffs[i] > 0)
                {
                    yield return CreateTrait(_buffFlags[i], buffDuration, buffs[i]);
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

            var categoryName = "";
            if (typeAndCategory.Length > 1)
            {
                var category = int.Parse(typeAndCategory[1]);
                if (_categoryFlags.ContainsKey(category))
                {
                    categoryName = _categoryFlags[category];
                    if (!string.IsNullOrWhiteSpace(categoryName))
                    {
                        yield return CreateTrait(categoryName);
                    }
                }
            }

            var type = typeAndCategory[0];
            if (type != categoryName && !string.IsNullOrWhiteSpace(type))
            {
                if (_typeFlags.ContainsKey(type))
                {
                    type = _typeFlags[type];
                }

                if (!string.IsNullOrWhiteSpace(type))
                {
                    yield return CreateTrait(type);
                }
            }
        }

        private GiftTrait CreateTrait(string trait, double duration = 1.0, double quality = 1.0)
        {
            return new GiftTrait(trait, duration, quality);
        }

        private static readonly Dictionary<int, string> _buffFlags = new()
        {
            // { ConsumableBuff.FARMING, GiftFlag.Farming },
            { ConsumableBuff.FISHING, GiftFlag.Fish },
            { ConsumableBuff.MINING, GiftFlag.Tool },
            { ConsumableBuff.DIGGING, GiftFlag.Tool },
            // { ConsumableBuff.LUCK, GiftFlag.Luck },
            // { ConsumableBuff.FORAGING, GiftFlag.Foraging },
            // { ConsumableBuff.CRAFTING, GiftFlag.Crafting },
            { ConsumableBuff.MAX_ENERGY, GiftFlag.Mana },
            // { ConsumableBuff.MAGNETISM, GiftFlag.Magnetism },
            { ConsumableBuff.SPEED, GiftFlag.Speed },
            { ConsumableBuff.DEFENSE, GiftFlag.Armor },
            { ConsumableBuff.ATTACK, GiftFlag.Weapon },
        };

        private static readonly Dictionary<int, string> _categoryFlags = new()
        {
            //{ Category.GEM, GiftFlag.Gem },
            { Category.FISH, GiftFlag.Fish },
            //{ Category.EGG, GiftFlag.Egg },
            //{ Category.MILK, GiftFlag.Milk },
            //{ Category.COOKING, GiftFlag.Cooking },
            //{ Category.CRAFTING, GiftFlag.Crafting },
            //{ Category.BIG_CRAFTABLE, GiftFlag.BigCraftable },
            //{ Category.MINERAL, GiftFlag.Mineral },
            //{ Category.MEAT, GiftFlag.Meat },
            { Category.METAL, GiftFlag.Metal },
            { Category.BUILDING, GiftFlag.Material },
            //{ Category.SELL_AT_PIERRE, GiftFlag.SellAtPierre },
            //{ Category.SELL_AT_PIERRE_AND_MARNIE, GiftFlag.SellAtPierreAndMarnie },
            //{ Category.FERTILIZER, GiftFlag.Fertilizer },
            //{ Category.TRASH, GiftFlag.Trash },
            //{ Category.BAIT, GiftFlag.Bait },
            //{ Category.TACKLE, GiftFlag.Tackle },
            //{ Category.SELL_AT_FISH_SHOP, GiftFlag.SellAtFishShop },
            //{ Category.FURNITURE, GiftFlag.Furniture },
            //{ Category.INGREDIENT, GiftFlag.Ingredient },
            //{ Category.ARTISAN_GOOD, GiftFlag.ArtisanGood },
            //{ Category.SYRUP, GiftFlag.Syrup },
            { Category.MONSTER_LOOT, GiftFlag.Monster },
            { Category.EQUIPMENT, GiftFlag.Armor },
            { Category.SEED, GiftFlag.Seed },
            //{ Category.VEGETABLE, GiftFlag.Vegetable },
            //{ Category.FRUIT, GiftFlag.Fruit },
            //{ Category.FLOWER, GiftFlag.Flower },
            //{ Category.FORAGE, GiftFlag.Forage },
            //{ Category.HAT, GiftFlag.Cosmetic },
            { Category.RING, GiftFlag.Armor },
            { Category.WEAPON, GiftFlag.Weapon },
            { Category.TOOL, GiftFlag.Tool },
        };

        private static readonly Dictionary<string, string> _typeFlags = new()
        {
            { "Arch", "Artifact" },
            { "Basic", "" },
            { "Minerals", "Mineral" },
        };
    }
}
