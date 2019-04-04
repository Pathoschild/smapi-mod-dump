using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using Microsoft.Xna.Framework;
using xTile;
using xTile.ObjectModel;
using xTile.Display;
using Harmony;

namespace FarmHouseRedone
{
    public static class FarmHouseStates
    {
        public static int wallAndFloorsSheet;
        public static int interiorSheet;
        public static int farmSheet;
        public static string WallsData;
        public static string FloorsData;
        public static Dictionary<Rectangle, string> floorDictionary;
        public static Dictionary<Rectangle, string> wallDictionary;
        public static string bedData;
        public static string spouseRoomData;
        public static string kitchenData;
        public static string entryData;
        public static string cellarData;
        public static string levelThreeData;

        internal static IContentHelper loader;
        internal static IReflectionHelper reflector;
        internal static HarmonyInstance harmony;

        public static Dictionary<string, int> spouseRooms;
        internal static List<char> spaceEquivalents = new List<char>
        {
            '|',
            '_',
            '[',
            ']',
            '(',
            ')',
            '{',
            '}',
            ',',
            '.'
        };

        public static List<int> townInteriorWalls = new List<int>
        {
            33,
            34,
            35,
            38,
            39,
            40,
            44,
            45,
            46,
            50,
            51,
            52,
            53,
            54,
            55,
            65,
            66,
            67,
            70,
            71,
            72,
            76,
            77,
            78,
            79,
            80,
            81,
            82,
            83,
            84,
            85,
            86,
            87,
            97,
            98,
            99,
            102,
            103,
            104,
            105,
            106,
            107,
            108,
            109,
            110,
            111,
            112,
            113,
            114,
            115,
            116,
            117,
            118,
            119,
            127,
            129,
            131,
            134,
            136,
            140,
            142,
            146,
            148,
            154,
            155,
            169,
            170,
            171,
            172,
            173,
            174,
            201,
            202,
            203,
            204,
            205,
            206,
            218,
            219,
            233,
            234,
            235,
            236,
            237,
            238,
            254,
            265,
            266,
            267,
            268,
            269,
            270,
            286,
            297,
            298,
            299,
            300,
            301,
            302,
            303,
            304,
            305,
            318,
            329,
            330,
            361,
            591,
            592,
            593,
            1137,
            1138,
            1217,
            1218,
            1225,
            1226,
            1402,
            1403,
            1404,
            1405,
            1406,
            1407,
            1434,
            1435,
            1436,
            1437,
            1438,
            1439,
            1466,
            1467,
            1468,
            1469,
            1470,
            1471,
            1498,
            1501,
            1638,
            1639,
            1640,
            1641,
            1670,
            1671,
            1672,
            1673,
            1702,
            1703,
            1704,
            1705
        };

