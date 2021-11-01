/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Enaium-StardewValleyMods/KeyBindUI
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using EnaiumToolKit.Framework.Screen;
using EnaiumToolKit.Framework.Screen.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace KeyBindUI.Framework.Screen
{
    public class KeyPressScreen : GuiScreen
    {
        private readonly List<string> _keyList = new List<string>();

        public KeyPressScreen()
        {
            var x = Game1.uiViewport.Width / 2 - 100;
            var y = Game1.uiViewport.Height / 2 - 35;
            AddComponent(new Button(
                ModEntry.GetInstance().Helper.Translation.Get("KeyBindUI.Framework.Screen.KeyPressScreen.done"), "", x,
                y + 100, 200, 80)
            {
                OnLeftClicked = () =>
                {
                    ModEntry.GetInstance().Monitor.Log(string.Join(" + ", _keyList), LogLevel.Debug);

                    Game1.playSound("coin");
                }
            });
            AddComponent(new Button(
                ModEntry.GetInstance().Helper.Translation.Get("KeyBindUI.Framework.Screen.KeyPressScreen.cancel"), "",
                x,
                y + 200, 200, 80)
            {
                OnLeftClicked = () =>
                {
                    Game1.playSound("bigDeSelect");
                    Game1.exitActiveMenu();
                }
            });
        }

        public override void draw(SpriteBatch b)
        {
            b.Draw(Game1.staminaRect, new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height),
                new Rectangle(0, 0, 1, 1), Color.Black * 0.75f, 0.0f, Vector2.Zero, SpriteEffects.None,
                0.999f);
            b.DrawString(Game1.dialogueFont,
                _keyList.Count == 0
                    ? Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsElement.cs.11225")
                    : string.Join(" + ", _keyList.ToArray()),
                Utility.getTopLeftPositionForCenteringOnScreen(192, 64), Color.White, 0.0f, Vector2.Zero, 1f,
                SpriteEffects.None, 0.9999f);
            base.draw(b);
        }

        public override void receiveKeyPress(Keys key)
        {
            if (!_keyList.Contains(key.ToString()))
            {
                _keyList.Add(key.ToString());
            }

            base.receiveKeyPress(key);
        }
    }
}