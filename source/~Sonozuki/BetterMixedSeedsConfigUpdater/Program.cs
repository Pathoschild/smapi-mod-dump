/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sonozuki/StardewMods
**
*************************************************/

using BetterMixedSeedsConfigUpdater.Models.V1;
using BetterMixedSeedsConfigUpdater.Models.V2;
using BetterMixedSeedsConfigUpdater.Models.V3;
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

                var inputValid = false;
                var currentVersion = 0;
                var targetVersion = 0;
                while (!inputValid)
                {
                    Console.Write("How should the config file version be updated? V1 -> V2 (a), V1 -> V3 (b), or V2 -> V3 (c):");
                    var result = Console.ReadLine().ToLower();
                    if (result != "a" && result != "b" && result != "c")
                    {
                        Console.WriteLine($"{result} is invalid. Input 'a', 'b', or 'c' depending on the current and final config file version");
                        continue;
                    }

                    switch (result)
                    {
                        case ("a"): currentVersion = 1; targetVersion = 2; break;
                        case ("b"): currentVersion = 1; targetVersion = 3; break;
                        case ("c"): currentVersion = 2; targetVersion = 3; break;
                    }

                    inputValid = true;
                }

                // update config
                if (currentVersion == 1)
                {
                    var modConfig_V1 = DeserialiseObject<ModConfig_V1>(args[0]);
                    var modConfig_V2 = ConvertV1ToV2(modConfig_V1);
                    if (targetVersion == 2)
                        SerialiseObject(modConfig_V2, args[1]);
                    else
                    {
                        var modConfig_V3 = ConvertV2ToV3(modConfig_V2);
                        SerialiseObject(modConfig_V3, args[1]);
                    }
                }
                else
                {
                    var modConfig_V2 = DeserialiseObject<ModConfig_V2>(args[0]);
                    var modConfig_V3 = ConvertV2ToV3(modConfig_V2);
                    SerialiseObject(modConfig_V3, args[1]);
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\nconfig.json successfully converted");
                Console.ForegroundColor = ConsoleColor.White;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nconfig.json failed to convert: {ex}");
                Console.ForegroundColor = ConsoleColor.White;
            }

            Console.Read();
        }


        /*********
        ** Private Methods 
        *********/
        /// <summary>Deserialises a file.</summary>
        /// <typeparam name="T">The type to deserialise the file to.</typeparam>
        /// <param name="path">The path of the file to deserialise.</param>
        /// <returns>The deserialised file.</returns>
        private static T DeserialiseObject<T>(string path)
        {
            using (var fileStream = File.OpenText(path))
            {
                JsonSerializer serializer = new JsonSerializer();
                return (T)serializer.Deserialize(fileStream, typeof(T));
            }
        }

        /// <summary>Serialises an object.</summary>
        /// <typeparam name="T">The type of the object to serialise.</typeparam>
        /// <param name="object">The object to serialise.</param>
        /// <param name="path">THe path of the file to serialise to.</param>
        private static void SerialiseObject<T>(T @object, string path)
        {
            // ensure file exists
            if (!File.Exists(path))
                File.Create(path).Close();

            // serialise the object
            using (var streamWriter = new StreamWriter(path))
            using (var jsonWriter = new JsonTextWriter(streamWriter))
            {
                jsonWriter.Formatting = Formatting.Indented;
                new JsonSerializer().Serialize(jsonWriter, @object);
            }
        }

        /// <summary>Converts V1 config to V2 config.</summary>
        /// <param name="oldModConfig">The V1 config object to convert.</param>
        /// <returns>The converted config object.</returns>
        private static ModConfig_V2 ConvertV1ToV2(ModConfig_V1 oldModConfig)
        {
            var newModConfig = new ModConfig_V2();
            newModConfig.PercentDropChanceForMixedSeedsWhenNotFiber = oldModConfig.PercentDropChanceForMixedSeedsWhenNotFiber;
            newModConfig.StardewValley = new CropMod_V2
            (
                spring: new Season_V2(
                    new List<Crop_V2>
                    {
                        new Crop_V2("Ancient Fruit", oldModConfig.UseAncientFruit_SPRING, 1),
                        new Crop_V2("Blue Jazz", oldModConfig.UseBlueJazz, 1),
                        new Crop_V2("Cauliflower", oldModConfig.UseCauliflower, 1),
                        new Crop_V2("Coffee Bean", oldModConfig.UseCoffeeBean, 1),
                        new Crop_V2("Garlic", oldModConfig.UseGarlic, 1),
                        new Crop_V2("Green Bean", oldModConfig.UseGreenBean, 1),
                        new Crop_V2("Kale", oldModConfig.UseKale, 1),
                        new Crop_V2("Parsnip", oldModConfig.UseParsnip, 1),
                        new Crop_V2("Potato", oldModConfig.UsePotato, 1),
                        new Crop_V2("Rhubarb", oldModConfig.UseRhubarb, 1),
                        new Crop_V2("Strawberry", oldModConfig.UseStrawberry, 1),
                        new Crop_V2("Tulip", oldModConfig.UseTulip, 1)
                    }),
                summer: new Season_V2(
                    new List<Crop_V2>
                    {
                        new Crop_V2("Ancient Fruit", oldModConfig.UseAncientFruit_SUMMER, 1),
                        new Crop_V2("Blueberry", oldModConfig.UseBlueberry, 1),
                        new Crop_V2("Corn", oldModConfig.UseBlue_Corn_SUMMER, 1),
                        new Crop_V2("Hops", oldModConfig.UseHops, 1),
                        new Crop_V2("Hot Pepper", oldModConfig.UseHotPepper, 1),
                        new Crop_V2("Melon", oldModConfig.UseMelon, 1),
                        new Crop_V2("Poppy", oldModConfig.UsePoppy, 1),
                        new Crop_V2("Radish", oldModConfig.UseRadish, 1),
                        new Crop_V2("Red Cabbage", oldModConfig.UseRedCabbage, 1),
                        new Crop_V2("Starfruit", oldModConfig.UseStarfruit, 1),
                        new Crop_V2("Summer Spangle", oldModConfig.UseSummerSpangle, 1),
                        new Crop_V2("Sunflower", oldModConfig.UseSunflower_SUMMER, 1),
                        new Crop_V2("Tomato", oldModConfig.UseTomato, 1),
                        new Crop_V2("Wheat", oldModConfig.UseWheat_SUMMER, 1)
                    }),
                fall: new Season_V2(
                    new List<Crop_V2>
                    {
                        new Crop_V2("Ancient Fruit", oldModConfig.UseAncientFruit_FALL, 1),
                        new Crop_V2("Amaranth", oldModConfig.UseAmaranth, 2),
                        new Crop_V2("Artichoke", oldModConfig.UseArtichoke, 1),
                        new Crop_V2("Beet", oldModConfig.UseBeet, 1),
                        new Crop_V2("Bok Choy", oldModConfig.UseBokChoy, 1),
                        new Crop_V2("Corn", oldModConfig.UseCorn_FALL, 1),
                        new Crop_V2("Cranberries", oldModConfig.UseCranberries, 1),
                        new Crop_V2("Eggplant", oldModConfig.UseEggplant, 1),
                        new Crop_V2("Fairy Rose", oldModConfig.UseFairyRose, 1),
                        new Crop_V2("Grape", oldModConfig.UseGrape, 1),
                        new Crop_V2("Pumpkin", oldModConfig.UsePumpkin, 1),
                        new Crop_V2("Sunflower", oldModConfig.UseSunflower_FALL, 1),
                        new Crop_V2("Sweet Gem Berry", oldModConfig.UseSweetGemBerry, 1),
                        new Crop_V2("Wheat", oldModConfig.UseWheat_FALL, 1),
                        new Crop_V2("Yam", oldModConfig.UseYam, 1)
                    }),
                winter: null
            );

            newModConfig.FantasyCrops = new CropMod_V2
            (
                spring: new Season_V2(
                    new List<Crop_V2>
                    {
                        new Crop_V2("Coal Root", oldModConfig.UseCoal_Root_SPRING, 1),
                        new Crop_V2("Copper Leaf", oldModConfig.UseCopper_Leaf, 1)
                    }),
                summer: new Season_V2(
                    new List<Crop_V2>
                    {
                        new Crop_V2("Coal Root", oldModConfig.UseCoal_Root_SUMMER, 1),
                        new Crop_V2("Iron Leaf", oldModConfig.UseIron_Leaf, 1)
                    }),
                fall: new Season_V2(
                    new List<Crop_V2>
                    {
                        new Crop_V2("Gold Leaf", oldModConfig.UseGold_Leaf, 1),
                        new Crop_V2("Money Plant", oldModConfig.UseMoney_Plant, 1)
                    }),
                winter: new Season_V2(
                    new List<Crop_V2>
                    {
                        new Crop_V2("Iridium Fern", oldModConfig.UseIridium_Fern, 1)
                    })
            );

            newModConfig.FreshMeat = new CropMod_V2
            (
                spring: new Season_V2(
                    new List<Crop_V2>
                    {
                        new Crop_V2("Beef", oldModConfig.UseBeef_SPRING, 1),
                        new Crop_V2("Chicken", oldModConfig.UseChicken_SPRING, 1),
                        new Crop_V2("Mutton", oldModConfig.UseMutton_SPRING, 1),
                        new Crop_V2("Rabbit", oldModConfig.UseRabbit_SPRING, 1)
                    }),
                summer: new Season_V2(
                    new List<Crop_V2>
                    {
                        new Crop_V2("Beef", oldModConfig.UseBeef_SUMMER, 1),
                        new Crop_V2("Chevon", oldModConfig.UseChevon_SUMMER, 1),
                        new Crop_V2("Chicken", oldModConfig.UseChicken_SUMMER, 1),
                        new Crop_V2("Duck", oldModConfig.UseDuck_SUMMER, 1),
                        new Crop_V2("Mutton", oldModConfig.UseMutton_SUMMER, 1),
                        new Crop_V2("Pork", oldModConfig.UsePork_SUMMER, 1),
                        new Crop_V2("Rabbit", oldModConfig.UseRabbit_SUMMER, 1)
                    }),
                fall: new Season_V2(
                    new List<Crop_V2>
                    {
                        new Crop_V2("Chevon", oldModConfig.UseChevon_FALL, 1),
                        new Crop_V2("Duck", oldModConfig.UseDuck_FALL, 1),
                        new Crop_V2("Pork", oldModConfig.UsePork_FALL, 1)
                    }),
                winter: null
            );

            newModConfig.FruitAndVeggies = new CropMod_V2
            (
                spring: new Season_V2(
                    new List<Crop_V2>
                    {
                        new Crop_V2("Basil", oldModConfig.UseBasil, 1),
                        new Crop_V2("Cabbage", oldModConfig.UseCabbage_SPRING, 1),
                        new Crop_V2("Muskmelon", oldModConfig.UseMuskmelon, 1),
                        new Crop_V2("Onion", oldModConfig.UseOnion_FruitAndVeggies, 1),
                        new Crop_V2("Parsley", oldModConfig.UseParsley, 1),
                        new Crop_V2("Passion Fruit", oldModConfig.UsePassion_Fruit, 1),
                        new Crop_V2("Pineapple", oldModConfig.UsePineapple_FruitAndVeggies, 1),
                        new Crop_V2("Rice", oldModConfig.UseRice_SPRING, 1),
                        new Crop_V2("Spinach", oldModConfig.UseSpinach_SPRING, 1),
                        new Crop_V2("Sugar Beet", oldModConfig.UseSugar_Beet, 1),
                        new Crop_V2("Sweet Canary Melon", oldModConfig.UseSweet_Canary_Melon, 1),
                        new Crop_V2("Tea", oldModConfig.UseTea_SPRING, 1)
                    }),
                summer: new Season_V2(
                    new List<Crop_V2>
                    {
                        new Crop_V2("Adzuki Bean", oldModConfig.UseAdzuki_Bean, 1),
                        new Crop_V2("Aloe", oldModConfig.UseAloe, 1),
                        new Crop_V2("Cassava", oldModConfig.UseCassava, 1),
                        new Crop_V2("Chives", oldModConfig.UseChives, 1),
                        new Crop_V2("Cotton", oldModConfig.UseCotton_SUMMER, 1),
                        new Crop_V2("Cucumber", oldModConfig.UseCucumber_FruitAndVeggies, 1),
                        new Crop_V2("Gooseberry", oldModConfig.UseGooseberry, 1),
                        new Crop_V2("Green Pea", oldModConfig.UseGreen_Pea, 1),
                        new Crop_V2("Kiwi", oldModConfig.UseKiwi_SUMMER, 1),
                        new Crop_V2("Lettuce", oldModConfig.UseLettuce, 1),
                        new Crop_V2("Navy Bean", oldModConfig.UseNavy_Bean, 1),
                        new Crop_V2("Oregano", oldModConfig.UseOregano, 1),
                        new Crop_V2("Raspberry", oldModConfig.UseRaspberry, 1),
                        new Crop_V2("Rice", oldModConfig.UseRice_SUMMER, 1),
                        new Crop_V2("Sugar Cane", oldModConfig.UseSugar_Cane, 1),
                        new Crop_V2("Tea", oldModConfig.UseTea_SUMMER, 1),
                        new Crop_V2("Wasabi", oldModConfig.UseWasabi, 1)
                    }),
                fall: new Season_V2(
                    new List<Crop_V2>
                    {
                        new Crop_V2("Barley", oldModConfig.UseBarley, 1),
                        new Crop_V2("Bell Pepper", oldModConfig.UseBell_Pepper, 1),
                        new Crop_V2("Blackberry", oldModConfig.UseBlackberry, 1),
                        new Crop_V2("Broccoli", oldModConfig.UseBroccoli, 1),
                        new Crop_V2("Cabbage", oldModConfig.UseCabbage_FALL, 1),
                        new Crop_V2("Carrot", oldModConfig.UseCarrot_FruitAndVeggies, 1),
                        new Crop_V2("Celery", oldModConfig.UseCelery, 1),
                        new Crop_V2("Cotton", oldModConfig.UseCotton_FALL, 1),
                        new Crop_V2("Fennel", oldModConfig.UseFennel, 1),
                        new Crop_V2("Ginger", oldModConfig.UseGinger, 1),
                        new Crop_V2("Kiwi", oldModConfig.UseKiwi_FALL, 1),
                        new Crop_V2("Peanut", oldModConfig.UsePeanut_FruitAndVeggies, 1),
                        new Crop_V2("Rice", oldModConfig.UseRice_FALL, 1),
                        new Crop_V2("Rosemary", oldModConfig.UseRosemary, 1),
                        new Crop_V2("Sage", oldModConfig.UseSage, 1),
                        new Crop_V2("Soybean", oldModConfig.UseSoybean, 1),
                        new Crop_V2("Spinach", oldModConfig.UseSpinach_FALL, 1),
                        new Crop_V2("Sweet Potato", oldModConfig.UseSweet_Potato, 1),
                        new Crop_V2("Tea", oldModConfig.UseTea_FALL, 1),
                        new Crop_V2("Thyme", oldModConfig.UseThyme, 1),
                        new Crop_V2("Watermelon Mizu", oldModConfig.UseWatermelon_Mizu, 1)
                    }),
                winter: new Season_V2(
                    new List<Crop_V2>
                    {
                        new Crop_V2("Elderberry", oldModConfig.UseElderberry, 1),
                        new Crop_V2("Juniper", oldModConfig.UseJuniper, 1),
                        new Crop_V2("Mint", oldModConfig.UseMint, 1)
                    })
            );

            newModConfig.MizusFlowers = new CropMod_V2
            (
                spring: new Season_V2(
                    new List<Crop_V2>
                    {
                        new Crop_V2("Chamomile", oldModConfig.UseChamomile, 1),
                        new Crop_V2("Honeysuckle", oldModConfig.UseHoneysuckle_SPRING, 1),
                        new Crop_V2("Pink Cat", oldModConfig.UsePink_Cat, 1),
                        new Crop_V2("Rose", oldModConfig.UseRose_SPRING, 1),
                        new Crop_V2("Shaded Violet", oldModConfig.UseShaded_Violet, 1),
                        new Crop_V2("Spring Rose", oldModConfig.UseSpring_Rose, 1)
                    }),
                summer: new Season_V2(
                    new List<Crop_V2>
                    {
                        new Crop_V2("Blue Mist", oldModConfig.UseBlue_Mist, 1),
                        new Crop_V2("Clary Sage", oldModConfig.UseClary_Sage, 1),
                        new Crop_V2("Fragrant Lilac", oldModConfig.UseFragrant_Lilac, 1),
                        new Crop_V2("Herbal Lavender", oldModConfig.UseHerbal_Lavender, 1),
                        new Crop_V2("Honeysuckle", oldModConfig.UseHoneysuckle_SUMMER, 1),
                        new Crop_V2("Passion Flower", oldModConfig.UsePassion_Flower, 1),
                        new Crop_V2("Rose", oldModConfig.UseRose_SUMMER, 1),
                        new Crop_V2("Summer Rose", oldModConfig.UseSummer_Rose, 1)
                    }),
                fall: new Season_V2(
                    new List<Crop_V2>
                    {
                        new Crop_V2("Bee Balm", oldModConfig.UseBee_Balm, 1),
                        new Crop_V2("Fairy Duster", oldModConfig.UseFairy_Duster, 1),
                        new Crop_V2("Fall Rose", oldModConfig.UseFall_Rose, 1),
                        new Crop_V2("Purple Coneflower", oldModConfig.UsePurple_Coneflower, 1),
                        new Crop_V2("Rose", oldModConfig.UseRose_FALL, 1),
                        new Crop_V2("Sweet Jasmine", oldModConfig.UseSweet_Jasmine, 1)
                    }),
                winter: null
            );

            newModConfig.CannabisKit = new CropMod_V2
            (
                spring: new Season_V2(
                    new List<Crop_V2>
                    {
                        new Crop_V2("Cannabis", oldModConfig.UseCannabis_SPRING, 1),
                        new Crop_V2("Hemp", oldModConfig.UseHemp_SPRING, 1),
                        new Crop_V2("Tobacco", oldModConfig.UseTobacco_SPRING, 1)
                    }),
                summer: new Season_V2(
                    new List<Crop_V2>
                    {
                        new Crop_V2("Blue Dream", oldModConfig.UseBlue_Dream_SUMMER, 1),
                        new Crop_V2("Cannabis", oldModConfig.UseCannabis_SUMMER, 1),
                        new Crop_V2("Girl Scout Cookies", oldModConfig.UseGirl_Scout_Cookies_SUMMER, 1),
                        new Crop_V2("Green Crack", oldModConfig.UseGreen_Crack_SUMMER, 1),
                        new Crop_V2("Hemp", oldModConfig.UseHemp_SUMMER, 1),
                        new Crop_V2("Hybrid", oldModConfig.UseHybrid_SUMMER, 1),
                        new Crop_V2("Indica", oldModConfig.UseIndica, 1),
                        new Crop_V2("Northern Lights", oldModConfig.UseNorthern_Lights, 1),
                        new Crop_V2("OG Kush", oldModConfig.UseOG_Kush_SUMMER, 1),
                        new Crop_V2("Strawberry Cough", oldModConfig.UseStrawberry_Cough_SUMMER, 1),
                        new Crop_V2("Tobacco", oldModConfig.UseTobacco_SUMMER, 1),
                        new Crop_V2("White Widow", oldModConfig.UseWhite_Widow_SUMMER, 1)
                    }),
                fall: new Season_V2(
                    new List<Crop_V2>
                    {
                        new Crop_V2("Blue Dream", oldModConfig.UseBlue_Dream_FALL, 1),
                        new Crop_V2("Cannabis", oldModConfig.UseCannabis_FALL, 1),
                        new Crop_V2("Girl Scout Cookies", oldModConfig.UseGirl_Scout_Cookies_FALL, 1),
                        new Crop_V2("Green Crack", oldModConfig.UseGreen_Crack_FALL, 1),
                        new Crop_V2("Hemp", oldModConfig.UseHemp_FALL, 1),
                        new Crop_V2("Hybrid", oldModConfig.UseHybrid_FALL, 1),
                        new Crop_V2("OG Kush", oldModConfig.UseOG_Kush_FALL, 1),
                        new Crop_V2("Sativa", oldModConfig.UseSativa, 1),
                        new Crop_V2("Sour Diesel", oldModConfig.UseSour_Diesel, 1),
                        new Crop_V2("Strawberry Cough", oldModConfig.UseStrawberry_Cough_FALL, 1),
                        new Crop_V2("White Widow", oldModConfig.UseWhite_Widow_FALL, 1)
                    }),
                winter: null
            );

            newModConfig.SixPlantableCrops = new CropMod_V2
            (
                spring: null,
                summer: null,
                fall: null,
                winter: new Season_V2(
                    new List<Crop_V2>
                    {
                        new Crop_V2("Blue Rose", oldModConfig.UseBlue_Rose, 1),
                        new Crop_V2("Daikon", oldModConfig.UseDaikon, 1),
                        new Crop_V2("Gentian", oldModConfig.UseGentian, 1),
                        new Crop_V2("Napa Cabbage", oldModConfig.UseNapa_Cabbage, 1),
                        new Crop_V2("Snowdrop", oldModConfig.UseSnowdrop, 1),
                        new Crop_V2("Winter Broccoli", oldModConfig.UseWinter_Broccoli, 1)
                    })
            );

            newModConfig.BonsterCrops = new CropMod_V2
            (
                spring: new Season_V2(
                    new List<Crop_V2>
                    {
                        new Crop_V2("Cranberry Bean", oldModConfig.UseCranberry_Beans, 1),
                        new Crop_V2("Red Currant", oldModConfig.UseRedCurrant, 1),
                        new Crop_V2("Rose Hip", oldModConfig.UseRose_Hips_SPRING, 1)
                    }),
                summer: new Season_V2(
                    new List<Crop_V2>
                    {
                        new Crop_V2("Blackcurrant", oldModConfig.UseBlackcurrant, 1),
                        new Crop_V2("Blue Corn", oldModConfig.UseBlue_Corn_SUMMER, 1),
                        new Crop_V2("Cardamom", oldModConfig.UseCardamom, 1),
                        new Crop_V2("Maypop", oldModConfig.UseMaypop, 1),
                        new Crop_V2("Peppercorn", oldModConfig.UsePeppercorn_SUMMER, 1),
                        new Crop_V2("Rose Hip", oldModConfig.UseRose_Hips_SUMMER, 1),
                        new Crop_V2("Roselle Hibiscus", oldModConfig.UseRoselle_Hibiscus, 1),
                        new Crop_V2("Summer Squash", oldModConfig.UseSummer_Squash, 1),
                        new Crop_V2("Taro", oldModConfig.UseTaro_SUMMER, 1)
                    }),
                fall: new Season_V2(
                    new List<Crop_V2>
                    {
                        new Crop_V2("Blue Corn", oldModConfig.UseBlue_Corn_FALL, 1),
                        new Crop_V2("Peppercorn", oldModConfig.UsePeppercorn_FALL, 1),
                        new Crop_V2("Taro", oldModConfig.UseTaro_FALL, 1),
                        new Crop_V2("White Currant", oldModConfig.UseWhite_Currant, 1)
                    }),
                winter: null
            );

            newModConfig.RevenantCrops = new CropMod_V2
            (
                spring: new Season_V2(
                    new List<Crop_V2>
                    {
                        new Crop_V2("Enoki Mushroom", oldModConfig.UseEnoki_Mushroom_SPRING, 1)
                    }),
                summer: new Season_V2(
                    new List<Crop_V2>
                    {
                        new Crop_V2("Enoki Mushroom", oldModConfig.UseEnoki_Mushroom_SUMMER, 1)
                    }),
                fall: new Season_V2(
                    new List<Crop_V2>
                    {
                        new Crop_V2("Maitake Mushroom", oldModConfig.UseMaitake_Mushroom, 1),
                        new Crop_V2("Oyster Mushroom", oldModConfig.UseOyster_Mushroom, 1)
                    }),
                winter: new Season_V2(
                    new List<Crop_V2>
                    {
                        new Crop_V2("Gai Lan", oldModConfig.UseGai_Lan, 1)
                    })
            );

            newModConfig.FarmerToFlorist = new CropMod_V2
            (
                spring: new Season_V2(
                    new List<Crop_V2>
                    {
                        new Crop_V2("Camellia", oldModConfig.UseCamellia_SPRING, 1),
                        new Crop_V2("Carnation", oldModConfig.UseCarnation_SPRING, 1),
                        new Crop_V2("Delphinium", oldModConfig.UseDelphinium, 1),
                        new Crop_V2("Herbal Peony", oldModConfig.UseHerbalPeony, 1),
                        new Crop_V2("Hyacinth", oldModConfig.UseHyacinth_FarmerToFlorist, 1),
                        new Crop_V2("Lilac", oldModConfig.UseLilac, 1),
                        new Crop_V2("Violet", oldModConfig.UseViolet, 1),
                        new Crop_V2("Wisteria", oldModConfig.UseWisteria, 1)
                    }),
                summer: new Season_V2(
                    new List<Crop_V2>
                    {
                        new Crop_V2("Allium", oldModConfig.UseAllium, 1),
                        new Crop_V2("Carnation", oldModConfig.UseCarnation_SUMMER, 1),
                        new Crop_V2("Hydrangea", oldModConfig.UseHydrangea, 1),
                        new Crop_V2("Lavender", oldModConfig.UseLavender, 1),
                        new Crop_V2("Lily", oldModConfig.UseLily, 1),
                        new Crop_V2("Lotus", oldModConfig.UseLotus, 1),
                        new Crop_V2("Petunia", oldModConfig.UsePetunia, 1)
                    }),
                fall: new Season_V2(
                    new List<Crop_V2>
                    {
                        new Crop_V2("Camellia", oldModConfig.UseCamellia_FALL, 1),
                        new Crop_V2("Chrysanthemum", oldModConfig.UseChrysanthemum, 1),
                        new Crop_V2("Clematis", oldModConfig.UseClematis, 1),
                        new Crop_V2("Dahlia", oldModConfig.UseDahlia, 1),
                        new Crop_V2("English Rose", oldModConfig.UseEnglish_Rose, 1),
                        new Crop_V2("Freesia", oldModConfig.UseFreesia, 1),
                        new Crop_V2("Geranium", oldModConfig.UseGeranium, 1),
                        new Crop_V2("Iris", oldModConfig.UseIris, 1)
                    }),
                winter: null
            );

            newModConfig.LuckyClover = new CropMod_V2
            (
                spring: new Season_V2(
                    new List<Crop_V2>
                    {
                        new Crop_V2("Lucky Clover", oldModConfig.UseLuckyClover, 1)
                    }),
                summer: null,
                fall: null,
                winter: null
            );

            newModConfig.FishsFlowers = new CropMod_V2
            (
                spring: new Season_V2(
                    new List<Crop_V2>
                    {
                        new Crop_V2("Hyacinth", oldModConfig.UseHyacinth_FishsFlowers, 1),
                        new Crop_V2("Pansy", oldModConfig.UsePansy_SPRING, 1)
                    }),
                summer: new Season_V2(
                    new List<Crop_V2>
                    {
                        new Crop_V2("Pansy", oldModConfig.UsePansy_SUMMER, 1)
                    }),
                fall: new Season_V2(
                    new List<Crop_V2>
                    {
                        new Crop_V2("Pansy", oldModConfig.UsePansy_FALL, 1)
                    }),
                winter: null
            );

            newModConfig.StephansLotsOfCrops = new CropMod_V2
            (
                spring: new Season_V2(
                    new List<Crop_V2>
                    {
                        new Crop_V2("Cucumber", oldModConfig.UseCucumber_StephanLotsOfCrops, 1),
                        new Crop_V2("Pea Pod", oldModConfig.UsePea_Pod, 1),
                        new Crop_V2("Turnip", oldModConfig.UseTurnip, 1)
                    }),
                summer: new Season_V2(
                    new List<Crop_V2>
                    {
                        new Crop_V2("Onion", oldModConfig.UseOnion_StephanLotsOfCrops, 1),
                        new Crop_V2("Pineapple", oldModConfig.UsePineapple_StephanLotsOfCrops, 1),
                        new Crop_V2("Watermelon", oldModConfig.UseWatermelon, 1)
                    }),
                fall: new Season_V2(
                    new List<Crop_V2>
                    {
                        new Crop_V2("Carrot", oldModConfig.UseCarrot_StephanLotsOfCrops, 1),
                        new Crop_V2("Peanut", oldModConfig.UsePeanut_StephanLotsOfCrops, 1),
                        new Crop_V2("Spinach", oldModConfig.UseSpinach, 1)
                    }),
                winter: null
            );

            newModConfig.EemiesCrops = new CropMod_V2
            (
                spring: null,
                summer: new Season_V2(
                    new List<Crop_V2>
                    {
                        new Crop_V2("Cantaloupe Melon", oldModConfig.UseCantaloupe_Melon, 1),
                        new Crop_V2("Charentais Melon", oldModConfig.UseCharentais_Melon, 1),
                        new Crop_V2("Korean Melon", oldModConfig.UseKorean_Melon, 1),
                        new Crop_V2("Large Watermelon", oldModConfig.UseLarge_Watermelon, 1),
                        new Crop_V2("Rich Canary Melon", oldModConfig.UseRich_Canary_Melon, 1),
                        new Crop_V2("Rich Sweetness Melon", oldModConfig.UseRich_Sweetness_Melon, 1)
                    }),
                fall: new Season_V2(
                    new List<Crop_V2>
                    {
                        new Crop_V2("Acorn Squash", oldModConfig.UseAcorn_Squash, 1),
                        new Crop_V2("Black Forest Squash", oldModConfig.UseBlack_Forest_Squash, 1),
                        new Crop_V2("Crookneck Squash", oldModConfig.UseCrookneck_Squash, 1),
                        new Crop_V2("Golden Hubbard Squash", oldModConfig.UseGolden_Hubbard_Squash, 1),
                        new Crop_V2("Jack O Lantern Pumpkin", oldModConfig.UseJack_O_Lantern_Pumpkin, 1),
                        new Crop_V2("Sweet Lightning Pumpkin", oldModConfig.UseSweet_Lightning_Pumpkin, 1)
                    }),
                winter: null
            );

            newModConfig.TeaTime = new CropMod_V2
            (
                spring: new Season_V2(
                    new List<Crop_V2>
                    {
                        new Crop_V2("Mint Tea Plant", oldModConfig.UseMint_Tea_Plant_SPRING, 1),
                        new Crop_V2("Tea Leaf Plant", oldModConfig.UseTea_Leaf_Plant_SPRING, 1)
                    }),
                summer: new Season_V2(
                    new List<Crop_V2>
                    {
                        new Crop_V2("Mint Tea Plant", oldModConfig.UseMint_Tea_Plant_SUMMER, 1),
                        new Crop_V2("Tea Leaf Plant", oldModConfig.UseTea_Leaf_Plant_SUMMER, 1)
                    }),
                fall: new Season_V2(
                    new List<Crop_V2>
                    {
                        new Crop_V2("Mint Tea Plant", oldModConfig.UseMint_Tea_Plant_FALL, 1),
                        new Crop_V2("Tea Leaf Plant", oldModConfig.UseTea_Leaf_Plant_FALL, 1)
                    }),
                winter: null
            );

            newModConfig.ForageToFarm = new CropMod_V2
            (
                spring: new Season_V2(
                    new List<Crop_V2>
                    {
                        new Crop_V2("Cave Carrot", oldModConfig.UseCave_Carrot_SPRING, 1),
                        new Crop_V2("Common Mushroom", oldModConfig.UseCommon_Mushroom_SPRING, 1),
                        new Crop_V2("Daffodil", oldModConfig.UseDaffodil, 1),
                        new Crop_V2("Dandelion", oldModConfig.UseDandelion, 1),
                        new Crop_V2("Wild Horseradish", oldModConfig.UseWild_Horseradish, 1),
                        new Crop_V2("Leek", oldModConfig.UseLeek, 1),
                        new Crop_V2("Morel Mushroom", oldModConfig.UseMorel_Mushroom, 1),
                        new Crop_V2("Salmonberry", oldModConfig.UseSalmonberry, 1),
                        new Crop_V2("Spring Onion", oldModConfig.UseSpring_Onion, 1)
                    }),
                summer: new Season_V2(
                    new List<Crop_V2>
                    {
                        new Crop_V2("Cave Carrot", oldModConfig.UseCave_Carrot_SUMMER, 1),
                        new Crop_V2("Coconut", oldModConfig.UseCoconut, 1),
                        new Crop_V2("Fiddlehead Fern", oldModConfig.UseFiddlehead_Fern, 1),
                        new Crop_V2("Red Mushroom", oldModConfig.UseRed_Mushroom_SUMMER, 1),
                        new Crop_V2("Spice Berry", oldModConfig.UseSpice_Berry, 1),
                        new Crop_V2("Sweet Pea", oldModConfig.UseSweet_Pea, 1)
                    }),
                fall: new Season_V2(
                    new List<Crop_V2>
                    {
                        new Crop_V2("Cave Carrot", oldModConfig.UseCave_Carrot_FALL, 1),
                        new Crop_V2("Chanterelle Mushroom", oldModConfig.UseChanterelle_Mushroom, 1),
                        new Crop_V2("Common Mushroom", oldModConfig.UseCommon_Mushroom_FALL, 1),
                        new Crop_V2("Hazelnut", oldModConfig.UseHazelnut, 1),
                        new Crop_V2("Purple Mushroom", oldModConfig.UsePurple_Mushroom, 1),
                        new Crop_V2("Red Mushroom", oldModConfig.UseRed_Mushroom_FALL, 1),
                        new Crop_V2("Wild Blackberry", oldModConfig.UseWild_Blackberry, 1),
                        new Crop_V2("Wild Plum", oldModConfig.UseWild_Plum, 1)
                    }),
                winter: new Season_V2(
                    new List<Crop_V2>
                    {
                        new Crop_V2("Crocus", oldModConfig.UseCrocus, 1),
                        new Crop_V2("Crystal Fruit", oldModConfig.UseCrystal_Fruit, 1),
                        new Crop_V2("Holly", oldModConfig.UseHolly, 1),
                        new Crop_V2("Snow Yam", oldModConfig.UseSnow_Yam, 1),
                        new Crop_V2("Winter Root", oldModConfig.UseWinter_Root, 1)
                    })
            );

            newModConfig.GemAndMineralCrops = new CropMod_V2
            (
                spring: null,
                summer: null,
                fall: null,
                winter: new Season_V2(
                    new List<Crop_V2>
                    {
                        new Crop_V2("Aerinite Root", oldModConfig.UseAerinite_Root, 1),
                        new Crop_V2("Aquamarine", oldModConfig.UseAquamarine_Rose, 1),
                        new Crop_V2("Celestine Flower", oldModConfig.UseCelestine_Flower, 1),
                        new Crop_V2("Diamond Flower", oldModConfig.UseDiamond_Flower, 1),
                        new Crop_V2("Ghost Rose", oldModConfig.UseGhost_Rose, 1),
                        new Crop_V2("Kyanite Flower", oldModConfig.UseKyanite_Flower, 1),
                        new Crop_V2("Opal Cat", oldModConfig.UseOpal_Cat, 1),
                        new Crop_V2("Slate Bean", oldModConfig.UseSlate_Bean, 1),
                        new Crop_V2("Soap Root", oldModConfig.UseSoap_Root, 1)
                    })
            );

            return newModConfig;
        }

        /// <summary>Converts V2 config to V3 config.</summary>
        /// <param name="oldModConfig">The V2 config object to convert.</param>
        /// <returns>The converted config object.</returns>
        private static ModConfig_V3 ConvertV2ToV3(ModConfig_V2 oldModConfig)
        {
            var newModConfig = new ModConfig_V3();
            newModConfig.PercentDropChanceForMixedSeedsWhenNotFiber = oldModConfig.PercentDropChanceForMixedSeedsWhenNotFiber;
            newModConfig.UseSeedYearRequirement = oldModConfig.UseCropYearRequirement;
            newModConfig.StardewValley = new CropMod_V3(oldModConfig.StardewValley);
            newModConfig.CropModSettings = new Dictionary<string, CropMod_V3>()
            {
                { "ParadigmNomad.FantasyCrops", new CropMod_V3(oldModConfig.FantasyCrops) },
                { "paradigmnomad.freshmeat", new CropMod_V3(oldModConfig.FreshMeat) },
                { "ppja.fruitsandveggies", new CropMod_V3(oldModConfig.FruitAndVeggies) },
                { "mizu.flowers", new CropMod_V3(oldModConfig.MizusFlowers) },
                { "PPJA.cannabiskit", new CropMod_V3(oldModConfig.CannabisKit) },
                { "Popobug.SPCFW", new CropMod_V3(oldModConfig.SixPlantableCrops) },
                { "BFV.FruitVeggie", new CropMod_V3(oldModConfig.BonsterCrops) },
                { "RevenantCrops", new CropMod_V3(oldModConfig.RevenantCrops) },
                { "kildarien.farmertoflorist", new CropMod_V3(oldModConfig.FarmerToFlorist) },
                { "Fish.LuckyClover", new CropMod_V3(oldModConfig.LuckyClover) },
                { "Fish.FishsFlowers", new CropMod_V3(oldModConfig.FishsFlowers) },
                { "Fish.FishsFlowersCompatibilityVersion", new CropMod_V3(oldModConfig.FishsFlowersCompatibilityVersion) },
                { "StephansLotsOfCrops", new CropMod_V3(oldModConfig.StephansLotsOfCrops) },
                { "minervamaga.JA.EemieCrops", new CropMod_V3(oldModConfig.EemiesCrops) },
                { "jfujii.TeaTime", new CropMod_V3(oldModConfig.TeaTime) },
                { "Mae.foragetofarm", new CropMod_V3(oldModConfig.ForageToFarm) },
                { "rearda88.GemandMineralCrops", new CropMod_V3(oldModConfig.GemAndMineralCrops) },
                { "6480.crops.arabidopsis", new CropMod_V3(oldModConfig.MouseEarCress) },
                { "ppja.ancientcrops", new CropMod_V3(oldModConfig.AncientCrops) },
                { "PokeCropsJson", new CropMod_V3(oldModConfig.PokeCrops) },
                { "jawsawn.StarboundValley", new CropMod_V3(oldModConfig.StarboundValley) },
                { "key.cropspack", new CropMod_V3(oldModConfig.IKeychainsWinterLycheePlant) },
                { "hung2563hn.GreenPear", new CropMod_V3(oldModConfig.GreenPear) },
                { "BlatantDecoy.SodaVine", new CropMod_V3(oldModConfig.SodaVine) },
                { "amburr.spoopyvalley", new CropMod_V3(oldModConfig.SpoopyValley) },
                { "yaramy.svbakery", new CropMod_V3(oldModConfig.StardewBakery) },
                { "Hesper.JA.Succulents", new CropMod_V3(oldModConfig.Succulents) },
                { "SSaturn.TropicalFarm", new CropMod_V3(oldModConfig.TropicalFarm) }
            };
            return newModConfig;
        }
    }
}
