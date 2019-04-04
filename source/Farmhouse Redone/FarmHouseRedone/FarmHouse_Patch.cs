using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using Harmony;
using StardewValley.Locations;
using StardewValley;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using xTile;
using xTile.ObjectModel;
using xTile.Tiles;

namespace FarmHouseRedone
{
    class FarmHouse_Patch
    {
        public static void Postfix(List<Rectangle> __result, FarmHouse __instance)
        {
            if (__instance is Cabin)
                return;
            __result.Clear();
            FarmHouseStates.floorDictionary.Clear();
            Logger.Log("Getting floor rectangles...");
            if (FarmHouseStates.FloorsData == null)
                FarmHouseStates.updateFromMapPath(__instance.mapPath);
            if (FarmHouseStates.FloorsData != "")
            {
                string[] floorArray = FarmHouseStates.FloorsData.Split(' ');
                for (int index = 0; index < floorArray.Length; index += 5)
                {
                    try
                    {
                        Rectangle rectResult = new Rectangle(Convert.ToInt32(floorArray[index]), Convert.ToInt32(floorArray[index + 1]), Convert.ToInt32(floorArray[index + 3]), Convert.ToInt32(floorArray[index + 4]));
                        __result.Add(rectResult);
                        FarmHouseStates.floorDictionary[rectResult] = floorArray[index + 2];
                    }
                    catch (IndexOutOfRangeException)
                    {
                        Logger.Log("Partial floor rectangle definition detected! (" + FarmHouseStates.FloorsData.Substring(index) + ")  Floor rectangles must be defined as\nX Y Identifier Width Height\nPlease ensure all floor definitions have exactly these 5 values.", LogLevel.Error);
                    }
                    catch (FormatException)
                    {
                        string errorLocation = "";
                        for(int errorIndex = index; errorIndex < floorArray.Length && errorIndex - index < 5; errorIndex += 1)
                        {
                            errorLocation += floorArray[errorIndex] + " ";
                        }
                        Logger.Log("Incorrect floor rectangle format. (" + errorLocation + ")  Floor rectangles must be defined as\nX Y Identifier Width Height\nPlease ensure all floor definitions have exactly these 5 values.", LogLevel.Error);
                    }
                }
                foreach (Rectangle floorRect in __result)
                {
                    //Logger.Log("Found floor rectangle (" + floorRect.X + ", " + floorRect.Y + ", " + floorRect.Width + ", " + floorRect.Height + ")");
                }
            }
            else
            {
                switch (__instance.upgradeLevel)
                {
                    case 0:
                        __result.Add(new Rectangle(1, 3, 10, 9));
                        break;
                    case 1:
                        __result.Add(new Rectangle(1, 3, 6, 9));
                        __result.Add(new Rectangle(7, 3, 11, 9));
                        __result.Add(new Rectangle(18, 8, 2, 2));
                        __result.Add(new Rectangle(20, 3, 9, 8));
                        break;
                    case 2:
                    case 3:
                        __result.Add(new Rectangle(1, 3, 12, 6));
                        __result.Add(new Rectangle(15, 3, 13, 6));
                        __result.Add(new Rectangle(13, 5, 2, 2));
                        __result.Add(new Rectangle(0, 12, 10, 11));
                        __result.Add(new Rectangle(10, 12, 11, 9));
                        __result.Add(new Rectangle(21, 17, 2, 2));
                        __result.Add(new Rectangle(23, 12, 11, 11));
                        break;
                }
                Logger.Log("Found " + __result.Count + " floors.");
            }
        }
    }

    class FarmHouse_getWalls_Patch
    {
        public static void Postfix(List<Rectangle> __result, FarmHouse __instance)
        {
            if (__instance is Cabin)
                return;
            __result.Clear();
            FarmHouseStates.wallDictionary.Clear();
            Logger.Log("Getting walls...");
            if (FarmHouseStates.WallsData == null)
                FarmHouseStates.updateFromMapPath(__instance.mapPath);
            if (FarmHouseStates.WallsData != "")
            {
                string[] wallArray = FarmHouseStates.WallsData.Split(' ');
                for (int index = 0; index < wallArray.Length; index += 5)
                {
                    try
                    {
                        Rectangle rectResult = new Rectangle(Convert.ToInt32(wallArray[index]), Convert.ToInt32(wallArray[index + 1]), Convert.ToInt32(wallArray[index + 3]), Convert.ToInt32(wallArray[index + 4]));
                        __result.Add(rectResult);
                        FarmHouseStates.wallDictionary[rectResult] = wallArray[index + 2];
                    }
                    catch (IndexOutOfRangeException)
                    {
                        Logger.Log("Partial wall rectangle definition detected! (" + FarmHouseStates.WallsData.Substring(index) + ")  Wall rectangles must be defined as\nX Y Identifier Width Height\nPlease ensure all wall definitions have exactly these 5 values.", LogLevel.Error);
                    }
                    catch (FormatException)
                    {
                        string errorLocation = "";
                        for (int errorIndex = index; errorIndex < wallArray.Length && errorIndex - index < 5; errorIndex += 1)
                        {
                            errorLocation += wallArray[errorIndex] + " ";
                        }
                        Logger.Log("Incorrect wall rectangle format. (" + errorLocation + ")  Wall rectangles must be defined as\nX Y Identifier Width Height\nPlease ensure all wall definitions have exactly these 5 values.", LogLevel.Error);
                    }
                }
                foreach (Rectangle floorRect in __result)
                {
                    //Logger.Log("Found wall rectangle (" + floorRect.X + ", " + floorRect.Y + ", " + floorRect.Width + ", " + floorRect.Height + ")");
                }
            }
            else
            {
                switch (__instance.upgradeLevel)
                {
                    case 0:
                        __result.Add(new Rectangle(1, 1, 10, 3));
                        break;
                    case 1:
                        __result.Add(new Rectangle(1, 1, 17, 3));
                        __result.Add(new Rectangle(18, 6, 2, 2));
                        __result.Add(new Rectangle(20, 1, 9, 3));
                        break;
                    case 2:
                    case 3:
                        __result.Add(new Rectangle(1, 1, 12, 3));
                        __result.Add(new Rectangle(15, 1, 13, 3));
                        __result.Add(new Rectangle(13, 3, 2, 2));
                        __result.Add(new Rectangle(1, 10, 10, 3));
                        __result.Add(new Rectangle(13, 10, 8, 3));
                        __result.Add(new Rectangle(21, 15, 2, 2));
                        __result.Add(new Rectangle(23, 10, 11, 3));
                        break;
                }
            }
        }
    }

    class FarmHouse_doSetVisibleFloor_Patch
    {
        static void setMapTileIndexIfOnFloorSheet(Map map, DecoratableLocation instance, int x, int y, int index, string layer, int tileSheet, int tileSheetToMatch)
        {
            if (map.GetLayer(layer).Tiles[x, y] != null && map.GetLayer(layer).Tiles[x, y].TileSheet.Equals((object)map.TileSheets[tileSheetToMatch]) && map.GetLayer(layer).Tiles[x, y].TileIndex >= 336)
                instance.setMapTileIndex(x, y, index, layer, tileSheet);
        }

