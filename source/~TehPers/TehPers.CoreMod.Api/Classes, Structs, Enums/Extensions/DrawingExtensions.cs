/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.BellsAndWhistles;
using TehPers.CoreMod.Api.Drawing;
using XnaRectangle = Microsoft.Xna.Framework.Rectangle;
using XTileRectangle = xTile.Dimensions.Rectangle;

namespace TehPers.CoreMod.Api.Extensions {
    public static class DrawingExtensions {
        /// <summary>Draws a filled rectangle.</summary>
        /// <param name="batch">The batch to draw to.</param>
        /// <param name="drawingApi">The drawing API to use.</param>
        /// <param name="x">The top-left x-coordinate of the rectangle.</param>
        /// <param name="y">The top-left y-coordinate of the rectangle.</param>
        /// <param name="width">The width of the rectangle.</param>
        /// <param name="height">The height of the rectangle.</param>
        /// <param name="color">The color of the rectangle</param>
        /// <param name="depth">The depth to draw to.</param>
        public static void FillRectangle(this SpriteBatch batch, IDrawingApi drawingApi, int x, int y, int width, int height, Color color, float depth = 0f) => batch.FillRectangle(drawingApi, new XnaRectangle(x, y, width, height), color, depth);

        /// <summary>Draws a filled rectangle.</summary>
        /// <param name="batch">The batch to draw to.</param>
        /// <param name="drawingApi">The drawing API to use.</param>
        /// <param name="bounds">The target rectangle to draw to.</param>
        /// <param name="color">The color of the rectangle</param>
        /// <param name="depth">The depth to draw to.</param>
        public static void FillRectangle(this SpriteBatch batch, IDrawingApi drawingApi, XTileRectangle bounds, Color color, float depth = 0f) => batch.FillRectangle(drawingApi, bounds.ToXnaRectangle(), color, depth);

        /// <summary>Draws a filled rectangle.</summary>
        /// <param name="batch">The batch to draw to.</param>
        /// <param name="drawingApi">The drawing API to use.</param>
        /// <param name="bounds">The target rectangle to draw to.</param>
        /// <param name="color">The color of the rectangle</param>
        /// <param name="depth">The depth to draw to.</param>
        public static void FillRectangle(this SpriteBatch batch, IDrawingApi drawingApi, XnaRectangle bounds, Color color, float depth = 0f) {
            batch.Draw(drawingApi.WhitePixel, bounds, null, color, 0F, Vector2.Zero, SpriteEffects.None, depth);
        }

        /// <summary>Draws the background for a menu.</summary>
        /// <param name="batch">The batch to draw with.</param>
        /// <param name="bounds">The target rectangle to draw to.</param>
        /// <param name="depth">The depth to draw to.</param>
        public static void DrawMenuBox(this SpriteBatch batch, XnaRectangle bounds, float depth) {
            XnaRectangle sourceRect = new XnaRectangle(Game1.tileSize, Game1.tileSize * 2, Game1.tileSize, Game1.tileSize);
            Game1.spriteBatch.Draw(Game1.menuTexture, new XnaRectangle(28 + bounds.X, 28 + bounds.Y, bounds.Width - Game1.tileSize, bounds.Height - Game1.tileSize), sourceRect, Color.White, 0f, Vector2.Zero, SpriteEffects.None, depth);
            sourceRect.Y = 0;
            sourceRect.X = 0;
            Game1.spriteBatch.Draw(Game1.menuTexture, new Vector2(bounds.X, bounds.Y), sourceRect, Color.White, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, depth);
            sourceRect.X = Game1.tileSize * 3;
            Game1.spriteBatch.Draw(Game1.menuTexture, new Vector2(bounds.X + bounds.Width - Game1.tileSize, bounds.Y), sourceRect, Color.White, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, depth);
            sourceRect.Y = Game1.tileSize * 3;
            Game1.spriteBatch.Draw(Game1.menuTexture, new Vector2(bounds.X + bounds.Width - Game1.tileSize, bounds.Y + bounds.Height - Game1.tileSize), sourceRect, Color.White, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, depth);
            sourceRect.X = 0;
            Game1.spriteBatch.Draw(Game1.menuTexture, new Vector2(bounds.X, bounds.Y + bounds.Height - Game1.tileSize), sourceRect, Color.White, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, depth);
            sourceRect.X = Game1.tileSize * 2;
            sourceRect.Y = 0;
            Game1.spriteBatch.Draw(Game1.menuTexture, new XnaRectangle(Game1.tileSize + bounds.X, bounds.Y, bounds.Width - Game1.tileSize * 2, Game1.tileSize), sourceRect, Color.White, 0f, Vector2.Zero, SpriteEffects.None, depth);
            sourceRect.Y = 3 * Game1.tileSize;
            Game1.spriteBatch.Draw(Game1.menuTexture, new XnaRectangle(Game1.tileSize + bounds.X, bounds.Y + bounds.Height - Game1.tileSize, bounds.Width - Game1.tileSize * 2, Game1.tileSize), sourceRect, Color.White, 0f, Vector2.Zero, SpriteEffects.None, depth);
            sourceRect.Y = Game1.tileSize * 2;
            sourceRect.X = 0;
            Game1.spriteBatch.Draw(Game1.menuTexture, new XnaRectangle(bounds.X, bounds.Y + Game1.tileSize, Game1.tileSize, bounds.Height - Game1.tileSize * 2), sourceRect, Color.White, 0f, Vector2.Zero, SpriteEffects.None, depth);
            sourceRect.X = 3 * Game1.tileSize;
            Game1.spriteBatch.Draw(Game1.menuTexture, new XnaRectangle(bounds.X + bounds.Width + -Game1.tileSize, bounds.Y + Game1.tileSize, Game1.tileSize, bounds.Height - Game1.tileSize * 2), sourceRect, Color.White, 0f, Vector2.Zero, SpriteEffects.None, depth);
        }

