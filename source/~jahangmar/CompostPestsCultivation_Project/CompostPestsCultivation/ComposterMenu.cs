//Copyright (c) 2019 Jahangmar

//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU Lesser General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//GNU Lesser General Public License for more details.

//You should have received a copy of the GNU Lesser General Public License
//along with this program. If not, see <https://www.gnu.org/licenses/>.

//using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using StardewValley;
using StardewValley.TerrainFeatures;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using StardewValley.BellsAndWhistles;
using StardewValley.Objects;
using System.Linq;
//using System;

namespace CompostPestsCultivation
{
    public class ComposterMenu : IClickableMenu
    {
        const int space = 32;
        const int nutrition_area_height = 128;
        const int cancel_button_size = 64;
        const int activate_button_size = 64;
        const int hover_button_inc = 4;

        //const int min_activate_amount = 100;

        const int rotten_plant_idx = 747;

        readonly Rectangle CursorsOkButtonRect = new Rectangle(128, 256, 64, 64);
        readonly Rectangle CursorsHammerButtonRect = new Rectangle(366, 373, 16, 16);
        readonly Rectangle CursorsCancelButtonRect = new Rectangle(192, 256, 256 - 192, 320 - 256);
        readonly Rectangle CursorsPlayButtonRect = new Rectangle(175, 379, 191 - 175, 394 - 379);
        readonly Rectangle CursorsGreenTileRect = new Rectangle(194 + 0 * 16, 388, 16, 16);
        readonly Rectangle CursorsRedTileRect = new Rectangle(194 + 1 * 16, 388, 16, 16);
        Rectangle CursorsColoredTileRect(int color)
        {
            switch (color)
            {
                case 0:
                    return CursorsGreenTileRect;
                default:
                    return CursorsRedTileRect;
            }
        }

        private string hoverText = "";

        private bool applyMode = false;
        enum State
        {
            fill,
            ready,
            active
        };
        private State state;

        private Vector2 BinPos;

        private List<Item> compostItems = null;

        private PlayerInventoryMenu playerInventoryMenu;
        private CompostInventoryMenu compostInventoryMenu;

        private NutritionsComponent nutritionsComponent;

        private ClickableComponent cancelButton, activateButton, applyButton;
        private bool hoverCancelButton, hoverActivateButton, hoverApplyButton;

        private Item heldItem = null;

        private List<Vector2> greenTiles;

        public ComposterMenu(CompostingBin bin)
        {
            BinPos = new Vector2(bin.tileX, bin.tileY);

            nutritionsComponent = new NutritionsComponent(0, 0, 0, 0);

            cancelButton = new ClickableComponent(new Rectangle(), "cancel");
            activateButton = new ClickableComponent(new Rectangle(), "activate");
            applyButton = new ClickableComponent(new Rectangle(), "apply");

            AddCompostItems();

            playerInventoryMenu = new PlayerInventoryMenu(this);
            compostInventoryMenu = new CompostInventoryMenu(this, nutritionsComponent, compostItems);
            compostInventoryMenu.SetOtherInventoryMenu(playerInventoryMenu);
            playerInventoryMenu.SetOtherInventoryMenu(compostInventoryMenu);

            ResetGUI();

            SetState();

            UpdateGreenTiles();
        }

        private void SetState()
        {
            if (Composting.ComposterDaysLeft.ContainsKey(BinPos) && Composting.ComposterDaysLeft[BinPos] > 0) //composter is active
                ShowAsActive();
            if (Composting.ComposterCompostLeft.ContainsKey(BinPos) && Composting.ComposterCompostLeft[BinPos] > 0) //composter has compost in it
            {
                MakeReady();
            }
        }

        private void UpdateGreenTiles()
        {
            greenTiles = new List<Vector2>(Composting.CompostAppliedDays.Keys);
            foreach (Vector2 tile in greenTiles.ToList())
            {
                List<Vector2> adjacents = ModComponent.GetAdjacentTiles(tile);
                foreach (Vector2 adj in adjacents)
                {
                    if (!greenTiles.Contains(adj))
                        greenTiles.Add(adj);
                }
            }
        }

