/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Goldenrevolver/Stardew-Valley-Mods
**
*************************************************/

namespace HorseOverhaul
{
    using StardewValley;
    using System.Collections.Generic;
    using StardewObject = StardewValley.Object;

    //// TODO ingame horse book OR horse TV? TODO compatibility with butcher mod tv show?

    internal class HorseFoodData : FoodData
    {
        public const int DefaultValue = 10;

        public const int OtherSpecialValue = 15;

        public const int NormalTreeFruitValue = 14;
        public const int ExoticTreeFruitValue = 16;
        public const int RareFruitValue = 22;

        public const int NormalVegetableValue = 10;
        public const int RareVegetableValue = 18;

        public const int CommonBerryValue = 9;

        public const int GrainsValue = 7;

        public const int RareForageValue = 15;
        public const int SpecialForageValue = 11;
        public const int NormalForageValue = 8;
        public const int CommonForageValue = 4;

        public const int ShouldntID = 0;
        public const int HayId = -1;
        public const int FiberId = -2;
        public const int CabbageId = -3;
        public const int OnionId = -4;
        public const int CoffeeID = -5;
        public const int NightshadeID = -6;
        public const int HorseRadishID = -7;
        public const int DislikeID = -8;

        public HorseFoodData(int friendshipOnFeed, string replyOnFeed)
        {
            FriendshipOnFeed = friendshipOnFeed;
            ReplyOnFeed = replyOnFeed;
        }

        public bool IsHorseFood { get => FriendshipOnFeed > 0; }

        public int FriendshipOnFeed { get; set; }

        public string ReplyOnFeed { get; set; }

        // Looking for a snack for your loyal equine companion but it's ignoring the mushrooms you've gathered?
        // Let's find out what your horse won't refuse and, while we are at it, what you shouldn't offer in the first place.
        // Most fruits from around the valley are wonderful treats in moderation, but remember to stay away from rhubarb and peppers.
        // For vegetables you have to be careful of all kinds of cabbages, onions and nightshades, which include potatoes, tomatoes and eggplants,
        // but other than those, there are so many great options like most grains and favorites like carrots and beets.
        // Despite the name, horse radish is also a no go, while normal radish is perfectly fine.
        // Fiddlehead fern and amaranth as well as most flowers are poisonous for horses, but sunflower seeds and dandelions are excellent treats.
        // Of course, you shouldn't feed your horse dairy products, chocolate, coffee or meat of any kind.

