using System.Collections.Generic;

namespace BetterMixedSeeds.Data
{
    /// <summary>A list of integrated mods containing all the seed data.</summary>
    public class CropModData
    {
        /// <summary>The list of seed data for Stardew Valley.</summary>
        public static List<SeedData> StardewValley { get; } = new List<SeedData>
        {
            new SeedData("Amaranth", 299),
            new SeedData("Ancient Fruit", 499),
            new SeedData("Artichoke", 489, yearRequirement: 2),
            new SeedData("Beet", 494),
            new SeedData("Blue Jazz", 429),
            new SeedData("Blueberry", 481),
            new SeedData("Bok Choy", 491),
            new SeedData("Cauliflower", 474),
            new SeedData("Coffee Bean", 433),
            new SeedData("Corn", 487),
            new SeedData("Cranberries", 493),
            new SeedData("Eggplant", 488),
            new SeedData("Fairy Rose", 425),
            new SeedData("Fall Seeds", 497),
            new SeedData("Garlic", 476, yearRequirement: 2),
            new SeedData("Grape", 301),
            new SeedData("Green Bean", 473),
            new SeedData("Hops", 302),
            new SeedData("Hot Pepper", 482),
            new SeedData("Kale", 477),
            new SeedData("Melon", 479),
            new SeedData("Parsnip", 472),
            new SeedData("Poppy", 453),
            new SeedData("Potato", 475),
            new SeedData("Pumpkin", 490),
            new SeedData("Radish", 484),
            new SeedData("Red Cabbage", 485, yearRequirement: 2),
            new SeedData("Rhubarb", 478),
            new SeedData("Rice", 273),
            new SeedData("Spring Seeds", 495),
            new SeedData("Starfruit", 486),
            new SeedData("Strawberry", 745),
            new SeedData("Summer Seeds", 496),
            new SeedData("Summer Spangle", 455),
            new SeedData("Sunflower", 431),
            new SeedData("Sweet Gem Berry", 347),
            new SeedData("Tomato", 480),
            new SeedData("Tulip", 427),
            new SeedData("Wheat", 483),
            new SeedData("Winter Seeds", 498),
            new SeedData("Yam", 492)
        };

        /// <summary>The list of seed data for Fantasy Crop.</summary>
        public static List<SeedData> FantasyCrops { get; } = new List<SeedData>
        {
            new SeedData("Coal Root", seedName: "Coal Seeds"),
            new SeedData("Copper Leaf", seedName: "Copper Seeds"),
            new SeedData("Gold Leaf", seedName: "Gold Seeds"),
            new SeedData("Iridium Fern", seedName: "Iridium Seeds"),
            new SeedData("Iron Leaf", seedName: "Iron Seeds"),
            new SeedData("Money Plant", seedName: "Doubloon Seeds")
        };

        /// <summary>The list of seed data for Fresh Meat.</summary>
        public static List<SeedData> FreshMeat { get; } = new List<SeedData>
        {
            new SeedData("Beef", seedName: "Beefvine Seeds"),
            new SeedData("Chevon", seedName: "Chevonvine Seeds"),
            new SeedData("Chicken", seedName: "Chickenvine Seeds"),
            new SeedData("Duck", seedName: "Duckvine Seeds"),
            new SeedData("Mutton", seedName: "Muttonvine Seeds"),
            new SeedData("Pork", seedName: "Porkvine Seeds"),
            new SeedData("Rabbit", seedName: "Rabbitvine Seeds")
        };

