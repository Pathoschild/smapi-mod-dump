using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System;

namespace DailyTasksReport.UI
{
    internal class InputListener : OptionsElement
    {
        private static readonly Rectangle ButtonSource = new Rectangle(294, 428, 21, 11);

        private readonly ModConfig _config;
        private readonly OptionsEnum _option;
        private readonly Rectangle _buttonBounds;
        private string _buttonName;
        private bool _listening;

        public InputListener(string label, OptionsEnum whichOption, int slotWidth, ModConfig config)
            : base(label, -1, -1, slotWidth, Game1.pixelZoom * 11, (int)whichOption)
        {
            _buttonBounds = new Rectangle(slotWidth - 28 * Game1.pixelZoom, Game1.pixelZoom * 3 - 1,
                21 * Game1.pixelZoom, 11 * Game1.pixelZoom);

            _config = config;
            _option = whichOption;

            switch (whichOption)
            {
                case OptionsEnum.OpenReportKey:
                    _buttonName = config.OpenReportKey.ToString();
                    break;

                case OptionsEnum.OpenSettings:
                    _buttonName = config.OpenSettings.ToString();
                    break;

                case OptionsEnum.ToggleBubbles:
                    _buttonName = config.ToggleBubbles.ToString();
                    break;

                default:
                    throw new ArgumentOutOfRangeException($"Option {_option} is not possible on a InputListener.");
            }
        }

        public override void receiveLeftClick(int x, int y)
        {
            if (greyedOut || _listening || !_buttonBounds.Contains(x, y))
                return;
            _listening = true;
            InputEvents.ButtonPressed += InputEvents_ButtonPressed;
            Game1.playSound("breathin");
        }

        private void InputEvents_ButtonPressed(object sender, EventArgsInput e)
        {
            if (e.Button == (SButton)Keys.Escape)
            {
                Game1.playSound("bigDeSelect");
            }
            else
            {
                switch (_option)
                {
                    case OptionsEnum.OpenReportKey:
                        _config.OpenReportKey = e.Button;
                        break;

                    case OptionsEnum.OpenSettings:
                        _config.OpenSettings = e.Button;
                        break;

                    case OptionsEnum.ToggleBubbles:
                        _config.ToggleBubbles = e.Button;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException($"Option {_option} is not possible on a InputListener.");
                }
                _buttonName = e.Button.ToString();
                Game1.playSound("coin");
            }
            _listening = false;
            InputEvents.ButtonPressed -= InputEvents_ButtonPressed;
            e.SuppressButton();
        }

        public override void draw(SpriteBatch b, int slotX, int slotY)
        {
            if (_buttonName.Length > 0 || whichOption == -1)
                if (whichOption == -1)
                    Utility.drawTextWithShadow(b, label, Game1.dialogueFont,
                        new Vector2(bounds.X + slotX, bounds.Y + slotY), Game1.textColor, 1f, 0.15f);
                else
                    Utility.drawTextWithShadow(b, label + ": " + _buttonName, Game1.dialogueFont,
                        new Vector2(bounds.X + slotX, bounds.Y + slotY), Game1.textColor, 1f, 0.15f);
            Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2(_buttonBounds.X + slotX, _buttonBounds.Y + slotY),
                ButtonSource, Color.White, 0.0f, Vector2.Zero, Game1.pixelZoom, false, 0.15f);
            if (!_listening)
                return;
            b.Draw(Game1.staminaRect, new Rectangle(0, 0, Game1.viewport.Width, Game1.viewport.Height),
                new Rectangle(0, 0, 1, 1), Color.Black * 0.75f, 0.0f, Vector2.Zero, SpriteEffects.None,
                0.999f);
            b.DrawString(Game1.dialogueFont, "Press new Key...",
                Utility.getTopLeftPositionForCenteringOnScreen(Game1.tileSize * 3, Game1.tileSize), Color.White,
                0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.9999f);
        }
    }
}