        public static bool Prefix(int whichRoom, int which, DecoratableLocation __instance)
        {
            if (__instance is Cabin)
                return true;
            List<Microsoft.Xna.Framework.Rectangle> floors = __instance.getFloors();
            int index = 336 + which % 8 * 2 + which / 8 * 32;
            Logger.Log("Chosen index " + index + ".");
            if (whichRoom == -1)
            {
                foreach (Microsoft.Xna.Framework.Rectangle rectangle in floors)
                {
                    int x = rectangle.X;
                    while (x < rectangle.Right)
                    {
                        int y = rectangle.Y;
                        while (y < rectangle.Bottom)
                        {
                            if (rectangle.Contains(x, y))
                                setMapTileIndexIfOnFloorSheet(__instance.map, __instance, x, y, index, "Back", 0, FarmHouseStates.wallAndFloorsSheet);
                            if (rectangle.Contains(x + 1, y))
                                setMapTileIndexIfOnFloorSheet(__instance.map, __instance, x + 1, y, index + 1, "Back", 0, FarmHouseStates.wallAndFloorsSheet);
                            if (rectangle.Contains(x, y + 1))
                                setMapTileIndexIfOnFloorSheet(__instance.map, __instance, x, y + 1, index + 16, "Back", 0, FarmHouseStates.wallAndFloorsSheet);
                            if (rectangle.Contains(x + 1, y + 1))
                                setMapTileIndexIfOnFloorSheet(__instance.map, __instance, x + 1, y + 1, index + 17, "Back", 0, FarmHouseStates.wallAndFloorsSheet);
                            y += 2;
                        }
                        x += 2;
                    }
                }
            }
            else
            {
                if (floors.Count <= whichRoom)
                {
                    Logger.Log("Floor rectangle exceeded floors count!", StardewModdingAPI.LogLevel.Warn);
                    return false;
                }
                //List<Rectangle> connnectedFloors = new List<Rectangle>();
                //connnectedFloors.Add(floors[whichRoom]);
                //if (FarmHouseStates.floorDictionary.ContainsKey(floors[whichRoom]))
                //{
                //    string roomString = FarmHouseStates.floorDictionary[floors[whichRoom]];
                //    foreach (KeyValuePair<Rectangle, string> floorDefinition in FarmHouseStates.floorDictionary)
                //    {
                //        if (floorDefinition.Value.Equals(roomString))
                //            connnectedFloors.Add(floorDefinition.Key);
                //    }
                //}
                //foreach (Rectangle rectangle in connnectedFloors)
                //{
                Rectangle rectangle = floors[whichRoom];
                int x = rectangle.X;
                while (x < rectangle.Right)
                {
                    int y = rectangle.Y;
                    while (y < rectangle.Bottom)
                    {
                        if (rectangle.Contains(x, y))
                            setMapTileIndexIfOnFloorSheet(__instance.map, __instance, x, y, index, "Back", 0, FarmHouseStates.wallAndFloorsSheet);
                        if (rectangle.Contains(x + 1, y))
                            setMapTileIndexIfOnFloorSheet(__instance.map, __instance, x + 1, y, index + 1, "Back", 0, FarmHouseStates.wallAndFloorsSheet);
                        if (rectangle.Contains(x, y + 1))
                            setMapTileIndexIfOnFloorSheet(__instance.map, __instance, x, y + 1, index + 16, "Back", 0, FarmHouseStates.wallAndFloorsSheet);
                        if (rectangle.Contains(x + 1, y + 1))
                            setMapTileIndexIfOnFloorSheet(__instance.map, __instance, x + 1, y + 1, index + 17, "Back", 0, FarmHouseStates.wallAndFloorsSheet);
                        y += 2;
                    }
                    x += 2;
                }
            }
            return false;
        }
    }

    internal class CharacterArguments
    {
        internal int spouseGender = -1;
        internal int farmerGender = -1;

    }

    class FarmHouse_loadSpouseRoom_Patch
    {
        internal static bool Prefix(FarmHouse __instance)
        {
            if (__instance is Cabin)
                return true;
            List<Rectangle> spouseRoomRects = new List<Rectangle>();

            if (FarmHouseStates.spouseRoomData == null)
                FarmHouseStates.updateFromMapPath(__instance.mapPath);
            if (FarmHouseStates.spouseRoomData != "")
            {
                string[] spousePoint = FarmHouseStates.spouseRoomData.Split(' ');
                if (spousePoint.Length < 2)
                {
                    Logger.Log("Spouse room was defined, but did not have at least two numerical values!  Given '" + FarmHouseStates.spouseRoomData + "'.", LogLevel.Error);
                    return true;
                }
                try
                {
                    applyAllSpouseRooms(__instance, spousePoint);
                }
                catch (FormatException)
                {
                    Logger.Log("Spouse room was defined, but its values do not seem to be numerical!  Given '" + FarmHouseStates.spouseRoomData + "'.", LogLevel.Error);
                    return true;
                }
            }
            else
            {
                Logger.Log("No spouse room location was defined.");
                if(FarmHouseStates.spouseRooms.Keys.Contains(Game1.player.spouse))
                    return true;
                Logger.Log("Spouse was not a base-game marriage candidate, applying vanilla-like spouse room behavior for modded spouse '" + Game1.player.spouse + "'...");
                makeSpouseRoom(__instance, __instance.upgradeLevel == 1 ? 29 : 35, __instance.upgradeLevel == 1 ? 1 : 10, true, true, true, true, new List<string>(), new List<string>(), "", false);
            }

            return false;
        }

        internal static void applyAllSpouseRooms(FarmHouse __instance, string[] spouseRoomData)
        {
            int readerIndex = 0;
            bool floorFlag = true;
            bool wallFlag = true;
            bool clutterFlag = true;
            bool windowFlag = true;
            List<string> nameWhitelist = new List<string>();
            List<string> nameBlacklist = new List<string>();
            string nameOverride = "";
            CharacterArguments args = new CharacterArguments();
            bool destructive = false;
            int x = -1;
            int y = -1;
            while(readerIndex < spouseRoomData.Length)
            {
                if (isNumeric(spouseRoomData[readerIndex]) && isNumeric(spouseRoomData[readerIndex + 1]))
                {
                    if (x != -1 && y != -1)
                    {
                        Logger.Log("Found spouse room definition: " + x + " " + y + (floorFlag ? " flooring " : " no flooring ") + (wallFlag ? " walls " : " no walls ") + (clutterFlag ? " clutter" : " no clutter"));
                        makeSpouseRoom(__instance, x, y, floorFlag, wallFlag, windowFlag, clutterFlag, nameWhitelist, nameBlacklist, nameOverride, destructive);
                        floorFlag = true;
                        wallFlag = true;
                        clutterFlag = true;
                        windowFlag = true;
                        nameWhitelist.Clear();
                        nameBlacklist.Clear();
                        nameOverride = "";
                        args = new CharacterArguments();
                        destructive = false;
                        x = -1;
                        y = -1;
                    }
                    x = Convert.ToInt32(spouseRoomData[readerIndex]);
                    y = Convert.ToInt32(spouseRoomData[readerIndex + 1]);
                    readerIndex+=2;
                }
                else if (x == -1 || y == -1)
                {
                    Logger.Log("Improper spouse room data!  No coordinates appear to be present!  Please ensure all spouse room definitions begin with an X and Y coordinate.", LogLevel.Error);
                    readerIndex++;
                }
                else
                {
                    if (spouseRoomData[readerIndex].StartsWith("-"))
                    {
                        string flag = spouseRoomData[readerIndex].TrimStart('-').ToLower();
                        Logger.Log("Found flag: " + flag);
                        if (flag.StartsWith("wi"))
                        {
                            Logger.Log("Interpreted as 'no window' flag.");
                            windowFlag = false;
                        }
                        else if (flag.StartsWith("w"))
                        {
                            Logger.Log("Interpreted as 'no wall' flag.");
                            wallFlag = false;
                        }
                        else if (flag.StartsWith("fu"))
                        {
                            Logger.Log("Interpreted as 'no furniture' flag.");
                            clutterFlag = false;
                        }
                        else if (flag.StartsWith("f"))
                        {
                            Logger.Log("Interpreted as 'no floor' flag.");
                            floorFlag = false;
                        }
                        else if (flag.StartsWith("c"))
                        {
                            Logger.Log("Interpreted as 'no clutter' flag.");
                            clutterFlag = false;
                        }
                        else if (flag.StartsWith("d"))
                        {
                            Logger.Log("Interpreted as 'destructive' flag.");
                            destructive = true;
                        }
                    }
                    else if (spouseRoomData[readerIndex].StartsWith("+"))
                    {
                        string name = spouseRoomData[readerIndex].TrimStart('+');
                        Logger.Log(name + " was added to the whitelist.");
                        nameWhitelist.Add(name);
                    }
                    else if (spouseRoomData[readerIndex].StartsWith("!"))
                    {
                        string name = spouseRoomData[readerIndex].TrimStart('!');
                        Logger.Log(name + " was added to the blacklist.");
                        nameBlacklist.Add(name);
                    }
                    else if (spouseRoomData[readerIndex].StartsWith("="))
                    {
                        string name = spouseRoomData[readerIndex].TrimStart('=');
                        Logger.Log("Spouse room set to behave as " + name + "'s spouse room (" + name + "_spouseroom.tbin)");
                        nameOverride = name;
                    }
                    else if (spouseRoomData[readerIndex].StartsWith("%"))
                    {
                        string argumentString = spouseRoomData[readerIndex].TrimStart('%').ToLower();
                        Logger.Log("Character argument detected: " + argumentString);
                        string[] argument = argumentString.Split(':');
                        if (argument[0].StartsWith("gen"))
                        {
                            bool ofSpouse = false;
                            int gender = -1;
                            Logger.Log("Interpreting as gender-based argument...");
                            if(argument[1].StartsWith("p") || argument[1].StartsWith("f"))
                            {
                                Logger.Log("Gender of Player argument...");
                            }
                            else if(argument[1].StartsWith("s") || argument[1].StartsWith("n") || argument[1].StartsWith("h") || argument[1].StartsWith("w"))
                            {
                                Logger.Log("Gender of Spouse argument...");
                            }
                            if (argument[2].StartsWith("m") || argument[2].StartsWith("b"))
                            {
                                Logger.Log("Gender set to Male...");
                                gender = 0;
                            }
                            else if (argument[2].StartsWith("w") || argument[2].StartsWith("g") || argument[2].StartsWith("f"))
                            {
                                Logger.Log("Gender set to Female...");
                                gender = 1;
                            }
                            else
                            {
                                Logger.Log("Gender set to Nonbinary...");
                                gender = 2;
                            }
                            if (ofSpouse)
                            {
                                args.spouseGender = gender;
                            }
                            else
                            {
                                args.farmerGender = gender;
                            }
                        }
                        if (argument[0].StartsWith("anx"))
                        {
                            Logger.Log("Interpreting as anxiety-based argument...");
                        }
                    }
                    else
                    {
                        Logger.Log("Unsure what to do with '" + spouseRoomData[readerIndex] + "'...");
                    }
                    readerIndex++;
                }
            }
            if (x != -1 && y != -1)
            {
                Logger.Log("Found spouse room definition: " + x + " " + y + (floorFlag ? " flooring " : " no flooring ") + (wallFlag ? " walls " : " no walls ") + (clutterFlag ? " clutter" : " no clutter"));
                makeSpouseRoom(__instance, x, y, floorFlag, wallFlag, windowFlag, clutterFlag, nameWhitelist, nameBlacklist, nameOverride, destructive);
            }
        }

