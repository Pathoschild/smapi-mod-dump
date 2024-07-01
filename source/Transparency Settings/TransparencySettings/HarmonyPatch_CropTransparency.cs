/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/TransparencySettings
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace TransparencySettings
{
    public static class HarmonyPatch_CropTransparency
    {
        /// <summary>True if this patch is currently applied.</summary>
        public static bool Applied { get; private set; } = false;
        /// <summary>The SMAPI helper instance to use for events and other API access.</summary>
        private static IModHelper Helper { get; set; } = null;
        /// <summary>The monitor instance to use for log messages. Null if not provided.</summary>
        private static IMonitor Monitor { get; set; } = null;

        /// <summary>Applies this Harmony patch to the game.</summary>
        /// <param name="harmony">The <see cref="Harmony"/> created with this mod's ID.</param>
        /// <param name="helper">The <see cref="IModHelper"/> provided to this mod by SMAPI. Used for events and other API access.</param>
        /// <param name="monitor">The <see cref="IMonitor"/> provided to this mod by SMAPI. Used for log messages.</param>
        public static void ApplyPatch(Harmony harmony, IModHelper helper, IMonitor monitor)
        {
            if (!Applied && helper != null && monitor != null) //if NOT already applied AND valid tools were provided
            {
                Helper = helper; //store helper
                Monitor = monitor; //store monitor

                Monitor.Log($"Applying Harmony patch \"{nameof(HarmonyPatch_CropTransparency)}\": transpiling SDV method \"Crop.draw(SpriteBatch, Vector2, Color, float)\".", LogLevel.Trace);
                harmony.Patch(
                    original: AccessTools.Method(typeof(Crop), nameof(Crop.draw), new[] { typeof(SpriteBatch), typeof(Vector2), typeof(Color), typeof(float) }),
                    transpiler: new HarmonyMethod(typeof(HarmonyPatch_CropTransparency), nameof(Crop_draw))
                );

                Monitor.Log($"Applying Harmony patch \"{nameof(HarmonyPatch_CropTransparency)}\": transpiling SDV method \"Crop.drawWithOffset(SpriteBatch, Vector2, Color, float, Vector2)\".", LogLevel.Trace);
                harmony.Patch(
                    original: AccessTools.Method(typeof(Crop), nameof(Crop.drawWithOffset), new[] { typeof(SpriteBatch), typeof(Vector2), typeof(Color), typeof(float), typeof(Vector2) }),
                    transpiler: new HarmonyMethod(typeof(HarmonyPatch_CropTransparency), nameof(Crop_drawWithOffset))
                );

                helper.Events.GameLoop.UpdateTicking += UpdateCropTransparency;

                Applied = true;
            }
        }

        /// <summary>Modifies the alpha (transparency) value of each crop instance at the current location.</summary>
        private static void UpdateCropTransparency(object sender, StardewModdingAPI.Events.UpdateTickingEventArgs e)
        {
            try
            {
                if (ModEntry.Config.CropSettings.Enable && Game1.player.currentLocation is GameLocation location) //if this type of custom transparency is enabled AND the local player's location isn't null
                {
                    var crops = new List<Crop>(); //a list of all crops at this location

                    foreach (var tf in location.terrainFeatures.Values) //get crops planted in the ground
                    {
                        if (tf is HoeDirt dirt && dirt.crop is Crop dirtCrop)
                            crops.Add(dirtCrop);
                    }

                    foreach (var obj in location.objects.Values) //get crops planted in garden pots
                    {
                        if (obj is IndoorPot pot && pot.hoeDirt.Value?.crop is Crop potCrop)
                            crops.Add(potCrop);
                    }

                    foreach (Crop crop in crops)
                    {
                        if (InputManager.FullTransparency.Value || //if full transparency is toggled on, or ALL of the following is true:
                                    (InputManager.DisableTransparency.Value == false && //if transparency is NOT toggled off
                                    (ModEntry.Config.CropSettings.BelowPlayerOnly == false || CacheManager.CurrentPlayerTile.Y < crop.tilePosition.Y) && //AND if the crop is below the player's Y level OR that option is disabled,
                                    Vector2.Distance(crop.tilePosition, CacheManager.CurrentPlayerTile) < ModEntry.Config.CropSettings.TileDistance)) //AND if the player is within range of this crop
                        {
                            CacheManager.GetAlpha(crop, -0.05f, ModEntry.Config.CropSettings.MinimumOpacity); //make this crop MORE transparent
                        }
                        else
                        {
                            CacheManager.GetAlpha(crop, 0.05f, ModEntry.Config.CropSettings.MinimumOpacity); //make this crop LESS transparent
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Monitor.LogOnce($"Harmony patch \"{nameof(HarmonyPatch_CropTransparency)}\" has encountered an error in the \"{UpdateCropTransparency}\" event. Custom crop transparency might not work correctly. Full error message: \n{ex.ToString()}", LogLevel.Error);
                return; //run the original method
            }
        }

        /// <summary>Modifies the crop draw code to apply transparency.</summary>
        /// <remarks>
        /// Old C#:
        ///     b.Draw(... toTint, rotation, ...);
        ///     ...
        ///     b.Draw(... toTint, rotation, ...);
        ///     ...
        ///     b.Draw(... tintColor, rotation, ...);
        /// 
        /// New C#:
        ///     b.Draw(... ApplyTransparency(toTint, this), rotation, ...);
        ///     ...
        ///     b.Draw(... ApplyTransparency(toTint, this), rotation, ...);
        ///     ...
        ///     b.Draw(... ApplyTransparency(tintColor, this), rotation, ...);
        /// 
        /// Old IL:
        ///     IL_01f5: ldarg.3
        ///     IL_01f6: ldarg.s rotation
        ///     ...
        ///     IL_023e: ldarg.3
        ///     IL_023f: ldarg.s rotation
        ///     ...
        ///     IL_02ab: ldloc.3
        ///     IL_02ac: ldarg.s rotation
        /// 
        /// New IL:
        ///     IL_01f5: ldarg.3
        ///         (?): ldarg.0
        ///         (?): call static [MonoGame.Framework]Microsoft.Xna.Framework.Color CustomTransparency.HarmonyPatch_CropTransparency::ApplyTransparency([MonoGame.Framework]Microsoft.Xna.Framework.Color originalColor, object instance)
        ///     IL_01f6: ldarg.s rotation
        ///         ...
        ///     IL_023e: ldarg.3
        ///         (?): ldarg.0
        ///         (?): call static [MonoGame.Framework]Microsoft.Xna.Framework.Color CustomTransparency.HarmonyPatch_CropTransparency::ApplyTransparency([MonoGame.Framework]Microsoft.Xna.Framework.Color originalColor, object instance)
        ///     IL_023f: ldarg.s rotation
        ///         ...
        ///     IL_02ab: ldloc.3
        ///         (?): ldarg.0
        ///         (?): call static [MonoGame.Framework]Microsoft.Xna.Framework.Color CustomTransparency.HarmonyPatch_CropTransparency::ApplyTransparency([MonoGame.Framework]Microsoft.Xna.Framework.Color originalColor, object instance)
        ///     IL_02ac: ldarg.s rotation
        /// </remarks>
        private static IEnumerable<CodeInstruction> Crop_draw(IEnumerable<CodeInstruction> instructions)
        {
            try
            {
                List<CodeInstruction> patched = new List<CodeInstruction>(instructions); //make a copy of the instructions to modify

                var applyMethod = AccessTools.Method(typeof(HarmonyPatch_CropTransparency), nameof(ApplyTransparency)); //get info for the method that applies transparency

                for (int x = 0; x < patched.Count - 1; x++) //for each instruction
                {
                    if ((patched[x].opcode == OpCodes.Ldarg_3 || patched[x].opcode == OpCodes.Ldloc_3) //if this code loads arg 3 (toTint) or local 3 (tintColor)
                     && patched[x + 1].opcode == OpCodes.Ldarg_S && patched[x + 1].operand?.ToString() == "4") //and the next code loads arg 4 (rotation)
                    {
                        patched.InsertRange(x + 1, new[] //after the color is loaded onto the stack...
                        {
                            new CodeInstruction(OpCodes.Ldarg_0), //load this object instance onto the stack
                            new CodeInstruction(OpCodes.Call, applyMethod) //call the apply method; this removes the instance and color from the stack, and returns a modified color
                        });
                        Monitor.VerboseLog($"Transpiler inserted a call to {nameof(ApplyTransparency)} at line {x}.");
                    }
                }

                return patched; //return the patched instructions
            }
            catch (Exception ex)
            {
                Monitor.LogOnce($"Harmony patch \"{nameof(HarmonyPatch_CropTransparency)}\" has encountered an error. Transpiler \"{nameof(Crop_draw)}\" will not be applied. Full error message:\n{ex.ToString()}", LogLevel.Error);
                return instructions; //return the original instructions
            }
        }

        /// <summary>Modifies the "garden pot" crop draw code to apply transparency.</summary>
        /// <remarks>
        /// Old C#:
        ///     b.Draw(... toTint, rotation, ...);
        ///     ...
        ///     b.Draw(... toTint, rotation, ...);
        ///     ...
        ///     b.Draw(... tintColor.Value, rotation, ...);
        /// 
        /// New C#:
        ///     b.Draw(... ApplyTransparency(toTint, this), rotation, ...);
        ///     ...
        ///     b.Draw(... ApplyTransparency(toTint, this), rotation, ...);
        ///     ...
        ///     b.Draw(... ApplyTransparency(tintColor.Value, this), rotation, ...);
        /// 
        /// Old IL:
        ///     IL_0070: ldarg.3
        ///     IL_0071: ldarg.s rotation
        ///     ...
        ///     IL_019c: ldarg.3
        ///     IL_019d: ldarg.s rotation
        ///     ...
        ///     IL_0276: callvirt instance !0 class Netcode.NetFieldBase`2<valuetype [MonoGame.Framework]Microsoft.Xna.Framework.Color, class Netcode.NetColor>::get_Value()
        ///     IL_027b: ldarg.s rotation
        /// 
        /// New IL:
        ///     IL_0070: ldarg.3
        ///         (?): ldarg.0
        ///         (?): call static [MonoGame.Framework]Microsoft.Xna.Framework.Color CustomTransparency.HarmonyPatch_CropTransparency::ApplyTransparency([MonoGame.Framework]Microsoft.Xna.Framework.Color originalColor, object instance)
        ///     IL_0071: ldarg.s rotation
        ///     ...
        ///     IL_019c: ldarg.3
        ///         (?): ldarg.0
        ///         (?): call static [MonoGame.Framework]Microsoft.Xna.Framework.Color CustomTransparency.HarmonyPatch_CropTransparency::ApplyTransparency([MonoGame.Framework]Microsoft.Xna.Framework.Color originalColor, object instance)
        ///     IL_019d: ldarg.s rotation
        ///     ...
        ///     IL_0276: callvirt instance !0 class Netcode.NetFieldBase`2<valuetype [MonoGame.Framework]Microsoft.Xna.Framework.Color, class Netcode.NetColor>::get_Value()
        ///         (?): ldarg.0
        ///         (?): call static [MonoGame.Framework]Microsoft.Xna.Framework.Color CustomTransparency.HarmonyPatch_CropTransparency::ApplyTransparency([MonoGame.Framework]Microsoft.Xna.Framework.Color originalColor, object instance)
        ///     IL_027b: ldarg.s rotation
        /// </remarks>
        private static IEnumerable<CodeInstruction> Crop_drawWithOffset(IEnumerable<CodeInstruction> instructions)
        {
            try
            {
                List<CodeInstruction> patched = new List<CodeInstruction>(instructions); //make a copy of the instructions to modify

                var applyMethod = AccessTools.Method(typeof(HarmonyPatch_CropTransparency), nameof(ApplyTransparency)); //get info for the method that applies transparency

                for (int x = 0; x < patched.Count - 1; x++) //for each instruction
                {
                    if ((patched[x].opcode == OpCodes.Ldarg_3 || //if this code loads arg 3 (toTint)...
                        (patched[x].opcode == OpCodes.Callvirt && patched[x].operand?.ToString().Contains("get_Value", StringComparison.Ordinal) == true)) //...or it gets a NetColor value
                     && patched[x + 1].opcode == OpCodes.Ldarg_S && patched[x + 1].operand?.ToString() == "4") //and the next code loads arg 4 (rotation)
                    {
                        patched.InsertRange(x + 1, new[] //after the color is loaded onto the stack...
                        {
                            new CodeInstruction(OpCodes.Ldarg_0), //load this object instance onto the stack
                            new CodeInstruction(OpCodes.Call, applyMethod) //call the apply method; this removes the instance and color from the stack, and returns a modified color
                        });
                        Monitor.VerboseLog($"Transpiler inserted a call to {nameof(ApplyTransparency)} at line {x}.");
                    }
                }

                return patched; //return the patched instructions
            }
            catch (Exception ex)
            {
                Monitor.LogOnce($"Harmony patch \"{nameof(HarmonyPatch_CropTransparency)}\" has encountered an error. Transpiler \"{nameof(Crop_drawWithOffset)}\" will not be applied. Full error message:\n{ex.ToString()}", LogLevel.Error);
                return instructions; //return the original instructions
            }
        }

        /// <summary>Modifies a color with the current alpha transparency level of this object.</summary>
        /// <param name="instance">The grass instance being drawn.</param>
        /// <param name="originalColor">The unmodified color of the object, generally with no alpha transparency.</param>
        /// <returns>The color with this object's alpha transparency level applied.</returns>
        private static Color ApplyTransparency(Color originalColor, object instance)
        {
            return originalColor * CacheManager.GetAlpha(instance, 0f, ModEntry.Config.CropSettings.MinimumOpacity); //return the color multiplied by this object's current alpha transparency level
        }
    }
}