        private void ResetGUI()
        {
            int SpaceToClearTopBorder = 32;
            int SpaceToClearSideBorder = 32;

            int nutritionAreaHeight = state == State.fill ? nutrition_area_height : 0;

            //playerInventoryMenu = new PlayerInventoryMenu(this, 0,0);
            width = playerInventoryMenu.width + SpaceToClearSideBorder * 2;
            height = playerInventoryMenu.height * 2 + space * 3 + nutritionAreaHeight + activate_button_size + SpaceToClearTopBorder * 2;
            xPositionOnScreen = Game1.viewport.Width / 2 - width / 2;
            yPositionOnScreen = Game1.viewport.Height / 2 - height / 2;

            initialize(xPositionOnScreen, yPositionOnScreen, width + SpaceToClearSideBorder + SpaceToClearSideBorder/2, height, true);
            width = playerInventoryMenu.width + SpaceToClearSideBorder * 2; //initialize sets the given width and height values

            compostInventoryMenu.SetPosition(xPositionOnScreen + SpaceToClearSideBorder, yPositionOnScreen + SpaceToClearTopBorder);
            playerInventoryMenu.SetPosition(compostInventoryMenu.xPositionOnScreen, compostInventoryMenu.yPositionOnScreen + compostInventoryMenu.height + 64);

            nutritionsComponent.SetPosition(playerInventoryMenu.xPositionOnScreen, playerInventoryMenu.yPositionOnScreen + playerInventoryMenu.height + space, playerInventoryMenu.width, nutrition_area_height);

            cancelButton.bounds = new Rectangle(Game1.viewport.Width - space - cancel_button_size, Game1.viewport.Height - space - cancel_button_size, cancel_button_size, cancel_button_size);
            //int actWidth = SpriteText.getWidthOfString(ModEntry.GetHelper().Translation.Get("composter.activate_button"));

            activateButton.bounds = new Rectangle(compostInventoryMenu.xPositionOnScreen + compostInventoryMenu.width / 2 - activate_button_size / 2 - activate_button_size, playerInventoryMenu.yPositionOnScreen + playerInventoryMenu.height + space + nutritionAreaHeight, activate_button_size, activate_button_size);
            applyButton.bounds = new Rectangle(activateButton.bounds.X + activateButton.bounds.Width*2, activateButton.bounds.Y, activate_button_size, activate_button_size);
        }

        private void ShowError(string msg, object args = null)
        {
            Game1.showRedMessage(args == null ? ModEntry.GetHelper().Translation.Get(msg) : ModEntry.GetHelper().Translation.Get(msg, args));
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            base.gameWindowSizeChanged(oldBounds, newBounds);
            ResetGUI();
        }

        public override void receiveKeyPress(Keys key)
        {
            Game1.player.Halt();
            if (Game1.options.doesInputListContain(Game1.options.menuButton, key) && applyMode)
            {
                ApplyMode(false);
            }
            base.receiveKeyPress(key);
        }

        public override bool readyToClose()
        {
            return (heldItem == null && !applyMode);
        }

        public override void emergencyShutDown()
        {
            base.emergencyShutDown();
            if (heldItem != null)
            {
                //TODO Game1.creat debris
                //Game1.player.addItemToInventoryBool(heldItem, false);
                Game1.playSound("coin");
            }
            Game1.displayHUD = true;
            Game1.player.forceCanMove();
        }

