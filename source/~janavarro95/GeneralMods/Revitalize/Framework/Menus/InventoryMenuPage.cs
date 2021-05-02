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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;
using StardustCore.Animations;
using StardustCore.UIUtilities;
using StardustCore.UIUtilities.MenuComponents;
using StardustCore.UIUtilities.MenuComponents.ComponentsV2.Buttons;

namespace Revitalize.Framework.Menus
{
    /// <summary>
    /// Deals with displaying the internal contents of an inventory. Used mainly as a component for another menu that extends this functionality.
    /// </summary>
    public class InventoryMenuPage
    {

        int index;
        public List<ItemDisplayButton> storageDisplay;
        public int amountToDisplay;

        public InventoryMenuPage()
        {

        }

        public InventoryMenuPage(int index, List<ItemDisplayButton> Buttons, int AmountToDisplay)
        {
            this.index = index;
            this.storageDisplay = Buttons;
            this.amountToDisplay = AmountToDisplay;
        }
    }

    /// <summary>
    /// An inventory menu that displays the contents of an inventory but doesn't do much else.
    /// </summary>
    public class InventoryMenu : IClickableMenuExtended
    {
        public IList<Item> items;
        public int capacity;
        public Item activeItem;

        public int rows;
        public int collumns;
        public int xOffset = 64;
        public int yOffset = 128;

        public StardewValley.Menus.TextBox searchBox;

        public Dictionary<int, InventoryMenuPage> pages;
        public int pageIndex = 0;

        public AnimatedButton nextPage;
        public AnimatedButton previousPage;
        public Color backgroundColor;

        public string hoverText;
        public string hoverTitle;
        public Item displayItem;

        /// <summary>
        /// Checks if the given inventory is full or not.
        /// </summary>
        public bool isFull
        {
            get
            {
                return this.items.Count >= this.capacity && this.items.Where(i => i == null).Count() == 0;
            }
        }

