using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;

namespace ServerBookmarker
{
    class TextMenu : NamingMenu
    {
        public TextMenu(string title, doneNamingBehavior b)
            : base(b, title, "")
        {
            base.textBox.limitWidth = false;
            base.textBox.Width = 512;
            base.textBox.X -= 128;
            base.randomButton.visible = false;
            base.doneNamingButton.bounds.X += 128;
            base.minLength = 0;
        }

        public override void update(GameTime time)
        {
            GamePadState pad = GamePad.GetState(PlayerIndex.One);
            KeyboardState keyboard = Game1.GetKeyboardState();
            if (Game1.IsPressEvent(ref pad, Buttons.B) || Game1.IsPressEvent(ref keyboard, Keys.Escape))
            {
                if (Game1.activeClickableMenu is TitleMenu)
                {
                    (Game1.activeClickableMenu as TitleMenu).backButtonPressed();
                }
                else
                {
                    Game1.exitActiveMenu();
                }
            }
            base.update(time);
        }
    }
}
