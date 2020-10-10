/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mjSurber/FarmHouseRedone
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewModdingAPI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using xTile;
using xTile.Tiles;

namespace FarmHouseRedone
{
    public static class FarmState
    {
        public static Vector2 houseLocation;
        public static Vector2 greenhouseLocation;
        public static Vector2 mailBoxLocation;
        public static Vector2 shippingCrateLocation;
        public static Vector2 spouseOutdoorLocation;
        public static Vector2 farmWarpLocation;
        public static Vector2 frontDoorLocation;
        public static Vector2 shrineLocation;

        public static Point porchStandingLocation;

        public static Rectangle remove = new Rectangle(0, 0, 0, 0);

        public static Rectangle houseRect;
        public static Rectangle greenhouseRect;

        public static Random seededRandom;

        public static Dictionary<Vector2, int> buildingShadows;

        public static List<Vector2> chimneys;

        public const int SHADOW_LEFT = 0;
        public const int SHADOW_MID = 1;
        public const int SHADOW_RIGHT = 2;

        public static void init()
        {
            houseLocation = new Vector2(58f, 8.125f);
            frontDoorLocation = new Vector2(64, 14);
            greenhouseLocation = new Vector2(25f, 6f);
            mailBoxLocation = new Vector2(68f, 16f);
            shippingCrateLocation = new Vector2(71f, 14f);
            shrineLocation = new Vector2(512f, 448f);

            spouseOutdoorLocation = new Vector2(69, 6);
            if (Game1.whichFarm == 5)
                farmWarpLocation = new Vector2(48, 39);
            else
                farmWarpLocation = new Vector2(48, 7);

            porchStandingLocation = new Point(66, 15);

            buildingShadows = new Dictionary<Vector2, int>();

            chimneys = new List<Vector2> { new Vector2(4204, 540)};

            houseRect = new Rectangle(0, 144 * ((int)((NetFieldBase<int, NetInt>)Game1.MasterPlayer.houseUpgradeLevel) == 3 ? 2 : (int)((NetFieldBase<int, NetInt>)Game1.MasterPlayer.houseUpgradeLevel)), 160, 144);
            greenhouseRect = new Microsoft.Xna.Framework.Rectangle(160, 160 * (Game1.MasterPlayer.mailReceived.Contains("ccPantry") ? 1 : 0), 112, 160);
        }

        public static int seedFromString(string seedString)
        {
            int seed = 0;
            foreach(Char c in seedString)
            {
                seed += (int)c;
            }
            Logger.Log("Seed for this save is " + seed + ".  Source was " + seedString);
            return seed;
        }

        public static void setUpBaseFarm()
        {
            setUpFarm(Game1.getFarm());
        }

        public static void setUpFarm(Farm farm)
        {
            //seededRandom = new Random(seedFromString(StardewModdingAPI.Constants.SaveFolderName));
            seededRandom = new Random(seedFromString(Constants.SaveFolderName.Split('_')[1]));
            Map map = farm.map;
            if (map.Properties.ContainsKey("FarmHouse"))
            {
                Logger.Log("Farm contained a value for FarmHouse: " + map.Properties["FarmHouse"].ToString());
                setFarmhouseLocation(chooseHouseLocation(map.Properties["FarmHouse"].ToString()));
            }
            else
            {
                setFarmhouseLocation(new Vector2(64, 14));
            }
            if (map.Properties.ContainsKey("GreenHouse"))
            {
                Logger.Log("Farm contained a value for GreenHouse: " + map.Properties["GreenHouse"].ToString());
                setGreenhouseLocation(getCoordsFromString(map.Properties["GreenHouse"].ToString()));
            }
            else if(Game1.whichFarm == 5)
            {
                setGreenhouseLocation(new Vector2(39, 34));
            }
            if (map.Properties.ContainsKey("ShippingBin"))
            {
                Logger.Log("Farm contained a value for ShippingBin: " + map.Properties["ShippingBin"].ToString());
                setShippingCrateLocation(getCoordsFromString(map.Properties["ShippingBin"].ToString()));
            }
            if (map.Properties.ContainsKey("Mailbox"))
            {
                Logger.Log("Farm contained a value for Mailbox: " + map.Properties["Mailbox"].ToString());
                setMailBoxLocation(getCoordsFromString(map.Properties["Mailbox"].ToString()));
            }
            if (map.Properties.ContainsKey("Patio"))
            {
                Logger.Log("Farm contained a value for Patio: " + map.Properties["Patio"].ToString());
                setPatioLocation(getCoordsFromString(map.Properties["Patio"].ToString()));
            }
            if (map.Properties.ContainsKey("WarpStatue"))
            {
                Logger.Log("Farm contained a value for WarpStatue: " + map.Properties["WarpStatue"].ToString());
                setWarpLocation(getCoordsFromString(map.Properties["WarpStatue"].ToString()));
            }
            if (map.Properties.ContainsKey("Shrine"))
            {
                Logger.Log("Farm contained a value for Shrine: " + map.Properties["Shrine"].ToString());
                setShrineLocation(getCoordsFromString(map.Properties["Shrine"].ToString()));
            }
            //setMailBoxLocation(new Vector2(67, 32));
            //setShippingCrateLocation(new Vector2(72f, 14f));
        }

