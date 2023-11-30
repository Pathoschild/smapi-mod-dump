/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using System.Collections.Generic;
using StardewValley.Locations;
using System.Linq;
using System.Collections;
using StardewValley.Menus;
using System.Drawing;
using xTile.Dimensions;
using xTile.Tiles;
using System.ComponentModel.Design;
using System;

namespace StardewDruid.Map
{
    static class SpawnData
    {

        public static List<int> StoneIndex()
        {

            List<int> stoneIndex = new()
            {
                2,  // ruby
                4,  // diamond
                6,  // jade
                8,  // amethyst
                10, // topaz
                12, // emerald
                14, // aquamarine
                44, // special ore
                46, // mystic ore
            };

            return stoneIndex;

        }

        public static List<int> GrizzleList()
        {
            List<int> grizzleIndex = new()
            {
                92, // Sap
                766, // Slime
                311, // PineCone
                310, // MapleSeed
                309, // Acorn
                292, // Mahogany
                767, // BatWings
                420, // RedMushroom
                831, // Taro Tuber
            };

            return grizzleIndex;
        }

        public static List<int> SashimiList()
        {

            List<int> sashimiIndex = new()
            {
                399, // SpringOnion
                403, // Snackbar
                404, // FieldMushroom
                257, // Morel
                281, // Chanterelle
                152, // Seaweed
                153, // Algae
                157, // white Algae
                78, // Carrot
                227, // Sashimi
                296, // Salmonberry
                410, // Blackberry
                424, // Cheese
                24, // Parsnip
                851, // Magma Cap
            };

            return sashimiIndex;

        }

        public static Dictionary<int, int> CoffeeList()
        {

            Dictionary<int, int> coffeeList = new()
            {
                [167] = 60000, // Joja Cola
                [433] = 20000, // Coffee Beans
                [829] = 60000, // Ginger
                [815] = 60000, // Tea Leaves
                [614] = 120000, // Tea
                [395] = 90000, // Coffee
                [253] = 300000, // Triple Espresso

            };

            return coffeeList;

        }

        public static int RandomTree(GameLocation location)
        {
            List<int> treeIndex;

            if (location is Desert || location is IslandLocation)
            {

                treeIndex = new()
                    {
                        6,8,9,
                    };

            }
            else
            {

                treeIndex = new()
                    {
                        1,2,3,1,2,3,1,2,3,8,
                    };

                if (location is Farm)
                {

                    treeIndex.Add(7);

                }

            };

            return treeIndex[Game1.random.Next(treeIndex.Count)];

        }

