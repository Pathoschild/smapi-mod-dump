/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/bitwisejon/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;


/// <summary>
/// 11/14/20: Adding support for cellars and casks. Added code to detect hover over on house. 
/// Add support for cabins with cellars - hit rectangle will be different - only owner of cabin can do it
/// Add support for greenhouse
/// Add support for autograbber - can I make this also grab all truffles on farm? Activate through config.
/// Add language support?
/// 
/// NOTE: Instead of keeping track of building, use building.indoors since this is a GameLocation as is the cellar - then use indoors/objects to get stuff
/// The greenhouse is also a game location so it should have objects as well without using indoors.
///   
/// 
/// </summary>
namespace BitwiseJonMods
{
    public class ModEntry : Mod
    {
        private List<string> _supportedBuildingTypes = new List<string>() {
            "Shed",
            "Barn",
            "Coop"
        };

        private List<string> _supportedContainerTypes = new List<string>() {
            "Bee House",
            "Cask",
            "Charcoal Kiln",
            "Cheese Press",
            "Crystalarium",
            "Furnace",
            "Keg",
            "Loom",
            "Mayonnaise Machine",
            "Mushroom Box",
            "Oil Maker",
            "Preserves Jar",
            "Recycling Machine",
            "Seed Maker",
            "Auto-Grabber"
        };

        private GameLocation _currentTileLocation = null;

        public override void Entry(IModHelper helper)
        {
            Common.Utility.InitLogging(this.Monitor);

            Helper.Events.Input.ButtonPressed += Input_ButtonPressed;
            Helper.Events.Display.RenderingHud += DrawHoverTooltip;
            Helper.Events.GameLoop.UpdateTicked += GetTileUnderCursor;
        }

        private void GetTileUnderCursor(object sender, UpdateTickedEventArgs e)
        {
            //Simulate previous SMAPI event GameEvents.FourthUpdateTick
            if (e.IsMultipleOf(4))
            {
                //See if we have a building under the cursor
                if (Game1.currentLocation is BuildableGameLocation buildableLocation)
                {
                    var building = buildableLocation.getBuildingAt(Game1.currentCursorTile);

                    if (building != null && _supportedBuildingTypes.Any(b => building.buildingType.Contains(b)))
                    {
                        _currentTileLocation = building.indoors.Value;
                        return;
                    }
                }

                //See if we have the farmhouse or a cabin under the cursor that has a cellar
                var cellar = GetCellarForFarmHouseUnderCursor(Game1.currentCursorTile);
                if (cellar != null)
                {
                    //Common.Utility.Log($"{DateTime.Now.Ticks} House/Cabin under cursor has a cellar!");
                    _currentTileLocation = cellar;
                    return;
                }

                //See if we have the greenhouse under the cursor
                var greenhouse = GetGreenHouseUnderCursor(Game1.currentCursorTile);
                if (greenhouse != null)
                {
                    //Common.Utility.Log($"{DateTime.Now.Ticks} Greenhouse under cursor!");
                    _currentTileLocation = greenhouse;
                    return;
                }

                //See if we have the farm cave under the cursor
                var cave = GetFarmCaveUnderCursor(Game1.currentCursorTile);
                if (cave != null)
                {
                    //Common.Utility.Log($"{DateTime.Now.Ticks} Cave under cursor!");
                    _currentTileLocation = cave;
                    return;
                }


                //If we made it here, user is not hovering over supported building so set to null to hide tooltip
                _currentTileLocation = null;
            }
        }

