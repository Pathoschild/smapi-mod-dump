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
using StardewValley;
using StardewValley.Menus;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using StardewValley.BellsAndWhistles;
using StardewValley.Objects;
using StardewModdingAPI;

namespace BCC.Menus
{
    class PantryMenu : InventoryMenu
    {
        public static List<Chest> CCFridgeChests = new List<Chest>();
        public bool Donated;
        public bool hasBeenAdded = false;

        public IModHelper Helper;
        public IMonitor Monitor;

        public static List<Item> ItemsDonated = new List<Item>();

        public static int Amount = 0;

        public PantryMenu(IModHelper helper, IMonitor monitor) : base(Game1.viewport.Width / 2 - 384, Game1.viewport.Height / 2 + 36, true, highlightMethod: new InventoryMenu.highlightThisItem(Util.IsEdibleCookableItem), capacity: 36)
        {
            Helper = helper;
            Monitor = monitor;
            exitFunction = (IClickableMenu.onExit)(() => Util.PantryExit(Donated));
        }
        public PantryMenu(List<Chest> fridgeList, IModHelper helper, IMonitor monitor) : base(Game1.viewport.Width / 2 - 384, Game1.viewport.Height / 2 + 36, true, highlightMethod: new InventoryMenu.highlightThisItem(Util.IsEdibleCookableItem), capacity: 36)
        {
            Helper = helper;
            Monitor = monitor;
            CCFridgeChests = fridgeList;
            exitFunction = (IClickableMenu.onExit)(() => Util.PantryExit(Donated));
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);
            Item itemXY = getItemAt(x, y);
            if (itemXY.Name != Util.previousDonatedName)
                Amount = 0;
            if (itemXY != null)
            {
                if (CCFridgeChests != null)
                {
                    if (!hasBeenAdded)
                    {
                        foreach (Chest chest in CCFridgeChests)
                        {
                            if (!hasBeenAdded)
                            {
                                if (Chest.capacity != chest.items.Count || Util.DoesChestHaveItem(itemXY))
                                {
                                    var oneItem = itemXY.getOne();
                                    chest.addItem(oneItem);
                                    hasBeenAdded = true;
                                    Amount += 1;
                                    ItemsDonated.Add(oneItem);
                                }
                            }
                        }
                    }
                }
                Donated = true;
                --itemXY.Stack;
                if (itemXY.Stack == 0)
                    Game1.player.removeItemFromInventory(itemXY);
                if (hasBeenAdded)
                {
                    hasBeenAdded = false;
                }
            }
            else
                return;
        }

        public override void draw(SpriteBatch b)
        {
            base.draw(b);
            if (!Game1.options.showMenuBackground)
                b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.4f);
            else
                drawBackground(b);
            string MenuTitle = Util.i18n.Get("PantryTitle");
            SpriteText.drawStringWithScrollCenteredAt(b, MenuTitle, Game1.viewport.Width / 2, Game1.viewport.Height / 2 - 128, MenuTitle);
            Game1.drawDialogueBox(xPositionOnScreen - 64, yPositionOnScreen - 160, width + 128, height + 192, false, true);
            base.draw(b);
            drawMouse(b);
        }
    }
}