        public InventoryMenu(int xPos, int yPos, int width, int height, int Rows, int Collumns, bool showCloseButton, IList<Item> Inventory, int maxCapacity, Color BackgroundColor) : base(xPos, yPos, width, height, showCloseButton)
        {
            //Amount to display is the lower cap per page.
            //
            this.backgroundColor = BackgroundColor == null ? Color.White : BackgroundColor;

            this.items = Inventory;
            this.pages = new Dictionary<int, InventoryMenuPage>();
            this.capacity = maxCapacity;
            this.rows = Rows;
            this.collumns = Collumns;
            this.populateClickableItems(this.rows, this.collumns, xPos + this.xOffset, yPos + this.yOffset);

            this.searchBox = new TextBox((Texture2D)null, (Texture2D)null, Game1.dialogueFont, Game1.textColor);
            this.searchBox.X = this.xPositionOnScreen;
            this.searchBox.Y = this.yPositionOnScreen;
            this.searchBox.Width = 256;
            this.searchBox.Height = 192;
            Game1.keyboardDispatcher.Subscriber = (IKeyboardSubscriber)this.searchBox;
            this.searchBox.Selected = false;

            this.nextPage = new AnimatedButton(new StardustCore.Animations.AnimatedSprite("Next Page", new Vector2(128 + (this.searchBox.X + this.searchBox.Width), this.searchBox.Y), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "InventoryMenu", "NextPageButton"), new Animation(0, 0, 32, 32)), Color.White), new Rectangle(0, 0, 32, 32), 2f);
            this.previousPage = new AnimatedButton(new StardustCore.Animations.AnimatedSprite("Previous Page", new Vector2(64 + (this.searchBox.X + this.searchBox.Width), this.searchBox.Y), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "InventoryMenu", "PreviousPageButton"), new Animation(0, 0, 32, 32)), Color.White), new Rectangle(0, 0, 32, 32), 2f);

        }

        /// <summary>
        /// What happens when the game's viewport is changed.
        /// </summary>
        /// <param name="oldBounds"></param>
        /// <param name="newBounds"></param>
        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            base.gameWindowSizeChanged(oldBounds, newBounds);

            this.searchBox.X = this.xPositionOnScreen;
            this.searchBox.Y = this.yPositionOnScreen;
            this.populateClickableItems(this.rows, this.collumns, this.xPositionOnScreen + this.xOffset, this.yPositionOnScreen + this.yOffset);
            this.nextPage.Position = new Vector2(128 + (this.searchBox.X + this.searchBox.Width), this.searchBox.Y);
            this.previousPage.Position = new Vector2(64 + (this.searchBox.X + this.searchBox.Width), this.searchBox.Y);
        }

        public void populateClickableItems()
        {
            this.populateClickableItems(this.rows, this.collumns, this.xPositionOnScreen + this.xOffset, this.yPositionOnScreen + this.yOffset);
        }

        /// <summary>
        /// Populates the menu with all of the items on display.
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="collums"></param>
        /// <param name="xPosition"></param>
        /// <param name="yPosition"></param>
        public void populateClickableItems(int rows, int collums, int xPosition, int yPosition)
        {
            this.pages.Clear();
            int size = this.capacity;
            //ModCore.log("Hello World! SIZE IS: " + size);

            int maxPages = ((size) / (this.rows * this.collumns)) + 1;
            for (int i = 0; i < maxPages; i++)
            {
                int amount = Math.Min(rows * collums, size);
                this.pages.Add(i, new InventoryMenuPage(i, new List<ItemDisplayButton>(), amount));
                //ModCore.log("Added in a new page with size: " + size);
                size -= amount;
                for (int y = 0; y < collums; y++)
                {
                    for (int x = 0; x < rows; x++)
                    {
                        int index = ((y * rows) + x) + (rows * collums * i);
                        if (index >= this.pages[i].amountToDisplay + (rows * collums * i))
                        {
                            //ModCore.log("Break page creation.");
                            //ModCore.log("Index is: " + index);
                            //ModCore.log("Max display is: " + this.pages[i].amountToDisplay);
                            break;
                        }

                        if (index > this.items.Count)
                        {
                            Vector2 pos2 = new Vector2(x * 64 + xPosition, y * 64 + yPosition);
                            ItemDisplayButton b2 = new ItemDisplayButton(null, new StardustCore.Animations.AnimatedSprite("ItemBackground", pos2, new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "InventoryMenu", "ItemBackground"), new Animation(0, 0, 32, 32)), Color.White), pos2, new Rectangle(0, 0, 32, 32), 2f, true, Color.White);
                            this.pages[i].storageDisplay.Add(b2);
                            continue;
                        }

                        Item item = this.getItemFromList(index);
                        Vector2 pos = new Vector2(x * 64 + xPosition, y * 64 + yPosition);
                        ItemDisplayButton b = new ItemDisplayButton(item, new StardustCore.Animations.AnimatedSprite("ItemBackground", pos, new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "InventoryMenu", "ItemBackground"), new Animation(0, 0, 32, 32)), Color.White), pos, new Rectangle(0, 0, 32, 32), 2f, true, Color.White);
                        this.pages[i].storageDisplay.Add(b);
                    }
                }

            }
        }

        /// <summary>
        /// Gets an item from the list of items.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private Item getItemFromList(int index)
        {
            if (this.items == null) return null;
            if (index >= this.items.Count) return null;
            else return this.items.ElementAt(index);
        }

        /// <summary>
        /// What happens when this menu is hover overed.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public override void performHoverAction(int x, int y)
        {
            bool hovered = false;
            foreach (ItemDisplayButton button in this.pages[this.pageIndex].storageDisplay)
            {
                if (button.Contains(x, y))
                {
                    if (button.item == null)
                    {
                        continue;
                    }
                    else if (button.item != null)
                    {
                        this.displayItem = button.item;
                        this.hoverTitle = this.displayItem.DisplayName;
                        this.hoverText = this.displayItem.getDescription();
                        hovered = true;
                    }
                }
            }
            if (hovered == false)
            {
                this.displayItem = null;
            }
        }

        /// <summary>
        /// What happens when a key is pressed.
        /// </summary>
        /// <param name="key"></param>
        public override void receiveKeyPress(Keys key)
        {
            base.receiveKeyPress(key);
        }


        /// <summary>
        /// What happens when the menu is left clicked.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="playSound"></param>
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            int index = 0 + (this.rows * this.collumns * this.pageIndex);
            Item swap = null;
            foreach (ItemDisplayButton button in this.pages[this.pageIndex].storageDisplay)
            {
                if (button.receiveLeftClick(x, y))
                {
                    if (index > this.capacity) continue;
                    if (this.activeItem == null)
                    {
                        this.activeItem = button.item;
                        if (this.activeItem != null)
                        {
                            //ModCore.log("Got item: " + this.activeItem.DisplayName);
                        }
                        return;
                    }
                    else if (this.activeItem != null)
                    {
                        if (button.item == null)
                        {
                            //ModCore.log("Placed item: " + this.activeItem.DisplayName);
                            swap = this.activeItem;
                            this.activeItem = null;
                            break;
                        }
                        else
                        {
                            swap = button.item;
                            //ModCore.log("Swap item: " + swap.DisplayName);
                            break;
                        }
                    }
                }
                index++;
            }
            if (swap != null && this.activeItem == null)
            {
                this.swapItemPosition(index, swap);
                swap = null;
            }
            else if (swap != null && this.activeItem != null)
            {
                this.swapItemPosition(index, this.activeItem);
                this.activeItem = null;
                swap = null;
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

            if (this.nextPage.containsPoint(x, y))
            {
                //ModCore.log("Left click next page");
                if (this.pageIndex + 1 < this.pages.Count)
                {
                    this.pageIndex++;
                    Game1.soundBank.PlayCue("shwip");
                }
            }
            if (this.previousPage.containsPoint(x, y))
            {
                //ModCore.log("Left click previous page");
                if (this.pageIndex > 0)
                {
                    this.pageIndex--;
                    Game1.soundBank.PlayCue("shwip");
                }
            }
        }

        /// <summary>
        /// Swaps the item's position in the menu.
        /// </summary>
        /// <param name="insertIndex"></param>
        /// <param name="I"></param>
        public void swapItemPosition(int insertIndex, Item I)
        {
            if (I == null)
            {
                //ModCore.log("Odd item is null");
                return;
            }
            if (insertIndex + 1 > this.items.Count)
            {
                this.items.Remove(I);
                this.items.Add(I);
                this.populateClickableItems(this.rows, this.collumns, this.xPositionOnScreen + this.xOffset, this.yPositionOnScreen + this.yOffset);
                return;
            }
            this.items.Insert(insertIndex + 1, I);
            this.items.Remove(I);
            this.populateClickableItems(this.rows, this.collumns, this.xPositionOnScreen + this.xOffset, this.yPositionOnScreen + this.yOffset);
        }

        /// <summary>
        /// Takes the active item from this menu.
        /// </summary>
        /// <returns></returns>
        public Item takeActiveItem()
        {
            this.items.Remove(this.activeItem);
            this.populateClickableItems(this.rows, this.collumns, this.xPositionOnScreen + this.xOffset, this.yPositionOnScreen + this.yOffset);
            Item i = this.activeItem;
            this.activeItem = null;
            return i;
        }

        /// <summary>
        /// What happens when this menu is right clicked.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="playSound"></param>
        public override void receiveRightClick(int x, int y, bool playSound = true)
        {

        }

        /// <summary>
        /// What happens when the menu is updated.
        /// </summary>
        /// <param name="time"></param>
        public override void update(GameTime time)
        {

        }

        /// <summary>
        /// Draws the menu to the screen.
        /// </summary>
        /// <param name="b"></param>
        public override void draw(SpriteBatch b)
        {
            this.drawDialogueBoxBackground(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, this.backgroundColor);

            int index = -1;
            foreach (ItemDisplayButton button in this.pages[this.pageIndex].storageDisplay)
            {
                index++;
                float alpha = this.getItemDrawAlpha(button.item, index + this.pageIndex * this.rows*this.collumns);
                button.draw(b, 0.25f, alpha, true);

            }

            this.searchBox.Draw(b, true);

            this.nextPage.draw(b, 0.25f, 1f);
            this.previousPage.draw(b, 0.25f, 1f);

            b.DrawString(Game1.dialogueFont, ("Page: " + (this.pageIndex + 1) + " / " + this.pages.Count).ToString(), new Vector2(this.xPositionOnScreen, this.yPositionOnScreen + this.height), Color.White);

            this.drawMouse(b);

            this.drawToolTip(b);
            //base.draw(b);
        }

        public void drawToolTip(SpriteBatch b)
        {
            if (this.displayItem != null) IClickableMenu.drawToolTip(b, this.hoverText, this.hoverTitle, this.displayItem, false, -1, 0, -1, -1, (CraftingRecipe)null, -1);
        }

        /// <summary>
        /// Draws the item as a transparent icon if the item is not part of the seach patern.
        /// </summary>
        /// <param name="I"></param>
        /// <returns></returns>
        private float getItemDrawAlpha(Item I, int index)
        {
            if (index >= this.capacity) return 0.0f;
            if (string.IsNullOrEmpty(this.searchBox.Text) == false)
            {
                if (I == null) return 0.25f;
                return I.DisplayName.ToLowerInvariant().Contains(this.searchBox.Text.ToLowerInvariant()) ? 1f : 0.25f;
            }
            else
            {
                return 1f;
            }
        }
    }
}
