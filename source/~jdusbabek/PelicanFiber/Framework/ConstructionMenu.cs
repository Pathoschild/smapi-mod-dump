/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jdusbabek/stardewvalley
**
*************************************************/

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
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using SObject = StardewValley.Object;

namespace PelicanFiber.Framework
{
    [SuppressMessage("ReSharper", "InconsistentNaming",
        Justification = "The field names match the game code where applicable.")]
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

        private BluePrint currentBlueprint => blueprints[currentBlueprintIndex];


        /*********
        ** Public methods
        *********/
        public ConstructionMenu(bool magicalConstruction, Action onMenuOpened)
        {
            this.magicalConstruction = magicalConstruction;
            OnMenuOpened = onMenuOpened;

            WhereToGo = Game1.player.currentLocation.Name;

            Game1.player.forceCanMove();
            ResetBounds();
            blueprints = new List<BluePrint>();
            if (magicalConstruction)
            {
                blueprints.Add(new BluePrint("Junimo Hut"));
                blueprints.Add(new BluePrint("Earth Obelisk"));
                blueprints.Add(new BluePrint("Water Obelisk"));
                blueprints.Add(new BluePrint("Desert Obelisk"));
                blueprints.Add(new BluePrint("Gold Clock"));
            }
            else
            {
                blueprints.Add(new BluePrint("Coop"));
                blueprints.Add(new BluePrint("Barn"));
                blueprints.Add(new BluePrint("Well"));
                blueprints.Add(new BluePrint("Silo"));
                blueprints.Add(new BluePrint("Mill"));
                blueprints.Add(new BluePrint("Shed"));
                blueprints.Add(new BluePrint("Fish Pond"));
                var buildingsConstructed = Game1.getFarm().getNumberBuildingsConstructed("Cabin");
                if (Game1.IsMasterGame && buildingsConstructed < Game1.CurrentPlayerLimit - 1)
                {
                    blueprints.Add(new BluePrint("Stone Cabin"));
                    blueprints.Add(new BluePrint("Plank Cabin"));
                    blueprints.Add(new BluePrint("Log Cabin"));
                }

                if (Game1.getFarm().getNumberBuildingsConstructed("Stable") < buildingsConstructed + 1)
                    blueprints.Add(new BluePrint("Stable"));
                blueprints.Add(new BluePrint("Slime Hutch"));
                if (Game1.getFarm().isBuildingConstructed("Coop"))
                    blueprints.Add(new BluePrint("Big Coop"));
                if (Game1.getFarm().isBuildingConstructed("Big Coop"))
                    blueprints.Add(new BluePrint("Deluxe Coop"));
                if (Game1.getFarm().isBuildingConstructed("Barn"))
                    blueprints.Add(new BluePrint("Big Barn"));
                if (Game1.getFarm().isBuildingConstructed("Big Barn"))
                    blueprints.Add(new BluePrint("Deluxe Barn"));
                if (Game1.getFarm().isBuildingConstructed("Shed"))
                    blueprints.Add(new BluePrint("Big Shed"));
                blueprints.Add(new BluePrint("Shipping Bin"));
            }

            SetNewActiveBlueprint();
        }

