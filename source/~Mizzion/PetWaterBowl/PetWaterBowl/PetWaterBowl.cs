using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using SObject = StardewValley.Object;

namespace PetWaterBowl
{
    public class PetWaterBowl : Mod
    {
        private Config _config;

        private bool debugging;
        //Config Settings
        private bool _enableMod;
        private bool _enableSnowWatering;
        private bool _enableSprinklers;
        private Vector2 _waterBowlLocation;

        public override void Entry(IModHelper helper)
        {
            _config = helper.ReadConfig<Config>();
            _enableMod = _config.EnableMod;
            _enableSnowWatering = _config.EnableSnowWatering;
            _enableSprinklers = _config.EnableSprinklerWatering;
            _waterBowlLocation = _config.WaterBowlLocation;
            
            //Whether Im debugging
            debugging = false;

            //Events
            helper.Events.Input.ButtonPressed += KeyPressed;
            helper.Events.GameLoop.SaveLoaded += AfterLoad;
            helper.Events.GameLoop.Saved += AfterSave;
        }

        private void KeyPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;
            //Reload Config file
            if (Helper.Input.IsDown(SButton.F5))
            {
                _config = Helper.ReadConfig<Config>();
                _enableMod = _config.EnableMod;
                _enableSnowWatering = _config.EnableSnowWatering;
                _enableSprinklers = _config.EnableSprinklerWatering;
                _waterBowlLocation = _config.WaterBowlLocation;
                Monitor.Log("Config Reloaded", LogLevel.Info);
            }
            //Grab the Coords of the mouse cursor
            if (Helper.Input.IsDown(SButton.F9))
            {
                ICursorPosition cur = Helper.Input.GetCursorPosition();
                _config.WaterBowlLocation = new Vector2(cur.Tile.X, cur.Tile.Y);
                Helper.WriteConfig(_config);
                //Reload the config after writing to it.
                _config = Helper.ReadConfig<Config>();
                _enableMod = _config.EnableMod;
                _enableSnowWatering = _config.EnableSnowWatering;
                _enableSprinklers = _config.EnableSprinklerWatering;
                _waterBowlLocation = _config.WaterBowlLocation;
                Monitor.Log($"Current Water Bowl Location: X:{cur.Tile.X}, Y:{cur.Tile.Y}. Settings Updated.");
            }
            //For my testing purposes
            if (Helper.Input.IsDown(SButton.NumPad9) && debugging)
            {
                Monitor.Log($"Sprinkler Watering Activated: {_enableSprinklers}");
            }
            //Check for water bowl at location
            if (Helper.Input.IsDown(SButton.NumPad8) && debugging)
            {
                if(CheckBowlLocation(_waterBowlLocation))
                    Monitor.Log("Bowl found.", LogLevel.Alert);
                else
                    Monitor.Log("Bowl not found", LogLevel.Alert);
            }
        }

        private void AfterSave(object sender, SavedEventArgs e)
        {
            if (Game1.whichFarm < 4)
                WaterPetBowl(new Vector2(54, 7));
            else
            {
                if (_waterBowlLocation.X == 54 && _waterBowlLocation.Y == 7)
                    Monitor.Log("It appears, you are using a custom map. In order for this mod to work correctly, you will need to hover your mouse over the water bowl and hit F9. This will set the water bowls coords, then the mod should work correctly.", LogLevel.Info);
                else
                    WaterPetBowl(_waterBowlLocation);
            }
        }
        private void AfterLoad(object sender, SaveLoadedEventArgs e)
        {
            if (Game1.whichFarm < 4)
                WaterPetBowl(new Vector2(54, 7));
            else
            {
                if(_waterBowlLocation.X == 54 && _waterBowlLocation.Y == 7)
                    Monitor.Log("It appears, you are using a custom map. In order for this mod to work correctly, you will need to hover your mouse over the water bowl and hit F9. This will set the water bowls coords, then the mod should work correctly.", LogLevel.Info);
                else
                    WaterPetBowl(_waterBowlLocation);
            }
        }

        //Method that will be used to water the bowl. I added this, so I wont have to copy and paste code.
        private void WaterPetBowl(Vector2 tileLocation)
        {
            if (!_enableMod)
                return;
            Farm farm = Game1.getFarm();
            if (Game1.isRaining || Game1.isLightning || (Game1.isSnowing && _enableSnowWatering) ||
                CheckForSprinklers(tileLocation))
            {
                farm.setMapTileIndex(Convert.ToInt32(tileLocation.X), Convert.ToInt32(tileLocation.Y), 1939, "Buildings", 0);
                Monitor.Log("Water bowl should be filled.");
            }
            else
                farm.setMapTileIndex(Convert.ToInt32(tileLocation.X), Convert.ToInt32(tileLocation.Y), 1938, "Buildings", 0);


        }

        private bool CheckForSprinklers(Vector2 tileLocation)
        {
            bool sprinklerFound = false;
            Farm farm = Game1.getFarm();
            foreach (KeyValuePair<Vector2, SObject> farmObjects in farm.objects.Pairs)
            {
                if (_config.EnableSprinklerWatering && farmObjects.Value.ParentSheetIndex == 645)
                {
                    for (int x = (int)tileLocation.X - 2; x <= tileLocation.X + 2; x++)
                    {
                        for (int y = (int) tileLocation.Y - 2; y <= tileLocation.Y + 2; y++)
                        {
                            Vector2 newLoc = new Vector2(x, y);
                            if (farm.getTileIndexAt(Convert.ToInt32(newLoc.X), Convert.ToInt32(newLoc.Y),
                                    "Buildings") == 1938)
                                sprinklerFound = true;
                        }
                    }
                }
            }
            return sprinklerFound;
        }

        private bool CheckBowlLocation(Vector2 tileLocation)
        {
            bool found = false;
            Farm farm = Game1.getFarm();
            if (farm.getTileIndexAt((int)tileLocation.X, (int)tileLocation.Y, "Buildings") == 1938)
                found = true;
            return found;
        }
    }
}
