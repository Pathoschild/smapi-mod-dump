using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Tools;
using StardewValley.Menus;
using StardewValley.BellsAndWhistles;
using StardewValley.Objects;
using System.Timers;
using StardewValley.TerrainFeatures;
using StardewValley.Monsters;
using StardewValley.Locations;
using System.Collections;

namespace InfiniteInventory
{
    /// <summary>The mod entry point.</summary>
    /// 
    public class ModEntry : Mod
    {
        public static Random rnd;
        public static InfInv iv;
        public static Mod instance;
        public Texture2D back;
        public int resp;
        private bool drawSlider;

        public bool[] backward = { false,false};
        public bool[] forward = { false,false};

        public ModData cfg;

        public ModEntry()
        {
            instance = this;
        }

        public override void Entry(IModHelper helper)
        {
            rnd = new Random();
            back = Helper.Content.Load<Texture2D>("backpack.png");

            InputEvents.ButtonPressed += InputEvents_ButtonPressed;
            InputEvents.ButtonReleased += InputEvents_ButtonReleased;
            TimeEvents.AfterDayStarted += TimeEvents_AfterDayStarted;
            GameEvents.EighthUpdateTick += GameEvents_EighthSecondTick;
            SaveEvents.BeforeSave += SaveEvents_BeforeSave;
            SaveEvents.AfterLoad += SaveEvents_AfterLoad;
            MenuEvents.MenuChanged += MenuEvents_MenuChanged;
            MenuEvents.MenuClosed += MenuEvents_MenuClosed;
            GraphicsEvents.OnPostRenderEvent += GraphicsEvents_OnPostRenderEvent;

            helper.ConsoleCommands.Add("buy_tab", "Buys the next inventory tab.", this.buy_tab);
            helper.ConsoleCommands.Add("set_tab", "(CHEAT) Sets the number of tabs in you inventory. Syntax: set_tab <Integer>", this.set_tab);

            helper.ConsoleCommands.Add("cost", "Cost for the next inventory tab.", this.cost);
            helper.ConsoleCommands.Add("set_cost", "Sets cost multiplier for tabs. Syntax: set_cost <Integer>. Default: 30000.", this.set_cost);
        }

        private void InputEvents_ButtonReleased(object sender, EventArgsInput e)
        {
            if (!Context.IsWorldReady)
                return;

            if (e.Button == cfg.tabChangeBack)
            {
                backward[0] = false;
                backward[1] = false;
            }
            if (e.Button == cfg.tabChangeForward)
            {
                forward[0] = false;
                forward[1] = false;
            }
        }

        private void MenuEvents_MenuClosed(object sender, EventArgsClickableMenuClosed e)
        {
            if (!Context.IsWorldReady)
                return;

            if (e.PriorMenu is DialogueBox db)
            {
                if (Game1.currentLocation.lastQuestionKey == "dl_infinv" && resp == 0)
                {
                    string[] str = { "" };
                    buy_tab("buy_tab", str);
                }
                else
                {

                }

                GameEvents.UpdateTick -= selectedResponse;
                resp = -1;
            }
        }

        Vector2 tabLoc = new Vector2(-1, -1);

