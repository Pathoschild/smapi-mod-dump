/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;

namespace WallPaperRecycler
{
    public class WallPaperRecycler : Mod
    {
        //public static Mod instance;
        private const int DoNothing = -333000;

        private DecorationFacade _wallPaper;
        private DecorationFacade _floor;
        //private int wallPaper;
        //private int floor;

        //Variable for testing purposes
        public bool Debugging = false;

        //Set up ModConfig
        private ModConfig _config;

        public override void Entry(IModHelper helper)
        {
            //Read Config.
            _config = helper.ReadConfig<ModConfig>();
            helper.Events.Player.InventoryChanged += InventoryChanged;
            helper.Events.Input.ButtonPressed += ButtonPressed;
            helper.Events.GameLoop.DayStarted += DayStarted;
        }

        //When the day starts. So we can populate the shit.
        private void DayStarted(object sender, DayStartedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;
            //Run population code
            FarmHouse farm = null;
            Cabin cabin = null;
            bool isFarmHouse = Game1.player.currentLocation.GetType() == typeof(FarmHouse);
            bool isCabin = Game1.player.currentLocation.GetType() == typeof(Cabin);
            if (isFarmHouse)
            {
                farm = Game1.player.currentLocation as FarmHouse;
                DoWallpaperSave(farm);
            }
            if (isCabin)
            {
                cabin = Game1.player.currentLocation as Cabin;
                DoWallpaperSave(cabin);
            }

            DecorationFacade curWall = isFarmHouse ? farm?.wallPaper : cabin?.wallPaper;

            if (curWall != null)
            {
                for (int i = 0; i < curWall.Count; i++)
                {
                    if (!_wallPaper[i].Equals(curWall[i]))
                        Monitor.Log($"Current Wallpaper: {curWall[i]}", LogLevel.Alert);
                }
            }
            //Populate flooring
            DecorationFacade curFloor = isFarmHouse ? farm?.floor : cabin?.floor;
            if (curFloor != null)
            {
                _floor = new DecorationFacade();
                foreach (int i in curFloor)
                {
                    _floor.Add(i);
                    if (Debugging)
                        Monitor.Log($"Current Floor: {i}", LogLevel.Alert);
                }
            }
        }
        //Button Pressed event for debugging

        private void ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;
            if (e.IsDown(SButton.NumPad5))
            {
                FarmHouse farm = null;
                Cabin cabin = null;
                bool isFarmHouse = Game1.player.currentLocation.GetType() == typeof(FarmHouse);
                bool isCabin = Game1.player.currentLocation.GetType() == typeof(Cabin);
                if (isFarmHouse)
                {
                    farm = Game1.player.currentLocation as FarmHouse;
                    DoWallpaperSave(farm);
                }
                if (isCabin)
                {
                    cabin = Game1.player.currentLocation as Cabin;
                    DoWallpaperSave(cabin);
                }

                DecorationFacade curWall = isFarmHouse ? farm?.wallPaper : cabin?.wallPaper;
                if (curWall != null)
                {
                    for (int i = 0; i < curWall.Count; i++)
                    {
                        if (!_wallPaper[i].Equals(curWall[i]))
                            Monitor.Log($"Current Wallpaper: {curWall[i]}", LogLevel.Alert);
                    }
                }
            }
        }
        //Inventory Changed Void. Handles Saving floors and wallpaper if its changed.
        private void InventoryChanged(object sender, InventoryChangedEventArgs e)
        {
            if (!Context.IsWorldReady || Game1.activeClickableMenu != null)
                return;
            //Check locations to see if they're farmhouses or cabins
            FarmHouse farm = null;
            Cabin cabin = null;
            bool isFarmHouse = Game1.player.currentLocation.GetType() == typeof(FarmHouse);
            bool isCabin = Game1.player.currentLocation.GetType() == typeof(Cabin);
            /*
            if (isFarmHouse)
                farm = Game1.player.currentLocation as FarmHouse;
            if (isCabin)
                cabin = Game1.player.currentLocation as Cabin;
            */
            if (isFarmHouse)
                farm = Game1.player.currentLocation as FarmHouse;
            if (isCabin)
                cabin = Game1.player.currentLocation as Cabin;
            //Make sure either farm or cabin is NOT null
            if (farm == null && cabin == null)
                return;

            //Everything seems to have passed. Now we can move on
            Wallpaper removed = RemovePaper(e.Removed);
            int whatChanged = 0;
            if (removed == null && Game1.activeClickableMenu == null)
            {
                if(Debugging)
                    Monitor.Log("Removed was null.. Line 55", LogLevel.Alert);
                return;
            }
            //Process Cabin
            if (isCabin)
            {
                if (removed != null && removed.isFloor.Value)
                {
                    whatChanged = FloorChanged(cabin);
                    DoFloorSave(cabin);
                }
                else
                {
                    whatChanged = WallpaperChanged(cabin);
                    DoWallpaperSave(cabin);
                }
            }
            //Process Farm
            if (isFarmHouse)
            {
                if (removed != null && removed.isFloor.Value)
                {
                    whatChanged = FloorChanged(farm);
                    DoFloorSave(farm);
                }
                else
                {
                    whatChanged = WallpaperChanged(farm);
                    DoWallpaperSave(farm);
                }
            }
            if (whatChanged != DoNothing && removed != null)
                ChangeWallFloor(whatChanged, removed.isFloor.Value);
            /*else
                Game1.player.addItemToInventory(removed);*/
        }