        /// <summary>The list of seed data for Fruits and Veggies.</summary>
        public static List<SeedData> FruitAndVeggies { get; } = new List<SeedData>
        {
            new SeedData("Adzuki Bean", seedName: "Adzuki Bean Seeds"),
            new SeedData("Aloe", seedName: "Aloe Pod", yearRequirement: 2),
            new SeedData("Asparagus", seedName: "Asparagus Seeds", yearRequirement: 4),
            new SeedData("Bamboo", seedName: "Bamboo Cutting", yearRequirement: 4),
            new SeedData("Barley", seedName: "Barley Seeds", yearRequirement: 2),
            new SeedData("Basil", seedName: "Basil Seeds"),
            new SeedData("Bell Pepper", seedName: "Bell Pepper Seeds"),
            new SeedData("Blackberry", seedName: "Blackberry Seeds", yearRequirement: 2),
            new SeedData("Blue Agave", seedName: "Blue Agave Seeds", yearRequirement: 3),
            new SeedData("Broccoli", seedName: "Broccoli Seeds", yearRequirement: 2),
            new SeedData("Butternut Squash", seedName: "Butternut Squash Seeds", yearRequirement: 3),
            new SeedData("Cabbage", seedName: "Cabbage Seeds"),
            new SeedData("Cactus Flower", seedName: "Cactus Flower Seeds", yearRequirement: 3),
            new SeedData("Carrot", seedName: "Carrot Seeds"),
            new SeedData("Cassava", seedName: "Cassava Seeds", yearRequirement: 2),
            new SeedData("Celery", seedName: "Celery Seeds", yearRequirement: 2),
            new SeedData("Chickpea", seedName: "Chickpea Seeds", yearRequirement: 4),
            new SeedData("Chives", seedName: "Chive Seeds"),
            new SeedData("Cotton", seedName: "Cotton Seeds"),
            new SeedData("Cucumber", seedName: "Cucumber Starter", yearRequirement: 2),
            new SeedData("Durum", seedName: "Durum Seeds", yearRequirement: 4),
            new SeedData("Elderberry", seedName: "Elderberry Seeds", yearRequirement: 2),
            new SeedData("Fennel", seedName: "Fennel Seeds", yearRequirement: 2),
            new SeedData("Ginger", seedName: "Ginger Seeds"),
            new SeedData("Gooseberry", seedName: "Gooseberry Seeds", yearRequirement: 2),
            new SeedData("Green Pea", seedName: "Green Pea Seeds", yearRequirement: 4),
            new SeedData("Habanero", seedName: "Habanero Seeds", yearRequirement: 3),
            new SeedData("Jalapeno Pepper", seedName: "Jalapeno Seeds", yearRequirement: 3),
            new SeedData("Juniper", seedName: "Juniper Berry Seeds"),
            new SeedData("Kidney Bean", seedName: "Kidney Bean Starter", yearRequirement: 4),
            new SeedData("Kiwi", seedName: "Kiwi Seeds"),
            new SeedData("Lettuce", seedName: "Lettuce Seeds"),
            new SeedData("Licorice Root", seedName: "Licorice Seeds", yearRequirement: 4),
            new SeedData("Maguey", seedName: "Maguey Seeds", yearRequirement: 4),
            new SeedData("Mint", seedName: "Mint Seeds"),
            new SeedData("Muskmelon", seedName: "Muskmelon Seeds", yearRequirement: 2),
            new SeedData("Navy Bean", seedName: "Navy Bean Seeds"),
            new SeedData("Oat", seedName: "Oat Seeds", yearRequirement: 3),
            new SeedData("Okra", seedName: "Okra Seeds", yearRequirement: 3),
            new SeedData("Onion", seedName: "Onion Seeds"),
            new SeedData("Oregano", seedName: "Oregano Seeds"),
            new SeedData("Paddy Taro", seedName: "Paddy Taro Seeds", yearRequirement: 2),
            new SeedData("Parsley", seedName: "Parsley Seeds"),
            new SeedData("Passion Fruit", seedName: "Passion Fruit Seeds"),
            new SeedData("Peanut", seedName: "Peanut Seeds", yearRequirement: 2),
            new SeedData("Pineapple", seedName: "Pineapple Seeds"),
            new SeedData("Quinoa", seedName: "Quinoa Seeds", yearRequirement: 4),
            new SeedData("Rapeseed", seedName: "Rapeseed Seeds", yearRequirement: 3),
            new SeedData("Raspberry", seedName: "Raspberry Seeds", yearRequirement: 2),
            new SeedData("Red Onion", seedName: "Red Onion Seeds", yearRequirement: 3),
            new SeedData("Rosemary", seedName: "Rosemary Seeds"),
            new SeedData("Sage", seedName: "Sage Seeds"),
            new SeedData("Shallot", seedName: "Shallot Seeds", yearRequirement: 4),
            new SeedData("Shiitake Mushroom", seedName: "Shiitake Mushroom Starter", yearRequirement: 4),
            new SeedData("Soybean", seedName: "Soybean Seeds", yearRequirement: 2),
            new SeedData("Spinach", seedName: "Spinach Seeds"),
            new SeedData("Sugar Beet", seedName: "Sugar Beet Seeds", yearRequirement: 2),
            new SeedData("Sugar Cane", seedName: "Sugar Cane Seeds"),
            new SeedData("Sulfur Shelf Mushroom", seedName: "Sulfur Shelf Mushroom Seeds", yearRequirement: 4),
            new SeedData("Sweet Canary Melon", seedName: "Sweet Canary Melon Seeds", yearRequirement: 2),
            new SeedData("Sweet Potato", seedName: "Sweet Potato Seeds"),
            new SeedData("Tabasco Pepper", seedName: "Tabasco Pepper Seeds", yearRequirement: 3),
            new SeedData("Thyme", seedName: "Thyme Seeds"),
            new SeedData("Wasabi", seedName: "Wasabi Seeds"),
            new SeedData("Watermelon Mizu", seedName: "Watermelon Seeds"),
            new SeedData("Zucchini", seedName: "Zucchini Seeds", yearRequirement: 3)
        };

