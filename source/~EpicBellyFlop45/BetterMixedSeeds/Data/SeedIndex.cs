using System.Collections.Generic;

namespace BetterMixedSeeds.Data
{
    /// <summary>
    /// A list of integrated mods where using the crop name, I can get the seed name
    /// </summary>
    public class SeedIndex
    {
        public Dictionary<string, string> StardewValley { get; } = new Dictionary<string, string>
        {
            { "Amaranth", "299" },
            { "Ancient Fruit", "499" },
            { "Artichoke", "489" },
            { "Beet", "494" },
            { "Blue Jazz", "429" },
            { "Blueberry", "481" },
            { "Bok Choy", "491" },
            { "Cauliflower", "474" },
            { "Coffee Bean", "433" },
            { "Corn", "487" },
            { "Cranberries", "493" },
            { "Eggplant", "488" },
            { "Fairy Rose", "425" },
            { "Garlic", "476" },
            { "Grape", "301" },
            { "Green Bean", "473" },
            { "Hops", "302" },
            { "Hot Pepper", "482" },
            { "Kale", "477" },
            { "Melon", "479" },
            { "Parsnip", "472" },
            { "Poppy", "453" },
            { "Potato", "475" },
            { "Pumpkin", "490" },
            { "Radish", "484" },
            { "Red Cabbage", "485" },
            { "Rhubarb", "478" },
            { "Rice", "273" },
            { "Starfruit", "486" },
            { "Strawberry", "745" },
            { "Summer Spangle", "455" },
            { "Sunflower", "431" },
            { "Sweet Gem Berry", "347" },
            { "Tomato", "480" },
            { "Tulip", "427" },
            { "Wheat", "483" },
            { "Yam", "492" }
        };

        public Dictionary<string, string> FantasyCrops { get; } = new Dictionary<string, string>
        {
            { "Coal Root", "Coal Seeds" },
            { "Copper Leaf", "Copper Seeds" },
            { "Gold Leaf", "Gold Seeds" },
            { "Iridium Fern", "Iridium Seeds" },
            { "Iron Leaf", "Iron Seeds" },
            { "Money Plant", "Doubloon Seeds" }
        };

        public Dictionary<string, string> FreshMeat { get; } = new Dictionary<string, string>
        {
            { "Beef", "Beefvine Seeds" },
            { "Chevon", "Chevonvine Seeds" },
            { "Chicken", "Chickenvine Seeds" },
            { "Duck", "Duckvine Seeds" },
            { "Mutton", "Muttonvine Seeds" },
            { "Pork", "Porkvine Seeds" },
            { "Rabbit", "Rabbitvine Seeds" }
        };

