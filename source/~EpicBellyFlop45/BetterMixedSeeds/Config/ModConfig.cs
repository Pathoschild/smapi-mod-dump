using BetterMixedSeeds.Config;
using System.Collections.Generic;

namespace BetterMixedSeeds
{
    public class ModConfig
    {
        public int PercentDropChanceForMixedSeedsWhenNotFiber { get; set; } = 5;

        public CropMod StardewValley { get; set; } = new CropMod
        (
            spring: new Season(
                new List<Crop> {
                    new Crop("Ancient Fruit", true, 1),
                    new Crop("Blue Jazz", true, 1),
                    new Crop("Cauliflower", true, 1),
                    new Crop("Coffee Bean", true, 1),
                    new Crop("Garlic", true, 1),
                    new Crop("Green Bean", true, 1),
                    new Crop("Kale", true, 1),
                    new Crop("Parsnip", true, 1),
                    new Crop("Potato", true, 1),
                    new Crop("Rhubarb", true, 1),
                    new Crop("Strawberry", true, 1),
                    new Crop("Tulip", true, 1)
                }),
            summer: new Season(
                new List<Crop> {
                    new Crop("Ancient Fruit", true, 1),
                    new Crop("Blueberry", true, 1),
                    new Crop("Corn", true, 1),
                    new Crop("Hops", true, 1),
                    new Crop("Hot Pepper", true, 1),
                    new Crop("Melon", true, 1),
                    new Crop("Poppy", true, 1),
                    new Crop("Radish", true, 1),
                    new Crop("Red Cabbage", true, 1),
                    new Crop("Starfruit", true, 1),
                    new Crop("Summer Spangle", true, 1),
                    new Crop("Sunflower", true, 1),
                    new Crop("Tomato", true, 1),
                    new Crop("Wheat", true, 1)
                }),
            fall: new Season(
                new List<Crop> {
                    new Crop("Ancient Fruit", true, 1),
                    new Crop("Amaranth", true, 2),
                    new Crop("Artichoke", true, 1),
                    new Crop("Beet", true, 1),
                    new Crop("Bok Choy", true, 1),
                    new Crop("Corn", true, 1),
                    new Crop("Cranberries", true, 1),
                    new Crop("Eggplant", true, 1),
                    new Crop("Fairy Rose", true, 1),
                    new Crop("Grape", true, 1),
                    new Crop("Pumpkin", true, 1),
                    new Crop("Sunflower", true, 1),
                    new Crop("Sweet Gem Berry", true, 1),
                    new Crop("Wheat", true, 1),
                    new Crop("Yam", true, 1)
                }),
            winter: null
        );

        public CropMod FantasyCrops { get; set; } = new CropMod
        (
            spring: new Season(
                new List<Crop>
                {
                    new Crop("Coal Root", true, 1),
                    new Crop("Copper Leaf", true, 1)
                }),
            summer: new Season(
                new List<Crop>
                {
                    new Crop("Coal Root", true, 1),
                    new Crop("Iron Leaf", true, 1)
                }),
            fall: new Season(
                new List<Crop>
                {
                    new Crop("Gold Leaf", true, 1),
                    new Crop("Money Plant", true, 1)
                }),
            winter: new Season(
                new List<Crop>
                {
                    new Crop("Iridium Fern", true, 1)
                })
        );

        public CropMod FreshMeat { get; set; } = new CropMod
        (
            spring: new Season(
                new List<Crop>
                {
                    new Crop("Beef", true, 1),
                    new Crop("Chicken", true, 1),
                    new Crop("Mutton", true, 1),
                    new Crop("Rabbit", true, 1)
                }),
            summer: new Season(
                new List<Crop>
                {
                    new Crop("Beef", true, 1),
                    new Crop("Chevon", true, 1),
                    new Crop("Chicken", true, 1),
                    new Crop("Duck", true, 1),
                    new Crop("Mutton", true, 1),
                    new Crop("Pork", true, 1),
                    new Crop("Rabbit", true, 1)
                }),
            fall: new Season(
                new List<Crop>
                {
                    new Crop("Chevon", true, 1),
                    new Crop("Duck", true, 1),
                    new Crop("Pork", true, 1)
                }),
            winter: null
        );

