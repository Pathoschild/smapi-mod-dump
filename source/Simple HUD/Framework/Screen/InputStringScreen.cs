/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Enaium-StardewValleyMods/SimpleHUD
**
*************************************************/

using System;
using EnaiumToolKit.Framework.Screen;
using EnaiumToolKit.Framework.Screen.Components;
using Microsoft.Xna.Framework.Input;
using StardewValley;

namespace SimpleHUD.Framework.Screen
{
    public class InputStringScreen : GuiScreen
    {
        private readonly Action<string> _input;

        public InputStringScreen(Action<string> input)
        {
            _input = input;
        }

        protected override void Init()
        {
            var x = Game1.uiViewport.Width / 2 - 100;
            var y = Game1.uiViewport.Height / 2 - 35;
            var name = new TextField("",
                Get("input.textField.description"), x,
                y, 200, 70);
            AddComponent(name);
            var done = new Button(Get("input.button.done.title"),
                Get("input.button.done.title"), x, y + 100, 200, 80)
            {
                OnLeftClicked = () =>
                {
                    _input(name.Text);
                    Game1.exitActiveMenu();
                }
            };

            AddComponent(done);
            AddComponent(new Button(Get("input.button.cancel.title"),
                Get("input.button.cancel.title"), x, y + 200, 200, 80)
            {
                OnLeftClicked = Game1.exitActiveMenu
            });
            base.Init();
        }

        private string Get(string key)
        {
            return ModEntry.GetInstance().Helper.Translation.Get(key);
        }

        public override void receiveKeyPress(Keys key)
        {
            if (key != Game1.options.menuButton[Game1.options.menuButton.Length - 1].key)
                return;
            base.receiveKeyPress(key);
        }
    }
}