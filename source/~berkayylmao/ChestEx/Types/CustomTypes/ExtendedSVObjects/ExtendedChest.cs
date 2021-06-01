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
using System.Diagnostics.CodeAnalysis;

using ChestEx.LanguageExtensions;
using ChestEx.Types.BaseTypes;

using Harmony;

using JetBrains.Annotations;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI.Events;

using StardewValley;
using StardewValley.Objects;

using Object = System.Object;

namespace ChestEx.Types.CustomTypes.ExtendedSVObjects {
  public class ExtendedChest : Chest {
    // Public:
  #region Public

    // Consts:
  #region Consts

    public const Int32 CONST_CHEST_SPRITE_SIZE = 16;

  #endregion

    // Enums:
  #region Enums

    public enum ChestType {
      None,
      WoodenChest,
      StoneChest,
      Fridge
    }

  #endregion

    public Color mChestColour {
      get => this.playerChoiceColor.Value;
      set => this.playerChoiceColor.Value = value;
    }

    public Color mHingesColour { get; set; }

    public String mChestName { get; set; }

    public Single mChestScale { get; set; }

    public ChestType mChestType { get; set; }

    public Vector2 mChestSize => new(CONST_CHEST_SPRITE_SIZE * this.mChestScale, CONST_CHEST_SPRITE_SIZE * this.mChestScale * 12);

    // Inaccessible 'Chest + bases' fields exposed through reflection properties:
  #region Inaccessible 'Chest + bases' fields exposed through reflection properties

    public Int32 mSVCurrentLidFrame {
      get => Traverse.Create(this).Field<Int32>("currentLidFrame").Value;
      set => Traverse.Create(this).Field<Int32>("currentLidFrame").Value = value;
    }

  #endregion

    public void Draw(SpriteBatch b, Vector2 position, Single alpha = 1.0f) {
      if (this.playerChoiceColor.Value.Equals(Color.Black)) {
        b.Draw(Game1.bigCraftableSpriteSheet,
               position,
               Game1.getSourceRectForStandardTileSheet(Game1.bigCraftableSpriteSheet, this.ParentSheetIndex, 16, 32),
               Color.White * alpha,
               0f,
               Vector2.Zero,
               this.mChestScale,
               SpriteEffects.None,
               0.9f);
        b.Draw(Game1.bigCraftableSpriteSheet,
               position,
               Game1.getSourceRectForStandardTileSheet(Game1.bigCraftableSpriteSheet, this.mSVCurrentLidFrame, 16, 32),
               Color.White * alpha * alpha,
               0f,
               Vector2.Zero,
               this.mChestScale,
               SpriteEffects.None,
               0.91f);

        return;
      }

      Color chest_colour = Color.FromNonPremultiplied(this.mChestColour.R, this.mChestColour.G, this.mChestColour.B, (Int32)(this.mChestColour.A * alpha));

      // bottom half
      b.Draw(Game1.bigCraftableSpriteSheet,
             position,
             Game1.getSourceRectForStandardTileSheet(Game1.bigCraftableSpriteSheet, this.mChestType == ChestType.WoodenChest ? 168 : 232, 16, 32),
             chest_colour,
             0f,
             Vector2.Zero,
             this.mChestScale,
             SpriteEffects.None,
             0.9f);

      // top half
      b.Draw(Game1.bigCraftableSpriteSheet,
             position,
             Game1.getSourceRectForStandardTileSheet(Game1.bigCraftableSpriteSheet, this.mSVCurrentLidFrame + (this.mChestType == ChestType.WoodenChest ? 38 : 0), 16, 32),
             chest_colour,
             0f,
             Vector2.Zero,
             this.mChestScale,
             SpriteEffects.None,
             0.9f);

      Texture2D hinges_texture = this.mHingesColour == Color.Black ? Game1.bigCraftableSpriteSheet : TexturePresets.gBigCraftableSpriteSheetGrayScale;
      Color hinges_colour = this.mHingesColour == Color.Black ?
        Color.White :
        Color.FromNonPremultiplied(this.mHingesColour.R, this.mHingesColour.G, this.mHingesColour.B, (Int32)(this.mHingesColour.A * alpha));

      // bottom 'metal hinge'
      b.Draw(hinges_texture,
             new Vector2(position.X, position.Y + 21 * this.mChestScale),
             new Rectangle(0, (this.mChestType == ChestType.WoodenChest ? 168 : 232) / 8 * 32 + 53, 16, 8),
             hinges_colour,
             0f,
             Vector2.Zero,
             this.mChestScale,
             SpriteEffects.None,
             0.91f);

      // top 'metal hinge'
      b.Draw(hinges_texture,
             position,
             Game1.getSourceRectForStandardTileSheet(Game1.bigCraftableSpriteSheet, this.mSVCurrentLidFrame + (this.mChestType == ChestType.WoodenChest ? 46 : 8), 16, 32),
             hinges_colour,
             0f,
             Vector2.Zero,
             this.mChestScale,
             SpriteEffects.None,
             0.91f);
    }

