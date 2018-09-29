using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;

namespace ExtendedFridge
{
    public class M007_ExtendedFridge_Mod : Mod
    {
        private static FridgeChest _fridge;
        private static FridgeModConfig config;
        internal static ISemanticVersion Version;
        private static bool IsInFridgeMenu = false;
        private static readonly int FRIDGE_TILE_ID = 173;

        public override void Entry(IModHelper helper)
        {
            var modPath = Helper.DirectoryPath;
            config = Helper.ReadConfig<FridgeModConfig>();
            Version = this.ModManifest.Version;

            MenuEvents.MenuChanged += Event_MenuChanged;

            //LocationEvents.CurrentLocationChanged += Event_LocationChanged;
            StardewModdingAPI.Events.ControlEvents.KeyReleased += Event_KeyReleased;
            this.Monitor.Log("ExtendedFridge Entry");
        }

        public void CheckForAction(bool isBack = false)
        {
            int a = 5;
        }

        private void Event_KeyReleased(object send, EventArgsKeyPressed e)
        {
            if (e.KeyPressed.ToString().Equals(config.fridgeNextPageKey) && Game1.activeClickableMenu is FridgeGrabMenu)
            {
                _fridge.MovePageToNext();
            }

            if (e.KeyPressed.ToString().Equals(config.fridgePrevPageKey) && Game1.activeClickableMenu is FridgeGrabMenu)
            {
                _fridge.MovePageToPrevious();
            }
        }

        private void Event_LocationChanged(object send, EventArgsCurrentLocationChanged e)
        {
            var priorlocation = e.PriorLocation;
            if (e.NewLocation is FarmHouse)
            {
                FarmHouse ptrFH = (FarmHouse)Game1.currentLocation;
            }
        }

        private void Event_MenuChanged(object send, EventArgsClickableMenuChanged e)
        {
            Microsoft.Xna.Framework.Vector2 lastGrabbedTile = Game1.player.lastGrabTile;
            //Log.Debug("M007_ExtendedFridge Event_MenuChanged HIT", new object[0]);

            if (Game1.currentLocation is FarmHouse)
            {
                this.Monitor.Log(String.Format("M007_ExtendedFridge lastGrabTileX:{0} lastGrabTileY:{1}", (int)Game1.player.lastGrabTile.X, (int)Game1.player.lastGrabTile.Y));
            }

            if (ClickedOnFridge())
            {

                IsInFridgeMenu = true;
                if (e.NewMenu is ItemGrabMenu)
                {
                    ItemGrabMenu ptrMenu = (ItemGrabMenu)e.NewMenu;

                    if (_fridge == null)
                    {
                        _fridge = new FridgeChest(config.autoSwitchPageOnGrab);
                        StardewValley.Locations.FarmHouse h = (StardewValley.Locations.FarmHouse)Game1.currentLocation;
                        _fridge.items.AddRange(h.fridge.items);
                    }
                    _fridge.ShowCurrentPage();
                    this.Monitor.Log("M007_ExtendedFridge Fridge HOOKED");
                }

            }
        }

        void runConfig()
        {
            var myconfig = Helper.ReadConfig<FridgeModConfig>();
        }

        public void grabItemFromChest(Item item, StardewValley.Farmer who)
        {
            _fridge.grabItemFromChest(item, who);
        }

        public void grabItemFromInventory(Item item, StardewValley.Farmer who)
        {
            _fridge.grabItemFromInventory(item, who);
        }

        private bool ClickedOnFridge()
        {  
            if (Game1.currentLocation is FarmHouse)
            {
                FarmHouse ptrFarmhouse = (FarmHouse)Game1.currentLocation;
                xTile.Dimensions.Location tileLocation = new xTile.Dimensions.Location((int)Game1.player.lastGrabTile.X, (int)Game1.player.lastGrabTile.Y);

                if (ptrFarmhouse.map.GetLayer("Buildings").Tiles[tileLocation] != null)
                {
                    int currTileIdx = ptrFarmhouse.map.GetLayer("Buildings").Tiles[tileLocation].TileIndex;

                    return currTileIdx == FRIDGE_TILE_ID;
                }
            }

            return false;
        }
    }
}