        public static List<int> RockFall(GameLocation location, Farmer player, int specialChance = 10)
        {

            List<int> rockList = new();

            int objectIndex;

            int scatterIndex;

            int debrisIndex;

            Dictionary<int, int> objectIndexes;

            Dictionary<int, int> specialIndexes;

            if (location is MineShaft shaftLocation)
            {

                if (shaftLocation.mineLevel <= 40)
                {

                    objectIndexes = new()
                    {
                        [0] = 32, // grade 1 stone
                        [1] = 40, // grade 1 stone
                        [2] = 42, // grade 1 stone
                    };

                    specialIndexes = new()
                    {

                        [0] = 382, // coal stone
                        [1] = 378, // copper stone
                        [2] = 378, // copper stone
                        [3] = 378, // copper stone
                        //[4] = 66, // amethyst
                        //[5] = 68, // topaz

                    };


                }
                else if (shaftLocation.mineLevel <= 80)
                {

                    objectIndexes = new()
                    {
                        [0] = 48, // grade 2a stone
                        [1] = 50, // grade 2b stone
                        [2] = 52, // grade 2c stone
                        [3] = 54, // grade 2d stone
                    };

                    specialIndexes = new()
                    {

                        [0] = 382, // coal stone
                        [1] = 380, // iron ore
                        [2] = 380, // iron ore
                        [3] = 380, // iron ore
                        //[4] = 60, // emerald
                        //[5] = 62, // aquamarine

                    };



                }
                else if (shaftLocation.mineLevel <= 120)
                {

                    objectIndexes = new()
                    {
                        [0] = 760, // grade 3 stone
                        [1] = 762, // grade 3 stone
                        [2] = 56, // grade 3 stone
                    };

                    specialIndexes = new()
                    {

                        [0] = 382, // coal stone
                        [1] = 384, // gold ore
                        [2] = 384, // gold ore
                        [3] = 384, // gold ore
                        //[4] = 72, // ruby
                        //[5] = 64, // diamond*

                    };

                }
                else // Skull Cavern
                {
                    objectIndexes = new()
                    {
                        [0] = 760, // grade 3 stone
                        [1] = 762, // grade 3 stone
                        [2] = 56, // grade 3 stone
                    };

                    specialIndexes = new()
                    {

                        [0] = 382, // coal stone
                        [1] = 386, // iridium ore
                        [2] = 386, // iridium ore
                        [3] = 386, // iridium ore
                        [4] = 386, // iridium ore

                    };

                }

            }
            else // assume Volcano dungeon
            {

                objectIndexes = new()
                {
                    [0] = 845, // volcanic stone
                    [1] = 846, // volcanic stone
                    [2] = 847, // volcanic stone
                };

                specialIndexes = new()
                {

                    [0] = 848, // cinder shards
                    [1] = 848, // cinder shards
                    [2] = 848, // cinder shards
                    [3] = 386, // iridium ore
                    [4] = 384, // gold ore

                };

            }

            Dictionary<int, int> scatterIndexes = new()
            {
                [32] = 33,
                [40] = 41,
                [42] = 43,
                [44] = 45,
                [48] = 49,
                [50] = 51,
                [52] = 53,
                [54] = 55,
                [56] = 57,
                [58] = 59,
                [760] = 761,
                [762] = 763,
                [845] = 761,
                [846] = 763,
                [847] = 761,
            };


            objectIndex = objectIndexes[Game1.random.Next(objectIndexes.Count)];

            scatterIndex = scatterIndexes[objectIndex];

            debrisIndex = 390;

            if (Game1.random.Next(specialChance) == 0)
            {
                debrisIndex = specialIndexes[Game1.random.Next(specialIndexes.Count)];

                player.gainExperience(3, 4); // gain mining experience for special drops

            }

            rockList.Add(objectIndex);

            rockList.Add(scatterIndex);

            rockList.Add(debrisIndex);

            return rockList;

            /*
            Dictionary<int, int> specialIndexes = new()
            {

                [0] = 668, // coal stone
                [1] = 32, // stone
                [2] = 40,
                [3] = 42,
                [4] = 42,

            };

            if (shaftLocation.mineLevel <= 40)
            {

                objectIndexes = new()
                {

                    [1] = 32, // grade 1 stone
                    [2] = 32, // grade 1 stone
                    [3] = 40, // grade 1 stone
                    [4] = 40, // grade 1 stone
                    [5] = 40, // grade 1 stone
                    [6] = 42, // grade 1 stone
                    [7] = 42, // grade 1 stone
                    [8] = 42, // grade 1 stone


                };

                specialIndexes = new()
                {

                    [0] = 668, // coal stone
                    [1] = 751, // copper stone
                    [2] = 751, // copper stone
                    [3] = 8, // amethyst
                    [4] = 10, // topaz

                };

            }
            else if (shaftLocation.mineLevel <= 80)
            {

                objectIndexes = new()
                {
                    [0] = 48, // grade 2a stone
                    [1] = 48, // grade 2a stone
                    [2] = 48, // grade 2a stone
                    [3] = 50, // grade 2b stone
                    [4] = 50, // grade 2b stone
                    [5] = 50, // grade 2b stone
                    [6] = 52, // grade 2c stone
                    [7] = 52, // grade 2c stone
                    [8] = 54, // grade 2d stone
                    [9] = 54, // grade 2d stone
                };

                if (powerLevel >= 1)
                {

                    specialIndexes = new()
                    {

                        [0] = 668, // coal stone
                        [1] = 290, // iron ore
                        [2] = 290, // iron ore
                        [3] = 12, // emerald
                        [4] = 14, // aquamarine*

                    };

                }

            }
            else if (shaftLocation.mineLevel <= 120)
            {

                objectIndexes = new()
                {
                    [0] = 760, // grade 3 stone
                    [1] = 760, // grade 3 stone
                    [2] = 760, // grade 3 stone
                    [3] = 762, // grade 3 stone
                    [4] = 762, // grade 3 stone
                    [5] = 762, // grade 3 stone
                    [6] = 56, // grade 3 stone
                    [7] = 56, // grade 3 stone
                    [8] = 58, // grade 3 stone
                    [9] = 58, // grade 3 stone

                };

                if (powerLevel >= 2)
                {

                    specialIndexes = new()
                    {

                        [0] = 668, // coal stone
                        [1] = 764, // gold ore
                        [2] = 764, // gold ore
                        [3] = 2, // ruby
                        [4] = 4, // diamond*

                    };

                };

            }
            else // Skull Cavern
            {

                objectIndexes = new()
                {
                    [0] = 760, // grade 3 stone
                    [1] = 760, // grade 3 stone
                    [2] = 760, // grade 3 stone
                    [3] = 762, // grade 3 stone
                    [4] = 762, // grade 3 stone
                    [5] = 762, // grade 3 stone
                    [6] = 56, // grade 3 stone
                    [7] = 56, // grade 3 stone
                    [8] = 58, // grade 3 stone
                    [9] = 58, // grade 3 stone

                };

                if (powerLevel >= 3)
                {

                    specialIndexes = new()
                    {

                        [0] = 668, // coal stone
                        [1] = 765, // iridium ore
                        [2] = 765, // iridium ore
                        [3] = 765, // iridium ore
                        [4] = 765, // iridium ore

                    };

                };

            }

            Dictionary<int, int> scatterIndexes = new()
            {

                [2] = 3,
                [4] = 5,
                [8] = 9,
                [10] = 11,
                [12] = 13,
                [14] = 15,
                [32] = 33,
                [40] = 41,
                [42] = 43,
                [44] = 45,
                [48] = 49,
                [50] = 51,
                [52] = 53,
                [54] = 55,
                [56] = 57,
                [58] = 59,
                [290] = 35,
                [668] = 669,
                [670] = 671,
                [751] = 33,
                [760] = 761,
                [762] = 763,
                [764] = 761,
                [765] = 763,

            };

            Dictionary<int, int> debrisIndexes = new()
            {

                [2] = 72,
                [4] = 64,
                [8] = 66,
                [10] = 68,
                [12] = 60,
                [14] = 62,
                [668] = 382,
                [290] = 380,
                [751] = 378,
                [764] = 384,
                [765] = 386,

            };

            if (probability == 10)
            {
                objectIndex = specialIndexes[randomIndex.Next(5)];

                targetPlayer.gainExperience(3, 4); // gain mining experience for special drops

            }
            else
            {
                objectIndex = objectIndexes[probability];

            }

            debrisIndex = 390;

            if (debrisIndexes.ContainsKey(objectIndex))
            {
                debrisIndex = debrisIndexes[objectIndex];

            }

            int scatterIndex = scatterIndexes[objectIndex];
            */


        }