        //jon, 11/21/20: Due to a current limitation with SMAPI, locations outside of the farm, farmhouse, and farm buildings are not synced to
        //  other players. Therefore, we cannot support harvesting the cellar unless the player is the main player.
        private Cellar GetCellarForFarmHouseUnderCursor(Vector2 cursorTile)
        {
            Cellar result = null;
            Rectangle? hitRectangle = null;

            //Player's cabin/house must have a cellar (upgrade level 3).
            if (Game1.currentLocation == null || Game1.player.houseUpgradeLevel < 3) return null;

            var homeOfFarmer = Utility.getHomeOfFarmer(Game1.player);
            if (homeOfFarmer == null) return null;

            if (Game1.currentLocation.IsFarm && Game1.currentLocation.IsOutdoors && Context.IsMainPlayer)
            {
                //Main player only can do this from the outside of the building

                //Get house/cabin rectangle by finding the porch standing spot and creating house/cabin-size rectangle above it
                var porchSpot = homeOfFarmer.getPorchStandingSpot();
                hitRectangle = (homeOfFarmer is Cabin) ? new Rectangle(porchSpot.X - 1, porchSpot.Y - 3, 5, 3) : new Rectangle(porchSpot.X - 7, porchSpot.Y - 4, 9, 4);

                var isHit = isPointInRectangle(Utility.Vector2ToPoint(cursorTile), hitRectangle.Value);
                if (isHit)
                {
                    result = Game1.getLocationFromName(homeOfFarmer.GetCellarName()) as Cellar;
                }
            }
            else if (Game1.currentLocation.Name == homeOfFarmer.GetCellarName())
            {
                //Other players have to actually be inside the location for the game to sync the contents
                var cellar = Game1.currentLocation as Cellar;
                var mainDoorPoint = new Point(cellar.warps[0].X, cellar.warps[0].Y);
                hitRectangle = new Rectangle(mainDoorPoint.X - 2, mainDoorPoint.Y, 6, 3);

                var isHit = isPointInRectangle(Utility.Vector2ToPoint(cursorTile), hitRectangle.Value);
                if (isHit)
                {
                    result = cellar;
                }
            }

            return result;
        }

        //jon, 11/21/20: Due to a current limitation with SMAPI, locations outside of the farm, farmhouse, and farm buildings are not synced to
        //  other players. Therefore, we cannot support harvesting the greenhouse unless the player is the main player.
        private GameLocation GetGreenHouseUnderCursor(Vector2 cursorTile)
        {
            GameLocation result = null;
            Rectangle? hitRectangle = null;

            if (Game1.currentLocation == null) return null;

            //Only consistent way to tell if player has greenhouse is to check if main player has or will receive pantry mail.
            var greenhouse = Game1.getLocationFromName("GreenHouse");
            if (greenhouse == null || greenhouse.warps == null || greenhouse.warps.Count() == 0 || (!Game1.MasterPlayer.hasOrWillReceiveMail("jojaPantry") && !Game1.MasterPlayer.hasOrWillReceiveMail("ccPantry"))) return null;

            if (Game1.currentLocation.IsFarm && Game1.currentLocation.IsOutdoors && Context.IsMainPlayer)
            {
                //Only main player can load from outside
                var mainDoorPoint = new Point(greenhouse.warps[0].TargetX, greenhouse.warps[0].TargetY);
                hitRectangle = new Rectangle(mainDoorPoint.X - 3, mainDoorPoint.Y - 6, 7, 6);

                var isHit = isPointInRectangle(Utility.Vector2ToPoint(cursorTile), hitRectangle.Value);
                if (isHit)
                {
                    result = greenhouse;
                }
            }
            else if (Game1.currentLocation.Name == greenhouse.Name)
            {
                //Other players have to actually be inside the location for the game to sync the contents
                var mainDoorPoint = new Point(greenhouse.warps[0].X, greenhouse.warps[0].Y);
                hitRectangle = new Rectangle(mainDoorPoint.X - 2, mainDoorPoint.Y - 1, 5, 4);

                var isHit = isPointInRectangle(Utility.Vector2ToPoint(cursorTile), hitRectangle.Value);
                if (isHit)
                {
                    //Only the "currentLocation" object is synced with main game
                    result = Game1.currentLocation;
                }
            }

            return result;
        }

        //jon, 11/21/20: Due to a current limitation with SMAPI, locations outside of the farm, farmhouse, and farm buildings are not synced to
        //  other players. Therefore, we cannot support harvesting the cave unless the player is the main player.
        private GameLocation GetFarmCaveUnderCursor(Vector2 cursorTile)
        {
            GameLocation result = null;
            Rectangle? hitRectangle = null;

            if (Game1.currentLocation == null) return null;

            var farmcave = Game1.getLocationFromName("FarmCave") as FarmCave;
            if (farmcave == null || farmcave.warps == null || farmcave.warps.Count() == 0) return null;

            if (Game1.currentLocation.IsFarm && Game1.currentLocation.IsOutdoors && Context.IsMainPlayer)
            {
                //Only main player can load from outside
                var mainDoorPoint = new Point(farmcave.warps[0].TargetX, farmcave.warps[0].TargetY);
                hitRectangle = hitRectangle = new Rectangle(mainDoorPoint.X - 1, mainDoorPoint.Y - 1, 3, 3);

                var isHit = isPointInRectangle(Utility.Vector2ToPoint(cursorTile), hitRectangle.Value);
                if (isHit)
                {
                    result = farmcave;
                }
            }
            else if (Game1.currentLocation.Name == farmcave.Name)
            {
                //Other players have to actually be inside the location for the game to sync the contents
                var mainDoorPoint = new Point(farmcave.warps[0].X, farmcave.warps[0].Y);
                hitRectangle = new Rectangle(mainDoorPoint.X - 2, mainDoorPoint.Y - 1, 5, 4);

                var isHit = isPointInRectangle(Utility.Vector2ToPoint(cursorTile), hitRectangle.Value);
                if (isHit)
                {
                    //Only the "currentLocation" object is synced with main game
                    result = Game1.currentLocation;
                }
            }

            return result;
        }

