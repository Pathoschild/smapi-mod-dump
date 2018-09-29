
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
