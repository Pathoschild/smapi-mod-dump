/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/AlternativeTextures
**
*************************************************/

using AlternativeTextures.Framework.Models;
using AlternativeTextures.Framework.Utilities;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using System;

namespace AlternativeTextures.Framework.Patches.StandardObjects
{
    internal class FencePatch : PatchTemplate
    {
        private readonly Type _object = typeof(Fence);
        private const int VANILLA_FENCE_TEXTURE_WIDTH = 48;

        internal FencePatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_object, nameof(Fence.draw), new[] { typeof(SpriteBatch), typeof(int), typeof(int), typeof(float) }), prefix: new HarmonyMethod(GetType(), nameof(DrawPrefix)));
            harmony.Patch(AccessTools.Method(_object, nameof(Fence.performObjectDropInAction), new[] { typeof(Item), typeof(bool), typeof(Farmer) }), postfix: new HarmonyMethod(GetType(), nameof(PerformObjectDropInActionPostfix)));

            if (PatchTemplate.IsDGAUsed())
            {
                try
                {
                    if (Type.GetType("DynamicGameAssets.Game.CustomFence, DynamicGameAssets") is Type dgaFenceType && dgaFenceType != null)
                    {
                        // DGA doesn't use either of these methods for CustomFence
                        //harmony.Patch(AccessTools.Method(dgaFenceType, nameof(Fence.draw), new[] { typeof(SpriteBatch), typeof(int), typeof(int), typeof(float) }), prefix: new HarmonyMethod(GetType(), nameof(DrawPrefix)));
                        //harmony.Patch(AccessTools.Method(dgaFenceType, nameof(Fence.performObjectDropInAction), new[] { typeof(Item), typeof(bool), typeof(Farmer) }), postfix: new HarmonyMethod(GetType(), nameof(PerformObjectDropInActionPostfix)));
                    }
                }
                catch (Exception ex)
                {
                    _monitor.Log($"Failed to patch Dynamic Game Assets in {this.GetType().Name}: AT may not be able to override certain DGA object types!", LogLevel.Warn);
                    _monitor.Log($"Patch for DGA failed in {this.GetType().Name}: {ex}", LogLevel.Trace);
                }
            }
        }

        private static bool DrawPrefix(Fence __instance, SpriteBatch b, int x, int y, float alpha = 1f)
        {
            if (__instance.modData.ContainsKey(ModDataKeys.ALTERNATIVE_TEXTURE_NAME))
            {
                var textureModel = AlternativeTextures.textureManager.GetSpecificTextureModel(__instance.modData[ModDataKeys.ALTERNATIVE_TEXTURE_NAME]);
                if (textureModel is null)
                {
                    return true;
                }

                var textureVariation = Int32.Parse(__instance.modData[ModDataKeys.ALTERNATIVE_TEXTURE_VARIATION]);
                if (textureVariation == -1 || AlternativeTextures.modConfig.IsTextureVariationDisabled(textureModel.GetId(), textureVariation))
                {
                    return true;
                }
                int sourceRectPosition = 1;
                int drawSum = __instance.getDrawSum(Game1.currentLocation);
                if ((float)__instance.health > 1f || __instance.repairQueued.Value)
                {
                    sourceRectPosition = Fence.fenceDrawGuide[drawSum];
                }

                var gateOffset = __instance.isGate ? 128 : 0;
                var textureOffset = textureModel.GetTextureOffset(textureVariation);
                if ((bool)__instance.isGate)
                {
                    Vector2 offset = new Vector2(0f, 0f);
                    switch (drawSum)
                    {
                        case 10:
                            b.Draw(textureModel.GetTexture(textureVariation), Game1.GlobalToLocal(Game1.viewport, offset + new Vector2(x * 64 - 16, y * 64 - 128)), new Rectangle(((int)__instance.gatePosition == 88) ? 24 : 0, textureOffset + (192 - gateOffset), 24, 48), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, (float)(y * 64 + 32 + 1) / 10000f);
                            return false;
                        case 100:
                            b.Draw(textureModel.GetTexture(textureVariation), Game1.GlobalToLocal(Game1.viewport, offset + new Vector2(x * 64 - 16, y * 64 - 128)), new Rectangle(((int)__instance.gatePosition == 88) ? 24 : 0, textureOffset + (240 - gateOffset), 24, 48), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, (float)(y * 64 + 32 + 1) / 10000f);
                            return false;
                        case 1000:
                            b.Draw(textureModel.GetTexture(textureVariation), Game1.GlobalToLocal(Game1.viewport, offset + new Vector2(x * 64 + 20, y * 64 - 64 - 20)), new Rectangle(((int)__instance.gatePosition == 88) ? 24 : 0, textureOffset + (288 - gateOffset), 24, 32), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, (float)(y * 64 - 32 + 2) / 10000f);
                            return false;
                        case 500:
                            b.Draw(textureModel.GetTexture(textureVariation), Game1.GlobalToLocal(Game1.viewport, offset + new Vector2(x * 64 + 20, y * 64 - 64 - 20)), new Rectangle(((int)__instance.gatePosition == 88) ? 24 : 0, textureOffset + (320 - gateOffset), 24, 32), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, (float)(y * 64 + 96 - 1) / 10000f);
                            return false;
                        case 110:
                            b.Draw(textureModel.GetTexture(textureVariation), Game1.GlobalToLocal(Game1.viewport, offset + new Vector2(x * 64 - 16, y * 64 - 64)), new Rectangle(((int)__instance.gatePosition == 88) ? 24 : 0, textureOffset + (128 - gateOffset), 24, 32), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, (float)(y * 64 + 32 + 1) / 10000f);
                            return false;
                        case 1500:
                            b.Draw(textureModel.GetTexture(textureVariation), Game1.GlobalToLocal(Game1.viewport, offset + new Vector2(x * 64 + 20, y * 64 - 64 - 20)), new Rectangle(((int)__instance.gatePosition == 88) ? 16 : 0, textureOffset + (160 - gateOffset), 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, (float)(y * 64 - 32 + 2) / 10000f);
                            b.Draw(textureModel.GetTexture(textureVariation), Game1.GlobalToLocal(Game1.viewport, offset + new Vector2(x * 64 + 20, y * 64 - 64 + 44)), new Rectangle(((int)__instance.gatePosition == 88) ? 16 : 0, textureOffset + (176 - gateOffset), 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, (float)(y * 64 + 96 - 1) / 10000f);
                            return false;
                    }
                    sourceRectPosition = 5;
                }
                else if (__instance.heldObject.Value != null)
                {
                    Vector2 offset2 = Vector2.Zero;
                    switch (drawSum)
                    {
                        case 10:
                            if (__instance.whichType.Value == 2)
                            {
                                offset2.X = -4f;
                            }
                            else if (__instance.whichType.Value == 3)
                            {
                                offset2.X = 8f;
                            }
                            else
                            {
                                offset2.X = 0f;
                            }
                            break;
                        case 100:
                            if (__instance.whichType.Value == 2)
                            {
                                offset2.X = 0f;
                            }
                            else if (__instance.whichType.Value == 3)
                            {
                                offset2.X = -8f;
                            }
                            else
                            {
                                offset2.X = -4f;
                            }
                            break;
                    }
                    if ((int)__instance.whichType == 2)
                    {
                        offset2.Y = 16f;
                    }
                    else if ((int)__instance.whichType == 3)
                    {
                        offset2.Y -= 8f;
                    }
                    if ((int)__instance.whichType == 3)
                    {
                        offset2.X -= 2f;
                    }

                    __instance.heldObject.Value.draw(b, x * 64 + (int)offset2.X, (y - 1) * 64 - 16 + (int)offset2.Y, (float)(y * 64 + 64) / 10000f, 1f);
                }

                b.Draw(textureModel.GetTexture(textureVariation), Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64, y * 64 - 64)), new Rectangle((sourceRectPosition * Fence.fencePieceWidth % VANILLA_FENCE_TEXTURE_WIDTH), textureOffset + (sourceRectPosition * Fence.fencePieceWidth / VANILLA_FENCE_TEXTURE_WIDTH * Fence.fencePieceHeight), Fence.fencePieceWidth, Fence.fencePieceHeight), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, (float)(y * 64 + 32) / 10000f);

                return false;
            }
            return true;
        }

        private static void PerformObjectDropInActionPostfix(Fence __instance, bool __result, Item dropIn, bool probe, Farmer who)
        {
            // Assign Gate modData to this fence (if applicable)
            if (dropIn.parentSheetIndex == 325 && __result)
            {
                var instanceName = $"{AlternativeTextureModel.TextureType.Craftable}_{Game1.objectInformation[dropIn.parentSheetIndex].Split('/')[0]}";
                var instanceSeasonName = $"{instanceName}_{Game1.currentSeason}";

                if (AlternativeTextures.textureManager.DoesObjectHaveAlternativeTexture(instanceName) && AlternativeTextures.textureManager.DoesObjectHaveAlternativeTexture(instanceSeasonName))
                {
                    var result = Game1.random.Next(2) > 0 ? AssignModData(__instance, instanceSeasonName, true, __instance.bigCraftable) : AssignModData(__instance, instanceName, false, __instance.bigCraftable);
                    return;
                }
                else
                {
                    if (AlternativeTextures.textureManager.DoesObjectHaveAlternativeTexture(instanceName))
                    {
                        AssignModData(__instance, instanceName, false, __instance.bigCraftable);
                        return;
                    }

                    if (AlternativeTextures.textureManager.DoesObjectHaveAlternativeTexture(instanceSeasonName))
                    {
                        AssignModData(__instance, instanceSeasonName, true, __instance.bigCraftable);
                        return;
                    }
                }

                AssignDefaultModData(__instance, instanceSeasonName, true, __instance.bigCraftable);
            }
        }
    }
}
