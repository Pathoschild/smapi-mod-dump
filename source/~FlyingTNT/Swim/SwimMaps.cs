/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/FlyingTNT/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using xTile;
using xTile.Dimensions;
using xTile.ObjectModel;
using xTile.Tiles;
using Object = StardewValley.Object;

namespace Swim
{
    public class SwimMaps
    {
        private static IMonitor SMonitor;
        private static ModConfig Config;
        private static IModHelper SHelper;

        public static void Initialize(IMonitor monitor, IModHelper helper, ModConfig config)
        {
            SMonitor = monitor;
            Config = config;
            SHelper = helper;
        }

        public static Object SpawnForageItem(GameLocation location, Vector2 position, string itemID)
        {
            Object item = ItemRegistry.Create<Object>(itemID.StartsWith("(") ? itemID : "(O)" + itemID);
            SMonitor.Log($"Spawning forage {item.Name} at ({position.X}, {position.Y})");
            location.numberOfSpawnedObjectsOnMap++;
            location.overlayObjects[position] = item;
            item.IsSpawnedObject = true;
            item.CanBeGrabbed = true;
            return item;
        }

        public static Object SpawnWorldItem(GameLocation location, Vector2 position, string itemID)
        {
            Object item = ItemRegistry.Create<Object>(itemID.StartsWith("(") ? itemID : "(O)" + itemID);
            SMonitor.Log($"Spawning world item {item.Name} at ({position.X}, {position.Y})");
            location.overlayObjects[position] = item;
            return item;
        }

        public static void AddScubaChest(GameLocation gameLocation, Vector2 pos, string which)
        {
            if (which == "ScubaTank" && !Game1.player.mailReceived.Contains(which))
            {
                gameLocation.overlayObjects[pos] = new Chest( new List<Item>() { new Clothing(ModEntry.scubaTankID.Value) }, pos, false, 0);
            }
            else if (which == "ScubaMask" && !Game1.player.mailReceived.Contains(which))
            {
                gameLocation.overlayObjects[pos] = new Chest( new List<Item>() { new Hat(ModEntry.scubaMaskID.Value )}, pos, false, 0);
            }
            else if (which == "ScubaFins" && !Game1.player.mailReceived.Contains(which))
            {
                gameLocation.overlayObjects[pos] = new Chest( new List<Item>() { new Boots(ModEntry.scubaFinsID.Value) }, pos, false, 0);
            }
        }
        public static void AddWaterTiles(GameLocation gameLocation)
        {
            gameLocation.waterTiles = new WaterTiles(new bool[gameLocation.map.Layers[0].LayerWidth, gameLocation.map.Layers[0].LayerHeight]);
            bool foundAnyWater = false;
            for (int x = 0; x < gameLocation.map.Layers[0].LayerWidth; x++)
            {
                for (int y = 0; y < gameLocation.map.Layers[0].LayerHeight; y++)
                {
                    if (gameLocation.doesTileHaveProperty(x, y, "Water", "Back") != null)
                    {
                        foundAnyWater = true;
                        gameLocation.waterTiles[x, y] = true;
                    }
                }
            }
            if (!foundAnyWater)
            {
                SMonitor.Log($"{Game1.player.currentLocation.Name} has no water tiles");
                gameLocation.waterTiles = null;
            }
            else
            {
                SMonitor.Log($"Gave {Game1.player.currentLocation.Name} water tiles");
            }
        }


