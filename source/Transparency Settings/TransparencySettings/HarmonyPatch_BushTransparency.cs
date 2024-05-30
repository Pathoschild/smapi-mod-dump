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
    public static class HarmonyPatch_BushTransparency
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

                Monitor.Log($"Applying Harmony patch \"{nameof(HarmonyPatch_BushTransparency)}\": postfixing SDV method \"Bush.tickUpdate(GameTime, Vector2, GameLocation)\".", LogLevel.Trace);
                harmony.Patch(
                    original: AccessTools.Method(typeof(Bush), nameof(Bush.tickUpdate), new[] { typeof(GameTime) }),
                    postfix: new HarmonyMethod(typeof(HarmonyPatch_BushTransparency), nameof(Bush_tickUpdate))
                );

                Monitor.Log($"Applying Harmony patch \"{nameof(HarmonyPatch_BushTransparency)}\": transpiling SDV method \"Bush.draw(SpriteBatch)\".", LogLevel.Trace);
                harmony.Patch(
                    original: AccessTools.Method(typeof(Bush), nameof(Bush.draw), new[] { typeof(SpriteBatch) }),
                    transpiler: new HarmonyMethod(typeof(HarmonyPatch_BushTransparency), nameof(Bush_draw))
                );

                Applied = true;
            }
        }

        /// <summary>Modifies the alpha (transparency) value of this instance after its per-tick update.</summary>
        /// <param name="__instance">The instance calling the original method.</param>
        private static void Bush_tickUpdate(Bush __instance)
        {
            try
            {
                if (ModEntry.Config.BushSettings.Enable) //if this type of custom transparency is enabled
                {
                    if (__instance.size.Value != 3) //if this bush is NOT a tea bush
                    {
                        Point centerPixel = __instance.getRenderBounds().Center; //get the center pixel of the bush's visible area

                        if (InputManager.FullTransparency.Value || //if full transparency is toggled on, or ALL of the following is true:
                            (InputManager.DisableTransparency.Value == false && //if transparency is NOT toggled off
                            (ModEntry.Config.BushSettings.BelowPlayerOnly == false || CacheManager.CurrentPlayerTile.Y < __instance.Tile.Y) && //AND if the bush is below the player's Y level OR that option is disabled,
                            Vector2.Distance(new Vector2(centerPixel.X, centerPixel.Y), CacheManager.CurrentPlayerTile * Game1.tileSize) < (ModEntry.Config.BushSettings.TileDistance * Game1.tileSize))) //AND if the player is within range of this bush's center pixel
                        {
                            CacheManager.GetAlpha(__instance, -0.05f, ModEntry.Config.BushSettings.MinimumOpacity); //increase transparency in this bush's cached alpha
                        }
                        else
                        {
                            CacheManager.GetAlpha(__instance, 0.05f, ModEntry.Config.BushSettings.MinimumOpacity); //decrease transparency in this bush's cached alpha
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Monitor.LogOnce($"Harmony patch \"{nameof(HarmonyPatch_BushTransparency)}\" has encountered an error. Custom bush transparency might not work correctly. Full error message: \n{ex.ToString()}", LogLevel.Error);
                return; //run the original method
            }
        }

        /// <summary>Modifies the Bush draw code to apply transparency, similar to the "alpha" field removed in Stardew v1.6.</summary>
        /// <remarks>
        /// Old C#:
        ///     spriteBatch.Draw(/*...*/ this.sourceRect.Value, Color.White, this.shakeRotation, /*...*/);
        /// 
        /// New C#:
        ///     spriteBatch.Draw(/*...*/ this.sourceRect.Value, ApplyTransparency(Color.White, this), this.shakeRotation, /*...*/);
        /// 
        /// 
        /// Old IL:
        ///     IL_01ed: call valuetype [MonoGame.Framework]Microsoft.Xna.Framework.Color [MonoGame.Framework]Microsoft.Xna.Framework.Color::get_White()
        ///     IL_01f2: ldarg.0
	    ///     IL_01f3: ldfld float32 StardewValley.TerrainFeatures.Bush::shakeRotation
        /// 
        /// New IL:
        ///     IL_01ed: call valuetype [MonoGame.Framework]Microsoft.Xna.Framework.Color [MonoGame.Framework]Microsoft.Xna.Framework.Color::get_White()
        ///         (?): ldarg.0
        ///         (?): call static [MonoGame.Framework]Microsoft.Xna.Framework.Color CustomTransparency.HarmonyPatch_BushTransparency::ApplyTransparency([MonoGame.Framework]Microsoft.Xna.Framework.Color originalColor, object instance)
        ///     IL_01f2: ldarg.0
	    ///     IL_01f3: ldfld float32 StardewValley.TerrainFeatures.Bush::shakeRotation
        /// </remarks>
        private static IEnumerable<CodeInstruction> Bush_draw(IEnumerable<CodeInstruction> instructions)
        {
            try
            {
                List<CodeInstruction> patched = new List<CodeInstruction>(instructions); //make a copy of the instructions to modify

                var applyMethod = AccessTools.Method(typeof(HarmonyPatch_BushTransparency), nameof(ApplyTransparency)); //get info for the method that applies transparency
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
                Monitor.LogOnce($"Harmony patch \"{nameof(HarmonyPatch_BushTransparency)}\" has encountered an error. Transpiler \"{nameof(Bush_draw)}\" will not be applied. Full error message:\n{ex.ToString()}", LogLevel.Error);
                return instructions; //return the original instructions
            }
        }

        /// <summary>Modifies a color with the current alpha transparency level of this object.</summary>
        /// <param name="instance">The instance being drawn with this color, e.g. a bush.</param>
        /// <param name="originalColor">The unmodified color of the object, generally with no alpha transparency.</param>
        /// <returns>The color with this object's alpha transparency level applied.</returns>
        private static Color ApplyTransparency(Color originalColor, object instance)
        {
            return originalColor * CacheManager.GetAlpha(instance, 0f, ModEntry.Config.BushSettings.MinimumOpacity); //return the color multiplied by this object's current alpha transparency level
        }
    }
}
