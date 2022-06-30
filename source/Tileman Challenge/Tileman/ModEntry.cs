/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spicykai/StardewValley-Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Locations;


namespace Tileman
{
    public class ModEntry : Mod
    {



        public bool do_loop = true;
        public bool do_collision = true;
        public bool allow_player_placement = false;
        public bool toggle_overlay = true;
        private bool tool_button_pushed = false;
        private bool location_changed = false;


        public double tile_price = 1.0;
        public double tile_price_raise = 0.0008;
        public double dynamic_tile_price;


        public int caverns_extra = 0;
        public int difficulty_mode = 0;
        public int purchase_count=0;
        public int overlay_mode = 0;

        int tile_count;

        public int amountLocations = 200;
        private int locationDelay = 0;

        private int collisionTick = 0;

        List<KaiTile> tileList = new();
        List<KaiTile> ThisLocationTiles = new();
        Dictionary<string, List<KaiTile>> tileDict = new();

        Texture2D tileTexture  = new(Game1.game1.GraphicsDevice, Game1.tileSize, Game1.tileSize);
        Texture2D tileTexture2 = new(Game1.game1.GraphicsDevice, Game1.tileSize, Game1.tileSize);
        Texture2D tileTexture3 = new(Game1.game1.GraphicsDevice, Game1.tileSize, Game1.tileSize);


       




        public override void Entry(IModHelper helper)
        {
            helper.Events.Input.ButtonReleased += this.OnButtonReleased;
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            helper.Events.Display.RenderedWorld += this.DrawUpdate;


            helper.Events.GameLoop.Saved += this.SaveModData;
            helper.Events.GameLoop.SaveLoaded += this.LoadModData;
            helper.Events.GameLoop.DayStarted += this.DayStartedUpdate;
            helper.Events.GameLoop.ReturnedToTitle += this.TitleReturnUpdate;
      
            tileTexture = helper.ModContent.Load<Texture2D>("assets/tile.png");
            tileTexture2 = helper.ModContent.Load<Texture2D>("assets/tile_2.png");
            tileTexture3 = helper.ModContent.Load<Texture2D>("assets/tile_3.png");



            //CalculateTileSum();

        }

        private void removeSpecificTile(int xTile, int yTile, string gameLocation)
        {

            var tileData = this.Helper.Data.ReadJsonFile<MapData>($"jsons/{Constants.SaveFolderName}/{gameLocation}.json") ?? new MapData();
            var tempList = tileData.AllKaiTilesList;


            for (int i = 0; i < tileData.AllKaiTilesList.Count; i++)
            {
                KaiTile t = tileData.AllKaiTilesList[i];

                if (t.IsSpecifiedTile(xTile, yTile, gameLocation)) {
                    
                    tempList.Remove(t);
                    RemoveProperties(t, Game1.getLocationFromName(gameLocation));

                }

            }
            var mapData = new MapData
            {
                AllKaiTilesList = tempList,
            };



            Helper.Data.WriteJsonFile<MapData>($"jsons/{Constants.SaveFolderName}/{gameLocation}.json", mapData);
            tileList = new();



        }

        private void RemoveProperties(KaiTile tile, GameLocation gameLocation)
        {
            gameLocation.removeTileProperty(tile.tileX, tile.tileY, "Back", "Buildable");
            if (gameLocation.doesTileHavePropertyNoNull(tile.tileX, tile.tileY, "Type", "Back") == "Dirt"
                || gameLocation.doesTileHavePropertyNoNull(tile.tileX, tile.tileY, "Type", "Back") == "Grass") gameLocation.setTileProperty(tile.tileX, tile.tileY, "Back", "Diggable", "true");

            gameLocation.removeTileProperty(tile.tileX, tile.tileY, "Back", "NoFurtniture");
            gameLocation.removeTileProperty(tile.tileX, tile.tileY, "Back", "NoSprinklers");

            gameLocation.removeTileProperty(tile.tileX, tile.tileY, "Back", "Passable");
            gameLocation.removeTileProperty(tile.tileX, tile.tileY, "Back", "Placeable");


            ThisLocationTiles.Remove(tile);
            tileList.Remove(tile);

        }
        public void RemoveTileExceptions()
        {

            this.Monitor.Log("Removing Unusual Tiles", LogLevel.Debug);

            removeSpecificTile(18,27,"Desert");

            removeSpecificTile(12, 9, "BusStop");




        }