        public override void draw(SpriteBatch b)
        {
            Rectangle rectButton(Rectangle rect, bool hover)
            {
                int off = hover ? hover_button_inc / 2 : 0;
                return new Rectangle(rect.X - off, rect.Y - off, rect.Width + off + off, rect.Height + off + off);
            }

            if (!applyMode)
            {
                base.draw(b);

                //IClickableMenu.drawTextureBox(b, xPositionOnScreen - 96, yPositionOnScreen - 16, compostInventoryMenu.width + 64, compostInventoryMenu.height*2 + space + nutrition_area_height + space + 64*2, Color.White);
                IClickableMenu.drawTextureBox(b, xPositionOnScreen, yPositionOnScreen, width, height, Color.White);

                compostInventoryMenu.draw(b);
                playerInventoryMenu.draw(b);
                if (state == State.fill)
                    nutritionsComponent.draw(b);

                b.Draw(Game1.mouseCursors, rectButton(applyButton.bounds, hoverApplyButton), CursorsOkButtonRect, Color.White);

                if (state == State.fill && nutritionsComponent.GoodDistribution())
                    b.Draw(Game1.mouseCursors, rectButton(activateButton.bounds, hoverActivateButton), CursorsPlayButtonRect, Color.White);

                //int needed = nutritionsComponent.ItemsNeeded();
                //needed = needed <= 0 ? 0 : needed;

                switch (state)
                {
                    case State.fill:
                        SpriteText.drawStringWithScrollCenteredAt(b, ModEntry.GetHelper().Translation.Get("composter.fillheadline", new { parts = nutritionsComponent.Parts(), min_needed = Composting.config.composter_min_parts }), Game1.viewport.Width / 2, 64);
                        break;
                    case State.ready:
                        SpriteText.drawStringWithScrollCenteredAt(b, ModEntry.GetHelper().Translation.Get("composter.readyheadline", new { amount = Composting.ComposterCompostLeft[BinPos]}), Game1.viewport.Width / 2, 64);
                        break;
                    case State.active:
                        SpriteText.drawStringWithScrollCenteredAt(b, ModEntry.GetHelper().Translation.Get("composter.activeheadline", new { amount = Composting.ComposterDaysLeft[BinPos]}), Game1.viewport.Width / 2, 64);
                        break;
                }
            }
            else //applyMode
            {
                int color = 0; //1: red, 0: green
                               
                foreach (Vector2 v in greenTiles)
                    b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, (new Vector2(v.X, v.Y)) * 64f), CursorsColoredTileRect(color), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.999f);
                foreach (Vector2 vec in Composting.CompostAppliedDays.Keys)
                    b.DrawString(Game1.smallFont, Composting.CompostAppliedDays[vec].ToString(), Game1.GlobalToLocal(Game1.viewport, new Vector2(vec.X * Game1.tileSize + Game1.tileSize/2, vec.Y * Game1.tileSize + Game1.tileSize/2)), Color.White);

                b.Draw(Game1.mouseCursors, rectButton(cancelButton.bounds, hoverCancelButton), CursorsCancelButtonRect, Color.White);
                if (Composting.ComposterCompostLeft.ContainsKey(BinPos) && Composting.ComposterCompostLeft[BinPos] > 0)
                    SpriteText.drawStringWithScrollCenteredAt(b, ModEntry.GetHelper().Translation.Get("composter.applyheadline", new { amount = Composting.ComposterCompostLeft[BinPos] }), Game1.viewport.Width / 2, 64);
            }

            drawMouse(b);
            if (heldItem != null)
                heldItem.drawInMenu(b, new Vector2(Game1.getOldMouseX()+32, Game1.getOldMouseY()-32), 1);

