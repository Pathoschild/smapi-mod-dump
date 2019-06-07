using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace HDSprites
{
    // Modified from PyTK.Overrides.OvSpritebatch
    // Origial Source: https://github.com/Platonymous/Stardew-Valley-Mods/blob/master/PyTK/Overrides/OvSpritebatch.cs
    // Original Licence: GNU General Public License v3.0
    // Original Author: Platonymous
    public class DrawFix
    {

        internal static MethodInfo drawMethod = AccessTools.Method(Type.GetType("Microsoft.Xna.Framework.Graphics.SpriteBatch, Microsoft.Xna.Framework.Graphics"), "InternalDraw");

        [HarmonyPatch]
        internal class SpriteBatchFix
        {
            internal static MethodInfo TargetMethod()
            {
                if (Type.GetType("Microsoft.Xna.Framework.Graphics.SpriteBatch, Microsoft.Xna.Framework.Graphics") != null)
                    return AccessTools.Method(Type.GetType("Microsoft.Xna.Framework.Graphics.SpriteBatch, Microsoft.Xna.Framework.Graphics"), "InternalDraw");
                else
                    return AccessTools.Method(typeof(FakeSpriteBatch), "InternalDraw");
            }

            static Dictionary<int, Color[]> WhiteBoxData = new Dictionary<int, Color[]>();
            static bool skip = false;

            internal static bool Prefix(ref SpriteBatch __instance, ref Texture2D texture, ref Vector4 destination, ref bool scaleDestination, ref Rectangle? sourceRectangle, ref Color color, ref float rotation, ref Vector2 origin, ref SpriteEffects effects, ref float depth)
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
                            if (assetTexture.checkUniqueID(data)) texture = assetTexture;
                        }
                    }
                }

                if (texture is AssetTexture a && sourceRectangle != null && sourceRectangle.Value is Rectangle r)
                {
                    var newDestination = new Vector4(destination.X, destination.Y, destination.Z / a.Scale, destination.W / a.Scale);
                    var newSR = new Rectangle?(new Rectangle((int)(r.X * a.Scale), (int)(r.Y * a.Scale), (int)(r.Width * a.Scale), (int)(r.Height * a.Scale)));
                    var newOrigin = new Vector2(origin.X * a.Scale, origin.Y * a.Scale);

                    // Fix scaling assets
                    if (HDSpritesMod.ScaleFixAssets.Contains(a.AssetName))
                    {
                        if (!scaleDestination) newDestination = destination;
                    }

                    skip = true;
                    drawMethod.Invoke(__instance, new object[] { a.STexture, newDestination, scaleDestination, newSR, color, rotation, newOrigin, effects, depth });
                    skip = false;

                    return false;
                }

                return true;
            }

            internal class FakeSpriteBatch
            {
                internal void DrawInternal(Texture2D texture, Vector4 destinationRectangle, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, SpriteEffects effect, float depth, bool autoFlush) { return; }
                internal void InternalDraw(Texture2D texture, ref Vector4 destination, bool scaleDestination, ref Rectangle? sourceRectangle, Color color, float rotation, ref Vector2 origin, SpriteEffects effects, float depth) { return; }
            }
        }
    }
}
