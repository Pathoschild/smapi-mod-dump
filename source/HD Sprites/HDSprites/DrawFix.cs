using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace HDSprites
{
    // Modified from PyTK.Overrides.OvSpritebatchNew
    // Origial Source: https://github.com/Platonymous/Stardew-Valley-Mods/blob/master/PyTK/Overrides/OvSpritebatchNew.cs
    // Original Licence: GNU General Public License v3.0
    // Original Author: Platonymous
    public class DrawFix
    {
        internal static bool skip = false;
        internal static Dictionary<int, Color[]> WhiteBoxData = new Dictionary<int, Color[]>();

        internal static void InitializePatch(HarmonyInstance instance)
        {
            foreach (MethodInfo method in typeof(DrawFix).GetMethods(BindingFlags.Static | BindingFlags.Public).Where(m => m.Name == "Draw"))
                instance.Patch(typeof(SpriteBatch).GetMethod("Draw", method.GetParameters().Select(p => p.ParameterType).Where(t => !t.Name.Contains("SpriteBatch")).ToArray()), new HarmonyMethod(method), null, null);
        }

        public static bool _DrawFix(SpriteBatch __instance, Texture2D texture, Rectangle destination, Rectangle? sourceRectangle, Color color, Vector2 origin, float rotation = 0f, SpriteEffects effects = SpriteEffects.None, float layerDepth = 0f)
        {
            if (skip || !sourceRectangle.HasValue || !HDSpritesMod.EnableMod) return true;

            // Fix white box assets
            foreach (string fixAsset in HDSpritesMod.WhiteBoxFixAssets)
            {
                AssetTexture assetTexture;
                if (HDSpritesMod.AssetTextures.TryGetValue(fixAsset, out assetTexture)
                    && texture.Width == assetTexture.Width
                    && texture.Height == assetTexture.Height)
                {
                    int dataKey = assetTexture.Width * assetTexture.Height;
                    if (!WhiteBoxData.ContainsKey(dataKey)) WhiteBoxData.Add(dataKey, new Color[dataKey]);

                    Color[] data;
                    if (WhiteBoxData.TryGetValue(dataKey, out data))
                    {
                        texture.GetData(data);
                        if (assetTexture.CheckUniqueID(data)) texture = assetTexture;
                    }
                }
            }

            if (texture is AssetTexture a && sourceRectangle != null && sourceRectangle.Value is Rectangle r)
            {
                var newDestination = new Rectangle(destination.X, destination.Y, (int)destination.Width, (int)destination.Height);
                var newSR = new Rectangle?(new Rectangle((int)(r.X * a.Scale), (int)(r.Y * a.Scale), (int)(r.Width * a.Scale), (int)(r.Height * a.Scale)));
                var newOrigin = new Vector2(origin.X * a.Scale, origin.Y * a.Scale);

                if (a.HDTexture == null) return false;

                var t = a.HDTexture;
                if (r.Y * a.Scale > 4096 && a.EXTexture != null)
                {
                    newSR = new Rectangle?(new Rectangle((int)(r.X * a.Scale), (int)(r.Y * a.Scale) - 4096, (int)(r.Width * a.Scale), (int)(r.Height * a.Scale)));
                    t = a.EXTexture;
                }

                skip = true;
                __instance.Draw(t, newDestination, newSR, color, rotation, newOrigin, effects, layerDepth);
                skip = false;

                return false;
            }

            return true;
        }

        public static bool Draw(SpriteBatch __instance, Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, SpriteEffects effects, float layerDepth)
        {
            return _DrawFix(__instance, texture, destinationRectangle, sourceRectangle, color, origin, rotation, effects, layerDepth);
        }
        public static bool Draw(SpriteBatch __instance, Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color)
        {
            return _DrawFix(__instance, texture, destinationRectangle, sourceRectangle, color, Vector2.Zero);
        }
        public static bool Draw(SpriteBatch __instance, Texture2D texture, Rectangle destinationRectangle, Color color)
        {
            Rectangle sourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
            return _DrawFix(__instance, texture, destinationRectangle, sourceRectangle, color, Vector2.Zero);
        }
        public static bool Draw(SpriteBatch __instance, Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
        {
            sourceRectangle = sourceRectangle.HasValue ? sourceRectangle.Value : new Rectangle(0, 0, texture.Width, texture.Height);
            return _DrawFix(__instance, texture, new Rectangle((int)(position.X), (int)(position.Y), (int)(sourceRectangle.Value.Width * scale.X), (int)(sourceRectangle.Value.Height * scale.Y)), sourceRectangle, color, origin, rotation, effects, layerDepth);
        }
        public static bool Draw(SpriteBatch __instance, Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
        {
            sourceRectangle = sourceRectangle.HasValue ? sourceRectangle.Value : new Rectangle(0, 0, texture.Width, texture.Height);
            return _DrawFix(__instance, texture, new Rectangle((int)(position.X), (int)(position.Y), (int)(sourceRectangle.Value.Width * scale), (int)(sourceRectangle.Value.Height * scale)), sourceRectangle, color, origin, rotation, effects, layerDepth);
        }
        public static bool Draw(SpriteBatch __instance, Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color)
        {
            sourceRectangle = sourceRectangle.HasValue ? sourceRectangle.Value : new Rectangle(0, 0, texture.Width, texture.Height);
            return _DrawFix(__instance, texture, new Rectangle((int)(position.X), (int)(position.Y), (int)(sourceRectangle.Value.Width), (int)(sourceRectangle.Value.Height)), sourceRectangle, color, Vector2.Zero);
        }
        public static bool Draw(SpriteBatch __instance, Texture2D texture, Vector2 position, Color color)
        {
            Rectangle sourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
            return _DrawFix(__instance, texture, new Rectangle((int)(position.X), (int)(position.Y), (int)(texture.Width), (int)(texture.Height)), sourceRectangle, color, Vector2.Zero);

        }
    }
}
