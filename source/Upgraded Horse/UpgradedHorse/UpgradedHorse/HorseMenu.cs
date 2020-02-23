using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using StardewValley;
using StardewValley.Menus;

using StardewModdingAPI;

namespace UpgradedHorseMod
{
    public class HorseMenu : IClickableMenu
    {
        new static int width = Game1.tileSize * 6;
        new static int height = Game1.tileSize * 8;

        private string hoverText = "";

        const int region_okButton = 101;
        const int region_love = 102;
        const int region_sellButton = 103;
        const int region_fullnessHover = 107;
        const int region_happinessHover = 108;
        const int region_loveHover = 109;
        const int region_textBoxCC = 110;

        private UpgradedHorse horse;
        private TextBox textBox;
        private ClickableTextureComponent okButton;
        private ClickableTextureComponent love;
        private ClickableComponent loveHover;
        private ClickableComponent textBoxCC;

        private double friendshipLevel;

        private ModEntry mod;

        public HorseMenu(UpgradedHorse horse, ModEntry mod)
          : base(Game1.viewport.Width / 2 - HorseMenu.width / 2, Game1.viewport.Height / 2 - HorseMenu.height / 2, HorseMenu.width, HorseMenu.height, false)
        {
            this.mod = mod;
            Game1.player.Halt();
            HorseMenu.width = Game1.tileSize * 6;
            HorseMenu.height = Game1.tileSize * 8;

            this.horse = horse;
            this.textBox = new TextBox((Texture2D)null, (Texture2D)null, Game1.dialogueFont, Game1.textColor);
            this.textBox.X = Game1.viewport.Width / 2 - Game1.tileSize * 2 - 12;
            this.textBox.Y = this.yPositionOnScreen - 4 + Game1.tileSize * 2;
            this.textBox.Width = Game1.tileSize * 4;
            this.textBox.Height = Game1.tileSize * 3;

            this.textBoxCC = new ClickableComponent(new Microsoft.Xna.Framework.Rectangle(this.textBox.X, this.textBox.Y, this.textBox.Width, Game1.tileSize), "")
            {
                myID = 110,
                downNeighborID = 104
            };
            this.textBox.Text = horse.displayName;

            this.textBox.Selected = false;

            ClickableTextureComponent textureComponent1 = new ClickableTextureComponent(new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen + HorseMenu.width + 4, this.yPositionOnScreen + HorseMenu.height - Game1.tileSize - IClickableMenu.borderWidth, Game1.tileSize, Game1.tileSize), Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46, -1, -1), 1f, false);
            int num1 = 101;
            textureComponent1.myID = num1;
            int num2 = 103;
            textureComponent1.upNeighborID = num2;
            this.okButton = textureComponent1;