        public static Map getFarmHouseCollisionMap(FarmHouse house)
        {
            Map collisionMap = getCollisionMap("FarmHouse" + house.upgradeLevel, true);
            if(collisionMap == null)
            {
                return getCollisionMap("FarmHouse");
            }
            return collisionMap;
        }

        public static Map getCollisionMap(string mapName, bool ignoreProblems = false)
        {
            try
            {
                return FarmHouseStates.loader.Load<Map>("Maps/Collision_" + mapName, StardewModdingAPI.ContentSource.GameContent);
            }
            catch (Microsoft.Xna.Framework.Content.ContentLoadException)
            {
                if(!ignoreProblems)
                    Logger.Log("No map found at maps/Collision_" + mapName + " within the game content, using default if provided...");
            }
            try
            {
                return FarmHouseStates.loader.Load<Map>("assets/maps/Collision_" + mapName + ".tbin", StardewModdingAPI.ContentSource.ModFolder);
            }
            catch (Microsoft.Xna.Framework.Content.ContentLoadException)
            {
                if(!ignoreProblems)
                    Logger.Log("No map found at assets/maps/Collision_" + mapName + ".tbin within either the game content or the mod content!", LogLevel.Error);
            }
            return null;
        }

        public static bool hasFarmWarpChanged()
        {
            if(Game1.whichFarm == 5)
            {
                return farmWarpLocation != new Vector2(48, 39);
            }
            else
            {
                return farmWarpLocation != new Vector2(48, 7);
            }
        }

        public static Vector2 getFrontDoorOffset()
        {
            return new Vector2(frontDoorLocation.X - 64, frontDoorLocation.Y - 14);
        }

        public static Vector2 chooseHouseLocation(string input)
        {
            input = Utility.cleanup(input);
            int totalWeights = 0;
            Dictionary<Vector2, int> weightedLocations = new Dictionary<Vector2, int>();

            string[] inputs = input.Split(' ');

            int x = -1;
            int y = -1;
            int thisWeight = 100;
            for(int i = 0; i < inputs.Length;)
            {
                Logger.Log("Index " + i + ": " + inputs[i]);
                if (isNumeric(inputs[i]) && isNumeric(inputs[i + 1]))
                {
                    if (x != -1 && y != -1)
                    {
                        totalWeights += thisWeight;
                        weightedLocations[new Vector2(x, y)] = totalWeights;
                        Logger.Log("Created farmhouse location possibility at (" + x + ", " + y + "), with a weight of " + totalWeights);
                        x = -1;
                        y = -1;
                        thisWeight = 100;
                    }
                    x = Convert.ToInt32(inputs[i]);
                    y = Convert.ToInt32(inputs[i + 1]);
                    i += 2;
                }
                else if (x == -1 || y == -1)
                {
                    Logger.Log("Improper farmhouse relocation data!  No coordinates appear to be present!  Please ensure all farmhouse definitions begin with an X and Y coordinate.", LogLevel.Error);
                    i++;
                }
                else
                {
                    if (inputs[i].StartsWith("#"))
                    {
                        Logger.Log("Interpreting " + inputs[i] + " as a weight...");
                        if (isNumeric(inputs[i].Substring(1)))
                        {
                            thisWeight = Convert.ToInt32(inputs[i].Substring(1));
                            Logger.Log("Weight set to " + thisWeight);
                        }
                    }
                    i++;
                }
            }
            if (x != -1 && y != -1)
            {
                totalWeights += thisWeight;
                weightedLocations[new Vector2(x, y)] = totalWeights;
                Logger.Log("Created farmhouse location possibility at (" + x + ", " + y + "), with a weight of " + totalWeights);
            }

            int randomFromSeed = seededRandom.Next(totalWeights);
            Logger.Log("Random value from seed: " + randomFromSeed);
            foreach(Vector2 tileLocation in weightedLocations.Keys)
            {
                if(weightedLocations[tileLocation] >= randomFromSeed)
                {
                    Logger.Log("Chosen " + tileLocation.ToString() + "; weight " + weightedLocations[tileLocation]);
                    return tileLocation;
                }
            }
            return weightedLocations.Keys.First();
        }

