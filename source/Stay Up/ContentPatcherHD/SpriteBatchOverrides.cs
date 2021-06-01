/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/su226/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Reflection;
using System.Linq;
using Harmony;

namespace Su226.ContentPatcherHD {
  class SpriteBatchOverrides {
    public static void PatchAll(HarmonyInstance harmony) {
      foreach (MethodInfo method in typeof(SpriteBatchOverrides).GetMember("Draw")) {
        harmony.Patch(
          typeof(SpriteBatch).GetMethod("Draw", method.GetParameters().Skip(1).Select(p => p.ParameterType).ToArray()),
          new HarmonyMethod(method)
        );
      }
    }

    public static bool DoDraw(SpriteBatch __instance, Texture2D texture, Rectangle destinationRectangle, Rectangle sourceRectangle, Color color, float rotation, Vector2 origin, SpriteEffects effects, float layerDepth) {
      if (texture is Texture2DWrapper wrapper) {
        sourceRectangle = Texture2DWrapper.MultiplyRect(sourceRectangle, wrapper.Scale);
        __instance.Draw(wrapper.Wrapped, destinationRectangle, sourceRectangle, color, rotation, origin * wrapper.Scale, effects, layerDepth);
        return false;
      }
      return true;
    }

    public static bool Draw(SpriteBatch __instance, Texture2D texture, Rectangle destinationRectangle, Color color) {
      Rectangle realSourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
      return DoDraw(__instance, texture, destinationRectangle, realSourceRectangle, color, 0, Vector2.Zero, SpriteEffects.None, 0);
    }
    public static bool Draw(SpriteBatch __instance, Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color) {
      Rectangle realSourceRectangle = sourceRectangle ?? new Rectangle(0, 0, texture.Width, texture.Height);
      return DoDraw(__instance, texture, destinationRectangle, realSourceRectangle, color, 0, Vector2.Zero, SpriteEffects.None, 0);
    }
    public static bool Draw(SpriteBatch __instance, Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, SpriteEffects effects, float layerDepth) {
      Rectangle realSourceRectangle = sourceRectangle ?? new Rectangle(0, 0, texture.Width, texture.Height);
      return DoDraw(__instance, texture, destinationRectangle, realSourceRectangle, color, rotation, origin, effects, layerDepth);
    }
    public static bool Draw(SpriteBatch __instance, Texture2D texture, Vector2 position, Color color) {
      Rectangle destinationRectangle = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
      Rectangle realSourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
      return DoDraw(__instance, texture, destinationRectangle, realSourceRectangle, color, 0, Vector2.Zero, SpriteEffects.None, 0);
    }
    public static bool Draw(SpriteBatch __instance, Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color) {
      int width = sourceRectangle?.Width ?? texture.Width;
      int height = sourceRectangle?.Height ?? texture.Height;
      Rectangle destinationRectangle = new Rectangle((int)position.X, (int)position.Y, width, height);
      Rectangle realSourceRectangle = sourceRectangle ?? new Rectangle(0, 0, texture.Width, texture.Height);
      return DoDraw(__instance, texture, destinationRectangle, realSourceRectangle, color, 0, Vector2.Zero, SpriteEffects.None, 0);
    }
    public static bool Draw(SpriteBatch __instance, Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth) {
      int width = (int)((sourceRectangle?.Width ?? texture.Width) * scale);
      int height = (int)((sourceRectangle?.Height ?? texture.Height) * scale);
      Rectangle destinationRectangle = new Rectangle((int)position.X, (int)position.Y, width, height);
      Rectangle realSourceRectangle = sourceRectangle ?? new Rectangle(0, 0, texture.Width, texture.Height);
      return DoDraw(__instance, texture, destinationRectangle, realSourceRectangle, color, rotation, origin, effects, layerDepth);
    }
    public static bool Draw(SpriteBatch __instance, Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth) {
      int width = (int)((sourceRectangle?.Width ?? texture.Width) * scale.X);
      int height = (int)((sourceRectangle?.Height ?? texture.Height) * scale.Y);
      Rectangle destinationRectangle = new Rectangle((int)position.X, (int)position.Y, width, height);
      Rectangle realSourceRectangle = sourceRectangle ?? new Rectangle(0, 0, texture.Width, texture.Height);
      return DoDraw(__instance, texture, destinationRectangle, realSourceRectangle, color, rotation, origin, effects, layerDepth);
    }
  }
}
