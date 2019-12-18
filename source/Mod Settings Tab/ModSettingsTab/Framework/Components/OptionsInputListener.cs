using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace ModSettingsTab.Framework.Components
{
    public class OptionsInputListener : OptionsElement
    {
        private static readonly Rectangle SetButtonSource = new Rectangle(294, 428, 21, 11);
        private SButton _button;
        private string _listenerMessage;
        private bool _listening;
        private Rectangle _buttonBounds;
        private readonly string _label;


        private SButton Button
        {
            get => _button;
            set
            {
                _button = value;
                Label = $"{_label} : {_button}";
            }
        }

        public OptionsInputListener(
            string name,
            string modId,
            string label,
            StaticConfig config,
            Point slotSize,
            SButton button = SButton.None)
            : base(name, modId, label, config, 32, slotSize.Y / 2, slotSize.X - 1, 44)
        {
            _label = label;
            Button = button;
            _buttonBounds = new Rectangle(Bounds.Width - 112, Bounds.Y - 12, 84, Bounds.Height);
            Offset.X = -Bounds.Width - 8;
        }

        public override void ReceiveLeftClick(int x, int y)
        {
            if (GreyedOut || _listening || !_buttonBounds.Contains(x, y))
                return;
            ModEntry.Helper.Events.Input.ButtonPressed +=
                OptionsInputOnButtonPressed;
            _listening = true;
            Game1.playSound("breathin");
            GameMenu.forcePreventClose = true;
            _listenerMessage = Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsElement.cs.11225");
        }

        private void OptionsInputOnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            ModEntry.Helper.Input.Suppress(e.Button);
            if (GreyedOut || !_listening)
                ModEntry.Helper.Events.Input.ButtonPressed -=
                    OptionsInputOnButtonPressed;
            else if (e.Button == SButton.Escape)
            {
                Game1.playSound("bigDeSelect");
                _listening = false;
                ModEntry.Helper.Events.Input.ButtonPressed -=
                    OptionsInputOnButtonPressed;
                GameMenu.forcePreventClose = false;
            }
            else if (!e.Button.Equals(Button))
            {
                Config[Name] = e.Button.ToString();
                Button = e.Button;
                Game1.playSound("coin");
                _listening = false;
                ModEntry.Helper.Events.Input.ButtonPressed -= OptionsInputOnButtonPressed;
                GameMenu.forcePreventClose = false;
            }
            else
                _listenerMessage = Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsElement.cs.11228");
        }

        public override void Draw(SpriteBatch b, int slotX, int slotY)
        {
            base.Draw(b, slotX, slotY);
            Utility.drawWithShadow(b, Game1.mouseCursors,
                new Vector2(_buttonBounds.X + slotX, _buttonBounds.Y + slotY),
                SetButtonSource, Color.White, 0.0f, Vector2.Zero, 4f, false, 0.15f);
            if (!_listening)
                return;
            b.Draw(Game1.staminaRect, new Rectangle(0, 0, Game1.viewport.Width, Game1.viewport.Height),
                new Rectangle(0, 0, 1, 1), Color.Black * 0.75f, 0.0f, Vector2.Zero, SpriteEffects.None, 0.999f);
            b.DrawString(Game1.dialogueFont, _listenerMessage,
                Utility.getTopLeftPositionForCenteringOnScreen(192, 64), Color.White, 0.0f, Vector2.Zero, 1f,
                SpriteEffects.None, 0.9999f);
        }
    }
}