        internal static bool isNumeric(string candidate)
        {
            int n;
            return int.TryParse(candidate, out n);
        }

        public static Vector2 getCoordsFromString(string input)
        {
            input = Utility.cleanup(input);
            string[] coords = input.Split(' ');
            if(coords.Length < 2)
            {
                Logger.Log("Coordinate " + input + " did not seem to represent two numerical integer values!", StardewModdingAPI.LogLevel.Error);
                return Vector2.Zero;
            }
            try
            {
                int x = Convert.ToInt32(coords[0]);
                int y = Convert.ToInt32(coords[1]);
                return new Vector2(x, y);
            }
            catch (FormatException)
            {
                Logger.Log("Coordinate " + input + " did not seem to represent two numerical integer values!", StardewModdingAPI.LogLevel.Error);
                return Vector2.Zero;
            }
        }

        public static void setMailBoxLocation(Vector2 tileLocation)
        {
            Farm farm = Game1.getFarm();
            mailBoxLocation = new Vector2(tileLocation.X, tileLocation.Y);

            Map map = farm.map;
            Logger.Log("Moving mailbox...");
            removeVanillaMailboxTiles(map);
            removeEverythingFromTile(farm, tileLocation);
            buildMailBox(map);
        }

        public static void setPatioLocation(Vector2 tileLocation)
        {
            Logger.Log("Moving patio...");
            spouseOutdoorLocation = new Vector2(tileLocation.X, tileLocation.Y);
        }

        public static void setWarpLocation(Vector2 tileLocation)
        {
            Logger.Log("Moving warp statue...");
            farmWarpLocation = new Vector2(tileLocation.X, tileLocation.Y);
        }

        public static void setShrineLocation(Vector2 tileLocation)
        {
            Logger.Log("Moving shrine...");
            shrineLocation = new Vector2(tileLocation.X * 64f, tileLocation.Y * 64f);
            buildShrine(Game1.getFarm().map);
        }

        public static void buildShrine(Map map)
        {
            int x = (int)(shrineLocation.X / 64f);
            int y = (int)(shrineLocation.Y / 64f);
            if(map.GetLayer("Buildings").Tiles[x,y] != null)
            {
                string actionProperty = (map.GetLayer("Buildings").Tiles[x, y].Properties.ContainsKey("Action") ? map.GetLayer("Buildings").Tiles[x, y].Properties["Action"].ToString() : "");
                if (actionProperty.Contains("Message"))
                    return;
                actionProperty += "Message \"Farm.1\"";
                map.GetLayer("Buildings").Tiles[x, y].Properties["Action"] = actionProperty;
            }
        }

        public static void buildMailBox(Map map)
        {
            TileSheet sheet = null;
            foreach (TileSheet tSheet in map.TileSheets)
            {
                if (tSheet.ImageSource.Contains("outdoorsTileSheet"))
                {
                    sheet = tSheet;
                    break;
                }
            }
            if (sheet == null)
            {
                sheet = map.TileSheets[0];
                Logger.Log("Could not find outdoor tilesheet!  Defaulting to the first available tilesheet, '" + sheet.Id + "'...");
            }

            xTile.Layers.Layer buildings = map.GetLayer("Buildings");
            xTile.Layers.Layer front = map.GetLayer("Front");

            buildings.Tiles[(int)mailBoxLocation.X, (int)mailBoxLocation.Y] = new StaticTile(buildings, sheet, BlendMode.Alpha, 1955);
            buildings.Tiles[(int)mailBoxLocation.X, (int)mailBoxLocation.Y].Properties["Action"] = "Mailbox";
            front.Tiles[(int)mailBoxLocation.X, (int)mailBoxLocation.Y - 1] = new StaticTile(front, sheet, BlendMode.Alpha, 1930);
        }

        public static void removeVanillaMailboxTiles(Map map)
        {
            xTile.Layers.Layer buildings = map.GetLayer("Buildings");
            xTile.Layers.Layer front = map.GetLayer("Front");

            deleteTileIfIndex(buildings, 68, 16, 1955);
            deleteTileIfIndex(front, 68, 15, 1930);
        }

        public static void setShippingCrateLocation(Vector2 tileLocation)
        {
            Farm farm = Game1.getFarm();
            shippingCrateLocation = new Vector2(tileLocation.X, tileLocation.Y);

            IReflectedField<Rectangle> shippingBinLidOpenArea = FarmHouseStates.reflector.GetField<Rectangle>(farm, "shippingBinLidOpenArea");
            shippingBinLidOpenArea.SetValue(new Rectangle((int)(shippingCrateLocation.X - 1) * 64, (int)(shippingCrateLocation.Y - 1) * 64, 256, 192));

            Map map = farm.map;
            Logger.Log("Moving shipping crate tiles...");
            removeVanillaShippingCrateTiles(map);
            removeEverythingFromTile(farm, shippingCrateLocation);
            removeEverythingFromTile(farm, shippingCrateLocation + new Vector2(1, 0));
            buildShippingCrate(map);
        }