        public static void AddMinerals(GameLocation l)
        {
            List<Vector2> spots = new List<Vector2>();
            for (int x = 0; x < l.map.Layers[0].LayerWidth; x++)
            {
                for (int y = 0; y < l.map.Layers[0].LayerHeight; y++)
                {
                    Tile tile = l.map.GetLayer("Back").PickTile(new Location(x, y) * Game1.tileSize, Game1.viewport.Size);
                    if (tile != null && l.map.GetLayer("Buildings").PickTile(new Location(x, y) * Game1.tileSize, Game1.viewport.Size) == null && l.map.GetLayer("Front").PickTile(new Location(x, y) * Game1.tileSize, Game1.viewport.Size) == null)
                    {
                        spots.Add(new Vector2(x, y));
                    }
                }
            }
            int n = spots.Count;
            while (n > 1)
            {
                n--;
                int k = Game1.random.Next(n + 1);
                var value = spots[k];
                spots[k] = spots[n];
                spots[n] = value;
            }

            int mineralNo = (int)Math.Round(Game1.random.Next(Config.MineralPerThousandMin, Config.MineralPerThousandMax) / 1000f * spots.Count);
            List<Vector2> mineralSpots = spots.Take(mineralNo).ToList();

            foreach (Vector2 tile in mineralSpots)
            {
                double chance = Game1.random.NextDouble();
                if (chance < 0.2 && !l.map.GetLayer("Back").Tiles[(int)tile.X, (int)tile.Y].Properties.ContainsKey("Treasure") && !l.map.GetLayer("Back").Tiles[(int)tile.X, (int)tile.Y].Properties.ContainsKey("Diggable"))
                {
                    l.map.GetLayer("Back").Tiles[(int)tile.X, (int)tile.Y].TileIndex = 1299;
                    l.map.GetLayer("Back").Tiles[(int)tile.X, (int)tile.Y].Properties.Add("Treasure", new PropertyValue("Object " + SwimUtils.CheckForBuriedItem(Game1.player)));
                    l.map.GetLayer("Back").Tiles[(int)tile.X, (int)tile.Y].Properties.Add("Diggable", new PropertyValue("T"));
                }
                else if (chance < 0.4)
                {
                    SpawnWorldItem(l, tile, "(O)751").MinutesUntilReady = 2; // Copper stone
                }
                else if (chance < 0.5)
                {
                    SpawnWorldItem(l, tile, "(O)290").MinutesUntilReady = 4; // Iron stone
                }
                else if (chance < 0.55)
                {
                    SpawnWorldItem(l, tile, "(O)764").MinutesUntilReady = 8; // Gold stone
                }
                else if (chance < 0.56)
                {
                    SpawnWorldItem(l, tile, "(O)765").MinutesUntilReady = 16; // Iridium stone
                }
                else if (chance < 0.65)
                {
                    SpawnForageItem(l, tile, "(O)80"); // Quartz 
                }
                else if (chance < 0.74)
                {
                    SpawnForageItem(l, tile, "(O)82"); // Fire Quartz
                }
                else if (chance < 0.83)
                {
                    SpawnForageItem(l, tile, "(O)84"); // Frozen Tear
                }
                else if (chance < 0.90)
                {
                    SpawnForageItem(l, tile, "(O)86"); // Earth crystal
                }
                else
                {
                    string[] gems = { "4","6","8","10","12","14","40" };
                    string whichGem = gems[Game1.random.Next(gems.Length)];
                    SpawnWorldItem(l, tile, whichGem).MinutesUntilReady = 5;
                }
            }
        }

        public static void AddCrabs(GameLocation l)
        {
            if (Config.AddCrabs)
            {
                List<Vector2> spots = new List<Vector2>();
                for (int x = 0; x < l.map.Layers[0].LayerWidth; x++)
                {
                    for (int y = 0; y < l.map.Layers[0].LayerHeight; y++)
                    {
                        Tile tile = l.map.GetLayer("Back").PickTile(new Location(x, y) * Game1.tileSize, Game1.viewport.Size);
                        if (tile != null && l.map.GetLayer("Buildings").PickTile(new Location(x, y) * Game1.tileSize, Game1.viewport.Size) == null && l.map.GetLayer("Front").PickTile(new Location(x, y) * Game1.tileSize, Game1.viewport.Size) == null && !l.overlayObjects.ContainsKey(new Vector2(x, y)))
                        {
                            spots.Add(new Vector2(x, y));
                        }
                    }
                }
                int n = spots.Count;
                while (n > 1)
                {
                    n--;
                    int k = Game1.random.Next(n + 1);
                    var value = spots[k];
                    spots[k] = spots[n];
                    spots[n] = value;
                }
                int crabs = (int)(Game1.random.Next(Config.CrabsPerThousandMin, Config.CrabsPerThousandMax) / 1000f * spots.Count);
                for (int i = 0; i < crabs; i++)
                {
                    int idx = Game1.random.Next(spots.Count);
                    l.characters.Add(new SeaCrab(new Vector2(spots[idx].X * Game1.tileSize, spots[idx].Y * Game1.tileSize)));
                }
            }
        }

