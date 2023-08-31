/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DiogoAlbano/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.BellsAndWhistles;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Objects;
using xTile.Dimensions;
using StardewValley;
using StardewValley.Menus;
using Object = StardewValley.Object;

namespace NovoMundo.Farm1
{
    public class Farm1_Builder : IClickableMenu
    {
        public const int region_backButton = 101;

        public const int region_forwardButton = 102;

        public const int region_upgradeIcon = 103;

        public const int region_demolishButton = 104;

        public const int region_moveBuitton = 105;

        public const int region_okButton = 106;

        public const int region_cancelButton = 107;

        public const int region_paintButton = 108;

        public int maxWidthOfBuildingViewer = 448;

        public int maxHeightOfBuildingViewer = 512;

        public int maxWidthOfDescription = 416;

        private List<BluePrint> blueprints;

        private int currentBlueprintIndex;

        public ClickableTextureComponent okButton;

        public ClickableTextureComponent cancelButton;

        public ClickableTextureComponent backButton;

        public ClickableTextureComponent forwardButton;

        public ClickableTextureComponent upgradeIcon;

        public ClickableTextureComponent demolishButton;

        public ClickableTextureComponent moveButton;

        public ClickableTextureComponent paintButton;

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

        private bool painting;

        protected BluePrint _demolishCheckBlueprint;

        private string hoverText = "";

        public bool readOnly
        {
            set
            {
                if (value)
                {
                    upgradeIcon.visible = false;
                    demolishButton.visible = false;
                    moveButton.visible = false;
                    okButton.visible = false;
                    paintButton.visible = false;
                    cancelButton.leftNeighborID = 102;
                }
            }
        }

        public BluePrint CurrentBlueprint => blueprints[currentBlueprintIndex];
        public static NMFarm1 getFarm1()
        {
            return Game1.getLocationFromName("NMFarm1") as NMFarm1;
        }

        public Farm1_Builder(bool magicalConstruction = false)
        {
            this.magicalConstruction = magicalConstruction;
            Game1.player.forceCanMove();
            resetBounds();
            blueprints = new List<BluePrint>
            {
                new BluePrint("Coop"),
                new BluePrint("Barn"),
                new BluePrint("Well"),
                new BluePrint("Mill"),
                new BluePrint("Shed"),
        };
            if (getFarm1().isBuildingConstructed("Coop"))
            {
                blueprints.Add(new BluePrint("Big Coop"));
            }
            if (getFarm1().isBuildingConstructed("Big Coop"))
            {
                blueprints.Add(new BluePrint("Deluxe Coop"));
            }
            if (getFarm1().isBuildingConstructed("Barn"))
            {
                blueprints.Add(new BluePrint("Big Barn"));
            }
            if (getFarm1().isBuildingConstructed("Big Barn"))
            {
                blueprints.Add(new BluePrint("Deluxe Barn"));
            }
            if (getFarm1().isBuildingConstructed("Shed"))
            {
                blueprints.Add(new BluePrint("Big Shed"));
            }
            setNewActiveBlueprint();
            if (Game1.options.SnappyMenus)
            {
                populateClickableComponentList();
                snapToDefaultClickableComponent();
            }
        }

        public override bool shouldClampGamePadCursor()
        {
            return onFarm;
        }

        public override void snapToDefaultClickableComponent()
        {
            currentlySnappedComponent = getComponentWithID(107);
            snapCursorToCurrentSnappedComponent();
        }

        private void resetBounds()
        {
            xPositionOnScreen = Game1.uiViewport.Width / 2 - maxWidthOfBuildingViewer - IClickableMenu.spaceToClearSideBorder;
            yPositionOnScreen = Game1.uiViewport.Height / 2 - maxHeightOfBuildingViewer / 2 - IClickableMenu.spaceToClearTopBorder + 32;
            width = maxWidthOfBuildingViewer + maxWidthOfDescription + IClickableMenu.spaceToClearSideBorder * 2 + 64;
            height = maxHeightOfBuildingViewer + IClickableMenu.spaceToClearTopBorder;
            initialize(xPositionOnScreen, yPositionOnScreen, width, height, showUpperRightCloseButton: true);
            okButton = new ClickableTextureComponent("OK", new Microsoft.Xna.Framework.Rectangle(xPositionOnScreen + width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - 192 - 12, yPositionOnScreen + maxHeightOfBuildingViewer + 64, 64, 64), null, null, Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(366, 373, 16, 16), 4f)
            {
                myID = 106,
                rightNeighborID = 104,
                leftNeighborID = 105
            };
            cancelButton = new ClickableTextureComponent("OK", new Microsoft.Xna.Framework.Rectangle(xPositionOnScreen + width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - 64, yPositionOnScreen + maxHeightOfBuildingViewer + 64, 64, 64), null, null, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 47), 1f)
            {
                myID = 107,
                leftNeighborID = 104
            };
            backButton = new ClickableTextureComponent(new Microsoft.Xna.Framework.Rectangle(xPositionOnScreen + 64, yPositionOnScreen + maxHeightOfBuildingViewer + 64, 48, 44), Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(352, 495, 12, 11), 4f)
            {
                myID = 101,
                rightNeighborID = 102
            };
            forwardButton = new ClickableTextureComponent(new Microsoft.Xna.Framework.Rectangle(xPositionOnScreen + maxWidthOfBuildingViewer - 256 + 16, yPositionOnScreen + maxHeightOfBuildingViewer + 64, 48, 44), Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(365, 495, 12, 11), 4f)
            {
                myID = 102,
                leftNeighborID = 101,
                rightNeighborID = -99998
            };
            demolishButton = new ClickableTextureComponent(Game1.content.LoadString("Strings\\UI:Carpenter_Demolish"), new Microsoft.Xna.Framework.Rectangle(xPositionOnScreen + width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - 128 - 8, yPositionOnScreen + maxHeightOfBuildingViewer + 64 - 4, 64, 64), null, null, Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(348, 372, 17, 17), 4f)
            {
                myID = 104,
                rightNeighborID = 107,
                leftNeighborID = 106
            };
            upgradeIcon = new ClickableTextureComponent(new Microsoft.Xna.Framework.Rectangle(xPositionOnScreen + maxWidthOfBuildingViewer - 128 + 32, yPositionOnScreen + 8, 36, 52), Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(402, 328, 9, 13), 4f)
            {
                myID = 103,
                rightNeighborID = 104,
                leftNeighborID = 105
            };
            moveButton = new ClickableTextureComponent(Game1.content.LoadString("Strings\\UI:Carpenter_MoveBuildings"), new Microsoft.Xna.Framework.Rectangle(xPositionOnScreen + width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - 256 - 20, yPositionOnScreen + maxHeightOfBuildingViewer + 64, 64, 64), null, null, Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(257, 284, 16, 16), 4f)
            {
                myID = 105,
                rightNeighborID = 106,
                leftNeighborID = -99998
            };
            paintButton = new ClickableTextureComponent(Game1.content.LoadString("Strings\\UI:Carpenter_PaintBuildings"), new Microsoft.Xna.Framework.Rectangle(xPositionOnScreen + width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - 320 - 20, yPositionOnScreen + maxHeightOfBuildingViewer + 64, 64, 64), null, null, Game1.mouseCursors2, new Microsoft.Xna.Framework.Rectangle(80, 208, 16, 16), 4f)
            {
                myID = 105,
                rightNeighborID = -99998,
                leftNeighborID = -99998
            };
            bool flag = false;
            bool visible = CanPaintHouse() && HasPermissionsToPaint(null);
            foreach (Building building in getFarm1().buildings)
            {
                if (building.hasCarpenterPermissions())
                {
                    flag = true;
                }

                if (building.CanBePainted() && HasPermissionsToPaint(building))
                {
                    visible = true;
                }
            }