        public static void buildShippingCrate(Map map)
        {
            TileSheet sheet = null;
            foreach (TileSheet tSheet in map.TileSheets)
            {
                if (tSheet.ImageSource.Contains("outdoorsTileSheet"))
                {
                    sheet = tSheet;
                    break;
                }
            }
            if (sheet == null)
            {
                sheet = map.TileSheets[0];
                Logger.Log("Could not find outdoor tilesheet!  Defaulting to the first available tilesheet, '" + sheet.Id + "'...");
            }

            xTile.Layers.Layer buildings = map.GetLayer("Buildings");
            xTile.Layers.Layer front = map.GetLayer("Front");

            buildings.Tiles[(int)shippingCrateLocation.X, (int)shippingCrateLocation.Y] = new StaticTile(buildings, sheet, BlendMode.Alpha, 387);
            buildings.Tiles[(int)shippingCrateLocation.X + 1, (int)shippingCrateLocation.Y] = new StaticTile(buildings, sheet, BlendMode.Alpha, 388);

            front.Tiles[(int)shippingCrateLocation.X, (int)shippingCrateLocation.Y - 1] = new StaticTile(front, sheet, BlendMode.Alpha, 362);
            front.Tiles[(int)shippingCrateLocation.X + 1, (int)shippingCrateLocation.Y - 1] = new StaticTile(front, sheet, BlendMode.Alpha, 363);
        }

        public static void removeVanillaShippingCrateTiles(Map map)
        {
            xTile.Layers.Layer buildings = map.GetLayer("Buildings");
            xTile.Layers.Layer front = map.GetLayer("Front");

            deleteTileIfIndex(buildings, 71, 14, 387);
            deleteTileIfIndex(buildings, 72, 14, 388);
            deleteTileIfIndex(front, 71, 13, 362);
            deleteTileIfIndex(front, 72, 13, 363);
        }

        public static void deleteTileIfIndex(xTile.Layers.Layer layer, int x, int y, int index)
        {
            if (layer.Tiles[x, y] != null && layer.Tiles[x, y].TileIndex == index)
                layer.Tiles[x, y] = null;
        }

        public static void setGreenhouseLocation(Vector2 tileLocation)
        {
            Farm farm = Game1.getFarm();
            greenhouseLocation = new Vector2(tileLocation.X - 3f, tileLocation.Y - 9f);

            GameLocation greenHouse = null;

            foreach (GameLocation location in Game1.locations)
            {
                if (location.name.Equals("Greenhouse"))
                {
                    greenHouse = location;
                    break;
                }
            }

            foreach (Warp warp in greenHouse.warps)
            {
                if (warp.TargetName.Equals("Farm"))
                {
                    warp.TargetX = (int)tileLocation.X;
                    warp.TargetY = (int)tileLocation.Y + 1;
                }
            }

            setGreenhouseCollision(farm, new Vector2(tileLocation.X - 3f, tileLocation.Y - 5f), greenHouse);
        }