        public static void AddFishies(GameLocation l, bool smol = true)
        {
            if (Config.AddFishies)
            {
                List<Vector2> spots = new List<Vector2>();
                for (int x = 0; x < l.map.Layers[0].LayerWidth; x++)
                {
                    for (int y = 0; y < l.map.Layers[0].LayerHeight; y++)
                    {
                        Tile tile = l.map.GetLayer("Back").PickTile(new Location(x, y) * Game1.tileSize, Game1.viewport.Size);
                        if (tile != null && l.map.GetLayer("Buildings").PickTile(new Location(x, y) * Game1.tileSize, Game1.viewport.Size) == null && l.map.GetLayer("Front").PickTile(new Location(x, y) * Game1.tileSize, Game1.viewport.Size) == null && !l.overlayObjects.ContainsKey(new Vector2(x, y)))
                        {
                            spots.Add(new Vector2(x, y));
                        }
                    }
                }
                if(spots.Count == 0)
                {
                    SMonitor.Log($"No spots for fishies in map {l.Name}", LogLevel.Warn);
                    return;
                }
                int n = spots.Count;
                while (n > 1)
                {
                    n--;
                    int k = Game1.random.Next(n + 1);
                    var value = spots[k];
                    spots[k] = spots[n];
                    spots[n] = value;
                }
                if (smol)
                {
                    int fishes = Game1.random.Next(Config.MinSmolFishies, Config.MaxSmolFishies);
                    for (int i = 0; i < fishes; i++)
                    {
                        int idx = Game1.random.Next(spots.Count);
                        l.characters.Add(new Fishie(new Vector2(spots[idx].X * Game1.tileSize, spots[idx].Y * Game1.tileSize)));
                    }
                }
                else
                {
                    int bigFishes = (int)(Game1.random.Next(Config.BigFishiesPerThousandMin, Config.BigFishiesPerThousandMax) / 1000f * spots.Count);
                    for (int i = 0; i < bigFishes; i++)
                    {
                        int idx = Game1.random.Next(spots.Count);
                        l.characters.Add(new BigFishie(new Vector2(spots[idx].X * Game1.tileSize, spots[idx].Y * Game1.tileSize)));
                    }
                }
            }
        }
        public static void AddOceanForage(GameLocation l)
        {
            List<Vector2> spots = new List<Vector2>();
            for (int x = 0; x < l.map.Layers[0].LayerWidth; x++)
            {
                for (int y = 0; y < l.map.Layers[0].LayerHeight; y++)
                {
                    Tile tile = l.map.GetLayer("Back").PickTile(new Location(x, y) * Game1.tileSize, Game1.viewport.Size);
                    if (tile != null && l.map.GetLayer("Buildings").PickTile(new Location(x, y) * Game1.tileSize, Game1.viewport.Size) == null && l.map.GetLayer("Front").PickTile(new Location(x, y) * Game1.tileSize, Game1.viewport.Size) == null && !l.overlayObjects.ContainsKey(new Vector2(x, y)))
                    {
                        spots.Add(new Vector2(x, y));
                    }
                }
            }
            int n = spots.Count;
            while (n > 1)
            {
                n--;
                int k = Game1.random.Next(n + 1);
                var value = spots[k];
                spots[k] = spots[n];
                spots[n] = value;
            }
            int forageNo = (int)(Game1.random.Next(Config.OceanForagePerThousandMin, Config.OceanForagePerThousandMax) / 1000f * spots.Count);
            List<Vector2> forageSpots = spots.Take(forageNo).ToList();

            foreach (Vector2 v in forageSpots)
            {
                double chance = Game1.random.NextDouble();
                if (chance < 0.25)
                {
                    SpawnForageItem(l, v, "(O)152");// Seaweed
                }
                else if (chance < 0.4)
                {
                    SpawnForageItem(l, v, "(O)153");// Green Algae
                }
                else if (chance < 0.6)
                {
                    SpawnForageItem(l, v, "(O)157");// White Algae
                }
                else if (chance < 0.75)
                {
                    SpawnForageItem(l, v, "(O)372");// Clam
                }
                else if (chance < 0.85)
                {
                    SpawnForageItem(l, v, "(O)393");// Coral
                }
                else if (chance < 0.94)
                {
                    SpawnForageItem(l, v, "(O)397");// Sea Urchin
                }
                else if (chance < 0.97)
                {
                    SpawnForageItem(l, v, "(O)394");// Rainbow Shell
                }
                else
                {
                    SpawnForageItem(l, v, "(O)392");// Nautilus Shell
                }
            }
        }
        public static void AddOceanTreasure(GameLocation l)
        {
            List<Vector2> spots = new List<Vector2>();
            for (int x = 0; x < l.map.Layers[0].LayerWidth; x++)
            {
                for (int y = 0; y < l.map.Layers[0].LayerHeight; y++)
                {
                    Tile tile = l.map.GetLayer("Back").PickTile(new Location(x, y) * Game1.tileSize, Game1.viewport.Size);
                    if (tile != null && l.map.GetLayer("Buildings").PickTile(new Location(x, y) * Game1.tileSize, Game1.viewport.Size) == null && l.map.GetLayer("Front").PickTile(new Location(x, y) * Game1.tileSize, Game1.viewport.Size) == null && !l.overlayObjects.ContainsKey(new Vector2(x, y)))
                    {
                        spots.Add(new Vector2(x, y));
                    }
                }
            }
            int n = spots.Count;
            while (n > 1)
            {
                n--;
                int k = Game1.random.Next(n + 1);
                var value = spots[k];
                spots[k] = spots[n];
                spots[n] = value;
            }

            int treasureNo = (int)(Game1.random.Next(Config.MinOceanChests, Config.MaxOceanChests));

            List<Vector2> treasureSpots = new List<Vector2>(spots).Take(treasureNo).ToList();

            foreach (Vector2 v in treasureSpots)
            {

                List<Item> treasures = new List<Item>();
                float chance = 1f;
                while (Game1.random.NextDouble() <= (double)chance)
                {
                    chance *= 0.4f;
                    if (Game1.random.NextDouble() < 0.5)
                    {
                        treasures.Add(new Object("774", 2 + ((Game1.random.NextDouble() < 0.25) ? 2 : 0), false, -1, 0));
                    }
                    switch (Game1.random.Next(4))
                    {
                        case 0:
                            if (Game1.random.NextDouble() < 0.03)
                            {
                                treasures.Add(new Object("386", Game1.random.Next(1, 3), false, -1, 0));
                            }
                            else
                            {
                                List<string> possibles = new List<string>();
                                possibles.Add("384");
                                if (possibles.Count == 0 || Game1.random.NextDouble() < 0.6)
                                {
                                    possibles.Add("380");
                                }
                                if (possibles.Count == 0 || Game1.random.NextDouble() < 0.6)
                                {
                                    possibles.Add("378");
                                }
                                if (possibles.Count == 0 || Game1.random.NextDouble() < 0.6)
                                {
                                    possibles.Add("388");
                                }
                                if (possibles.Count == 0 || Game1.random.NextDouble() < 0.6)
                                {
                                    possibles.Add("390");
                                }
                                possibles.Add("382");
                                treasures.Add(new Object(possibles.ElementAt(Game1.random.Next(possibles.Count)), Game1.random.Next(2, 7) * ((Game1.random.NextDouble() < 0.05 + (double)Game1.player.luckLevel.Value * 0.015) ? 2 : 1), false, -1, 0));
                                if (Game1.random.NextDouble() < 0.05 + (double)Game1.player.LuckLevel * 0.03)
                                {
                                    treasures.Last().Stack *= 2;
                                }
                            }
                            break;
                        case 1:
                            if (Game1.random.NextDouble() < 0.1)
                            {
                                treasures.Add(new Object("687", 1, false, -1, 0));
                            }
                            else if (Game1.random.NextDouble() < 0.25 && Game1.player.craftingRecipes.ContainsKey("Wild Bait"))
                            {
                                treasures.Add(new Object("774", 5 + ((Game1.random.NextDouble() < 0.25) ? 5 : 0), false, -1, 0));
                            }
                            else
                            {
                                treasures.Add(new Object("685", 10, false, -1, 0));
                            }
                            break;
                        case 2:
                            if (Game1.random.NextDouble() < 0.1 && Game1.netWorldState.Value.LostBooksFound < 21 && Game1.player.hasOrWillReceiveMail("lostBookFound"))
                            {
                                treasures.Add(new Object("102", 1, false, -1, 0));
                            }
                            else if (Game1.player.archaeologyFound.Count() > 0)
                            {
                                if (Game1.random.NextDouble() < 0.125)
                                {
                                    treasures.Add(new Object("585", 1, false, -1, 0));
                                }
                                else if (Game1.random.NextDouble() < 0.25)
                                {
                                    treasures.Add(new Object("588", 1, false, -1, 0));
                                }
                                else if (Game1.random.NextDouble() < 0.5)
                                {
                                    treasures.Add(new Object("103", 1, false, -1, 0));
                                }
                                if (Game1.random.NextDouble() < 0.5)
                                {
                                    treasures.Add(new Object("120", 1, false, -1, 0));
                                }

                                else
                                {
                                    treasures.Add(new Object("535", 1, false, -1, 0));
                                }
                            }
                            else
                            {
                                treasures.Add(new Object("382", Game1.random.Next(1, 3), false, -1, 0));
                            }
                            break;
                        case 3:
                            switch (Game1.random.Next(3))
                            {
                                case 0:
                                    switch (Game1.random.Next(3))
                                    {
                                        case 0:
                                            treasures.Add(new Object((537 + ((Game1.random.NextDouble() < 0.4) ? Game1.random.Next(-2, 0) : 0)).ToString(), Game1.random.Next(1, 4), false, -1, 0));
                                            break;
                                        case 1:
                                            treasures.Add(new Object((536 + ((Game1.random.NextDouble() < 0.4) ? -1 : 0)).ToString(), Game1.random.Next(1, 4), false, -1, 0));
                                            break;
                                        case 2:
                                            treasures.Add(new Object("535", Game1.random.Next(1, 4), false, -1, 0));
                                            break;
                                    }
                                    if (Game1.random.NextDouble() < 0.05 + (double)Game1.player.LuckLevel * 0.03)
                                    {
                                        treasures.Last().Stack *= 2;
                                    }
                                    break;
                                case 1:
                                    switch (Game1.random.Next(4))
                                    {
                                        case 0:
                                            treasures.Add(new Object("382", Game1.random.Next(1, 4), false, -1, 0));
                                            break;
                                        case 1:
                                            treasures.Add(new Object(((Game1.random.NextDouble() < 0.3) ? 82 : ((Game1.random.NextDouble() < 0.5) ? 64 : 60)).ToString(), Game1.random.Next(1, 3), false, -1, 0));
                                            break;
                                        case 2:
                                            treasures.Add(new Object(((Game1.random.NextDouble() < 0.3) ? 84 : ((Game1.random.NextDouble() < 0.5) ? 70 : 62)).ToString(), Game1.random.Next(1, 3), false, -1, 0));
                                            break;
                                        case 3:
                                            treasures.Add(new Object(((Game1.random.NextDouble() < 0.3) ? 86 : ((Game1.random.NextDouble() < 0.5) ? 66 : 68)).ToString(), Game1.random.Next(1, 3), false, -1, 0));
                                            break;
                                    }
                                    if (Game1.random.NextDouble() < 0.05)
                                    {
                                        treasures.Add(new Object("72", 1, false, -1, 0));
                                    }
                                    if (Game1.random.NextDouble() < 0.05)
                                    {
                                        treasures.Last().Stack *= 2;
                                    }
                                    break;
                                case 2:
                                    if (Game1.player.FishingLevel < 2)
                                    {
                                        treasures.Add(new Object("770", Game1.random.Next(1, 4), false, -1, 0));
                                    }
                                    else
                                    {
                                        float luckModifier = (1f + (float)Game1.player.DailyLuck);
                                        if (Game1.random.NextDouble() < 0.05 * (double)luckModifier && !Game1.player.specialItems.Contains("14"))
                                        {
                                            treasures.Add(new MeleeWeapon("14")
                                            {
                                                specialItem = true
                                            });
                                        }
                                        if (Game1.random.NextDouble() < 0.05 * (double)luckModifier && !Game1.player.specialItems.Contains("51"))
                                        {
                                            treasures.Add(new MeleeWeapon("51")
                                            {
                                                specialItem = true
                                            });
                                        }
                                        if (Game1.random.NextDouble() < 0.07 * (double)luckModifier)
                                        {
                                            switch (Game1.random.Next(3))
                                            {
                                                case 0:
                                                    treasures.Add(new Ring("516" + ((Game1.random.NextDouble() < (double)((float)Game1.player.LuckLevel / 11f)) ? 1 : 0)));
                                                    break;
                                                case 1:
                                                    treasures.Add(new Ring("518" + ((Game1.random.NextDouble() < (double)((float)Game1.player.LuckLevel / 11f)) ? 1 : 0)));
                                                    break;
                                                case 2:
                                                    treasures.Add(new Ring(""+Game1.random.Next(529, 535)));
                                                    break;
                                            }
                                        }
                                        if (Game1.random.NextDouble() < 0.02 * (double)luckModifier)
                                        {
                                            treasures.Add(new Object("166", 1, false, -1, 0));
                                        }
                                        if (Game1.random.NextDouble() < 0.001 * (double)luckModifier)
                                        {
                                            treasures.Add(new Object("74", 1, false, -1, 0));
                                        }
                                        if (Game1.random.NextDouble() < 0.01 * (double)luckModifier)
                                        {
                                            treasures.Add(new Object("127", 1, false, -1, 0));
                                        }
                                        if (Game1.random.NextDouble() < 0.01 * (double)luckModifier)
                                        {
                                            treasures.Add(new Object("126", 1, false, -1, 0));
                                        }
                                        if (Game1.random.NextDouble() < 0.01 * (double)luckModifier)
                                        {
                                            treasures.Add(new Ring("527"));
                                        }
                                        if (Game1.random.NextDouble() < 0.01 * (double)luckModifier)
                                        {
                                            treasures.Add(new Boots("" + Game1.random.Next(504, 514)));
                                        }
                                        if (treasures.Count == 1)
                                        {
                                            treasures.Add(new Object("72", 1, false, -1, 0));
                                        }
                                    }
                                    break;
                            }
                            break;
                    }
                }
                if (treasures.Count == 0)
                {
                    treasures.Add(new Object("685", Game1.random.Next(1, 4) * 5, false, -1, 0));
                }
                if (treasures.Count > 0)
                {
                    Color tint = Color.White;
                    l.overlayObjects[v] = new Chest( new List<Item>() { treasures[ModEntry.myRand.Value.Next(treasures.Count)] }, v, false, 0)
                    {
                        Tint = tint
                    };
                }
                foreach (var obj in treasures)
                {
                    SMonitor.Log($"Treasures: {obj.QualifiedItemId} {obj.DisplayName}");
                }
            }
        }


