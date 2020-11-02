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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley.Objects;

namespace ChestEx.Types.CustomTypes.ExtendedSVObjects {
   public class ExtendedChest : Chest {
      // Private:
      #region Private

      private Color _targetColour {
         get { return this.playerChoiceColor.Value; }
      }

      #endregion

      // Public:
      #region Public

      // Consts:
      #region Consts

      public const Int32 CONST_CHEST_SPRITE_SIZE = 16;

      #endregion

      public Single ChestScale { get; set; }

      public Vector2 ChestSize {
         get {
            return new Vector2(
               ExtendedChest.CONST_CHEST_SPRITE_SIZE * this.ChestScale,
               ExtendedChest.CONST_CHEST_SPRITE_SIZE * this.ChestScale * 12);
         }
      }

      // Inaccessible 'Chest + bases' fields exposed through reflection properties:
      #region Inaccessible 'Chest + bases' fields exposed through reflection properties

      public Int32 sv_currentLidFrame {
         get { return Harmony.Traverse.Create(this).Field<Int32>("currentLidFrame").Value; }
         set { Harmony.Traverse.Create(this).Field<Int32>("currentLidFrame").Value = value; }
      }

      #endregion

      public void Draw(SpriteBatch b, Vector2 position, Single alpha = 1.0f) {
         var finalColour = _targetColour * alpha;

         // bottom half
         b.Draw(StardewValley.Game1.bigCraftableSpriteSheet,
                position,
                StardewValley.Game1.getSourceRectForStandardTileSheet(StardewValley.Game1.bigCraftableSpriteSheet, 168, 16, 32),
                finalColour, 0f, Vector2.Zero, this.ChestScale, SpriteEffects.None, 0.9f);

         // top half
         b.Draw(StardewValley.Game1.bigCraftableSpriteSheet,
                position,
                StardewValley.Game1.getSourceRectForStandardTileSheet(StardewValley.Game1.bigCraftableSpriteSheet, this.sv_currentLidFrame + 38, 16, 32),
                finalColour * alpha, 0f, Vector2.Zero, this.ChestScale, SpriteEffects.None, 0.9f);

         // botom 'metal hinge'
         b.Draw(StardewValley.Game1.bigCraftableSpriteSheet,
                new Vector2(position.X, (position.Y + (21 * this.ChestScale))),
                new Rectangle(0, 725, 16, 8),
                Color.White * alpha, 0f, Vector2.Zero, this.ChestScale, SpriteEffects.None, 0.91f);

         // top 'metal hinge'
         b.Draw(StardewValley.Game1.bigCraftableSpriteSheet,
                position,
                StardewValley.Game1.getSourceRectForStandardTileSheet(StardewValley.Game1.bigCraftableSpriteSheet, this.sv_currentLidFrame + 46, 16, 32),
                Color.White * alpha, 0f, Vector2.Zero, this.ChestScale, SpriteEffects.None, 0.91f);
      }

      public void Draw(SpriteBatch b, Rectangle bounds, Single alpha = 1.0f) {
         this.Draw(b, new Vector2(bounds.X, bounds.Y), alpha);
      }

      #endregion

      // Constructors:
      #region Constructors

      public ExtendedChest(Single chestScale) : base(false) {
         this.startingLidFrame.Value = 131;
         this.resetLidFrame();

         this.ChestScale = chestScale;
      }

      public ExtendedChest(Rectangle bounds) : this((Single)bounds.Width / ExtendedChest.CONST_CHEST_SPRITE_SIZE) { }

      #endregion
   }
}
