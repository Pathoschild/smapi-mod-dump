/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using GenericModConfigMenu;
using StardewDruid.Cast;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;

namespace StardewDruid.Data
{
    static class SpawnData
    {
        public static Dictionary<int, Rite.rites> WeaponAttunement(bool reserved = false)
        {

            Dictionary<int, Rite.rites> weapons = new();

            if (!reserved)
            {
                weapons = Mod.instance.save.attunement;

            }

            weapons[15] = Rite.rites.weald;
            weapons[14] = Rite.rites.mists;
            weapons[9] = Rite.rites.stars;
            weapons[53] = Rite.rites.fates;
            weapons[57] = Rite.rites.ether;

            if (Mod.instance.Helper.ModRegistry.IsLoaded("DaLion.Overhaul"))
            {
                weapons[44] = Rite.rites.weald;
                weapons[7] = Rite.rites.mists;

            }

            return weapons;

        }

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

        /*public static List<int> RoughageList()
        {
            List<int> roughageIndex = new()
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

            return roughageIndex;
        }*/

        /*public static List<int> LunchList()
        {

            List<int> lunchIndex = new()
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
                196, // Salad
                349, // Tonic
            };

            return lunchIndex;

        }*/

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
                [349] = 300000, //349: "Energy Tonic/500/200/Crafting/Energy Tonic/Restores a lot of energy./drink/0 0 0 0 0 0 0 0 0 0 0/0",

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
                        6,8,
                    };

            }
            else if (location is IslandLocation)
            {

                treeIndex = new()
                    {
                        8,9,
                    };

            }
            else
            {

                treeIndex = new()
                    {
                        1,2,3,1,2,3,8,7
                    };

                /*if (location is Farm)
                {

                    treeIndex.Add(7);

                }*/

                if(Game1.player.ForagingLevel > 5)
                {

                    treeIndex.Add(10);
                    treeIndex.Add(11);
                    treeIndex.Add(12);

                }

                if (Game1.player.ForagingLevel > 10)
                {

                    treeIndex.Add(13);

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

        }

        public static List<string> CropList(GameLocation location)
        {

            List<string> objectIndexes;

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
                        "478", // rhubarb
                        "476", // garlic
                        "433", // coffee
                        "745", // strawberry
                        "473", // bean
                        "477", // kale
                        "CarrotSeeds",
                    };

                    break;

                case "summer":
                case "greenhouse":

                    objectIndexes = new()
                    {
                        "479", // melon
                        "485", // red cabbage
                        "433", // coffee
                        "481", // blueberry
                        "302", // hops
                        "483", // wheat
                        "SummerSquashSeeds",
                    };

                    break;

                case "fall":

                    objectIndexes = new()
                    {
                        "490", // pumpkin
                        "492", // yam
                        "299", // amaranth
                        "493", // cranberry
                        "301", // grape
                        "BroccoliSeeds",
                    };

                    break;

                case "island":

                    objectIndexes = new()
                    {
                        "833", // pineapple
                        "831", // taro
                        "486", // starfruit
                        "829", // ginger

                    };

                    break;

                default:

                    objectIndexes = new()
                    {
                        "PowdermelonSeeds",

                    };

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

            if (location is IslandEast || location is Woods)
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
                    if (Game1.currentSeason == "winter" && location is Woods)
                    {
                        randomCrops[3] = 416;
                    }

                    randomCrop = randomCrops[Game1.random.Next(randomCrops.Count)];

                    break;

                default: //"fall":

                    randomCrop = 404 + Game1.random.Next(4) * 2;

                    break;

            }

            return randomCrop;

        }

        public static string RandomLowFish(GameLocation location)
        {
            List<string> indexes = new();

            if (location is Beach)
            {

                if (Game1.isRaining)
                {

                    indexes.Add("150"); // red snapper
                    indexes.Add("SeaJelly"); // red snapper

                }

                switch (Game1.currentSeason)
                {

                    case "spring":
                        indexes.Add("147"); // herring
                        indexes.Add("129"); // anchovy
                        break;
                    case "summer":
                        indexes.Add("701"); // tilapia
                        indexes.Add("131"); // sardine
                        break;
                    case "fall":
                        indexes.Add("129"); // anchovy
                        indexes.Add("131"); // sardine
                        break;
                    case "winter":
                        indexes.Add("131"); // sardine
                        indexes.Add("147"); // herring
                        break;

                }

            }
            else if (location is IslandLocation)
            {

                if (Game1.isRaining)
                {

                    indexes.Add("150"); // red snapper
                    indexes.Add("SeaJelly"); // red snapper

                }

                indexes.Add("838"); // blue discuss
                indexes.Add("837"); // lionfish

            }
            else if (location is Town || location is Forest)
            {
                
                if(Game1.timeOfDay >= 1700)
                {

                    indexes.Add("132"); // bream
                    indexes.Add("RiverJelly"); // red snapper
                }

                switch (Game1.currentSeason)
                {

                    case "spring":

                        indexes.Add("137"); // smallmouth bass
                        indexes.Add("142"); // sunfish
                        break;
                    case "summer":
                        indexes.Add("142"); // sunfish
                        indexes.Add("138"); // rainbow trout
                        break;

                    case "fall":
                        indexes.Add("137"); // smallmouth bass
                        indexes.Add("141"); // perch
                        break;

                    case "winter":
                        indexes.Add("141"); // perch
                        break;

                }

            }
            else
            {

                if (Game1.timeOfDay >= 1700)
                {

                    indexes.Add("132"); // bream
                    indexes.Add("CaveJelly"); // red snapper

                }

                indexes.Add("145"); // carp

            }

            string randomFish = indexes[Game1.random.Next(indexes.Count)];

            return randomFish;

        }

        public static int RandomHighFish(GameLocation location, bool enableRare)
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
                        seasonStar = 143; // catfish
                        break;
                    default:
                        seasonStar = 698; // sturgeon
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

        public static string RandomPoolFish(GameLocation location)
        {

            Dictionary<int, int> objectIndexes;

            if (location.Name.Contains("Beach"))
            {

                objectIndexes = new Dictionary<int, int>()
                {

                    [0] = 392, // nautilus shell
                    [1] = 152, // seaweed
                    [2] = 152, // seaweed
                    [3] = 397, // urchin
                    [4] = 718, // cockle
                    [5] = 715, // lobster
                    [6] = 720, // shrimp
                    [7] = 719, // mussel
                };

            }
            else
            {

                objectIndexes = new Dictionary<int, int>()
                {

                    [0] = 153, // algae
                    [1] = 153, // algae
                    [2] = 153, // algae
                    [3] = 153, // algae
                    [4] = 721, // snail 721
                    [5] = 716, // crayfish 716
                    [6] = 722, // periwinkle 722
                    [7] = 717, // crab 717

                };

            }

            int probability = new Random().Next(objectIndexes.Count);

            int objectIndex = objectIndexes[probability];

            return objectIndex.ToString();

        }

        public static int RandomBushForage()
        {

            int seasonal = 414; // crystal fruit

            switch (Game1.currentSeason)
            {

                case "spring":

                    seasonal = 296; // salmonberry

                    break;

                case "summer":

                    seasonal = 398; // grape

                    break;

                case "fall":

                    seasonal = 410; // blackberry

                    break;

            }
            Dictionary<int, int> objectIndexes = new()
            {
                [0] = 257, // 257 morel
                [1] = 281, // 281 chanterelle
                [2] = 404, // 404 mushroom
                [3] = seasonal,
                [4] = seasonal,
                [5] = seasonal,
                [6] = seasonal,
                [7] = seasonal,
                [8] = seasonal,

            };

            int objectIndex = objectIndexes[new Random().Next(objectIndexes.Count)];

            return objectIndex;

        }

        public static int RandomTreasure(GameLocation location, bool rareTreasure = false)
        {

            Dictionary<int, int> objectIndexes;

            if (location is Beach || location is Farm && Game1.whichFarm == 6)
            {

                objectIndexes = new Dictionary<int, int>()
                {
                    [0] = 797, //"Pearl/2500/-300/Basic/Pearl/A rare treasure from the sea.///",
                    [1] = 275, //"Artifact Trove/0/-300/Basic/Artifact Trove/A blacksmith can open this for you. These troves often contain ancient relics and curiosities./100 101 103 104 105 106 108 109 110 111 112 113 114 115 116 117 118 119 120 121 122 123 124 125 166 373 797//",
                    [2] = 166, //"Treasure Chest/5000/-300/Basic/Treasure Chest/Wow, it's loaded with treasure! This is sure to fetch a good price./Day Night^Spring Summer Fall Winter//",
                    [3] = 128, // puff ball
                    [4] = 167, //"Joja Cola/25/5/Fish -20/Joja Cola/The flagship product of Joja corporation./drink/0 0 0 0 0 0 0 0 0 0 0/0",
                    [5] = 392, // nautilus shell
                    [6] = 152, // seaweed
                    [7] = 152, // seaweed
                    [8] = 397, // urchin
                    [9] = 718, // cockle
                    [10] = 715, // lobster
                    [11] = 720, // shrimp
                    [12] = 719, // mussel
                    [13] = 393, //"Coral/80/-300/Basic -23/Coral/A colony of tiny creatures that clump together to form beautiful structures.///",
                    [14] = 394, //"Rainbow Shell/300/-300/Basic -23/Rainbow Shell/It's a very beautiful shell.///",
                    [15] = 131, // sardine
                    [16] = 147, // herring
                    [17] = 129, // anchovy
                    [18] = 701, // tilapia
                    [19] = 150, // red snapper
                    [20] = 148, // eel
                    [21] = 149, // squid
                    [22] = 151, // octopus
                    [23] = 155, // super cucumber
                    [24] = 167, //"Joja Cola/25/5/Fish -20/Joja Cola/The flagship product of Joja corporation./drink/0 0 0 0 0 0 0 0 0 0 0/0",
                    [25] = 167, //"Joja Cola/25/5/Fish -20/Joja Cola/The flagship product of Joja corporation./drink/0 0 0 0 0 0 0 0 0 0 0/0",
                };

            }
            else if (location is Caldera)
            {

                objectIndexes = new Dictionary<int, int>()
                {
                    [0] = 848, // cinder shard,
                    [1] = 848, // cinder shard,
                    [2] = 162, // lava eel
                    [3] = 162, // lava eel
                    [4] = 167, //"Joja Cola/25/5/Fish -20/Joja Cola/The flagship product of Joja corporation./drink/0 0 0 0 0 0 0 0 0 0 0/0",
                    [5] = 167, //"Joja Cola/25/5/Fish -20/Joja Cola/The flagship product of Joja corporation./drink/0 0 0 0 0 0 0 0 0 0 0/0",
                    [6] = 167, //"Joja Cola/25/5/Fish -20/Joja Cola/The flagship product of Joja corporation./drink/0 0 0 0 0 0 0 0 0 0 0/0",
                };

            }
            else if (location is IslandLocation)
            {

                objectIndexes = new Dictionary<int, int>()
                {

                    [0] = 797, //"Pearl/2500/-300/Basic/Pearl/A rare treasure from the sea.///",
                    [1] = 275, //"Artifact Trove/0/-300/Basic/Artifact Trove/A blacksmith can open this for you. These troves often contain ancient relics and curiosities./100 101 103 104 105 106 108 109 110 111 112 113 114 115 116 117 118 119 120 121 122 123 124 125 166 373 797//",
                    [2] = 166, //"Treasure Chest/5000/-300/Basic/Treasure Chest/Wow, it's loaded with treasure! This is sure to fetch a good price./Day Night^Spring Summer Fall Winter//",
                    [3] = 150, // red snapper = 852
                    [4] = 167, //"Joja Cola/25/5/Fish -20/Joja Cola/The flagship product of Joja corporation./drink/0 0 0 0 0 0 0 0 0 0 0/0",
                    [5] = 392, // nautilus shell
                    [6] = 152, // seaweed
                    [7] = 152, // seaweed
                    [8] = 397, // urchin
                    [9] = 718, // cockle
                    [10] = 715, // lobster
                    [11] = 720, // shrimp
                    [12] = 719, // mussel
                    [13] = 393, //"Coral/80/-300/Basic -23/Coral/A colony of tiny creatures that clump together to form beautiful structures.///",
                    [14] = 394, //"Rainbow Shell/300/-300/Basic -23/Rainbow Shell/It's a very beautiful shell.///",
                    [15] = 838, // blue discuss
                    [16] = 837, // lionfish
                    [17] = 267, // flounder
                    [18] = 701, // tilapia
                    [19] = 838, // blue discuss
                    [20] = 837, // lionfish
                    [21] = 267, // flounder
                    [22] = 167, //"Joja Cola/25/5/Fish -20/Joja Cola/The flagship product of Joja corporation./drink/0 0 0 0 0 0 0 0 0 0 0/0",
                    [23] = 167, //"Joja Cola/25/5/Fish -20/Joja Cola/The flagship product of Joja corporation./drink/0 0 0 0 0 0 0 0 0 0 0/0",
                };

            }

            else
            {

                objectIndexes = new Dictionary<int, int>()
                {

                    [0] = 797, //"Pearl/2500/-300/Basic/Pearl/A rare treasure from the sea.///",
                    [1] = 275, //"Artifact Trove/0/-300/Basic/Artifact Trove/A blacksmith can open this for you. These troves often contain ancient relics and curiosities./100 101 103 104 105 106 108 109 110 111 112 113 114 115 116 117 118 119 120 121 122 123 124 125 166 373 797//",
                    [2] = 166, //"Treasure Chest/5000/-300/Basic/Treasure Chest/Wow, it's loaded with treasure! This is sure to fetch a good price./Day Night^Spring Summer Fall Winter//",
                    [3] = 158, // stone fish
                    [4] = 167, //"Joja Cola/25/5/Fish -20/Joja Cola/The flagship product of Joja corporation./drink/0 0 0 0 0 0 0 0 0 0 0/0",
                    [5] = 145, // carp
                    [6] = 137, // smallmouth bass
                    [7] = 142,  // sunfish
                    [8] = 141, // perch
                    [9] = 132,  // bream
                    [10] = 153, // algae
                    [11] = 153, // algae
                    [12] = 721, // snail 721
                    [13] = 716, // crayfish 716
                    [14] = 722, // periwinkle 722
                    [15] = 717, // crab 717
                    [16] = 517, //"Glow Ring/200/-300/Ring/Glow Ring/Emits a constant light.///",
                    [17] = 519, //"Magnet Ring/200/-300/Ring/Magnet Ring/Increases your radius for collecting items.///",
                    [18] = 143, // cat fish
                    [19] = 698, // sturgeon
                    [20] = 140, // walleye
                    [21] = 699, // tiger trout
                    [22] = 167, //"Joja Cola/25/5/Fish -20/Joja Cola/The flagship product of Joja corporation./drink/0 0 0 0 0 0 0 0 0 0 0/0",
                    [23] = 167, //"Joja Cola/25/5/Fish -20/Joja Cola/The flagship product of Joja corporation./drink/0 0 0 0 0 0 0 0 0 0 0/0",
                };

            }

            int bottom = rareTreasure ? 0 : 4;

            int probability = new Random().Next(bottom, objectIndexes.Count);

            int objectIndex = objectIndexes[probability];

            return objectIndex;

        }

        public static int HighTreasure(string terrain)
        {

            Dictionary<int, int> objectIndexes;

            switch (terrain)
            {


                case "water":

                    objectIndexes = new Dictionary<int, int>()
                    {

                        [0] = 797, //"Pearl/2500/-300/Basic/Pearl/A rare treasure from the sea.///",

                        [1] = 852, //"Dragon Tooth/500/-300/Basic/Dragon Tooth/These are rumored to be the teeth of ancient serpents. The enamel is made of pure iridium!///",
                    };

                    break;

                default:

                    objectIndexes = new Dictionary<int, int>()
                    {

                        [0] = 166, //"Treasure Chest/5000/-300/Basic/Treasure Chest/Wow, it's loaded with treasure! This is sure to fetch a good price./Day Night^Spring Summer Fall Winter//",

                        [1] = 852, //"Dragon Tooth/500/-300/Basic/Dragon Tooth/These are rumored to be the teeth of ancient serpents. The enamel is made of pure iridium!///",
                    };

                    break;

            }

            int probability = new Random().Next(objectIndexes.Count);

            int objectIndex = objectIndexes[probability];

            return objectIndex;

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
                "Furnace",
                "Geode Crusher",
            };

            return machineList;

        }

        public static int RandomBarbeque()
        {
            List<int> cookingList = new() {
                194, //"Fried Egg/35/20/Cooking -7/Fried Egg/Sunny-side up./food/0 0 0 0 0 0 0 0 0 0 0/0",
                195, //"Omelet/125/40/Cooking -7/Omelet/It's super fluffy./food/0 0 0 0 0 0 0 0 0 0 0/0",
                //196, //"Salad/110/45/Cooking -7/Salad/A healthy garden salad./food/0 0 0 0 0 0 0 0 0 0 0/0",
                //197, //"Cheese Cauliflower/300/55/Cooking -7/Cheese Cauliflower/It smells great!/food/0 0 0 0 0 0 0 0 0 0 0/0",
                198, //"Baked Fish/100/30/Cooking -7/Baked Fish/Baked fish on a bed of herbs./food/0 0 0 0 0 0 0 0 0 0 0/0",
                //199, //"Parsnip Soup/120/34/Cooking -7/Parsnip Soup/It's fresh and hearty./food/0 0 0 0 0 0 0 0 0 0 0/0",
                //200, //"Vegetable Medley/120/66/Cooking -7/Vegetable Medley/This is very nutritious./food/0 0 0 0 0 0 0 0 0 0 0/0",
                201, //"Complete Breakfast/350/80/Cooking -7/Complete Breakfast/You'll feel ready to take on the world!/food/2 0 0 0 0 0 0 50 0 0 0/600",
                202, //"Fried Calamari/150/32/Cooking -7/Fried Calamari/It's so chewy./food/0 0 0 0 0 0 0 0 0 0 0/0",
                203, //"Strange Bun/225/40/Cooking -7/Strange Bun/What's inside?/food/0 0 0 0 0 0 0 0 0 0 0/0",
                204, //"Lucky Lunch/250/40/Cooking -7/Lucky Lunch/A special little meal./food/0 0 0 0 3 0 0 0 0 0 0/960",
                //205, //"Fried Mushroom/200/54/Cooking -7/Fried Mushroom/Earthy and aromatic./food/0 0 0 0 0 0 0 0 0 0 0 2/600",
                //206, //"Pizza/300/60/Cooking -7/Pizza/It's popular for all the right reasons./food/0 0 0 0 0 0 0 0 0 0 0/0",
                //207, //"Bean Hotpot/100/50/Cooking -7/Bean Hotpot/It sure is healthy./food/0 0 0 0 0 0 0 30 32 0 0/600",
                //208, //"Glazed Yams/200/80/Cooking -7/Glazed Yams/Sweet and satisfying... The sugar gives it a hint of caramel./food/0 0 0 0 0 0 0 0 0 0 0/0",
                //209, //"Carp Surprise/150/36/Cooking -7/Carp Surprise/It's bland and oily./food/0 0 0 0 0 0 0 0 0 0 0/0",
                //210, //"Hashbrowns/120/36/Cooking -7/Hashbrowns/Crispy and golden-brown!/food/1 0 0 0 0 0 0 0 0 0 0/480",
                //211, //"Pancakes/80/36/Cooking -7/Pancakes/A double stack of fluffy, soft pancakes./food/0 0 0 0 0 2 0 0 0 0 0/960",
                212, //"Salmon Dinner/300/50/Cooking -7/Salmon Dinner/The lemon spritz makes it special./food/0 0 0 0 0 0 0 0 0 0 0/0",
                213, //"Fish Taco/500/66/Cooking -7/Fish Taco/It smells delicious./food/0 2 0 0 0 0 0 0 0 0 0/600",
                214, //"Crispy Bass/150/36/Cooking -7/Crispy Bass/Wow, the breading is perfect./food/0 0 0 0 0 0 0 0 64 0 0/600",
                //215, //"Pepper Poppers/200/52/Cooking -7/Pepper Poppers/Spicy breaded peppers filled with cheese./food/2 0 0 0 0 0 0 0 0 1 0/600",
                //216, //"Bread/60/20/Cooking -7/Bread/A crusty baguette./food/0 0 0 0 0 0 0 0 0 0 0/0",
                //218, //"Tom Kha Soup/250/70/Cooking -7/Tom Kha Soup/These flavors are incredible!/food/2 0 0 0 0 0 0 30 0 0 0/600",
                //219, //"Trout Soup/100/40/Cooking -7/Trout Soup/Pretty salty./food/0 1 0 0 0 0 0 0 0 0 0/400",
                //220, //"Chocolate Cake/200/60/Cooking -7/Chocolate Cake/Rich and moist with a thick fudge icing./food/0 0 0 0 0 0 0 0 0 0 0/0",
                //221, //"Pink Cake/480/100/Cooking -7/Pink Cake/There's little heart candies on top./food/0 0 0 0 0 0 0 0 0 0 0/0",
                222, //"Rhubarb Pie/400/86/Cooking -7/Rhubarb Pie/Mmm, tangy and sweet!/food/0 0 0 0 0 0 0 0 0 0 0/0",
                //223, //"Cookie/140/36/Cooking -7/Cookie/Very chewy./food/0 0 0 0 0 0 0 0 0 0 0/0",
                224, //"Spaghetti/120/30/Cooking -7/Spaghetti/An old favorite./food/0 0 0 0 0 0 0 0 0 0 0/0",
                //225, //"Fried Eel/120/30/Cooking -7/Fried Eel/Greasy but flavorful./food/0 0 0 0 1 0 0 0 0 0 0/600",
                226, //"Spicy Eel/175/46/Cooking -7/Spicy Eel/It's really spicy! Be careful./food/0 0 0 0 1 0 0 0 0 1 0/600",
                227, //"Sashimi/75/30/Cooking -7/Sashimi/Raw fish sliced into thin pieces./food/0 0 0 0 0 0 0 0 0 0 0/0",
                228, //"Maki Roll/220/40/Cooking -7/Maki Roll/Fish and rice wrapped in seaweed./food/0 0 0 0 0 0 0 0 0 0 0/0",
                //229, //"Tortilla/50/20/Cooking -7/Tortilla/Can be used as a vessel for food or eaten by itself./food/0 0 0 0 0 0 0 0 0 0 0/0",
                230, //"Red Plate/400/96/Cooking -7/Red Plate/Full of antioxidants./food/0 0 0 0 0 0 0 50 0 0 0/300",
                //231, //"Eggplant Parmesan/200/70/Cooking -7/Eggplant Parmesan/Tangy, cheesy, and wonderful./food/0 0 1 0 0 0 0 0 0 0 3/400",
                //232, //"Rice Pudding/260/46/Cooking -7/Rice Pudding/It's creamy, sweet, and fun to eat./food/0 0 0 0 0 0 0 0 0 0 0/0",
                //233, //"Ice Cream/120/40/Cooking -7/Ice Cream/It's hard to find someone who doesn't like this./food/0 0 0 0 0 0 0 0 0 0 0/0",
                //234, //"Blueberry Tart/150/50/Cooking -7/Blueberry Tart/It's subtle and refreshing./food/0 0 0 0 0 0 0 0 0 0 0/0",
                235, //"Autumn's Bounty/350/88/Cooking -7/Autumn's Bounty/A taste of the season./food/0 0 0 0 0 2 0 0 0 0 2/660",
                //236, //"Pumpkin Soup/300/80/Cooking -7/Pumpkin Soup/A seasonal favorite./food/0 0 0 0 2 0 0 0 0 0 2/660",
                237, //"Super Meal/220/64/Cooking -7/Super Meal/It's a really energizing meal./food/0 0 0 0 0 0 0 40 0 1 0/300",
                //238, //"Cranberry Sauce/120/50/Cooking -7/Cranberry Sauce/A festive treat./food/0 0 2 0 0 0 0 0 0 0 0/300",
                //239, //"Stuffing/165/68/Cooking -7/Stuffing/Ahh... the smell of warm bread and sage./food/0 0 0 0 0 0 0 0 0 0 2/480",
                240, //"Farmer's Lunch/150/80/Cooking -7/Farmer's Lunch/This'll keep you going./food/3 0 0 0 0 0 0 0 0 0 0/480",
                241, //"Survival Burger/180/50/Cooking -7/Survival Burger/A convenient snack for the explorer./food/0 0 0 0 0 3 0 0 0 0 0/480",
                242, //"Dish O' The Sea/220/60/Cooking -7/Dish O' The Sea/This'll keep you warm in the cold sea air./food/0 3 0 0 0 0 0 0 0 0 0/480",
                243, //"Miner's Treat/200/50/Cooking -7/Miner's Treat/This should keep your energy up./food/0 0 3 0 0 0 0 0 32 0 0/480",
                244, //"Roots Platter/100/50/Cooking -7/Roots Platter/This'll get you digging for more./food/0 0 0 0 0 0 0 0 0 0 0 3/480",
                //604, //"Plum Pudding/260/70/Cooking -7/Plum Pudding/A traditional holiday treat./food/0 0 0 0 0 0 0 0 0 0 0/0",
                //605, //"Artichoke Dip/210/40/Cooking -7/Artichoke Dip/It's cool and refreshing./food/0 0 0 0 0 0 0 0 0 0 0/0",
                606, //"Stir Fry/335/80/Cooking -7/Stir Fry/Julienned vegetables on a bed of rice./food/0 0 0 0 0 0 0 0 0 0 0/0",
                //607, //"Roasted Hazelnuts/270/70/Cooking -7/Roasted Hazelnuts/The roasting process creates a rich forest flavor./food/0 0 0 0 0 0 0 0 0 0 0/0",
                //608, //"Pumpkin Pie/385/90/Cooking -7/Pumpkin Pie/Silky pumpkin cream in a flaky crust./food/0 0 0 0 0 0 0 0 0 0 0/0",
                //609, //"Radish Salad/300/80/Cooking -7/Radish Salad/The radishes are so crisp!/food/0 0 0 0 0 0 0 0 0 0 0/0",
                //610, //"Fruit Salad/450/105/Cooking -7/Fruit Salad/A delicious combination of summer fruits./food/0 0 0 0 0 0 0 0 0 0 0/0",
                //611, //"Blackberry Cobbler/260/70/Cooking -7/Blackberry Cobbler/There's nothing quite like it./food/0 0 0 0 0 0 0 0 0 0 0/0",
                //612, //"Cranberry Candy/175/50/Cooking -7/Cranberry Candy/It's sweet enough to mask the bitter fruit./drink/0 0 0 0 0 0 0 0 0 0 0/0",
                //618, //"Bruschetta/210/45/Cooking -7/Bruschetta/Roasted tomatoes on a crisp white bread./food/0 0 0 0 0 0 0 0 0 0 0/0",
                //648, //"Coleslaw/345/85/Cooking -7/Coleslaw/It's light, fresh and very healthy./food/0 0 0 0 0 0 0 0 0 0 0/0",
                //649, //"Fiddlehead Risotto/350/90/Cooking -7/Fiddlehead Risotto/A creamy rice dish served with sauteed fern heads. It's a little bland./food/0 0 0 0 0 0 0 0 0 0 0/0",
                //651, //"Poppyseed Muffin/250/60/Cooking -7/Poppyseed Muffin/It has a soothing effect./food/0 0 0 0 0 0 0 0 0 0 0/0",
                727, //"Chowder/135/90/Cooking -7/Chowder/A perfect way to warm yourself after a cold night at sea./food/0 1 0 0 0 0 0 0 0 0 0/1440",																									
                730, //"Lobster Bisque/205/90/Cooking -7/Lobster Bisque/This delicate soup is a secret family recipe of Willy's./food/0 3 0 0 0 0 0 50 0 0 0/1440",																									
                728, //"Fish Stew/175/90/Cooking -7/Fish Stew/It smells a lot like the sea. Tastes better, though./food/0 3 0 0 0 0 0 0 0 0 0/1440",																									
                729, //"Escargot/125/90/Cooking -7/Escargot/Butter-soaked snails cooked to perfection./food/0 2 0 0 0 0 0 0 0 0 0/1440",																									
                //731, //"Maple Bar/300/90/Cooking -7/Maple Bar/It's a sweet doughnut topped with a rich maple glaze./food/1 1 1 0 0 0 0 0 0 0 0/1440",																									
                732, //"Crab Cakes/275/90/Cooking -7/Crab Cakes/Crab, bread crumbs, and egg formed into patties then fried to a golden brown./food/0 0 0 0 0 0 0 0 0 1 1/1440",																									
                733, //"Shrimp Cocktail/160/90/Cooking -7/Shrimp Cocktail/A sumptuous appetizer made with freshly-caught shrimp./food/0 1 0 0 1 0 0 0 0 0 0/860",
            };

            int probability = new Random().Next(cookingList.Count);

            int objectIndex = cookingList[probability];

            return objectIndex;

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
                ["wildspawn"] = false,
                ["fishspot"] = false,
                ["cropseed"] = false,
                ["whisk"] = false,
                ["gravity"] = false,
                ["teahouse"] = false,
                ["crate"] = false,

            };

            return spawnTemplate;

        }

        public static Dictionary<string, bool> AnywhereTemplate(GameLocation playerLocation)
        {

            Dictionary<string, bool> spawnTemplate = new()
            {

                ["weeds"] = true,
                ["forage"] = true,
                ["flower"] = true,
                ["grass"] = true,
                ["trees"] = true,
                ["fishup"] = true,
                ["wildspawn"] = true,
                ["fishspot"] = true,
                ["cropseed"] = true,
                ["artifact"] = true,
                ["whisk"] = true,
                ["gravity"] = true,
                ["teahouse"] = false,
                ["crate"] = false,

            };

            if (playerLocation.Map.Layers[0].LayerWidth * playerLocation.Map.Layers[0].LayerHeight > 2000)
            {

                spawnTemplate["crate"] = true;

            }

            if (playerLocation is Shed || playerLocation is Farm)
            {

                spawnTemplate["teahouse"] = true;

            }

            if (playerLocation is Beach || playerLocation is IslandSouth || playerLocation is IslandSouthEast || playerLocation is IslandSouthEastCave)
            {

                spawnTemplate["tree"] = false;

            }

            return spawnTemplate;

        }

        public static Dictionary<string, bool> SpawnIndex(GameLocation playerLocation)
        {

            Dictionary<string, bool> spawnIndex;

            if (Mod.instance.Config.castAnywhere)
            {

                return AnywhereTemplate(playerLocation);

            }

            spawnIndex = SpawnTemplate();

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
                spawnIndex["teahouse"] = true;

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

                if (playerLocation is IslandWest)
                {

                    spawnIndex["crate"] = true;

                }

            }
            else if (playerLocation is Forest || playerLocation is Mountain || playerLocation is Desert || playerLocation is BusStop || playerLocation is BugLand)
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

                if (playerLocation.Map.Layers[0].LayerWidth * playerLocation.Map.Layers[0].LayerHeight > 2000)
                {

                    spawnIndex["crate"] = true;

                }

            }
            else if (playerLocation.Name.Contains("Backwoods") || playerLocation is Railroad)
            {

                spawnIndex["forage"] = true;
                spawnIndex["flower"] = true;
                spawnIndex["grass"] = true;
                spawnIndex["trees"] = true;
                spawnIndex["wildspawn"] = true;
                spawnIndex["artifact"] = true;
                spawnIndex["whisk"] = true;
                spawnIndex["weeds"] = true;

            }
            else if (playerLocation is Woods || playerLocation is IslandEast || playerLocation is IslandShrine || playerLocation is StardewDruid.Location.Grove)
            {

                spawnIndex["forage"] = true;
                spawnIndex["flower"] = true;
                spawnIndex["grass"] = true;
                spawnIndex["wildspawn"] = true;
                spawnIndex["fishspot"] = true;
                spawnIndex["weeds"] = true;

            }
            else if (playerLocation is MineShaft || playerLocation is VolcanoDungeon || playerLocation is Location.Crypt) //|| playerLocation.Name.Contains("Mine"))
            {

                if (playerLocation.Name.Contains("20") || playerLocation.Name.Contains("60") || playerLocation.Name.Contains("100"))
                {
                    spawnIndex["fishspot"] = true;

                }

                if (playerLocation is MineShaft mineShaft)
                {
                    List<int> mineLevels = new() { 3, 7 };

                    if (mineShaft.mineLevel == MineShaft.bottomOfMineLevel || mineShaft.mineLevel == MineShaft.quarryMineShaft)
                    {



                    }
                    else if (mineLevels.Contains(mineShaft.mineLevel % 10))
                    {

                        spawnIndex["crate"] = true;

                    }

                }

                spawnIndex["weeds"] = true;

            }
            else if (playerLocation is Beach || playerLocation is IslandSouth || playerLocation is IslandSouthEast || playerLocation is IslandSouthEastCave || playerLocation is StardewDruid.Location.Atoll)
            {

                spawnIndex["wildspawn"] = true;
                spawnIndex["fishup"] = true;
                spawnIndex["fishspot"] = true;
                spawnIndex["artifact"] = true;
                spawnIndex["whisk"] = true;
                spawnIndex["weeds"] = true;

                if (playerLocation is Beach || playerLocation is IslandSouth || playerLocation is IslandSouthEast)
                {

                    spawnIndex["crate"] = true;

                }

            }
            else if (playerLocation is Town)
            {
                spawnIndex["weeds"] = true;
                spawnIndex["forage"] = true;
                spawnIndex["flower"] = true;
                spawnIndex["fishup"] = true;
                spawnIndex["fishspot"] = true;
                spawnIndex["artifact"] = true;
                spawnIndex["whisk"] = true;

            }
            else if (playerLocation.Name.Contains("DeepWoods"))
            {

                spawnIndex["wildspawn"] = true;
                spawnIndex["fishspot"] = true;

                if (playerLocation.Map.Layers[0].LayerWidth * playerLocation.Map.Layers[0].LayerHeight > 2000)
                {

                    spawnIndex["crate"] = true;

                }

            }
            else if (playerLocation is AnimalHouse)
            {

                spawnIndex["hay"] = true;

            }
            else if (playerLocation is Caldera || playerLocation is Sewer)
            {

                spawnIndex["fishspot"] = true;

            }
            else if (playerLocation is Shed)
            {

                spawnIndex["teahouse"] = true;

            }
            else if(playerLocation.Name.Contains("Saloon"))
            {

                spawnIndex["crate"] = true;

            }
            else
            {
                
                return new();

            }

            return spawnIndex;

        }

    }


}