        //DoWallpaperSave method for FarmHouse's
        private void DoWallpaperSave(FarmHouse house)
        {
            DecorationFacade curWallpaper = house.wallPaper;
            _wallPaper = new DecorationFacade();
            foreach (int i in curWallpaper)
            {
                _wallPaper.Add(i);
                if (Debugging)
                    Monitor.Log($"Current Wallpaper: {i}", LogLevel.Alert);
            }
        }
        
        //DoFloorSave method for FarmHouse's.
        private void DoFloorSave(FarmHouse house)
        {
            DecorationFacade curFloor = house.floor;
            _floor = new DecorationFacade();
            foreach (int i in curFloor)
            {
                _floor.Add(i);
                if (Debugging)
                    Monitor.Log($"Current Floor: {i}", LogLevel.Alert);
            }
        }
       
        //WallpaperChanged method for FarmHouses's
        private int WallpaperChanged(FarmHouse house)
        {
            DecorationFacade curWall = house.wallPaper;
            int wallId;

            if (curWall == null || curWall.Count == 0 || _wallPaper == null || _wallPaper.Count == 0)
                wallId = 0;
            else
            {
                for (int i = 0; i < curWall.Count; i++)
                {
                    if (!_wallPaper[i].Equals(curWall[i]))
                        return _wallPaper[i];
                }
                wallId = DoNothing;
            }
            return wallId;
        }
        
        //FloorChanged method for FarmHouse's
        private int FloorChanged(FarmHouse house)
        {
            DecorationFacade curFloor = house.floor;
            int floorId;
            if (curFloor == null || curFloor.Count == 0 || _floor == null || _floor.Count == 0)
                floorId = 0;
            else
            {
                for (int i = 0; i < curFloor.Count; i++)
                {
                    if (!_floor[i].Equals(curFloor[i]))
                        return _floor[i];
                }
                floorId = DoNothing;
            }

            return floorId;
        }

        //DoWallpaperSave method for Cabin's
        private void DoWallpaperSave(Cabin house)
        {
            DecorationFacade curWallpaper = house.wallPaper;
            _wallPaper = new DecorationFacade();
            foreach (int i in curWallpaper)
            {
                _wallPaper.Add(i);
                if (Debugging)
                    Monitor.Log($"Current Wallpaper: {i}", LogLevel.Alert);
            }
        }
        
        //DoFloorSave method for Cabin's.
        private void DoFloorSave(Cabin house)
        {
            DecorationFacade curFloor = house.floor;
            _floor = new DecorationFacade();
            foreach (int i in curFloor)
            {
                _floor.Add(i);
                if (Debugging)
                    Monitor.Log($"Current Floor: {i}", LogLevel.Alert);
            }
        }
        
        //WallpaperChanged method for Cabin's
        private int WallpaperChanged(Cabin house)
        {
            DecorationFacade curWall = house.wallPaper;
            int wallId;

            if (curWall == null || curWall.Count == 0 || _wallPaper == null || _wallPaper.Count == 0)
                wallId = 0;
            else
            {
                for (int i = 0; i < curWall.Count; i++)
                {
                    if (!_wallPaper[i].Equals(curWall[i]))
                        return _wallPaper[i];
                }
                wallId = DoNothing;
            }
            return wallId;
        }
        
        //FloorChanged method for Cabin's
        private int FloorChanged(Cabin house)
        {
            DecorationFacade curFloor = house.floor;
            int floorId;
            if (curFloor == null || curFloor.Count == 0 || _floor == null || _floor.Count == 0)
                floorId = 0;
            else
            {
                for (int i = 0; i < curFloor.Count; i++)
                {
                    if (!_floor[i].Equals(curFloor[i]))
                        return _floor[i];
                }
                floorId = DoNothing;
            }

            return floorId;
        }

        //ChangeWallFloor method
        private void ChangeWallFloor(int i, bool isFlooring)
        {
            Wallpaper paper = new Wallpaper(i, isFlooring);
            Game1.player.addItemToInventory(paper);
            string wallFloor = isFlooring ? "Floor" : "Wallpaper";

            if(Debugging)
                Monitor.Log($"Current {wallFloor} Name: {paper.Name}, ID: {paper.ParentSheetIndex}");

            if (_config.ShowMessages)
            {
                HUDMessage hmsg = new HUDMessage($"Added: {wallFloor}");
                //Game1.addHUDMessage(new HUDMessage("Decor", 1, true, Color.LightGoldenrodYellow, (Item)new StardewValley.Object(paper.ParentSheetIndex, 1, false, -1, 0)));
                Game1.addHUDMessage(hmsg);
            }
        }

        //RemovePaper method
        private Wallpaper RemovePaper(IEnumerable<Item> changedWalls)
        {
            foreach (Item curr in changedWalls)
            {
                if (curr.GetType() == typeof(Wallpaper))
                    return (Wallpaper) curr;
            }
            return null;
        }
    }
}