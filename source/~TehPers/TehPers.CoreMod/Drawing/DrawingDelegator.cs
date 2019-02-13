using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TehPers.CoreMod.Api.Drawing;
using TehPers.CoreMod.Api.Structs;

namespace TehPers.CoreMod.Drawing {
    internal static class DrawingDelegator {
        private static bool _patched = false;
        private static bool _drawing = false;
        private static readonly ConditionalWeakTable<Texture2D, TrackedTexture> _textureToTrackedTexture = new ConditionalWeakTable<Texture2D, TrackedTexture>();
        private static readonly Dictionary<AssetLocation, TrackedTexture> _assetToTrackedTexture = new Dictionary<AssetLocation, TrackedTexture>();

        public static void PatchIfNeeded() {
            if (DrawingDelegator._patched) return;
            DrawingDelegator._patched = true;

            HarmonyInstance harmony = HarmonyInstance.Create("TehPers.CoreMod.DrawingDelegator");
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

            // Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, SpriteEffects effects, float layerDepth)
            target = targetType.GetMethod(nameof(SpriteBatch.Draw), new[] { typeof(Texture2D), typeof(Rectangle), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(SpriteEffects), typeof(float) });
            replacement = typeof(DrawingDelegator).GetMethod(nameof(DrawingDelegator.DrawPrefix7), BindingFlags.NonPublic | BindingFlags.Static);
            harmony.Patch(target, new HarmonyMethod(replacement));
        }

        public static TrackedTexture GetOrCreateTrackedTexture(AssetLocation location, Func<Texture2D> textureFactory) {
            // If a tracked texture for this location already exists, just return that
            if (DrawingDelegator._assetToTrackedTexture.TryGetValue(location, out TrackedTexture trackedTexture)) {
                return trackedTexture;
            }

            // Create the tracked texture
            trackedTexture = new TrackedTexture(textureFactory());
            DrawingDelegator._assetToTrackedTexture.Add(location, trackedTexture);
            DrawingDelegator._textureToTrackedTexture.Add(trackedTexture.CurrentTexture, trackedTexture);
            return trackedTexture;
        }

        public static void UpdateTexture(AssetLocation location, Texture2D newTexture) {
            if (DrawingDelegator._assetToTrackedTexture.TryGetValue(location, out TrackedTexture trackedTexture)) {
                trackedTexture.CurrentTexture = newTexture;
                DrawingDelegator._textureToTrackedTexture.GetValue(newTexture, _ => trackedTexture);
            }
        }

        #region Patches
        private static bool DrawPrefix1(SpriteBatch __instance, Texture2D texture, Vector2 position, Color color) {
            return !DrawingDelegator.DrawReplaced(new DrawingInfo(__instance, texture, null, position, color, Vector2.Zero, 0, SpriteEffects.None, 0));
        }

        private static bool DrawPrefix2(SpriteBatch __instance, Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color) {
            return !DrawingDelegator.DrawReplaced(new DrawingInfo(__instance, texture, sourceRectangle, position, color, Vector2.Zero, 0, SpriteEffects.None, 0));
        }

        private static bool DrawPrefix3(SpriteBatch __instance, Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth) {
            DrawingInfo info = new DrawingInfo(__instance, texture, sourceRectangle, position, color, origin, rotation, effects, layerDepth);
            info.SetScale(scale);
            return !DrawingDelegator.DrawReplaced(info);
        }

        private static bool DrawPrefix4(SpriteBatch __instance, Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth) {
            DrawingInfo info = new DrawingInfo(__instance, texture, sourceRectangle, position, color, origin, rotation, effects, layerDepth);
            info.SetScale(scale);
            return !DrawingDelegator.DrawReplaced(info);
        }

        private static bool DrawPrefix5(SpriteBatch __instance, Texture2D texture, Rectangle destinationRectangle, Color color) {
            return !DrawingDelegator.DrawReplaced(new DrawingInfo(__instance, texture, null, destinationRectangle, color, Vector2.Zero, 0, SpriteEffects.None, 0));
        }

        private static bool DrawPrefix6(SpriteBatch __instance, Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color) {
            return !DrawingDelegator.DrawReplaced(new DrawingInfo(__instance, texture, sourceRectangle, destinationRectangle, color, Vector2.Zero, 0, SpriteEffects.None, 0));
        }

        private static bool DrawPrefix7(SpriteBatch __instance, Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, SpriteEffects effects, float layerDepth) {
            return !DrawingDelegator.DrawReplaced(new DrawingInfo(__instance, texture, sourceRectangle, destinationRectangle, color, origin, rotation, effects, layerDepth));
        }
        #endregion

        private static bool DrawReplaced(DrawingInfo info) {
            // Don't override if currently patching
            if (DrawingDelegator._drawing) return false;

            // Check if this texture is being tracked
            if (!DrawingDelegator._textureToTrackedTexture.TryGetValue(info.Texture, out TrackedTexture trackedTexture)) return false;

            // Check if any overrides handle this drawing call
            bool modified = false;
            foreach (EventHandler<IDrawingInfo> drawingHandler in trackedTexture.GetDrawingHandlers()) {
                // Set the drawing info's Modified property to false, then execute the overrider
                info.Reset();
                drawingHandler(null, info);

                // Check if modified
                if (info.Modified) modified = true;

                // Check if should continue propagating
                if (!info.Propagate || info.Cancelled) break;
            }

            // Check if any handlers modified the drawing info
            if (!modified) {
                RaiseAfterDrawn();
                return false;
            }

            // Call the native draw code if not cancelled
            if (!info.Cancelled) {
                DrawingDelegator._drawing = true;
                info.Draw();
                DrawingDelegator._drawing = false;
                RaiseAfterDrawn();
            }

            // Return whether it was handled
            return true;

            void RaiseAfterDrawn() {
                ReadonlyDrawingInfo finalInfo = new ReadonlyDrawingInfo(info);
                foreach (EventHandler<IReadonlyDrawingInfo> drawnHandler in trackedTexture.GetDrawnHandlers()) {
                    drawnHandler(null, finalInfo);
                }
            }
        }
    }
}