        public static void setGreenhouseCollision(Farm farm, Vector2 topLeft, GameLocation greenHouse)
        {
            Map map = farm.map;
            Map collisionMap = FarmHouseStates.loader.Load<Map>("assets/maps/Collision_GreenHouse.tbin", StardewModdingAPI.ContentSource.ModFolder);

            TileSheet sheet = null;
            foreach (TileSheet tSheet in map.TileSheets)
            {
                if (tSheet.ImageSource.Contains("outdoorsTileSheet"))
                {
                    sheet = tSheet;
                    break;
                }
            }
            if (sheet == null)
            {
                sheet = map.TileSheets[0];
                Logger.Log("Could not find outdoor tilesheet!  Defaulting to the first available tilesheet, '" + sheet.Id + "'...");
            }

            //string houseWarp = "Warp " + FarmHouseStates.getEntryLocation(house).X + " " + FarmHouseStates.getEntryLocation(house).Y + " FarmHouse";
            //Logger.Log("House warp set to '" + houseWarp + "'");

            for (int x = 0; x < collisionMap.GetLayer("Back").LayerWidth; x++)
            {
                for (int y = 0; y < collisionMap.GetLayer("Back").LayerHeight; y++)
                {
                    Vector2 currentTile = new Vector2(x + topLeft.X, y + topLeft.Y);

                    if (collisionMap.GetLayer("Back").Tiles[x, y] != null)
                    {
                        if (map.GetLayer("Back").Tiles[x + (int)topLeft.X, y + (int)topLeft.Y] == null)
                            map.GetLayer("Back").Tiles[x + (int)topLeft.X, y + (int)topLeft.Y] = new StaticTile(map.GetLayer("Back"), sheet, BlendMode.Alpha, 16);
                        foreach (string key in collisionMap.GetLayer("Back").Tiles[x, y].Properties.Keys)
                        {
                            map.GetLayer("Back").Tiles[x + (int)topLeft.X, y + (int)topLeft.Y].Properties[key] = collisionMap.GetLayer("Back").Tiles[x, y].Properties[key];
                        }
                    }
                    if (collisionMap.GetLayer("Buildings").Tiles[x, y] != null)
                    {
                        if (map.GetLayer("Buildings").Tiles[x + (int)topLeft.X, y + (int)topLeft.Y] == null)
                            map.GetLayer("Buildings").Tiles[x + (int)topLeft.X, y + (int)topLeft.Y] = new StaticTile(map.GetLayer("Buildings"), sheet, BlendMode.Alpha, 16);
                        foreach (string key in collisionMap.GetLayer("Buildings").Tiles[x, y].Properties.Keys)
                        {
                            map.GetLayer("Buildings").Tiles[x + (int)topLeft.X, y + (int)topLeft.Y].Properties[key] = collisionMap.GetLayer("Buildings").Tiles[x, y].Properties[key];
                        }
                        removeEverythingFromTile(farm, currentTile);
                    }
                    else if (map.GetLayer("Buildings").Tiles[x + (int)topLeft.X, y + (int)topLeft.Y] != null)
                    {
                        map.GetLayer("Buildings").Tiles[x + (int)topLeft.X, y + (int)topLeft.Y] = null;
                    }
                }
            }
        }

        public static void setFarmhouseLocation(Vector2 tileLocation)
        {
            Farm farm = Game1.getFarm();
            frontDoorLocation = tileLocation;
            houseLocation = new Vector2(tileLocation.X - 6f, (tileLocation.Y - 6f) + 0.125f);

            FarmHouse house = null;

            foreach (GameLocation location in Game1.locations)
            {
                if (location is FarmHouse && !(location is Cabin))
                {
                    house = location as FarmHouse;
                    break;
                }
            }

            foreach(Warp warp in house.warps)
            {
                if (warp.TargetName.Equals("Farm"))
                {
                    warp.TargetX = (int)tileLocation.X;
                    warp.TargetY = (int)tileLocation.Y + 1;
                }
            }

            //setFarmhouseCollision(farm, new Vector2(tileLocation.X - 5f, tileLocation.Y - 3f), house);
            setFarmhouseCollision(farm, new Vector2(tileLocation.X, tileLocation.Y), house);
        }