        public static Dictionary<int, int> CropList(GameLocation location)
        {

            Dictionary<int, int> objectIndexes;

            string season = Game1.currentSeason;

            if (location.isGreenhouse.Value)
            {

                season = "greenhouse";

            }

            if (location is IslandLocation)
            {

                season = "island";

            }

            switch (season)
            {

                case "spring":

                    objectIndexes = new()
                    {
                        [0] = 478, // rhubarb
                        [1] = 476, // garlic
                        [2] = 433, // coffee
                        [3] = 745, // strawberry
                        [4] = 473, // bean
                        [5] = 477, // kale
                    };

                    break;

                case "summer":
                case "greenhouse":

                    objectIndexes = new()
                    {
                        [0] = 479, // melon
                        [1] = 485, // red cabbage
                        [2] = 433, // coffee
                        [3] = 481, // blueberry
                        [4] = 302, // hops
                        [5] = 483, // wheat
                    };


                    break;

                case "fall":

                    objectIndexes = new()
                    {
                        [0] = 490, // pumpkin
                        [1] = 492, // yam
                        [2] = 299, // amaranth
                        [3] = 493, // cranberry
                        [4] = 301, // grape
                    };

                    break;

                case "island":

                    objectIndexes = new()
                    {
                        [0] = 833, // pineapple
                        [1] = 831, // taro
                        [2] = 486, // starfruit
                        [3] = 829, // ginger

                    };

                    break;

                default:

                    objectIndexes = new();
                    break;

            }

            return objectIndexes;

        }