            ClickableTextureComponent textureComponent5 = new ClickableTextureComponent(
                (horse.horseData.Friendship / 10.0).ToString() + "<",
                new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + Game1.tileSize / 2 + 16, this.yPositionOnScreen - Game1.tileSize / 2 + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 4 - Game1.tileSize / 2, HorseMenu.width - Game1.tileSize * 2, Game1.tileSize), (string)null, "Friendship", Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(172, 512, 16, 16), 4f, false);
            int num10 = 102;
            textureComponent5.myID = num10;
            this.love = textureComponent5;
            this.loveHover = new ClickableComponent(new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder, this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 3 - Game1.tileSize / 2, HorseMenu.width, Game1.tileSize), "Friendship")
            {
                myID = 109
            };

            this.friendshipLevel = horse.horseData.Friendship / 1000.0;
            if (!Game1.options.SnappyMenus)
                return;
            this.populateClickableComponentList();
            this.snapToDefaultClickableComponent();
        }

        public override void snapToDefaultClickableComponent()
        {
            this.currentlySnappedComponent = this.getComponentWithID(101);
            this.snapCursorToCurrentSnappedComponent();
        }

        public void textBoxEnter(TextBox sender)
        {
        }

        public override void receiveKeyPress(Keys key)
        {
            if (Game1.globalFade)
                return;
            if (((IEnumerable<InputButton>)Game1.options.menuButton).Contains<InputButton>(new InputButton(key)) && (this.textBox == null || !this.textBox.Selected))
            {
                Game1.playSound("smallSelect");
                if (this.readyToClose())
                {
                    Game1.exitActiveMenu();
                    if (this.textBox.Text.Length <= 0)
                        return;
                    this.horse.displayName = this.textBox.Text;
                }
            }
            else
            {
                if (!Game1.options.SnappyMenus || ((IEnumerable<InputButton>)Game1.options.menuButton).Contains<InputButton>(new InputButton(key)) && this.textBox != null && this.textBox.Selected)
                    return;
                base.receiveKeyPress(key);
            }
        }

        public override void update(GameTime time)
        {
            base.update(time);
            int num1 = Game1.getOldMouseX() + Game1.viewport.X;
            int num2 = Game1.getOldMouseY() + Game1.viewport.Y;
            if (num1 - Game1.viewport.X < Game1.tileSize)
                Game1.panScreen(-8, 0);
            else if (num1 - (Game1.viewport.X + Game1.viewport.Width) >= -Game1.tileSize)
                Game1.panScreen(8, 0);
            if (num2 - Game1.viewport.Y < Game1.tileSize)
                Game1.panScreen(0, -8);
            else if (num2 - (Game1.viewport.Y + Game1.viewport.Height) >= -Game1.tileSize)
                Game1.panScreen(0, 8);
        }

        public void finishedPlacinghorse()
        {
            Game1.exitActiveMenu();
            Game1.currentLocation = Game1.player.currentLocation;
            Game1.currentLocation.resetForPlayerEntry();
            Game1.globalFadeToClear((Game1.afterFadeFunction)null, 0.02f);
            Game1.displayHUD = true;
            Game1.viewportFreeze = false;
            Game1.displayFarmer = true;
            Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:horseQuery_Moving_HomeChanged"), Color.LimeGreen, 3500f));
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (Game1.globalFade)
                return;

            if (this.okButton != null)
            {
                if (this.okButton.containsPoint(x, y))
                    Game1.exitActiveMenu();

            }

        }


        public override bool readyToClose()
        {
            this.textBox.Selected = false;
            if (base.readyToClose())
                return !Game1.globalFade;
            return false;
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            if (Game1.globalFade)
                return;

            if (this.okButton != null)
            {
                if (this.okButton.containsPoint(x, y))
                    Game1.exitActiveMenu();

            }
        }

        public override void performHoverAction(int x, int y)
        {
            this.hoverText = "";
            if (this.okButton != null)
            {
                if (this.okButton.containsPoint(x, y))
                    this.okButton.scale = Math.Min(1.1f, this.okButton.scale + 0.05f);
                else
                    this.okButton.scale = Math.Max(1f, this.okButton.scale - 0.05f);
            }
        }

        public override void draw(SpriteBatch b)
        {
            if (!Game1.globalFade)
            {
                b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
                this.textBox.Draw(b);
                Game1.drawDialogueBox(this.xPositionOnScreen, this.yPositionOnScreen + Game1.tileSize * 2, HorseMenu.width, HorseMenu.height - Game1.tileSize * 2, false, true, (string)null, false);

                string text1 = this.getFullnessMessage();
                Utility.drawTextWithShadow(b, text1, Game1.smallFont, new Vector2((float)(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + Game1.tileSize / 2), (float)(this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + Game1.tileSize / 4 + Game1.tileSize * 2)), Game1.textColor, 1f, -1f, -1, -1, 1f, 3);

                /* Drawing Friendship Level */
                int num2 = 0;
                int num3 = this.friendshipLevel * 1000.0 % 200.0 >= 100.0 ? (int)(this.friendshipLevel * 1000.0 / 200.0) : -100;
                for (int index = 0; index < 5; ++index)
                {
                    b.Draw(Game1.mouseCursors, new Vector2((float)(this.xPositionOnScreen + Game1.tileSize * 3 / 2 + 8 * Game1.pixelZoom * index), (float)(num2 + this.yPositionOnScreen - Game1.tileSize / 2 + Game1.tileSize * 5)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(211 + (this.friendshipLevel * 1000.0 <= (double)((index + 1) * 195) ? 7 : 0), 428, 7, 6)), Color.White, 0.0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, 0.89f);
                    if (num3 == index)
                        b.Draw(Game1.mouseCursors, new Vector2((float)(this.xPositionOnScreen + Game1.tileSize * 3 / 2 + 8 * Game1.pixelZoom * index), (float)(num2 + this.yPositionOnScreen - Game1.tileSize / 2 + Game1.tileSize * 5)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(211, 428, 4, 6)), Color.White, 0.0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, 0.891f);
                }

                Utility.drawTextWithShadow(b, Game1.parseText(this.horse.getMoodMessage(), Game1.smallFont, HorseMenu.width - IClickableMenu.spaceToClearSideBorder * 2 - Game1.tileSize), Game1.smallFont, new Vector2((float)(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + Game1.tileSize / 2), (float)(num2 + this.yPositionOnScreen + Game1.tileSize * 6 - Game1.tileSize + Game1.pixelZoom)), Game1.textColor, 1f, -1f, -1, -1, 1f, 3);
                this.okButton.draw(b);

                if (this.hoverText != null && this.hoverText.Length > 0)
                    IClickableMenu.drawHoverText(b, this.hoverText, Game1.smallFont, 0, 0, -1, (string)null, -1, (string[])null, (Item)null, 0, -1, -1, -1, -1, 1f, (CraftingRecipe)null);
            }
            else if (!Game1.globalFade)
            {
                string text = "test";
                Game1.drawDialogueBox(Game1.tileSize / 2, -Game1.tileSize, (int)Game1.dialogueFont.MeasureString(text).X + IClickableMenu.borderWidth * 2 + Game1.tileSize / 4, Game1.tileSize * 2 + IClickableMenu.borderWidth * 2, false, true, (string)null, false);
                b.DrawString(Game1.dialogueFont, text, new Vector2((float)(Game1.tileSize / 2 + IClickableMenu.spaceToClearSideBorder * 2 + 8), (float)(Game1.tileSize / 2 + Game1.pixelZoom * 3)), Game1.textColor);
                this.okButton.draw(b);
            }
            this.drawMouse(b);
        }

        private string getFullnessMessage()
        {
            if (this.horse.horseData.Full)
            {
                return String.Format("{0} is full!", horse.displayName);
            }
            else
            {
                return String.Format("{0} is hungry.", horse.displayName);
            }
        }
    }
}