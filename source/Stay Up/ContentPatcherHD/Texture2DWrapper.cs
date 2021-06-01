/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/su226/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace Su226.ContentPatcherHD {
  class Texture2DWrapper : Texture2D {
    public Texture2D Wrapped;
    public readonly int Scale;
    public readonly string Path;
    public string Locale;

    public Texture2DWrapper(Texture2D wrapped, int scale, string path) : base(wrapped.GraphicsDevice, wrapped.Width, wrapped.Height) {
      Scale = scale;
      Path = path;
    }

    public static Texture2D ScaleUp(Texture2D texture, int scale) {
      Texture2D scaled = new Texture2D(texture.GraphicsDevice, texture.Width * scale, texture.Height * scale);
      Color[] data = new Color[texture.Width * texture.Height];
      Color[] newData = new Color[scaled.Width * scaled.Height];
      texture.GetData(data);
      for (int x = 0; x < texture.Width; x++) {
        for (int y = 0; y < texture.Height; y++) {
          for (int offsetX = 0; offsetX < scale; offsetX++) {
            for (int offsetY = 0; offsetY < scale; offsetY++) {
              newData[(y * scale + offsetY) * scaled.Width + (x * scale + offsetX)] = data[y * texture.Width + x];
            }
          }
        }
      }
      scaled.SetData(newData);
      return scaled;
    }

    public static Texture2D Crop(Texture2D texture, Rectangle area) {
      Texture2D cropped = new Texture2D(texture.GraphicsDevice, area.Width, area.Height);
      Color[] data = new Color[area.Width * area.Height];
      texture.GetData(0, area, data, 0, data.Length);
      cropped.SetData(data);
      return cropped;
    }

    public static Rectangle MultiplyRect(Rectangle rect, int scale) {
      return new Rectangle(rect.X * scale, rect.Y * scale, rect.Width * scale, rect.Height * scale);
    }

    public static void SaveDebugImage(Texture2D texture, string name) {
      if (texture is Texture2DWrapper wrapper) {
        texture = wrapper.Wrapped;
      }
      using (Stream stream = new FileStream(name, FileMode.Create)) {
        texture.SaveAsPng(stream, texture.Width, texture.Height);
      }
    }
  }
}
