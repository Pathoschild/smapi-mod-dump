using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;


namespace ItemTooltips
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            GraphicsEvents.OnPostRenderHudEvent += GraphicsEventsOnOnPostRenderHudEvent;
        }

        private int _yValue;
        private int _fade;
        private const int FadeMax = 90;
        private Item _item;

        private void GraphicsEventsOnOnPostRenderHudEvent(object sender, EventArgs eventArgs)
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
            }

            if (_fade <= 0 || _item == null) return;
            _fade--;
            var currentItemDisplayName = Game1.player.CurrentItem.DisplayName;
            var x = Game1.viewport.Width / 2 - Game1.dialogueFont.MeasureString(currentItemDisplayName).Length() / 2;

            Game1.spriteBatch.DrawString(Game1.dialogueFont, currentItemDisplayName, new Vector2(x-4, _yValue - 96 - 8), Color.Black);
            Game1.spriteBatch.DrawString(Game1.dialogueFont, currentItemDisplayName, new Vector2(x+4, _yValue - 96 - 8), Color.Black);
            Game1.spriteBatch.DrawString(Game1.dialogueFont, currentItemDisplayName, new Vector2(x, _yValue - 96 - 4), Color.Black);
            Game1.spriteBatch.DrawString(Game1.dialogueFont, currentItemDisplayName, new Vector2(x, _yValue - 96 - 12), Color.Black);

            


            Game1.spriteBatch.DrawString(Game1.dialogueFont, currentItemDisplayName, new Vector2(x, _yValue - 96 - 8), Color.White);

        }
    }
}