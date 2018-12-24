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
            "Preserves Jar",
            "Keg",
            "Oil Maker",
            "Loom",
            "Mayonnaise Machine",
            "Cheese Press",
            "Crystalarium"
        };

        private Building _currentTileBuilding = null;

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
                    _currentTileBuilding = buildableLocation.getBuildingAt(Game1.currentCursorTile);
                }
                else
                {
                    _currentTileBuilding = null;
                }
            }
        }

        private void DrawHoverTooltip(object sender, RenderingHudEventArgs e)
        {
            //If the building is a shed or barn, show a tooltip with info about the containers inside. Make sure no menu is showing.
            if (_currentTileBuilding != null && Game1.activeClickableMenu == null)
            {
                if (_supportedBuildingTypes.Any(b => _currentTileBuilding.buildingType.Contains(b)) && _currentTileBuilding.indoors.Value != null)
                {
                    //Get info about building contents
                    var buildingInfo = new BuildingContentsInfo(_currentTileBuilding, _supportedContainerTypes);

                    //Building instructions depending on proximity to building and if player is holding an item.
                    var instructions = GetToolTipInstructions(buildingInfo);
                                       
                    //Build and display tooltip
                    string tooltip = string.Format("{0} total containers{1}{2} ready to harvest{1}{3} ready to load{4}", buildingInfo.NumberOfContainers, Environment.NewLine, buildingInfo.NumberReadyToHarvest, buildingInfo.NumberReadyToLoad, instructions);
                    IClickableMenu.drawHoverText(Game1.spriteBatch, tooltip, Game1.smallFont);
                }
            }
        }

        private string GetToolTipInstructions(BuildingContentsInfo buildingInfo)
        {
            string instructions = ""; 

            //Let's see if player is close enough to harvest/load
            var tileLocation = GetCursorTileLocation();
            var isPlayerClose = StardewValley.Utility.tileWithinRadiusOfPlayer((int)tileLocation.X, (int)tileLocation.Y, 4, Game1.player);
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
                if (Context.IsWorldReady && Game1.didPlayerJustLeftClick() && Game1.currentLocation is Farm farm && Game1.activeClickableMenu == null)
                {
                    Common.Utility.Log($"{DateTime.Now} - {Game1.player.Name} left clicked on something on the farm.");

                    if (DidPlayerClickOnASupportedBuilding())
                    {
                        Helper.Input.Suppress(e.Button);
                        Common.Utility.Log($"{DateTime.Now} - {Game1.player.Name} clicked on a supported building to harvest all supported containers within it.");
                        HarvestAllItemsInBuilding(_currentTileBuilding);
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
            if (_currentTileBuilding != null)
            {
                Common.Utility.Log($"{DateTime.Now} - {Game1.player.Name} clicked on a building.");

                //Need to get tile location so we can tell if the building is close enough to the player for an action.
                var tileLocation = GetCursorTileLocation();

                //Is the building close to the player and one of the supported types?
                if (_supportedBuildingTypes.Any(b => _currentTileBuilding.buildingType.Contains(b)) && _currentTileBuilding.indoors.Value != null && StardewValley.Utility.tileWithinRadiusOfPlayer((int)tileLocation.X, (int)tileLocation.Y, 4, Game1.player))
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

        private void HarvestAllItemsInBuilding(Building building)
        {
            var buildingInfo = new BuildingContentsInfo(building, _supportedContainerTypes);

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
            catch (InventoryFullException)
            {
                Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
            }
            catch (Exception ex)
            {
                Common.Utility.Log($"Exception while harvesting contents in BitwiseJonMods.OneClickShedReloader.HarvestAllItemsInBuilding: {ex.Message}", LogLevel.Error);
            }

            if (numItemsHarvested > 0)
            {
                Game1.playSound("coin");
                Game1.showGlobalMessage(string.Format("Harvested {0} {1}!", numItemsHarvested, numItemsHarvested == 1 ? "item" : "items"));
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
