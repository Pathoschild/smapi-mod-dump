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
using Igorious.StardewValley.DynamicAPI.Interfaces;
using Igorious.StardewValley.DynamicAPI.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Tools;
using Object = StardewValley.Object;
using XColor = Microsoft.Xna.Framework.Color;

namespace Igorious.StardewValley.DynamicAPI.Objects
{
    public class SmartObject : Object, ISmartObject
    {
        public int ID
        {
            get { return ParentSheetIndex; }
            set { ParentSheetIndex = value; }
        }

        protected static int TileSize => Game1.tileSize;

        #region	Constructors

        public SmartObject() { }

        public SmartObject(int id) : base(Vector2.Zero, id) { }

        public SmartObject(int id, int count) : base(Vector2.Zero, id, count) { }

        #endregion

        #region Bounding

        public int BoundingTileWidth { get; set; } = 1;
        public int BoundingTileHeight { get; set; } = 1;

        public sealed override Rectangle getBoundingBox(Vector2 tile)
        {
            boundingBox.X = (int)tile.X * Game1.tileSize;
            boundingBox.Y = (int)tile.Y * Game1.tileSize;
            boundingBox.Height = BoundingTileHeight * Game1.tileSize;
            boundingBox.Width = BoundingTileWidth * Game1.tileSize;
            return boundingBox;
        }

        public override bool canBePlacedHere(GameLocation l, Vector2 tile)
        {
            for (var w = 0; w < BoundingTileWidth; ++w)
                for (var h = 0; h < BoundingTileHeight; ++h)
                {
                    if (!base.canBePlacedHere(l, new Vector2(tile.X + w, tile.Y + h))) return false;
                }
            return true;
        }

        #endregion

        #region Draw

        #region	Properties

        protected virtual TextureType TextureType { get; set; } = TextureType.Items;
        protected Texture2D Texture => TextureInfo.Default[TextureType].Texture;
        public int SpriteWidth { get; set; } = 1;
        public int SpriteHeight { get; set; } = 1;
        public int VerticalShift { get; set; } = 0;
        protected virtual Rectangle SourceRect => GetSourceRect(ParentSheetIndex);
        protected bool UsedPreviewIcon => (SpriteHeight > 2) || (SpriteWidth > 1);
        public XColor? Color { get; set; }

        #endregion

        #region Native Methods

        public override void drawPlacementBounds(SpriteBatch spriteBatch, GameLocation location)
        {
            if (!isPlaceable()) return;

            int x, y;
            GetPlacementMarkerPosition(out x, out y);
            DrawPlacementMarker(spriteBatch, location, x, y);
            draw(spriteBatch, x / TileSize, y / TileSize, 0.5f);
        }

        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, bool drawStackNumber)
        {
            if (isRecipe)
            {
                transparency = 0.5f;
                scaleSize *= 0.75f;
            }

            DrawShadow(spriteBatch, location, scaleSize, layerDepth);
            DrawMenuItem(spriteBatch, location, transparency, scaleSize, layerDepth);
            if (Color != null) DrawMenuItem(spriteBatch, location, transparency, scaleSize, layerDepth, Color);
            DrawStackNumber(location, scaleSize, drawStackNumber);
            DrawQualityStar(spriteBatch, location, scaleSize, transparency, layerDepth, drawStackNumber);
            DrawTackleBar(spriteBatch, location, scaleSize);
            DrawRecipe(spriteBatch, location, layerDepth);
        }

        public override void drawWhenHeld(SpriteBatch spriteBatch, Vector2 objectPosition, Farmer farmer)
        {
            DrawHeld(spriteBatch, objectPosition, farmer);
            if (Color != null) DrawHeld(spriteBatch, objectPosition, farmer, Color);
        }

