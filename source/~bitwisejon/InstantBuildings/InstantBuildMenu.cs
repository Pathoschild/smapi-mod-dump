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


namespace BitwiseJonMods
{
    public class InstantBuildMenu : IClickableMenu
    {
        private const int MENU_DELAY = 200;
        private const int MULTIPLAYER_LIMIT = 4;  //Because Game1.multiplayer (and hence .playerLimit) is inaccessible due to protection level

        public int maxWidthOfBuildingViewer = 448;
        public int maxHeightOfBuildingViewer = 512;
        public int maxWidthOfDescription = 384;
        private List<Item> ingredients = new List<Item>();
        private bool drawBG = true;
        private string hoverText = "";
        public const int region_backButton = 101;
        public const int region_forwardButton = 102;
        public const int region_upgradeIcon = 103;
        public const int region_demolishButton = 104;
        public const int region_moveBuitton = 105;
        public const int region_okButton = 106;
        public const int region_cancelButton = 107;
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
        private int price;
        private bool onFarm;
        private bool freeze;
        private bool upgrading;
        private bool demolishing;
        private bool moving;
        private bool magicalConstruction;

        private ModConfig _config;

        public BluePrint CurrentBlueprint
        {
            get
            {
                return this.blueprints[this.currentBlueprintIndex];
            }
        }

        public InstantBuildMenu(ModConfig config, BluePrint tractorBlueprint)
        {
            _config = config;

            this.magicalConstruction = true;
            Game1.player.forceCanMove();
            this.resetBounds();
            this.blueprints = new List<BluePrint>();

            this.blueprints.Add(new BluePrint("Coop"));
            this.blueprints.Add(new BluePrint("Barn"));
            this.blueprints.Add(new BluePrint("Well"));
            this.blueprints.Add(new BluePrint("Silo"));
            this.blueprints.Add(new BluePrint("Mill"));
            this.blueprints.Add(new BluePrint("Shed"));
            int buildingsConstructed = Game1.getFarm().getNumberBuildingsConstructed("Cabin");
            if (Game1.IsMasterGame && buildingsConstructed < MULTIPLAYER_LIMIT - 1)
            {
                this.blueprints.Add(new BluePrint("Stone Cabin"));
                this.blueprints.Add(new BluePrint("Plank Cabin"));
                this.blueprints.Add(new BluePrint("Log Cabin"));
            }
            if (Game1.getFarm().getNumberBuildingsConstructed("Stable") < buildingsConstructed + 1)
                this.blueprints.Add(new BluePrint("Stable"));
            this.blueprints.Add(new BluePrint("Slime Hutch"));
            //if (Game1.getFarm().isBuildingConstructed("Coop"))
                this.blueprints.Add(new BluePrint("Big Coop"));
            //if (Game1.getFarm().isBuildingConstructed("Big Coop"))
                this.blueprints.Add(new BluePrint("Deluxe Coop"));
            //if (Game1.getFarm().isBuildingConstructed("Barn"))
                this.blueprints.Add(new BluePrint("Big Barn"));
            //if (Game1.getFarm().isBuildingConstructed("Big Barn"))
                this.blueprints.Add(new BluePrint("Deluxe Barn"));
            this.blueprints.Add(new BluePrint("Shipping Bin"));

            if (tractorBlueprint != null)
            {
                this.blueprints.Add(tractorBlueprint);
            }

            if (_config.AllowMagicalBuildingsWithoutMagicInk || (!_config.AllowMagicalBuildingsWithoutMagicInk && Game1.player.hasMagicInk))
            {
                this.blueprints.Add(new BluePrint("Junimo Hut"));
                this.blueprints.Add(new BluePrint("Earth Obelisk"));
                this.blueprints.Add(new BluePrint("Water Obelisk"));
                this.blueprints.Add(new BluePrint("Gold Clock"));
            }

            ModifyBlueprints();


            this.setNewActiveBlueprint();
            if (!Game1.options.SnappyMenus)
                return;
            this.populateClickableComponentList();
            this.snapToDefaultClickableComponent();
        }