        public void AddTileExceptions()
        {

            this.Monitor.Log("Placing Unusual Tiles", LogLevel.Debug);

            var tempName = "Town";

            //ADD UNUSAL TILES HERE
            tileDict[tempName].Add(new KaiTile(21, 42, tempName));
            tileDict[tempName].Add(new KaiTile(21, 43, tempName));
            tileDict[tempName].Add(new KaiTile(21, 44, tempName));
            tileDict[tempName].Add(new KaiTile(21, 45, tempName));
            tileDict[tempName].Add(new KaiTile(21, 46, tempName));

            tileDict[tempName].Add(new KaiTile(49, 42, tempName));
            tileDict[tempName].Add(new KaiTile(49, 43, tempName));
            tileDict[tempName].Add(new KaiTile(49, 44, tempName));
            tileDict[tempName].Add(new KaiTile(49, 45, tempName));
            tileDict[tempName].Add(new KaiTile(49, 46, tempName));
            tileDict[tempName].Add(new KaiTile(49, 47, tempName));
            tileDict[tempName].Add(new KaiTile(49, 48, tempName));
            tileDict[tempName].Add(new KaiTile(49, 49, tempName));
            tileDict[tempName].Add(new KaiTile(49, 50, tempName));
            tileDict[tempName].Add(new KaiTile(49, 51, tempName));

            tileDict[tempName].Add(new KaiTile(55, 102, tempName));
            tileDict[tempName].Add(new KaiTile(55, 103, tempName));
            tileDict[tempName].Add(new KaiTile(55, 104, tempName));
            tileDict[tempName].Add(new KaiTile(55, 105, tempName));
            tileDict[tempName].Add(new KaiTile(55, 106, tempName));
            tileDict[tempName].Add(new KaiTile(55, 107, tempName));





            //Helper.Data.WriteJsonFile<MapData>($"jsons/{Constants.SaveFolderName}/{tempName}.json", mapData);

            //
            //specific tiles to add in /// COPY ABOVE
            //Mountain 3X3 50,6 -> 52,8

           
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            

            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady) return;


            if (!Context.IsPlayerFree) return;

            if (Game1.player.isFakeEventActor) return;
            

            if (e.Button == SButton.G)
            {
                toggle_overlay = !toggle_overlay;
                this.Monitor.Log($"Tileman Overlay set to:{toggle_overlay}", LogLevel.Debug);
                if(toggle_overlay) Game1.playSoundPitched("coin", 1000 );
                if(!toggle_overlay) Game1.playSoundPitched("coin", 600 );

            }
            if (e.Button == SButton.H)
            {
                overlay_mode++;
                var mode = "Mouse";
                if (overlay_mode > 1)
                {
                    mode = "Controller";
                    overlay_mode = 0;
                }

                Monitor.Log($"Tileman Overlay Mode set to:{mode}", LogLevel.Debug);
                Game1.playSoundPitched("coin", 1200);


            }



            if (!toggle_overlay) return;
            

            if (e.Button.IsUseToolButton()) tool_button_pushed = true;


            

            



        }

        private void OnButtonReleased(object sender, ButtonReleasedEventArgs e)
        {
            if (e.Button.IsUseToolButton()) tool_button_pushed = false;




        }

        private void DayStartedUpdate(object sender, DayStartedEventArgs e)
        {

            PlaceInMaps();
            GetLocationTiles(Game1.currentLocation);
            


            //Game1.timeOfDay = 900;
            //Game1.dayOfMonth = 14;
            //Monitor.Log($"TIME OF DAY SET TO:{Game1.timeOfDay}",LogLevel.Debug);







        }

        private void TitleReturnUpdate(object sender, ReturnedToTitleEventArgs e)
        {
            ResetValues();
            
        }