        public static List<int> townInteriorFloors = new List<int>
        {
            4,
            5,
            21,
            22,
            23,
            24,
            25,
            26,
            27,
            28,
            29,
            31,
            57,
            58,
            59,
            60,
            61,
            62,
            63,
            69,
            89,
            90,
            91,
            92,
            93,
            94,
            121,
            122,
            123,
            124,
            125,
            126,
            135,
            150,
            151,
            156,
            157,
            158,
            159,
            198,
            199,
            200,
            228,
            230,
            231,
            232,
            248,
            249,
            250,
            251,
            252,
            253,
            255,
            260,
            263,
            264,
            280,
            281,
            282,
            283,
            284,
            285,
            287,
            289,
            292,
            315,
            316,
            317,
            328,
            362,
            442,
            445,
            446,
            447,
            477,
            478,
            479,
            507,
            508,
            509,
            510,
            511,
            539,
            540,
            541,
            542,
            543,
            571,
            572,
            573,
            574,
            575,
            603,
            604,
            605,
            606,
            607,
            635,
            636,
            637,
            638,
            639,
            670,
            671,
            702,
            703,
            798,
            799,
            830,
            831,
            866,
            928,
            994,
            995,
            997,
            998,
            999,
            1000,
            1001,
            1002,
            1112,
            1113,
            1114,
            1144,
            1145,
            1146,
            1149,
            1150,
            1151,
            1169,
            1170,
            1171,
            1172,
            1175,
            1176,
            1177,
            1181,
            1182,
            1183,
            1207,
            1208,
            1209,
            1214,
            1215,
            1239,
            1240,
            1241,
            1246,
            1247,
            1271,
            1272,
            1273,
            1278,
            1279,
            1299,
            1300,
            1301,
            1302,
            1303,
            1304,
            1305,
            1308,
            1309,
            1310,
            1311,
            1331,
            1332,
            1333,
            1334,
            1336,
            1337,
            1342,
            1343,
            1499,
            1500,
            1502,
            1503,
            1508,
            1509,
            1510,
            1511,
            1512,
            1513,
            1530,
            1531,
            1532,
            1533,
            1534,
            1535,
            1540,
            1541,
            1542,
            1543,
            1544,
            1545,
            1560,
            1561,
            1562,
            1563,
            1564,
            1565,
            1566,
            1567,
            1572,
            1573,
            1574,
            1575,
            1576,
            1577,
            1592,
            1593,
            1594,
            1595,
            1596,
            1597,
            1598,
            1599,
            1604,
            1605,
            1606,
            1607,
            1608,
            1609,
            1625,
            1626,
            1627,
            1628,
            1629,
            1630,
            1631,
            1657,
            1658,
            1659,
            1660,
            1661,
            1662,
            1663,
            1664,
            1665,
            1666,
            1689,
            1690,
            1691,
            1692,
            1693,
            1694,
            1695,
            1696,
            1697,
            1698,
            1710,
            1714,
            1721,
            1722,
            1723,
            1724,
            1725,
            1726,
            1727,
            1753,
            1754,
            1755,
            1756,
            1757,
            1758,
            1759,
            1788,
            1789,
            1790,
            1791,
            1822,
            1845,
            1846,
            1847,
            1848,
            1849,
            1850,
            1851,
            1852,
            1853,
            1854,
            1887,
            1915,
            1916,
            1917,
            1945,
            1946,
            1947,
            1948,
            1949,
            2075,
            2076,
            2078,
            2079,
            2083,
            2084,
            2085,
            2106,
            2107,
            2108,
            2109,
            2110,
            2112,
            2113,
            2114,
            2116,
            2117,
            2118,
            2119,
            2122,
            2127,
            2128,
            2130,
            2131,
            2132,
            2133,
            2138,
            2139,
            2140,
            2144,
            2145,
            2146,
            2148,
            2149,
            2150,
            2151,
            2154,
            2155,
            2161,
            2162,
            2164,
            2166,
            2171,
            2172
        };

        public static List<int> townInteriorWallFurniture = new List<int>
        {
            177,
            180,
            207,
            212,
            213,
            214,
            239,
            240,
            243,
            244,
            246,
            271,
            272,
            275,
            276,
            279,
            306,
            308,
            338,
            341,
            342,
            343,
            344,
            371,
            372,
            373,
            375,
            407,
            411,
            532,
            533,
            564,
            565,
            674,
            826,
            992,
            1032,
            1033,
            1034,
            1065,
            1066,
            1097,
            1098,
            1344
        };