        internal static void makeSpouseRoom(FarmHouse __instance, int x, int y, bool floor, bool wall, bool window, bool clutter, List<string> whiteList, List<string> blackList, string nameOverride, bool destructive)
        {
            bool hadAcceptableSpouse = false;
            bool hadOnlyBlacklistSpouse = true;
            if (whiteList.Count == 0)
                hadAcceptableSpouse = true;
            if (blackList.Count == 0)
                hadOnlyBlacklistSpouse = false;
            foreach (string npcName in FarmHouseStates.GetAllCharacterNames(true))
            {
                NPC npc = Game1.getCharacterFromName(npcName);
                if(npc.isMarried() && npc.getSpouse() == __instance.owner)
                {
                    Logger.Log(npc.name + " is married to " + __instance.owner.name);
                    if(whiteList.Count > 0 && whiteList.Contains(npc.name))
                    {
                        Logger.Log(nameOverride + " was in the whitelist!");
                        hadAcceptableSpouse = true;
                    }
                    if (whiteList.Count > 0 && !blackList.Contains(npc.name))
                    {
                        Logger.Log(nameOverride + " was not in the blacklist!");
                        hadOnlyBlacklistSpouse = false;
                    }
                }
                else
                {
                    Logger.Log(npc.name + " was not married to " + __instance.owner.name);
                }
            }
            if (!hadAcceptableSpouse || hadOnlyBlacklistSpouse)
                return;
            NPC spouse = __instance.owner.getSpouse();
            if (spouse == null) {
                Logger.Log("Had no spouse!");
                return;
            }
            string spouseName = spouse.name;
            if(nameOverride != null && nameOverride != "")
            {
                Logger.Log("Applying name override of " + nameOverride);
                spouseName = nameOverride;
            }
            FarmHouseStates.clear();

            pasteSpouseRoom(__instance, spouseName, new Rectangle(x, y, 6, 9), floor, wall, window, clutter, destructive);
        }

        internal static bool isNumeric(string candidate)
        {
            int n;
            return int.TryParse(candidate, out n);
        }

