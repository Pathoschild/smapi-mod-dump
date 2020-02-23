using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
// ReSharper disable SwitchStatementMissingSomeCases

namespace ModSettingsTab.Framework.Components
{
    public class PopupTextBox : IClickableMenu
    {
        private const int MinLength = 1;
        private readonly ClickableTextureComponent _doneEnterButton;
        private readonly TextBox _textBox;
        private readonly TextBox.TextEnter _doneEnter;
        private readonly string _title;

        public PopupTextBox(TextBox.TextEnter b, string title,bool numbersOnly = false) 
        {
            _doneEnter = b;
            xPositionOnScreen = 0;
            yPositionOnScreen = 0;
            width = Game1.viewport.Width;
            height = Game1.viewport.Height;
            _title = title;
            _textBox = new TextBox(_ => { })
            {
                X = Game1.viewport.Width / 2 - 192,
                Y = Game1.viewport.Height / 2,
                Width = 350,
                Height = 192,
                Text = "",
                numbersOnly = numbersOnly
            };
            _textBox.OnEnterPressed += TextBoxEnter;
            Game1.keyboardDispatcher.Subscriber = _textBox;
            _textBox.Selected = true;
            upperRightCloseButton = new ClickableTextureComponent(
                new Rectangle(Game1.viewport.Width / 2+200, Game1.viewport.Height / 2 - 170, 48, 48),
                Game1.mouseCursors, 
                new Rectangle(337, 494, 12, 12), 
                4f);

            var textureComponent2 = new ClickableTextureComponent(
                new Rectangle(_textBox.X + _textBox.Width + 32 + 4,
                    Game1.viewport.Height / 2 - 8, 64, 64),
                Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46), 1f)
            {
                myID = 102, rightNeighborID = 103, leftNeighborID = 104
            };
            _doneEnterButton = textureComponent2;
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
            if (_doneEnter != null)
            {
                _doneEnter(sender.Text);
                _textBox.Selected = false;
            }
        }

        public override void receiveGamePadButton(Buttons b)
        {
            base.receiveGamePadButton(b);
            if (!_textBox.Selected)
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
                    _textBox.Selected = false;
                    break;
            }
        }

        public override void receiveKeyPress(Keys key)
        {
            if (_textBox.Selected || Game1.options.doesInputListContain(Game1.options.menuButton, key))
                return;
            base.receiveKeyPress(key);
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
        }

        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);
            if (_doneEnterButton == null) return;
            _doneEnterButton.scale = _doneEnterButton.containsPoint(x, y) 
                ? Math.Min(1.1f, _doneEnterButton.scale + 0.05f) 
                : Math.Max(1f, _doneEnterButton.scale - 0.05f);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (upperRightCloseButton != null || readyToClose() || upperRightCloseButton.containsPoint(x, y))
            {
                if (playSound)
                    Game1.playSound("bigDeSelect");
                _doneEnter(_textBox.Text);
                return;
            }
            _textBox.Update();
            if (_doneEnterButton.containsPoint(x, y))
            {
                TextBoxEnter(_textBox);
                Game1.playSound("smallSelect");
            }
            _textBox.Update();
        }
        public override bool shouldDrawCloseButton()
        {
            return true;
        }

        public override void draw(SpriteBatch b)
        {
            base.draw(b);
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, null,Color.Black * 0.75f,0f,Vector2.Zero,SpriteEffects.None,0.6f);
            SpriteText.drawStringWithScrollCenteredAt(b, _title, Game1.viewport.Width / 2,
                Game1.viewport.Height / 2 - 128, _title);
            Game1.drawDialogueBox(_textBox.X - 32, 
                _textBox.Y - 112 + 10, 
                _textBox.Width + 80, 
                _textBox.Height, false, true);
            _textBox.Draw(b);
            _doneEnterButton.draw(b);
            drawMouse(b);
        }
    }
}