using System;
using System.Reflection;
using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using TehPers.Core.Helpers.Static;
using TehPers.Core.Items.Managed;

namespace TehPers.Core.Items.Delegators {
    internal static class DrawingDelegator {
        private static bool _patched = false;

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

        // ReSharper disable once InconsistentNaming
        private static bool DrawPrefix1(SpriteBatch __instance, Texture2D texture, Vector2 position, Color color) {
            return !DrawingDelegator.DrawReplaced(texture, null, (t, s) => __instance.Draw(t, position, s, color));
        }

        // ReSharper disable once InconsistentNaming
        private static bool DrawPrefix2(SpriteBatch __instance, Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color) {
            return !DrawingDelegator.DrawReplaced(texture, sourceRectangle, (t, s) => __instance.Draw(t, position, s, color));
        }

        // ReSharper disable once InconsistentNaming
        private static bool DrawPrefix3(SpriteBatch __instance, Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth) {
            return !DrawingDelegator.DrawReplaced(texture, sourceRectangle, (t, s) => __instance.Draw(t, position, s, color, rotation, origin, scale, effects, layerDepth));
        }

        // ReSharper disable once InconsistentNaming
        private static bool DrawPrefix4(SpriteBatch __instance, Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth) {
            return !DrawingDelegator.DrawReplaced(texture, sourceRectangle, (t, s) => __instance.Draw(t, position, s, color, rotation, origin, scale, effects, layerDepth));
        }

        // ReSharper disable once InconsistentNaming
        private static bool DrawPrefix5(SpriteBatch __instance, Texture2D texture, Rectangle destinationRectangle, Color color) {
            return !DrawingDelegator.DrawReplaced(texture, null, (t, s) => __instance.Draw(t, destinationRectangle, s, color));
        }

        // ReSharper disable once InconsistentNaming
        private static bool DrawPrefix6(SpriteBatch __instance, Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color) {
            return !DrawingDelegator.DrawReplaced(texture, sourceRectangle, (t, s) => __instance.Draw(t, destinationRectangle, s, color));
        }

        // ReSharper disable once InconsistentNaming
        private static bool DrawPrefix7(SpriteBatch __instance, Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, SpriteEffects effects, float layerDepth) {
            return !DrawingDelegator.DrawReplaced(texture, sourceRectangle, (t, s) => __instance.Draw(t, destinationRectangle, s, color, rotation, origin, effects, layerDepth));
        }

        private static bool DrawReplaced(Texture2D texture, Rectangle? sourceRectangle, Action<Texture2D, Rectangle?> drawTexture) {
            // Custom item textures all come from springobjects
            if (texture != Game1.objectSpriteSheet)
                return false;

            // Make sure there is a source rectangle specified
            if (!sourceRectangle.HasValue)
                return false;

            // Check the source rectangle to see if it is associated with an existing item
            int index = DrawingDelegator.GetIndexForSourceRectangle(sourceRectangle.Value);
            if (!ItemApi.IndexToItem.TryGetValue(index, out IApiManagedObject managedObject)) {
                return false;
            }

            // Draw the managed object
            drawTexture(managedObject.GetTexture(), managedObject.GetSourceRectangle());
            return true;
        }

        private static int GetIndexForSourceRectangle(Rectangle sourceRectangle) {
            const int tileWidth = 32;
            const int tileHeight = 32;
            int x = sourceRectangle.X / tileWidth;
            int y = sourceRectangle.Y / tileHeight;
            int w = Game1.objectSpriteSheet.Width / tileWidth;
            return x * w + y;
        }
    }
}