        /// <summary>Draws a rectangle using the same code for generic menu backgrounds and dialogue box backgrounds.</summary>
        /// <param name="batch">The batch to draw with.</param>
        /// <param name="bounds">The target rectangle to draw to.</param>
        /// <param name="source">The source for the menu texture.</param>
        /// <param name="depth">The depth to draw to.</param>
        public static void DrawTextureBox(this SpriteBatch batch, XnaRectangle bounds, XnaRectangle source, float depth) => batch.DrawTextureBox(Game1.mouseCursors, bounds, source, depth, 1f, Color.White);

        /// <summary>Draws a rectangle using the same code for generic menu backgrounds and dialogue box backgrounds.</summary>
        /// <param name="batch">The batch to draw with.</param>
        /// <param name="bounds">The target rectangle to draw to.</param>
        /// <param name="source">The source for the menu texture.</param>
        /// <param name="depth">The depth to draw to.</param>
        /// <param name="scale">The scale of the texture box.</param>
        public static void DrawTextureBox(this SpriteBatch batch, XnaRectangle bounds, XnaRectangle source, float depth, float scale) => batch.DrawTextureBox(Game1.mouseCursors, bounds, source, depth, scale, Color.White);

        /// <summary>Draws a rectangle using the same code for generic menu backgrounds and dialogue box backgrounds.</summary>
        /// <param name="batch">The batch to draw with.</param>
        /// <param name="bounds">The target rectangle to draw to.</param>
        /// <param name="source">The source for the menu texture.</param>
        /// <param name="depth">The depth to draw to.</param>
        /// <param name="tint">What color to tint the texture box.</param>
        public static void DrawTextureBox(this SpriteBatch batch, XnaRectangle bounds, XnaRectangle source, float depth, Color tint) => batch.DrawTextureBox(Game1.mouseCursors, bounds, source, depth, 1f, tint);

