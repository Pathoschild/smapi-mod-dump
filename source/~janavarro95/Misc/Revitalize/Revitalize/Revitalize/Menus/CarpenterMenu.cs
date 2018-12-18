using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using xTile.Dimensions;

namespace Revitalize.Menus
{
    public class CarpenterMenu : IClickableMenu
    {
        public const int region_backButton = 101;

        public const int region_forwardButton = 102;

        public const int region_upgradeIcon = 103;

        public const int region_demolishButton = 104;

        public const int region_moveBuitton = 105;

        public const int region_okButton = 106;

        public const int region_cancelButton = 107;

        public int maxWidthOfBuildingViewer = 7 * Game1.tileSize;

        public int maxHeightOfBuildingViewer = 8 * Game1.tileSize;

        public int maxWidthOfDescription = 6 * Game1.tileSize;

        private List<BluePrint> blueprints;

        private int currentBlueprintIndex;

        public ClickableTextureComponent okButton;

        public ClickableTextureComponent cancelButton;

        public ClickableTextureComponent backButton;

        public ClickableTextureComponent forwardButton;

        public ClickableTextureComponent upgradeIcon;

        public ClickableTextureComponent demolishButton;

        public ClickableTextureComponent moveButton;

        private Building currentBuilding;

        private Building buildingToMove;

        private string buildingDescription;

        private string buildingName;

        private List<Item> ingredients = new List<Item>();

        private int price;

        private bool onFarm;

        private bool drawBG = true;

        private bool freeze;

        private bool upgrading;

        private bool demolishing;

        private bool moving;

        private bool magicalConstruction;

        private string hoverText = "";

        bool flipFlop;

        public BluePrint CurrentBlueprint
        {
            get
            {
                return this.blueprints[this.currentBlueprintIndex];
            }
        }

        public CarpenterMenu(bool magicalConstruction = false)
        {
            this.magicalConstruction = magicalConstruction;
            Game1.player.forceCanMove();
            this.resetBounds();
            this.blueprints = new List<BluePrint>();
            if (magicalConstruction)
            {
                this.blueprints.Add(new BluePrint("Junimo Hut"));
                this.blueprints.Add(new BluePrint("Earth Obelisk"));
                this.blueprints.Add(new BluePrint("Water Obelisk"));
                this.blueprints.Add(new BluePrint("Gold Clock"));
            }
            else
            {
                this.blueprints.Add(new BluePrint("Coop"));
                this.blueprints.Add(new BluePrint("Barn"));
                this.blueprints.Add(new BluePrint("Well"));
                this.blueprints.Add(new BluePrint("Silo"));
                this.blueprints.Add(new BluePrint("Mill"));
                this.blueprints.Add(new BluePrint("Shed"));
                if (!Game1.getFarm().isBuildingConstructed("Stable"))
                {
                    this.blueprints.Add(new BluePrint("Stable"));
                }
                this.blueprints.Add(new BluePrint("Slime Hutch"));
                if (Game1.getFarm().isBuildingConstructed("Coop"))
                {
                    this.blueprints.Add(new BluePrint("Big Coop"));
                }
                if (Game1.getFarm().isBuildingConstructed("Big Coop"))
                {
                    this.blueprints.Add(new BluePrint("Deluxe Coop"));
                }
                if (Game1.getFarm().isBuildingConstructed("Barn"))
                {
                    this.blueprints.Add(new BluePrint("Big Barn"));
                }
                if (Game1.getFarm().isBuildingConstructed("Big Barn"))
                {
                    this.blueprints.Add(new BluePrint("Deluxe Barn"));
                }
            }
            this.setNewActiveBlueprint();
            if (Game1.options.SnappyMenus)
            {
                base.populateClickableComponentList();
                this.snapToDefaultClickableComponent();
            }
        }

        public override void snapToDefaultClickableComponent()
        {
            this.currentlySnappedComponent = base.getComponentWithID(107);
            this.snapCursorToCurrentSnappedComponent();
        }