        public static readonly Dictionary<int, int> HorseEdibility = new()
        {
            // Other
            { 152, HorseFoodData.NormalForageValue - 2 }, // Seaweed
            { 178, HorseFoodData.HayId }, // Hay, simply set to false, because that would be trivial
            { 245, HorseFoodData.OtherSpecialValue }, // Sugar
            { 253, HorseFoodData.CoffeeID }, // Triple Shot Espresso
            { 395, HorseFoodData.CoffeeID }, // Coffee
            { 431, HorseFoodData.GrainsValue }, // Sunflower Seeds
            { 433, HorseFoodData.CoffeeID }, // Coffee Bean
            { 771, HorseFoodData.FiberId }, // Fiber, simply set to false, because that would be trivial

            // Category -17
            { 417, HorseFoodData.RareFruitValue }, // Sweet Gem Berry
            { 430, HorseFoodData.DislikeID }, // Truffle

            // Mushrooms
            { 257, HorseFoodData.DislikeID }, // Morel
            { 281, HorseFoodData.DislikeID }, // Chanterelle
            { 404, HorseFoodData.DislikeID }, // Common Mushroom
            { 420, HorseFoodData.DislikeID }, // Red Mushroom
            { 422, HorseFoodData.DislikeID }, // Purple Mushroom
            { 851, HorseFoodData.DislikeID }, // Magma Cap

            // Other forage
            { 16, HorseFoodData.HorseRadishID }, // Wild Horseradish
            { 18, HorseFoodData.ShouldntID }, // Daffodil
            { 20, HorseFoodData.OnionId }, // Leek
            { 22, HorseFoodData.NormalForageValue }, // Dandelion
            { 78, HorseFoodData.NormalForageValue + 2 }, // Cave Carrot
            { 92, HorseFoodData.ShouldntID }, // Sap
            { 283, HorseFoodData.ShouldntID }, // Holly
            { 399, HorseFoodData.OnionId }, // Spring Onion
            { 408, HorseFoodData.DislikeID }, // Hazelnut
            { 412, HorseFoodData.SpecialForageValue }, // Winter Root
            { 416, HorseFoodData.SpecialForageValue }, // Snow Yam
            { 829, HorseFoodData.SpecialForageValue }, // Ginger

            // Flowers
            { 376, HorseFoodData.ShouldntID }, // Poppy
            { 402, HorseFoodData.ShouldntID }, // Sweet Pea
            { 418, HorseFoodData.ShouldntID }, // Crocus
            { 421, HorseFoodData.ShouldntID }, // Sunflower
            { 591, HorseFoodData.ShouldntID }, // Tulip
            { 593, HorseFoodData.ShouldntID }, // Summer Spangle
            { 595, HorseFoodData.ShouldntID }, // Fairy Rose
            { 597, HorseFoodData.ShouldntID }, // Blue Jazz

            // Fruits
            { 88, HorseFoodData.SpecialForageValue }, // Coconut
            { 90, HorseFoodData.SpecialForageValue }, // Cactus Fruit
            { 91, HorseFoodData.ExoticTreeFruitValue }, // Banana
            { 252, HorseFoodData.ShouldntID }, // Rhubarb
            { 254, HorseFoodData.RareFruitValue }, // Melon
            { 258, HorseFoodData.CommonBerryValue }, // Blueberry
            { 260, HorseFoodData.NightshadeID }, // Hot Pepper
            { 268, HorseFoodData.RareFruitValue }, // Starfruit
            { 282, HorseFoodData.CommonBerryValue }, // Cranberries
            { 296, HorseFoodData.CommonForageValue / 2 }, // Salmonberry
            { 396, HorseFoodData.NormalForageValue }, // Spice Berry
            { 398, HorseFoodData.NormalForageValue }, // Grape
            { 400, HorseFoodData.CommonBerryValue }, // Strawberry
            { 406, HorseFoodData.NormalForageValue }, // Wild Plum
            { 410, HorseFoodData.CommonForageValue }, // Blackberry
            { 414, HorseFoodData.RareForageValue }, // Crystal Fruit
            { 454, HorseFoodData.RareFruitValue }, // Ancient Fruit
            { 613, HorseFoodData.NormalTreeFruitValue + 4 }, // Apple
            { 634, HorseFoodData.NormalTreeFruitValue }, // Apricot
            { 635, HorseFoodData.NormalTreeFruitValue }, // Orange
            { 636, HorseFoodData.NormalTreeFruitValue }, // Peach
            { 637, HorseFoodData.NormalTreeFruitValue }, // Pomegranate
            { 638, HorseFoodData.NormalTreeFruitValue }, // Cherry
            { 832, HorseFoodData.RareFruitValue }, // Pineapple
            { 834, HorseFoodData.ExoticTreeFruitValue }, // Mango
            { 889, HorseFoodData.DislikeID }, // Qi Fruit

            // Vegetables
            { 24, HorseFoodData.NormalVegetableValue }, // Parsnip
            { 188, HorseFoodData.NormalVegetableValue }, // Green Bean
            { 190, HorseFoodData.CabbageId }, // Cauliflower
            { 192, HorseFoodData.NightshadeID }, // Potato
            { 248, HorseFoodData.OnionId }, // Garlic
            { 250, HorseFoodData.CabbageId }, // Kale
            { 256, HorseFoodData.NightshadeID }, // Tomato
            { 259, HorseFoodData.ShouldntID }, // Fiddlehead Fern
            { 262, HorseFoodData.GrainsValue }, // Wheat
            { 264, HorseFoodData.NormalVegetableValue }, // Radish
            { 266, HorseFoodData.CabbageId }, // Red Cabbage
            { 270, HorseFoodData.GrainsValue }, // Corn
            { 271, HorseFoodData.GrainsValue }, // Unmilled Rice
            { 272, HorseFoodData.NightshadeID }, // Eggplant
            { 274, HorseFoodData.RareVegetableValue }, // Artichoke
            { 276, HorseFoodData.RareVegetableValue }, // Pumpkin
            { 278, HorseFoodData.CabbageId }, // Bok Choy
            { 280, HorseFoodData.RareVegetableValue }, // Yam
            { 284, HorseFoodData.NormalVegetableValue }, // Beet
            { 300, HorseFoodData.ShouldntID }, // Amaranth
            { 304, HorseFoodData.ShouldntID }, // Hops
            { 815, HorseFoodData.NormalVegetableValue }, // Tea Leaves
            { 830, HorseFoodData.ShouldntID }, // Taro Root
        };

