/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using ChestDisplays.Patches;
using ChestDisplays.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChestDisplays
{
    public class ChangeDisplayMenu : MenuWithInventory
    {
        private const int BaseIdInventory = 36000;
        private const int OkButtonId = 35999;
        private const int DisplaySlotId = 69420; //Heh. Funny number
        private const int NameBoxId = 42069;

        private readonly Chest editingChest;
        private ModData? data;
        private ClickableTextureComponent displaySlot;
        private bool finishedInitializing = false;
        private Rectangle entryBackgroundBounds;
        private TextBox nameBox;
        private ClickableComponent nameBoxComponent;

        private IModHelper _helper => ModEntry.IHelper;
        private IMonitor _monitor => ModEntry.IMonitor;

        public ChangeDisplayMenu(Chest c, string tempText = "") : base(i => i is not null, true, menuOffsetHack: 64)
        {
            editingChest = c;
            data = c.modData.ContainsKey(_helper.ModRegistry.ModID) ? JsonConvert.DeserializeObject<ModData>(c.modData[_helper.ModRegistry.ModID]) : null;
            loadViewComponents();
            if (!string.IsNullOrWhiteSpace(tempText))
                nameBox!.Text = tempText;
            if (string.IsNullOrWhiteSpace(nameBox!.Text))
            {
                if (c.modData.TryGetValue("furyx639.BetterChests/StorageName", out string betterChestsName) && !string.IsNullOrWhiteSpace(betterChestsName))
                    nameBox.Text = betterChestsName;
                else if (c.modData.TryGetValue("Pathoschild.ChestsAnywhere/Name", out string chestsAnywhereName) && !string.IsNullOrWhiteSpace(chestsAnywhereName))
                    nameBox.Text = chestsAnywhereName;
            }
            snapToDefaultClickableComponent();
        }

        public override void update(GameTime time)
        {
            base.update(time);
            nameBox?.Update();
        }

        public override void receiveKeyPress(Keys key)
        {
            if (Game1.options.menuButton.Contains(new(key)) && nameBox.Selected)
                return;
            if (Game1.options.menuButton.Contains(new(key)))
            {
                updateModData(Utils.BuildItemFromData(data));
                exitThisMenu();
            }
            else
                base.receiveKeyPress(key);
        }

        public override void snapToDefaultClickableComponent() => currentlySnappedComponent = getComponentWithID(BaseIdInventory);

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds) => Game1.activeClickableMenu = new ChangeDisplayMenu(editingChest, nameBox.Text);

        public override void setCurrentlySnappedComponentTo(int id)
        {
            currentlySnappedComponent = getComponentWithId(id);
            if (currentlySnappedComponent == null)
            {
                _monitor.Log($"Couldn't snap to component with id : {id}, Snapping to default", LogLevel.Warn);
                snapToDefaultClickableComponent();
            }
            Game1.playSound("smallSelect");
        }

        public override void setUpForGamePadMode()
        {
            snapToDefaultClickableComponent();
            snapCursorToCurrentSnappedComponent();
        }

        public override void applyMovementKey(int direction)
        {
            if (currentlySnappedComponent == null) snapToDefaultClickableComponent();
            switch (direction)
            {
                case 0: //Up
                    if (currentlySnappedComponent!.upNeighborID < 0) goto default;
                    setCurrentlySnappedComponentTo(currentlySnappedComponent.upNeighborID);
                    snapCursorToCurrentSnappedComponent();
                    break;
                case 1: //Right
                    if (currentlySnappedComponent!.rightNeighborID < 0) goto default;
                    setCurrentlySnappedComponentTo(currentlySnappedComponent.rightNeighborID);
                    snapCursorToCurrentSnappedComponent();
                    break;
                case 2: //Down
                    if (currentlySnappedComponent!.downNeighborID < 0) goto default;
                    setCurrentlySnappedComponentTo(currentlySnappedComponent.downNeighborID);
                    snapCursorToCurrentSnappedComponent();
                    break;
                case 3: //Left
                    if (currentlySnappedComponent!.leftNeighborID < 0) goto default;
                    setCurrentlySnappedComponentTo(currentlySnappedComponent.leftNeighborID);
                    snapCursorToCurrentSnappedComponent();
                    break;
                default:
                    base.applyMovementKey(direction);
                    break;
            }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true) => receiveRightClick(x, y, playSound);

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            if (!finishedInitializing) return;
            
            if (inventory.isWithinBounds(x, y))
            {
                Item? item = inventory.getItemAt(x, y);
                if (item is null) 
                    return;
                displaySlot.item = item.getOne();
                updateModData(item);
            }
            else if (displaySlot.bounds.Contains(x, y) && displaySlot.item is not null)
            {
                //editingChest.modData.Remove(_helper.ModRegistry.ModID);
                //data = null;
                displaySlot.item = null;
                updateModData(null);
                //Utils.updateCache(editingChest);
            }
            else if (okButton.bounds.Contains(x, y))
            {
                updateModData(Utils.BuildItemFromData(data));
                exitThisMenu();
            }
        }

        public override void performHoverAction(int x, int y)
        {
            hoveredItem = null;
            inventory.hover(x, y, null);

            foreach (var slot in inventory.inventory)
                if (slot.containsPoint(x, y))
                    hoveredItem = inventory.actualInventory.ElementAtOrDefault(inventory.inventory.IndexOf(slot));

            if (displaySlot.containsPoint(x, y))
                hoveredItem = displaySlot.item;

            nameBox?.Hover(x, y);

            base.performHoverAction(x, y);
        }

        public override void draw(SpriteBatch b)
        {
            b.Draw(Game1.fadeToBlackRect, new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height), Color.Black * 0.6f);
            draw(b, false, false);

            //These three things just to have the little backpack icon... AGAIN!!!
            b.Draw(Game1.mouseCursors, new Vector2((xPositionOnScreen - 64), (yPositionOnScreen + height / 2 + 64 + 16)), new Rectangle?(new Rectangle(16, 368, 12, 16)), Color.White, 4.712389f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
            b.Draw(Game1.mouseCursors, new Vector2((xPositionOnScreen - 64), (yPositionOnScreen + height / 2 + 64 - 16)), new Rectangle?(new Rectangle(21, 368, 11, 16)), Color.White, 4.712389f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
            b.Draw(Game1.mouseCursors, new Vector2((xPositionOnScreen - 40), (yPositionOnScreen + height / 2 + 64 - 44)), new Rectangle?(new Rectangle(4, 372, 8, 11)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);

            Game1.drawDialogueBox(entryBackgroundBounds.X, entryBackgroundBounds.Y, entryBackgroundBounds.Width, entryBackgroundBounds.Height, false, true);
            b.Draw(Game1.menuTexture, new Vector2(entryBackgroundBounds.X + 12, entryBackgroundBounds.Y + entryBackgroundBounds.Height - 32), new(128, 16, 16, 16), Color.White, 0f, Vector2.Zero, new Vector2(20f, 1f), SpriteEffects.None, 1f);

            displaySlot.draw(b);
            if (displaySlot.item is not null)
            {
                var pos = new Vector2(displaySlot.bounds.X + (displaySlot.bounds.Width / 2) - 32, displaySlot.bounds.Y + (displaySlot.bounds.Height / 2) - 32);
                displaySlot.item.drawInMenu(b, pos, .75f, 1f, Utils.GetDepthFromItemType(Utils.getItemType(displaySlot.item), (int)pos.X, (int)pos.Y));
            }
            nameBox?.Draw(b, false);

            if (hoveredItem is not null)
                drawToolTip(b, hoveredItem.getDescription(), hoveredItem.DisplayName, hoveredItem);

            Game1.mouseCursorTransparency = 1f;
            drawMouse(b);
            if (!finishedInitializing) finishedInitializing = true;
        }

        private void loadViewComponents()
        {
            inventory.showGrayedOutSlots = true;

            entryBackgroundBounds = new Rectangle(inventory.xPositionOnScreen + (inventory.width / 2 - 142), inventory.yPositionOnScreen - 344, 344, 344);
            displaySlot = new(new Rectangle(entryBackgroundBounds.Center.X - 60, entryBackgroundBounds.Center.Y - 36, 120, 120), Game1.menuTexture, new Rectangle(0, 256, 60, 60), 2f)
            {
                item = Utils.BuildItemFromData(data),
                myID = DisplaySlotId,
                leftNeighborID = -1,
                rightNeighborID = OkButtonId,
                upNeighborID = -1,
                downNeighborID = NameBoxId,
            };
            nameBox = new(Game1.content.Load<Texture2D>("LooseSprites\\textBox"), Game1.staminaRect, Game1.smallFont, Game1.textColor)
            {
                X = displaySlot.bounds.X - 40,// - (nameBox?.Width ?? 0 - displaySlot.bounds.Width) / 2,
                Y = displaySlot.bounds.Y + displaySlot.bounds.Height + 4,
                Text = data?.Name
            };
            nameBoxComponent = new(new(nameBox.X, nameBox.Y, nameBox.Width, nameBox.Height), "Gerald")
            {
                myID = NameBoxId,
                leftNeighborID = -1,
                rightNeighborID = -1,
                upNeighborID = DisplaySlotId,
                visible = true,
            };

            okButton.myID = OkButtonId;

            const int rowLength = 12;
            int colLength = inventory.inventory.Count / rowLength;

            for (int c = 0; c < colLength; c++)
            {
                for (int r = 0; r < rowLength; r++)
                {
                    int index = r + (rowLength * c);
                    inventory.inventory[index].myID = BaseIdInventory + index;

                    if (c != 0) 
                        inventory.inventory[index].upNeighborID = BaseIdInventory + index - rowLength;
                    else 
                        inventory.inventory[index].upNeighborID = NameBoxId;

                    if (c != (colLength - 1)) 
                        inventory.inventory[index].downNeighborID = BaseIdInventory + index + rowLength;
                    else 
                        inventory.inventory[index].downNeighborID = -1;

                    if (r != 0) 
                        inventory.inventory[index].leftNeighborID = BaseIdInventory + index - 1;
                    else 
                        inventory.inventory[index].leftNeighborID = -1;

                    if (r != (rowLength - 1)) 
                        inventory.inventory[index].rightNeighborID = BaseIdInventory + index + 1;
                    else 
                        inventory.inventory[index].rightNeighborID = -1;

                    if (r == (rowLength - 1) && c == (colLength - 1))
                    {
                        inventory.inventory[index].rightNeighborID = OkButtonId;
                        okButton.leftNeighborID = inventory.inventory[index].myID;
                    }

                    if (c == 0 && r == (rowLength / 2))
                        nameBoxComponent.downNeighborID = inventory.inventory[index].myID;
                }
            }

            allClickableComponents =
            [
                .. inventory.inventory,
            ];
        }

        private void updateModData(Item? item)
        {
            data = new()
            {
                ItemId = item?.QualifiedItemId,
                Color = item is ColoredObject co ? co.color.Value : null,
                ItemQuality = item?.Quality ?? -1,
                UpgradeLevel = item is Tool tu ? tu.UpgradeLevel : -1,
                Name = nameBox.Text
            };
            if (string.IsNullOrWhiteSpace(data.ItemId) && string.IsNullOrWhiteSpace(data.Name))
            {
                editingChest.modData.Remove(_helper.ModRegistry.ModID);
                Utils.updateCache(editingChest);
                return;
            }
            editingChest.modData[_helper.ModRegistry.ModID] = JsonConvert.SerializeObject(data);
            Utils.updateCache(editingChest);
        }

        private ClickableComponent? getComponentWithId(int id)
        {
            if (id == NameBoxId)
                return nameBoxComponent;
            if (id == DisplaySlotId)
                return displaySlot;
            if (id == OkButtonId)
                return okButton;
            return getComponentWithID(id);
        }
    }
}
