/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/HaulinOats/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;

namespace DynamicCrops
{
    public class ModData
    {
        //https://stardewcommunitywiki.com/Modding:Crop_data
        //[0]Growth Stages/[1]Growth Seasons/[2]Sprite Sheet Index/[3]Harvest Item Index/[4]Regrow After Harvest (-1 = no regrow)/[5]Harvest Method (0 = no scythe needed)/[6]Chance For Extra Harvest/[7]Raised Seeds (true if trellis item)/[8]Tint Color
        public Dictionary<string, string> CropData { get; set; } = new Dictionary<string, string>(){
            { "273", "1 2 2 3/spring/34/271/-1/1/true 1 1 10 .1/false/false"},
            { "299", "1 2 2 2/fall/39/300/-1/1/false/false/false"},
            { "301", "1 1 2 3 3/fall/38/398/3/0/false/true/false"},
            { "302", "1 1 2 3 4/summer/37/304/1/0/false/true/false"},
            { "347", "2 4 6 6 6/fall/32/417/-1/0/false/false/false"},
            { "425", "1 4 4 3/fall/31/595/-1/0/false/false/true 187 0 255 119 137 255 71 227 255 255 127 144 205 178 255 140 119 255"},
            { "427", "1 1 2 2/spring/26/591/-1/0/false/false/true 255 186 255 223 191 255 255 246 0 255 80 0 255 158 193"},
            { "429", "1 2 2 2/spring/27/597/-1/0/false/false/true 35 127 255 109 131 255 112 207 255 191 228 255 94 121 255 40 150 255"},
            { "431", "1 2 3 2/summer fall/30/421/-1/0/false/false/false"},
            { "433", "1 2 2 3 2/spring summer/40/433/2/0/true 4 4 0 .02/false/false"},
            { "453", "1 2 2 2/summer/28/376/-1/0/false/false/true 255 0 0 254 254 254 255 170 0"},
            { "455", "1 2 3 2/summer/29/593/-1/0/false/false/true 0 208 255 99 255 210 255 212 0 255 144 122 255 0 238 206 91 255"},
            { "472", "1 1 1 1/spring/0/24/-1/0/false/false/false"},
            { "473", "1 1 1 3 4/spring/1/188/3/0/false/true/false"},
            { "474", "1 2 4 4 1/spring/2/190/-1/0/false/false/false"},
            { "475", "1 1 1 2 1/spring/3/192/-1/0/true 1 1 0 .2/false/false"},
            { "476", "1 1 1 1/spring/4/248/-1/0/false/false/false"},
            { "477", "1 2 2 1/spring/5/250/-1/1/false/false/false"},
            { "478", "2 2 2 3 4/spring/6/252/-1/0/false/false/false"},
            { "479", "1 2 3 3 3/summer/7/254/-1/0/false/false/false"},
            { "480", "2 2 2 2 3/summer/8/256/4/0/true 1 1 0 .05/false/false"},
            { "481", "1 3 3 4 2/summer/9/258/4/0/true 3 3 0 .02/false/false"},
            { "482", "1 1 1 1 1/summer/10/260/3/0/true 1 1 0 .03/false/false"},
            { "483", "1 1 1 1/summer fall/11/262/-1/1/false/false/false"},
            { "484", "2 1 2 1/summer/12/264/-1/0/false/false/false"},
            { "485", "2 1 2 2 2/summer/13/266/-1/0/false/false/false"},
            { "486", "2 3 2 3 3/summer/14/268/-1/0/false/false/false"},
            { "487", "2 3 3 3 3/summer fall/15/270/4/0/false/false/false"},
            { "488", "1 1 1 1 1/fall/16/272/5/0/true 1 1 0 .002/false/false"},
            { "489", "2 2 1 2 1/fall/17/274/-1/0/false/false/false"},
            { "490", "1 2 3 4 3/fall/18/276/-1/0/false/false/false"},
            { "491", "1 1 1 1/fall/19/278/-1/0/false/false/false"},
            { "492", "1 3 3 3/fall/20/280/-1/0/false/false/false"},
            { "493", "1 2 1 1 2/fall/21/282/5/0/true 2 2 0 .1/false/false"},
            { "494", "1 1 2 2/fall/22/284/-1/0/false/false/false"},
            { "499", "2 7 7 7 5/spring summer fall/24/454/7/0/false/false/false"},
            { "745", "1 1 2 2 2/spring/36/400/4/0/true 1 1 0 .02/false/false"},
            { "802", "2 2 2 3 3/spring summer fall winter/41/90/3/0/false/false/false"},
            { "831", "1 2 3 4/summer/42/830/-1/0/false/false/false"},
            { "833", "1 3 3 4 3/summer/43/832/7/0/false/false/false"},
        };
        //[0]English Name/[1]Sell Price (Seed Cost)/[2]Edibility/[3]Seed Category/[4]Display Name/[5]Description
        public Dictionary<string, string> ObjectData { get; set; } = new Dictionary<string, string>(){
            { "16", "Wild Horseradish/50/5/Basic -81/Wild Horseradish/A spicy root found in the spring."},
            { "18", "Daffodil/30/0/Basic -81/Daffodil/A traditional spring flower that makes a nice gift."},
            { "20", "Leek/60/16/Basic -81/Leek/A tasty relative of the onion."},
            { "22", "Dandelion/40/10/Basic -81/Dandelion/Not the prettiest flower, but the leaves make a good salad."},
            { "24", "Parsnip/2/10/Basic -75/Parsnip/A spring tuber closely related to the carrot. It has an earthy taste and is full of nutrients."},
            { "78", "Cave Carrot/25/12/Basic -81/Cave Carrot/A starchy snack found in caves. It helps miners work longer."},
            { "88", "Coconut/100/-300/Basic -79/Coconut/A seed of the coconut palm. It has many culinary uses."},
            { "90", "Cactus Fruit/75/30/Basic -79/Cactus Fruit/The sweet fruit of the prickly pear cactus."},
            { "188", "Green Bean/40/10/Basic -75/Green Bean/A juicy little bean with a cool, crisp snap."},
            { "190", "Cauliflower/175/30/Basic -75/Cauliflower/Valuable, but slow-growing. Despite its pale color, the florets are packed with nutrients."},
            { "192", "Potato/80/10/Basic -75/Potato/A widely cultivated tuber."},
            { "248", "Garlic/60/8/Basic -75/Garlic/Adds a wonderful zestiness to dishes. High quality garlic can be pretty spicy."},
            { "250", "Kale/110/20/Basic -75/Kale/The waxy leaves are great in soups and stir frys."},
            { "252", "Rhubarb/220/-300/Basic -79/Rhubarb/The stalks are extremely tart, but make a great dessert when sweetened."},
            { "254", "Melon/250/45/Basic -79/Melon/A cool, sweet summer treat."},
            { "256", "Tomato/60/8/Basic -75/Tomato/Rich and slightly tangy, the Tomato has a wide variety of culinary uses."},
            { "257", "Morel/150/8/Basic -81/Morel/Sought after for its unique nutty flavor."},
            { "258", "Blueberry/50/10/Basic -79/Blueberry/A popular berry reported to have many health benefits. The blue skin has the highest nutrient concentration."},
            { "259", "Fiddlehead Fern/90/10/Basic -75/Fiddlehead Fern/The young shoots are an edible specialty."},
            { "260", "Hot Pepper/40/5/Basic -79/Hot Pepper/Fiery hot with a hint of sweetness."},
            { "262", "Wheat/25/-300/Basic -75/Wheat/One of the most widely cultivated grains. Makes a great flour for breads and cakes."},
            { "264", "Radish/90/18/Basic -75/Radish/A crisp and refreshing root vegetable with hints of pepper when eaten raw."},
            { "266", "Red Cabbage/260/30/Basic -75/Red Cabbage/Often used in salads and coleslaws. The color can range from purple to blue to green-yellow depending on soil conditions."},
            { "268", "Starfruit/750/50/Basic -79/Starfruit/An extremely juicy fruit that grows in hot, humid weather. Slightly sweet with a sour undertone."},
            { "270", "Corn/50/10/Basic -75/Corn/One of the most popular grains. The sweet, fresh cobs are a summer favorite."},
            { "271", "Unmilled Rice/30/1/Basic -75/Unmilled Rice/Rice in its rawest form. Run this through a mill to increase the value."},
            { "272", "Eggplant/60/8/Basic -75/Eggplant/A rich and wholesome relative of the tomato. Delicious fried or stewed."},
            { "273", "Rice Shoot/20/-300/Seeds -74/Rice Shoot/Plant these in the spring. Takes 8 days to mature. Grows faster if planted near a body of water. Harvest with the scythe."},
            { "274", "Artichoke/160/12/Basic -75/Artichoke/The bud of a thistle plant. The spiny outer leaves conceal a fleshy, filling interior."},
            { "276", "Pumpkin/320/-300/Basic -75/Pumpkin/A fall favorite, grown for its crunchy seeds and delicately flavored flesh. As a bonus, the hollow shell can be carved into a festive decoration."},
            { "278", "Bok Choy/80/10/Basic -75/Bok Choy/The leafy greens and fibrous stalks are healthy and delicious."},
            { "280", "Yam/160/18/Basic -75/Yam/A starchy tuber with a lot of culinary versatility."},
            { "281", "Chanterelle/160/30/Basic -81/Chanterelle/A tasty mushroom with a fruity smell and slightly peppery flavor."},
            { "282", "Cranberries/75/15/Basic -79/Cranberries/These tart red berries are a traditional winter food."},
            { "283", "Holly/80/-15/Basic -81/Holly/The leaves and bright red berries make a popular winter decoration."},
            { "284", "Beet/100/12/Basic -75/Beet/A sweet and earthy root vegatable. As a bonus, the leaves make a great salad."},
            { "296", "Salmonberry/5/10/Basic -79/Salmonberry/A spring-time berry with the flavor of the forest."},
            { "299", "Amaranth Seeds/35/-300/Seeds -74/Amaranth Seeds/Plant these in the fall. Takes 7 days to grow. Harvest with the scythe."},
            { "300", "Amaranth/150/20/Basic -75/Amaranth/A purple grain cultivated by an ancient civilization."},
            { "301", "Grape Starter/30/-300/Seeds -74/Grape Starter/Plant these in the fall. Takes 10 days to grow, but keeps producing after that. Grows on a trellis."},
            { "302", "Hops Starter/30/-300/Seeds -74/Hops Starter/Plant these in the summer. Takes 11 days to grow, but keeps producing after that. Grows on a trellis."},
            { "304", "Hops/25/18/Basic -75/Hops/A bitter, tangy flower used to flavor beer."},
            { "347", "Rare Seed/200/-300/Seeds -74/Rare Seed/Sow in fall. Takes all season to grow."},
            { "376", "Poppy/140/18/Basic -80/Poppy/In addition to its colorful flower, the Poppy has culinary and medicinal uses."},
            { "396", "Spice Berry/80/10/Basic -79/Spice Berry/It fills the air with a pungent aroma."},
            { "398", "Grape/80/15/Basic -79/Grape/A sweet cluster of fruit."},
            { "399", "Spring Onion/8/5/Basic -81/Spring Onion/These grow wild during the spring."},
            { "400", "Strawberry/120/20/Basic -79/Strawberry/A sweet, juicy favorite with an appealing red color."},
            { "402", "Sweet Pea/50/0/Basic -80/Sweet Pea/A fragrant summer flower."},
            { "404", "Common Mushroom/40/15/Basic -81/Common Mushroom/Slightly nutty, with good texture."},
            { "406", "Wild Plum/80/10/Basic -79/Wild Plum/Tart and juicy with a pungent aroma."},
            { "408", "Hazelnut/90/12/Basic -81/Hazelnut/That's one big hazelnut!"},
            { "410", "Blackberry/20/10/Basic -79/Blackberry/An early-fall treat."},
            { "412", "Winter Root/70/10/Basic -81/Winter Root/A starchy tuber."},
            { "414", "Crystal Fruit/150/25/Basic -79/Crystal Fruit/A delicate fruit that pops up from the snow."},
            { "416", "Snow Yam/100/12/Basic -81/Snow Yam/This little yam was hiding beneath the snow."},
            { "417", "Sweet Gem Berry/3000/-300/Basic -17/Sweet Gem Berry/It's by far the sweetest thing you've ever smelled." },
            { "418", "Crocus/60/0/Basic -80/Crocus/A flower that can bloom in the winter."},
            { "420", "Red Mushroom/75/-20/Basic -81/Red Mushroom/A spotted mushroom sometimes found in caves."},
            { "421", "Sunflower/80/18/Basic -80/Sunflower/A common misconception is that the flower turns so it's always facing the sun."},
            { "422", "Purple Mushroom/250/50/Basic -81/Purple Mushroom/A rare mushroom found deep in caves."},
            { "425", "Fairy Seeds/100/-300/Seeds -74/Fairy Seeds/Plant in fall. Takes 12 days to produce a mysterious flower. Assorted Colors."},
            { "427", "Tulip Bulb/10/-300/Seeds -74/Tulip Bulb/Plant in spring. Takes 6 days to produce a colorful flower. Assorted colors."},
            { "429", "Jazz Seeds/15/-300/Seeds -74/Jazz Seeds/Plant in spring. Takes 7 days to produce a blue puffball flower."},
            { "431", "Sunflower Seeds/20/-300/Seeds -74/Sunflower Seeds/Plant in summer or fall. Takes 8 days to produce a large sunflower. Yields more seeds at harvest."},
            { "433", "Coffee Bean/15/-300/Seeds -74/Coffee Bean/Plant in spring or summer to grow a coffee plant. Place five beans in a keg to make coffee."},
            { "453", "Poppy Seeds/50/-300/Seeds -74/Poppy Seeds/Plant in summer. Produces a bright red flower in 7 days."},
            { "454", "Ancient Fruit/550/-300/Basic -79/Ancient Fruit/It's been dormant for eons."},
            { "455", "Spangle Seeds/25/-300/Seeds -74/Spangle Seeds/Plant in summer. Takes 8 days to produce a vibrant tropical flower. Assorted colors."},
            { "472", "Parsnip Seeds/10/-300/Seeds -74/Parsnip Seeds/Plant these in the spring. Takes 4 days to mature."},
            { "473", "Bean Starter/30/-300/Seeds -74/Bean Starter/Plant these in the spring. Takes 10 days to mature, but keeps producing after that. Grows on a trellis."},
            { "474", "Cauliflower Seeds/40/-300/Seeds -74/Cauliflower Seeds/Plant these in the spring. Takes 12 days to produce a large cauliflower."},
            { "475", "Potato Seeds/25/-300/Seeds -74/Potato Seeds/Plant these in the spring. Takes 6 days to mature, and has a chance of yielding multiple potatoes at harvest."},
            { "476", "Garlic Seeds/20/-300/Seeds -74/Garlic Seeds/Plant these in the spring. Takes 4 days to mature."},
            { "477", "Kale Seeds/35/-300/Seeds -74/Kale Seeds/Plant these in the spring. Takes 6 days to mature. Harvest with the scythe."},
            { "478", "Rhubarb Seeds/50/-300/Seeds -74/Rhubarb Seeds/Plant these in the spring. Takes 13 days to mature."},
            { "479", "Melon Seeds/40/-300/Seeds -74/Melon Seeds/Plant these in the summer. Takes 12 days to mature."},
            { "480", "Tomato Seeds/25/-300/Seeds -74/Tomato Seeds/Plant these in the summer. Takes 11 days to mature, and continues to produce after first harvest."},
            { "481", "Blueberry Seeds/40/-300/Seeds -74/Blueberry Seeds/Plant these in the summer. Takes 13 days to mature, and continues to produce after first harvest."},
            { "482", "Pepper Seeds/20/-300/Seeds -74/Pepper Seeds/Plant these in the summer. Takes 5 days to mature, and continues to produce after first harvest."},
            { "483", "Wheat Seeds/5/-300/Seeds -74/Wheat Seeds/Plant these in the summer or fall. Takes 4 days to mature. Harvest with the scythe."},
            { "484", "Radish Seeds/20/-300/Seeds -74/Radish Seeds/Plant these in the summer. Takes 6 days to mature."},
            { "485", "Red Cabbage Seeds/50/-300/Seeds -74/Red Cabbage Seeds/Plant these in the summer. Takes 9 days to mature."},
            { "486", "Starfruit Seeds/200/-300/Seeds -74/Starfruit Seeds/Plant these in the summer. Takes 13 days to mature."},
            { "487", "Corn Seeds/75/-300/Seeds -74/Corn Seeds/Plant these in the summer or fall. Takes 14 days to mature, and continues to produce after first harvest."},
            { "488", "Eggplant Seeds/10/-300/Seeds -74/Eggplant Seeds/Plant these in the fall. Takes 5 days to mature, and continues to produce after first harvest."},
            { "489", "Artichoke Seeds/15/-300/Seeds -74/Artichoke Seeds/Plant these in the fall. Takes 8 days to mature."},
            { "490", "Pumpkin Seeds/50/-300/Seeds -74/Pumpkin Seeds/Plant these in the fall. Takes 13 days to mature."},
            { "491", "Bok Choy Seeds/25/-300/Seeds -74/Bok Choy Seeds/Plant these in the fall. Takes 4 days to mature."},
            { "492", "Yam Seeds/30/-300/Seeds -74/Yam Seeds/Plant these in the fall. Takes 10 days to mature."},
            { "493", "Cranberry Seeds/120/-300/Seeds -74/Cranberry Seeds/Plant these in the fall. Takes 7 days to mature, and continues to produce after first harvest."},
            { "494", "Beet Seeds/10/-300/Seeds -74/Beet Seeds/Plant these in the fall. Takes 6 days to mature."},
            { "499", "Ancient Seeds/30/-300/Seeds -74/Ancient Seeds/Could these still grow?"},
            { "591", "Tulip/30/18/Basic -80/Tulip/The most popular spring flower. Has a very faint sweet smell."},
            { "593", "Summer Spangle/90/18/Basic -80/Summer Spangle/A tropical bloom that thrives in the humid summer air. Has a sweet, tangy aroma."},
            { "595", "Fairy Rose/290/18/Basic -80/Fairy Rose/An old folk legend suggests that the sweet smell of this flower attracts fairies."},
            { "597", "Blue Jazz/50/18/Basic -80/Blue Jazz/The flower grows in a sphere to invite as many butterflies as possible."},
            { "613", "Apple/100/15/Basic -79/Apple/A crisp fruit used for juice and cider."},
            { "634", "Apricot/50/15/Basic -79/Apricot/A tender little fruit with a rock-hard pit."},
            { "635", "Orange/100/15/Basic -79/Orange/Juicy, tangy, and bursting with sweet summer aroma."},
            { "636", "Peach/140/15/Basic -79/Peach/It's almost fuzzy to the touch."},
            { "637", "Pomegranate/140/15/Basic -79/Pomegranate/Within the fruit are clusters of juicy seeds."},
            { "638", "Cherry/80/15/Basic -79/Cherry/It's popular, and ripens sooner than most other fruits."},
            { "745", "Strawberry Seeds/0/-300/Seeds -74/Strawberry Seeds/Plant these in spring. Takes 8 days to mature, and keeps producing strawberries after that."},
            { "770", "Mixed Seeds/0/-300/Seeds -74/Mixed Seeds/There's a little bit of everything here. Plant them and see what grows!"},
            { "802", "Cactus Seeds/0/-300/Seeds -74/Cactus Seeds/Can only be grown indoors. Takes 12 days to mature, and then produces fruit every 3 days."},
            { "815", "Tea Leaves/50/-300/Basic -75/Tea Leaves/The young leaves of the tea plant. Can be brewed into the popular, energizing beverage."},
            { "830", "Taro Root/100/15/Basic -75/Taro Root/This starchy root is one of the most ancient crops."},
            { "831", "Taro Tuber/20/-300/Seeds -74/Taro Tuber/Plant these in warm weather. Takes 10 days to mature. Grows faster if planted near a body of water."},
            { "832", "Pineapple/300/55/Basic -79/Pineapple/A sweet and tangy tropical treat."},
            { "833", "Pineapple Seeds/240/-300/Seeds -74/Pineapple Seeds/Plant these in warm weather. Takes 14 days to mature, and keeps producing fruit after that." },
        };