        private void DrawUpdate(object sender, RenderedWorldEventArgs e)
        {
            if (!Context.IsWorldReady) return;



            

            //Makes sure to not draw while a cutscene is happening
            if (Game1.CurrentEvent != null) {
                if (!Game1.CurrentEvent.playerControlSequence)
                {
                    return;

                }
            }

            GroupIfLocationChange();


            for (int i = 0; i < ThisLocationTiles.Count; i++)
            {
                KaiTile t = ThisLocationTiles[i];
                if (t.tileIsWhere == Game1.currentLocation.Name || Game1.currentLocation.Name == "Temp")
                {
                    if (toggle_overlay)
                    {
                        var texture = tileTexture;
                        var stringColor = Color.Gold;
                        //Cursor
                        if (overlay_mode == 1)
                        {
                                
                            if (Game1.currentCursorTile == new Vector2(t.tileX,t.tileY))
                            {
                                texture = tileTexture2;

                                    

                                if (Game1.player.Money < (int)Math.Floor(tile_price))
                                {
                                    stringColor = Color.Red;
                                    texture = tileTexture3;
                                }

                                e.SpriteBatch.DrawString(Game1.dialogueFont, $"${ (int)Math.Floor(tile_price)}",
                                    new Vector2(Game1.getMousePosition().X, Game1.getMousePosition().Y - Game1.tileSize), stringColor);

                            }
                        }
                        //Keyboard or Controller
                        else
                        {
                            if (Game1.player.nextPositionTile().X == t.tileX && Game1.player.nextPositionTile().Y == t.tileY)
                            {
                                texture = tileTexture2;
                                    
                                if (Game1.player.Money < (int)Math.Floor(tile_price))
                                {
                                    texture = tileTexture3;
                                    stringColor = Color.Red;

                                }


                                e.SpriteBatch.DrawString(Game1.dialogueFont, $"${ (int)Math.Floor(tile_price)}",
                                    new Vector2((t.tileX) * 64 - Game1.viewport.X, (t.tileY) * 64 - 64 - Game1.viewport.Y), stringColor);




                            }
                        }
                        t.DrawTile(texture, e.SpriteBatch);
                    }


                    //Prevent player from being pushed out of bounds
                    if (do_collision) PlayerCollisionCheck(t);
                    //Monitor.Log($"{Game1.currentLocation.doesTileHavePropertyNoNull(t.tileX,t.tileY, "Diggable", "back")}", LogLevel.Debug);

                }




            }






                if (tool_button_pushed) PurchaseTilePreCheck();

            



        }


        private static IEnumerable<GameLocation> GetLocations()
        {
            var locations = Game1.locations
                .Concat(
                    from location in Game1.locations.OfType<BuildableGameLocation>()
                    from building in location.buildings
                    where building.indoors.Value != null
                    select building.indoors.Value
                );

            

            return locations;
        }
        


        private bool IsTileAt(int tileX, int tileY, GameLocation TileIsAt)
        {
            foreach (KaiTile t in tileList)
            {
                if (tileX == t.tileX && tileY == t.tileY && TileIsAt == Game1.getLocationFromName(t.tileIsWhere))
                {
                    return true;
                }

            }
            return false;


        }

        private void GetTilePrice()
        {
            switch (difficulty_mode)
            {
                case 0:
                    //Slowly increase tile cost over time // Change 0 for initial buffer
                    if (purchase_count > 0) dynamic_tile_price += tile_price_raise;
                    break;

                case 1:
                    //Increase tile cost through milestones
                    if (purchase_count > 1)      dynamic_tile_price = tile_price * 2;
                    if (purchase_count > 10)     dynamic_tile_price = tile_price * 4;
                    if (purchase_count > 100)    dynamic_tile_price = tile_price * 8;
                    if (purchase_count > 1000)   dynamic_tile_price = tile_price * 16;
                    if (purchase_count > 10000)  dynamic_tile_price = tile_price * 32;
                    if (purchase_count > 100000) dynamic_tile_price = tile_price * 64;

                    break;

                case 2:
                    //Increment tile price with each one purchased
                    dynamic_tile_price = purchase_count;
                    break;
            }
        }

        private void PurchaseTilePreCheck()
        {

            for (int i = 0; i < ThisLocationTiles.Count; i++)
            {

                KaiTile t = ThisLocationTiles[i];

                //Cursor 
                if (overlay_mode == 1)
                {
                    if (Game1.currentCursorTile == new Vector2(t.tileX,t.tileY) )
                    {
                        PurchaseTileCheck(t);
                    }
                }
                //Keyboard or Controller
                else 
                {

                    if (Game1.player.nextPositionTile().X == t.tileX && Game1.player.nextPositionTile().Y == t.tileY)
                    {
                        PurchaseTileCheck(t);
                    }
                }

            }

        }