        public static string GetReplyForID(int id)
        {
            return id switch
            {
                ShouldntID => "HorseEatShouldnt",
                HayId => "HorseEatHay",
                FiberId => "HorseEatFiber",
                CoffeeID => "HorseEatCoffee",
                CabbageId => "HorseEatCabbage",
                OnionId => "HorseEatOnion",
                NightshadeID => "HorseEatNightshade",
                HorseRadishID => "HorseEatHorseRadish",
                DislikeID => "HorseEatDislike",
                _ => null,
            };
        }

        public static HorseFoodData ClassifyHorseFood(Item itemToFeed)
        {
            if (HorseEdibility.TryGetValue(itemToFeed.ParentSheetIndex, out int result))
            {
                if (result > 0)
                {
                    return new HorseFoodData(result, null);
                }
                else
                {
                    return new HorseFoodData(0, GetReplyForID(result));
                }
            }
            else if (itemToFeed.Category == StardewObject.VegetableCategory || itemToFeed.Category == StardewObject.FruitsCategory || itemToFeed.Category == StardewObject.GreensCategory)
            {
                // filter out some modded cabbage, onions and peppers
                // I can't filter out other potatoes because some crops like "sweet potato" are not actually related to potatoes (or yam, even if they are similar)
                if (itemToFeed?.Name?.ToLower()?.Contains("cabbage") == true || itemToFeed?.DisplayName?.ToLower()?.Contains("cabbage") == true)
                {
                    return new HorseFoodData(0, GetReplyForID(CabbageId));
                }
                else if (itemToFeed?.Name?.ToLower()?.Contains("onion") == true || itemToFeed?.DisplayName?.ToLower()?.Contains("onion") == true)
                {
                    return new HorseFoodData(0, GetReplyForID(OnionId));
                }
                else if (itemToFeed?.Name?.ToLower()?.Contains("pepper") == true || itemToFeed?.DisplayName?.ToLower()?.Contains("pepper") == true)
                {
                    return new HorseFoodData(0, GetReplyForID(NightshadeID));
                }
                else
                {
                    // benefit of the doubt for modded or new update items from these categories. all items of those categories in 1.5.6 are already in the dictionary
                    return new HorseFoodData(DefaultValue, null);
                }
            }
            else
            {
                if (IsDairyProduct(itemToFeed))
                {
                    return new HorseFoodData(0, "LactoseIntolerantHorses");
                }
                else if (IsEdibleMeat(itemToFeed))
                {
                    return new HorseFoodData(0, "HorseHerbivore");
                }
                else if (itemToFeed.ParentSheetIndex == 216)
                {
                    return new HorseFoodData(0, "HorseNoBread");
                }
                else if (IsChocolate(itemToFeed))
                {
                    return new HorseFoodData(0, "DontFeedChocolate");
                }
                else
                {
                    return new HorseFoodData(0, null);
                }
            }
        }
    }
}