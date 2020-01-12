using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using System;
using xTile.Dimensions;
using xTile.Layers;
using xTile.Tiles;

namespace GetGlam.Framework
{
    /// <summary>Class that handles the Dresser stuff.</summary>
    public class DresserHandler
    {
        //Instance of ModEntry
        private ModEntry Entry;

        //Instance of ModConfig
        private ModConfig Config;

        //Instance of the FarmHouse GameLocation
        public GameLocation FarmHouse;

        //The dressers position in the house
        private Point DresserPosition;

        //The Dressers texture
        public Texture2D Texture;

        //The Dresser texture source rect
        public Microsoft.Xna.Framework.Rectangle TextureSourceRect = new Microsoft.Xna.Framework.Rectangle(0, 0, 16, 32);

        //Instance of GlamMenu
        public GlamMenu Menu;

        //The Tile sheet position of the dresser, default (1, 0)
        private Point DresserTileSheetPoint = new Point(1, 0);

        /// <summary>DresserHandlers Constructor</summary>
        /// <param name="entry">The instance of <see cref="ModEntry"/></param>
        /// <param name="config">The instance of <see cref="ModConfig"/></param>
        public DresserHandler(ModEntry entry, ModConfig config)
        {
            //set the vars to the instances
            Entry = entry;
            Config = config;
        }

        /// <summary>Places the dresser inside of the Farmhouse</summary> 
        public void PlaceDresser()
        {
            if (!Config.PatchDresserInFarmHouse)
                return;

            //Create the new Tilesheet
            TileSheet dresserTilesheet = new TileSheet("z_Dresser", FarmHouse.map, $"Mods/{Entry.ModManifest.UniqueID}/dresser.png", new Size(1, Texture.Height / 16), new Size(16, 16));

            //Get the dresser position
            DresserPosition = GetDresserPosition();

            //Add and Load the Tilesheet to the Farmhouse map
            FarmHouse.map.AddTileSheet(dresserTilesheet);
            FarmHouse.map.LoadTileSheets(Game1.mapDisplayDevice);

            //Set the tile property
            FarmHouse.setTileProperty(DresserPosition.X, DresserPosition.Y, "Building", "Action", "T");

            //Grab the building layer from the farmhouse map
            Layer buildingLayer = FarmHouse.map.GetLayer("Buildings");
            Layer frontLayer = FarmHouse.map.GetLayer("Front");

            //Wrap in try..catch in case the user removes the dresser texture
            try
            {
                //Set the Tiles to the new tiles
                buildingLayer.Tiles[DresserPosition.X, DresserPosition.Y + 1] = new StaticTile(buildingLayer, dresserTilesheet, BlendMode.Alpha, DresserTileSheetPoint.X);
                frontLayer.Tiles[DresserPosition.X, DresserPosition.Y] = new StaticTile(frontLayer, dresserTilesheet, BlendMode.Alpha, DresserTileSheetPoint.Y);
            }
            catch (Exception ex)
            {
                //Print an error and set it to the default dresser
                Entry.Monitor.Log("Could not find the dresser in the TileSheet, setting the dresser to default.", LogLevel.Warn);
                buildingLayer.Tiles[DresserPosition.X, DresserPosition.Y + 1] = new StaticTile(buildingLayer, dresserTilesheet, BlendMode.Alpha, 1);
                frontLayer.Tiles[DresserPosition.X, DresserPosition.Y] = new StaticTile(frontLayer, dresserTilesheet, BlendMode.Alpha, 0);
                TextureSourceRect.Y = 0;
            }

            //Update the farmhouse map
            FarmHouse.updateMap();
        }

