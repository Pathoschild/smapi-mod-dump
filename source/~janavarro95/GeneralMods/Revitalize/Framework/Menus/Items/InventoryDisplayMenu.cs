/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Netcode;
using Omegasis.Revitalize.Framework.Constants;
using Omegasis.Revitalize.Framework.HUD;
using Omegasis.Revitalize.Framework.Managers;
using Omegasis.Revitalize.Framework.Menus.MenuComponents;
using Omegasis.Revitalize.Framework.Utilities.JsonContentLoading;
using Omegasis.Revitalize.Framework.World.Buildings;
using Omegasis.Revitalize.Framework.World.Objects;
using Omegasis.Revitalize.Framework.World.Objects.Interfaces;
using Omegasis.Revitalize.Framework.World.WorldUtilities;
using Omegasis.StardustCore.UIUtilities.MenuComponents.ComponentsV2.Buttons;
using StardewValley;
using StardewValley.Menus;

namespace Omegasis.Revitalize.Framework.Menus.Items
{
    public class InventoryDisplayMenu : IClickableMenu
    {

        public static int menuWidth = 632 + borderWidth * 2;
        public static int menuHeight = 600 + borderWidth * 2 + Game1.tileSize;


        /*********
** Fields
*********/
        /// <summary>The labels to draw.</summary>
        public List<ClickableComponent> Labels = new List<ClickableComponent>();
        public List<ItemDisplayButton> itemButtons = new List<ItemDisplayButton>();

        /// <summary>The OK button to draw.</summary>
        public ClickableTextureComponent OkButton;

        /// <summary>
        /// The item that is currently being hover overed.
        /// </summary>
        public Item currentHoverTextureItem;

        public bool allFinished;

        public int currentPageNumber;

        public ClickableTextureComponent _leftButton;
        public ClickableTextureComponent _rightButton;

        public ClickableTextureComponent _searchModeButton;



        public int _maxRowsToDisplay = 5;
        public int _maxColumnsToDisplay = 6;

        /// <summary>
        /// The search box used for looking for specific items.
        /// </summary>
        public ItemSearchTextBox searchBox;

        public StardewValley.Menus.InventoryMenu playersInventory;

        public string hoverText;

        public string defaultTitle;
        public ClickableComponent capacityDisplayComponent;


        public Color? itemBackgroundDisplayColor;
        public Color? capacityDisplayTextColor;


        public IList<Item> itemsToAccess;

        public long inventoryMaxCapacity;

        public ObjectColorPicker objectColorPicker;
        public StardewValley.Object customObjectForColoring;

        /*********
** Public methods
*********/
        /// <summary>Construct an instance.</summary>
        /// <param name="season">The initial birthday season.</param>
        /// <param name="day">The initial birthday day.</param>
        /// <param name="onChanged">The callback to invoke when the birthday value changes.</param>
        public InventoryDisplayMenu(Color? ItemBackgroundDisplayColor, Color? CapacityDisplayTextColor ,IList<Item> ItemsToAccess, long InventoryMaxCapacity)
            : base((int)getAppropriateMenuPosition(menuWidth, menuHeight).X, (int)getAppropriateMenuPosition(menuWidth, menuHeight).Y, menuWidth, menuHeight)
        {
            this.searchBox = new ItemSearchTextBox(null, null, Game1.dialogueFont, Game1.textColor, new Rectangle(0, 0, 0, 0));
            Game1.keyboardDispatcher.Subscriber = this.searchBox;
            this.searchBox.Selected = false;
            this.searchBox.onTextReceived += this.SearchBox_onTextReceived;
            this.searchBox.OnBackspacePressed += this.SearchBox_OnBackspacePressed;

            this.defaultTitle = JsonContentPackUtilities.LoadStringFromDictionaryFile(Path.Combine(Constants.PathConstants.StringsPaths.Menus, "InventoryDisplayMenu.json"), "Capacity");


            this.itemBackgroundDisplayColor = ItemBackgroundDisplayColor;
            this.capacityDisplayTextColor = CapacityDisplayTextColor ?? Game1.textColor;
            this.itemsToAccess = ItemsToAccess;
            this.inventoryMaxCapacity = InventoryMaxCapacity;


            this.setUpPositions();
        }

