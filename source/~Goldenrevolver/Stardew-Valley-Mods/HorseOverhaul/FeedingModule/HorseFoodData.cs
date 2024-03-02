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
        protected const string BreadID = "(O)216";

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

        public static readonly Dictionary<string, int> HorseEdibility = new()
        {
            // Other
            { "(O)152", HorseFoodData.NormalForageValue - 2 }, // Seaweed
            { "(O)178", HorseFoodData.HayId }, // Hay, simply set to false, because that would be trivial
            { "(O)245", HorseFoodData.OtherSpecialValue }, // Sugar
            { "(O)253", HorseFoodData.CoffeeID }, // Triple Shot Espresso
            { "(O)395", HorseFoodData.CoffeeID }, // Coffee
            { "(O)431", HorseFoodData.GrainsValue }, // Sunflower Seeds
            { "(O)433", HorseFoodData.CoffeeID }, // Coffee Bean
            { "(O)771", HorseFoodData.FiberId }, // Fiber, simply set to false, because that would be trivial

            // Category -17
            { "(O)417", HorseFoodData.RareFruitValue }, // Sweet Gem Berry
            { "(O)430", HorseFoodData.DislikeID }, // Truffle

            // Mushrooms
            { "(O)257", HorseFoodData.DislikeID }, // Morel
            { "(O)281", HorseFoodData.DislikeID }, // Chanterelle
            { "(O)404", HorseFoodData.DislikeID }, // Common Mushroom
            { "(O)420", HorseFoodData.DislikeID }, // Red Mushroom
            { "(O)422", HorseFoodData.DislikeID }, // Purple Mushroom
            { "(O)851", HorseFoodData.DislikeID }, // Magma Cap

            // Other forage
            { "(O)16", HorseFoodData.HorseRadishID }, // Wild Horseradish
            { "(O)18", HorseFoodData.ShouldntID }, // Daffodil
            { "(O)20", HorseFoodData.OnionId }, // Leek
            { "(O)22", HorseFoodData.NormalForageValue }, // Dandelion
            { "(O)78", HorseFoodData.NormalForageValue + 2 }, // Cave Carrot
            { "(O)92", HorseFoodData.ShouldntID }, // Sap
            { "(O)283", HorseFoodData.ShouldntID }, // Holly
            { "(O)399", HorseFoodData.OnionId }, // Spring Onion
            { "(O)408", HorseFoodData.DislikeID }, // Hazelnut
            { "(O)412", HorseFoodData.SpecialForageValue }, // Winter Root
            { "(O)416", HorseFoodData.SpecialForageValue }, // Snow Yam
            { "(O)829", HorseFoodData.SpecialForageValue }, // Ginger

            // Flowers
            { "(O)376", HorseFoodData.ShouldntID }, // Poppy
            { "(O)402", HorseFoodData.ShouldntID }, // Sweet Pea
            { "(O)418", HorseFoodData.ShouldntID }, // Crocus
            { "(O)421", HorseFoodData.ShouldntID }, // Sunflower
            { "(O)591", HorseFoodData.ShouldntID }, // Tulip
            { "(O)593", HorseFoodData.ShouldntID }, // Summer Spangle
            { "(O)595", HorseFoodData.ShouldntID }, // Fairy Rose
            { "(O)597", HorseFoodData.ShouldntID }, // Blue Jazz

            // Fruits
            { "(O)88", HorseFoodData.SpecialForageValue }, // Coconut
            { "(O)90", HorseFoodData.SpecialForageValue }, // Cactus Fruit
            { "(O)91", HorseFoodData.ExoticTreeFruitValue }, // Banana
            { "(O)252", HorseFoodData.ShouldntID }, // Rhubarb
            { "(O)254", HorseFoodData.RareFruitValue }, // Melon
            { "(O)258", HorseFoodData.CommonBerryValue }, // Blueberry
            { "(O)260", HorseFoodData.NightshadeID }, // Hot Pepper
            { "(O)268", HorseFoodData.RareFruitValue }, // Starfruit
            { "(O)282", HorseFoodData.CommonBerryValue }, // Cranberries
            { "(O)296", HorseFoodData.CommonForageValue / 2 }, // Salmonberry
            { "(O)396", HorseFoodData.NormalForageValue }, // Spice Berry
            { "(O)398", HorseFoodData.NormalForageValue }, // Grape
            { "(O)400", HorseFoodData.CommonBerryValue }, // Strawberry
            { "(O)406", HorseFoodData.NormalForageValue }, // Wild Plum
            { "(O)410", HorseFoodData.CommonForageValue }, // Blackberry
            { "(O)414", HorseFoodData.RareForageValue }, // Crystal Fruit
            { "(O)454", HorseFoodData.RareFruitValue }, // Ancient Fruit
            { "(O)613", HorseFoodData.NormalTreeFruitValue + 4 }, // Apple
            { "(O)634", HorseFoodData.NormalTreeFruitValue }, // Apricot
            { "(O)635", HorseFoodData.NormalTreeFruitValue }, // Orange
            { "(O)636", HorseFoodData.NormalTreeFruitValue }, // Peach
            { "(O)637", HorseFoodData.NormalTreeFruitValue }, // Pomegranate
            { "(O)638", HorseFoodData.NormalTreeFruitValue }, // Cherry
            { "(O)832", HorseFoodData.RareFruitValue }, // Pineapple
            { "(O)834", HorseFoodData.ExoticTreeFruitValue }, // Mango
            { "(O)889", HorseFoodData.DislikeID }, // Qi Fruit

            // Vegetables
            { "(O)24", HorseFoodData.NormalVegetableValue }, // Parsnip
            { "(O)188", HorseFoodData.NormalVegetableValue }, // Green Bean
            { "(O)190", HorseFoodData.CabbageId }, // Cauliflower
            { "(O)192", HorseFoodData.NightshadeID }, // Potato
            { "(O)248", HorseFoodData.OnionId }, // Garlic
            { "(O)250", HorseFoodData.CabbageId }, // Kale
            { "(O)256", HorseFoodData.NightshadeID }, // Tomato
            { "(O)259", HorseFoodData.ShouldntID }, // Fiddlehead Fern
            { "(O)262", HorseFoodData.GrainsValue }, // Wheat
            { "(O)264", HorseFoodData.NormalVegetableValue }, // Radish
            { "(O)266", HorseFoodData.CabbageId }, // Red Cabbage
            { "(O)270", HorseFoodData.GrainsValue }, // Corn
            { "(O)271", HorseFoodData.GrainsValue }, // Unmilled Rice
            { "(O)272", HorseFoodData.NightshadeID }, // Eggplant
            { "(O)274", HorseFoodData.RareVegetableValue }, // Artichoke
            { "(O)276", HorseFoodData.RareVegetableValue }, // Pumpkin
            { "(O)278", HorseFoodData.CabbageId }, // Bok Choy
            { "(O)280", HorseFoodData.RareVegetableValue }, // Yam
            { "(O)284", HorseFoodData.NormalVegetableValue }, // Beet
            { "(O)300", HorseFoodData.ShouldntID }, // Amaranth
            { "(O)304", HorseFoodData.ShouldntID }, // Hops
            { "(O)815", HorseFoodData.NormalVegetableValue }, // Tea Leaves
            { "(O)830", HorseFoodData.ShouldntID }, // Taro Root
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
            if (HorseEdibility.TryGetValue(itemToFeed.QualifiedItemId, out int result))
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
                else if (itemToFeed.QualifiedItemId == BreadID)
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