        private bool isPointInRectangle(Point cursorTilePoint, Rectangle hitRect)
        {
            if (cursorTilePoint.X >= hitRect.X && cursorTilePoint.X < hitRect.X + hitRect.Width && cursorTilePoint.Y >= hitRect.Y && cursorTilePoint.Y < hitRect.Y + hitRect.Height)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        private void DrawHoverTooltip(object sender, RenderingHudEventArgs e)
        {
            //If the building is a supported building, show a tooltip with info about the containers inside. Make sure no menu is showing.
            if (_currentTileLocation != null && Game1.activeClickableMenu == null)
            {
                //Get info about location contents
                var buildingInfo = new BuildingContentsInfo(_currentTileLocation, _supportedContainerTypes);

                //Building instructions depending on proximity to building and if player is holding an item.
                var instructions = GetToolTipInstructions(buildingInfo);

                //Build and display tooltip
                string readyToHarvestTip = buildingInfo.NumberReadyToHarvest != buildingInfo.NumberOfItems ? string.Format("{0} ready to harvest with {1} total {2}", buildingInfo.NumberReadyToHarvest, buildingInfo.NumberOfItems, buildingInfo.NumberOfItems == 1 ? "item" : "items") : string.Format("{0} ready to harvest", buildingInfo.NumberReadyToHarvest);
                string tooltip = string.Format("{0} total containers{1}{2}{1}{3} ready to load{4}", buildingInfo.NumberOfContainers, Environment.NewLine, readyToHarvestTip, buildingInfo.NumberReadyToLoad, instructions);
                IClickableMenu.drawHoverText(Game1.spriteBatch, tooltip, Game1.smallFont);
            }
        }

        private string GetToolTipInstructions(BuildingContentsInfo buildingInfo)
        {
            string instructions = "";

            //Let's see if player is close enough to harvest/load
            var tileLocation = GetCursorTileLocation();
            var isPlayerClose = Utility.tileWithinRadiusOfPlayer((int)tileLocation.X, (int)tileLocation.Y, 4, Game1.player);
            var isPlayerHoldingItem = (Game1.player.ActiveObject != null);
            var anyItemsToHarvest = buildingInfo.NumberReadyToHarvest > 0;
            var anyItemsToLoad = buildingInfo.NumberReadyToLoad > 0;

            //Only show instructions if there is anything to load or harvest.
            if (anyItemsToHarvest || anyItemsToLoad)
            {
                if (!isPlayerClose)
                {
                    if (anyItemsToHarvest && anyItemsToLoad && isPlayerHoldingItem) instructions = "Move closer to harvest and load items.";
                    else if (anyItemsToLoad && isPlayerHoldingItem) instructions = "Move closer to load items.";
                    else if (anyItemsToHarvest) instructions = "Move closer to harvest items.";
                    else if (anyItemsToLoad && !isPlayerHoldingItem) instructions = "Move closer and hold an item to load.";
                }
                else
                {
                    if (anyItemsToHarvest && anyItemsToLoad && isPlayerHoldingItem) instructions = "Left click to harvest and load items.";
                    else if (anyItemsToLoad && isPlayerHoldingItem) instructions = "Left click to load items.";
                    else if (anyItemsToHarvest) instructions = "Left click to harvest items.";
                    else if (anyItemsToLoad && !isPlayerHoldingItem) instructions = "Hold an item and click to load.";
                }

                instructions = string.Format("{0}{0}{1}", Environment.NewLine, instructions);
            }

            return instructions;
        }

        private void Input_ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                //If world is ready (save is loaded) and player left clicked on a supported building, harvest and reload items in that building. Make sure menu is not showing.
                if (Context.IsWorldReady && Game1.didPlayerJustLeftClick() && _currentTileLocation != null && Game1.activeClickableMenu == null)
                {

                    if (DidPlayerClickOnASupportedBuilding())
                    {
                        Helper.Input.Suppress(e.Button);
                        Common.Utility.Log($"{DateTime.Now} - {Game1.player.Name} clicked on a supported building to harvest all supported containers within it.");
                        HarvestAllItemsInBuilding(_currentTileLocation);
                    }
                }
            }
            catch (Exception ex)
            {
                Common.Utility.Log($"Exception in BitwiseJonMods.OneClickShedReloader mod: {ex.Message}", LogLevel.Error);
            }
        }

