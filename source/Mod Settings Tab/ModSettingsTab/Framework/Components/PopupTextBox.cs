using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;

namespace ModSettingsTab.Framework.Components
{
    public class PopupTextBox : IClickableMenu
    {
        public int MinLength = 1;
        public ClickableTextureComponent DoneEnterButton;
        public TextBox TextBox;
        public ClickableComponent textBoxCC;
        private TextBox.TextEnter doneEnter;
        public string Title;

        public PopupTextBox(TextBox.TextEnter b, string title,bool numbersOnly = false) 
        {
            doneEnter = b;
            xPositionOnScreen = 0;
            yPositionOnScreen = 0;
            width = Game1.viewport.Width;
            height = Game1.viewport.Height;
            Title = title;
            TextBox = new TextBox(_ => { })
            {
                X = Game1.viewport.Width / 2 - 192,
                Y = Game1.viewport.Height / 2,
                Width = 350,
                Height = 192,
                Text = "",
                numbersOnly = numbersOnly
            };
            TextBox.OnEnterPressed += TextBoxEnter;
            Game1.keyboardDispatcher.Subscriber = TextBox;
            TextBox.Selected = true;
            upperRightCloseButton = new ClickableTextureComponent(
                new Rectangle(Game1.viewport.Width / 2+200, Game1.viewport.Height / 2 - 170, 48, 48),
                Game1.mouseCursors, 
                new Rectangle(337, 494, 12, 12), 
                4f);

            var textureComponent2 = new ClickableTextureComponent(
                new Rectangle(TextBox.X + TextBox.Width + 32 + 4,
                    Game1.viewport.Height / 2 - 8, 64, 64),
                Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46), 1f)
            {
                myID = 102, rightNeighborID = 103, leftNeighborID = 104
            };
            DoneEnterButton = textureComponent2;
            textBoxCC = new ClickableComponent(new Rectangle(TextBox.X, TextBox.Y, 350, 48), "")
            {
                myID = 104,
                rightNeighborID = 102
            };
            if (!Game1.options.SnappyMenus)
                return;
            populateClickableComponentList();
            snapToDefaultClickableComponent();
        }

        public sealed override void snapToDefaultClickableComponent()
        {
            currentlySnappedComponent = getComponentWithID(104);
            snapCursorToCurrentSnappedComponent();
        }

        private void TextBoxEnter(StardewValley.Menus.TextBox sender)
        {
            if (sender.Text.Length < MinLength)
                return;
            if (doneEnter != null)
            {
                doneEnter(sender.Text);
                TextBox.Selected = false;
            }
            else
                Game1.exitActiveMenu();
        }

        public override void receiveGamePadButton(Buttons b)
        {
            base.receiveGamePadButton(b);
            if (!TextBox.Selected)
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
                    TextBox.Selected = false;
                    break;
            }
        }

        public override void receiveKeyPress(Keys key)
        {
            if (TextBox.Selected || Game1.options.doesInputListContain(Game1.options.menuButton, key))
                return;
            base.receiveKeyPress(key);
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
        }

        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);
            if (DoneEnterButton == null) return;
            DoneEnterButton.scale = DoneEnterButton.containsPoint(x, y) 
                ? Math.Min(1.1f, DoneEnterButton.scale + 0.05f) 
                : Math.Max(1f, DoneEnterButton.scale - 0.05f);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (upperRightCloseButton != null || readyToClose() || upperRightCloseButton.containsPoint(x, y))
            {
                if (playSound)
                    Game1.playSound("bigDeSelect");
                doneEnter(TextBox.Text);
                return;
            }
            TextBox.Update();
            if (DoneEnterButton.containsPoint(x, y))
            {
                TextBoxEnter(TextBox);
                Game1.playSound("smallSelect");
            }
            TextBox.Update();
        }
        public override bool shouldDrawCloseButton()
        {
            return true;
        }

        public override void draw(SpriteBatch b)
        {
            base.draw(b);
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, null,Color.Black * 0.75f,0f,Vector2.Zero,SpriteEffects.None,0.6f);
            SpriteText.drawStringWithScrollCenteredAt(b, Title, Game1.viewport.Width / 2,
                Game1.viewport.Height / 2 - 128, Title);
            Game1.drawDialogueBox(TextBox.X - 32, 
                TextBox.Y - 112 + 10, 
                TextBox.Width + 80, 
                TextBox.Height, false, true);
            TextBox.Draw(b);
            DoneEnterButton.draw(b);
            drawMouse(b);
        }
    }
}