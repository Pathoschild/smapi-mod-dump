using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTN.Menus
{
    internal class MTNTitleTextInputMenu : NamingMenu
    {
        public MTNTitleTextInputMenu(string title, NamingMenu.doneNamingBehavior b) : base(b, title, "")
        {
            textBox.limitWidth = false;
            textBox.Width = 512;
            textBox.X -= 128;
            randomButton.visible = false;
            ClickableTextureComponent doneNamingButton = this.doneNamingButton;
            doneNamingButton.bounds.X = doneNamingButton.bounds.X + 128;
            minLength = 0;
        }

        // Token: 0x060015AB RID: 5547 RVA: 0x0015B918 File Offset: 0x00159B18
        public override void update(GameTime time)
        {
            InputState input = (InputState)Traverse.Create(typeof(Game1)).Field("input").GetValue();
            GamePadState pad = input.GetGamePadState();
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