        public CropMod FruitAndVeggies { get; set; } = new CropMod
        (
            spring: new Season(
                new List<Crop>
                {
                    new Crop("Basil", true, 1),
                    new Crop("Cabbage", true, 1),
                    new Crop("Muskmelon", true, 1),
                    new Crop("Onion", true, 1),
                    new Crop("Parsley", true, 1),
                    new Crop("Passion Fruit", true, 1),
                    new Crop("Pineapple", true, 1),
                    new Crop("Rice", true, 1),
                    new Crop("Spinach", true, 1),
                    new Crop("Sugar Beet", true, 1),
                    new Crop("Sweet Canary Melon", true, 1),
                    new Crop("Tea", true, 1)
                }),
            summer: new Season(
                new List<Crop>
                {
                    new Crop("Adzuki Bean", true, 1),
                    new Crop("Aloe", true, 1),
                    new Crop("Cassava", true, 1),
                    new Crop("Chives", true, 1),
                    new Crop("Cotton", true, 1),
                    new Crop("Cucumber", true, 1),
                    new Crop("Gooseberry", true, 1),
                    new Crop("Green Pea", true, 1),
                    new Crop("Kiwi", true, 1),
                    new Crop("Lettuce", true, 1),
                    new Crop("Navy Bean", true, 1),
                    new Crop("Oregano", true, 1),
                    new Crop("Raspberry", true, 1),
                    new Crop("Rice", true, 1),
                    new Crop("Sugar Cane", true, 1),
                    new Crop("Tea", true, 1),
                    new Crop("Wasabi", true, 1)
                }),
            fall: new Season(
                new List<Crop>
                {
                    new Crop("Barley", true, 1),
                    new Crop("Bell Pepper", true, 1),
                    new Crop("Blackberry", true, 1),
                    new Crop("Broccoli", true, 1),
                    new Crop("Cabbage", true, 1),
                    new Crop("Carrot", true, 1),
                    new Crop("Celery", true, 1),
                    new Crop("Cotton", true, 1),
                    new Crop("Fennel", true, 1),
                    new Crop("Ginger", true, 1),
                    new Crop("Kiwi", true, 1),
                    new Crop("Peanut", true, 1),
                    new Crop("Rice", true, 1),
                    new Crop("Rosemary", true, 1),
                    new Crop("Sage", true, 1),
                    new Crop("Soybean", true, 1),
                    new Crop("Spinach", true, 1),
                    new Crop("Sweet Potato", true, 1),
                    new Crop("Tea", true, 1),
                    new Crop("Thyme", true, 1),
                    new Crop("Watermelon Mizu", true, 1)
                }),
            winter: new Season(
                new List<Crop>
                {
                    new Crop("Elderberry", true, 1),
                    new Crop("Juniper", true, 1),
                    new Crop("Mint", true, 1)
                })
        );

        public CropMod MizusFlowers { get; set; } = new CropMod
        (
            spring: new Season(
                new List<Crop>
                {
                    new Crop("Chamomile", true, 1),
                    new Crop("Honeysuckle", true, 1),
                    new Crop("Pink Cat", true, 1),
                    new Crop("Rose", true, 1),
                    new Crop("Shaded Violet", true, 1),
                    new Crop("Spring Rose", true, 1)
                }),
            summer: new Season(
                new List<Crop>
                {
                    new Crop("Blue Mist", true, 1),
                    new Crop("Clary Sage", true, 1),
                    new Crop("Fragrant Lilac", true, 1),
                    new Crop("Herbal Lavender", true, 1),
                    new Crop("Honeysuckle", true, 1),
                    new Crop("Passion Flower", true, 1),
                    new Crop("Rose", true, 1),
                    new Crop("Summer Rose", true, 1)
                }),
            fall: new Season(
                new List<Crop>
                {
                    new Crop("Bee Balm", true, 1),
                    new Crop("Fairy Duster", true, 1),
                    new Crop("Fall Rose", true, 1),
                    new Crop("Purple Coneflower", true, 1),
                    new Crop("Rose", true, 1),
                    new Crop("Sweet Jasmine", true, 1)
                }),
            winter: null
        );

