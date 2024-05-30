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
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace TransparencySettings
{
    public static class HarmonyPatch_GrassTransparency
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

                Monitor.Log($"Applying Harmony patch \"{nameof(HarmonyPatch_GrassTransparency)}\": transpiling SDV method \"Grass.draw(SpriteBatch)\".", LogLevel.Trace);
                harmony.Patch(
                    original: AccessTools.Method(typeof(Grass), nameof(Grass.draw), new[] { typeof(SpriteBatch) }),
                    transpiler: new HarmonyMethod(typeof(HarmonyPatch_GrassTransparency), nameof(Grass_draw))
                );

                helper.Events.GameLoop.UpdateTicking += UpdateGrassTransparency;

                Applied = true;
            }
        }

        /// <summary>Modifies the alpha (transparency) value of each grass instance at the current location.</summary>
        private static void UpdateGrassTransparency(object sender, StardewModdingAPI.Events.UpdateTickingEventArgs e)
        {
            try
            {
                if (ModEntry.Config.GrassSettings.Enable && Game1.player.currentLocation is GameLocation location) //if this type of custom transparency is enabled AND the local player's location isn't null
                {
                    foreach (var tf in location.terrainFeatures.Values) //for each terrain feature at the l
                    {
                        if (tf is Grass grass)
                        {
                            if (InputManager.FullTransparency.Value || //if full transparency is toggled on, or ALL of the following is true:
                                (InputManager.DisableTransparency.Value == false && //if transparency is NOT toggled off
                                (ModEntry.Config.GrassSettings.BelowPlayerOnly == false || CacheManager.CurrentPlayerTile.Y < grass.Tile.Y) && //AND if the grass is below the player's Y level OR that option is disabled,
                                Vector2.Distance(grass.Tile, CacheManager.CurrentPlayerTile) < ModEntry.Config.GrassSettings.TileDistance)) //AND if the player is within range of this grass
                            {
                                CacheManager.GetAlpha(grass, -0.05f, ModEntry.Config.GrassSettings.MinimumOpacity); //make this grass MORE transparent
                            }
                            else
                            {
                                CacheManager.GetAlpha(grass, 0.05f, ModEntry.Config.GrassSettings.MinimumOpacity); //make this grass LESS transparent
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Monitor.LogOnce($"Harmony patch \"{nameof(HarmonyPatch_GrassTransparency)}\" has encountered an error in the \"{UpdateGrassTransparency}\" event. Custom grass transparency might not work correctly. Full error message: \n{ex.ToString()}", LogLevel.Error);
                return; //run the original method
            }
        }

        /// <summary>Modifies the grass draw code to apply transparency.</summary>
        /// <remarks>
        /// Old C#:
        ///     spriteBatch.Draw(/*...*/ Color.White, /*...*/);
        /// 
        /// New C#:
        ///     spriteBatch.Draw(/*...*/ ApplyTransparency(Color.White, this), /*...*/);
        /// 
        /// 
        /// Old IL:
        ///     IL_00d5: call valuetype [MonoGame.Framework]Microsoft.Xna.Framework.Color [MonoGame.Framework]Microsoft.Xna.Framework.Color::get_White()
		///     IL_00da: ldarg.0
		///     IL_00db: ldfld float32 StardewValley.TerrainFeatures.Grass::shakeRotation
        /// 
        /// New IL:
        ///     IL_00d5: call valuetype [MonoGame.Framework]Microsoft.Xna.Framework.Color [MonoGame.Framework]Microsoft.Xna.Framework.Color::get_White()
        ///         (?): ldarg.0
        ///         (?): call static [MonoGame.Framework]Microsoft.Xna.Framework.Color CustomTransparency.HarmonyPatch_GrassTransparency::ApplyTransparency([MonoGame.Framework]Microsoft.Xna.Framework.Color originalColor, object instance)
        ///     IL_00da: ldarg.0
		///     IL_00db: ldfld float32 StardewValley.TerrainFeatures.Grass::shakeRotation
        /// </remarks>
        private static IEnumerable<CodeInstruction> Grass_draw(IEnumerable<CodeInstruction> instructions)
        {
            try
            {
                List<CodeInstruction> patched = new List<CodeInstruction>(instructions); //make a copy of the instructions to modify

                var applyMethod = AccessTools.Method(typeof(HarmonyPatch_GrassTransparency), nameof(ApplyTransparency)); //get info for the method that applies transparency
                CodeInstruction applyInstruction = new CodeInstruction(OpCodes.Call, applyMethod); //make an instruction that calls it

                for (int x = 0; x < patched.Count; x++) //for each instruction
                {
                    if ((patched[x].opcode == OpCodes.Call || patched[x].opcode == OpCodes.Callvirt) && patched[x].operand?.ToString()?.Contains("Color", StringComparison.Ordinal) == true //if this code is a method call to get a Color
                     && patched[x + 1].opcode == OpCodes.Ldarg_0 //and the next code loads arg 0 (this object instance)
                     && patched[x + 2].opcode == OpCodes.Ldfld && patched[x + 2].operand?.ToString().Contains("shakeRotation", StringComparison.Ordinal) == true) //and the next code loads the field shakeRotation
                    {
                        patched.InsertRange(x + 1, new[] //after the color is loaded onto the stack...
                        {
                            new CodeInstruction(OpCodes.Ldarg_0), //load this object instance onto the stack
                            applyInstruction //call the apply method; this removes the instance and color from the stack, and returns a modified color
                        });
                        Monitor.VerboseLog($"Transpiler inserted a call to {nameof(ApplyTransparency)} at line {x}.");
                        break; //stop after finding 1 match
                    }
                }

                return patched; //return the patched instructions
            }
            catch (Exception ex)
            {
                Monitor.LogOnce($"Harmony patch \"{nameof(HarmonyPatch_GrassTransparency)}\" has encountered an error. Transpiler \"{nameof(Grass_draw)}\" will not be applied. Full error message:\n{ex.ToString()}", LogLevel.Error);
                return instructions; //return the original instructions
            }
        }

        /// <summary>Modifies a color with the current alpha transparency level of this object.</summary>
        /// <param name="instance">The grass instance being drawn.</param>
        /// <param name="originalColor">The unmodified color of the object, generally with no alpha transparency.</param>
        /// <returns>The color with this object's alpha transparency level applied.</returns>
        private static Color ApplyTransparency(Color originalColor, object instance)
        {
            return originalColor * CacheManager.GetAlpha(instance, 0f, ModEntry.Config.GrassSettings.MinimumOpacity); //return the color multiplied by this object's current alpha transparency level
        }
    }
}