        /// <summary>The list of seed data for Mizus Flowers.</summary>
        public static List<SeedData> MizusFlowers { get; } = new List<SeedData>
        {
            new SeedData("Bee Balm", seedName: "Bee Balm Seeds"),
            new SeedData("Blue Mist", seedName: "Blue Mist Seeds"),
            new SeedData("Chamomile", seedName: "Chamomile Seeds"),
            new SeedData("Clary Sage", seedName: "Clary Sage Seeds"),
            new SeedData("Fairy Duster", seedName: "Fairy Duster Pod"),
            new SeedData("Fall Rose", seedName: "Fall Rose Starter"),
            new SeedData("Fragrant Lilac", seedName: "Fragrant Lilac Pod"),
            new SeedData("Herbal Lavender", seedName: "Herbal Lavender Seeds"),
            new SeedData("Honeysuckle", seedName: "Honeysuckle Starter"),
            new SeedData("Passion Flower", seedName: "Passion Flower Seeds"),
            new SeedData("Pink Cat", seedName: "Pink Cat Seeds"),
            new SeedData("Pitcher Plant", seedName: "Pitcher Plant Starter", yearRequirement: 4),
            new SeedData("Purple Coneflower", seedName: "Purple Coneflower Seeds"),
            new SeedData("Rafflesia", seedName: "Rafflesia Seeds", yearRequirement: 4),
            new SeedData("Rose", seedName: "Rose Starter"),
            new SeedData("Shaded Violet", seedName: "Shaded Violet Seeds"),
            new SeedData("Spring Rose", seedName: "Spring Rose Starter"),
            new SeedData("Summer Rose", seedName: "Summer Rose Starter"),
            new SeedData("Sweet Jasmine", seedName: "Sweet Jasmine Seeds")
        };