        public InventoryDisplayMenu(Color? ItemBackgroundDisplayColor, Color? CapacityDisplayTextColor, IList<Item> ItemsToAccess, long InventoryMaxCapacity, StardewValley.Object CustomModObjectForColoring)
    : this(ItemBackgroundDisplayColor,CapacityDisplayTextColor,ItemsToAccess,InventoryMaxCapacity)
        {
            this.customObjectForColoring= CustomModObjectForColoring;
            this.setUpPositions();
        }

        public virtual long getInventoryMaxCapacity()
        {
            return this.inventoryMaxCapacity;
        }


        public virtual void SearchBox_OnBackspacePressed(TextBox sender)
        {
            SoundUtilities.PlaySound(Enums.StardewSound.Cowboy_gunshot);
            this.searchBox.backSpacePressed();
            this.populateItemsToDisplay();
        }

        public virtual void SearchBox_onTextReceived(object sender, string e)
        {
            SoundUtilities.PlaySound(Enums.StardewSound.Cowboy_gunshot);
            this.populateItemsToDisplay();
        }

        /// <summary>The method called when the game window changes size.</summary>
        /// <param name="oldBounds">The former viewport.</param>
        /// <param name="newBounds">The new viewport.</param>
        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            base.gameWindowSizeChanged(oldBounds, newBounds);
            this.xPositionOnScreen = Game1.viewport.Width / 2 - (632 + borderWidth * 2) / 2;
            this.yPositionOnScreen = Game1.viewport.Height / 2 - (600 + borderWidth * 2) / 2 - Game1.tileSize;

            this.setUpPositions();
        }

        /// <summary>Regenerate the UI.</summary>
        public virtual void setUpPositions()
        {
            this.OkButton = new ClickableTextureComponent("OK", new Rectangle(this.xPositionOnScreen + this.width - borderWidth - spaceToClearSideBorder - Game1.tileSize, this.yPositionOnScreen + this.height - borderWidth - spaceToClearTopBorder + Game1.tileSize / 4, Game1.tileSize, Game1.tileSize), "", null, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46), 1f);
            this._leftButton = new ClickableTextureComponent("LeftButton", new Rectangle(this.xPositionOnScreen + this.width - borderWidth - spaceToClearSideBorder - Game1.tileSize, this.yPositionOnScreen + this.height - borderWidth - spaceToClearTopBorder + Game1.tileSize / 4 + 96, Game1.tileSize, Game1.tileSize), "", null, TextureManagers.Menus_InventoryMenu.getExtendedTexture("PreviousPageButton").getTexture(), new Rectangle(0, 0, 32, 32), 2f);
            this._rightButton = new ClickableTextureComponent("RightButton", new Rectangle(this.xPositionOnScreen + this.width - borderWidth - spaceToClearSideBorder - Game1.tileSize + 96, this.yPositionOnScreen + this.height - borderWidth - spaceToClearTopBorder + Game1.tileSize / 4 + 96, Game1.tileSize, Game1.tileSize), "", null, TextureManagers.Menus_InventoryMenu.getExtendedTexture("NextPageButton").getTexture(), new Rectangle(0, 0, 32, 32), 2f);

            this.capacityDisplayComponent = new ClickableComponent(new Rectangle(this.xPositionOnScreen + 128, this.yPositionOnScreen + 128, 1, 1), "");

            this.searchBox.Bounds = new Rectangle(this.xPositionOnScreen + 96, this.yPositionOnScreen, 256, 192);

            this._searchModeButton = new ClickableTextureComponent("SearchMode", new Rectangle(this.searchBox.X - 96, this.searchBox.Y, 64, 64), "", "", TextureManagers.Menus_InventoryMenu.getExtendedTexture("SearchButton").getTexture(), new Rectangle(0, 0, 32, 32), 2f);

            if (this.customObjectForColoring != null)
            {
                this.objectColorPicker = new ObjectColorPicker(this.searchBox.X + this.searchBox.Width + 64, this.searchBox.Y, 0, this.customObjectForColoring);
            }