            demolishButton.visible = Game1.IsMasterGame;
            moveButton.visible = Game1.IsMasterGame || Game1.player.team.farmhandsCanMoveBuildings.Value == FarmerTeam.RemoteBuildingPermissions.On || (Game1.player.team.farmhandsCanMoveBuildings.Value == FarmerTeam.RemoteBuildingPermissions.OwnedBuildings && flag);
            paintButton.visible = visible;
            if (magicalConstruction)
            {
                paintButton.visible = false;
            }

            if (!demolishButton.visible)
            {
                upgradeIcon.rightNeighborID = demolishButton.rightNeighborID;
                okButton.rightNeighborID = demolishButton.rightNeighborID;
                cancelButton.leftNeighborID = demolishButton.leftNeighborID;
            }

            if (!moveButton.visible)
            {
                upgradeIcon.leftNeighborID = moveButton.leftNeighborID;
                forwardButton.rightNeighborID = -99998;
                okButton.leftNeighborID = moveButton.leftNeighborID;
            }
        }

        public void setNewActiveBlueprint()
        {
            if (blueprints[currentBlueprintIndex].name.Contains("Coop"))
            {
                currentBuilding = new Coop(blueprints[currentBlueprintIndex], Vector2.Zero);
            }
            else if (blueprints[currentBlueprintIndex].name.Contains("Barn"))
            {
                currentBuilding = new Barn(blueprints[currentBlueprintIndex], Vector2.Zero);
            }
            else if (blueprints[currentBlueprintIndex].name.Contains("Mill"))
            {
                currentBuilding = new Mill(blueprints[currentBlueprintIndex], Vector2.Zero);
            }
            else if (blueprints[currentBlueprintIndex].name.Contains("Junimo Hut"))
            {
                currentBuilding = new JunimoHut(blueprints[currentBlueprintIndex], Vector2.Zero);
            }
            else if (blueprints[currentBlueprintIndex].name.Contains("Shipping Bin"))
            {
                currentBuilding = new ShippingBin(blueprints[currentBlueprintIndex], Vector2.Zero);
            }
            else if (blueprints[currentBlueprintIndex].name.Contains("Fish Pond"))
            {
                currentBuilding = new FishPond(blueprints[currentBlueprintIndex], Vector2.Zero);
            }
            else if (blueprints[currentBlueprintIndex].name.Contains("Greenhouse"))
            {
                currentBuilding = new GreenhouseBuilding(blueprints[currentBlueprintIndex], Vector2.Zero);
            }
            else
            {
                currentBuilding = new Building(blueprints[currentBlueprintIndex], Vector2.Zero);
            }

            price = blueprints[currentBlueprintIndex].moneyRequired;
            ingredients.Clear();
            foreach (KeyValuePair<int, int> item in blueprints[currentBlueprintIndex].itemsRequired)
            {
                ingredients.Add(new Object(item.Key, item.Value));
            }

            buildingDescription = blueprints[currentBlueprintIndex].description;
            buildingName = blueprints[currentBlueprintIndex].displayName;
        }

