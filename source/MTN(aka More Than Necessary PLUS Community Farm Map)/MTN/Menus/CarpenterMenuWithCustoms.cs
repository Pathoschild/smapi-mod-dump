using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Netcode;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;
using xTile.Dimensions;
using XNARectangle = Microsoft.Xna.Framework.Rectangle;

namespace MTN.Menus
{
    /// <summary>
    /// This class is almost an identical replica of CarpenterMenu, except we will be adding the ability to target
    /// which farm map, if multiple exists. In addition, it will read for a different source for cabin limitations, rather
    /// than just a static 3.
    /// </summary>
    public class CarpenterMenuWithCustoms : CarpenterMenu
    {

        private List<Item> ingredients = new List<Item>();
        private bool drawBG = true;
        private string hoverText = "";

        private List<BluePrint> blueprints;
        private int currentBlueprintIndex;

        public ClickableTextureComponent farmSelectForwardButton;
        public ClickableTextureComponent farmSelectBackButton;

        private Building currentBuilding;
        private Building buildingToMove;
        private string buildingDescription;
        private string buildingName;
        private int price;
        private bool onFarm;
        private bool freeze;
        private bool upgrading;
        private bool demolishing;
        private bool moving;
        private bool magicalConstruction;

        private int currentFarmIndex;
        private string currentFarmDisplay;
        BuildableGameLocation currentFarmMap;

        public new BluePrint CurrentBlueprint {
            get {
                return this.blueprints[this.currentBlueprintIndex];
            }
        }

        public CarpenterMenuWithCustoms(bool magicalConstruction = false) {
            this.magicalConstruction = magicalConstruction;
            Game1.player.forceCanMove();
            this.resetBounds();
            this.blueprints = new List<BluePrint>();
            if (magicalConstruction) {
                this.blueprints.Add(new BluePrint("Junimo Hut"));
                this.blueprints.Add(new BluePrint("Earth Obelisk"));
                this.blueprints.Add(new BluePrint("Water Obelisk"));
                this.blueprints.Add(new BluePrint("Gold Clock"));
            } else {
                this.blueprints.Add(new BluePrint("Coop"));
                this.blueprints.Add(new BluePrint("Barn"));
                this.blueprints.Add(new BluePrint("Well"));
                this.blueprints.Add(new BluePrint("Silo"));
                this.blueprints.Add(new BluePrint("Mill"));
                this.blueprints.Add(new BluePrint("Shed"));
                int buildingsConstructed = Game1.getFarm().getNumberBuildingsConstructed("Cabin");
                //Will need Harmony. Ugh
                //if (Game1.IsMasterGame && buildingsConstructed < Game1.multiplayer.playerLimit - 1)
                if (Game1.IsMasterGame) {
                    this.blueprints.Add(new BluePrint("Stone Cabin"));
                    this.blueprints.Add(new BluePrint("Plank Cabin"));
                    this.blueprints.Add(new BluePrint("Log Cabin"));
                }
                if (Game1.getFarm().getNumberBuildingsConstructed("Stable") < buildingsConstructed + 1)
                    this.blueprints.Add(new BluePrint("Stable"));
                this.blueprints.Add(new BluePrint("Slime Hutch"));
                if (Game1.getFarm().isBuildingConstructed("Coop"))
                    this.blueprints.Add(new BluePrint("Big Coop"));
                if (Game1.getFarm().isBuildingConstructed("Big Coop"))
                    this.blueprints.Add(new BluePrint("Deluxe Coop"));
                if (Game1.getFarm().isBuildingConstructed("Barn"))
                    this.blueprints.Add(new BluePrint("Big Barn"));
                if (Game1.getFarm().isBuildingConstructed("Big Barn"))
                    this.blueprints.Add(new BluePrint("Deluxe Barn"));
                this.blueprints.Add(new BluePrint("Shipping Bin"));
            }
            currentFarmIndex = 0;
            this.setNewActiveBlueprint();
            setNewActiveFarmMap();
            if (!Game1.options.SnappyMenus)
                return;
            this.populateClickableComponentList();
            this.snapToDefaultClickableComponent();
        }

        public override void snapToDefaultClickableComponent() {
            this.currentlySnappedComponent = this.getComponentWithID(107);
            this.snapCursorToCurrentSnappedComponent();
        }