            if (hoverText.Length > 0)
            {
                drawHoverText(b, hoverText, Game1.smallFont, 0, 0, -1, null, -1, null, null, 0, -1, -1, -1, -1, 1f, null);
            }
        }

        public override void update(GameTime time)
        {
            if (applyMode)
            {
                int num = Game1.getOldMouseX() + Game1.viewport.X;
                int num2 = Game1.getOldMouseY() + Game1.viewport.Y;
                if (num - Game1.viewport.X < 64)
                {
                    Game1.panScreen(-8, 0);
                }
                else if (num - (Game1.viewport.X + Game1.viewport.Width) >= -128)
                {
                    Game1.panScreen(8, 0);
                }
                if (num2 - Game1.viewport.Y < 64)
                {
                    Game1.panScreen(0, -8);
                }
                else if (num2 - (Game1.viewport.Y + Game1.viewport.Height) >= -64)
                {
                    Game1.panScreen(0, 8);
                }
                Keys[] pressedKeys = Game1.oldKBState.GetPressedKeys();
                foreach (Keys key in pressedKeys)
                {
                    if (Game1.options.doesInputListContain(Game1.options.moveDownButton, key))
                    {
                        Game1.panScreen(0, 8);
                    }
                    else if (Game1.options.doesInputListContain(Game1.options.moveRightButton, key))
                    {
                        Game1.panScreen(8, 0);
                    }
                    else if (Game1.options.doesInputListContain(Game1.options.moveUpButton, key))
                    {
                        Game1.panScreen(0, -8);
                    }
                    else if (Game1.options.doesInputListContain(Game1.options.moveLeftButton, key))
                    {
                        Game1.panScreen(-8, 0);
                    }
                }
            }
            base.update(time);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            Game1.player.Halt();
            Vector2 vec = new Vector2((Game1.viewport.X + x) / 64, (Game1.viewport.Y + y) / 64);
            if (!applyMode)
            {
                compostInventoryMenu.receiveLeftClick(x, y, playSound);
                playerInventoryMenu.receiveLeftClick(x, y, playSound);

                if (applyButton.containsPoint(x, y))
                {
                    ApplyMode(true);
                }
                else if (state == State.fill && nutritionsComponent.GoodDistribution() && activateButton.containsPoint(x, y))
                    MakeActive();

                base.receiveLeftClick(x, y, playSound);
            }
            else if (applyMode && Game1.currentLocation is Farm farm)
            {
                if (cancelButton.bounds.Contains(x, y))
                {
                    ApplyMode(false);
                }
                else if (farm.terrainFeatures.ContainsKey(vec) && farm.terrainFeatures[vec] is HoeDirt && Composting.ComposterCompostLeft.ContainsKey(BinPos) && Composting.ComposterCompostLeft[BinPos] > 0)
                {
                    void apply()
                    {
                        Composting.ComposterCompostLeft[BinPos]--;
                        if (Composting.ComposterCompostLeft[BinPos] <= 0)
                            MakeInactive();

                        UpdateGreenTiles();
                        Game1.playSound("grassyStep");
                        //greenTiles.Add(vec);
                        //greenTiles.AddRange(ModComponent.GetAdjacentTiles(vec));
                    }

                    if (Composting.CompostAppliedDays.ContainsKey(vec))
                    {
                        if (Composting.CompostAppliedDays[vec] < Composting.config.compost_last_for_days)
                        {
                            Composting.CompostAppliedDays[vec] = Composting.config.compost_last_for_days;
                            apply();
                        }
                        else
                        {
                            ShowError("composter.msg.error_already_affected");
                        }
                    }
                    else
                    {
                        Composting.CompostAppliedDays.Add(vec, Composting.config.compost_last_for_days);
                        apply();
                    }
                }
                else if (!Composting.ComposterCompostLeft.ContainsKey(BinPos) || Composting.ComposterCompostLeft[BinPos] <= 0)
                {
                    ShowError("composter.msg.error_no_compost_left");
                }
                else if (!farm.terrainFeatures.ContainsKey(vec) || !(farm.terrainFeatures[vec] is HoeDirt))
                {
                    ShowError("composter.msg.error_no_tilled_dirt");
                }
                else if (Composting.AffectedByCompost(vec))
                {
                    ShowError("composter.msg.error_already_affected");
                }

            }
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            if (!applyMode)
            {
                compostInventoryMenu.receiveRightClick(x, y, playSound);
                playerInventoryMenu.receiveRightClick(x, y, playSound);
            }
        }

        public override void performHoverAction(int x, int y)
        {
            hoverApplyButton = false;
            hoverActivateButton = false;
            hoverCancelButton = false;

            if (!applyMode)
            {
                Item item = playerInventoryMenu.getItemAt(x, y);
                if (item == null)
                    item = compostInventoryMenu.getItemAt(x, y);

                if (state == State.fill && item != null)
                {
                    int brown = Browns(item);
                    int green = Greens(item);
                    float parts = (float)(green + brown) / Composting.one_part;

                    if (brown > 0 || green > 0)
                        hoverText = ModEntry.GetHelper().Translation.Get("composter.brown") + ": " + brown + " / " + ModEntry.GetHelper().Translation.Get("composter.green") + ": " + green + " (" + ModEntry.GetHelper().Translation.Get("composter.parts", new { parts }) + ")";
                    else
                        hoverText = "";
                }
                else if (state == State.fill && nutritionsComponent.GetBoundingBox().Contains(x, y))
                {
                    hoverText = ModEntry.GetHelper().Translation.Get("composter.greenbar");
                }
                else if (state == State.fill && nutritionsComponent.GoodDistribution() && activateButton.containsPoint(x, y))
                {
                    hoverText = ModEntry.GetHelper().Translation.Get("composter.activate_button");
                    hoverActivateButton = true;
                }
                else if (state == State.ready && applyButton.containsPoint(x, y))
                {
                    hoverText = ModEntry.GetHelper().Translation.Get("composter.apply_button");
                    hoverApplyButton = true;
                }
                else if (applyButton.containsPoint(x, y))
                {
                    hoverText = ModEntry.GetHelper().Translation.Get("composter.apply_button_show");
                    hoverApplyButton = true;
                }
                else
                {
                    hoverText = "";
                }
            }
            else if (applyMode){
                if (cancelButton.containsPoint(x, y))
                {
                    //hoverText = ModEntry.GetHelper().Translation.Get("composter.cancel_button");
                    hoverCancelButton = true;
                }
                else
                {
                    hoverText = "";
                }
            }

            base.performHoverAction(x, y);
        }

        public void AddCompostItems()
        {
            List<Item> items;
            if (Composting.ComposterContents.ContainsKey(BinPos))
                items = Composting.ComposterContents[BinPos];
            else
            {
                items = new List<Item>();
                Composting.ComposterContents.Add(BinPos, items);
            }

            //List<Item> items = Composting.ComposterContents.ContainsKey(BinPos) ? Composting.ComposterContents[BinPos] : new List<Item>();
            int count = items.Count;
            for (int i = 1; i <= 36-count; i++)
                items.Add(null);
            items.Capacity = 36;
            compostItems = items;
        }

        public void SaveCompostItems()
        {
            if (Composting.ComposterContents.ContainsKey(BinPos))
            {
                Composting.ComposterContents[BinPos] = new List<Item>(compostInventoryMenu.actualInventory);
            }
            else
            {
                Composting.ComposterContents.Add(BinPos, new List<Item>(compostInventoryMenu.actualInventory));
            }
        }

        public void ShowAsReady()
        {
            state = State.ready;
            compostInventoryMenu.highlightMethod = InventoryMenu.highlightNoItems;
            playerInventoryMenu.highlightMethod = InventoryMenu.highlightNoItems;
            ResetGUI();
        }

        public void MakeReady()
        {
            List<Item> contents = Composting.ComposterContents[BinPos];
            ShowAsReady();
            /*
            for (int i = 0; i < contents.Count; i++)
            {
                if (contents[i] != null)
                    contents[i] = ObjectFactory.getItemFromDescription(ObjectFactory.regularObject, rotten_plant_idx, contents[i].Stack);
            }
            */
        }

        public void ShowAsActive()
        {
            state = State.active;
            compostInventoryMenu.highlightMethod = InventoryMenu.highlightNoItems;
            playerInventoryMenu.highlightMethod = InventoryMenu.highlightNoItems;
            ResetGUI();
        }

        public void MakeActive()
        {
            if (Composting.ComposterDaysLeft.ContainsKey(BinPos))
                Composting.ComposterDaysLeft[BinPos] = Composting.config.composter_takes_days;
            else
                Composting.ComposterDaysLeft.Add(BinPos, Composting.config.composter_takes_days);

            ShowAsActive();
        }

        public void ShowAsInActive()
        {
            state = State.fill;
            compostInventoryMenu.highlightMethod = HighlightMethod;
            playerInventoryMenu.highlightMethod = HighlightMethod;
            ResetGUI();
        }

        public void MakeInactive()
        {
            if (Composting.ComposterDaysLeft.ContainsKey(BinPos))
                Composting.ComposterDaysLeft[BinPos] = 0;
            else
                Composting.ComposterDaysLeft.Add(BinPos, 0);

            ShowAsInActive();
        }

        public void ApplyMode(bool mode)
        {
            applyMode = mode;
            Game1.displayHUD = !mode;
            Game1.viewportFreeze = mode;
        }

        public static bool HighlightMethod(Item item)
        {
            int brown = Composting.GetBrown(item);
            int green = Composting.GetGreen(item);
            return (brown > 0 || green > 0);
        }

        public static int Browns(Item item) => item == null ? 0 : Composting.GetBrown(item) * item.Stack;
           
        public static int Greens(Item item) => item == null ? 0 : Composting.GetGreen(item) * item.Stack;

        abstract class ModInventoryMenu: InventoryMenu
        {
            protected ModInventoryMenu OtherInventoryMenu;

            private ComposterMenu menu;

            public ModInventoryMenu(ComposterMenu menu, bool playerInventory, List<Item> items) : base(0, 0, playerInventory, items, HighlightMethod) {
                this.menu = menu;
            }

            public void SetOtherInventoryMenu(ModInventoryMenu other)
            {
                OtherInventoryMenu = other;
                CalcGreenBrown();
            }

            public void SetPosition(int x, int y)
            {
                int xDiff = x - xPositionOnScreen;
                int yDiff = y - yPositionOnScreen;

                foreach (ClickableComponent item in inventory)
                {
                    item.bounds.X = item.bounds.X + xDiff;
                    item.bounds.Y = item.bounds.Y + yDiff;
                }
                dropItemInvisibleButton.bounds.X = dropItemInvisibleButton.bounds.X + xDiff;
                dropItemInvisibleButton.bounds.Y = dropItemInvisibleButton.bounds.Y + yDiff;

                xPositionOnScreen = x;
                yPositionOnScreen = y;
            }

            public override void receiveLeftClick(int x, int y, bool playSound = true)
            {
                if (this.isWithinBounds(x, y))
                {
                    Item clickedItem = leftClick(x, y, null, false);
                    if (clickedItem != null)
                    {
                        Item remItem = OtherInventoryMenu.tryToAddItem(clickedItem);
                        if (remItem != null && remItem.Stack == clickedItem.Stack)
                        {
                            Game1.playSound("cancel");
                            Game1.showRedMessage(ModEntry.GetHelper().Translation.Get("composter.msg.error_inventory_full"));
                        }
                        //this.tryToAddItem(remItem);
                        leftClick(x, y, remItem, false);
                        CalcGreenBrown();
                    }
                }
            }

            public override void receiveRightClick(int x, int y, bool playSound = true)
            {
                if (this.isWithinBounds(x, y))
                {
                    Item clickedItem = rightClick(x, y, null, false);
                    if (clickedItem != null)
                    {
                        Item remItem = OtherInventoryMenu.tryToAddItem(clickedItem);
                        if (remItem != null && remItem.Stack == clickedItem.Stack)
                        {
                            Game1.playSound("cancel");
                            Game1.showRedMessage(ModEntry.GetHelper().Translation.Get("composter.msg.error_inventory_full"));
                        }
                        if (remItem?.Stack == 1)
                            leftClick(x, y, remItem, false);
                        else
                            this.tryToAddItem(remItem);
                        CalcGreenBrown();
                    }
                }
            }

            public abstract void CalcGreenBrown();
        }

        class CompostInventoryMenu : ModInventoryMenu
        {
            private NutritionsComponent nutritionsComponent;

            public CompostInventoryMenu(ComposterMenu menu, NutritionsComponent nutritionsComponent, List<Item> items) : base(menu, false, items)
            {
                this.nutritionsComponent = nutritionsComponent;
            }

            public override void CalcGreenBrown()
            {
                int green = 0, brown = 0;

                foreach (Item item in actualInventory)
                {
                    brown += Browns(item);
                    green += Greens(item);
                }

                nutritionsComponent.SetColors(green, brown);
            }

        }

        class PlayerInventoryMenu : ModInventoryMenu
        {
            private ComposterMenu menu;

            public PlayerInventoryMenu(ComposterMenu menu) : base(menu, true, null)
            {
                this.menu = menu;
            }

            public override void CalcGreenBrown()
            {
                OtherInventoryMenu.CalcGreenBrown();
            }
        }

        class NutritionsComponent
        {
            private const int arrow_width = 32;
            private const int green_bar_width = 128;

            private int green = 0;
            private int brown = 0;

            private int x, y, width, height;

            public void SetColors(int green, int brown)
            {
                this.green = green;
                this.brown = brown;
            }

            public NutritionsComponent(int x, int y, int width, int height)
            {
                this.x = x;
                this.y = y;
                this.width = width;
                this.height = height;
            }

            public void SetPosition(int x, int y, int width, int height)
            {
                this.x = x;
                this.y = y;
                this.width = width;
                this.height = height;
            }

            public Rectangle GetBoundingBox() => new Rectangle(x, y, width, height);

            public float Parts() => (float)(green + brown) / Composting.one_part;

            public int ItemsNeeded()
            {
                return Composting.config.composter_min_parts - (int)Parts();
            }

            public bool GoodDistribution()
            {
                Rectangle greenBarArea = GetGreenBarArea();
                double brownq = (green + brown) == 0 ? 0.5 : (double)brown / (brown + green);
                return (ItemsNeeded() <= 0 && greenBarArea.X <= x + brownq*width && x + brownq*width <= greenBarArea.X + greenBarArea.Width);
            }

            public Rectangle GetGreenBarArea()
            {
                return new Rectangle(x + width / 2 - green_bar_width / 2, (int) (y+(height/2)*0.2), green_bar_width, (int) (height / 2 - (height / 2) * 0.6));
            }

            public void draw(SpriteBatch b)
            {
                double greenq = (green + brown) == 0 ? 0.5 : (double)green / (green + brown);
                double brownq = (green + brown) == 0 ? 0.5 : (double)brown / (brown + green);

                greenq = System.Math.Round(greenq, 2);
                brownq = System.Math.Round(brownq, 2);

                b.DrawString(Game1.smallFont, ModEntry.GetHelper().Translation.Get("composter.C", new { C = brown } ), new Vector2(x, y + height / 2), Color.Brown);
                string greenstr = ModEntry.GetHelper().Translation.Get("composter.N", new { N = green });

                b.DrawString(Game1.smallFont, greenstr, new Vector2(x+width-Game1.smallFont.MeasureString(greenstr).X, y + height / 2), Color.Green);

                b.Draw(Game1.mouseCursors, new Rectangle(x, y, width, height / 2), new Rectangle(192+2, 1868, 240-(192+3), 1882-1866), Color.White);
                //draw green bar
                b.Draw(Game1.mouseCursors, GetGreenBarArea(), new Rectangle(682+1, 2078, 690 - (682+1), 2086 - 2078), Color.White);
                //draw arrow
                b.Draw(Game1.mouseCursors, new Rectangle((int) (x + greenq*width - arrow_width / 2), (int) (y+height / 2 - (height / 2)*0.3) , (int)(arrow_width), height / 2), new Rectangle(120, 1234, 128 - 120, 1250 - 1234), Color.White);
                //b.DrawString(Game1.dialogueFont, greenq.ToString(), new Vector2((int)(x + greenq * width - arrow_width / 2), (int)(y + height / 2 - (height / 2) * 0.3)), Color.White);
            }
        }
    }
}
