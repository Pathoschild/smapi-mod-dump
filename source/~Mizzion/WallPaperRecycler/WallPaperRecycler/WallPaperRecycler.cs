/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

using System;
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
        

        private readonly Dictionary<string, string> _curWallPaper = new();

        private readonly Dictionary<string, string> _currentFloor = new();

        //Variable for testing purposes
        public bool Debugging = false;

        //Set up ModConfig
        private ModConfig _config;

        public override void Entry(IModHelper helper)
        {
            //Read Config.
            _config = helper.ReadConfig<ModConfig>();
            helper.Events.Player.InventoryChanged += PlayerEvent_InventoryChanged;
            helper.Events.Input.ButtonPressed += InputEvent_ButtonPressed;
            helper.Events.GameLoop.DayStarted += GameLoopEvent_DayStarted;
            helper.Events.Player.Warped += PlayerEvent_Warped;
        }

        /// <summary>
        /// Event that happens when a button is pressed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InputEvent_ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;
            if (e.IsDown(SButton.NumPad5))
            {
                //Manually check stuff. For debugging purposes.
            }
        }

        /// <summary>
        /// Event that happens when a new day starts, or if the player just logs into the game.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameLoopEvent_DayStarted(object sender, DayStartedEventArgs e)
        {
            GetCurrentWalls();
            GetCurrentFloors();
        }

        /// <summary>
        /// Event that happens when the player warps to a new location.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlayerEvent_Warped(object sender, WarpedEventArgs e)
        {
            if (e.NewLocation is FarmHouse)
            {
                GetCurrentWalls();
                GetCurrentFloors();
            }
            else if (e.NewLocation is Cabin)
            {
                GetCurrentWalls();
                GetCurrentFloors();
            }
        }
        /// <summary>
        /// Event that happens when the players inventory changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlayerEvent_InventoryChanged(object sender, InventoryChangedEventArgs e)
        {
            FarmHouse farm = null;
            Cabin cabin = null;
            bool isFarmHouse = Game1.player.currentLocation.GetType() == typeof(FarmHouse);
            bool isCabin = Game1.player.currentLocation.GetType() == typeof(Cabin);
            
            if (isFarmHouse)
                farm = Game1.player.currentLocation as FarmHouse;
            if (isCabin)
                cabin = Game1.player.currentLocation as Cabin;
            //Make sure either farm or cabin is NOT null
            if (farm == null && cabin == null)
                return;

            Wallpaper removed = RemovePaper(e.Removed);


            if (removed == null)
                return;

            if (isFarmHouse)
            {
                if (removed.isFloor.Value)
                {
                    DoFloorChange();
                }
                else
                {
                    DoWallPaperChange();
                }
            }
            if (isCabin)
            {
                if (removed.isFloor.Value)
                {
                    DoFloorChange(false);
                }
                else
                {
                    DoWallPaperChange(false);
                }
            }
        }

        //Custom Methods
        /// <summary>
        /// Grabs the current Walls applied inside the house/cabin. That way we can check for changes.
        /// </summary>
        private void GetCurrentWalls()
        {
            bool isFarmHouse = Game1.player.currentLocation.GetType() == typeof(FarmHouse);
            bool isCabin = Game1.player.currentLocation.GetType() == typeof(Cabin);
            _curWallPaper.Clear();

            if (isFarmHouse)
            {
                if (Game1.player.currentLocation is FarmHouse farm)
                    foreach (var walls in farm.appliedWallpaper.Pairs)
                    {
                        _curWallPaper.Add(walls.Key, walls.Value);
                        if (Debugging)
                        {
                            Monitor.Log($"Added Wallpaper ID: {walls.Value} Location: {walls.Key} to curWallPaper.",
                                LogLevel.Alert);
                        }
                    }
            }
            else if (isCabin)
            {
                if (Game1.player.currentLocation is Cabin cabin)
                    foreach (var walls in cabin.appliedWallpaper.Pairs)
                    {
                        _curWallPaper.Add(walls.Key, walls.Value);
                        if (Debugging)
                        {
                            Monitor.Log($"Added Wallpaper ID: {walls.Value} Location: {walls.Key} to curWallPaper.",
                                LogLevel.Alert);
                        }
                    }
            }
        }

        /// <summary>
        /// Grabs the current Floors applied inside the house/cabin. That way we can check for changes.
        /// </summary>
        private void GetCurrentFloors()
        {
            bool isFarmHouse = Game1.player.currentLocation.GetType() == typeof(FarmHouse);
            bool isCabin = Game1.player.currentLocation.GetType() == typeof(Cabin);
            _currentFloor.Clear();

            if (isFarmHouse)
            {
                if (Game1.player.currentLocation is FarmHouse farm)
                    foreach (var floor in farm.appliedFloor.Pairs)
                    {
                        _currentFloor.Add(floor.Key, floor.Value);
                        if (Debugging)
                        {
                            Monitor.Log($"Added Floor ID: {floor.Value} Location: {floor.Key} to currentFloor.",
                                LogLevel.Alert);
                        }
                    }
            }
            else if (isCabin)
            {
                if (Game1.player.currentLocation is Cabin cabin)
                    foreach (var floor in cabin.appliedFloor.Pairs)
                    {
                        _currentFloor.Add(floor.Key, floor.Value);
                        if (Debugging)
                        {
                            Monitor.Log($"Added Floor ID: {floor.Value} Location: {floor.Key} to currentFloor.",
                                LogLevel.Alert);
                        }
                    }
            }
        }

        private void DoWallPaperChange(bool isFarmHouse = true)
        {
            if (isFarmHouse)
            {
                //Lets scan curWallPaper and see what was changed
                foreach (var walls in ((FarmHouse)Game1.player.currentLocation).appliedWallpaper.Pairs)
                {
                    foreach (var curWalls in _curWallPaper)
                    {
                        if (curWalls.Key.Equals(walls.Key))
                        {
                            if (curWalls.Value != walls.Value)
                            {
                                Wallpaper paper = new Wallpaper(Convert.ToInt32(curWalls.Value));
                                Game1.player.addItemToInventory(paper);
                                _curWallPaper[curWalls.Key] = walls.Value;
                                if (_config.ShowMessages)
                                {
                                    HUDMessage hmsg = new HUDMessage($"Added: Wallpaper from: {curWalls.Key}.");
                                    Game1.addHUDMessage(hmsg);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                //Lets scan curWallPaper and see what was changed
                foreach (var walls in ((Cabin)Game1.player.currentLocation).appliedWallpaper.Pairs)
                {
                    foreach (var curWalls in _curWallPaper)
                    {
                        if (curWalls.Key.Equals(walls.Key))
                        {
                            if (curWalls.Value != walls.Value)
                            {
                                Wallpaper paper = new Wallpaper(Convert.ToInt32(curWalls.Value));
                                Game1.player.addItemToInventory(paper);
                                _curWallPaper[curWalls.Key] = walls.Value;
                                if (_config.ShowMessages)
                                {
                                    HUDMessage hmsg = new HUDMessage($"Added: Wallpaper from: {curWalls.Key}.");
                                    Game1.addHUDMessage(hmsg);
                                }
                            }
                        }
                    }
                }
            }
            
        }

        private void DoFloorChange(bool isFarmHouse = true)
        {
            if (isFarmHouse)
            {
                //Lets scan currentFloor and see what was changed
                foreach (var floor in ((FarmHouse)Game1.player.currentLocation).appliedFloor.Pairs)
                {
                    foreach (var curFloor in _currentFloor)
                    {
                        if (curFloor.Key.Equals(floor.Key))
                        {
                            if (curFloor.Value != floor.Value)
                            {
                                Wallpaper paper = new Wallpaper(Convert.ToInt32(curFloor.Value), true);
                                Game1.player.addItemToInventory(paper);
                                _currentFloor[curFloor.Key] = floor.Value;
                                if (_config.ShowMessages)
                                {
                                    HUDMessage hmsg = new HUDMessage($"Added: Floor from: {curFloor.Key}.");
                                    Game1.addHUDMessage(hmsg);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                //Lets scan currentFloor and see what was changed
                foreach (var floor in ((Cabin)Game1.player.currentLocation).appliedFloor.Pairs)
                {
                    foreach (var curFloor in _currentFloor)
                    {
                        if (curFloor.Key.Equals(floor.Key))
                        {
                            if (curFloor.Value != floor.Value)
                            {
                                Wallpaper paper = new Wallpaper(Convert.ToInt32(curFloor.Value));
                                Game1.player.addItemToInventory(paper);
                                _currentFloor[curFloor.Key] = floor.Value;
                                if (_config.ShowMessages)
                                {
                                    HUDMessage hmsg = new HUDMessage($"Added: Floor from: {curFloor.Key}.");
                                    Game1.addHUDMessage(hmsg);
                                }
                            }
                        }
                    }
                }
            }
            
        }

        private Wallpaper RemovePaper(IEnumerable<Item> changedWalls)
        {
            foreach (Item curr in changedWalls)
            {
                if (curr.GetType() == typeof(Wallpaper))
                    return (Wallpaper)curr;
            }
            return null;
        }
    }
}