        private void resetBounds()
        {
            this.xPositionOnScreen = Game1.viewport.Width / 2 - this.maxWidthOfBuildingViewer - IClickableMenu.spaceToClearSideBorder;
            this.yPositionOnScreen = Game1.viewport.Height / 2 - this.maxHeightOfBuildingViewer / 2 - IClickableMenu.spaceToClearTopBorder + Game1.tileSize / 2;
            this.width = this.maxWidthOfBuildingViewer + this.maxWidthOfDescription + IClickableMenu.spaceToClearSideBorder * 2 + Game1.tileSize;
            this.height = this.maxHeightOfBuildingViewer + IClickableMenu.spaceToClearTopBorder;
            base.initialize(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, true);
            this.okButton = new ClickableTextureComponent("OK", new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen + this.width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - Game1.tileSize * 3 - Game1.pixelZoom * 3, this.yPositionOnScreen + this.maxHeightOfBuildingViewer + Game1.tileSize, Game1.tileSize, Game1.tileSize), null, null, Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(366, 373, 16, 16), (float)Game1.pixelZoom, false)
            {
                myID = 106,
                rightNeighborID = 104,
                leftNeighborID = 105
            };
            this.cancelButton = new ClickableTextureComponent("OK", new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen + this.width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - Game1.tileSize, this.yPositionOnScreen + this.maxHeightOfBuildingViewer + Game1.tileSize, Game1.tileSize, Game1.tileSize), null, null, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 47, -1, -1), 1f, false)
            {
                myID = 107,
                leftNeighborID = 104
            };
            this.backButton = new ClickableTextureComponent(new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen + Game1.tileSize, this.yPositionOnScreen + this.maxHeightOfBuildingViewer + Game1.tileSize, 12 * Game1.pixelZoom, 11 * Game1.pixelZoom), Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(352, 495, 12, 11), (float)Game1.pixelZoom, false)
            {
                myID = 101,
                rightNeighborID = 102
            };
            this.forwardButton = new ClickableTextureComponent(new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen + this.maxWidthOfBuildingViewer - Game1.tileSize * 4 + Game1.tileSize / 4, this.yPositionOnScreen + this.maxHeightOfBuildingViewer + Game1.tileSize, 12 * Game1.pixelZoom, 11 * Game1.pixelZoom), Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(365, 495, 12, 11), (float)Game1.pixelZoom, false)
            {
                myID = 102,
                leftNeighborID = 101,
                rightNeighborID = 105
            };
            this.demolishButton = new ClickableTextureComponent(Game1.content.LoadString("Strings\\UI:Carpenter_Demolish", new object[0]), new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen + this.width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - Game1.tileSize * 2 - Game1.pixelZoom * 2, this.yPositionOnScreen + this.maxHeightOfBuildingViewer + Game1.tileSize - Game1.pixelZoom, Game1.tileSize, Game1.tileSize), null, null, Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(348, 372, 17, 17), (float)Game1.pixelZoom, false)
            {
                myID = 104,
                rightNeighborID = 107,
                leftNeighborID = 106
            };
            this.upgradeIcon = new ClickableTextureComponent(new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen + this.maxWidthOfBuildingViewer - Game1.tileSize * 2 + Game1.tileSize / 2, this.yPositionOnScreen + Game1.pixelZoom * 2, 9 * Game1.pixelZoom, 13 * Game1.pixelZoom), Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(402, 328, 9, 13), (float)Game1.pixelZoom, false)
            {
                myID = 103,
                rightNeighborID = 104,
                leftNeighborID = 105
            };
            this.moveButton = new ClickableTextureComponent(Game1.content.LoadString("Strings\\UI:Carpenter_MoveBuildings", new object[0]), new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen + this.width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - Game1.tileSize * 4 - Game1.pixelZoom * 5, this.yPositionOnScreen + this.maxHeightOfBuildingViewer + Game1.tileSize, Game1.tileSize, Game1.tileSize), null, null, Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(257, 284, 16, 16), (float)Game1.pixelZoom, false)
            {
                myID = 105,
                rightNeighborID = 106,
                leftNeighborID = 102
            };
        }

        public void setNewActiveBlueprint()
        {
            if (this.blueprints[this.currentBlueprintIndex].name.Contains("Coop"))
            {
                this.currentBuilding = new Coop(this.blueprints[this.currentBlueprintIndex], Vector2.Zero);
            }
            else if (this.blueprints[this.currentBlueprintIndex].name.Contains("Barn"))
            {
                this.currentBuilding = new Barn(this.blueprints[this.currentBlueprintIndex], Vector2.Zero);
            }
            else if (this.blueprints[this.currentBlueprintIndex].name.Contains("Mill"))
            {
                this.currentBuilding = new Mill(this.blueprints[this.currentBlueprintIndex], Vector2.Zero);
            }
            else if (this.blueprints[this.currentBlueprintIndex].name.Contains("Junimo Hut"))
            {
                this.currentBuilding = new JunimoHut(this.blueprints[this.currentBlueprintIndex], Vector2.Zero);
            }
            else
            {
                this.currentBuilding = new Building(this.blueprints[this.currentBlueprintIndex], Vector2.Zero);
            }
            this.price = this.blueprints[this.currentBlueprintIndex].moneyRequired;
            this.ingredients.Clear();
            foreach (KeyValuePair<int, int> current in this.blueprints[this.currentBlueprintIndex].itemsRequired)
            {
                this.ingredients.Add(new StardewValley.Object(current.Key, current.Value, false, -1, 0));
            }
            this.buildingDescription = this.blueprints[this.currentBlueprintIndex].description;
            this.buildingName = this.blueprints[this.currentBlueprintIndex].displayName;
        }

        public override void performHoverAction(int x, int y)
        {
            this.cancelButton.tryHover(x, y, 0.1f);
            base.performHoverAction(x, y);
            if (this.onFarm)
            {
                if ((this.upgrading || this.demolishing || this.moving) && !this.freeze)
                {
                    using (List<Building>.Enumerator enumerator = ((Farm)Game1.getLocationFromName("Farm")).buildings.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            enumerator.Current.color = Color.White;
                        }
                    }
                    Building buildingAt = ((Farm)Game1.getLocationFromName("Farm")).getBuildingAt(new Vector2((float)((Game1.viewport.X + Game1.getOldMouseX()) / Game1.tileSize), (float)((Game1.viewport.Y + Game1.getOldMouseY()) / Game1.tileSize)));
                    if (buildingAt == null)
                    {
                        buildingAt = ((Farm)Game1.getLocationFromName("Farm")).getBuildingAt(new Vector2((float)((Game1.viewport.X + Game1.getOldMouseX()) / Game1.tileSize), (float)((Game1.viewport.Y + Game1.getOldMouseY() + Game1.tileSize * 2) / Game1.tileSize)));
                        if (buildingAt == null)
                        {
                            buildingAt = ((Farm)Game1.getLocationFromName("Farm")).getBuildingAt(new Vector2((float)((Game1.viewport.X + Game1.getOldMouseX()) / Game1.tileSize), (float)((Game1.viewport.Y + Game1.getOldMouseY() + Game1.tileSize * 3) / Game1.tileSize)));
                        }
                    }
                    if (this.upgrading)
                    {
                        if (buildingAt != null && this.CurrentBlueprint.nameOfBuildingToUpgrade != null && this.CurrentBlueprint.nameOfBuildingToUpgrade.Equals(buildingAt.buildingType))
                        {
                            buildingAt.color = Color.Lime * 0.8f;
                            return;
                        }
                        if (buildingAt != null)
                        {
                            buildingAt.color = Color.Red * 0.8f;
                            return;
                        }
                    }
                    else if (this.demolishing)
                    {
                        if (buildingAt != null)
                        {
                            buildingAt.color = Color.Red * 0.8f;
                            return;
                        }
                    }
                    else if (this.moving && buildingAt != null)
                    {
                        buildingAt.color = Color.Lime * 0.8f;
                    }
                }
                return;
            }
            this.backButton.tryHover(x, y, 1f);
            this.forwardButton.tryHover(x, y, 1f);
            this.okButton.tryHover(x, y, 0.1f);
            this.demolishButton.tryHover(x, y, 0.1f);
            this.moveButton.tryHover(x, y, 0.1f);
            if (this.CurrentBlueprint.isUpgrade() && this.upgradeIcon.containsPoint(x, y))
            {
                this.hoverText = Game1.content.LoadString("Strings\\UI:Carpenter_Upgrade", new object[]
                {
                    new BluePrint(this.CurrentBlueprint.nameOfBuildingToUpgrade).displayName
                });
                return;
            }
            if (this.demolishButton.containsPoint(x, y))
            {
                this.hoverText = Game1.content.LoadString("Strings\\UI:Carpenter_Demolish", new object[0]);
                return;
            }
            if (this.moveButton.containsPoint(x, y))
            {
                this.hoverText = Game1.content.LoadString("Strings\\UI:Carpenter_MoveBuildings", new object[0]);
                return;
            }
            if (this.okButton.containsPoint(x, y) )
            {
                this.hoverText = Game1.content.LoadString("Strings\\UI:Carpenter_Build", new object[0]);
                return;
            }
            this.hoverText = "";
        }

        public override bool readyToClose()
        {
            return base.readyToClose() && this.buildingToMove == null;
        }

        public override void receiveGamePadButton(Buttons b)
        {
            base.receiveGamePadButton(b);
            if (!this.onFarm && b == Buttons.LeftTrigger)
            {
                this.currentBlueprintIndex--;
                if (this.currentBlueprintIndex < 0)
                {
                    this.currentBlueprintIndex = this.blueprints.Count - 1;
                }
                this.setNewActiveBlueprint();
                Game1.playSound("shwip");
            }
            if (!this.onFarm && b == Buttons.RightTrigger)
            {
                this.currentBlueprintIndex = (this.currentBlueprintIndex + 1) % this.blueprints.Count;
                this.setNewActiveBlueprint();
                Game1.playSound("shwip");
            }
        }

        public override void receiveKeyPress(Keys key)
        {
            KeyboardState newState = Keyboard.GetState();

            // Is the SPACE key down?
            if (newState.IsKeyDown(Keys.T))
            {
                if (flipFlop == false)
                {
                    if (this.drawBG) this.drawBG = false;
                    else this.drawBG = true;

                    flipFlop = true;
                }
            }
            
            if (this.freeze)
            {
                return;
            }
            if (!this.onFarm)
            {
                base.receiveKeyPress(key);
            }
            if (!Game1.globalFade && this.onFarm)
            {
                if (Game1.options.doesInputListContain(Game1.options.menuButton, key) && this.readyToClose())
                {
                    Game1.globalFadeToBlack(new Game1.afterFadeFunction(this.returnToCarpentryMenu), 0.02f);
                    return;
                }
                if (!Game1.options.SnappyMenus)
                {
                    if (Game1.options.doesInputListContain(Game1.options.moveDownButton, key))
                    {
                        Game1.panScreen(0, 4);
                        return;
                    }
                    if (Game1.options.doesInputListContain(Game1.options.moveRightButton, key))
                    {
                        Game1.panScreen(4, 0);
                        return;
                    }
                    if (Game1.options.doesInputListContain(Game1.options.moveUpButton, key))
                    {
                        Game1.panScreen(0, -4);
                        return;
                    }
                    if (Game1.options.doesInputListContain(Game1.options.moveLeftButton, key))
                    {
                        Game1.panScreen(-4, 0);
                    }
                }
            }
        }

        public override void update(GameTime time)
        {
            base.update(time);
            KeyboardState newState = Keyboard.GetState();
            if (flipFlop == true && newState.IsKeyUp(Keys.T))
            {
                flipFlop = false;
            }

            if (this.onFarm && !Game1.globalFade)
            {
                int num = Game1.getOldMouseX() + Game1.viewport.X;
                int num2 = Game1.getOldMouseY() + Game1.viewport.Y;
                if (num - Game1.viewport.X < Game1.tileSize)
                {
                    Game1.panScreen(-8, 0);
                }
                else if (num - (Game1.viewport.X + Game1.viewport.Width) >= -Game1.tileSize * 2)
                {
                    Game1.panScreen(8, 0);
                }
                if (num2 - Game1.viewport.Y < Game1.tileSize)
                {
                    Game1.panScreen(0, -8);
                }
                else if (num2 - (Game1.viewport.Y + Game1.viewport.Height) >= -Game1.tileSize)
                {
                    Game1.panScreen(0, 8);
                }
                Keys[] pressedKeys = Game1.oldKBState.GetPressedKeys();
                for (int i = 0; i < pressedKeys.Length; i++)
                {
                    Keys key = pressedKeys[i];
                    this.receiveKeyPress(key);
                }
            }
        }

        

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (this.freeze)
            {
                return;
            }
            if (!this.onFarm)
            {
                base.receiveLeftClick(x, y, playSound);
            }
            if (this.cancelButton.containsPoint(x, y))
            {
                if (!this.onFarm)
                {
                    base.exitThisMenu(true);
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
                    Game1.globalFadeToBlack(new Game1.afterFadeFunction(this.returnToCarpentryMenu), 0.02f);
                    Game1.playSound("smallSelect");
                    return;
                }
            }
            if (!this.onFarm && this.backButton.containsPoint(x, y))
            {
                this.currentBlueprintIndex--;
                if (this.currentBlueprintIndex < 0)
                {
                    this.currentBlueprintIndex = this.blueprints.Count - 1;
                }
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
            if (!this.onFarm && this.demolishButton.containsPoint(x, y))
            {
                Game1.globalFadeToBlack(new Game1.afterFadeFunction(this.setUpForBuildingPlacement), 0.02f);
                Game1.playSound("smallSelect");
                this.onFarm = true;
                this.demolishing = true;
            }
            if (!this.onFarm && this.moveButton.containsPoint(x, y))
            {
                Game1.globalFadeToBlack(new Game1.afterFadeFunction(this.setUpForBuildingPlacement), 0.02f);
                Game1.playSound("smallSelect");
                this.onFarm = true;
                this.moving = true;
            }
            if (this.okButton.containsPoint(x, y) && !this.onFarm && Game1.player.money >= this.price )
            {
                Game1.globalFadeToBlack(new Game1.afterFadeFunction(this.setUpForBuildingPlacement), 0.02f);
                Game1.playSound("smallSelect");
                this.onFarm = true;
            }
            if (this.onFarm && !this.freeze && !Game1.globalFade)
            {
                if (this.demolishing)
                {
                    Building buildingAt = ((Farm)Game1.getLocationFromName("Farm")).getBuildingAt(new Vector2((float)((Game1.viewport.X + Game1.getOldMouseX()) / Game1.tileSize), (float)((Game1.viewport.Y + Game1.getOldMouseY()) / Game1.tileSize)));
                    if (buildingAt != null && (buildingAt.daysOfConstructionLeft > 0 || buildingAt.daysUntilUpgrade > 0))
                    {
                        Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_DuringConstruction", new object[0]), Color.Red, 3500f));
                        return;
                    }
                    if (buildingAt != null && buildingAt.indoors != null && buildingAt.indoors is AnimalHouse && (buildingAt.indoors as AnimalHouse).animalsThatLiveHere.Count > 0)
                    {
                        Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_AnimalsHere", new object[0]), Color.Red, 3500f));
                        return;
                    }
                    if (buildingAt != null && ((Farm)Game1.getLocationFromName("Farm")).destroyStructure(buildingAt))
                    {
                        int arg_366_0 = buildingAt.tileY;
                        int arg_36D_0 = buildingAt.tilesHigh;
                        Game1.flashAlpha = 1f;
                        buildingAt.showDestroyedAnimation(Game1.getFarm());
                        Game1.playSound("explosion");
                        Utility.spreadAnimalsAround(buildingAt, (Farm)Game1.getLocationFromName("Farm"));
                        DelayedAction.fadeAfterDelay(new Game1.afterFadeFunction(this.returnToCarpentryMenu), 1500);
                        this.freeze = true;
                    }
                    return;
                }
                else if (this.upgrading)
                {
                    Building buildingAt2 = ((Farm)Game1.getLocationFromName("Farm")).getBuildingAt(new Vector2((float)((Game1.viewport.X + Game1.getOldMouseX()) / Game1.tileSize), (float)((Game1.viewport.Y + Game1.getOldMouseY()) / Game1.tileSize)));
                    if (buildingAt2 != null && this.CurrentBlueprint.name != null && buildingAt2.buildingType.Equals(this.CurrentBlueprint.nameOfBuildingToUpgrade))
                    {

                        buildingAt2.daysUntilUpgrade = 2;
                        buildingAt2.showUpgradeAnimation(Game1.getFarm());
                        Game1.playSound("axe");
                        DelayedAction.fadeAfterDelay(new Game1.afterFadeFunction(this.returnToCarpentryMenuAfterSuccessfulBuild), 1500);
                        this.freeze = true;
                        return;
                    }
                    if (buildingAt2 != null)
                    {
                        Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantUpgrade_BuildingType", new object[0]), Color.Red, 3500f));
                    }
                    return;
                }
                else if (this.moving)
                {
                    if (this.buildingToMove == null)
                    {
                        this.buildingToMove = ((Farm)Game1.getLocationFromName("Farm")).getBuildingAt(new Vector2((float)((Game1.viewport.X + Game1.getMouseX()) / Game1.tileSize), (float)((Game1.viewport.Y + Game1.getMouseY()) / Game1.tileSize)));
                        if (this.buildingToMove != null)
                        {
                            if (this.buildingToMove.daysOfConstructionLeft > 0)
                            {
                                this.buildingToMove = null;
                                return;
                            }
                            ((Farm)Game1.getLocationFromName("Farm")).buildings.Remove(this.buildingToMove);
                            Game1.playSound("axchop");
                        }
                        return;
                    }
                    if (((Farm)Game1.getLocationFromName("Farm")).buildStructure(this.buildingToMove, new Vector2((float)((Game1.viewport.X + Game1.getMouseX()) / Game1.tileSize), (float)((Game1.viewport.Y + Game1.getMouseY()) / Game1.tileSize)), false, Game1.player))
                    {
                        this.buildingToMove = null;
                        Game1.playSound("axchop");
                        DelayedAction.playSoundAfterDelay("dirtyHit", 50);
                        DelayedAction.playSoundAfterDelay("dirtyHit", 150);
                        return;
                    }
                    Game1.playSound("cancel");
                    return;
                }
                else
                {
                    /*
                    if (this.tryToBuild())
                    {
                        this.CurrentBlueprint.consumeResources();
                        DelayedAction.fadeAfterDelay(new Game1.afterFadeFunction(this.returnToCarpentryMenuAfterSuccessfulBuild), 2000);
                        this.freeze = true;
                        return;
                    }
                    */
                    Game1.addHUDMessage(new HUDMessage(Game1.currentCursorTile.ToString(), Color.Red, 3500f));
                    Game1.currentLocation.removeObject(Game1.currentCursorTile, false);
                    //Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantBuild", new object[0]), Color.Red, 3500f));
                }
            }
        }

        public bool tryToBuild()
        {
            return ((Farm)Game1.getLocationFromName("Farm")).buildStructure(this.CurrentBlueprint, new Vector2((float)((Game1.viewport.X + Game1.getOldMouseX()) / Game1.tileSize), (float)((Game1.viewport.Y + Game1.getOldMouseY()) / Game1.tileSize)), false, Game1.player, this.magicalConstruction);
        }

        public void returnToCarpentryMenu()
        {
            Game1.currentLocation.cleanupBeforePlayerExit();
            Game1.currentLocation = Game1.player.currentLocation;
            Game1.currentLocation.resetForPlayerEntry();
            Game1.globalFadeToClear(null, 0.02f);
            this.onFarm = false;
            this.resetBounds();
            this.upgrading = false;
            this.moving = false;
            this.freeze = false;
            Game1.displayHUD = true;
            Game1.viewportFreeze = false;
            Game1.viewport.Location = new Location(5 * Game1.tileSize, 24 * Game1.tileSize);
            this.drawBG = true;
            this.demolishing = false;
            Game1.displayFarmer = true;
        }

        public void returnToCarpentryMenuAfterSuccessfulBuild()
        {
            Game1.currentLocation.cleanupBeforePlayerExit();
            Game1.currentLocation = Game1.getLocationFromName(this.magicalConstruction ? "WizardHouse" : "ScienceHouse");
            Game1.currentLocation.resetForPlayerEntry();
            Game1.globalFadeToClear(new Game1.afterFadeFunction(this.robinConstructionMessage), 0.02f);
            Game1.displayHUD = true;
            Game1.viewportFreeze = false;
            Game1.viewport.Location = new Location(5 * Game1.tileSize, 24 * Game1.tileSize);
            this.freeze = true;
            Game1.displayFarmer = true;
        }

        public void robinConstructionMessage()
        {
            base.exitThisMenu(true);
            Game1.player.forceCanMove();
            if (!this.magicalConstruction)
            {
                string text = "Data\\ExtraDialogue:Robin_" + (this.upgrading ? "Upgrade" : "New") + "Construction";
                if (Utility.isFestivalDay(Game1.dayOfMonth + 1, Game1.currentSeason))
                {
                    text += "_Festival";
                }
                Game1.drawDialogue(Game1.getCharacterFromName("Robin", false), Game1.content.LoadString(text, new object[]
                {
                    (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.de) ? this.CurrentBlueprint.displayName : this.CurrentBlueprint.displayName.ToLower(),
                    (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.de) ? this.CurrentBlueprint.displayName.Split(new char[]
                    {
                        ' '
                    }).Last<string>().Split(new char[]
                    {
                        '-'
                    }).Last<string>() : ((LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.pt || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.es) ? this.CurrentBlueprint.displayName.ToLower().Split(new char[]
                    {
                        ' '
                    }).First<string>() : this.CurrentBlueprint.displayName.ToLower().Split(new char[]
                    {
                        ' '
                    }).Last<string>())
                }));
            }
        }

        public override bool overrideSnappyMenuCursorMovementBan()
        {
            return this.onFarm;
        }

        public void setUpForBuildingPlacement()
        {
            Game1.currentLocation.cleanupBeforePlayerExit();
            this.hoverText = "";
            Game1.currentLocation = Game1.getLocationFromName("Farm");
            Game1.currentLocation.resetForPlayerEntry();
            Game1.globalFadeToClear(null, 0.02f);
            this.onFarm = true;
            this.cancelButton.bounds.X = Game1.viewport.Width - Game1.tileSize * 2;
            this.cancelButton.bounds.Y = Game1.viewport.Height - Game1.tileSize * 2;
            Game1.displayHUD = false;
            Game1.viewportFreeze = true;
            Game1.viewport.Location = new Location(49 * Game1.tileSize, 5 * Game1.tileSize);
            Game1.panScreen(0, 0);
            this.drawBG = false;
            this.freeze = false;
            Game1.displayFarmer = false;
            if (!this.demolishing && this.CurrentBlueprint.nameOfBuildingToUpgrade != null && this.CurrentBlueprint.nameOfBuildingToUpgrade.Length > 0 && !this.moving)
            {
                this.upgrading = true;
            }
        }

        public override void gameWindowSizeChanged(Microsoft.Xna.Framework.Rectangle oldBounds, Microsoft.Xna.Framework.Rectangle newBounds)
        {
            this.resetBounds();
        }

        public override void draw(SpriteBatch b)
        {
          if (this.drawBG)
            {
                b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.5f);
            }
            if (Game1.globalFade || this.freeze)
            {
                return;
            }
            if (!this.onFarm)
            {
                base.draw(b);
                IClickableMenu.drawTextureBox(b, this.xPositionOnScreen - Game1.tileSize * 3 / 2, this.yPositionOnScreen - Game1.tileSize / 4, this.maxWidthOfBuildingViewer + Game1.tileSize, this.maxHeightOfBuildingViewer + Game1.tileSize, this.magicalConstruction ? Color.RoyalBlue : Color.White);
                this.currentBuilding.drawInMenu(b, this.xPositionOnScreen + this.maxWidthOfBuildingViewer / 2 - this.currentBuilding.tilesWide * Game1.tileSize / 2 - Game1.tileSize, this.yPositionOnScreen + this.maxHeightOfBuildingViewer / 2 - this.currentBuilding.getSourceRectForMenu().Height * Game1.pixelZoom / 2);
                if (this.CurrentBlueprint.isUpgrade())
                {
                    this.upgradeIcon.draw(b);
                }
                string text = " Deluxe  Barn   ";
                SpriteText.drawStringWithScrollBackground(b, this.buildingName, this.xPositionOnScreen + this.maxWidthOfBuildingViewer - IClickableMenu.spaceToClearSideBorder - Game1.tileSize / 4 + Game1.tileSize + ((this.width - (this.maxWidthOfBuildingViewer + Game1.tileSize * 2)) / 2 - SpriteText.getWidthOfString(text) / 2), this.yPositionOnScreen, text, 1f, -1);
                IClickableMenu.drawTextureBox(b, this.xPositionOnScreen + this.maxWidthOfBuildingViewer - Game1.tileSize / 4, this.yPositionOnScreen + Game1.tileSize * 5 / 4, this.maxWidthOfDescription + Game1.tileSize, this.maxWidthOfDescription + Game1.tileSize * 3 / 2, this.magicalConstruction ? Color.RoyalBlue : Color.White);
                if (this.magicalConstruction)
                {
                    Utility.drawTextWithShadow(b, Game1.parseText(this.buildingDescription, Game1.dialogueFont, this.maxWidthOfDescription + Game1.tileSize / 2), Game1.dialogueFont, new Vector2((float)(this.xPositionOnScreen + this.maxWidthOfDescription + Game1.tileSize - Game1.pixelZoom), (float)(this.yPositionOnScreen + Game1.tileSize * 5 / 4 + Game1.pixelZoom * 4 + Game1.pixelZoom)), Game1.textColor * 0.25f, 1f, -1f, -1, -1, 0f, 3);
                    Utility.drawTextWithShadow(b, Game1.parseText(this.buildingDescription, Game1.dialogueFont, this.maxWidthOfDescription + Game1.tileSize / 2), Game1.dialogueFont, new Vector2((float)(this.xPositionOnScreen + this.maxWidthOfDescription + Game1.tileSize - 1), (float)(this.yPositionOnScreen + Game1.tileSize * 5 / 4 + Game1.pixelZoom * 4 + Game1.pixelZoom)), Game1.textColor * 0.25f, 1f, -1f, -1, -1, 0f, 3);
                }
                Utility.drawTextWithShadow(b, Game1.parseText(this.buildingDescription, Game1.dialogueFont, this.maxWidthOfDescription + Game1.tileSize / 2), Game1.dialogueFont, new Vector2((float)(this.xPositionOnScreen + this.maxWidthOfDescription + Game1.tileSize), (float)(this.yPositionOnScreen + Game1.tileSize * 5 / 4 + Game1.pixelZoom * 4)), this.magicalConstruction ? Color.PaleGoldenrod : Game1.textColor, 1f, -1f, -1, -1, this.magicalConstruction ? 0f : 0.25f, 3);
                Vector2 vector = new Vector2((float)(this.xPositionOnScreen + this.maxWidthOfDescription + Game1.tileSize / 4 + Game1.tileSize), (float)(this.yPositionOnScreen + Game1.tileSize * 4 + Game1.tileSize / 2));
                SpriteText.drawString(b, "$", (int)vector.X, (int)vector.Y, 999999, -1, 999999, 1f, 0.88f, false, -1, "", -1);
                if (this.magicalConstruction)
                {
                    Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\StringsFromCSFiles:LoadGameMenu.cs.11020", new object[]
                    {
                        this.price
                    }), Game1.dialogueFont, new Vector2(vector.X + (float)Game1.tileSize, vector.Y + (float)(Game1.pixelZoom * 2)), Game1.textColor * 0.5f, 1f, -1f, -1, -1, this.magicalConstruction ? 0f : 0.25f, 3);
                    Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\StringsFromCSFiles:LoadGameMenu.cs.11020", new object[]
                    {
                        this.price
                    }), Game1.dialogueFont, new Vector2(vector.X + (float)Game1.tileSize + (float)Game1.pixelZoom - 1f, vector.Y + (float)(Game1.pixelZoom * 2)), Game1.textColor * 0.25f, 1f, -1f, -1, -1, this.magicalConstruction ? 0f : 0.25f, 3);
                }
                Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\StringsFromCSFiles:LoadGameMenu.cs.11020", new object[]
                {
                    this.price
                }), Game1.dialogueFont, new Vector2(vector.X + (float)Game1.tileSize + (float)Game1.pixelZoom, vector.Y + (float)Game1.pixelZoom), (Game1.player.money >= this.price) ? (this.magicalConstruction ? Color.PaleGoldenrod : Game1.textColor) : Color.Red, 1f, -1f, -1, -1, this.magicalConstruction ? 0f : 0.25f, 3);
                vector.X -= (float)(Game1.tileSize / 4);
                vector.Y -= (float)(Game1.tileSize / 3);
                foreach (Item current in this.ingredients)
                {
                    vector.Y += (float)(Game1.tileSize + Game1.pixelZoom);
                    current.drawInMenu(b, vector, 1f);
                    bool flag = !(current is StardewValley.Object) || Game1.player.hasItemInInventory((current as StardewValley.Object).parentSheetIndex, current.Stack, 0);
                    if (this.magicalConstruction)
                    {
                        Utility.drawTextWithShadow(b, current.DisplayName, Game1.dialogueFont, new Vector2(vector.X + (float)Game1.tileSize + (float)(Game1.pixelZoom * 3), vector.Y + (float)(Game1.pixelZoom * 6)), Game1.textColor * 0.25f, 1f, -1f, -1, -1, this.magicalConstruction ? 0f : 0.25f, 3);
                        Utility.drawTextWithShadow(b, current.DisplayName, Game1.dialogueFont, new Vector2(vector.X + (float)Game1.tileSize + (float)(Game1.pixelZoom * 4) - 1f, vector.Y + (float)(Game1.pixelZoom * 6)), Game1.textColor * 0.25f, 1f, -1f, -1, -1, this.magicalConstruction ? 0f : 0.25f, 3);
                    }
                    Utility.drawTextWithShadow(b, current.DisplayName, Game1.dialogueFont, new Vector2(vector.X + (float)Game1.tileSize + (float)(Game1.pixelZoom * 4), vector.Y + (float)(Game1.pixelZoom * 5)), flag ? (this.magicalConstruction ? Color.PaleGoldenrod : Game1.textColor) : Color.Red, 1f, -1f, -1, -1, this.magicalConstruction ? 0f : 0.25f, 3);
                }
                this.backButton.draw(b);
                this.forwardButton.draw(b);
                this.okButton.draw(b,  Color.White , 0.88f);
                this.demolishButton.draw(b);
                this.moveButton.draw(b);
            }
            else
            {
                string s = this.upgrading ? Game1.content.LoadString("Strings\\UI:Carpenter_SelectBuilding_Upgrade", new object[]
                {
                    new BluePrint(this.CurrentBlueprint.nameOfBuildingToUpgrade).displayName
                }) : (this.demolishing ? Game1.content.LoadString("Strings\\UI:Carpenter_SelectBuilding_Demolish", new object[0]) : Game1.content.LoadString("Strings\\UI:Carpenter_ChooseLocation", new object[0]));
                SpriteText.drawStringWithScrollBackground(b, s, Game1.viewport.Width / 2 - SpriteText.getWidthOfString(s) / 2, Game1.tileSize / 4, "", 1f, -1);
                if (!this.upgrading && !this.demolishing && !this.moving)
                {
                    Vector2 vector2 = new Vector2((float)((Game1.viewport.X + Game1.getOldMouseX()) / Game1.tileSize), (float)((Game1.viewport.Y + Game1.getOldMouseY()) / Game1.tileSize));
                    for (int i = 0; i < this.CurrentBlueprint.tilesHeight; i++)
                    {
                        for (int j = 0; j < this.CurrentBlueprint.tilesWidth; j++)
                        {
                            int num = this.CurrentBlueprint.getTileSheetIndexForStructurePlacementTile(j, i);
                            Vector2 vector3 = new Vector2(vector2.X + (float)j, vector2.Y + (float)i);
                           // if (!(Game1.currentLocation as BuildableGameLocation).isBuildable(vector3))
                           // {
                           //     num++;
                           // }
                            b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, vector3 * (float)Game1.tileSize), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(194 + num * 16, 388, 16, 16)), Color.White, 0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, 0.999f);
                        }
                    }
                }
                else if (this.moving && this.buildingToMove != null)
                {
                    Vector2 vector4 = new Vector2((float)((Game1.viewport.X + Game1.getOldMouseX()) / Game1.tileSize), (float)((Game1.viewport.Y + Game1.getOldMouseY()) / Game1.tileSize));
                    for (int k = 0; k < this.buildingToMove.tilesHigh; k++)
                    {
                        for (int l = 0; l < this.buildingToMove.tilesWide; l++)
                        {
                            int num2 = this.buildingToMove.getTileSheetIndexForStructurePlacementTile(l, k);
                            Vector2 vector5 = new Vector2(vector4.X + (float)l, vector4.Y + (float)k);
                            if (!(Game1.currentLocation as BuildableGameLocation).isBuildable(vector5))
                            {
                                num2++;
                            }
                            b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, vector5 * (float)Game1.tileSize), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(194 + num2 * 16, 388, 16, 16)), Color.White, 0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, 0.999f);
                        }
                    }
                }
            }
            this.cancelButton.draw(b);
            base.drawMouse(b);
            if (this.hoverText.Length > 0)
            {
                IClickableMenu.drawHoverText(b, this.hoverText, Game1.dialogueFont, 0, 0, -1, null, -1, null, null, 0, -1, -1, -1, -1, 1f, null);
            }
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            if (this.freeze)
            {
                return;
            }
            if (!this.onFarm)
            {
                base.receiveLeftClick(x, y, playSound);
            }
            if (this.cancelButton.containsPoint(x, y))
            {
                if (!this.onFarm)
                {
                    base.exitThisMenu(true);
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
                    Game1.globalFadeToBlack(new Game1.afterFadeFunction(this.returnToCarpentryMenu), 0.02f);
                    Game1.playSound("smallSelect");
                    return;
                }
            }
            if (!this.onFarm && this.backButton.containsPoint(x, y))
            {
                this.currentBlueprintIndex--;
                if (this.currentBlueprintIndex < 0)
                {
                    this.currentBlueprintIndex = this.blueprints.Count - 1;
                }
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
            if (!this.onFarm && this.demolishButton.containsPoint(x, y))
            {
                Game1.globalFadeToBlack(new Game1.afterFadeFunction(this.setUpForBuildingPlacement), 0.02f);
                Game1.playSound("smallSelect");
                this.onFarm = true;
                this.demolishing = true;
            }
            if (!this.onFarm && this.moveButton.containsPoint(x, y))
            {
                Game1.globalFadeToBlack(new Game1.afterFadeFunction(this.setUpForBuildingPlacement), 0.02f);
                Game1.playSound("smallSelect");
                this.onFarm = true;
                this.moving = true;
            }
            if (this.okButton.containsPoint(x, y) && !this.onFarm && Game1.player.money >= this.price )
            {
                Game1.globalFadeToBlack(new Game1.afterFadeFunction(this.setUpForBuildingPlacement), 0.02f);
                Game1.playSound("smallSelect");
                this.onFarm = true;
            }
            if (this.onFarm && !this.freeze && !Game1.globalFade)
            {
                if (this.demolishing)
                {
                    Building buildingAt = ((Farm)Game1.getLocationFromName("Farm")).getBuildingAt(new Vector2((float)((Game1.viewport.X + Game1.getOldMouseX()) / Game1.tileSize), (float)((Game1.viewport.Y + Game1.getOldMouseY()) / Game1.tileSize)));
                    if (buildingAt != null && (buildingAt.daysOfConstructionLeft > 0 || buildingAt.daysUntilUpgrade > 0))
                    {
                        Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_DuringConstruction", new object[0]), Color.Red, 3500f));
                        return;
                    }
                    if (buildingAt != null && buildingAt.indoors != null && buildingAt.indoors is AnimalHouse && (buildingAt.indoors as AnimalHouse).animalsThatLiveHere.Count > 0)
                    {
                        Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_AnimalsHere", new object[0]), Color.Red, 3500f));
                        return;
                    }
                    if (buildingAt != null && ((Farm)Game1.getLocationFromName("Farm")).destroyStructure(buildingAt))
                    {
                        int arg_366_0 = buildingAt.tileY;
                        int arg_36D_0 = buildingAt.tilesHigh;
                        Game1.flashAlpha = 1f;
                        buildingAt.showDestroyedAnimation(Game1.getFarm());
                        Game1.playSound("explosion");
                        Utility.spreadAnimalsAround(buildingAt, (Farm)Game1.getLocationFromName("Farm"));
                        DelayedAction.fadeAfterDelay(new Game1.afterFadeFunction(this.returnToCarpentryMenu), 1500);
                        this.freeze = true;
                    }
                    return;
                }
                else if (this.upgrading)
                {
                    Building buildingAt2 = ((Farm)Game1.getLocationFromName("Farm")).getBuildingAt(new Vector2((float)((Game1.viewport.X + Game1.getOldMouseX()) / Game1.tileSize), (float)((Game1.viewport.Y + Game1.getOldMouseY()) / Game1.tileSize)));
                    if (buildingAt2 != null && this.CurrentBlueprint.name != null && buildingAt2.buildingType.Equals(this.CurrentBlueprint.nameOfBuildingToUpgrade))
                    {
            
                        buildingAt2.daysUntilUpgrade = 2;
                        buildingAt2.showUpgradeAnimation(Game1.getFarm());
                        Game1.playSound("axe");
                        DelayedAction.fadeAfterDelay(new Game1.afterFadeFunction(this.returnToCarpentryMenuAfterSuccessfulBuild), 1500);
                        this.freeze = true;
                        return;
                    }
                    if (buildingAt2 != null)
                    {
                        Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantUpgrade_BuildingType", new object[0]), Color.Red, 3500f));
                    }
                    return;
                }
                else if (this.moving)
                {
                    if (this.buildingToMove == null)
                    {
                        this.buildingToMove = ((Farm)Game1.getLocationFromName("Farm")).getBuildingAt(new Vector2((float)((Game1.viewport.X + Game1.getMouseX()) / Game1.tileSize), (float)((Game1.viewport.Y + Game1.getMouseY()) / Game1.tileSize)));
                        if (this.buildingToMove != null)
                        {
                            if (this.buildingToMove.daysOfConstructionLeft > 0)
                            {
                                this.buildingToMove = null;
                                return;
                            }
                            ((Farm)Game1.getLocationFromName("Farm")).buildings.Remove(this.buildingToMove);
                            Game1.playSound("axchop");
                        }
                        return;
                    }
                    if (((Farm)Game1.getLocationFromName("Farm")).buildStructure(this.buildingToMove, new Vector2((float)((Game1.viewport.X + Game1.getMouseX()) / Game1.tileSize), (float)((Game1.viewport.Y + Game1.getMouseY()) / Game1.tileSize)), false, Game1.player))
                    {
                        this.buildingToMove = null;
                        Game1.playSound("axchop");
                        DelayedAction.playSoundAfterDelay("dirtyHit", 50);
                        DelayedAction.playSoundAfterDelay("dirtyHit", 150);
                        return;
                    }
                    Game1.playSound("cancel");
                    return;
                }
                else
                {
                    /*
                    if (this.tryToBuild())
                    {
                        this.CurrentBlueprint.consumeResources();
                        DelayedAction.fadeAfterDelay(new Game1.afterFadeFunction(this.returnToCarpentryMenuAfterSuccessfulBuild), 2000);
                        this.freeze = true;
                        return;
                    }
                    */
                    Game1.addHUDMessage(new HUDMessage(Game1.currentCursorTile.ToString(), Color.Red, 3500f));
                    Game1.currentLocation.removeObject(Game1.currentCursorTile, false);
                    //Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantBuild", new object[0]), Color.Red, 3500f));
                }
            }
        }
    }
}