        private void GraphicsEvents_OnPostRenderEvent(object sender, EventArgs e)
        {
            if (Context.IsWorldReady && (Game1.activeClickableMenu is GameMenu || Game1.activeClickableMenu is ItemGrabMenu) && Game1.player.MaxItems >= 36)
            {

                if (drawSlider && tabLoc.X != -1)
                {

                    double wid = Math.Max(((double)Game1.getMouseX() - ((double)tabLoc.X + 3.0)) / (double)(((double)tabLoc.X + 3.0 + 100.0) - ((double)tabLoc.X + 3.0)), 0.0);
                    if (wid > 1.0)
                        wid = 1.0;
                    double factor = 100.0 / iv.maxTab;
                    int t = (int)Math.Round(wid * (double)iv.maxTab, 15);
                    t = Math.Max(t, 1);

                    Game1.spriteBatch.Draw(Game1.staminaRect, new Rectangle((int)tabLoc.X, (int)tabLoc.Y, 106, 40), Color.Black);
                    Game1.spriteBatch.Draw(Game1.staminaRect, new Rectangle((int)(tabLoc.X + 3.0f), (int)(tabLoc.Y + 3.0f), (int)(Math.Round((t) * factor, 15)), 34), Color.ForestGreen);
                    Game1.spriteBatch.Draw(Game1.mouseCursors, new Vector2(Game1.getMouseX(), Game1.getMouseY()), Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, Game1.options.SnappyMenus ? 44 : 0, 16, 16), Color.White * Game1.mouseCursorTransparency, 0.0f, Vector2.Zero, Game1.pixelZoom + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 0.1f);

                    if (t <= iv.maxTab && t != iv.currTab)
                    {
                        iv.changeTabs(t);
                    }
                }
                else
                {

                    float f = 1.0f;
                    if (Game1.activeClickableMenu is GameMenu)
                    {
                        List<IClickableMenu> tabs = this.Helper.Reflection.GetField<List<IClickableMenu>>(Game1.activeClickableMenu, "pages").GetValue();
                        IClickableMenu curTab = tabs[(Game1.activeClickableMenu as GameMenu).currentTab];

                        if (curTab is InventoryPage)
                        {
                            tabLoc = new Vector2(curTab.xPositionOnScreen + curTab.width - 150, curTab.yPositionOnScreen + curTab.height - 300);

                            f = Math.Min(0.1f + (Vector2.Distance(new Vector2(Game1.getMouseX(), Game1.getMouseY()), new Vector2(curTab.xPositionOnScreen + curTab.width - 100, curTab.yPositionOnScreen + curTab.height - 280)) / 200.0f), 1.0f);

                            Game1.spriteBatch.Draw(back, new Vector2(curTab.xPositionOnScreen + curTab.width - 100, curTab.yPositionOnScreen + curTab.height - 100), new Rectangle?(new Rectangle(0, 0, back.Width, back.Height)), Color.White, 0.0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, 0.5f);

                            Game1.spriteBatch.DrawString(Game1.smallFont, $"Tab: {iv.currTab}", tabLoc, new Color(36, 47, 48) * f, 0.0f, Vector2.Zero, (float)(Game1.pixelZoom / 3), SpriteEffects.None, 0.5f);

                            Game1.spriteBatch.Draw(Game1.mouseCursors, new Vector2(Game1.getMouseX(), Game1.getMouseY()), Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, Game1.options.SnappyMenus ? 44 : 0, 16, 16), Color.White * Game1.mouseCursorTransparency, 0.0f, Vector2.Zero, Game1.pixelZoom + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 0.1f);
                        }
                    }
                    else
                    {


                        IClickableMenu menu = Game1.activeClickableMenu;

                        tabLoc = new Vector2(menu.xPositionOnScreen - 128, menu.yPositionOnScreen + 168);

                        f = Math.Min(0.1f + (Vector2.Distance(new Vector2(Game1.getMouseX(), Game1.getMouseY()), new Vector2(menu.xPositionOnScreen - 118 + 38, menu.yPositionOnScreen + 185)) / 200.0f), 1.0f);

                        Game1.drawDialogueBox(menu.xPositionOnScreen - 185, menu.yPositionOnScreen + 48, 200, 200, false, true, (string)null, false);

                        Game1.spriteBatch.DrawString(Game1.smallFont, $"Tab: {iv.currTab}", tabLoc, new Color(36, 47, 48) * f, 0.0f, Vector2.Zero, (float)(Game1.pixelZoom / 3), SpriteEffects.None, 0.5f);

                        Game1.spriteBatch.Draw(Game1.mouseCursors, new Vector2(Game1.getMouseX(), Game1.getMouseY()), Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, Game1.options.SnappyMenus ? 44 : 0, 16, 16), Color.White * Game1.mouseCursorTransparency, 0.0f, Vector2.Zero, Game1.pixelZoom + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 0.1f);
                    }

