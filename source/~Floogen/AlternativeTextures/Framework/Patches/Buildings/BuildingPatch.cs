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
using AlternativeTextures.Framework.Utilities.Extensions;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.GameData.Buildings;
using StardewValley.GameData.HomeRenovations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AlternativeTextures.Framework.Patches.Buildings
{
    internal class BuildingPatch : PatchTemplate
    {
        private readonly Type _entity = typeof(Building);
        private const int TRACTOR_GARAGE_ID = -794739;

        internal BuildingPatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_entity, nameof(Building.Update), new[] { typeof(GameTime) }), postfix: new HarmonyMethod(GetType(), nameof(UpdatePostfix)));
            harmony.Patch(AccessTools.Method(_entity, nameof(Building.resetTexture), null), prefix: new HarmonyMethod(GetType(), nameof(ResetTexturePrefix)));
            harmony.Patch(AccessTools.Method(_entity, nameof(Building.getSourceRect), null), postfix: new HarmonyMethod(GetType(), nameof(GetSourceRectPostfix)));
            harmony.Patch(AccessTools.Method(_entity, nameof(Building.draw), new[] { typeof(SpriteBatch) }), prefix: new HarmonyMethod(GetType(), nameof(DrawPrefix)));

            harmony.Patch(AccessTools.Constructor(_entity, new[] { typeof(string), typeof(Vector2) }), postfix: new HarmonyMethod(GetType(), nameof(BuildingPostfix)));

            harmony.CreateReversePatcher(AccessTools.Method(_entity, nameof(Building.resetTexture), null), new HarmonyMethod(GetType(), nameof(ResetTextureReversePatch))).Patch();
            harmony.CreateReversePatcher(AccessTools.Method(_entity, nameof(Building.getSourceRect), null), new HarmonyMethod(GetType(), nameof(GetSourceRectReversePatch))).Patch();
        }

        private static void UpdatePostfix(Building __instance, GameTime time)
        {
            if (!__instance.modData.ContainsKey(ModDataKeys.ALTERNATIVE_TEXTURE_NAME) || AlternativeTextures.textureManager.GetSpecificTextureModel(__instance.modData[ModDataKeys.ALTERNATIVE_TEXTURE_NAME]) is null)
            {
                return;
            }

            var instanceName = String.Concat(__instance.modData[ModDataKeys.ALTERNATIVE_TEXTURE_OWNER], ".", $"{AlternativeTextureModel.TextureType.Building}_{GetBuildingName(__instance)}");
            var instanceSeasonName = $"{instanceName}_{Game1.currentSeason}";

            if (!String.Equals(__instance.modData[ModDataKeys.ALTERNATIVE_TEXTURE_NAME], instanceName, StringComparison.OrdinalIgnoreCase) && !String.Equals(__instance.modData[ModDataKeys.ALTERNATIVE_TEXTURE_NAME], instanceSeasonName, StringComparison.OrdinalIgnoreCase))
            {
                __instance.modData[ModDataKeys.ALTERNATIVE_TEXTURE_NAME] = String.Concat(__instance.modData[ModDataKeys.ALTERNATIVE_TEXTURE_OWNER], ".", $"{AlternativeTextureModel.TextureType.Building}_{GetBuildingName(__instance)}");
                if (__instance.modData.ContainsKey(ModDataKeys.ALTERNATIVE_TEXTURE_SEASON) && !String.IsNullOrEmpty(__instance.modData[ModDataKeys.ALTERNATIVE_TEXTURE_SEASON]))
                {
                    __instance.modData[ModDataKeys.ALTERNATIVE_TEXTURE_SEASON] = Game1.currentSeason;
                    __instance.modData[ModDataKeys.ALTERNATIVE_TEXTURE_NAME] = String.Concat(__instance.modData[ModDataKeys.ALTERNATIVE_TEXTURE_NAME], "_", __instance.modData[ModDataKeys.ALTERNATIVE_TEXTURE_SEASON]);

                    __instance.resetTexture();
                }
            }
        }

        internal static void CondensedDrawInMenu(Building building, Texture2D texture, SpriteBatch b, int x, int y, float scale, float alpha = 1f)
        {
            switch (building)
            {
                case FishPond fishPond:
                    y += 32;
                    //building.drawShadow(b, x, y);
                    b.Draw(texture, new Vector2(x, y), new Rectangle(0, 80, 80, 80), new Color(60, 126, 150) * alpha, 0f, new Vector2(0f, 0f), scale, SpriteEffects.None, 1f);
                    for (int yWater = building.tileY; yWater < (int)building.tileY + 5; yWater++)
                    {
                        for (int xWater = building.tileX; xWater < (int)building.tileX + 4; xWater++)
                        {
                            bool num = yWater == (int)building.tileY + 4;
                            bool topY = yWater == (int)building.tileY;
                            if (num)
                            {
                                b.Draw(Game1.mouseCursors, new Vector2(x + xWater * 64 + 32, y + (yWater + 1) * 64 - (int)Game1.currentLocation.waterPosition - 32), new Rectangle(Game1.currentLocation.waterAnimationIndex * 64, 2064 + (((xWater + yWater) % 2 != 0) ? ((!Game1.currentLocation.waterTileFlip) ? 128 : 0) : (Game1.currentLocation.waterTileFlip ? 128 : 0)), 64, 32 + (int)Game1.currentLocation.waterPosition - 5), Game1.currentLocation.waterColor.Value, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
                            }
                            else
                            {
                                b.Draw(Game1.mouseCursors, new Vector2(x + xWater * 64 + 32, y + yWater * 64 + 32 - (int)((!topY) ? Game1.currentLocation.waterPosition : 0f)), new Rectangle(Game1.currentLocation.waterAnimationIndex * 64, 2064 + (((xWater + yWater) % 2 != 0) ? ((!Game1.currentLocation.waterTileFlip) ? 128 : 0) : (Game1.currentLocation.waterTileFlip ? 128 : 0)) + (topY ? ((int)Game1.currentLocation.waterPosition) : 0), 64, 64 + (topY ? ((int)(0f - Game1.currentLocation.waterPosition)) : 0)), Game1.currentLocation.waterColor.Value, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
                            }
                        }
                    }
                    b.Draw(texture, new Vector2(x, y), new Rectangle(0, 0, 80, 80), Color.White * alpha, 0f, new Vector2(0f, 0f), scale, SpriteEffects.None, 1f);
                    b.Draw(texture, new Vector2(x + 32, y + 24 + ((Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 2500.0 < 1250.0) ? 4 : 0)), new Rectangle(16, 160, 48, 7), Color.White * alpha, 0f, Vector2.Zero, scale, SpriteEffects.None, 1f);
                    b.Draw(texture, new Vector2(x, y - 64), new Rectangle(80, fishPond.nettingStyle.Value * 48, 80, 48), Color.White * alpha, 0f, new Vector2(0f, 0f), scale, SpriteEffects.None, 1f);
                    return;
                case JunimoHut junimoHut:
                    //building.drawShadow(b, x, y);
                    b.Draw(texture, new Vector2(x, y), junimoHut.getSourceRect(), Color.White, 0f, new Vector2(0f, 0f), scale, SpriteEffects.None, 0.89f);
                    return;
                case ShippingBin shippingBin:
                default:
                    //building.drawShadow(b, x, y);
                    //b.Draw(texture, new Vector2(x, y), building.getSourceRect(), Color.White, 0f, new Vector2(0f, 0f), scale, SpriteEffects.None, 0.89f);

                    BuildingData data = building.GetData();
                    if (data != null)
                    {
                        x += (int)(data.DrawOffset.X * 4f);
                        y += (int)(data.DrawOffset.Y * 4f);
                    }
                    float baseSortY = (int)building.tilesHigh * 64;
                    float sortY = baseSortY;
                    if (data != null)
                    {
                        sortY -= data.SortTileOffset * 64f;
                    }
                    sortY /= 10000f;
                    if (building.ShouldDrawShadow(data))
                    {
                        //building.drawShadow(b, x, y);
                    }
                    Rectangle mainSourceRect = GetSourceRectReversePatch(building);
                    b.Draw(texture, new Vector2(x, y), mainSourceRect, building.color, 0f, new Vector2(0f, 0f), scale, SpriteEffects.None, sortY);
                    if (data?.DrawLayers == null)
                    {
                        return;
                    }
                    foreach (BuildingDrawLayer drawLayer in data.DrawLayers)
                    {
                        if (drawLayer.OnlyDrawIfChestHasContents == null)
                        {
                            sortY = baseSortY - drawLayer.SortTileOffset * 64f;
                            sortY += 1f;
                            if (drawLayer.DrawInBackground)
                            {
                                sortY = 0f;
                            }
                            sortY /= 10000f;
                            Rectangle sourceRect = drawLayer.GetSourceRect((int)Game1.currentGameTime.TotalGameTime.TotalMilliseconds);
                            sourceRect = building.ApplySourceRectOffsets(sourceRect);
                            Texture2D layerTexture = texture;
                            if (drawLayer.Texture != null)
                            {
                                layerTexture = Game1.content.Load<Texture2D>(drawLayer.Texture);
                            }
                            b.Draw(layerTexture, new Vector2(x, y) + drawLayer.DrawPosition * scale, sourceRect, Color.White, 0f, new Vector2(0f, 0f), scale, SpriteEffects.None, sortY);
                        }
                    }
                    return;
            }
        }

        internal static bool ResetTexturePrefix(Building __instance)
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

                __instance.texture = new Lazy<Texture2D>(delegate
                {
                    return GetBuildingTextureWithPaint(__instance, textureModel, textureVariation);
                });
                return false;
            }

            return true;
        }

        internal static void ForceResetTexture(Building __instance, string textureName, string variation)
        {
            var textureModel = AlternativeTextures.textureManager.GetSpecificTextureModel(textureName);
            if (textureModel is null)
            {
                return;
            }

            var textureVariation = Int32.Parse(variation);
            if (textureVariation == -1 || AlternativeTextures.modConfig.IsTextureVariationDisabled(textureModel.GetId(), textureVariation))
            {
                return;
            }

            __instance.texture = new Lazy<Texture2D>(delegate
            {
                return GetBuildingTextureWithPaint(__instance, textureModel, textureVariation);
            });
        }

        private static void GetSourceRectPostfix(Building __instance, ref Rectangle __result)
        {
            if (__instance.modData.ContainsKey(ModDataKeys.ALTERNATIVE_TEXTURE_NAME) is false)
            {
                return;
            }

            var textureModel = AlternativeTextures.textureManager.GetSpecificTextureModel(__instance.modData[ModDataKeys.ALTERNATIVE_TEXTURE_NAME]);
            if (textureModel is null)
            {
                return;
            }

            var textureVariation = Int32.Parse(__instance.modData[ModDataKeys.ALTERNATIVE_TEXTURE_VARIATION]);
            if (textureVariation == -1 || AlternativeTextures.modConfig.IsTextureVariationDisabled(textureModel.GetId(), textureVariation))
            {
                return;
            }

            var buildingData = __instance.GetData();
            var xOffset = buildingData is null ? 0 : buildingData.SourceRect.X;
            var yOffset = textureModel.GetTextureOffset(textureVariation) + (buildingData is null ? 0 : buildingData.SourceRect.Y);

            // Handle Greenhouse logic
            if (__instance.buildingType.Value == "Greenhouse")
            {
                Farm farm = __instance.GetParentLocation() as Farm;
                if (farm is not null && farm.greenhouseUnlocked.Value is false)
                {
                    yOffset -= buildingData.SourceRect.Height;
                }
            }

            __result = new Rectangle(xOffset, yOffset, __result.Width, __result.Height);
        }

        internal static bool DrawPrefix(Building __instance, float ___alpha, SpriteBatch b)
        {
            if (__instance.modData.ContainsKey(ModDataKeys.ALTERNATIVE_TEXTURE_NAME) is false)
            {
                return true;
            }

            if (__instance is Stable || __instance.maxOccupants == TRACTOR_GARAGE_ID)
            {
                if (__instance.isMoving || __instance.daysOfConstructionLeft > 0)
                {
                    return true;
                }

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

                var paintedTexture = BuildingPatch.GetBuildingTextureWithPaint(__instance, textureModel, textureVariation);

                __instance.drawShadow(b);
                b.Draw(paintedTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2((int)__instance.tileX * 64, (int)__instance.tileY * 64 + (int)__instance.tilesHigh * 64)), paintedTexture.Bounds, __instance.color * ___alpha, 0f, new Vector2(0f, __instance.texture.Value.Bounds.Height), 4f, SpriteEffects.None, (float)(((int)__instance.tileY + (int)__instance.tilesHigh - 1) * 64) / 10000f);

                return false;
            }
            else if (__instance.buildingType.Value == "Farmhouse" && __instance.GetParentLocation().modData.ContainsKey("AlternativeTextureName.Mailbox"))
            {
                BuildingData data = __instance.GetData();
                if (data is null)
                {
                    return true;
                }

                // Set the default texture, if the existing one is a Alternative Textures token
                var drawLayer = data.DrawLayers.FirstOrDefault(l => l.Id == "Default_Mailbox");
                if (drawLayer is null)
                {
                    return true;
                }
                else if (AlternativeTextures.textureManager.GetModelByToken(drawLayer.Texture) is not null)
                {
                    drawLayer.Texture = "Buildings\\Mailbox";
                }

                // Handle mailbox
                var textureModel = AlternativeTextures.textureManager.GetSpecificTextureModel(Game1.currentLocation.modData["AlternativeTextureName.Mailbox"]);
                if (textureModel is null || Game1.currentLocation.modData.TryGetValue("AlternativeTextureVariation.Mailbox", out string rawVariationIndex) is false)
                {
                    return true;
                }
                var textureVariation = Int32.Parse(rawVariationIndex);
                if (textureVariation == -1 || AlternativeTextures.modConfig.IsTextureVariationDisabled(textureModel.GetId(), textureVariation))
                {
                    return true;
                }

                // Set the layer to use the AT token for the texture
                drawLayer.Texture = $"{AlternativeTextures.TEXTURE_TOKEN_HEADER}{textureModel.GetTokenId(textureVariation)}";

                return true;
            }

            return true;
        }

        internal static Texture2D GetBuildingTextureWithPaint(Building building, AlternativeTextureModel textureModel, int textureVariation, bool canBePaintedOverride = false)
        {
            var yOffset = textureModel.GetTextureOffset(textureVariation);

            var baseTexture = textureModel.GetTexture(textureVariation);

            // Handle instances where required paint masks are missing but textureModel.IgnoreBuildingColorMask is false
            bool canReallyBePainted = (building.CanBePainted() || canBePaintedOverride) && textureModel.IgnoreBuildingColorMask is false;
            var originalTexture = AlternativeTextures.modHelper.GameContent.Load<Texture2D>(building.textureName());
            if (originalTexture is not null)
            {
                if (canReallyBePainted && baseTexture.Width <= originalTexture.Width)
                {
                    canReallyBePainted = false;
                }
            }

            if (building.paintedTexture != null)
            {
                building.paintedTexture = null;
            }

            var textureWidth = canReallyBePainted ? baseTexture.Width / 2 : building is ShippingBin ? textureModel.TextureWidth : baseTexture.Width;

            var texture2D = baseTexture.CreateSelectiveCopy(Game1.graphics.GraphicsDevice, new Rectangle(0, yOffset, textureWidth, baseTexture.Height));
            if (canReallyBePainted)
            {
                var paintedTexture2D = baseTexture.CreateSelectiveCopy(Game1.graphics.GraphicsDevice, new Rectangle(textureWidth, yOffset, textureWidth, baseTexture.Height));
                building.paintedTexture = GetPaintedOverlay(building, texture2D, paintedTexture2D, building.netBuildingPaintColor.Value);
                if (building.paintedTexture != null)
                {
                    texture2D = building.paintedTexture;
                }
            }

            return texture2D;
        }

        internal static Texture2D GetPaintedOverlay(Building building, Texture2D base_texture, Texture2D paint_mask_texture, BuildingPaintColor color)
        {
            List<List<int>> paint_indices = null;
            try
            {
                Color[] mask_pixels = new Color[paint_mask_texture.Width * paint_mask_texture.Height];
                paint_mask_texture.GetData(mask_pixels);
                paint_indices = new List<List<int>>();
                for (int j = 0; j < 3; j++)
                {
                    paint_indices.Add(new List<int>());
                }
                for (int i = 0; i < mask_pixels.Length; i++)
                {
                    if (mask_pixels[i] == Color.Red)
                    {
                        paint_indices[0].Add(i);
                    }
                    else if (mask_pixels[i] == Color.Lime)
                    {
                        paint_indices[1].Add(i);
                    }
                    else if (mask_pixels[i] == Color.Blue)
                    {
                        paint_indices[2].Add(i);
                    }
                }
            }
            catch (Exception)
            {
                //_monitor.Log($"Unhandled error occured while getting paint overlay for buildings: {ex}", LogLevel.Debug);
                return null;
            }

            if (paint_indices == null)
            {
                return null;
            }
            if (color is null || !color.RequiresRecolor())
            {
                return null;
            }
            Color[] painted_pixels = new Color[base_texture.Width * base_texture.Height];
            base_texture.GetData(painted_pixels);
            Texture2D texture2D = new Texture2D(Game1.graphics.GraphicsDevice, base_texture.Width, base_texture.Height);
            if (!color.Color1Default.Value)
            {
                ApplyPaint(0, -100, 0, ref painted_pixels, paint_indices[0]);
                ApplyPaint(color.Color1Hue.Value, color.Color1Saturation.Value, color.Color1Lightness.Value, ref painted_pixels, paint_indices[0]);
            }
            if (!color.Color2Default.Value)
            {
                ApplyPaint(0, -100, 0, ref painted_pixels, paint_indices[1]);
                ApplyPaint(color.Color2Hue.Value, color.Color2Saturation.Value, color.Color2Lightness.Value, ref painted_pixels, paint_indices[1]);
            }
            if (!color.Color3Default.Value)
            {
                ApplyPaint(0, -100, 0, ref painted_pixels, paint_indices[2]);
                ApplyPaint(color.Color3Hue.Value, color.Color3Saturation.Value, color.Color3Lightness.Value, ref painted_pixels, paint_indices[2]);
            }

            texture2D.SetData(painted_pixels);
            return texture2D;
        }

        private static void ApplyPaint(int h_shift, int s_shift, int l_shift, ref Color[] pixels, List<int> indices)
        {
            foreach (int index in indices)
            {
                Color color = pixels[index];
                Utility.RGBtoHSL(color.R, color.G, color.B, out var h, out var s, out var i);
                h += (double)h_shift;
                s += (double)s_shift / 100.0;
                i += (double)l_shift / 100.0;
                while (h > 360.0)
                {
                    h -= 360.0;
                }
                for (; h < 0.0; h += 360.0)
                {
                }
                if (s < 0.0)
                {
                    s = 0.0;
                }
                if (s > 1.0)
                {
                    s = 1.0;
                }
                if (i < 0.0)
                {
                    i = 0.0;
                }
                if (i > 1.0)
                {
                    i = 1.0;
                }
                Utility.HSLtoRGB(h, s, i, out var r, out var g, out var b);
                color.R = (byte)r;
                color.G = (byte)g;
                color.B = (byte)b;
                pixels[index] = color;
            }
        }

        private static void BuildingPostfix(Building __instance, string type, Vector2 tile)
        {
            var instanceName = $"{AlternativeTextureModel.TextureType.Building}_{GetBuildingName(__instance)}";
            var instanceSeasonName = $"{instanceName}_{Game1.currentSeason}";

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

        public static void ResetTextureReversePatch(Building __instance)
        {
            new NotImplementedException("It's a stub!");
        }

        public static Rectangle GetSourceRectReversePatch(Building __instance)
        {
            return new Rectangle();
        }
    }
}