        public static int RandomFlower()
        {

            Dictionary<int, int> objectIndexes;

            switch (Game1.currentSeason)
            {

                case "spring":

                    objectIndexes = new()
                    {
                        [0] = 591, // tulip
                        [1] = 597, // jazz
                    };

                    break;

                case "summer":

                    objectIndexes = new()
                    {
                        [0] = 593, // spangle
                        [1] = 376, // poppy
                    };

                    break;

                default: //"fall":

                    objectIndexes = new()
                    {
                        [0] = 595, // fairy
                        [1] = 421, // sunflower
                        [2] = 418, // crocus
                    };

                    break;

            }

            int randomFlower = objectIndexes[Game1.random.Next(objectIndexes.Count)];

            return randomFlower;

        }
        
        public static int RandomForage(GameLocation location)
        {
            Dictionary<int, int> randomCrops;

            int randomCrop;

            string season;

            if(location is IslandEast || location is Woods)
            {

                season = "woods";

            }
            else
            {
                season = Game1.currentSeason;

            }

            switch (season)
            {

                case "spring":

                    randomCrop = 16 + Game1.random.Next(4) * 2;

                    break;

                case "summer":

                    randomCrops = new()
                    {
                        [0] = 396,
                        [1] = 396,
                        [2] = 402,
                        [3] = 398,
                    };

                    randomCrop = randomCrops[Game1.random.Next(4)];

                    break;

                case "woods":

                    randomCrops = new()
                    {

                        [0] = 257,
                        [1] = 259,
                        [2] = 815,

                    };

                    randomCrop = randomCrops[Game1.random.Next(randomCrops.Count)];

                    break;

                default: //"fall":

                    randomCrop = 404 + Game1.random.Next(4) * 2;

                    break;

            }

            return randomCrop;

        }

        public static int RandomLowFish(GameLocation location)
        {
            Dictionary<int, int> objectIndexes;

            if (location is Beach)
            {

                objectIndexes = new Dictionary<int, int>()
                {

                    //[0] = 152, // seaweed
                    //[1] = 152, // seaweed
                    //[2] = 152, // seaweed
                    [0] = 131, // sardine
                    [1] = 147, // herring
                    [2] = 129, // anchovy
                    [3] = 701, // tilapia
                    [4] = 131, // sardine
                    [5] = 147, // herring
                    [6] = 129, // anchovy
                    [7] = 150, // red snapper

                };

            }
            else if (location is IslandLocation)
            {

                objectIndexes = new Dictionary<int, int>()
                {

                    //[0] = 152, // seaweed
                    //[1] = 152, // seaweed
                    //[2] = 152, // seaweed
                    [0] = 838, // blue discuss
                    [1] = 837, // lionfish
                    [2] = 267, // flounder
                    [3] = 701, // tilapia
                    [4] = 838, // blue discuss
                    [5] = 837, // lionfish
                    [6] = 267, // flounder
                    [7] = 150, // red snapper

                };

            }
            else
            {

                objectIndexes = new Dictionary<int, int>()
                {

                    //[0] = 153, // algae
                    //[1] = 153, // algae
                    //[2] = 153, // algae
                    [0] = 145, // carp
                    [1] = 137, // smallmouth bass
                    [2] = 142,  // sunfish
                    [3] = 141, // perch
                    [4] = 145, // carp
                    [5] = 137, // smallmouth bass
                    [6] = 142,  // sunfish
                    [7] = 132  // bream

                };

            }

            int randomFish = objectIndexes[Game1.random.Next(objectIndexes.Count)];

            return randomFish;

        }

