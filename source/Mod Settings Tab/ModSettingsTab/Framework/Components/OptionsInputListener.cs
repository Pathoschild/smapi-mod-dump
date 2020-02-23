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
            if (Game1.gameMode == 0)
            {
                ((TitleMenu) Game1.activeClickableMenu).backButton = null;
            }

            Helper.Events.Input.ButtonPressed +=
                OptionsInputOnButtonPressed;
            _listening = true;
            Game1.playSound("breathin");
            GameMenu.forcePreventClose = true;
            _listenerMessage = Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsElement.cs.11225");
        }

        private void OptionsInputOnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            Helper.Input.Suppress(e.Button);
            if (GreyedOut || !_listening)
                Helper.Events.Input.ButtonPressed -= OptionsInputOnButtonPressed;
            else if (e.Button == SButton.Escape)
            {
                Game1.playSound("bigDeSelect");
                Exit();
            }
            else if (!e.Button.Equals(Button))
            {
                Config[Name] = e.Button.ToString();
                Button = e.Button;
                Game1.playSound("coin");
                Exit();
            }
            else
                _listenerMessage = Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsElement.cs.11228");

            void Exit()
            {
                _listening = false;
                Helper.Events.Input.ButtonPressed -= OptionsInputOnButtonPressed;
                GameMenu.forcePreventClose = false;
                if (Game1.gameMode != 0) return;
                if (Game1.activeClickableMenu is TitleMenu menu)
                    menu.backButton = new ClickableTextureComponent(
                        menu.menuContent.LoadString("Strings\\StringsFromCSFiles:TitleMenu.cs.11739"),
                        new Rectangle(Game1.viewport.Width - 198 - 48, Game1.viewport.Height - 81 - 24, 198, 81),
                        null,
                        "", menu.titleButtonsTexture, new Rectangle(296, 252, 66, 27), 3f)
                    {
                        myID = 81114
                    };
            }
        }

        public override void Draw(SpriteBatch b, int slotX, int slotY)
        {
            base.Draw(b, slotX, slotY);
            Utility.drawWithShadow(b, Game1.mouseCursors,
                new Vector2(_buttonBounds.X + slotX, _buttonBounds.Y + slotY),
                SetButtonSource, Color.White, 0.0f, Vector2.Zero, 4f, false, 0.15f);
            if (!_listening)
                return;
            var scale = Game1.gameMode == 0 ? 1.2f : 1f;
            b.End();
            b.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null,
                Game1.gameMode == 0 ? Matrix.CreateScale(0.9f) : new Matrix?());
            b.Draw(Game1.staminaRect,
                new Rectangle(0, 0, (int) (Game1.viewport.Width * scale), (int) (Game1.viewport.Height * scale)),
                new Rectangle(0, 0, 1, 1), Color.Black * 0.75f, 0.0f, Vector2.Zero, SpriteEffects.None, 0.999f);
            b.DrawString(Game1.dialogueFont, _listenerMessage,
                Utility.getTopLeftPositionForCenteringOnScreen(192, 64), Color.White, 0.0f, Vector2.Zero, 1f,
                SpriteEffects.None, 0.9999f);
        }
    }
}