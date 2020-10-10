/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Igorious/StardevValleyNewMachinesMod
**
*************************************************/

using System;
using Igorious.StardewValley.DynamicAPI.Constants;
using Igorious.StardewValley.DynamicAPI.Data.Supporting;
using Igorious.StardewValley.DynamicAPI.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using XColor = Microsoft.Xna.Framework.Color;

namespace Igorious.StardewValley.DynamicAPI.Objects
{
    public abstract partial class SmartBigCrafrableBase : SmartObject
    {
        protected SmartBigCrafrableBase(int id) : base(id)
        {
            VerticalShift = -1;
        }

        protected MachineState State
        {
            get
            {
                if (readyForHarvest) return MachineState.Ready;
                if (heldObject != null) return MachineState.Working;
                return MachineState.Empty;
            }
        }

        #region Draw

        #region Properties

        protected override TextureType TextureType { get; set; } = TextureType.Craftables;

        #endregion

        #region Native

        public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1)
        {
            base.draw(spriteBatch, x, y, alpha);
            DrawDetails(spriteBatch, x, y, alpha);
            DrawHeldObject(spriteBatch, x, y);
        }

        #endregion

        #region	Auxiliary Methods

        protected virtual void DrawDetails(SpriteBatch spriteBatch, int x, int y, float alpha)
        {
            if (ID != (int)CraftableID.Loom || minutesUntilReady <= 0) return;

            var itemTextureInfo = TextureInfo.Default[TextureType.Items];
            spriteBatch.Draw(
                itemTextureInfo.Texture,
                getLocalPosition(Game1.viewport) + new Vector2(TileSize / 2f, 0),
                itemTextureInfo.GetSourceRect(435),
                XColor.White * alpha,
                scale.X,
                new Vector2(8, 8),
                Game1.pixelZoom,
                SpriteEffects.None,
                Math.Max(0, (y + 1) * TileSize / 10000f + 0.0001f + x * 0.00000001f));
        }

        protected void DrawBallon(SpriteBatch spriteBatch, int x, int y, float deltaY, float depth)
        {
            spriteBatch.Draw(
                Game1.mouseCursors,
                Game1.GlobalToLocal(Game1.viewport, new Vector2(x * TileSize - 8, y * TileSize - TileSize * 3 / 2 - 16 + deltaY)),
                Rectangle(141, 465, 20, 24),
                XColor.White * 0.75f,
                0,
                Vector2.Zero,
                4,
                SpriteEffects.None,
                depth - 0.0002f);
        }

        protected void DrawBaloonItem(SpriteBatch spriteBatch, int x, int y, float deltaY, float depth, XColor? color = null)
        {
            var baloonCenter = Game1.GlobalToLocal(Game1.viewport, new Vector2(x * TileSize + TileSize / 2, (y - 1) * TileSize - TileSize / 8 + deltaY));

            var itemsTextureInfo = TextureInfo.Default[TextureType.Items];
            spriteBatch.Draw(
                itemsTextureInfo.Texture,
                baloonCenter,
                itemsTextureInfo.GetSourceRect(heldObject.parentSheetIndex + (color.HasValue ? 1 : 0)),
                (color ?? XColor.White) * 0.75f,
                0,
                new Vector2(8, 8),
                Game1.pixelZoom,
                SpriteEffects.None,
                depth + (color.HasValue ? 0.0001f : 0));
        }

        protected void DrawHeldObject(SpriteBatch spriteBatch, int x, int y)
        {
            if (!readyForHarvest || heldObject == null) return;

            var deltaY = (float)(4 * Math.Round(Math.Sin(DateTime.Now.TimeOfDay.TotalMilliseconds / 250), 2));
            var depth = (y + 1) * Game1.tileSize / 10000f + tileLocation.X / 10000f;

            DrawBallon(spriteBatch, x, y, deltaY, depth);
            DrawBaloonItem(spriteBatch, x, y, deltaY, depth);
            var color = heldObject.GetColor();
            if (color != null) DrawBaloonItem(spriteBatch, x, y, deltaY, depth, color);
        }

        protected override Vector2 GetScale(bool change = true)
        {
            if (heldObject == null && minutesUntilReady <= 0 || readyForHarvest) return Vector2.Zero;
            if (ID == (int)CraftableID.BeeHouse || ID == (int)CraftableID.Tapper) return Vector2.Zero;
            if (ID == (int)CraftableID.Loom)
            {
                if (change) scale.X = (float)((scale.X + Game1.pixelZoom / 100.0) % (2.0 * Math.PI));
                return Vector2.Zero;
            }
            if (change)
            {
                scale.X -= 0.1f;
                scale.Y += 0.1f;
                if (scale.X <= 0) scale.X = 10;
                if (scale.Y >= 10) scale.Y = 0;
            }
            return new Vector2(Math.Abs(scale.X - 5), Math.Abs(scale.Y - 5));
        }

        #endregion

        #endregion

        protected void PutItem(int itemID, int count, int itemQuality = 0, string overridedName = null, int? overridedPrice = null, XColor? color = null)
        {
            PutItemValidator.Validate(itemID, count, itemQuality, overridedName, overridedPrice, color);

            heldObject = new SmartObject(itemID, count)
            {
                quality = itemQuality,
                Color = color,
            };
            if (overridedName != null) heldObject.Name = string.Format(overridedName, heldObject.Name);
            if (overridedPrice != null) heldObject.Price = overridedPrice.Value;
        }
    }
}
