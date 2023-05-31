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
using Omegasis.Revitalize.Framework.Constants;
using Omegasis.Revitalize.Framework.Managers;
using Omegasis.Revitalize.Framework.World.WorldUtilities;
using Omegasis.StardustCore.Animations;
using Omegasis.StardustCore.UIUtilities;
using Omegasis.StardustCore.UIUtilities.MenuComponents.ComponentsV2.Buttons;
using StardewValley;
using StardewValley.Menus;

namespace Omegasis.Revitalize.Framework.Menus
{
    /// <summary>
    /// Used with transfering items between two inventories.
    /// 
    /// </summary>


    [Obsolete("Please use InventoryDisplayMenu instead.")]
    public class InventoryTransferMenu : IClickableMenuExtended
    {
        public InventoryMenu playerInventory;
        public InventoryMenu otherInventory;
        private IList<Item> otherItems;
        private bool isPlayerInventory;
        public AnimatedButton transferButton;
        public AnimatedButton trashButton;
        public string hoverText;

        public ItemDisplayButton trashedItem;
        private bool displayTrashedItem;

        private enum CurrentMode
        {
            TransferItems,
            TrashItem
        }
        private CurrentMode currentMode;

        public InventoryTransferMenu(int x, int y, int width, int height, IList<Item> OtherItems, int OtherCapacity, int OtherRows = 6, int OtherCollumns = 6) : base(x, y, width, height, true)
        {
            this.playerInventory = new InventoryMenu(x, y, width, height, 6, 6, true, Game1.player.Items, Game1.player.MaxItems, Color.SandyBrown);
            this.otherItems = OtherItems;
            this.otherInventory = new InventoryMenu(this.playerInventory.xPositionOnScreen + this.playerInventory.width + 128, y, width, height, OtherRows, OtherCollumns, true, this.otherItems, OtherCapacity, Color.SandyBrown);
            this.isPlayerInventory = true;
            this.currentMode = CurrentMode.TransferItems;
            this.transferButton = new AnimatedButton(new StardustCore.Animations.AnimatedSprite("Transfer Button", new Vector2(this.playerInventory.xPositionOnScreen + this.playerInventory.width + 64, this.playerInventory.yPositionOnScreen + this.playerInventory.height * .3f), new AnimationManager(TextureManagers.Menus_InventoryMenu.getExtendedTexture("ItemTransferButton"), new Animation(0, 0, 32, 32)), Color.White), new Rectangle(0, 0, 32, 32), 2f);
            this.trashButton = new AnimatedButton(new StardustCore.Animations.AnimatedSprite("Trash Button", new Vector2(this.playerInventory.xPositionOnScreen + this.playerInventory.width + 64, this.playerInventory.yPositionOnScreen + this.playerInventory.height * .3f + 96), new AnimationManager(TextureManagers.Menus_InventoryMenu.getExtendedTexture("TrashButton"), new Animation(0, 0, 32, 32)), Color.White), new Rectangle(0, 0, 32, 32), 2f);
            this.trashedItem = new ItemDisplayButton(null, new StardustCore.Animations.AnimatedSprite("ItemBackground", new Vector2(this.playerInventory.xPositionOnScreen + this.playerInventory.width + 64, this.playerInventory.yPositionOnScreen + this.playerInventory.height * .3f + 180), new AnimationManager(TextureManagers.Menus_InventoryMenu.getExtendedTexture("ItemBackground"), new Animation(0, 0, 32, 32)), Color.White), new Vector2(this.playerInventory.xPositionOnScreen + this.playerInventory.width + 64, this.playerInventory.yPositionOnScreen + this.playerInventory.height * .3f + 180), new Rectangle(0, 0, 32, 32), 2f, true, Color.White);
        }

        public override bool readyToClose()
        {
            return Game1.input.GetGamePadState().IsButtonDown(Microsoft.Xna.Framework.Input.Buttons.B) || Game1.input.GetKeyboardState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape) || Game1.input.GetGamePadState().IsButtonDown(Microsoft.Xna.Framework.Input.Buttons.Start);
        }

        public override void exitMenu(bool playSound = true)
        {
            base.exitMenu(playSound);
        }

        /// <summary>
        /// Handles what happens when the menu is left clicked.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="playSound"></param>
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (this.currentMode == CurrentMode.TransferItems)
            {
                if (this.otherInventory.isFull == false)
                {
                    this.playerInventory.receiveLeftClick(x, y);
                    if (this.playerInventory.activeItem != null)
                        this.transferItem(ref this.playerInventory, ref this.otherInventory);
                }
                if (this.playerInventory.isFull == false)
                {
                    this.otherInventory.receiveLeftClick(x, y);
                    if (this.otherInventory.activeItem != null)
                        this.transferItem(ref this.otherInventory, ref this.playerInventory);
                }
            }

            if (this.currentMode == CurrentMode.TrashItem)
            {
                this.playerInventory.receiveLeftClick(x, y);
                this.otherInventory.receiveLeftClick(x, y);
                if (this.playerInventory.activeItem != null)
                    this.trashItem(ref this.playerInventory);
                if (this.otherInventory.activeItem != null)
                    this.trashItem(ref this.otherInventory);
            }

