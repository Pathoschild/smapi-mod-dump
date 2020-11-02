/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/berkayylmao/StardewValleyMods
**
*************************************************/

//
//    Copyright (C) 2020 Berkay Yigit <berkaytgy@gmail.com>
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

using System;
using System.IO;

using ChestEx.Types.BaseTypes;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChestEx.Types.CustomTypes.ChestExMenu.Items {
   public partial class ConfigPanel {
      private class ConfigButton : BaseTypes.ICustomItemGrabMenuItem.BasicComponent {
         // Protected:
         #region Protected

         // Overrides:
         #region Overrides

         protected override Texture2D GetTexture(GraphicsDevice device) {
            if (this.texture is null) {
               // create texture
               using var stream = new MemoryStream();
               //ChestEx.Properties.Resources.ConfigIcon.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
               this.texture = Texture2D.FromStream(StardewValley.Game1.graphics.GraphicsDevice, stream);

               // set bounds
               this.Bounds = new Rectangle(
                  this.Bounds.X - this.texture.Width + 8,
                  this.Bounds.Y - this.texture.Height - 8,
                  this.texture.Width,
                  this.texture.Height);
            }

            return this.texture;
         }

         #endregion

         #endregion

         // Constructors:
         #region Constructors

         public ConfigButton(ICustomItemGrabMenuItem hostMenuItem, Point point, String componentName = "", EventHandler<ICustomMenu.MouseStateEx> onMouseClick = null, String hoverText = "", BaseTypes.Colours textureTintColours = null)
            : base(hostMenuItem, point, true, componentName, onMouseClick, hoverText, textureTintColours) { }

         #endregion
      }
   }
}