    public void Draw(SpriteBatch b, Rectangle bounds, Single alpha = 1.0f) { this.Draw(b, bounds.ExtractXYAsXNAVector2(), alpha); }

  #region Statics

    [UsedImplicitly]
    [EventPriority(EventPriority.High + 1000)]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static Boolean BeforeDraw(SpriteBatch spriteBatch, Int32 x, Int32 y, Single alpha,
                                     Chest       __instance,  Int32 ___currentLidFrame) {
      if (__instance.playerChoiceColor.Value == Color.Black) return true;
      if (__instance.GetCustomConfigHingesColour() == Color.Black) return true;
      if (!__instance.playerChest.Value && (__instance.ParentSheetIndex != 130 || __instance.ParentSheetIndex != 232)) return true;

      // original game code, albeit slightly modified
      {
        Color chest_colour = __instance.playerChoiceColor.Value;
        chest_colour = Color.FromNonPremultiplied(chest_colour.R, chest_colour.G, chest_colour.B, (Int32)(chest_colour.A * alpha));
        Color hinges_colour = __instance.GetCustomConfigHingesColour();
        hinges_colour = Color.FromNonPremultiplied(hinges_colour.R, hinges_colour.G, hinges_colour.B, (Int32)(hinges_colour.A * alpha));

        Single draw_x = x;
        Single draw_y = y;
        if (__instance.localKickStartTile.HasValue) {
          draw_x = Utility.Lerp(__instance.localKickStartTile.Value.X, draw_x, __instance.kickProgress);
          draw_y = Utility.Lerp(__instance.localKickStartTile.Value.Y, draw_y, __instance.kickProgress);
        }
        Single base_sort_order = Math.Max(0f, ((draw_y + 1f) * 64f - 24f) / 10000f) + draw_x * 1E-05f;
        if (__instance.localKickStartTile.HasValue) {
          spriteBatch.Draw(Game1.shadowTexture,
                           Game1.GlobalToLocal(Game1.viewport, new Vector2((draw_x + 0.5f) * 64f, (draw_y + 0.5f) * 64f)),
                           Game1.shadowTexture.Bounds,
                           Color.Black * 0.5f,
                           0f,
                           new Vector2(Game1.shadowTexture.Bounds.Center.X, Game1.shadowTexture.Bounds.Center.Y),
                           4f,
                           SpriteEffects.None,
                           0.0001f);
          draw_y -= (Single)Math.Sin(__instance.kickProgress * Math.PI) * 0.5f;
        }

        spriteBatch.Draw(Game1.bigCraftableSpriteSheet,
                         Game1.GlobalToLocal(Game1.viewport, new Vector2(draw_x * 64f, (draw_y - 1f) * 64f + (__instance.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0))),
                         Game1.getSourceRectForStandardTileSheet(Game1.bigCraftableSpriteSheet, __instance.ParentSheetIndex == 130 ? 168 : __instance.ParentSheetIndex, 16, 32),
                         chest_colour,
                         0f,
                         Vector2.Zero,
                         4f,
                         SpriteEffects.None,
                         base_sort_order);
        spriteBatch.Draw(TexturePresets.gBigCraftableSpriteSheetGrayScale,
                         Game1.GlobalToLocal(Game1.viewport, new Vector2(draw_x * 64f, draw_y * 64f + 20f)),
                         new Rectangle(0, (__instance.ParentSheetIndex == 130 ? 168 : __instance.ParentSheetIndex) / 8 * 32 + 53, 16, 11),
                         hinges_colour,
                         0f,
                         Vector2.Zero,
                         4f,
                         SpriteEffects.None,
                         base_sort_order + 2E-05f);
        spriteBatch.Draw(TexturePresets.gBigCraftableSpriteSheetGrayScale,
                         Game1.GlobalToLocal(Game1.viewport, new Vector2(draw_x * 64f, (draw_y - 1f) * 64f + (__instance.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0))),
                         Game1.getSourceRectForStandardTileSheet(Game1.bigCraftableSpriteSheet, ___currentLidFrame + (__instance.ParentSheetIndex == 130 ? 46 : 8), 16, 32),
                         hinges_colour,
                         0f,
                         Vector2.Zero,
                         4f,
                         SpriteEffects.None,
                         base_sort_order + 2E-05f);
        spriteBatch.Draw(Game1.bigCraftableSpriteSheet,
                         Game1.GlobalToLocal(Game1.viewport, new Vector2(draw_x * 64f, (draw_y - 1f) * 64f + (__instance.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0))),
                         Game1.getSourceRectForStandardTileSheet(Game1.bigCraftableSpriteSheet, ___currentLidFrame + (__instance.ParentSheetIndex == 130 ? 38 : 0), 16, 32),
                         chest_colour,
                         0f,
                         Vector2.Zero,
                         4f,
                         SpriteEffects.None,
                         base_sort_order + 1E-05f);
      }
      return false;
    }