        private void PurchaseTileCheck(KaiTile thisTile)
        {
            int floor_price = (int)Math.Floor(dynamic_tile_price);

            if (Game1.player.Money >= floor_price)
            {
                
                Game1.player.Money -= floor_price;

                GetTilePrice();
                

                tile_count--;
                purchase_count++;

                Game1.playSoundPitched("purchase", 700 + (100* new Random().Next(0, 7)) );

                

                var gameLocation = Game1.currentLocation;

                gameLocation.removeTileProperty(thisTile.tileX, thisTile.tileY, "Back", "Buildable");
                if (gameLocation.doesTileHavePropertyNoNull(thisTile.tileX, thisTile.tileY, "Type", "Back") == "Dirt"
                        || gameLocation.doesTileHavePropertyNoNull(thisTile.tileX, thisTile.tileY, "Type", "Back") == "Grass") gameLocation.setTileProperty(thisTile.tileX, thisTile.tileY, "Back", "Diggable", "true");

                gameLocation.removeTileProperty(thisTile.tileX, thisTile.tileY, "Back", "NoFurniture");
                gameLocation.removeTileProperty(thisTile.tileX, thisTile.tileY, "Back", "NoSprinklers");

                gameLocation.removeTileProperty(thisTile.tileX, thisTile.tileY, "Back", "Passable");
                gameLocation.removeTileProperty(thisTile.tileX, thisTile.tileY, "Back", "Placeable");

                ThisLocationTiles.Remove(thisTile);
                tileList.Remove(thisTile);

            }
            else Game1.playSoundPitched("grunt", 700 + (100 * new Random().Next(0, 7)));





        }

        private void PlaceInMaps()
        {
            if (Context.IsWorldReady)
            {



                if (do_loop == true)
                {


                    var locationCount = 0;
                    foreach (GameLocation location in GetLocations())
                    {
                        if (!tileDict.ContainsKey(location.Name))
                        {

                            Monitor.Log($"Placing Tiles in: {location.Name}", LogLevel.Debug);

                            locationCount++;

                            if (locationCount < amountLocations)
                            {
                                PlaceTiles(Game1.getLocationFromName(location.NameOrUniqueName));

                            }
                            else
                            {
                                break;
                            }

                            tileDict.Add(location.Name, tileList);
                            tileList = new();
                        }
                    }

                    
                    //Place Tiles in the Mine // Mine 1-120 // Skull Caverns 121-???
                    for (int i = 1; i <= 220 + caverns_extra; i++)

                    {
                        var mineString = Game1.getLocationFromName("UndergroundMine" + i).Name;

                        if (!tileDict.ContainsKey(mineString))
                        {

                            if (Game1.getLocationFromName(mineString) != null)
                            {
                                PlaceTiles(Game1.getLocationFromName(mineString));
                                Monitor.Log($"Placing Tiles in: {mineString}", LogLevel.Debug);

                                tileDict.Add(mineString, tileList);
                                tileList = new();
                            }

                        }
                    }

                    //VolcanoDungeon0 - 9
                    for (int i = 0; i <= 9; i++)

                    {
                        var mineString = Game1.getLocationFromName("VolcanoDungeon" + i).Name;
                        
                        if (!tileDict.ContainsKey(mineString))
                        {

                            if (Game1.getLocationFromName(mineString) != null)
                            {
                                PlaceTiles(Game1.getLocationFromName(mineString));
                                Monitor.Log($"Placing Tiles in: {mineString}", LogLevel.Debug);

                                tileDict.Add(mineString, tileList);
                                tileList = new();
                            }
                        }
                    }

                    AddTileExceptions();
                    RemoveTileExceptions();
                

                    do_loop = false;

                    //Save all the created files
                    foreach (KeyValuePair<string, List<KaiTile>> entry in tileDict)
                    {
                        SaveLocationTiles(Game1.getLocationFromName(entry.Key));
                    }
                    tileDict.Clear();

                    Monitor.Log("Press 'G' to toggle Tileman Overlay", LogLevel.Debug);
                    Monitor.Log("Press 'H' to switch between Overlay Modes", LogLevel.Debug);


                }
            }

        }

        private void PlaceInTempArea(GameLocation gameLocation)
        {
            Monitor.Log($"Placing Tiles in Temporary Area: {Game1.whereIsTodaysFest}", LogLevel.Debug);

            
            PlaceTiles(gameLocation);
            ThisLocationTiles = tileList;
            tileList = new();


        }