            if (this.transferButton.receiveLeftClick(x, y))
            {
                this.currentMode = CurrentMode.TransferItems;
                SoundUtilities.PlaySound(Enums.StardewSound.shwip);
            }
            if (this.trashButton.receiveLeftClick(x, y))
            {
                this.currentMode = CurrentMode.TrashItem;
                SoundUtilities.PlaySound(Enums.StardewSound.shwip);
            }
            if (this.trashedItem.receiveLeftClick(x, y))
            {
                this.recoverTrashedItem();
                this.playerInventory.populateClickableItems();
                this.otherInventory.populateClickableItems();
            }
        }

        /// <summary>
        /// What happens when the menu is right clicked.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="playSound"></param>
        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            if (this.currentMode == CurrentMode.TransferItems)
            {
                if (this.otherInventory.isFull == false)
                {
                    this.playerInventory.receiveLeftClick(x, y);
                    if (this.playerInventory.activeItem != null)
                        this.transferOneItem(ref this.playerInventory, ref this.otherInventory);
                }
                if (this.playerInventory.isFull == false)
                {
                    this.otherInventory.receiveLeftClick(x, y);
                    if (this.otherInventory.activeItem != null)
                        this.transferOneItem(ref this.otherInventory, ref this.playerInventory);
                }
            }
        }

        /// <summary>
        /// Handles what happens when a menu is hover overed.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public override void performHoverAction(int x, int y)
        {
            this.playerInventory.performHoverAction(x, y);
            this.otherInventory.performHoverAction(x, y);

            if (this.transferButton.containsPoint(x, y))
                this.hoverText = "Transfer Items";
            else if (this.trashButton.containsPoint(x, y))
                this.hoverText = "Trash Items";
            else
                this.hoverText = "";

            if (this.trashedItem.ContainsPoint(x, y))
                if (this.trashedItem.item != null)
                    this.displayTrashedItem = true;
                else
                    this.displayTrashedItem = false;
            else
                this.displayTrashedItem = false;
        }

        /// <summary>
        /// Transfers an item between inventories.
        /// </summary>
        /// <param name="From"></param>
        /// <param name="To"></param>
        private void transferItem(ref InventoryMenu From, ref InventoryMenu To)
        {
            //Stack size control logic.
            foreach (Item I in To.items)
            {
                if (I == null) continue;
                if (From.activeItem.canStackWith(I))
                {
                    I.addToStack(From.activeItem);
                    From.items.Remove(From.activeItem);
                    From.activeItem = null;
                    From.populateClickableItems();
                    To.populateClickableItems();
                    return;
                }
                else if (I.maximumStackSize() > I.Stack + From.activeItem.Stack && From.activeItem.canStackWith(I))
                {
                    int sizeLeft = I.getRemainingStackSpace();
                    I.Stack = I.maximumStackSize();
                    From.activeItem.Stack -= sizeLeft;

                    break;
                }
            }
            if (To.isFull == false)
            {
                //
                bool addedItem = false;
                for (int i = 0; i < To.items.Count; i++)
                    if (To.items[i] == null)
                    {
                        To.items[i] = From.activeItem;
                        addedItem = true;
                        break;
                    }
                if (addedItem == false)
                    To.items.Add(From.activeItem);

                From.items.Remove(From.activeItem);
                From.activeItem = null;
                From.populateClickableItems();
                To.populateClickableItems();
            }
        }

        /// <summary>
        /// Transfers exactly one item across inventories.
        /// </summary>
        /// <param name="From"></param>
        /// <param name="To"></param>
        private void transferOneItem(ref InventoryMenu From, ref InventoryMenu To)
        {
            //Stack size control logic.
            foreach (Item I in To.items)
            {
                if (I == null) continue;
                if (From.activeItem.canStackWith(I))
                {
                    //I.addToStack(I);
                    I.Stack++;
                    From.activeItem.Stack--;
                    if (From.activeItem.Stack <= 0)
                        From.items.Remove(From.activeItem);
                    From.activeItem = null;
                    From.populateClickableItems();
                    To.populateClickableItems();
                    return;
                }
            }
            if (To.isFull == false)
            {
                To.items.Add(From.activeItem.getOne());
                From.activeItem.Stack--;
                if (From.activeItem.Stack <= 0)
                    From.items.Remove(From.activeItem);
                From.activeItem = null;
                From.populateClickableItems();
                To.populateClickableItems();
            }
        }


        /// <summary>
        /// Puts an item into the trash.
        /// </summary>
        /// <param name="From"></param>
        private void trashItem(ref InventoryMenu From)
        {
            if (From.activeItem != null)
            {
                this.trashedItem.item = From.activeItem;
                From.items.Remove(From.activeItem);
                From.activeItem = null;
                From.populateClickableItems();
            }
        }

        /// <summary>
        /// Trashes a single item from the given inventory.
        /// </summary>
        /// <param name="From"></param>
        private void transhOneItem(ref InventoryMenu From)
        {
            if (From.activeItem != null)
                if (this.trashedItem.item == null)
                {
                    this.trashedItem.item = From.activeItem.getOne();
                    From.activeItem.Stack--;
                    if (From.activeItem.Stack == 0)
                        From.items.Remove(From.activeItem);
                    From.activeItem = null;
                    From.populateClickableItems();
                }
                else if (this.trashedItem.item != null)
                    if (From.activeItem.canStackWith(this.trashedItem.item))
                    {
                        this.trashedItem.item.Stack += 1;
                        From.activeItem.Stack--;
                        if (From.activeItem.Stack == 0)
                            From.items.Remove(From.activeItem);
                        From.activeItem = null;
                        From.populateClickableItems();
                        return;
                    }
                    else
                    {
                        this.trashedItem.item = From.activeItem.getOne();
                        From.activeItem.Stack--;
                        if (From.activeItem.Stack == 0)
                            From.items.Remove(From.activeItem);
                        From.activeItem = null;
                        From.populateClickableItems();
                        return;
                    }
        }

        /// <summary>
        /// Tries to recover the previously trashed item.
        /// </summary>
        private void recoverTrashedItem()
        {
            if (this.trashedItem.item != null)
                if (this.playerInventory.isFull == false)
                {
                    foreach (Item I in this.playerInventory.items)
                    {
                        if (I == null) continue;
                        if (this.trashedItem.item.canStackWith(I))
                        {
                            I.addToStack(this.trashedItem.item);
                            this.trashedItem.item = null;
                            return;
                        }
                        else if (I.maximumStackSize() > I.Stack + this.trashedItem.item.Stack && this.trashedItem.item.canStackWith(I))
                        {
                            int sizeLeft = I.getRemainingStackSpace();
                            I.Stack = I.maximumStackSize();
                            this.trashedItem.item.Stack -= sizeLeft;
                            break;
                        }
                    }
                    if (this.playerInventory.isFull == false)
                    {
                        this.playerInventory.items.Add(this.trashedItem.item);
                        this.trashedItem.item = null;
                        return;
                    }
                    else if (this.otherInventory.isFull == false)
                    {
                        foreach (Item I in this.otherInventory.items)
                        {
                            if (I == null) continue;
                            if (this.trashedItem.item.canStackWith(I))
                            {
                                I.addToStack(this.trashedItem.item);
                                this.trashedItem.item = null;
                                return;
                            }
                            else if (I.maximumStackSize() > I.Stack + this.trashedItem.item.Stack && this.trashedItem.item.canStackWith(I))
                            {
                                int sizeLeft = I.getRemainingStackSpace();
                                I.Stack = I.maximumStackSize();
                                this.trashedItem.item.Stack -= sizeLeft;

                                break;
                            }
                        }
                        if (this.otherInventory.isFull == false)
                        {
                            this.otherInventory.items.Add(this.trashedItem.item);
                            this.trashedItem.item = null;
                            return;
                        }
                    }
                }
                else if (this.otherInventory.isFull == false)
                {
                    foreach (Item I in this.otherInventory.items)
                    {
                        if (I == null) continue;
                        if (this.trashedItem.item.canStackWith(I))
                        {
                            I.addToStack(this.trashedItem.item);
                            this.trashedItem.item = null;
                            return;
                        }
                        else if (I.maximumStackSize() > I.Stack + this.trashedItem.item.Stack && this.trashedItem.item.canStackWith(I))
                        {
                            int sizeLeft = I.getRemainingStackSpace();
                            I.Stack = I.maximumStackSize();
                            this.trashedItem.item.Stack -= sizeLeft;

                            break;
                        }
                    }
                    if (this.otherInventory.isFull == false)
                    {
                        this.otherInventory.items.Add(this.trashedItem.item);
                        this.trashedItem.item = null;
                        return;
                    }

                }
        }

        /// <summary>
        /// Draws 
        /// </summary>
        /// <param name="b"></param>
        public override void draw(SpriteBatch b)
        {
            this.playerInventory.draw(b);
            this.otherInventory.draw(b);

            this.transferButton.draw(b, 1f, this.currentMode == CurrentMode.TransferItems ? 1f : .4f);
            this.trashButton.draw(b, 1f, this.currentMode == CurrentMode.TrashItem ? 1f : .4f);
            this.trashedItem.draw(b, 0.25f, this.currentMode == CurrentMode.TrashItem || this.trashedItem.item != null ? 1f : .4f, false);
            if (this.hoverText != null)
                drawHoverText(b, this.hoverText, Game1.dialogueFont);
            //To prevent awkward overlap from the other menu.
            if (this.playerInventory.hoverText != null)
                this.playerInventory.drawToolTip(b);

            this.drawToolTip(b);
            this.drawMouse(b);
        }

        public void drawToolTip(SpriteBatch b)
        {
            if (this.displayTrashedItem && this.trashedItem.item != null) drawToolTip(b, this.trashedItem.item.getDescription(), this.trashedItem.item.DisplayName, this.trashedItem.item, false, -1, 0, -1, -1, null, -1);
        }


        public void updateInventory()
        {
            this.playerInventory.populateClickableItems();
            this.otherInventory.populateClickableItems();
        }
    }
}