        /// <summary>The list of seed data for Fresh Meat.</summary>
        public static List<SeedData> CannabisKit { get; } = new List<SeedData>
        {
            new SeedData("Blue Dream", seedName: "Blue Dream Starter"),
            new SeedData("Cannabis", seedName: "Cannabis Starter"),
            new SeedData("Girl Scout Cookies", seedName: "Girl Scout Cookies Starter"),
            new SeedData("Green Crack", seedName: "Green Crack Starter"),
            new SeedData("Hemp", seedName: "Hemp Starter"),
            new SeedData("Hybrid", seedName: "Hybrid Starter"),
            new SeedData("Indica", seedName: "Indica Starter"),
            new SeedData("Northern Lights", seedName: "Northern Lights Starter"),
            new SeedData("OG Kush", seedName: "OG Kush Starter"),
            new SeedData("Sativa", seedName: "Sativa Starter"),
            new SeedData("Sour Diesel", seedName: "Sour Diesel Starter"),
            new SeedData("Strawberry Cough", seedName: "Strawberry Cough Starter"),
            new SeedData("Tobacco", seedName: "Tobacco Seeds"),
            new SeedData("White Widow", seedName: "White Widow Starter")
        };

        /// <summary>The list of seed data for Six Plantable Crops.</summary>
        public static List<SeedData> SixPlantableCrops { get; } = new List<SeedData>
        {
            new SeedData("Blue Rose", seedName: "Blue Rose Seeds"),
            new SeedData("Daikon", seedName: "Daikon Seeds"),
            new SeedData("Gentian", seedName: "Gentian Seeds"),
            new SeedData("Napa Cabbage", seedName: "Napa Cabbage Seeds"),
            new SeedData("Snowdrop", seedName: "Snowdrop Seeds"),
            new SeedData("Winter Broccoli", seedName: "Winter Broccoli Seeds")
        };

        /// <summary>The list of seed data for Bonsters Crops.</summary>
        public static List<SeedData> BonsterCrops { get; } = new List<SeedData>
        {
            new SeedData("Benne Sesame", seedName: "Benne Sesame Seed"),
            new SeedData("Black Beans", seedName: "Black Bean Seeds"),
            new SeedData("Blackcurrant", seedName: "Blackcurrant Seeds", yearRequirement: 2),
            new SeedData("Blue Corn", seedName: "Blue Corn Seeds"),
            new SeedData("Candy Roaster Squash", seedName: "Candy Roaster Squash Seeds"),
            new SeedData("Cardamom", seedName: "Cardamom Seeds"),
            new SeedData("Chayote", seedName: "Chayote Seeds"),
            new SeedData("Cloudberry", seedName: "Cloudberry Seeds"),
            new SeedData("Cowpeas", seedName: "Cowpea Seeds"),
            new SeedData("Cranberry Beans", seedName: "Cranberry Bean Seeds", yearRequirement: 2),
            new SeedData("Green Chile", seedName: "Green Chile Seeds"),
            new SeedData("Lingonberry", seedName: "Lingonberry Seeds"),
            new SeedData("Maypop", seedName: "Maypop Seeds", yearRequirement: 2),
            new SeedData("Nasturtium", seedName: "Nasturtium Seeds"),
            new SeedData("Oats", seedName: "Oats Seeds"),
            new SeedData("Peppercorn", seedName: "Peppercorn Seeds"),
            new SeedData("Prickly Pear", seedName: "Prickly Pear Seeds"),
            new SeedData("Red Currant", seedName: "Red Currant Seeds", yearRequirement: 2),
            new SeedData("Rose Hips", seedName: "Rose Hip Seeds", yearRequirement: 2),
            new SeedData("Roselle Hibiscus", seedName: "Roselle Hibiscus Seeds", yearRequirement: 2),
            new SeedData("Salsify", seedName: "Salsify Seeds"),
            new SeedData("Scotch Bonnet Pepper", seedName: "Scotch Bonnet Pepper Seeds"),
            new SeedData("Summer Squash", seedName: "Summer Squash Seeds"),
            new SeedData("Sunchoke", seedName: "Sunchoke Seeds"),
            new SeedData("Taro", seedName: "Taro Root"),
            new SeedData("White Alpine Strawberry", seedName: "White Alpine Strawberry Seeds"),
            new SeedData("White Currant", seedName: "White Currant Seeds", yearRequirement: 2)
        };

