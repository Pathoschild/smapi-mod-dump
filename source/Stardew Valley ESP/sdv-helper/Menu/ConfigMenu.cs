using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using sdv_helper.Config;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sdv_helper.Menu
{
    class ConfigMenu : IClickableMenu
    {
        public static readonly Rectangle TitleRect = new Rectangle(0, 256, 60, 60);
        public static readonly string MenuText = "Stardew Valley ESP";
        public static readonly string[] TabNames = new string[] { "Colors", "Hotkeys" };
        public static readonly int PaddingX = 30;
        public static readonly int PaddingY = 20;
        public static readonly int TextLength = 300;
        public static readonly int TabHeight = 50;

        private readonly Dictionary<string, ColorComponent> colorPickers = new Dictionary<string, ColorComponent>();
        private readonly List<Tab> tabs = new List<Tab>();
        private readonly List<Tab> settingsTabs = new List<Tab>();
        private readonly Settings settings;

        private readonly int bWidth = (int)(TextLength * 1.5);
        private readonly int bStartX = PaddingX * 2;
        private readonly int bStartY = PaddingY;
        private int currentEntry = 0;
        private bool scrolling = false;

        private Scrollbar scrollbar;
        private int bHeight;
        private int entriesPerPage;
        private int pages;

        // 0x01 - colors window
        // 0x10 - settings window
        // 0x20 - settings window picking menu key
        // 0x30 - settings window picking load key
        private int state = 0x01;

        public ConfigMenu(Settings settings)
        {
            this.settings = settings;
            ResetColorPickers();
            ResetSizes();
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            foreach (Tab t in tabs)
                if (t.WasClicked(x, y))
                {
                    state = 1 << t.TabIndex * 4;
                    return;
                }
            // if color menu
            if ((state & 0xF) > 0)
                foreach (KeyValuePair<string, ColorComponent> c in colorPickers)
                    c.Value.receiveLeftClick(x, y, playSound);
            // if settings menu, not picking color
            else if ((state & 0xF0) == 0x10)
            {
                foreach (Tab t in settingsTabs)
                    if (t.WasClicked(x, y))
                    {
                        state += t.TabIndex << 4;
                        return;
                    }
            }
            // was picking color, but clicked so cancel it
            else
                state = 0x10;
        }

        public override void releaseLeftClick(int x, int y)
        {
            if (IsColorPickerOpen())
                return;

            scrolling = false;
        }

        public override void leftClickHeld(int x, int y)
        {
            if (IsColorPickerOpen())
                return;

            if (x >= scrollbar.Left && x <= scrollbar.Right && y >= scrollbar.Top && y <= scrollbar.Bottom)
                scrolling = true;
            if (!scrolling)
                return;

            int sbHeight = scrollbar.Bottom - scrollbar.Top;
            int position = (int)(1f * (Game1.getMouseY() - scrollbar.Top) / sbHeight * pages);
            ScrollTo(position);
        }

        public override void receiveScrollWheelAction(int direction)
        {
            if ((state & 0xF) == 0) return;

            if (direction < 0)
                ScrollDown();
            else
                ScrollUp();
        }

        public override void receiveKeyPress(Keys key)
        {
            if (Game1.options.doesInputListContain(Game1.options.menuButton, key) && this.readyToClose())
            {
                this.exitThisMenu(true);
                return;
            }
            if (key == Keys.None)
                return;
            int result = state & 0xF0;
            if (result <= 0x10)
                return;
            if (result == 0x20)
                settings.MenuKey = key.ToSButton();
            else if (result == 0x30)
                settings.LoadKey = key.ToSButton();
            state = 0x10;
            ResetSettingsTabs();
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            base.gameWindowSizeChanged(oldBounds, newBounds);
            ResetSizes();
        }

        public override void draw(SpriteBatch b)
        {
            // if there are new entries, reset the stuff
            if (settings.DSettings.Count != colorPickers.Count)
            {
                ResetColorPickers();
                pages = settings.DSettings.Count - entriesPerPage;
                scrollbar.Pages = pages;
            }

            if (!Game1.options.showMenuBackground)
                b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.5f);

            // main menu thing
            drawTextureBox(b, Game1.menuTexture, TitleRect, bStartX, bStartY, bWidth, bHeight, Color.White);

            // title box
            Vector2 size = Game1.dialogueFont.MeasureString(MenuText);
            drawTextureBox(b, Game1.menuTexture, TitleRect, bStartX - PaddingX / 2, bStartY - PaddingY / 2, (int)size.X + PaddingX, (int)size.Y + PaddingY, Color.White);
            Utility.drawTextWithShadow(b, MenuText, Game1.dialogueFont, new Vector2(bStartX, bStartY), Game1.textColor);

            // menu content
            int yCoord;
            switch (state)
            {
                case 0x20:
                case 0x30:
                    Utility.drawTextWithShadow(b, "Press a key!", Game1.dialogueFont, new Vector2(bStartX + borderWidth, bStartY + bHeight - 200), Game1.textColor);
                    goto case 0x10;
                case 0x10:
                    foreach (Tab t in settingsTabs)
                        t.draw(b);
                    break;
                case 0x01:
                default:
                    // scrollbar
                    scrollbar.draw(b);

                    StringBuilder sb = new StringBuilder();
                    for (int i = currentEntry; i < currentEntry + entriesPerPage && i < colorPickers.Count; i++)
                    {
                        sb.Clear();
                        yCoord = bStartY + borderWidth * 2 + (i - currentEntry) * (28 + 36 + 5);
                        foreach (char c in colorPickers.ElementAt(i).Key)
                        {
                            sb.Append(c);
                            if (Game1.dialogueFont.MeasureString(sb).X > TextLength)
                            {
                                sb.Remove(sb.Length - 2, 2);
                                break;
                            }
                        }
                        Utility.drawTextWithShadow(b, sb.ToString(), Game1.dialogueFont, new Vector2(bStartX + borderWidth, yCoord), Game1.textColor);
                        colorPickers.ElementAt(i).Value.DrawAt(b, TextLength + 150, yCoord);
                    }
                    break;
            }

            // tabs
            foreach (Tab t in tabs)
                t.draw(b);

            if (!Game1.options.hardwareCursor)
                b.Draw(Game1.mouseCursors, new Vector2(Game1.getOldMouseX(), Game1.getOldMouseY()),
                    Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, Game1.options.gamepadControls ? 44 : 0, 16, 16), Color.White, 0f, Vector2.Zero,
                    Game1.pixelZoom + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 1f);
        }

        private void ResetSettingsTabs()
        {
            settingsTabs.Clear();
            string label = "Menu Key: " + settings.MenuKey;
            Vector2 size = Game1.dialogueFont.MeasureString(label);
            settingsTabs.Add(new Tab(bStartX + PaddingX / 2, borderWidth * 2 + bStartY + PaddingY / 2, bWidth - PaddingX,
                (int)size.Y + PaddingY, label, 1, Game1.dialogueFont));
            label = "Load Key: " + settings.LoadKey;
            size = Game1.dialogueFont.MeasureString(label);
            settingsTabs.Add(new Tab(bStartX + PaddingX / 2, (int)size.Y + PaddingY + borderWidth * 2 + bStartY + PaddingY / 2,
                bWidth - PaddingX, (int)size.Y + PaddingY, label, 2, Game1.dialogueFont));
        }

        private void ResetSizes()
        {
            bHeight = Game1.viewport.Height - PaddingY * 2 - TabHeight - PaddingY;
            entriesPerPage = (int)Math.Floor((bHeight - borderWidth * 2) / (Game1.dialogueFont.MeasureString("A").Y + 20));
            pages = settings.DSettings.Count - entriesPerPage;
            scrollbar = new Scrollbar(bStartX + bWidth, bStartY, bHeight - borderWidth, pages);

            ResetTabs();
            ResetSettingsTabs();
        }

        private void ResetTabs()
        {
            tabs.Clear();
            int off = 0;
            for (int i = 0; i < TabNames.Length; i++)
            {
                Vector2 size = Game1.smallFont.MeasureString(TabNames[i]);
                int width = (int)size.X + PaddingX;
                tabs.Add(new Tab(bStartX + off, bStartY + bHeight + PaddingY / 2, width, (int)size.Y + PaddingY, TabNames[i], i));
                off += width + PaddingX / 2;
            }
        }

        private void ResetColorPickers()
        {
            List<string> keys = settings.DSettings.Keys.ToList();
            keys.Sort();

            colorPickers.Clear();
            foreach (string key in keys)
            {
                ColorComponent c = new ColorComponent(key, settings.DSettings[key], settings);
                colorPickers.Add(key, c);
            }
        }

        private bool IsColorPickerOpen()
        {
            if ((state & 0x0F) == 0)
                return false;
            foreach (KeyValuePair<string, ColorComponent> c in colorPickers)
                if (c.Value.ColorPicker.visible) return true;
            return false;
        }

        private void ScrollUp()
        {
            ScrollTo(currentEntry - 1);
        }

        private void ScrollDown()
        {
            ScrollTo(currentEntry + 1);
        }

        private void ScrollTo(int position)
        {
            if (position > pages || position < 0) return;

            currentEntry = position;
            scrollbar.SetBarAt(position);
            ResetColorPickers(); // kind of bad, because of unnecessary resort
        }
    }
}
