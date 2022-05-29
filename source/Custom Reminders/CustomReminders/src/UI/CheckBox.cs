/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Dem1se/CustomReminders
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley.Menus;

namespace Dem1se.CustomReminders.UI
{
    class CheckBox
    {
        public int PosX, PosY;
        public bool IsChecked = false;
        public float Scale = 3.5f;
        public ClickableTextureComponent CheckBox_Box, CheckBox_Check;

        public CheckBox(int x, int y)
        {
            PosX = x;
            PosY = y;
            SetupUI();
        }

        private void SetupUI()
        {
            CheckBox_Box = new ClickableTextureComponent(
                new Rectangle(PosX, PosY, (int)(18 * Scale), (int)(18 * Scale)),
                Utilities.Globals.Helper.ModContent.Load<Texture2D>("assets/CheckBox_Box.png"),
                new Rectangle(),
                Scale
            );

            CheckBox_Check = new ClickableTextureComponent(
                new Rectangle(PosX + (int)(5 * Scale), PosY + (int)(6 * Scale), (int)(8 * Scale), (int)(7 * Scale)),
                Utilities.Globals.Helper.ModContent.Load<Texture2D>("assets/CheckBox_Check.png"),
                new Rectangle(),
                Scale
            );

        }

        public void receiveLeftClick(int x, int y)
        {
            if (CheckBox_Box.containsPoint(x, y))
                IsChecked = !IsChecked;
        }

        public void draw(SpriteBatch b)
        {
            CheckBox_Box.draw(b);
            if (IsChecked)
                CheckBox_Check.draw(b);
        }
    }
}