        private void PlaceTiles(GameLocation mapLocation)
        {

            int mapWidth = mapLocation.map.Layers[0].LayerWidth;
            int mapHeight = mapLocation.map.Layers[0].LayerHeight;


            for (int i = 1; i < mapWidth - 1; i++)
            {
                for (int j = 1; j < mapHeight - 1; j++)
                {
                    if (/*!IsTileAt(i, j, mapLocation)
                        &&*/ !mapLocation.isObjectAtTile(i, j) 
                        && !mapLocation.isOpenWater(i, j) 
                        && !mapLocation.isTerrainFeatureAt(i, j) 
                        && mapLocation.isTilePlaceable(new Vector2(i, j))
                        && mapLocation.isTileLocationTotallyClearAndPlaceable(new Vector2(i, j))
                        && mapLocation.Map.Layers[0].IsValidTileLocation(i,j) 
                        && mapLocation.isCharacterAtTile(new Vector2(i,j)) == null

                        )
                    {
                        if (new Vector2(Game1.player.position.X, Game1.player.position.Y)!= new Vector2(i, j))
                        {
                            var t = new KaiTile(i, j, mapLocation.Name);
                            tileList.Add(t);

                            tile_count++;
                        }
                    }
                }
            }


            
            

        }


        private void GroupIfLocationChange()

        {
            if (Game1.locationRequest != null)
            {
                if (Game1.locationRequest.Location != Game1.currentLocation && !location_changed)
                {
                    locationDelay = 35;
                    location_changed = true;

                    if (Game1.currentLocation.Name == "Temp") {
                        SaveLocationTiles(Game1.currentLocation);
                    }

                }
                
            }
            else if (location_changed)
            {
                if (locationDelay <= 0)
                {
                    //First encounter with specific Temp area
                    if(Game1.currentLocation.Name == "Temp") 
                    { 
                        if (Helper.Data.ReadJsonFile<MapData>($"jsons/" +
                            $"{Constants.SaveFolderName}/" +
                            $"{Game1.currentLocation.Name + Game1.whereIsTodaysFest}.json") == null)
                        {
                            PlaceInTempArea(Game1.currentLocation);
                        }else
                        {
                            Monitor.Log($"Grouping Tiles At: {Game1.currentLocation.NameOrUniqueName}", LogLevel.Debug);
                            GetLocationTiles(Game1.currentLocation);
 
                        }
                        location_changed = false;
                    }
                    else
                    {

                        Monitor.Log($"Grouping Tiles At: {Game1.currentLocation.NameOrUniqueName}", LogLevel.Debug);
                        GetLocationTiles(Game1.currentLocation);

                        location_changed = false;
                    }
                    
                }

                locationDelay--;

            }


        }
        private void SaveLocationTiles(GameLocation gameLocation)
        {

            var locationName = gameLocation.Name;

            if (locationName == "Temp") locationName += Game1.whereIsTodaysFest;
            Monitor.Log($"Saving in {locationName}", LogLevel.Debug);


            var tileData = Helper.Data.ReadJsonFile<MapData>($"jsons/{Constants.SaveFolderName}/{locationName}.json") ?? new MapData();



            if (gameLocation.Name == "Temp")
            { tileData.AllKaiTilesList = ThisLocationTiles; }
            else
            {
                tileData.AllKaiTilesList = tileDict[locationName];
            }
            Helper.Data.WriteJsonFile<MapData>($"jsons/{Constants.SaveFolderName}/{locationName}.json", tileData);
            
        }
        private void GetLocationTiles(GameLocation gameLocation)
        {
            var locationName = gameLocation.Name;

            if (locationName == "Temp") locationName += Game1.whereIsTodaysFest;

            if (tileDict.ContainsKey(locationName))
            {
                ThisLocationTiles = tileDict[locationName];
            }
            else 
            { 
                var tileData = Helper.Data.ReadJsonFile<MapData>($"jsons/{Constants.SaveFolderName}/{locationName}.json") ?? new MapData();
                if(tileData.AllKaiTilesList.Count > 0) ThisLocationTiles = tileData.AllKaiTilesList;
                if(gameLocation.Name != "Temp")tileDict.Add(locationName, ThisLocationTiles);
            }

            if (gameLocation.Name != "Temp")
            {
                for (int i = 0; i < ThisLocationTiles.Count; i++)
                {
                    var t = ThisLocationTiles[i];

                    if (!allow_player_placement)
                    {
                        gameLocation.removeTileProperty(t.tileX, t.tileY, "Back", "Diggable");

                        gameLocation.setTileProperty(t.tileX, t.tileY, "Back", "Buildable", "false");
                        gameLocation.setTileProperty(t.tileX, t.tileY, "Back", "NoFurniture", "true");
                        gameLocation.setTileProperty(t.tileX, t.tileY, "Back", "NoSprinklers", "");
                        gameLocation.setTileProperty(t.tileX, t.tileY, "Back", "Placeable", "");
                    }
                    //if (do_collision) gameLocation.setTileProperty(t.tileX, t.tileY, "Back", "Passable", "");



                }
            }

        }

       
        private void ResetValues()
        {
            do_loop = true;
            toggle_overlay = true;
            do_collision = true;

            tile_price = 1.0;
            tile_price_raise = 0.20;
            purchase_count = 0;
            tile_count = 0;

            tileList.Clear();
            ThisLocationTiles.Clear();

            tileDict.Clear();




    }



