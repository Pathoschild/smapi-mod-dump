/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/evfredericksen/StardewSpeak
**
*************************************************/

using Newtonsoft.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json.Linq;
using StardewSpeak.Pathfinder;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using StardewValley.Menus;
using System.Reflection;
using xTile.Layers;

namespace StardewSpeak
{
    class Requests
    {

        public static dynamic HandleRequest(dynamic request)
        {
            dynamic error = null;
            try
            {
                string msgType = request.type;
                dynamic msgData = request.data;
                dynamic body = Requests.HandleRequestMessage(msgType, msgData);
                return new { body, error };
            }
            catch (Exception e)
            {
                string body = e.ToString();
                error = "STACK_TRACE";
                return new { body, error };
            }
        }

        public static dynamic HandleRequestMessage(string msgType, dynamic data = null)
        {
            var player = Game1.player;
            int playerX = player.getTileX();
            int playerY = player.getTileY();
            GameLocation location = player.currentLocation;
            switch (msgType)
            {
                case "HEARTBEAT": // engine will shutdown if heartbeat not received after 10 seconds
                    return null;
                case "REQUEST_BATCH":
                    var body = new List<dynamic>();
                    foreach (dynamic batchedRequest in data)
                    {
                        string batchedMsgType = batchedRequest.type;
                        dynamic batchedMsgData = JsonConvert.DeserializeObject(batchedRequest.data.ToString());
                        body.Add(HandleRequestMessage(batchedMsgType, batchedMsgData));
                    }
                    return body;
                case "PLAYER_STATUS":
                    return GameState.PlayerStatus();
                case "TOOL_STATUS":
                    return GameState.ToolStatus();
                case "CHARACTERS_AT_LOCATION":
                    return GameState.CharactersAtLocation(Game1.currentLocation);
                case "ANIMALS_AT_LOCATION":
                    return GameState.AnimalsAtLocation(Game1.currentLocation);
                case "PLAYER_POSITION":
                    return GameState.PlayerPosition;
                case "PLAYER_ITEMS":
                    return GameState.PlayerItems();
                case "GET_MONSTER_TARGET":
                    {
                        var chars = new List<dynamic>();
                        var monsters = GameState.CharactersAtLocation(Game1.currentLocation).
                            Select(x => x.isMonster).
                            OrderByDescending(x => x.isInvisible);
                        foreach (var monster in monsters)
                        {

                        }
                        return null;
                    }
                case "NEW_STREAM":
                    {
                        string streamId = data.stream_id;
                        string streamName = data.name;
                        object streamData = data.data;
                        var stream = new Stream(streamName, streamId, streamData);
                        // shallow copy as naive but simple way to avoid multithreading issues
                        var newStreams = new Dictionary<string, Stream>(ModEntry.Streams);
                        newStreams.Add(streamId, stream);
                        ModEntry.Streams = newStreams;
                        return true;
                    }
                case "STOP_STREAM":
                    {
                        string streamId = data;
                        // shallow copy as naive but simple way to avoid multithreading issues
                        var newStreams = new Dictionary<string, Stream>(ModEntry.Streams);
                        newStreams.Remove(streamId);
                        ModEntry.Streams = newStreams;
                        return true;
                    }
                case "ROUTE":
                    {
                        GameLocation fromLocation = player.currentLocation;
                        string toLocationStr = data.toLocation;
                        GameLocation toLocation = Game1.getLocationFromName(toLocationStr);
                        //throw new InvalidOperationException($"Missing location {name}")
                        return Routing.GetRoute(fromLocation.NameOrUniqueName, toLocation.NameOrUniqueName);
                    }
                case "ROUTE_INDOORS":
                    {
                        GameLocation fromLocation = player.currentLocation;
                        return true;
                    }
                case "path_to_tile":
                    {
                        int targetX = data.x;
                        int targetY = data.y;
                        int cutoff = data.cutoff;
                        var path = Pathfinder.Pathfinder.FindPath(player.currentLocation, playerX, playerY, targetX, targetY, cutoff);
                        return path;
                    }
                case "PATH_TO_EDGE":
                    {
                        int direction = data.direction;
                        Layer layer = player.currentLocation.map.Layers[0];
                        bool isXAxis = false;
                        int testX = playerX;
                        int testY = 0;
                        int increment = 1;
                        if (direction == 1)
                        {
                            isXAxis = true;
                            testX = layer.LayerWidth - 1;
                            testY = playerY;
                            increment = -1;
                        }
                        else if (direction == 2)
                        {
                            testX = playerX;
                            testY = layer.LayerHeight - 1;
                            increment = -1;
                        }
                        else if (direction == 3)
                        {
                            isXAxis = true;
                            testX = 0;
                            testY = playerY;
                        }
                        dynamic path = null;
                        while (testX != playerX || testY != playerY)
                        {
                            if (Pathfinder.Pathfinder.isTileWalkable(location, testX, testY))
                            {
                                path = Pathfinder.Pathfinder.FindPath(location, testX, testY, playerX, playerY);
                                if (path != null)
                                {
                                    path.Reverse();
                                    break;
                                };
                            }
                            if (isXAxis) testX += increment;
                            else testY += increment;

                        }
                        return path;
                    }
                case "GET_NEAREST_CHARACTER":
                    {
                        string characterType = data.characterType; // animal, npc, monster etc
                        string getBy = data.getBy;
                        bool getPath = data.getPath;
                        string requiredName = data.requiredName;
                        List<List<int>> currentPathTiles = data.pathTiles;
                        dynamic target = JsonConvert.DeserializeObject<dynamic>(data.target.ToString());
                        string targetName = requiredName ?? (target?.name);
                        int? targetTileX = target?.tileX;
                        int? targetTileY = target?.tileY;
                        string targetTrackingId = target?.trackingId;
                        float fromPositionX = target == null ? player.Position.X : target.center[0];
                        float fromPositionY = target == null ? player.Position.Y : target.center[1];
                        var candidates = new List<dynamic>();
                        List<dynamic> sorted;
                        if (characterType == "animal")
                        {
                            if (!(location is IAnimalLocation)) return null;
                            foreach (FarmAnimal farmAnimal in (location as IAnimalLocation).Animals.Values)
                            {
                                var animal = Serialization.SerializeAnimal(farmAnimal);
                                if ((getBy == "unpet" && !animal.wasPet) || (getBy == "readyForHarvest" && animal.readyForHarvest))
                                {
                                    candidates.Add(animal);
                                }
                            }
                            sorted = candidates.OrderByDescending(x => targetTrackingId != null && x.trackingId == targetTrackingId).
                                ThenBy(x => Utils.DistanceBeteenPoints(x.position[0], x.position[1], fromPositionX, fromPositionY)).
                                ToList();
                        }
                        else if (characterType == "npc" || characterType == "monster")
                        {
                            var charList = Game1.CurrentEvent != null ? Game1.CurrentEvent.actors : location.characters.ToList();
                            foreach (var character in charList)
                            {
                                if ((characterType == "monster" && character.IsMonster) || (characterType == "npc" && !character.IsMonster))
                                    candidates.Add(Serialization.SerializeCharacter(character));
                            }
                            sorted = candidates.OrderBy(x => x.isInvisible).
                                ThenByDescending(x => targetTrackingId != null && x.trackingId == targetTrackingId).
                                ThenByDescending(x => targetName != null && x.name == targetName).
                                ThenBy(x => Utils.DistanceBeteenPoints(x.position[0], x.position[1], fromPositionX, fromPositionY)).
                                ToList();
                        }
                        else
                        {
                            throw new InvalidDataException();
                        }
                        if (requiredName != null) sorted = sorted.Where(x => x.name == requiredName).ToList();
                        if (!getPath) return sorted.Count > 0 ? sorted[0] : null;
                        foreach (var candidate in sorted)
                        {
                            if (candidate.tileX == targetTileX && candidate.tileY == targetTileY) return candidate;
                            if (Utils.DistanceBeteenPoints(candidate.tileX, candidate.tileY, playerX, playerY) < 2) return candidate;
                            var pathTiles = Pathfinder.Pathfinder.FindPath(player.currentLocation,
                                candidate.tileX, candidate.tileY, playerX, playerY, -1);
                            if (pathTiles != null) return Utils.Merge(candidate, new { pathTiles });
                        }
                        return null;
                    }
                case "PATH_TO_PLAYER":
                    {
                        int fromX = data.x;
                        int fromY = data.y;
                        int cutoff = data.cutoff;
                        var tiles = Pathfinder.Pathfinder.FindPath(player.currentLocation, fromX, fromY, playerX, playerY, cutoff);
                        return new { tiles, location = player.currentLocation.NameOrUniqueName };
                    }
                case "BED_TILE":
                    {
                        if (Game1.player.currentLocation is StardewValley.Locations.FarmHouse)
                        {
                            var fh = Game1.player.currentLocation as StardewValley.Locations.FarmHouse;
                            var bed = fh.getBedSpot();
                            return new { tileX = bed.X, tileY = bed.Y };
                        }
                        return null;
                    }
                case "SHIPPING_BIN_TILE":
                    {
                        if (Game1.player.currentLocation is StardewValley.Farm)
                        {
                            var farm = Game1.player.currentLocation as StardewValley.Farm;
                            var sb = Game1.getFarm().GetStarterShippingBinLocation();
                            return new { tileX = (int)sb.X, tileY = (int)sb.Y, width = 2, height = 1 };
                        }
                        return null;
                    }
                case "LOCATION_CONNECTION":
                    {
                        GameLocation fromLocation = player.currentLocation;
                        string toLocationStr = data.toLocation;
                        GameLocation toLocation = Game1.getLocationFromName(toLocationStr);
                        var locationConnection = Routing.FindLocationConnection(fromLocation, toLocation);
                        return locationConnection;
                    }
                case "GET_LOCATION_CONNECTIONS":
                    {
                        GameLocation fromLocation = player.currentLocation;
                        return Routing.MapConnections[fromLocation.NameOrUniqueName];
                    }
                case "GET_LADDERS_DOWN":
                    {
                        var ladders = new List<dynamic>();
                        xTile.Tiles.TileArray tiles = location.Map.GetLayer("Buildings").Tiles;
                        var buildingsLayer = location.Map.GetLayer("Buildings");
                        for (int y = 0; y < buildingsLayer.LayerHeight; y++)
                        {
                            for (int x = 0; x < buildingsLayer.LayerWidth; x++)
                            {
                                if (buildingsLayer.Tiles[x, y] != null)
                                {
                                    int currentTileIndex = buildingsLayer.Tiles[x, y].TileIndex;
                                    if (currentTileIndex == 173)
                                    {
                                        ladders.Add(new { tileX = x, tileY = y });
                                    }
                                }
                            }
                        }
                        return ladders;
                    }
                case "GET_LOCATION_BUILDINGS":
                    {
                        return GameState.LocationBuildings(location);
                    }
                case "GET_ELEVATOR_TILE":
                    {
                        if (player.currentLocation is StardewValley.Locations.MineShaft)
                        {
                            Vector2 elevator = Utils.GetPrivateField(player.currentLocation, "tileBeneathElevator");
                            return new { tileX = (int)elevator.X, tileY = (int)elevator.Y };
                        }
                        return null;
                    }
                case "SHOW_HUD_MESSAGE":
                    {
                        string message = data.message;
                        int mType = data.msgType;
                        var msg = new HUDMessage(message, mType);
                        if (Context.IsWorldReady)
                        {
                            Game1.addHUDMessage(msg);
                        }
                        else
                        {
                            ModEntry.QueuedMessage = msg;
                        }
                        return true;
                    }
                case "GET_LADDER_UP_TILE":
                    {
                        if (player.currentLocation is StardewValley.Locations.MineShaft)
                        {
                            Vector2 elevator = Utils.GetPrivateField(player.currentLocation, "tileBeneathLadder");
                            return new { tileX = (int)elevator.X, tileY = (int)elevator.Y };
                        }
                        return null;
                    }
                case "PET_ANIMAL_BY_NAME":
                    {
                        string name = data.name;
                        FarmAnimal animal = Utils.FindAnimalByName(name);
                        bool didPet = false;
                        if (animal != null && !animal.wasPet)
                        {
                            animal.pet(player);
                            didPet = true;
                        }
                        return didPet;
                    }
                case "USE_TOOL_ON_ANIMAL_BY_NAME":
                    {
                        string name = data.name;
                        FarmAnimal animal = Utils.FindAnimalByName(name);
                        bool didUseTool = false;
                        if (animal != null && player.CurrentTool?.BaseName == animal.toolUsedForHarvest.Value)
                        {
                            Rectangle rect = animal.GetHarvestBoundingBox();
                            int x = rect.Center.X - Game1.viewport.X;
                            int y = rect.Center.Y - Game1.viewport.Y;

                            Game1.setMousePosition(x, y);
                            Game1.pressUseToolButton();
                            //player.CurrentTool.beginUsing(player.currentLocation, (int)animal.Position.X, (int)animal.Position.Y, player);
                            didUseTool = true;
                        }
                        return didUseTool;
                    }
                case "GET_TERRAIN_FEATURES":
                    {
                        return GameState.TerrainFeatures();
                    }
                case "GET_DEBRIS":
                    {
                        return GameState.Debris();
                    }
                case "GET_HOE_DIRT":
                    {
                        return GameState.HoeDirtTiles();
                    }
                case "GET_LOCATION_OBJECTS":
                    {
                        return GameState.LocationObjects();
                    }
                case "GET_DIGGABLE_TILES":
                    {
                        List<dynamic> testTiles = data.tiles.ToObject<List<dynamic>>();
                        return testTiles.Where(tile => Utils.IsTileHoeable(Game1.player.currentLocation, (int)tile.tileX, (int)tile.tileY));
                    }
                case "GET_WATER_TILES":
                    {
                        WaterTiles.WaterTileData[,] allTiles = Game1.player.currentLocation.waterTiles.waterTiles;
                        var wt = new List<List<int>>();
                        if (allTiles != null)
                        {
                            int width = allTiles.GetLength(0);
                            int height = allTiles.GetLength(1);
                            for (int x = 0; x < width; x++)
                            {
                                for (int y = 0; y < height; y++)
                                {
                                    bool isWaterTile = allTiles[x, y].isWater;
                                    if (isWaterTile)
                                    {
                                        var tile = new List<int> { x, y };
                                        wt.Add(tile);
                                    }
                                }
                            }
                        }
                        return wt;
                    }
                case "GET_ACTIVE_MENU":
                    return Utils.SerializeMenu(Game1.activeClickableMenu);
                case "GET_MOUSE_POSITION":
                    {
                        return new List<int> { Game1.getMouseX(), Game1.getMouseY() };
                    }
                case "TTS_SPEAK":
                    {
                        string text = data.text;
                        return true;
                    }
                case "SET_MOUSE_POSITION":
                    {
                        int x = data.x;
                        int y = data.y;
                        bool fromViewport = data.from_viewport;
                        if (fromViewport)
                        {
                            x -= Game1.viewport.X;
                            y -= Game1.viewport.Y;
                        }
                        Game1.setMousePosition(x, y);
                        return true;
                    }
                case "SET_MOUSE_POSITION_ON_TILE":
                    {
                        int x = data.x * 64 + 32 - Game1.viewport.X;
                        int y = data.y * 64 + 32 - Game1.viewport.Y;
                        Game1.setMousePosition(x, y);
                        return true;
                    }
                case "SET_MOUSE_POSITION_RELATIVE":
                    {
                        int x = data.x;
                        int y = data.y;
                        Game1.setMousePosition(Game1.getMouseX() + x, Game1.getMouseY() + y);
                        return true;
                    }
                case "MOUSE_CLICK":
                    {
                        string btn = data.btn;
                        if (btn == "left") StardewSpeak.Input.LeftClick();
                        else if (btn == "right") StardewSpeak.Input.RightClick();
                        return true;
                    }
                case "UPDATE_HELD_BUTTONS":
                    {
                        List<string> toHold = data.toHold.ToObject<List<string>>();
                        List<string> toRelease = data.toRelease.ToObject<List<string>>();
                        foreach (string key in toRelease)
                        {
                            Input.Release(key);
                        }
                        foreach (string key in toHold)
                        {
                            Input.Hold(key);
                        }
                        return true;
                    }
                case "RELEASE_ALL_KEYS":
                    {
                        Input.ClearHeld();
                        return true;
                    }
                case "PRESS_KEY":
                    {
                        string key = data.key;
                        Input.SetDown(key);
                        return true;
                    }
                case "GET_ALL_GAME_LOCATIONS":
                    {
                        var locs = Routing.AllGameLocations(includeBuildings: false).Select(x => x.NameOrUniqueName).ToList();
                        return locs;
                    }
                case "GET_LATEST_GAME_EVENT":
                    {
                        return Serialization.SerializeGameEvent(Game1.CurrentEvent);
                    }
                case "CATCH_FISH":
                    {
                        var am = Game1.activeClickableMenu;
                        if (am is BobberBar)
                        {
                            var bb = am as BobberBar;
                            var distanceFromCatching = (float)Utils.GetPrivateField(bb, "distanceFromCatching");
                            bool treasure = (bool)Utils.GetPrivateField(bb, "treasure");
                            if (treasure)
                            {
                                Utils.SetPrivateField(bb, "treasureCaught", true);
                            }
                            Utils.SetPrivateField(bb, "distanceFromCatching", distanceFromCatching + 100);

                        }
                        return true;
                    }
                case "GET_RESOURCE_CLUMPS":
                    {
                        return GameState.ResourceClumps();
                    }
            }
            throw new InvalidDataException($"Unhandled request {msgType}");
        }
    }
}
