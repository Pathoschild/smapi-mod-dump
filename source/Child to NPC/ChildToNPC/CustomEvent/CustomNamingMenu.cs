using System;
using StardewValley;
using StardewValley.Menus;
using StardewValley.BellsAndWhistles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ChildToNPC.CustomEvent
{
    class CustomNamingMenu : IClickableMenu
    {
        protected int minLength = 1;
        public const int region_okButton = 101;
        public const int region_doneNamingButton = 102;
        public const int region_randomButton = 103;
        public const int region_namingBox = 104;
        public const int region_male = 105;
        public const int region_female = 106;
        public ClickableTextureComponent doneNamingButton;
        public ClickableTextureComponent randomButton;
        public ClickableTextureComponent maleButton;
        public ClickableTextureComponent femaleButton;
        protected TextBox textBox;
        public ClickableComponent textBoxCC;
        private TextBoxEvent e;
        private doneNamingBehavior doneNaming;
        private string titleMale;
        private string titleFemale;
        private string gender;

        public CustomNamingMenu(doneNamingBehavior b, string titleMaleIn, string titleFemaleIn, string defaultName = null)
        {
            doneNaming = b;
            xPositionOnScreen = 0;
            yPositionOnScreen = 0;
            width = Game1.viewport.Width;
            height = Game1.viewport.Height;
            titleMale = titleMaleIn;
            titleFemale = titleFemaleIn;
            gender = "Male";
            //randomButton = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width + 51 + 64, Game1.viewport.Height / 2, 64, 64), Game1.mouseCursors, new Rectangle(381, 361, 10, 10), 4f, false);
            textBox = new TextBox(null, null, Game1.dialogueFont, Game1.textColor);
            textBox.X = Game1.viewport.Width / 2 - 192;
            textBox.Y = Game1.viewport.Height / 2;
            textBox.Width = 256;
            textBox.Height = 192;
            e = new TextBoxEvent(this.textBoxEnter);
            textBox.OnEnterPressed += this.e;
            Game1.keyboardDispatcher.Subscriber = textBox;
            textBox.Text = defaultName != null ? defaultName : Dialogue.randomName();
            textBox.Selected = true;

            ClickableTextureComponent textureComponent1 = new ClickableTextureComponent(new Rectangle(textBox.X + textBox.Width + 64 + 48 - 8, Game1.viewport.Height / 2 + 4, 64, 64), Game1.mouseCursors, new Rectangle(381, 361, 10, 10), 4f, false);
            textureComponent1.myID = 103;
            textureComponent1.leftNeighborID = 102;
            textureComponent1.rightNeighborID = region_male;
            randomButton = textureComponent1;

            ClickableTextureComponent textureComponent2 = new ClickableTextureComponent(new Rectangle(textBox.X + textBox.Width + 32 + 4, Game1.viewport.Height / 2 - 8, 64, 64), Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46, -1, -1), 1f, false);
            textureComponent2.myID = 102;
            textureComponent2.rightNeighborID = 103;
            textureComponent2.leftNeighborID = 104;
            doneNamingButton = textureComponent2;

            ClickableTextureComponent textureComponent3 = new ClickableTextureComponent("Male", new Rectangle(textBox.X + textBox.Width + 64 + 48 + 32 + 8, Game1.viewport.Height / 2 + 4, 64, 64), null, "Male", Game1.mouseCursors, new Rectangle(128, 192, 16, 16), 4f, false);
            textureComponent3.myID = region_male;
            textureComponent3.rightNeighborID = region_female;
            textureComponent3.leftNeighborID = region_randomButton;
            maleButton = textureComponent3;

            ClickableTextureComponent textureComponent4 = new ClickableTextureComponent("Female", new Rectangle(textBox.X + textBox.Width + 64 + 48 + 32 + 8 + 64, Game1.viewport.Height / 2 + 4, 64, 64), null, "Female", Game1.mouseCursors, new Rectangle(144, 192, 16, 16), 4f, false);
            textureComponent4.myID = region_female;
            textureComponent4.leftNeighborID = region_male;
            femaleButton = textureComponent4;

            textBoxCC = new ClickableComponent(new Rectangle(textBox.X, textBox.Y, 192, 48), "")
            {
                myID = 104,
                rightNeighborID = 102
            };
            if (!Game1.options.SnappyMenus)
                return;
            populateClickableComponentList();
            snapToDefaultClickableComponent();
        }

        public override void snapToDefaultClickableComponent()
        {
            currentlySnappedComponent = getComponentWithID(104);
            snapCursorToCurrentSnappedComponent();
        }

        public void textBoxEnter(TextBox sender)
        {
            if (sender.Text.Length < minLength)
                return;
            if (doneNaming != null)
            {
                doneNaming(sender.Text, gender);
                textBox.Selected = false;
            }
            else
                Game1.exitActiveMenu();
        }

        public override void receiveGamePadButton(Buttons b)
        {
            base.receiveGamePadButton(b);
            if (!textBox.Selected)
                return;
            switch (b)
            {
                case Buttons.DPadUp:
                case Buttons.DPadDown:
                case Buttons.DPadLeft:
                case Buttons.DPadRight:
                case Buttons.LeftThumbstickLeft:
                case Buttons.LeftThumbstickUp:
                case Buttons.LeftThumbstickDown:
                case Buttons.LeftThumbstickRight:
                    textBox.Selected = false;
                    break;
            }
        }

        public override void receiveKeyPress(Keys key)
        {
            if (textBox.Selected || Game1.options.doesInputListContain(Game1.options.menuButton, key))
                return;
            base.receiveKeyPress(key);
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
        }

        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);
            if (doneNamingButton != null)
            {
                if (doneNamingButton.containsPoint(x, y))
                    doneNamingButton.scale = Math.Min(1.1f, this.doneNamingButton.scale + 0.05f);
                else
                    doneNamingButton.scale = Math.Max(1f, this.doneNamingButton.scale - 0.05f);
            }
            if (maleButton != null)
            {
                if (maleButton.containsPoint(x, y))
                    maleButton.scale = Math.Min(maleButton.scale + 0.05f, maleButton.baseScale + 0.5f);
                else
                    maleButton.scale = Math.Max(maleButton.scale - 0.05f, maleButton.baseScale);
            }
            if (femaleButton != null)
            {
                if (femaleButton.containsPoint(x, y))
                    femaleButton.scale = Math.Min(femaleButton.scale + 0.05f, femaleButton.baseScale + 0.5f);
                else
                    femaleButton.scale = Math.Max(femaleButton.scale - 0.05f, femaleButton.baseScale);
            }
            randomButton.tryHover(x, y, 0.5f);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);
            textBox.Update();
            if (doneNamingButton.containsPoint(x, y))
            {
                textBoxEnter(textBox);
                Game1.playSound("smallSelect");
            }
            else if (randomButton.containsPoint(x, y))
            {
                textBox.Text = Dialogue.randomName();
                randomButton.scale = randomButton.baseScale;
                Game1.playSound("drumkit6");
            }
            else if (maleButton.containsPoint(x, y))
            {
                gender = "Male";
                Game1.playSound("smallSelect");
                maleButton.scale -= 0.5f;
                maleButton.scale = Math.Max(3.5f, maleButton.scale);
            }
            else if (femaleButton.containsPoint(x, y))
            {
                gender = "Female";
                Game1.playSound("smallSelect");
                femaleButton.scale -= 0.5f;
                femaleButton.scale = Math.Max(3.5f, femaleButton.scale);
            }
            textBox.Update();
        }

        public override void draw(SpriteBatch b)
        {
            base.draw(b);
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
            //I've added this to change the gender of the title text depending on which gender child you have selected.
            if (gender.Equals("Male"))
                SpriteText.drawStringWithScrollCenteredAt(b, titleMale, Game1.viewport.Width / 2, Game1.viewport.Height / 2 - 128, titleMale, 1f, -1, 0, 0.88f, false);
            else
                SpriteText.drawStringWithScrollCenteredAt(b, titleFemale, Game1.viewport.Width / 2, Game1.viewport.Height / 2 - 128, titleFemale, 1f, -1, 0, 0.88f, false);
            textBox.Draw(b, true);
            doneNamingButton.draw(b);
            randomButton.draw(b);
            maleButton.draw(b);
            femaleButton.draw(b);
            b.Draw(Game1.mouseCursors, (gender.Equals("Male") ? maleButton.bounds : femaleButton.bounds), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 34, -1, -1)), Color.White);
            drawMouse(b);
        }

        public delegate void doneNamingBehavior(string s, string g);
    }
}