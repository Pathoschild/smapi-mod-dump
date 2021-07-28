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
//    but WITHOUT ANY WARRANTY, without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//    GNU Affero General Public License for more details.
// 
//    You should have received a copy of the GNU Affero General Public License
//    along with this program. If not, see <https://www.gnu.org/licenses/>.

#endregion

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

using ChestEx.LanguageExtensions;

using Microsoft.Xna.Framework.Graphics;

using StardewValley;

namespace ChestEx.Types.BaseTypes {
  public static class TexturePresets {
    // Private:
  #region Private

    private static Byte[] sButtonBackgroundTextureBytes,
                          sConfigTextureBytes,
                          sFillStacksTextureBytes,
                          sOrganizeTextureBytes,
                          sOKTextureBytes,
                          sCancelTextureBytes,
                          sChestColouringModeTextureBytes,
                          sChestHingesColouringModeTextureBytes;

    private static Texture2D sBigCraftableSpriteSheetGrayScale,
                             sCursorsGrayScale,
                             sMenuTextureGrayScale,
                             sVerticalGradient;

  #endregion

    // Public:  
  #region Public

    /// <summary>
    /// <para>Size: 16x16</para>
    /// <para>Needs to be disposed.</para>
    /// </summary>
    public static Texture2D gButtonBackgroundTexture {
      get {
        if (Game1.graphics.GraphicsDevice is null) return null;

        sButtonBackgroundTextureBytes ??=
          Convert.FromBase64String(@"iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAABQSURBVDhPY2QAgsTEsv8gmlQwf34XI1jzw4df/v/584ckDNID0ssEMklamoPh6dMfJGGQHhAAG0AJGDVg1AAQGCQGwNI2KRikBwQozM5djAAOtG1Zzkf68AAAAABJRU5ErkJggg==");

        using var ms = new MemoryStream();
        ms.Write(sButtonBackgroundTextureBytes, 0, sButtonBackgroundTextureBytes.Length);
        ms.Seek(0, SeekOrigin.Begin);

        return Texture2D.FromStream(Game1.graphics.GraphicsDevice, ms);
      }
    }

    /// <summary>
    /// <para>Size: 16x16</para>
    /// <para>Needs to be disposed.</para>
    /// </summary>
    public static Texture2D gConfigButtonTexture {
      get {
        if (Game1.graphics.GraphicsDevice is null) return null;

        sConfigTextureBytes ??=
          Convert.FromBase64String(@"iVBORw0KGgoAAAANSUhEUgAAABAAAAAQBAMAAADt3eJSAAAABGdBTUEAALGPC/xhBQAAAAFzUkdCAK7OHOkAAAASUExURQAAAOHh9GFhdvz8/G5ublFRUaJ6VgoAAAABdFJOUwBA5thmAAAAPUlEQVQI12NgUgIDBQZFYzAQYlAWBAMjFIYLjCHiCGE4whghLlCGaKgLTFeII5Qh6gIzJwTNQAgDbinMGQCNWRBCnlBNtgAAAABJRU5ErkJggg==");

        using var ms = new MemoryStream();
        ms.Write(sConfigTextureBytes, 0, sConfigTextureBytes.Length);
        ms.Seek(0, SeekOrigin.Begin);

        return Texture2D.FromStream(Game1.graphics.GraphicsDevice, ms);
      }
    }

    /// <summary>
    /// <para>Size: 16x16</para>
    /// <para>Needs to be disposed.</para>
    /// </summary>
    public static Texture2D gFillStacksPickerButtonTexture {
      get {
        if (Game1.graphics.GraphicsDevice is null) return null;

        sFillStacksTextureBytes ??=
          Convert.FromBase64String(@"iVBORw0KGgoAAAANSUhEUgAAABAAAAAQBAMAAADt3eJSAAAABGdBTUEAALGPC/xhBQAAAAFzUkdCAK7OHOkAAAAPUExURQAAAGFhduHh9Pz8/L+/8rk6gBsAAAABdFJOUwBA5thmAAAAT0lEQVQI12NgFAQDAQYhYzBQZBBWAgNDBmFFQ0NDYyMQAwiUYAwnMAOkSQjIUHFxFHFxMgTpEhQE61ICygmBGUrChkpoDJgaqBVwS2HOAADBTxCpfWz0QwAAAABJRU5ErkJggg==");

        using var ms = new MemoryStream();
        ms.Write(sFillStacksTextureBytes, 0, sFillStacksTextureBytes.Length);
        ms.Seek(0, SeekOrigin.Begin);

        return Texture2D.FromStream(Game1.graphics.GraphicsDevice, ms);
      }
    }