        /// <summary>The list of seed data for Revenants Crops.</summary>
        public static List<SeedData> RevenantCrops { get; } = new List<SeedData>
        {
            new SeedData("Enoki Mushroom", seedName: "Enoki Mushroom Kit", yearRequirement: 2),
            new SeedData("Gai Lan", seedName: "Gai Lan Seeds", yearRequirement: 2),
            new SeedData("Maitake Mushroom", seedName: "Maitake Mushroom Kit", yearRequirement: 2),
            new SeedData("Oyster Mushroom", seedName: "Oyster Mushroom Kit", yearRequirement: 2)
        };

        /// <summary>The list of seed data for Farmer to Florist.</summary>
        public static List<SeedData> FarmerToFlorist { get; } = new List<SeedData>
        {
            new SeedData("Allium", seedName: "Allium Seeds"),
            new SeedData("Camellia", seedName: "Camellia Seeds", yearRequirement: 2),
            new SeedData("Carnation", seedName: "Carnation Seeds"),
            new SeedData("Chrysanthemum", seedName: "Chrysanthemum Seeds"),
            new SeedData("Clematis", seedName: "Clematis Starter"),
            new SeedData("Dahlia", seedName: "Dahlia Seeds", yearRequirement: 3),
            new SeedData("Delphinium", seedName: "Delphinium Seeds", yearRequirement: 2),
            new SeedData("English Rose", seedName: "English Rose Seeds", yearRequirement: 3),
            new SeedData("Freesia", seedName: "Freesia Bulb"),
            new SeedData("Geranium", seedName: "Geranium Seeds"),
            new SeedData("Herbal Peony", seedName: "Herbal Peony Seeds"),
            new SeedData("Hyacinth", seedName: "Hyacinth Seeds", yearRequirement: 2),
            new SeedData("Hydrangea", seedName: "Hydrangea Seeds", yearRequirement: 3),
            new SeedData("Iris", seedName: "Iris Bulb"),
            new SeedData("Lavender", seedName: "Lavender Seeds"),
            new SeedData("Lilac", seedName: "Lilac Seeds"),
            new SeedData("Lily", seedName: "Lily Bulb"),
            new SeedData("Lotus", seedName: "Lotus Starter"),
            new SeedData("Petunia", seedName: "Petunia Seeds", yearRequirement: 3),
            new SeedData("Violet", seedName: "Violet Seeds"),
            new SeedData("Wisteria", seedName: "Wisteria Seeds")
        };

        /// <summary>The list of seed data for Lucky Clover.</summary>
        public static List<SeedData> LuckyClover { get; } = new List<SeedData>
        {
            new SeedData("Lucky Clover", seedName: "Clover Seeds")
        };

        /// <summary>The list of seed data for Fishs Flowers.</summary>
        public static List<SeedData> FishsFlowers { get; } = new List<SeedData>
        {
            new SeedData("Hyacinth", seedName: "Hyacinth Bulb"),
            new SeedData("Pansy", seedName: "Pansy Seeds")
        };

        /// <summary>The list of seed data for Fishs Flowers Compatiblity Version.</summary>
        public static List<SeedData> FishsFlowersCompatibilityVersion { get; } = new List<SeedData>
        {
            new SeedData("Grape Hyacinth", seedName: "Grape Hyacinth Bulb"),
            new SeedData("Pansy", seedName: "Pansy Seeds")
        };