        /// <summary>Draws a rectangle using the same code for generic menu backgrounds and dialogue box backgrounds with the given texture.</summary>
        /// <param name="batch">The batch to draw with.</param>
        /// <param name="texture">The source containing the texture.</param>
        /// <param name="bounds">The target rectangle to draw to.</param>
        /// <param name="source">The source for the menu texture.</param>
        /// <param name="depth">The depth to draw to.</param>
        /// <param name="scale">The scale of the texture box.</param>
        /// <param name="tint">What color to tint the texture box.</param>
        public static void DrawTextureBox(this SpriteBatch batch, Texture2D texture, XnaRectangle bounds, XnaRectangle source, float depth, float scale, Color tint) {
            int num = source.Width / 3;
            batch.Draw(texture, new XnaRectangle((int) (num * (double) scale) + bounds.X, (int) (num * (double) scale) + bounds.Y, bounds.Width - (int) (num * (double) scale * 2.0), bounds.Height - (int) (num * (double) scale * 2.0)), new XnaRectangle(num + source.X, num + source.Y, num, num), tint, 0.0f, Vector2.Zero, SpriteEffects.None, depth);
            batch.Draw(texture, new Vector2(bounds.X, bounds.Y), new XnaRectangle(source.X, source.Y, num, num), tint, 0.0f, Vector2.Zero, scale, SpriteEffects.None, depth);
            batch.Draw(texture, new Vector2(bounds.X + bounds.Width - (int) (num * (double) scale), bounds.Y), new XnaRectangle(source.X + num * 2, source.Y, num, num), tint, 0.0f, Vector2.Zero, scale, SpriteEffects.None, depth);
            batch.Draw(texture, new Vector2(bounds.X, bounds.Y + bounds.Height - (int) (num * (double) scale)), new XnaRectangle(source.X, num * 2 + source.Y, num, num), tint, 0.0f, Vector2.Zero, scale, SpriteEffects.None, depth);
            batch.Draw(texture, new Vector2(bounds.X + bounds.Width - (int) (num * (double) scale), bounds.Y + bounds.Height - (int) (num * (double) scale)), new XnaRectangle(source.X + num * 2, num * 2 + source.Y, num, num), tint, 0.0f, Vector2.Zero, scale, SpriteEffects.None, depth);
            batch.Draw(texture, new XnaRectangle(bounds.X + (int) (num * (double) scale), bounds.Y, bounds.Width - (int) (num * (double) scale) * 2, (int) (num * (double) scale)), new XnaRectangle(source.X + num, source.Y, num, num), tint, 0.0f, Vector2.Zero, SpriteEffects.None, depth);
            batch.Draw(texture, new XnaRectangle(bounds.X + (int) (num * (double) scale), bounds.Y + bounds.Height - (int) (num * (double) scale), bounds.Width - (int) (num * (double) scale) * 2, (int) (num * (double) scale)), new XnaRectangle(source.X + num, num * 2 + source.Y, num, num), tint, 0.0f, Vector2.Zero, SpriteEffects.None, depth);
            batch.Draw(texture, new XnaRectangle(bounds.X, bounds.Y + (int) (num * (double) scale), (int) (num * (double) scale), bounds.Height - (int) (num * (double) scale) * 2), new XnaRectangle(source.X, num + source.Y, num, num), tint, 0.0f, Vector2.Zero, SpriteEffects.None, depth);
            batch.Draw(texture, new XnaRectangle(bounds.X + bounds.Width - (int) (num * (double) scale), bounds.Y + (int) (num * (double) scale), (int) (num * (double) scale), bounds.Height - (int) (num * (double) scale) * 2), new XnaRectangle(source.X + num * 2, num + source.Y, num, num), tint, 0.0f, Vector2.Zero, SpriteEffects.None, depth);
        }

        /// <summary>Converts a xTile <see cref="XTileRectangle"/> to a XNA <see cref="XnaRectangle"/>.</summary>
        /// <param name="source">The rectangle to convert.</param>
        /// <returns>A XNA <see cref="XnaRectangle"/> with the same dimensions as the source rectangle.</returns>
        public static XnaRectangle ToXnaRectangle(this XTileRectangle source) => new XnaRectangle(source.X, source.Y, source.Width, source.Height);

        /// <summary>Converts a XNA <see cref="XnaRectangle"/> to a xTile <see cref="XTileRectangle"/>.</summary>
        /// <param name="source">The rectangle to convert.</param>
        /// <returns>A xTile <see cref="XTileRectangle"/> with the same dimensions as the source rectangle.</returns>
        public static XTileRectangle ToXTileRectangle(this XnaRectangle source) => new XTileRectangle(source.X, source.Y, source.Width, source.Height);

        /// <summary>Calculates the intersection of two rectangles.</summary>
        /// <param name="a">The first rectangle.</param>
        /// <param name="b">The second rectangle.</param>
        /// <returns>A rectangle representing the intersection, or null if they do not intersect.</returns>
        public static XnaRectangle? Intersection(this XnaRectangle a, XnaRectangle b) => a.ToXTileRectangle().Intersection(b.ToXTileRectangle())?.ToXnaRectangle();

        /// <summary>Calculates the intersection of two rectangles.</summary>
        /// <param name="a">The first rectangle.</param>
        /// <param name="b">The second rectangle.</param>
        /// <returns>A rectangle representing the intersection, or null if they do not intersect.</returns>
        public static XTileRectangle? Intersection(this XTileRectangle a, XTileRectangle b) {
            int x1 = Math.Max(a.X, b.X);
            int y1 = Math.Max(a.Y, b.Y);
            int x2 = Math.Min(a.X + a.Width, b.X + b.Width);
            int y2 = Math.Min(a.Y + a.Height, b.Y + b.Height);

            // Make sure it's a valid rectangle (the rectangles intersect)
            if (x1 >= x2 || y1 >= y2)
                return null;

            return new XTileRectangle(x1, y1, x2 - x1, y2 - y1);
        }