        public static int RandomHighFish(GameLocation location,bool enableRare)
        {
            
            Dictionary<int, int> objectIndexes;

            int seasonStar;

            if (location is Woods || location is Desert || location is Caldera || location is MineShaft)
            {

                switch (Game1.currentSeason)
                {
                    case "spring":
                        seasonStar = 734;
                        break;
                    case "fall":
                    case "winter":
                        seasonStar = 161;
                        break;
                    default:
                        seasonStar = 162;
                        break;

                }

                objectIndexes = new()
                {
                    [0] = 161, // ice pip
                    [1] = 734, // wood skip
                    [2] = 164, // sand fish
                    [3] = 165, // scorpion carp
                    [4] = 156, // ghost fish
                    [5] = seasonStar,
                    [6] = seasonStar,
                };

                if (enableRare)
                {
                    objectIndexes[7] = 162;  // lava eel

                }
            }
            else if (location is Beach || location is IslandLocation)
            {

                switch (Game1.currentSeason)
                {
                    case "spring":
                    case "fall":
                        seasonStar = 148;
                        break;
                    case "winter":
                        seasonStar = 151;
                        break;
                    default:
                        seasonStar = 155;
                        break;

                }

                objectIndexes = new()
                {
                    [0] = 148, // eel
                    [1] = 149, // squid
                    [2] = 151, // octopus
                    [3] = 155, // super cucumber
                    [4] = 128, // puff ball
                    [5] = seasonStar,
                    [6] = seasonStar,
                };

                if (enableRare)
                {
                    objectIndexes[7] = 836;  // stingray

                }

            } 
            else
            {

                switch (Game1.currentSeason)
                {
                    case "spring":
                    case "fall":
                        seasonStar = 143;
                        break;
                    default:
                        seasonStar = 698;
                        break;

                }

                objectIndexes = new()
                {
                    [0] = 143, // cat fish
                    [1] = 698, // sturgeon
                    [2] = 140, // walleye
                    [3] = 699, // tiger trout
                    [4] = 158, // stone fish
                    [5] = seasonStar,
                    [6] = seasonStar,

                };

                if (enableRare)
                {
                    objectIndexes[7] = 269;  // midnight carp

                }

            }

            int randomFish = objectIndexes[Game1.random.Next(objectIndexes.Count)];

            return randomFish;

        }

        public static int RandomJumpFish(GameLocation location)
        {

            int fishIndex;

            if (location is Caldera || location is MineShaft)
            {
                if (location.Name.Contains("60"))
                {
                    fishIndex = 161; // ice pip
                }
                else
                {
                    fishIndex = 165;  // scorpion carp
                }
            }
            else if (location is Woods)
            {
                fishIndex = 734; // wood skip
            }
            else if (location is Desert)
            {
                fishIndex = 164; // sand fish
            }
            else if (location is Beach)
            {
                fishIndex = 128; // puff ball 
            }
            else if (location is IslandLocation)
            {
                fishIndex = 836;  // stingray  
            }
            else
            {
                fishIndex = 699; // tiger trout
            }

            return fishIndex;

        }

        public static List<string> RecipeList()
        {
            List<string> recipeList = new(){
                "Salad",
                "Baked Fish",
                "Fried Mushroom",
                "Carp Surprise",
                "Hashbrowns",
                "Fried Eel",
                "Sashimi",
                "Maki Roll",
                "Algae Soup",
                "Fish Stew",
                "Escargot",
                "Pale Broth",
            };

            return recipeList;

        }

        public static List<string> MachineList()
        {

            List<string> machineList = new(){
                "Deconstructor",
                "Bone Mill",
                "Keg",
                "Preserves Jar",
                "Cheese Press",
                "Mayonnaise Machine",
                "Loom",
                "Oil Maker",
                "Recycling Machine",
                "Furnace",
                "Geode Crusher",
            };

            return machineList;

        }

        public static Dictionary<string, bool> SpawnTemplate()
        {
            Dictionary<string, bool> spawnTemplate = new()
            {

                ["weeds"] = false,
                ["forage"] = false,
                ["flower"] = false,
                ["grass"] = false,
                ["trees"] = false,
                ["fishup"] = false,
                ["portal"] = false,
                ["wildspawn"] = false,
                ["fishspot"] = false,
                ["cropseed"] = false,
                ["artifact"] = false,
                ["whisk"] = false,
                ["gravity"] = false,

            };

            return spawnTemplate;

        }