        /// <summary>The list of seed data for Stephans Lots of Crops.</summary>
        public static List<SeedData> StephansLotsOfCrops { get; } = new List<SeedData>
        {
            new SeedData("Carrot", seedName: "Carrot Seeds"),
            new SeedData("Cucumber", seedName: "Cucumber Seeds"),
            new SeedData("Onion", seedName: "Onion Seeds"),
            new SeedData("Pea Pod", seedName: "Pea Starter"),
            new SeedData("Peanut", seedName: "Peanut Sprouts"),
            new SeedData("Pineapple", seedName: "Pineapple Seeds"),
            new SeedData("Spinach", seedName: "Spinach Seeds"),
            new SeedData("Turnip", seedName: "Turnip Seeds"),
            new SeedData("Watermelon", seedName: "Watermelon Seeds")
        };

        /// <summary>The list of seed data for Eemies Crops.</summary>
        public static List<SeedData> EemiesCrops { get; } = new List<SeedData>
        {
            new SeedData("Acorn Squash", seedName: "Acorn Squash Seeds"),
            new SeedData("Black Forest Squash", seedName: "Black Forest Squash Seeds"),
            new SeedData("Cantaloupe Melon", seedName: "Cantaloupe Melon Seeds"),
            new SeedData("Charentais Melon", seedName: "Charentais Melon Seeds"),
            new SeedData("Crookneck Squash", seedName: "Crookneck Squash Seeds"),
            new SeedData("Golden Hubbard Squash", seedName: "Golden Hubbard Squash Seeds"),
            new SeedData("Jack O Lantern Pumpkin", seedName: "Jack O Lantern Pumpkin Seeds"),
            new SeedData("Korean Melon", seedName: "Korean Melon Seeds"),
            new SeedData("Large Watermelon", seedName: "Large Watermelon Seeds"),
            new SeedData("Rich Canary Melon", seedName: "Rich Canary Melon Seeds"),
            new SeedData("Rich Sweetness Melon", seedName: "Rich Sweetness Melon Seeds"),
            new SeedData("Sweet Lightning Pumpkin", seedName: "Sweet Lightning Pumpkin Seeds")
        };

        /// <summary>The list of seed data for Tea Time.</summary>
        public static List<SeedData> TeaTime { get; } = new List<SeedData>
        {
            new SeedData("Mint Tea Plant", seedName: "Fresh Mint Seeds")
        };

        /// <summary>The list of seed data for Forage To Farm.</summary>
        public static List<SeedData> ForageToFarm { get; } = new List<SeedData>
        {
            new SeedData("Cave Carrot", seedName: "Cave Carrot Seeds"),
            new SeedData("Chanterelle Mushroom", seedName: "Chanterelle Mushroom Spores"),
            new SeedData("Coconut", seedName: "Coconut Seed"),
            new SeedData("Common Mushroom", seedName: "Common Mushroom Spores"),
            new SeedData("Crocus", seedName: "Crocus Seeds"),
            new SeedData("Crystal Fruit", seedName: "Crystal Fruit Seeds"),
            new SeedData("Daffodil", seedName: "Daffodil Seeds"),
            new SeedData("Dandelion", seedName: "Dandelion Seeds"),
            new SeedData("Fiddlehead Fern", seedName: "Fiddlehead Fern Spores"),
            new SeedData("Hazelnut", seedName: "Hazelnut Seed"),
            new SeedData("Holly", seedName: "Holly Seeds"),
            new SeedData("Wild Horseradish", seedName: "Wild Horseradish Seeds"),
            new SeedData("Leek", seedName: "Leek Seeds"),
            new SeedData("Morel Mushroom", seedName: "Morel Mushroom Spores"),
            new SeedData("Purple Mushroom", seedName: "Purple Mushroom Spores"),
            new SeedData("Red Mushroom", seedName: "Red Mushroom Spores"),
            new SeedData("Salmonberry", seedName: "Salmonberry Seeds"),
            new SeedData("Snow Yam", seedName: "Snow Yam Seeds"),
            new SeedData("Spice Berry", seedName: "Spice Berry Seeds"),
            new SeedData("Spring Onion", seedName: "Spring Onion Seeds"),
            new SeedData("Sweet Pea", seedName: "Sweet Pea Seeds"),
            new SeedData("Wild Blackberry", seedName: "Wild Blackberry Seeds"),
            new SeedData("Wild Plum", seedName: "Wild Plum Seed"),
            new SeedData("Winter Root", seedName: "Winter Root Seeds")
        };

