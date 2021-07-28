/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PlatoTK.Patching
{
    internal class SpriteBatchPatches
    {
        internal static bool _skipArea = false;

        internal static bool _patched = false;
        internal static HashSet<AreaDrawPatch> DrawPatches = new HashSet<AreaDrawPatch>();
        public static void InitializePatch()
        {
            if (_patched)
                return;

            _patched = true;

            var harmony = HarmonyInstance.Create($"Plato.DrawPatches");

            foreach (MethodInfo method in AccessTools.GetDeclaredMethods(typeof(SpriteBatch)).Where(m => m.IsPublic && m.Name == "Draw"))
            {
                List<Type> parameterTypes = new List<Type>() { typeof(SpriteBatch)};
                parameterTypes.AddRange(method.GetParameters().Select(p => p.ParameterType));
                if (AccessTools.DeclaredMethod(typeof(SpriteBatchPatches), "Draw", parameterTypes.ToArray()) is MethodInfo targetMethod)
                    harmony.Patch(method, new HarmonyMethod(targetMethod), null, null);
            }
        }

        public static bool DrawFix(SpriteBatch __instance, Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, Vector2 origin, float rotation = 0f, SpriteEffects effects = SpriteEffects.None, float layerDepth = 0f)
        {
            if (texture == null)
                return true;

            if (texture is IPlatoTexture platoTexture &&
                !platoTexture.SkipHandler
                && platoTexture.CallTextureHandler(
                    __instance,
                    texture,
                    destinationRectangle,
                    sourceRectangle,
                    color, origin,
                    rotation, effects,
                    layerDepth))
                return false;

            if (!_skipArea)
            {
                string tag = texture.Tag is string s ? s : "";
                string name = texture.Name is string n ? n : "";

                if (!sourceRectangle.HasValue)
                    sourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);

                if (name.Contains(":") && name.Split(':') is string[] sc && sc.Length == 3 && sc[0] == "Extended" && int.TryParse(sc[2], out int yAdjust))
                {
                    texture.Name = sc[1];
                    sourceRectangle = new Rectangle(sourceRectangle.Value.X, sourceRectangle.Value.Y + (yAdjust * 4096), sourceRectangle.Value.Width, sourceRectangle.Value.Height);
                }

                foreach (var patch in DrawPatches.Where(p => sourceRectangle.Value == p.TargetArea() || (tag == p.Id)))
                {
                    if (!(patch.Patch is Texture2D) || ((!patch.Texture(texture)) && !(tag == patch.Id)))
                        continue;

                    _skipArea = true;
                    __instance.Draw(patch.Patch, destinationRectangle, patch.SourceArea, color, rotation, origin, effects, layerDepth);
                    _skipArea = false;
                    return false;
                }
            }

            return true;
        }

        public static bool Draw(SpriteBatch __instance, Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, SpriteEffects effects, float layerDepth)
        {
            if (__instance is SpriteBatch && texture is Texture2D)
                return DrawFix(__instance, texture, destinationRectangle, sourceRectangle, color, origin, rotation, effects, layerDepth);

            return true;
        }
        public static bool Draw(SpriteBatch __instance, Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color)
        {
            if (__instance is SpriteBatch && texture is Texture2D)
                return DrawFix(__instance, texture, destinationRectangle, sourceRectangle, color, Vector2.Zero);

            return true;
        }
        public static bool Draw(SpriteBatch __instance, Texture2D texture, Rectangle destinationRectangle, Color color)
        {
            if (__instance is SpriteBatch && texture is Texture2D)
            {
                Rectangle sourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
                return DrawFix(__instance, texture, destinationRectangle, sourceRectangle, color, Vector2.Zero);
            }

            return true;
        }
        public static bool Draw(SpriteBatch __instance, Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
        {
            if (__instance is SpriteBatch && texture is Texture2D)
            {
                sourceRectangle = sourceRectangle.HasValue ? sourceRectangle.Value : new Rectangle(0, 0, texture.Width, texture.Height);
                return DrawFix(__instance, texture, new Rectangle((int)(position.X), (int)(position.Y), (int)(sourceRectangle.Value.Width * scale.X), (int)(sourceRectangle.Value.Height * scale.Y)), sourceRectangle, color, origin, rotation, effects, layerDepth);
            }

            return true;
        }
        public static bool Draw(SpriteBatch __instance, Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
        {
            if (__instance is SpriteBatch && texture is Texture2D)
            {
                sourceRectangle = sourceRectangle.HasValue ? sourceRectangle.Value : new Rectangle(0, 0, texture.Width, texture.Height);
                return DrawFix(__instance, texture, new Rectangle((int)(position.X), (int)(position.Y), (int)(sourceRectangle.Value.Width * scale), (int)(sourceRectangle.Value.Height * scale)), sourceRectangle, color, origin, rotation, effects, layerDepth);
            }

            return true;
        }
        public static bool Draw(SpriteBatch __instance, Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color)
        {
            if (__instance is SpriteBatch && texture is Texture2D)
            {
                sourceRectangle = sourceRectangle.HasValue ? sourceRectangle.Value : new Rectangle(0, 0, texture.Width, texture.Height);
                return DrawFix(__instance, texture, new Rectangle((int)(position.X), (int)(position.Y), (int)(sourceRectangle.Value.Width), (int)(sourceRectangle.Value.Height)), sourceRectangle, color, Vector2.Zero);
            }

            return true;
        }
        public static bool Draw(SpriteBatch __instance, Texture2D texture, Vector2 position, Color color)
        {
            if (__instance is SpriteBatch && texture is Texture2D)
            {
                Rectangle sourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
                return DrawFix(__instance, texture, new Rectangle((int)(position.X), (int)(position.Y), (int)(texture.Width), (int)(texture.Height)), sourceRectangle, color, Vector2.Zero);
            }

            return true;
        }
    }
}