        public static void setFarmhouseCollision(Farm farm, Vector2 topLeft, FarmHouse house)
        {
            Map map = farm.map;
            Map collisionMap = getFarmHouseCollisionMap(house); //FarmHouseStates.loader.Load<Map>("assets/maps/Collision_FarmHouse.tbin", StardewModdingAPI.ContentSource.ModFolder);

            TileSheet sheet = null;
            foreach(TileSheet tSheet in map.TileSheets)
            {
                if (tSheet.ImageSource.Contains("outdoorsTileSheet"))
                {
                    sheet = tSheet;
                    break;
                }
            }
            if(sheet == null)
            {
                sheet = map.TileSheets[0];
                Logger.Log("Could not find outdoor tilesheet!  Defaulting to the first available tilesheet, '" + sheet.Id + "'...");
            }

            string houseWarp = "Warp " + FarmHouseStates.getEntryLocation(house).X + " " + FarmHouseStates.getEntryLocation(house).Y + " FarmHouse";
            Logger.Log("House warp set to '" + houseWarp + "'");

            

            Logger.Log("Finding the front door on the collision map for the farmhouse...");
            Vector2 frontDoor = new Vector2(-1, -1);

            for(int x = 0; x < collisionMap.GetLayer("Buildings").LayerWidth; x++)
            {
                for(int y = 0; y < collisionMap.GetLayer("Buildings").LayerHeight; y++)
                {
                    if(collisionMap.GetLayer("Buildings").Tiles[x,y] != null && collisionMap.GetLayer("Buildings").Tiles[x, y].Properties.ContainsKey("Action"))
                    {
                        string tileAction = collisionMap.GetLayer("Buildings").Tiles[x, y].Properties["Action"].ToString();
                        if(tileAction.Contains("Warp") && tileAction.Contains("FarmHouse"))
                        {
                            Logger.Log("Found the front door!  Located at (" + x + ", " + y + ")");
                            frontDoor = new Vector2(x, y);
                            break;
                        }
                    }
                }
                if (frontDoor.X != -1)
                    break;
            }

            if (frontDoor.X == -1)
                frontDoor = new Vector2(5, 4);

            topLeft.X -= frontDoor.X;
            topLeft.Y -= frontDoor.Y;

            bool useDefaultPorch = true;
            if (collisionMap.Properties.ContainsKey("Porch"))
            {
                string porchProperty = Utility.cleanup(collisionMap.Properties["Porch"]);

                string[] coords = porchProperty.Split(' ');
                if (coords.Length >= 2)
                {
                    Vector2 coordsFromString = getCoordsFromString(porchProperty);
                    porchStandingLocation = new Point((int)(coordsFromString.X + topLeft.X), (int)(coordsFromString.Y + topLeft.Y));
                    Logger.Log("Found valid porch property; porch is now at " + porchStandingLocation.ToString());
                    useDefaultPorch = false;
                }
            }
            if (useDefaultPorch)
            {
                porchStandingLocation = new Point((int)topLeft.X + 7, (int)topLeft.Y + 5);
            }

            chimneys.Clear();

            for (int x = 0; x < collisionMap.GetLayer("Back").LayerWidth; x++)
            {
                for (int y = 0; y < collisionMap.GetLayer("Back").LayerHeight; y++)
                {
                    Vector2 currentTile = new Vector2(x + topLeft.X, y + topLeft.Y);

                    if (collisionMap.GetLayer("Back").Tiles[x, y] != null)
                    {
                        if (map.GetLayer("Back").Tiles[x + (int)topLeft.X, y + (int)topLeft.Y] == null)
                            map.GetLayer("Back").Tiles[x + (int)topLeft.X, y + (int)topLeft.Y] = new StaticTile(map.GetLayer("Back"), sheet, BlendMode.Alpha, 16);
                        foreach (string key in collisionMap.GetLayer("Back").Tiles[x, y].Properties.Keys)
                        {
                            map.GetLayer("Back").Tiles[x + (int)topLeft.X, y + (int)topLeft.Y].Properties[key] = collisionMap.GetLayer("Back").Tiles[x, y].Properties[key];
                        }
                    }
                    if (collisionMap.GetLayer("Buildings").Tiles[x,y] != null)
                    {
                        if (map.GetLayer("Buildings").Tiles[x + (int)topLeft.X, y + (int)topLeft.Y] == null)
                            map.GetLayer("Buildings").Tiles[x + (int)topLeft.X, y + (int)topLeft.Y] = new StaticTile(map.GetLayer("Buildings"), sheet, BlendMode.Alpha, 16);
                        if (collisionMap.GetLayer("Buildings").Tiles[x, y].TileSheet == sheet && collisionMap.GetLayer("Buildings").Tiles[x, y].TileIndex == 125)
                        {
                            map.GetLayer("Buildings").Tiles[x + (int)topLeft.X, y + (int)topLeft.Y].Properties["Passable"] = "T";
                        }
                        foreach (string key in collisionMap.GetLayer("Buildings").Tiles[x, y].Properties.Keys)
                        {
                            if (key.Equals("Action"))
                            {
                                string warpValue = collisionMap.GetLayer("Buildings").Tiles[x, y].Properties[key].ToString();
                                if (warpValue.Contains("Warp") && warpValue.Contains("FarmHouse"))
                                {
                                    map.GetLayer("Buildings").Tiles[x + (int)topLeft.X, y + (int)topLeft.Y].Properties[key] = houseWarp;
                                }
                            }
                            else if (key.Equals("Chimney"))
                            {
                                Logger.Log("Found a chimney at tile (" + x + ", " + y + ")");
                                string chimneyValue = collisionMap.GetLayer("Buildings").Tiles[x, y].Properties[key].ToString();
                                if(chimneyValue.Split(' ').Length == 1)
                                {
                                    Vector2 newChimney = new Vector2((topLeft.X + x + 0.5f) * 64f, (topLeft.Y + y + 0.5f) * 64f);
                                    Logger.Log("Chimney did not specify pixel coordinates; using tile center instead: " + newChimney.ToString());
                                    chimneys.Add(newChimney);
                                }
                                else
                                {
                                    Vector2 chimneyPosition = getCoordsFromString(chimneyValue);
                                    Vector2 newChimney = new Vector2((topLeft.X + x) * 64f + chimneyPosition.X, (topLeft.Y + y) * 64f + chimneyPosition.Y);
                                    Logger.Log("Chimney did specified pixel coordinates; " + chimneyPosition.ToString() + ": " + newChimney.ToString());
                                    chimneys.Add(newChimney);
                                }
                            }
                            else
                            {
                                map.GetLayer("Buildings").Tiles[x + (int)topLeft.X, y + (int)topLeft.Y].Properties[key] = collisionMap.GetLayer("Buildings").Tiles[x, y].Properties[key];
                            }
                        }
                        removeEverythingFromTile(farm, currentTile);
                    }
                    //else if(map.GetLayer("Buildings").Tiles[x + (int)topLeft.X, y + (int)topLeft.Y] != null)
                    //{
                    //    map.GetLayer("Buildings").Tiles[x + (int)topLeft.X, y + (int)topLeft.Y] = null;
                    //}

                    
                    if (castsShadow(collisionMap, x, y) && (y == collisionMap.GetLayer("Buildings").LayerHeight - 1 || !castsShadow(collisionMap, x, y+1)))
                    {
                        Logger.Log("Tile (" + x + ", " + y + ") casts a shadow...");
                        if(x == 0 || !castsShadow(collisionMap, x-1,y))
                        {
                            Logger.Log("Shadow is on the LEFT");
                            buildingShadows[new Vector2(x + (int)topLeft.X, y + (int)topLeft.Y)] = SHADOW_LEFT;
                        }
                        else if (x == collisionMap.GetLayer("Back").LayerWidth - 1 || !castsShadow(collisionMap, x + 1, y))
                        {
                            Logger.Log("Shadow is on the RIGHT");
                            buildingShadows[new Vector2(x + (int)topLeft.X, y + (int)topLeft.Y)] = SHADOW_RIGHT;
                        }
                        else
                        {
                            buildingShadows[new Vector2(x + (int)topLeft.X, y + (int)topLeft.Y)] = SHADOW_MID;
                        }
                    }
                }
            }
        }