        internal static void pasteSpouseRoom(FarmHouse __instance, string spouse, Rectangle spouseRoomRect, bool floor, bool wall, bool window, bool clutter, bool destructive)
        {
            int num = FarmHouseStates.getSpouseRoom(spouse);
            Map map = FarmHouseStates.loadSpouseRoomIfPresent(spouse);
            Point point;
            if (map == null)
            {
                if (num == -1)
                {
                    Logger.Log("No spouse room could be found for " + spouse + "!");
                    return;
                }
                map = Game1.game1.xTileContent.Load<Map>("Maps\\spouseRooms");
                point = new Point(num % 5 * 6, num / 5 * 9);
            }
            else
            {
                point = new Point(0, 0);
            }
            __instance.map.Properties.Remove("DayTiles");
            __instance.map.Properties.Remove("NightTiles");

            //mergeMaps(map, point, __instance, spouseRoomRect);

            IReflectedMethod adjustMapLightPropertiesForLamp = FarmHouseStates.reflector.GetMethod(__instance, "adjustMapLightPropertiesForLamp");

            foreach (TileSheet sheet in __instance.map.TileSheets)
            {
                int tileWidth = sheet.SheetWidth;
                string contentSource = sheet.ImageSource;
                Logger.Log("Looking for " + contentSource + "...");
                int pixelHeight = FarmHouseStates.loader.Load<Texture2D>(contentSource, ContentSource.GameContent).Height;
                sheet.SheetSize = new xTile.Dimensions.Size(tileWidth, (pixelHeight / 16));
                Logger.Log("Found " + (sheet.SheetSize.Width * sheet.SheetSize.Height) + " tiles for " + sheet.Id + ", with a width of " + tileWidth + " and a height of " + (int)(pixelHeight / 16) + ".");
            }

            for (int index1 = 0; index1 < spouseRoomRect.Width; ++index1)
            {
                for (int index2 = 0; index2 < spouseRoomRect.Height; ++index2)
                {

                    pasteBackLayer(__instance.map, map, spouseRoomRect, point, index1, index2, floor, wall, clutter, destructive);
                    addProperties(map, __instance.Map, new Point(spouseRoomRect.X, spouseRoomRect.Y), "Back", index1, index2);
                    pasteBuildingsLayer(__instance.map, map, spouseRoomRect, point, index1, index2, adjustMapLightPropertiesForLamp, floor, wall, window, clutter, destructive);
                    addProperties(map, __instance.Map, new Point(spouseRoomRect.X, spouseRoomRect.Y), "Buildings", index1, index2);
                    pasteFrontLayer(__instance.map, map, spouseRoomRect, point, index1, index2, adjustMapLightPropertiesForLamp, floor, wall, window, clutter, destructive);
                    addProperties(map, __instance.Map, new Point(spouseRoomRect.X, spouseRoomRect.Y), "Front", index1, index2);
                    //if (map.GetLayer("Back").Tiles[point.X + index1, point.Y + index2] != null)
                    //    __instance.map.GetLayer("Back").Tiles[spouseRoomRect.X + index1, spouseRoomRect.Y + index2] = (Tile)new StaticTile(__instance.map.GetLayer("Back"), __instance.map.TileSheets[getEquivalentSheet(map.GetLayer("Back").Tiles[point.X + index1, point.Y + index2])], BlendMode.Alpha, map.GetLayer("Back").Tiles[point.X + index1, point.Y + index2].TileIndex);
                    //if (map.GetLayer("Buildings").Tiles[point.X + index1, point.Y + index2] != null)
                    //{
                    //    __instance.map.GetLayer("Buildings").Tiles[spouseRoomRect.X + index1, spouseRoomRect.Y + index2] = (Tile)new StaticTile(__instance.map.GetLayer("Buildings"), __instance.map.TileSheets[getEquivalentSheet(map.GetLayer("Buildings").Tiles[point.X + index1, point.Y + index2], true)], BlendMode.Alpha, map.GetLayer("Buildings").Tiles[point.X + index1, point.Y + index2].TileIndex);
                    //    adjustMapLightPropertiesForLamp.Invoke(map.GetLayer("Buildings").Tiles[point.X + index1, point.Y + index2].TileIndex, spouseRoomRect.X + index1, spouseRoomRect.Y + index2, "Buildings");
                    //}
                    //else
                    //    __instance.map.GetLayer("Buildings").Tiles[spouseRoomRect.X + index1, spouseRoomRect.Y + index2] = (Tile)null;
                    //if (index2 < spouseRoomRect.Height - 1 && map.GetLayer("Front").Tiles[point.X + index1, point.Y + index2] != null)
                    //{
                    //    __instance.map.GetLayer("Front").Tiles[spouseRoomRect.X + index1, spouseRoomRect.Y + index2] = (Tile)new StaticTile(__instance.map.GetLayer("Front"), __instance.map.TileSheets[getEquivalentSheet(map.GetLayer("Front").Tiles[point.X + index1, point.Y + index2])], BlendMode.Alpha, map.GetLayer("Front").Tiles[point.X + index1, point.Y + index2].TileIndex);
                    //    adjustMapLightPropertiesForLamp.Invoke(map.GetLayer("Front").Tiles[point.X + index1, point.Y + index2].TileIndex, spouseRoomRect.X + index1, spouseRoomRect.Y + index2, "Front");
                    //}
                    //else if (index2 < spouseRoomRect.Height - 1)
                    //    __instance.map.GetLayer("Front").Tiles[spouseRoomRect.X + index1, spouseRoomRect.Y + index2] = (Tile)null;
                    if (index1 == 4 && index2 == 4 && !__instance.map.GetLayer("Back").Tiles[spouseRoomRect.X + index1, spouseRoomRect.Y + index2].Properties.ContainsKey("NoFurniture"))
                        __instance.map.GetLayer("Back").Tiles[spouseRoomRect.X + index1, spouseRoomRect.Y + index2].Properties.Add(new KeyValuePair<string, PropertyValue>("NoFurniture", new PropertyValue("T")));
                }
            }

            Point spousePoint = new Point(spouseRoomRect.X, spouseRoomRect.Y);

            //Combine properties
            foreach (KeyValuePair<string, PropertyValue> pair in map.Properties)
            {
                PropertyValue value = pair.Value;
                string propertyName = pair.Key;
                Logger.Log("Spouse room map contained Properties data, applying now...");
                if (propertyName.Equals("Warp"))
                {
                    //propertyName = "Warp";
                    Logger.Log("Adjusting warp property...");
                    string[] warpParts = FarmHouseStates.cleanup(value.ToString()).Split(' ');
                    string warpShifted = "";
                    for (int index = 0; index < warpParts.Length; index += 5)
                    {
                        try
                        {
                            Logger.Log("Relative warp found: " + warpParts[index + 0] + " " + warpParts[index + 1] + " " + warpParts[index + 2] + " " + warpParts[index + 3] + " " + warpParts[index + 4]);
                            int xi = -1;
                            int yi = -1;
                            int xii = -1;
                            int yii = -1;
                            if (warpParts[0].StartsWith("~"))
                                xi = spousePoint.X + Convert.ToInt32(warpParts[index + 0].TrimStart('~'));
                            else
                                xi = Convert.ToInt32(warpParts[0]);
                            if (warpParts[1].StartsWith("~"))
                                yi = spousePoint.Y + Convert.ToInt32(warpParts[index + 1].TrimStart('~'));
                            else
                                yi = Convert.ToInt32(warpParts[1]);
                            if (warpParts[3].StartsWith("~"))
                                xii = spousePoint.X + Convert.ToInt32(warpParts[index + 3].TrimStart('~'));
                            else
                                xii = Convert.ToInt32(warpParts[3]);
                            if (warpParts[4].StartsWith("~"))
                                yii = spousePoint.Y + Convert.ToInt32(warpParts[index + 4].TrimStart('~'));
                            else
                                yii = Convert.ToInt32(warpParts[4]);
                            string returnWarp = xi + " " + yi + " " + warpParts[index + 2] + " " + xii + " " + yii + " ";
                            Logger.Log("Relative warp became " + returnWarp);
                            warpShifted += returnWarp;
                        }
                        catch (IndexOutOfRangeException)
                        {
                            Logger.Log("Incomplete warp definition found!  Please ensure all warp definitions are formatted as 'X Y map X Y'", LogLevel.Warn);
                        }
                        catch (FormatException)
                        {
                            Logger.Log("Invalid warp definition found!  Please ensure all warp definitions use numbers fro the X and Y coordinates!", LogLevel.Warn);
                        }
                    }
                    value = warpShifted.Trim(' ');
                    Logger.Log("Warp property is now " + value.ToString());
                }
                if (!__instance.map.Properties.ContainsKey(propertyName))
                {
                    Logger.Log("FarmHouse map did not have a '" + propertyName + "' property, setting it to '" + propertyName + "'...");
                    __instance.map.Properties.Add(propertyName, value);
                }
                else
                {
                    PropertyValue houseValue = __instance.map.Properties[propertyName];
                    Logger.Log("Farmhouse map already had a '" + propertyName + "' value, appending...");
                    __instance.map.Properties[propertyName] = (houseValue.ToString() + " " + value.ToString()).Trim(' ');
                    Logger.Log(propertyName + " is now " + __instance.map.Properties[propertyName].ToString());
                }
            }
        }

        internal static void addProperties(Map sectionMap, Map houseMap, Point housePoint, string layer, int x, int y)
        {
            if (sectionMap.GetLayer(layer).Tiles[x, y] != null)
            {
                Logger.Log("Checking for properties...");
                if (sectionMap.GetLayer(layer).Tiles[x, y].Properties.Keys.Count > 0)
                {
                    Logger.Log("Properties discovered.");
                    foreach (KeyValuePair<string, PropertyValue> pair in sectionMap.GetLayer(layer).Tiles[x, y].Properties)
                    {
                        Logger.Log("Applying property '" + pair.Key + "' to tile " + new Vector2(housePoint.X + x, housePoint.Y + y).ToString() + "...");
                        houseMap.GetLayer(layer).Tiles[housePoint.X + x, housePoint.Y + y].Properties[pair.Key] = pair.Value;
                        Logger.Log("Applied.");
                    }
                }
            }
        }

        internal static void pasteFrontLayer(Map houseMap, Map spouseMap, Rectangle spouseRoomRect, Point spousePoint, int x, int y, IReflectedMethod adjustMapLightPropertiesForLamp, bool floor, bool wall, bool window, bool clutter, bool destructive)
        {
            

            if (y < spouseRoomRect.Height - 1 && spouseMap.GetLayer("Front").Tiles[spousePoint.X + x, spousePoint.Y + y] != null)
            {
                TileSheet sheet = houseMap.TileSheets[getEquivalentSheet(spouseMap.GetLayer("Front").Tiles[spousePoint.X + x, spousePoint.Y + y])];
                int tileIndex = spouseMap.GetLayer("Front").Tiles[spousePoint.X + x, spousePoint.Y + y].TileIndex;
                bool isWall = false;
                bool isFloor = false;
                bool isWindow = false;
                bool isClutter = false;
                if (sheet.ImageSource.Contains("walls_and_floors"))
                {
                    isWall = tileIndex < 336;
                    isFloor = tileIndex >= 336;
                }
                else if (sheet.ImageSource.Contains("townInterior"))
                {
                    isWall = FarmHouseStates.townInteriorWalls.Contains(tileIndex);
                    isFloor = FarmHouseStates.townInteriorFloors.Contains(tileIndex);
                    isWindow = FarmHouseStates.townInteriorWindows.Contains(tileIndex);
                    isClutter = FarmHouseStates.townInteriorWallFurniture.Contains(tileIndex) || FarmHouseStates.townInteriorFloorFurniture.Contains(tileIndex);
                }
                if (isWall && !wall)
                    return;
                if (isFloor && !floor)
                    return;
                if (isWindow && !window)
                    return;
                if (isClutter && !clutter)
                    return;
                houseMap.GetLayer("Front").Tiles[spouseRoomRect.X + x, spouseRoomRect.Y + y] = (Tile)new StaticTile(houseMap.GetLayer("Front"), houseMap.TileSheets[getEquivalentSheet(spouseMap.GetLayer("Front").Tiles[spousePoint.X + x, spousePoint.Y + y])], BlendMode.Alpha, spouseMap.GetLayer("Front").Tiles[spousePoint.X + x, spousePoint.Y + y].TileIndex);
                adjustMapLightPropertiesForLamp.Invoke(spouseMap.GetLayer("Front").Tiles[spousePoint.X + x, spousePoint.Y + y].TileIndex, spouseRoomRect.X + x, spouseRoomRect.Y + y, "Front");
            }
            else if (y < spouseRoomRect.Height - 1 || destructive)
                houseMap.GetLayer("Front").Tiles[spouseRoomRect.X + x, spouseRoomRect.Y + y] = (Tile)null;
        }