        private void ModifyBlueprints()
        {
            //Set the cost of all buildings to zero with no items required
            foreach (var blueprint in this.blueprints)
            {
                blueprint.magical = true;
                blueprint.daysToConstruct = 0;  //jon, 8/4/18: Attempt to fix problem with tractor garage not honoring the magical nature of the blueprint so it builds instantly.

                if (!_config.BuildUsesResources)
                {
                    blueprint.itemsRequired = new Dictionary<int, int>();
                    blueprint.moneyRequired = 0;
                }
            }
        }

        public override void snapToDefaultClickableComponent()
        {
            this.currentlySnappedComponent = this.getComponentWithID(107);
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
            this.demolishButton.visible = Game1.IsMasterGame;
            this.moveButton.visible = Game1.IsMasterGame;
            if (!this.demolishButton.visible)
            {
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

        public void setNewActiveBlueprint()
        {
            this.currentBuilding = !this.blueprints[this.currentBlueprintIndex].name.Contains("Coop") ? (!this.blueprints[this.currentBlueprintIndex].name.Contains("Barn") ? (!this.blueprints[this.currentBlueprintIndex].name.Contains("Mill") ? (!this.blueprints[this.currentBlueprintIndex].name.Contains("Junimo Hut") ? (!this.blueprints[this.currentBlueprintIndex].name.Contains("Shipping Bin") ? new Building(this.blueprints[this.currentBlueprintIndex], Vector2.Zero) : (Building)new ShippingBin(this.blueprints[this.currentBlueprintIndex], Vector2.Zero)) : (Building)new JunimoHut(this.blueprints[this.currentBlueprintIndex], Vector2.Zero)) : (Building)new Mill(this.blueprints[this.currentBlueprintIndex], Vector2.Zero)) : (Building)new Barn(this.blueprints[this.currentBlueprintIndex], Vector2.Zero)) : (Building)new Coop(this.blueprints[this.currentBlueprintIndex], Vector2.Zero);
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
                foreach (Building building in ((BuildableGameLocation)Game1.getLocationFromName("Farm")).buildings)
                    building.color.Value = Color.White;
                Building building1 = ((BuildableGameLocation)Game1.getLocationFromName("Farm")).getBuildingAt(new Vector2((float)((Game1.viewport.X + Game1.getOldMouseX()) / 64), (float)((Game1.viewport.Y + Game1.getOldMouseY()) / 64))) ?? ((BuildableGameLocation)Game1.getLocationFromName("Farm")).getBuildingAt(new Vector2((float)((Game1.viewport.X + Game1.getOldMouseX()) / 64), (float)((Game1.viewport.Y + Game1.getOldMouseY() + 128) / 64))) ?? ((BuildableGameLocation)Game1.getLocationFromName("Farm")).getBuildingAt(new Vector2((float)((Game1.viewport.X + Game1.getOldMouseX()) / 64), (float)((Game1.viewport.Y + Game1.getOldMouseY() + 192) / 64)));
                if (this.upgrading)
                {
                    if (building1 != null && this.CurrentBlueprint.nameOfBuildingToUpgrade != null && this.CurrentBlueprint.nameOfBuildingToUpgrade.Equals((string)((NetFieldBase<string, NetString>)building1.buildingType)))
                    {
                        building1.color.Value = Color.Lime * 0.8f;
                    }
                    else
                    {
                        if (building1 == null)
                            return;
                        building1.color.Value = Color.Red * 0.8f;
                    }
                }
                else if (this.demolishing)
                {
                    if (building1 == null)
                        return;
                    building1.color.Value = Color.Red * 0.8f;
                }
                else
                {
                    if (!this.moving || building1 == null)
                        return;
                    building1.color.Value = Color.Lime * 0.8f;
                }
            }
        }

        public override void receiveGamePadButton(Buttons b)
        {
            base.receiveGamePadButton(b);
            if (!this.onFarm && b == Buttons.LeftTrigger)
            {
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

        public override void receiveKeyPress(Keys key)
        {
            if (this.freeze)
                return;
            if (!this.onFarm)
                base.receiveKeyPress(key);
            if (Game1.delayedActions.Count() > 0 || !this.onFarm)
                return;
            if (Game1.options.doesInputListContain(Game1.options.menuButton, key) && this.readyToClose())
            {
                Game1.delayedActions.Add(new DelayedAction(MENU_DELAY, new DelayedAction.delayedBehavior(this.returnToFarm)));
            }
            else
            {
                if (Game1.options.SnappyMenus)
                    return;
                if (Game1.options.doesInputListContain(Game1.options.moveDownButton, key))
                    Game1.panScreen(0, 12);
                else if (Game1.options.doesInputListContain(Game1.options.moveRightButton, key))
                    Game1.panScreen(12, 0);
                else if (Game1.options.doesInputListContain(Game1.options.moveUpButton, key))
                {
                    Game1.panScreen(0, -12);
                }
                else
                {
                    if (!Game1.options.doesInputListContain(Game1.options.moveLeftButton, key))
                        return;
                    Game1.panScreen(-12, 0);
                }
            }
        }

        public override void update(GameTime time)
        {
            base.update(time);
            if (!this.onFarm || Game1.delayedActions.Count() > 0)
                return;
            int num1 = Game1.getOldMouseX() + Game1.viewport.X;
            int num2 = Game1.getOldMouseY() + Game1.viewport.Y;
            if (num1 - Game1.viewport.X < 64)
                Game1.panScreen(-16, 0);
            else if (num1 - (Game1.viewport.X + Game1.viewport.Width) >= (int)sbyte.MinValue)
                Game1.panScreen(16, 0);
            if (num2 - Game1.viewport.Y < 64)
                Game1.panScreen(0, -16);
            else if (num2 - (Game1.viewport.Y + Game1.viewport.Height) >= -64)
                Game1.panScreen(0, 16);
            foreach (Keys pressedKey in Game1.oldKBState.GetPressedKeys())
                this.receiveKeyPress(pressedKey);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (this.freeze)
                return;
            if (!this.onFarm)
            {
                Game1.displayFarmer = true;
                base.receiveLeftClick(x, y, playSound);
            }
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
                    Game1.delayedActions.Add(new DelayedAction(MENU_DELAY, new DelayedAction.delayedBehavior(this.returnToFarm)));
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
                Game1.delayedActions.Add(new DelayedAction(MENU_DELAY, new DelayedAction.delayedBehavior(this.setUpForBuildingPlacement)));
                Game1.playSound("smallSelect");
                this.onFarm = true;
                this.demolishing = true;
            }
            if (!this.onFarm && this.moveButton.containsPoint(x, y) && this.moveButton.visible)
            {
                Game1.delayedActions.Add(new DelayedAction(MENU_DELAY, new DelayedAction.delayedBehavior(this.setUpForBuildingPlacement)));
                Game1.playSound("smallSelect");
                this.onFarm = true;
                this.moving = true;
            }
            if (this.okButton.containsPoint(x, y) && !this.onFarm && (Game1.player.money >= this.price && this.blueprints[this.currentBlueprintIndex].doesFarmerHaveEnoughResourcesToBuild()))
            {
                Game1.delayedActions.Add(new DelayedAction(MENU_DELAY, new DelayedAction.delayedBehavior(this.setUpForBuildingPlacement)));
                Game1.playSound("smallSelect");
                this.onFarm = true;
            }
            if (!this.onFarm || this.freeze || Game1.delayedActions.Count() > 0)
                return;
            if (this.demolishing)
            {
                Farm farm = Game1.getLocationFromName("Farm") as Farm;
                Building destroyed = farm.getBuildingAt(new Vector2((float)((Game1.viewport.X + Game1.getOldMouseX()) / 64), (float)((Game1.viewport.Y + Game1.getOldMouseY()) / 64)));
                Action buildingLockFailed = (Action)(() => Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_LockFailed"), Color.Red, 3500f)));
                Action continueDemolish = (Action)(() =>
                {
                    if (destroyed == null || !farm.buildings.Contains(destroyed))
                        return;
                    if ((int)((NetFieldBase<int, NetInt>)destroyed.daysOfConstructionLeft) > 0 || (int)((NetFieldBase<int, NetInt>)destroyed.daysUntilUpgrade) > 0)
                        Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_DuringConstruction"), Color.Red, 3500f));
                    else if (destroyed.indoors.Value != null && destroyed.indoors.Value is AnimalHouse && (destroyed.indoors.Value as AnimalHouse).animalsThatLiveHere.Count > 0)
                        Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_AnimalsHere"), Color.Red, 3500f));
                    else if (destroyed.indoors.Value != null && destroyed.indoors.Value.farmers.Count<Farmer>() > 0)
                        Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_PlayerHere"), Color.Red, 3500f));
                    else if (destroyed.indoors.Value is Cabin && (destroyed.indoors.Value as Cabin).farmhand.Value != null && (destroyed.indoors.Value as Cabin).farmhand.Value.isActive())  //jon, 8/4/18: Fixed bug where cabins cannot be destroyed if not in multiplayer.
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
                                chest.items.Set((IList<Item>)objList);
                            }
                        }
                        if (!farm.destroyStructure(destroyed))
                            return;
                        int tileY = (int)((NetFieldBase<int, NetInt>)destroyed.tileY);
                        int tilesHigh = (int)((NetFieldBase<int, NetInt>)destroyed.tilesHigh);
                        Game1.flashAlpha = 1f;
                        destroyed.showDestroyedAnimation((GameLocation)Game1.getFarm());
                        Game1.playSound("explosion");
                        Utility.spreadAnimalsAround(destroyed, farm);
                        Game1.delayedActions.Add(new DelayedAction(MENU_DELAY, new DelayedAction.delayedBehavior(this.returnToFarm)));
                        this.freeze = true;
                        if (chest == null)
                            return;
                        farm.objects[new Vector2((float)((int)((NetFieldBase<int, NetInt>)destroyed.tileX) + (int)((NetFieldBase<int, NetInt>)destroyed.tilesWide) / 2), (float)((int)((NetFieldBase<int, NetInt>)destroyed.tileY) + (int)((NetFieldBase<int, NetInt>)destroyed.tilesHigh) / 2))] = (StardewValley.Object)chest;
                    }
                });
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
                                Game1.delayedActions.Add(new DelayedAction(MENU_DELAY, new DelayedAction.delayedBehavior(this.returnToFarm)));
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
                Building buildingAt = ((BuildableGameLocation)Game1.getLocationFromName("Farm")).getBuildingAt(new Vector2((float)((Game1.viewport.X + Game1.getOldMouseX()) / 64), (float)((Game1.viewport.Y + Game1.getOldMouseY()) / 64)));
                if (buildingAt != null && this.CurrentBlueprint.name != null && buildingAt.buildingType.Equals((object)this.CurrentBlueprint.nameOfBuildingToUpgrade))
                {
                    this.CurrentBlueprint.consumeResources();
                    doInstantUpgrade(buildingAt);

                    Game1.playSound("axe");
                    Game1.delayedActions.Add(new DelayedAction(MENU_DELAY, new DelayedAction.delayedBehavior(this.returnToFarm)));
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
                    this.buildingToMove = ((BuildableGameLocation)Game1.getLocationFromName("Farm")).getBuildingAt(new Vector2((float)((Game1.viewport.X + Game1.getMouseX()) / 64), (float)((Game1.viewport.Y + Game1.getMouseY()) / 64)));
                    if (this.buildingToMove == null)
                        return;
                    if ((int)((NetFieldBase<int, NetInt>)this.buildingToMove.daysOfConstructionLeft) > 0)
                    {
                        this.buildingToMove = (Building)null;
                    }
                    else
                    {
                        this.buildingToMove.isMoving = true;
                        Game1.playSound("axchop");
                    }
                }
                else if (((BuildableGameLocation)Game1.getLocationFromName("Farm")).buildStructure(this.buildingToMove, new Vector2((float)((Game1.viewport.X + Game1.getMouseX()) / 64), (float)((Game1.viewport.Y + Game1.getMouseY()) / 64)), Game1.player, false))
                {
                    this.buildingToMove.isMoving = false;
                    if (this.buildingToMove is ShippingBin)
                        (this.buildingToMove as ShippingBin).initLid();
                    this.buildingToMove = (Building)null;
                    Game1.playSound("axchop");
                    DelayedAction.playSoundAfterDelay("dirtyHit", 50, (GameLocation)null);
                    DelayedAction.playSoundAfterDelay("dirtyHit", 150, (GameLocation)null);
                }
                else
                    Game1.playSound("cancel");
            }
            else if (this.tryToBuild())
            {
                this.CurrentBlueprint.consumeResources();
                Game1.delayedActions.Add(new DelayedAction(MENU_DELAY, new DelayedAction.delayedBehavior(this.returnToFarm)));
                this.freeze = true;
            }
            else
                Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantBuild"), Color.Red, 3500f));
        }