        public int CalculateTileSum(int tileCount = 50000, double price = 1.0, double priceIncrease = 0.0008)
        {
            var totalCost = 0;
            switch (difficulty_mode) {
                case 0:
                                       

                    for (int i = 0; i < tileCount; i++)
                    {
                        totalCost += (int)Math.Floor(price);
                        price += priceIncrease;

                    }
                    break;

                case 1:
                    price = tile_price;


                    for (int i = 0; i < tileCount; i++) 
                    {
                        totalCost += (int)price;
                        if (purchase_count > 10) price = 2.0;
                        if (purchase_count > 100) price = 3.0;
                        if (purchase_count > 1000) price = 4.0;
                        if (purchase_count > 10000) price = 5.0;

                    }

                    break;

                case 2:
                    price = tile_price;


                    for (int i = 0; i < tileCount; i++)
                    {
                        totalCost += (int)price;
                        if (purchase_count > 10) price = 2.0;
                        if (purchase_count > 100) price = 3.0;
                        if (purchase_count > 1000) price = 4.0;
                        if (purchase_count > 10000) price = 5.0;

                    }

                    break;
            }
            this.Monitor.Log($"Cost of {tileCount} tiles by the end: {totalCost}", LogLevel.Debug);

            return totalCost;
        }

        public void BuyAllTilesInLocation(GameLocation gameLocation)
        {
            var tileData = this.Helper.Data.ReadJsonFile<MapData>($"jsons/{Constants.SaveFolderName}/{gameLocation}.json") ?? new MapData();
            tileList = tileData.AllKaiTilesList;

            if (CalculateTileSum(tileList.Count) <= Game1.player.Money)
            {
                for (int i = 0; i < tileList.Count; i++)
                {
                    PurchaseTileCheck(tileList[i]);
                }


                var mapData = new MapData
                {
                    AllKaiTilesList = tileList,
                };



                Helper.Data.WriteJsonFile<MapData>($"jsons/{Constants.SaveFolderName}/{gameLocation}.json", mapData);
                tileList=new();
            }

        }

