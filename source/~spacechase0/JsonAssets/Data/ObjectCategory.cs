/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using StardewValley;

namespace JsonAssets.Data
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ObjectCategory
    {
        Vegetable = Object.VegetableCategory,
        Fruit = Object.FruitsCategory,
        Flower = Object.flowersCategory,
        Gem = Object.GemCategory,
        Fish = Object.FishCategory,
        Egg = Object.EggCategory,
        Milk = Object.MilkCategory,
        Cooking = Object.CookingCategory,
        Crafting = Object.CraftingCategory,
        Mineral = Object.mineralsCategory,
        Meat = Object.meatCategory,
        Metal = Object.metalResources,
        Junk = Object.junkCategory,
        Syrup = Object.syrupCategory,
        MonsterLoot = Object.monsterLootCategory,
        ArtisanGoods = Object.artisanGoodsCategory,
        Seeds = Object.SeedsCategory,
        Ring = Object.ringCategory,
        AnimalGoods = Object.sellAtPierresAndMarnies,
        Greens = Object.GreensCategory,
        Artifact = int.MinValue // Special case
    }
}
