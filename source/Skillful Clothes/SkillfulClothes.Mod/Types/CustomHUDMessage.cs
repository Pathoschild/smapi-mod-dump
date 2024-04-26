/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LunaticShade/StardewValley.SkillfulClothes
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillfulClothes.Types
{
    class CustomHUDMessage : HUDMessage
    {
		Color itemColor;
        Item drawForItem;

        public CustomHUDMessage(string message, Item forItem, Color itemColor, TimeSpan displayDuration) 
            : base(message)
        {
            drawForItem = forItem;
			this.itemColor = itemColor;
			this.timeLeft = (float)displayDuration.TotalMilliseconds;
		}        

        public override void draw(SpriteBatch b, int i, ref int heightUsed)
		{
			base.draw(b, i, ref heightUsed);

			Rectangle tsarea = Game1.graphics.GraphicsDevice.Viewport.GetTitleSafeArea();

			Vector2 itemBoxPosition = new Vector2(tsarea.Left + 16, tsarea.Bottom - (i + 1) * 64 * 7 / 4 - 64);
			if (Game1.isOutdoorMapSmallerThanViewport())
			{
				itemBoxPosition.X = Math.Max(tsarea.Left + 16, -Game1.uiViewport.X + 16);
			}
			if (Game1.uiViewport.Width < 1400)
			{
				itemBoxPosition.Y -= 48f;
			}
			itemBoxPosition.X += 16f;
			itemBoxPosition.Y += 16f;
			drawForItem?.drawInMenu(b, itemBoxPosition, 1f + Math.Max(0f, (timeLeft - 3000f) / 900f), transparency, 1f, StackDrawType.Hide, itemColor, true);
		}
	}
}