        internal static void pasteBuildingsLayer(Map houseMap, Map spouseMap, Rectangle spouseRoomRect, Point spousePoint, int x, int y, IReflectedMethod adjustMapLightPropertiesForLamp, bool floor, bool wall, bool window, bool clutter, bool destructive)
        {
            if (spouseMap.GetLayer("Buildings").Tiles[spousePoint.X + x, spousePoint.Y + y] != null)
            {
                //Get the equivalent tilesheet.
                TileSheet sheet = houseMap.TileSheets[getEquivalentSheet(spouseMap.GetLayer("Buildings").Tiles[spousePoint.X + x, spousePoint.Y + y])];
                int tileIndex = spouseMap.GetLayer("Buildings").Tiles[spousePoint.X + x, spousePoint.Y + y].TileIndex;
                bool isWall = false;
                bool isFloor = false;
                bool isWindow = false;
                bool isClutter = false;
                if (sheet.ImageSource.Contains("walls_and_floors"))
                {
                    isWall = tileIndex < 336;
                    isFloor = tileIndex >= 336;
                }
                else if (sheet.ImageSource.Contains("townInterior"))
                {
                    isWall = FarmHouseStates.townInteriorWalls.Contains(tileIndex);
                    isFloor = FarmHouseStates.townInteriorFloors.Contains(tileIndex);
                    isWindow = FarmHouseStates.townInteriorWindows.Contains(tileIndex);
                    isClutter = FarmHouseStates.townInteriorWallFurniture.Contains(tileIndex) || FarmHouseStates.townInteriorFloorFurniture.Contains(tileIndex);
                }
                if (isWall && !wall)
                    return;
                if (isFloor && !floor)
                    return;
                if (isWindow && !window)
                    return;
                if (isClutter && !clutter)
                    return;
                houseMap.GetLayer("Buildings").Tiles[spouseRoomRect.X + x, spouseRoomRect.Y + y] 
                    = (Tile)new StaticTile(
                        houseMap.GetLayer("Buildings"),
                        houseMap.TileSheets[getEquivalentSheet(spouseMap.GetLayer("Buildings").Tiles[spousePoint.X + x, spousePoint.Y + y], true)],
                        BlendMode.Alpha,
                        spouseMap.GetLayer("Buildings").Tiles[spousePoint.X + x, spousePoint.Y + y].TileIndex
                );

                adjustMapLightPropertiesForLamp.Invoke(spouseMap.GetLayer("Buildings").Tiles[spousePoint.X + x, spousePoint.Y + y].TileIndex, spouseRoomRect.X + x, spouseRoomRect.Y + y, "Buildings");
            }
            else if (destructive)
            {
                houseMap.GetLayer("Buildings").Tiles[spouseRoomRect.X + x, spouseRoomRect.Y + y] = null;
            }
        }

        internal static void pasteBackLayer(Map houseMap, Map spouseMap, Rectangle spouseRoomRect, Point spousePoint, int x, int y, bool floor, bool wall, bool clutter, bool destructive)
        {
            //If the house has a tile on the back layer at the corresponding coordinate
            if (spouseMap.GetLayer("Back").Tiles[spousePoint.X + x, spousePoint.Y + y] != null)
            {
                //Get the equivalent tilesheet.
                TileSheet sheet = houseMap.TileSheets[getEquivalentSheet(spouseMap.GetLayer("Back").Tiles[spousePoint.X + x, spousePoint.Y + y])];
                int tileIndex = spouseMap.GetLayer("Back").Tiles[spousePoint.X + x, spousePoint.Y + y].TileIndex;
                bool isWall = false;
                bool isFloor = false;
                if (sheet.ImageSource.Contains("walls_and_floors"))
                {
                    isWall = tileIndex < 336;
                    isFloor = tileIndex >= 336;
                }
                else if (sheet.ImageSource.Contains("townInterior"))
                {
                    isWall = FarmHouseStates.townInteriorWalls.Contains(tileIndex);
                    isFloor = FarmHouseStates.townInteriorFloors.Contains(tileIndex);
                }

                if (isWall && !wall)
                    return;
                if (isFloor && !floor)
                    return;
                houseMap.GetLayer("Back").Tiles[spouseRoomRect.X + x, spouseRoomRect.Y + y]
                    = new StaticTile(
                        houseMap.GetLayer("Back"),
                        sheet,
                        BlendMode.Alpha,
                        spouseMap.GetLayer("Back").Tiles[spousePoint.X + x, spousePoint.Y + y].TileIndex
                );
            }
            else if (destructive)
            {
                houseMap.GetLayer("Back").Tiles[spouseRoomRect.X + x, spouseRoomRect.Y + y] = null;
            }
        }

        internal static int getEquivalentSheet(Tile tile, bool crybaby=false)
        {
            if (tile == null)
                return 0;
            string tileSheetId = tile.TileSheet.Id;
            string tileSheetSourceImage = tile.TileSheet.ImageSource;
            if (tileSheetSourceImage.Contains("walls_and_floors"))
            {
                if(crybaby)
                    Logger.Log(tileSheetId + " appears to be equivalent to walls and floors.");
                return FarmHouseStates.wallAndFloorsSheet;
            }
            if (tileSheetSourceImage.Contains("townInterior"))
            {
                if (crybaby)
                    Logger.Log(tileSheetId + " appears to be equivalent to indoor.");
                return FarmHouseStates.interiorSheet;
            }
            if (tileSheetSourceImage.Contains("farmhouse_tiles"))
            {
                if (crybaby)
                    Logger.Log(tileSheetId + " appears to be equivalent to farmhouse.");
                return FarmHouseStates.farmSheet;
            }
            if (crybaby)
                Logger.Log(tileSheetId + " appears to have no equivalence.");
            return 0;
        }

        internal static void mergeMaps(Map map, Point point, FarmHouse __instance, Rectangle spouseRoomRect)
        {
            IReflectedMethod adjustMapLightPropertiesForLamp = FarmHouseStates.reflector.GetMethod(__instance, "adjustMapLightPropertiesForLamp");
            Dictionary<string, int> mergedSheets = mergeSheets(map, __instance);
            for (int index1 = 0; index1 < spouseRoomRect.Width; ++index1)
            {
                for (int index2 = 0; index2 < spouseRoomRect.Height; ++index2)
                {
                    pasteIfNotNull(__instance.map, map, new Point(spouseRoomRect.X + index1, spouseRoomRect.Y + index2), new Point(point.X + index1, point.Y + index2), mergedSheets, adjustMapLightPropertiesForLamp);
                }
            }
        }