    /// <summary>
    /// <para>Size: 16x16</para>
    /// <para>Needs to be disposed.</para>
    /// </summary>
    public static Texture2D gOrganizeButtonTexture {
      get {
        if (Game1.graphics.GraphicsDevice is null) return null;

        sOrganizeTextureBytes ??=
          Convert.FromBase64String(@"iVBORw0KGgoAAAANSUhEUgAAABAAAAAQBAMAAADt3eJSAAAABGdBTUEAALGPC/xhBQAAAAFzUkdCAK7OHOkAAAASUExURQAAAGFhdvz8/OHh9L+/8uXl5SMhPUcAAAABdFJOUwBA5thmAAAAO0lEQVQI12NgFAQDAQZhJTAwZBAyBgNFIAMkA2IYBikpqQqDGCAlwigiJi4uzqhSuEXgBsKsgFsKcwYA1cIRVgwGxWEAAAAASUVORK5CYII=");

        using var ms = new MemoryStream();
        ms.Write(sOrganizeTextureBytes, 0, sOrganizeTextureBytes.Length);
        ms.Seek(0, SeekOrigin.Begin);

        return Texture2D.FromStream(Game1.graphics.GraphicsDevice, ms);
      }
    }

    /// <summary>
    /// <para>Size: 64x64</para>
    /// <para>Needs to be disposed.</para>
    /// </summary>
    public static Texture2D gOKButtonTexture {
      get {
        if (Game1.graphics.GraphicsDevice is null) return null;

        sOKTextureBytes ??=
          Convert.FromBase64String(@"iVBORw0KGgoAAAANSUhEUgAAAEAAAABABAMAAABYR2ztAAAABGdBTUEAALGPC/xhBQAAAAFzUkdCAK7OHOkAAAAVUExURQAAAOHh9GFhdvz8/Do6db+/8uXl5Y3YrVYAAAABdFJOUwBA5thmAAAAXElEQVRIx2NgYFDCAxhAgOYKlJQEBY1xAEFBoBK6KAAxsANj45GnwAUMIKzQ0IFQAJIEsUBKIApArJGpACYMUpqWNtAKYNFGfwWj6QFbxhkIBaPl5MDXWYOg7gYA0zkxADXkwuUAAAAASUVORK5CYII=");

        using var ms = new MemoryStream();
        ms.Write(sOKTextureBytes, 0, sOKTextureBytes.Length);
        ms.Seek(0, SeekOrigin.Begin);

        return Texture2D.FromStream(Game1.graphics.GraphicsDevice, ms);
      }
    }

    /// <summary>
    /// <para>Size: 16x16</para>
    /// <para>Needs to be disposed.</para>
    /// </summary>
    public static Texture2D gCancelButtonTexture {
      get {
        if (Game1.graphics.GraphicsDevice is null) return null;

        sCancelTextureBytes ??=
          Convert.FromBase64String(@"iVBORw0KGgoAAAANSUhEUgAAABAAAAAQBAMAAADt3eJSAAAABGdBTUEAALGPC/xhBQAAAAFzUkdCAK7OHOkAAAAbUExURQAAAFsrKmFhduHh9NQyAPz8/PLy+f9dK/n5+/r5U4UAAAABdFJOUwBA5thmAAAAYklEQVQI1zXOMQqAMAwF0OANQnIAsSCuaU5QCHQXvIK4Zur1bVP901vyf2DZIiuke+TJsKsgkp4d3JoPiCFW6mBzZR/gShJA8wniihNiHijKVr5zoyg0K73w+CfSFcnwv/ECuDYa5O4JC2YAAAAASUVORK5CYII=");

        using var ms = new MemoryStream();
        ms.Write(sCancelTextureBytes, 0, sCancelTextureBytes.Length);
        ms.Seek(0, SeekOrigin.Begin);

        return Texture2D.FromStream(Game1.graphics.GraphicsDevice, ms);
      }
    }

    /// <summary>
    /// <para>Size: 12x12</para>
    /// <para>Needs to be disposed.</para>
    /// </summary>
    public static Texture2D gChestColouringModeTexture {
      get {
        if (Game1.graphics.GraphicsDevice is null) return null;

        sChestColouringModeTextureBytes ??=
          Convert.FromBase64String(@"iVBORw0KGgoAAAANSUhEUgAAAAwAAAAMBAMAAACkW0HUAAAABGdBTUEAALGPC/xhBQAAAAFzUkdCAK7OHOkAAAAhUExURQ8PDxQUFAsLCwAAAAYGBigoKEhISIuLi1dXV3Z2dqKiovEDvl0AAAAFdFJOU8jIyADIovTuOAAAAE9JREFUCNdjMHZJS3MxZjBjmDmTIZkhU7C8XHAaQ7rgqlWCZTAqkqG8nGEqQ6hLkKpLKEOYUgKbUipDBEMrEAGptgwgFabU0QEUNA0FgmAAv+sWlVrG+HEAAAAASUVORK5CYII=");

        using var ms = new MemoryStream();
        ms.Write(sChestColouringModeTextureBytes, 0, sChestColouringModeTextureBytes.Length);
        ms.Seek(0, SeekOrigin.Begin);

        return Texture2D.FromStream(Game1.graphics.GraphicsDevice, ms);
      }
    }

