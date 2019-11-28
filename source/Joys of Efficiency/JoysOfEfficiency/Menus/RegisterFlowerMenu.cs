using System;
using System.Collections.Generic;
using JoysOfEfficiency.Core;
using JoysOfEfficiency.OptionsElements;
using JoysOfEfficiency.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;

namespace JoysOfEfficiency.Menus
{
    internal class RegisterFlowerMenu : IClickableMenu
    {
        private static readonly Logger Logger = new Logger("RegisterFlowerMenu");

        // ReSharper disable once InconsistentNaming
        private const int MARGIN_COMPONENTS = 8;

        private readonly List<OptionsElement> _elements = new List<OptionsElement>();

        private readonly ColorBox _colorPreviewBox;

        private Color _currentColor;

        private readonly int _itemIndex;

        private readonly Action<int, Color> _onButtonPressed;

        public RegisterFlowerMenu(int width, int height, Color initialColor, int item, Action<int, Color> buttonCallBack = null) : base(Game1.viewport.Width / 2 - width / 2,
            Game1.viewport.Height / 2 - height / 2, width, height, true)
        {
            _onButtonPressed = buttonCallBack ?? ((i, c) =>
            {
                Logger.Log($"({i}): {c}");
                exitThisMenu();
            });


            _elements.Add(new EmptyLabel());
            if (item != -1)
            {
                string s = string.Format(InstanceHolder.Translation.Get("options.flower"), Util.GetItemName(item));
                _elements.Add(new LabelComponent(s));
                _itemIndex = item;
            }
            _currentColor = initialColor;
            _elements.Add(new ModifiedSlider("R", 0, initialColor.R, 0, 255, OnSliderValueChange));
            _elements.Add(new ModifiedSlider("G", 1, initialColor.G, 0, 255, OnSliderValueChange));
            _elements.Add(new ModifiedSlider("B", 2, initialColor.B, 0, 255, OnSliderValueChange));

            _elements.Add(new EmptyLabel());

            _elements.Add(new LabelComponent(InstanceHolder.Translation.Get("options.previewColor")));
            _colorPreviewBox = new ColorBox("preview", 0, initialColor);
            _elements.Add(_colorPreviewBox);

            _elements.Add(new EmptyLabel());


            _elements.Add(new ButtonWithLabel("register", 0, OnButtonPressed));
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);
            foreach (OptionsElement element in _elements)
            {
                if (element.bounds.Contains(x - xPositionOnScreen - element.bounds.X / 2,
                    y - yPositionOnScreen - element.bounds.Y / 2))
                {
                    element.receiveLeftClick(x, y);
                }
                y -= element.bounds.Height + MARGIN_COMPONENTS;
            }
            
        }

        public override void leftClickHeld(int x, int y)
        {
            base.leftClickHeld(x, y);
            foreach (OptionsElement element in _elements)
            {
                if (element.bounds.Contains(x - xPositionOnScreen - element.bounds.X / 2, y - yPositionOnScreen - element.bounds.Y / 2))
                {
                    element.leftClickHeld(x - element.bounds.X - xPositionOnScreen, y - element.bounds.Y - yPositionOnScreen);
                }
                y -= element.bounds.Height + MARGIN_COMPONENTS;
            }
        }

        public override void releaseLeftClick(int x, int y)
        {
            base.releaseLeftClick(x, y);
            foreach (OptionsElement element in _elements)
            {
                if (element.bounds.Contains(x - xPositionOnScreen - element.bounds.X / 2, y - yPositionOnScreen - element.bounds.Y / 2))
                {
                    element.leftClickReleased(x - element.bounds.X - xPositionOnScreen, y - element.bounds.Y - yPositionOnScreen);
                }
                y -= element.bounds.Height + MARGIN_COMPONENTS;
            }
        }

        public override void receiveKeyPress(Keys key)
        {
            base.receiveKeyPress(key);
            if (key == Keys.Escape)
            {
                CloseMenu();
            }
        }

        /// <summary>
        /// Close the menu and return to the game.
        /// </summary>
        private static void CloseMenu()
        {
            if (!(Game1.activeClickableMenu is JoeMenu))
            {
                return;
            }

            Game1.playSound("bigDeSelect");
            Game1.exitActiveMenu();
        }

        public override void draw(SpriteBatch b)
        {
            if (InstanceHolder.Config.FilterBackgroundInMenu)
            {
                //Darken background.
                b.Draw(Game1.fadeToBlackRect, new Rectangle(0, 0, Game1.viewport.Width, Game1.viewport.Height),
                    Color.Black * 0.5f);
            }
            //Draw window frame.
            drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60), xPositionOnScreen, yPositionOnScreen, width, height, Color.White, 1.0f, false);
            base.draw(b);

            int x = xPositionOnScreen + 16;
            int y = yPositionOnScreen + 16;

            foreach (OptionsElement element in _elements)
            {
                // Draw each option elements.
                element.draw(b, x, y);
                y += element.bounds.Height + MARGIN_COMPONENTS;
            }

            Util.DrawCursor();
        }

        private void OnSliderValueChange(int which, int value)
        {
            switch (which)
            {
                case 0:
                    _currentColor.R = (byte) value;
                    break;
                case 1:
                    _currentColor.G = (byte) value;
                    break;
                case 2:
                    _currentColor.B = (byte) value;
                    break;
            }
            _colorPreviewBox.SetColor(_currentColor);
        }

        private void OnButtonPressed(int which)
        {
            _onButtonPressed(_itemIndex, _currentColor);
            exitThisMenu();
        }
    }
}