        public override void performHoverAction(int x, int y)
        {
            cancelButton.tryHover(x, y);
            base.performHoverAction(x, y);
            if (!onFarm)
            {
                backButton.tryHover(x, y, 1f);
                forwardButton.tryHover(x, y, 1f);
                okButton.tryHover(x, y);
                demolishButton.tryHover(x, y);
                moveButton.tryHover(x, y);
                paintButton.tryHover(x, y);
                if (CurrentBlueprint.isUpgrade() && upgradeIcon.containsPoint(x, y))
                {
                    hoverText = Game1.content.LoadString("Strings\\UI:Carpenter_Upgrade", new BluePrint(CurrentBlueprint.nameOfBuildingToUpgrade).displayName);
                }
                else if (demolishButton.containsPoint(x, y) && CanDemolishThis(CurrentBlueprint))
                {
                    hoverText = Game1.content.LoadString("Strings\\UI:Carpenter_Demolish");
                }
                else if (moveButton.containsPoint(x, y))
                {
                    hoverText = Game1.content.LoadString("Strings\\UI:Carpenter_MoveBuildings");
                }
                else if (okButton.containsPoint(x, y) && CurrentBlueprint.doesFarmerHaveEnoughResourcesToBuild())
                {
                    hoverText = Game1.content.LoadString("Strings\\UI:Carpenter_Build");
                }
                else if (paintButton.containsPoint(x, y))
                {
                    hoverText = paintButton.name;
                }
                else
                {
                    hoverText = "";
                }
            }
            else
            {
                if ((!upgrading && !demolishing && !moving && !painting) || freeze)
                {
                    return;
                }

                NMFarm1 farm = getFarm1();
                Vector2 vector = new Vector2((Game1.viewport.X + Game1.getOldMouseX(ui_scale: false)) / 64, (Game1.viewport.Y + Game1.getOldMouseY(ui_scale: false)) / 64);
                if (painting && farm.GetHouseRect().Contains(Utility.Vector2ToPoint(vector)) && HasPermissionsToPaint(null) && CanPaintHouse())
                {
                    farm.frameHouseColor = Color.Lime;
                }

                foreach (Building building in ((NMFarm1)Game1.getLocationFromName("NMFarm1")).buildings)
                {
                    building.color.Value = Color.White;
                }

                Building buildingAt = ((NMFarm1)Game1.getLocationFromName("NMFarm1")).getBuildingAt(vector);
                if (buildingAt == null)
                {
                    buildingAt = ((NMFarm1)Game1.getLocationFromName("NMFarm1")).getBuildingAt(new Vector2((Game1.viewport.X + Game1.getOldMouseX(ui_scale: false)) / 64, (Game1.viewport.Y + Game1.getOldMouseY(ui_scale: false) + 128) / 64));
                    if (buildingAt == null)
                    {
                        buildingAt = ((NMFarm1)Game1.getLocationFromName("NMFarm1")).getBuildingAt(new Vector2((Game1.viewport.X + Game1.getOldMouseX(ui_scale: false)) / 64, (Game1.viewport.Y + Game1.getOldMouseY(ui_scale: false) + 192) / 64));
                    }
                }

                if (upgrading)
                {
                    if (buildingAt != null && CurrentBlueprint.nameOfBuildingToUpgrade != null && CurrentBlueprint.nameOfBuildingToUpgrade.Equals((string)buildingAt.buildingType))
                    {
                        buildingAt.color.Value = Color.Lime * 0.8f;
                    }
                    else if (buildingAt != null)
                    {
                        buildingAt.color.Value = Color.Red * 0.8f;
                    }
                }
                else if (demolishing)
                {
                    if (buildingAt != null && hasPermissionsToDemolish(buildingAt) && CanDemolishThis(buildingAt))
                    {
                        buildingAt.color.Value = Color.Red * 0.8f;
                    }
                }
                else if (moving)
                {
                    if (buildingAt != null && hasPermissionsToMove(buildingAt))
                    {
                        buildingAt.color.Value = Color.Lime * 0.8f;
                    }
                }
                else if (painting && buildingAt != null && buildingAt.CanBePainted() && HasPermissionsToPaint(buildingAt))
                {
                    buildingAt.color.Value = Color.Lime * 0.8f;
                }
            }
        }

        public bool hasPermissionsToDemolish(Building b)
        {
            if (Game1.IsMasterGame)
            {
                return CanDemolishThis(b);
            }

            return false;
        }

        public bool CanPaintHouse()
        {
            return Game1.MasterPlayer.HouseUpgradeLevel >= 2;
        }

        public bool HasPermissionsToPaint(Building b)
        {
            if (b == null)
            {
                if (Game1.player.UniqueMultiplayerID == Game1.MasterPlayer.UniqueMultiplayerID)
                {
                    return true;
                }

                if (Game1.player.spouse == Game1.MasterPlayer.UniqueMultiplayerID.ToString())
                {
                    return true;
                }

                return false;
            }

            if (b.isCabin && b.indoors.Value is Cabin)
            {
                Farmer owner = (b.indoors.Value as Cabin).owner;
                if (Game1.player.UniqueMultiplayerID == owner.UniqueMultiplayerID)
                {
                    return true;
                }

                if (Game1.player.spouse == owner.UniqueMultiplayerID.ToString())
                {
                    return true;
                }

                return false;
            }

            return true;
        }

        public bool hasPermissionsToMove(Building b)
        {
            if (!getFarm1().greenhouseUnlocked.Value && b is GreenhouseBuilding)
            {
                return false;
            }

            if (Game1.IsMasterGame)
            {
                return true;
            }

            if (Game1.player.team.farmhandsCanMoveBuildings.Value == FarmerTeam.RemoteBuildingPermissions.On)
            {
                return true;
            }

            if (Game1.player.team.farmhandsCanMoveBuildings.Value == FarmerTeam.RemoteBuildingPermissions.OwnedBuildings && b.hasCarpenterPermissions())
            {
                return true;
            }

            return false;
        }

        public override void receiveGamePadButton(Buttons b)
        {
            base.receiveGamePadButton(b);
            if (!onFarm && b == Buttons.LeftTrigger)
            {
                currentBlueprintIndex--;
                if (currentBlueprintIndex < 0)
                {
                    currentBlueprintIndex = blueprints.Count - 1;
                }

                setNewActiveBlueprint();
                Game1.playSound("shwip");
            }

            if (!onFarm && b == Buttons.RightTrigger)
            {
                currentBlueprintIndex = (currentBlueprintIndex + 1) % blueprints.Count;
                setNewActiveBlueprint();
                Game1.playSound("shwip");
            }
        }

        public override void receiveKeyPress(Keys key)
        {
            if (freeze)
            {
                return;
            }

            if (!onFarm)
            {
                base.receiveKeyPress(key);
            }

            if (Game1.IsFading() || !onFarm)
            {
                return;
            }

            if (Game1.options.doesInputListContain(Game1.options.menuButton, key) && readyToClose() && Game1.locationRequest == null)
            {
                returnToCarpentryMenu();
            }
            else if (!Game1.options.SnappyMenus)
            {
                if (Game1.options.doesInputListContain(Game1.options.moveDownButton, key))
                {
                    Game1.panScreen(0, 4);
                }
                else if (Game1.options.doesInputListContain(Game1.options.moveRightButton, key))
                {
                    Game1.panScreen(4, 0);
                }
                else if (Game1.options.doesInputListContain(Game1.options.moveUpButton, key))
                {
                    Game1.panScreen(0, -4);
                }
                else if (Game1.options.doesInputListContain(Game1.options.moveLeftButton, key))
                {
                    Game1.panScreen(-4, 0);
                }
            }
        }

