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

        //const int min_activate_amount = 100;

        const int rotten_plant_idx = 747;

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

        private Item heldItem;

        private List<Vector2> greenTiles;

        public ComposterMenu(CompostingBin bin)
        {
            BinPos = new Vector2(bin.tileX, bin.tileY);

            nutritionsComponent = new NutritionsComponent(0, 0, 0, 0);

            cancelButton = new ClickableComponent(new Rectangle(), "cancel");
            activateButton = new ClickableComponent(new Rectangle(), "activate");
            applyButton = new ClickableComponent(new Rectangle(), "apply");

            AddCompostItems();

            resetGUI();

            if (Composting.ComposterDaysLeft.ContainsKey(BinPos) && Composting.ComposterDaysLeft[BinPos] > 0) //composter is active
                ShowAsActive();
            if (Composting.ComposterCompostLeft.ContainsKey(BinPos) && Composting.ComposterCompostLeft[BinPos] > 0) //composter has compost in it
            {
                MakeReady();
            }

            Game1.player.Halt();

            UpdateGreenTiles();
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

        private void resetGUI()
        {
            int SpaceToClearTopBorder = 32;
            int SpaceToClearSideBorder = 32;
            playerInventoryMenu = new PlayerInventoryMenu(this, 0,0);
            width = playerInventoryMenu.width + SpaceToClearSideBorder * 2;
            height = playerInventoryMenu.height * 2 + space * 3 + nutrition_area_height + activate_button_size + SpaceToClearTopBorder * 2;
            xPositionOnScreen = Game1.viewport.Width / 2 - width / 2;
            yPositionOnScreen = Game1.viewport.Height / 2 - height / 2;

            initialize(xPositionOnScreen, yPositionOnScreen, width, height, true);

            compostInventoryMenu = new CompostInventoryMenu(this, nutritionsComponent, compostItems, xPositionOnScreen+SpaceToClearSideBorder, yPositionOnScreen+SpaceToClearTopBorder);
            playerInventoryMenu = new PlayerInventoryMenu(this, compostInventoryMenu.xPositionOnScreen, compostInventoryMenu.yPositionOnScreen + compostInventoryMenu.height + 64);
            compostInventoryMenu.SetPlayerInventoryMenu(playerInventoryMenu);
            playerInventoryMenu.SetCompostInventoryMenu(compostInventoryMenu);

            nutritionsComponent.SetPosition(playerInventoryMenu.xPositionOnScreen, playerInventoryMenu.yPositionOnScreen + playerInventoryMenu.height + space, playerInventoryMenu.width, nutrition_area_height);

            cancelButton.bounds = new Rectangle(Game1.viewport.Width - space - cancel_button_size, Game1.viewport.Height - space - cancel_button_size, cancel_button_size, cancel_button_size);
            //int actWidth = SpriteText.getWidthOfString(ModEntry.GetHelper().Translation.Get("composter.activate_button"));
            activateButton.bounds = new Rectangle(compostInventoryMenu.xPositionOnScreen + compostInventoryMenu.width / 2 - activate_button_size / 2 - activate_button_size, playerInventoryMenu.yPositionOnScreen + playerInventoryMenu.height + space + nutrition_area_height + space, activate_button_size, activate_button_size);
            applyButton.bounds = new Rectangle(activateButton.bounds.X + activateButton.bounds.Width*2, activateButton.bounds.Y, activate_button_size, activate_button_size);
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            base.gameWindowSizeChanged(oldBounds, newBounds);
            resetGUI();
        }

        public override bool readyToClose()
        {
            return (heldItem == null);
        }

        public override void emergencyShutDown()
        {
            base.emergencyShutDown();
            if (heldItem != null)
            {
                Game1.player.addItemToInventoryBool(heldItem, false);
                Game1.playSound("coin");
            }
        }


        public override void draw(SpriteBatch b)
        {
            if (!applyMode)
            {
                base.draw(b);

                //IClickableMenu.drawTextureBox(b, xPositionOnScreen - 96, yPositionOnScreen - 16, compostInventoryMenu.width + 64, compostInventoryMenu.height*2 + space + nutrition_area_height + space + 64*2, Color.White);
                IClickableMenu.drawTextureBox(b, xPositionOnScreen, yPositionOnScreen, width, height, Color.White);

                compostInventoryMenu.draw(b);
                playerInventoryMenu.draw(b);
                if (state == State.fill)
                    nutritionsComponent.draw(b);

                b.Draw(Game1.mouseCursors, applyButton.bounds, new Rectangle(366, 373, 16, 16), Color.White); //okButton (366, 373, 16, 16)

                if (state == State.fill && nutritionsComponent.GoodDistribution())
                    b.Draw(Game1.mouseCursors, activateButton.bounds, new Rectangle(175, 379, 191 - 175, 394 - 379), Color.White);

                switch (state)
                {
                    case State.fill:
                        SpriteText.drawStringWithScrollCenteredAt(b, ModEntry.GetHelper().Translation.Get("composter.fillheadline"), Game1.viewport.Width / 2, 64);
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
                               /*
                               foreach (Vector2 vec in Composting.CompostAppliedDays.Keys)
                               {

                                   for (int i=1; i<=3; i++)
                                       b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, vec * 64f), new Rectangle(194 + color * 16, 388, 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.999f);

                                   foreach (Vector2 v in ModComponent.GetAdjacentTiles(vec))
                                       b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, (new Vector2(v.X,v.Y)) * 64f), new Rectangle(194 + color * 16, 388, 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.999f);


            }
            */
                foreach (Vector2 v in greenTiles)
                    b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, (new Vector2(v.X, v.Y)) * 64f), new Rectangle(194 + color * 16, 388, 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.999f);
                foreach (Vector2 vec in Composting.CompostAppliedDays.Keys)
                    b.DrawString(Game1.smallFont, Composting.CompostAppliedDays[vec].ToString(), Game1.GlobalToLocal(Game1.viewport, new Vector2(vec.X * Game1.tileSize + Game1.tileSize/2, vec.Y * Game1.tileSize + Game1.tileSize/2)), Color.White);

                b.Draw(Game1.mouseCursors, cancelButton.bounds, new Rectangle(192, 256, 256 - 192, 320 - 256), Color.White);

                SpriteText.drawStringWithScrollCenteredAt(b, ModEntry.GetHelper().Translation.Get("composter.applyheadline", new { amount = Composting.ComposterCompostLeft[BinPos] }), Game1.viewport.Width / 2, 64);
            }

            drawMouse(b);
            if (heldItem != null)
                heldItem.drawInMenu(b, new Vector2(Game1.getOldMouseX()+32, Game1.getOldMouseY()-32), 1);

            if (!applyMode && hoverText.Length > 0)
            {
                drawHoverText(b, hoverText, Game1.dialogueFont, 0, 0, -1, null, -1, null, null, 0, -1, -1, -1, -1, 1f, null);
            }
        }

        public override void update(GameTime time)
        {
            base.update(time);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            Vector2 vec = new Vector2((Game1.viewport.X + x) / 64, (Game1.viewport.Y + y) / 64);
            if (!applyMode)
            {
                compostInventoryMenu.receiveLeftClick(x, y, playSound);
                playerInventoryMenu.receiveLeftClick(x, y, playSound);

                if (applyButton.containsPoint(x, y))
                    applyMode = true;
                else if (state == State.fill && nutritionsComponent.GoodDistribution() && activateButton.containsPoint(x, y))
                    MakeActive();

                base.receiveLeftClick(x, y, playSound);
            }
            else if (Game1.currentLocation is Farm farm && !Composting.AffectedByCompost(vec) && farm.terrainFeatures.ContainsKey(vec) && farm.terrainFeatures[vec] is HoeDirt && Composting.ComposterCompostLeft[BinPos] > 0)
            {
                void apply()
                {
                    Composting.ComposterCompostLeft[BinPos]--;
                    if (Composting.ComposterCompostLeft[BinPos] <= 0)
                        MakeInactive();

                    UpdateGreenTiles();
                    //greenTiles.Add(vec);
                    //greenTiles.AddRange(ModComponent.GetAdjacentTiles(vec));
                }

                if (Composting.CompostAppliedDays.ContainsKey(vec))
                {
                    if (Composting.CompostAppliedDays[vec] <= 0)
                    {
                        Composting.CompostAppliedDays[vec] = Composting.config.compost_last_for_days;
                        apply();
                    }
                    else
                        Game1.playSound("cancel");
                }
                else
                {
                    Composting.CompostAppliedDays.Add(vec, Composting.config.compost_last_for_days);
                    apply();
                }
            }
            else if (cancelButton.bounds.Contains(x, y))
            {
                ApplyMode(false);
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

        public override void receiveKeyPress(Keys key)
        {
            base.receiveKeyPress(key);
        }

        public override void performHoverAction(int x, int y)
        {

            Item item = playerInventoryMenu.getItemAt(x, y);
            if (item == null)
                item = compostInventoryMenu.getItemAt(x, y);

            if (item != null)
            {
                int brown = Browns(item);
                int green = Greens(item);
                if (brown > 0 || green > 0)
                    hoverText = ModEntry.GetHelper().Translation.Get("composter.brown") + ": " + brown + " / " + ModEntry.GetHelper().Translation.Get("composter.green") + ": " + green;
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
            }
            else if (state == State.ready && applyButton.containsPoint(x, y))
            {
                hoverText = ModEntry.GetHelper().Translation.Get("composter.apply_button");
            }
            else if (applyButton.containsPoint(x, y))
            {
                hoverText = ModEntry.GetHelper().Translation.Get("composter.apply_button_show");
            }
            else
            {
                hoverText = "";
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
        }

        public void MakeReady()
        {
            ShowAsReady();
            List<Item> contents = Composting.ComposterContents[BinPos];
            for (int i = 0; i < contents.Count; i++)
            {
                if (contents[i] != null)
                    contents[i] = ObjectFactory.getItemFromDescription(ObjectFactory.regularObject, rotten_plant_idx, contents[i].Stack);
            }
        }

        public void ShowAsActive()
        {
            state = State.active;
            compostInventoryMenu.highlightMethod = InventoryMenu.highlightNoItems;
            playerInventoryMenu.highlightMethod = InventoryMenu.highlightNoItems;
        }

        public void MakeActive()
        {
            ShowAsActive();

            if (Composting.ComposterDaysLeft.ContainsKey(BinPos))
                Composting.ComposterDaysLeft[BinPos] = Composting.config.composter_takes_days;
            else
                Composting.ComposterDaysLeft.Add(BinPos, Composting.config.composter_takes_days);
        }

        public void ShowAsInActive()
        {
            state = State.fill;
            compostInventoryMenu.highlightMethod = HighlightMethod;
            playerInventoryMenu.highlightMethod = HighlightMethod;
        }

        public void MakeInactive()
        {
            ShowAsInActive();

            if (Composting.ComposterDaysLeft.ContainsKey(BinPos))
                Composting.ComposterDaysLeft[BinPos] = 0;
            else
                Composting.ComposterDaysLeft.Add(BinPos, 0);
        }

        public void ApplyMode(bool mode)
        {
            applyMode = mode;
            Game1.displayHUD = !mode;
        }

        public static bool HighlightMethod(Item item)
        {
            int brown = Composting.GetBrown(item);
            int green = Composting.GetGreen(item);
            return (brown > 0 || green > 0);
        }

        public static int Browns(Item item) => item == null ? 0 : Composting.GetBrown(item) * item.Stack;
           
        public static int Greens(Item item) => item == null ? 0 : Composting.GetGreen(item) * item.Stack;

        class CompostInventoryMenu : InventoryMenu
        {
            private PlayerInventoryMenu playerInventoryMenu;
            private NutritionsComponent nutritionsComponent;
            private ComposterMenu menu;

            public CompostInventoryMenu(ComposterMenu menu, NutritionsComponent nutritionsComponent, List<Item> items, int x, int y) : base(x, y, false, items, HighlightMethod)
            {
                this.nutritionsComponent = nutritionsComponent;
                this.menu = menu;
                CalcGreenBrown();
            }

            public void SetPlayerInventoryMenu(PlayerInventoryMenu playerInventoryMenu)
            {
                this.playerInventoryMenu = playerInventoryMenu;
            }

            public override void receiveLeftClick(int x, int y, bool playSound = true)
            {
                Item clickedItem = leftClick(x, y, menu.heldItem, false);
                if (clickedItem == null)
                    return;
                Item remItem = playerInventoryMenu.tryToAddItem(clickedItem);
                if (remItem != null && remItem.Stack > 0)
                    menu.heldItem = remItem;

                CalcGreenBrown();
            }

            public override void receiveRightClick(int x, int y, bool playSound = true)
            {
                Item clickedItem = rightClick(x, y, menu.heldItem, false);
                if (clickedItem == null)
                    return;
                Item remItem = playerInventoryMenu.tryToAddItem(clickedItem);
                if (remItem != null && remItem.Stack > 0)
                    menu.heldItem = remItem;

                CalcGreenBrown();
            }


            public void CalcGreenBrown()
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

            class PlayerInventoryMenu : InventoryMenu
        {
            private CompostInventoryMenu compostInventoryMenu;
            private ComposterMenu menu;

            public PlayerInventoryMenu(ComposterMenu menu, int x, int y) : base(x, y, true, null, HighlightMethod)
            {
                this.menu = menu;
            }

            public void SetCompostInventoryMenu(CompostInventoryMenu compostInventoryMenu)
            {
                this.compostInventoryMenu = compostInventoryMenu;
            }

            public override void receiveLeftClick(int x, int y, bool playSound = true)
            {
                Item clickedItem = leftClick(x, y, menu.heldItem, false);
                if (clickedItem == null)
                    return;
                Item remItem = compostInventoryMenu.tryToAddItem(clickedItem);
                if (remItem != null && remItem.Stack > 0)
                    menu.heldItem = remItem;

                compostInventoryMenu.CalcGreenBrown();

            }

            public override void receiveRightClick(int x, int y, bool playSound = true)
            {
                Item clickedItem = rightClick(x, y, menu.heldItem, false);
                if (clickedItem == null)
                    return;
                Item remItem = compostInventoryMenu.tryToAddItem(clickedItem);
                if (remItem != null && remItem.Stack > 0)
                    menu.heldItem = remItem;

                compostInventoryMenu.CalcGreenBrown();
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

            public bool GoodDistribution()
            {
                Rectangle greenBarArea = GetGreenBarArea();
                double brownq = (green + brown) == 0 ? 0.5 : (double)brown / (brown + green);
                return (green+brown >= Composting.config.composter_takes_days*10) && greenBarArea.X <= x + brownq*width && x + brownq*width <= greenBarArea.X + greenBarArea.Width;
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
                b.DrawString(Game1.smallFont, greenstr, new Vector2(x+width-SpriteText.getWidthOfString(greenstr), y + height / 2), Color.Green);

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