        private void resetBounds() {
            this.xPositionOnScreen = Game1.viewport.Width / 2 - this.maxWidthOfBuildingViewer - IClickableMenu.spaceToClearSideBorder;
            this.yPositionOnScreen = Game1.viewport.Height / 2 - this.maxHeightOfBuildingViewer / 2 - IClickableMenu.spaceToClearTopBorder + 32;
            this.width = this.maxWidthOfBuildingViewer + this.maxWidthOfDescription + IClickableMenu.spaceToClearSideBorder * 2 + 64;
            this.height = this.maxHeightOfBuildingViewer + IClickableMenu.spaceToClearTopBorder;
            this.initialize(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, true);
            ClickableTextureComponent textureComponent1 = new ClickableTextureComponent("OK", new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen + this.width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - 192 - 12, this.yPositionOnScreen + this.maxHeightOfBuildingViewer + 64, 64, 64), (string)null, (string)null, Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(366, 373, 16, 16), 4f, false);
            textureComponent1.myID = 106;
            textureComponent1.rightNeighborID = 104;
            textureComponent1.leftNeighborID = 105;
            this.okButton = textureComponent1;
            ClickableTextureComponent textureComponent2 = new ClickableTextureComponent("OK", new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen + this.width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - 64, this.yPositionOnScreen + this.maxHeightOfBuildingViewer + 64, 64, 64), (string)null, (string)null, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 47, -1, -1), 1f, false);
            textureComponent2.myID = 107;
            textureComponent2.leftNeighborID = 104;
            this.cancelButton = textureComponent2;
            ClickableTextureComponent textureComponent3 = new ClickableTextureComponent(new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen + 64, this.yPositionOnScreen + this.maxHeightOfBuildingViewer + 64, 48, 44), Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(352, 495, 12, 11), 4f, false);
            textureComponent3.myID = 101;
            textureComponent3.rightNeighborID = 102;
            this.backButton = textureComponent3;
            ClickableTextureComponent textureComponent4 = new ClickableTextureComponent(new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen + this.maxWidthOfBuildingViewer - 256 + 16, this.yPositionOnScreen + this.maxHeightOfBuildingViewer + 64, 48, 44), Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(365, 495, 12, 11), 4f, false);
            textureComponent4.myID = 102;
            textureComponent4.leftNeighborID = 101;
            textureComponent4.rightNeighborID = 105;
            this.forwardButton = textureComponent4;
            ClickableTextureComponent textureComponent5 = new ClickableTextureComponent(Game1.content.LoadString("Strings\\UI:Carpenter_Demolish"), new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen + this.width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - 128 - 8, this.yPositionOnScreen + this.maxHeightOfBuildingViewer + 64 - 4, 64, 64), (string)null, (string)null, Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(348, 372, 17, 17), 4f, false);
            textureComponent5.myID = 104;
            textureComponent5.rightNeighborID = 107;
            textureComponent5.leftNeighborID = 106;
            this.demolishButton = textureComponent5;
            ClickableTextureComponent textureComponent6 = new ClickableTextureComponent(new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen + this.maxWidthOfBuildingViewer - 128 + 32, this.yPositionOnScreen + 8, 36, 52), Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(402, 328, 9, 13), 4f, false);
            textureComponent6.myID = 103;
            textureComponent6.rightNeighborID = 104;
            textureComponent6.leftNeighborID = 105;
            this.upgradeIcon = textureComponent6;
            ClickableTextureComponent textureComponent7 = new ClickableTextureComponent(Game1.content.LoadString("Strings\\UI:Carpenter_MoveBuildings"), new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen + this.width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - 256 - 20, this.yPositionOnScreen + this.maxHeightOfBuildingViewer + 64, 64, 64), (string)null, (string)null, Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(257, 284, 16, 16), 4f, false);
            textureComponent7.myID = 105;
            textureComponent7.rightNeighborID = 106;
            textureComponent7.leftNeighborID = 102;
            this.moveButton = textureComponent7;

            farmSelectForwardButton = new ClickableTextureComponent(new XNARectangle(xPositionOnScreen + maxWidthOfBuildingViewer - 80, yPositionOnScreen - 64, 48, 44), Game1.mouseCursors, new XNARectangle(365, 495, 12, 11), 4f, false);

            farmSelectBackButton = new ClickableTextureComponent(new XNARectangle(xPositionOnScreen - 64 - 24, yPositionOnScreen - 64, 48, 44), Game1.mouseCursors, new XNARectangle(352, 495, 12, 11), 4f, false);

            this.demolishButton.visible = (int)Game1.multiplayerMode == 0;
            this.moveButton.visible = (int)Game1.multiplayerMode == 0;
            if (!this.demolishButton.visible) {
                this.upgradeIcon.rightNeighborID = this.demolishButton.rightNeighborID;
                this.okButton.rightNeighborID = this.demolishButton.rightNeighborID;
                this.cancelButton.leftNeighborID = this.demolishButton.leftNeighborID;
            }
            if (this.moveButton.visible)
                return;
            this.upgradeIcon.leftNeighborID = this.moveButton.leftNeighborID;
            this.forwardButton.rightNeighborID = this.moveButton.rightNeighborID;
            this.okButton.leftNeighborID = this.moveButton.leftNeighborID;
        }

        public new void setNewActiveBlueprint() {
            this.currentBuilding = !this.blueprints[this.currentBlueprintIndex].name.Contains("Coop") ? (!this.blueprints[this.currentBlueprintIndex].name.Contains("Barn") ? (!this.blueprints[this.currentBlueprintIndex].name.Contains("Mill") ? (!this.blueprints[this.currentBlueprintIndex].name.Contains("Junimo Hut") ? (!this.blueprints[this.currentBlueprintIndex].name.Contains("Shipping Bin") ? new Building(this.blueprints[this.currentBlueprintIndex], Vector2.Zero) : (Building)new ShippingBin(this.blueprints[this.currentBlueprintIndex], Vector2.Zero)) : (Building)new JunimoHut(this.blueprints[this.currentBlueprintIndex], Vector2.Zero)) : (Building)new Mill(this.blueprints[this.currentBlueprintIndex], Vector2.Zero)) : (Building)new Barn(this.blueprints[this.currentBlueprintIndex], Vector2.Zero)) : (Building)new Coop(this.blueprints[this.currentBlueprintIndex], Vector2.Zero);
            this.price = this.blueprints[this.currentBlueprintIndex].moneyRequired;
            this.ingredients.Clear();
            foreach (KeyValuePair<int, int> keyValuePair in this.blueprints[this.currentBlueprintIndex].itemsRequired)
                this.ingredients.Add((Item)new StardewValley.Object(keyValuePair.Key, keyValuePair.Value, false, -1, 0));
            this.buildingDescription = this.blueprints[this.currentBlueprintIndex].description;
            this.buildingName = this.blueprints[this.currentBlueprintIndex].displayName;
        }

        //
        public void setNewActiveFarmMap() {
            if (currentFarmIndex < 0) {
                currentFarmIndex = Memory.farmMaps.Count - 1;
            } else if (currentFarmIndex > Memory.farmMaps.Count - 1) {
                currentFarmIndex = 0;
            }

            currentFarmDisplay = Memory.farmMaps[currentFarmIndex].displayName;
            currentFarmMap = (BuildableGameLocation)Game1.getLocationFromName(Memory.farmMaps[currentFarmIndex].Location);
        }
        //

        public override void performHoverAction(int x, int y) {
            this.cancelButton.tryHover(x, y, 0.1f);
            if (this.upperRightCloseButton != null) {
                this.upperRightCloseButton.tryHover(x, y, 0.5f);
            }
            if (!this.onFarm) {
                this.backButton.tryHover(x, y, 1f);
                this.forwardButton.tryHover(x, y, 1f);
                this.okButton.tryHover(x, y, 0.1f);
                this.demolishButton.tryHover(x, y, 0.1f);
                this.moveButton.tryHover(x, y, 0.1f);
                if (this.CurrentBlueprint.isUpgrade() && this.upgradeIcon.containsPoint(x, y))
                    this.hoverText = Game1.content.LoadString("Strings\\UI:Carpenter_Upgrade", (object)new BluePrint(this.CurrentBlueprint.nameOfBuildingToUpgrade).displayName);
                else if (this.demolishButton.containsPoint(x, y))
                    this.hoverText = Game1.content.LoadString("Strings\\UI:Carpenter_Demolish");
                else if (this.moveButton.containsPoint(x, y))
                    this.hoverText = Game1.content.LoadString("Strings\\UI:Carpenter_MoveBuildings");
                else if (this.okButton.containsPoint(x, y) && this.CurrentBlueprint.doesFarmerHaveEnoughResourcesToBuild())
                    this.hoverText = Game1.content.LoadString("Strings\\UI:Carpenter_Build");
                else
                    this.hoverText = "";
            } else {
                if (!this.upgrading && !this.demolishing && !this.moving || this.freeze)
                    return;
                foreach (Building building in currentFarmMap.buildings)
                    building.color.Value = Color.White;
                Building building1 = currentFarmMap.getBuildingAt(new Vector2((float)((Game1.viewport.X + Game1.getOldMouseX()) / 64), (float)((Game1.viewport.Y + Game1.getOldMouseY()) / 64))) ?? currentFarmMap.getBuildingAt(new Vector2((float)((Game1.viewport.X + Game1.getOldMouseX()) / 64), (float)((Game1.viewport.Y + Game1.getOldMouseY() + 128) / 64))) ?? currentFarmMap.getBuildingAt(new Vector2((float)((Game1.viewport.X + Game1.getOldMouseX()) / 64), (float)((Game1.viewport.Y + Game1.getOldMouseY() + 192) / 64)));
                if (this.upgrading) {
                    if (building1 != null && this.CurrentBlueprint.nameOfBuildingToUpgrade != null && this.CurrentBlueprint.nameOfBuildingToUpgrade.Equals((string)((NetFieldBase<string, NetString>)building1.buildingType))) {
                        building1.color.Value = Color.Lime * 0.8f;
                    } else {
                        if (building1 == null)
                            return;
                        building1.color.Value = Color.Red * 0.8f;
                    }
                } else if (this.demolishing) {
                    if (building1 == null)
                        return;
                    building1.color.Value = Color.Red * 0.8f;
                } else {
                    if (!this.moving || building1 == null)
                        return;
                    building1.color.Value = Color.Lime * 0.8f;
                }
            }
        }

        public override void receiveGamePadButton(Buttons b) {
            //base.receiveGamePadButton(b);
            if (!this.onFarm && b == Buttons.LeftTrigger) {
                --this.currentBlueprintIndex;
                if (this.currentBlueprintIndex < 0)
                    this.currentBlueprintIndex = this.blueprints.Count - 1;
                this.setNewActiveBlueprint();
                Game1.playSound("shwip");
            }
            if (this.onFarm || b != Buttons.RightTrigger)
                return;
            this.currentBlueprintIndex = (this.currentBlueprintIndex + 1) % this.blueprints.Count;
            this.setNewActiveBlueprint();
            Game1.playSound("shwip");
        }

        public override void receiveKeyPress(Keys key) {
            if (this.freeze)
                return;
            if (!this.onFarm)
                //base.receiveKeyPress(key);
            if (Game1.globalFade || !this.onFarm)
                return;
            if (Game1.options.doesInputListContain(Game1.options.menuButton, key) && this.readyToClose()) {
                Game1.globalFadeToBlack(new Game1.afterFadeFunction(this.returnToCarpentryMenu), 0.02f);
            } else {
                if (Game1.options.SnappyMenus)
                    return;
                if (Game1.options.doesInputListContain(Game1.options.moveDownButton, key))
                    Game1.panScreen(0, 4);
                else if (Game1.options.doesInputListContain(Game1.options.moveRightButton, key))
                    Game1.panScreen(4, 0);
                else if (Game1.options.doesInputListContain(Game1.options.moveUpButton, key)) {
                    Game1.panScreen(0, -4);
                } else {
                    if (!Game1.options.doesInputListContain(Game1.options.moveLeftButton, key))
                        return;
                    Game1.panScreen(-4, 0);
                }
            }
        }

        public override void update(GameTime time) {
            //base.update(time);
            if (!this.onFarm || Game1.globalFade)
                return;
            int num1 = Game1.getOldMouseX() + Game1.viewport.X;
            int num2 = Game1.getOldMouseY() + Game1.viewport.Y;
            if (num1 - Game1.viewport.X < 64)
                Game1.panScreen(-8, 0);
            else if (num1 - (Game1.viewport.X + Game1.viewport.Width) >= (int)sbyte.MinValue)
                Game1.panScreen(8, 0);
            if (num2 - Game1.viewport.Y < 64)
                Game1.panScreen(0, -8);
            else if (num2 - (Game1.viewport.Y + Game1.viewport.Height) >= -64)
                Game1.panScreen(0, 8);
            foreach (Keys pressedKey in Game1.oldKBState.GetPressedKeys())
                this.receiveKeyPress(pressedKey);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true) {
            if (this.freeze)
                return;
            if (!this.onFarm) {
                //base.receiveLeftClick(x, y, playSound);
                if (this.upperRightCloseButton != null && this.readyToClose() && this.upperRightCloseButton.containsPoint(x, y)) {
                    if (playSound) {
                        Game1.playSound("bigDeSelect");
                    }
                    this.exitThisMenu(true);
                }
            }
            if (this.cancelButton.containsPoint(x, y)) {
                if (!this.onFarm) {
                    this.exitThisMenu(true);
                    Game1.player.forceCanMove();
                    Game1.playSound("bigDeSelect");
                } else {
                    if (this.moving && this.buildingToMove != null) {
                        Game1.playSound("cancel");
                        return;
                    }
                    Game1.globalFadeToBlack(new Game1.afterFadeFunction(this.returnToCarpentryMenu), 0.02f);
                    Game1.playSound("smallSelect");
                    return;
                }
            }
            if (!onFarm && farmSelectBackButton.containsPoint(x, y)) {
                currentFarmIndex--;
                setNewActiveFarmMap();
                Game1.playSound("shwip");
                farmSelectBackButton.scale = farmSelectBackButton.baseScale;
            }
            if (!onFarm && farmSelectForwardButton.containsPoint(x, y)) {
                currentFarmIndex++;
                setNewActiveFarmMap();
                Game1.playSound("shwip");
                farmSelectForwardButton.scale = farmSelectForwardButton.baseScale;
            }
            if (!this.onFarm && this.backButton.containsPoint(x, y)) {
                --this.currentBlueprintIndex;
                if (this.currentBlueprintIndex < 0)
                    this.currentBlueprintIndex = this.blueprints.Count - 1;
                this.setNewActiveBlueprint();
                Game1.playSound("shwip");
                this.backButton.scale = this.backButton.baseScale;
            }
            if (!this.onFarm && this.forwardButton.containsPoint(x, y)) {
                this.currentBlueprintIndex = (this.currentBlueprintIndex + 1) % this.blueprints.Count;
                this.setNewActiveBlueprint();
                this.backButton.scale = this.backButton.baseScale;
                Game1.playSound("shwip");
            }
            if (!this.onFarm && this.demolishButton.containsPoint(x, y) && this.demolishButton.visible) {
                Game1.globalFadeToBlack(new Game1.afterFadeFunction(this.setUpForBuildingPlacement), 0.02f);
                Game1.playSound("smallSelect");
                this.onFarm = true;
                this.demolishing = true;
            }
            if (!this.onFarm && this.moveButton.containsPoint(x, y) && this.moveButton.visible) {
                Game1.globalFadeToBlack(new Game1.afterFadeFunction(this.setUpForBuildingPlacement), 0.02f);
                Game1.playSound("smallSelect");
                this.onFarm = true;
                this.moving = true;
            }
            if (this.okButton.containsPoint(x, y) && !this.onFarm && (Game1.player.money >= this.price && this.blueprints[this.currentBlueprintIndex].doesFarmerHaveEnoughResourcesToBuild())) {
                Game1.globalFadeToBlack(new Game1.afterFadeFunction(this.setUpForBuildingPlacement), 0.02f);
                Game1.playSound("smallSelect");
                this.onFarm = true;
            }
            if (!this.onFarm || this.freeze || Game1.globalFade)
                return;
            if (this.demolishing) {
                Farm farm = currentFarmMap as Farm;
                Building destroyed = farm.getBuildingAt(new Vector2((float)((Game1.viewport.X + Game1.getOldMouseX()) / 64), (float)((Game1.viewport.Y + Game1.getOldMouseY()) / 64)));
                Chest chest = (Chest)null;
                if (destroyed != null && ((int)((NetFieldBase<int, NetInt>)destroyed.daysOfConstructionLeft) > 0 || (int)((NetFieldBase<int, NetInt>)destroyed.daysUntilUpgrade) > 0))
                    Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_DuringConstruction"), Color.Red, 3500f));
                else if (destroyed != null && destroyed.indoors.Value != null && (destroyed.indoors.Value is AnimalHouse && (destroyed.indoors.Value as AnimalHouse).animalsThatLiveHere.Count > 0)) {
                    Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_AnimalsHere"), Color.Red, 3500f));
                } else {
                    Action continueDemolish = (Action)(() => {
                        if (destroyed == null || !farm.destroyStructure(destroyed))
                            return;
                        int tileY = (int)((NetFieldBase<int, NetInt>)destroyed.tileY);
                        int tilesHigh = (int)((NetFieldBase<int, NetInt>)destroyed.tilesHigh);
                        Game1.flashAlpha = 1f;
                        destroyed.showDestroyedAnimation((GameLocation)Game1.getFarm());
                        Game1.playSound("explosion");
                        Utility.spreadAnimalsAround(destroyed, farm);
                        DelayedAction.fadeAfterDelay(new Game1.afterFadeFunction(this.returnToCarpentryMenu), 1500);
                        this.freeze = true;
                        if (chest == null)
                            return;
                        farm.objects[new Vector2((float)((int)((NetFieldBase<int, NetInt>)destroyed.tileX) + (int)((NetFieldBase<int, NetInt>)destroyed.tilesWide) / 2), (float)((int)((NetFieldBase<int, NetInt>)destroyed.tileY) + (int)((NetFieldBase<int, NetInt>)destroyed.tilesHigh) / 2))] = (StardewValley.Object)chest;
                    });
                    if (destroyed != null && destroyed.indoors.Value is Cabin) {
                        Cabin cabin = destroyed.indoors.Value as Cabin;
                        if (cabin.farmhand.Value != null && (bool)((NetFieldBase<bool, NetBool>)cabin.farmhand.Value.isCustomized)) {
                            if (cabin.farmhand.Value.isActive()) {
                                Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_FarmhandOnline"), Color.Red, 3500f));
                                return;
                            }
                            Game1.currentLocation.createQuestionDialogue(Game1.content.LoadString("Strings\\UI:Carpenter_DemolishCabinConfirm", (object)cabin.farmhand.Value.Name), Game1.currentLocation.createYesNoResponses(), (GameLocation.afterQuestionBehavior)((f, answer) => {
                                if (answer == "Yes") {
                                    List<Item> objList = cabin.demolish();
                                    if (objList.Count > 0) {
                                        chest = new Chest(true);
                                        chest.items.Set((IList<Item>)objList);
                                    }
                                    continueDemolish();
                                } else
                                    DelayedAction.fadeAfterDelay(new Game1.afterFadeFunction(this.returnToCarpentryMenu), 500);
                            }), (NPC)null);
                            return;
                        }
                    }
                    continueDemolish();
                }
            } else if (this.upgrading) {
                Building buildingAt = currentFarmMap.getBuildingAt(new Vector2((float)((Game1.viewport.X + Game1.getOldMouseX()) / 64), (float)((Game1.viewport.Y + Game1.getOldMouseY()) / 64)));
                if (buildingAt != null && this.CurrentBlueprint.name != null && buildingAt.buildingType.Equals((object)this.CurrentBlueprint.nameOfBuildingToUpgrade)) {
                    this.CurrentBlueprint.consumeResources();
                    buildingAt.daysUntilUpgrade.Value = 2;
                    buildingAt.showUpgradeAnimation((GameLocation)Game1.getFarm());
                    Game1.playSound("axe");
                    DelayedAction.fadeAfterDelay(new Game1.afterFadeFunction(this.returnToCarpentryMenuAfterSuccessfulBuild), 1500);
                    this.freeze = true;
                } else {
                    if (buildingAt == null)
                        return;
                    Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantUpgrade_BuildingType"), Color.Red, 3500f));
                }
            } else if (this.moving) {
                if (this.buildingToMove == null) {
                    this.buildingToMove = currentFarmMap.getBuildingAt(new Vector2((float)((Game1.viewport.X + Game1.getMouseX()) / 64), (float)((Game1.viewport.Y + Game1.getMouseY()) / 64)));
                    if (this.buildingToMove == null)
                        return;
                    if ((int)((NetFieldBase<int, NetInt>)this.buildingToMove.daysOfConstructionLeft) > 0) {
                        this.buildingToMove = (Building)null;
                    } else {
                        currentFarmMap.buildings.Remove(this.buildingToMove);
                        Game1.playSound("axchop");
                    }
                } else if (currentFarmMap.buildStructure(this.buildingToMove, new Vector2((float)((Game1.viewport.X + Game1.getMouseX()) / 64), (float)((Game1.viewport.Y + Game1.getMouseY()) / 64)), Game1.player, false)) {
                    if (this.buildingToMove is ShippingBin)
                        (this.buildingToMove as ShippingBin).initLid();
                    this.buildingToMove = (Building)null;
                    Game1.playSound("axchop");
                    DelayedAction.playSoundAfterDelay("dirtyHit", 50, (GameLocation)null);
                    DelayedAction.playSoundAfterDelay("dirtyHit", 150, (GameLocation)null);
                } else
                    Game1.playSound("cancel");
            } else if (this.tryToBuild()) {
                this.CurrentBlueprint.consumeResources();
                DelayedAction.fadeAfterDelay(new Game1.afterFadeFunction(this.returnToCarpentryMenuAfterSuccessfulBuild), 2000);
                this.freeze = true;
            } else
                Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantBuild"), Color.Red, 3500f));
        }

        public new bool tryToBuild() {
            return currentFarmMap.buildStructure(this.CurrentBlueprint, new Vector2((float)((Game1.viewport.X + Game1.getOldMouseX()) / 64), (float)((Game1.viewport.Y + Game1.getOldMouseY()) / 64)), Game1.player, this.magicalConstruction, false);
        }

        public new void returnToCarpentryMenu() {
            LocationRequest locationRequest = Game1.getLocationRequest(this.magicalConstruction ? "WizardHouse" : "ScienceHouse", false);
            locationRequest.OnWarp += (LocationRequest.Callback)(() => {
                onFarm = false;
                resetBounds();
                upgrading = false;
                moving = false;
                buildingToMove = (Building)null;
                freeze = false;
                Game1.displayHUD = true;
                Game1.viewportFreeze = false;
                Game1.viewport.Location = new Location(320, 1536);
                drawBG = true;
                demolishing = false;
                Game1.displayFarmer = true;
                if (!Game1.options.SnappyMenus)
                    return;
                populateClickableComponentList();
                snapToDefaultClickableComponent();
            });
            Game1.warpFarmer(locationRequest, Game1.player.getTileX(), Game1.player.getTileY(), (int)((NetFieldBase<int, NetInt>)Game1.player.facingDirection));
        }

        public new void returnToCarpentryMenuAfterSuccessfulBuild() {
            LocationRequest locationRequest = Game1.getLocationRequest(this.magicalConstruction ? "WizardHouse" : "ScienceHouse", false);
            locationRequest.OnWarp += (LocationRequest.Callback)(() => {
                Game1.displayHUD = true;
                Game1.viewportFreeze = false;
                Game1.viewport.Location = new Location(320, 1536);
                freeze = true;
                Game1.displayFarmer = true;
                this.robinConstructionMessage();
            });
            Game1.warpFarmer(locationRequest, Game1.player.getTileX(), Game1.player.getTileY(), (int)((NetFieldBase<int, NetInt>)Game1.player.facingDirection));
        }

        public new void robinConstructionMessage() {
            this.exitThisMenu(true);
            Game1.player.forceCanMove();
            if (this.magicalConstruction)
                return;
            string str1 = "Data\\ExtraDialogue:Robin_" + (this.upgrading ? "Upgrade" : "New") + "Construction";
            if (Utility.isFestivalDay(Game1.dayOfMonth + 1, Game1.currentSeason))
                str1 += "_Festival";
            if (this.CurrentBlueprint.daysToConstruct <= 0) {
                Game1.drawDialogue(Game1.getCharacterFromName("Robin", false), Game1.content.LoadString("Data\\ExtraDialogue:Robin_Instant", LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.de ? (object)this.CurrentBlueprint.displayName : (object)this.CurrentBlueprint.displayName.ToLower()));
            } else {
                NPC characterFromName = Game1.getCharacterFromName("Robin", false);
                LocalizedContentManager content = Game1.content;
                string path = str1;
                string str2 = LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.de ? this.CurrentBlueprint.displayName : this.CurrentBlueprint.displayName.ToLower();
                string str3;
                switch (LocalizedContentManager.CurrentLanguageCode) {
                    case LocalizedContentManager.LanguageCode.pt:
                    case LocalizedContentManager.LanguageCode.es:
                        str3 = ((IEnumerable<string>)this.CurrentBlueprint.displayName.ToLower().Split(' ')).First<string>();
                        break;
                    case LocalizedContentManager.LanguageCode.de:
                        str3 = ((IEnumerable<string>)((IEnumerable<string>)this.CurrentBlueprint.displayName.Split(' ')).Last<string>().Split('-')).Last<string>();
                        break;
                    default:
                        str3 = ((IEnumerable<string>)this.CurrentBlueprint.displayName.ToLower().Split(' ')).Last<string>();
                        break;
                }
                string dialogue = content.LoadString(path, (object)str2, (object)str3);
                Game1.drawDialogue(characterFromName, dialogue);
            }
        }

        public override bool overrideSnappyMenuCursorMovementBan() {
            return this.onFarm;
        }

        public new void setUpForBuildingPlacement() {
            Game1.currentLocation.cleanupBeforePlayerExit();
            this.hoverText = "";
            Game1.currentLocation = currentFarmMap;
            Game1.currentLocation.resetForPlayerEntry();
            Game1.globalFadeToClear((Game1.afterFadeFunction)null, 0.02f);
            this.onFarm = true;
            this.cancelButton.bounds.X = Game1.viewport.Width - 128;
            this.cancelButton.bounds.Y = Game1.viewport.Height - 128;
            Game1.displayHUD = false;
            Game1.viewportFreeze = true;
            Game1.viewport.Location = (Memory.isCustomFarmLoaded) ? new Location(Memory.loadedFarm.farmHousePorchX() * 64, Memory.loadedFarm.farmHousePorchY() * 64) : new Location(3136, 320);
            Game1.panScreen(0, 0);
            this.drawBG = false;
            this.freeze = false;
            Game1.displayFarmer = false;
            if (this.demolishing || this.CurrentBlueprint.nameOfBuildingToUpgrade == null || (this.CurrentBlueprint.nameOfBuildingToUpgrade.Length <= 0 || this.moving))
                return;
            this.upgrading = true;
        }

        public override void gameWindowSizeChanged(Microsoft.Xna.Framework.Rectangle oldBounds, Microsoft.Xna.Framework.Rectangle newBounds) {
            this.resetBounds();
        }

        public override void draw(SpriteBatch b) {
            if (this.drawBG)
                b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.5f);
            if (Game1.globalFade || this.freeze)
                return;
            if (!this.onFarm) {
                if (upperRightCloseButton != null) {
                    upperRightCloseButton.draw(b);
                }
                IClickableMenu.drawTextureBox(b, this.xPositionOnScreen - 96, this.yPositionOnScreen - 16, this.maxWidthOfBuildingViewer + 64, this.maxHeightOfBuildingViewer + 64, this.magicalConstruction ? Color.RoyalBlue : Color.White);
                this.currentBuilding.drawInMenu(b, this.xPositionOnScreen + this.maxWidthOfBuildingViewer / 2 - (int)((NetFieldBase<int, NetInt>)this.currentBuilding.tilesWide) * 64 / 2 - 64, this.yPositionOnScreen + this.maxHeightOfBuildingViewer / 2 - this.currentBuilding.getSourceRectForMenu().Height * 4 / 2);
                if (this.CurrentBlueprint.isUpgrade())
                    this.upgradeIcon.draw(b);
                string str = " Deluxe  Barn   ";
                SpriteText.drawStringWithScrollBackground(b, this.buildingName, this.xPositionOnScreen + this.maxWidthOfBuildingViewer - IClickableMenu.spaceToClearSideBorder - 16 + 64 + ((this.width - (this.maxWidthOfBuildingViewer + 128)) / 2 - SpriteText.getWidthOfString(str) / 2), this.yPositionOnScreen, str, 1f, -1);
                IClickableMenu.drawTextureBox(b, this.xPositionOnScreen + this.maxWidthOfBuildingViewer - 16, this.yPositionOnScreen + 80, this.maxWidthOfDescription + 64, this.maxWidthOfDescription + 96, this.magicalConstruction ? Color.RoyalBlue : Color.White);
                //
                string str2 = " Maps Maps  ";
                SpriteText.drawStringWithScrollBackground(b, currentFarmDisplay, xPositionOnScreen + spaceToClearSideBorder - 26 + ((width - (maxWidthOfBuildingViewer + 128)) / 2 - SpriteText.getWidthOfString(str2) / 2), yPositionOnScreen - 75, str2, 1f, -1);
                //
                if (this.magicalConstruction) {
                    Utility.drawTextWithShadow(b, Game1.parseText(this.buildingDescription, Game1.dialogueFont, this.maxWidthOfDescription + 32), Game1.dialogueFont, new Vector2((float)(this.xPositionOnScreen + this.maxWidthOfDescription + 64 - 4), (float)(this.yPositionOnScreen + 80 + 16 + 4)), Game1.textColor * 0.25f, 1f, -1f, -1, -1, 0.0f, 3);
                    Utility.drawTextWithShadow(b, Game1.parseText(this.buildingDescription, Game1.dialogueFont, this.maxWidthOfDescription + 32), Game1.dialogueFont, new Vector2((float)(this.xPositionOnScreen + this.maxWidthOfDescription + 64 - 1), (float)(this.yPositionOnScreen + 80 + 16 + 4)), Game1.textColor * 0.25f, 1f, -1f, -1, -1, 0.0f, 3);
                }
                Utility.drawTextWithShadow(b, Game1.parseText(this.buildingDescription, Game1.dialogueFont, this.maxWidthOfDescription + 32), Game1.dialogueFont, new Vector2((float)(this.xPositionOnScreen + this.maxWidthOfDescription + 64), (float)(this.yPositionOnScreen + 80 + 16)), this.magicalConstruction ? Color.PaleGoldenrod : Game1.textColor, 1f, -1f, -1, -1, this.magicalConstruction ? 0.0f : 0.25f, 3);
                Vector2 location = new Vector2((float)(this.xPositionOnScreen + this.maxWidthOfDescription + 16 + 64), (float)(this.yPositionOnScreen + 256 + 32));
                SpriteText.drawString(b, "$", (int)location.X, (int)location.Y, 999999, -1, 999999, 1f, 0.88f, false, -1, "", -1);
                if (this.magicalConstruction) {
                    Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\StringsFromCSFiles:LoadGameMenu.cs.11020", (object)this.price), Game1.dialogueFont, new Vector2(location.X + 64f, location.Y + 8f), Game1.textColor * 0.5f, 1f, -1f, -1, -1, this.magicalConstruction ? 0.0f : 0.25f, 3);
                    Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\StringsFromCSFiles:LoadGameMenu.cs.11020", (object)this.price), Game1.dialogueFont, new Vector2((float)((double)location.X + 64.0 + 4.0 - 1.0), location.Y + 8f), Game1.textColor * 0.25f, 1f, -1f, -1, -1, this.magicalConstruction ? 0.0f : 0.25f, 3);
                }
                Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\StringsFromCSFiles:LoadGameMenu.cs.11020", (object)this.price), Game1.dialogueFont, new Vector2((float)((double)location.X + 64.0 + 4.0), location.Y + 4f), Game1.player.money >= this.price ? (this.magicalConstruction ? Color.PaleGoldenrod : Game1.textColor) : Color.Red, 1f, -1f, -1, -1, this.magicalConstruction ? 0.0f : 0.25f, 3);
                location.X -= 16f;
                location.Y -= 21f;
                foreach (Item ingredient in this.ingredients) {
                    location.Y += 68f;
                    ingredient.drawInMenu(b, location, 1f);
                    bool flag = !(ingredient is StardewValley.Object) || Game1.player.hasItemInInventory((int)((NetFieldBase<int, NetInt>)(ingredient as StardewValley.Object).parentSheetIndex), ingredient.Stack, 0);
                    if (this.magicalConstruction) {
                        Utility.drawTextWithShadow(b, ingredient.DisplayName, Game1.dialogueFont, new Vector2((float)((double)location.X + 64.0 + 12.0), location.Y + 24f), Game1.textColor * 0.25f, 1f, -1f, -1, -1, this.magicalConstruction ? 0.0f : 0.25f, 3);
                        Utility.drawTextWithShadow(b, ingredient.DisplayName, Game1.dialogueFont, new Vector2((float)((double)location.X + 64.0 + 16.0 - 1.0), location.Y + 24f), Game1.textColor * 0.25f, 1f, -1f, -1, -1, this.magicalConstruction ? 0.0f : 0.25f, 3);
                    }
                    Utility.drawTextWithShadow(b, ingredient.DisplayName, Game1.dialogueFont, new Vector2((float)((double)location.X + 64.0 + 16.0), location.Y + 20f), flag ? (this.magicalConstruction ? Color.PaleGoldenrod : Game1.textColor) : Color.Red, 1f, -1f, -1, -1, this.magicalConstruction ? 0.0f : 0.25f, 3);
                }

                farmSelectBackButton.draw(b);
                farmSelectForwardButton.draw(b);
                this.backButton.draw(b);
                this.forwardButton.draw(b);
                this.okButton.draw(b, this.blueprints[this.currentBlueprintIndex].doesFarmerHaveEnoughResourcesToBuild() ? Color.White : Color.Gray * 0.8f, 0.88f);
                this.demolishButton.draw(b);
                this.moveButton.draw(b);
            } else {
                string s = this.upgrading ? Game1.content.LoadString("Strings\\UI:Carpenter_SelectBuilding_Upgrade", (object)new BluePrint(this.CurrentBlueprint.nameOfBuildingToUpgrade).displayName) : (this.demolishing ? Game1.content.LoadString("Strings\\UI:Carpenter_SelectBuilding_Demolish") : Game1.content.LoadString("Strings\\UI:Carpenter_ChooseLocation"));
                SpriteText.drawStringWithScrollBackground(b, s, Game1.viewport.Width / 2 - SpriteText.getWidthOfString(s) / 2, 16, "", 1f, -1);
                if (!this.upgrading && !this.demolishing && !this.moving) {
                    Vector2 vector2 = new Vector2((float)((Game1.viewport.X + Game1.getOldMouseX()) / 64), (float)((Game1.viewport.Y + Game1.getOldMouseY()) / 64));
                    for (int y = 0; y < this.CurrentBlueprint.tilesHeight; ++y) {
                        for (int x = 0; x < this.CurrentBlueprint.tilesWidth; ++x) {
                            int structurePlacementTile = this.CurrentBlueprint.getTileSheetIndexForStructurePlacementTile(x, y);
                            Vector2 tileLocation = new Vector2(vector2.X + (float)x, vector2.Y + (float)y);
                            if (!(Game1.currentLocation as BuildableGameLocation).isBuildable(tileLocation))
                                ++structurePlacementTile;
                            b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, tileLocation * 64f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(194 + structurePlacementTile * 16, 388, 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.999f);
                        }
                    }
                } else if (this.moving && this.buildingToMove != null) {
                    Vector2 vector2 = new Vector2((float)((Game1.viewport.X + Game1.getOldMouseX()) / 64), (float)((Game1.viewport.Y + Game1.getOldMouseY()) / 64));
                    for (int y = 0; y < (int)((NetFieldBase<int, NetInt>)this.buildingToMove.tilesHigh); ++y) {
                        for (int x = 0; x < (int)((NetFieldBase<int, NetInt>)this.buildingToMove.tilesWide); ++x) {
                            int structurePlacementTile = this.buildingToMove.getTileSheetIndexForStructurePlacementTile(x, y);
                            Vector2 tileLocation = new Vector2(vector2.X + (float)x, vector2.Y + (float)y);
                            if (!(Game1.currentLocation as BuildableGameLocation).isBuildable(tileLocation))
                                ++structurePlacementTile;
                            b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, tileLocation * 64f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(194 + structurePlacementTile * 16, 388, 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.999f);
                        }
                    }
                }
            }
            this.cancelButton.draw(b);
            this.drawMouse(b);
            if (this.hoverText.Length <= 0)
                return;
            IClickableMenu.drawHoverText(b, this.hoverText, Game1.dialogueFont, 0, 0, -1, (string)null, -1, (string[])null, (Item)null, 0, -1, -1, -1, -1, 1f, (CraftingRecipe)null);
        }

        public override void receiveRightClick(int x, int y, bool playSound = true) {
        }
    }
}