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
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI.Events;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using StardewValley.Objects;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace BCC.Menus
{
    public class BoilerMenu : IClickableMenu
    {
        public IMonitor Monitor;
        public IModHelper Helper;

        public static bool isSmelting;

        public string Title;

        public int BoxCount;
        public int activeCount = 0;

        public List<KeyValuePair<int, DelayedAction>> kvp = new List<KeyValuePair<int, DelayedAction>>();

        public static Chest connectedChest;

        public InventoryMenu inventoryMenu;

        public static List<Item> inventory = new List<Item>();

        public InventoryMenu.highlightThisItem inventoryHighlighter;

        public ItemGrabMenu.behaviorOnItemSelect itemSelectBehavior;

        public static List<ClickableComponent> inputComponents = new List<ClickableComponent>();
        public static List<ClickableComponent> outputComponents = new List<ClickableComponent>();
        public List<ClickableComponent> allComponents = new List<ClickableComponent>();

        public List<CKeyValPair<int, int>> smeltTimes = new List<CKeyValPair<int, int>>();

        public BoilerMenu()
        {
        }

        public BoilerMenu(IMonitor monitor, IModHelper helper, Chest coalStorageObject, int boxCount = 3) : base(0, 0, 0, 0, true)
        {
            Monitor = monitor;
            Helper = helper;
            Title = Util.i18n.Get("BoilerTitle");
            BoxCount = boxCount;
            connectedChest = coalStorageObject;
            inventoryHighlighter = new InventoryMenu.highlightThisItem(Util.IsStackedSmeltableItem);

            width = 800 + borderWidth * 2;
            height = 600 + borderWidth * 2;
            xPositionOnScreen = Game1.viewport.Width / 2 - (800 + borderWidth * 2) / 2;
            yPositionOnScreen = Game1.viewport.Height / 2 - (600 + borderWidth * 2) / 2;

            this.exitFunction = new onExit(Util.BoilerExit);

            inventoryMenu = new InventoryMenu(xPositionOnScreen + spaceToClearSideBorder + borderWidth, yPositionOnScreen + spaceToClearTopBorder + borderWidth + 320 - 16, false, highlightMethod: inventoryHighlighter);
            inventoryMenu.showGrayedOutSlots = true;

            upperRightCloseButton = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width - 30, Game1.viewport.Height / 2 - 296, 48, 48), Game1.mouseCursors, new Rectangle(337, 494, 12, 12), 4f);
            int measureComp1 = 24;
            int measureComp2 = 24;
            int indexer = 0;
            for (int i = 0; i < boxCount; ++i)
            {
                ClickableComponent component = new ClickableComponent(new Rectangle(xPositionOnScreen + 120, yPositionOnScreen + 96 + measureComp1, 64, 64), "")
                {
                    myID = i,
                    downNeighborID = i == 2 ? -7777 : i - 1,
                    upNeighborID = i == 0 ? -7777 : i + 1
                };
                inputComponents.Add(component);
                measureComp1 += 96;
                indexer++;
            }
            for (int i = 0; i < boxCount; ++i)
            {
                ClickableComponent component = new ClickableComponent(new Rectangle(xPositionOnScreen + width - 168, yPositionOnScreen + 96 + measureComp2, 64, 64), "")
                {
                    myID = i + boxCount,
                    downNeighborID = i == 2 ? -7777 : i - 1,
                    upNeighborID = i == 0 ? -7777 : i + 1
                };
                outputComponents.Add(component);
                measureComp2 += 96;
                indexer++;
            }
            foreach (ClickableComponent obj in inputComponents)
                allComponents.Add(obj);
            foreach (ClickableComponent obj in outputComponents)
                allComponents.Add(obj);

            List<Item> list = checkContainerContents();
            inventoryMenu.populateClickableComponentList();

            if (BoilerData.dataList != null)
            {
                foreach (data data in BoilerData.dataList)
                {
                    Item temp = (Item)new Object(data.ParentSheetIndex, data.Stack);
                    inventory.Add(temp);
                    if (outputComponents != null)
                    {
                        foreach (ClickableComponent c in outputComponents)
                        {
                            if (c.myID == data.HoldingComponentID)
                                c.item = temp;
                        }
                    }
                    else if(inputComponents != null)
                    {
                        foreach(ClickableComponent c in inputComponents)
                        {
                            if (c.myID == data.HoldingComponentID)
                                c.item = temp;
                        }
                    }
                }
            }

            BCC.RequestableHelper.Events.GameLoop.DayEnding += GameLoop_DayEnding_Boiler;
        }

        private void GameLoop_UpdateTicked_Boiler(object sender, UpdateTickedEventArgs e)
        {
            foreach(ClickableComponent c in inputComponents)
            {
                for(int i=0; i<smeltTimes.Count; i++)
                {
                    if (smeltTimes[i].Id == c.myID)
                    {
                        if (c.item != null)
                        {
                            int timeUntilSmelt = getTicksUntilReady(c.item.Name);
                            int nextTime = smeltTimes[i].Value;
                            if (timeUntilSmelt - nextTime == 0)
                                createBar(c);
                        }
                        smeltTimes[i].Value += 1;
                    }
                }
            }
        }

        private void GameLoop_DayEnding_Boiler(object sender, DayEndingEventArgs e)
        {
            foreach(ClickableComponent c in inputComponents)
            {
                if(c.item != null)
                {
                    forceStopSmelting(c.bounds.X, c.bounds.Y, c.item);
                    createBar(c);
                }
            }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);
            bool itemAdded = false;
            List<data> tempListForRemoval = new List<data>();
            foreach (ClickableComponent comp in inventoryMenu.allClickableComponents)
            {
                if(comp.containsPoint(x, y))
                {
                    if(inventoryMenu.getItemAt(x, y) != null)
                    {
                        Item temp = inventoryMenu.getItemAt(x, y);
                        if (Util.IsStackedSmeltableItem(temp))
                        {
                            foreach (ClickableComponent c in inputComponents)
                            {
                                if (!itemAdded)
                                {
                                    if (c.item == null)
                                    {
                                        c.item = new Object(temp.parentSheetIndex, 5);
                                        temp.Stack -= 5;
                                        if (temp.Stack == 0)
                                            Game1.player.removeItemFromInventory(temp);
                                        inventory.Add(c.item);
                                        BoilerData.dataList.Add(new data(c.item.parentSheetIndex, c.item.Stack, c.myID));
                                        itemAdded = true;
                                    }
                                    else if (c.item.Name == temp.Name) // Was to be able to stack items but does not fit with current system, future change?
                                    {
                                        c.item.addToStack(new Object(temp.parentSheetIndex, 5));
                                        Monitor.LogOnce($"{c.item.Name} - {c.item.Stack}", LogLevel.Debug);
                                        temp.Stack -= 5;
                                        if (temp.Stack == 0)
                                            Game1.player.removeItemFromInventory(temp);
                                        inventory.Add(c.item);
                                        itemAdded = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            foreach(ClickableComponent c in inputComponents)
            {
                if(c.containsPoint(x, y))
                {
                    if(c.item != null)
                    {
                        Game1.player.addItemToInventory((Item)new Object(c.item.parentSheetIndex, 5));
                        c.item.Stack -= 5;
                        if (c.item.Stack == 0)
                        {
                            forceStopSmelting(x, y, c.item);
                            c.item = null;
                        }
                        else if(c.item.Stack < 5)
                        {
                            forceStopSmelting(x, y, c.item);
                        }
                    }
                }
            }

            foreach(ClickableComponent c in outputComponents)
            {
                if(c.containsPoint(x, y))
                {
                    if (c.item != null)
                    {
                        if (Game1.player.items.Count == 36 && !Game1.player.items.Contains(c.item))
                        {
                            --c.item.Stack;
                            Game1.player.addItemToInventory((Item)new Object(c.item.parentSheetIndex, 1));
                            if (c.item.Stack == 0)
                                c.item = null;
                            foreach (data d in BoilerData.dataList)
                                if (d.HoldingComponentID == c.myID)
                                    tempListForRemoval.Add(d);

                        }
                    }
                }
            }
            foreach (data d in tempListForRemoval)
                if (BoilerData.dataList.Contains(d))
                    BoilerData.dataList.Remove(d);
        }

        public override void draw(SpriteBatch b)
        {
            if (!Game1.options.showMenuBackground)
                b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.4f);
            else
                drawBackground(b);
            base.draw(b);
            Game1.drawDialogueBox(xPositionOnScreen, yPositionOnScreen, width, height, false, true);
            drawHorizontalPartition(b, yPositionOnScreen + borderWidth + spaceToClearTopBorder + 240);

            int measureComp1 = 24;
            int measureComp2 = 24;
            for(int i=0; i<BoxCount; ++i)
            {
                Vector2 pos1 = new Vector2(xPositionOnScreen + 120, yPositionOnScreen + 96 + measureComp1);
                b.Draw(Game1.menuTexture, pos1, new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 10)), Color.White, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.5f);
                measureComp1 += 96;
            }
            for(int i=0; i<BoxCount; ++i)
            {
                Vector2 pos2 = new Vector2(xPositionOnScreen + width - 168, yPositionOnScreen + 96 + measureComp2);
                b.Draw(Game1.menuTexture, pos2, new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 10)), Color.White, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.5f);
                measureComp2 += 96;
            }

            foreach(ClickableComponent c in inputComponents)
            {
                if(c.item != null)
                {
                    c.item.drawInMenu(b, new Vector2(c.bounds.X + 4, c.bounds.Y + 4), 1f);
                }
            }

            foreach(ClickableComponent c in outputComponents)
            {
                if (c.item != null)
                {
                    c.item.drawInMenu(b, new Vector2(c.bounds.X + 4, c.bounds.Y + 4), 1f);
                }
            }

            inventoryMenu.draw(b);
            SpriteText.drawStringWithScrollCenteredAt(b, Title, Game1.viewport.Width / 2 + 20, Game1.viewport.Height / 2 - 310, Title, 1f, -1, 0, 0.88f, false);
            drawMouse(b);
        }

        protected virtual List<Item> checkContainerContents()
        {
            if (connectedChest == null)
                return null;
            List<Item> itemList = new List<Item>();
            itemList.AddRange(connectedChest.items);
            return itemList;
        }

        public void forceStopSmelting(int x, int y, Item i)
        {
            foreach (ClickableComponent c in inputComponents)
            {
                if (c.containsPoint(x, y))
                {
                    if (c.item != null && c.item == i)
                    {
                        isSmelting = false;
                        BCC.RequestableHelper.Events.GameLoop.UpdateTicked -= GameLoop_UpdateTicked_Boiler;
                    }
                }
            }
        }

        public bool checkIfCanSmelt()
        {
            List<Item> list = checkContainerContents();
            Item coal = (Item)new Object(382, 1);
            foreach(Item i in list)
            {
                if(i.Name == coal.Name)
                {
                    foreach (ClickableComponent c in inputComponents)
                    {
                        if (c.item != null)
                        {
                            if (c.item.Stack >= 5)
                            {
                                startSmelt(c, list);
                                return true;
                            }
                        }
                    }
                }
            }
            
            return false;
        }

        private void startSmelt(ClickableComponent c, List<Item> IList)
        {
            if (!isSmelting)
            {
                if (c.item != null)
                {
                    if (IList.Contains((Item)new Object(382, 1)))
                        IList.Remove((Item)new Object(382, 1));
                    else if (Game1.player.coalPieces > 0)
                        Game1.player.CoalPieces -= 1;
                    int delay = getTicksUntilReady(c.item.Name);
                    smeltTimes.Add(new CKeyValPair<int, int>(c.myID, 0));
                    BCC.RequestableHelper.Events.GameLoop.UpdateTicked += GameLoop_UpdateTicked_Boiler;
                    isSmelting = true;
                }
            }
        }

        public void createBar(ClickableComponent comp)
        {
            List<CKeyValPair<int, int>> ckvpr = new List<CKeyValPair<int, int>>();
            foreach(ClickableComponent c in inputComponents)
            {
                if (comp.myID == c.myID)
                {
                    if (c.item != null)
                    {
                        int outputID = getOutputItemID(c.item.Name);
                        Item temp;
                        if (!c.item.Name.Contains("Fire"))
                            temp = (Item)new Object(outputID, 1);
                        else
                            temp = (Item)new Object(outputID, 3);
                        bool isItemAdded = false;
                        if (temp != null)
                        {
                            foreach (ClickableComponent c1 in outputComponents)
                            {
                                if (!isItemAdded)
                                {
                                    if (c1.bounds.Y == c.bounds.Y)
                                    {
                                        if (c1.item == null || c1.item.canStackWith(temp))
                                        {
                                            c.item.Stack -= 5;
                                            if (c.item.Stack == 0)
                                            {
                                                c.item = null;
                                                inventory.Remove(c.item);
                                            }
                                            c1.item = temp;
                                            isSmelting = false;
                                            isItemAdded = true;
                                            inventory.Add(c1.item);
                                            BoilerData.dataList.Add(new data(c1.item.parentSheetIndex, c1.item.Stack, c1.myID));
                                        }
                                    }
                                }
                            }
                        }
                    }
                    foreach (CKeyValPair<int, int> ckvp in smeltTimes)
                        if (ckvp.Id == c.myID)
                            ckvpr.Add(ckvp);
                }
            }
            foreach (CKeyValPair<int, int> ckvp in ckvpr)
                smeltTimes.Remove(ckvp);
            BCC.RequestableHelper.Events.GameLoop.UpdateTicked -= GameLoop_UpdateTicked_Boiler;
            checkIfCanSmelt();
        }

        public int getTicksUntilReady(string itemName)
        {
            var split = itemName.ToLower().Split(' ');
            if (split.Contains("quartz"))
                return 9 * (Game1.realMilliSecondsPerGameTenMinutes / 7);
            if (split.Contains("iridium"))
                return 48 * (Game1.realMilliSecondsPerGameTenMinutes / 7);
            if (split.Contains("gold"))
                return 30 * (Game1.realMilliSecondsPerGameTenMinutes / 7);
            if (split.Contains("iron"))
                return 12 * (Game1.realMilliSecondsPerGameTenMinutes / 7);
            if (split.Contains("copper"))
                return 3 * (Game1.realMilliSecondsPerGameTenMinutes / 7);
            return -1;
        }

        public int getOutputItemID(string itemName)
        {
            var split = itemName.ToLower().Split(' ');
            if (split.Contains("quartz"))
                return 80;
            if (split.Contains("iridium"))
                return 337;
            if (split.Contains("gold"))
                return 336;
            if (split.Contains("iron"))
                return 335;
            if (split.Contains("copper"))
                return 334;
            return -1;
        }

        public int getItemMinutesUntilReady(string itemName)
        {
            var split = itemName.ToLower().Split(' ');
            if (split.Contains("quartz"))
                return 90;
            if (split.Contains("iridium"))
                return 480;
            if (split.Contains("gold"))
                return 300;
            if (split.Contains("iron"))
                return 120;
            if (split.Contains("copper"))
                return 30;
            return -1;
        }
    }
}
