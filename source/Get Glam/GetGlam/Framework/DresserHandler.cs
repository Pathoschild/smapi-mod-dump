/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MartyrPher/GetGlam
**
*************************************************/

using GetGlam.Framework.Menus;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using xTile.Dimensions;
using xTile.Layers;
using xTile.Tiles;

namespace GetGlam.Framework
{
    /// <summary>
    /// Class that handles the Dresser stuff.
    /// </summary>
    public class DresserHandler
    {
        // Instance of ModEntry
        private ModEntry Entry;

        // Instance of ModConfig
        private ModConfig Config;

        // Instance of ContentPackHelper
        public ContentPackHelper PackHelper;

        // Instance of the FarmHouse GameLocation
        public GameLocation FarmHouse;

        // The dressers position in the house
        private Point DresserPosition;

        // The Dresser texture source rect
        public Microsoft.Xna.Framework.Rectangle TextureSourceRect = new Microsoft.Xna.Framework.Rectangle(0, 0, 16, 32);

        // Instance of GlamMenu
        public GlamMenu Menu;

        // The Tilesheet position of the dresser, default (1, 0)
        private Point DresserTileSheetPoint = new Point(1, 0);

        /// <summary>
        /// DresserHandlers Constructor.
        /// </summary>
        /// <param name="entry">The instance of <see cref="ModEntry"/></param>
        /// <param name="config">The instance of <see cref="ModConfig"/></param>
        public DresserHandler(ModEntry entry, ModConfig config, ContentPackHelper packHelper)
        {
            // Set the vars to the instances
            Entry = entry;
            Config = config;
            PackHelper = packHelper;
        }

        /// <summary>
        /// Places the dresser inside of the Farmhouse.
        /// </summary> 
        public void PlaceDresser()
        {
            if (!Config.PatchDresserInFarmHouse || !Context.IsMainPlayer)
                return;

            // Create the new Tilesheet
            TileSheet dresserTilesheet = new TileSheet("z_Dresser", FarmHouse.map, $"Mods/{Entry.ModManifest.UniqueID}/dresser.png", new Size(1, PackHelper.DresserTextureHeight / 16), new Size(16, 16));

            // Get the dresser position
            DresserPosition = GetDresserPosition();
            AddTileSheetToFarmhouse(dresserTilesheet);

            // Grab the building layer from the farmhouse map
            Layer buildingLayer = FarmHouse.map.GetLayer("Buildings");
            Layer frontLayer = FarmHouse.map.GetLayer("Front");

            UpdateFarmhouseTiles(dresserTilesheet, buildingLayer, frontLayer);

            // Update the farmhouse map
            FarmHouse.updateMap();
        }

        /// <summary>
        /// Adds the dresser tilesheet to the FarmHouse and sets the Tile Property.
        /// </summary>
        /// <param name="dresserTilesheet">The dresser tilesheet.</param>
        private void AddTileSheetToFarmhouse(TileSheet dresserTilesheet)
        {
            FarmHouse.map.AddTileSheet(dresserTilesheet);
            FarmHouse.map.LoadTileSheets(Game1.mapDisplayDevice);
            FarmHouse.setTileProperty(DresserPosition.X, DresserPosition.Y, "Building", "Action", "T");
        }

        /// <summary>
        /// Updates the farmhouse tiles with dresser position. 
        /// </summary>
        /// <param name="dresserTilesheet">The dresser tilesheet</param>
        /// <param name="buildingLayer">The building layer</param>
        /// <param name="frontLayer">The front layer</param>
        private void UpdateFarmhouseTiles(TileSheet dresserTilesheet, Layer buildingLayer, Layer frontLayer)
        {
            try
            {
                UpdateFarmhouseTileWithDresser(dresserTilesheet, buildingLayer, frontLayer, DresserTileSheetPoint.X, DresserTileSheetPoint.Y);
            }
            catch
            {
                // Print an error and set it to the default dresser
                Entry.Monitor.Log("Could not find the dresser in the TileSheet, setting the dresser to default.", LogLevel.Warn);
                UpdateFarmhouseTileWithDresser(dresserTilesheet, buildingLayer, frontLayer, 0, 1);
                TextureSourceRect.Y = 0;
            }
        }

        /// <summary>
        /// Updates the front layer and building layer tiles.
        /// </summary>
        /// <param name="dresserTilesheet">Dresser Tilesheet</param>
        /// <param name="buildingLayer">Building Layer</param>
        /// <param name="frontLayer">Front Layer</param>
        /// <param name="dresserTilesheetX">The new x coordinate of the dresser tilesheet</param>
        /// <param name="dresserTilesheetY">The new y coordinate of the dresser tilesheet</param>
        private void UpdateFarmhouseTileWithDresser(TileSheet dresserTilesheet, Layer buildingLayer, Layer frontLayer, int dresserTilesheetX, int dresserTilesheetY)
        {
            buildingLayer.Tiles[DresserPosition.X, DresserPosition.Y + 1] = new StaticTile(buildingLayer, dresserTilesheet, BlendMode.Alpha, dresserTilesheetX);
            frontLayer.Tiles[DresserPosition.X, DresserPosition.Y] = new StaticTile(frontLayer, dresserTilesheet, BlendMode.Alpha, dresserTilesheetY);
        }