    [EventPriority(EventPriority.Low)]
    public static void OnRenderingHud(Object sender, RenderingHudEventArgs e) {
      if (!Config.Get().mShowChestHoverTooltip) return;
      if (Game1.activeClickableMenu is not null) return;
      if (!Game1.currentLocation.Objects.TryGetValue(Game1.currentCursorTile, out StardewValley.Object obj) || obj is not Chest chest) return;

      Color  chest_colour = chest.GetActualColour();
      String info_text    = String.IsNullOrWhiteSpace(chest.GetCustomConfigDescription()) ? "" : $"{chest.GetCustomConfigDescription()}\r\n\r\n";
      info_text += $"{chest.GetItemsForPlayer(Game1.player.UniqueMultiplayerID).Count} / {chest.GetActualCapacity()} items";

      CustomMenu.DrawHoverText(e.SpriteBatch, Game1.smallFont, info_text, chest.GetCustomConfigName(), backgroundColour: chest_colour, textColour: chest_colour.ContrastColour());
    }

  #endregion

  #endregion

    // Constructors:
  #region Constructors

    public ExtendedChest(Single chestScale, Color hingesColour, ChestType chestType)
      : base(false) {
      this.mChestScale      = chestScale;
      this.mChestType       = chestType;
      this.mHingesColour    = hingesColour == default ? Color.Black : hingesColour;
      this.ParentSheetIndex = this.mChestType == ChestType.WoodenChest ? 130 : 232;

      this.startingLidFrame.Value = this.ParentSheetIndex + 1;
      this.resetLidFrame();
    }

    public ExtendedChest(Rectangle bounds, Color hingesColour = default, ChestType chestType = ChestType.WoodenChest)
      : this((Single)bounds.Width / CONST_CHEST_SPRITE_SIZE, hingesColour, chestType) { }

  #endregion
  }
}