        private void doInstantUpgrade(Building buildingAt)
        {
            Game1.player.checkForQuestComplete((NPC)null, -1, -1, (Item)null, buildingAt.getNameOfNextUpgrade(), 8, -1);
            BluePrint bluePrint = new BluePrint(buildingAt.getNameOfNextUpgrade());
            buildingAt.indoors.Value.mapPath.Value = "Maps\\" + bluePrint.mapToWarpTo;
            buildingAt.indoors.Value.name.Value = bluePrint.mapToWarpTo;
            buildingAt.buildingType.Value = bluePrint.name;

            if (buildingAt.indoors.Value is AnimalHouse)
            {
                ((AnimalHouse)(GameLocation)((NetFieldBase<GameLocation, NetRef<GameLocation>>)buildingAt.indoors)).resetPositionsOfAllAnimals();
                ((AnimalHouse)(GameLocation)((NetFieldBase<GameLocation, NetRef<GameLocation>>)buildingAt.indoors)).animalLimit.Value += 4;
                buildingAt.indoors.Value.loadLights();
            }
            buildingAt.upgrade();
            buildingAt.resetTexture();

            if (!buildingAt.buildingType.Value.Contains("Deluxe"))
                return;
            (buildingAt.indoors.Value as AnimalHouse).feedAllAnimals();
        }