        // static void Main(string[] args)
        // {
        //     // Display the number of command line arguments.
        //     ModData.initUtility();
        // }

        public static ModData initUtility(ModConfig config)
        {
            var cropAndObjectData = new ModData();
            var flowerSeedIndexes = new string[] { "425", "427", "429", "453", "455", "431" };
            var seasonCrops = new Dictionary<string, List<string>>
            {
                { "spring", new List<string>() },
                { "summer", new List<string>() },
                { "fall", new List<string>() },
                { "winter", new List<string>() }
            };

            //default values for crop growth ranges
            var growthRangeShortMin = 5;
            var growthRangeShortMax = 8;
            var growthRangeMediumMin = 9;
            var growthRangeMediumMax = 15;
            var growthRangeLongMin = 16;
            var growthRangeLongMax = 25;

            //crop seed and price range gold per day multipliers
            var regularSeedGPDMultiplierMin = 2;
            var regularSeedGPDMultiplierMax = 5;
            var regularCropGPDMultiplierMin = 10;
            var regularCropGPDMultiplierMax = 15;
            var regrowSeedGPDMultiplierMin = 4;
            var regrowSeedGPDMultiplierMax = 7;
            var regrowCropGPDMultiplierMin = 8;
            var regrowCropGPDMultiplierMax = 11;

            Console.WriteLine($"Growth range (Short): {growthRangeShortMin} - {growthRangeShortMax}");
            Console.WriteLine($"Growth range (Medium): {growthRangeMediumMin} - {growthRangeMediumMax}");
            Console.WriteLine($"Growth range (Long): {growthRangeLongMin} - {growthRangeLongMax}");

            //separate each crop/seed into seasons
            foreach (var crop in cropAndObjectData.CropData)
            {
                var seasons = crop.Value.Split('/')[1].Split(' ');
                foreach (var season in seasons)
                {
                    seasonCrops[season].Add(crop.Key);
                }
            }

            //loop through each season
            foreach (var season in seasonCrops)
            {
                Console.WriteLine($"{season.Key.ToUpper()}");

                var seasonCropPool = new List<string>(seasonCrops[season.Key].Shuffle());
                var totalSeasonCrops = seasonCropPool.Count;

                //set number of crops per season that are allowed to regrow
                var totalRegrowthCropsPercentage = 0.40;
                var totalRegrowthCrops = Math.Ceiling(totalSeasonCrops * totalRegrowthCropsPercentage);
                Console.WriteLine($"total regrowth crops: {totalRegrowthCrops}");

                //set how many crops per season will fall into short, medium, and long-term harvests
                //medium crop percentage will end up being percentage difference leftover after removing long and short crop percentages from 100%
                var totalShortCropsPercentage = 0.20;
                var totalLongCropsPercentage = 0.20;
                var totalMediumCropsPercentage = 1 - (totalShortCropsPercentage + totalLongCropsPercentage);
                var totalShortCrops = Math.Ceiling(totalSeasonCrops * totalShortCropsPercentage);
                var totalLongCrops = Math.Ceiling(totalSeasonCrops * totalLongCropsPercentage);
                var totalMediumCrops = Math.Ceiling(totalSeasonCrops * totalMediumCropsPercentage);
                var totalCropTypeSum = totalShortCrops + totalMediumCrops + totalLongCrops;

                //get total crops that will be allowed to have extra yields
                var totalExtraYieldCropsPercentage = Helpers.GetRandomIntegerInRange(10, 15);
                var totalExtraYieldCrops = Math.Ceiling((totalSeasonCrops * totalExtraYieldCropsPercentage) * 0.01);
                Console.WriteLine($"total extra yields for {season.Key}: {totalExtraYieldCrops}");

                //if the sum of calculated harvest categories does not equal actual crops in season
                //remove/add difference from medium harvest length crops
                if (totalCropTypeSum > totalSeasonCrops) totalMediumCrops -= totalCropTypeSum - totalSeasonCrops;
                if (totalCropTypeSum < totalSeasonCrops) totalMediumCrops += totalSeasonCrops - totalCropTypeSum;
                Console.WriteLine($"short:{totalShortCrops} medium:{totalMediumCrops} long:{totalLongCrops}");
                Console.WriteLine($"total crops for: {totalShortCrops + totalMediumCrops + totalLongCrops}");

                //loop through each individual season's crop index array
                for (int seasonCropIdx = 0; seasonCropIdx < seasonCropPool.Count; seasonCropIdx++)
                {
                    var seedIdx = seasonCropPool[seasonCropIdx];
                    var objIdx = cropAndObjectData.CropData[seedIdx].Split('/')[3];
                    Console.WriteLine($"seedIdx: {seedIdx}");
                    Console.WriteLine($"objIdx: {objIdx}");
                    var item = new Dictionary<string, string[]>
                    {
                        { "cropData", cropAndObjectData.CropData[seedIdx].Split('/') },
                        { "seedObjData", cropAndObjectData.ObjectData[seedIdx].Split('/') },
                        { "cropObjData", cropAndObjectData.ObjectData[objIdx].Split('/') },
                    };

                    //generate random growth (harvest) times for different crops
                    //manually set Parsnip to be a short-term crop since it's the only crop
                    //you have access to start making money from at the start of new game
                    var totalGrowthTime = Helpers.GetRandomIntegerInRange(growthRangeMediumMin, growthRangeMediumMax);
                    if (totalShortCrops > 0 || seedIdx == "472")
                    {
                        //for all crops that are NOT Parsnip
                        if (seedIdx != "472")
                        {
                            totalGrowthTime = Helpers.GetRandomIntegerInRange(growthRangeShortMin, growthRangeShortMax);
                        }
                        else
                        {
                            //for Parsnip
                            if (totalShortCrops == 0) totalMediumCrops--;
                            totalGrowthTime = 4;
                        }
                        totalShortCrops--;
                    }
                    else if (totalLongCrops > 0)
                    {
                        totalGrowthTime = Helpers.GetRandomIntegerInRange(growthRangeLongMin, growthRangeLongMax);
                        totalLongCrops--;
                    }
                    Console.WriteLine($"total growth time: {totalGrowthTime} days");

                    //dynamically generate growth stages
                    var growthStagesArr = Array.ConvertAll(item["cropData"][0].Split(' '), s => int.Parse(s));
                    var averageGrowthStageDays = totalGrowthTime / growthStagesArr.Length;
                    for (var i = 0; i < growthStagesArr.Length; i++)
                    {
                        growthStagesArr[i] = averageGrowthStageDays;
                    }

                    //if total sum of growth stages is less than total grow time, remove the difference from last stage
                    var growthStagesSum = growthStagesArr.Aggregate((total, next) => total + next);

                    if (growthStagesSum < totalGrowthTime)
                    {
                        growthStagesArr[growthStagesArr.Length - 1] += totalGrowthTime - growthStagesSum;
                    }
                    //if last growth stage is at least 2 days higher than previous day,
                    //distribute excess to previous day
                    var growthDayDiff = growthStagesArr.Last() - growthStagesArr[growthStagesArr.Length - 2];
                    if (growthDayDiff > 1)
                    {
                        growthStagesArr[growthStagesArr.Length - 1]--;
                        growthStagesArr[growthStagesArr.Length - 2]++;
                    }
                    //shuffles array so growth stage positions are randomized
                    var newGrowthStagesArr = Array.ConvertAll(growthStagesArr, i => i.ToString()).Shuffle();
                    item["cropData"][0] = string.Join(' ', newGrowthStagesArr);

                    //set up dynamic description for seeds
                    var cropSeasons = string.Join(", ", item["cropData"][1].Split(' ').Select((season, idx) => Helpers.Capitalize(season)));
                    var seedDescription = $"Plant these in {Helpers.ReplaceLastOccurrence(cropSeasons, ", ", " or ")}. ";
                    var daysString = totalGrowthTime < 2 ? "day" : "days";
                    seedDescription += $"Takes {totalGrowthTime} {daysString} to mature";

                    //if crop is regrowth capable or on a trellis
                    var isTrellisCrop = Convert.ToBoolean(Helpers.Capitalize(item["cropData"][7]));
                    var isFlower = flowerSeedIndexes.All(seedIdx.Contains);
                    var flowersCanRegrow = config.flowersCanRegrow;
                    Console.WriteLine($"is flower: {flowersCanRegrow}");

                    if (totalRegrowthCrops > 0 || isTrellisCrop) applyRegrowValues();
                    else applyRegularValues();

                    //store updated description
                    item["seedObjData"][5] = seedDescription;
                    Console.WriteLine($"seed description: {seedDescription}");

                    // if crop is allowed to have extra chance for multiple harvesting
                    if (totalExtraYieldCrops <= 0)
                    {
                        item["cropData"][6] = "false";
                    }
                    else
                    {
                        //balance out values to prevent high-priced crops from 
                        var cropSellPrice = int.Parse(item["cropObjData"][1]);
                        var maxAllowedHarvest = 1;
                        var extraYieldChancePercentageMax = 5;
                        if (cropSellPrice <= 50)
                        {
                            maxAllowedHarvest = 3;
                            extraYieldChancePercentageMax = 24;
                        }
                        else if (cropSellPrice > 50 && cropSellPrice <= 125)
                        {
                            maxAllowedHarvest = 2;
                            extraYieldChancePercentageMax = 16;
                        }
                        else if (cropSellPrice > 125 && cropSellPrice <= 150)
                        {
                            maxAllowedHarvest = 2;
                            extraYieldChancePercentageMax = 8;
                        }
                        var minYieldHarvest = Helpers.GetRandomIntegerInRange(1, maxAllowedHarvest);
                        var maxYieldHarvest = Helpers.GetRandomIntegerInRange(minYieldHarvest, maxAllowedHarvest);
                        var chanceForExtraCrops = Helpers.GetRandomIntegerInRange(2, extraYieldChancePercentageMax) * 0.01;
                        item["cropData"][6] = $"true {minYieldHarvest} {maxYieldHarvest} 0 {chanceForExtraCrops}";

                        //reduce crop sell price due to extra yield chance
                        item["cropObjData"][1] = Math.Ceiling(int.Parse(item["cropObjData"][1]) * (1 - (chanceForExtraCrops * (minYieldHarvest - 1)))).ToString();

                        Console.WriteLine($"** EXTRA YIELD **");
                        Console.WriteLine($"{item["cropData"][6]}");
                        Console.WriteLine($"updated crop sell price: {item["cropObjData"][1]}");
                        totalExtraYieldCrops--;
                    }

                    //calculates and prints out gold per day/month per plot
                    {
                        double maxHarvests = 1;
                        double extraSeedPurchaseMultiplier = 1;
                        double sellPricePerHarvest = int.Parse(item["cropObjData"][1]);
                        double seedPurchasePrice = int.Parse(item["seedObjData"][1]);
                        double daysToRegrow = int.Parse(item["cropData"][4]);
                        double daysToMaturity = item["cropData"][0].Split(' ').Select(day => int.Parse(day)).Aggregate(0, (total, next) => total + next);
                        double growingDays = 1;
                        var extraYieldsVal = item["cropData"][6];

                        //if regrow capable
                        if (int.Parse(item["cropData"][4]) > -1) {
                            maxHarvests = Math.Floor((27 - daysToMaturity) / daysToRegrow + 1);
                            growingDays = daysToMaturity + (maxHarvests - 1) * daysToRegrow;
                        } else {
                            maxHarvests = Math.Floor(27 / daysToMaturity);
                            extraSeedPurchaseMultiplier = maxHarvests;
                            growingDays = daysToMaturity + (maxHarvests - 1) * daysToMaturity;
                        }

                        //if crop has extra yield potential
                        if (extraYieldsVal.Length > 6) {
                            sellPricePerHarvest *= int.Parse(extraYieldsVal.Split(' ')[1]);
                        }

                        Console.WriteLine($"days to maturity: {daysToMaturity}");
                        Console.WriteLine($"max harvests: {maxHarvests}");
                        Console.WriteLine($"days to regrow: {daysToRegrow}");
                        var goldPerDay = (maxHarvests * sellPricePerHarvest - seedPurchasePrice * extraSeedPurchaseMultiplier) / growingDays;
                        Console.WriteLine($"Gold per day (per plot): {Math.Round(goldPerDay, 2)}g");
                        Console.WriteLine($"Gold per season (per plot): {Math.Round(goldPerDay * 27, 2)}g");
                    }
                    
                    //join arrays and update crop and object data
                    cropAndObjectData.CropData[seedIdx] = string.Join('/', item["cropData"]);
                    cropAndObjectData.ObjectData[seedIdx] = string.Join('/', item["seedObjData"]);
                    cropAndObjectData.ObjectData[objIdx] = string.Join('/', item["cropObjData"]);
                    Console.WriteLine("----------------------");
                    
                    //calculation helper functions
                    void applyRegrowValues()
                    {
                        //if flower but flowers aren't allowed to regrow, exit regrowth function and apply regular values to flower instead
                        if (isFlower && !flowersCanRegrow)
                        {
                            applyRegularValues();
                            return;
                        }

                        //if more crops are allowed to be given regrowth capabilities, set regrowth time to be between 30% - 42% of total grow time.
                        var regrowthPercentage = Helpers.GetRandomIntegerInRange(30, 60);
                        item["cropData"][4] = Math.Ceiling(totalGrowthTime * (regrowthPercentage * 0.01)).ToString();
                        Console.WriteLine($"regrowth: {item["cropData"][4]} days");

                        //set crop and seed sell prices
                        var seedPriceMultiplier = Helpers.GetRandomIntegerInRange(regrowSeedGPDMultiplierMin, regrowSeedGPDMultiplierMax);
                        var cropPriceMultiplier = Helpers.GetRandomIntegerInRange(regrowCropGPDMultiplierMin, regrowCropGPDMultiplierMax);
                        item["seedObjData"][1] = (totalGrowthTime * seedPriceMultiplier).ToString();
                        item["cropObjData"][1] = (totalGrowthTime * cropPriceMultiplier).ToString();
                        Console.WriteLine($"seed price multiplier: {seedPriceMultiplier}");
                        Console.WriteLine($"crop price multiplier: {cropPriceMultiplier}");
                        Console.WriteLine($"seed purchase price: {item["seedObjData"][1]}g");
                        Console.WriteLine($"crop sell price    : {item["cropObjData"][1]}g");
                        // (total growth time x seed price multiplier):

                        //append season text with regrowth verbiage
                        seedDescription += ", but keeps producing after that." + (isTrellisCrop ? " Grows on a trellis." : "");
                        totalRegrowthCrops--;
                    }

                    void applyRegularValues()
                    {
                        //if crop is NOT regrowth capable, set crop to not regrow
                        item["cropData"][4] = "-1";
                        Console.WriteLine($"no regrowth");

                        //set crop and seed sell prices
                        var seedPriceMultiplier = Helpers.GetRandomIntegerInRange(regularSeedGPDMultiplierMin, regularSeedGPDMultiplierMax);
                        var cropPriceMultiplier = Helpers.GetRandomIntegerInRange(regularCropGPDMultiplierMin, regularCropGPDMultiplierMax);
                        item["seedObjData"][1] = (totalGrowthTime * seedPriceMultiplier).ToString();
                        item["cropObjData"][1] = (totalGrowthTime * cropPriceMultiplier).ToString();
                        Console.WriteLine($"seed price multiplier: {seedPriceMultiplier}");
                        Console.WriteLine($"crop price multiplier: {cropPriceMultiplier}");
                        Console.WriteLine($"seed purchase price: {item["seedObjData"][1]}g");
                        Console.WriteLine($"crop sell price    : {item["cropObjData"][1]}g");

                        //append period to season text
                        seedDescription += '.';
                    }
                }
            }
            return cropAndObjectData;
        }
    }
}

