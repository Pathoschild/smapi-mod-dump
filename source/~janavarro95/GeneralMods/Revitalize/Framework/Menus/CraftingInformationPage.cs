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
using StardewValley;
using Omegasis.Revitalize.Framework.Constants;
using Omegasis.Revitalize.Framework.Crafting;
using Omegasis.Revitalize.Framework.Menus.MenuComponents;
using Omegasis.Revitalize.Framework.Utilities;
using Omegasis.Revitalize.Framework.World.Objects.Machines;
using Omegasis.StardustCore.UIUtilities;
using Omegasis.StardustCore.UIUtilities.MenuComponents.ComponentsV2.Buttons;
using Omegasis.Revitalize.Framework.World.WorldUtilities;
using ObjectUtilities = Omegasis.Revitalize.Framework.Utilities.ObjectUtilities;
using Omegasis.Revitalize.Framework.Managers;
using StardewValley.Menus;

namespace Omegasis.Revitalize.Framework.Menus
{
    /// <summary>
    /// Also need to make the crafting menu scroll better.
    /// </summary>
    public class CraftingInformationPage : IClickableMenuExtended
    {

        public CraftingRecipeButton infoButton;
        public Color backgroundColor;

        public Vector2 itemDisplayLocation;

        public IList<Item> inventory;
        public IList<Item> outputInventory;

        private Dictionary<ItemDisplayButton, int> requiredItems;

        public AnimatedButton craftingButton;
        public AnimatedButton goldButton;

        public bool isPlayerInventory;

        private Machine machine;

        string hoverText;

        public ItemDisplayButton currentHoverOverItemComponent;

        public Item actualItem
        {
            get
            {
                return this.infoButton.displayItem.item;
            }
        }

        public CraftingInformationPage() : base()
        {

        }

        public CraftingInformationPage(int x, int y, int width, int height, Color BackgroundColor, CraftingRecipeButton ItemToDisplay, ref IList<Item> Inventory, bool IsPlayerInventory) : base(x, y, width, height, false)
        {
            this.initializeMenu(BackgroundColor,ItemToDisplay,ref Inventory,IsPlayerInventory);

            this.outputInventory = this.inventory;

            if (this.infoButton.recipe.statCost != null)
                if (this.infoButton.recipe.statCost.gold > 0)
                    this.goldButton = new AnimatedButton(new StardustCore.Animations.AnimatedSprite("GoldButton", this.getMoneyRequiredOffset(), TextureManagers.Menus_CraftingMenu.createAnimationManager("GoldButton", new StardustCore.Animations.Animation(0, 0, 16, 16)), Color.White), new Rectangle(0, 0, 16, 16), 2f);
        }

        public CraftingInformationPage(int x, int y, int width, int height, Color BackgroundColor, CraftingRecipeButton ItemToDisplay, ref IList<Item> Inventory, ref IList<Item> OutputInventory, bool IsPlayerInventory, Machine Machine) : base(x, y, width, height, false)
        {
            this.initializeMenu(BackgroundColor, ItemToDisplay, ref Inventory, IsPlayerInventory);
            if (OutputInventory == null)
                this.outputInventory = this.inventory;
            if (this.infoButton.recipe.statCost != null)
                if (this.infoButton.recipe.statCost.gold > 0)
                    this.goldButton = new AnimatedButton(new StardustCore.Animations.AnimatedSprite("GoldButton", this.getMoneyRequiredOffset(), TextureManagers.Menus_CraftingMenu.createAnimationManager("GoldButton", new StardustCore.Animations.Animation(0, 0, 16, 16)), Color.White), new Rectangle(0, 0, 16, 16), 2f);
            this.outputInventory = OutputInventory;
            this.machine = Machine;
        }

