using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Buildings;
using StardewValley.Menus;
using xTile.Dimensions;
using Object = StardewValley.Object;

namespace PelicanFiber.Framework
{
    internal class ConstructionMenu : IClickableMenu
    {
        /*********
        ** Properties
        *********/
        private readonly string WhereToGo;
        private readonly int MaxWidthOfBuildingViewer = 7 * Game1.tileSize;
        private readonly int MaxHeightOfBuildingViewer = 8 * Game1.tileSize;
        private readonly int MaxWidthOfDescription = 6 * Game1.tileSize;
        private readonly List<Item> Ingredients = new List<Item>();
        private bool DrawBG = true;
        private string HoverText = "";
        private readonly List<BluePrint> Blueprints;
        private int CurrentBlueprintIndex;
        private ClickableTextureComponent OkButton;
        private ClickableTextureComponent CancelButton;
        private ClickableTextureComponent BackButton;
        private ClickableTextureComponent ForwardButton;
        private ClickableTextureComponent UpgradeIcon;
        private ClickableTextureComponent DemolishButton;
        private ClickableTextureComponent MoveButton;
        private ClickableTextureComponent GoBackButton;
        private Building CurrentBuilding;
        private Building BuildingToMove;
        private string BuildingDescription;
        private string BuildingName;
        private int Price;
        private bool OnFarm;
        private bool Freeze;
        private bool Upgrading;
        private bool Demolishing;
        private bool Moving;
        private readonly bool MagicalConstruction;
        private readonly Action ShowMainMenu;

        private BluePrint CurrentBlueprint => this.Blueprints[this.CurrentBlueprintIndex];


        /*********
        ** Public methods
        *********/
        public ConstructionMenu(bool magicalConstruction, Action showMainMenu)
        {
            this.MagicalConstruction = magicalConstruction;
            this.ShowMainMenu = showMainMenu;

            this.WhereToGo = Game1.player.currentLocation.Name;

            Game1.player.forceCanMove();
            this.ResetBounds();
            this.Blueprints = new List<BluePrint>();
            if (magicalConstruction)
            {
                this.Blueprints.Add(new BluePrint("Junimo Hut"));
                this.Blueprints.Add(new BluePrint("Earth Obelisk"));
                this.Blueprints.Add(new BluePrint("Water Obelisk"));
                this.Blueprints.Add(new BluePrint("Gold Clock"));
            }
            else
            {
                this.Blueprints.Add(new BluePrint("Coop"));
                this.Blueprints.Add(new BluePrint("Barn"));
                this.Blueprints.Add(new BluePrint("Well"));
                this.Blueprints.Add(new BluePrint("Silo"));
                this.Blueprints.Add(new BluePrint("Mill"));
                this.Blueprints.Add(new BluePrint("Shed"));
                if (!Game1.getFarm().isBuildingConstructed("Stable"))
                    this.Blueprints.Add(new BluePrint("Stable"));
                this.Blueprints.Add(new BluePrint("Slime Hutch"));
                if (Game1.getFarm().isBuildingConstructed("Coop"))
                    this.Blueprints.Add(new BluePrint("Big Coop"));
                if (Game1.getFarm().isBuildingConstructed("Big Coop"))
                    this.Blueprints.Add(new BluePrint("Deluxe Coop"));
                if (Game1.getFarm().isBuildingConstructed("Barn"))
                    this.Blueprints.Add(new BluePrint("Big Barn"));
                if (Game1.getFarm().isBuildingConstructed("Big Barn"))
                    this.Blueprints.Add(new BluePrint("Deluxe Barn"));
            }
            this.SetNewActiveBlueprint();
        }

