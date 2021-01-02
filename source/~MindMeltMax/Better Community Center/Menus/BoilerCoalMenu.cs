/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using BCC.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using StardewValley.Objects;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCC.Menus
{
    public class BoilerCoalMenu : InventoryMenu
    {
        public Chest CoalStorageChest;
        public IMonitor Monitor;
        public IModHelper Helper;
        public bool hasBeenAdded = false;
        public static List<Item> ItemsDonated = new List<Item>();

        public BoilerCoalMenu(Chest chest, IMonitor monitor, IModHelper helper) : base(Game1.viewport.Width / 2 - 384, Game1.viewport.Height / 2 + 36, true, highlightMethod:new InventoryMenu.highlightThisItem(Util.IsFuelItem), capacity: 36)
        {
            Helper = helper;
            Monitor = monitor;
            CoalStorageChest = chest;
            exitFunction = (IClickableMenu.onExit)(() => Util.CoalChestExit());
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);
            Item itemXY = getItemAt(x, y);
            if(itemXY != null)
            {
                if(itemXY.parentSheetIndex == 382 || (itemXY.parentSheetIndex == 388 && itemXY.Stack >= 20) || (itemXY.parentSheetIndex == 709 && itemXY.Stack >= 5))
                {
                    if (CoalStorageChest != null)
                    {
                        if (!hasBeenAdded)
                        {
                            if (Chest.capacity != CoalStorageChest.items.Count || Util.DoesChestHaveItem(itemXY))
                            {
                                if (itemXY.parentSheetIndex == 388)
                                {
                                    var newObject = new Object(382, 1);
                                    var item = newObject as Item;
                                    var oneItem = item.getOne();
                                    CoalStorageChest.addItem(oneItem);
                                    hasBeenAdded = true;
                                    ItemsDonated.Add(oneItem);
                                }
                                else if (itemXY.parentSheetIndex == 709)
                                {
                                    var newObject = new Object(382, 1);
                                    var item = newObject as Item;
                                    var oneItem = item.getOne();
                                    CoalStorageChest.addItem(oneItem);
                                    hasBeenAdded = true;
                                    ItemsDonated.Add(oneItem);
                                }
                                else
                                {
                                    var oneItem = itemXY.getOne();
                                    CoalStorageChest.addItem(oneItem);
                                    hasBeenAdded = true;
                                    ItemsDonated.Add(oneItem);
                                }
                            }
                        }
                    }
                }
                if (itemXY.parentSheetIndex == 382)
                {
                    --itemXY.Stack;
                    if (itemXY.Stack == 0)
                        Game1.player.removeItemFromInventory(itemXY);
                }
                else if(itemXY.parentSheetIndex == 388 && itemXY.Stack >= 20)
                {
                    itemXY.Stack -= 20;
                    if (itemXY.Stack == 0)
                        Game1.player.removeItemFromInventory(itemXY);
                }
                else if(itemXY.parentSheetIndex == 709 && itemXY.Stack >= 5)
                {
                    itemXY.Stack -= 5;
                    if (itemXY.Stack == 0)
                        Game1.player.removeItemFromInventory(itemXY);
                }
                if (hasBeenAdded)
                {
                    hasBeenAdded = false;
                }
            }
        }

        public override void draw(SpriteBatch b)
        {
            base.draw(b);
            if (!Game1.options.showMenuBackground)
                b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.4f);
            else
                drawBackground(b);
            string MenuTitle = Util.i18n.Get("CoalChestTitle");
            SpriteText.drawStringWithScrollCenteredAt(b, MenuTitle, Game1.viewport.Width / 2, Game1.viewport.Height / 2 - 128, MenuTitle);
            Game1.drawDialogueBox(xPositionOnScreen - 64, yPositionOnScreen - 160, width + 128, height + 192, false, true);
            base.draw(b);
            drawMouse(b);
        }
    }
}