        /// <summary>Draws a string with a shadow behind it.</summary>
        /// <param name="batch">The batch to draw with.</param>
        /// <param name="font">The font the text should use.</param>
        /// <param name="text">The string to draw.</param>
        /// <param name="position">The position of the string.</param>
        /// <param name="color">The color of the string. The shadow is black.</param>
        /// <param name="depth">The depth of the string.</param>
        /// <param name="shadowDepth">The depth of the shadow.</param>
        public static void DrawStringWithShadow(this SpriteBatch batch, SpriteFont font, string text, Vector2 position, Color color, float depth = 0F, float shadowDepth = 0.005F) {
            batch.DrawStringWithShadow(font, text, position, color, Color.Black, Vector2.One, depth, shadowDepth);
        }

        /// <summary>Draws a string with a shadow behind it.</summary>
        /// <param name="batch">The batch to draw with.</param>
        /// <param name="font">The font the text should use.</param>
        /// <param name="text">The string to draw.</param>
        /// <param name="position">The position of the string.</param>
        /// <param name="color">The color of the string. The shadow is black.</param>
        /// <param name="shadowColor">The color of the shadow.</param>
        /// <param name="scale">The amount to scale the size of the string by.</param>
        /// <param name="depth">The depth of the string.</param>
        /// <param name="shadowDepth">The depth of the shadow.</param>
        public static void DrawStringWithShadow(this SpriteBatch batch, SpriteFont font, string text, Vector2 position, Color color, Color shadowColor, Vector2 scale, float depth, float shadowDepth) {
            batch.DrawString(font, text, position + Vector2.One * Game1.pixelZoom * scale / 2f, shadowColor, 0F, Vector2.Zero, scale, SpriteEffects.None, shadowDepth);
            batch.DrawString(font, text, position, color, 0F, Vector2.Zero, scale, SpriteEffects.None, depth);
        }

        /// <summary>Draws a speech bubble over a <see cref="Character"/>.</summary>
        /// <param name="who">Who to draw the speech bubble over</param>
        /// <param name="text">The text in the speech bubble.</param>
        /// <param name="color">The color of the text.</param>
        /// <param name="style">0 for shaky bubble, any other number for static bubble.</param>
        public static void DrawSpeechBubble(this Character who, string text, Color color, int style = 1) {
            Vector2 local = Game1.GlobalToLocal(new Vector2(who.getStandingX(), who.getStandingY() - 192 + who.yJumpOffset));
            if (style == 0)
                local += new Vector2(Game1.random.Next(-1, 2), Game1.random.Next(-1, 2));
            SpriteText.drawStringWithScrollCenteredAt(Game1.spriteBatch, text, (int) local.X, (int) local.Y, "", 1F, (int) color.PackedValue, 1, (float) (who.getTileY() * 64 / 10000.0 + 1.0 / 1000.0 + who.getTileX() / 10000.0));
        }

        /// <summary>Executes some code with a given scissor rectangle (drawing will be limited to the rectangle)</summary>
        /// <param name="batch">The <see cref="SpriteBatch"/> being used for drawing calls</param>
        /// <param name="scissorRect">The rectangle to limit drawing to</param>
        /// <param name="action">The function to execute with the given scissor rectangle</param>
        /// <param name="respectExistingScissor">Whether to limit the new scissor rectangle to a subrectangle of the current scissor rectangle</param>
        public static void WithScissorRect(this SpriteBatch batch, XnaRectangle scissorRect, Action<SpriteBatch> action, bool respectExistingScissor = true) {
            // Stop the old drawing code
            batch.End();

            // Keep track of the old scissor rectangle (this needs to come after End() so they're applied to the GraphicsDevice)
            XnaRectangle oldScissor = batch.GraphicsDevice.ScissorRectangle;
            BlendState oldBlendState = batch.GraphicsDevice.BlendState;
            DepthStencilState oldStencilState = batch.GraphicsDevice.DepthStencilState;
            RasterizerState oldRasterizerState = batch.GraphicsDevice.RasterizerState;

            // Trim current scissor to the existing one if necessary
            if (respectExistingScissor)
                scissorRect = scissorRect.Intersection(oldScissor) ?? new XnaRectangle(0, 0, 0, 0);

            // Draw with the new scissor rectangle
            using (RasterizerState rasterizerState = new RasterizerState { ScissorTestEnable = true }) {
                batch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, rasterizerState);

                // Set scissor rectangle
                batch.GraphicsDevice.ScissorRectangle = scissorRect;

                // Perform the action
                action(batch);

                // Draw the batch
                batch.End();
            }

            // Reset scissor rectangle
            batch.GraphicsDevice.ScissorRectangle = oldScissor;

            // Return to last state
            batch.Begin(SpriteSortMode.BackToFront, oldBlendState, SamplerState.PointClamp, oldStencilState, oldRasterizerState);
        }
    }
}
