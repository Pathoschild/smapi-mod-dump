using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;
using WallPaperRecycler;
using SFarmer = StardewValley.Farmer;
using SObject = StardewValley.Object;

namespace WallPaperRecycler
{
    public class ModEntry : Mod
    {
        //public static Mod instance;
        private static int DoNothing = -333000;
        private DecorationFacade wallPaper;
        private DecorationFacade floor;
        //private int wallPaper;
        //private int floor;

        //Variable for testing purposes
        public bool debugging = false;

        //Set up ModConfig
        private ModConfig Config;

        public override void Entry(IModHelper helper)
        {
           // instance = this;
            //Read Config.
            this.Config = helper.ReadConfig<ModConfig>();
            PlayerEvents.InventoryChanged += this.PlayerEvents_InventoryChanged;
        }
        
        //Inventory Changed Void. Handles Saving floors and wallpaper if its changed.
        public void PlayerEvents_InventoryChanged(object sender, EventArgsInventoryChanged e)
        {
            if (!Game1.hasLoadedGame || (Game1.player.currentLocation.GetType() != typeof(FarmHouse)) || Game1.activeClickableMenu != null)
                return;
            FarmHouse location = Game1.player.currentLocation as FarmHouse;
            Wallpaper removed = this.RemovePaper(e.Removed);
            if (removed == null)
                return;

            int whatChanged;
            if (removed.isFloor.Value)
            {
                whatChanged = this.FloorChanged(location);
                this.doFloorSave(location);
            }
            else
            {
                whatChanged = this.WallpaperChanged(location);
                this.doWallpaperSave(location);
            }

            if (whatChanged != DoNothing)
                this.changeWallFloor(whatChanged, removed.isFloor.Value);
            else
                Game1.player.addItemToInventory((Item)removed);
            
        }

        //Custom Voids
        
        private void doWallpaperSave(FarmHouse house)
        {
            DecorationFacade curWallpaper = house.wallPaper;
            this.wallPaper = new DecorationFacade();
            for(int i = 0; i < curWallpaper.Count; i++)
            {
                this.wallPaper.Add(curWallpaper[i]);
                if(debugging)
                    this.Monitor.Log($"Current Wallpaper:{curWallpaper[i]}", LogLevel.Alert);
            }
            
        }
        private void doFloorSave(FarmHouse house)
        {
            DecorationFacade curFloor = house.floor;
            this.floor = new DecorationFacade();
            for(int i =0; i < curFloor.Count; i++)
            {
                this.floor.Add(curFloor[i]);
                if (debugging)
                    this.Monitor.Log($"Current Floor:{curFloor[i]}", LogLevel.Alert);
            }
        }
        private int WallpaperChanged(FarmHouse house)
        {
            DecorationFacade curWallPaper = house.wallPaper;
            if (curWallPaper == null || curWallPaper.Count == 0 || (this.wallPaper == null || this.wallPaper.Count == 0))
                return 0;

            for(int i = 0; i < curWallPaper.Count; i++)
            {
                if (!this.wallPaper[i].Equals(curWallPaper[i]))
                    return this.wallPaper[i];
            }
            return DoNothing;
        }
        private int FloorChanged(FarmHouse house)
        {
            DecorationFacade curFloor = house.floor;

            if (curFloor == null || curFloor.Count == 0 || (this.floor == null || this.floor.Count == 0))
                return 0;

            for (int i = 0; i < curFloor.Count; i++)
            {
                if (!this.floor[i].Equals(curFloor[i]))
                    return this.floor[i];
            }

            return DoNothing;
        }
        private Wallpaper RemovePaper(List<ItemStackChange> changedWalls)
        {
            using(List<ItemStackChange>.Enumerator wenumerator = changedWalls.GetEnumerator())
            {
                while (wenumerator.MoveNext())
                {
                    ItemStackChange curr = wenumerator.Current;
                    if (curr.Item.GetType() == typeof(Wallpaper) && curr.ChangeType == 0)
                        return (Wallpaper)curr.Item;
                }
            }
            return (Wallpaper)null;
        }
        private void changeWallFloor(int i, bool isFlooring)
        {
            Wallpaper paper = new Wallpaper(i, isFlooring);
            Game1.player.addItemToInventory((Item)paper);
           // Game1.updatewa
            string wallFloor = "Wall";
            if(debugging)
                this.Monitor.Log($"Current Floor:{paper.Name} id: {paper.ParentSheetIndex}", LogLevel.Alert);
            if (isFlooring)
                wallFloor = "Floor";
            if (this.Config.showMessages)
            {
                HUDMessage hmsg = new HUDMessage("Added: " + wallFloor);
                Game1.addHUDMessage(hmsg);
            }
            
        }    
    }
}
