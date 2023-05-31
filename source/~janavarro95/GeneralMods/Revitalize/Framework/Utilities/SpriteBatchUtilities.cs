/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Omegasis.Revitalize.Framework.World.Objects;
using Omegasis.Revitalize.Framework.World.Objects.Interfaces;
using Omegasis.Revitalize.Framework.World.Objects.Items.Tools;
using StardewValley;

namespace Omegasis.Revitalize.Framework.Utilities
{
    /// <summary>
    /// Utilities for dealing with the XNA/Monogames spritebatch class.
    /// </summary>
    public static class SpriteBatchUtilities
    {

        public static void Draw(SpriteBatch spriteBatch, StardewValley.Object obj, Item itemToDraw, float alpha, float addedDepth)
        {
            if (itemToDraw.GetType() == typeof(StardewValley.Object))
            {
                Rectangle rectangle;
                SpriteBatch spriteBatch1 = spriteBatch;
                Texture2D shadowTexture = Game1.shadowTexture;
                Vector2 position = Game1.GlobalToLocal(Game1.viewport, new Vector2(obj.TileLocation.X, obj.TileLocation.Y));
                Rectangle? sourceRectangle = new Rectangle?(Game1.shadowTexture.Bounds);
                Color color = Color.White * alpha;
                rectangle = Game1.shadowTexture.Bounds;
                double x1 = rectangle.Center.X;
                rectangle = Game1.shadowTexture.Bounds;
                double y1 = rectangle.Center.Y;
                Vector2 origin = new Vector2((float)x1, (float)y1);
                double num = obj.boundingBox.Bottom / 10000.0;
                spriteBatch1.Draw(shadowTexture, position, sourceRectangle, color, 0.0f, origin, 4f, SpriteEffects.None, (float)num);
                spriteBatch.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, obj.TileLocation * Game1.tileSize), Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, itemToDraw.ParentSheetIndex, 16, 16), Color.White * alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (obj.boundingBox.Bottom + 1) / 10000f + addedDepth);