        public CropMod CannabisKit { get; set; } = new CropMod
        (
            spring: new Season(
                new List<Crop>
                {
                    new Crop("Cannabis", true, 1),
                    new Crop("Hemp", true, 1),
                    new Crop("Tobacco", true, 1)
                }),
            summer: new Season(
                new List<Crop>
                {
                    new Crop("Blue Dream", true, 1),
                    new Crop("Cannabis", true, 1),
                    new Crop("Girl Scout Cookies", true, 1),
                    new Crop("Green Crack", true, 1),
                    new Crop("Hemp", true, 1),
                    new Crop("Hybrid", true, 1),
                    new Crop("Indica", true, 1),
                    new Crop("Northern Lights", true, 1),
                    new Crop("OG Kush", true, 1),
                    new Crop("Strawberry Cough", true, 1),
                    new Crop("Tobacco", true, 1),
                    new Crop("White Widow", true, 1)
                }),
            fall: new Season(
                new List<Crop>
                {
                    new Crop("Blue Dream", true, 1),
                    new Crop("Cannabis", true, 1),
                    new Crop("Girl Scout Cookies", true, 1),
                    new Crop("Green Crack", true, 1),
                    new Crop("Hemp", true, 1),
                    new Crop("Hybrid", true, 1),
                    new Crop("OG Kush", true, 1),
                    new Crop("Sativa", true, 1),
                    new Crop("Sour Diesel", true, 1),
                    new Crop("Strawberry Cough", true, 1),
                    new Crop("White Widow", true, 1)
                }),
            winter: null
        );

        public CropMod SixPlantableCrops { get; set; } = new CropMod
        (
            spring: null,
            summer: null,
            fall: null,
            winter: new Season(
                new List<Crop>
                {
                    new Crop("Blue Rose", true, 1),
                    new Crop("Daikon", true, 1),
                    new Crop("Gentian", true, 1),
                    new Crop("Napa Cabbage", true, 1),
                    new Crop("Snowdrop", true, 1),
                    new Crop("Winter Broccoli", true, 1)
                })
        );

        public CropMod BonsterCrops { get; set; } = new CropMod
        (
            spring: new Season(
                new List<Crop>
                {
                    new Crop("Cranberry Bean", true, 1),
                    new Crop("Red Currant", true, 1),
                    new Crop("Rose Hip", true, 1)
                }),
            summer: new Season(
                new List<Crop>
                {
                    new Crop("Blackcurrant", true, 1),
                    new Crop("Blue Corn", true, 1),
                    new Crop("Cardamom", true, 1),
                    new Crop("Maypop", true, 1),
                    new Crop("Peppercorn", true, 1),
                    new Crop("Rose Hip", true, 1),
                    new Crop("Roselle Hibiscus", true, 1),
                    new Crop("Summer Squash", true, 1),
                    new Crop("Taro", true, 1)
                }),
            fall: new Season(
                new List<Crop>
                {
                    new Crop("Blue Corn", true, 1),
                    new Crop("Peppercorn", true, 1),
                    new Crop("Taro", true, 1),
                    new Crop("White Currant", true, 1)
                }),
            winter: null
        );

        public CropMod RevenantCrops { get; set; } = new CropMod
        (
            spring: new Season(
                new List<Crop>
                {
                    new Crop("Enoki Mushroom", true, 1)
                }),
            summer: new Season(
                new List<Crop>
                {
                    new Crop("Enoki Mushroom", true, 1)
                }),
            fall: new Season(
                new List<Crop>
                {
                    new Crop("Maitake Mushroom", true, 1),
                    new Crop("Oyster Mushroom", true, 1)
                }),
            winter: new Season(
                new List<Crop>
                {
                    new Crop("Gai Lan", true, 1)
                })
        );

