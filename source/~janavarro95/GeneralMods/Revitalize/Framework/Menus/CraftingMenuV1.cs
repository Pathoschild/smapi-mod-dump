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
using Revitalize.Framework.Menus.MenuComponents;
using Revitalize.Framework.World.Objects.Machines;
using StardewValley;
using StardewValley.Menus;
using StardustCore.UIUtilities;
using StardustCore.UIUtilities.MenuComponents.ComponentsV2.Buttons;
using Revitalize;

namespace Revitalize.Framework.Menus
{
    /// <summary>
    /// A simple menu for displaying craftable objects.
    /// </summary>
    public class CraftingMenuV1 : IClickableMenuExtended
    {

        /// <summary>
        /// All the different pages for crafting.
        /// </summary>
        public Dictionary<string, AnimatedButton> CraftingTabs;

        /// <summary>
        /// All of the actual buttons to display for crafting.
        /// </summary>
        public Dictionary<string, List<CraftingRecipeButton>> craftingItemsToDisplay;
        /// <summary>
        /// The inventory to tke items from.
        /// </summary>
        public IList<Item> fromInventory;
        /// <summary>
        /// The inventory to put items into.
        /// </summary>
        public IList<Item> toInventory;

        /// <summary>
        /// The current page index for the current tab.
        /// </summary>
        public int currentPageIndex;
        /// <summary>
        /// The name of the current tab.
        /// </summary>
        public string currentTab;

        /// <summary>
        /// The background color for the menu.
        /// </summary>
        public Color backgroundColor;

        /// <summary>
        /// The x offset for the menu so that tabs can be interacted with properly.
        /// </summary>
        public int xOffset = 72;

        /// <summary>
        /// Teh hover text to display.
        /// </summary>
        public string hoverText;

        /// <summary>
        /// How many crafting recipes to display at a time.
        /// </summary>
        public int amountOfRecipesToShow = 9;

        /// <summary>
        /// Is this menu the player's?
        /// </summary>
        public bool playerInventory;

        /// <summary>
        /// The crafting info menu that displays when a recipe is clicked.
        /// </summary>
        public CraftingInformationPage craftingInfo;

        /// <summary>
        /// The previous page button.
        /// </summary>
        public AnimatedButton leftButton;
        /// <summary>
        /// The next page button.
        /// </summary>
        public AnimatedButton rightButton;
        /// <summary>
        /// The search box used for looking for specific items.
        /// </summary>
        public StardewValley.Menus.TextBox searchBox;

        private Machine machine;

        /// <summary>
        /// The maximum amount of pages to display.
        /// </summary>
        private int maxPages
        {
            get
            {
                if (string.IsNullOrEmpty(this.currentTab)) return 0;
                List<CraftingRecipeButton> searchSelection;
                if (string.IsNullOrEmpty(this.searchBox.Text) == false)
                {
                    searchSelection = this.craftingItemsToDisplay[this.currentTab].FindAll(i => i.displayItem.item.DisplayName.ToLowerInvariant().Contains(this.searchBox.Text.ToLowerInvariant()));
                }
                else
                {
                    searchSelection = this.craftingItemsToDisplay[this.currentTab];
                }
                return (int)(Math.Ceiling((double)(searchSelection.Count / this.amountOfRecipesToShow)));
            }
        }

        public CraftingMenuV1() : base()
        {

        }

        /// <summary>
        /// Constructor to be used when the inventory is the player's
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
        /// <param name="BackgroundColor"></param>
        /// <param name="Inventory"></param>
        public CraftingMenuV1(int X, int Y, int Width, int Height, Color BackgroundColor, IList<Item> Inventory) : base(X, Y, Width, Height, false)
        {
            this.backgroundColor = BackgroundColor;
            this.CraftingTabs = new Dictionary<string, AnimatedButton>();
            this.craftingItemsToDisplay = new Dictionary<string, List<CraftingRecipeButton>>();
            this.currentPageIndex = 0;
            this.fromInventory = Inventory;
            this.toInventory = Inventory;
            this.playerInventory = true;
            this.initializeButtons();
        }

