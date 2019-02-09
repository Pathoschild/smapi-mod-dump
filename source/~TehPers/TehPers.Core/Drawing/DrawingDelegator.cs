using System;
using System.Collections.Generic;
using System.Reflection;
using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace TehPers.Core.Drawing {
    public static class DrawingDelegator {
        private static bool _patched = false;
        private static readonly List<DrawingOverrider> _overrides = new List<DrawingOverrider>();
        private static bool _drawing = false;

        public static void PatchIfNeeded() {
            if (DrawingDelegator._patched) {
                return;
            }
            DrawingDelegator._patched = true;

            HarmonyInstance harmony = HarmonyInstance.Create("TehPers.Core.Items.DrawingDelegator");
            Type targetType = typeof(SpriteBatch);

            // Draw(Texture2D texture, Vector2 position, Color color)
            MethodInfo target = targetType.GetMethod(nameof(SpriteBatch.Draw), new[] { typeof(Texture2D), typeof(Vector2), typeof(Color) });
            MethodInfo replacement = typeof(DrawingDelegator).GetMethod(nameof(DrawingDelegator.DrawPrefix1), BindingFlags.NonPublic | BindingFlags.Static);
            harmony.Patch(target, new HarmonyMethod(replacement));

            // Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color)
            target = targetType.GetMethod(nameof(SpriteBatch.Draw), new[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color) });
            replacement = typeof(DrawingDelegator).GetMethod(nameof(DrawingDelegator.DrawPrefix2), BindingFlags.NonPublic | BindingFlags.Static);
            harmony.Patch(target, new HarmonyMethod(replacement));

            // Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
            target = targetType.GetMethod(nameof(SpriteBatch.Draw), new[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) });
            replacement = typeof(DrawingDelegator).GetMethod(nameof(DrawingDelegator.DrawPrefix3), BindingFlags.NonPublic | BindingFlags.Static);
            harmony.Patch(target, new HarmonyMethod(replacement));

            // Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
            target = targetType.GetMethod(nameof(SpriteBatch.Draw), new[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(Vector2), typeof(SpriteEffects), typeof(float) });
            replacement = typeof(DrawingDelegator).GetMethod(nameof(DrawingDelegator.DrawPrefix4), BindingFlags.NonPublic | BindingFlags.Static);
            harmony.Patch(target, new HarmonyMethod(replacement));

            // Draw(Texture2D texture, Rectangle destinationRectangle, Color color)
            target = targetType.GetMethod(nameof(SpriteBatch.Draw), new[] { typeof(Texture2D), typeof(Rectangle), typeof(Color) });
            replacement = typeof(DrawingDelegator).GetMethod(nameof(DrawingDelegator.DrawPrefix5), BindingFlags.NonPublic | BindingFlags.Static);
            harmony.Patch(target, new HarmonyMethod(replacement));

            // Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color)
            target = targetType.GetMethod(nameof(SpriteBatch.Draw), new[] { typeof(Texture2D), typeof(Rectangle), typeof(Rectangle?), typeof(Color) });
            replacement = typeof(DrawingDelegator).GetMethod(nameof(DrawingDelegator.DrawPrefix6), BindingFlags.NonPublic | BindingFlags.Static);
            harmony.Patch(target, new HarmonyMethod(replacement));

            // Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
            // Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, SpriteEffects effects, float layerDepth)
            target = targetType.GetMethod(nameof(SpriteBatch.Draw), new[] { typeof(Texture2D), typeof(Rectangle), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(SpriteEffects), typeof(float) });
            replacement = typeof(DrawingDelegator).GetMethod(nameof(DrawingDelegator.DrawPrefix7), BindingFlags.NonPublic | BindingFlags.Static);
            harmony.Patch(target, new HarmonyMethod(replacement));
        }

        public static void AddOverride(DrawingOverrider overrider) {
            DrawingDelegator._overrides.Add(overrider);
        }

        public static bool RemoveOverride(DrawingOverrider overrider) {
            return DrawingDelegator._overrides.Remove(overrider);
        }

        #region Patches
        private static bool DrawPrefix1(SpriteBatch __instance, Texture2D texture, Vector2 position, Color color) {
            return !DrawingDelegator.DrawReplaced(texture, null, color, Vector2.One, info => {
                __instance.Draw(info.Texture, position, info.SourceRectangle, info.Tint, 0, Vector2.Zero, info.Scale, SpriteEffects.None, 0);
            });
        }

        private static bool DrawPrefix2(SpriteBatch __instance, Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color) {
            return !DrawingDelegator.DrawReplaced(texture, sourceRectangle, color, Vector2.One, info => {
                __instance.Draw(info.Texture, position, info.SourceRectangle, info.Tint, 0, Vector2.Zero, info.Scale, SpriteEffects.None, 0);
            });
        }

        private static bool DrawPrefix3(SpriteBatch __instance, Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth) {
            Vector2 scaleVector = new Vector2(scale, scale);
            return !DrawingDelegator.DrawReplaced(texture, sourceRectangle, color, scaleVector, info => {
                Vector2 newOrigin = new Vector2(origin.X * scaleVector.X / info.Scale.X, origin.Y * scaleVector.Y / info.Scale.Y);
                __instance.Draw(info.Texture, position, info.SourceRectangle, info.Tint, rotation, newOrigin, info.Scale, effects, layerDepth);
            });
        }

        private static bool DrawPrefix4(SpriteBatch __instance, Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth) {
            return !DrawingDelegator.DrawReplaced(texture, sourceRectangle, color, scale, info => {
                Vector2 newOrigin = new Vector2(origin.X * scale.X / info.Scale.X, origin.Y * scale.Y / info.Scale.Y);
                __instance.Draw(info.Texture, position, info.SourceRectangle, info.Tint, rotation, newOrigin, info.Scale, effects, layerDepth);
            });
        }

        private static bool DrawPrefix5(SpriteBatch __instance, Texture2D texture, Rectangle destinationRectangle, Color color) {
            Rectangle sourceBounds = texture.Bounds;
            Vector2 scaleVector = new Vector2((float) destinationRectangle.Width / sourceBounds.Width, (float) destinationRectangle.Height / sourceBounds.Height);
            return !DrawingDelegator.DrawReplaced(texture, null, color, scaleVector, info => {
                Rectangle dest = new Rectangle(destinationRectangle.X, destinationRectangle.Y, (int) (sourceBounds.Width * info.Scale.X), (int) (sourceBounds.Height * info.Scale.Y));
                __instance.Draw(info.Texture, dest, info.SourceRectangle, info.Tint);
            });
        }

        private static bool DrawPrefix6(SpriteBatch __instance, Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color) {
            Rectangle sourceBounds = sourceRectangle ?? texture.Bounds;
            Vector2 scaleVector = new Vector2((float) destinationRectangle.Width / sourceBounds.Width, (float) destinationRectangle.Height / sourceBounds.Height);
            return !DrawingDelegator.DrawReplaced(texture, sourceRectangle, color, scaleVector, info => {
                Rectangle dest = new Rectangle(destinationRectangle.X, destinationRectangle.Y, (int) (sourceBounds.Width * info.Scale.X), (int) (sourceBounds.Height * info.Scale.Y));
                __instance.Draw(info.Texture, dest, info.SourceRectangle, info.Tint);
            });
        }

        private static bool DrawPrefix7(SpriteBatch __instance, Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, SpriteEffects effects, float layerDepth) {
            Rectangle sourceBounds = sourceRectangle ?? texture.Bounds;
            Vector2 scaleVector = new Vector2((float) destinationRectangle.Width / sourceBounds.Width, (float) destinationRectangle.Height / sourceBounds.Height);
            return !DrawingDelegator.DrawReplaced(texture, sourceRectangle, color, scaleVector, info => {
                Rectangle dest = new Rectangle(destinationRectangle.X, destinationRectangle.Y, (int) (sourceBounds.Width * info.Scale.X), (int) (sourceBounds.Height * info.Scale.Y));
                Vector2 newOrigin = new Vector2(origin.X * scaleVector.X / info.Scale.X, origin.Y * scaleVector.Y / info.Scale.Y);
                __instance.Draw(info.Texture, dest, info.SourceRectangle, info.Tint, rotation, newOrigin, effects, layerDepth);
            });
        }
        #endregion

        private static bool DrawReplaced(Texture2D texture, in Rectangle? sourceRectangle, in Color tint, in Vector2 scale, NativeDraw nativeDraw) {
            // Don't override if currently patching
            if (DrawingDelegator._drawing) {
                return false;
            }

            // Create the drawing info object
            Action resetSignal = null;
            DrawingInfo info = new DrawingInfo(texture, sourceRectangle, tint, scale, nativeDraw, signal => resetSignal = signal);

            // Check if any overrides handle this drawing info
            //DrawingOverrider handler = DrawingDelegator._overrides.FirstOrDefault(overrider => overrider(info));
            DrawingOverrider handler = null;
            foreach (DrawingOverrider overrider in DrawingDelegator._overrides) {
                // Set the drawingInfo's Modified property to false, then execute the overrider
                resetSignal();
                overrider(info);

                // Check if modified
                if (info.Modified) {
                    handler = overrider;
                }

                // Check if should continue propagating
                if (!info.Propagate || info.Cancelled) {
                    break;
                }
            }

            // Check if any handlers modified the drawing info
            if (handler == null) {
                return false;
            }

            // Call the native draw code if not cancelled
            if (!info.Cancelled) {
                DrawingDelegator._drawing = true;
                nativeDraw(info);
                DrawingDelegator._drawing = false;
            }

            // Return whether it was handled
            return true;
        }

        public static int GetIndexForSourceRectangle(Rectangle sourceRectangle) {
            const int tileWidth = 16;
            const int tileHeight = 16;
            int x = sourceRectangle.X / tileWidth;
            int y = sourceRectangle.Y / tileHeight;
            int w = Game1.objectSpriteSheet.Width / tileWidth;
            return y * w + x;
        }

        public static Rectangle GetSourceRectangleForIndex(int index) {
            const int tileWidth = 16;
            const int tileHeight = 16;
            return Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, index, tileWidth, tileHeight);
        }

        internal delegate void NativeDraw(DrawingInfo info);

        /// <summary>Overrider for drawing calls to <see cref="SpriteBatch"/>.</summary>
        /// <param name="info">Information about what is being drawn.</param>
        public delegate void DrawingOverrider(DrawingInfo info);
    }

    public interface IDrawingHandler {
        bool ShouldHandle(DrawingInfo info);
        bool Handle(DrawingInfo info);
    }
}