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
using AlternativeTextures.Framework.Utilities.Extensions;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Characters;
using StardewValley.Locations;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Object = StardewValley.Object;

namespace AlternativeTextures.Framework.Patches.Buildings
{
    internal class BuildingPatch : PatchTemplate
    {
        private readonly Type _entity = typeof(Building);

        internal BuildingPatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_entity, nameof(Building.Update), new[] { typeof(GameTime) }), postfix: new HarmonyMethod(GetType(), nameof(UpdatePostfix)));
            harmony.Patch(AccessTools.Method(_entity, nameof(Building.resetTexture), null), prefix: new HarmonyMethod(GetType(), nameof(ResetTexturePrefix)));
            harmony.Patch(AccessTools.Constructor(_entity, new[] { typeof(BluePrint), typeof(Vector2) }), postfix: new HarmonyMethod(GetType(), nameof(BuildingPostfix)));

            harmony.CreateReversePatcher(AccessTools.Method(_entity, nameof(Building.resetTexture), null), new HarmonyMethod(GetType(), nameof(ResetTextureReversePatch))).Patch();
        }

        private static void UpdatePostfix(Building __instance, GameTime time)
        {
            if (!__instance.modData.ContainsKey("AlternativeTextureName") || AlternativeTextures.textureManager.GetSpecificTextureModel(__instance.modData["AlternativeTextureName"]) is null)
            {
                return;
            }

            var instanceName = String.Concat(__instance.modData["AlternativeTextureOwner"], ".", $"{AlternativeTextureModel.TextureType.Building}_{GetBuildingName(__instance)}");
            var instanceSeasonName = $"{instanceName}_{Game1.currentSeason}";
            if (__instance.modData["AlternativeTextureName"].ToLower() != instanceName && __instance.modData["AlternativeTextureName"].ToLower() != instanceSeasonName)
            {
                __instance.modData["AlternativeTextureName"] = String.Concat(__instance.modData["AlternativeTextureOwner"], ".", $"{AlternativeTextureModel.TextureType.Building}_{GetBuildingName(__instance)}");
                if (__instance.modData.ContainsKey("AlternativeTextureSeason") && !String.IsNullOrEmpty(__instance.modData["AlternativeTextureSeason"]))
                {
                    __instance.modData["AlternativeTextureSeason"] = Game1.currentSeason;
                    __instance.modData["AlternativeTextureName"] = String.Concat(__instance.modData["AlternativeTextureName"], "_", __instance.modData["AlternativeTextureSeason"]);

                    BuildingPatch.ResetTextureReversePatch(__instance);
                }
            }

            ResetTexturePrefix(__instance);
        }

        internal static void CondensedDrawInMenu(Building building, Texture2D texture, SpriteBatch b, int x, int y, float scale, float alpha = 1f)
        {
            switch (building)
            {
                case Barn barn:
                    //building.drawShadow(b, x, y);
                    b.Draw(texture, new Vector2(x, y) + new Vector2(building.animalDoor.X, building.animalDoor.Y + 3) * 16f * scale, new Rectangle(64, 112, 32, 16), Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0.888f);
                    b.Draw(texture, new Vector2(x, y) + new Vector2(building.animalDoor.X, (float)building.animalDoor.Y + 2.25f) * 16f * scale, new Rectangle(0, 112, 32, 16), Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, (float)(((int)building.tileY + (int)building.tilesHigh - 1) * 64) / 10000f - 1E-07f);
                    b.Draw(texture, new Vector2(x, y), new Rectangle(0, 0, 112, 112), building.color, 0f, new Vector2(0f, 0f), scale, SpriteEffects.None, 0.89f);
                    return;
                case Coop coop:
                    //building.drawShadow(b, x, y);
                    b.Draw(texture, new Vector2(x, y) + new Vector2(building.animalDoor.X, building.animalDoor.Y + 4) * 16f * scale, new Rectangle(16, 112, 16, 16), Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 1E-06f);
                    b.Draw(texture, new Vector2(x, y) + new Vector2(building.animalDoor.X, (float)building.animalDoor.Y + 3.5f) * 16f * scale, new Rectangle(0, 112, 16, 15), Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, (float)(((int)building.tileY + (int)building.tilesHigh) * 64) / 10000f - 1E-07f);
                    b.Draw(texture, new Vector2(x, y), new Rectangle(0, 0, 96, 112), building.color, 0f, new Vector2(0f, 0f), scale, SpriteEffects.None, 0.89f);
                    return;
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
                                b.Draw(Game1.mouseCursors, new Vector2(x + xWater * 64 + 32, y + (yWater + 1) * 64 - (int)Game1.currentLocation.waterPosition - 32), new Rectangle(Game1.currentLocation.waterAnimationIndex * 64, 2064 + (((xWater + yWater) % 2 != 0) ? ((!Game1.currentLocation.waterTileFlip) ? 128 : 0) : (Game1.currentLocation.waterTileFlip ? 128 : 0)), 64, 32 + (int)Game1.currentLocation.waterPosition - 5), Game1.currentLocation.waterColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
                            }
                            else
                            {
                                b.Draw(Game1.mouseCursors, new Vector2(x + xWater * 64 + 32, y + yWater * 64 + 32 - (int)((!topY) ? Game1.currentLocation.waterPosition : 0f)), new Rectangle(Game1.currentLocation.waterAnimationIndex * 64, 2064 + (((xWater + yWater) % 2 != 0) ? ((!Game1.currentLocation.waterTileFlip) ? 128 : 0) : (Game1.currentLocation.waterTileFlip ? 128 : 0)) + (topY ? ((int)Game1.currentLocation.waterPosition) : 0), 64, 64 + (topY ? ((int)(0f - Game1.currentLocation.waterPosition)) : 0)), Game1.currentLocation.waterColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
                            }
                        }
                    }
                    b.Draw(texture, new Vector2(x, y), new Rectangle(0, 0, 80, 80), building.color.Value * alpha, 0f, new Vector2(0f, 0f), scale, SpriteEffects.None, 1f);
                    b.Draw(texture, new Vector2(x + 64, y + 44 + ((Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 2500.0 < 1250.0) ? 4 : 0)), new Rectangle(16, 160, 48, 7), building.color.Value * alpha, 0f, Vector2.Zero, scale, SpriteEffects.None, 1f);
                    b.Draw(texture, new Vector2(x, y - 128), new Rectangle(80, 0, 80, 48), building.color.Value * alpha, 0f, new Vector2(0f, 0f), scale, SpriteEffects.None, 1f);
                    return;
                case GreenhouseBuilding greenhouse:
                    Rectangle rectangle = greenhouse.getSourceRect();
                    b.Draw(texture, new Vector2(x, y + 128), rectangle, building.color, 0f, new Vector2(0f, rectangle.Height / 2), scale, SpriteEffects.None, 0.89f);
                    return;
                case JunimoHut junimoHut:
                    //building.drawShadow(b, x, y);
                    b.Draw(texture, new Vector2(x, y), new Rectangle(0, 0, 48, 64), building.color, 0f, new Vector2(0f, 0f), scale, SpriteEffects.None, 0.89f);
                    return;
                case Mill mill:
                    b.Draw(texture, new Vector2(x, y), building.getSourceRectForMenu(), building.color, 0f, new Vector2(0f, 0f), scale, SpriteEffects.None, 0.89f);
                    b.Draw(texture, new Vector2(x + 32, y + 4), new Rectangle(64 + (int)Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 800 / 89 * 32 % 160, (int)Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 800 / 89 * 32 / 160 * 32, 32, 32), building.color, 0f, new Vector2(0f, 0f), scale, SpriteEffects.None, 0.9f);
                    return;
                case ShippingBin shippingBin:
                case Stable stable:
                default:
                    //building.drawShadow(b, x, y);
                    b.Draw(texture, new Vector2(x, y), building.getSourceRect(), building.color, 0f, new Vector2(0f, 0f), scale, SpriteEffects.None, 0.89f);

                    if (building is ShippingBin)
                    {
                        b.Draw(Game1.mouseCursors, new Vector2(x + 4, y - 20), new Rectangle(134, 226, 30, 25), Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 1f);
                    }
                    return;
            }
        }

        private static bool ResetTexturePrefix(Building __instance)
        {
            if (__instance.modData.ContainsKey("AlternativeTextureName"))
            {
                var textureModel = AlternativeTextures.textureManager.GetSpecificTextureModel(__instance.modData["AlternativeTextureName"]);
                if (textureModel is null)
                {
                    return false;
                }

                var textureVariation = Int32.Parse(__instance.modData["AlternativeTextureVariation"]);
                if (textureVariation == -1 || AlternativeTextures.modConfig.IsTextureVariationDisabled(textureModel.GetId(), textureVariation))
                {
                    return false;
                }

                __instance.texture = new Lazy<Texture2D>(delegate
                {
                    return GetBuildingTextureWithPaint(__instance, textureModel, textureVariation);
                });
                return false;
            }

            return true;
        }

        internal static Texture2D GetBuildingTextureWithPaint(Building building, AlternativeTextureModel textureModel, int textureVariation)
        {
            var xOffset = building.tilesWide * 16;
            var yOffset = textureVariation * textureModel.TextureHeight;
            var textureWidth = building.CanBePainted() ? xOffset : textureModel.TextureWidth;

            var texture2D = textureModel.GetTexture(textureVariation).CreateSelectiveCopy(Game1.graphics.GraphicsDevice, new Rectangle(0, yOffset, textureWidth, textureModel.TextureHeight));
            if (building.paintedTexture != null)
            {
                building.paintedTexture = null;
            }

            if (building.CanBePainted() && xOffset * 2 <= textureModel.GetTexture(textureVariation).Width)
            {
                var paintedTexture2D = textureModel.GetTexture(textureVariation).CreateSelectiveCopy(Game1.graphics.GraphicsDevice, new Rectangle(xOffset, yOffset, xOffset, textureModel.TextureHeight));
                building.paintedTexture = GetPaintedOverlay(building, texture2D, paintedTexture2D, building.netBuildingPaintColor.Value);
                if (building.paintedTexture != null)
                {
                    texture2D = building.paintedTexture;
                }
            }

            return texture2D;
        }

        private static Texture2D GetPaintedOverlay(Building building, Texture2D base_texture, Texture2D paint_mask_texture, BuildingPaintColor color)
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
            if (!color.RequiresRecolor())
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

        private static void BuildingPostfix(Building __instance, BluePrint blueprint, Vector2 tileLocation)
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
    }
}