        public static List<int> townInteriorFloorFurniture = new List<int>
        {
            216, 217, 313, 314, 331, 334, 335, 336, 337, 345, 346, 347, 348, 349, 350, 368, 369, 377, 178, 379, 380, 381, 382, 383, 400, 401, 408, 409, 412, 413, 415, 431, 433, 434, 440, 441, 444, 472, 473, 474, 476, 482, 483, 486, 487, 491, 502, 503, 504, 505, 506, 518, 519, 523, 534, 535, 536, 537, 538, 544, 545, 546, 551, 552, 556, 557, 558, 577, 578, 583, 584, 588, 589, 590, 608, 609, 610, 611, 612, 613, 614, 623, 624, 625, 641, 642, 643, 644, 645, 646, 651, 655, 656, 657, 658, 659, 660, 663, 672, 673, 675, 679, 680, 687, 688, 689, 690, 694, 695, 691, 692, 704, 711, 712, 722, 723, 724, 726, 727, 736, 743, 768, 800, 813, 814, 815, 832, 833, 834, 860, 864, 875, 876, 886, 892, 897, 865, 1029, 1062, 1063, 1064, 1094, 1095, 1096, 1108, 1109, 1111, 1143, 1179, 1141, 1180, 1191, 1127, 1128, 1136, 1168, 1179, 1200, 1322, 1327, 1328, 1335, 1348, 1354, 1359, 1360, 1367, 1387, 1388, 1459, 1491, 1525, 1589, 1621, 1837, 1912, 1940, 1944, 1974, 1982, 1983, 1950, 1951, 2115, 2136, 2137, 2141, 2147, 2167, 2168, 2169, 2173
        };

        public static List<int> townInteriorWindows = new List<int>
        {
            225,
            256,
            257,
            288,
            405,
            406,
            469,
            501,
            1219,
            1220,
            1221,
            1222,
            1224,
            1252,
            1253,
            1254,
            1285
        };

        public static void init(IContentHelper contentHelper)
        {
            WallsData = null;
            FloorsData = null;
            wallDictionary = new Dictionary<Rectangle, string>();
            floorDictionary = new Dictionary<Rectangle, string>();
            bedData = null;
            spouseRoomData = null;
            kitchenData = null;
            entryData = null;
            cellarData = null;
            interiorSheet = 0;
            farmSheet = 1;
            wallAndFloorsSheet = 2;
            loader = contentHelper;
            loadVanillaSpouses();
        }

        internal static void clear()
        {
            WallsData = null;
            FloorsData = null;
            wallDictionary = new Dictionary<Rectangle, string>();
            floorDictionary = new Dictionary<Rectangle, string>();
            bedData = null;
            spouseRoomData = null;
            kitchenData = null;
            entryData = null;
            cellarData = null;
            interiorSheet = 0;
            farmSheet = 1;
            wallAndFloorsSheet = 2;
        }

        public static int getSpouseRoom(string name)
        {
            int result = -1;
            spouseRooms.TryGetValue(name, out result);
            return result;
        }

        public static void loadVanillaSpouses()
        {
            spouseRooms["Abigail"] = 0;
            spouseRooms["Penny"] = 1;
            spouseRooms["Leah"] = 2;
            spouseRooms["Haley"] = 3;
            spouseRooms["Maru"] = 4;
            spouseRooms["Sebastian"] = 5;
            spouseRooms["Alex"] = 6;
            spouseRooms["Harvey"] = 7;
            spouseRooms["Elliott"] = 8;
            spouseRooms["Sam"] = 9;
            spouseRooms["Shane"] = 10;
            spouseRooms["Emily"] = 11;
        }

        public static string cleanup(string dirty)
        {
            string outString = "";
            Logger.Log("Given " + dirty);
            dirty = dirty.Replace("\n", " ");
            dirty = dirty.Replace("\\n", " ");
            Logger.Log("Line breaks cleaned as " + dirty);
            for (int index = 0; index < dirty.Length; index++)
            {
                if ((dirty[index] == ' ' && outString.EndsWith(" ")))
                    continue;
                if (spaceEquivalents.Contains(dirty[index]))
                {
                    outString += (outString.EndsWith(" ") ? "" : " ");
                    continue;
                }
                outString += dirty[index];
            }
            Logger.Log("Returning " + outString);
            return outString.Trim(' ');
        }