    /// <summary>
    /// <para>Size: 12x12</para>
    /// <para>Needs to be disposed.</para>
    /// </summary>
    public static Texture2D gChestHingesColouringModeTexture {
      get {
        if (Game1.graphics.GraphicsDevice is null) return null;

        sChestHingesColouringModeTextureBytes ??=
          Convert.FromBase64String(@"iVBORw0KGgoAAAANSUhEUgAAAAwAAAAMBAMAAACkW0HUAAAABGdBTUEAALGPC/xhBQAAAAFzUkdCAK7OHOkAAAAhUExURQQEBAcHBw4ODgkJCQAAAAwMDBAQEJGRkc3NzW5ubj09PSd/Dv0AAAAHdFJOU8jIyMgAyMirLdVaAAAAT0lEQVQI12NwWSUouMqFwbE8NLRchEG0Q0mpI5BBqCMtrUMRRrGWKymVBzAwrOKcsIqBgXGmeOFMAQbmcgMgAlKGwkCKcaaxMVDQgQEIWACZKxOpVheo4wAAAABJRU5ErkJggg==");

        using var ms = new MemoryStream();
        ms.Write(sChestHingesColouringModeTextureBytes, 0, sChestHingesColouringModeTextureBytes.Length);
        ms.Seek(0, SeekOrigin.Begin);

        return Texture2D.FromStream(Game1.graphics.GraphicsDevice, ms);
      }
    }

    /// <summary>
    /// <para>Size: big</para>
    /// <para>DO NOT DISPOSE.</para>
    /// </summary>
    public static Texture2D gBigCraftableSpriteSheetGrayScale {
      get {
        if (Game1.graphics.GraphicsDevice is null && sBigCraftableSpriteSheetGrayScale is null) return null;

        sBigCraftableSpriteSheetGrayScale ??= Game1.bigCraftableSpriteSheet.ToGrayScale(Game1.graphics.GraphicsDevice);
        return sBigCraftableSpriteSheetGrayScale;
      }
    }

    /// <summary>
    /// <para>Size: big</para>
    /// <para>DO NOT DISPOSE.</para>
    /// </summary>
    public static Texture2D gCursorsGrayScale {
      get {
        if (Game1.graphics.GraphicsDevice is null && sCursorsGrayScale is null) return null;

        sCursorsGrayScale ??= Game1.mouseCursors.ToGrayScale(Game1.graphics.GraphicsDevice);
        return sCursorsGrayScale;
      }
    }

    /// <summary>
    /// <para>Size: big</para>
    /// <para>DO NOT DISPOSE.</para>
    /// </summary>
    public static Texture2D gMenuTextureGrayScale {
      get {
        if (Game1.graphics.GraphicsDevice is null && sMenuTextureGrayScale is null) return null;

        sMenuTextureGrayScale ??= Game1.menuTexture.ToGrayScale(Game1.graphics.GraphicsDevice);
        return sMenuTextureGrayScale;
      }
    }

    /// <summary>
    /// <para>Size: 1x128. (Darker towards bottom)</para>
    /// <para>DO NOT DISPOSE.</para>
    /// </summary>
    public static Texture2D gVerticalGradient {
      get {
        if (Game1.graphics.GraphicsDevice is null && sVerticalGradient is null) return null;
        if (sVerticalGradient is null) {
          using var      bitmap         = new Bitmap(1, 128);
          using var      ms             = new MemoryStream();
          using Graphics g              = Graphics.FromImage(bitmap);
          using var      black_gradient = new LinearGradientBrush(new Rectangle(0, 0, 1, 128), Color.White, Color.FromArgb(200, 200, 200), LinearGradientMode.Vertical);
          g.FillRectangle(black_gradient, 0, 0, 1, 128);
          bitmap.Save(ms, ImageFormat.Png);
          ms.Seek(0, SeekOrigin.Begin);
          sVerticalGradient = Texture2D.FromStream(Game1.graphics.GraphicsDevice, ms);
        }

        return sVerticalGradient;
      }
    }

  #endregion
  }
}