        public CropMod FarmerToFlorist { get; set; } = new CropMod
        (
            spring: new Season(
                new List<Crop>
                {
                    new Crop("Camellia", true, 1),
                    new Crop("Carnation", true, 1),
                    new Crop("Delphinium", true, 1),
                    new Crop("Herbal Peony", true, 1),
                    new Crop("Hyacinth", true, 1),
                    new Crop("Lilac", true, 1),
                    new Crop("Violet", true, 1),
                    new Crop("Wisteria", true, 1)
                }),
            summer: new Season(
                new List<Crop>
                {
                    new Crop("Allium", true, 1),
                    new Crop("Carnation", true, 1),
                    new Crop("Hydrangea", true, 1),
                    new Crop("Lavender", true, 1),
                    new Crop("Lily", true, 1),
                    new Crop("Lotus", true, 1),
                    new Crop("Petunia", true, 1)
                }),
            fall: new Season(
                new List<Crop>
                {
                    new Crop("Camellia", true, 1),
                    new Crop("Chrysanthemum", true, 1),
                    new Crop("Clematis", true, 1),
                    new Crop("Dahlia", true, 1),
                    new Crop("English Rose", true, 1),
                    new Crop("Freesia", true, 1),
                    new Crop("Geranium", true, 1),
                    new Crop("Iris", true, 1)
                }),
            winter: null
        );

        public CropMod LuckyClover { get; set; } = new CropMod
        (
            spring: new Season(
                new List<Crop>
                {
                    new Crop("Lucky Clover", true, 1)
                }),
            summer: null,
            fall: null,
            winter: null
        );

        public CropMod FishsFlowers { get; set; } = new CropMod
        (
            spring: new Season(
                new List<Crop>
                {
                    new Crop("Hyacinth", true, 1),
                    new Crop("Pansy", true, 1)
                }),
            summer: new Season(
                new List<Crop>
                {
                    new Crop("Pansy", true, 1)
                }),
            fall: new Season(
                new List<Crop>
                {
                    new Crop("Pansy", true, 1)
                }),
            winter: null
        );

        public CropMod StephansLotsOfCrops { get; set; } = new CropMod
        (
            spring: new Season(
                new List<Crop>
                {
                    new Crop("Cucumber", true, 1),
                    new Crop("Pea Pod", true, 1),
                    new Crop("Turnip", true, 1)
                }),
            summer: new Season(
                new List<Crop>
                {
                    new Crop("Onion", true, 1),
                    new Crop("Pineapple", true, 1),
                    new Crop("Watermelon", true, 1)
                }),
            fall: new Season(
                new List<Crop>
                {
                    new Crop("Carrot", true, 1),
                    new Crop("Peanut", true, 1),
                    new Crop("Spinach", true, 1)
                }),
            winter: null
        );

        public CropMod EemiesCrops { get; set; } = new CropMod
        (
            spring: null,
            summer: new Season(
                new List<Crop>
                {
                    new Crop("Cantaloupe Melon", true, 1),
                    new Crop("Charentais Melon", true, 1),
                    new Crop("Korean Melon", true, 1),
                    new Crop("Large Watermelon", true, 1),
                    new Crop("Rich Canary Melon", true, 1),
                    new Crop("Rich Sweetness Melon", true, 1)
                }),
            fall: new Season(
                new List<Crop>
                {
                    new Crop("Acorn Squash", true, 1),
                    new Crop("Black Forest Squash", true, 1),
                    new Crop("Crookneck Squash", true, 1),
                    new Crop("Golden Hubbard Squash", true, 1),
                    new Crop("Jack O Lantern Pumpkin", true, 1),
                    new Crop("Sweet Lightning Pumpkin", true, 1)
                }),
            winter: null
        );

        public CropMod TeaTime { get; set; } = new CropMod
        (
            spring: new Season(
                new List<Crop>
                {
                    new Crop("Mint Tea Plant", true, 1),
                    new Crop("Tea Leaf Plant", true, 1)
                }),
            summer: new Season(
                new List<Crop>
                {
                    new Crop("Mint Tea Plant", true, 1),
                    new Crop("Tea Leaf Plant", true, 1)
                }),
            fall: new Season(
                new List<Crop>
                {
                    new Crop("Mint Tea Plant", true, 1),
                    new Crop("Tea Leaf Plant", true, 1)
                }),
            winter: null
        );