        /// <summary>Updates the dresser in the farmhouse when switching dressers in the Glam Menu</summary>
        public void UpdateDresserInFarmHouse()
        {
            if (!Config.PatchDresserInFarmHouse)
                return;

            //Grab the tilesheet from the farmhouse and Load it
            TileSheet dresserTileSheet = FarmHouse.map.GetTileSheet("z_Dresser");
            FarmHouse.map.LoadTileSheets(Game1.mapDisplayDevice);

            //Grab the building layer and Front layer from the farmhouse map
            Layer buildingLayer = FarmHouse.map.GetLayer("Buildings");
            Layer frontLayer = FarmHouse.map.GetLayer("Front");

            //Set the new tiles to the new tiles
            buildingLayer.Tiles[DresserPosition.X, DresserPosition.Y + 1] = new StaticTile(buildingLayer, dresserTileSheet, BlendMode.Alpha, DresserTileSheetPoint.X);
            frontLayer.Tiles[DresserPosition.X, DresserPosition.Y] = new StaticTile(frontLayer, dresserTileSheet, BlendMode.Alpha, DresserTileSheetPoint.Y);

            //Update the farmhouse map
            FarmHouse.updateMap();
        }

        /// <summary>Set the Dressers texture</summary>
        public void SetDresserTexture()
        {
            Texture = Entry.Helper.Content.Load<Texture2D>($"Mods/{Entry.ModManifest.UniqueID}/dresser.png", ContentSource.GameContent);
        }

        /// <summary>Sets the dresser tilesheet point</summary>
        /// <param name="dresserIndex">The current dresser index</param>
        public void SetDresserTileSheetPoint(int dresserIndex)
        {
            DresserTileSheetPoint.X = dresserIndex.Equals(1) ? 1: TextureSourceRect.Y / 16 + 1;
            DresserTileSheetPoint.Y = dresserIndex.Equals(1) ? 0 : TextureSourceRect.Y / 16;
        }

        /// <summary>Gets the number of dressers using the dresser texture height</summary>
        /// <returns>The number of dressers</returns>
        public int GetNumberOfDressers()
        {
            return Texture.Height / 32;
        }

        /// <summary>Used to check if the Dresser has been interacted with</summary>
        /// <param name="button">The button that was pressed</param>
        public void DresserInteractCheck(SButton button)
        {
            //Check if they are in the Farmhouse, they're free, they clicked the right button and the menu is null
            if (Game1.player.currentLocation == FarmHouse && Context.IsPlayerFree && Game1.player.GetGrabTile() == new Vector2(DresserPosition.X, DresserPosition.Y + 1) && IsActionButton(button) && Game1.activeClickableMenu == null)
            {
                //Take a snapshot of the player
                Menu.TakeSnapshot();

                //Change the player direction and stop whatever they were doing, set the menu
                Game1.player.faceDirection(2);
                Game1.player.FarmerSprite.StopAnimation();
                Game1.player.completelyStopAnimatingOrDoingAction();
                Game1.activeClickableMenu = Menu;
            }
        }

        /// <summary>Gets the dressers position based on the current farmhouse upgrade level</summary>
        /// <returns>The point of the dresser in the farmhouse</returns>
        private Point GetDresserPosition()
        {
            //Set the point to Zero
            Point newPosition = new Point(0, 0);

            //If the Config is (0, 0)
            if (Config.DresserTableLocationX == 0 && Config.DresserTableLocationY == 0)
            {
                //Check the farmhouse level and set the new point
                switch ((this.FarmHouse as FarmHouse).upgradeLevel)
                {
                    case 0:
                        newPosition = new Point(Config.StoveInCorner ? 7 : 10, 2);
                        break;
                    case 1:
                        newPosition = new Point(Game1.player.isMarried() ? 25 : 28, 2);
                        break;
                    case 2:
                        newPosition = new Point(33, 11);
                        break;
                    case 3:
                        newPosition = new Point(33, 11);
                        break;
                    default:
                        Entry.Monitor.Log($"Could not patch dresser into the FarmHouse upgrade level: {(this.FarmHouse as FarmHouse). upgradeLevel}", LogLevel.Warn);
                        break;
                }
            }
            else
            {
                //The new postion is now the Configs position
                newPosition = new Point(Config.DresserTableLocationX, Config.DresserTableLocationY);
            }

            //Return teh position
            return newPosition;
        }

        /// <summary>Checks if an Action button was pressed</summary>
        /// <param name="button">The button in question</param>
        /// <returns>Wether an Action Button was pressed</returns>
        private bool IsActionButton(SButton button)
        {
            //Check the different buttons
            if (button.Equals(SButton.MouseRight) || button.Equals(SButton.ControllerA) || button.IsActionButton())
                return true;

            return false;
        }
    }
}
