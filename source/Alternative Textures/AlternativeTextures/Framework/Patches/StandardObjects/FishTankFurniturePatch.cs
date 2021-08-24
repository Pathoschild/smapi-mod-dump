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
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
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

namespace AlternativeTextures.Framework.Patches.StandardObjects
{
    internal class FishTankFurniturePatch : PatchTemplate
    {
        private readonly Type _object = typeof(FishTankFurniture);

        internal FishTankFurniturePatch(IMonitor modMonitor) : base(modMonitor)
        {

        }

        internal void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_object, nameof(FishTankFurniture.draw), new[] { typeof(SpriteBatch), typeof(int), typeof(int), typeof(float) }), prefix: new HarmonyMethod(GetType(), nameof(DrawPrefix)));
        }

        private static bool DrawPrefix(FishTankFurniture __instance, NetInt ___sourceIndexOffset, NetVector2 ___drawPosition, SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
        {
            if (__instance.modData.ContainsKey("AlternativeTextureName"))
            {
                var textureModel = AlternativeTextures.textureManager.GetSpecificTextureModel(__instance.modData["AlternativeTextureName"]);
                if (textureModel is null)
                {
                    return true;
                }

                var textureVariation = Int32.Parse(__instance.modData["AlternativeTextureVariation"]);
                if (textureVariation == -1)
                {
                    return true;
                }
                var textureOffset = textureVariation * textureModel.TextureHeight;

                Vector2 shake = Vector2.Zero;
                if (!__instance.isTemporarilyInvisible)
                {
                    Vector2 draw_position = ___drawPosition.Value;
                    if (!Furniture.isDrawingLocationFurniture)
                    {
                        draw_position = new Vector2(x, y) * 64f;
                        draw_position.Y -= __instance.sourceRect.Height * 4 - __instance.boundingBox.Height;
                    }
                    if (__instance.shakeTimer > 0)
                    {
                        shake = new Vector2(Game1.random.Next(-1, 2), Game1.random.Next(-1, 2));
                    }

                    Rectangle sourceRect = new Rectangle(__instance.sourceRect.Value.Width, __instance.sourceRect.Value.Y, __instance.sourceRect.Value.Width, __instance.sourceRect.Value.Height);
                    sourceRect.Y = textureOffset;

                    spriteBatch.Draw(textureModel.Texture, Game1.GlobalToLocal(Game1.viewport, draw_position + shake), sourceRect, Color.White * alpha, 0f, Vector2.Zero, 4f, __instance.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, __instance.GetGlassDrawLayer());
                    if (Furniture.isDrawingLocationFurniture)
                    {
                        int hatsDrawn = 0;
                        for (int i = 0; i < __instance.tankFish.Count; i++)
                        {
                            TankFish fish = __instance.tankFish[i];
                            float fish_layer = Utility.Lerp(__instance.GetFishSortRegion().Y, __instance.GetFishSortRegion().X, fish.zPosition / 20f);
                            fish_layer += 1E-07f * (float)i;
                            fish.Draw(spriteBatch, alpha, fish_layer);
                            if (fish.fishIndex != 86)
                            {
                                continue;
                            }
                            int hatsSoFar = 0;
                            foreach (Item h in __instance.heldItems)
                            {
                                if (h is Hat)
                                {
                                    if (hatsSoFar == hatsDrawn)
                                    {
                                        h.drawInMenu(spriteBatch, Game1.GlobalToLocal(fish.GetWorldPosition() + new Vector2(-30 + (fish.facingLeft ? (-4) : 0), -55f)), 0.75f, 1f, fish_layer + 1E-08f, StackDrawType.Hide);
                                        hatsDrawn++;
                                        break;
                                    }
                                    hatsSoFar++;
                                }
                            }
                        }
                        for (int j = 0; j < __instance.floorDecorations.Count; j++)
                        {
                            if (__instance.floorDecorations[j].HasValue)
                            {
                                KeyValuePair<Rectangle, Vector2> decoration = __instance.floorDecorations[j].Value;
                                Vector2 decoration_position = decoration.Value;
                                Rectangle decoration_source_rect = decoration.Key;
                                float decoration_layer = Utility.Lerp(__instance.GetFishSortRegion().Y, __instance.GetFishSortRegion().X, decoration_position.Y / 20f) - 1E-06f;
                                spriteBatch.Draw(__instance.GetAquariumTexture(), Game1.GlobalToLocal(new Vector2((float)__instance.GetTankBounds().Left + decoration_position.X * 4f, (float)(__instance.GetTankBounds().Bottom - 4) - decoration_position.Y * 4f)), decoration_source_rect, Color.White * alpha, 0f, new Vector2(decoration_source_rect.Width / 2, decoration_source_rect.Height - 4), 4f, SpriteEffects.None, decoration_layer);
                            }
                        }
                        foreach (Vector4 bubble in __instance.bubbles)
                        {
                            float layer = Utility.Lerp(__instance.GetFishSortRegion().Y, __instance.GetFishSortRegion().X, bubble.Z / 20f) - 1E-06f;
                            spriteBatch.Draw(__instance.GetAquariumTexture(), Game1.GlobalToLocal(new Vector2((float)__instance.GetTankBounds().Left + bubble.X, (float)(__instance.GetTankBounds().Bottom - 4) - bubble.Y - bubble.Z * 4f)), new Rectangle(0, 240, 16, 16), Color.White * alpha, 0f, new Vector2(8f, 8f), 4f * bubble.W, SpriteEffects.None, layer);
                        }
                    }
                    FurniturePatch.DrawPrefix(__instance, ___sourceIndexOffset, ___drawPosition, spriteBatch, x, y, alpha);
                }

                return false;
            }
            return true;
        }
    }
}