        public bool tryToBuild()
        {
            return ((BuildableGameLocation)Game1.getLocationFromName("Farm")).buildStructure(this.CurrentBlueprint, new Vector2((float)((Game1.viewport.X + Game1.getOldMouseX()) / 64), (float)((Game1.viewport.Y + Game1.getOldMouseY()) / 64)), Game1.player, this.magicalConstruction, false);
        }

        public void returnToFarm()
        {
            Game1.currentLocation.cleanupBeforePlayerExit();
            Game1.currentLocation = Game1.getLocationFromName("Farm");
            Game1.currentLocation.resetForPlayerEntry();
            Game1.delayedActions.Add(new DelayedAction(MENU_DELAY, new DelayedAction.delayedBehavior(this.doNothing)));
            this.onFarm = false;
            this.resetBounds();
            this.upgrading = false;
            this.moving = false;
            this.freeze = true;
            Game1.displayHUD = true;
            Game1.viewportFreeze = false;
            Game1.viewport.Location = new Location(5 * Game1.tileSize, 24 * Game1.tileSize);
            this.drawBG = true;
            this.demolishing = false;
            //Game1.displayFarmer = false;
        }

        public void doNothing()
        {
            //Used to replace screen fades and clears to remove the blinking effect -- not needed since we are not flashing between Robin's shop and the farm.
            this.freeze = false;
            Game1.displayFarmer = true;  //jon, 8/4/18: Attempt to fix disappearing character problem which I am having trouble reproducing but other players are getting.
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
            Game1.delayedActions.Add(new DelayedAction(MENU_DELAY, new DelayedAction.delayedBehavior(this.doNothing)));
            this.onFarm = true;
            this.cancelButton.bounds.X = Game1.viewport.Width - 128;
            this.cancelButton.bounds.Y = Game1.viewport.Height - 128;
            Game1.displayHUD = false;
            Game1.viewportFreeze = true;
            Game1.viewport.Location = new Location((Game1.player.getTileX() * Game1.tileSize) - (Game1.viewport.Width / 2), (Game1.player.getTileY() * Game1.tileSize) - (Game1.viewport.Height / 2));
            Game1.panScreen(0, 0);
            this.drawBG = false;
            this.freeze = true;
            Game1.displayFarmer = false;
            if (this.demolishing || this.CurrentBlueprint.nameOfBuildingToUpgrade == null || (this.CurrentBlueprint.nameOfBuildingToUpgrade.Length <= 0 || this.moving))
                return;
            this.upgrading = true;
        }