            this.populateItemsToDisplay();

            int yPositionForInventory = this.yPositionOnScreen + this.height + 96;
            int xPositionForInventory = this.xPositionOnScreen;
            this.playersInventory = new StardewValley.Menus.InventoryMenu(xPositionForInventory, yPositionForInventory, true, null, null);

        }

        /// <summary>Handle a button click.</summary>
        /// <param name="name">The button name that was clicked.</param>
        public virtual void handleButtonClick(string name)
        {
            if (name == null)
                return;

            switch (name)
            {
                // OK button
                case "OK":
                    this.allFinished = true;
                    SoundUtilities.PlaySound(Enums.StardewSound.coin);
                    Game1.exitActiveMenu();
                    return;

                case "LeftButton":
                    if (this.currentPageNumber == 0) break;
                    else
                    {
                        this.currentPageNumber--;
                        this.setUpPositions();
                    }
                    SoundUtilities.PlaySound(Enums.StardewSound.shwip);
                    return;

                case "RightButton":
                    int value = (this.currentPageNumber + 1) * this._maxRowsToDisplay * this._maxColumnsToDisplay;
                    if (value >= this.itemsToAccess.Count) break;
                    else
                    {
                        this.currentPageNumber++;
                        this.setUpPositions();
                    }
                    SoundUtilities.PlaySound(Enums.StardewSound.shwip);
                    return;

                case "SearchMode":
                    this.searchBox.cycleSearchMode();
                    SoundUtilities.PlaySound(Enums.StardewSound.coin);
                    this.populateItemsToDisplay();
                    return;

                default:
                    break;
            }
            //SoundUtilities.PlaySound(Enums.StardewSound.coin);
        }

        /// <summary>The method invoked when the player left-clicks on the menu.</summary>
        /// <param name="x">The X-position of the cursor.</param>
        /// <param name="y">The Y-position of the cursor.</param>
        /// <param name="playSound">Whether to enable sound.</param>
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            bool clickedItem = false;
            foreach (ItemDisplayButton button in this.itemButtons)
                if (button.containsPoint(x, y))
                {
                    this.handleButtonClick(button.name);
                    button.scale -= 0.5f;
                    button.scale = Math.Max(3.5f, button.scale);

                    Item i = button.item;

                    if (i == null)
                    {
                        continue;
                    }
                    SoundUtilities.PlaySound(Enums.StardewSound.coin);
                    Game1.player.addItemToInventory(i);
                    this.itemsToAccess.Remove(i);
                    clickedItem = true;
                    break;
                }
            if (clickedItem)
            {
                this.populateItemsToDisplay();
                return;
            }

            bool clickedPlayersItem = false;
            foreach (ClickableComponent c in this.playersInventory.inventory)
            {
                if (!c.containsPoint(x, y))
                {
                    continue;
                }
                else
                {
                    int slotNumber = Convert.ToInt32(c.name);
                    Item item = Game1.player.Items.ElementAt(slotNumber);
                    if (item == null)
                    {
                        continue;
                    }
                    if(item== this.customObjectForColoring)
                    {
                        //Prevent putting the storage container into itself, effectively destroying the container.
                        //But just for fun, allow putting storage containers into storage containers.
                        continue;
                    }
                    SoundUtilities.PlaySound(Enums.StardewSound.coin);
                    bool added = this.addItemToInventory(item);
                    if (!added) break;
                    Game1.player.removeItemFromInventory(item);
                    clickedPlayersItem = true;

                    break;
                }
            }
            if (clickedPlayersItem)
            {
                this.populateItemsToDisplay();
                return;
            }


            if (this.OkButton.containsPoint(x, y))
            {
                this.handleButtonClick(this.OkButton.name);
                this.OkButton.scale -= 0.25f;
                this.OkButton.scale = Math.Max(0.75f, this.OkButton.scale);
            }
            if (this._leftButton.containsPoint(x, y))
                this.handleButtonClick(this._leftButton.name);
            if (this._rightButton.containsPoint(x, y))
                this.handleButtonClick(this._rightButton.name);
            if (this._searchModeButton.containsPoint(x, y))
            {
                this.handleButtonClick(this._searchModeButton.name);
            }

            Rectangle r = new Rectangle(this.searchBox.X, this.searchBox.Y, this.searchBox.Width, this.searchBox.Height / 2);
            if (r.Contains(x, y))
            {
                this.searchBox.Update();
                this.searchBox.SelectMe();
            }
            else
            {
                this.searchBox.Selected = false;
            }

            if (this.objectColorPicker != null)
            {
                this.objectColorPicker.receiveLeftClick(x, y);
            }
        }

        /// <summary>The method invoked when the player right-clicks on the lookup UI.</summary>
        /// <param name="x">The X-position of the cursor.</param>
        /// <param name="y">The Y-position of the cursor.</param>
        /// <param name="playSound">Whether to enable sound.</param>
        public override void receiveRightClick(int x, int y, bool playSound = true)
        {

            bool clickedItem = false;

            int amountToGrab = 1;

            foreach (ItemDisplayButton button in this.itemButtons)
                if (button.containsPoint(x, y))
                {
                    this.handleButtonClick(button.name);
                    button.scale -= 0.5f;
                    button.scale = Math.Max(3.5f, button.scale);



                    if (button.item == null)
                    {
                        continue;
                    }

                    if (Game1.input.GetKeyboardState().IsKeyDown(Keys.LeftShift) || Game1.input.GetKeyboardState().IsKeyDown(Keys.Right))
                    {
                        amountToGrab = Math.Max(button.item.Stack / 2, 1);
                    }


                    Item i = button.item.getOne();
                    i.Stack = amountToGrab;

                    Game1.player.addItemToInventory(i);

                    button.item.Stack -= amountToGrab;

                    if (button.item.Stack <= 0)
                    {
                        clickedItem = true;
                        this.itemsToAccess.Remove(button.item);
                    }
                    SoundUtilities.PlaySound(Enums.StardewSound.coin);
                    break;
                }
            if (clickedItem)
            {
                this.populateItemsToDisplay();
                return;
            }

            bool clickedPlayersItem = false;
            foreach (ClickableComponent c in this.playersInventory.inventory)
            {
                if (!c.containsPoint(x, y))
                {
                    continue;
                }
                else
                {
                    int slotNumber = Convert.ToInt32(c.name);
                    Item item = Game1.player.Items.ElementAt(slotNumber);
                    if (item == null)
                    {
                        continue;
                    }
                    SoundUtilities.PlaySound(Enums.StardewSound.coin);

                    if (Game1.input.GetKeyboardState().IsKeyDown(Keys.LeftShift) || Game1.input.GetKeyboardState().IsKeyDown(Keys.Right))
                    {
                        amountToGrab = Math.Max(item.Stack / 2, 1);
                    }

                    Item itemToAdd = item.getOne();
                    itemToAdd.Stack = amountToGrab;
                    bool added = this.addItemToInventory(itemToAdd);
                    if (!added)
                    {
                        break;
                    }
                    item.Stack -= amountToGrab;
                    clickedPlayersItem = true;
                    if (item.Stack <= 0)
                    {
                        Game1.player.removeItemFromInventory(item);
                    }
                    break;
                }
            }
            if (clickedPlayersItem)
            {
                this.populateItemsToDisplay();
                return;
            }

        }

        /// <summary>The method invoked when the player hovers the cursor over the menu.</summary>
        /// <param name="x">The X-position of the cursor.</param>
        /// <param name="y">The Y-position of the cursor.</param>
        public override void performHoverAction(int x, int y)
        {
            bool hoverOverItemButtonThisFrame = false;
            foreach (ItemDisplayButton button in this.itemButtons)
            {
                if (button.containsPoint(x, y))
                {
                    button.scale = button.containsPoint(x, y)
                        ? Math.Min(button.scale + 0.02f, button.baseScale + 0.1f)
                        : Math.Max(button.scale - 0.02f, button.baseScale);
                    this.currentHoverTextureItem = button.item;
                    hoverOverItemButtonThisFrame = true;
                }
            }

            foreach (ClickableComponent c in this.playersInventory.inventory)
            {

                if (!c.containsPoint(x, y))
                {
                    continue;
                }
                else
                {
                    int slotNumber = Convert.ToInt32(c.name);
                    Item item = Game1.player.Items.ElementAt(slotNumber);
                    if (item == null)
                    {
                        continue;
                    }
                    this.currentHoverTextureItem = item;
                    hoverOverItemButtonThisFrame = true;
                    break;
                }

            }

            if (hoverOverItemButtonThisFrame == false)
                this.currentHoverTextureItem = null;

            this.OkButton.scale = this.OkButton.containsPoint(x, y)
    ? Math.Min(this.OkButton.scale + 0.02f, this.OkButton.baseScale + 0.1f)
    : Math.Max(this.OkButton.scale - 0.02f, this.OkButton.baseScale);

            if (this._searchModeButton.containsPoint(x, y))
            {
                this.hoverText = this.searchBox.getCurrentSearchModeDisplayString();
                this._searchModeButton.scale = Math.Min(this._searchModeButton.scale + 0.02f, this._searchModeButton.baseScale + 0.1f);
            }
            else
            {
                this.hoverText = "";
                this._searchModeButton.scale = Math.Max(this._searchModeButton.scale - 0.02f, this._searchModeButton.baseScale);
            }
        }


        public override bool readyToClose()
        {
            return this.allFinished;
        }

        public virtual void updateDisplayTexts()
        {
            this.capacityDisplayComponent.name = string.Format(this.defaultTitle, this.itemsToAccess.Count.ToString(), this.getInventoryMaxCapacity());
        }

        /// <summary>
        /// Populates all of the items for the menu.
        /// </summary>
        /// <returns></returns>
        public virtual void populateItemsToDisplay()
        {
            this.updateDisplayTexts();

            this.itemButtons.Clear();
            IList<Item> validItems = this.searchBox.getValidItems(this.itemsToAccess);

            for (int row = 0; row < this._maxRowsToDisplay; row++)
                for (int column = 0; column < this._maxColumnsToDisplay; column++)
                {
                    int value = this.currentPageNumber * this._maxRowsToDisplay * this._maxColumnsToDisplay + row * this._maxColumnsToDisplay + column;
                    if (value >= validItems.Count) continue;


                    Item selectedItem = validItems.ElementAt(value);
                    float itemScale = 4f;
                    Rectangle placementBounds = new Rectangle((int)(this.xPositionOnScreen + 64 + (column * 24 * itemScale)), (int)(this.yPositionOnScreen + 256 + row * 16 * itemScale), 16, 16);
                    ItemDisplayButton itemButton = new ItemDisplayButton(selectedItem.DisplayName, selectedItem, null, placementBounds, 4f, true, Color.White);
                    itemButton.item = selectedItem;

                    this.itemButtons.Add(itemButton);
                }

        }


        public override void receiveGamePadButton(Buttons b)
        {
            if (b.Equals(Buttons.A))
            {
                this.receiveLeftClick(Game1.getMouseX(), Game1.getMouseY(), true);
            }
            if (b.Equals(Buttons.Y) || b.Equals(Buttons.Start))
            {
                this.allFinished = true;
            }
        }

        public override void receiveKeyPress(Keys key)
        {
            if (key.Equals(Keys.Escape))
            {
                this.allFinished = true;
            }
            else
            {
                base.receiveKeyPress(key);
            }
        }

        public override bool areGamePadControlsImplemented()
        {
            return true;
        }

        /// <summary>
        /// Make this true if free cursor movement is desired.
        /// </summary>
        /// <returns></returns>
        public override bool overrideSnappyMenuCursorMovementBan()
        {
            return true;
        }

        /// <summary>Draw the menu to the screen.</summary>
        /// <param name="b">The sprite batch.</param>
        public override void draw(SpriteBatch b)
        {
            // draw menu box
            Game1.drawDialogueBox(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, false, true, r: this.itemBackgroundDisplayColor.HasValue ? this.itemBackgroundDisplayColor.Value.R : -1, g: this.itemBackgroundDisplayColor.HasValue ? this.itemBackgroundDisplayColor.Value.G : -1, b: this.itemBackgroundDisplayColor.HasValue ? this.itemBackgroundDisplayColor.Value.B : -1);

            //Why does this have really weird positioning? Once again, not sure, but for some reason, this is what works.
            Game1.drawDialogueBox(this.playersInventory.xPositionOnScreen - 32, this.playersInventory.yPositionOnScreen - 136, (int)(this.playersInventory.width * 1.1f), (int)(this.playersInventory.height * 1.8f), false, true);

            //b.Draw(Game1.daybg, new Vector2((this.xPositionOnScreen + Game1.tileSize + Game1.tileSize * 2 / 3 - 2), (this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - Game1.tileSize / 4)), Color.White);
            //Game1.player.FarmerSprite.draw(b, new Vector2((this.xPositionOnScreen + Game1.tileSize + Game1.tileSize * 2 / 3 - 2), (this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - Game1.tileSize / 4)),1f);

            this.searchBox.Draw(b, true);
            this._searchModeButton.draw(b);

            if (this.objectColorPicker != null)
            {
                this.objectColorPicker.draw(b);
            }

            // draw season buttons
            foreach (ItemDisplayButton button in this.itemButtons)
            {
                button.draw(b);
                //Utility.drawTinyDigits(button.item.Stack, b, new Vector2(button.boundingBox.Right - 16, button.boundingBox.Bottom - 16), 3f, 1f, Color.White);
            }

            Color color = Color.Violet;
            Utility.drawTextWithShadow(b, this.capacityDisplayComponent.name, Game1.smallFont, new Vector2(this.capacityDisplayComponent.bounds.X, this.capacityDisplayComponent.bounds.Y), Color.Violet);
            string text = "";
            Utility.drawTextWithShadow(b, this.capacityDisplayComponent.name, Game1.smallFont, new Vector2(this.capacityDisplayComponent.bounds.X, this.capacityDisplayComponent.bounds.Y), this.capacityDisplayTextColor.Value);
            if (text.Length > 0)
                Utility.drawTextWithShadow(b, text, Game1.smallFont, new Vector2(this.capacityDisplayComponent.bounds.X + Game1.tileSize / 3 - Game1.smallFont.MeasureString(text).X / 2f, this.capacityDisplayComponent.bounds.Y + Game1.tileSize / 2), color);


            if (!string.IsNullOrEmpty(this.hoverText))
            {
                drawHoverText(b, this.hoverText, Game1.dialogueFont);
            }

            // draw OK button
            this.OkButton.draw(b);
            this._leftButton.draw(b);
            this._rightButton.draw(b);

            this.playersInventory.draw(b);

            if (this.currentHoverTextureItem != null)
                //Draws the item tooltip in the menu.
                drawToolTip(b, this.currentHoverTextureItem.getDescription(), this.currentHoverTextureItem.DisplayName, this.currentHoverTextureItem);

            // draw cursor
            this.drawMouse(b);
        }


        public virtual bool addItemToInventory(Item item)
        {
            for (int i = 0; i < this.itemsToAccess.Count; i++)
            {
                //Check to see if the items can stack. If they can simply add them together and then continue on.
                if (this.itemsToAccess[i] != null && this.itemsToAccess[i].canStackWith(item))
                {
                    this.itemsToAccess[i].Stack += item.Stack;
                    return true;
                }
            }

            if ((long)this.itemsToAccess.Count < this.getInventoryMaxCapacity())
            {
                this.itemsToAccess.Add(item);
                return true;
            }
            else
            {
                HudUtilities.ShowInventoryFullErrorMessage();
            }
            return false;
        }


        public static Vector2 getAppropriateMenuPosition(int MenuWidth, int MenuHeight)
        {
            Vector2 defaultPosition = new Vector2(Game1.viewport.Width / 2 - MenuWidth / 2, (Game1.viewport.Height / 2 - MenuHeight / 2));

            //Force the viewport into a position that it should fit into on the screen???
            if (defaultPosition.X + MenuWidth > Game1.viewport.Width)
            {
                defaultPosition.X = 0;
            }

            if (defaultPosition.Y + MenuHeight > Game1.viewport.Height)
            {
                defaultPosition.Y = 0;
            }
            return defaultPosition;

        }
    }
}
