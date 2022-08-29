/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Prism-99/FarmExpansion
**
*************************************************/

using FarmExpansion.Framework;
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
using System;
using System.Collections.Generic;
using System.Linq;
using xTile.Dimensions;
using Object = StardewValley.Object;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace FarmExpansion.Menus
{
    public class FECarpenterMenu : IClickableMenu
    {
        protected readonly ICollection<BluePrint> _farmBluePrintsToAdd;

        protected readonly ICollection<BluePrint> _expansionBluePrintsToAdd;

        public void AddFarmBluePrint(BluePrint blueprint)
        {
            _farmBluePrintsToAdd.Add(blueprint);
        }

        public void AddExpansionBluePrint(BluePrint blueprint)
        {
            _expansionBluePrintsToAdd.Add(blueprint);
        }

        FEFramework framework;

        GameLocation previousLocation = Game1.currentLocation;

        private Farm currentFarm = Game1.getFarm();

        public const int region_backButton = 101;

        public const int region_forwardButton = 102;

        public const int region_upgradeIcon = 103;

        public const int region_demolishButton = 104;

        public const int region_moveBuitton = 105;

        public const int region_okButton = 106;

        public const int region_cancelButton = 107;

        public const int region_previousFarmButton = 108;

        public const int region_nextFarmButton = 109;

        public int maxWidthOfBuildingViewer = 7 * Game1.tileSize;

        public int maxHeightOfBuildingViewer = 8 * Game1.tileSize;

        public int maxWidthOfDescription = 6 * Game1.tileSize;

        private List<BluePrint> blueprints = new List<BluePrint>();

        private int currentBlueprintIndex;

        public ClickableTextureComponent okButton;

        public ClickableTextureComponent cancelButton;

        public ClickableTextureComponent backButton;

        public ClickableTextureComponent forwardButton;

        public ClickableTextureComponent upgradeIcon;

        public ClickableTextureComponent demolishButton;

        public ClickableTextureComponent moveButton;

        public ClickableTextureComponent previousFarmButton;

        public ClickableTextureComponent nextFarmButton;

        private Building currentBuilding;

        private Building buildingToMove;

        private string buildingDescription;

        private string buildingName;

        private string[] farmName = new string[2] {"  Farm  ", "Expansion"};

        private List<Item> ingredients = new List<Item>();

        private int price;

        private bool onFarm;

        private bool drawBG = true;

        private bool freeze;

        private bool upgrading;

        private bool demolishing;

        private bool moving;

        private bool magicalConstruction => false;

        //private bool magicalConstruction;

        private string hoverText = "";

        public BluePrint CurrentBlueprint
        {
            get
            {
                return this.blueprints[this.currentBlueprintIndex];
            }
        }

        public FECarpenterMenu(/*bool magicalConstruction = false*/FEFramework framework, ICollection<BluePrint> farmBlueprints, ICollection<BluePrint> expansionBlueprints)
        {
            this._expansionBluePrintsToAdd = expansionBlueprints;
            this._farmBluePrintsToAdd = farmBlueprints;

            //this.magicalConstruction = magicalConstruction;
            this.framework = framework;
            Game1.player.forceCanMove();
            this.resetBounds();
            this.populateFarmBlueprints();
            this.setNewActiveBlueprint();
            if (!Game1.options.SnappyMenus)
            {
                return;
            }
            this.populateClickableComponentList();
            this.snapToDefaultClickableComponent();
        }

        private void populateFarmBlueprints()
        {
            // split off ctor in FE
            this.blueprints = new List<BluePrint>();

            if (this.magicalConstruction)
            {
                this.blueprints.Add(new BluePrint("Junimo Hut"));
                this.blueprints.Add(new BluePrint("Earth Obelisk"));
                this.blueprints.Add(new BluePrint("Water Obelisk"));
                this.blueprints.Add(new BluePrint("Desert Obelisk"));
                this.blueprints.Add(new BluePrint("Gold Clock"));
            }
            else
            {
                bool buildingOnExpansion = framework.expansionSelected(currentFarm.Name);

                this.blueprints.Add(new BluePrint("Coop"));
                this.blueprints.Add(new BluePrint("Barn"));
                this.blueprints.Add(new BluePrint("Well"));
                if (!buildingOnExpansion)
                {
                    this.blueprints.Add(new BluePrint("Silo"));
                }
                this.blueprints.Add(new BluePrint("Mill"));
                this.blueprints.Add(new BluePrint("Shed"));
                if (!buildingOnExpansion)
                {
                    this.blueprints.Add(new BluePrint("Fish Pond"));
                }
                int buildingsConstructed = currentFarm.getNumberBuildingsConstructed("Cabin");
                if (Game1.IsMasterGame && buildingsConstructed < Game1.CurrentPlayerLimit - 1 && !buildingOnExpansion)
                {
                    this.blueprints.Add(new BluePrint("Stone Cabin"));
                    this.blueprints.Add(new BluePrint("Plank Cabin"));
                    this.blueprints.Add(new BluePrint("Log Cabin"));
                }
                if (currentFarm.getNumberBuildingsConstructed("Stable") < buildingsConstructed + 1 && !buildingOnExpansion)
                {
                    this.blueprints.Add(new BluePrint("Stable"));
                }
                if (!buildingOnExpansion)
                {
                    this.blueprints.Add(new BluePrint("Slime Hutch"));
                }
                if (currentFarm.isBuildingConstructed("Coop"))
                {
                    this.blueprints.Add(new BluePrint("Big Coop"));
                }
                if (currentFarm.isBuildingConstructed("Big Coop"))
                {
                    this.blueprints.Add(new BluePrint("Deluxe Coop"));
                }
                if (currentFarm.isBuildingConstructed("Barn"))
                {
                    this.blueprints.Add(new BluePrint("Big Barn"));
                }
                if (currentFarm.isBuildingConstructed("Big Barn"))
                {
                    this.blueprints.Add(new BluePrint("Deluxe Barn"));
                }
                if (currentFarm.isBuildingConstructed("Shed"))
                {
                    this.blueprints.Add(new BluePrint("Big Shed"));
                }
                if (!buildingOnExpansion)
                {
                    this.blueprints.Add(new BluePrint("Shipping Bin"));
                }
                if (!buildingOnExpansion)
                    this.blueprints.AddRange(_farmBluePrintsToAdd);
                else
                    this.blueprints.AddRange(_expansionBluePrintsToAdd);
            }
            this.setNewActiveBlueprint();
            if (!Game1.options.SnappyMenus)
                return;
            this.populateClickableComponentList();
            this.snapToDefaultClickableComponent();
        }

        public override void snapToDefaultClickableComponent()
        {
            // straight copy from StardewValley.Menus.CarpenterMenu, nothing FE specific
            this.currentlySnappedComponent = base.getComponentWithID(107);
            this.snapCursorToCurrentSnappedComponent();
        }

        private void resetBounds()
        {
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
            // FE - moved to the left to make space for building name
            ClickableTextureComponent textureComponent3 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen - Game1.tileSize / 2 - Game1.tileSize / 12, yPositionOnScreen + maxHeightOfBuildingViewer + Game1.tileSize, 12 * Game1.pixelZoom, 11 * Game1.pixelZoom), Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(352, 495, 12, 11), 4f, false);
            textureComponent3.myID = 101;
            textureComponent3.rightNeighborID = 102;
            this.backButton = textureComponent3;
            // FE - moved to the right to make space for building name
            ClickableTextureComponent textureComponent4 = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width - borderWidth - spaceToClearSideBorder - Game1.tileSize * 5 - Game1.tileSize * 2 / 3 - Game1.pixelZoom, yPositionOnScreen + maxHeightOfBuildingViewer + Game1.tileSize, 12 * Game1.pixelZoom, 11 * Game1.pixelZoom), Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(365, 495, 12, 11), 4f, false);
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
            bool flag = false;
            foreach (Building building in currentFarm.buildings)
            {
                if (building.hasCarpenterPermissions())
                    flag = true;
            }
            this.demolishButton.visible = Game1.IsMasterGame;
            this.moveButton.visible = Game1.IsMasterGame || Game1.player.team.farmhandsCanMoveBuildings.Value == FarmerTeam.RemoteBuildingPermissions.On || Game1.player.team.farmhandsCanMoveBuildings.Value == FarmerTeam.RemoteBuildingPermissions.OwnedBuildings & flag;
            if (!this.demolishButton.visible)
            {
                this.upgradeIcon.rightNeighborID = this.demolishButton.rightNeighborID;
                this.okButton.rightNeighborID = this.demolishButton.rightNeighborID;
                this.cancelButton.leftNeighborID = this.demolishButton.leftNeighborID;
            }

            // added for FE
            this.previousFarmButton = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + maxWidthOfBuildingViewer - spaceToClearSideBorder / 2, yPositionOnScreen, 12 * Game1.pixelZoom, 11 * Game1.pixelZoom), Game1.mouseCursors, new Rectangle(352, 495, 12, 11), Game1.pixelZoom)
            {
                myID = 108,
                leftNeighborID = 109,
                rightNeighborID = 109,
                downNeighborID = 101
            };
            this.nextFarmButton = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width - spaceToClearSideBorder - borderWidth - Game1.tileSize + Game1.tileSize / 12, yPositionOnScreen, 12 * Game1.pixelZoom, 11 * Game1.pixelZoom), Game1.mouseCursors, new Rectangle(365, 495, 12, 11), Game1.pixelZoom)
            {
                myID = 109,
                leftNeighborID = 108,
                rightNeighborID = 108,
                downNeighborID = 102
            };

            if (this.moveButton.visible)
                return;
            this.upgradeIcon.leftNeighborID = this.moveButton.leftNeighborID;
            this.forwardButton.rightNeighborID = this.moveButton.rightNeighborID;
            this.okButton.leftNeighborID = this.moveButton.leftNeighborID;
        }

        public void setNewActiveBlueprint()
        {
            // straight copy from StardewValley.Menus.CarpenterMenu, nothing FE specific
            this.currentBuilding = !this.blueprints[this.currentBlueprintIndex].name.Contains("Coop") ? (!this.blueprints[this.currentBlueprintIndex].name.Contains("Barn") ? (!this.blueprints[this.currentBlueprintIndex].name.Contains("Mill") ? (!this.blueprints[this.currentBlueprintIndex].name.Contains("Junimo Hut") ? (!this.blueprints[this.currentBlueprintIndex].name.Contains("Shipping Bin") ? (!this.blueprints[this.currentBlueprintIndex].name.Contains("Fish Pond") ? new Building(this.blueprints[this.currentBlueprintIndex], Vector2.Zero) : (Building)new FishPond(this.blueprints[this.currentBlueprintIndex], Vector2.Zero)) : (Building)new ShippingBin(this.blueprints[this.currentBlueprintIndex], Vector2.Zero)) : (Building)new JunimoHut(this.blueprints[this.currentBlueprintIndex], Vector2.Zero)) : (Building)new Mill(this.blueprints[this.currentBlueprintIndex], Vector2.Zero)) : (Building)new Barn(this.blueprints[this.currentBlueprintIndex], Vector2.Zero)) : (Building)new Coop(this.blueprints[this.currentBlueprintIndex], Vector2.Zero);
            this.price = this.blueprints[this.currentBlueprintIndex].moneyRequired;
            this.ingredients.Clear();
            foreach (KeyValuePair<int, int> keyValuePair in this.blueprints[this.currentBlueprintIndex].itemsRequired)
                this.ingredients.Add((Item)new StardewValley.Object(keyValuePair.Key, keyValuePair.Value, false, -1, 0));
            this.buildingDescription = this.blueprints[this.currentBlueprintIndex].description;
            this.buildingName = this.blueprints[this.currentBlueprintIndex].displayName;
        }

        public override void performHoverAction(int x, int y)
        {
            this.cancelButton.tryHover(x, y, 0.1f);
            base.performHoverAction(x, y);
            if (!this.onFarm)
            {
                this.backButton.tryHover(x, y, 1f);
                this.forwardButton.tryHover(x, y, 1f);
                this.okButton.tryHover(x, y, 0.1f);
                this.demolishButton.tryHover(x, y, 0.1f);
                this.moveButton.tryHover(x, y, 0.1f);
                // slight modification for FE
                this.previousFarmButton.tryHover(x, y, 1f);
                this.nextFarmButton.tryHover(x, y, 1f);
                // /slight modification for FE
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
            }
            else
            {
                if (!this.upgrading && !this.demolishing && !this.moving || this.freeze)
                    return;
                // FE: use currentFarm in several places
                foreach (Building building in currentFarm.buildings)
                    building.color.Value = Color.White;
                Building b = currentFarm.getBuildingAt(new Vector2((float)((Game1.viewport.X + Game1.getOldMouseX()) / 64), (float)((Game1.viewport.Y + Game1.getOldMouseY()) / 64))) ?? currentFarm.getBuildingAt(new Vector2((float)((Game1.viewport.X + Game1.getOldMouseX()) / 64), (float)((Game1.viewport.Y + Game1.getOldMouseY() + 128) / 64))) ?? currentFarm.getBuildingAt(new Vector2((float)((Game1.viewport.X + Game1.getOldMouseX()) / 64), (float)((Game1.viewport.Y + Game1.getOldMouseY() + 192) / 64)));
                if (this.upgrading)
                {
                    if (b != null && this.CurrentBlueprint.nameOfBuildingToUpgrade != null && this.CurrentBlueprint.nameOfBuildingToUpgrade.Equals((string)((NetFieldBase<string, NetString>)b.buildingType)))
                    {
                        b.color.Value = Color.Lime * 0.8f;
                    }
                    else
                    {
                        if (b == null)
                            return;
                        b.color.Value = Color.Red * 0.8f;
                    }
                }
                else if (this.demolishing)
                {
                    if (b == null || !this.hasPermissionsToDemolish(b))
                        return;
                    b.color.Value = Color.Red * 0.8f;
                }
                else
                {
                    if (!this.moving || b == null || !this.hasPermissionsToMove(b))
                        return;
                    b.color.Value = Color.Lime * 0.8f;
                }
            }
        }

        public bool hasPermissionsToDemolish(Building b)
        {
            // straight copy from StardewValley.Menus.CarpenterMenu, nothing FE specific
            return Game1.IsMasterGame;
        }

        public bool hasPermissionsToMove(Building b)
        {
            // straight copy from StardewValley.Menus.CarpenterMenu, nothing FE specific
            return Game1.IsMasterGame || Game1.player.team.farmhandsCanMoveBuildings.Value == FarmerTeam.RemoteBuildingPermissions.On || Game1.player.team.farmhandsCanMoveBuildings.Value == FarmerTeam.RemoteBuildingPermissions.OwnedBuildings && b.hasCarpenterPermissions();
        }

        public override void receiveGamePadButton(Buttons b)
        {
            // straight copy from StardewValley.Menus.CarpenterMenu, nothing FE specific
            base.receiveGamePadButton(b);
            if (!this.onFarm && b == Buttons.LeftTrigger)
            {
                --this.currentBlueprintIndex;
                if (this.currentBlueprintIndex < 0)
                    this.currentBlueprintIndex = this.blueprints.Count - 1;
                this.setNewActiveBlueprint();
                Game1.playSound("shwip");
            }
            if (!this.onFarm && b == Buttons.LeftShoulder)
            {
                currentFarm = framework.swapFarm(currentFarm);
                this.currentBlueprintIndex = 0;
                this.populateFarmBlueprints();
                this.setNewActiveBlueprint();
                Game1.playSound("shwip");
                previousFarmButton.scale = previousFarmButton.baseScale;
            }
            if (!this.onFarm && b == Buttons.RightShoulder)
            {
                currentFarm = framework.swapFarm(currentFarm);
                this.currentBlueprintIndex = 0;
                this.populateFarmBlueprints();
                this.setNewActiveBlueprint();
                nextFarmButton.scale = nextFarmButton.baseScale;
                Game1.playSound("shwip");
            }
            if (this.onFarm || b != Buttons.RightTrigger)
                return;
            this.currentBlueprintIndex = (this.currentBlueprintIndex + 1) % this.blueprints.Count;
            this.setNewActiveBlueprint();
            Game1.playSound("shwip");
        }

        public override void receiveKeyPress(Keys key)
        {
            // straight copy from StardewValley.Menus.CarpenterMenu, nothing FE specific
            if (this.freeze)
                return;
            if (!this.onFarm)
                base.receiveKeyPress(key);
            if (Game1.globalFade || !this.onFarm)
                return;
            if (Game1.options.doesInputListContain(Game1.options.menuButton, key) && this.readyToClose())
            {
                this.returnToCarpentryMenu();
            }
            else
            {
                if (Game1.options.SnappyMenus)
                    return;
                if (Game1.options.doesInputListContain(Game1.options.moveDownButton, key))
                    Game1.panScreen(0, 4);
                else if (Game1.options.doesInputListContain(Game1.options.moveRightButton, key))
                    Game1.panScreen(4, 0);
                else if (Game1.options.doesInputListContain(Game1.options.moveUpButton, key))
                {
                    Game1.panScreen(0, -4);
                }
                else
                {
                    if (!Game1.options.doesInputListContain(Game1.options.moveLeftButton, key))
                        return;
                    Game1.panScreen(-4, 0);
                }
            }
        }

        public override void update(GameTime time)
        {
            base.update(time);
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
            if (Game1.IsMultiplayer)
                return;
            // slight modification for FE - use currentFarm instead of Game1.getFarm()
            Farm farm = currentFarm;
            foreach (Character character in farm.animals.Values)
                character.MovePosition(Game1.currentGameTime, Game1.viewport, (GameLocation)farm);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (this.freeze)
                return;
            if (!this.onFarm)
                base.receiveLeftClick(x, y, playSound);
            if (this.cancelButton.containsPoint(x, y))
            {
                if (!this.onFarm)
                {
                    this.exitThisMenu(true);
                    Game1.player.forceCanMove();
                    Game1.playSound("bigDeSelect");
                }
                else
                {
                    if (this.moving && this.buildingToMove != null)
                    {
                        Game1.playSound("cancel");
                        return;
                    }
                    this.returnToCarpentryMenu();
                    Game1.playSound("smallSelect");
                    return;
                }
            }
            if (!this.onFarm && this.backButton.containsPoint(x, y))
            {
                --this.currentBlueprintIndex;
                if (this.currentBlueprintIndex < 0)
                    this.currentBlueprintIndex = this.blueprints.Count - 1;
                this.setNewActiveBlueprint();
                Game1.playSound("shwip");
                this.backButton.scale = this.backButton.baseScale;
            }
            if (!this.onFarm && this.forwardButton.containsPoint(x, y))
            {
                this.currentBlueprintIndex = (this.currentBlueprintIndex + 1) % this.blueprints.Count;
                this.setNewActiveBlueprint();
                this.backButton.scale = this.backButton.baseScale;
                Game1.playSound("shwip");
            }
            if (!this.onFarm && this.demolishButton.containsPoint(x, y) && this.demolishButton.visible)
            {
                Game1.globalFadeToBlack(new Game1.afterFadeFunction(this.setUpForBuildingPlacement), 0.02f);
                Game1.playSound("smallSelect");
                this.onFarm = true;
                this.demolishing = true;
            }
            if (!this.onFarm && this.moveButton.containsPoint(x, y) && this.moveButton.visible)
            {
                Game1.globalFadeToBlack(new Game1.afterFadeFunction(this.setUpForBuildingPlacement), 0.02f);
                Game1.playSound("smallSelect");
                this.onFarm = true;
                this.moving = true;
            }
            if (this.okButton.containsPoint(x, y) && !this.onFarm && (Game1.player.Money >= this.price && this.blueprints[this.currentBlueprintIndex].doesFarmerHaveEnoughResourcesToBuild()))
            {
                Game1.globalFadeToBlack(new Game1.afterFadeFunction(this.setUpForBuildingPlacement), 0.02f);
                Game1.playSound("smallSelect");
                this.onFarm = true;
            }

            // added for FE
            if (!this.onFarm && this.previousFarmButton.containsPoint(x, y))
            {
                currentFarm = framework.swapFarm(currentFarm);
                this.currentBlueprintIndex = 0;
                this.populateFarmBlueprints();
                this.setNewActiveBlueprint();
                Game1.playSound("shwip");
                previousFarmButton.scale = previousFarmButton.baseScale;
            }
            if (!this.onFarm && nextFarmButton.containsPoint(x, y))
            {
                currentFarm = framework.swapFarm(currentFarm);
                this.currentBlueprintIndex = 0;
                this.populateFarmBlueprints();
                this.setNewActiveBlueprint();
                nextFarmButton.scale = nextFarmButton.baseScale;
                Game1.playSound("shwip");
            }

            if (!this.onFarm || this.freeze || Game1.globalFade)
                return;
            if (this.demolishing)
            {
                // FE: use currentFarm
                Farm farm = currentFarm;
                Building destroyed = farm.getBuildingAt(new Vector2((float)((Game1.viewport.X + Game1.getOldMouseX()) / 64), (float)((Game1.viewport.Y + Game1.getOldMouseY()) / 64)));
                Action buildingLockFailed = (Action)(() =>
                {
                    if (!this.demolishing)
                        return;
                    Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_LockFailed"), Color.Red, 3500f));
                });
                Action continueDemolish = (Action)(() =>
                {
                    if (!this.demolishing || destroyed == null || !farm.buildings.Contains(destroyed))
                        return;
                    if ((int)((NetFieldBase<int, NetInt>)destroyed.daysOfConstructionLeft) > 0 || (int)((NetFieldBase<int, NetInt>)destroyed.daysUntilUpgrade) > 0)
                        Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_DuringConstruction"), Color.Red, 3500f));
                    else if (destroyed.indoors.Value != null && destroyed.indoors.Value is AnimalHouse && (destroyed.indoors.Value as AnimalHouse).animalsThatLiveHere.Count > 0)
                        Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_AnimalsHere"), Color.Red, 3500f));
                    else if (destroyed.indoors.Value != null && destroyed.indoors.Value.farmers.Count<Farmer>() > 0)
                    {
                        Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_PlayerHere"), Color.Red, 3500f));
                    }
                    else
                    {
                        if (destroyed.indoors.Value != null && destroyed.indoors.Value is Cabin)
                        {
                            foreach (Character allFarmer in Game1.getAllFarmers())
                            {
                                if (allFarmer.currentLocation.Name == (destroyed.indoors.Value as Cabin).GetCellarName())
                                {
                                    Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_PlayerHere"), Color.Red, 3500f));
                                    return;
                                }
                            }
                        }
                        if (destroyed.indoors.Value is Cabin && (destroyed.indoors.Value as Cabin).farmhand.Value.isActive())
                        {
                            Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_FarmhandOnline"), Color.Red, 3500f));
                        }
                        else
                        {
                            Chest chest = (Chest)null;
                            if (destroyed.indoors.Value is Cabin)
                            {
                                List<Item> objList = (destroyed.indoors.Value as Cabin).demolish();
                                if (objList.Count > 0)
                                {
                                    chest = new Chest(true);
                                    chest.fixLidFrame();
                                    chest.items.Set((IList<Item>)objList);
                                }
                            }
                            if (!farm.destroyStructure(destroyed))
                                return;
                            int tileY = (int)((NetFieldBase<int, NetInt>)destroyed.tileY);
                            int tilesHigh = (int)((NetFieldBase<int, NetInt>)destroyed.tilesHigh);
                            Game1.flashAlpha = 1f;
                            // FE: use current farm
                            destroyed.showDestroyedAnimation(currentFarm);
                            Game1.playSound("explosion");
                            Utility.spreadAnimalsAround(destroyed, farm);
                            DelayedAction.functionAfterDelay(new DelayedAction.delayedBehavior(this.returnToCarpentryMenu), 1500);
                            this.freeze = true;
                            if (chest == null)
                                return;
                            farm.objects[new Vector2((float)((int)((NetFieldBase<int, NetInt>)destroyed.tileX) + (int)((NetFieldBase<int, NetInt>)destroyed.tilesWide) / 2), (float)((int)((NetFieldBase<int, NetInt>)destroyed.tileY) + (int)((NetFieldBase<int, NetInt>)destroyed.tilesHigh) / 2))] = (StardewValley.Object)chest;
                        }
                    }
                });
                if (destroyed != null)
                {
                    if (destroyed.indoors.Value != null && destroyed.indoors.Value is Cabin && !Game1.IsMasterGame)
                    {
                        Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_LockFailed"), Color.Red, 3500f));
                        destroyed = (Building)null;
                        return;
                    }
                    if (!Game1.IsMasterGame && !this.hasPermissionsToDemolish(destroyed))
                    {
                        destroyed = (Building)null;
                        return;
                    }
                }
                if (destroyed != null && destroyed.indoors.Value is Cabin)
                {
                    Cabin cabin = destroyed.indoors.Value as Cabin;
                    if (cabin.farmhand.Value != null && (bool)((NetFieldBase<bool, NetBool>)cabin.farmhand.Value.isCustomized))
                    {
                        Game1.currentLocation.createQuestionDialogue(Game1.content.LoadString("Strings\\UI:Carpenter_DemolishCabinConfirm", (object)cabin.farmhand.Value.Name), Game1.currentLocation.createYesNoResponses(), (GameLocation.afterQuestionBehavior)((f, answer) =>
                        {
                            if (answer == "Yes")
                            {
                                Game1.activeClickableMenu = (IClickableMenu)this;
                                Game1.player.team.demolishLock.RequestLock(continueDemolish, buildingLockFailed);
                            }
                            else
                                DelayedAction.functionAfterDelay(new DelayedAction.delayedBehavior(this.returnToCarpentryMenu), 500);
                        }), (NPC)null);
                        return;
                    }
                }
                if (destroyed == null)
                    return;
                Game1.player.team.demolishLock.RequestLock(continueDemolish, buildingLockFailed);
            }
            else if (this.upgrading)
            {
                // use currentFarm for FE
                Building buildingAt = currentFarm.getBuildingAt(new Vector2((float)((Game1.viewport.X + Game1.getOldMouseX()) / 64), (float)((Game1.viewport.Y + Game1.getOldMouseY()) / 64)));
                if (buildingAt != null && this.CurrentBlueprint.name != null && buildingAt.buildingType.Equals((object)this.CurrentBlueprint.nameOfBuildingToUpgrade))
                {
                    this.CurrentBlueprint.consumeResources();
                    buildingAt.daysUntilUpgrade.Value = 2;
                    // use currentFarm for FE
                    buildingAt.showUpgradeAnimation(currentFarm);
                    Game1.playSound("axe");
                    DelayedAction.functionAfterDelay(new DelayedAction.delayedBehavior(this.returnToCarpentryMenuAfterSuccessfulBuild), 1500);
                    this.freeze = true;
                }
                else
                {
                    if (buildingAt == null)
                        return;
                    Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantUpgrade_BuildingType"), Color.Red, 3500f));
                }
            }
            else if (this.moving)
            {
                if (this.buildingToMove == null)
                {
                    this.buildingToMove = currentFarm.getBuildingAt(new Vector2((float)((Game1.viewport.X + Game1.getMouseX()) / 64), (float)((Game1.viewport.Y + Game1.getMouseY()) / 64)));
                    if (this.buildingToMove == null)
                        return;
                    if ((int)((NetFieldBase<int, NetInt>)this.buildingToMove.daysOfConstructionLeft) > 0)
                        this.buildingToMove = (Building)null;
                    else if (!Game1.IsMasterGame && !this.hasPermissionsToMove(this.buildingToMove))
                    {
                        this.buildingToMove = (Building)null;
                    }
                    else
                    {
                        this.buildingToMove.isMoving = true;
                        Game1.playSound("axchop");
                    }
                }
                else if (currentFarm.buildStructure(this.buildingToMove, new Vector2((float)((Game1.viewport.X + Game1.getMouseX()) / 64), (float)((Game1.viewport.Y + Game1.getMouseY()) / 64)), Game1.player, false))
                {
                    this.buildingToMove.isMoving = false;
                    if (this.buildingToMove is ShippingBin)
                        (this.buildingToMove as ShippingBin).initLid();
                    this.buildingToMove.performActionOnBuildingPlacement();
                    this.buildingToMove = (Building)null;
                    Game1.playSound("axchop");
                    DelayedAction.playSoundAfterDelay("dirtyHit", 50, (GameLocation)null, -1);
                    DelayedAction.playSoundAfterDelay("dirtyHit", 150, (GameLocation)null, -1);
                }
                else
                    Game1.playSound("cancel");
            }
            else
                Game1.player.team.buildLock.RequestLock((Action)(() =>
                {
                    if (this.onFarm && Game1.locationRequest == null)
                    {
                        if (this.tryToBuild())
                        {
                            this.CurrentBlueprint.consumeResources();
                            DelayedAction.functionAfterDelay(new DelayedAction.delayedBehavior(this.returnToCarpentryMenuAfterSuccessfulBuild), 2000);
                            this.freeze = true;
                        }
                        else
                            Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantBuild"), Color.Red, 3500f));
                    }
                    Game1.player.team.buildLock.ReleaseLock();
                }), (Action)null);
        }

        public bool tryToBuild()
        {
            // use currentFarm for FE, and constant false for magicalConstruction
            return currentFarm.buildStructure(this.CurrentBlueprint, new Vector2((float)((Game1.viewport.X + Game1.getOldMouseX()) / 64), (float)((Game1.viewport.Y + Game1.getOldMouseY()) / 64)), Game1.player, false, false);
        }

        public void returnToCarpentryMenu()
        {
            // straight copy from StardewValley.Menus.CarpenterMenu, nothing FE specific
            LocationRequest locationRequest = Game1.getLocationRequest(this.magicalConstruction ? "WizardHouse" : "ScienceHouse", false);
            locationRequest.OnWarp += (LocationRequest.Callback)(() =>
            {
                this.onFarm = false;
                Game1.player.viewingLocation.Value = (string)null;
                this.resetBounds();
                this.upgrading = false;
                this.moving = false;
                this.buildingToMove = (Building)null;
                this.freeze = false;
                Game1.displayHUD = true;
                Game1.viewportFreeze = false;
                Game1.viewport.Location = new Location(320, 1536);
                this.drawBG = true;
                this.demolishing = false;
                Game1.displayFarmer = true;
                if (!Game1.options.SnappyMenus)
                    return;
                this.populateClickableComponentList();
                this.snapToDefaultClickableComponent();
            });
            Game1.warpFarmer(locationRequest, Game1.player.getTileX(), Game1.player.getTileY(), (int)Game1.player.facingDirection);
        }

        public void returnToCarpentryMenuAfterSuccessfulBuild()
        {
            // straight copy from StardewValley.Menus.CarpenterMenu, nothing FE specific
            LocationRequest locationRequest = Game1.getLocationRequest(this.magicalConstruction ? "WizardHouse" : "ScienceHouse", false);
            locationRequest.OnWarp += (LocationRequest.Callback)(() =>
            {
                Game1.displayHUD = true;
                Game1.player.viewingLocation.Value = (string)null;
                Game1.viewportFreeze = false;
                Game1.viewport.Location = new Location(320, 1536);
                this.freeze = true;
                Game1.displayFarmer = true;
                this.robinConstructionMessage();
            });
            Game1.warpFarmer(locationRequest, Game1.player.getTileX(), Game1.player.getTileY(), (int)Game1.player.facingDirection);
        }

        public void robinConstructionMessage()
        {
            // straight copy from StardewValley.Menus.CarpenterMenu, nothing FE specific
            this.exitThisMenu(true);
            Game1.player.forceCanMove();
            if (this.magicalConstruction)
                return;
            string str1 = "Data\\ExtraDialogue:Robin_" + (this.upgrading ? "Upgrade" : "New") + "Construction";
            if (Utility.isFestivalDay(Game1.dayOfMonth + 1, Game1.currentSeason))
                str1 += "_Festival";
            if (this.CurrentBlueprint.daysToConstruct <= 0)
            {
                Game1.drawDialogue(Game1.getCharacterFromName("Robin", true), Game1.content.LoadString("Data\\ExtraDialogue:Robin_Instant", LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.de ? (object)this.CurrentBlueprint.displayName : (object)this.CurrentBlueprint.displayName.ToLower()));
            }
            else
            {
                NPC characterFromName = Game1.getCharacterFromName("Robin", true);
                LocalizedContentManager content = Game1.content;
                string path = str1;
                string str2 = LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.de ? this.CurrentBlueprint.displayName : this.CurrentBlueprint.displayName.ToLower();
                string str3;
                switch (LocalizedContentManager.CurrentLanguageCode)
                {
                    case LocalizedContentManager.LanguageCode.pt:
                    case LocalizedContentManager.LanguageCode.es:
                    case LocalizedContentManager.LanguageCode.it:
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

        public override bool overrideSnappyMenuCursorMovementBan()
        {
            // straight copy from StardewValley.Menus.CarpenterMenu, nothing FE specific
            return this.onFarm;
        }

        public void setUpForBuildingPlacement()
        {
            Game1.currentLocation.cleanupBeforePlayerExit();
            this.hoverText = "";
            // FE: use currentFarm and "FarmExpansion"
            Game1.currentLocation = currentFarm;
            Game1.player.viewingLocation.Value = "FarmExpansion";
            Game1.currentLocation.resetForPlayerEntry();
            Game1.globalFadeToClear((Game1.afterFadeFunction)null, 0.02f);
            this.onFarm = true;
            this.cancelButton.bounds.X = Game1.viewport.Width - 128;
            this.cancelButton.bounds.Y = Game1.viewport.Height - 128;
            Game1.displayHUD = false;
            Game1.viewportFreeze = true;
            Game1.viewport.Location = new Location(3136, 320);
            Game1.panScreen(0, 0);
            this.drawBG = false;
            this.freeze = false;
            Game1.displayFarmer = false;
            if (this.demolishing || this.CurrentBlueprint.nameOfBuildingToUpgrade == null || (this.CurrentBlueprint.nameOfBuildingToUpgrade.Length <= 0 || this.moving))
                return;
            this.upgrading = true;
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            // straight copy from StardewValley.Menus.CarpenterMenu, nothing FE specific
            this.resetBounds();
        }

        public override void draw(SpriteBatch b)
        {
            if (this.drawBG)
                b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.5f);
            if (Game1.globalFade || this.freeze)
                return;
            if (!this.onFarm)
            {
                base.draw(b);
                IClickableMenu.drawTextureBox(b, this.xPositionOnScreen - 96, this.yPositionOnScreen - 16, this.maxWidthOfBuildingViewer + 64, this.maxHeightOfBuildingViewer + 64, this.magicalConstruction ? Color.RoyalBlue : Color.White);
                this.currentBuilding.drawInMenu(b, this.xPositionOnScreen + this.maxWidthOfBuildingViewer / 2 - (int)((NetFieldBase<int, NetInt>)this.currentBuilding.tilesWide) * 64 / 2 - 64, this.yPositionOnScreen + this.maxHeightOfBuildingViewer / 2 - this.currentBuilding.getSourceRectForMenu().Height * 4 / 2);
                if (this.CurrentBlueprint.isUpgrade())
                    this.upgradeIcon.draw(b);
                string s = " Deluxe  Barn   ";
                if (SpriteText.getWidthOfString(this.buildingName, 999999) >= SpriteText.getWidthOfString(s, 999999))
                    s = this.buildingName + " ";
                {
                    // FE: move this between back and forward buttons to make space for farm/FE switcher
                    var buildingNameScrollX = backButton.bounds.X + backButton.bounds.Width + 15 * Game1.pixelZoom;
                    var buildingNameScrollY = backButton.bounds.Y;
                    var buildingScrollWidth = forwardButton.bounds.X - buildingNameScrollX - 15 * Game1.pixelZoom;
                    buildingNameScrollX += buildingScrollWidth / 2;
                    SpriteText.drawStringWithScrollCenteredAt(b, this.buildingName, buildingNameScrollX, buildingNameScrollY, buildingScrollWidth, 1f, -1, 0, 0.88f, false);
                }
                int width;
                switch (LocalizedContentManager.CurrentLanguageCode)
                {
                    case LocalizedContentManager.LanguageCode.es:
                        width = this.maxWidthOfDescription + 64 + (this.CurrentBlueprint?.name == "Deluxe Barn" ? 96 : 0);
                        break;
                    case LocalizedContentManager.LanguageCode.fr:
                        width = this.maxWidthOfDescription + 96 + (this.CurrentBlueprint?.name == "Slime Hutch" || this.CurrentBlueprint?.name == "Deluxe Coop" || this.CurrentBlueprint?.name == "Deluxe Barn" ? 72 : 0);
                        break;
                    case LocalizedContentManager.LanguageCode.ko:
                        width = this.maxWidthOfDescription + 96 + (this.CurrentBlueprint?.name == "Slime Hutch" ? 64 : (this.CurrentBlueprint?.name == "Deluxe Coop" ? 96 : (this.CurrentBlueprint?.name == "Deluxe Barn" ? 112 : (this.CurrentBlueprint?.name == "Big Barn" ? 64 : 0))));
                        break;
                    case LocalizedContentManager.LanguageCode.it:
                        width = this.maxWidthOfDescription + 96;
                        break;
                    default:
                        width = this.maxWidthOfDescription + 64;
                        break;
                }
                IClickableMenu.drawTextureBox(b, this.xPositionOnScreen + this.maxWidthOfBuildingViewer - 16, this.yPositionOnScreen + 80, width, this.maxHeightOfBuildingViewer - 32, this.magicalConstruction ? Color.RoyalBlue : Color.White);
                if (this.magicalConstruction)
                {
                    Utility.drawTextWithShadow(b, Game1.parseText(this.buildingDescription, Game1.dialogueFont, width - 32), Game1.dialogueFont, new Vector2((float)(this.xPositionOnScreen + this.maxWidthOfBuildingViewer - 4), (float)(this.yPositionOnScreen + 80 + 16 + 4)), Game1.textColor * 0.25f, 1f, -1f, -1, -1, 0.0f, 3);
                    Utility.drawTextWithShadow(b, Game1.parseText(this.buildingDescription, Game1.dialogueFont, width - 32), Game1.dialogueFont, new Vector2((float)(this.xPositionOnScreen + this.maxWidthOfBuildingViewer - 1), (float)(this.yPositionOnScreen + 80 + 16 + 4)), Game1.textColor * 0.25f, 1f, -1f, -1, -1, 0.0f, 3);
                }
                Utility.drawTextWithShadow(b, Game1.parseText(this.buildingDescription, Game1.dialogueFont, width - 32), Game1.dialogueFont, new Vector2((float)(this.xPositionOnScreen + this.maxWidthOfBuildingViewer), (float)(this.yPositionOnScreen + 80 + 16)), this.magicalConstruction ? Color.PaleGoldenrod : Game1.textColor, 1f, -1f, -1, -1, this.magicalConstruction ? 0.0f : 0.75f, 3);
                Vector2 location = new Vector2((float)(this.xPositionOnScreen + this.maxWidthOfBuildingViewer + 16), (float)(this.yPositionOnScreen + 256 + 32));
                if (this.ingredients.Count < 3 && (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.fr || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ko || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.pt))
                    location.Y += 64f;
                SpriteText.drawString(b, "$", (int)location.X, (int)location.Y, 999999, -1, 999999, 1f, 0.88f, false, -1, "", -1, SpriteText.ScrollTextAlignment.Left);
                if (this.magicalConstruction)
                {
                    Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\StringsFromCSFiles:LoadGameMenu.cs.11020", (object)this.price), Game1.dialogueFont, new Vector2(location.X + 64f, location.Y + 8f), Game1.textColor * 0.5f, 1f, -1f, -1, -1, this.magicalConstruction ? 0.0f : 0.25f, 3);
                    Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\StringsFromCSFiles:LoadGameMenu.cs.11020", (object)this.price), Game1.dialogueFont, new Vector2((float)((double)location.X + 64.0 + 4.0 - 1.0), location.Y + 8f), Game1.textColor * 0.25f, 1f, -1f, -1, -1, this.magicalConstruction ? 0.0f : 0.25f, 3);
                }
                Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\StringsFromCSFiles:LoadGameMenu.cs.11020", (object)this.price), Game1.dialogueFont, new Vector2((float)((double)location.X + 64.0 + 4.0), location.Y + 4f), Game1.player.Money >= this.price ? (this.magicalConstruction ? Color.PaleGoldenrod : Game1.textColor) : Color.Red, 1f, -1f, -1, -1, this.magicalConstruction ? 0.0f : 0.25f, 3);
                location.X -= 16f;
                location.Y -= 21f;
                foreach (Item ingredient in this.ingredients)
                {
                    location.Y += 68f;
                    ingredient.drawInMenu(b, location, 1f);
                    bool flag = !(ingredient is StardewValley.Object) || Game1.player.hasItemInInventory((int)((NetFieldBase<int, NetInt>)(ingredient as StardewValley.Object).parentSheetIndex), ingredient.Stack, 0);
                    if (this.magicalConstruction)
                    {
                        Utility.drawTextWithShadow(b, ingredient.DisplayName, Game1.dialogueFont, new Vector2((float)((double)location.X + 64.0 + 12.0), location.Y + 24f), Game1.textColor * 0.25f, 1f, -1f, -1, -1, this.magicalConstruction ? 0.0f : 0.25f, 3);
                        Utility.drawTextWithShadow(b, ingredient.DisplayName, Game1.dialogueFont, new Vector2((float)((double)location.X + 64.0 + 16.0 - 1.0), location.Y + 24f), Game1.textColor * 0.25f, 1f, -1f, -1, -1, this.magicalConstruction ? 0.0f : 0.25f, 3);
                    }
                    Utility.drawTextWithShadow(b, ingredient.DisplayName, Game1.dialogueFont, new Vector2((float)((double)location.X + 64.0 + 16.0), location.Y + 20f), flag ? (this.magicalConstruction ? Color.PaleGoldenrod : Game1.textColor) : Color.Red, 1f, -1f, -1, -1, this.magicalConstruction ? 0.0f : 0.25f, 3);
                }
                this.backButton.draw(b);
                this.forwardButton.draw(b);
                this.okButton.draw(b, this.blueprints[this.currentBlueprintIndex].doesFarmerHaveEnoughResourcesToBuild() ? Color.White : Color.Gray * 0.8f, 0.88f, 0);
                this.demolishButton.draw(b);
                this.moveButton.draw(b);
                // added for FE
                SpriteText.drawStringWithScrollBackground(b, farmName[framework.expansionSelected(currentFarm.Name) ? 1 : 0], this.xPositionOnScreen + this.maxWidthOfBuildingViewer - IClickableMenu.spaceToClearSideBorder - Game1.tileSize / 4 + Game1.tileSize + ((this.width - (this.maxWidthOfBuildingViewer + Game1.tileSize * 2)) / 2 - SpriteText.getWidthOfString("Expansion") / 2), this.yPositionOnScreen, "Expansion", 1f, -1);
                this.previousFarmButton.draw(b);
                this.nextFarmButton.draw(b);
            }
            else
            {
                string s = this.upgrading ? Game1.content.LoadString("Strings\\UI:Carpenter_SelectBuilding_Upgrade", (object)new BluePrint(this.CurrentBlueprint.nameOfBuildingToUpgrade).displayName) : (this.demolishing ? Game1.content.LoadString("Strings\\UI:Carpenter_SelectBuilding_Demolish") : Game1.content.LoadString("Strings\\UI:Carpenter_ChooseLocation"));
                SpriteText.drawStringWithScrollBackground(b, s, Game1.viewport.Width / 2 - SpriteText.getWidthOfString(s, 999999) / 2, 16, "", 1f, -1, SpriteText.ScrollTextAlignment.Left);
                if (!this.upgrading && !this.demolishing && !this.moving)
                {
                    Vector2 vector2 = new Vector2((float)((Game1.viewport.X + Game1.getOldMouseX()) / 64), (float)((Game1.viewport.Y + Game1.getOldMouseY()) / 64));
                    for (int y = 0; y < this.CurrentBlueprint.tilesHeight; ++y)
                    {
                        for (int x = 0; x < this.CurrentBlueprint.tilesWidth; ++x)
                        {
                            int structurePlacementTile = this.CurrentBlueprint.getTileSheetIndexForStructurePlacementTile(x, y);
                            Vector2 tileLocation = new Vector2(vector2.X + (float)x, vector2.Y + (float)y);
                            if (!(Game1.currentLocation as BuildableGameLocation).isBuildable(tileLocation))
                                ++structurePlacementTile;
                            b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, tileLocation * 64f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(194 + structurePlacementTile * 16, 388, 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.999f);
                        }
                    }
                }
                else if (this.moving && this.buildingToMove != null)
                {
                    Vector2 vector2_1 = new Vector2((float)((Game1.viewport.X + Game1.getOldMouseX()) / 64), (float)((Game1.viewport.Y + Game1.getOldMouseY()) / 64));
                    BuildableGameLocation currentLocation = Game1.currentLocation as BuildableGameLocation;
                    for (int y = 0; y < (int)((NetFieldBase<int, NetInt>)this.buildingToMove.tilesHigh); ++y)
                    {
                        for (int x = 0; x < (int)((NetFieldBase<int, NetInt>)this.buildingToMove.tilesWide); ++x)
                        {
                            int structurePlacementTile = this.buildingToMove.getTileSheetIndexForStructurePlacementTile(x, y);
                            Vector2 vector2_2 = new Vector2(vector2_1.X + (float)x, vector2_1.Y + (float)y);
                            bool flag = currentLocation.buildings.Contains(this.buildingToMove) && this.buildingToMove.occupiesTile(vector2_2);
                            if (!currentLocation.isBuildable(vector2_2) && !flag)
                                ++structurePlacementTile;
                            b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, vector2_2 * 64f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(194 + structurePlacementTile * 16, 388, 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.999f);
                        }
                    }
                }
            }
            this.cancelButton.draw(b);
            this.drawMouse(b);
            if (this.hoverText.Length <= 0)
                return;
            IClickableMenu.drawHoverText(b, this.hoverText, Game1.dialogueFont, 0, 0, -1, (string)null, -1, (string[])null, (Item)null, 0, -1, -1, -1, -1, 1f, (CraftingRecipe)null, (IList<Item>)null);
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            // straight copy from StardewValley.Menus.CarpenterMenu, nothing FE specific
        }
    }
}