        /// <summary>
        /// Updates the dresser in the farmhouse when switching dressers in the Glam Menu.
        /// </summary>
        public void UpdateDresserInFarmHouse()
        {
            if (!Config.PatchDresserInFarmHouse)
                return;

            // Grab the tilesheet from the farmhouse and Load it
            TileSheet dresserTilesheet = FarmHouse.map.GetTileSheet("z_Dresser");
            FarmHouse.map.LoadTileSheets(Game1.mapDisplayDevice);

            // Grab the building layer and Front layer from the farmhouse map
            Layer buildingLayer = FarmHouse.map.GetLayer("Buildings");
            Layer frontLayer = FarmHouse.map.GetLayer("Front");

            UpdateFarmhouseTileWithDresser(dresserTilesheet, buildingLayer, frontLayer, DresserTileSheetPoint.X, DresserTileSheetPoint.Y);

            // Update the farmhouse map
            FarmHouse.updateMap();
        }

        /// <summary>
        /// Sets the dresser tilesheet point.
        /// </summary>
        /// <param name="dresserIndex">The current dresser index</param>
        public void SetDresserTileSheetPoint(int dresserIndex)
        {
            DresserTileSheetPoint.X = dresserIndex.Equals(1) ? 1: TextureSourceRect.Y / 16 + 1;
            DresserTileSheetPoint.Y = dresserIndex.Equals(1) ? 0 : TextureSourceRect.Y / 16;
        }

        /// <summary>
        /// Gets the number of dressers using the dresser texture height.
        /// </summary>
        /// <returns>The number of dressers</returns>
        public int GetNumberOfDressers()
        {
            return PackHelper.DresserTextureHeight / 32;
        }

        /// <summary>
        /// Used to check if the Dresser has been interacted with.
        /// </summary>
        /// <param name="button">The button that was pressed</param>
        public void DresserInteractCheck(SButton button)
        {   
            if (CanPlayerOpenMenu(button))
            {
                Menu.TakeSnapshot();
                ChangePlayerDirection();
            }
        }

        /// <summary>
        /// Checks whether the player can open the menu.
        /// </summary>
        /// <param name="button">Current pressed button</param>
        /// <returns>If the player can open the menu</returns>
        private bool CanPlayerOpenMenu(SButton button)
        {
            // Check if they are in the Farmhouse, they're free, they clicked the right button and the menu is null
            return Game1.player.currentLocation == FarmHouse && Context.IsPlayerFree && Game1.player.GetGrabTile() == new Vector2(DresserPosition.X, DresserPosition.Y + 1) && IsActionButton(button) && Game1.activeClickableMenu == null;
        }

        /// <summary>
        /// Stops player animations and changes facing direction.
        /// </summary>
        private void ChangePlayerDirection()
        {
            Game1.player.faceDirection(2);
            Game1.player.FarmerSprite.StopAnimation();
            Game1.player.completelyStopAnimatingOrDoingAction();
            Game1.activeClickableMenu = Menu;
        }

        /// <summary>
        /// Gets the dressers position based on the current farmhouse upgrade level.
        /// </summary>
        /// <returns>The point of the dresser in the farmhouse</returns>
        private Point GetDresserPosition()
        {
            // Set the point to Zero
            Point newPosition = new Point(0, 0);

            // If the Config is (0, 0)
            if (Config.DresserTableLocationX == 0 && Config.DresserTableLocationY == 0)
            {
                // Check the farmhouse level and set the new point
                newPosition = SetDresserPointBasedOnFarmHouseUpgrade(newPosition);
            }
            else
            {
                // The new postion is now the Configs position
                newPosition = new Point(Config.DresserTableLocationX, Config.DresserTableLocationY);
            }

            // Return teh position
            return newPosition;
        }

        /// <summary>
        /// Sets the dressers point in the Farmhouse based on Upgrade Level.
        /// </summary>
        /// <param name="newPosition">The new position</param>
        /// <returns>The new point where the dresser will be placed</returns>
        private Point SetDresserPointBasedOnFarmHouseUpgrade(Point newPosition)
        {
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
                    Entry.Monitor.Log($"Could not patch dresser into the FarmHouse upgrade level: {(this.FarmHouse as FarmHouse).upgradeLevel}", LogLevel.Warn);
                    break;
            }

            return newPosition;
        }

        /// <summary>
        /// Checks if an Action button was pressed.
        /// </summary>
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
