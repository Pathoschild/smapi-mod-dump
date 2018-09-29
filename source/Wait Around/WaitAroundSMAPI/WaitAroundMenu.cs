using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace WaitAroundSMAPI
{
    class WaitAroundMenu : IClickableMenu
    {
        private WaitAroundMod Mod { get; set; }
        private Rectangle MenuRect { get; set; }
        private readonly MenuButton[] Buttons;

        public WaitAroundMenu(WaitAroundMod mod)
        {
            this.Mod = mod;
            this.Buttons = new[]
            {
                this.GetMenuButton(name: "up", relativeX: 0, relativeY: -((64 + 10 + 64 + 10 + 64) / 2), spritePosition: 12, callback: this.upButton),
                this.GetMenuButton(name: "down", relativeX: 0, relativeY: -((64 + 10 + 64 + 10 + 64) / 2) + 64 + 10, spritePosition: 11, callback: this.downButton),
                this.GetMenuButton(name: "okay", relativeX: 0, relativeY: -((64 + 10 + 64 + 10 + 64) / 2) + 64 + 10 + 64 + 10, spritePosition: 46, callback: this.enterButton)
            };
        }

        private MenuButton GetMenuButton(string name, int relativeX, int relativeY, int spritePosition, Action<MenuButton> callback)
        {
            return new MenuButton(
                name: name,
                relativeX: relativeX,
                relativeY: relativeY,
                spriteWidth: 64,
                spriteHeight: 64,
                parentMenuFactor: new Vector2(-0.25f, 0.5f),
                parentMenu: this.MenuRect,
                spritesheet: Game1.mouseCursors,
                spritePosition: spritePosition,
                callback: callback
            );
        }

        private void upButton(MenuButton menuButton)
        {
            Mod.timeToWait += 10;
        }

        private void downButton(MenuButton menuButton)
        {
            Mod.timeToWait -= 10;
        }

        private void enterButton(MenuButton menuButton)
        {
            if (Mod.timeToWait > 0)
            {
                this.reloadMap();
                for (int i = 0; i < Mod.timeToWait / 10; i++)
                {
                    Game1.performTenMinuteClockUpdate();
                }
            }
            this.Close();
        }

        private void reloadMap()
        {
            Game1.locationAfterWarp = Game1.currentLocation;
            Game1.xLocationAfterWarp = (int)(Game1.player.Position.X / Game1.tileSize);
            Game1.yLocationAfterWarp = (int)(Game1.player.Position.Y / Game1.tileSize);
            Game1.facingDirectionAfterWarp = 2;
            Game1.fadeToBlack = true;
            Game1.fadeIn = true;
            Game1.fadeToBlackAlpha = 0.0f;
            Game1.player.CanMove = false;
        }

        public void Close()
        {
            Game1.activeClickableMenu = null;
            Mod.timeToWait = 0;
        }

        public override void draw(SpriteBatch b)
        {
            //Draw menu background
            MenuRect = drawBaseMenu(b, 450, 300, 550, 300);

            //Draw buttons
            foreach (MenuButton button in Buttons)
            {
                button.Draw(b, MenuRect);
            }

            //Draw title
            String titleString = "How long do you want to wait?";
            b.DrawString(Game1.dialogueFont, titleString, new Vector2(MenuRect.X + (MenuRect.Width / 2) - (Game1.dialogueFont.MeasureString(titleString).X / 2), MenuRect.Y + 15), Color.Black);

            //Draw time
            String timeString = String.Format("{0:00}:{1:00}", Math.Floor(Mod.timeToWait / 60.0), Mod.timeToWait % 60);
            b.DrawString(Game1.dialogueFont, timeString, new Vector2((MenuRect.Width / 2) - (Game1.dialogueFont.MeasureString(timeString).X) + MenuRect.X, (MenuRect.Height / 2) - (Game1.dialogueFont.MeasureString(timeString).Y) + MenuRect.Y), Color.Black, 0f, Vector2.Zero, 2.0f, SpriteEffects.None, 0.0f);

            //Draw mouse
            this.drawMouse(b);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            foreach (MenuButton button in this.Buttons)
            {
                if (button.containsPoint(x, y))
                    button.Callback(button);
            }
        }

        public override void receiveRightClick(int x, int y, bool playSound = true) { }

        //Borrowed from NPCLocations by Kemenor
        private Rectangle drawBaseMenu(SpriteBatch b, int leftRightPadding, int upperLowerPadding, int minWidth, int minHeight)
        {
            Texture2D MenuTiles = Game1.content.Load<Texture2D>(Path.Combine("Maps", "MenuTiles"));
            var viewport = Game1.viewport;
            var textColor = Color.Black;

            //calculate the dimensions of the menu
            int width = Math.Max(viewport.Width - leftRightPadding * 2, 1);
            int height = Math.Max(viewport.Height - upperLowerPadding * 2, 1);
            if (width < minWidth && minWidth <= viewport.Width)
            {
                width = minWidth;
                leftRightPadding = (viewport.Width - width) / 2;
            }
            if (height < minHeight && minHeight <= viewport.Height)
            {
                height = minHeight;
                upperLowerPadding = (viewport.Height - height) / 2;
            }

            //Texture2D for the menu
            Texture2D menu = new Texture2D(Game1.graphics.GraphicsDevice, width, height);
            //get the upper left corner of the menu
            Vector2 screenLoc = new Vector2(leftRightPadding, upperLowerPadding);

            //fill menu with dump data so it shows
            var data = new uint[width * height];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = 0xffffffff;
            }
            menu.SetData<uint>(data);

            //draw the ugly menu
            Vector2 menubar = screenLoc - new Vector2(32, 32);
            b.Draw(menu, screenLoc, new Color(232, 207, 128));

            Rectangle upperLeft = new Rectangle(0, 0, 64, 64);
            Rectangle upperRight = new Rectangle(192, 0, 64, 64);
            Rectangle lowerLeft = new Rectangle(0, 192, 64, 64);
            Rectangle lowerRight = new Rectangle(192, 192, 64, 64);
            Rectangle upperBar = new Rectangle(128, 0, 64, 64);
            Rectangle leftBar = new Rectangle(0, 128, 64, 64);
            Rectangle rightBar = new Rectangle(192, 128, 64, 64);
            Rectangle lowerBar = new Rectangle(128, 192, 64, 64);

            int menuHeight = height + 2 * 32;
            int menuWidth = width + 2 * 32;

            int rightUpperCorner = menuWidth - 64;
            int leftLowerCorner = menuHeight - 64;


            //Draw upperbar
            for (int i = 64; i < rightUpperCorner - 64; i += 64)
            {
                b.Draw(MenuTiles, menubar + new Vector2(i, 0), upperBar, Color.White);
            }
            int leftOver = rightUpperCorner % 64;
            Rectangle leftOverRect = new Rectangle(upperBar.X, upperBar.Y, leftOver, upperBar.Height);
            b.Draw(MenuTiles, menubar + new Vector2(rightUpperCorner - leftOver, 0), leftOverRect, Color.White);

            //draw left bar
            for (int i = 64; i < leftLowerCorner - 64; i += 64)
            {
                b.Draw(MenuTiles, menubar + new Vector2(0, i), leftBar, Color.White);
            }
            leftOver = leftLowerCorner % 64;
            leftOverRect = new Rectangle(leftBar.X, leftBar.Y, leftBar.Width, leftOver);
            b.Draw(MenuTiles, menubar + new Vector2(0, leftLowerCorner - leftOver), leftOverRect, Color.White);

            //draw right bar
            for (int i = 64; i < leftLowerCorner - 64; i += 64)
            {
                b.Draw(MenuTiles, menubar + new Vector2(rightUpperCorner, i), rightBar, Color.White);
            }
            leftOver = leftLowerCorner % 64;
            leftOverRect = new Rectangle(rightBar.X, rightBar.Y, rightBar.Width, leftOver);
            b.Draw(MenuTiles, menubar + new Vector2(rightUpperCorner, leftLowerCorner - leftOver), leftOverRect, Color.White);

            //draw lower Bar
            for (int i = 64; i < rightUpperCorner - 64; i += 64)
            {
                b.Draw(MenuTiles, menubar + new Vector2(i, leftLowerCorner), lowerBar, Color.White);
            }
            leftOver = rightUpperCorner % 64;
            leftOverRect = new Rectangle(lowerBar.X, lowerBar.Y, leftOver, lowerBar.Height);
            b.Draw(MenuTiles, menubar + new Vector2(rightUpperCorner - leftOver, leftLowerCorner), leftOverRect, Color.White);

            //draw upper left corner
            b.Draw(MenuTiles, menubar, upperLeft, Color.White);
            //draw upper right corner
            b.Draw(MenuTiles, menubar + new Vector2(rightUpperCorner, 0), upperRight, Color.White);
            //draw lower left corner
            b.Draw(MenuTiles, menubar + new Vector2(0, leftLowerCorner), lowerLeft, Color.White);
            //draw lower right Corner
            b.Draw(MenuTiles, menubar + new Vector2(rightUpperCorner, leftLowerCorner), lowerRight, Color.White);

            return new Rectangle((int)screenLoc.X, (int)screenLoc.Y, width, height);
        }
    }
}