        /// <summary>
        /// Constructor to be used when inventory destination is the same and not the player.
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
        /// <param name="BackgroundColor"></param>
        /// <param name="Inventory"></param>
        public CraftingMenuV1(int X, int Y, int Width, int Height, Color BackgroundColor, ref IList<Item> Inventory) : base(X, Y, Width, Height, false)
        {
            this.backgroundColor = BackgroundColor;
            this.CraftingTabs = new Dictionary<string, AnimatedButton>();
            this.craftingItemsToDisplay = new Dictionary<string, List<CraftingRecipeButton>>();
            this.currentPageIndex = 0;
            this.fromInventory = Inventory;
            this.toInventory = Inventory;
            this.initializeButtons();
        }

        /// <summary>
        /// Inventory constructor to be used when the input and output inventories are different.
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
        /// <param name="BackgroundColor"></param>
        /// <param name="FromInventory"></param>
        /// <param name="ToInventory"></param>
        public CraftingMenuV1(int X, int Y, int Width, int Height, Color BackgroundColor, ref IList<Item> FromInventory, ref IList<Item> ToInventory,Machine Machine) : base(X, Y, Width, Height, false)
        {
            this.backgroundColor = BackgroundColor;
            this.CraftingTabs = new Dictionary<string, AnimatedButton>();
            this.craftingItemsToDisplay = new Dictionary<string, List<CraftingRecipeButton>>();
            this.currentPageIndex = 0;
            this.fromInventory = FromInventory;
            this.toInventory = ToInventory;
            this.initializeButtons();
            this.machine = Machine;
            this.playerInventory = false;
        }

        /// <summary>
        /// Fix the display of menu elements when the menu is resized.
        /// </summary>
        /// <param name="oldBounds"></param>
        /// <param name="newBounds"></param>
        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            base.gameWindowSizeChanged(oldBounds, newBounds);
            if (this.craftingInfo != null)
            {
                this.craftingInfo.gameWindowSizeChanged(oldBounds, newBounds);

            }
        }