        private bool DidPlayerClickOnASupportedBuilding()
        {
            bool result = false;

            //Only supporting buildings on the farm and on left click since right-click sometimes asks if user wants to eat the item they are holding.
            //jon, 11/14/20: _currentTileLocation is only set now if building is in supported list (or is house/cellar)
            if (_currentTileLocation != null)
            {
                Common.Utility.Log($"{DateTime.Now} - {Game1.player.Name} clicked on a building.");

                //Need to get tile location so we can tell if the building is close enough to the player for an action.
                var tileLocation = GetCursorTileLocation();

                //Is the building close to the player?
                if (Utility.tileWithinRadiusOfPlayer((int)tileLocation.X, (int)tileLocation.Y, 4, Game1.player))
                {
                    result = true;
                }
            }

            return result;
        }

        private Vector2 GetCursorTileLocation()
        {
            return new Vector2((int)(Game1.getOldMouseX() + Game1.viewport.X) / Game1.tileSize, (int)(Game1.getOldMouseY() + Game1.viewport.Y) / Game1.tileSize);
        }

        private void HarvestAllItemsInBuilding(GameLocation location)
        {
            var buildingInfo = new BuildingContentsInfo(location, _supportedContainerTypes);

            Common.Utility.Log($"  {buildingInfo.Containers.Count()} containers found in building.");
            Common.Utility.Log($"  Of these containers, {buildingInfo.ReadyToHarvestContainers.Count()} are ready for harvest.");
            Common.Utility.Log($"  Of these containers, {buildingInfo.ReadyToLoadContainers.Count()} are empty and ready to be loaded.");

            var buildingHandler = new BuildingContentsHandler(buildingInfo);
            int numItemsHarvested = 0;
            int numItemsLoaded = 0;


            //Harvest all containers in building that are ready and place contents in player's inventory.
            try
            {
                numItemsHarvested = buildingHandler.HarvestContents(Game1.player);
            }
            catch (InventoryFullException ife)
            {
                numItemsHarvested = ife.NumItemsHarvestedBeforeFull;
                Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
            }
            catch (Exception ex)
            {
                Common.Utility.Log($"Exception while harvesting contents in BitwiseJonMods.OneClickShedReloader.HarvestAllItemsInBuilding: {ex.Message}", LogLevel.Error);
            }

            if (numItemsHarvested > 0)
            {
                var numContainersHarvested = buildingInfo.Containers.Count();
                Game1.playSound("coin");
                Game1.showGlobalMessage(string.Format("Harvested {0} {1} from {2} {3}!", numItemsHarvested, numItemsHarvested == 1 ? "item" : "items", numContainersHarvested, numContainersHarvested == 1 ? "container" : "containers"));
            }
            else
            {
                Game1.playSound("trashcan");
                Game1.showGlobalMessage("No items harvested.");
            }


            //If player is holding an object that can go into a container in the building, load as many as possible into any empty containers in building.
            try
            {
                numItemsLoaded = buildingHandler.LoadContents(Game1.player);
            }
            catch (Exception ex)
            {
                Common.Utility.Log($"Exception while loading contents in BitwiseJonMods.OneClickShedReloader.HarvestAllItemsInBuilding: {ex.Message}", LogLevel.Error);
            }

            if (numItemsLoaded > 0)
            {
                Game1.playSound("Ship");
                Game1.showGlobalMessage(string.Format("Loaded {0} {1}!", numItemsLoaded, numItemsLoaded == 1 ? "item" : "items"));
            }
            else
            {
                Game1.playSound("trashcan");
                Game1.showGlobalMessage("No items loaded.");
            }
        }

    }
}
