using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using SObject = StardewValley.Object;

namespace CustomCropsDecay
{
    class CustomCropsDecayData
    {
        public List<int> id { get; set; } = new List<int>();
        public List<string> name { get; set; } = new List<string>();
        public List<Category_> category { get; set; } = new List<Category_>();
        public Dictionary<int, int> decayDays { get; set; } = new Dictionary<int, int>();
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum Category_
    {
        Vegetable = SObject.VegetableCategory,
        Fruit = SObject.FruitsCategory,
        Flower = SObject.flowersCategory,
        Gem = SObject.GemCategory,
        Fish = SObject.FishCategory,
        Egg = SObject.EggCategory,
        Milk = SObject.MilkCategory,
        Cooking = SObject.CookingCategory,
        Crafting = SObject.CraftingCategory,
        Mineral = SObject.mineralsCategory,
        Meat = SObject.meatCategory,
        Metal = SObject.metalResources,
        Junk = SObject.junkCategory,
        Syrup = SObject.syrupCategory,
        MonsterLoot = SObject.monsterLootCategory,
        ArtisanGoods = SObject.artisanGoodsCategory,
        Seeds = SObject.SeedsCategory,
        Ring = SObject.ringCategory,
        AnimalGoods = SObject.sellAtPierresAndMarnies,
        Greens = SObject.GreensCategory,
        Artifact = int.MinValue, // Special case
    }
}
