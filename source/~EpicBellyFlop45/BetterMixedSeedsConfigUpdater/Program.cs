/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/EpicBellyFlop45/StardewMods
**
*************************************************/

using BetterMixedSeedsConfigUpdater.ModConfigs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace BetterMixedSeedsConfigUpdater
{
    /// <summary>The program entry point.</summary>
    public class Program
    {
        /*********
        ** Public Methods 
        *********/
        /// <summary>The program entry point.</summary>
        /// <param name="args">The passed arguments.</param>
        public static void Main(string[] args)
        {
            try
            {
                if (args.Length < 2)
                {
                    Console.WriteLine("Invalid args supplied");
                    Console.ReadLine();
                    return;
                }

                OldModConfig oldModConfig = GetOldModConfig(Path.GetFullPath(args[0]));
                NewModConfig newModConfig = ConvertToNewConfigLayout(oldModConfig);
                SerializeNewModConfig(newModConfig, args[1]);
                
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\nconfig.json successfully converted");
                Console.ForegroundColor = ConsoleColor.White;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nconfig.json failed to convert: {ex.Message}\n{ex.StackTrace}");
                Console.ForegroundColor = ConsoleColor.White;
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }


        /*********
        ** Private Methods 
        *********/
        /// <summary>Get the deserialized old config.json file.</summary>
        /// <param name="path">The path where the old config.json file is located.</param>
        /// <returns>The old config object.</returns>
        private static OldModConfig GetOldModConfig(string path)
        {
            // Deserialize old config directly from the file
            OldModConfig oldModConfig;

            using (StreamReader file = File.OpenText(path))
            {
                JsonSerializer serializer = new JsonSerializer();
                oldModConfig = (OldModConfig)serializer.Deserialize(file, typeof(OldModConfig));
            }

            return oldModConfig;
        }

        /// <summary>Serialized the new config object.</summary>
        /// <param name="newModConfig">The new config object to serialize.</param>
        /// <param name="newModConfigPath">The path the new config.json file should be located.</param>
        private static void SerializeNewModConfig(NewModConfig newModConfig, string newModConfigPath)
        {
            // Serialize the new config firectly in the NewConfig folder
            using (StreamWriter sWriter = new StreamWriter(newModConfigPath))
            using (JsonWriter jWriter = new JsonTextWriter(sWriter))
            {
                jWriter.Formatting = Formatting.Indented;

                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(jWriter, newModConfig);
            }
        }

        /// <summary>Convert the old config object to the new layout.</summary>
        /// <param name="oldModConfig">The old config object to convert.</param>
        /// <returns>The converted config object.</returns>
        private static NewModConfig ConvertToNewConfigLayout(OldModConfig oldModConfig)
        {
            NewModConfig newModConfig = new NewModConfig();

            newModConfig.PercentDropChanceForMixedSeedsWhenNotFiber = oldModConfig.PercentDropChanceForMixedSeedsWhenNotFiber;
            newModConfig.StardewValley = new CropMod
            (
                spring: new Season(
                    new List<Crop>
                    {
                        new Crop("Ancient Fruit", oldModConfig.UseAncientFruit_SPRING, 1),
                        new Crop("Blue Jazz", oldModConfig.UseBlueJazz, 1),
                        new Crop("Cauliflower", oldModConfig.UseCauliflower, 1),
                        new Crop("Coffee Bean", oldModConfig.UseCoffeeBean, 1),
                        new Crop("Garlic", oldModConfig.UseGarlic, 1),
                        new Crop("Green Bean", oldModConfig.UseGreenBean, 1),
                        new Crop("Kale", oldModConfig.UseKale, 1),
                        new Crop("Parsnip", oldModConfig.UseParsnip, 1),
                        new Crop("Potato", oldModConfig.UsePotato, 1),
                        new Crop("Rhubarb", oldModConfig.UseRhubarb, 1),
                        new Crop("Strawberry", oldModConfig.UseStrawberry, 1),
                        new Crop("Tulip", oldModConfig.UseTulip, 1)
                    }),
                summer: new Season(
                    new List<Crop>
                    {
                        new Crop("Ancient Fruit", oldModConfig.UseAncientFruit_SUMMER, 1),
                        new Crop("Blueberry", oldModConfig.UseBlueberry, 1),
                        new Crop("Corn", oldModConfig.UseBlue_Corn_SUMMER, 1),
                        new Crop("Hops", oldModConfig.UseHops, 1),
                        new Crop("Hot Pepper", oldModConfig.UseHotPepper, 1),
                        new Crop("Melon", oldModConfig.UseMelon, 1),
                        new Crop("Poppy", oldModConfig.UsePoppy, 1),
                        new Crop("Radish", oldModConfig.UseRadish, 1),
                        new Crop("Red Cabbage", oldModConfig.UseRedCabbage, 1),
                        new Crop("Starfruit", oldModConfig.UseStarfruit, 1),
                        new Crop("Summer Spangle", oldModConfig.UseSummerSpangle, 1),
                        new Crop("Sunflower", oldModConfig.UseSunflower_SUMMER, 1),
                        new Crop("Tomato", oldModConfig.UseTomato, 1),
                        new Crop("Wheat", oldModConfig.UseWheat_SUMMER, 1)
                    }),
                fall: new Season(
                    new List<Crop>
                    {
                        new Crop("Ancient Fruit", oldModConfig.UseAncientFruit_FALL, 1),
                        new Crop("Amaranth", oldModConfig.UseAmaranth, 2),
                        new Crop("Artichoke", oldModConfig.UseArtichoke, 1),
                        new Crop("Beet", oldModConfig.UseBeet, 1),
                        new Crop("Bok Choy", oldModConfig.UseBokChoy, 1),
                        new Crop("Corn", oldModConfig.UseCorn_FALL, 1),
                        new Crop("Cranberries", oldModConfig.UseCranberries, 1),
                        new Crop("Eggplant", oldModConfig.UseEggplant, 1),
                        new Crop("Fairy Rose", oldModConfig.UseFairyRose, 1),
                        new Crop("Grape", oldModConfig.UseGrape, 1),
                        new Crop("Pumpkin", oldModConfig.UsePumpkin, 1),
                        new Crop("Sunflower", oldModConfig.UseSunflower_FALL, 1),
                        new Crop("Sweet Gem Berry", oldModConfig.UseSweetGemBerry, 1),
                        new Crop("Wheat", oldModConfig.UseWheat_FALL, 1),
                        new Crop("Yam", oldModConfig.UseYam, 1)
                    }),
                winter: null
            );

            newModConfig.FantasyCrops = new CropMod
            (
                spring: new Season(
                    new List<Crop>
                    {
                        new Crop("Coal Root", oldModConfig.UseCoal_Root_SPRING, 1),
                        new Crop("Copper Leaf", oldModConfig.UseCopper_Leaf, 1)
                    }),
                summer: new Season(
                    new List<Crop>
                    {
                        new Crop("Coal Root", oldModConfig.UseCoal_Root_SUMMER, 1),
                        new Crop("Iron Leaf", oldModConfig.UseIron_Leaf, 1)
                    }),
                fall: new Season(
                    new List<Crop>
                    {
                        new Crop("Gold Leaf", oldModConfig.UseGold_Leaf, 1),
                        new Crop("Money Plant", oldModConfig.UseMoney_Plant, 1)
                    }),
                winter: new Season(
                    new List<Crop>
                    {
                        new Crop("Iridium Fern", oldModConfig.UseIridium_Fern, 1)
                    })
            );

            newModConfig.FreshMeat = new CropMod
            (
                spring: new Season(
                    new List<Crop>
                    {
                        new Crop("Beef", oldModConfig.UseBeef_SPRING, 1),
                        new Crop("Chicken", oldModConfig.UseChicken_SPRING, 1),
                        new Crop("Mutton", oldModConfig.UseMutton_SPRING, 1),
                        new Crop("Rabbit", oldModConfig.UseRabbit_SPRING, 1)
                    }),
                summer: new Season(
                    new List<Crop>
                    {
                        new Crop("Beef", oldModConfig.UseBeef_SUMMER, 1),
                        new Crop("Chevon", oldModConfig.UseChevon_SUMMER, 1),
                        new Crop("Chicken", oldModConfig.UseChicken_SUMMER, 1),
                        new Crop("Duck", oldModConfig.UseDuck_SUMMER, 1),
                        new Crop("Mutton", oldModConfig.UseMutton_SUMMER, 1),
                        new Crop("Pork", oldModConfig.UsePork_SUMMER, 1),
                        new Crop("Rabbit", oldModConfig.UseRabbit_SUMMER, 1)
                    }),
                fall: new Season(
                    new List<Crop>
                    {
                        new Crop("Chevon", oldModConfig.UseChevon_FALL, 1),
                        new Crop("Duck", oldModConfig.UseDuck_FALL, 1),
                        new Crop("Pork", oldModConfig.UsePork_FALL, 1)
                    }),
                winter: null
            );

            newModConfig.FruitAndVeggies = new CropMod
            (
                spring: new Season(
                    new List<Crop>
                    {
                        new Crop("Basil", oldModConfig.UseBasil, 1),
                        new Crop("Cabbage", oldModConfig.UseCabbage_SPRING, 1),
                        new Crop("Muskmelon", oldModConfig.UseMuskmelon, 1),
                        new Crop("Onion", oldModConfig.UseOnion_FruitAndVeggies, 1),
                        new Crop("Parsley", oldModConfig.UseParsley, 1),
                        new Crop("Passion Fruit", oldModConfig.UsePassion_Fruit, 1),
                        new Crop("Pineapple", oldModConfig.UsePineapple_FruitAndVeggies, 1),
                        new Crop("Rice", oldModConfig.UseRice_SPRING, 1),
                        new Crop("Spinach", oldModConfig.UseSpinach_SPRING, 1),
                        new Crop("Sugar Beet", oldModConfig.UseSugar_Beet, 1),
                        new Crop("Sweet Canary Melon", oldModConfig.UseSweet_Canary_Melon, 1),
                        new Crop("Tea", oldModConfig.UseTea_SPRING, 1)
                    }),
                summer: new Season(
                    new List<Crop>
                    {
                        new Crop("Adzuki Bean", oldModConfig.UseAdzuki_Bean, 1),
                        new Crop("Aloe", oldModConfig.UseAloe, 1),
                        new Crop("Cassava", oldModConfig.UseCassava, 1),
                        new Crop("Chives", oldModConfig.UseChives, 1),
                        new Crop("Cotton", oldModConfig.UseCotton_SUMMER, 1),
                        new Crop("Cucumber", oldModConfig.UseCucumber_FruitAndVeggies, 1),
                        new Crop("Gooseberry", oldModConfig.UseGooseberry, 1),
                        new Crop("Green Pea", oldModConfig.UseGreen_Pea, 1),
                        new Crop("Kiwi", oldModConfig.UseKiwi_SUMMER, 1),
                        new Crop("Lettuce", oldModConfig.UseLettuce, 1),
                        new Crop("Navy Bean", oldModConfig.UseNavy_Bean, 1),
                        new Crop("Oregano", oldModConfig.UseOregano, 1),
                        new Crop("Raspberry", oldModConfig.UseRaspberry, 1),
                        new Crop("Rice", oldModConfig.UseRice_SUMMER, 1),
                        new Crop("Sugar Cane", oldModConfig.UseSugar_Cane, 1),
                        new Crop("Tea", oldModConfig.UseTea_SUMMER, 1),
                        new Crop("Wasabi", oldModConfig.UseWasabi, 1)
                    }),
                fall: new Season(
                    new List<Crop>
                    {
                        new Crop("Barley", oldModConfig.UseBarley, 1),
                        new Crop("Bell Pepper", oldModConfig.UseBell_Pepper, 1),
                        new Crop("Blackberry", oldModConfig.UseBlackberry, 1),
                        new Crop("Broccoli", oldModConfig.UseBroccoli, 1),
                        new Crop("Cabbage", oldModConfig.UseCabbage_FALL, 1),
                        new Crop("Carrot", oldModConfig.UseCarrot_FruitAndVeggies, 1),
                        new Crop("Celery", oldModConfig.UseCelery, 1),
                        new Crop("Cotton", oldModConfig.UseCotton_FALL, 1),
                        new Crop("Fennel", oldModConfig.UseFennel, 1),
                        new Crop("Ginger", oldModConfig.UseGinger, 1),
                        new Crop("Kiwi", oldModConfig.UseKiwi_FALL, 1),
                        new Crop("Peanut", oldModConfig.UsePeanut_FruitAndVeggies, 1),
                        new Crop("Rice", oldModConfig.UseRice_FALL, 1),
                        new Crop("Rosemary", oldModConfig.UseRosemary, 1),
                        new Crop("Sage", oldModConfig.UseSage, 1),
                        new Crop("Soybean", oldModConfig.UseSoybean, 1),
                        new Crop("Spinach", oldModConfig.UseSpinach_FALL, 1),
                        new Crop("Sweet Potato", oldModConfig.UseSweet_Potato, 1),
                        new Crop("Tea", oldModConfig.UseTea_FALL, 1),
                        new Crop("Thyme", oldModConfig.UseThyme, 1),
                        new Crop("Watermelon Mizu", oldModConfig.UseWatermelon_Mizu, 1)
                    }),
                winter: new Season(
                    new List<Crop>
                    {
                        new Crop("Elderberry", oldModConfig.UseElderberry, 1),
                        new Crop("Juniper", oldModConfig.UseJuniper, 1),
                        new Crop("Mint", oldModConfig.UseMint, 1)
                    })
            );

            newModConfig.MizusFlowers = new CropMod
            (
                spring: new Season(
                    new List<Crop>
                    {
                        new Crop("Chamomile", oldModConfig.UseChamomile, 1),
                        new Crop("Honeysuckle", oldModConfig.UseHoneysuckle_SPRING, 1),
                        new Crop("Pink Cat", oldModConfig.UsePink_Cat, 1),
                        new Crop("Rose", oldModConfig.UseRose_SPRING, 1),
                        new Crop("Shaded Violet", oldModConfig.UseShaded_Violet, 1),
                        new Crop("Spring Rose", oldModConfig.UseSpring_Rose, 1)
                    }),
                summer: new Season(
                    new List<Crop>
                    {
                        new Crop("Blue Mist", oldModConfig.UseBlue_Mist, 1),
                        new Crop("Clary Sage", oldModConfig.UseClary_Sage, 1),
                        new Crop("Fragrant Lilac", oldModConfig.UseFragrant_Lilac, 1),
                        new Crop("Herbal Lavender", oldModConfig.UseHerbal_Lavender, 1),
                        new Crop("Honeysuckle", oldModConfig.UseHoneysuckle_SUMMER, 1),
                        new Crop("Passion Flower", oldModConfig.UsePassion_Flower, 1),
                        new Crop("Rose", oldModConfig.UseRose_SUMMER, 1),
                        new Crop("Summer Rose", oldModConfig.UseSummer_Rose, 1)
                    }),
                fall: new Season(
                    new List<Crop>
                    {
                        new Crop("Bee Balm", oldModConfig.UseBee_Balm, 1),
                        new Crop("Fairy Duster", oldModConfig.UseFairy_Duster, 1),
                        new Crop("Fall Rose", oldModConfig.UseFall_Rose, 1),
                        new Crop("Purple Coneflower", oldModConfig.UsePurple_Coneflower, 1),
                        new Crop("Rose", oldModConfig.UseRose_FALL, 1),
                        new Crop("Sweet Jasmine", oldModConfig.UseSweet_Jasmine, 1)
                    }),
                winter: null
            );

            newModConfig.CannabisKit = new CropMod
            (
                spring: new Season(
                    new List<Crop>
                    {
                        new Crop("Cannabis", oldModConfig.UseCannabis_SPRING, 1),
                        new Crop("Hemp", oldModConfig.UseHemp_SPRING, 1),
                        new Crop("Tobacco", oldModConfig.UseTobacco_SPRING, 1)
                    }),
                summer: new Season(
                    new List<Crop>
                    {
                        new Crop("Blue Dream", oldModConfig.UseBlue_Dream_SUMMER, 1),
                        new Crop("Cannabis", oldModConfig.UseCannabis_SUMMER, 1),
                        new Crop("Girl Scout Cookies", oldModConfig.UseGirl_Scout_Cookies_SUMMER, 1),
                        new Crop("Green Crack", oldModConfig.UseGreen_Crack_SUMMER, 1),
                        new Crop("Hemp", oldModConfig.UseHemp_SUMMER, 1),
                        new Crop("Hybrid", oldModConfig.UseHybrid_SUMMER, 1),
                        new Crop("Indica", oldModConfig.UseIndica, 1),
                        new Crop("Northern Lights", oldModConfig.UseNorthern_Lights, 1),
                        new Crop("OG Kush", oldModConfig.UseOG_Kush_SUMMER, 1),
                        new Crop("Strawberry Cough", oldModConfig.UseStrawberry_Cough_SUMMER, 1),
                        new Crop("Tobacco", oldModConfig.UseTobacco_SUMMER, 1),
                        new Crop("White Widow", oldModConfig.UseWhite_Widow_SUMMER, 1)
                    }),
                fall: new Season(
                    new List<Crop>
                    {
                        new Crop("Blue Dream", oldModConfig.UseBlue_Dream_FALL, 1),
                        new Crop("Cannabis", oldModConfig.UseCannabis_FALL, 1),
                        new Crop("Girl Scout Cookies", oldModConfig.UseGirl_Scout_Cookies_FALL, 1),
                        new Crop("Green Crack", oldModConfig.UseGreen_Crack_FALL, 1),
                        new Crop("Hemp", oldModConfig.UseHemp_FALL, 1),
                        new Crop("Hybrid", oldModConfig.UseHybrid_FALL, 1),
                        new Crop("OG Kush", oldModConfig.UseOG_Kush_FALL, 1),
                        new Crop("Sativa", oldModConfig.UseSativa, 1),
                        new Crop("Sour Diesel", oldModConfig.UseSour_Diesel, 1),
                        new Crop("Strawberry Cough", oldModConfig.UseStrawberry_Cough_FALL, 1),
                        new Crop("White Widow", oldModConfig.UseWhite_Widow_FALL, 1)
                    }),
                winter: null
            );

            newModConfig.SixPlantableCrops = new CropMod
            (
                spring: null,
                summer: null,
                fall: null,
                winter: new Season(
                    new List<Crop>
                    {
                        new Crop("Blue Rose", oldModConfig.UseBlue_Rose, 1),
                        new Crop("Daikon", oldModConfig.UseDaikon, 1),
                        new Crop("Gentian", oldModConfig.UseGentian, 1),
                        new Crop("Napa Cabbage", oldModConfig.UseNapa_Cabbage, 1),
                        new Crop("Snowdrop", oldModConfig.UseSnowdrop, 1),
                        new Crop("Winter Broccoli", oldModConfig.UseWinter_Broccoli, 1)
                    })
            );

            newModConfig.BonsterCrops = new CropMod
            (
                spring: new Season(
                    new List<Crop>
                    {
                        new Crop("Cranberry Bean", oldModConfig.UseCranberry_Beans, 1),
                        new Crop("Red Currant", oldModConfig.UseRedCurrant, 1),
                        new Crop("Rose Hip", oldModConfig.UseRose_Hips_SPRING, 1)
                    }),
                summer: new Season(
                    new List<Crop>
                    {
                        new Crop("Blackcurrant", oldModConfig.UseBlackcurrant, 1),
                        new Crop("Blue Corn", oldModConfig.UseBlue_Corn_SUMMER, 1),
                        new Crop("Cardamom", oldModConfig.UseCardamom, 1),
                        new Crop("Maypop", oldModConfig.UseMaypop, 1),
                        new Crop("Peppercorn", oldModConfig.UsePeppercorn_SUMMER, 1),
                        new Crop("Rose Hip", oldModConfig.UseRose_Hips_SUMMER, 1),
                        new Crop("Roselle Hibiscus", oldModConfig.UseRoselle_Hibiscus, 1),
                        new Crop("Summer Squash", oldModConfig.UseSummer_Squash, 1),
                        new Crop("Taro", oldModConfig.UseTaro_SUMMER, 1)
                    }),
                fall: new Season(
                    new List<Crop>
                    {
                        new Crop("Blue Corn", oldModConfig.UseBlue_Corn_FALL, 1),
                        new Crop("Peppercorn", oldModConfig.UsePeppercorn_FALL, 1),
                        new Crop("Taro", oldModConfig.UseTaro_FALL, 1),
                        new Crop("White Currant", oldModConfig.UseWhite_Currant, 1)
                    }),
                winter: null
            );

            newModConfig.RevenantCrops = new CropMod
            (
                spring: new Season(
                    new List<Crop>
                    {
                        new Crop("Enoki Mushroom", oldModConfig.UseEnoki_Mushroom_SPRING, 1)
                    }),
                summer: new Season(
                    new List<Crop>
                    {
                        new Crop("Enoki Mushroom", oldModConfig.UseEnoki_Mushroom_SUMMER, 1)
                    }),
                fall: new Season(
                    new List<Crop>
                    {
                        new Crop("Maitake Mushroom", oldModConfig.UseMaitake_Mushroom, 1),
                        new Crop("Oyster Mushroom", oldModConfig.UseOyster_Mushroom, 1)
                    }),
                winter: new Season(
                    new List<Crop>
                    {
                        new Crop("Gai Lan", oldModConfig.UseGai_Lan, 1)
                    })
            );

            newModConfig.FarmerToFlorist = new CropMod
            (
                spring: new Season(
                    new List<Crop>
                    {
                        new Crop("Camellia", oldModConfig.UseCamellia_SPRING, 1),
                        new Crop("Carnation", oldModConfig.UseCarnation_SPRING, 1),
                        new Crop("Delphinium", oldModConfig.UseDelphinium, 1),
                        new Crop("Herbal Peony", oldModConfig.UseHerbalPeony, 1),
                        new Crop("Hyacinth", oldModConfig.UseHyacinth_FarmerToFlorist, 1),
                        new Crop("Lilac", oldModConfig.UseLilac, 1),
                        new Crop("Violet", oldModConfig.UseViolet, 1),
                        new Crop("Wisteria", oldModConfig.UseWisteria, 1)
                    }),
                summer: new Season(
                    new List<Crop>
                    {
                        new Crop("Allium", oldModConfig.UseAllium, 1),
                        new Crop("Carnation", oldModConfig.UseCarnation_SUMMER, 1),
                        new Crop("Hydrangea", oldModConfig.UseHydrangea, 1),
                        new Crop("Lavender", oldModConfig.UseLavender, 1),
                        new Crop("Lily", oldModConfig.UseLily, 1),
                        new Crop("Lotus", oldModConfig.UseLotus, 1),
                        new Crop("Petunia", oldModConfig.UsePetunia, 1)
                    }),
                fall: new Season(
                    new List<Crop>
                    {
                        new Crop("Camellia", oldModConfig.UseCamellia_FALL, 1),
                        new Crop("Chrysanthemum", oldModConfig.UseChrysanthemum, 1),
                        new Crop("Clematis", oldModConfig.UseClematis, 1),
                        new Crop("Dahlia", oldModConfig.UseDahlia, 1),
                        new Crop("English Rose", oldModConfig.UseEnglish_Rose, 1),
                        new Crop("Freesia", oldModConfig.UseFreesia, 1),
                        new Crop("Geranium", oldModConfig.UseGeranium, 1),
                        new Crop("Iris", oldModConfig.UseIris, 1)
                    }),
                winter: null
            );

            newModConfig.LuckyClover = new CropMod
            (
                spring: new Season(
                    new List<Crop>
                    {
                        new Crop("Lucky Clover", oldModConfig.UseLuckyClover, 1)
                    }),
                summer: null,
                fall: null,
                winter: null
            );

            newModConfig.FishsFlowers = new CropMod
            (
                spring: new Season(
                    new List<Crop>
                    {
                        new Crop("Hyacinth", oldModConfig.UseHyacinth_FishsFlowers, 1),
                        new Crop("Pansy", oldModConfig.UsePansy_SPRING, 1)
                    }),
                summer: new Season(
                    new List<Crop>
                    {
                        new Crop("Pansy", oldModConfig.UsePansy_SUMMER, 1)
                    }),
                fall: new Season(
                    new List<Crop>
                    {
                        new Crop("Pansy", oldModConfig.UsePansy_FALL, 1)
                    }),
                winter: null
            );

            newModConfig.StephansLotsOfCrops = new CropMod
            (
                spring: new Season(
                    new List<Crop>
                    {
                        new Crop("Cucumber", oldModConfig.UseCucumber_StephanLotsOfCrops, 1),
                        new Crop("Pea Pod", oldModConfig.UsePea_Pod, 1),
                        new Crop("Turnip", oldModConfig.UseTurnip, 1)
                    }),
                summer: new Season(
                    new List<Crop>
                    {
                        new Crop("Onion", oldModConfig.UseOnion_StephanLotsOfCrops, 1),
                        new Crop("Pineapple", oldModConfig.UsePineapple_StephanLotsOfCrops, 1),
                        new Crop("Watermelon", oldModConfig.UseWatermelon, 1)
                    }),
                fall: new Season(
                    new List<Crop>
                    {
                        new Crop("Carrot", oldModConfig.UseCarrot_StephanLotsOfCrops, 1),
                        new Crop("Peanut", oldModConfig.UsePeanut_StephanLotsOfCrops, 1),
                        new Crop("Spinach", oldModConfig.UseSpinach, 1)
                    }),
                winter: null
            );

            newModConfig.EemiesCrops = new CropMod
            (
                spring: null,
                summer: new Season(
                    new List<Crop>
                    {
                        new Crop("Cantaloupe Melon", oldModConfig.UseCantaloupe_Melon, 1),
                        new Crop("Charentais Melon", oldModConfig.UseCharentais_Melon, 1),
                        new Crop("Korean Melon", oldModConfig.UseKorean_Melon, 1),
                        new Crop("Large Watermelon", oldModConfig.UseLarge_Watermelon, 1),
                        new Crop("Rich Canary Melon", oldModConfig.UseRich_Canary_Melon, 1),
                        new Crop("Rich Sweetness Melon", oldModConfig.UseRich_Sweetness_Melon, 1)
                    }),
                fall: new Season(
                    new List<Crop>
                    {
                        new Crop("Acorn Squash", oldModConfig.UseAcorn_Squash, 1),
                        new Crop("Black Forest Squash", oldModConfig.UseBlack_Forest_Squash, 1),
                        new Crop("Crookneck Squash", oldModConfig.UseCrookneck_Squash, 1),
                        new Crop("Golden Hubbard Squash", oldModConfig.UseGolden_Hubbard_Squash, 1),
                        new Crop("Jack O Lantern Pumpkin", oldModConfig.UseJack_O_Lantern_Pumpkin, 1),
                        new Crop("Sweet Lightning Pumpkin", oldModConfig.UseSweet_Lightning_Pumpkin, 1)
                    }),
                winter: null
            );

            newModConfig.TeaTime = new CropMod
            (
                spring: new Season(
                    new List<Crop>
                    {
                        new Crop("Mint Tea Plant", oldModConfig.UseMint_Tea_Plant_SPRING, 1),
                        new Crop("Tea Leaf Plant", oldModConfig.UseTea_Leaf_Plant_SPRING, 1)
                    }),
                summer: new Season(
                    new List<Crop>
                    {
                        new Crop("Mint Tea Plant", oldModConfig.UseMint_Tea_Plant_SUMMER, 1),
                        new Crop("Tea Leaf Plant", oldModConfig.UseTea_Leaf_Plant_SUMMER, 1)
                    }),
                fall: new Season(
                    new List<Crop>
                    {
                        new Crop("Mint Tea Plant", oldModConfig.UseMint_Tea_Plant_FALL, 1),
                        new Crop("Tea Leaf Plant", oldModConfig.UseTea_Leaf_Plant_FALL, 1)
                    }),
                winter: null
            );

            newModConfig.ForageToFarm = new CropMod
            (
                spring: new Season(
                    new List<Crop>
                    {
                        new Crop("Cave Carrot", oldModConfig.UseCave_Carrot_SPRING, 1),
                        new Crop("Common Mushroom", oldModConfig.UseCommon_Mushroom_SPRING, 1),
                        new Crop("Daffodil", oldModConfig.UseDaffodil, 1),
                        new Crop("Dandelion", oldModConfig.UseDandelion, 1),
                        new Crop("Wild Horseradish", oldModConfig.UseWild_Horseradish, 1),
                        new Crop("Leek", oldModConfig.UseLeek, 1),
                        new Crop("Morel Mushroom", oldModConfig.UseMorel_Mushroom, 1),
                        new Crop("Salmonberry", oldModConfig.UseSalmonberry, 1),
                        new Crop("Spring Onion", oldModConfig.UseSpring_Onion, 1)
                    }),
                summer: new Season(
                    new List<Crop>
                    {
                        new Crop("Cave Carrot", oldModConfig.UseCave_Carrot_SUMMER, 1),
                        new Crop("Coconut", oldModConfig.UseCoconut, 1),
                        new Crop("Fiddlehead Fern", oldModConfig.UseFiddlehead_Fern, 1),
                        new Crop("Red Mushroom", oldModConfig.UseRed_Mushroom_SUMMER, 1),
                        new Crop("Spice Berry", oldModConfig.UseSpice_Berry, 1),
                        new Crop("Sweet Pea", oldModConfig.UseSweet_Pea, 1)
                    }),
                fall: new Season(
                    new List<Crop>
                    {
                        new Crop("Cave Carrot", oldModConfig.UseCave_Carrot_FALL, 1),
                        new Crop("Chanterelle Mushroom", oldModConfig.UseChanterelle_Mushroom, 1),
                        new Crop("Common Mushroom", oldModConfig.UseCommon_Mushroom_FALL, 1),
                        new Crop("Hazelnut", oldModConfig.UseHazelnut, 1),
                        new Crop("Purple Mushroom", oldModConfig.UsePurple_Mushroom, 1),
                        new Crop("Red Mushroom", oldModConfig.UseRed_Mushroom_FALL, 1),
                        new Crop("Wild Blackberry", oldModConfig.UseWild_Blackberry, 1),
                        new Crop("Wild Plum", oldModConfig.UseWild_Plum, 1)
                    }),
                winter: new Season(
                    new List<Crop>
                    {
                        new Crop("Crocus", oldModConfig.UseCrocus, 1),
                        new Crop("Crystal Fruit", oldModConfig.UseCrystal_Fruit, 1),
                        new Crop("Holly", oldModConfig.UseHolly, 1),
                        new Crop("Snow Yam", oldModConfig.UseSnow_Yam, 1),
                        new Crop("Winter Root", oldModConfig.UseWinter_Root, 1)
                    })
            );

            newModConfig.GemAndMineralCrops = new CropMod
            (
                spring: null,
                summer: null,
                fall: null,
                winter: new Season(
                    new List<Crop>
                    {
                        new Crop("Aerinite Root", oldModConfig.UseAerinite_Root, 1),
                        new Crop("Aquamarine", oldModConfig.UseAquamarine_Rose, 1),
                        new Crop("Celestine Flower", oldModConfig.UseCelestine_Flower, 1),
                        new Crop("Diamond Flower", oldModConfig.UseDiamond_Flower, 1),
                        new Crop("Ghost Rose", oldModConfig.UseGhost_Rose, 1),
                        new Crop("Kyanite Flower", oldModConfig.UseKyanite_Flower, 1),
                        new Crop("Opal Cat", oldModConfig.UseOpal_Cat, 1),
                        new Crop("Slate Bean", oldModConfig.UseSlate_Bean, 1),
                        new Crop("Soap Root", oldModConfig.UseSoap_Root, 1)
                    })
            );

            return newModConfig;
        }
    }
}