        public Dictionary<string, string> FruitAndVeggies { get; } = new Dictionary<string, string>
        {
            { "Adzuki Bean", "Adzuki Bean Seeds" },
            { "Aloe", "Aloe Pod" },
            { "Asparagus", "Asparagus Seeds" },
            { "Bamboo", "Bamboo Cutting" },
            { "Barley", "Barley Seeds" },
            { "Basil", "Basil Seeds" },
            { "Bell Pepper", "Bell Pepper Seeds" },
            { "Blackberry", "Blackberry Seeds" },
            { "Blue Agave", "Blue Agave Seeds" },
            { "Broccoli", "Broccoli Seeds" },
            { "Butternut Squash", "Butternut Squash Seeds" },
            { "Cabbage", "Cabbage Seeds" },
            { "Cactus Flower", "Cactus Flower Seeds" },
            { "Carrot", "Carrot Seeds" },
            { "Cassava", "Cassava Seeds" },
            { "Celery", "Celery Seeds" },
            { "Chickpea", "Chickpea Seeds" },
            { "Chives", "Chive Seeds" },
            { "Cotton", "Cotton Seeds" },
            { "Cucumber", "Cucumber Starter" },
            { "Durum", "Durum Seeds" },
            { "Elderberry", "Elderberry Seeds" },
            { "Fennel", "Fennel Seeds" },
            { "Ginger", "Ginger Seeds" },
            { "Gooseberry", "Gooseberry Seeds" },
            { "Green Pea", "Green Pea Seeds" },
            { "Habanero", "Habanero Seeds" },
            { "Jalapeno Pepper", "Jalapeno Seeds" },
            { "Juniper", "Juniper Berry Seeds" },
            { "Kidney Bean", "Kidney Bean Starter" },
            { "Kiwi", "Kiwi Seeds" },
            { "Lettuce", "Lettuce Seeds" },
            { "Licorice Root", "Licorice Seeds" },
            { "Maguey", "Maguey Seeds" },
            { "Mint", "Mint Seeds" },
            { "Muskmelon", "Muskmelon Seeds" },
            { "Navy Bean", "Navy Bean Seeds" },
            { "Oat", "Oat Seeds" },
            { "Okra", "Okra Seeds" },
            { "Onion", "Onion Seeds" },
            { "Oregano", "Oregano Seeds" },
            { "Paddy Taro", "Paddy Taro Seeds" },
            { "Parsley", "Parsley Seeds" },
            { "Passion Fruit", "Passion Fruit Seeds" },
            { "Peanut", "Peanut Seeds" },
            { "Pineapple", "Pineapple Seeds" },
            { "Quinoa", "Quinoa Seeds" },
            { "Rapeseed", "Rapeseed Seeds" },
            { "Raspberry", "Raspberry Seeds" },
            { "Red Onion", "Red Onion Seeds" },
            { "Rosemary", "Rosemary Seeds" },
            { "Sage", "Sage Seeds" },
            { "Shallot", "Shallot Seeds" },
            { "Shiitake Mushroom", "Shiitake Mushroom Starter" },
            { "Soybean", "Soybean Seeds" },
            { "Spinach", "Spinach Seeds" },
            { "Sugar Beet", "Sugar Beet Seeds" },
            { "Sugar Cane", "Sugar Cane Seeds" },
            { "Sulfur Shelf Mushroom", "Sulfur Shelf Mushroom Seeds" },
            { "Sweet Canary Melon", "Sweet Canary Melon Seeds" },
            { "Sweet Potato", "Sweet Potato Seeds" },
            { "Tabasco Pepper", "Tabasco Pepper Seeds" },
            { "Thyme", "Thyme Seeds" },
            { "Wasabi", "Wasabi Seeds" },
            { "Watermelon Mizu", "Watermelon Seeds" },
            { "Zucchini", "Zucchini Seeds" }
        };

        public Dictionary<string, string> MizusFlowers { get; } = new Dictionary<string, string>
        {
            { "Bee Balm", "Bee Balm Seeds" },
            { "Blue Mist", "Blue Mist Seeds" },
            { "Chamomile", "Chamomile Seeds" },
            { "Clary Sage", "Clary Sage Seeds" },
            { "Fairy Duster", "Fairy Duster Pod" },
            { "Fall Rose", "Fall Rose Starter" },
            { "Fragrant Lilac", "Fragrant Lilac Pod" },
            { "Herbal Lavender", "Herbal Lavender Seeds" },
            { "Honeysuckle", "Honeysuckle Starter" },
            { "Passion Flower", "Passion Flower Seeds" },
            { "Pink Cat", "Pink Cat Seeds" },
            { "Pitcher Plant", "Pitcher Plant Starter" },
            { "Purple Coneflower", "Purple Coneflower Seeds" },
            { "Rafflesia", "Rafflesia Seeds" },
            { "Rose", "Rose Starter" },
            { "Shaded Violet", "Shaded Violet Seeds" },
            { "Spring Rose", "Spring Rose Starter" },
            { "Summer Rose", "Summer Rose Starter" },
            { "Sweet Jasmine", "Sweet Jasmine Seeds" }
        };

        public Dictionary<string, string> CannabisKit { get; } = new Dictionary<string, string>
        {
            { "Blue Dream", "Blue Dream Starter" },
            { "Cannabis", "Cannabis Starter" },
            { "Girl Scout Cookies", "Girl Scout Cookies Starter" },
            { "Green Crack", "Green Crack Starter" },
            { "Hemp", "Hemp Starter" },
            { "Hybrid", "Hybrid Starter" },
            { "Indica", "Indica Starter" },
            { "Northern Lights", "Northern Lights Starter" },
            { "OG Kush", "OG Kush Starter" },
            { "Sativa", "Sativa Starter" },
            { "Sour Diesel", "Sour Diesel Starter" },
            { "Strawberry Cough", "Strawberry Cough Starter" },
            { "Tobacco", "Tobacco Seeds" },
            { "White Widow", "White Widow Starter" }
        };

