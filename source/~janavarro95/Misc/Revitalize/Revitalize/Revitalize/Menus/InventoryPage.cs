using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using System;
using System.Collections.Generic;

namespace Revitalize.Menus
{
    public class InventoryPage : IClickableMenu
    {
        public const int region_inventory = 0;

        public const int region_hat = 1;

        public const int region_ring1 = 2;

        public const int region_ring2 = 3;

        public const int region_boots = 4;

        private InventoryMenu inventory;

        private string descriptionText = "";

        private string hoverText = "";

        private string descriptionTitle = "";

        private string hoverTitle = "";

        private Item heldItem;

        private Item hoveredItem;

        private List<ClickableComponent> equipmentIcons = new List<ClickableComponent>();

        private ClickableComponent portrait;

        private ClickableTextureComponent trashCan;

        private ClickableTextureComponent organizeButton;

        private float trashCanLidRotation;

        private string horseName = "";

        public InventoryPage(int x, int y, int width, int height) : base(x, y, width, height, false)
        {
            this.inventory = new InventoryMenu(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth, this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth, true, null, null, -1, 3, 0, 0, true);
            this.equipmentIcons.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + Game1.tileSize * 3 / 4, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 4 * Game1.tileSize - 12, Game1.tileSize, Game1.tileSize), "Hat"));
            this.equipmentIcons.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + Game1.tileSize * 3 / 4, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 5 * Game1.tileSize - 12, Game1.tileSize, Game1.tileSize), "Left Ring"));
            this.equipmentIcons.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + Game1.tileSize * 3 / 4, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 6 * Game1.tileSize - 12, Game1.tileSize, Game1.tileSize), "Right Ring"));
            this.equipmentIcons.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + Game1.tileSize * 3 / 4, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 7 * Game1.tileSize - 12, Game1.tileSize, Game1.tileSize), "Boots"));
            this.portrait = new ClickableComponent(new Rectangle(this.xPositionOnScreen + Game1.tileSize * 3 - Game1.tileSize + Game1.tileSize / 2, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 4 * Game1.tileSize - 8 + Game1.tileSize, Game1.tileSize, Game1.tileSize * 3 / 2), "32");
            this.trashCan = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + width / 3 + Game1.tileSize * 9 + Game1.tileSize / 2, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 3 * Game1.tileSize + Game1.tileSize, Game1.tileSize, 104), Game1.mouseCursors, new Rectangle(669, 261, 16, 26), (float)Game1.pixelZoom, false);
            this.organizeButton = new ClickableTextureComponent("", new Rectangle(this.xPositionOnScreen + width, this.yPositionOnScreen + height / 3 - Game1.tileSize + Game1.pixelZoom * 2, Game1.tileSize, Game1.tileSize), "", "Organize", Game1.mouseCursors, new Rectangle(162, 440, 16, 16), (float)Game1.pixelZoom, false);
            if (Utility.findHorse() != null)
            {
                this.horseName = Utility.findHorse().name;
            }
        }

        public override void receiveKeyPress(Keys key)
        {
            base.receiveKeyPress(key);
            if (Game1.isAnyGamePadButtonBeingPressed() && Game1.options.doesInputListContain(Game1.options.menuButton, key) && this.heldItem != null)
            {
                Game1.setMousePosition(this.trashCan.bounds.Center);
            }
            if (key.Equals(Keys.Delete) && this.heldItem != null && this.heldItem.canBeTrashed())
            {
                if (this.heldItem is StardewValley.Object && Game1.player.specialItems.Contains((this.heldItem as StardewValley.Object).parentSheetIndex))
                {
                    Game1.player.specialItems.Remove((this.heldItem as StardewValley.Object).parentSheetIndex);
                }
                this.heldItem = null;
                Game1.playSound("trashcan");
            }
            if (Game1.options.doesInputListContain(Game1.options.inventorySlot1, key))
            {
                Game1.player.CurrentToolIndex = 0;
                Game1.playSound("toolSwap");
                return;
            }
            if (Game1.options.doesInputListContain(Game1.options.inventorySlot2, key))
            {
                Game1.player.CurrentToolIndex = 1;
                Game1.playSound("toolSwap");
                return;
            }
            if (Game1.options.doesInputListContain(Game1.options.inventorySlot3, key))
            {
                Game1.player.CurrentToolIndex = 2;
                Game1.playSound("toolSwap");
                return;
            }
            if (Game1.options.doesInputListContain(Game1.options.inventorySlot4, key))
            {
                Game1.player.CurrentToolIndex = 3;
                Game1.playSound("toolSwap");
                return;
            }
            if (Game1.options.doesInputListContain(Game1.options.inventorySlot5, key))
            {
                Game1.player.CurrentToolIndex = 4;
                Game1.playSound("toolSwap");
                return;
            }
            if (Game1.options.doesInputListContain(Game1.options.inventorySlot6, key))
            {
                Game1.player.CurrentToolIndex = 5;
                Game1.playSound("toolSwap");
                return;
            }
            if (Game1.options.doesInputListContain(Game1.options.inventorySlot7, key))
            {
                Game1.player.CurrentToolIndex = 6;
                Game1.playSound("toolSwap");
                return;
            }
            if (Game1.options.doesInputListContain(Game1.options.inventorySlot8, key))
            {
                Game1.player.CurrentToolIndex = 7;
                Game1.playSound("toolSwap");
                return;
            }
            if (Game1.options.doesInputListContain(Game1.options.inventorySlot9, key))
            {
                Game1.player.CurrentToolIndex = 8;
                Game1.playSound("toolSwap");
                return;
            }
            if (Game1.options.doesInputListContain(Game1.options.inventorySlot10, key))
            {
                Game1.player.CurrentToolIndex = 9;
                Game1.playSound("toolSwap");
                return;
            }
            if (Game1.options.doesInputListContain(Game1.options.inventorySlot11, key))
            {
                Game1.player.CurrentToolIndex = 10;
                Game1.playSound("toolSwap");
                return;
            }
            if (Game1.options.doesInputListContain(Game1.options.inventorySlot12, key))
            {
                Game1.player.CurrentToolIndex = 11;
                Game1.playSound("toolSwap");
            }
        }

        public new void moveCursorInDirection(int direction)
        {
            int num = 0;
             base.moveCursorInDirection(direction);
            return;
            if (this.currentRegion == 0)
            {
                //int num = this.inventory.moveCursorInDirection(direction);
                if (num == 0)
                {
                    return;
                }
                if (num == 2)
                {
                    this.currentRegion = 1;
                    this.equipmentIcons[0].snapMouseCursor();
                }
            }
            else if (this.currentRegion == 1)
            {
                if (direction == 0)
                {
                    this.currentRegion = 0;
                    ClickableComponent lastClickableComponentInThisListThatContainsThisXCoord = base.getLastClickableComponentInThisListThatContainsThisXCoord(this.inventory.inventory, Game1.getMousePosition().X);
                    if (lastClickableComponentInThisListThatContainsThisXCoord != null)
                    {
                        lastClickableComponentInThisListThatContainsThisXCoord.snapMouseCursor();
                    }
                }
                else if (direction == 2)
                {
                    this.currentRegion = 2;
                    this.equipmentIcons[1].snapMouseCursor();
                }
            }
            else if (this.currentRegion == 2)
            {
                if (direction == 0)
                {
                    this.currentRegion = 1;
                    this.equipmentIcons[0].snapMouseCursor();
                }
                else if (direction == 2)
                {
                    this.currentRegion = 3;
                    this.equipmentIcons[2].snapMouseCursor();
                }
            }
            else if (this.currentRegion == 3)
            {
                if (direction == 0)
                {
                    this.currentRegion = 2;
                    this.equipmentIcons[1].snapMouseCursor();
                }
                else if (direction == 2)
                {
                    this.currentRegion = 4;
                    this.equipmentIcons[3].snapMouseCursor();
                }
            }
            else if (this.currentRegion == 4 && direction == 0)
            {
                this.currentRegion = 3;
                this.equipmentIcons[2].snapMouseCursor();
            }
            return;
        }

        public override void setUpForGamePadMode()
        {
            base.setUpForGamePadMode();
            if (this.inventory != null)
            {
                this.inventory.setUpForGamePadMode();
            }
            this.currentRegion = 0;
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            foreach (ClickableComponent current in this.equipmentIcons)
            {
                if (current.containsPoint(x, y))
                {
                    bool flag = this.heldItem == null;
                    string name = current.name;
                    if (!(name == "Hat"))
                    {
                        if (!(name == "Left Ring"))
                        {
                            if (!(name == "Right Ring"))
                            {
                                if (name == "Boots")
                                {
                                    if (this.heldItem == null || this.heldItem is Boots)
                                    {
                                        Boots boots = (Boots)this.heldItem;
                                        this.heldItem = Game1.player.boots;
                                        Game1.player.boots = boots;
                                        if (this.heldItem != null)
                                        {
                                            (this.heldItem as Boots).onUnequip();
                                        }
                                        if (Game1.player.boots != null)
                                        {
                                            Game1.player.boots.onEquip();
                                        }
                                        if (this.heldItem == null)
                                        {
                                            Game1.playSound("sandyStep");
                                            DelayedAction.playSoundAfterDelay("sandyStep", 150);
                                        }
                                        else
                                        {
                                            Game1.playSound("dwop");
                                        }
                                    }
                                }
                            }
                            else if (this.heldItem == null || this.heldItem is Ring)
                            {
                                Ring rightRing = (Ring)this.heldItem;
                                this.heldItem = Game1.player.rightRing;
                                Game1.player.rightRing = rightRing;
                                if (this.heldItem != null)
                                {
                                    (this.heldItem as Ring).onUnequip(Game1.player);
                                }
                                if (Game1.player.rightRing != null)
                                {
                                    Game1.player.rightRing.onEquip(Game1.player);
                                }
                                if (this.heldItem == null)
                                {
                                    Game1.playSound("crit");
                                }
                                else
                                {
                                    Game1.playSound("dwop");
                                }
                            }
                        }
                        else if (this.heldItem == null || this.heldItem is Ring)
                        {
                            Ring leftRing = (Ring)this.heldItem;
                            this.heldItem = Game1.player.leftRing;
                            Game1.player.leftRing = leftRing;
                            if (this.heldItem != null)
                            {
                                (this.heldItem as Ring).onUnequip(Game1.player);
                            }
                            if (Game1.player.leftRing != null)
                            {
                                Game1.player.leftRing.onEquip(Game1.player);
                            }
                            if (this.heldItem == null)
                            {
                                Game1.playSound("crit");
                            }
                            else
                            {
                                Game1.playSound("dwop");
                            }
                        }
                    }
                    else if (this.heldItem == null || this.heldItem is Hat)
                    {
                        Hat hat = (Hat)this.heldItem;
                        this.heldItem = Game1.player.hat;
                        Game1.player.hat = hat;
                        if (this.heldItem == null)
                        {
                            Game1.playSound("grassyStep");
                        }
                        else
                        {
                            Game1.playSound("dwop");
                        }
                    }
                    if (flag && this.heldItem != null && Game1.oldKBState.IsKeyDown(Keys.LeftShift))
                    {
                        for (int i = 0; i < Game1.player.items.Count; i++)
                        {
                            if (Game1.player.items[i] == null || Game1.player.items[i].canStackWith(this.heldItem))
                            {
                                if (Game1.player.CurrentToolIndex == i && this.heldItem != null)
                                {
                                    this.heldItem.actionWhenBeingHeld(Game1.player);
                                }
                                this.heldItem = Utility.addItemToInventory(this.heldItem, i, this.inventory.actualInventory, null);
                                if (Game1.player.CurrentToolIndex == i && this.heldItem != null)
                                {
                                    this.heldItem.actionWhenStopBeingHeld(Game1.player);
                                }
                                Game1.playSound("stoneStep");
                                return;
                            }
                        }
                    }
                }
            }
            this.heldItem = this.inventory.leftClick(x, y, this.heldItem, !Game1.oldKBState.IsKeyDown(Keys.LeftShift));
            if (this.heldItem != null && this.heldItem is StardewValley.Object && (this.heldItem as StardewValley.Object).ParentSheetIndex == 434)
            {
                Game1.playSound("smallSelect");
                Game1.playerEatObject(this.heldItem as StardewValley.Object, true);
                this.heldItem = null;
                Game1.exitActiveMenu();
            }
            else if (this.heldItem != null && Game1.oldKBState.IsKeyDown(Keys.LeftShift))
            {
                if (this.heldItem is Ring)
                {
                    if (Game1.player.leftRing == null)
                    {
                        Game1.player.leftRing = (this.heldItem as Ring);
                        (this.heldItem as Ring).onEquip(Game1.player);
                        this.heldItem = null;
                        Game1.playSound("crit");
                        return;
                    }
                    if (Game1.player.rightRing == null)
                    {
                        Game1.player.rightRing = (this.heldItem as Ring);
                        (this.heldItem as Ring).onEquip(Game1.player);
                        this.heldItem = null;
                        Game1.playSound("crit");
                        return;
                    }
                }
                else if (this.heldItem is Hat)
                {
                    if (Game1.player.hat == null)
                    {
                        Game1.player.hat = (this.heldItem as Hat);
                        Game1.playSound("grassyStep");
                        this.heldItem = null;
                        return;
                    }
                }
                else if (this.heldItem is Boots && Game1.player.boots == null)
                {
                    Game1.player.boots = (this.heldItem as Boots);
                    (this.heldItem as Boots).onEquip();
                    Game1.playSound("sandyStep");
                    DelayedAction.playSoundAfterDelay("sandyStep", 150);
                    this.heldItem = null;
                    return;
                }
                if (this.inventory.getInventoryPositionOfClick(x, y) >= 12)
                {
                    for (int j = 0; j < 12; j++)
                    {
                        if (Game1.player.items[j] == null || Game1.player.items[j].canStackWith(this.heldItem))
                        {
                            if (Game1.player.CurrentToolIndex == j && this.heldItem != null)
                            {
                                this.heldItem.actionWhenBeingHeld(Game1.player);
                            }
                            this.heldItem = Utility.addItemToInventory(this.heldItem, j, this.inventory.actualInventory, null);
                            if (this.heldItem != null)
                            {
                                this.heldItem.actionWhenStopBeingHeld(Game1.player);
                            }
                            Game1.playSound("stoneStep");
                            return;
                        }
                    }
                }
            }
            if (this.portrait.containsPoint(x, y))
            {
                this.portrait.name = (this.portrait.name.Equals("32") ? "8" : "32");
            }
            if (this.heldItem != null && this.trashCan.containsPoint(x, y) && this.heldItem.canBeTrashed())
            {
                if (this.heldItem is StardewValley.Object && Game1.player.specialItems.Contains((this.heldItem as StardewValley.Object).parentSheetIndex))
                {
                    Game1.player.specialItems.Remove((this.heldItem as StardewValley.Object).parentSheetIndex);
                }
                this.heldItem = null;
                Game1.playSound("trashcan");
            }
            else if (this.heldItem != null && !this.isWithinBounds(x, y) && this.heldItem.canBeTrashed())
            {
                Game1.playSound("throwDownITem");
                Game1.createItemDebris(this.heldItem, Game1.player.getStandingPosition(), Game1.player.FacingDirection);
                this.heldItem = null;
            }
            if (this.organizeButton != null && this.organizeButton.containsPoint(x, y))
            {
                ItemGrabMenu.organizeItemsInList(Game1.player.items);
                Game1.playSound("Ship");
            }
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            this.heldItem = this.inventory.rightClick(x, y, this.heldItem, true);
        }

        public override void performHoverAction(int x, int y)
        {
            this.descriptionText = "";
            this.descriptionTitle = "";
            this.hoveredItem = this.inventory.hover(x, y, this.heldItem);
            this.hoverText = this.inventory.hoverText;
            this.hoverTitle = this.inventory.hoverTitle;
            foreach (ClickableComponent current in this.equipmentIcons)
            {
                if (current.containsPoint(x, y))
                {
                    string name = current.name;
                    if (!(name == "Hat"))
                    {
                        if (!(name == "Right Ring"))
                        {
                            if (!(name == "Left Ring"))
                            {
                                if (name == "Boots")
                                {
                                    if (Game1.player.boots != null)
                                    {
                                        this.hoveredItem = Game1.player.boots;
                                        this.hoverText = Game1.player.boots.getDescription();
                                        this.hoverTitle = Game1.player.boots.name;
                                    }
                                }
                            }
                            else if (Game1.player.leftRing != null)
                            {
                                this.hoveredItem = Game1.player.leftRing;
                                this.hoverText = Game1.player.leftRing.getDescription();
                                this.hoverTitle = Game1.player.leftRing.name;
                            }
                        }
                        else if (Game1.player.rightRing != null)
                        {
                            this.hoveredItem = Game1.player.rightRing;
                            this.hoverText = Game1.player.rightRing.getDescription();
                            this.hoverTitle = Game1.player.rightRing.name;
                        }
                    }
                    else if (Game1.player.hat != null)
                    {
                        this.hoveredItem = Game1.player.hat;
                        this.hoverText = Game1.player.hat.getDescription();
                        this.hoverTitle = Game1.player.hat.name;
                    }
                    current.scale = Math.Min(current.scale + 0.05f, 1.1f);
                }
                current.scale = Math.Max(1f, current.scale - 0.025f);
            }
            if (this.portrait.containsPoint(x, y))
            {
                this.portrait.scale += 0.2f;
                this.hoverText = Game1.content.LoadString("Strings\\UI:Inventory_PortraitHover_Level", new object[]
                {
                    Game1.player.Level
                }) + Environment.NewLine + Game1.player.getTitle();
            }
            else
            {
                this.portrait.scale = 0f;
            }
            if (this.trashCan.containsPoint(x, y))
            {
                if (this.trashCanLidRotation <= 0f)
                {
                    Game1.playSound("trashcanlid");
                }
                this.trashCanLidRotation = Math.Min(this.trashCanLidRotation + 0.06544985f, 1.57079637f);
            }
            else if (this.trashCanLidRotation != 0f)
            {
                this.trashCanLidRotation = Math.Max(this.trashCanLidRotation - 0.1308997f, 0f);
                if (this.trashCanLidRotation == 0f)
                {
                    Game1.playSound("thudStep");
                }
            }
            if (this.organizeButton != null)
            {
                this.organizeButton.tryHover(x, y, 0.1f);
                if (this.organizeButton.containsPoint(x, y))
                {
                    this.hoverText = this.organizeButton.hoverText;
                }
            }
        }

        public override bool readyToClose()
        {
            return this.heldItem == null;
        }

        public override void draw(SpriteBatch b)
        {
            base.drawHorizontalPartition(b, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 3 * Game1.tileSize, false);
            this.inventory.draw(b);
            foreach (ClickableComponent current in this.equipmentIcons)
            {
                string name = current.name;
                if (!(name == "Hat"))
                {
                    if (!(name == "Right Ring"))
                    {
                        if (!(name == "Left Ring"))
                        {
                            if (name == "Boots")
                            {
                                if (Game1.player.boots != null)
                                {
                                    b.Draw(Game1.menuTexture, current.bounds, new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 10, -1, -1)), Color.White);
                                    Game1.player.boots.drawInMenu(b, new Vector2((float)current.bounds.X, (float)current.bounds.Y), current.scale);
                                }
                                else
                                {
                                    b.Draw(Game1.menuTexture, current.bounds, new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 40, -1, -1)), Color.White);
                                }
                            }
                        }
                        else if (Game1.player.leftRing != null)
                        {
                            b.Draw(Game1.menuTexture, current.bounds, new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 10, -1, -1)), Color.White);
                            Game1.player.leftRing.drawInMenu(b, new Vector2((float)current.bounds.X, (float)current.bounds.Y), current.scale);
                        }
                        else
                        {
                            b.Draw(Game1.menuTexture, current.bounds, new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 41, -1, -1)), Color.White);
                        }
                    }
                    else if (Game1.player.rightRing != null)
                    {
                        b.Draw(Game1.menuTexture, current.bounds, new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 10, -1, -1)), Color.White);
                        Game1.player.rightRing.drawInMenu(b, new Vector2((float)current.bounds.X, (float)current.bounds.Y), current.scale);
                    }
                    else
                    {
                        b.Draw(Game1.menuTexture, current.bounds, new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 41, -1, -1)), Color.White);
                    }
                }
                else if (Game1.player.hat != null)
                {
                    b.Draw(Game1.menuTexture, current.bounds, new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 10, -1, -1)), Color.White);
                    Game1.player.hat.drawInMenu(b, new Vector2((float)current.bounds.X, (float)current.bounds.Y), current.scale, 1f, 0.866f, false);
                }
                else
                {
                    b.Draw(Game1.menuTexture, current.bounds, new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 42, -1, -1)), Color.White);
                }
            }
            b.Draw((Game1.timeOfDay >= 1900) ? Game1.nightbg : Game1.daybg, new Vector2((float)(this.xPositionOnScreen + Game1.tileSize * 3 - Game1.tileSize), (float)(this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 4 * Game1.tileSize - 8)), Color.White);
            Game1.player.FarmerRenderer.draw(b, new FarmerSprite.AnimationFrame(0, Game1.player.bathingClothes ? 108 : 0, false, false, null, false), Game1.player.bathingClothes ? 108 : 0, new Rectangle(0, Game1.player.bathingClothes ? 576 : 0, 16, 32), new Vector2((float)(this.xPositionOnScreen + Game1.tileSize * 3 - Game1.tileSize / 2), (float)(this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 5 * Game1.tileSize - Game1.tileSize / 2)), Vector2.Zero, 0.8f, 2, Color.White, 0f, 1f, Game1.player);
            if (Game1.timeOfDay >= 1900)
            {
                Game1.player.FarmerRenderer.draw(b, new FarmerSprite.AnimationFrame(0, 0, false, false, null, false), 0, new Rectangle(0, 0, 16, 32), new Vector2((float)(this.xPositionOnScreen + Game1.tileSize * 3 - Game1.tileSize / 2), (float)(this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 5 * Game1.tileSize - Game1.tileSize / 2)), Vector2.Zero, 0.8f, 2, Color.DarkBlue * 0.3f, 0f, 1f, Game1.player);
            }
            Utility.drawTextWithShadow(b, Game1.player.name, Game1.dialogueFont, new Vector2((float)(this.xPositionOnScreen + Game1.tileSize * 3) - Math.Min((float)Game1.tileSize, Game1.dialogueFont.MeasureString(Game1.player.name).X / 2f), (float)(this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 7 * Game1.tileSize + Game1.pixelZoom * 2)), Game1.textColor, 1f, -1f, -1, -1, 1f, 3);
            string text = Game1.content.LoadString("Strings\\UI:Inventory_FarmName", new object[]
            {
                Game1.player.farmName
            });
            Utility.drawTextWithShadow(b, text, Game1.dialogueFont, new Vector2((float)(this.xPositionOnScreen + Game1.tileSize * 8 + Game1.tileSize / 2) - Game1.dialogueFont.MeasureString(text).X / 2f, (float)(this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 4 * Game1.tileSize + Game1.pixelZoom)), Game1.textColor, 1f, -1f, -1, -1, 1f, 3);
            string text2 = Game1.content.LoadString("Strings\\UI:Inventory_CurrentFunds", new object[]
            {
                Utility.getNumberWithCommas(Game1.player.Money)
            });
            Utility.drawTextWithShadow(b, text2, Game1.dialogueFont, new Vector2((float)(this.xPositionOnScreen + Game1.tileSize * 8 + Game1.tileSize / 2) - Game1.dialogueFont.MeasureString(text2).X / 2f, (float)(this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 5 * Game1.tileSize)), Game1.textColor, 1f, -1f, -1, -1, 1f, 3);
            string text3 = Game1.content.LoadString("Strings\\UI:Inventory_TotalEarnings", new object[]
            {
                Utility.getNumberWithCommas((int)Game1.player.totalMoneyEarned)
            });
            Utility.drawTextWithShadow(b, text3, Game1.dialogueFont, new Vector2((float)(this.xPositionOnScreen + Game1.tileSize * 8 + Game1.tileSize / 2) - Game1.dialogueFont.MeasureString(text3).X / 2f, (float)(this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 6 * Game1.tileSize - Game1.pixelZoom)), Game1.textColor, 1f, -1f, -1, -1, 1f, 3);
            if (Game1.player != null)
            {
                
                if (Game1.player.hasPet())
                {
                    string petName = Game1.player.getPetName();
                    Utility.drawTextWithShadow(b, petName, Game1.dialogueFont, new Vector2((float)(this.xPositionOnScreen + Game1.tileSize * 5) + Math.Max((float)Game1.tileSize, Game1.dialogueFont.MeasureString(Game1.player.name).X / 2f), (float)(this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 7 * Game1.tileSize + Game1.pixelZoom * 2)), Game1.textColor, 1f, -1f, -1, -1, 1f, 3);
                    Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2((float)(this.xPositionOnScreen + Game1.tileSize * 4) + Math.Max((float)Game1.tileSize, Game1.dialogueFont.MeasureString(Game1.player.name).X / 2f), (float)(this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 7 * Game1.tileSize - Game1.pixelZoom)), new Rectangle(160 + (Game1.player.catPerson ? 0 : 16), 192, 16, 16), Color.White, 0f, Vector2.Zero, (float)Game1.pixelZoom, false, -1f, -1, -1, 0.35f);
                }
                
            }
            if (this.horseName.Length > 0)
            {
                Utility.drawTextWithShadow(b, this.horseName, Game1.dialogueFont, new Vector2((float)(this.xPositionOnScreen + Game1.tileSize * 6) + Math.Max((float)Game1.tileSize, Game1.dialogueFont.MeasureString(Game1.player.name).X / 2f) + ((Game1.player.getPetName() != null) ? Math.Max((float)Game1.tileSize, Game1.dialogueFont.MeasureString(Game1.player.getPetName()).X) : 0f), (float)(this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 7 * Game1.tileSize + Game1.pixelZoom * 2)), Game1.textColor, 1f, -1f, -1, -1, 1f, 3);
                Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2((float)(this.xPositionOnScreen + Game1.tileSize * 5 + Game1.pixelZoom * 2) + Math.Max((float)Game1.tileSize, Game1.dialogueFont.MeasureString(Game1.player.name).X / 2f) + ((Game1.player.getPetName() != null) ? Math.Max((float)Game1.tileSize, Game1.dialogueFont.MeasureString(Game1.player.getPetName()).X) : 0f), (float)(this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 7 * Game1.tileSize - Game1.pixelZoom)), new Rectangle(193, 192, 16, 16), Color.White, 0f, Vector2.Zero, (float)Game1.pixelZoom, false, -1f, -1, -1, 0.35f);
            }
            int arg_A52_0 = this.xPositionOnScreen;
            int arg_A5B_0 = this.width / 3;
            int arg_A61_0 = Game1.tileSize;
            int arg_A67_0 = Game1.tileSize;
            if (this.organizeButton != null)
            {
                this.organizeButton.draw(b);
            }
            this.trashCan.draw(b);
            b.Draw(Game1.mouseCursors, new Vector2((float)(this.trashCan.bounds.X + 60), (float)(this.trashCan.bounds.Y + 40)), new Rectangle?(new Rectangle(686, 256, 18, 10)), Color.White, this.trashCanLidRotation, new Vector2(16f, 10f), (float)Game1.pixelZoom, SpriteEffects.None, 0.86f);
            if (this.heldItem != null)
            {
                this.heldItem.drawInMenu(b, new Vector2((float)(Game1.getOldMouseX() + 16), (float)(Game1.getOldMouseY() + 16)), 1f);
            }
            if (this.hoverText != null && !this.hoverText.Equals(""))
            {
                IClickableMenu.drawToolTip(b, this.hoverText, this.hoverTitle, this.hoveredItem, this.heldItem != null, -1, 0, -1, -1, null, -1);
            }
        }
    }
}
