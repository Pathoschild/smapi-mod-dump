using Microsoft.Xna.Framework.Graphics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace MapPings.Framework.Utilities {

	public static class ImageUtilities {

		public static Image Texture2Image(Texture2D texture) {
			Image img;
			using(MemoryStream memoryStream = new MemoryStream()) {
				texture.SaveAsPng(memoryStream, texture.Width, texture.Height);
				memoryStream.Seek(0, SeekOrigin.Begin);
				img = Image.FromStream(memoryStream);
			}
			return img;
		}

		public static Bitmap MakeGrayscale(Bitmap original) {

			//create a blank bitmap the same size as original
			Bitmap newBitmap = new Bitmap(original.Width, original.Height);

			//get a graphics object from the new image
			using(Graphics graphics = Graphics.FromImage(newBitmap)) {

				//create the grayscale ColorMatrix
				ColorMatrix colorMatrix = new ColorMatrix(
					new float[][]{
					new float[] {.3f, .3f, .3f, 0, 0},
					new float[] {.59f, .59f, .59f, 0, 0},
					new float[] {.11f, .11f, .11f, 0, 0},
					new float[] {0, 0, 0, 1, 0},
					new float[] {0, 0, 0, 0, 1}
					}
				);

				//create some image attributes
				ImageAttributes attributes = new ImageAttributes();

				//set the color matrix attribute
				attributes.SetColorMatrix(colorMatrix);

				//draw the original image on the new image
				//using the grayscale color matrix
				graphics.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height), 0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);

			}

			return newBitmap;
		}

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