        /// <summary>The list of seed data for Gem and Mineral Crops.</summary>
        public static List<SeedData> GemAndMineralCrops { get; } = new List<SeedData>
        {
            new SeedData("Aerinite Root", seedName: "Aerinite Root Seeds"),
            new SeedData("Aquamarine", seedName: "Aquamarine Rose Seeds"),
            new SeedData("Celestine Flower", seedName: "Celestine Flower Seeds"),
            new SeedData("Diamond Flower", seedName: "Diamond Flower Seed"),
            new SeedData("Ghost Rose", seedName: "Ghost Rose Seed"),
            new SeedData("Kyanite Flower", seedName: "Kyanite Flower Seed"),
            new SeedData("Opal Cat", seedName: "Opal Cat Seeds"),
            new SeedData("Slate Bean", seedName: "Slate Bean Seed"),
            new SeedData("Soap Root", seedName: "Soap Root Seed")
        };

        /// <summary>The list of seed data for Mouse Ear Cress.</summary>
        public static List<SeedData> MouseEarCress { get; } = new List<SeedData>
        {
            new SeedData("Mouse-Ear Cress", seedName: "Mouse-Ear Cress Seeds")
        };

        /// <summary>The list of seed data for Ancient Crops.</summary>
        public static List<SeedData> AncientCrops { get; } = new List<SeedData>
        {
            new SeedData("Ancient Coffee Plant", seedName: "Ancient Coffee Bean"),
            new SeedData("Ancient Fern", seedName: "Ancient Fern Seeds"),
            new SeedData("Ancient Flower", seedName: "Ancient Flower Seeds"),
            new SeedData("Ancient Nut", seedName: "Ancient Nut Seeds"),
            new SeedData("Ancient Olive Plant", seedName: "Ancient Olive Seeds"),
            new SeedData("Ancient Tuber", seedName: "Ancient Tuber Seeds")
        };

        /// <summary>The list of seed data for Poke Crops.</summary>
        public static List<SeedData> PokeCrops { get; } = new List<SeedData>
        {
            new SeedData("Aspear Berry", seedName: "Aspear Berry Seeds"),
            new SeedData("Cheri Berry", seedName: "Cheri Berry Seeds"),
            new SeedData("Chesto Berry", seedName: "Chesto Berry Seeds"),
            new SeedData("Leppa Berry", seedName: "Leppa Berry Seeds"),
            new SeedData("Lum Berry", seedName: "Lum Berry Seeds"),
            new SeedData("Nanab Berry", seedName: "Nanab Berry Seeds"),
            new SeedData("Oran Berry", seedName: "Oran Berry Seeds"),
            new SeedData("Pecha Berry", seedName: "Pecha Berry Seeds"),
            new SeedData("Persim Berry", seedName: "Persim Berry Seeds"),
            new SeedData("Rawst Berry", seedName: "Rawst Berry Seeds"),
            new SeedData("Sitrus Berry", seedName: "Sitrus Berry Seeds")
        };

