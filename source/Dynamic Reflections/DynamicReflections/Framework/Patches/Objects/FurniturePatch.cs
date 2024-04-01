/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/DynamicReflections
**
*************************************************/

using DynamicReflections.Framework.Models;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace DynamicReflections.Framework.Patches.Objects
{
    internal class FurniturePatch : PatchTemplate
    {
        private readonly Type _type = typeof(Furniture);
        internal FurniturePatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal void Apply(Harmony harmony)
        {
            if (DynamicReflections.modHelper.ModRegistry.IsLoaded("spacechase0.DynamicGameAssets"))
            {
                try
                {
                    if (Type.GetType("DynamicGameAssets.Game.CustomBasicFurniture, DynamicGameAssets") is Type dgaFurnitureType && dgaFurnitureType != null)
                    {
                        harmony.Patch(AccessTools.Method(dgaFurnitureType, nameof(Furniture.draw), new[] { typeof(SpriteBatch), typeof(int), typeof(int), typeof(float) }), prefix: new HarmonyMethod(GetType(), nameof(DrawPrefix)));
                        harmony.Patch(AccessTools.Method(dgaFurnitureType, nameof(Furniture.draw), new[] { typeof(SpriteBatch), typeof(int), typeof(int), typeof(float) }), postfix: new HarmonyMethod(GetType(), nameof(DrawPostfix)));
                    }
                }
                catch (Exception ex)
                {
                    _monitor.Log($"Failed to patch Dynamic Game Assets in {this.GetType().Name}: DR will not be able to apply furniture reflections!", LogLevel.Warn);
                    _monitor.Log($"Patch for DGA failed in {this.GetType().Name}: {ex}", LogLevel.Trace);
                }
            }
        }

        [HarmonyBefore(new string[] { "spacechase0.DynamicGameAssets", "PeacefulEnd.AlternativeTextures" })]
        private static bool DrawPrefix(Furniture __instance, NetInt ___sourceIndexOffset, NetVector2 ___drawPosition, SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
        {
            if (DynamicReflections.isFilteringMirror is true && DynamicReflections.isDrawingMirrorReflection is false)
            {
                foreach (var mirror in DynamicReflections.mirrors.Values.ToList())
                {
                    if (mirror.IsEnabled is false || mirror.FurnitureLink != __instance || DynamicReflections.mirrorsManager.GetMask(__instance.Name) is null)
                    {
                        continue;
                    }

                    // Attempt to get the animation frame
                    Rectangle? sourceRectangle = null;
                    try
                    {
                        var packConfig = DynamicReflections.modHelper.Reflection.GetMethod(__instance, "GetCurrentConfiguration", required: false).Invoke<object>();
                        var packTextureName = DynamicReflections.modHelper.Reflection.GetProperty<string>(packConfig, "Texture", required: false).GetValue();

                        var packData = DynamicReflections.modHelper.Reflection.GetProperty<object>(__instance, "Data", required: false).GetValue();
                        var actualContentPack = DynamicReflections.modHelper.Reflection.GetField<object>(packData, "pack", required: false).GetValue();
                        var textureRectangleData = DynamicReflections.modHelper.Reflection.GetMethod(actualContentPack, "GetTexture", required: false).Invoke<object>(packTextureName, mirror.Settings.Dimensions.Width, mirror.Settings.Dimensions.Height);
                        sourceRectangle = DynamicReflections.modHelper.Reflection.GetProperty<Rectangle?>(textureRectangleData, "Rect", required: false).GetValue();

                        // Verify we actually got the source rectangle for the DGA texture
                        if (sourceRectangle is null)
                        {
                            _monitor.LogOnce($"Failed to get texture source rectangle from the DGA item {__instance.Name}", LogLevel.Warn);
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        _monitor.LogOnce($"Failed to get texture source rectangle from the DGA item {__instance.Name}", LogLevel.Warn);
                        _monitor.LogOnce($"Failed to get texture source rectangle from the DGA item {__instance.Name}: {ex}", LogLevel.Trace);
                        return false;
                    }

                    spriteBatch.Draw(DynamicReflections.mirrorsManager.GetMask(__instance.Name), Game1.GlobalToLocal(Game1.viewport, ___drawPosition.Value + ((__instance.shakeTimer > 0) ? new Vector2(Game1.random.Next(-1, 2), Game1.random.Next(-1, 2)) : Vector2.Zero)), sourceRectangle, Color.White * alpha, 0f, new Vector2(-mirror.Settings.Dimensions.X, -mirror.Settings.Dimensions.Y), 4f, __instance.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, ((int)__instance.furniture_type == 12) ? (2E-09f + __instance.tileLocation.Y / 100000f) : ((float)(__instance.boundingBox.Value.Bottom - (((int)__instance.furniture_type == 6 || (int)__instance.furniture_type == 17 || (int)__instance.furniture_type == 13) ? 48 : 8)) / 10000f));

                    return false;
                }
            }
            return true;
        }

        private static void DrawPostfix(Furniture __instance, NetVector2 ___drawPosition, SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
        {
            foreach (var mirror in DynamicReflections.mirrors.Values.ToList())
            {
                if (mirror.IsEnabled is false || mirror.FurnitureLink != __instance)
                {
                    continue;
                }

                var layerOffset = ((int)__instance.furniture_type == 12) ? (2E-09f + __instance.tileLocation.Y / 100000f) : ((float)(__instance.boundingBox.Value.Bottom - (((int)__instance.furniture_type == 6 || (int)__instance.furniture_type == 17 || (int)__instance.furniture_type == 13) ? 48 : 8)) / 10000f);
                spriteBatch.Draw(DynamicReflections.maskedPlayerMirrorReflectionRenders[mirror.ActiveIndex], Vector2.Zero, DynamicReflections.maskedPlayerMirrorReflectionRenders[mirror.ActiveIndex].Bounds, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, layerOffset + 0.001f);

                break;
            }
        }
    }
}
