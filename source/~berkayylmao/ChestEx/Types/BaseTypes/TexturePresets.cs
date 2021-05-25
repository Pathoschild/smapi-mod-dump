/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/berkayylmao/StardewValleyMods
**
*************************************************/

#region License

// clang-format off
// 
//    ChestEx (StardewValleyMods)
//    Copyright (c) 2021 Berkay Yigit <berkaytgy@gmail.com>
// 
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU Affero General Public License as published
//    by the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
// 
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU Affero General Public License for more details.
// 
//    You should have received a copy of the GNU Affero General Public License
//    along with this program. If not, see <https://www.gnu.org/licenses/>.
// 
// clang-format on

#endregion

using System;
using System.IO;

using ChestEx.LanguageExtensions;

using Microsoft.Xna.Framework.Graphics;

using StardewValley;

namespace ChestEx.Types.BaseTypes {
  public static class TexturePresets {
    // Private:
  #region Private

    private static Byte[] sButtonBackgroundTextureBytes,
                          sFillStacksTextureBytes,
                          sOrganizeTextureBytes,
                          sOKTextureBytes,
                          sCancelTextureBytes;

    private static Texture2D sBigCraftableSpriteSheetGrayScale;

  #endregion

    // Public:  
  #region Public

    // 16x16
    public static Texture2D gButtonBackgroundTexture {
      get {
        if (Game1.graphics.GraphicsDevice is null) return null;

        sButtonBackgroundTextureBytes ??=
          Convert.FromBase64String(@"iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAABQSURBVDhPY2QAgsTEsv8gmlQwf34XI1jzw4df/v/584ckDNID0ssEMklamoPh6dMfJGGQHhAAG0AJGDVg1AAQGCQGwNI2KRikBwQozM5djAAOtG1Zzkf68AAAAABJRU5ErkJggg==");

        using var ms = new MemoryStream();

        // create base button texture
        ms.Write(sButtonBackgroundTextureBytes, 0, sButtonBackgroundTextureBytes.Length);
        ms.Seek(0, SeekOrigin.Begin);

        return Texture2D.FromStream(Game1.graphics.GraphicsDevice, ms);
      }
    }

    // 16x16
    public static Texture2D gFillStacksPickerButtonTexture {
      get {
        if (Game1.graphics.GraphicsDevice is null) return null;

        sFillStacksTextureBytes ??=
          Convert.FromBase64String(@"iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAMAAAAoLQ9TAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAPUExURWFhduHh9Pz8/L+/8gAAAL2YKuwAAAAFdFJOU/////8A+7YOUwAAAAlwSFlzAAAOwQAADsEBuJFr7QAAAEtJREFUKFNtjwEKACEIBNet/7/5NvUyoxECpxUREw21xgOTox3QBRilQIosu8QosUkxBLDeFMK/95bo3VSCbImHiIkSP0vcx/XzMT+FywD5PU4oXwAAAABJRU5ErkJggg==");

        using var ms = new MemoryStream();

        // create base button texture
        ms.Write(sFillStacksTextureBytes, 0, sFillStacksTextureBytes.Length);
        ms.Seek(0, SeekOrigin.Begin);

        return Texture2D.FromStream(Game1.graphics.GraphicsDevice, ms);
      }
    }

    // 16x16
    public static Texture2D gOrganizeButtonTexture {
      get {
        if (Game1.graphics.GraphicsDevice is null) return null;

        sOrganizeTextureBytes ??=
          Convert.FromBase64String(@"iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAMAAAAoLQ9TAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAASUExURWFhduHh9Pz8/OXl5b+/8gAAABqBQdoAAAAGdFJOU///////ALO/pL8AAAAJcEhZcwAADsAAAA7AAWrWiQkAAABFSURBVChTlY9BDgAgCMNgyv+/7IRoUOLBnqAZCROTA66KhNJBEwjhYbKEtMg3noeIHdiiJLQ7HF4n/4nyWGKKu9xZX2wArg0BS38SMcwAAAAASUVORK5CYII=");

        using var ms = new MemoryStream();

        // create base button texture
        ms.Write(sOrganizeTextureBytes, 0, sOrganizeTextureBytes.Length);
        ms.Seek(0, SeekOrigin.Begin);

        return Texture2D.FromStream(Game1.graphics.GraphicsDevice, ms);
      }
    }

    // 64x64
    public static Texture2D gOKButtonTexture {
      get {
        if (Game1.graphics.GraphicsDevice is null) return null;

        sOKTextureBytes ??=
          Convert.FromBase64String(@"iVBORw0KGgoAAAANSUhEUgAAAEAAAABACAMAAACdt4HsAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAVUExURWFhduHh9Pz8/Do6db+/8uXl5QAAADi66YEAAAAHdFJOU////////wAaSwNGAAAACXBIWXMAAA7BAAAOwQG4kWvtAAAAkUlEQVRYR+2XOw7DMAzF0p/vf+SKgggUmep2smIukZ78uOcYwfEj2W0ggFtwn4ROlqGBwGAGOlXfAmgmeHxQUWbPoNakqyBbgRmzuQIz7lkGFo9gxmy+BcsJzo/58u4VmF1RIOzQXWDGbH6Wcs8ysHgEM2bzLVhGADyQii4n+JYt6CcwmIFO1ZcX/P3jubpgjDce4hdxPy6UBwAAAABJRU5ErkJggg==");

        using var ms = new MemoryStream();

        // create base button texture
        ms.Write(sOKTextureBytes, 0, sOKTextureBytes.Length);
        ms.Seek(0, SeekOrigin.Begin);

        return Texture2D.FromStream(Game1.graphics.GraphicsDevice, ms);
      }
    }

    // 16x16
    public static Texture2D gCancelButtonTexture {
      get {
        if (Game1.graphics.GraphicsDevice is null) return null;

        sCancelTextureBytes ??=
          Convert.FromBase64String(@"iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAMAAAAoLQ9TAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAbUExURWFhduHh9PLy+fn5+1srKvz8/NQyAP9dKwAAABAisJcAAAAJdFJOU///////////AFNPeBIAAAAJcEhZcwAADsIAAA7CARUoSoAAAABaSURBVChTXc9RFkARCATQIrH/Fb+ZcsibH7o6SJY8QantpiusqapFsBkJ5pOBJJg7GxwSgJp79tkGUMiFlAK8ZlqFlANbsAacZ+EJeRp9hP7/uo4SDPeOL+sDm60DIWIqx+sAAAAASUVORK5CYII=");

        using var ms = new MemoryStream();

        // create base button texture
        ms.Write(sCancelTextureBytes, 0, sCancelTextureBytes.Length);
        ms.Seek(0, SeekOrigin.Begin);

        return Texture2D.FromStream(Game1.graphics.GraphicsDevice, ms);
      }
    }

    // Do not dispose!
    public static Texture2D gBigCraftableSpriteSheetGrayScale {
      get {
        if (Game1.graphics.GraphicsDevice is null) return null;

        sBigCraftableSpriteSheetGrayScale ??= Game1.bigCraftableSpriteSheet.ToGrayScale(Game1.graphics.GraphicsDevice);
        return sBigCraftableSpriteSheetGrayScale;
      }
    }

  #endregion
  }
}