        /// <summary>
        /// Initialize the buttons for the menu.
        /// </summary>
        private void initializeButtons()
        {
            this.leftButton = new AnimatedButton(new StardustCore.Animations.AnimatedSprite("Left Button", new Vector2(this.xPositionOnScreen, this.yPositionOnScreen), new StardustCore.Animations.AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "InventoryMenu", "PreviousPageButton"), new StardustCore.Animations.Animation(0, 0, 32, 32)), Color.White), new Rectangle(0, 0, 32, 32), 2f);
            this.rightButton = new AnimatedButton(new StardustCore.Animations.AnimatedSprite("Right Button", new Vector2(this.xPositionOnScreen + this.width, this.yPositionOnScreen), new StardustCore.Animations.AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "InventoryMenu", "NextPageButton"), new StardustCore.Animations.Animation(0, 0, 32, 32)), Color.White), new Rectangle(0, 0, 32, 32), 2f);

            this.searchBox = new TextBox((Texture2D)null, (Texture2D)null, Game1.dialogueFont, Game1.textColor);
            this.searchBox.X = this.xPositionOnScreen + this.width + 96;
            this.searchBox.Y = this.yPositionOnScreen;
            this.searchBox.Width = 256;
            this.searchBox.Height = 192;
            Game1.keyboardDispatcher.Subscriber = (IKeyboardSubscriber)this.searchBox;
            this.searchBox.Selected = false;
        }
        /// <summary>
        /// What happens when the menu receives a key press.
        /// </summary>
        /// <param name="key"></param>
        public override void receiveKeyPress(Keys key)
        {
            if (this.searchBox.Selected && key != Keys.Escape)
            {
                return;
            }
            else
            {
                base.receiveKeyPress(key);
            }
        }
        /// <summary>
        /// Sorts all of the recipes for the menu.
        /// </summary>
        public void sortRecipes()
        {
            foreach (KeyValuePair<string, List<CraftingRecipeButton>> pair in this.craftingItemsToDisplay)
            {
                List<CraftingRecipeButton> copy = pair.Value.ToList();
                pair.Value.Clear();

                copy = copy.OrderBy(x => x.displayItem.item.DisplayName).ToList();
                foreach (CraftingRecipeButton b in copy)
                {
                    this.addInCraftingRecipe(b, pair.Key);
                }
            }


        }
        /// <summary>
        /// Adds in a new tab for the crafting recipe menu.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="Button"></param>
        public void addInCraftingPageTab(string name, AnimatedButton Button)
        {
            int count = this.CraftingTabs.Count;

            if (this.CraftingTabs.ContainsKey(name))
            {
                return;
            }
            else
            {
                Vector2 newPos = new Vector2(100 + (48), (this.yPositionOnScreen+24) + ((24 * 4) * (count + 1)));
                ModCore.log("newPos: " + newPos.ToString());
                Button.Position = newPos;
                this.CraftingTabs.Add(name, Button);
                this.craftingItemsToDisplay.Add(name, new List<CraftingRecipeButton>());
            }

        }

        /// <summary>
        /// Adds in a crafting recipe to the crafting menu.
        /// </summary>
        /// <param name="Button"></param>
        /// <param name="WhichTab"></param>
        public void addInCraftingRecipe(CraftingRecipeButton Button, string WhichTab)
        {
            if (this.craftingItemsToDisplay.ContainsKey(WhichTab))
            {
                int count = this.craftingItemsToDisplay[WhichTab].Count % this.amountOfRecipesToShow;
                Vector2 newPos = new Vector2(this.xPositionOnScreen + (128), (this.yPositionOnScreen + 64) + (64 * (count + 1)));
                Button.displayItem.Position = newPos;
                this.craftingItemsToDisplay[WhichTab].Add(Button);
            }
            else
            {
                throw new Exception("Tab: " + WhichTab + " doesn't exist!");
            }
        }

        /// <summary>
        /// What happens when you hover over a menu element.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public override void performHoverAction(int x, int y)
        {
            bool hovered = false;
            foreach (KeyValuePair<string, AnimatedButton> pair in this.CraftingTabs)
            {
                if (pair.Value.containsPoint(x, y))
                {
                    this.hoverText = pair.Key;
                    hovered = true;
                }
            }

            //get range of buttons to show

            if (string.IsNullOrEmpty(this.currentTab) == false)
            {
                List<CraftingRecipeButton> buttonsToDraw = this.getRecipeButtonsToDisplay();
                foreach (CraftingRecipeButton button in buttonsToDraw)
                {
                    if (button.containsPoint(x, y))
                    {
                        this.hoverText = button.recipe.outputName;
                        hovered = true;
                    }
                }
            }
            if (hovered == false)
            {
                this.hoverText = "";
            }
        }

        /// <summary>
        /// What happens when the meu receives a left click.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="playSound"></param>
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (this.leftButton.containsPoint(x, y))
            {
                if (this.currentPageIndex <= 0) this.currentPageIndex = 0;
                else
                {
                    this.currentPageIndex--;
                    Game1.playSound("shwip");
                }
            }
            if (this.rightButton.containsPoint(x, y))
            {
                if (this.currentPageIndex < this.maxPages)
                {
                    this.currentPageIndex++;
                    Game1.playSound("shwip");
                }
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

            foreach (KeyValuePair<string, AnimatedButton> pair in this.CraftingTabs)
            {
                if (pair.Value.containsPoint(x, y))
                {
                    this.currentTab = pair.Key;
                    this.currentPageIndex = 0;
                    return;
                }
            }

            //get range of buttons to show

            if (string.IsNullOrEmpty(this.currentTab) == false)
            {
                List<CraftingRecipeButton> buttonsToDraw = this.getRecipeButtonsToDisplay();
                foreach (CraftingRecipeButton button in buttonsToDraw)
                {
                    if (button.containsPoint(x, y))
                    {
                        //button.craftItem(this.fromInventory, this.toInventory);

                        if (this.playerInventory)
                        {
                            this.fromInventory = Game1.player.Items;
                        }

                        this.craftingInfo = new CraftingInformationPage(this.xPositionOnScreen + this.width + this.xOffset, this.yPositionOnScreen, 400, this.height, this.backgroundColor, button, ref this.fromInventory,ref this.toInventory,this.playerInventory,this.machine);
                        Game1.soundBank.PlayCue("coin");
                        if (this.playerInventory)
                        {
                            Game1.player.Items = this.toInventory;
                            return;
                        }
                        //ModCore.log("Button has been clicked!");
                        return;
                    }
                }
            }
            //ModCore.log("Menu has been clicked");

            if (this.craftingInfo != null)
            {
                this.craftingInfo.receiveLeftClick(x, y);
                if (this.craftingInfo.doesMenuContainPoint(x, y)) return;
            }
            this.craftingInfo = null;
        }


        /// <summary>
        /// Draws the menu to the screen.
        /// </summary>
        /// <param name="b"></param>
        public override void draw(SpriteBatch b)
        {
            this.drawDialogueBoxBackground(this.xPositionOnScreen + this.xOffset, this.yPositionOnScreen, this.width, this.height, this.backgroundColor);

            if (this.currentPageIndex > this.maxPages) this.currentPageIndex = 0;

            this.leftButton.draw(b);
            //Draw page numbers here.
            //b.DrawString(Game1.smallFont,"Page: "+this.currentPageIndex.ToString()/)
            b.DrawString(Game1.dialogueFont, ("Page: " + (this.currentPageIndex + 1) + " / " + (this.maxPages + 1)).ToString(), new Vector2(this.xPositionOnScreen + 128, this.yPositionOnScreen+32), Color.White);
            this.rightButton.draw(b);

            this.searchBox.Draw(b, true);

            //this.drawDialogueBoxBackground();
            foreach (KeyValuePair<string, AnimatedButton> pair in this.CraftingTabs)
            {
                pair.Value.draw(b);
            }

            if (string.IsNullOrEmpty(this.currentTab))
            {
                if (string.IsNullOrEmpty(this.hoverText) == false)
                {
                    IClickableMenuExtended.drawHoverText(b, this.hoverText, Game1.dialogueFont);
                }
                this.drawMouse(b);
                return;
            }

            List<CraftingRecipeButton> buttonsToDraw = this.getRecipeButtonsToDisplay();

            foreach (CraftingRecipeButton button in buttonsToDraw)
            {
                if (button.recipe.CanCraft(this.fromInventory))
                {
                    button.draw(b);
                }
                else
                {
                    button.draw(b, .25f);
                }

                b.DrawString(Game1.smallFont, button.displayItem.item.DisplayName, button.displayItem.Position + new Vector2(64, 0), Color.Brown);
            }

            if (this.craftingInfo != null)
            {
                this.craftingInfo.draw(b);
            }

            if (string.IsNullOrEmpty(this.hoverText) == false)
            {
                IClickableMenuExtended.drawHoverText(b, this.hoverText, Game1.dialogueFont);
            }

            this.drawMouse(b);
        }

        /// <summary>
        /// Gets all of the crafting buttons to display.
        /// </summary>
        /// <returns></returns>
        private List<CraftingRecipeButton> getRecipeButtonsToDisplay()
        {
            List<CraftingRecipeButton> searchSelection;
            if (string.IsNullOrEmpty(this.searchBox.Text) == false)
            {
                searchSelection = this.craftingItemsToDisplay[this.currentTab].FindAll(i => i.displayItem.item.DisplayName.ToLowerInvariant().Contains(this.searchBox.Text.ToLowerInvariant()));
            }
            else
            {
                searchSelection = this.craftingItemsToDisplay[this.currentTab];
            }
            searchSelection = this.searchSort(searchSelection);

            int amount = searchSelection.Count / this.amountOfRecipesToShow;
            int min = this.currentPageIndex == amount ? searchSelection.Count % this.amountOfRecipesToShow : this.amountOfRecipesToShow;
            List<CraftingRecipeButton> buttonsToDraw = searchSelection.GetRange(this.currentPageIndex * this.amountOfRecipesToShow, min);
            return buttonsToDraw;
        }

        /// <summary>
        /// Repositions crafting buttons based on the current sort context.
        /// </summary>
        /// <param name="Unsorted"></param>
        /// <returns></returns>
        private List<CraftingRecipeButton> searchSort(List<CraftingRecipeButton> Unsorted)
        {
            List<CraftingRecipeButton> copy = Unsorted.ToList();
            //copy = copy.OrderBy(x => x.displayItem.item.DisplayName).ToList(); //Sort the recipes again. Probably unnecessary.

            //Do sorting;

            for (int i = 0; i < Unsorted.Count; i++)
            {
                int count = i % this.amountOfRecipesToShow;
                copy[i].displayItem.Position = new Vector2(this.xPositionOnScreen + (128), (this.yPositionOnScreen + 64) + (64 * (count + 1)));
            }
            return copy;
        }

    }
}