        /// <summary>The list of seed data for Starbound Valley.</summary>
        public static List<SeedData> StarboundValley { get; } = new List<SeedData>
        {
            new SeedData("Automato", seedName: "Automato Seeds"),
            new SeedData("Avesmingo", seedName: "Avesmingo Seeds"),
            new SeedData("Beakseed", seedName: "Beakseed Seeds"),
            new SeedData("Boltbulb", seedName: "Boltbulb Seeds"),
            new SeedData("Boneboo", seedName: "Boneboo Seeds"),
            new SeedData("Coralcreep", seedName: "Coralcreep Tank", yearRequirement: 2),
            new SeedData("Crystal Plant", seedName: "Crystal Plant Seeds"),
            new SeedData("Currentcorn", seedName: "Currentcorn Seeds"),
            new SeedData("Diodia", seedName: "Diodia Seeds"),
            new SeedData("Dirturchin", seedName: "Dirturchin Seeds"),
            new SeedData("Eggshoot", seedName: "Eggshoot Seeds"),
            new SeedData("Feathercrown", seedName: "Feathercrown Seeds"),
            new SeedData("Neonmelon", seedName: "Neonmelon Seeds"),
            new SeedData("Oculemon", seedName: "Oculemon Seeds"),
            new SeedData("Pearlpea", seedName: "Pearlpea Seeds"),
            new SeedData("Pussplum", seedName: "Pussplum Seeds"),
            new SeedData("Reefpod", seedName: "Reefpod Tank"),
            new SeedData("Toxictop", seedName: "Toxictop Seeds"),
            new SeedData("Wartweed", seedName: "Wartweed Seeds")
        };

        /// <summary>The list of seed data for iKeychain's Winter Lychee Plant.</summary>
        public static List<SeedData> IKeychainsWinterLycheePlant { get; } = new List<SeedData>
        {
            new SeedData("Winter Lychee", seedName: "Winter Lychee Seeds", yearRequirement: 2)
        };

        /// <summary>The list of seed data for Green Pear.</summary>
        public static List<SeedData> GreenPear { get; } = new List<SeedData>
        {
            new SeedData("Green Pear", seedName: "Green Pear Seeds")
        };

        /// <summary>The list of seed data for Soda Vine.</summary>
        public static List<SeedData> SodaVine { get; } = new List<SeedData>
        {
            new SeedData("Soda Vine", seedName: "Soda Vine Starter")
        };

        /// <summary>The list of seed data for Spoopy Valley.</summary>
        public static List<SeedData> SpoopyValley { get; } = new List<SeedData>
        {
            new SeedData("Amethyst Basil", seedName: "Amethyst Basil Seeds"),
            new SeedData("Black Carrot", seedName: "Black Carrot Seeds"),
            new SeedData("Black Goji Berry", seedName: "Black Goji Berry Seeds"),
            new SeedData("Black Huckleberry", seedName: "Black Huckleberry Seeds"),
            new SeedData("Black Magic Viola", seedName: "Black Magic Viola Seeds"),
            new SeedData("Black Mulberry", seedName: "Black Mulberry Seeds"),
            new SeedData("Black Velvet Petunia", seedName: "Black Velvet Petunia Seeds"),
            new SeedData("Futsu Pumpkin", seedName: "Futsu Pumpkin Seeds"),
            new SeedData("Hungarian Chile", seedName: "Hungarian Chile Seeds"),
            new SeedData("Indigo Rose Tomato", seedName: "Indigo Rose Tomato Seeds"),
            new SeedData("Kulli Corn", seedName: "Kulli Corn Seeds"),
            new SeedData("Purple Beauty Bell Pepper", seedName: "Purple Beauty Bell Pepper Seeds"),
            new SeedData("Queen of the Night Tulip", seedName: "Queen of the Night Tulip Seeds")
        };


        /// <summary>The list of seed data for Stardew Bakery.</summary>
        public static List<SeedData> StardewBakery { get; } = new List<SeedData>
        {
            new SeedData("Cookie Plant", seedName: "Cookie Seeds")
        };

        /// <summary>The list of seed data for Succulents.</summary>
        public static List<SeedData> Succulents { get; } = new List<SeedData>
        {
            new SeedData("Decorative Succulents", seedName: "Decorative Succulents Starter"),
            new SeedData("Edible Succulents", seedName: "Edible Succulents Starter")
        };
    }
}