                (itemToDraw as StardewValley.Object).draw(spriteBatch, (int)obj.TileLocation.X * Game1.tileSize, (int)obj.TileLocation.Y * Game1.tileSize, Math.Max(0f, (obj.TileLocation.Y + 1 + addedDepth) * Game1.tileSize / 10000f) + .0001f, alpha);
                return;
            }
            if (TypeUtilities.IsSameOrSubclass(typeof(CustomObject), itemToDraw.GetType()))
                (itemToDraw as CustomObject).draw(spriteBatch, (int)obj.TileLocation.X, (int)obj.TileLocation.Y);
        }

        /// <summary>
        /// Code to draw items as held objects to be shown above a different given item.
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="itemToDraw"></param>
        /// <param name="TileLocation"></param>
        /// <param name="alpha"></param>
        /// <param name="addedDepth"></param>
        public static void DrawHeldObject(SpriteBatch spriteBatch, Item itemToDraw, Vector2 TileLocation, float alpha, float addedDepth)
        {
            if (itemToDraw.GetType() == typeof(StardewValley.Object))
            {
                (itemToDraw as StardewValley.Object).draw(spriteBatch, (int)(TileLocation.X * Game1.tileSize), (int)(TileLocation.Y * Game1.tileSize), Math.Max(0f, (TileLocation.Y + 1 + addedDepth) * Game1.tileSize / 10000f) + .0001f, alpha);
                return;
            }
            if (TypeUtilities.IsSameOrSubclass(typeof(CustomObject), itemToDraw.GetType()))
                (itemToDraw as CustomObject).draw(spriteBatch, (int)TileLocation.X, (int)TileLocation.Y, alpha);
        }

        /// <summary>What happens when the object is drawn at a tile location.</summary>
        public static void DrawICustomModObject<T>(this T obj, SpriteBatch spriteBatch, int x, int y, bool flipped, float alpha = 1f, StardewValley.Object heldObject = null) where T : StardewValley.Object, ICustomModObject
        {
            if (x <= -1)
            {
                x = (int)obj.TileLocation.X;
                //spriteBatch.Draw(this.basicItemInfo.animationManager.getTexture(), Game1.GlobalToLocal(Game1.viewport, this.TileLocation), new Rectangle?(this.AnimationManager.currentAnimation.sourceRectangle), this.basicItemInfo.DrawColor * alpha, 0f, Vector2.Zero, (float)Game1.pixelZoom, this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0f, (float)(this.TileLocation.Y * Game1.tileSize) / 10000f));
            }
            if (y <= -1)
            {
                y = (int)obj.TileLocation.Y;
            }

            if (obj.basicItemInformation.animationManager == null)
            {
                spriteBatch.Draw(obj.basicItemInformation.animationManager.getTexture(), Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(x * Game1.tileSize) + obj.basicItemInformation.shakeTimerOffset(), (y * Game1.tileSize) + obj.basicItemInformation.shakeTimerOffset())), new Rectangle?(obj.basicItemInformation.animationManager.getCurrentAnimation().getCurrentAnimationFrameRectangle()), obj.basicItemInformation.DrawColor * alpha, 0f, Vector2.Zero, (float)Game1.pixelZoom, flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0f, (float)(y * Game1.tileSize) / 10000f));
            }
            else
            {
                obj.basicItemInformation.animationManager.draw(spriteBatch, obj.basicItemInformation.animationManager.getTexture(), Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(x * Game1.tileSize) + obj.basicItemInformation.shakeTimerOffset(), (y * Game1.tileSize) + obj.basicItemInformation.shakeTimerOffset())), new Rectangle?(obj.basicItemInformation.animationManager.getCurrentAnimation().getCurrentAnimationFrameRectangle()), obj.basicItemInformation.DrawColor * alpha, 0f, Vector2.Zero, (float)Game1.pixelZoom * obj.AnimationManager.getCurrentAnimation().getCurrentAnimationFrame().scale, flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0f, (float)((y) * Game1.tileSize) / 10000f) + .00001f);
                if (heldObject != null) SpriteBatchUtilities.Draw(spriteBatch, obj, heldObject, alpha, 0);
            }
        }

        public static void DrawICustomModObject<T>(this T obj, SpriteBatch spriteBatch, float alpha = 1f) where T : StardewValley.Object, ICustomModObject
        {
            obj.DrawICustomModObject(spriteBatch, (int)obj.TileLocation.X + (int)obj.basicItemInformation.drawOffset.X, (int)obj.TileLocation.Y + (int)obj.basicItemInformation.drawOffset.Y, alpha, obj.heldObject, (int)obj.TileLocation.Y - (int)obj.basicItemInformation.drawOffset.Y);
        }

        public static void DrawICustomModObject<T>(this T obj, SpriteBatch spriteBatch, float alpha = 1f, bool DrawHeldObject = true) where T : StardewValley.Object, ICustomModObject
        {
            obj.DrawICustomModObject(spriteBatch, (int)obj.TileLocation.X + (int)obj.basicItemInformation.drawOffset.X, (int)obj.TileLocation.Y + (int)obj.basicItemInformation.drawOffset.Y, alpha, DrawHeldObject ? obj.heldObject : null, (int)obj.TileLocation.Y - (int)obj.basicItemInformation.drawOffset.Y);
        }

        public static void DrawICustomModObject<T>(this T obj, SpriteBatch spriteBatch, int x, int y, float alpha = 1f, StardewValley.Object heldObject = null, int YTileDepth = 0) where T : StardewValley.Object, ICustomModObject
        {
            if (YTileDepth == 0)
            {
                YTileDepth = y;
            }

            if (obj.basicItemInformation.animationManager == null)
            {
                spriteBatch.Draw(obj.basicItemInformation.animationManager.getTexture(), Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(x * Game1.tileSize) + obj.basicItemInformation.shakeTimerOffset(), (y * Game1.tileSize) + obj.basicItemInformation.shakeTimerOffset())), new Rectangle?(obj.basicItemInformation.animationManager.getCurrentAnimation().getCurrentAnimationFrameRectangle()), obj.basicItemInformation.DrawColor * alpha, 0f, Vector2.Zero, (float)Game1.pixelZoom, obj.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0f, (float)(YTileDepth * Game1.tileSize) / 10000f));
            }
            else
            {
                obj.basicItemInformation.animationManager.draw(spriteBatch, obj.basicItemInformation.animationManager.getTexture(), Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(x * Game1.tileSize) + obj.basicItemInformation.shakeTimerOffset(), (y * Game1.tileSize) + obj.basicItemInformation.shakeTimerOffset())), new Rectangle?(obj.basicItemInformation.animationManager.getCurrentAnimation().getCurrentAnimationFrameRectangle()), obj.basicItemInformation.DrawColor * alpha, 0f, Vector2.Zero, (float)Game1.pixelZoom, obj.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0f, (float)((YTileDepth) * Game1.tileSize) / 10000f) + .00001f);
                if (heldObject != null) SpriteBatchUtilities.Draw(spriteBatch, obj, heldObject, alpha, 0);
            }
        }

        public static void DrawICustomModObjectWhenHeld<T>(this T obj, SpriteBatch spriteBatch, Vector2 objectPosition, Farmer f) where T : StardewValley.Object, ICustomModObject
        {

            DrawICustomModObjectWhenHeld(obj, spriteBatch, objectPosition, f, 1f, 1f);
        }

        public static void DrawICustomModObjectWhenHeld<T>(this T obj, SpriteBatch spriteBatch, Vector2 objectPosition, Farmer f, float transparency, float Scale) where T : ICustomModObject
        {
            int hightOffset = Game1.tileSize;

            if (f.ActiveObject.bigCraftable.Value)
            {
                spriteBatch.Draw(obj.CurrentTextureToDisplay, objectPosition + new Vector2(0, hightOffset), obj.AnimationManager.getCurrentAnimationFrameRectangle(), obj.basicItemInformation.DrawColor * transparency, 0f, Vector2.Zero, (float)Game1.pixelZoom * Scale * obj.AnimationManager.getCurrentAnimation().getCurrentAnimationFrame().scale, SpriteEffects.None, Math.Max(0f, (float)(f.getStandingY() + 2) / 10000f));
                return;
            }

            spriteBatch.Draw(obj.CurrentTextureToDisplay, objectPosition, obj.AnimationManager.getCurrentAnimationFrameRectangle(), obj.basicItemInformation.DrawColor * transparency, 0f, Vector2.Zero, (float)Game1.pixelZoom * Scale, SpriteEffects.None, Math.Max(0f, (float)(f.getStandingY() + 2) / 10000f));
            if (f.ActiveObject != null && f.ActiveObject.Name.Contains("="))
            {
                spriteBatch.Draw(obj.CurrentTextureToDisplay, objectPosition + new Vector2(0, hightOffset) + new Vector2((float)(Game1.tileSize / 2), (float)(Game1.tileSize / 2)), obj.AnimationManager.getCurrentAnimationFrameRectangle(), Color.White, 0f, new Vector2((float)(Game1.tileSize / 2), (float)(Game1.tileSize / 2)), (float)Game1.pixelZoom + Math.Abs(Game1.starCropShimmerPause) / 8f * Scale * obj.AnimationManager.getCurrentAnimation().getCurrentAnimationFrame().scale, SpriteEffects.None, Math.Max(0f, (float)(f.getStandingY() + 2) / 10000f));
                if (Math.Abs(Game1.starCropShimmerPause) <= 0.05f && Game1.random.NextDouble() < 0.97)
                {
                    return;
                }
                Game1.starCropShimmerPause += 0.04f;
                if (Game1.starCropShimmerPause >= 0.8f)
                {
                    Game1.starCropShimmerPause = -0.8f;
                }
            }
            //base.drawWhenHeld(spriteBatch, objectPosition, f);
        }

        public static void DrawICustomModObjectInMenu<T>(this T obj, SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow) where T : StardewValley.Object, ICustomModObject
        {

            if (obj.basicItemInformation == null) return;

            int scaleNerfing = Math.Max(obj.AnimationManager.getCurrentAnimationFrameRectangle().Width, obj.AnimationManager.getCurrentAnimationFrameRectangle().Height) / 16;

            spriteBatch.Draw(obj.CurrentTextureToDisplay, location + new Vector2((float)(Game1.tileSize / 2), (float)(Game1.tileSize)), new Rectangle?(obj.AnimationManager.getCurrentAnimationFrameRectangle()), obj.basicItemInformation.DrawColor * transparency, 0f, new Vector2((float)(obj.AnimationManager.getCurrentAnimationFrameRectangle().Width / 2), (float)(obj.AnimationManager.getCurrentAnimationFrameRectangle().Height)), (scaleSize * 4f) / scaleNerfing, SpriteEffects.None, layerDepth);

            if (drawStackNumber.ShouldDrawFor(obj) && obj.maximumStackSize() > 1 && ((double)scaleSize > 0.3 && obj.Stack != int.MaxValue) && obj.Stack > 1)
                Utility.drawTinyDigits(obj.Stack, spriteBatch, location + new Vector2((float)(Game1.tileSize - Utility.getWidthOfTinyDigitString(obj.Stack, 3f * scaleSize)) + 3f * scaleSize, (float)((double)Game1.tileSize - 18.0 * (double)scaleSize + 2.0)), 3f * scaleSize, 1f, Color.White);
            if (drawStackNumber.ShouldDrawFor(obj) && obj.Quality > 0)
            {
                float num = obj.Quality < 4 ? 0.0f : (float)((Math.Cos((double)Game1.currentGameTime.TotalGameTime.Milliseconds * Math.PI / 512.0) + 1.0) * 0.0500000007450581);
                spriteBatch.Draw(Game1.mouseCursors, location + new Vector2(12f, (float)(Game1.tileSize - 12) + num), new Microsoft.Xna.Framework.Rectangle?(obj.Quality < 4 ? new Microsoft.Xna.Framework.Rectangle(338 + (obj.Quality - 1) * 8, 400, 8, 8) : new Microsoft.Xna.Framework.Rectangle(346, 392, 8, 8)), Color.White * transparency, 0.0f, new Vector2(4f, 4f), (float)(3.0 * (double)scaleSize * (1.0 + (double)num)), SpriteEffects.None, layerDepth);
            }
        }

    }
}
