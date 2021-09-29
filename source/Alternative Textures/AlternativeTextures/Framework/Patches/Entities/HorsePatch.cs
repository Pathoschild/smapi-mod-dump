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
using StardewValley.Characters;
using StardewValley.Locations;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Object = StardewValley.Object;

namespace AlternativeTextures.Framework.Patches.Entities
{
    internal class HorsePatch : PatchTemplate
    {
        private readonly Type _entity = typeof(Horse);

        internal HorsePatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_entity, nameof(Horse.draw), new[] { typeof(SpriteBatch) }), prefix: new HarmonyMethod(GetType(), nameof(DrawPrefix)));
            harmony.Patch(AccessTools.Constructor(_entity, new[] { typeof(Guid), typeof(int), typeof(int) }), postfix: new HarmonyMethod(GetType(), nameof(HorsePostfix)));
        }

        private static bool DrawPrefix(Horse __instance, SpriteBatch b, NetRef<Hat> ___hat, int ___shakeTimer)
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

                __instance.flip = __instance.FacingDirection == 3;
                __instance.Sprite.UpdateSourceRect();

                __instance.Sprite.spriteTexture = textureModel.GetTexture(textureVariation);
                __instance.Sprite.sourceRect.Y = textureOffset + (__instance.Sprite.currentFrame * __instance.Sprite.SpriteWidth / __instance.Sprite.Texture.Width * __instance.Sprite.SpriteHeight);
                CharacterPatch.DrawReversePatch(__instance, b);

                if (__instance.FacingDirection == 2 && __instance.rider != null)
                {
                    b.Draw(__instance.Sprite.Texture, __instance.getLocalPosition(Game1.viewport) + new Vector2(48f, -24f - __instance.rider.yOffset), new Rectangle(160, 96 + textureOffset, 9, 15), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, (__instance.Position.Y + 64f) / 10000f);
                }
                bool draw_hat = true;
                if (___hat.Value == null)
                {
                    return false;
                }
                Vector2 hatOffset = Vector2.Zero;
                switch ((int)___hat.Value.which)
                {
                    case 14:
                        if ((int)__instance.facingDirection == 0)
                        {
                            hatOffset.X = -100f;
                        }
                        break;
                    case 6:
                        hatOffset.Y += 2f;
                        if (__instance.FacingDirection == 2)
                        {
                            hatOffset.Y -= 1f;
                        }
                        break;
                    case 10:
                        hatOffset.Y += 3f;
                        if ((int)__instance.facingDirection == 0)
                        {
                            draw_hat = false;
                        }
                        break;
                    case 9:
                    case 32:
                        if (__instance.FacingDirection == 0 || __instance.FacingDirection == 2)
                        {
                            hatOffset.Y += 1f;
                        }
                        break;
                    case 31:
                        hatOffset.Y += 1f;
                        break;
                    case 11:
                    case 39:
                        if (__instance.FacingDirection == 3 || __instance.FacingDirection == 1)
                        {
                            if (__instance.flip)
                            {
                                hatOffset.X += 2f;
                            }
                            else
                            {
                                hatOffset.X -= 2f;
                            }
                        }
                        break;
                    case 26:
                        if (__instance.FacingDirection == 3 || __instance.FacingDirection == 1)
                        {
                            if (__instance.flip)
                            {
                                hatOffset.X += 1f;
                            }
                            else
                            {
                                hatOffset.X -= 1f;
                            }
                        }
                        break;
                    case 56:
                    case 67:
                        if (__instance.FacingDirection == 0)
                        {
                            draw_hat = false;
                        }
                        break;
                }
                hatOffset *= 4f;
                if (___shakeTimer > 0)
                {
                    hatOffset += new Vector2(Game1.random.Next(-1, 2), Game1.random.Next(-1, 2));
                }
                if (hatOffset.X <= -100f)
                {
                    return false;
                }
                float horse_draw_layer = (float)__instance.GetBoundingBox().Center.Y / 10000f;
                if (__instance.rider != null)
                {
                    horse_draw_layer = ((__instance.FacingDirection == 0) ? ((__instance.position.Y + 64f - 32f) / 10000f) : ((__instance.FacingDirection != 2) ? ((__instance.position.Y + 64f - 1f) / 10000f) : ((__instance.position.Y + 64f + (float)((__instance.rider != null) ? 1 : 1)) / 10000f)));
                }
                if (!draw_hat)
                {
                    return false;
                }
                horse_draw_layer += 1E-07f;
                switch (__instance.Sprite.CurrentFrame)
                {
                    case 0:
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                        ___hat.Value.draw(b, Utility.snapDrawPosition(__instance.getLocalPosition(Game1.viewport) + hatOffset + new Vector2(30f, -42f - ((__instance.rider != null) ? __instance.rider.yOffset : 0f))), 1.33333337f, 1f, horse_draw_layer, 2);
                        break;
                    case 7:
                    case 11:
                        if (__instance.flip)
                        {
                            ___hat.Value.draw(b, __instance.getLocalPosition(Game1.viewport) + hatOffset + new Vector2(-14f, -74f), 1.33333337f, 1f, horse_draw_layer, 3);
                        }
                        else
                        {
                            ___hat.Value.draw(b, Utility.snapDrawPosition(__instance.getLocalPosition(Game1.viewport) + hatOffset + new Vector2(66f, -74f)), 1.33333337f, 1f, horse_draw_layer, 1);
                        }
                        break;
                    case 8:
                        if (__instance.flip)
                        {
                            ___hat.Value.draw(b, __instance.getLocalPosition(Game1.viewport) + hatOffset + new Vector2(-18f, -74f), 1.33333337f, 1f, horse_draw_layer, 3);
                        }
                        else
                        {
                            ___hat.Value.draw(b, Utility.snapDrawPosition(__instance.getLocalPosition(Game1.viewport) + hatOffset + new Vector2(70f, -74f)), 1.33333337f, 1f, horse_draw_layer, 1);
                        }
                        break;
                    case 9:
                        if (__instance.flip)
                        {
                            ___hat.Value.draw(b, __instance.getLocalPosition(Game1.viewport) + hatOffset + new Vector2(-18f, -70f), 1.33333337f, 1f, horse_draw_layer, 3);
                        }
                        else
                        {
                            ___hat.Value.draw(b, Utility.snapDrawPosition(__instance.getLocalPosition(Game1.viewport) + hatOffset + new Vector2(70f, -70f)), 1.33333337f, 1f, horse_draw_layer, 1);
                        }
                        break;
                    case 10:
                        if (__instance.flip)
                        {
                            ___hat.Value.draw(b, __instance.getLocalPosition(Game1.viewport) + hatOffset + new Vector2(-14f, -70f), 1.33333337f, 1f, horse_draw_layer, 3);
                        }
                        else
                        {
                            ___hat.Value.draw(b, Utility.snapDrawPosition(__instance.getLocalPosition(Game1.viewport) + hatOffset + new Vector2(66f, -70f)), 1.33333337f, 1f, horse_draw_layer, 1);
                        }
                        break;
                    case 12:
                        if (__instance.flip)
                        {
                            ___hat.Value.draw(b, Utility.snapDrawPosition(__instance.getLocalPosition(Game1.viewport) + hatOffset + new Vector2(-14f, -78f)), 1.33333337f, 1f, horse_draw_layer, 3);
                        }
                        else
                        {
                            ___hat.Value.draw(b, Utility.snapDrawPosition(__instance.getLocalPosition(Game1.viewport) + hatOffset + new Vector2(66f, -78f)), 1.33333337f, 1f, horse_draw_layer, 1);
                        }
                        break;
                    case 13:
                        if (__instance.flip)
                        {
                            ___hat.Value.draw(b, Utility.snapDrawPosition(__instance.getLocalPosition(Game1.viewport) + hatOffset + new Vector2(-18f, -78f)), 1.33333337f, 1f, horse_draw_layer, 3);
                        }
                        else
                        {
                            ___hat.Value.draw(b, Utility.snapDrawPosition(__instance.getLocalPosition(Game1.viewport) + hatOffset + new Vector2(70f, -78f)), 1.33333337f, 1f, horse_draw_layer, 1);
                        }
                        break;
                    case 21:
                        if (__instance.flip)
                        {
                            ___hat.Value.draw(b, Utility.snapDrawPosition(__instance.getLocalPosition(Game1.viewport) + hatOffset + new Vector2(-14f, -66f)), 1.33333337f, 1f, horse_draw_layer, 3);
                        }
                        else
                        {
                            ___hat.Value.draw(b, Utility.snapDrawPosition(__instance.getLocalPosition(Game1.viewport) + hatOffset + new Vector2(66f, -66f)), 1.33333337f, 1f, horse_draw_layer, 1);
                        }
                        break;
                    case 22:
                        if (__instance.flip)
                        {
                            ___hat.Value.draw(b, Utility.snapDrawPosition(__instance.getLocalPosition(Game1.viewport) + hatOffset + new Vector2(-18f, -54f)), 1.33333337f, 1f, horse_draw_layer, 3);
                        }
                        else
                        {
                            ___hat.Value.draw(b, Utility.snapDrawPosition(__instance.getLocalPosition(Game1.viewport) + hatOffset + new Vector2(70f, -54f)), 1.33333337f, 1f, horse_draw_layer, 1);
                        }
                        break;
                    case 23:
                        if (__instance.flip)
                        {
                            ___hat.Value.draw(b, Utility.snapDrawPosition(__instance.getLocalPosition(Game1.viewport) + hatOffset + new Vector2(-18f, -42f)), 1.33333337f, 1f, horse_draw_layer, 3);
                        }
                        else
                        {
                            ___hat.Value.draw(b, Utility.snapDrawPosition(__instance.getLocalPosition(Game1.viewport) + hatOffset + new Vector2(70f, -42f)), 1.33333337f, 1f, horse_draw_layer, 1);
                        }
                        break;
                    case 24:
                        if (__instance.flip)
                        {
                            ___hat.Value.draw(b, Utility.snapDrawPosition(__instance.getLocalPosition(Game1.viewport) + hatOffset + new Vector2(-18f, -42f)), 1.33333337f, 1f, horse_draw_layer, 3);
                        }
                        else
                        {
                            ___hat.Value.draw(b, Utility.snapDrawPosition(__instance.getLocalPosition(Game1.viewport) + hatOffset + new Vector2(70f, -42f)), 1.33333337f, 1f, horse_draw_layer, 1);
                        }
                        break;
                    case 14:
                    case 15:
                    case 16:
                    case 17:
                    case 18:
                    case 19:
                    case 20:
                    case 25:
                        ___hat.Value.draw(b, __instance.getLocalPosition(Game1.viewport) + hatOffset + new Vector2(28f, -106f - ((__instance.rider != null) ? __instance.rider.yOffset : 0f)), 1.33333337f, 1f, horse_draw_layer, 0);
                        break;
                }

                return false;
            }

            return true;
        }

        private static void HorsePostfix(Horse __instance, Guid horseId, int xTile, int yTile)
        {
            var instanceName = $"{AlternativeTextureModel.TextureType.Character}_{GetCharacterName(__instance)}";
            var instanceSeasonName = $"{instanceName}_{Game1.GetSeasonForLocation(__instance.currentLocation)}";

            if (AlternativeTextures.textureManager.DoesObjectHaveAlternativeTexture(instanceName) && AlternativeTextures.textureManager.DoesObjectHaveAlternativeTexture(instanceSeasonName))
            {
                var result = Game1.random.Next(2) > 0 ? AssignModData(__instance, instanceSeasonName, true) : AssignModData(__instance, instanceName, false);
                return;
            }
            else
            {
                if (AlternativeTextures.textureManager.DoesObjectHaveAlternativeTexture(instanceName))
                {
                    AssignModData(__instance, instanceName, false);
                    return;
                }

                if (AlternativeTextures.textureManager.DoesObjectHaveAlternativeTexture(instanceSeasonName))
                {
                    AssignModData(__instance, instanceSeasonName, true);
                    return;
                }
            }

            AssignDefaultModData(__instance, instanceSeasonName, true);
        }
    }
}