        public override void performHoverAction(int x, int y)
        {
            this.CancelButton.tryHover(x, y);
            this.GoBackButton.tryHover(x, y, 1f);

            base.performHoverAction(x, y);
            if (!this.OnFarm)
            {
                this.BackButton.tryHover(x, y, 1f);
                this.ForwardButton.tryHover(x, y, 1f);
                this.OkButton.tryHover(x, y);
                this.DemolishButton.tryHover(x, y);
                this.MoveButton.tryHover(x, y);
                if (this.CurrentBlueprint.isUpgrade() && this.UpgradeIcon.containsPoint(x, y))
                    this.HoverText = Game1.content.LoadString("Strings\\UI:Carpenter_Upgrade", this.CurrentBlueprint.nameOfBuildingToUpgrade);
                else if (this.DemolishButton.containsPoint(x, y))
                    this.HoverText = Game1.content.LoadString("Strings\\UI:Carpenter_Demolish");
                else if (this.MoveButton.containsPoint(x, y))
                    this.HoverText = Game1.content.LoadString("Strings\\UI:Carpenter_MoveBuildings");
                else if (this.OkButton.containsPoint(x, y) && this.CurrentBlueprint.doesFarmerHaveEnoughResourcesToBuild())
                    this.HoverText = Game1.content.LoadString("Strings\\UI:Carpenter_Build");
                else
                    this.HoverText = "";
            }
            else
            {
                Farm farm = Game1.getFarm();

                if (!this.Upgrading && !this.Demolishing && !this.Moving || this.Freeze)
                    return;
                foreach (Building building in farm.buildings)
                    building.color.Value = Color.White;
                Building building1 = farm.getBuildingAt(new Vector2((Game1.viewport.X + Game1.getOldMouseX()) / Game1.tileSize, (Game1.viewport.Y + Game1.getOldMouseY()) / Game1.tileSize)) ?? farm.getBuildingAt(new Vector2((Game1.viewport.X + Game1.getOldMouseX()) / Game1.tileSize, (Game1.viewport.Y + Game1.getOldMouseY() + Game1.tileSize * 2) / Game1.tileSize)) ?? farm.getBuildingAt(new Vector2((Game1.viewport.X + Game1.getOldMouseX()) / Game1.tileSize, (Game1.viewport.Y + Game1.getOldMouseY() + Game1.tileSize * 3) / Game1.tileSize));
                if (this.Upgrading)
                {
                    if (building1 != null && this.CurrentBlueprint.nameOfBuildingToUpgrade != null && this.CurrentBlueprint.nameOfBuildingToUpgrade.Equals(building1.buildingType.Value))
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
                else if (this.Demolishing)
                {
                    if (building1 == null)
                        return;
                    building1.color.Value = Color.Red * 0.8f;
                }
                else
                {
                    if (!this.Moving || building1 == null)
                        return;
                    building1.color.Value = Color.Lime * 0.8f;
                }
            }
        }

        public override bool readyToClose()
        {
            if (base.readyToClose())
                return this.BuildingToMove == null;
            return false;
        }

        public override void receiveKeyPress(Keys key)
        {
            if (this.Freeze)
                return;
            if (!this.OnFarm)
                base.receiveKeyPress(key);
            if (Game1.globalFade || !this.OnFarm)
                return;
            if (Game1.options.doesInputListContain(Game1.options.menuButton, key) && this.readyToClose())
                Game1.globalFadeToBlack(this.ReturnToCarpentryMenu);
            else if (Game1.options.doesInputListContain(Game1.options.moveDownButton, key))
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

        public override void update(GameTime time)
        {
            base.update(time);
            if (!this.OnFarm || Game1.globalFade)
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
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (this.Freeze)
                return;
            Farm farm = Game1.getFarm();


            if (this.GoBackButton != null && this.GoBackButton.containsPoint(x, y))
            {
                this.GoBackButton.scale = this.GoBackButton.baseScale;
                this.GoBackButtonPressed();
            }

            if (!this.OnFarm)
                base.receiveLeftClick(x, y, playSound);
            if (this.CancelButton.containsPoint(x, y))
            {
                if (!this.OnFarm)
                {
                    this.exitThisMenu();
                    Game1.player.forceCanMove();
                    Game1.playSound("bigDeSelect");
                }
                else
                {
                    if (this.Moving && this.BuildingToMove != null)
                    {
                        Game1.playSound("cancel");
                        return;
                    }
                    Game1.globalFadeToBlack(this.ReturnToCarpentryMenu);
                    Game1.playSound("smallSelect");
                    return;
                }
            }
            if (!this.OnFarm && this.BackButton.containsPoint(x, y))
            {
                this.CurrentBlueprintIndex = this.CurrentBlueprintIndex - 1;
                if (this.CurrentBlueprintIndex < 0)
                    this.CurrentBlueprintIndex = this.Blueprints.Count - 1;
                this.SetNewActiveBlueprint();
                Game1.playSound("shwip");
                this.BackButton.scale = this.BackButton.baseScale;
            }
            if (!this.OnFarm && this.ForwardButton.containsPoint(x, y))
            {
                this.CurrentBlueprintIndex = (this.CurrentBlueprintIndex + 1) % this.Blueprints.Count;
                this.SetNewActiveBlueprint();
                this.BackButton.scale = this.BackButton.baseScale;
                Game1.playSound("shwip");
            }
            if (!this.OnFarm && this.DemolishButton.containsPoint(x, y))
            {
                Game1.globalFadeToBlack(this.SetUpForBuildingPlacement);
                Game1.playSound("smallSelect");
                this.OnFarm = true;
                this.Demolishing = true;
            }
            if (!this.OnFarm && this.MoveButton.containsPoint(x, y))
            {
                Game1.globalFadeToBlack(this.SetUpForBuildingPlacement);
                Game1.playSound("smallSelect");
                this.OnFarm = true;
                this.Moving = true;
            }
            if (this.OkButton.containsPoint(x, y) && !this.OnFarm && (Game1.player.money >= this.Price && this.Blueprints[this.CurrentBlueprintIndex].doesFarmerHaveEnoughResourcesToBuild()))
            {
                Game1.globalFadeToBlack(this.SetUpForBuildingPlacement);
                Game1.playSound("smallSelect");
                this.OnFarm = true;
            }
            if (!this.OnFarm || this.Freeze || Game1.globalFade)
                return;
            if (this.Demolishing)
            {
                Building buildingAt = farm.getBuildingAt(new Vector2((Game1.viewport.X + Game1.getOldMouseX()) / Game1.tileSize, (Game1.viewport.Y + Game1.getOldMouseY()) / Game1.tileSize));
                if (buildingAt != null && (buildingAt.daysOfConstructionLeft.Value > 0 || buildingAt.daysUntilUpgrade.Value > 0))
                    Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_DuringConstruction"), Color.Red, 3500f));
                else if (buildingAt?.indoors.Value is AnimalHouse animalHouse && animalHouse.animalsThatLiveHere.Count > 0)
                {
                    Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_AnimalsHere"), Color.Red, 3500f));
                }
                else
                {
                    if (buildingAt == null || !farm.destroyStructure(buildingAt))
                        return;
                    Game1.flashAlpha = 1f;
                    buildingAt.showDestroyedAnimation(farm);
                    Game1.playSound("explosion");
                    Utility.spreadAnimalsAround(buildingAt, farm);
                    DelayedAction.fadeAfterDelay(this.ReturnToCarpentryMenu, 1500);
                    this.Freeze = true;
                }
            }
            else if (this.Upgrading)
            {
                Building buildingAt = farm.getBuildingAt(new Vector2((Game1.viewport.X + Game1.getOldMouseX()) / Game1.tileSize, (Game1.viewport.Y + Game1.getOldMouseY()) / Game1.tileSize));
                if (buildingAt != null && this.CurrentBlueprint.name != null && buildingAt.buildingType.Value.Equals(this.CurrentBlueprint.nameOfBuildingToUpgrade))
                {
                    this.CurrentBlueprint.consumeResources();
                    buildingAt.daysUntilUpgrade.Value = 2;
                    buildingAt.showUpgradeAnimation(farm);
                    Game1.playSound("axe");
                    DelayedAction.fadeAfterDelay(this.ReturnToCarpentryMenuAfterSuccessfulBuild, 1500);
                    this.Freeze = true;
                }
                else
                {
                    if (buildingAt == null)
                        return;
                    Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantUpgrade_BuildingType"), Color.Red, 3500f));
                }
            }
            else if (this.Moving)
            {
                if (this.BuildingToMove == null)
                {
                    this.BuildingToMove = farm.getBuildingAt(new Vector2((Game1.viewport.X + Game1.getMouseX()) / Game1.tileSize, (Game1.viewport.Y + Game1.getMouseY()) / Game1.tileSize));
                    if (this.BuildingToMove == null)
                        return;
                    farm.buildings.Remove(this.BuildingToMove);
                    Game1.playSound("axchop");
                }
                else if (farm.buildStructure(this.BuildingToMove, new Vector2((Game1.viewport.X + Game1.getMouseX()) / Game1.tileSize, (Game1.viewport.Y + Game1.getMouseY()) / Game1.tileSize), Game1.player))
                {
                    this.BuildingToMove = null;
                    Game1.playSound("axchop");
                    DelayedAction.playSoundAfterDelay("dirtyHit", 50);
                    DelayedAction.playSoundAfterDelay("dirtyHit", 150);
                }
                else
                    Game1.playSound("cancel");
            }
            else if (this.TryToBuild())
            {
                this.CurrentBlueprint.consumeResources();
                DelayedAction.fadeAfterDelay(this.ReturnToCarpentryMenuAfterSuccessfulBuild, 2000);
                this.Freeze = true;
            }
            else
                Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantBuild"), Color.Red, 3500f));
        }

        public override void gameWindowSizeChanged(Microsoft.Xna.Framework.Rectangle oldBounds, Microsoft.Xna.Framework.Rectangle newBounds)
        {
            this.ResetBounds();
        }

        public override void draw(SpriteBatch b)
        {
            if (this.DrawBG)
                b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.5f);
            if (Game1.globalFade || this.Freeze)
                return;
            if (!this.OnFarm)
            {
                base.draw(b);
                IClickableMenu.drawTextureBox(b, this.xPositionOnScreen - Game1.tileSize * 3 / 2, this.yPositionOnScreen - Game1.tileSize / 4, this.MaxWidthOfBuildingViewer + Game1.tileSize, this.MaxHeightOfBuildingViewer + Game1.tileSize, this.MagicalConstruction ? Color.RoyalBlue : Color.White);
                this.CurrentBuilding.drawInMenu(b, this.xPositionOnScreen + this.MaxWidthOfBuildingViewer / 2 - this.CurrentBuilding.tilesWide.Value * Game1.tileSize / 2 - Game1.tileSize, this.yPositionOnScreen + this.MaxHeightOfBuildingViewer / 2 - this.CurrentBuilding.getSourceRectForMenu().Height * Game1.pixelZoom / 2);
                if (this.CurrentBlueprint.isUpgrade())
                    this.UpgradeIcon.draw(b);
                SpriteText.drawStringWithScrollBackground(b, this.BuildingName, this.xPositionOnScreen + this.MaxWidthOfBuildingViewer - IClickableMenu.spaceToClearSideBorder - Game1.tileSize / 4 + Game1.tileSize + ((this.width - (this.MaxWidthOfBuildingViewer + Game1.tileSize * 2)) / 2 - SpriteText.getWidthOfString("Deluxe Barn") / 2), this.yPositionOnScreen, "Deluxe Barn");
                IClickableMenu.drawTextureBox(b, this.xPositionOnScreen + this.MaxWidthOfBuildingViewer - Game1.tileSize / 4, this.yPositionOnScreen + Game1.tileSize * 5 / 4, this.MaxWidthOfDescription + Game1.tileSize, this.MaxWidthOfDescription + Game1.tileSize * 3 / 2, this.MagicalConstruction ? Color.RoyalBlue : Color.White);
                if (this.MagicalConstruction)
                {
                    Utility.drawTextWithShadow(b, Game1.parseText(this.BuildingDescription, Game1.dialogueFont, this.MaxWidthOfDescription + Game1.tileSize / 2), Game1.dialogueFont, new Vector2(this.xPositionOnScreen + this.MaxWidthOfDescription + Game1.tileSize - Game1.pixelZoom, this.yPositionOnScreen + Game1.tileSize * 5 / 4 + Game1.pixelZoom * 4 + Game1.pixelZoom), Game1.textColor * 0.25f, 1f, -1f, -1, -1, 0.0f);
                    Utility.drawTextWithShadow(b, Game1.parseText(this.BuildingDescription, Game1.dialogueFont, this.MaxWidthOfDescription + Game1.tileSize / 2), Game1.dialogueFont, new Vector2(this.xPositionOnScreen + this.MaxWidthOfDescription + Game1.tileSize - 1, this.yPositionOnScreen + Game1.tileSize * 5 / 4 + Game1.pixelZoom * 4 + Game1.pixelZoom), Game1.textColor * 0.25f, 1f, -1f, -1, -1, 0.0f);
                }
                Utility.drawTextWithShadow(b, Game1.parseText(this.BuildingDescription, Game1.dialogueFont, this.MaxWidthOfDescription + Game1.tileSize / 2), Game1.dialogueFont, new Vector2(this.xPositionOnScreen + this.MaxWidthOfDescription + Game1.tileSize, this.yPositionOnScreen + Game1.tileSize * 5 / 4 + Game1.pixelZoom * 4), this.MagicalConstruction ? Color.PaleGoldenrod : Game1.textColor, 1f, -1f, -1, -1, this.MagicalConstruction ? 0.0f : 0.25f);
                Vector2 location = new Vector2(this.xPositionOnScreen + this.MaxWidthOfDescription + Game1.tileSize / 4 + Game1.tileSize, this.yPositionOnScreen + Game1.tileSize * 4 + Game1.tileSize / 2);
                SpriteText.drawString(b, "$", (int)location.X, (int)location.Y);
                if (this.MagicalConstruction)
                {
                    Utility.drawTextWithShadow(b, this.Price.ToString() + "g", Game1.dialogueFont, new Vector2(location.X + Game1.tileSize, location.Y + Game1.pixelZoom * 2), Game1.textColor * 0.5f, 1f, -1f, -1, -1, this.MagicalConstruction ? 0.0f : 0.25f);
                    Utility.drawTextWithShadow(b, this.Price.ToString() + "g", Game1.dialogueFont, new Vector2((float)(location.X + (double)Game1.tileSize + Game1.pixelZoom - 1.0), location.Y + Game1.pixelZoom * 2), Game1.textColor * 0.25f, 1f, -1f, -1, -1, this.MagicalConstruction ? 0.0f : 0.25f);
                }
                Utility.drawTextWithShadow(b, this.Price.ToString() + "g", Game1.dialogueFont, new Vector2(location.X + Game1.tileSize + Game1.pixelZoom, location.Y + Game1.pixelZoom), Game1.player.money >= this.Price ? (this.MagicalConstruction ? Color.PaleGoldenrod : Game1.textColor) : Color.Red, 1f, -1f, -1, -1, this.MagicalConstruction ? 0.0f : 0.25f);
                location.X -= Game1.tileSize / 4;
                location.Y -= Game1.tileSize / 3;
                foreach (Item ingredient in this.Ingredients)
                {
                    location.Y += Game1.tileSize + Game1.pixelZoom;
                    ingredient.drawInMenu(b, location, 1f);
                    bool flag = !(ingredient is Object) || Game1.player.hasItemInInventory((ingredient as Object).ParentSheetIndex, ingredient.Stack);
                    if (this.MagicalConstruction)
                    {
                        Utility.drawTextWithShadow(b, ingredient.Name, Game1.dialogueFont, new Vector2(location.X + Game1.tileSize + Game1.pixelZoom * 3, location.Y + Game1.pixelZoom * 6), Game1.textColor * 0.25f, 1f, -1f, -1, -1, this.MagicalConstruction ? 0.0f : 0.25f);
                        Utility.drawTextWithShadow(b, ingredient.Name, Game1.dialogueFont, new Vector2((float)(location.X + (double)Game1.tileSize + Game1.pixelZoom * 4 - 1.0), location.Y + Game1.pixelZoom * 6), Game1.textColor * 0.25f, 1f, -1f, -1, -1, this.MagicalConstruction ? 0.0f : 0.25f);
                    }
                    Utility.drawTextWithShadow(b, ingredient.Name, Game1.dialogueFont, new Vector2(location.X + Game1.tileSize + Game1.pixelZoom * 4, location.Y + Game1.pixelZoom * 5), flag ? (this.MagicalConstruction ? Color.PaleGoldenrod : Game1.textColor) : Color.Red, 1f, -1f, -1, -1, this.MagicalConstruction ? 0.0f : 0.25f);
                }
                this.BackButton.draw(b);
                this.ForwardButton.draw(b);
                this.OkButton.draw(b, this.Blueprints[this.CurrentBlueprintIndex].doesFarmerHaveEnoughResourcesToBuild() ? Color.White : Color.Gray * 0.8f, 0.88f);
                this.DemolishButton.draw(b);
                this.MoveButton.draw(b);

                this.GoBackButton.draw(b);
            }
            else
            {
                string str;
                if (!this.Upgrading)
                    str = this.Demolishing ? Game1.content.LoadString("Strings\\UI:Carpenter_SelectBuilding_Demolish") : Game1.content.LoadString("Strings\\UI:Carpenter_ChooseLocation");
                else
                    str = Game1.content.LoadString("Strings\\UI:Carpenter_SelectBuilding_Upgrade", this.CurrentBlueprint.nameOfBuildingToUpgrade);
                string s = str;
                SpriteText.drawStringWithScrollBackground(b, s, Game1.viewport.Width / 2 - SpriteText.getWidthOfString(s) / 2, Game1.tileSize / 4);
                if (!this.Upgrading && !this.Demolishing && !this.Moving)
                {
                    Vector2 vector2_1 = new Vector2((Game1.viewport.X + Game1.getOldMouseX()) / Game1.tileSize, (Game1.viewport.Y + Game1.getOldMouseY()) / Game1.tileSize);
                    for (int y = 0; y < this.CurrentBlueprint.tilesHeight; ++y)
                    {
                        for (int x = 0; x < this.CurrentBlueprint.tilesWidth; ++x)
                        {
                            int structurePlacementTile = this.CurrentBlueprint.getTileSheetIndexForStructurePlacementTile(x, y);
                            Vector2 vector2_2 = new Vector2(vector2_1.X + x, vector2_1.Y + y);
                            if (Game1.player.getTileLocation().Equals(vector2_2) || Game1.currentLocation.isTileOccupied(vector2_2) || (!Game1.currentLocation.isTilePassable(new Location((int)vector2_2.X, (int)vector2_2.Y), Game1.viewport) || Game1.currentLocation.doesTileHaveProperty((int)vector2_2.X, (int)vector2_2.Y, "Diggable", "Back") == null))
                                ++structurePlacementTile;
                            b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, vector2_2 * Game1.tileSize), new Microsoft.Xna.Framework.Rectangle(194 + structurePlacementTile * 16, 388, 16, 16), Color.White, 0.0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 0.999f);
                        }
                    }
                }
                else if (this.Moving && this.BuildingToMove != null)
                {
                    Vector2 vector2_1 = new Vector2((Game1.viewport.X + Game1.getOldMouseX()) / Game1.tileSize, (Game1.viewport.Y + Game1.getOldMouseY()) / Game1.tileSize);
                    for (int y = 0; y < this.BuildingToMove.tilesHigh.Value; ++y)
                    {
                        for (int x = 0; x < this.BuildingToMove.tilesWide.Value; ++x)
                        {
                            int structurePlacementTile = this.BuildingToMove.getTileSheetIndexForStructurePlacementTile(x, y);
                            Vector2 vector2_2 = new Vector2(vector2_1.X + x, vector2_1.Y + y);
                            if (Game1.player.getTileLocation().Equals(vector2_2) || Game1.currentLocation.isTileOccupied(vector2_2) || (!Game1.currentLocation.isTilePassable(new Location((int)vector2_2.X, (int)vector2_2.Y), Game1.viewport) || Game1.currentLocation.doesTileHaveProperty((int)vector2_2.X, (int)vector2_2.Y, "Diggable", "Back") == null))
                                ++structurePlacementTile;
                            b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, vector2_2 * Game1.tileSize), new Microsoft.Xna.Framework.Rectangle(194 + structurePlacementTile * 16, 388, 16, 16), Color.White, 0.0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 0.999f);
                        }
                    }
                }
            }
            this.CancelButton.draw(b);
            this.drawMouse(b);
            if (this.HoverText.Length <= 0)
                return;
            IClickableMenu.drawHoverText(b, this.HoverText, Game1.dialogueFont);
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
        }

        /*********
        ** Private methods
        *********/
        private void ResetBounds()
        {
            this.xPositionOnScreen = Game1.viewport.Width / 2 - this.MaxWidthOfBuildingViewer - IClickableMenu.spaceToClearSideBorder;
            this.yPositionOnScreen = Game1.viewport.Height / 2 - this.MaxHeightOfBuildingViewer / 2 - IClickableMenu.spaceToClearTopBorder + Game1.tileSize / 2;
            this.width = this.MaxWidthOfBuildingViewer + this.MaxWidthOfDescription + IClickableMenu.spaceToClearSideBorder * 2 + Game1.tileSize;
            this.height = this.MaxHeightOfBuildingViewer + IClickableMenu.spaceToClearTopBorder;
            this.initialize(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, true);
            this.OkButton = new ClickableTextureComponent("OK", new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen + this.width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - Game1.tileSize * 3 - Game1.pixelZoom * 3, this.yPositionOnScreen + this.MaxHeightOfBuildingViewer + Game1.tileSize, Game1.tileSize, Game1.tileSize), null, null, Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(366, 373, 16, 16), Game1.pixelZoom);
            this.CancelButton = new ClickableTextureComponent("OK", new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen + this.width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - Game1.tileSize, this.yPositionOnScreen + this.MaxHeightOfBuildingViewer + Game1.tileSize, Game1.tileSize, Game1.tileSize), null, null, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 47), 1f);
            this.BackButton = new ClickableTextureComponent(new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen + Game1.tileSize, this.yPositionOnScreen + this.MaxHeightOfBuildingViewer + Game1.tileSize, 12 * Game1.pixelZoom, 11 * Game1.pixelZoom), Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(352, 495, 12, 11), Game1.pixelZoom);
            this.ForwardButton = new ClickableTextureComponent(new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen + this.MaxWidthOfBuildingViewer - Game1.tileSize * 4 + Game1.tileSize / 4, this.yPositionOnScreen + this.MaxHeightOfBuildingViewer + Game1.tileSize, 12 * Game1.pixelZoom, 11 * Game1.pixelZoom), Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(365, 495, 12, 11), Game1.pixelZoom);
            this.DemolishButton = new ClickableTextureComponent(Game1.content.LoadString("Strings\\UI:Carpenter_Demolish"), new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen + this.width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - Game1.tileSize * 2 - Game1.pixelZoom * 2, this.yPositionOnScreen + this.MaxHeightOfBuildingViewer + Game1.tileSize - Game1.pixelZoom, Game1.tileSize, Game1.tileSize), null, null, Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(348, 372, 17, 17), Game1.pixelZoom);
            this.UpgradeIcon = new ClickableTextureComponent(new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen + this.MaxWidthOfBuildingViewer - Game1.tileSize * 2 + Game1.tileSize / 2, this.yPositionOnScreen + Game1.pixelZoom * 2, 9 * Game1.pixelZoom, 13 * Game1.pixelZoom), Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(402, 328, 9, 13), Game1.pixelZoom);
            this.MoveButton = new ClickableTextureComponent(Game1.content.LoadString("Strings\\UI:Carpenter_MoveBuildings"), new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen + this.width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - Game1.tileSize * 4 - Game1.pixelZoom * 5, this.yPositionOnScreen + this.MaxHeightOfBuildingViewer + Game1.tileSize, Game1.tileSize, Game1.tileSize), null, null, Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(257, 284, 16, 16), Game1.pixelZoom);
            this.GoBackButton = new ClickableTextureComponent(new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen - 2 * Game1.tileSize, this.yPositionOnScreen - Game1.tileSize, 12 * Game1.pixelZoom, 11 * Game1.pixelZoom), Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(352, 495, 12, 11), Game1.pixelZoom);
        }

        private void SetNewActiveBlueprint()
        {
            this.CurrentBuilding = !this.Blueprints[this.CurrentBlueprintIndex].name.Contains("Coop") ? (!this.Blueprints[this.CurrentBlueprintIndex].name.Contains("Barn") ? (!this.Blueprints[this.CurrentBlueprintIndex].name.Contains("Mill") ? (!this.Blueprints[this.CurrentBlueprintIndex].name.Contains("Junimo Hut") ? new Building(this.Blueprints[this.CurrentBlueprintIndex], Vector2.Zero) : new JunimoHut(this.Blueprints[this.CurrentBlueprintIndex], Vector2.Zero)) : new Mill(this.Blueprints[this.CurrentBlueprintIndex], Vector2.Zero)) : new Barn(this.Blueprints[this.CurrentBlueprintIndex], Vector2.Zero)) : new Coop(this.Blueprints[this.CurrentBlueprintIndex], Vector2.Zero);
            this.Price = this.Blueprints[this.CurrentBlueprintIndex].moneyRequired;
            this.Ingredients.Clear();
            foreach (KeyValuePair<int, int> keyValuePair in this.Blueprints[this.CurrentBlueprintIndex].itemsRequired)
                this.Ingredients.Add(new Object(keyValuePair.Key, keyValuePair.Value));
            this.BuildingDescription = this.Blueprints[this.CurrentBlueprintIndex].description;
            this.BuildingName = this.Blueprints[this.CurrentBlueprintIndex].name;
        }

        private void GoBackButtonPressed()
        {
            if (this.readyToClose())
            {
                this.exitThisMenu();
                this.ShowMainMenu();
            }
        }

        private bool TryToBuild()
        {
            return Game1.getFarm().buildStructure(this.CurrentBlueprint, new Vector2((Game1.viewport.X + Game1.getOldMouseX()) / Game1.tileSize, (Game1.viewport.Y + Game1.getOldMouseY()) / Game1.tileSize), Game1.player, this.MagicalConstruction);
        }

        private void ReturnToCarpentryMenu()
        {
            Game1.currentLocation.cleanupBeforePlayerExit();
            Game1.currentLocation = Game1.getLocationFromName(this.WhereToGo);
            //Game1.currentLocation = Game1.getLocationFromName(this.magicalConstruction ? "WizardHouse" : "ScienceHouse");
            Game1.currentLocation.resetForPlayerEntry();
            Game1.globalFadeToClear();
            this.OnFarm = false;
            this.ResetBounds();
            this.Upgrading = false;
            this.Moving = false;
            this.Freeze = false;
            Game1.displayHUD = true;
            Game1.viewportFreeze = false;
            Game1.viewport.Location = new Location(5 * Game1.tileSize, 24 * Game1.tileSize);
            this.DrawBG = true;
            this.Demolishing = false;
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
            this.Freeze = true;
            Game1.displayFarmer = true;
        }

        private void RobinConstructionMessage()
        {
            this.exitThisMenu();
            Game1.player.forceCanMove();

            Game1.activeClickableMenu = new ConstructionMenu(this.MagicalConstruction, this.ShowMainMenu);
            //if (this.magicalConstruction)
            //    return;
            //string path = "Data\\ExtraDialogue:Robin_" + (this.upgrading ? "Upgrade" : "New") + "Construction";
            //if (Utility.isFestivalDay(Game1.dayOfMonth + 1, Game1.currentSeason))
            //    path += "_Festival";
            //Game1.drawDialogue(Game1.getCharacterFromName("Robin"), Game1.content.LoadString(path, (object)this.CurrentBlueprint.name.ToLower(), (object)((IEnumerable<string>)this.CurrentBlueprint.name.ToLower().Split(' ')).Last<string>()));
        }

        private void SetUpForBuildingPlacement()
        {
            Game1.currentLocation.cleanupBeforePlayerExit();
            this.HoverText = "";
            Game1.currentLocation = Game1.getFarm();
            Game1.currentLocation.resetForPlayerEntry();
            Game1.globalFadeToClear();
            this.OnFarm = true;
            this.CancelButton.bounds.X = Game1.viewport.Width - Game1.tileSize * 2;
            this.CancelButton.bounds.Y = Game1.viewport.Height - Game1.tileSize * 2;
            Game1.displayHUD = false;
            Game1.viewportFreeze = true;
            Game1.viewport.Location = new Location(49 * Game1.tileSize, 5 * Game1.tileSize);
            Game1.panScreen(0, 0);
            this.DrawBG = false;
            this.Freeze = false;
            Game1.displayFarmer = false;
            if (this.Demolishing || this.CurrentBlueprint.nameOfBuildingToUpgrade == null || (this.CurrentBlueprint.nameOfBuildingToUpgrade.Length <= 0 || this.Moving))
                return;
            this.Upgrading = true;
        }
    }
}