        protected virtual void initializeMenu(Color BackgroundColor, CraftingRecipeButton ItemToDisplay, ref IList<Item> Inventory, bool IsPlayerInventory)
        {
            this.backgroundColor = BackgroundColor;
            this.infoButton = ItemToDisplay;
            this.itemDisplayLocation = new Vector2(this.xPositionOnScreen + this.width / 2 - 32, this.yPositionOnScreen + 128);
            this.inventory = Inventory;
            this.isPlayerInventory = IsPlayerInventory;

            this.requiredItems = new Dictionary<ItemDisplayButton, int>();
            for (int i = 0; i < this.infoButton.recipe.ingredients.Count; i++)
            {
                ItemDisplayButton b = new ItemDisplayButton(this.infoButton.recipe.ingredients.ElementAt(i).item, null, new Vector2(this.xPositionOnScreen + 64 + this.width, this.yPositionOnScreen + i * 64 + 128), new Rectangle(0, 0, 32, 32), 2f, true, Color.White);
                this.requiredItems.Add(b, this.infoButton.recipe.ingredients.ElementAt(i).requiredAmount);
            }
            this.craftingButton = new AnimatedButton(new StardustCore.Animations.AnimatedSprite("CraftingButton", new Vector2(this.xPositionOnScreen + this.width / 2 - 96, this.getCraftingButtonHeight()), TextureManagers.Menus_CraftingMenu.createAnimationManager("CraftButton", new StardustCore.Animations.Animation(0, 0, 48, 16)), Color.White), new Rectangle(0, 0, 48, 16), 4f);

        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (this.craftingButton.containsPoint(x, y))
                if (this.canCraftRecipe())
                {
                    SoundUtilities.PlaySound(Enums.StardewSound.coin);

                    if (this.isPlayerInventory)
                        this.infoButton.craftItem();
                    else
                        this.infoButton.craftItem(this.inventory, this.outputInventory);
                    if (this.machine != null)
                        if (this.infoButton.recipe.timeToCraft == 0)
                            this.machine.GetInventoryManager().dumpBufferToItems();
                        else
                            this.machine.MinutesUntilReady = this.infoButton.recipe.timeToCraft;

                    if (this.isPlayerInventory)
                        this.inventory = Game1.player.Items;
                }
        }

        public override void performHoverAction(int x, int y)
        {
            bool hovered = false;
            if (this.craftingButton.containsPoint(x, y))
            {
                if (this.infoButton.recipe.CanCraft(this.inventory) == false)
                {
                    this.hoverText = "Not enough items.";
                    hovered = true;
                }
                if (this.machine != null)
                {
                    if (this.machine.MinutesUntilReady > 0)
                    {
                        this.hoverText = "Crafting in progress...";
                        hovered = true;
                    }
                    if (this.machine.MinutesUntilReady == 0 && this.machine.GetInventoryManager().hasItemsInBuffer())
                    {
                        this.hoverText = "Items in buffer. Please make room in the inventory for: " + System.Environment.NewLine + this.machine.GetInventoryManager() + " items.";
                        hovered = true;
                    }
                    if (this.machine.GetInventoryManager().isFull())
                    {
                        this.hoverText = "Inventory is full!";
                        hovered = true;
                    }
                }
            }

            foreach(ItemDisplayButton itemDisplayButton in this.requiredItems.Keys)
            {
                if (itemDisplayButton.ContainsPoint(x, y))
                {
                    this.currentHoverOverItemComponent = itemDisplayButton;
                    hovered = true;
                }
            }

            if (hovered == false)
            {
                this.currentHoverOverItemComponent = null;
                this.hoverText = "";
                
            }
        }