        public static void RemoveWaterTiles(GameLocation l)
        {
            if (l == null || l.map == null)
                return;
            Map map = l.map;
            string mapName = l.Name;
            for (int x = 0; x < map.Layers[0].LayerWidth; x++)
            {
                for (int y = 0; y < map.Layers[0].LayerHeight; y++)
                {
                    if (SwimUtils.doesTileHaveProperty(map, x, y, "Water", "Back") != null)
                    {
                        Tile tile = map.GetLayer("Back").PickTile(new Location(x, y) * Game1.tileSize, Game1.viewport.Size);
                        if (tile != null)
                            tile.TileIndexProperties.Remove("Water");
                    }
                }
            }
        }


        public static void SwitchToWaterTiles(GameLocation location)
        {

            string mapName = location.Name;

            Map map = location.Map;
            for (int x = 0; x < map.Layers[0].LayerWidth; x++)
            {
                for (int y = 0; y < map.Layers[0].LayerHeight; y++)
                {
                    if (SwimUtils.doesTileHaveProperty(map, x, y, "Water", "Back") != null)
                    {
                        Tile tile = map.GetLayer("Back").PickTile(new Location(x, y) * Game1.tileSize, Game1.viewport.Size);
                        if (tile != null)
                        {
                            if (tile.TileIndexProperties.ContainsKey("Passable"))
                            {
                                tile.TileIndexProperties["Passable"] = "T";
                            }
                        }
                        tile = map.GetLayer("Buildings").PickTile(new Location(x, y) * Game1.tileSize, Game1.viewport.Size);
                        if (tile != null)
                        {
                            if (tile.TileIndexProperties.ContainsKey("Passable"))
                            {
                                tile.TileIndexProperties["Passable"] = "T";
                            }
                            else
                            {
                                tile.TileIndexProperties.Add("Passable", "T");
                            }
                        }
                    }
                }
            }
        }
        public static void SwitchToLandTiles(GameLocation location)
        {
            string mapName = location.Name;

            Map map = location.Map;
            for (int x = 0; x < map.Layers[0].LayerWidth; x++)
            {
                for (int y = 0; y < map.Layers[0].LayerHeight; y++)
                {
                    if (SwimUtils.doesTileHaveProperty(map, x, y, "Water", "Back") != null)
                    {
                        Tile tile = map.GetLayer("Back").PickTile(new Location(x, y) * Game1.tileSize, Game1.viewport.Size);
                        if (tile != null)
                        {
                            if (tile.TileIndexProperties.ContainsKey("Passable"))
                            {
                                tile.TileIndexProperties["Passable"] = "F";
                            }
                        }
                        tile = map.GetLayer("Buildings").PickTile(new Location(x, y) * Game1.tileSize, Game1.viewport.Size);
                        if (tile != null)
                        {
                            if (tile.TileIndexProperties.ContainsKey("Passable"))
                            {
                                tile.TileIndexProperties["Passable"] = "F";
                            }
                            else
                            {
                                tile.TileIndexProperties.Add("Passable", "F");
                            }
                        }
                    }
                }
            }
        }
    }
}
