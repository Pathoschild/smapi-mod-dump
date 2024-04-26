/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/BinaryLip/TrainInfo
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.Menus;

namespace TrainInfo
{
    internal class CustomHUDMessage : HUDMessage
    {
        public string titleText;
        /// <summary>Construct an instance with the default time and an empty icon.</summary>
        /// <param name="message">The message text to show.</param>
        public CustomHUDMessage(string message) : base(message)
        {
            this.noIcon = true;
        }

        public override void draw(SpriteBatch b, int i, ref int heightUsed)
        {
            Rectangle tsarea = Game1.graphics.GraphicsDevice.Viewport.GetTitleSafeArea();
            if (noIcon)
            {
                int overrideX = tsarea.Left + 16;
                int height2 = (int)Game1.smallFont.MeasureString(message).Y + 64;
                int overrideY = ((Game1.uiViewport.Width < 1400) ? (-64) : 0) + tsarea.Bottom - height2 - heightUsed - 64;
                heightUsed += height2;
                IClickableMenu.drawHoverText(b: b, text: message, font: Game1.smallFont, overrideX: overrideX, overrideY: overrideY, alpha: transparency, boldTitleText: titleText);
                return;
            }
            else
            {
                base.draw(b, i, ref heightUsed);
            }

        }
    }
}