        public static void updateFromMapPath(string mapPath)
        {
            clear();
            Map map = loader.Load<Map>(mapPath, ContentSource.GameContent);
            Logger.Log("Updating walls and floors...");
            PropertyValue floors;
            map.Properties.TryGetValue("Floors", out floors);
            if (floors != null)
                FloorsData = cleanup(floors.ToString());
            else
                FloorsData = "";
            PropertyValue walls;
            map.Properties.TryGetValue("Walls", out walls);
            if (walls != null)
                WallsData = cleanup(walls.ToString());
            else
                WallsData = "";
            PropertyValue bed;
            map.Properties.TryGetValue("Bed", out bed);
            if (bed != null)
                bedData = cleanup(bed.ToString());
            else
                bedData = "";
            PropertyValue spouseRoom;
            map.Properties.TryGetValue("Spouse", out spouseRoom);
            if (spouseRoom != null)
                spouseRoomData = cleanup(spouseRoom.ToString());
            else
                spouseRoomData = "";
            PropertyValue kitchen;
            map.Properties.TryGetValue("Kitchen", out kitchen);
            if (kitchen != null)
                kitchenData = cleanup(kitchen.ToString());
            else
                kitchenData = "";

            PropertyValue entry;
            map.Properties.TryGetValue("Entry", out entry);
            if (entry != null)
                entryData = cleanup(entry.ToString());
            else
                entryData = "";
            
            PropertyValue cellar;
            map.Properties.TryGetValue("Cellar", out cellar);
            if (cellar != null)
                cellarData = cleanup(cellar.ToString());
            else
                cellarData = "";

            PropertyValue levelThree;
            map.Properties.TryGetValue("Level3", out levelThree);
            if (levelThree != null)
                levelThreeData = cleanup(levelThree.ToString());
            else
                levelThreeData = "";

            foreach (xTile.Tiles.TileSheet sheet in map.TileSheets)
            {
                if (sheet.ImageSource.Contains("walls_and_floors"))
                {
                    wallAndFloorsSheet = map.TileSheets.IndexOf(sheet);
                }
                else if (sheet.ImageSource.Contains("townInterior"))
                {
                    interiorSheet = map.TileSheets.IndexOf(sheet);
                }
                else if (sheet.ImageSource.Contains("farmhouse_tiles"))
                {
                    farmSheet = map.TileSheets.IndexOf(sheet);
                }
            }
        }

        public static Point getEntryLocation(StardewValley.Locations.FarmHouse house)
        {
            if (entryData == null)
                updateFromMapPath(house.mapPath);
            if (entryData != "")
            {
                Logger.Log("Map defined entry location...");
                string[] entryPoint = entryData.Split(' ');
                return new Point(Convert.ToInt32(entryPoint[0]), Convert.ToInt32(entryPoint[1]));
            }
            else
            {
                Logger.Log("Map did not define entry location.");
                return house.getEntryLocation();
            }
        }

        public static Point getCellarLocation(StardewValley.Locations.FarmHouse house)
        {
            if (cellarData == null)
                updateFromMapPath(house.mapPath);
            if (cellarData != "")
            {
                Logger.Log("Map defined entry location...");
                string[] cellarPoint = cellarData.Split(' ');
                return new Point(Convert.ToInt32(cellarPoint[0]), Convert.ToInt32(cellarPoint[1]));
            }
            else
            {
                Logger.Log("Map did not define cellar return location.");
                return new Point(-1, -1);
            }
        }

        public static Point getKitchenSpot(StardewValley.Locations.FarmHouse house)
        {
            if (kitchenData == null)
                updateFromMapPath(house.mapPath);
            if (kitchenData != "")
            {
                Logger.Log("Map defined kitchen location...");
                string[] kitchenPoint = kitchenData.Split(' ');
                return new Point(Convert.ToInt32(kitchenPoint[0]), Convert.ToInt32(kitchenPoint[1]));
            }
            else
            {
                Logger.Log("Map did not define kitchen location.");
                return house.getKitchenStandingSpot();
            }
        }

        public static Point getBedSpot(StardewValley.Locations.FarmHouse house, bool spouse = false)
        {
            Logger.Log("Getting bed location...");
            if (bedData == null)
                FarmHouseStates.updateFromMapPath(house.mapPath);
            if (FarmHouseStates.bedData != "")
            {
                Logger.Log("Map defined bed location...");
                string[] bedPoint = FarmHouseStates.bedData.Split(' ');

                return new Point(Convert.ToInt32(bedPoint[0]) + (spouse ? 2 : 0), Convert.ToInt32(bedPoint[1]));
            }
            else
            {
                Logger.Log("Map did not define a bed location, using vanilla.");
                if (spouse)
                    return house.getSpouseBedSpot();
                return house.getBedSpot();
            }
        }

