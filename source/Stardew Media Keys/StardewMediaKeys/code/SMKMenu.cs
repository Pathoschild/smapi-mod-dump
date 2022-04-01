/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AngeloC3/StardewMods
**
*************************************************/

using StardewValley.Menus;
using StardewValley;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;

/* This code takes parts and logic from:
 *      https://github.com/janavarro95/Stardew_Valley_Mods/blob/master/GeneralMods/HappyBirthday/Framework/BirthdayMenu.cs
 */

namespace StardewMediaKeys
{
    internal class SMKMenu : IClickableMenu
    {
        /// <summary>Config object for the mod</summary>
        private ModConfig Config;
        /// <summary>Name of the mod</summary>
        private const string modName = "Stardew Media Keys";
        /// <summary>Texture for the buttons</summary>
        private Texture2D LBTexture;
        /// <summary>Whether or not this is used with Mobile Phone</summary>
        private bool onMobile;
        /// <summary>Mobile phone api object</summary>
        private IMobilePhoneApi api;
        /// <summary>The media buttons to draw.</summary>
        private readonly List<ClickableTextureComponent> mediaButtons = new List<ClickableTextureComponent>();
        /// <summary>The text labels to draw</summary>
        private readonly List<ClickableComponent> labels = new List<ClickableComponent>();
        /// <summary>Width of the UI</summary>
        private const int UIWidth = 500;
        /// <summary>Height of the UI</summary>
        private const int UIHeight = 100;

        /// <summary>Constructs a SMKMenu with an IModHelper and a ModConfig object</summary>
        /// <param name="helper">The IModHelper.</param>
        /// <param name="Config">The mod's ModConfig object.</param>
        public SMKMenu(IModHelper helper, ModConfig Config, bool onMobile = false)
            : base(Game1.viewport.Width / 2 - (UIWidth + IClickableMenu.borderWidth * 2) / 2, Game1.viewport.Height / 2 - (UIHeight + IClickableMenu.borderWidth * 2) / 2 - Game1.tileSize, UIWidth + IClickableMenu.borderWidth * 2, UIHeight + IClickableMenu.borderWidth * 2 + Game1.tileSize)
        {
            this.onMobile = onMobile;
            if (onMobile)
            {
                this.api = helper.ModRegistry.GetApi<IMobilePhoneApi>("aedenthorn.MobilePhone");
                exitFunction = () => this.onExitFunc();
            }
            LBTexture = helper.Content.Load<Texture2D>("assets/LBTexture.png", ContentSource.ModFolder);
            this.Config = Config;
            this.initializeUpperRightCloseButton();
            this.setUpMenu();
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            base.gameWindowSizeChanged(oldBounds, newBounds);
            this.xPositionOnScreen = Game1.viewport.Width / 2 - (UIWidth + IClickableMenu.borderWidth * 2) / 2;
            this.yPositionOnScreen = Game1.viewport.Height / 2 - (UIWidth + IClickableMenu.borderWidth * 2) / 2 - Game1.tileSize;
            this.setUpMenu();
        }