        public static bool castsShadow(Map map, int x, int y)
        {
            if (map.GetLayer("Buildings").Tiles[x, y] != null)
            {
                return (!map.GetLayer("Buildings").Tiles[x, y].Properties.ContainsKey("NoShadow"));
            }
            else if (map.GetLayer("Back").Tiles[x, y] != null)
            {
                return (map.GetLayer("Back").Tiles[x, y].Properties.ContainsKey("Shadow"));
            }
            return false;
        }

        public static void removeEverythingFromTile(Farm location, Vector2 tileLocation)
        {
            if (location.terrainFeatures.ContainsKey(tileLocation))
            {
                Logger.Log("Removing " + location.terrainFeatures[tileLocation].GetType().ToString() + " at " + tileLocation.ToString());
                location.terrainFeatures.Remove(tileLocation);
            }
            if (location.objects.ContainsKey(tileLocation))
            {
                Logger.Log("Removing " + location.objects[tileLocation].GetType().ToString() + " at " + tileLocation.ToString());
                location.objects.Remove(tileLocation);
            }
            List<StardewValley.TerrainFeatures.ResourceClump> clumpsToRemove = new List<StardewValley.TerrainFeatures.ResourceClump>();
            foreach (StardewValley.TerrainFeatures.ResourceClump clump in location.resourceClumps)
            {
                if (clump.occupiesTile((int)tileLocation.X, (int)tileLocation.Y))
                {
                    clumpsToRemove.Add(clump);
                }
            }
            foreach (StardewValley.TerrainFeatures.ResourceClump clump in clumpsToRemove)
            {
                location.resourceClumps.Remove(clump);
            }
        }

        public static Point getFrontDoorSpot()
        {
            Point frontDoor = new Point((int)frontDoorLocation.X, (int)frontDoorLocation.Y);
            Logger.Log("Got patched front door spot: " + frontDoor.ToString());
            return frontDoor;
        }

        public static Microsoft.Xna.Framework.Point getPorchStandingSpotAndLog()
        {
            Point porchSpot = getPorchStandingSpot();
            Logger.Log("Porch standing spot: " + porchSpot.ToString());
            Logger.Log("House location: " + houseLocation.ToString());
            return porchSpot;
        }

        public static Microsoft.Xna.Framework.Point getPorchStandingSpot()
        {
            //return new Point((int)houseLocation.X + 8, (int)houseLocation.Y + 7);
            return porchStandingLocation;
        }

