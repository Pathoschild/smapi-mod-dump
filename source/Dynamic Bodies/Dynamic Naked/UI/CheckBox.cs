/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ribeena/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley.Menus;
using StardewValley;

namespace DynamicBodies.UI
{


	public class Checkbox : ClickableTextureComponent
	{
		public const int pixelsWide = 9;

		public bool isChecked;

		public static Rectangle sourceRectUnchecked = new Rectangle(227, 425, 9, 9);


		public Checkbox(string name, int x, int y, string label)
			: base(name, new Rectangle(x, y, sourceRectUnchecked.Width*4, sourceRectUnchecked.Height*4), label, "", Game1.mouseCursors, sourceRectUnchecked, 4f)
		{
		}

		public bool receiveLeftClick(int x, int y)
		{
			if (base.containsPoint(x, y))
			{
				Game1.playSound("drumkit6");
				this.isChecked = !this.isChecked;
				return true;
			}
			return false;
		}

		public void draw(SpriteBatch b, Color c, float layerDepth)
        {
			base.draw(b,c,layerDepth,isChecked?1:0);
        }
	}

}