        public CropMod ForageToFarm { get; set; } = new CropMod
        (
            spring: new Season(
                new List<Crop>
                {
                    new Crop("Cave Carrot", true, 1),
                    new Crop("Common Mushroom", true, 1),
                    new Crop("Daffodil", true, 1),
                    new Crop("Dandelion", true, 1),
                    new Crop("Wild Horseradish", true, 1),
                    new Crop("Leek", true, 1),
                    new Crop("Morel Mushroom", true, 1),
                    new Crop("Salmonberry", true, 1),
                    new Crop("Spring Onion", true, 1)
                }),
            summer: new Season(
                new List<Crop>
                {
                    new Crop("Cave Carrot", true, 1),
                    new Crop("Coconut", true, 1),
                    new Crop("Fiddlehead Fern", true, 1),
                    new Crop("Red Mushroom", true, 1),
                    new Crop("Spice Berry", true, 1),
                    new Crop("Sweet Pea", true, 1)
                }),
            fall: new Season(
                new List<Crop>
                {
                    new Crop("Cave Carrot", true, 1),
                    new Crop("Chanterelle Mushroom", true, 1),
                    new Crop("Common Mushroom", true, 1),
                    new Crop("Hazelnut", true, 1),
                    new Crop("Purple Mushroom", true, 1),
                    new Crop("Red Mushroom", true, 1),
                    new Crop("Wild Blackberry", true, 1),
                    new Crop("Wild Plum", true, 1)
                }),
            winter: new Season(
                new List<Crop>
                {
                    new Crop("Crocus", true, 1),
                    new Crop("Crystal Fruit", true, 1),
                    new Crop("Holly", true, 1),
                    new Crop("Snow Yam", true, 1),
                    new Crop("Winter Root", true, 1)
                })
        );

        public CropMod GemAndMineralCrops { get; set; } = new CropMod
        (
            spring: null,
            summer: null,
            fall: null,
            winter: new Season(
                new List<Crop>
                {
                    new Crop("Aerinite Root", true, 1),
                    new Crop("Aquamarine", true, 1),
                    new Crop("Celestine Flower", true, 1),
                    new Crop("Diamond Flower", true, 1),
                    new Crop("Ghost Rose", true, 1),
                    new Crop("Kyanite Flower", true, 1),
                    new Crop("Opal Cat", true, 1),
                    new Crop("Slate Bean", true, 1),
                    new Crop("Soap Root", true, 1)
                })
        );

        public CropMod MouseEarCress { get; set; } = new CropMod
        (
            spring: new Season(
                new List<Crop>
                {
                    new Crop("Mouse-Ear Cress", true, 1)
                }),
            summer: new Season(
                new List<Crop>
                {
                    new Crop("Mouse-Ear Cress", true, 1)
                }),
            fall: new Season(
                new List<Crop>
                {
                    new Crop("Mouse-Ear Cress", true, 1)
                }),
            winter: null
        );

        public CropMod AncientCrops { get; set; } = new CropMod
        (
            spring: new Season(
                new List<Crop>
                {
                    new Crop("Ancient Coffee Plant", true, 1),
                    new Crop("Ancient Fern", true, 1),
                    new Crop("Ancient Flower", true, 1),
                    new Crop("Ancient Nut", true, 1),
                    new Crop("Ancient Olive Plant", true, 1),
                    new Crop("Ancient Tuber", true, 1)
                }),
            summer: new Season(
                new List<Crop>
                {
                    new Crop("Ancient Coffee Plant", true, 1),
                    new Crop("Ancient Fern", true, 1),
                    new Crop("Ancient Nut", true, 1),
                    new Crop("Ancient Olive Plant", true, 1),
                    new Crop("Ancient Tuber", true, 1)
                }),
            fall: new Season(
                new List<Crop>
                {
                    new Crop("Ancient Coffee Plant", true, 1),
                    new Crop("Ancient Fern", true, 1),
                    new Crop("Ancient Nut", true, 1),
                    new Crop("Ancient Olive Plant", true, 1),
                    new Crop("Ancient Tuber", true, 1)
                }),
            winter: null
        );
    }
}
