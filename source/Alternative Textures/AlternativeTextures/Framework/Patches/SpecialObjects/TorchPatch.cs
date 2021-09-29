/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/AlternativeTextures
**
*************************************************/

using AlternativeTextures;
using AlternativeTextures.Framework.Models;
using AlternativeTextures.Framework.Patches.StandardObjects;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Object = StardewValley.Object;

namespace AlternativeTextures.Framework.Patches.SpecialObjects
{
    internal class TorchPatch : PatchTemplate
    {
        private readonly Type _object = typeof(Torch);

        internal TorchPatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_object, nameof(Torch.draw), new[] { typeof(SpriteBatch), typeof(int), typeof(int), typeof(float) }), prefix: new HarmonyMethod(GetType(), nameof(DrawPrefix)));
            harmony.Patch(AccessTools.Method(_object, nameof(Torch.placementAction), new[] { typeof(GameLocation), typeof(int), typeof(int), typeof(Farmer) }), postfix: new HarmonyMethod(GetType(), nameof(PlacementActionPostfix)));
        }

        private static bool DrawPrefix(Torch __instance, Vector2[] ___ashes, SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
        {
            if (__instance.modData.ContainsKey("AlternativeTextureName"))
            {
                var textureModel = AlternativeTextures.textureManager.GetSpecificTextureModel(__instance.modData["AlternativeTextureName"]);
                if (textureModel is null)
                {
                    return true;
                }

                var textureVariation = Int32.Parse(__instance.modData["AlternativeTextureVariation"]);
                if (textureVariation == -1 || AlternativeTextures.modConfig.IsTextureVariationDisabled(textureModel.GetId(), textureVariation))
                {
                    return true;
                }
                var textureOffset = textureVariation * textureModel.TextureHeight;

                if (Game1.eventUp && (Game1.currentLocation == null || Game1.currentLocation.currentEvent == null || !Game1.currentLocation.currentEvent.showGroundObjects) && !Game1.currentLocation.IsFarm)
                {
                    return false;
                }

                if (!__instance.bigCraftable)
                {
                    Rectangle sourceRect = new Rectangle(0, textureOffset, textureModel.TextureWidth, textureModel.TextureHeight);
                    sourceRect.Y += 8;
                    sourceRect.Height /= 2;
                    Vector2 position2 = Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64, y * 64 + 32));
                    Rectangle? sourceRectangle = sourceRect;
                    Color white = Color.White;
                    Vector2 zero = Vector2.Zero;

                    spriteBatch.Draw(textureModel.GetTexture(textureVariation), position2, sourceRectangle, white, 0f, zero, (__instance.scale.Y > 1f) ? __instance.getScale().Y : 4f, SpriteEffects.None, (float)__instance.getBoundingBox(new Vector2(x, y)).Bottom / 10000f);
                    spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64, y * 64 + 32)), new Rectangle(88, 1779, 30, 30), Color.PaleGoldenrod * (Game1.currentLocation.IsOutdoors ? 0.35f : 0.43f), 0f, new Vector2(15f, 15f), 4f + (float)(64.0 * Math.Sin((Game1.currentGameTime.TotalGameTime.TotalMilliseconds + (double)(x * 64 * 777) + (double)(y * 64 * 9746)) % 3140.0 / 1000.0) / 50.0), SpriteEffects.None, 1f);

                    sourceRect.X = 276 + (int)((Game1.currentGameTime.TotalGameTime.TotalMilliseconds + (double)(x * 3204) + (double)(y * 49)) % 700.0 / 100.0) * 8;
                    sourceRect.Y = 1965;
                    sourceRect.Width = 8;
                    sourceRect.Height = 8;
                    spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64 + 32 + 4, y * 64 + 16 + 4)), sourceRect, Color.White * 0.75f, 0f, new Vector2(4f, 4f), 3f, SpriteEffects.None, (float)(__instance.getBoundingBox(new Vector2(x, y)).Bottom + 1) / 10000f);

                    for (int i = 0; i < ___ashes.Length; i++)
                    {
                        spriteBatch.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(x * 64 + 32) + ___ashes[i].X, (float)(y * 64 + 32) + ___ashes[i].Y)), new Rectangle(344 + i % 3, 53, 1, 1), Color.White * 0.5f * ((-100f - ___ashes[i].Y / 2f) / -100f), 0f, Vector2.Zero, 3f, SpriteEffects.None, (float)__instance.getBoundingBox(new Vector2(x, y)).Bottom / 10000f);
                    }
                    return false;
                }
                __instance.draw(spriteBatch, x, y, alpha);
                float draw_layer = Math.Max(0f, (float)((y + 1) * 64 - 24) / 10000f) + (float)x * 1E-05f;

                if (!__instance.isOn)
                {
                    return false;
                }

                if ((int)__instance.parentSheetIndex == 146 || (int)__instance.parentSheetIndex == 278)
                {
                    spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64 + 16 - 4, y * 64 - 8)), new Rectangle(276 + (int)((Game1.currentGameTime.TotalGameTime.TotalMilliseconds + (double)(x * 3047) + (double)(y * 88)) % 400.0 / 100.0) * 12, 1985, 12, 11), Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, draw_layer + 0.0008f);
                    spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64 + 32 - 12, y * 64)), new Rectangle(276 + (int)((Game1.currentGameTime.TotalGameTime.TotalMilliseconds + (double)(x * 2047) + (double)(y * 98)) % 400.0 / 100.0) * 12, 1985, 12, 11), Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, draw_layer + 0.0009f);
                    spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64 + 32 - 20, y * 64 + 12)), new Rectangle(276 + (int)((Game1.currentGameTime.TotalGameTime.TotalMilliseconds + (double)(x * 2077) + (double)(y * 98)) % 400.0 / 100.0) * 12, 1985, 12, 11), Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, draw_layer + 0.001f);

                    if ((int)__instance.parentSheetIndex == 278)
                    {
                        ObjectPatch.DrawPrefix(__instance, spriteBatch, x, y, alpha);
                    }
                }
                else
                {
                    spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64 + 16 - 8, y * 64 - 64 + 8)), new Rectangle(276 + (int)((Game1.currentGameTime.TotalGameTime.TotalMilliseconds + (double)(x * 3047) + (double)(y * 88)) % 400.0 / 100.0) * 12, 1985, 12, 11), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, draw_layer + 0.0008f);
                }

                return false;
            }
            return true;
        }

        internal static void PlacementActionPostfix(Torch __instance, bool __result, GameLocation location, int x, int y, Farmer who)
        {
            ObjectPatch.PlacementActionPostfix(__instance, __result, location, x, y, who);
        }
    }
}