        internal static void pasteIfNotNull(Map house, Map spouse, Point houseTile, Point spouseTile, Dictionary<string, int> sheetDictionary, IReflectedMethod adjustMapLightPropertiesForLamp)
        {
            if(spouse.GetLayer("Back").Tiles[spouseTile.X, spouseTile.Y] != null)
            {
                Tile spouseTileTile = spouse.GetLayer("Back").Tiles[spouseTile.X, spouseTile.Y];
                house.GetLayer("Back").Tiles[houseTile.X, houseTile.Y] = new StaticTile(house.GetLayer("Back"), house.TileSheets[sheetDictionary[spouseTileTile.TileSheet.Id]], BlendMode.Alpha, spouseTileTile.TileIndex);
            }
            if(spouse.GetLayer("Buildings").Tiles[spouseTile.X, spouseTile.Y] != null)
            {
                Tile spouseTileTile = spouse.GetLayer("Buildings").Tiles[spouseTile.X, spouseTile.Y];
                house.GetLayer("Buildings").Tiles[houseTile.X, houseTile.Y] = new StaticTile(house.GetLayer("Buildings"), house.TileSheets[sheetDictionary[spouseTileTile.TileSheet.Id]], BlendMode.Alpha, spouseTileTile.TileIndex);
                adjustMapLightPropertiesForLamp.Invoke(spouse.GetLayer("Buildings").Tiles[spouseTile.X, spouseTile.Y].TileIndex, houseTile.X, houseTile.Y, "Buildings");
            }
            else
            {
                house.GetLayer("Buildings").Tiles[houseTile.X, houseTile.Y] = (Tile)null;
            }
            if (spouse.GetLayer("Front").Tiles[spouseTile.X, spouseTile.Y] != null)
            {
                Tile spouseTileTile = spouse.GetLayer("Front").Tiles[spouseTile.X, spouseTile.Y];
                house.GetLayer("Front").Tiles[houseTile.X, houseTile.Y] = new StaticTile(house.GetLayer("Front"), house.TileSheets[sheetDictionary[spouseTileTile.TileSheet.Id]], BlendMode.Alpha, spouseTileTile.TileIndex);
                adjustMapLightPropertiesForLamp.Invoke(spouse.GetLayer("Front").Tiles[spouseTile.X, spouseTile.Y].TileIndex, houseTile.X, houseTile.Y, "Front");
            }
        }

        internal static Dictionary<string, int> mergeSheets(Map map, FarmHouse __instance)
        {
            Dictionary<string, int> mergedSheets = new Dictionary<string, int>();
            List<TileSheet> sheetsToAdd = new List<TileSheet>();
            foreach (TileSheet sheet in map.TileSheets)
            {
                foreach (TileSheet houseSheet in __instance.map.TileSheets)
                {
                    if (sheet.ImageSource == houseSheet.ImageSource)
                    {
                        Logger.Log("Spouse room had copy of a sheet already within farmhouse, " + sheet.Id + "->" + houseSheet.Id);
                        mergedSheets[sheet.Id] = __instance.map.TileSheets.IndexOf(houseSheet);
                        break;
                    }
                }
                Logger.Log("Spouse room added a new sheet: " + sheet.Id);
                sheetsToAdd.Add(sheet);
            }
            foreach(TileSheet sheet in sheetsToAdd)
            {
                __instance.map.AddTileSheet(sheet);
                mergedSheets[sheet.Id] = __instance.map.TileSheets.IndexOf(sheet);
            }
            return mergedSheets;
        }
    }

    class FarmHouse_showSpouseRoom_Patch
    {

        internal static bool Prefix(FarmHouse __instance)
        {
            bool flag = __instance.owner.isMarried() && __instance.owner.spouse != null;
            IReflectedField<bool> displayingSpouseRoom = FarmHouseStates.reflector.GetField<bool>(__instance, "displayingSpouseRoom");
            int num = displayingSpouseRoom.GetValue() ? 1 : 0;
            displayingSpouseRoom.SetValue(flag);
            __instance.updateMap();
            __instance.loadObjects();

            //Cellar stuff!
            if (__instance.upgradeLevel == 3)
            {
                pasteCellar(__instance);
                //__instance.warps.Add(new Warp(4, 25, "Cellar", 3, 2, false));
                //__instance.warps.Add(new Warp(5, 25, "Cellar", 4, 2, false));
                if (!Game1.player.craftingRecipes.ContainsKey("Cask"))
                    Game1.player.craftingRecipes.Add("Cask", 0);
            }
            if (!flag)
                return false;
            __instance.loadSpouseRoom();
            return false;
        }

        internal static void pasteVanillaCellar(FarmHouse house)
        {
            house.setMapTileIndex(3, 22, 162, "Front", 0);
            house.removeTile(4, 22, "Front");
            house.removeTile(5, 22, "Front");
            house.setMapTileIndex(6, 22, 163, "Front", 0);
            house.setMapTileIndex(3, 23, 64, "Buildings", 0);
            house.setMapTileIndex(3, 24, 96, "Buildings", 0);
            house.setMapTileIndex(4, 24, 165, "Front", 0);
            house.setMapTileIndex(5, 24, 165, "Front", 0);
            house.removeTile(4, 23, "Back");
            house.removeTile(5, 23, "Back");
            house.setMapTileIndex(4, 23, 1043, "Back", 0);
            house.setMapTileIndex(5, 23, 1043, "Back", 0);
            house.setMapTileIndex(4, 24, 1075, "Back", 0);
            house.setMapTileIndex(5, 24, 1075, "Back", 0);
            house.setMapTileIndex(6, 23, 68, "Buildings", 0);
            house.setMapTileIndex(6, 24, 130, "Buildings", 0);
            house.setMapTileIndex(4, 25, 0, "Front", 0);
            house.setMapTileIndex(5, 25, 0, "Front", 0);
            house.removeTile(4, 23, "Buildings");
            house.removeTile(5, 23, "Buildings");
            house.warps.Add(new Warp(4, 25, "Cellar", 3, 2, false));
            house.warps.Add(new Warp(5, 25, "Cellar", 4, 2, false));
        }

        internal static void pasteCellar(FarmHouse house)
        {
            if (FarmHouseStates.levelThreeData == null)
                FarmHouseStates.updateFromMapPath(house.mapPath);
            //Dictionary<Point, Tuple<Map, bool>> levelThreeUpgrades = new Dictionary<Point, Tuple<Map, bool>>();
            if(FarmHouseStates.levelThreeData == "" && house.upgradeLevel == 3)
            {
                Logger.Log("Level three updgrades not defined, using vanilla...");
                //levelThreeUpgrades[new Point(3, 22)] = new Tuple<Map, bool> (FarmHouseStates.loadLevelThreeUpgradeIfPresent("Cellar"), false);
                Map cellarMap = FarmHouseStates.loadLevelThreeUpgradeIfPresent("Cellar");
                if(cellarMap == null)
                {
                    Logger.Log("Couldn't paste the default cellar map, the file was not found!", LogLevel.Error);
                    return;
                }
                pasteMapSection(house.map, cellarMap, new Point(3, 22), new Rectangle(0, 0, cellarMap.GetLayer("Back").LayerWidth, cellarMap.GetLayer("Back").LayerHeight), false);
                Logger.Log("Pasted the default cellar map at (3, 22).");
            }
            else
            {
                Logger.Log("Map defined level 3 upgrade data...");
                string[] levelThree = FarmHouseStates.levelThreeData.Split(' ');
                int x = -1;
                int y = -1;
                string mapName = "Cellar";
                bool destructive = false;
                int readerIndex = 0;
                //Read data of an undefined length, with optional parameters
                while (readerIndex < levelThree.Length)
                {
                    //If this and the next item are both numbers, assume they are an X and Y coordinate.
                    if (isNumeric(levelThree[readerIndex]) && isNumeric(levelThree[readerIndex + 1]))
                    {
                        //We already found coordinates, so this must be the next definition
                        if (x != -1 && y != -1)
                        {
                            //Paste upgrade using data we have so far
                            Map sectionMap = FarmHouseStates.loadLevelThreeUpgradeIfPresent(mapName);
                            if (sectionMap == null)
                            {
                                Logger.Log("Failed to paste " + mapName + "_levelthree at (" + x + ", " + y + "), no map was found!", LogLevel.Error);
                            }
                            else {
                                Rectangle sectionRectangle = new Rectangle(0, 0, sectionMap.GetLayer("Back").LayerWidth, sectionMap.GetLayer("Back").LayerHeight);
                                pasteMapSection(house.map, sectionMap, new Point(x, y), sectionRectangle, destructive);
                            }
                            //Reset variables
                            x = -1;
                            y = -1;
                            mapName = "Cellar";
                            destructive = false;
                        }
                        //Set the coordinates
                        x = Convert.ToInt32(levelThree[readerIndex]);
                        y = Convert.ToInt32(levelThree[readerIndex + 1]);
                        //Skip a number, since we just looked at two at the same time
                        readerIndex += 2;
                    }
                    else if (x == -1 || y == -1)
                    {
                        Logger.Log("Improper level 3 upgrade data!  No coordinates appear to be present!  Please ensure all level 3 definitions begin with an X and Y coordinate.", LogLevel.Error);
                        readerIndex++;
                    }
                    else
                    {
                        //Look for the -destructive flag
                        if (levelThree[readerIndex].StartsWith("-"))
                        {
                            string flag = levelThree[readerIndex].TrimStart('-').ToLower();
                            Logger.Log("Found flag: " + flag);
                            if (flag.StartsWith("d"))
                            {
                                Logger.Log("Interpreted as 'destructive' flag.");
                                destructive = true;
                            }
                        }
                        //If it wasn't numerical or prefixed by the '-' character, interpret it as the map name
                        else
                        {
                            mapName = levelThree[readerIndex];
                            Logger.Log("Map selected: " + mapName);
                        }
                        readerIndex++;
                    }
                }
                if (x != -1 && y != -1)
                {
                    Map sectionMap = FarmHouseStates.loadLevelThreeUpgradeIfPresent(mapName);
                    if (sectionMap == null)
                    {
                        Logger.Log("Failed to paste " + mapName + "_levelthree at (" + x + ", " + y + "), no map was found!", LogLevel.Error);
                    }
                    else
                    {
                        Rectangle sectionRectangle = new Rectangle(0, 0, sectionMap.GetLayer("Back").LayerWidth, sectionMap.GetLayer("Back").LayerHeight);
                        pasteMapSection(house.map, sectionMap, new Point(x, y), sectionRectangle, destructive);
                    }
                }
            }
        }

