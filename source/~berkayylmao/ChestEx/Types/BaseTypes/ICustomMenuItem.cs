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
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley.Menus;

namespace ChestEx.Types.BaseTypes {
   public partial class ICustomMenuItem : ClickableComponent, IDisposable {
      // Public:
      #region Public

      public Colours Colours { get; set; }

      public Rectangle Bounds { get => this.bounds; set => this.bounds = value; }

      public List<BasicComponent> Components { get; protected set; }

      public ICustomMenu HostMenu { get; private set; }

      public Boolean IsVisible { get; protected set; }

      public Boolean RaiseMouseClickEventOnRelease { get; protected set; }

      // Virtuals:
      #region Virtuals

      /// <summary>
      /// Base implementation sets '<see cref="IsVisible"/>' to '<paramref name="isVisible"/>'.
      /// </summary>
      /// <param name="isVisible">Whether this item should be visible.</param>
      public virtual void SetVisible(Boolean isVisible) {
         this.IsVisible = isVisible;
      }

      /// <summary>
      /// Base implementation draws the dialogue box background if '<see cref="IsVisible"/>' is true and likewise with this item's components.
      /// </summary>
      public virtual void Draw(SpriteBatch b) {
         if (!this.IsVisible)
            return;

         this.Components.ForEach((c) =>
         {
            if (c.IsVisible)
               c.Draw(b);
         });
      }

      /// <summary>Base implementation informs this item's components.</summary>
      public virtual void OnGameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds) {
         this.Components.ForEach((c) => c.OnGameWindowSizeChanged(oldBounds, newBounds));
      }

      /// <summary>Base implementation does nothing.</summary>
      public virtual void OnMouseClick(ICustomMenu.MouseStateEx mouseState) { }

      /// <summary>Base implementation does nothing.</summary>
      public virtual void OnCursorMoved(StardewModdingAPI.Events.CursorMovedEventArgs e) { }

      /// <summary>Base implementation does nothing.</summary>
      public virtual void OnButtonPressed(StardewModdingAPI.Events.ButtonPressedEventArgs e) { }

      /// <summary>Base implementation does nothing.</summary>
      public virtual void OnButtonReleased(StardewModdingAPI.Events.ButtonReleasedEventArgs e) { }

      #endregion

      #endregion

      // Constructors:
      #region Constructors

      public ICustomMenuItem(ICustomMenu hostMenu, Rectangle bounds, Boolean raiseMouseClickEventOnRelease, Colours colours, StardewValley.Item svItem) : base(bounds, svItem) {
         this.Colours = colours;
         this.Components = new List<BasicComponent>();
         this.RaiseMouseClickEventOnRelease = raiseMouseClickEventOnRelease;
         this.HostMenu = hostMenu;
         this.SetVisible(true);
      }

      public ICustomMenuItem(ICustomMenu hostMenu, Rectangle bounds, Boolean raiseMouseClickEventOnRelease, Colours colours, String svItemName, String svItemLabel) : base(bounds, svItemName, svItemLabel) {
         this.Colours = colours;
         this.Components = new List<BasicComponent>();
         this.RaiseMouseClickEventOnRelease = raiseMouseClickEventOnRelease;
         this.HostMenu = hostMenu;
         this.SetVisible(true);
      }

      public ICustomMenuItem(ICustomMenu hostMenu, Rectangle bounds, Boolean raiseMouseClickEventOnRelease, Colours colours) : this(hostMenu, bounds, raiseMouseClickEventOnRelease, colours, String.Empty, String.Empty) { }

      public ICustomMenuItem(ICustomMenu hostMenu, Rectangle bounds, Boolean raiseMouseClickEventOnRelease) : this(hostMenu, bounds, raiseMouseClickEventOnRelease, Colours.Default) { }

      public ICustomMenuItem(ICustomMenu hostMenu, Rectangle bounds) : this(hostMenu, bounds, true) { }

      #endregion

      // IDisposable:
      #region IDisposable

      /// <summary>
      /// <para>Base implementation:</para>
      /// <para>1. Calls '<see cref="SetVisible(Boolean)"/>' with 'false'.</para>
      /// <para>2. Disposes of this item's components.</para>
      /// </summary>
      public virtual void Dispose() {
         // hide
         this.SetVisible(false);
         // dispose of components
         this.Components.ForEach((c) => c.Dispose());
      }

      #endregion
   }
}