        public static void draw(SpriteBatch b)
        {
            if(b == null || b.IsDisposed)
            {
                Logger.Log("Spritebatch was not ready!", LogLevel.Error);
                return;
            }
            b.Draw(
                Farm.houseTextures,
                Game1.GlobalToLocal(Game1.viewport, new Vector2(FarmState.houseLocation.X * 64f, FarmState.houseLocation.Y * 64f)),
                new Microsoft.Xna.Framework.Rectangle?(FarmState.houseRect),
                Color.White,
                0.0f,
                Vector2.Zero,
                4f,
                SpriteEffects.None,
                ((FarmState.houseLocation.Y + 7) * 64f) / 10000f
            );
            b.Draw(
                Farm.houseTextures,
                Game1.GlobalToLocal(Game1.viewport, new Vector2(FarmState.greenhouseLocation.X * 64f, FarmState.greenhouseLocation.Y * 64f)),
                new Microsoft.Xna.Framework.Rectangle?(FarmState.greenhouseRect),
                Color.White,
                0.0f,
                Vector2.Zero,
                4f,
                SpriteEffects.None,
                ((FarmState.greenhouseLocation.Y + 7) * 64f) / 10000f
            );
            foreach (Vector2 key in FarmState.buildingShadows.Keys)
            {
                Rectangle shadowToCast;
                switch (FarmState.buildingShadows[key])
                {
                    case 0:
                        shadowToCast = Building.leftShadow;
                        break;
                    case 2:
                        shadowToCast = Building.rightShadow;
                        break;
                    default:
                        shadowToCast = Building.middleShadow;
                        break;
                }
                b.Draw(
                    Game1.mouseCursors,
                    Game1.GlobalToLocal(Game1.viewport, new Vector2(key.X * 64, (key.Y + 1) * 64)),
                    new Microsoft.Xna.Framework.Rectangle?(shadowToCast),
                    Color.White,
                    0.0f,
                    Vector2.Zero,
                    4f,
                    SpriteEffects.None,
                    ((key.Y + 1) * 64f) / 10000f
                );
            }
            if (Game1.mailbox.Count > 0)
            {
                float num = (float)(4.0 * Math.Round(Math.Sin(DateTime.UtcNow.TimeOfDay.TotalMilliseconds / 250.0), 2));
                b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(FarmState.mailBoxLocation.X * 64f, (FarmState.mailBoxLocation.Y - 2.25f) * 64f + num)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(141, 465, 20, 24)), Color.White * 0.75f, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, ((FarmState.mailBoxLocation.Y + 1) * 64 + 0.1f) / 10000f);
                b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(FarmState.mailBoxLocation.X * 64f + 36f, (FarmState.mailBoxLocation.Y - 1.5f) * 64f + num)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(189, 423, 15, 13)), Color.White, 0.0f, new Vector2(7f, 6f), 4f, SpriteEffects.None, ((FarmState.mailBoxLocation.Y + 1) * 64 + 1f) / 10000f);
            }
        }

        public static void updateChimneySmoke()
        {
            FarmHouse homeOfFarmer = StardewValley.Utility.getHomeOfFarmer(Game1.MasterPlayer);
            Farm farm = Game1.getFarm();
            if (homeOfFarmer != null && homeOfFarmer.hasActiveFireplace())
            {
                Point porchStandingSpot = homeOfFarmer.getPorchStandingSpot();
                foreach(Vector2 chimney in chimneys)
                {
                    List<TemporaryAnimatedSprite> temporarySprites = farm.temporarySprites;
                    TemporaryAnimatedSprite temporaryAnimatedSprite = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(372, 1956, 10, 10), chimney, false, 1f / 500f, Color.Gray);
                    temporaryAnimatedSprite.alpha = 0.75f;
                    Vector2 vector2_1 = new Vector2(0.0f, -0.5f);
                    temporaryAnimatedSprite.motion = vector2_1;
                    Vector2 vector2_2 = new Vector2(1f / 500f, 0.0f);
                    temporaryAnimatedSprite.acceleration = vector2_2;
                    double num1 = 99999.0;
                    temporaryAnimatedSprite.interval = (float)num1;
                    double num2 = 1.0;
                    temporaryAnimatedSprite.layerDepth = (float)num2;
                    double num3 = 2.0;
                    temporaryAnimatedSprite.scale = (float)num3;
                    double num4 = 0.0199999995529652;
                    temporaryAnimatedSprite.scaleChange = (float)num4;
                    double num5 = (double)Game1.random.Next(-5, 6) * 3.14159274101257 / 256.0;
                    temporaryAnimatedSprite.rotationChange = (float)num5;
                    temporarySprites.Add(temporaryAnimatedSprite);
                }
            }
        }

        public static Point getMailboxPosition(Farmer p)
        {
            foreach (StardewValley.Buildings.Building building in Game1.getFarm().buildings)
            {
                if (building.isCabin && building.nameOfIndoors == (string)((NetFieldBase<string, NetString>)p.homeLocation))
                    return building.getMailboxPosition();
            }
            return new Point((int)mailBoxLocation.X, (int)mailBoxLocation.Y);
        }
    }
}