        internal static bool isNumeric(string candidate)
        {
            int n;
            return int.TryParse(candidate, out n);
        }

        internal static void pasteMapSection(Map houseMap, Map sectionMap, Point housePoint, Rectangle sectionRect, bool destructive = false)
        {
            Logger.Log("Pasting map section named '" + sectionMap.Id + "' at " + housePoint.ToString() + ", with a bounds of " + sectionRect.ToString());
            //Iterate each coordinate within the pasted section
            for(int x = 0; x < sectionRect.Width; x++)
            {
                for(int y = 0; y < sectionRect.Height; y++)
                {
                    pasteTile(houseMap, sectionMap, housePoint, x, y, "Back", destructive);
                    pasteTile(houseMap, sectionMap, housePoint, x, y, "Buildings", destructive);
                    pasteTile(houseMap, sectionMap, housePoint, x, y, "Front", destructive);
                }
            }
            foreach(KeyValuePair<string, PropertyValue> pair in sectionMap.Properties)
            {
                PropertyValue value = pair.Value;
                string propertyName = pair.Key;
                Logger.Log("Upgrade map contained Properties data, applying now...");
                if (propertyName.Equals("Warp"))
                {
                    //propertyName = "Warp";
                    Logger.Log("Adjusting warp property...");
                    string[] warpParts = FarmHouseStates.cleanup(value.ToString()).Split(' ');
                    string warpShifted = "";
                    for (int index = 0; index < warpParts.Length; index += 5)
                    {
                        try
                        {
                            Logger.Log("Relative warp found: " + warpParts[index + 0] + " " + warpParts[index + 1] + " " + warpParts[index + 2] + " " + warpParts[index + 3] + " " + warpParts[index + 4]);
                            int xi = -1;
                            int yi = -1;
                            int xii = -1;
                            int yii = -1;
                            if (warpParts[0].StartsWith("~"))
                                xi = housePoint.X + Convert.ToInt32(warpParts[index + 0].TrimStart('~'));
                            else
                                xi = Convert.ToInt32(warpParts[0]);
                            if (warpParts[1].StartsWith("~"))
                                yi = housePoint.Y + Convert.ToInt32(warpParts[index + 1].TrimStart('~'));
                            else
                                yi = Convert.ToInt32(warpParts[1]);
                            if (warpParts[3].StartsWith("~"))
                                xii = housePoint.X + Convert.ToInt32(warpParts[index + 3].TrimStart('~'));
                            else
                                xii = Convert.ToInt32(warpParts[3]);
                            if (warpParts[4].StartsWith("~"))
                                yii = housePoint.Y + Convert.ToInt32(warpParts[index + 4].TrimStart('~'));
                            else
                                yii = Convert.ToInt32(warpParts[4]);
                            string returnWarp = xi + " " + yi + " " + warpParts[index + 2] + " " + xii + " " + yii + " ";
                            Logger.Log("Relative warp became " + returnWarp);
                            warpShifted += returnWarp;
                        }
                        catch (IndexOutOfRangeException)
                        {
                            Logger.Log("Incomplete warp definition found!  Please ensure all warp definitions are formatted as 'X Y map X Y'", LogLevel.Warn);
                        }
                        catch (FormatException)
                        {
                            Logger.Log("Invalid warp definition found!  Please ensure all warp definitions use numbers fro the X and Y coordinates!", LogLevel.Warn);
                        }
                    }
                    value = warpShifted.Trim(' ');
                    Logger.Log("Warp property is now " + value.ToString());
                }
                if (!houseMap.Properties.ContainsKey(propertyName))
                {
                    Logger.Log("FarmHouse map did not have a '" + propertyName + "' property, setting it to '" + propertyName + "'...");
                    houseMap.Properties.Add(propertyName, value);
                }
                else
                {
                    PropertyValue houseValue = houseMap.Properties[propertyName];
                    Logger.Log("Farmhouse map already had a '" + propertyName + "' value, appending...");
                    houseMap.Properties[propertyName] = (houseValue.ToString() + " " + value.ToString()).Trim(' ');
                    Logger.Log(propertyName + " is now " + houseMap.Properties[propertyName].ToString());
                }
            }
        }

        internal static void pasteTile(Map houseMap, Map sectionMap, Point housePoint, int x, int y, string layer, bool destructive = false)
        {
            //If the pasted map has a tile on the Back layer at that location
            Logger.Log("Trying to paste index (" + x + ", " + y + ") local; (" + (x + housePoint.X) + ", " + (y + housePoint.Y) + ") global...");
            if (sectionMap.GetLayer(layer).Tiles[x, y] != null)
            {
                Logger.Log("Local tile was not null...");
                Tile sectionTile = sectionMap.GetLayer(layer).Tiles[x, y];
                TileSheet sheet = getEquivalentSheet(houseMap, sectionTile.TileSheet);
                Logger.Log("Setting global tile to local tile...");
                houseMap.GetLayer(layer).Tiles[housePoint.X + x, housePoint.Y + y] = new StaticTile(houseMap.GetLayer(layer), sheet, BlendMode.Alpha, sectionTile.TileIndex);
                Logger.Log("Checking for properties...");
                if (sectionTile.Properties.Keys.Count > 0)
                {
                    Logger.Log("Properties discovered.");
                    foreach(KeyValuePair<string, PropertyValue> pair in sectionTile.Properties)
                    {
                        Logger.Log("Applying property '" + pair.Key + "'...");
                        houseMap.GetLayer(layer).Tiles[housePoint.X + x, housePoint.Y + y].Properties[pair.Key] = pair.Value;
                        Logger.Log("Applied.");
                    }
                }
            }
            else if((!layer.Equals("Back") || destructive) && houseMap.GetLayer(layer).Tiles[housePoint.X + x, housePoint.Y + y] != null)
            {
                Logger.Log("Global tile was not null, and local tile was.  Deleting...");
                houseMap.GetLayer(layer).Tiles[housePoint.X + x, housePoint.Y + y] = null;
                Logger.Log("Deleted.");
            }
        }

        internal static TileSheet getEquivalentSheet(Map houseMap, TileSheet sheet)
        {
            if (sheet.ImageSource.Contains("walls_and_floors"))
            {
                return houseMap.TileSheets[FarmHouseStates.wallAndFloorsSheet];
            }
            else if (sheet.ImageSource.Contains("townInterior"))
            {
                return houseMap.TileSheets[FarmHouseStates.interiorSheet];
            }
            else if (sheet.ImageSource.Contains("farmhouse_tiles"))
            {
                return houseMap.TileSheets[FarmHouseStates.farmSheet];
            }
            Logger.Log("Could not find an equivalent sheet with the image source '" + sheet.ImageSource + "' on the FarmHouse map.  Using the townInterior sheet...");
            return houseMap.TileSheets[FarmHouseStates.interiorSheet];
        }
    }

    class FarmHouse_getKitchenStandingSpot_Patch
    {