        public Dictionary<string, string> SixPlantableCrops { get; } = new Dictionary<string, string>
        {
            { "Blue Rose", "Blue Rose Seeds" },
            { "Daikon", "Daikon Seeds" },
            { "Gentian", "Gentian Seeds" },
            { "Napa Cabbage", "Napa Cabbage Seeds" },
            { "Snowdrop", "Snowdrop Seeds" },
            { "Winter Broccoli", "Winter Broccoli Seeds" }
        };

        public Dictionary<string, string> BonsterCrops { get; } = new Dictionary<string, string>
        {
            { "Benne Sesame", "Benne Sesame Seed" },
            { "Black Beans", "Black Bean Seeds" },
            { "Blackcurrant", "Blackcurrant Seeds" },
            { "Blue Corn", "Blue Corn Seeds" },
            { "Candy Roaster Squash", "Candy Roaster Squash Seeds" },
            { "Cardamom", "Cardamom Seeds" },
            { "Chayote", "Chayote Seeds" },
            { "Cloudberry", "Cloudberry Seeds" },
            { "Cowpeas", "Cowpea Seeds" },
            { "Cranberry Beans", "Cranberry Bean Seeds" },
            { "Green Chile", "Green Chile Seeds" },
            { "Lingonberry", "Lingonberry Seeds" },
            { "Maypop", "Maypop Seeds" },
            { "Nasturtium", "Nasturtium Seeds" },
            { "Oats", "Oats Seeds" },
            { "Peppercorn", "Peppercorn Seeds" },
            { "Prickly Pear", "Prickly Pear Seeds" },
            { "Red Currant", "Red Currant Seeds" },
            { "Rose Hips", "Rose Hip Seeds" },
            { "Roselle Hibiscus", "Roselle Hibiscus Seeds" },
            { "Salsify", "Salsify Seeds" },
            { "Scotch Bonnet Pepper", "Scotch Bonnet Pepper Seeds" },
            { "Summer Squash", "Summer Squash Seeds" },
            { "Sunchoke", "Sunchoke Seeds" },
            { "Taro", "Taro Root" },
            { "White Alpine Strawberry", "White Alpine Strawberry Seeds" },
            { "White Currant", "White Currant Seeds" }
        };

        public Dictionary<string, string> RevenantCrops { get; } = new Dictionary<string, string>
        {
            { "Enoki Mushroom", "Enoki Mushroom Kit" },
            { "Gai Lan", "Gai Lan Seeds" },
            { "Maitake Mushroom", "Maitake Mushroom Kit" },
            { "Oyster Mushroom", "Oyster Mushroom Kit" }
        };

        public Dictionary<string, string> FarmerToFlorist { get; } = new Dictionary<string, string>
        {
            { "Allium", "Allium Seeds" },
            { "Camellia", "Camellia Seeds" },
            { "Carnation", "Carnation Seeds" },
            { "Chrysanthemum", "Chrysanthemum Seeds" },
            { "Clematis", "Clematis Starter" },
            { "Dahlia", "Dahlia Seeds" },
            { "Delphinium", "Delphinium Seeds" },
            { "English Rose", "English Rose Seeds" },
            { "Freesia", "Freesia Bulb" },
            { "Geranium", "Geranium Seeds" },
            { "Herbal Peony", "Herbal Peony Seeds" },
            { "Hyacinth", "Hyacinth Seeds" },
            { "Hydrangea", "Hydrangea Seeds" },
            { "Iris", "Iris Bulb" },
            { "Lavender", "Lavender Seeds" },
            { "Lilac", "Lilac Seeds" },
            { "Lily", "Lily Bulb" },
            { "Lotus", "Lotus Starter" },
            { "Petunia", "Petunia Seeds" },
            { "Violet", "Violet Seeds" },
            { "Wisteria", "Wisteria Seeds" }
        };

        public Dictionary<string, string> LuckyClover { get; } = new Dictionary<string, string>
        {
            { "Lucky Clover", "Clover Seeds" }
        };

        public Dictionary<string, string> FishsFlowers { get; } = new Dictionary<string, string>
        {
            { "Hyacinth", "Hyacinth Bulb" },
            { "Pansy", "Pansy Seeds" }
        };

        public Dictionary<string, string> StephansLotsOfCrops { get; } = new Dictionary<string, string>
        {
            { "Carrot", "Carrot Seeds" },
            { "Cucumber", "Cucumber Seeds" },
            { "Onion", "Onion Seeds" },
            { "Pea Pod", "Pea Starter" },
            { "Peanut", "Peanut Sprouts" },
            { "Pineapple", "Pineapple Seeds" },
            { "Spinach", "Spinach Seeds" },
            { "Turnip", "Turnip Seeds" },
            { "Watermelon", "Watermelon Seeds" }
        };