        /// <summary>Sets up mediaButtons and labels with its contents for the menu</summary>
        private void setUpMenu()
        {
            this.mediaButtons.Clear();
            this.labels.Clear();
            labels.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen-150 + Game1.tileSize / 4 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 3 + 8, this.yPositionOnScreen-25 + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - Game1.tileSize / 8, 1, 1), modName));
            
            int xBetween = 175;

            int lbl_xoffset = 185;
            int lbl_yoffset = 33;
            labels.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen - lbl_xoffset + Game1.tileSize / 4 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 3 + 8, this.yPositionOnScreen+lbl_yoffset + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - Game1.tileSize / 8, 1, 1), "Prev"));
            labels.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen - lbl_xoffset + xBetween + Game1.tileSize / 4 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 3 + 8, this.yPositionOnScreen + lbl_yoffset + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - Game1.tileSize / 8, 1, 1), "Play"));
            labels.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen - lbl_xoffset + (2*xBetween) + Game1.tileSize / 4 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 3 + 8, this.yPositionOnScreen+lbl_yoffset + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - Game1.tileSize / 8, 1, 1), "Next"));

            int btn_xoffset = 50;
            int btn_yoffset = 160;
            this.mediaButtons.Add(new ClickableTextureComponent("Prev", new Rectangle(this.xPositionOnScreen- btn_xoffset + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 1 - Game1.tileSize / 4, this.yPositionOnScreen-btn_yoffset + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + (int)(Game1.tileSize * 3.10) - Game1.tileSize / 4, Game1.tileSize * 2, Game1.tileSize), "", "", LBTexture, this.getMediaButton(), Game1.pixelZoom, true));
            this.mediaButtons.Add(new ClickableTextureComponent("Play/Pause", new Rectangle(this.xPositionOnScreen - btn_xoffset + xBetween + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 1 - Game1.tileSize / 4, this.yPositionOnScreen - btn_yoffset + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + (int)(Game1.tileSize * 3.10) - Game1.tileSize / 4, Game1.tileSize * 2, Game1.tileSize), "", "", LBTexture, this.getMediaButton(), Game1.pixelZoom, true));
            this.mediaButtons.Add(new ClickableTextureComponent("Next", new Rectangle(this.xPositionOnScreen - btn_xoffset + (2*xBetween) + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 1 - Game1.tileSize / 4, this.yPositionOnScreen - btn_yoffset + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + (int)(Game1.tileSize * 3.10) - Game1.tileSize / 4, Game1.tileSize * 2, Game1.tileSize), "", "", LBTexture, this.getMediaButton(), Game1.pixelZoom, true));
            this.mediaButtons.Add(upperRightCloseButton);

        }

        /// <summary>Draws a dialague box, the buttons, the labels, and the mouse</summary>
        public override void draw(SpriteBatch b)
        {
            Game1.drawDialogueBox(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, false, true);

            foreach (ClickableTextureComponent button in this.mediaButtons)
                button.draw(b);

            foreach (ClickableComponent label in this.labels)
            {
                Color color = Game1.textColor;
                SpriteFont font = Game1.smallFont;
                if (label.name == modName) 
                { 
                    if (this.Config.BlueNotDefaultTitle)
                        color = Color.Blue;
                    font = Game1.dialogueFont;
                }
                Utility.drawTextWithShadow(b, label.name, font, new Vector2(label.bounds.X, label.bounds.Y), color);

            }

            drawMouse(b);
        }

        /// <summary>Handles a button click if the user clicks a button</summary>
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (this.onMobile && api.IsCallingNPC())
                return;

            foreach (ClickableTextureComponent button in this.mediaButtons)
            {
                if (button.containsPoint(x, y))
                {
                    this.handleButtonClick(button.name);
                    button.scale -= 0.5f;
                    button.scale = Math.Max(3.5f, button.scale);
                }
            }
        }

        /// <summary>Enlarges the buttons when hovered</summary>
        public override void performHoverAction(int x, int y)
        {
            foreach (ClickableTextureComponent button in this.mediaButtons)
            {
                button.scale = button.containsPoint(x, y)
                    ? Math.Min(button.scale + 0.5f, button.baseScale + 0.5f)
                    : Math.Max(button.scale - 0.02f, button.baseScale);
            }
        }

        /// <summary>Gets a rectangle for the media buttons</summary>
        private Rectangle getMediaButton()
        {
            return new Rectangle(188, 438, 32, 9);
        }

        /// <summary>Simulates a media key input based on which button is being handled</summary>
        private void handleButtonClick(string name)
        {
            KeyClicker kc = new KeyClicker();
            switch (name)
            {
                case "Play/Pause":
                    kc.keyClick(kc.PPKEY);
                    break;
                case "Next":
                    kc.keyClick(kc.NEXT);
                    break;
                case "Prev":
                    kc.keyClick(kc.PREV);
                    break;
                // this is the case for the close button
                case "":
                    this.exitThisMenu();
                    break;
                
            }
        }

        /// <summary>Exits on right click</summary>
        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            if (this.onMobile && api.IsCallingNPC())
                return;
            this.exitThisMenu();
        }

        private void onExitFunc()
        {
            if (this.onMobile && this.api != null)
            {
                api.SetAppRunning(false);
            }
        }

    }
}
