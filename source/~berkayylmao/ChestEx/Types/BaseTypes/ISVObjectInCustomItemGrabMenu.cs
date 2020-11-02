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

using ChestEx.LanguageExtensions;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChestEx.Types.BaseTypes {
   public class ISVObjectInCustomItemGrabMenu : ICustomItemGrabMenuItem.BasicComponent {
      // Protected:
      #region Protected

      protected Boolean objectDrawShadow;
      protected Single objectScale;


      // Virtuals:
      #region Virtuals

      /// <summary>
      /// Base implementation draws the hover text via '<see cref="ICustomMenu"/>.DrawHoverText()'.
      /// </summary>
      protected virtual void drawHoverText(SpriteBatch b) {
         ICustomMenu.DrawHoverText(b,
                                   StardewValley.Game1.smallFont,
                                   this.hoverText,
                                   (8, 8, 8, 8),
                                   this.HostMenuItem.Colours.BackgroundColour,
                                   this.HostMenuItem.Colours.ForegroundColour);
      }

      /// <summary>
      /// Base implementation draws the object via '<see cref="StardewValley.Item.drawInMenu(SpriteBatch, Vector2, Single, Single, Single, StardewValley.StackDrawType, Color, Boolean)"/>'.
      /// </summary>
      protected virtual void drawObject(SpriteBatch b) {
         this.SVObject.drawInMenu(b,
                                  this.Bounds.ExtractXYAsXNAVector2(),
                                  this.objectScale,
                                  this.textureTintColourCurrent.A / 255.0f,
                                  1.0f,
                                  StardewValley.StackDrawType.Hide,
                                  this.textureTintColourCurrent,
                                  this.objectDrawShadow);
      }

      /// <summary>Base implementation does nothing.</summary>
      protected virtual void playObjectAnimation() { }

      /// <summary>Base implementation does nothing.</summary>
      protected virtual void revertObjectAnimation() { }

      #endregion

      #endregion

      // Public:
      #region Public

      public StardewValley.Object SVObject { get; set; }

      public T GetSVObjectAs<T>() where T : StardewValley.Object {
         if (this.SVObject is T)
            return this.SVObject as T;

         return null;
      }

      // Overrides:
      #region Overrides

      /// <summary>
      /// <para>Returns if this object is not visible.</para>
      /// <para>1. Draws object via '<see cref="drawObject(SpriteBatch)"/>'.</para>
      /// <para>2. Draws hover text.</para>
      /// </summary>
      /// <param name="b"></param>
      public override void Draw(SpriteBatch b) {
         if (!this.IsVisible)
            return;

         this.drawObject(b);

         if (this.cursorStatus != CursorStatus.None && !String.IsNullOrWhiteSpace(this.hoverText))
            this.drawHoverText(b);
      }

      /// <summary>
      /// Calls '<see cref="playObjectAnimation()"/>' if cursor has activity on this object, '<see cref="revertObjectAnimation()"/>' if not.
      /// </summary>
      public override void OnCursorMoved(StardewModdingAPI.Events.CursorMovedEventArgs e) {
         base.OnCursorMoved(e);

         if (this.cursorStatus != CursorStatus.None)
            this.playObjectAnimation();
         else
            this.revertObjectAnimation();
      }

      #endregion

      #endregion

      // Constructors:
      #region Constructors

      public ISVObjectInCustomItemGrabMenu(ICustomItemGrabMenuItem hostMenuItem,
                                     Rectangle bounds,
                                     StardewValley.Object svObject,
                                     Boolean svObjectDrawShadow = false,
                                     Single svObjectScale = -1.0f,
                                     String componentName = "",
                                     EventHandler<ICustomMenu.MouseStateEx> onMouseClick = null,
                                     String hoverText = "",
                                     Colours textureTintColours = null)
         : base(hostMenuItem, bounds, true, componentName, onMouseClick, hoverText, textureTintColours) {
         this.objectDrawShadow = svObjectDrawShadow;
         this.objectScale = svObjectScale;

         this.SVObject = svObject;
      }

      #endregion
   }
}