        public Dictionary<string, string> EemiesCrops { get; } = new Dictionary<string, string>
        {
            { "Acorn Squash", "Acorn Squash Seeds" },
            { "Black Forest Squash", "Black Forest Squash Seeds" },
            { "Cantaloupe Melon", "Cantaloupe Melon Seeds" },
            { "Charentais Melon", "Charentais Melon Seeds" },
            { "Crookneck Squash", "Crookneck Squash Seeds" },
            { "Golden Hubbard Squash", "Golden Hubbard Squash Seeds" },
            { "Jack O Lantern Pumpkin", "Jack O Lantern Pumpkin Seeds" },
            { "Korean Melon", "Korean Melon Seeds" },
            { "Large Watermelon", "Large Watermelon Seeds" },
            { "Rich Canary Melon", "Rich Canary Melon Seeds" },
            { "Rich Sweetness Melon", "Rich Sweetness Melon Seeds" },
            { "Sweet Lightning Pumpkin", "Sweet Lightning Pumpkin Seeds" }
        };

        public Dictionary<string, string> TeaTime { get; } = new Dictionary<string, string>
        {
            { "Mint Tea Plant", "Fresh Mint Seeds" },
            { "Tea Leaf Plant", "Tea Leaf Seeds" }
        };

        public Dictionary<string, string> ForageToFarm { get; } = new Dictionary<string, string>
        {
            { "Cave Carrot", "Cave Carrot Seeds" },
            { "Chanterelle Mushroom", "Chanterelle Mushroom Spores" },
            { "Coconut", "Coconut Seed" },
            { "Common Mushroom", "Common Mushroom Spores" },
            { "Crocus", "Crocus Seeds" },
            { "Crystal Fruit", "Crystal Fruit Seeds" },
            { "Daffodil", "Daffodil Seeds" },
            { "Dandelion", "Dandelion Seeds" },
            { "Fiddlehead Fern", "Fiddlehead Fern Spores" },
            { "Hazelnut", "Hazelnut Seed" },
            { "Holly", "Holly Seeds" },
            { "Wild Horseradish", "Wild Horseradish Seeds" },
            { "Leek", "Leek Seeds" },
            { "Morel Mushroom", "Morel Mushroom Spores" },
            { "Purple Mushroom", "Purple Mushroom Spores" },
            { "Red Mushroom", "Red Mushroom Spores" },
            { "Salmonberry", "Salmonberry Seeds" },
            { "Snow Yam", "Snow Yam Seeds" },
            { "Spice Berry", "Spice Berry Seeds" },
            { "Spring Onion", "Spring Onion Seeds" },
            { "Sweet Pea", "Sweet Pea Seeds" },
            { "Wild Blackberry", "Wild Blackberry Seeds" },
            { "Wild Plum", "Wild Plum Seed" },
            { "Winter Root", "Winter Root Seeds" }
        };

        public Dictionary<string, string> GemAndMineralCrops { get; } = new Dictionary<string, string>
        {
            { "Aerinite Root", "Aerinite Root Seeds" },
            { "Aquamarine", "Aquamarine Rose Seeds" },
            { "Celestine Flower", "Celestine Flower Seeds" },
            { "Diamond Flower", "Diamond Flower Seed" },
            { "Ghost Rose", "Ghost Rose Seed" },
            { "Kyanite Flower", "Kyanite Flower Seed" },
            { "Opal Cat", "Opal Cat Seeds" },
            { "Slate Bean", "Slate Bean Seed" },
            { "Soap Root", "Soap Root Seed" }
        };

        public Dictionary<string, string> MouseEarCress { get; } = new Dictionary<string, string>
        {
            { "Mouse-Ear Cress", "Mouse-Ear Cress Seeds" }
        };

        public Dictionary<string, string> AncientCrops { get; } = new Dictionary<string, string>
        {
            { "Ancient Coffee Plant", "Ancient Coffee Bean" },
            { "Ancient Fern", "Ancient Fern Seeds" },
            { "Ancient Flower", "Ancient Flower Seeds" },
            { "Ancient Nut", "Ancient Nut Seeds" },
            { "Ancient Olive Plant", "Ancient Olive Seeds" },
            { "Ancient Tuber", "Ancient Tuber Seeds" }
        };
    }
}