        public override void gameWindowSizeChanged(Microsoft.Xna.Framework.Rectangle oldBounds, Microsoft.Xna.Framework.Rectangle newBounds)
        {
            this.resetBounds();
        }

        public override void draw(SpriteBatch b)
        {
            if (this.drawBG)
                b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.5f);
            if (Game1.delayedActions.Count() > 0 || this.freeze)
                return;
            if (!this.onFarm)
            {
                base.draw(b);
                IClickableMenu.drawTextureBox(b, this.xPositionOnScreen - 96, this.yPositionOnScreen - 16, this.maxWidthOfBuildingViewer + 64, this.maxHeightOfBuildingViewer + 64, this.magicalConstruction ? Color.RoyalBlue : Color.White);
                this.currentBuilding.drawInMenu(b, this.xPositionOnScreen + this.maxWidthOfBuildingViewer / 2 - (int)((NetFieldBase<int, NetInt>)this.currentBuilding.tilesWide) * 64 / 2 - 64, this.yPositionOnScreen + this.maxHeightOfBuildingViewer / 2 - this.currentBuilding.getSourceRectForMenu().Height * 4 / 2);
                if (this.CurrentBlueprint.isUpgrade())
                    this.upgradeIcon.draw(b);
                string str = " Deluxe  Barn   ";
                SpriteText.drawStringWithScrollBackground(b, this.buildingName, this.xPositionOnScreen + this.maxWidthOfBuildingViewer - IClickableMenu.spaceToClearSideBorder - 16 + 64 + ((this.width - (this.maxWidthOfBuildingViewer + 128)) / 2 - SpriteText.getWidthOfString(str) / 2), this.yPositionOnScreen, str, 1f, -1);
                IClickableMenu.drawTextureBox(b, this.xPositionOnScreen + this.maxWidthOfBuildingViewer - 16, this.yPositionOnScreen + 80, this.maxWidthOfDescription + 64, this.maxWidthOfDescription + 96, this.magicalConstruction ? Color.RoyalBlue : Color.White);
                if (this.magicalConstruction)
                {
                    Utility.drawTextWithShadow(b, Game1.parseText(this.buildingDescription, Game1.dialogueFont, this.maxWidthOfDescription + 32), Game1.dialogueFont, new Vector2((float)(this.xPositionOnScreen + this.maxWidthOfDescription + 64 - 4), (float)(this.yPositionOnScreen + 80 + 16 + 4)), Game1.textColor * 0.25f, 1f, -1f, -1, -1, 0.0f, 3);
                    Utility.drawTextWithShadow(b, Game1.parseText(this.buildingDescription, Game1.dialogueFont, this.maxWidthOfDescription + 32), Game1.dialogueFont, new Vector2((float)(this.xPositionOnScreen + this.maxWidthOfDescription + 64 - 1), (float)(this.yPositionOnScreen + 80 + 16 + 4)), Game1.textColor * 0.25f, 1f, -1f, -1, -1, 0.0f, 3);
                }
                Utility.drawTextWithShadow(b, Game1.parseText(this.buildingDescription, Game1.dialogueFont, this.maxWidthOfDescription + 32), Game1.dialogueFont, new Vector2((float)(this.xPositionOnScreen + this.maxWidthOfDescription + 64), (float)(this.yPositionOnScreen + 80 + 16)), this.magicalConstruction ? Color.PaleGoldenrod : Game1.textColor, 1f, -1f, -1, -1, this.magicalConstruction ? 0.0f : 0.25f, 3);
                Vector2 location = new Vector2((float)(this.xPositionOnScreen + this.maxWidthOfDescription + 16 + 64), (float)(this.yPositionOnScreen + 256 + 32));
                SpriteText.drawString(b, "$", (int)location.X, (int)location.Y, 999999, -1, 999999, 1f, 0.88f, false, -1, "", -1);
                if (this.magicalConstruction)
                {
                    Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\StringsFromCSFiles:LoadGameMenu.cs.11020", (object)this.price), Game1.dialogueFont, new Vector2(location.X + 64f, location.Y + 8f), Game1.textColor * 0.5f, 1f, -1f, -1, -1, this.magicalConstruction ? 0.0f : 0.25f, 3);
                    Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\StringsFromCSFiles:LoadGameMenu.cs.11020", (object)this.price), Game1.dialogueFont, new Vector2((float)((double)location.X + 64.0 + 4.0 - 1.0), location.Y + 8f), Game1.textColor * 0.25f, 1f, -1f, -1, -1, this.magicalConstruction ? 0.0f : 0.25f, 3);
                }
                Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\StringsFromCSFiles:LoadGameMenu.cs.11020", (object)this.price), Game1.dialogueFont, new Vector2((float)((double)location.X + 64.0 + 4.0), location.Y + 4f), Game1.player.money >= this.price ? (this.magicalConstruction ? Color.PaleGoldenrod : Game1.textColor) : Color.Red, 1f, -1f, -1, -1, this.magicalConstruction ? 0.0f : 0.25f, 3);
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
                this.okButton.draw(b, this.blueprints[this.currentBlueprintIndex].doesFarmerHaveEnoughResourcesToBuild() ? Color.White : Color.Gray * 0.8f, 0.88f);
                this.demolishButton.draw(b);
                this.moveButton.draw(b);
            }
            else
            {
                string s = this.upgrading ? Game1.content.LoadString("Strings\\UI:Carpenter_SelectBuilding_Upgrade", (object)new BluePrint(this.CurrentBlueprint.nameOfBuildingToUpgrade).displayName) : (this.demolishing ? Game1.content.LoadString("Strings\\UI:Carpenter_SelectBuilding_Demolish") : Game1.content.LoadString("Strings\\UI:Carpenter_ChooseLocation"));
                SpriteText.drawStringWithScrollBackground(b, s, Game1.viewport.Width / 2 - SpriteText.getWidthOfString(s) / 2, 16, "", 1f, -1);
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
            IClickableMenu.drawHoverText(b, this.hoverText, Game1.dialogueFont, 0, 0, -1, (string)null, -1, (string[])null, (Item)null, 0, -1, -1, -1, -1, 1f, (CraftingRecipe)null);
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
        }

    }
}