        public static Map loadSpouseRoomIfPresent(string name)
        {
            Map map = null;
            try
            {
                map = loader.Load<Map>("Maps/" + name + "_spouseroom", ContentSource.GameContent);
                Logger.Log("Found " + name + "_spouseRoom!");
            }
            catch (Microsoft.Xna.Framework.Content.ContentLoadException e)
            {
                Logger.Log("No spouse room found by the name " + name + "_spouseroom within Maps");
                Logger.Log(e.Message + e.StackTrace);
            }
            return map;
        }

        public static Map loadLevelThreeUpgradeIfPresent(string name)
        {
            Map map = null;
            //Try loading from the game content.  This is where Content Patcher patches will place them.
            try
            {
                map = loader.Load<Map>("Maps/" + name + "_levelthree", ContentSource.GameContent);
                Logger.Log("Found a map by the name '" + ("Maps/" + name + "_levelthree") + "'!");
            }
            catch (Microsoft.Xna.Framework.Content.ContentLoadException)
            {
                //No map found.
            }
            //Next try loading from the mod directory, for the default cellar map
            try
            {
                map = loader.Load<Map>("assets/maps/" + name + "_levelthree.tbin", ContentSource.ModFolder);
                Logger.Log("Found packaged default map by the name '" + ("assets/maps/" + name + "_levelthree") + "'");
            }
            catch (Microsoft.Xna.Framework.Content.ContentLoadException e)
            {
                Logger.Log("No upgrade map found by the name " + name + "_levelthree within assets/maps/");
                Logger.Log(e.Message + e.StackTrace);
            }
            if(map == null)
            {
                Logger.Log("No level three upgrade map could be found by the name '" + name + "_levelthree'!");
            }
            return map;
        }

        public static void setMissingWallpaperToDefault(StardewValley.Locations.FarmHouse house)
        {
            List<Rectangle> walls = house.getWalls();
            while(house.wallPaper.Count < walls.Count)
            {
                house.wallPaper[house.wallPaper.Count] = getWallpaperIndex(house.map, walls[house.wallPaper.Count].X, walls[house.wallPaper.Count].Y);
            }
        }

        public static void setMissingFloorsToDefault(StardewValley.Locations.FarmHouse house)
        {
            List<Rectangle> floors = house.getFloors();
            while (house.floor.Count < floors.Count)
            {
                house.floor[house.floor.Count] = getFloorIndex(house.map, floors[house.floor.Count].X, floors[house.floor.Count].Y);
            }
        }

        public static void setWallpaperDefaults(StardewValley.Locations.FarmHouse house)
        {
            List<Rectangle> walls = house.getWalls();
            List<Rectangle> floors = house.getFloors();
            for (int wallIndex = 0; wallIndex < walls.Count; wallIndex++)
            {
                Logger.Log("Setting default wall for the wall " + walls[wallIndex].ToString() + "...");
                int wallPaperIndex = getWallpaperIndex(house.map, walls[wallIndex].X, walls[wallIndex].Y);
                house.setWallpaper(wallPaperIndex, wallIndex, true);
            }
        }

        public static int getWallpaperIndex(Map map, int x, int y)
        {
            int wallIndex = getWallSpriteIndex(map, x, y);
            if (wallIndex == -1)
            {
                Logger.Log("Could not find any wall tile on any layer at (" + x + ", " + y + ")");
                return 0;
            }
            int wallPaperX = wallIndex % 16;
            int wallPaperY = wallIndex / 48;
            int wallPaperIndex = (wallPaperY * 16) + wallPaperX;
            Logger.Log("Found wallpaper index of " + wallPaperIndex + " for tilesheet index " + wallIndex + ".");
            return wallPaperIndex;
        }