        public static Dictionary<string, bool> SpawnIndex(GameLocation playerLocation)
        {

            Dictionary<string, bool> spawnIndex;

            spawnIndex = SpawnTemplate();

            if (Mod.instance.CastAnywhere())
            {

                foreach(KeyValuePair<string,bool> keyValuePair in spawnIndex)
                {

                    spawnIndex[keyValuePair.Key] = true;

                }

                return spawnIndex;

            }

            if (playerLocation is Farm || playerLocation.Name == "Custom_Garden")
            {

                spawnIndex["weeds"] = true;
                spawnIndex["forage"] = true;
                spawnIndex["flower"] = true;
                spawnIndex["grass"] = true;
                spawnIndex["trees"] = true;
                spawnIndex["fishup"] = true;
                spawnIndex["wildspawn"] = true;
                spawnIndex["cropseed"] = true;
                spawnIndex["whisk"] = true;
                spawnIndex["gravity"] = true;

            }
            else if (playerLocation.isGreenhouse.Value)
            {

                spawnIndex["cropseed"] = true;

            }
            else if (playerLocation is IslandWest || playerLocation is IslandNorth)
            {
                spawnIndex["fishup"] = true;
                spawnIndex["fishspot"] = true;
                spawnIndex["wildspawn"] = true;
                spawnIndex["cropseed"] = true;
                spawnIndex["trees"] = true;
                spawnIndex["weeds"] = true;
                spawnIndex["artifact"] = true;
                spawnIndex["whisk"] = true;
                spawnIndex["gravity"] = true;

            }
            else if (playerLocation is Forest || playerLocation is Mountain || playerLocation is Desert || playerLocation is BusStop)
            {
                spawnIndex["weeds"] = true;
                spawnIndex["forage"] = true;
                spawnIndex["flower"] = true;
                spawnIndex["grass"] = true;
                spawnIndex["trees"] = true;
                spawnIndex["fishup"] = true;
                spawnIndex["wildspawn"] = true;
                spawnIndex["fishspot"] = true;
                spawnIndex["artifact"] = true;
                spawnIndex["whisk"] = true;

            }
            else if (playerLocation.Name.Contains("Backwoods") || playerLocation is Railroad)
            {

                spawnIndex["forage"] = true;
                spawnIndex["flower"] = true;
                spawnIndex["grass"] = true;
                spawnIndex["trees"] = true;
                spawnIndex["wildspawn"] = true;
                spawnIndex["portal"] = true;
                spawnIndex["artifact"] = true;
                spawnIndex["whisk"] = true;

            }
            else if (playerLocation is Woods || playerLocation.Name.Contains("Grampleton") || playerLocation is IslandEast || playerLocation is IslandShrine)
            {

                spawnIndex["forage"] = true;
                spawnIndex["flower"] = true;
                spawnIndex["grass"] = true;
                spawnIndex["wildspawn"] = true;
                spawnIndex["portal"] = true;
                spawnIndex["fishspot"] = true;
                spawnIndex["weeds"] = true;

            }
            else if (playerLocation is MineShaft || playerLocation is VolcanoDungeon) //|| playerLocation.Name.Contains("Mine"))
            {

                if (playerLocation.Name.Contains("60") || playerLocation.Name.Contains("100"))
                {
                    spawnIndex["fishspot"] = true;

                }

                spawnIndex["weeds"] = true;

            }
            else if (playerLocation is Beach || playerLocation is IslandSouth || playerLocation is IslandSouthEast || playerLocation is IslandSouthEastCave)
            {

                spawnIndex["wildspawn"] = true;
                spawnIndex["fishup"] = true;
                spawnIndex["fishspot"] = true;
                spawnIndex["artifact"] = true;
                spawnIndex["whisk"] = true;

            }
            else if (playerLocation is Town)
            {
                spawnIndex["weeds"] = true;
                spawnIndex["forage"] = true;
                spawnIndex["flower"] = true;
                spawnIndex["fishup"] = true;
                spawnIndex["artifact"] = true;
                spawnIndex["whisk"] = true;

            }
            else if (playerLocation.Name.Contains("DeepWoods"))
            {

                spawnIndex["wildspawn"] = true;
                spawnIndex["fishspot"] = true;

            }
            else if (playerLocation is AnimalHouse)
            {

                spawnIndex["hay"] = true;

            }
            else if (playerLocation is Caldera)
            {

                spawnIndex["fishspot"] = true;

            }
            else if (playerLocation is Shed)
            {

                spawnIndex["machine"] = true;

            }
            else
            {
                return new();

            }

            return spawnIndex;

        }

    }


}
