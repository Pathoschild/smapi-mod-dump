/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jaredlll08/Item-Tooltips
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace Item_Tooltips
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            helper.Events.Display.RenderingHud += DisplayOnRenderingHud;
        }

        private int _yValue;
        private int _fade;
        private int _fadeOut;
        private const int FadeMax = 90;
        private Item _item;

        private void DisplayOnRenderingHud(object sender, RenderingHudEventArgs e)
        {
            if (Game1.CurrentEvent != null)
            {
                return;
            }

            if (Game1.activeClickableMenu != null)
                return;
            var center = Game1.player.GetBoundingBox().Center;
            var globalPosition = new Vector2(center.X, center.Y);
            var local = Game1.GlobalToLocal(Game1.viewport, globalPosition);
            bool flag;
            if (Game1.options.pinToolbarToggle)
            {
                flag = false;
            }
            else
            {
                flag = local.Y > (double) (Game1.viewport.Height / 2 + 64);
            }

            var num = Utility.makeSafeMarginY(8);
            if (!flag)
            {
                _yValue = Game1.viewport.Height;
                _yValue += 8;
                _yValue -= num;
                _yValue -= 16;
                _yValue -= 24;
                _yValue -= 8;
            }
            else
            {
                _yValue = 112;
                _yValue -= 8;
                _yValue += num;
                _yValue += 96 + 8;
            }

            if (_item == null || _item != Game1.player.CurrentItem)
            {
                _item = Game1.player.CurrentItem;
                _fade = FadeMax;
                _fadeOut = FadeMax/3;
            }

            if (_fade <= 0 && _fadeOut <= 0 || _item == null) return;
            _fade--;

            if (_fade <= 0)
            {
                _fadeOut--;
            }

            var currentItemDisplayName = Game1.player.CurrentItem.DisplayName;
            var x = Game1.viewport.Width / 2 - Game1.dialogueFont.MeasureString(currentItemDisplayName).Length() / 2;

            var black = Color.Black;
            var white = Color.White;


            var cutLength = (int)Remap(_fadeOut, FadeMax/3, 0, currentItemDisplayName.Length, 0);


            Game1.spriteBatch.DrawString(Game1.dialogueFont, currentItemDisplayName.Substring(0,cutLength), new Vector2(x - 4, _yValue - 96 - 8), black);
            Game1.spriteBatch.DrawString(Game1.dialogueFont, currentItemDisplayName.Substring(0,cutLength), new Vector2(x + 4, _yValue - 96 - 8), black);
            Game1.spriteBatch.DrawString(Game1.dialogueFont, currentItemDisplayName.Substring(0,cutLength), new Vector2(x, _yValue - 96 - 4), black);
            Game1.spriteBatch.DrawString(Game1.dialogueFont, currentItemDisplayName.Substring(0,cutLength), new Vector2(x, _yValue - 96 - 12), black);

            Game1.spriteBatch.DrawString(Game1.dialogueFont, currentItemDisplayName.Substring(0,cutLength), new Vector2(x, _yValue - 96 - 8), white);
        }

        public static float Remap (float value, float from1, float to1, float from2, float to2) {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }
    }
}