        public static int getFloorIndex(Map map, int x, int y)
        {
            int floorIndex = getFloorpriteIndex(map, x, y);
            if (floorIndex == -1 || floorIndex < 336)
            {
                Logger.Log("Could not find any floor tile on any layer at (" + x + ", " + y + ")");
                return 0;
            }
            floorIndex -= 336;
            int floorX = (floorIndex / 2) % 8;
            int floorY = floorIndex / 32;
            int flooringIndex = (floorY * 8) + floorX;
            Logger.Log("Found floor index of " + flooringIndex + " for tilesheet index " + floorIndex + ".");
            return flooringIndex;
        }

        public static int getWallSpriteIndex(Map map, int x, int y)
        {
            int index = -1;
            if (isTileAWall(map, x, y, "Back"))
            {
                index = map.GetLayer("Back").Tiles[x, y].TileIndex;
                Logger.Log("Found a wall tile on Back layer of index " + index);
            }
            else if (isTileAWall(map, x, y, "Buildings"))
            {
                index = map.GetLayer("Buildings").Tiles[x, y].TileIndex;
                Logger.Log("Found a wall tile on Buildings layer of index " + index);
            }
            else if (isTileAWall(map, x, y, "Front"))
            {
                index = map.GetLayer("Front").Tiles[x, y].TileIndex;
                Logger.Log("Found a wall tile on Front layer of index " + index);
            }
            return index;
        }

        public static bool isTileAWall(Map map, int x, int y, string layer)
        {
            bool result = map.GetLayer(layer).Tiles[x, y] != null && (map.GetLayer(layer).Tiles[x, y].TileSheet.ImageSource.Contains("walls_and_floors")) && map.GetLayer(layer).Tiles[x, y].TileIndex <= 335;
            return result;
        }

        public static int getFloorpriteIndex(Map map, int x, int y)
        {
            int index = -1;
            if (map.GetLayer("Back").Tiles[x, y] != null)
            {
                if (map.GetLayer("Back").Tiles[x, y].TileSheet.ImageSource.Contains("walls_and_floors"))
                {
                    index = map.GetLayer("Back").Tiles[x, y].TileIndex;
                    Logger.Log("Found a floor tile on Back layer of index " + index);
                }
            }
            return index;
        }

        //Stolen (with explicit permission) from original author Bwdy
        //https://github.com/bwdymods/SDV-bwdyworks/blob/master/ModUtil.cs
        public static HashSet<string> GetAllCharacterNames(bool onlyDateable = false, bool onlyVillager = false, StardewValley.GameLocation onlyThisLocation = null)
        {
            HashSet<string> characters = new HashSet<string>(); //hashset ensures only unique values exist
            if (onlyThisLocation != null)
            {
                foreach (var c in onlyThisLocation.characters)
                {
                    if (!string.IsNullOrWhiteSpace(c.Name))
                    {
                        if (!onlyVillager || c.isVillager())
                            if (!onlyDateable || c.datable.Value) characters.Add(c.Name);
                    }
                }
                return characters; //only checking the one location
            }
            //start with NPCDispositions
            Dictionary<string, string> dictionary = StardewValley.Game1.content.Load<Dictionary<string, string>>("Data\\NPCDispositions");
            foreach (string s in dictionary.Keys)
            {
                var c = StardewValley.Game1.getCharacterFromName(s, onlyVillager);
                if (c != null) //simple nullcheck to ensure they weren't removed
                {
                    if (!onlyDateable || c.datable.Value)
                        if (!string.IsNullOrWhiteSpace(c.Name))
                            characters.Add(c.Name);
                }
            }
            //iterate locations for mod-added NPCs that aren't in the data
            foreach (var loc in StardewValley.Game1.locations)
                foreach (var c in loc.characters)
                {
                    if (!string.IsNullOrWhiteSpace(c.Name))
                    {
                        if (!onlyVillager || c.isVillager())
                            if (!onlyDateable || c.datable.Value) characters.Add(c.Name);
                    }
                }

            //return the list
            return characters;
        }

    }
}