        public override void update(GameTime time)
        {
            base.update(time);
            if (!onFarm || Game1.IsFading())
            {
                return;
            }

            int num = Game1.getOldMouseX(ui_scale: false) + Game1.viewport.X;
            int num2 = Game1.getOldMouseY(ui_scale: false) + Game1.viewport.Y;
            if (num - Game1.viewport.X < 64)
            {
                Game1.panScreen(-8, 0);
            }
            else if (num - (Game1.viewport.X + Game1.viewport.Width) >= -128)
            {
                Game1.panScreen(8, 0);
            }

            if (num2 - Game1.viewport.Y < 64)
            {
                Game1.panScreen(0, -8);
            }
            else if (num2 - (Game1.viewport.Y + Game1.viewport.Height) >= -64)
            {
                Game1.panScreen(0, 8);
            }

            Keys[] pressedKeys = Game1.oldKBState.GetPressedKeys();
            foreach (Keys key in pressedKeys)
            {
                receiveKeyPress(key);
            }

            if (Game1.IsMultiplayer)
            {
                return;
            }

            NMFarm1 farm = getFarm1();
            foreach (FarmAnimal value in farm.animals.Values)
            {
                value.MovePosition(Game1.currentGameTime, Game1.viewport, farm);
            }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (freeze)
            {
                return;
            }

            if (!onFarm)
            {
                base.receiveLeftClick(x, y, playSound);
            }

            if (cancelButton.containsPoint(x, y))
            {
                if (onFarm)
                {
                    if (moving && buildingToMove != null)
                    {
                        Game1.playSound("cancel");
                        return;
                    }

                    returnToCarpentryMenu();
                    Game1.playSound("smallSelect");
                    return;
                }

                exitThisMenu();
                Game1.player.forceCanMove();
                Game1.playSound("bigDeSelect");
            }

            if (!onFarm && backButton.containsPoint(x, y))
            {
                currentBlueprintIndex--;
                if (currentBlueprintIndex < 0)
                {
                    currentBlueprintIndex = blueprints.Count - 1;
                }

                setNewActiveBlueprint();
                Game1.playSound("shwip");
                backButton.scale = backButton.baseScale;
            }

            if (!onFarm && forwardButton.containsPoint(x, y))
            {
                currentBlueprintIndex = (currentBlueprintIndex + 1) % blueprints.Count;
                setNewActiveBlueprint();
                backButton.scale = backButton.baseScale;
                Game1.playSound("shwip");
            }

            if (!onFarm && demolishButton.containsPoint(x, y) && demolishButton.visible && CanDemolishThis(blueprints[currentBlueprintIndex]))
            {
                Game1.globalFadeToBlack(setUpForBuildingPlacement);
                Game1.playSound("smallSelect");
                onFarm = true;
                demolishing = true;
            }

            if (!onFarm && moveButton.containsPoint(x, y) && moveButton.visible)
            {
                Game1.globalFadeToBlack(setUpForBuildingPlacement);
                Game1.playSound("smallSelect");
                onFarm = true;
                moving = true;
            }

            if (!onFarm && paintButton.containsPoint(x, y) && paintButton.visible)
            {
                Game1.globalFadeToBlack(setUpForBuildingPlacement);
                Game1.playSound("smallSelect");
                onFarm = true;
                painting = true;
            }

            if (okButton.containsPoint(x, y) && !onFarm && price >= 0 && Game1.player.Money >= price && blueprints[currentBlueprintIndex].doesFarmerHaveEnoughResourcesToBuild())
            {
                Game1.globalFadeToBlack(setUpForBuildingPlacement);
                Game1.playSound("smallSelect");
                onFarm = true;
            }

            if (!onFarm || freeze || Game1.IsFading())
            {
                return;
            }

            if (demolishing)
            {
                NMFarm1 farm = Game1.getLocationFromName("NMFarm1") as NMFarm1;
                Building destroyed = farm.getBuildingAt(new Vector2((Game1.viewport.X + Game1.getOldMouseX(ui_scale: false)) / 64, (Game1.viewport.Y + Game1.getOldMouseY(ui_scale: false)) / 64));
                Action buildingLockFailed = delegate
                {
                    if (demolishing)
                    {
                        Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_LockFailed"), Color.Red, 3500f));
                    }
                };
                Action continueDemolish = delegate
                {
                    if (demolishing && destroyed != null && farm.buildings.Contains(destroyed))
                    {
                        if ((int)destroyed.daysOfConstructionLeft > 0 || (int)destroyed.daysUntilUpgrade > 0)
                        {
                            Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_DuringConstruction"), Color.Red, 3500f));
                        }
                        else if (destroyed.indoors.Value != null && destroyed.indoors.Value is AnimalHouse && (destroyed.indoors.Value as AnimalHouse).animalsThatLiveHere.Count > 0)
                        {
                            Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_AnimalsHere"), Color.Red, 3500f));
                        }
                        else if (destroyed.indoors.Value != null && destroyed.indoors.Value.farmers.Any())
                        {
                            Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_PlayerHere"), Color.Red, 3500f));
                        }
                        else
                        {
                            if (destroyed.indoors.Value != null && destroyed.indoors.Value is Cabin)
                            {
                                foreach (Farmer allFarmer in Game1.getAllFarmers())
                                {
                                    if (allFarmer.currentLocation != null && allFarmer.currentLocation.Name == (destroyed.indoors.Value as Cabin).GetCellarName())
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
                                destroyed.BeforeDemolish();
                                Chest chest = null;
                                if (destroyed.indoors.Value is Cabin)
                                {
                                    List<Item> list = (destroyed.indoors.Value as Cabin).demolish();
                                    if (list.Count > 0)
                                    {
                                        chest = new Chest(playerChest: true);
                                        chest.fixLidFrame();
                                        chest.items.Set(list);
                                    }
                                }

                                if (farm.destroyStructure(destroyed))
                                {
                                    _ = (int)destroyed.tileY;
                                    _ = (int)destroyed.tilesHigh;
                                    Game1.flashAlpha = 1f;
                                    destroyed.showDestroyedAnimation(getFarm1());
                                    Game1.playSound("explosion");
                                    Utility.spreadAnimalsAround(destroyed, farm);
                                    DelayedAction.functionAfterDelay(returnToCarpentryMenu, 1500);
                                    freeze = true;
                                    if (chest != null)
                                    {
                                        farm.objects[new Vector2((int)destroyed.tileX + (int)destroyed.tilesWide / 2, (int)destroyed.tileY + (int)destroyed.tilesHigh / 2)] = chest;
                                    }
                                }
                            }
                        }
                    }
                };
                if (destroyed != null)
                {
                    if (destroyed.indoors.Value != null && destroyed.indoors.Value is Cabin && !Game1.IsMasterGame)
                    {
                        Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_LockFailed"), Color.Red, 3500f));
                        destroyed = null;
                        return;
                    }

                    if (!CanDemolishThis(destroyed))
                    {
                        destroyed = null;
                        return;
                    }

                    if (!Game1.IsMasterGame && !hasPermissionsToDemolish(destroyed))
                    {
                        destroyed = null;
                        return;
                    }
                }

                if (destroyed != null && destroyed.indoors.Value is Cabin)
                {
                    Cabin cabin = destroyed.indoors.Value as Cabin;
                    if (cabin.farmhand.Value != null && (bool)cabin.farmhand.Value.isCustomized)
                    {
                        Game1.currentLocation.createQuestionDialogue(Game1.content.LoadString("Strings\\UI:Carpenter_DemolishCabinConfirm", cabin.farmhand.Value.Name), Game1.currentLocation.createYesNoResponses(), delegate (Farmer f, string answer)
                        {
                            if (answer == "Yes")
                            {
                                Game1.activeClickableMenu = this;
                                Game1.player.team.demolishLock.RequestLock(continueDemolish, buildingLockFailed);
                            }
                            else
                            {
                                DelayedAction.functionAfterDelay(returnToCarpentryMenu, 500);
                            }
                        });
                        return;
                    }
                }

                if (destroyed != null)
                {
                    Game1.player.team.demolishLock.RequestLock(continueDemolish, buildingLockFailed);
                }

                return;
            }

            if (upgrading)
            {
                Building buildingAt = ((NMFarm1)Game1.getLocationFromName("NMFarm1")).getBuildingAt(new Vector2((Game1.viewport.X + Game1.getOldMouseX(ui_scale: false)) / 64, (Game1.viewport.Y + Game1.getOldMouseY(ui_scale: false)) / 64));
                if (buildingAt != null && CurrentBlueprint.name != null && buildingAt.buildingType.Equals(CurrentBlueprint.nameOfBuildingToUpgrade))
                {
                    CurrentBlueprint.consumeResources();
                    buildingAt.daysUntilUpgrade.Value = 2;
                    buildingAt.showUpgradeAnimation(getFarm1());
                    Game1.playSound("axe");
                    DelayedAction.functionAfterDelay(returnToCarpentryMenuAfterSuccessfulBuild, 1500);
                    freeze = true;
                }
                else if (buildingAt != null)
                {
                    Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantUpgrade_BuildingType"), Color.Red, 3500f));
                }

                return;
            }

            if (painting)
            {
                NMFarm1 farm_location = getFarm1();
                Vector2 vector = new Vector2((Game1.viewport.X + Game1.getMouseX(ui_scale: false)) / 64, (Game1.viewport.Y + Game1.getMouseY(ui_scale: false)) / 64);
                Building buildingAt2 = farm_location.getBuildingAt(vector);
                if (buildingAt2 != null)
                {
                    if (!buildingAt2.CanBePainted())
                    {
                        Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CannotPaint"), Color.Red, 3500f));
                        return;
                    }

                    if (!HasPermissionsToPaint(buildingAt2))
                    {
                        Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CannotPaint_Permission"), Color.Red, 3500f));
                        return;
                    }

                    buildingAt2.color.Value = Color.White;
                    SetChildMenu(new BuildingPaintMenu(buildingAt2));
                }
                else
                {
                    if (!farm_location.GetHouseRect().Contains(Utility.Vector2ToPoint(vector)))
                    {
                        return;
                    }

                    if (!CanPaintHouse())
                    {
                        Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CannotPaint"), Color.Red, 3500f));
                        return;
                    }

                    if (!HasPermissionsToPaint(null))
                    {
                        Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CannotPaint_Permission"), Color.Red, 3500f));
                        return;
                    }

                    SetChildMenu(new BuildingPaintMenu("House", () => (farm_location.paintedHouseTexture != null) ? farm_location.paintedHouseTexture : Farm.houseTextures, farm_location.houseSource.Value, farm_location.housePaintColor.Value));
                }

                return;
            }

            if (moving)
            {
                if (buildingToMove == null)
                {
                    buildingToMove = ((NMFarm1)Game1.getLocationFromName("NMFarm1")).getBuildingAt(new Vector2((Game1.viewport.X + Game1.getMouseX(ui_scale: false)) / 64, (Game1.viewport.Y + Game1.getMouseY(ui_scale: false)) / 64));
                    if (buildingToMove != null)
                    {
                        if ((int)buildingToMove.daysOfConstructionLeft > 0)
                        {
                            buildingToMove = null;
                            return;
                        }

                        if (!hasPermissionsToMove(buildingToMove))
                        {
                            buildingToMove = null;
                            return;
                        }

                        buildingToMove.isMoving = true;
                        Game1.playSound("axchop");
                    }
                }
                else if (((NMFarm1)Game1.getLocationFromName("NMFarm1")).buildStructure(buildingToMove, new Vector2((Game1.viewport.X + Game1.getMouseX(ui_scale: false)) / 64, (Game1.viewport.Y + Game1.getMouseY(ui_scale: false)) / 64), Game1.player))
                {
                    buildingToMove.isMoving = false;
                    if (buildingToMove is ShippingBin)
                    {
                        (buildingToMove as ShippingBin).initLid();
                    }

                    if (buildingToMove is GreenhouseBuilding)
                    {
                        getFarm1().greenhouseMoved.Value = true;
                    }

                    buildingToMove.performActionOnBuildingPlacement();
                    buildingToMove = null;
                    Game1.playSound("axchop");
                    DelayedAction.playSoundAfterDelay("dirtyHit", 50);
                    DelayedAction.playSoundAfterDelay("dirtyHit", 150);
                }
                else
                {
                    Game1.playSound("cancel");
                }

                return;
            }

            Game1.player.team.buildLock.RequestLock(delegate
            {
                if (onFarm && Game1.locationRequest == null)
                {
                    if (tryToBuild())
                    {
                        CurrentBlueprint.consumeResources();
                        DelayedAction.functionAfterDelay(returnToCarpentryMenuAfterSuccessfulBuild, 2000);
                        freeze = true;
                    }
                    else
                    {
                        Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantBuild"), Color.Red, 3500f));
                    }
                }

                Game1.player.team.buildLock.ReleaseLock();
            });
        }

        public bool tryToBuild()
        {
            return ((NMFarm1)Game1.getLocationFromName("NMFarm1")).buildStructure(CurrentBlueprint, new Vector2((Game1.viewport.X + Game1.getOldMouseX(ui_scale: false)) / 64, (Game1.viewport.Y + Game1.getOldMouseY(ui_scale: false)) / 64), Game1.player, magicalConstruction);
        }

        public void returnToCarpentryMenu()
        {
            LocationRequest locationRequest = Game1.getLocationRequest(magicalConstruction ? "WizardHouse" : "ScienceHouse");
            locationRequest.OnWarp += delegate
            {
                onFarm = false;
                Game1.player.viewingLocation.Value = null;
                resetBounds();
                upgrading = false;
                moving = false;
                painting = false;
                buildingToMove = null;
                freeze = false;
                Game1.displayHUD = true;
                Game1.viewportFreeze = false;
                Game1.viewport.Location = new Location(320, 1536);
                drawBG = true;
                demolishing = false;
                Game1.displayFarmer = true;
                if (Game1.options.SnappyMenus)
                {
                    populateClickableComponentList();
                    snapToDefaultClickableComponent();
                }
            };
            Game1.warpFarmer(locationRequest, Game1.player.getTileX(), Game1.player.getTileY(), Game1.player.facingDirection);
        }

        public void returnToCarpentryMenuAfterSuccessfulBuild()
        {
            LocationRequest locationRequest = Game1.getLocationRequest(magicalConstruction ? "WizardHouse" : "ScienceHouse");
            locationRequest.OnWarp += delegate
            {
                Game1.displayHUD = true;
                Game1.player.viewingLocation.Value = null;
                Game1.viewportFreeze = false;
                Game1.viewport.Location = new Location(320, 1536);
                freeze = true;
                Game1.displayFarmer = true;
                robinConstructionMessage();
            };
            Game1.warpFarmer(locationRequest, Game1.player.getTileX(), Game1.player.getTileY(), Game1.player.facingDirection);
        }

        public void robinConstructionMessage()
        {
            exitThisMenu();
            Game1.player.forceCanMove();
            if (!magicalConstruction)
            {
                string text = "Data\\ExtraDialogue:Robin_" + (upgrading ? "Upgrade" : "New") + "Construction";
                if (Utility.isFestivalDay(Game1.dayOfMonth + 1, Game1.currentSeason))
                {
                    text += "_Festival";
                }

                if (CurrentBlueprint.daysToConstruct <= 0)
                {
                    Game1.drawDialogue(Game1.getCharacterFromName("Robin"), Game1.content.LoadString("Data\\ExtraDialogue:Robin_Instant", (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.de) ? CurrentBlueprint.displayName : CurrentBlueprint.displayName.ToLower()));
                }
                else
                {
                    Game1.drawDialogue(Game1.getCharacterFromName("Robin"), Game1.content.LoadString(text, (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.de) ? CurrentBlueprint.displayName : CurrentBlueprint.displayName.ToLower(), (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.de) ? CurrentBlueprint.displayName.Split(' ').Last().Split('-')
                        .Last() : ((LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.pt || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.es || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.it) ? CurrentBlueprint.displayName.ToLower().Split(' ').First() : CurrentBlueprint.displayName.ToLower().Split(' ').Last())));
                }
            }
        }

        public override bool overrideSnappyMenuCursorMovementBan()
        {
            return onFarm;
        }

        public void setUpForBuildingPlacement()
        {
            Game1.currentLocation.cleanupBeforePlayerExit();
            hoverText = "";
            Game1.currentLocation = Game1.getLocationFromName("NMFarm1");
            Game1.player.viewingLocation.Value = "NMFarm1";
            Game1.currentLocation.resetForPlayerEntry();
            Game1.globalFadeToClear();
            onFarm = true;
            cancelButton.bounds.X = Game1.uiViewport.Width - 128;
            cancelButton.bounds.Y = Game1.uiViewport.Height - 128;
            Game1.displayHUD = false;
            Game1.viewportFreeze = true;
            Game1.viewport.Location = new Location(3136, 320);
            Game1.panScreen(0, 0);
            drawBG = false;
            freeze = false;
            Game1.displayFarmer = false;
            if (!demolishing && CurrentBlueprint.nameOfBuildingToUpgrade != null && CurrentBlueprint.nameOfBuildingToUpgrade.Length > 0 && !moving && !painting)
            {
                upgrading = true;
            }
        }

        public override void gameWindowSizeChanged(Microsoft.Xna.Framework.Rectangle oldBounds, Microsoft.Xna.Framework.Rectangle newBounds)
        {
            resetBounds();
        }

        public virtual bool CanDemolishThis(Building building)
        {
            if (building == null)
            {
                return false;
            }

            if (_demolishCheckBlueprint == null || _demolishCheckBlueprint.name != building.buildingType.Value)
            {
                _demolishCheckBlueprint = new BluePrint(building.buildingType);
            }

            if (_demolishCheckBlueprint != null)
            {
                return CanDemolishThis(_demolishCheckBlueprint);
            }

            return true;
        }

        public virtual bool CanDemolishThis(BluePrint blueprint)
        {
            if (blueprint.moneyRequired < 0)
            {
                return false;
            }

            if (blueprint.name == "Shipping Bin")
            {
                int num = 0;
                foreach (Building building in getFarm1().buildings)
                {
                    if (building is ShippingBin)
                    {
                        num++;
                    }

                    if (num > 1)
                    {
                        break;
                    }
                }

                if (num <= 1)
                {
                    return false;
                }
            }

            return true;
        }

        public override void draw(SpriteBatch b)
        {
            if (drawBG)
            {
                b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.5f);
            }

            if (Game1.IsFading() || freeze)
            {
                return;
            }

            if (!onFarm)
            {
                base.draw(b);
                IClickableMenu.drawTextureBox(b, xPositionOnScreen - 96, yPositionOnScreen - 16, maxWidthOfBuildingViewer + 64, maxHeightOfBuildingViewer + 64, magicalConstruction ? Color.RoyalBlue : Color.White);
                currentBuilding.drawInMenu(b, xPositionOnScreen + maxWidthOfBuildingViewer / 2 - (int)currentBuilding.tilesWide * 64 / 2 - 64, yPositionOnScreen + maxHeightOfBuildingViewer / 2 - currentBuilding.getSourceRectForMenu().Height * 4 / 2);
                if (CurrentBlueprint.isUpgrade())
                {
                    upgradeIcon.draw(b);
                }

                string s = " Deluxe  Barn   ";
                if (SpriteText.getWidthOfString(buildingName) >= SpriteText.getWidthOfString(s))
                {
                    s = buildingName + " ";
                }

                SpriteText.drawStringWithScrollCenteredAt(b, buildingName, xPositionOnScreen + maxWidthOfBuildingViewer - IClickableMenu.spaceToClearSideBorder - 16 + 64 + (width - (maxWidthOfBuildingViewer + 128)) / 2, yPositionOnScreen, SpriteText.getWidthOfString(s));
                int num = LocalizedContentManager.CurrentLanguageCode switch
                {
                    LocalizedContentManager.LanguageCode.es => maxWidthOfDescription + 64 + ((CurrentBlueprint?.name == "Deluxe Barn") ? 96 : 0),
                    LocalizedContentManager.LanguageCode.it => maxWidthOfDescription + 96,
                    LocalizedContentManager.LanguageCode.fr => maxWidthOfDescription + 96 + ((CurrentBlueprint?.name == "Slime Hutch" || CurrentBlueprint?.name == "Deluxe Coop" || CurrentBlueprint?.name == "Deluxe Barn") ? 72 : 0),
                    LocalizedContentManager.LanguageCode.ko => maxWidthOfDescription + 96 + ((CurrentBlueprint?.name == "Slime Hutch") ? 64 : ((CurrentBlueprint?.name == "Deluxe Coop") ? 96 : ((CurrentBlueprint?.name == "Deluxe Barn") ? 112 : ((CurrentBlueprint?.name == "Big Barn") ? 64 : 0)))),
                    _ => maxWidthOfDescription + 64,
                };
                IClickableMenu.drawTextureBox(b, xPositionOnScreen + maxWidthOfBuildingViewer - 16, yPositionOnScreen + 80, num, maxHeightOfBuildingViewer - 32, magicalConstruction ? Color.RoyalBlue : Color.White);
                if (magicalConstruction)
                {
                    Utility.drawTextWithShadow(b, Game1.parseText(buildingDescription, Game1.dialogueFont, num - 32), Game1.dialogueFont, new Vector2(xPositionOnScreen + maxWidthOfBuildingViewer - 4, yPositionOnScreen + 80 + 16 + 4), Game1.textColor * 0.25f, 1f, -1f, -1, -1, 0f);
                    Utility.drawTextWithShadow(b, Game1.parseText(buildingDescription, Game1.dialogueFont, num - 32), Game1.dialogueFont, new Vector2(xPositionOnScreen + maxWidthOfBuildingViewer - 1, yPositionOnScreen + 80 + 16 + 4), Game1.textColor * 0.25f, 1f, -1f, -1, -1, 0f);
                }

                Utility.drawTextWithShadow(b, Game1.parseText(buildingDescription, Game1.dialogueFont, num - 32), Game1.dialogueFont, new Vector2(xPositionOnScreen + maxWidthOfBuildingViewer, yPositionOnScreen + 80 + 16), magicalConstruction ? Color.PaleGoldenrod : Game1.textColor, 1f, -1f, -1, -1, magicalConstruction ? 0f : 0.75f);
                Vector2 location = new Vector2(xPositionOnScreen + maxWidthOfBuildingViewer + 16, yPositionOnScreen + 256 + 32);
                if (ingredients.Count < 3 && (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.fr || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ko || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.pt))
                {
                    location.Y += 64f;
                }

                if (price >= 0)
                {
                    SpriteText.drawString(b, "$", (int)location.X, (int)location.Y);
                    string numberWithCommas = Utility.getNumberWithCommas(price);
                    if (magicalConstruction)
                    {
                        Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\StringsFromCSFiles:LoadGameMenu.cs.11020", numberWithCommas), Game1.dialogueFont, new Vector2(location.X + 64f, location.Y + 8f), Game1.textColor * 0.5f, 1f, -1f, -1, -1, magicalConstruction ? 0f : 0.25f);
                        Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\StringsFromCSFiles:LoadGameMenu.cs.11020", numberWithCommas), Game1.dialogueFont, new Vector2(location.X + 64f + 4f - 1f, location.Y + 8f), Game1.textColor * 0.25f, 1f, -1f, -1, -1, magicalConstruction ? 0f : 0.25f);
                    }

                    Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\StringsFromCSFiles:LoadGameMenu.cs.11020", numberWithCommas), Game1.dialogueFont, new Vector2(location.X + 64f + 4f, location.Y + 4f), (Game1.player.Money < price) ? Color.Red : (magicalConstruction ? Color.PaleGoldenrod : Game1.textColor), 1f, -1f, -1, -1, magicalConstruction ? 0f : 0.25f);
                }

                location.X -= 16f;
                location.Y -= 21f;
                foreach (Item ingredient in ingredients)
                {
                    location.Y += 68f;
                    ingredient.drawInMenu(b, location, 1f);
                    bool flag = ((!(ingredient is Object) || Game1.player.hasItemInInventory((ingredient as Object).parentSheetIndex, ingredient.Stack)) ? true : false);
                    if (magicalConstruction)
                    {
                        Utility.drawTextWithShadow(b, ingredient.DisplayName, Game1.dialogueFont, new Vector2(location.X + 64f + 12f, location.Y + 24f), Game1.textColor * 0.25f, 1f, -1f, -1, -1, magicalConstruction ? 0f : 0.25f);
                        Utility.drawTextWithShadow(b, ingredient.DisplayName, Game1.dialogueFont, new Vector2(location.X + 64f + 16f - 1f, location.Y + 24f), Game1.textColor * 0.25f, 1f, -1f, -1, -1, magicalConstruction ? 0f : 0.25f);
                    }

                    Utility.drawTextWithShadow(b, ingredient.DisplayName, Game1.dialogueFont, new Vector2(location.X + 64f + 16f, location.Y + 20f), flag ? (magicalConstruction ? Color.PaleGoldenrod : Game1.textColor) : Color.Red, 1f, -1f, -1, -1, magicalConstruction ? 0f : 0.25f);
                }

                backButton.draw(b);
                forwardButton.draw(b);
                okButton.draw(b, blueprints[currentBlueprintIndex].doesFarmerHaveEnoughResourcesToBuild() ? Color.White : (Color.Gray * 0.8f), 0.88f);
                demolishButton.draw(b, CanDemolishThis(blueprints[currentBlueprintIndex]) ? Color.White : (Color.Gray * 0.8f), 0.88f);
                moveButton.draw(b);
                paintButton.draw(b);
            }
            else
            {
                string text = "";
                text = (upgrading ? Game1.content.LoadString("Strings\\UI:Carpenter_SelectBuilding_Upgrade", new BluePrint(CurrentBlueprint.nameOfBuildingToUpgrade).displayName) : (demolishing ? Game1.content.LoadString("Strings\\UI:Carpenter_SelectBuilding_Demolish") : ((!painting) ? Game1.content.LoadString("Strings\\UI:Carpenter_ChooseLocation") : Game1.content.LoadString("Strings\\UI:Carpenter_SelectBuilding_Paint"))));
                SpriteText.drawStringWithScrollBackground(b, text, Game1.uiViewport.Width / 2 - SpriteText.getWidthOfString(text) / 2, 16);
                Game1.StartWorldDrawInUI(b);
                if (!upgrading && !demolishing && !moving && !painting)
                {
                    Vector2 vector = new Vector2((Game1.viewport.X + Game1.getOldMouseX(ui_scale: false)) / 64, (Game1.viewport.Y + Game1.getOldMouseY(ui_scale: false)) / 64);
                    for (int i = 0; i < CurrentBlueprint.tilesHeight; i++)
                    {
                        for (int j = 0; j < CurrentBlueprint.tilesWidth; j++)
                        {
                            int num2 = CurrentBlueprint.getTileSheetIndexForStructurePlacementTile(j, i);
                            Vector2 vector2 = new Vector2(vector.X + (float)j, vector.Y + (float)i);
                            if (!(Game1.currentLocation as BuildableGameLocation).isBuildable(vector2))
                            {
                                num2++;
                            }

                            b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, vector2 * 64f), new Microsoft.Xna.Framework.Rectangle(194 + num2 * 16, 388, 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.999f);
                        }
                    }

                    foreach (Point additionalPlacementTile in CurrentBlueprint.additionalPlacementTiles)
                    {
                        int x = additionalPlacementTile.X;
                        int y = additionalPlacementTile.Y;
                        int num3 = CurrentBlueprint.getTileSheetIndexForStructurePlacementTile(x, y);
                        Vector2 vector3 = new Vector2(vector.X + (float)x, vector.Y + (float)y);
                        if (!(Game1.currentLocation as BuildableGameLocation).isBuildable(vector3))
                        {
                            num3++;
                        }

                        b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, vector3 * 64f), new Microsoft.Xna.Framework.Rectangle(194 + num3 * 16, 388, 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.999f);
                    }
                }
                else if (!painting && moving && buildingToMove != null)
                {
                    Vector2 vector4 = new Vector2((Game1.viewport.X + Game1.getOldMouseX(ui_scale: false)) / 64, (Game1.viewport.Y + Game1.getOldMouseY(ui_scale: false)) / 64);
                    BuildableGameLocation buildableGameLocation = Game1.currentLocation as BuildableGameLocation;
                    for (int k = 0; k < (int)buildingToMove.tilesHigh; k++)
                    {
                        for (int l = 0; l < (int)buildingToMove.tilesWide; l++)
                        {
                            int num4 = buildingToMove.getTileSheetIndexForStructurePlacementTile(l, k);
                            Vector2 vector5 = new Vector2(vector4.X + (float)l, vector4.Y + (float)k);
                            bool flag2 = buildableGameLocation.buildings.Contains(buildingToMove) && buildingToMove.occupiesTile(vector5);
                            if (!buildableGameLocation.isBuildable(vector5) && !flag2)
                            {
                                num4++;
                            }

                            b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, vector5 * 64f), new Microsoft.Xna.Framework.Rectangle(194 + num4 * 16, 388, 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.999f);
                        }
                    }

                    foreach (Point additionalPlacementTile2 in buildingToMove.additionalPlacementTiles)
                    {
                        int x2 = additionalPlacementTile2.X;
                        int y2 = additionalPlacementTile2.Y;
                        int num5 = buildingToMove.getTileSheetIndexForStructurePlacementTile(x2, y2);
                        Vector2 vector6 = new Vector2(vector4.X + (float)x2, vector4.Y + (float)y2);
                        bool flag3 = buildableGameLocation.buildings.Contains(buildingToMove) && buildingToMove.occupiesTile(vector6);
                        if (!buildableGameLocation.isBuildable(vector6) && !flag3)
                        {
                            num5++;
                        }

                        b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, vector6 * 64f), new Microsoft.Xna.Framework.Rectangle(194 + num5 * 16, 388, 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.999f);
                    }
                }

                Game1.EndWorldDrawInUI(b);
            }

            cancelButton.draw(b);
            drawMouse(b);
            if (hoverText.Length > 0)
            {
                IClickableMenu.drawHoverText(b, hoverText, Game1.dialogueFont);
            }
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
        }
    }
}