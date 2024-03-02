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
#nullable disable

    //Do not show SMAPI build warnings for this file because it was copied from the actual Stardew Valley decompiled Carpenter Menu code and then modified.
#pragma warning disable AvoidImplicitNetFieldCast, AvoidNetField

    public class InstantBuildMenu : IClickableMenu
    {
        private const int MENU_DELAY = 200;
        private const int MULTIPLAYER_LIMIT = 4;  //Because Game1.multiplayer (and hence .playerLimit) is inaccessible due to protection level


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
                if (!value)
                    return;
                this.upgradeIcon.visible = false;
                this.demolishButton.visible = false;
                this.moveButton.visible = false;
                this.okButton.visible = false;
                this.paintButton.visible = false;
                this.cancelButton.leftNeighborID = 102;
            }
        }


        private ModConfig _config;

        public BluePrint CurrentBlueprint => this.blueprints[this.currentBlueprintIndex];

        //jon, 1/30/24: This function is updated from the original to make all items avaialble according to the mod config.
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
            //if (Game1.getFarm().isBuildingConstructed("Coop"))
            this.blueprints.Add(new BluePrint("Big Coop"));
            //if (Game1.getFarm().isBuildingConstructed("Big Coop"))
            this.blueprints.Add(new BluePrint("Deluxe Coop"));
            //if (Game1.getFarm().isBuildingConstructed("Barn"))
            this.blueprints.Add(new BluePrint("Big Barn"));
            //if (Game1.getFarm().isBuildingConstructed("Big Barn"))
            this.blueprints.Add(new BluePrint("Deluxe Barn"));
            this.blueprints.Add(new BluePrint("Big Shed"));
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
                this.blueprints.Add(new BluePrint("Desert Obelisk"));
                this.blueprints.Add(new BluePrint("Island Obelisk"));
                this.blueprints.Add(new BluePrint("Gold Clock"));
            }

            ModifyBlueprints();


            this.setNewActiveBlueprint();
            if (!Game1.options.SnappyMenus)
                return;
            this.populateClickableComponentList();
            this.snapToDefaultClickableComponent();
        }

        //jon, 1/30/24: This function is new to update all blueprints to be completed instantly and to be free or not according to config.
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

        public override bool shouldClampGamePadCursor()
        {
            return this.onFarm;
        }

        public override void snapToDefaultClickableComponent()
        {
            this.currentlySnappedComponent = this.getComponentWithID(107);
            this.snapCursorToCurrentSnappedComponent();
        }

        private void resetBounds()
        {
            this.xPositionOnScreen = Game1.uiViewport.Width / 2 - this.maxWidthOfBuildingViewer - IClickableMenu.spaceToClearSideBorder;
            this.yPositionOnScreen = Game1.uiViewport.Height / 2 - this.maxHeightOfBuildingViewer / 2 - IClickableMenu.spaceToClearTopBorder + 32;
            this.width = this.maxWidthOfBuildingViewer + this.maxWidthOfDescription + IClickableMenu.spaceToClearSideBorder * 2 + 64;
            this.height = this.maxHeightOfBuildingViewer + IClickableMenu.spaceToClearTopBorder;
            this.initialize(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, true);
            ClickableTextureComponent textureComponent1 = new ClickableTextureComponent("OK", new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen + this.width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - 192 - 12, this.yPositionOnScreen + this.maxHeightOfBuildingViewer + 64, 64, 64), (string)null, (string)null, Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(366, 373, 16, 16), 4f);
            textureComponent1.myID = 106;
            textureComponent1.rightNeighborID = 104;
            textureComponent1.leftNeighborID = 105;
            this.okButton = textureComponent1;
            ClickableTextureComponent textureComponent2 = new ClickableTextureComponent("OK", new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen + this.width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - 64, this.yPositionOnScreen + this.maxHeightOfBuildingViewer + 64, 64, 64), (string)null, (string)null, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 47), 1f);
            textureComponent2.myID = 107;
            textureComponent2.leftNeighborID = 104;
            this.cancelButton = textureComponent2;
            ClickableTextureComponent textureComponent3 = new ClickableTextureComponent(new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen + 64, this.yPositionOnScreen + this.maxHeightOfBuildingViewer + 64, 48, 44), Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(352, 495, 12, 11), 4f);
            textureComponent3.myID = 101;
            textureComponent3.rightNeighborID = 102;
            this.backButton = textureComponent3;
            ClickableTextureComponent textureComponent4 = new ClickableTextureComponent(new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen + this.maxWidthOfBuildingViewer - 256 + 16, this.yPositionOnScreen + this.maxHeightOfBuildingViewer + 64, 48, 44), Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(365, 495, 12, 11), 4f);
            textureComponent4.myID = 102;
            textureComponent4.leftNeighborID = 101;
            textureComponent4.rightNeighborID = -99998;
            this.forwardButton = textureComponent4;
            ClickableTextureComponent textureComponent5 = new ClickableTextureComponent(Game1.content.LoadString("Strings\\UI:Carpenter_Demolish"), new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen + this.width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - 128 - 8, this.yPositionOnScreen + this.maxHeightOfBuildingViewer + 64 - 4, 64, 64), (string)null, (string)null, Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(348, 372, 17, 17), 4f);
            textureComponent5.myID = 104;
            textureComponent5.rightNeighborID = 107;
            textureComponent5.leftNeighborID = 106;
            this.demolishButton = textureComponent5;
            ClickableTextureComponent textureComponent6 = new ClickableTextureComponent(new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen + this.maxWidthOfBuildingViewer - 128 + 32, this.yPositionOnScreen + 8, 36, 52), Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(402, 328, 9, 13), 4f);
            textureComponent6.myID = 103;
            textureComponent6.rightNeighborID = 104;
            textureComponent6.leftNeighborID = 105;
            this.upgradeIcon = textureComponent6;
            ClickableTextureComponent textureComponent7 = new ClickableTextureComponent(Game1.content.LoadString("Strings\\UI:Carpenter_MoveBuildings"), new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen + this.width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - 256 - 20, this.yPositionOnScreen + this.maxHeightOfBuildingViewer + 64, 64, 64), (string)null, (string)null, Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(257, 284, 16, 16), 4f);
            textureComponent7.myID = 105;
            textureComponent7.rightNeighborID = 106;
            textureComponent7.leftNeighborID = -99998;
            this.moveButton = textureComponent7;
            ClickableTextureComponent textureComponent8 = new ClickableTextureComponent(Game1.content.LoadString("Strings\\UI:Carpenter_PaintBuildings"), new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen + this.width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - 320 - 20, this.yPositionOnScreen + this.maxHeightOfBuildingViewer + 64, 64, 64), (string)null, (string)null, Game1.mouseCursors2, new Microsoft.Xna.Framework.Rectangle(80, 208, 16, 16), 4f);
            textureComponent8.myID = 105;
            textureComponent8.rightNeighborID = -99998;
            textureComponent8.leftNeighborID = -99998;
            this.paintButton = textureComponent8;
            bool flag1 = false;
            bool flag2 = this.CanPaintHouse() && this.HasPermissionsToPaint((Building)null);
            foreach (Building building in Game1.getFarm().buildings)
            {
                if (building.hasCarpenterPermissions())
                    flag1 = true;
                if (building.CanBePainted() && this.HasPermissionsToPaint(building))
                    flag2 = true;
            }
            this.demolishButton.visible = Game1.IsMasterGame;
            this.moveButton.visible = Game1.IsMasterGame || Game1.player.team.farmhandsCanMoveBuildings.Value == FarmerTeam.RemoteBuildingPermissions.On || Game1.player.team.farmhandsCanMoveBuildings.Value == FarmerTeam.RemoteBuildingPermissions.OwnedBuildings & flag1;
            this.paintButton.visible = flag2;
            if (this.magicalConstruction)
                this.paintButton.visible = false;
            if (!this.demolishButton.visible)
            {
                this.upgradeIcon.rightNeighborID = this.demolishButton.rightNeighborID;
                this.okButton.rightNeighborID = this.demolishButton.rightNeighborID;
                this.cancelButton.leftNeighborID = this.demolishButton.leftNeighborID;
            }
            if (this.moveButton.visible)
                return;
            this.upgradeIcon.leftNeighborID = this.moveButton.leftNeighborID;
            this.forwardButton.rightNeighborID = -99998;
            this.okButton.leftNeighborID = this.moveButton.leftNeighborID;
        }

        public void setNewActiveBlueprint()
        {
            this.currentBuilding = !this.blueprints[this.currentBlueprintIndex].name.Contains("Coop") ? (!this.blueprints[this.currentBlueprintIndex].name.Contains("Barn") ? (!this.blueprints[this.currentBlueprintIndex].name.Contains("Mill") ? (!this.blueprints[this.currentBlueprintIndex].name.Contains("Junimo Hut") ? (!this.blueprints[this.currentBlueprintIndex].name.Contains("Shipping Bin") ? (!this.blueprints[this.currentBlueprintIndex].name.Contains("Fish Pond") ? (!this.blueprints[this.currentBlueprintIndex].name.Contains("Greenhouse") ? new Building(this.blueprints[this.currentBlueprintIndex], Vector2.Zero) : (Building)new GreenhouseBuilding(this.blueprints[this.currentBlueprintIndex], Vector2.Zero)) : (Building)new FishPond(this.blueprints[this.currentBlueprintIndex], Vector2.Zero)) : (Building)new ShippingBin(this.blueprints[this.currentBlueprintIndex], Vector2.Zero)) : (Building)new JunimoHut(this.blueprints[this.currentBlueprintIndex], Vector2.Zero)) : (Building)new Mill(this.blueprints[this.currentBlueprintIndex], Vector2.Zero)) : (Building)new Barn(this.blueprints[this.currentBlueprintIndex], Vector2.Zero)) : (Building)new Coop(this.blueprints[this.currentBlueprintIndex], Vector2.Zero);
            this.price = this.blueprints[this.currentBlueprintIndex].moneyRequired;
            this.ingredients.Clear();
            foreach (KeyValuePair<int, int> keyValuePair in this.blueprints[this.currentBlueprintIndex].itemsRequired)
                this.ingredients.Add((Item)new StardewValley.Object(keyValuePair.Key, keyValuePair.Value));
            this.buildingDescription = this.blueprints[this.currentBlueprintIndex].description;
            this.buildingName = this.blueprints[this.currentBlueprintIndex].displayName;
        }

        public override void performHoverAction(int x, int y)
        {
            this.cancelButton.tryHover(x, y);
            base.performHoverAction(x, y);
            if (!this.onFarm)
            {
                this.backButton.tryHover(x, y, 1f);
                this.forwardButton.tryHover(x, y, 1f);
                this.okButton.tryHover(x, y);
                this.demolishButton.tryHover(x, y);
                this.moveButton.tryHover(x, y);
                this.paintButton.tryHover(x, y);
                if (this.CurrentBlueprint.isUpgrade() && this.upgradeIcon.containsPoint(x, y))
                    this.hoverText = Game1.content.LoadString("Strings\\UI:Carpenter_Upgrade", (object)new BluePrint(this.CurrentBlueprint.nameOfBuildingToUpgrade).displayName);
                else if (this.demolishButton.containsPoint(x, y) && this.CanDemolishThis(this.CurrentBlueprint))
                    this.hoverText = Game1.content.LoadString("Strings\\UI:Carpenter_Demolish");
                else if (this.moveButton.containsPoint(x, y))
                    this.hoverText = Game1.content.LoadString("Strings\\UI:Carpenter_MoveBuildings");
                else if (this.okButton.containsPoint(x, y) && this.CurrentBlueprint.doesFarmerHaveEnoughResourcesToBuild())
                    this.hoverText = Game1.content.LoadString("Strings\\UI:Carpenter_Build");
                else if (this.paintButton.containsPoint(x, y))
                    this.hoverText = this.paintButton.name;
                else
                    this.hoverText = "";
            }
            else
            {
                if (!this.upgrading && !this.demolishing && !this.moving && !this.painting || this.freeze)
                    return;
                Farm farm = Game1.getFarm();
                Vector2 vector2 = new Vector2((float)((Game1.viewport.X + Game1.getOldMouseX(false)) / 64), (float)((Game1.viewport.Y + Game1.getOldMouseY(false)) / 64));
                if (this.painting && farm.GetHouseRect().Contains(Utility.Vector2ToPoint(vector2)) && this.HasPermissionsToPaint((Building)null) && this.CanPaintHouse())
                    farm.frameHouseColor = new Color?(Color.Lime);
                foreach (Building building in ((BuildableGameLocation)Game1.getLocationFromName("Farm")).buildings)
                    building.color.Value = Color.White;
                Building building1 = ((BuildableGameLocation)Game1.getLocationFromName("Farm")).getBuildingAt(vector2) ?? ((BuildableGameLocation)Game1.getLocationFromName("Farm")).getBuildingAt(new Vector2((float)((Game1.viewport.X + Game1.getOldMouseX(false)) / 64), (float)((Game1.viewport.Y + Game1.getOldMouseY(false) + 128) / 64))) ?? ((BuildableGameLocation)Game1.getLocationFromName("Farm")).getBuildingAt(new Vector2((float)((Game1.viewport.X + Game1.getOldMouseX(false)) / 64), (float)((Game1.viewport.Y + Game1.getOldMouseY(false) + 192) / 64)));
                if (this.upgrading)
                {
                    if (building1 != null && this.CurrentBlueprint.nameOfBuildingToUpgrade != null && this.CurrentBlueprint.nameOfBuildingToUpgrade.Equals((string)(NetFieldBase<string, NetString>)building1.buildingType))
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
                    if (building1 == null || !this.hasPermissionsToDemolish(building1) || !this.CanDemolishThis(building1))
                        return;
                    building1.color.Value = Color.Red * 0.8f;
                }
                else if (this.moving)
                {
                    if (building1 == null || !this.hasPermissionsToMove(building1))
                        return;
                    building1.color.Value = Color.Lime * 0.8f;
                }
                else
                {
                    if (!this.painting || building1 == null || !building1.CanBePainted() || !this.HasPermissionsToPaint(building1))
                        return;
                    building1.color.Value = Color.Lime * 0.8f;
                }
            }
        }

        public bool hasPermissionsToDemolish(Building b)
        {
            return Game1.IsMasterGame && this.CanDemolishThis(b);
        }

        public bool CanPaintHouse() => Game1.MasterPlayer.HouseUpgradeLevel >= 2;

        public bool HasPermissionsToPaint(Building b)
        {
            if (b == null)
                return Game1.player.UniqueMultiplayerID == Game1.MasterPlayer.UniqueMultiplayerID || Game1.player.spouse == Game1.MasterPlayer.UniqueMultiplayerID.ToString();
            if (!b.isCabin || !(b.indoors.Value is Cabin))
                return true;
            Farmer owner = (b.indoors.Value as Cabin).owner;
            return Game1.player.UniqueMultiplayerID == owner.UniqueMultiplayerID || Game1.player.spouse == owner.UniqueMultiplayerID.ToString();
        }


        public bool hasPermissionsToMove(Building b)
        {
            return (Game1.getFarm().greenhouseUnlocked.Value || !(b is GreenhouseBuilding)) && (Game1.IsMasterGame || Game1.player.team.farmhandsCanMoveBuildings.Value == FarmerTeam.RemoteBuildingPermissions.On || Game1.player.team.farmhandsCanMoveBuildings.Value == FarmerTeam.RemoteBuildingPermissions.OwnedBuildings && b.hasCarpenterPermissions());
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

        //jon, 1/30/24: This function is updated from the original to return to farm when done instead of carpenter and to make screen panning faster in build mode.
        public override void receiveKeyPress(Keys key)
        {
            if (this.freeze)
                return;
            if (!this.onFarm)
                base.receiveKeyPress(key);
            if (Game1.delayedActions.Count() > 0 || !this.onFarm)
                return;
            if (Game1.options.doesInputListContain(Game1.options.menuButton, key) && this.readyToClose() && Game1.locationRequest == null)
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

        //jon, 1/30/24: This function is updated from the original to return to make screen panning faster in build mode.
        public override void update(GameTime time)
        {
            base.update(time);
            if (!this.onFarm || Game1.IsFading())
                return;
            int num1 = Game1.getOldMouseX(false) + Game1.viewport.X;
            int num2 = Game1.getOldMouseY(false) + Game1.viewport.Y;
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
            if (Game1.IsMultiplayer)
                return;
            Farm farm = Game1.getFarm();
            foreach (Character character in farm.animals.Values)
                character.MovePosition(Game1.currentGameTime, Game1.viewport, (GameLocation)farm);
        }

        //jon, 1/30/24: This function is updated from the original to make updating farm house instant instead of waiting 2 more days.
        //  Also, change all instances of returnToCarpentryMenu to returnToFarm
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
                    DelayedAction.functionAfterDelay(new DelayedAction.delayedBehavior(this.returnToFarm), MENU_DELAY);
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
            if (!this.onFarm && this.demolishButton.containsPoint(x, y) && this.demolishButton.visible && this.CanDemolishThis(this.blueprints[this.currentBlueprintIndex]))
            {
                Game1.globalFadeToBlack(new Game1.afterFadeFunction(this.setUpForBuildingPlacement));
                Game1.playSound("smallSelect");
                this.onFarm = true;
                this.demolishing = true;
            }
            if (!this.onFarm && this.moveButton.containsPoint(x, y) && this.moveButton.visible)
            {
                Game1.globalFadeToBlack(new Game1.afterFadeFunction(this.setUpForBuildingPlacement));
                Game1.playSound("smallSelect");
                this.onFarm = true;
                this.moving = true;
            }
            if (!this.onFarm && this.paintButton.containsPoint(x, y) && this.paintButton.visible)
            {
                Game1.globalFadeToBlack(new Game1.afterFadeFunction(this.setUpForBuildingPlacement));
                Game1.playSound("smallSelect");
                this.onFarm = true;
                this.painting = true;
            }
            if (this.okButton.containsPoint(x, y) && !this.onFarm && this.price >= 0 && Game1.player.Money >= this.price && this.blueprints[this.currentBlueprintIndex].doesFarmerHaveEnoughResourcesToBuild())
            {
                Game1.globalFadeToBlack(new Game1.afterFadeFunction(this.setUpForBuildingPlacement));
                Game1.playSound("smallSelect");
                this.onFarm = true;
            }
            if (!this.onFarm || this.freeze || Game1.IsFading())
                return;
            if (this.demolishing)
            {
                Farm farm = Game1.getLocationFromName("Farm") as Farm;
                Building destroyed = farm.getBuildingAt(new Vector2((float)((Game1.viewport.X + Game1.getOldMouseX(false)) / 64), (float)((Game1.viewport.Y + Game1.getOldMouseY(false)) / 64)));
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
                    if ((int)(NetFieldBase<int, NetInt>)destroyed.daysOfConstructionLeft > 0 || (int)(NetFieldBase<int, NetInt>)destroyed.daysUntilUpgrade > 0)
                        Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_DuringConstruction"), Color.Red, 3500f));
                    else if (destroyed.indoors.Value != null && destroyed.indoors.Value is AnimalHouse && (destroyed.indoors.Value as AnimalHouse).animalsThatLiveHere.Count > 0)
                        Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_AnimalsHere"), Color.Red, 3500f));
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
                            Chest chest = (Chest)null;
                            if (destroyed.indoors.Value is Cabin)
                            {
                                List<Item> list = (destroyed.indoors.Value as Cabin).demolish();
                                if (list.Count > 0)
                                {
                                    chest = new Chest(true);
                                    chest.fixLidFrame();
                                    chest.items.Set((IList<Item>)list);
                                }
                            }
                            if (!farm.destroyStructure(destroyed))
                                return;
                            int tileY = (int)(NetFieldBase<int, NetInt>)destroyed.tileY;
                            int tilesHigh = (int)(NetFieldBase<int, NetInt>)destroyed.tilesHigh;
                            Game1.flashAlpha = 1f;
                            destroyed.showDestroyedAnimation((GameLocation)Game1.getFarm());
                            Game1.playSound("explosion");
                            Utility.spreadAnimalsAround(destroyed, farm);
                            DelayedAction.functionAfterDelay(new DelayedAction.delayedBehavior(this.returnToFarm), MENU_DELAY);
                            this.freeze = true;
                            if (chest == null)
                                return;
                            farm.objects[new Vector2((float)((int)(NetFieldBase<int, NetInt>)destroyed.tileX + (int)(NetFieldBase<int, NetInt>)destroyed.tilesWide / 2), (float)((int)(NetFieldBase<int, NetInt>)destroyed.tileY + (int)(NetFieldBase<int, NetInt>)destroyed.tilesHigh / 2))] = (StardewValley.Object)chest;
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
                    if (!this.CanDemolishThis(destroyed))
                    {
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
                                DelayedAction.functionAfterDelay(new DelayedAction.delayedBehavior(this.returnToFarm), MENU_DELAY);
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
                Building buildingAt = ((BuildableGameLocation)Game1.getLocationFromName("Farm")).getBuildingAt(new Vector2((float)((Game1.viewport.X + Game1.getOldMouseX(false)) / 64), (float)((Game1.viewport.Y + Game1.getOldMouseY(false)) / 64)));
                if (buildingAt != null && this.CurrentBlueprint.name != null && buildingAt.buildingType.Equals((object)this.CurrentBlueprint.nameOfBuildingToUpgrade))
                {
                    this.CurrentBlueprint.consumeResources();
                    doInstantUpgrade(buildingAt);
                    Game1.playSound("axe");
                    DelayedAction.functionAfterDelay(new DelayedAction.delayedBehavior(this.returnToFarm), MENU_DELAY);
                    this.freeze = true;
                }
                else
                {
                    if (buildingAt == null)
                        return;
                    Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantUpgrade_BuildingType"), Color.Red, 3500f));
                }
            }
            else if (this.painting)
            {
                Farm farm_location = Game1.getFarm();
                Vector2 vector2 = new Vector2((float)((Game1.viewport.X + Game1.getMouseX(false)) / 64), (float)((Game1.viewport.Y + Game1.getMouseY(false)) / 64));
                Building buildingAt = farm_location.getBuildingAt(vector2);
                if (buildingAt != null)
                {
                    if (!buildingAt.CanBePainted())
                        Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CannotPaint"), Color.Red, 3500f));
                    else if (!this.HasPermissionsToPaint(buildingAt))
                    {
                        Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CannotPaint_Permission"), Color.Red, 3500f));
                    }
                    else
                    {
                        buildingAt.color.Value = Color.White;
                        this.SetChildMenu((IClickableMenu)new BuildingPaintMenu(buildingAt));
                    }
                }
                else
                {
                    if (!farm_location.GetHouseRect().Contains(Utility.Vector2ToPoint(vector2)))
                        return;
                    if (!this.CanPaintHouse())
                        Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CannotPaint"), Color.Red, 3500f));
                    else if (!this.HasPermissionsToPaint((Building)null))
                        Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CannotPaint_Permission"), Color.Red, 3500f));
                    else
                        this.SetChildMenu((IClickableMenu)new BuildingPaintMenu("House", (Func<Texture2D>)(() =>
                        {
                            if (farm_location.paintedHouseTexture != null)
                                return farm_location.paintedHouseTexture;
                            return Farm.houseTextures;
                        }), farm_location.houseSource.Value, farm_location.housePaintColor.Value));
                }
            }
            else if (this.moving)
            {
                if (this.buildingToMove == null)
                {
                    this.buildingToMove = ((BuildableGameLocation)Game1.getLocationFromName("Farm")).getBuildingAt(new Vector2((float)((Game1.viewport.X + Game1.getMouseX(false)) / 64), (float)((Game1.viewport.Y + Game1.getMouseY(false)) / 64)));
                    if (this.buildingToMove == null)
                        return;
                    if ((int)((NetFieldBase<int, NetInt>)this.buildingToMove.daysOfConstructionLeft) > 0)
                        this.buildingToMove = (Building)null;
                    else if (!this.hasPermissionsToMove(this.buildingToMove))
                    {
                        this.buildingToMove = (Building)null;
                    }
                    else
                    {
                        this.buildingToMove.isMoving = true;
                        Game1.playSound("axchop");
                    }
                }
                else if (((BuildableGameLocation)Game1.getLocationFromName("Farm")).buildStructure(this.buildingToMove, new Vector2((float)((Game1.viewport.X + Game1.getMouseX(false)) / 64), (float)((Game1.viewport.Y + Game1.getMouseY(false)) / 64)), Game1.player, false))
                {
                    this.buildingToMove.isMoving = false;
                    if (this.buildingToMove is ShippingBin)
                        (this.buildingToMove as ShippingBin).initLid();
                    if (this.buildingToMove is GreenhouseBuilding)
                        Game1.getFarm().greenhouseMoved.Value = true;
                    this.buildingToMove.performActionOnBuildingPlacement();
                    this.buildingToMove = (Building)null;
                    Game1.playSound("axchop");
                    DelayedAction.playSoundAfterDelay("dirtyHit", 50, (GameLocation)null);
                    DelayedAction.playSoundAfterDelay("dirtyHit", 150, (GameLocation)null);
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
                           DelayedAction.functionAfterDelay(new DelayedAction.delayedBehavior(this.returnToFarm), MENU_DELAY);
                           this.freeze = true;
                       }
                       else
                           Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantBuild"), Color.Red, 3500f));
                   }
                   Game1.player.team.buildLock.ReleaseLock();
               }), (Action)null);
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
            return ((BuildableGameLocation)Game1.getLocationFromName("Farm")).buildStructure(this.CurrentBlueprint, new Vector2((float)((Game1.viewport.X + Game1.getOldMouseX(false)) / 64), (float)((Game1.viewport.Y + Game1.getOldMouseY(false)) / 64)), Game1.player, this.magicalConstruction);
        }

        //jon, 1/30/24: New function to return to farm after building instead of carpenter menu.
        //  Removed returnToCarpentryMenu, returnToCarpentryMenuAfterSuccessfulBuild, and robinConstructionMessage.
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
            this.painting = false;
            this.buildingToMove = (Building)null;
            this.freeze = true;
            Game1.displayHUD = true;
            Game1.viewportFreeze = false;
            Game1.viewport.Location = new Location(5 * Game1.tileSize, 24 * Game1.tileSize);
            this.drawBG = true;
            this.demolishing = false;
            //Game1.displayFarmer = false;
        }

        //jon, 1/30/24: New function to replace screen fades and clears to remove the blinking effect -- not needed since we are not flashing between Robin's shop and the farm.
        public void doNothing()
        {
            this.freeze = false;
            Game1.displayFarmer = true;  //jon, 8/4/18: Attempt to fix disappearing character problem which I am having trouble reproducing but other players are getting.
        }


        public override bool overrideSnappyMenuCursorMovementBan() => this.onFarm;

        //jon, 1/30/24: Updated function to call doNothing instead of making screen fade so the build feels more instant. Also reset game location to current player position.
        public void setUpForBuildingPlacement()
        {
            Game1.currentLocation.cleanupBeforePlayerExit();
            this.hoverText = "";
            Game1.currentLocation = Game1.getLocationFromName("Farm");
            Game1.currentLocation.resetForPlayerEntry();
            Game1.delayedActions.Add(new DelayedAction(MENU_DELAY, new DelayedAction.delayedBehavior(this.doNothing)));
            this.onFarm = true;
            this.cancelButton.bounds.X = Game1.uiViewport.Width - 128;
            this.cancelButton.bounds.Y = Game1.uiViewport.Height - 128;
            Game1.displayHUD = false;
            Game1.viewportFreeze = true;
            Game1.viewport.Location = new Location((Game1.player.getTileX() * Game1.tileSize) - (Game1.viewport.Width / 2), (Game1.player.getTileY() * Game1.tileSize) - (Game1.viewport.Height / 2));
            Game1.panScreen(0, 0);
            this.drawBG = false;
            this.freeze = true;
            Game1.displayFarmer = false;
            if (this.demolishing || this.CurrentBlueprint.nameOfBuildingToUpgrade == null || (this.CurrentBlueprint.nameOfBuildingToUpgrade.Length <= 0 || this.moving) || this.painting)
                return;
            this.upgrading = true;
        }

        public override void gameWindowSizeChanged(Microsoft.Xna.Framework.Rectangle oldBounds, Microsoft.Xna.Framework.Rectangle newBounds)
        {
            this.resetBounds();
        }

        public virtual bool CanDemolishThis(Building building)
        {
            if (building == null)
                return false;
            if (this._demolishCheckBlueprint == null || this._demolishCheckBlueprint.name != building.buildingType.Value)
                this._demolishCheckBlueprint = new BluePrint((string)(NetFieldBase<string, NetString>)building.buildingType);
            return this._demolishCheckBlueprint == null || this.CanDemolishThis(this._demolishCheckBlueprint);
        }

        public virtual bool CanDemolishThis(BluePrint blueprint)
        {
            if (blueprint.moneyRequired < 0)
                return false;
            if (blueprint.name == "Shipping Bin")
            {
                int num = 0;
                foreach (Building building in Game1.getFarm().buildings)
                {
                    if (building is ShippingBin)
                        ++num;
                    if (num > 1)
                        break;
                }
                if (num <= 1)
                    return false;
            }
            return true;
        }

        public override void draw(SpriteBatch b)
        {
            if (this.drawBG)
                b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.5f);
            if (Game1.IsFading() || this.freeze)
                return;
            if (!this.onFarm)
            {
                base.draw(b);
                IClickableMenu.drawTextureBox(b, this.xPositionOnScreen - 96, this.yPositionOnScreen - 16, this.maxWidthOfBuildingViewer + 64, this.maxHeightOfBuildingViewer + 64, this.magicalConstruction ? Color.RoyalBlue : Color.White);
                this.currentBuilding.drawInMenu(b, this.xPositionOnScreen + this.maxWidthOfBuildingViewer / 2 - (int)(NetFieldBase<int, NetInt>)this.currentBuilding.tilesWide * 64 / 2 - 64, this.yPositionOnScreen + this.maxHeightOfBuildingViewer / 2 - this.currentBuilding.getSourceRectForMenu().Height * 4 / 2);
                if (this.CurrentBlueprint.isUpgrade())
                    this.upgradeIcon.draw(b);
                string s = " Deluxe  Barn   ";
                if (SpriteText.getWidthOfString(this.buildingName) >= SpriteText.getWidthOfString(s))
                    s = this.buildingName + " ";
                SpriteText.drawStringWithScrollCenteredAt(b, this.buildingName, this.xPositionOnScreen + this.maxWidthOfBuildingViewer - IClickableMenu.spaceToClearSideBorder - 16 + 64 + (this.width - (this.maxWidthOfBuildingViewer + 128)) / 2, this.yPositionOnScreen, SpriteText.getWidthOfString(s));
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
                    Utility.drawTextWithShadow(b, Game1.parseText(this.buildingDescription, Game1.dialogueFont, width - 32), Game1.dialogueFont, new Vector2((float)(this.xPositionOnScreen + this.maxWidthOfBuildingViewer - 4), (float)(this.yPositionOnScreen + 80 + 16 + 4)), Game1.textColor * 0.25f, shadowIntensity: 0.0f);
                    Utility.drawTextWithShadow(b, Game1.parseText(this.buildingDescription, Game1.dialogueFont, width - 32), Game1.dialogueFont, new Vector2((float)(this.xPositionOnScreen + this.maxWidthOfBuildingViewer - 1), (float)(this.yPositionOnScreen + 80 + 16 + 4)), Game1.textColor * 0.25f, shadowIntensity: 0.0f);
                }
                Utility.drawTextWithShadow(b, Game1.parseText(this.buildingDescription, Game1.dialogueFont, width - 32), Game1.dialogueFont, new Vector2((float)(this.xPositionOnScreen + this.maxWidthOfBuildingViewer), (float)(this.yPositionOnScreen + 80 + 16)), this.magicalConstruction ? Color.PaleGoldenrod : Game1.textColor, shadowIntensity: this.magicalConstruction ? 0.0f : 0.75f);
                Vector2 location = new Vector2((float)(this.xPositionOnScreen + this.maxWidthOfBuildingViewer + 16), (float)(this.yPositionOnScreen + 256 + 32));
                if (this.ingredients.Count < 3 && (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.fr || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ko || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.pt))
                    location.Y += 64f;
                if (this.price >= 0)
                {
                    SpriteText.drawString(b, "$", (int)location.X, (int)location.Y);
                    string numberWithCommas = Utility.getNumberWithCommas(this.price);
                    if (this.magicalConstruction)
                    {
                        Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\StringsFromCSFiles:LoadGameMenu.cs.11020", (object)numberWithCommas), Game1.dialogueFont, new Vector2(location.X + 64f, location.Y + 8f), Game1.textColor * 0.5f, shadowIntensity: this.magicalConstruction ? 0.0f : 0.25f);
                        Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\StringsFromCSFiles:LoadGameMenu.cs.11020", (object)numberWithCommas), Game1.dialogueFont, new Vector2((float)((double)location.X + 64.0 + 4.0 - 1.0), location.Y + 8f), Game1.textColor * 0.25f, shadowIntensity: this.magicalConstruction ? 0.0f : 0.25f);
                    }
                    Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\StringsFromCSFiles:LoadGameMenu.cs.11020", (object)numberWithCommas), Game1.dialogueFont, new Vector2((float)((double)location.X + 64.0 + 4.0), location.Y + 4f), Game1.player.Money >= this.price ? (this.magicalConstruction ? Color.PaleGoldenrod : Game1.textColor) : Color.Red, shadowIntensity: this.magicalConstruction ? 0.0f : 0.25f);
                }
                location.X -= 16f;
                location.Y -= 21f;
                foreach (Item ingredient in this.ingredients)
                {
                    location.Y += 68f;
                    ingredient.drawInMenu(b, location, 1f);
                    bool flag = !(ingredient is StardewValley.Object) || Game1.player.hasItemInInventory((int)(NetFieldBase<int, NetInt>)(ingredient as StardewValley.Object).parentSheetIndex, ingredient.Stack);
                    if (this.magicalConstruction)
                    {
                        Utility.drawTextWithShadow(b, ingredient.DisplayName, Game1.dialogueFont, new Vector2((float)((double)location.X + 64.0 + 12.0), location.Y + 24f), Game1.textColor * 0.25f, shadowIntensity: this.magicalConstruction ? 0.0f : 0.25f);
                        Utility.drawTextWithShadow(b, ingredient.DisplayName, Game1.dialogueFont, new Vector2((float)((double)location.X + 64.0 + 16.0 - 1.0), location.Y + 24f), Game1.textColor * 0.25f, shadowIntensity: this.magicalConstruction ? 0.0f : 0.25f);
                    }
                    Utility.drawTextWithShadow(b, ingredient.DisplayName, Game1.dialogueFont, new Vector2((float)((double)location.X + 64.0 + 16.0), location.Y + 20f), flag ? (this.magicalConstruction ? Color.PaleGoldenrod : Game1.textColor) : Color.Red, shadowIntensity: this.magicalConstruction ? 0.0f : 0.25f);
                }
                this.backButton.draw(b);
                this.forwardButton.draw(b);
                this.okButton.draw(b, this.blueprints[this.currentBlueprintIndex].doesFarmerHaveEnoughResourcesToBuild() ? Color.White : Color.Gray * 0.8f, 0.88f);
                this.demolishButton.draw(b, this.CanDemolishThis(this.blueprints[this.currentBlueprintIndex]) ? Color.White : Color.Gray * 0.8f, 0.88f);
                this.moveButton.draw(b);
                this.paintButton.draw(b);
            }
            else
            {
                string s = !this.upgrading ? (!this.demolishing ? (!this.painting ? Game1.content.LoadString("Strings\\UI:Carpenter_ChooseLocation") : Game1.content.LoadString("Strings\\UI:Carpenter_SelectBuilding_Paint")) : Game1.content.LoadString("Strings\\UI:Carpenter_SelectBuilding_Demolish")) : Game1.content.LoadString("Strings\\UI:Carpenter_SelectBuilding_Upgrade", (object)new BluePrint(this.CurrentBlueprint.nameOfBuildingToUpgrade).displayName);
                SpriteText.drawStringWithScrollBackground(b, s, Game1.uiViewport.Width / 2 - SpriteText.getWidthOfString(s) / 2, 16);
                Game1.StartWorldDrawInUI(b);
                if (!this.upgrading && !this.demolishing && !this.moving && !this.painting)
                {
                    Vector2 vector2 = new Vector2((float)((Game1.viewport.X + Game1.getOldMouseX(false)) / 64), (float)((Game1.viewport.Y + Game1.getOldMouseY(false)) / 64));
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
                    foreach (Point additionalPlacementTile in this.CurrentBlueprint.additionalPlacementTiles)
                    {
                        int x = additionalPlacementTile.X;
                        int y = additionalPlacementTile.Y;
                        int structurePlacementTile = this.CurrentBlueprint.getTileSheetIndexForStructurePlacementTile(x, y);
                        Vector2 tileLocation = new Vector2(vector2.X + (float)x, vector2.Y + (float)y);
                        if (!(Game1.currentLocation as BuildableGameLocation).isBuildable(tileLocation))
                            ++structurePlacementTile;
                        b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, tileLocation * 64f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(194 + structurePlacementTile * 16, 388, 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.999f);
                    }
                }
                else if (!this.painting && this.moving && this.buildingToMove != null)
                {
                    Vector2 vector2_1 = new Vector2((float)((Game1.viewport.X + Game1.getOldMouseX(false)) / 64), (float)((Game1.viewport.Y + Game1.getOldMouseY(false)) / 64));
                    BuildableGameLocation currentLocation = Game1.currentLocation as BuildableGameLocation;
                    for (int y = 0; y < (int)(NetFieldBase<int, NetInt>)this.buildingToMove.tilesHigh; ++y)
                    {
                        for (int x = 0; x < (int)(NetFieldBase<int, NetInt>)this.buildingToMove.tilesWide; ++x)
                        {
                            int structurePlacementTile = this.buildingToMove.getTileSheetIndexForStructurePlacementTile(x, y);
                            Vector2 vector2_2 = new Vector2(vector2_1.X + (float)x, vector2_1.Y + (float)y);
                            bool flag = currentLocation.buildings.Contains(this.buildingToMove) && this.buildingToMove.occupiesTile(vector2_2);
                            if (!currentLocation.isBuildable(vector2_2) && !flag)
                                ++structurePlacementTile;
                            b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, vector2_2 * 64f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(194 + structurePlacementTile * 16, 388, 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.999f);
                        }
                    }
                    foreach (Point additionalPlacementTile in this.buildingToMove.additionalPlacementTiles)
                    {
                        int x = additionalPlacementTile.X;
                        int y = additionalPlacementTile.Y;
                        int structurePlacementTile = this.buildingToMove.getTileSheetIndexForStructurePlacementTile(x, y);
                        Vector2 vector2_3 = new Vector2(vector2_1.X + (float)x, vector2_1.Y + (float)y);
                        bool flag = currentLocation.buildings.Contains(this.buildingToMove) && this.buildingToMove.occupiesTile(vector2_3);
                        if (!currentLocation.isBuildable(vector2_3) && !flag)
                            ++structurePlacementTile;
                        b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, vector2_3 * 64f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(194 + structurePlacementTile * 16, 388, 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.999f);
                    }
                }
                Game1.EndWorldDrawInUI(b);
            }
            this.cancelButton.draw(b);
            this.drawMouse(b);
            if (this.hoverText.Length <= 0)
                return;
            IClickableMenu.drawHoverText(b, this.hoverText, Game1.dialogueFont);
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
        }

    }
}