        public override void draw(SpriteBatch b)
        {
            this.drawDialogueBoxBackground(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, this.backgroundColor);
            this.drawDialogueBoxBackground(this.xPositionOnScreen + this.width, this.yPositionOnScreen, this.width, this.height, this.backgroundColor);
            this.infoButton.draw(b, this.itemDisplayLocation);

            b.DrawString(Game1.dialogueFont, this.actualItem.DisplayName, new Vector2(this.xPositionOnScreen + this.width / 2, this.itemDisplayLocation.Y) + this.getHeightOffsetFromItem() - this.getItemNameOffset(), this.getNameColor());

            b.DrawString(Game1.smallFont, Game1.parseText(this.actualItem.getDescription(), Game1.smallFont, this.width), new Vector2(this.xPositionOnScreen + 64, this.getItemDescriptionOffset().Y), Color.Black);

            foreach (KeyValuePair<ItemDisplayButton, int> button in this.requiredItems)
            {
                button.Key.draw(b);
                b.DrawString(Game1.smallFont, button.Key.item.DisplayName + " x " + button.Value.ToString(), button.Key.Position + new Vector2(64, 16), this.getNameColor(button.Key.item, button.Value));
            }
            if (this.goldButton != null)
            {
                this.goldButton.draw(b);
                b.DrawString(Game1.smallFont, this.infoButton.recipe.statCost.gold + " G", this.goldButton.Position + new Vector2(0, 32), Color.Black);
            }

            this.craftingButton.draw(b, this.getCraftableColor().A);

            if (string.IsNullOrEmpty(this.hoverText) == false)
                drawHoverText(b, this.hoverText, Game1.dialogueFont);

            if (this.currentHoverOverItemComponent != null)
            {
                IClickableMenu.drawToolTip(b, this.currentHoverOverItemComponent.item.getDescription(), this.currentHoverOverItemComponent.item.DisplayName, this.currentHoverOverItemComponent.item, false);
            }

            this.drawMouse(b);
        }

        public bool doesMenuContainPoint(int x, int y)
        {
            Rectangle r = new Rectangle(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height);
            return r.Contains(x, y);
        }

        public bool canCraftRecipe()
        {
            bool canCraft = true;
            if (this.infoButton.recipe.CanCraft(this.inventory) == false) canCraft = false;

            if (this.machine != null)
            {
                if (this.machine.GetInventoryManager().hasItemsInBuffer()) canCraft = false;
                if (this.machine.GetInventoryManager().isFull()) canCraft = false;
            }

            return canCraft;
        }

        /// <summary>
        /// Gets the color for the crafting button.
        /// </summary>
        /// <returns></returns>
        private Color getCraftableColor()
        {
            if (this.canCraftRecipe()) return Color.White;
            else return new Color(1f, 1f, 1f, 0.25f);
        }

        public Color getNameColor()
        {
            if (this.canCraftRecipe()) return Color.Black;
            else return Color.Red;
        }

        public Color getNameColor(Item I, int amount)
        {
            CraftingRecipeComponent Pair = new CraftingRecipeComponent(I, amount);

            if (this.infoButton.recipe.InventoryContainsIngredient(this.inventory, Pair))
                return Color.Black;
            else
                return Color.Red;

        }

        private Vector2 getHeightOffsetFromItem()
        {
            if (TypeUtilities.IsSameType(typeof(StardewValley.Object), this.actualItem.GetType()))
                return new Vector2(0, 64f);

            return new Vector2(0, 64f);
        }

        private Vector2 getItemNameOffset()
        {
            Vector2 length = Game1.dialogueFont.MeasureString(this.actualItem.DisplayName);
            length.X = length.X / 2;
            length.Y = 0;
            return length;
        }

        private Vector2 getItemDescriptionOffset()
        {
            return this.getHeightOffsetFromItem() + new Vector2(0, 64) + new Vector2(0, this.itemDisplayLocation.Y);
        }

        /// <summary>
        /// Gets the height position for where to draw a required ingredient.
        /// </summary>
        /// <returns></returns>
        private Vector2 getMoneyRequiredOffset()
        {
            return new Vector2(this.xPositionOnScreen + 64 + this.width, this.yPositionOnScreen + 128);
        }

        private float getCraftingButtonHeight()
        {
            return this.yPositionOnScreen + this.height - 64 * 2;
        }

    }
}
