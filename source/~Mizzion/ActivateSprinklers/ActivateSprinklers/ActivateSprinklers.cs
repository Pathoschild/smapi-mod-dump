using System;
using System.Collections.Generic;
using SFarmer = StardewValley.Farmer;
using SObject = StardewValley.Object;

namespace ActivateSprinklers
{
    public class ActivateSprinklers : Mod
    {
        //Bool to see if better sprinklers is loaded
        private bool BetterSprinklersLoaded;
        //Bool to see if Simple Sprinkler is installed.
        private bool SimpleSprinklerLoaded;
        //Set up lists to contain the sprinker coords for Better Sprinklers and/or simple sprinklers
        private List<Vector2> sprinkler;
        private List<Vector2> quality_sprinkler;
        private List<Vector2> iridium_sprinkler;

        public override void Entry(IModHelper helper)
        {
            BetterSprinklersLoaded = helper.ModRegistry.IsLoaded("Speeder.BetterSprinklers");
            SimpleSprinklerLoaded = helper.ModRegistry.IsLoaded("tZed.SimpleSprinkler");
            GameEvents.UpdateTick += UpdateTick;
        }

        //Private Voids
        private void UpdateTick(object sender, EventArgs e)
        {
            if (Game1.currentLocation == null || !Context.IsWorldReady)
                return;
            MouseState currentMouseState = Mouse.GetState();
            KeyboardState currentKeyboardState = Keyboard.GetState();
            ActivateSprinklerz(currentKeyboardState, currentMouseState);
        }
        private void ActivateSprinklerz(KeyboardState keyboardState, MouseState mouseState)
        {
            if (Game1.currentLocation == null || !Context.IsWorldReady)
                return;
            Vector2 currentTile = Game1.currentCursorTile;
            SObject @object;
            WateringCan waterCan = new WateringCan();
            if (mouseState.RightButton == ButtonState.Pressed)
                if (Game1.currentLocation.objects.TryGetValue(currentTile, out @object))
                {
                    if (@object.name.ToLower().Contains("sprinkler"))
                    {
                        waterCan.WaterLeft = 100;
                        float stamina = Game1.player.Stamina;
                        Vector2 sprinklerLocation = @object.tileLocation;
                        List<Vector2> tileNeedWater = new List<Vector2>();
                        bool setup = false;
                        //Cycle through the custom sprinklers. Then go default if nothing else is found.
                        if (BetterSprinklersLoaded)
                        {
                            DoBetterSprinklers();//Gathers Better Sprinklers tiles that are set up.                                                        
                            if(@object.parentSheetIndex == 599)//Regular Sprinkler
                            {
                                foreach (Vector2 water in sprinkler)
                                {
                                    int x = (int)sprinklerLocation.X;
                                    int y = (int)sprinklerLocation.Y;
                                    x += (int)water.X;
                                    y += (int)water.Y;
                                    waterCan.DoFunction(Game1.currentLocation, x * Game1.tileSize, y * Game1.tileSize, 1, Game1.player);
                                    waterCan.WaterLeft++;
                                    Game1.player.Stamina = stamina;
                                }
                            }
                            else if(@object.parentSheetIndex == 621)//Quality Sprinkler
                            {
                                foreach (Vector2 water in quality_sprinkler)
                                {
                                    int x = (int)sprinklerLocation.X;
                                    int y = (int)sprinklerLocation.Y;
                                    x += (int)water.X;
                                    y += (int)water.Y;
                                    waterCan.DoFunction(Game1.currentLocation, x * Game1.tileSize, y * Game1.tileSize, 1, Game1.player);
                                    waterCan.WaterLeft++;
                                    Game1.player.Stamina = stamina;
                                }
                            }
                            else if(@object.parentSheetIndex == 645)//Iridium Sprinkler
                            {
                                foreach (Vector2 water in iridium_sprinkler)
                                {
                                    int x = (int)sprinklerLocation.X;
                                    int y = (int)sprinklerLocation.Y;
                                    x += (int)water.X;
                                    y += (int)water.Y;
                                    waterCan.DoFunction(Game1.currentLocation, x * Game1.tileSize, y * Game1.tileSize, 1, Game1.player);
                                    waterCan.WaterLeft++;
                                    Game1.player.Stamina = stamina;
                                }
                            }
                        }
                        else if (SimpleSprinklerLoaded)
                        {
                            DoSimpleSprinklers();
                            if (@object.parentSheetIndex == 599)//Regular Sprinkler
                            {
                                foreach (Vector2 water in sprinkler)
                                {
                                    int x = (int)sprinklerLocation.X;
                                    int y = (int)sprinklerLocation.Y;
                                    x += (int)water.X;
                                    y += (int)water.Y;
                                    waterCan.DoFunction(Game1.currentLocation, x * Game1.tileSize, y * Game1.tileSize, 1, Game1.player);
                                    waterCan.WaterLeft++;
                                    Game1.player.Stamina = stamina;
                                }
                            }
                            else if (@object.parentSheetIndex == 621)//Quality Sprinkler
                            {
                                foreach (Vector2 water in quality_sprinkler)
                                {
                                    int x = (int)sprinklerLocation.X;
                                    int y = (int)sprinklerLocation.Y;
                                    x += (int)water.X;
                                    y += (int)water.Y;
                                    waterCan.DoFunction(Game1.currentLocation, x * Game1.tileSize, y * Game1.tileSize, 1, Game1.player);
                                    waterCan.WaterLeft++;
                                    Game1.player.Stamina = stamina;
                                }
                            }
                            else if (@object.parentSheetIndex == 645)//Iridium Sprinkler
                            {
                                foreach (Vector2 water in iridium_sprinkler)
                                {
                                    int x = (int)sprinklerLocation.X;
                                    int y = (int)sprinklerLocation.Y;
                                    x += (int)water.X;
                                    y += (int)water.Y;
                                    waterCan.DoFunction(Game1.currentLocation, x * Game1.tileSize, y * Game1.tileSize, 1, Game1.player);
                                    waterCan.WaterLeft++;
                                    Game1.player.Stamina = stamina;
                                }
                            }
                        }
                        else
                        {
                            if(@object.name.ToLower().Contains("quality") && setup == false)
                            {
                                tileNeedWater = Vector2TileGrid(currentTile, 1);
                                setup = true;
                            }
                            if(@object.name.ToLower().Contains("iridium") && setup == false)
                            {
                                tileNeedWater = Vector2TileGrid(currentTile, 2);
                                setup = true;
                            }
                            if (setup == false)
                            {
                                tileNeedWater.Add(new Vector2(currentTile.X + 1, currentTile.Y));
                                tileNeedWater.Add(new Vector2(currentTile.X - 1, currentTile.Y));
                                tileNeedWater.Add(new Vector2(currentTile.X, currentTile.Y + 1));
                                tileNeedWater.Add(new Vector2(currentTile.X, currentTile.Y - 1));
                                setup = true;
                            }
                            foreach(Vector2 waterTile in tileNeedWater)
                            {
                                waterCan.DoFunction(Game1.currentLocation, (int)(waterTile.X * Game1.tileSize), (int)(waterTile.Y * Game1.tileSize), 1, Game1.player);
                                waterCan.WaterLeft++;
                                Game1.player.stamina = stamina;
                            }
                        }
                    }
                }
        }
        private void DoBetterSprinklers()
        {
            if (BetterSprinklersLoaded)
            {
                //Lets load up the integration from Better Sprinklers
                BetterSprinklersIntegration bsi = new BetterSprinklersIntegration(this.Helper.ModRegistry, this.Monitor);
                int bsi_area = bsi.MaxRadius;
                IDictionary<int, Vector2[]> bsi_tiles = bsi.GetSprinklerTiles();
                sprinkler = new List<Vector2>();
                quality_sprinkler = new List<Vector2>();
                iridium_sprinkler = new List<Vector2>();

                foreach (KeyValuePair<int, Vector2[]> tiles in bsi_tiles)
                {
                    for (int i = 0; i < tiles.Value.Length; i++)
                    {
                        if (tiles.Key == 599)
                        {
                            if (!sprinkler.Contains(tiles.Value[i]))
                                sprinkler.Add(tiles.Value[i]);
                        }
                        if (tiles.Key == 621)
                        {
                            if (!quality_sprinkler.Contains(tiles.Value[i]))
                                quality_sprinkler.Add(tiles.Value[i]);
                        }
                        if (tiles.Key == 645)
                        {
                            if (!iridium_sprinkler.Contains(tiles.Value[i]))
                                iridium_sprinkler.Add(tiles.Value[i]);
                        }
                    }
                }
            }
        }
        private void DoSimpleSprinklers()
        {
            if (SimpleSprinklerLoaded)
            {
                //Lets load up the integration from Simple Sprinklers
                SimpleSprinklerIntegration ssi = new SimpleSprinklerIntegration(this.Helper.ModRegistry, this.Monitor);
                IDictionary<int, Vector2[]> ssi_tiles = ssi.GetNewSprinklerTiles();
                sprinkler = new List<Vector2>();
                quality_sprinkler = new List<Vector2>();
                iridium_sprinkler = new List<Vector2>();

                foreach (KeyValuePair<int, Vector2[]> tiles in ssi_tiles)
                {
                    for (int i = 0; i < tiles.Value.Length; i++)
                    {
                        if (tiles.Key == 599)
                        {
                            if (!sprinkler.Contains(tiles.Value[i]))
                                sprinkler.Add(tiles.Value[i]);
                        }
                        if (tiles.Key == 621)
                        {
                            if (!quality_sprinkler.Contains(tiles.Value[i]))
                                quality_sprinkler.Add(tiles.Value[i]);
                        }
                        if (tiles.Key == 645)
                        {
                            if (!iridium_sprinkler.Contains(tiles.Value[i]))
                                iridium_sprinkler.Add(tiles.Value[i]);
                        }
                    }
                }
            }
        }
        private List<Vector2> Vector2TileGrid(Vector2 origin, int size)
        {
            List<Vector2> grid = new List<Vector2>();
            
            for(int i = 0; i < 2 * size + 1; i++)
            {
                for(int i1 = 0; i1 < 2 * size + 1; i1++)
                {
                    Vector2 newVec = new Vector2(origin.X - size, origin.Y - size);
                    newVec.X += (float)i;
                    newVec.Y += (float)i1;
                    grid.Add(newVec);
                }
            }
            return grid;
        }
    }
}
