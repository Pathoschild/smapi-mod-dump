using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;
using xTile.Dimensions;
using SObject = StardewValley.Object;

namespace PelicanFiber.Framework
{
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "The field names match the game code where applicable.")]
    internal class ConstructionMenu : IClickableMenu
    {
        /*********
        ** Properties
        *********/
        private readonly string WhereToGo;
        private readonly int maxWidthOfBuildingViewer = 7 * Game1.tileSize;
        private readonly int maxHeightOfBuildingViewer = 8 * Game1.tileSize;
        private readonly int maxWidthOfDescription = 6 * Game1.tileSize;
        private readonly List<Item> ingredients = new List<Item>();
        private bool drawBG = true;
        private string hoverText = "";
        private readonly List<BluePrint> blueprints;
        private int currentBlueprintIndex;
        private ClickableTextureComponent okButton;
        private ClickableTextureComponent cancelButton;
        private ClickableTextureComponent backButton;
        private ClickableTextureComponent forwardButton;
        private ClickableTextureComponent upgradeIcon;
        private ClickableTextureComponent demolishButton;
        private ClickableTextureComponent moveButton;
        private ClickableTextureComponent GoBackButton;
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
        private readonly bool magicalConstruction;
        private readonly Action OnMenuOpened;

        private BluePrint currentBlueprint => this.blueprints[this.currentBlueprintIndex];


        /*********
        ** Public methods
        *********/
        public ConstructionMenu(bool magicalConstruction, Action onMenuOpened)
        {
            this.magicalConstruction = magicalConstruction;
            this.OnMenuOpened = onMenuOpened;

            this.WhereToGo = Game1.player.currentLocation.Name;

            Game1.player.forceCanMove();
            this.ResetBounds();
            this.blueprints = new List<BluePrint>();
            if (magicalConstruction)
            {
                this.blueprints.Add(new BluePrint("Junimo Hut"));
                this.blueprints.Add(new BluePrint("Earth Obelisk"));
                this.blueprints.Add(new BluePrint("Water Obelisk"));
                this.blueprints.Add(new BluePrint("Desert Obelisk"));
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
                this.blueprints.Add(new BluePrint("Fish Pond"));
                int buildingsConstructed = Game1.getFarm().getNumberBuildingsConstructed("Cabin");
                if (Game1.IsMasterGame && buildingsConstructed < Game1.CurrentPlayerLimit - 1)
                {
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
                if (Game1.getFarm().isBuildingConstructed("Shed"))
                    this.blueprints.Add(new BluePrint("Big Shed"));
                this.blueprints.Add(new BluePrint("Shipping Bin"));
            }
            this.SetNewActiveBlueprint();
        }

        public override void performHoverAction(int x, int y)
        {
            this.cancelButton.tryHover(x, y);
            this.GoBackButton.tryHover(x, y, 1f);

            base.performHoverAction(x, y);
            if (!this.onFarm)
            {
                this.backButton.tryHover(x, y, 1f);
                this.forwardButton.tryHover(x, y, 1f);
                this.okButton.tryHover(x, y);
                this.demolishButton.tryHover(x, y);
                this.moveButton.tryHover(x, y);
                if (this.currentBlueprint.isUpgrade() && this.upgradeIcon.containsPoint(x, y))
                    this.hoverText = Game1.content.LoadString("Strings\\UI:Carpenter_Upgrade", new BluePrint(this.currentBlueprint.nameOfBuildingToUpgrade).displayName);
                else if (this.demolishButton.containsPoint(x, y))
                    this.hoverText = Game1.content.LoadString("Strings\\UI:Carpenter_Demolish");
                else if (this.moveButton.containsPoint(x, y))
                    this.hoverText = Game1.content.LoadString("Strings\\UI:Carpenter_MoveBuildings");
                else if (this.okButton.containsPoint(x, y) && this.currentBlueprint.doesFarmerHaveEnoughResourcesToBuild())
                    this.hoverText = Game1.content.LoadString("Strings\\UI:Carpenter_Build");
                else
                    this.hoverText = "";
            }
            else
            {
                if (!this.upgrading && !this.demolishing && !this.moving || this.freeze)
                    return;

                Farm farm = Game1.getFarm();

                foreach (Building b in farm.buildings)
                    b.color.Value = Color.White;

                Building building =
                    farm.getBuildingAt(new Vector2((Game1.viewport.X + Game1.getOldMouseX()) / Game1.tileSize, (Game1.viewport.Y + Game1.getOldMouseY()) / Game1.tileSize))
                    ?? farm.getBuildingAt(new Vector2((Game1.viewport.X + Game1.getOldMouseX()) / Game1.tileSize, (Game1.viewport.Y + Game1.getOldMouseY() + Game1.tileSize * 2) / Game1.tileSize))
                    ?? farm.getBuildingAt(new Vector2((Game1.viewport.X + Game1.getOldMouseX()) / Game1.tileSize, (Game1.viewport.Y + Game1.getOldMouseY() + Game1.tileSize * 3) / Game1.tileSize));

                if (this.upgrading)
                {
                    if (building != null && this.currentBlueprint.nameOfBuildingToUpgrade?.Equals(building.buildingType.Value) == true)
                    {
                        building.color.Value = Color.Lime * 0.8f;
                    }
                    else
                    {
                        if (building == null)
                            return;
                        building.color.Value = Color.Red * 0.8f;
                    }
                }
                else if (this.demolishing)
                {
                    if (building == null || !this.hasPermissionsToDemolish(building))
                        return;
                    building.color.Value = Color.Red * 0.8f;
                }
                else
                {
                    if (!this.moving || building == null || !this.hasPermissionsToMove(building))
                        return;
                    building.color.Value = Color.Lime * 0.8f;
                }
            }
        }

        public bool hasPermissionsToDemolish(Building b)
        {
            return Game1.IsMasterGame;
        }

        public bool hasPermissionsToMove(Building b)
        {
            return Game1.IsMasterGame || Game1.player.team.farmhandsCanMoveBuildings.Value == FarmerTeam.RemoteBuildingPermissions.On || Game1.player.team.farmhandsCanMoveBuildings.Value == FarmerTeam.RemoteBuildingPermissions.OwnedBuildings && b.hasCarpenterPermissions();
        }

        public override bool readyToClose()
        {
            if (base.readyToClose())
                return this.buildingToMove == null;
            return false;
        }

        public override void receiveKeyPress(Keys key)
        {
            if (this.freeze)
                return;
            if (!this.onFarm)
                base.receiveKeyPress(key);
            if (Game1.globalFade || !this.onFarm)
                return;
            if (Game1.options.doesInputListContain(Game1.options.menuButton, key) && this.readyToClose())
                Game1.globalFadeToBlack(this.ReturnToCarpentryMenu);
            else if (Game1.options.doesInputListContain(Game1.options.moveDownButton, key))
                Game1.panScreen(0, 4);
            else if (Game1.options.doesInputListContain(Game1.options.moveRightButton, key))
                Game1.panScreen(4, 0);
            else if (Game1.options.doesInputListContain(Game1.options.moveUpButton, key))
                Game1.panScreen(0, -4);
            else if (Game1.options.doesInputListContain(Game1.options.moveLeftButton, key))
                Game1.panScreen(-4, 0);
        }

        public override void update(GameTime time)
        {
            base.update(time);
            if (!this.onFarm || Game1.globalFade)
                return;
            int num1 = Game1.getOldMouseX() + Game1.viewport.X;
            int num2 = Game1.getOldMouseY() + Game1.viewport.Y;
            if (num1 - Game1.viewport.X < Game1.tileSize)
                Game1.panScreen(-8, 0);
            else if (num1 - (Game1.viewport.X + Game1.viewport.Width) >= -Game1.tileSize * 2)
                Game1.panScreen(8, 0);
            if (num2 - Game1.viewport.Y < Game1.tileSize)
                Game1.panScreen(0, -8);
            else if (num2 - (Game1.viewport.Y + Game1.viewport.Height) >= -Game1.tileSize)
                Game1.panScreen(0, 8);
            foreach (Keys pressedKey in Game1.oldKBState.GetPressedKeys())
                this.receiveKeyPress(pressedKey);

            if (!Game1.IsMultiplayer)
            {
                Farm farm = Game1.getFarm();
                foreach (FarmAnimal animal in farm.animals.Values)
                    animal.MovePosition(Game1.currentGameTime, Game1.viewport, farm);
            }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (this.freeze)
                return;
            Farm farm = Game1.getFarm();


            if (this.GoBackButton != null && this.GoBackButton.containsPoint(x, y))
            {
                this.GoBackButton.scale = this.GoBackButton.baseScale;
                this.GoBackButtonPressed();
            }

            if (!this.onFarm)
                base.receiveLeftClick(x, y, playSound);

            if (this.cancelButton.containsPoint(x, y))
            {
                if (!this.onFarm)
                {
                    this.exitThisMenu();
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
                    Game1.globalFadeToBlack(this.ReturnToCarpentryMenu);
                    Game1.playSound("smallSelect");
                    return;
                }
            }
            if (!this.onFarm && this.backButton.containsPoint(x, y))
            {
                this.currentBlueprintIndex = this.currentBlueprintIndex - 1;
                if (this.currentBlueprintIndex < 0)
                    this.currentBlueprintIndex = this.blueprints.Count - 1;
                this.SetNewActiveBlueprint();
                Game1.playSound("shwip");
                this.backButton.scale = this.backButton.baseScale;
            }
            if (!this.onFarm && this.forwardButton.containsPoint(x, y))
            {
                this.currentBlueprintIndex = (this.currentBlueprintIndex + 1) % this.blueprints.Count;
                this.SetNewActiveBlueprint();
                this.backButton.scale = this.backButton.baseScale;
                Game1.playSound("shwip");
            }
            if (!this.onFarm && this.demolishButton.containsPoint(x, y))
            {
                Game1.globalFadeToBlack(this.SetUpForBuildingPlacement);
                Game1.playSound("smallSelect");
                this.onFarm = true;
                this.demolishing = true;
            }
            if (!this.onFarm && this.moveButton.containsPoint(x, y))
            {
                Game1.globalFadeToBlack(this.SetUpForBuildingPlacement);
                Game1.playSound("smallSelect");
                this.onFarm = true;
                this.moving = true;
            }
            if (this.okButton.containsPoint(x, y) && !this.onFarm && (Game1.player.Money >= this.price && this.blueprints[this.currentBlueprintIndex].doesFarmerHaveEnoughResourcesToBuild()))
            {
                Game1.globalFadeToBlack(this.SetUpForBuildingPlacement);
                Game1.playSound("smallSelect");
                this.onFarm = true;
            }
            if (!this.onFarm || this.freeze || Game1.globalFade)
                return;
            if (this.demolishing)
            {
                Building destroyed = farm.getBuildingAt(new Vector2((Game1.viewport.X + Game1.getOldMouseX()) / Game1.tileSize, (Game1.viewport.Y + Game1.getOldMouseY()) / Game1.tileSize));
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
                    if (destroyed.daysOfConstructionLeft.Value > 0 || destroyed.daysUntilUpgrade.Value > 0)
                        Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_DuringConstruction"), Color.Red, 3500f));
                    else if (destroyed.indoors.Value is AnimalHouse _a && _a.animalsThatLiveHere.Count > 0)
                        Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_AnimalsHere"), Color.Red, 3500f));
                    else if (destroyed.indoors.Value != null && destroyed.indoors.Value.farmers.Any())
                        Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_PlayerHere"), Color.Red, 3500f));
                    else
                    {
                        if (destroyed.indoors.Value is Cabin cabin)
                        {
                            foreach (Farmer player in Game1.getAllFarmers())
                            {
                                if (player.currentLocation.Name == cabin.GetCellarName())
                                {
                                    Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_PlayerHere"), Color.Red, 3500f));
                                    return;
                                }
                            }
                        }
                        if (destroyed.indoors.Value is Cabin cabin2 && cabin2.farmhand.Value.isActive())
                            Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_FarmhandOnline"), Color.Red, 3500f));
                        else
                        {
                            Chest chest = null;
                            if (destroyed.indoors.Value is Cabin cabin3)
                            {
                                List<Item> objList = cabin3.demolish();
                                if (objList.Count > 0)
                                {
                                    chest = new Chest(true);
                                    chest.fixLidFrame();
                                    chest.items.Set(objList);
                                }
                            }
                            if (!farm.destroyStructure(destroyed))
                                return;
                            Game1.flashAlpha = 1f;
                            destroyed.showDestroyedAnimation(Game1.getFarm());
                            Game1.playSound("explosion");
                            Utility.spreadAnimalsAround(destroyed, farm);
                            DelayedAction.functionAfterDelay(this.ReturnToCarpentryMenu, 1500);
                            this.freeze = true;
                            if (chest != null)
                                farm.objects[new Vector2(destroyed.tileX.Value + destroyed.tilesWide.Value / 2, destroyed.tileY.Value + destroyed.tilesHigh.Value / 2)] = chest;
                        }
                    }
                });

                if (destroyed != null)
                {
                    if (destroyed.indoors.Value is Cabin && !Game1.IsMasterGame)
                    {
                        Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_LockFailed"), Color.Red, 3500f));
                        destroyed = null;
                        return;
                    }
                    if (!Game1.IsMasterGame && !this.hasPermissionsToDemolish(destroyed))
                    {
                        destroyed = null;
                        return;
                    }
                }

                if (destroyed?.indoors.Value is Cabin)
                {
                    Cabin cabin = (Cabin)destroyed.indoors.Value;
                    if (cabin.farmhand.Value != null && cabin.farmhand.Value.isCustomized.Value)
                    {
                        Game1.currentLocation.createQuestionDialogue(Game1.content.LoadString("Strings\\UI:Carpenter_DemolishCabinConfirm", cabin.farmhand.Value.Name), Game1.currentLocation.createYesNoResponses(), (f, answer) =>
                        {
                            if (answer == "Yes")
                            {
                                Game1.activeClickableMenu = this;
                                Game1.player.team.demolishLock.RequestLock(continueDemolish, buildingLockFailed);
                            }
                            else
                                DelayedAction.functionAfterDelay(this.ReturnToCarpentryMenu, 500);
                        });
                        return;
                    }
                }
                if (destroyed == null)
                    return;
                Game1.player.team.demolishLock.RequestLock(continueDemolish, buildingLockFailed);
            }
            else if (this.upgrading)
            {
                Building buildingAt = farm.getBuildingAt(new Vector2((Game1.viewport.X + Game1.getOldMouseX()) / Game1.tileSize, (Game1.viewport.Y + Game1.getOldMouseY()) / Game1.tileSize));
                if (buildingAt != null && this.currentBlueprint.name != null && buildingAt.buildingType.Value.Equals(this.currentBlueprint.nameOfBuildingToUpgrade))
                {
                    this.currentBlueprint.consumeResources();
                    buildingAt.daysUntilUpgrade.Value = 2;
                    buildingAt.showUpgradeAnimation(farm);
                    Game1.playSound("axe");
                    DelayedAction.fadeAfterDelay(this.ReturnToCarpentryMenuAfterSuccessfulBuild, 1500);
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
                    this.buildingToMove = farm.getBuildingAt(new Vector2((Game1.viewport.X + Game1.getMouseX()) / Game1.tileSize, (Game1.viewport.Y + Game1.getMouseY()) / Game1.tileSize));
                    if (this.buildingToMove == null)
                        return;
                    if (this.buildingToMove.daysOfConstructionLeft.Value > 0)
                        this.buildingToMove = null;
                    else if (!Game1.IsMasterGame && !this.hasPermissionsToMove(this.buildingToMove))
                        this.buildingToMove = null;
                    else
                    {
                        this.buildingToMove.isMoving = true;
                        Game1.playSound("axchop");
                    }
                }
                else if (farm.buildStructure(this.buildingToMove, new Vector2((Game1.viewport.X + Game1.getMouseX()) / Game1.tileSize, (Game1.viewport.Y + Game1.getMouseY()) / Game1.tileSize), Game1.player))
                {
                    this.buildingToMove.isMoving = false;
                    (this.buildingToMove as ShippingBin)?.initLid();
                    this.buildingToMove.performActionOnBuildingPlacement();
                    this.buildingToMove = null;
                    Game1.playSound("axchop");
                    DelayedAction.playSoundAfterDelay("dirtyHit", 50, (GameLocation)null, -1);
                    DelayedAction.playSoundAfterDelay("dirtyHit", 150, (GameLocation)null, -1);
                }
                else
                    Game1.playSound("cancel");
            }
            else
            {
                Game1.player.team.buildLock.RequestLock((Action)(() =>
                {
                    if (this.onFarm && Game1.locationRequest == null)
                    {
                        if (this.TryToBuild())
                        {
                            this.currentBlueprint.consumeResources();
                            DelayedAction.functionAfterDelay(this.ReturnToCarpentryMenuAfterSuccessfulBuild, 2000);
                            this.freeze = true;
                        }
                        else
                            Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantBuild"), Color.Red, 3500f));
                    }
                    Game1.player.team.buildLock.ReleaseLock();
                }));
            }
        }

        public override void gameWindowSizeChanged(Microsoft.Xna.Framework.Rectangle oldBounds, Microsoft.Xna.Framework.Rectangle newBounds)
        {
            this.ResetBounds();
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
                this.currentBuilding.drawInMenu(b, this.xPositionOnScreen + this.maxWidthOfBuildingViewer / 2 - this.currentBuilding.tilesWide.Value * 64 / 2 - 64, this.yPositionOnScreen + this.maxHeightOfBuildingViewer / 2 - this.currentBuilding.getSourceRectForMenu().Height * 4 / 2);
                if (this.currentBlueprint.isUpgrade())
                    this.upgradeIcon.draw(b);
                string s = " Deluxe  Barn   ";
                if (this.buildingName.Length > s.Length)
                    s = this.buildingName;
                SpriteText.drawStringWithScrollCenteredAt(b, this.buildingName, this.xPositionOnScreen + this.maxWidthOfBuildingViewer - IClickableMenu.spaceToClearSideBorder - 16 + 64 + (this.width - (this.maxWidthOfBuildingViewer + 128)) / 2, this.yPositionOnScreen, SpriteText.getWidthOfString(s));
                int width = LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.it ? this.maxWidthOfDescription + 96 : this.maxWidthOfDescription + 64;
                IClickableMenu.drawTextureBox(b, this.xPositionOnScreen + this.maxWidthOfBuildingViewer - 16, this.yPositionOnScreen + 80, width, this.maxHeightOfBuildingViewer - 32, this.magicalConstruction ? Color.RoyalBlue : Color.White);
                if (this.magicalConstruction)
                {
                    Utility.drawTextWithShadow(b, Game1.parseText(this.buildingDescription, Game1.dialogueFont, width - 32), Game1.dialogueFont, new Vector2((float)(this.xPositionOnScreen + this.maxWidthOfBuildingViewer - 4), (float)(this.yPositionOnScreen + 80 + 16 + 4)), Game1.textColor * 0.25f, 1f, -1f, -1, -1, 0.0f);
                    Utility.drawTextWithShadow(b, Game1.parseText(this.buildingDescription, Game1.dialogueFont, width - 32), Game1.dialogueFont, new Vector2((float)(this.xPositionOnScreen + this.maxWidthOfBuildingViewer - 1), (float)(this.yPositionOnScreen + 80 + 16 + 4)), Game1.textColor * 0.25f, 1f, -1f, -1, -1, 0.0f);
                }
                Utility.drawTextWithShadow(b, Game1.parseText(this.buildingDescription, Game1.dialogueFont, width - 32), Game1.dialogueFont, new Vector2((float)(this.xPositionOnScreen + this.maxWidthOfBuildingViewer), (float)(this.yPositionOnScreen + 80 + 16)), this.magicalConstruction ? Color.PaleGoldenrod : Game1.textColor, 1f, -1f, -1, -1, this.magicalConstruction ? 0.0f : 0.75f);
                Vector2 location = new Vector2((float)(this.xPositionOnScreen + this.maxWidthOfBuildingViewer + 16), (float)(this.yPositionOnScreen + 256 + 32));
                SpriteText.drawString(b, "$", (int)location.X, (int)location.Y);
                if (this.magicalConstruction)
                {
                    Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\StringsFromCSFiles:LoadGameMenu.cs.11020", (object)this.price), Game1.dialogueFont, new Vector2(location.X + 64f, location.Y + 8f), Game1.textColor * 0.5f, 1f, -1f, -1, -1, this.magicalConstruction ? 0.0f : 0.25f);
                    Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\StringsFromCSFiles:LoadGameMenu.cs.11020", (object)this.price), Game1.dialogueFont, new Vector2((float)((double)location.X + 64.0 + 4.0 - 1.0), location.Y + 8f), Game1.textColor * 0.25f, 1f, -1f, -1, -1, this.magicalConstruction ? 0.0f : 0.25f);
                }
                Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\StringsFromCSFiles:LoadGameMenu.cs.11020", (object)this.price), Game1.dialogueFont, new Vector2((float)((double)location.X + 64.0 + 4.0), location.Y + 4f), Game1.player.Money >= this.price ? (this.magicalConstruction ? Color.PaleGoldenrod : Game1.textColor) : Color.Red, 1f, -1f, -1, -1, this.magicalConstruction ? 0.0f : 0.25f);
                location.X -= 16f;
                location.Y -= 21f;
                foreach (Item ingredient in this.ingredients)
                {
                    location.Y += 68f;
                    ingredient.drawInMenu(b, location, 1f);
                    bool flag = !(ingredient is SObject obj) || Game1.player.hasItemInInventory(obj.ParentSheetIndex, ingredient.Stack);
                    if (this.magicalConstruction)
                    {
                        Utility.drawTextWithShadow(b, ingredient.DisplayName, Game1.dialogueFont, new Vector2((float)((double)location.X + 64.0 + 12.0), location.Y + 24f), Game1.textColor * 0.25f, 1f, -1f, -1, -1, this.magicalConstruction ? 0.0f : 0.25f);
                        Utility.drawTextWithShadow(b, ingredient.DisplayName, Game1.dialogueFont, new Vector2((float)((double)location.X + 64.0 + 16.0 - 1.0), location.Y + 24f), Game1.textColor * 0.25f, 1f, -1f, -1, -1, this.magicalConstruction ? 0.0f : 0.25f);
                    }
                    Utility.drawTextWithShadow(b, ingredient.DisplayName, Game1.dialogueFont, new Vector2((float)((double)location.X + 64.0 + 16.0), location.Y + 20f), flag ? (this.magicalConstruction ? Color.PaleGoldenrod : Game1.textColor) : Color.Red, 1f, -1f, -1, -1, this.magicalConstruction ? 0.0f : 0.25f);
                }
                this.backButton.draw(b);
                this.forwardButton.draw(b);
                this.okButton.draw(b, this.blueprints[this.currentBlueprintIndex].doesFarmerHaveEnoughResourcesToBuild() ? Color.White : Color.Gray * 0.8f, 0.88f);
                this.demolishButton.draw(b);
                this.moveButton.draw(b);
            }
            else
            {
                string s = this.upgrading ? Game1.content.LoadString("Strings\\UI:Carpenter_SelectBuilding_Upgrade", (object)new BluePrint(this.currentBlueprint.nameOfBuildingToUpgrade).displayName) : (this.demolishing ? Game1.content.LoadString("Strings\\UI:Carpenter_SelectBuilding_Demolish") : Game1.content.LoadString("Strings\\UI:Carpenter_ChooseLocation"));
                SpriteText.drawStringWithScrollBackground(b, s, Game1.viewport.Width / 2 - SpriteText.getWidthOfString(s) / 2, 16);
                if (!this.upgrading && !this.demolishing && !this.moving)
                {
                    Vector2 vector2 = new Vector2((float)((Game1.viewport.X + Game1.getOldMouseX()) / 64), (float)((Game1.viewport.Y + Game1.getOldMouseY()) / 64));
                    for (int y = 0; y < this.currentBlueprint.tilesHeight; ++y)
                    {
                        for (int x = 0; x < this.currentBlueprint.tilesWidth; ++x)
                        {
                            int structurePlacementTile = this.currentBlueprint.getTileSheetIndexForStructurePlacementTile(x, y);
                            Vector2 tileLocation = new Vector2(vector2.X + (float)x, vector2.Y + (float)y);
                            if (!(Game1.currentLocation as BuildableGameLocation).isBuildable(tileLocation))
                                ++structurePlacementTile;
                            b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, tileLocation * 64f), new Microsoft.Xna.Framework.Rectangle(194 + structurePlacementTile * 16, 388, 16, 16), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.999f);
                        }
                    }
                }
                else if (this.moving && this.buildingToMove != null)
                {
                    Vector2 vector2_1 = new Vector2((float)((Game1.viewport.X + Game1.getOldMouseX()) / 64), (float)((Game1.viewport.Y + Game1.getOldMouseY()) / 64));
                    BuildableGameLocation currentLocation = Game1.currentLocation as BuildableGameLocation;
                    for (int y = 0; y < this.buildingToMove.tilesHigh.Value; ++y)
                    {
                        for (int x = 0; x < this.buildingToMove.tilesWide.Value; ++x)
                        {
                            int structurePlacementTile = this.buildingToMove.getTileSheetIndexForStructurePlacementTile(x, y);
                            Vector2 vector2_2 = new Vector2(vector2_1.X + (float)x, vector2_1.Y + (float)y);
                            bool flag = currentLocation.buildings.Contains(this.buildingToMove) && this.buildingToMove.occupiesTile(vector2_2);
                            if (!currentLocation.isBuildable(vector2_2) && !flag)
                                ++structurePlacementTile;
                            b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, vector2_2 * 64f), new Microsoft.Xna.Framework.Rectangle(194 + structurePlacementTile * 16, 388, 16, 16), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.999f);
                        }
                    }
                }
            }
            this.cancelButton.draw(b);
            this.drawMouse(b);
            if (this.hoverText.Length <= 0)
                return;
            IClickableMenu.drawHoverText(b, this.hoverText, Game1.dialogueFont);
        }


        /*********
        ** Private methods
        *********/
        private void ResetBounds()
        {
            this.xPositionOnScreen = Game1.viewport.Width / 2 - this.maxWidthOfBuildingViewer - IClickableMenu.spaceToClearSideBorder;
            this.yPositionOnScreen = Game1.viewport.Height / 2 - this.maxHeightOfBuildingViewer / 2 - IClickableMenu.spaceToClearTopBorder + Game1.tileSize / 2;
            this.width = this.maxWidthOfBuildingViewer + this.maxWidthOfDescription + IClickableMenu.spaceToClearSideBorder * 2 + Game1.tileSize;
            this.height = this.maxHeightOfBuildingViewer + IClickableMenu.spaceToClearTopBorder;
            this.initialize(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, true);
            this.okButton = new ClickableTextureComponent("OK", new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen + this.width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - Game1.tileSize * 3 - Game1.pixelZoom * 3, this.yPositionOnScreen + this.maxHeightOfBuildingViewer + Game1.tileSize, Game1.tileSize, Game1.tileSize), null, null, Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(366, 373, 16, 16), Game1.pixelZoom);
            this.cancelButton = new ClickableTextureComponent("OK", new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen + this.width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - Game1.tileSize, this.yPositionOnScreen + this.maxHeightOfBuildingViewer + Game1.tileSize, Game1.tileSize, Game1.tileSize), null, null, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 47), 1f);
            this.backButton = new ClickableTextureComponent(new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen + Game1.tileSize, this.yPositionOnScreen + this.maxHeightOfBuildingViewer + Game1.tileSize, 12 * Game1.pixelZoom, 11 * Game1.pixelZoom), Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(352, 495, 12, 11), Game1.pixelZoom);
            this.forwardButton = new ClickableTextureComponent(new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen + this.maxWidthOfBuildingViewer - Game1.tileSize * 4 + Game1.tileSize / 4, this.yPositionOnScreen + this.maxHeightOfBuildingViewer + Game1.tileSize, 12 * Game1.pixelZoom, 11 * Game1.pixelZoom), Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(365, 495, 12, 11), Game1.pixelZoom);
            this.demolishButton = new ClickableTextureComponent(Game1.content.LoadString("Strings\\UI:Carpenter_Demolish"), new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen + this.width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - Game1.tileSize * 2 - Game1.pixelZoom * 2, this.yPositionOnScreen + this.maxHeightOfBuildingViewer + Game1.tileSize - Game1.pixelZoom, Game1.tileSize, Game1.tileSize), null, null, Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(348, 372, 17, 17), Game1.pixelZoom);
            this.upgradeIcon = new ClickableTextureComponent(new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen + this.maxWidthOfBuildingViewer - Game1.tileSize * 2 + Game1.tileSize / 2, this.yPositionOnScreen + Game1.pixelZoom * 2, 9 * Game1.pixelZoom, 13 * Game1.pixelZoom), Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(402, 328, 9, 13), Game1.pixelZoom);
            this.moveButton = new ClickableTextureComponent(Game1.content.LoadString("Strings\\UI:Carpenter_MoveBuildings"), new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen + this.width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - Game1.tileSize * 4 - Game1.pixelZoom * 5, this.yPositionOnScreen + this.maxHeightOfBuildingViewer + Game1.tileSize, Game1.tileSize, Game1.tileSize), null, null, Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(257, 284, 16, 16), Game1.pixelZoom);
            this.GoBackButton = new ClickableTextureComponent(new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen - 2 * Game1.tileSize, this.yPositionOnScreen - Game1.tileSize, 12 * Game1.pixelZoom, 11 * Game1.pixelZoom), Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(352, 495, 12, 11), Game1.pixelZoom);
        }

        private void SetNewActiveBlueprint()
        {
            this.currentBuilding = !this.blueprints[this.currentBlueprintIndex].name.Contains("Coop") ? (!this.blueprints[this.currentBlueprintIndex].name.Contains("Barn") ? (!this.blueprints[this.currentBlueprintIndex].name.Contains("Mill") ? (!this.blueprints[this.currentBlueprintIndex].name.Contains("Junimo Hut") ? (!this.blueprints[this.currentBlueprintIndex].name.Contains("Shipping Bin") ? (!this.blueprints[this.currentBlueprintIndex].name.Contains("Fish Pond") ? new Building(this.blueprints[this.currentBlueprintIndex], Vector2.Zero) : (Building)new FishPond(this.blueprints[this.currentBlueprintIndex], Vector2.Zero)) : (Building)new ShippingBin(this.blueprints[this.currentBlueprintIndex], Vector2.Zero)) : (Building)new JunimoHut(this.blueprints[this.currentBlueprintIndex], Vector2.Zero)) : (Building)new Mill(this.blueprints[this.currentBlueprintIndex], Vector2.Zero)) : (Building)new Barn(this.blueprints[this.currentBlueprintIndex], Vector2.Zero)) : (Building)new Coop(this.blueprints[this.currentBlueprintIndex], Vector2.Zero);
            this.price = this.blueprints[this.currentBlueprintIndex].moneyRequired;
            this.ingredients.Clear();
            foreach (KeyValuePair<int, int> keyValuePair in this.blueprints[this.currentBlueprintIndex].itemsRequired)
                this.ingredients.Add((Item)new SObject(keyValuePair.Key, keyValuePair.Value, false, -1, 0));
            this.buildingDescription = this.blueprints[this.currentBlueprintIndex].description;
            this.buildingName = this.blueprints[this.currentBlueprintIndex].displayName;
        }

        private void GoBackButtonPressed()
        {
            if (this.readyToClose())
                this.exitThisMenu();
        }

        private bool TryToBuild()
        {
            return Game1.getFarm().buildStructure(this.currentBlueprint, new Vector2((Game1.viewport.X + Game1.getOldMouseX()) / Game1.tileSize, (Game1.viewport.Y + Game1.getOldMouseY()) / Game1.tileSize), Game1.player, this.magicalConstruction);
        }

        private void ReturnToCarpentryMenu()
        {
            Game1.currentLocation.cleanupBeforePlayerExit();
            Game1.currentLocation = Game1.getLocationFromName(this.WhereToGo);
            //Game1.currentLocation = Game1.getLocationFromName(this.magicalConstruction ? "WizardHouse" : "ScienceHouse");
            Game1.currentLocation.resetForPlayerEntry();
            Game1.globalFadeToClear();
            this.onFarm = false;
            this.ResetBounds();
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

        private void ReturnToCarpentryMenuAfterSuccessfulBuild()
        {
            Game1.currentLocation.cleanupBeforePlayerExit();
            Game1.currentLocation = Game1.getLocationFromName(this.WhereToGo);
            //Game1.currentLocation = Game1.getLocationFromName(this.magicalConstruction ? "WizardHouse" : "ScienceHouse");
            Game1.currentLocation.resetForPlayerEntry();
            Game1.globalFadeToClear(this.RobinConstructionMessage);
            Game1.displayHUD = true;
            Game1.viewportFreeze = false;
            Game1.viewport.Location = new Location(5 * Game1.tileSize, 24 * Game1.tileSize);
            this.freeze = true;
            Game1.displayFarmer = true;
        }

        private void RobinConstructionMessage()
        {
            this.exitThisMenu();
            Game1.player.forceCanMove();

            Game1.activeClickableMenu = new ConstructionMenu(this.magicalConstruction, this.OnMenuOpened);
            //if (this.magicalConstruction)
            //    return;
            //string path = "Data\\ExtraDialogue:Robin_" + (this.upgrading ? "Upgrade" : "New") + "Construction";
            //if (Utility.isFestivalDay(Game1.dayOfMonth + 1, Game1.currentSeason))
            //    path += "_Festival";
            //Game1.drawDialogue(Game1.getCharacterFromName("Robin"), Game1.content.LoadString(path, (object)this.currentBlueprint.name.ToLower(), (object)((IEnumerable<string>)this.currentBlueprint.name.ToLower().Split(' ')).Last<string>()));
        }

        private void SetUpForBuildingPlacement()
        {
            Game1.currentLocation.cleanupBeforePlayerExit();
            this.hoverText = "";
            Game1.currentLocation = Game1.getFarm();
            Game1.currentLocation.resetForPlayerEntry();
            Game1.globalFadeToClear();
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
            if (this.demolishing || this.currentBlueprint.nameOfBuildingToUpgrade == null || (this.currentBlueprint.nameOfBuildingToUpgrade.Length <= 0 || this.moving))
                return;
            this.upgrading = true;
        }
    }
}
