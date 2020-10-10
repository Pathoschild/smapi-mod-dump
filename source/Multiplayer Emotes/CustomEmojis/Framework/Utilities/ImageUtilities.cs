/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/FerMod/StardewMods
**
*************************************************/


using Microsoft.Xna.Framework;

namespace CustomEmojis.Framework.Utilities {

	public class ImageUtilities {

		public static Color[] GetImageData(Color[] colorData, int width, Rectangle rectangle) {
			Color[] color = new Color[rectangle.Width * rectangle.Height];
			for(int x = 0; x < rectangle.Width; x++) {
				for(int y = 0; y < rectangle.Height; y++) {
					color[x + y * rectangle.Width] = colorData[x + rectangle.X + (y + rectangle.Y) * width];
				}
			}
			return color;
		}

	}

}