        public override void performHoverAction(int x, int y)
        {
            cancelButton.tryHover(x, y);
            GoBackButton.tryHover(x, y, 1f);

            base.performHoverAction(x, y);
            if (!onFarm)
            {
                backButton.tryHover(x, y, 1f);
                forwardButton.tryHover(x, y, 1f);
                okButton.tryHover(x, y);
                demolishButton.tryHover(x, y);
                moveButton.tryHover(x, y);
                if (currentBlueprint.isUpgrade() && upgradeIcon.containsPoint(x, y))
                    hoverText = Game1.content.LoadString("Strings\\UI:Carpenter_Upgrade",
                        new BluePrint(currentBlueprint.nameOfBuildingToUpgrade).displayName);
                else if (demolishButton.containsPoint(x, y))
                    hoverText = Game1.content.LoadString("Strings\\UI:Carpenter_Demolish");
                else if (moveButton.containsPoint(x, y))
                    hoverText = Game1.content.LoadString("Strings\\UI:Carpenter_MoveBuildings");
                else if (okButton.containsPoint(x, y) && currentBlueprint.doesFarmerHaveEnoughResourcesToBuild())
                    hoverText = Game1.content.LoadString("Strings\\UI:Carpenter_Build");
                else
                    hoverText = "";
            }
            else
            {
                if (!upgrading && !demolishing && !moving || freeze)
                    return;

                var farm = Game1.getFarm();

                foreach (var b in farm.buildings)
                    b.color.Value = Color.White;

                var building =
                    farm.getBuildingAt(new Vector2((Game1.uiViewport.X + Game1.getOldMouseX()) / Game1.tileSize,
                        (Game1.uiViewport.Y + Game1.getOldMouseY()) / Game1.tileSize))
                    ?? farm.getBuildingAt(new Vector2((Game1.uiViewport.X + Game1.getOldMouseX()) / Game1.tileSize,
                        (Game1.uiViewport.Y + Game1.getOldMouseY() + Game1.tileSize * 2) / Game1.tileSize))
                    ?? farm.getBuildingAt(new Vector2((Game1.uiViewport.X + Game1.getOldMouseX()) / Game1.tileSize,
                        (Game1.uiViewport.Y + Game1.getOldMouseY() + Game1.tileSize * 3) / Game1.tileSize));

                if (upgrading)
                {
                    if (building != null &&
                        currentBlueprint.nameOfBuildingToUpgrade?.Equals(building.buildingType.Value) == true)
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
                else if (demolishing)
                {
                    if (building == null || !hasPermissionsToDemolish(building))
                        return;
                    building.color.Value = Color.Red * 0.8f;
                }
                else
                {
                    if (!moving || building == null || !hasPermissionsToMove(building))
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
            return Game1.IsMasterGame ||
                   Game1.player.team.farmhandsCanMoveBuildings.Value == FarmerTeam.RemoteBuildingPermissions.On ||
                   Game1.player.team.farmhandsCanMoveBuildings.Value ==
                   FarmerTeam.RemoteBuildingPermissions.OwnedBuildings && b.hasCarpenterPermissions();
        }

        public override bool readyToClose()
        {
            if (base.readyToClose())
                return buildingToMove == null;
            return false;
        }

        public override void receiveKeyPress(Keys key)
        {
            if (freeze)
                return;
            if (!onFarm)
                base.receiveKeyPress(key);
            if (Game1.globalFade || !onFarm)
                return;
            if (Game1.options.doesInputListContain(Game1.options.menuButton, key) && readyToClose())
                Game1.globalFadeToBlack(ReturnToCarpentryMenu);
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
            if (!onFarm || Game1.globalFade)
                return;
            var num1 = Game1.getOldMouseX() + Game1.uiViewport.X;
            var num2 = Game1.getOldMouseY() + Game1.uiViewport.Y;
            if (num1 - Game1.uiViewport.X < Game1.tileSize)
                Game1.panScreen(-8, 0);
            else if (num1 - (Game1.uiViewport.X + Game1.uiViewport.Width) >= -Game1.tileSize * 2)
                Game1.panScreen(8, 0);
            if (num2 - Game1.uiViewport.Y < Game1.tileSize)
                Game1.panScreen(0, -8);
            else if (num2 - (Game1.uiViewport.Y + Game1.uiViewport.Height) >= -Game1.tileSize)
                Game1.panScreen(0, 8);
            foreach (var pressedKey in Game1.oldKBState.GetPressedKeys())
                receiveKeyPress(pressedKey);

            if (!Game1.IsMultiplayer)
            {
                var farm = Game1.getFarm();
                foreach (var animal in farm.animals.Values)
                    animal.MovePosition(Game1.currentGameTime, Game1.uiViewport, farm);
            }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (freeze)
                return;
            var farm = Game1.getFarm();


            if (GoBackButton != null && GoBackButton.containsPoint(x, y))
            {
                GoBackButton.scale = GoBackButton.baseScale;
                GoBackButtonPressed();
            }

            if (!onFarm)
                base.receiveLeftClick(x, y, playSound);

            if (cancelButton.containsPoint(x, y))
            {
                if (!onFarm)
                {
                    exitThisMenu();
                    Game1.player.forceCanMove();
                    Game1.playSound("bigDeSelect");
                }
                else
                {
                    if (moving && buildingToMove != null)
                    {
                        Game1.playSound("cancel");
                        return;
                    }

                    Game1.globalFadeToBlack(ReturnToCarpentryMenu);
                    Game1.playSound("smallSelect");
                    return;
                }
            }

            if (!onFarm && backButton.containsPoint(x, y))
            {
                currentBlueprintIndex = currentBlueprintIndex - 1;
                if (currentBlueprintIndex < 0)
                    currentBlueprintIndex = blueprints.Count - 1;
                SetNewActiveBlueprint();
                Game1.playSound("shwip");
                backButton.scale = backButton.baseScale;
            }

            if (!onFarm && forwardButton.containsPoint(x, y))
            {
                currentBlueprintIndex = (currentBlueprintIndex + 1) % blueprints.Count;
                SetNewActiveBlueprint();
                backButton.scale = backButton.baseScale;
                Game1.playSound("shwip");
            }

            if (!onFarm && demolishButton.containsPoint(x, y))
            {
                Game1.globalFadeToBlack(SetUpForBuildingPlacement);
                Game1.playSound("smallSelect");
                onFarm = true;
                demolishing = true;
            }

            if (!onFarm && moveButton.containsPoint(x, y))
            {
                Game1.globalFadeToBlack(SetUpForBuildingPlacement);
                Game1.playSound("smallSelect");
                onFarm = true;
                moving = true;
            }

            if (okButton.containsPoint(x, y) && !onFarm && Game1.player.Money >= price &&
                blueprints[currentBlueprintIndex].doesFarmerHaveEnoughResourcesToBuild())
            {
                Game1.globalFadeToBlack(SetUpForBuildingPlacement);
                Game1.playSound("smallSelect");
                onFarm = true;
            }

            if (!onFarm || freeze || Game1.globalFade)
                return;
            if (demolishing)
            {
                var destroyed = farm.getBuildingAt(new Vector2(
                    (Game1.uiViewport.X + Game1.getOldMouseX()) / Game1.tileSize,
                    (Game1.uiViewport.Y + Game1.getOldMouseY()) / Game1.tileSize));
                Action buildingLockFailed = () =>
                {
                    if (!demolishing)
                        return;
                    Game1.addHUDMessage(new HUDMessage(
                        Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_LockFailed"), Color.Red, 3500f));
                };
                Action continueDemolish = () =>
                {
                    if (!demolishing || destroyed == null || !farm.buildings.Contains(destroyed))
                        return;
                    if (destroyed.daysOfConstructionLeft.Value > 0 || destroyed.daysUntilUpgrade.Value > 0)
                    {
                        Game1.addHUDMessage(new HUDMessage(
                            Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_DuringConstruction"),
                            Color.Red, 3500f));
                    }
                    else if (destroyed.indoors.Value is AnimalHouse _a && _a.animalsThatLiveHere.Count > 0)
                    {
                        Game1.addHUDMessage(new HUDMessage(
                            Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_AnimalsHere"), Color.Red,
                            3500f));
                    }
                    else if (destroyed.indoors.Value != null && destroyed.indoors.Value.farmers.Any())
                    {
                        Game1.addHUDMessage(new HUDMessage(
                            Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_PlayerHere"), Color.Red,
                            3500f));
                    }
                    else
                    {
                        if (destroyed.indoors.Value is Cabin cabin)
                            foreach (var player in Game1.getAllFarmers())
                                if (player.currentLocation.Name == cabin.GetCellarName())
                                {
                                    Game1.addHUDMessage(new HUDMessage(
                                        Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_PlayerHere"),
                                        Color.Red, 3500f));
                                    return;
                                }

                        if (destroyed.indoors.Value is Cabin cabin2 && cabin2.farmhand.Value.isActive())
                        {
                            Game1.addHUDMessage(new HUDMessage(
                                Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_FarmhandOnline"),
                                Color.Red, 3500f));
                        }
                        else
                        {
                            Chest chest = null;
                            if (destroyed.indoors.Value is Cabin cabin3)
                            {
                                var objList = cabin3.demolish();
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
                            DelayedAction.functionAfterDelay(ReturnToCarpentryMenu, 1500);
                            freeze = true;
                            if (chest != null)
                                farm.objects[
                                    new Vector2(destroyed.tileX.Value + destroyed.tilesWide.Value / 2,
                                        destroyed.tileY.Value + destroyed.tilesHigh.Value / 2)] = chest;
                        }
                    }
                };

                if (destroyed != null)
                {
                    if (destroyed.indoors.Value is Cabin && !Game1.IsMasterGame)
                    {
                        Game1.addHUDMessage(new HUDMessage(
                            Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_LockFailed"), Color.Red,
                            3500f));
                        destroyed = null;
                        return;
                    }

                    if (!Game1.IsMasterGame && !hasPermissionsToDemolish(destroyed))
                    {
                        destroyed = null;
                        return;
                    }
                }

                if (destroyed?.indoors.Value is Cabin)
                {
                    var cabin = (Cabin) destroyed.indoors.Value;
                    if (cabin.farmhand.Value != null && cabin.farmhand.Value.isCustomized.Value)
                    {
                        Game1.currentLocation.createQuestionDialogue(
                            Game1.content.LoadString("Strings\\UI:Carpenter_DemolishCabinConfirm",
                                cabin.farmhand.Value.Name), Game1.currentLocation.createYesNoResponses(), (f, answer) =>
                            {
                                if (answer == "Yes")
                                {
                                    Game1.activeClickableMenu = this;
                                    Game1.player.team.demolishLock.RequestLock(continueDemolish, buildingLockFailed);
                                }
                                else
                                {
                                    DelayedAction.functionAfterDelay(ReturnToCarpentryMenu, 500);
                                }
                            });
                        return;
                    }
                }

                if (destroyed == null)
                    return;
                Game1.player.team.demolishLock.RequestLock(continueDemolish, buildingLockFailed);
            }
            else if (upgrading)
            {
                var buildingAt = farm.getBuildingAt(new Vector2(
                    (Game1.uiViewport.X + Game1.getOldMouseX()) / Game1.tileSize,
                    (Game1.uiViewport.Y + Game1.getOldMouseY()) / Game1.tileSize));
                if (buildingAt != null && currentBlueprint.name != null &&
                    buildingAt.buildingType.Value.Equals(currentBlueprint.nameOfBuildingToUpgrade))
                {
                    currentBlueprint.consumeResources();
                    buildingAt.daysUntilUpgrade.Value = 2;
                    buildingAt.showUpgradeAnimation(farm);
                    Game1.playSound("axe");
                    DelayedAction.fadeAfterDelay(ReturnToCarpentryMenuAfterSuccessfulBuild, 1500);
                    freeze = true;
                }
                else
                {
                    if (buildingAt == null)
                        return;
                    Game1.addHUDMessage(new HUDMessage(
                        Game1.content.LoadString("Strings\\UI:Carpenter_CantUpgrade_BuildingType"), Color.Red, 3500f));
                }
            }
            else if (moving)
            {
                if (buildingToMove == null)
                {
                    buildingToMove = farm.getBuildingAt(new Vector2(
                        (Game1.uiViewport.X + Game1.getMouseX()) / Game1.tileSize,
                        (Game1.uiViewport.Y + Game1.getMouseY()) / Game1.tileSize));
                    if (buildingToMove == null)
                        return;
                    if (buildingToMove.daysOfConstructionLeft.Value > 0)
                    {
                        buildingToMove = null;
                    }
                    else if (!Game1.IsMasterGame && !hasPermissionsToMove(buildingToMove))
                    {
                        buildingToMove = null;
                    }
                    else
                    {
                        buildingToMove.isMoving = true;
                        Game1.playSound("axchop");
                    }
                }
                else if (farm.buildStructure(buildingToMove,
                    new Vector2((Game1.uiViewport.X + Game1.getMouseX()) / Game1.tileSize,
                        (Game1.uiViewport.Y + Game1.getMouseY()) / Game1.tileSize), Game1.player))
                {
                    buildingToMove.isMoving = false;
                    (buildingToMove as ShippingBin)?.initLid();
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
            }
            else
            {
                Game1.player.team.buildLock.RequestLock(() =>
                {
                    if (onFarm && Game1.locationRequest == null)
                    {
                        if (TryToBuild())
                        {
                            currentBlueprint.consumeResources();
                            DelayedAction.functionAfterDelay(ReturnToCarpentryMenuAfterSuccessfulBuild, 2000);
                            freeze = true;
                        }
                        else
                        {
                            Game1.addHUDMessage(new HUDMessage(
                                Game1.content.LoadString("Strings\\UI:Carpenter_CantBuild"), Color.Red, 3500f));
                        }
                    }

                    Game1.player.team.buildLock.ReleaseLock();
                });
            }
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            ResetBounds();
        }

        public override void draw(SpriteBatch b)
        {
            if (drawBG)
                b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.5f);
            if (Game1.globalFade || freeze)
                return;
            if (!onFarm)
            {
                base.draw(b);
                drawTextureBox(b, xPositionOnScreen - 96, yPositionOnScreen - 16, maxWidthOfBuildingViewer + 64,
                    maxHeightOfBuildingViewer + 64, magicalConstruction ? Color.RoyalBlue : Color.White);
                currentBuilding.drawInMenu(b,
                    xPositionOnScreen + maxWidthOfBuildingViewer / 2 - currentBuilding.tilesWide.Value * 64 / 2 - 64,
                    yPositionOnScreen + maxHeightOfBuildingViewer / 2 -
                    currentBuilding.getSourceRectForMenu().Height * 4 / 2);
                if (currentBlueprint.isUpgrade())
                    upgradeIcon.draw(b);
                var s = " Deluxe  Barn   ";
                if (buildingName.Length > s.Length)
                    s = buildingName;
                SpriteText.drawStringWithScrollCenteredAt(b, buildingName,
                    xPositionOnScreen + maxWidthOfBuildingViewer - spaceToClearSideBorder - 16 + 64 +
                    (this.width - (maxWidthOfBuildingViewer + 128)) / 2, yPositionOnScreen,
                    SpriteText.getWidthOfString(s));
                var width = LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.it
                    ? maxWidthOfDescription + 96
                    : maxWidthOfDescription + 64;
                drawTextureBox(b, xPositionOnScreen + maxWidthOfBuildingViewer - 16, yPositionOnScreen + 80, width,
                    maxHeightOfBuildingViewer - 32, magicalConstruction ? Color.RoyalBlue : Color.White);
                if (magicalConstruction)
                {
                    Utility.drawTextWithShadow(b, Game1.parseText(buildingDescription, Game1.dialogueFont, width - 32),
                        Game1.dialogueFont,
                        new Vector2(xPositionOnScreen + maxWidthOfBuildingViewer - 4, yPositionOnScreen + 80 + 16 + 4),
                        Game1.textColor * 0.25f, 1f, -1f, -1, -1, 0.0f);
                    Utility.drawTextWithShadow(b, Game1.parseText(buildingDescription, Game1.dialogueFont, width - 32),
                        Game1.dialogueFont,
                        new Vector2(xPositionOnScreen + maxWidthOfBuildingViewer - 1, yPositionOnScreen + 80 + 16 + 4),
                        Game1.textColor * 0.25f, 1f, -1f, -1, -1, 0.0f);
                }

                Utility.drawTextWithShadow(b, Game1.parseText(buildingDescription, Game1.dialogueFont, width - 32),
                    Game1.dialogueFont,
                    new Vector2(xPositionOnScreen + maxWidthOfBuildingViewer, yPositionOnScreen + 80 + 16),
                    magicalConstruction ? Color.PaleGoldenrod : Game1.textColor, 1f, -1f, -1, -1,
                    magicalConstruction ? 0.0f : 0.75f);
                var location = new Vector2(xPositionOnScreen + maxWidthOfBuildingViewer + 16,
                    yPositionOnScreen + 256 + 32);
                SpriteText.drawString(b, "$", (int) location.X, (int) location.Y);
                if (magicalConstruction)
                {
                    Utility.drawTextWithShadow(b,
                        Game1.content.LoadString("Strings\\StringsFromCSFiles:LoadGameMenu.cs.11020", price),
                        Game1.dialogueFont, new Vector2(location.X + 64f, location.Y + 8f), Game1.textColor * 0.5f, 1f,
                        -1f, -1, -1, magicalConstruction ? 0.0f : 0.25f);
                    Utility.drawTextWithShadow(b,
                        Game1.content.LoadString("Strings\\StringsFromCSFiles:LoadGameMenu.cs.11020", price),
                        Game1.dialogueFont, new Vector2((float) (location.X + 64.0 + 4.0 - 1.0), location.Y + 8f),
                        Game1.textColor * 0.25f, 1f, -1f, -1, -1, magicalConstruction ? 0.0f : 0.25f);
                }

                Utility.drawTextWithShadow(b,
                    Game1.content.LoadString("Strings\\StringsFromCSFiles:LoadGameMenu.cs.11020", price),
                    Game1.dialogueFont, new Vector2((float) (location.X + 64.0 + 4.0), location.Y + 4f),
                    Game1.player.Money >= price
                        ? magicalConstruction ? Color.PaleGoldenrod : Game1.textColor
                        : Color.Red, 1f, -1f, -1, -1, magicalConstruction ? 0.0f : 0.25f);
                location.X -= 16f;
                location.Y -= 21f;
                foreach (var ingredient in ingredients)
                {
                    location.Y += 68f;
                    ingredient.drawInMenu(b, location, 1f);
                    var flag = !(ingredient is SObject obj) ||
                               Game1.player.hasItemInInventory(obj.ParentSheetIndex, ingredient.Stack);
                    if (magicalConstruction)
                    {
                        Utility.drawTextWithShadow(b, ingredient.DisplayName, Game1.dialogueFont,
                            new Vector2((float) (location.X + 64.0 + 12.0), location.Y + 24f), Game1.textColor * 0.25f,
                            1f, -1f, -1, -1, magicalConstruction ? 0.0f : 0.25f);
                        Utility.drawTextWithShadow(b, ingredient.DisplayName, Game1.dialogueFont,
                            new Vector2((float) (location.X + 64.0 + 16.0 - 1.0), location.Y + 24f),
                            Game1.textColor * 0.25f, 1f, -1f, -1, -1, magicalConstruction ? 0.0f : 0.25f);
                    }

                    Utility.drawTextWithShadow(b, ingredient.DisplayName, Game1.dialogueFont,
                        new Vector2((float) (location.X + 64.0 + 16.0), location.Y + 20f),
                        flag ? magicalConstruction ? Color.PaleGoldenrod : Game1.textColor : Color.Red, 1f, -1f, -1, -1,
                        magicalConstruction ? 0.0f : 0.25f);
                }

                backButton.draw(b);
                forwardButton.draw(b);
                okButton.draw(b,
                    blueprints[currentBlueprintIndex].doesFarmerHaveEnoughResourcesToBuild()
                        ? Color.White
                        : Color.Gray * 0.8f, 0.88f);
                demolishButton.draw(b);
                moveButton.draw(b);
            }
            else
            {
                var s = upgrading
                    ?
                    Game1.content.LoadString("Strings\\UI:Carpenter_SelectBuilding_Upgrade",
                        new BluePrint(currentBlueprint.nameOfBuildingToUpgrade).displayName)
                    : demolishing
                        ? Game1.content.LoadString("Strings\\UI:Carpenter_SelectBuilding_Demolish")
                        : Game1.content.LoadString("Strings\\UI:Carpenter_ChooseLocation");
                SpriteText.drawStringWithScrollBackground(b, s,
                    Game1.uiViewport.Width / 2 - SpriteText.getWidthOfString(s) / 2, 16);
                if (!upgrading && !demolishing && !moving)
                {
                    var vector2 = new Vector2((Game1.uiViewport.X + Game1.getOldMouseX()) / 64,
                        (Game1.uiViewport.Y + Game1.getOldMouseY()) / 64);
                    for (var y = 0; y < currentBlueprint.tilesHeight; ++y)
                    for (var x = 0; x < currentBlueprint.tilesWidth; ++x)
                    {
                        var structurePlacementTile = currentBlueprint.getTileSheetIndexForStructurePlacementTile(x, y);
                        var tileLocation = new Vector2(vector2.X + x, vector2.Y + y);
                        if (!(Game1.currentLocation as BuildableGameLocation).isBuildable(tileLocation))
                            ++structurePlacementTile;
                        b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.uiViewport, tileLocation * 64f),
                            new Rectangle(194 + structurePlacementTile * 16, 388, 16, 16), Color.White, 0.0f,
                            Vector2.Zero, 4f, SpriteEffects.None, 0.999f);
                    }
                }
                else if (moving && buildingToMove != null)
                {
                    var vector2_1 = new Vector2((Game1.uiViewport.X + Game1.getOldMouseX()) / 64,
                        (Game1.uiViewport.Y + Game1.getOldMouseY()) / 64);
                    var currentLocation = Game1.currentLocation as BuildableGameLocation;
                    for (var y = 0; y < buildingToMove.tilesHigh.Value; ++y)
                    for (var x = 0; x < buildingToMove.tilesWide.Value; ++x)
                    {
                        var structurePlacementTile = buildingToMove.getTileSheetIndexForStructurePlacementTile(x, y);
                        var vector2_2 = new Vector2(vector2_1.X + x, vector2_1.Y + y);
                        var flag = currentLocation.buildings.Contains(buildingToMove) &&
                                   buildingToMove.occupiesTile(vector2_2);
                        if (!currentLocation.isBuildable(vector2_2) && !flag)
                            ++structurePlacementTile;
                        b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.uiViewport, vector2_2 * 64f),
                            new Rectangle(194 + structurePlacementTile * 16, 388, 16, 16), Color.White, 0.0f,
                            Vector2.Zero, 4f, SpriteEffects.None, 0.999f);
                    }
                }
            }

            cancelButton.draw(b);
            drawMouse(b);
            if (hoverText.Length <= 0)
                return;
            drawHoverText(b, hoverText, Game1.dialogueFont);
        }


        /*********
        ** Private methods
        *********/
        private void ResetBounds()
        {
            xPositionOnScreen = Game1.uiViewport.Width / 2 - maxWidthOfBuildingViewer - spaceToClearSideBorder;
            yPositionOnScreen = Game1.uiViewport.Height / 2 - maxHeightOfBuildingViewer / 2 - spaceToClearTopBorder +
                                Game1.tileSize / 2;
            width = maxWidthOfBuildingViewer + maxWidthOfDescription + spaceToClearSideBorder * 2 + Game1.tileSize;
            height = maxHeightOfBuildingViewer + spaceToClearTopBorder;
            initialize(xPositionOnScreen, yPositionOnScreen, width, height, true);
            okButton = new ClickableTextureComponent("OK",
                new Rectangle(
                    xPositionOnScreen + width - borderWidth - spaceToClearSideBorder - Game1.tileSize * 3 -
                    Game1.pixelZoom * 3, yPositionOnScreen + maxHeightOfBuildingViewer + Game1.tileSize, Game1.tileSize,
                    Game1.tileSize), null, null, Game1.mouseCursors, new Rectangle(366, 373, 16, 16), Game1.pixelZoom);
            cancelButton = new ClickableTextureComponent("OK",
                new Rectangle(xPositionOnScreen + width - borderWidth - spaceToClearSideBorder - Game1.tileSize,
                    yPositionOnScreen + maxHeightOfBuildingViewer + Game1.tileSize, Game1.tileSize, Game1.tileSize),
                null, null, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 47), 1f);
            backButton = new ClickableTextureComponent(
                new Rectangle(xPositionOnScreen + Game1.tileSize,
                    yPositionOnScreen + maxHeightOfBuildingViewer + Game1.tileSize, 12 * Game1.pixelZoom,
                    11 * Game1.pixelZoom), Game1.mouseCursors, new Rectangle(352, 495, 12, 11), Game1.pixelZoom);
            forwardButton = new ClickableTextureComponent(
                new Rectangle(xPositionOnScreen + maxWidthOfBuildingViewer - Game1.tileSize * 4 + Game1.tileSize / 4,
                    yPositionOnScreen + maxHeightOfBuildingViewer + Game1.tileSize, 12 * Game1.pixelZoom,
                    11 * Game1.pixelZoom), Game1.mouseCursors, new Rectangle(365, 495, 12, 11), Game1.pixelZoom);
            demolishButton = new ClickableTextureComponent(Game1.content.LoadString("Strings\\UI:Carpenter_Demolish"),
                new Rectangle(
                    xPositionOnScreen + width - borderWidth - spaceToClearSideBorder - Game1.tileSize * 2 -
                    Game1.pixelZoom * 2,
                    yPositionOnScreen + maxHeightOfBuildingViewer + Game1.tileSize - Game1.pixelZoom, Game1.tileSize,
                    Game1.tileSize), null, null, Game1.mouseCursors, new Rectangle(348, 372, 17, 17), Game1.pixelZoom);
            upgradeIcon = new ClickableTextureComponent(
                new Rectangle(xPositionOnScreen + maxWidthOfBuildingViewer - Game1.tileSize * 2 + Game1.tileSize / 2,
                    yPositionOnScreen + Game1.pixelZoom * 2, 9 * Game1.pixelZoom, 13 * Game1.pixelZoom),
                Game1.mouseCursors, new Rectangle(402, 328, 9, 13), Game1.pixelZoom);
            moveButton = new ClickableTextureComponent(Game1.content.LoadString("Strings\\UI:Carpenter_MoveBuildings"),
                new Rectangle(
                    xPositionOnScreen + width - borderWidth - spaceToClearSideBorder - Game1.tileSize * 4 -
                    Game1.pixelZoom * 5, yPositionOnScreen + maxHeightOfBuildingViewer + Game1.tileSize, Game1.tileSize,
                    Game1.tileSize), null, null, Game1.mouseCursors, new Rectangle(257, 284, 16, 16), Game1.pixelZoom);
            GoBackButton = new ClickableTextureComponent(
                new Rectangle(xPositionOnScreen - 2 * Game1.tileSize, yPositionOnScreen - Game1.tileSize,
                    12 * Game1.pixelZoom, 11 * Game1.pixelZoom), Game1.mouseCursors, new Rectangle(352, 495, 12, 11),
                Game1.pixelZoom);
        }

        private void SetNewActiveBlueprint()
        {
            currentBuilding = !blueprints[currentBlueprintIndex].name.Contains("Coop")
                ? !blueprints[currentBlueprintIndex].name.Contains("Barn")
                    ?
                    !blueprints[currentBlueprintIndex].name.Contains("Mill")
                        ?
                        !blueprints[currentBlueprintIndex].name.Contains("Junimo Hut")
                            ?
                            !blueprints[currentBlueprintIndex].name.Contains("Shipping Bin")
                                ?
                                !blueprints[currentBlueprintIndex].name.Contains("Fish Pond")
                                    ?
                                    new Building(blueprints[currentBlueprintIndex], Vector2.Zero)
                                    : new FishPond(blueprints[currentBlueprintIndex], Vector2.Zero)
                                : new ShippingBin(blueprints[currentBlueprintIndex], Vector2.Zero)
                            : new JunimoHut(blueprints[currentBlueprintIndex], Vector2.Zero)
                        : new Mill(blueprints[currentBlueprintIndex], Vector2.Zero)
                    : new Barn(blueprints[currentBlueprintIndex], Vector2.Zero)
                : new Coop(blueprints[currentBlueprintIndex], Vector2.Zero);
            price = blueprints[currentBlueprintIndex].moneyRequired;
            ingredients.Clear();
            foreach (var keyValuePair in blueprints[currentBlueprintIndex].itemsRequired)
                ingredients.Add(new SObject(keyValuePair.Key, keyValuePair.Value));
            buildingDescription = blueprints[currentBlueprintIndex].description;
            buildingName = blueprints[currentBlueprintIndex].displayName;
        }

        private void GoBackButtonPressed()
        {
            if (readyToClose())
                exitThisMenu();
        }

        private bool TryToBuild()
        {
            return Game1.getFarm().buildStructure(currentBlueprint,
                new Vector2((Game1.uiViewport.X + Game1.getOldMouseX()) / Game1.tileSize,
                    (Game1.uiViewport.Y + Game1.getOldMouseY()) / Game1.tileSize), Game1.player, magicalConstruction);
        }

        private void ReturnToCarpentryMenu()
        {
            Game1.currentLocation.cleanupBeforePlayerExit();
            Game1.currentLocation = Game1.getLocationFromName(WhereToGo);
            //Game1.currentLocation = Game1.getLocationFromName(this.magicalConstruction ? "WizardHouse" : "ScienceHouse");
            Game1.currentLocation.resetForPlayerEntry();
            Game1.globalFadeToClear();
            onFarm = false;
            ResetBounds();
            upgrading = false;
            moving = false;
            freeze = false;
            Game1.displayHUD = true;
            Game1.viewportFreeze = false;
            Game1.uiViewport.Location = new Location(5 * Game1.tileSize, 24 * Game1.tileSize);
            drawBG = true;
            demolishing = false;
            Game1.displayFarmer = true;
        }

        private void ReturnToCarpentryMenuAfterSuccessfulBuild()
        {
            Game1.currentLocation.cleanupBeforePlayerExit();
            Game1.currentLocation = Game1.getLocationFromName(WhereToGo);
            //Game1.currentLocation = Game1.getLocationFromName(this.magicalConstruction ? "WizardHouse" : "ScienceHouse");
            Game1.currentLocation.resetForPlayerEntry();
            Game1.globalFadeToClear(RobinConstructionMessage);
            Game1.displayHUD = true;
            Game1.viewportFreeze = false;
            Game1.uiViewport.Location = new Location(5 * Game1.tileSize, 24 * Game1.tileSize);
            freeze = true;
            Game1.displayFarmer = true;
        }

        private void RobinConstructionMessage()
        {
            exitThisMenu();
            Game1.player.forceCanMove();

            Game1.activeClickableMenu = new ConstructionMenu(magicalConstruction, OnMenuOpened);
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
            hoverText = "";
            Game1.currentLocation = Game1.getFarm();
            Game1.currentLocation.resetForPlayerEntry();
            Game1.globalFadeToClear();
            onFarm = true;
            cancelButton.bounds.X = Game1.uiViewport.Width - Game1.tileSize * 2;
            cancelButton.bounds.Y = Game1.uiViewport.Height - Game1.tileSize * 2;
            Game1.displayHUD = false;
            Game1.viewportFreeze = true;
            Game1.uiViewport.Location = new Location(49 * Game1.tileSize, 5 * Game1.tileSize);
            Game1.panScreen(0, 0);
            drawBG = false;
            freeze = false;
            Game1.displayFarmer = false;
            if (demolishing || currentBlueprint.nameOfBuildingToUpgrade == null ||
                currentBlueprint.nameOfBuildingToUpgrade.Length <= 0 || moving)
                return;
            upgrading = true;
        }
    }
}