                    if (f <= 0.15f)
                    {
                        drawSlider = true;
                    }
                }
            }
        }

        private void set_cost(string arg1, string[] arg2)
        {
            if (int.TryParse(arg2[0], out int r))
            {
                iv.cost = r;
                return;
            }

            Monitor.Log("Error: Invalid parameters. Syntax: set_cost <Integer>", LogLevel.Error);
        }

        private void cost(string arg1, string[] arg2)
        {
            Monitor.Log($"Cost for tab {iv.maxTab + 1}: {iv.maxTab * iv.cost}; cost = {iv.cost}.");
        }

        private void set_tab(string arg1, string[] arg2)
        {
            if (int.TryParse(arg2[0], out int r))
            {
                if (r > 1)
                {
                    iv.maxTab = r;

                    Monitor.Log("Set_tab was successful.");
                    return;
                }
            }

            Monitor.Log("Error: Invalid parameters. Syntax: set_tab <Integer>", LogLevel.Error);
        }

        private void buy_tab(string arg1, string[] arg2)
        {
            int cost = (iv.maxTab) * iv.cost;

            if (Game1.player.Money < cost)
            {
                Game1.chatBox.addMessage($"You dont have {cost} coins for tab {iv.maxTab + 1}.", Color.DarkRed);

                return;
            }
            else
            {
                Game1.player.Money -= cost;

                iv.maxTab++;

                Game1.chatBox.addMessage($"You successfully purchased tab {iv.maxTab} for {cost} coins.", Color.ForestGreen);
            }
        }

        private void MenuEvents_MenuChanged(object sender, EventArgsClickableMenuChanged e)
        {
            if (e.NewMenu == null)
                return;

            if (e.NewMenu is DialogueBox)
            {
                GameEvents.UpdateTick += selectedResponse;
            }
        }

        private void selectedResponse(object sender, EventArgs e)
        {
            if (Game1.activeClickableMenu is DialogueBox db)
            {
                int sel = Helper.Reflection.GetField<int>(db, "selectedResponse").GetValue();
                if (sel != -1)
                    resp = sel;
            }
        }

        private void SaveEvents_AfterLoad(object sender, EventArgs e)
        {
            iv = new InfInv(instance);

            cfg = iv.config;
        }

        private void SaveEvents_BeforeSave(object sender, EventArgs e)
        {

            //
            if (iv.maxTab > 1)
            {
                if (iv.currTab != 1)
                    iv.changeTabs(1);

                iv.saveData();
            }


        }

        private void GameEvents_EighthSecondTick(object sender, EventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (backward[0] == true && backward[1] == true)
            {
                iv.changeTabs(iv.currTab - 1);
            }
            else if (forward[0] == true && forward[1] == true)
            {
                iv.changeTabs(iv.currTab + 1);
            }
        }

        private void TimeEvents_AfterDayStarted(object sender, EventArgs e)
        {

        }

        private void InputEvents_ButtonPressed(object sender, EventArgsInput e)
        {
            if (Context.IsWorldReady && (Game1.activeClickableMenu is GameMenu || Game1.activeClickableMenu is ItemGrabMenu) && Game1.player.MaxItems >= 36)
            {
                if (drawSlider)
                {
                    drawSlider = false;
                }

                if (backward[0] == true && e.Button == SButton.LeftControl)
                    backward[1] = true;
                else if (forward[0] == true && e.Button == SButton.LeftControl)
                    forward[1] = true;

                else if (Game1.activeClickableMenu is GameMenu)
                {

                    List<IClickableMenu> tabs = this.Helper.Reflection.GetField<List<IClickableMenu>>(Game1.activeClickableMenu, "pages").GetValue();
                    IClickableMenu curTab = tabs[(Game1.activeClickableMenu as GameMenu).currentTab];

                    if (curTab is InventoryPage)
                    {
                        if (e.Button == cfg.tabChangeBack)
                        {
                            iv.changeTabs(iv.currTab - 1);

                            backward[0] = true;
                        }
                        else if (e.Button == cfg.tabChangeForward)
                        {
                            iv.changeTabs(iv.currTab + 1);

                            forward[0] = true;
                        }

                        if (e.Button == cfg.backpackBuy)
                        {
                            int xval = curTab.xPositionOnScreen + curTab.width;
                            int yval = curTab.yPositionOnScreen + curTab.height;
                            if (e.Cursor.ScreenPixels.X > (xval - 100) && e.Cursor.ScreenPixels.X < (xval - 50) && e.Cursor.ScreenPixels.Y > (yval - 100) && e.Cursor.ScreenPixels.Y < (yval - 50))
                            {
                                Response yes = new Response("Yes", $"Purchase tab {iv.maxTab + 1} for {iv.maxTab * iv.cost}.");
                                Response no = new Response("No", $"Do not purchase tab {iv.maxTab + 1} for {iv.maxTab * iv.cost}.");
                                Response[] resps = new Response[] { yes, no };
                                Game1.currentLocation.createQuestionDialogue("Purchase a new inventory tab?", resps, "dl_infinv");

                            }
                        }
                    }
                }
                else
                {
                    if (e.Button == cfg.tabChangeBack)
                    {
                        iv.changeTabs(iv.currTab - 1);

                        backward[0] = true;
                    }
                    else if (e.Button == cfg.tabChangeForward)
                    {
                        iv.changeTabs(iv.currTab + 1);

                        forward[0] = true;
                    }
                }

            }
        }

        public int rand(int val, int add)
        {
            return (int)(Math.Round((rnd.NextDouble() * val) + add));
        }


    }

    public class ModData
    {
        public List<List<string>> itemInfo { get; set; }


        public int maxTab { get; set; }
        public int cost { get; set; }
        public SButton tabChangeBack { get; set; }
        public SButton tabChangeForward { get; set; }
        public SButton backpackBuy { get; set; }

        public ModData()
        {
            this.tabChangeBack = SButton.NumPad1;
            this.tabChangeForward = SButton.NumPad2;
            this.backpackBuy = SButton.MouseLeft;
            this.cost = 30000;
            this.maxTab = 1;

            this.itemInfo = new List<List<string>>();


        }
    }

}