        public static bool Prefix(FarmHouse __instance, Microsoft.Xna.Framework.Point __result)
        {
            return true;
            try
            {
                Logger.Log("Getting kitchen standing spot...");
                if (FarmHouseStates.kitchenData == null)
                    FarmHouseStates.updateFromMapPath(__instance.mapPath);
                if (FarmHouseStates.kitchenData != "")
                {
                    string[] kitchenPoint = FarmHouseStates.kitchenData.Split(' ');
                    if (kitchenPoint.Length < 2)
                    {
                        Logger.Log("Kitchen standing spot was defined, but did not have at least two numerical values!  Given '" + FarmHouseStates.spouseRoomData + "'.", LogLevel.Error);
                        return true;
                    }
                    try
                    {
                        __result = new Point(Convert.ToInt32(kitchenPoint[0]), Convert.ToInt32(kitchenPoint[1]));
                        Logger.Log("Kitchen standing spot has been set to " + __result.ToString());
                        return false;
                    }
                    catch (FormatException)
                    {
                        Logger.Log("Spouse room was defined, but its values do not seem to be numerical!  Given '" + FarmHouseStates.spouseRoomData + "'.", LogLevel.Error);
                        return true;
                    }
                }
                else
                {
                    return true;
                }
            }
            catch (Exception)
            {
                return true;
            }
        }

    }

    class FarmHouse_performTenMinuteUpdate_patch
    {
        internal static bool Prefix(int timeOfDay, FarmHouse __instance)
        {
            if (__instance is Cabin)
                return true;
            if (Game1.timeOfDay == 2200 && Game1.IsMasterGame)
            {
                foreach(NPC character in __instance.characters)
                {
                    if (character.isMarried())
                    {
                        Logger.Log(character.name + " is going to bed...");
                        character.controller = (PathFindController)null;
                        Point bedPoint = FarmHouseStates.getBedSpot(__instance, true);
                        Logger.Log(character.name + " is attempting to path to " + bedPoint.ToString());
                        character.controller = new PathFindController((Character)character, (GameLocation)__instance, bedPoint, 0);
                        if (character.controller.pathToEndPoint == null || !__instance.isTileOnMap(character.controller.pathToEndPoint.Last<Point>().X, character.controller.pathToEndPoint.Last<Point>().Y))
                        {
                            Logger.Log(character.name + " could not path to " + bedPoint.ToString() + "!");
                            character.controller = (PathFindController)null;
                        }
                        return false;
                    }
                }
            }
            return true;
        }
    }

    class FarmHouse_setMapForUpgradeLevel_patch
    {
        internal static bool Prefix(int level, FarmHouse __instance)
        {
            if (__instance is Cabin)
                return true;
            Logger.Log("Setting map upgrade level...");
            __instance.upgradeLevel = level;
            IReflectedField<int> currentlyDisplayedUpgradeLevel = FarmHouseStates.reflector.GetField<int>(__instance, "currentlyDisplayedUpgradeLevel");
            IReflectedField<bool> displayingSpouseRoom = FarmHouseStates.reflector.GetField<bool>(__instance, "displayingSpouseRoom");
            currentlyDisplayedUpgradeLevel.SetValue(level);
            bool flag = __instance.owner.isMarried() && __instance.owner.spouse != null;

            if (displayingSpouseRoom.GetValue() && !flag)
                displayingSpouseRoom.SetValue(false);
            __instance.updateMap();
            FarmHouseStates.updateFromMapPath(__instance.mapPath);
            if (flag || __instance.upgradeLevel == 3)
                __instance.showSpouseRoom();
            __instance.loadObjects();
            Logger.Log("Updating and setting wall defaults...");
            if(FarmHouseStates.WallsData == null || FarmHouseStates.FloorsData == null)
                FarmHouseStates.updateFromMapPath(__instance.mapPath);
            if (FarmHouseStates.WallsData.Equals(""))
            {
                Logger.Log("No walls data defined, using basegame wall setting method.");
                baseGameSetWallpaper(__instance);
            }
            else
            {
                Logger.Log("Setting wall defaults...");
                FarmHouseStates.setWallpaperDefaults(__instance);
            }
            Logger.Log("Updating and setting floor defaults...");
            if (FarmHouseStates.FloorsData.Equals(""))
            {
                Logger.Log("No floors data defined, using basegame wall setting method.");
                baseGameSetFloors(__instance);
            }
            else
            {
                Logger.Log("Debug using base game floors...");
                baseGameSetFloors(__instance);
            }
            Logger.Log("Clearing light glows...");
            __instance.lightGlows.Clear();
            Logger.Log("Done upgrading the house.");
            for (int wallIndex = 0; wallIndex < __instance.wallPaper.Count; wallIndex++)
            {
                Logger.Log("Wall " + wallIndex + " has a wallpaper index of " + __instance.wallPaper[wallIndex] + ".");
            }
            return false;
        }

        internal static void baseGameSetFloors(FarmHouse house)
        {
            if (house.floor.Count > 0)
            {
                if (house.upgradeLevel == 1 && house.floor.Count == 1)
                {
                    house.setFloor(house.floor[0], 1, true);
                    house.setFloor(house.floor[0], 2, true);
                    house.setFloor(house.floor[0], 3, true);
                    house.setFloor(22, 0, true);
                }
                if (house.upgradeLevel == 2 && house.floor.Count == 3)
                {
                    int which = house.floor[3];
                    house.setFloor(house.floor[2], 5, true);
                    house.setFloor(house.floor[0], 3, true);
                    house.setFloor(house.floor[1], 4, true);
                    house.setFloor(which, 6, true);
                    house.setFloor(1, 0, true);
                    house.setFloor(31, 1, true);
                    house.setFloor(31, 2, true);
                }
            }
        }

        internal static void baseGameSetWallpaper(FarmHouse house)
        {
            if (house.wallPaper.Count > 0)
            {
                if (house.upgradeLevel == 2 && house.wallPaper.Count <= 4)
                {
                    house.setWallpaper(house.wallPaper[0], 4, true);
                    house.setWallpaper(house.wallPaper[2], 6, true);
                    house.setWallpaper(house.wallPaper[1], 5, true);
                    house.setWallpaper(11, 0, true);
                    house.setWallpaper(61, 1, true);
                    house.setWallpaper(61, 2, true);
                }
            }
        }

        
    }


    //Issue: should not make use of typeof(FarmHouse) ever
    //class FarmHouse_getBedSpot_patch
    //{
    //    public static void Postfix(Point __result, FarmHouse __instance)
    //    {
    //        try
    //        {
    //            Logger.Log("Getting bed location...");
    //            if (FarmHouseStates.bedData == null)
    //            {
    //                Logger.Log("Bed was null, updating...");
    //                FarmHouseStates.updateFromMapPath(__instance.mapPath);
    //                Logger.Log("Updated.");
    //            }
    //            if (FarmHouseStates.bedData != "")
    //            {
    //                Logger.Log("Map defined bed location...");
    //                string[] bedPoint = FarmHouseStates.bedData.Split(' ');
    //                __result = new Point(Convert.ToInt32(bedPoint[0]), Convert.ToInt32(bedPoint[1]));
    //                Logger.Log("Bed set to " + __result.ToString());
    //            }
    //            else
    //            {
    //                Logger.Log("Map did not define a bed location, using vanilla.");
    //                //Run the vanilla code
    //                return;
    //            }
    //        }
    //        catch (Exception)
    //        {
    //            Logger.Log("Ran.");
    //        }
    //    }

    //    public static bool Prefix(Point __result, FarmHouse __instance)
    //    {
    //        Logger.Log("Getting bed location...");
    //        if (FarmHouseStates.bedData == null)
    //        {
    //            Logger.Log("Bed was null, updating...");
    //            FarmHouseStates.updateFromMapPath(__instance.mapPath);
    //            Logger.Log("Updated.");
    //        }
    //        if (FarmHouseStates.bedData != "")
    //        {
    //            Logger.Log("Map defined bed location...");
    //            string[] bedPoint = FarmHouseStates.bedData.Split(' ');
    //            __result = new Point(Convert.ToInt32(bedPoint[0]), Convert.ToInt32(bedPoint[1]));
    //            Logger.Log("Bed set to " + __result.ToString());
    //            return false;
    //        }
    //        else
    //        {
    //            Logger.Log("Map did not define a bed location, using vanilla.");
    //            //Run the vanilla code
    //            return true;
    //        }
    //    }
    //}
}