        public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1)
        {
            DrawObject(spriteBatch, x, y, alpha);
            if (Color != null) DrawObject(spriteBatch, x, y, alpha, Color);
        }

        #endregion

        #region	Auxiliary Methods

        protected void GetPlacementMarkerPosition(out int x, out int y)
        {
            if (Game1.mouseCursorTransparency == 0)
            {
                var grabbedTile = Game1.player.GetGrabTile();
                if (grabbedTile.Equals(Game1.player.getTileLocation()))
                {
                    var translatedVector2 = Utility.getTranslatedVector2(grabbedTile, Game1.player.facingDirection, 1);
                    x = (int)translatedVector2.X * TileSize;
                    y = (int)translatedVector2.Y * TileSize;
                }
                else
                {
                    x = (int)grabbedTile.X * TileSize;
                    y = (int)grabbedTile.Y * TileSize;
                }
            }
            else
            {
                x = Game1.getOldMouseX() + Game1.viewport.X;
                y = Game1.getOldMouseY() + Game1.viewport.Y;
            }
        }

        protected void DrawPlacementMarker(SpriteBatch spriteBatch, GameLocation location, int x, int y)
        {
            var canPlace = Utility.playerCanPlaceItemHere(location, this, x, y, Game1.player);
            spriteBatch.Draw(
                Game1.mouseCursors,
                new Vector2(x / TileSize * TileSize - Game1.viewport.X, y / TileSize * TileSize - Game1.viewport.Y),
                Rectangle(canPlace ? 194 : 210, 388, 16, 16),
                XColor.White,
                0,
                Vector2.Zero,
                Game1.pixelZoom,
                SpriteEffects.None,
                0.01f);
        }

        protected void DrawMenuItem(SpriteBatch spriteBatch, Vector2 location, float transparency, float scaleSize, float layerDepth, Color? color = null)
        {
            var sourceRect = GetSourceRect(ParentSheetIndex + (color.HasValue ? 1 : 0) + (UsedPreviewIcon ? -1 : 0), UsedPreviewIcon ? 1 : (int?)null, UsedPreviewIcon ? 1 : (int?)null);
            var scaleX = scaleSize * 16 / sourceRect.Height;
            var scaleY = scaleSize * 16 / sourceRect.Width;

            spriteBatch.Draw(
                Texture,
                Rectangle(
                    location.X + TileSize * (1 - scaleX) / 2,
                    location.Y + TileSize * (1 - scaleY) / 2,
                    TileSize * scaleX,
                    TileSize * scaleY),
                sourceRect,
                (color ?? XColor.White) * transparency,
                0,
                Vector2.Zero,
                SpriteEffects.None,
                layerDepth + (color.HasValue ? 0.00002f : 0));
        }

        protected Rectangle GetSourceRect(int index, int? length = null, int? height = null)
        {
            return TextureInfo.Default[TextureType].GetSourceRect(index + (UsedPreviewIcon ? 1 : 0) + (showNextIndex ? 1 : 0), length ?? SpriteWidth, height ?? SpriteHeight);
        }

        protected void DrawRecipe(SpriteBatch spriteBatch, Vector2 location, float layerDepth)
        {
            if (!isRecipe) return;

            spriteBatch.Draw(
                Game1.objectSpriteSheet,
                location + new Vector2(TileSize / 4f, TileSize / 4f),
                Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 451, 16, 16),
                XColor.White,
                0,
                Vector2.Zero,
                Game1.pixelZoom * 3f / 4,
                SpriteEffects.None,
                layerDepth + 0.0001f);
        }

        protected void DrawTackleBar(SpriteBatch spriteBatch, Vector2 location, float scaleSize)
        {
            if (category != tackleCategory || scale.Y == 1) return;

            spriteBatch.Draw(
                Game1.staminaRect,
                Rectangle(
                    location.X,
                    location.Y + (TileSize - 2 * Game1.pixelZoom) * scaleSize,
                    TileSize * scaleSize * scale.Y,
                    2 * Game1.pixelZoom * scaleSize),
                Utility.getRedToGreenLerpColor(scale.Y));
        }

        protected void DrawQualityStar(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, bool drawStackNumber)
        {
            if (!drawStackNumber || quality == 0) return;

            var blinkScale = (float)(Math.Cos(Game1.currentGameTime.TotalGameTime.Milliseconds * Math.PI / 512) + 1) * 0.05f;
            spriteBatch.Draw(
                Game1.mouseCursors,
                location + new Vector2(12, Game1.tileSize - 12 + blinkScale),
                Rectangle(330 + quality * 8, 400, 8, 8),
                XColor.White * transparency,
                0,
                new Vector2(4, 4),
                3 * scaleSize * (1 + blinkScale),
                SpriteEffects.None,
                layerDepth + 0.0003f);
        }

        protected void DrawStackNumber(Vector2 location, float scaleSize, bool drawStackNumber)
        {
            if (!drawStackNumber || maximumStackSize() <= 1 || scaleSize <= 0.3 || Stack == int.MaxValue || Stack <= 1) return;

            var fontScale = 0.5f + scaleSize;
            var message = stack.ToString();
            var measure = Game1.tinyFont.MeasureString(message);
            Game1.drawWithBorder(
                message,
                XColor.Black,
                XColor.White,
                location + new Vector2(TileSize - measure.X * fontScale, TileSize - measure.Y * 3 / 4 * fontScale),
                0,
                fontScale,
                1,
                true);
        }

        protected void DrawShadow(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float layerDepth)
        {
            spriteBatch.Draw(
                Game1.shadowTexture,
                location + new Vector2(Game1.tileSize / 2f, Game1.tileSize * 3 / 4f),
                Game1.shadowTexture.Bounds,
                XColor.White * 0.5f,
                0,
                new Vector2(Game1.shadowTexture.Bounds.Center.X, Game1.shadowTexture.Bounds.Center.Y),
                3,
                SpriteEffects.None,
                layerDepth - 0.0001f);
        }

        protected void DrawHeld(SpriteBatch spriteBatch, Vector2 objectPosition, Farmer farmer, XColor? color = null)
        {
            var sourceRect = GetSourceRect(ParentSheetIndex + (color.HasValue ? SpriteWidth : 0));
            spriteBatch.Draw(
                Texture,
                objectPosition - new Vector2((sourceRect.Width - 16) / 32f * TileSize, (sourceRect.Height / 16f - 1 + VerticalShift) * TileSize),
                sourceRect,
                color ?? XColor.White,
                0,
                Vector2.Zero,
                Game1.pixelZoom,
                SpriteEffects.None,
                Math.Max(0, (farmer.getStandingY() + 2 + (color.HasValue ? 1 : 0)) / 10000f));
        }

        protected virtual Vector2 GetScale(bool change = true)
        {
            if (category == tackleCategory || name.Contains("Table")) return Vector2.Zero;
            scale.Y = Math.Max(Game1.pixelZoom, scale.Y - Game1.pixelZoom / 100f);
            return scale;
        }

        protected virtual void DrawObject(SpriteBatch spriteBatch, int x, int y, float alpha, XColor? color = null, int sheetIndexDelta = 0)
        {
            var currentScale = GetScale(color == null) * Game1.pixelZoom;
            var destVector = Game1.GlobalToLocal(Game1.viewport, new Vector2(TileSize * x, TileSize * (y + VerticalShift)));
            if (shakeTimer > 0)
            {
                destVector.X += Game1.random.Next(-1, 2);
                destVector.Y += Game1.random.Next(-1, 2);
            }

            var destRect = Rectangle(
                destVector.X - currentScale.X / 4,
                destVector.Y - currentScale.Y / 4,
                TileSize * SourceRect.Width / 16f + currentScale.X / 2,
                TileSize * SourceRect.Height / 16f + currentScale.Y / 2);

            var depth = getBoundingBox(new Vector2(x, y)).Bottom / 10000f;

            spriteBatch.Draw(
                Texture,
                destRect,
                GetSourceRect(ParentSheetIndex + sheetIndexDelta * SpriteWidth),
                (color ?? XColor.White) * alpha,
                0,
                Vector2.Zero,
                SpriteEffects.None,
                depth + (color.HasValue ? 0.0001f : 0));
        }

        protected void PlayAnimation(Farmer farmer, Animation animation)
        {
            var animationSprites = farmer.currentLocation.temporarySprites;

            switch (animation)
            {
                case Animation.Steam:
                    animationSprites.Add(new TemporaryAnimatedSprite(
                        27,
                        tileLocation * Game1.tileSize + new Vector2(-TileSize / 4f, -TileSize * 2),
                        XColor.White,
                        4,
                        false,
                        50,
                        10,
                        TileSize,
                        (tileLocation.Y + 1) * TileSize / 10000 + 0.0001f)
                    {
                        alphaFade = 0.005f,
                    });
                    break;
            }
        }

        #endregion

        #endregion

        #region Action

        private static readonly Object ProbeObject = new Object();

        #region Native

        public sealed override bool checkForAction(Farmer who, bool justCheckingForActivity = false)
        {
            return ExceptionHandler.Invoke(() => justCheckingForActivity ? CanDoAction(who) : DoAction(who));
        }

        public sealed override bool performObjectDropInAction(Object item, bool isProbe, Farmer farmer)
        {
            return ExceptionHandler.Invoke(() =>
            {
                if (!CanPerformDropIn(item, farmer)) return false;

                if (isProbe)
                {
                    heldObject = ProbeObject;
                    return true;
                }
                else
                {
                    return PerformDropIn(item, farmer);
                }
            });
        }

        #endregion

        #region	Auxiliary Methods

        protected virtual bool CanDoAction(Farmer farmer) => base.checkForAction(farmer, true);

        protected virtual bool DoAction(Farmer farmer) => base.checkForAction(farmer);

        protected virtual bool CanPerformDropIn(Object item, Farmer farmer) => CanPerformDropInRaw(item, farmer);

        protected virtual bool PerformDropIn(Object dropInItem, Farmer farmer) => PerformDropInRaw(dropInItem, farmer);

        protected bool PerformDropInRaw(Object item, Farmer farmer) => base.performObjectDropInAction(item, false, farmer);

        protected bool CanPerformDropInRaw(Object item, Farmer farmer)
        {
            base.performObjectDropInAction(item, true, farmer);
            var result = (heldObject != null);
            heldObject = null;
            return result;
        }

        #endregion

        #endregion

        #region Tool Action

        public sealed override bool performToolAction(Tool tool)
        {
            return ExceptionHandler.Invoke(() =>
            {
                if (tool is Pickaxe) return OnPickaxeAction((Pickaxe)tool);
                if (tool is Axe) return OnAxeAction((Axe)tool);
                if (tool is Hoe) return OnHoeAction((Hoe)tool);
                if (tool is WateringCan)
                {
                    OnWateringCanAction((WateringCan)tool);
                    return false;
                }
                return OnOtherToolAction(tool);
            });
        }

        protected virtual bool OnPickaxeAction(Pickaxe pickaxe) => base.performToolAction(pickaxe);

        protected virtual bool OnAxeAction(Axe axe) => base.performToolAction(axe);

        protected virtual bool OnHoeAction(Hoe hoe) => base.performToolAction(hoe);

        protected virtual void OnWateringCanAction(WateringCan wateringCan) => base.performToolAction(wateringCan);

        protected virtual bool OnOtherToolAction(Tool tool) => base.performToolAction(tool);

        #endregion

        public override Item getOne()
        {
            var clone = (SmartObject)MemberwiseClone();
            clone.Stack = 1;
            return clone;
        }

        protected Random GetRandom()
        {
            return new Random((int)Game1.uniqueIDForThisGame / 2 + (int)Game1.stats.DaysPlayed + Game1.timeOfDay + (int)tileLocation.X * 200 + (int)tileLocation.Y);
        }

        protected static Rectangle Rectangle(float x, float y, float width, float height)
        {
            return new Rectangle((int)x, (int)y, (int)width, (int)height);
        }

        protected void PlaySound(Sound sound) => Game1.playSound(sound.GetDescription());

        protected void ShowRedMessage(Farmer farmer, string message)
        {
            if (!farmer.IsMainPlayer) return;
            Game1.showRedMessage(message);
        }
    }
}