        private void PlayerCollisionCheck(KaiTile tile)
        {

            if (Game1.getLocationFromName(tile.tileIsWhere) == Game1.currentLocation || Game1.currentLocation.Name == "Temp")
            {
                

                Rectangle tileBox = new(tile.tileX * 64, tile.tileY * 64, tile.tileW, tile.tileH);
                Rectangle playerBox = Game1.player.GetBoundingBox();
                
                if (playerBox.Intersects(tileBox))
                {
                    if (collisionTick > 120)
                    {
                        Game1.player.Money += (int)tile_price;
                        collisionTick = 0;
                        PurchaseTileCheck(tile);

                    }

                    var xDist  = playerBox.Right  - tileBox.Left;
                    var xDist2 = tileBox.Right    - playerBox.Left;
                    var yDist  = playerBox.Bottom - tileBox.Top;
                    var yDist2 = tileBox.Bottom   - playerBox.Top;
                    var xOffset = 0;

                    if (Game1.player.movementDirections.Count > 1
                    && (Game1.player.movementDirections[0] == 3 || Game1.player.movementDirections[1] == 3)) xOffset = 20;

                    if (Math.Abs(xDist - xDist2) >= Math.Abs(yDist - yDist2) + xOffset)
                    {
                        if (xDist >= xDist2)
                        {

                            var newPos = new Vector2(Game1.player.Position.X + xDist2, Game1.player.Position.Y);
                            Game1.player.Position = newPos;

                        }
                        //Collide from Right
                        else
                        {

                            var newPos = new Vector2(Game1.player.Position.X - xDist, Game1.player.Position.Y);
                            Game1.player.Position = newPos;

                        }
                    }
                    else {
                        //Collide from Top
                        if (yDist >= yDist2)
                            {

                                var newPos = new Vector2(Game1.player.Position.X, Game1.player.Position.Y + yDist2);
                                Game1.player.Position = newPos;

                            }
                            //Collide from Bottom
                            else
                            {

                                var newPos = new Vector2(Game1.player.Position.X, Game1.player.Position.Y - yDist);
                                Game1.player.Position = newPos;

                            }
                        }
                                          
                        

                        
                            
                    collisionTick++;

                }
                if (playerBox.Center == tileBox.Center || playerBox.Intersects(tileBox) && locationDelay > 0)
                {
                    if (collisionTick > 120)
                    {
                        Game1.player.Money += (int)tile_price;
                        collisionTick = 0;
                        PurchaseTileCheck(tile);

                    }
                    Game1.player.Position = Game1.player.lastPosition;
                    collisionTick++;


                }
                

                

            }

        }

        

        private void SaveModData(object sender, SavedEventArgs e) {


            foreach (KeyValuePair<string, List<KaiTile>> entry in tileDict)
            { 
                SaveLocationTiles(Game1.getLocationFromName(entry.Key));
            }
            tileDict.Clear();

            var tileData = new ModData
            {
                ToPlaceTiles   = do_loop,
                DoCollision    = do_collision,
                ToggleOverlay  = toggle_overlay,
                TilePrice      = tile_price,
                TilePriceRaise = tile_price_raise,
                CavernsExtra   = caverns_extra,
                DifficultyMode = difficulty_mode,
                PurchaseCount  = purchase_count
            };

            Helper.Data.WriteJsonFile<ModData>($"jsons/{Constants.SaveFolderName}/config.json", tileData);
        }

        private void LoadModData(object sender, SaveLoadedEventArgs e)
        {


            var tileData = Helper.Data.ReadJsonFile<ModData>("config.json") ?? new ModData();
            
            //Load config Information
            if (Helper.Data.ReadJsonFile<ModData>($"jsons/{Constants.SaveFolderName}/config.json") != null)
            {
                tileData = Helper.Data.ReadJsonFile<ModData>($"jsons/{Constants.SaveFolderName}/config.json") ?? new ModData();

            }
            else
            {
                Helper.Data.WriteJsonFile<ModData>($"jsons/{Constants.SaveFolderName}/config.json", tileData);
            }


            do_loop = tileData.ToPlaceTiles;
            toggle_overlay = tileData.ToggleOverlay;
            do_collision = tileData.DoCollision;
            tile_price = tileData.TilePrice;
            tile_price_raise = tileData.TilePriceRaise;
            caverns_extra = tileData.CavernsExtra;
            difficulty_mode = tileData.DifficultyMode;
            purchase_count = tileData.PurchaseCount;

            //
            //Load Individual Location Information

            System.IO.DirectoryInfo root = new($"{Constants.GamePath}/Mods/Tileman/jsons/{Constants.SaveFolderName}");

            System.IO.FileInfo[] files = root.GetFiles();
            /*foreach (System.IO.FileInfo file in files)
            {
                //Note, file.Name has a '.json' at the end, which messes things up with dictionary keys
                var fileName = System.IO.Path.GetFileNameWithoutExtension($"{Constants.GamePath}/Mods/Tileman/jsons/{Constants.SaveFolderName}/{file.Name}");

                if (fileName != "config")
                {
                    
                    var fileData = Helper.Data.ReadJsonFile<MapData>($"jsons/{Constants.SaveFolderName}/{fileName}.json") ?? new MapData();
                    tileDict.Add(fileName, fileData.AllKaiTilesList);

                }

            }*/

            Monitor.Log("Mod Data Loaded", LogLevel.Debug);

        }

        public void createJson(string fileName)
        {
            Monitor.Log($"Creating {fileName}.json", LogLevel.Debug);
            System.IO.File.Create($"jsons/{fileName}.json");
        }






    }


}