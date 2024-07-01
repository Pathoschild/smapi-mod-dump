/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Jolly-Alpaca/PrismaticValleyFramework
**
*************************************************/

using System.Reflection.Emit;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using PrismaticValleyFramework.Framework;
using StardewValley;

namespace PrismaticValleyFramework.Patches
{
    internal class FarmerRendererPatcher
    {
        private static IMonitor Monitor;

        /// <summary>
        /// Apply the harmony patches to FarmerRenderer.cs
        /// </summary>
        /// <param name="monitor">The Monitor instance for the PrismaticValleyFramework module</param>
        /// <param name="harmony">The Harmony instance for the PrismaticvalleyFramework module</param>
        internal static void Apply(IMonitor monitor, Harmony harmony)
        {
            Monitor = monitor;
            harmony.Patch(
                // Use Method with overload parameters as FarmerRenderer.cs has multiple implementations of the draw method
                // ApplyShoeColor is a private method so cannot be referenced using nameof
                original: AccessTools.Method(typeof(FarmerRenderer), "draw", new Type[] {typeof(SpriteBatch), typeof(FarmerSprite.AnimationFrame), typeof(int), typeof(Rectangle), typeof(Vector2), typeof(Vector2), typeof(float), typeof(int), typeof(Color), typeof(float), typeof(float), typeof(Farmer)}),
                transpiler: new HarmonyMethod(typeof(FarmerRendererPatcher), nameof(draw_Transpiler))
            );
        }

        /// <summary>
        /// Transpiler instructions to patch the draw method in FarmerRenderer.cs. 
        /// Calls a custom method to draw custom boot sprites if the currently equiped boots have a custom color override.
        /// </summary>
        /// <param name="instructions">The IL instructions</param>
        /// <param name="il">The IL generator</param>
        /// <returns>The patched IL instructions</returns>
        internal static IEnumerable<CodeInstruction> draw_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            var code = instructions.ToList();
            try
            {
                // Draw the boots just before the pants are drawn as this is after the swimming check. No need to draw shoes if the farmer is swimming.
                // Find the call to Farmer.GetDisplayPants 
                var matcher = new CodeMatcher(code, il);
                matcher.MatchStartForward(
                    new CodeMatch(OpCodes.Ldarg_S, null, "who"), // Load Farmer arg who
                    new CodeMatch(OpCodes.Ldloca_S), // Load the address for local variable 1 to store texture from GetDisplayPants
                    new CodeMatch(OpCodes.Ldloca_S)  // Load the address for local variable 2 to store pantsIndex from GetDisplayPants
                ).ThrowIfNotMatch("Could not find proper entry point for draw_Transpiler in FarmerRenderer");

                // Call a custom method to draw the custom boots
                // Replace the load Farmer who arg for GetDisplayPants to steal the labels so the custom draw method isn't added to an unreachable code block
                matcher.SetAndAdvance(OpCodes.Ldarg_0, null); // Loads the FarmerRenderer instance
                matcher.Insert(
                    // Load the parameters for the custom draw method
                    new CodeInstruction(OpCodes.Ldarg_S, 12), // Loads the Farmer who arg
                    new CodeInstruction(OpCodes.Ldarg_1), // Loads the SpriteBatch arg
                    new CodeInstruction(OpCodes.Ldarg_S, 5), // Loads the position arg
                    new CodeInstruction(OpCodes.Ldarg_S, 6), // Loads the origin arg
                        // Load positionOffset 
                    new CodeInstruction(OpCodes.Ldarg_0), // Loads the FarmerRenderer instance
                    new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(FarmerRenderer), "positionOffset")), // Loads the positionOffset field for the FarmerRenderer instance
                        // Load positionOffset end
                    new CodeInstruction(OpCodes.Ldarg_S, 4), // Loads the sourceRect arg
                    new CodeInstruction(OpCodes.Ldarg_S, 9), // Loads the overrideColor arg
                    new CodeInstruction(OpCodes.Ldarg_S, 10), // Loads the rotation arg
                    new CodeInstruction(OpCodes.Ldarg_S, 11), // Loads the scale arg
                    new CodeInstruction(OpCodes.Ldarg_2), // Loads the animationFrame arg
                        // Load layerDepth: FarmerRenderer.GetLayerDepth(layerDepth, FarmerSpriteLayers.Base)
                    new CodeInstruction(OpCodes.Ldarg_S, 7), // Loads the layerDepth arg as parameter to pass to GetLayerDepth
                    new CodeInstruction(OpCodes.Ldc_I4_2), // Loads 2 onto the stack as the parameter FarmerSpriteLayers layer arg of GetLayerDepth. FarmerSpriteLayers in an enum, where Base is at index 2.
                    new CodeInstruction(OpCodes.Ldc_I4_0), // Loads 0 onto the stack to pass false to the optional bool arg of GetLayerDepth
                    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(FarmerRenderer), nameof(FarmerRenderer.GetLayerDepth))), // Call FarmerRenderer.GetLayerDepth
                    // Insert a call to the custom draw boots method
                    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(CustomDrawUtilities), nameof(CustomDrawUtilities.DrawCustomBoots))),
                    // Readd the load Farmer who arg for GetDisplayPants
                    new CodeInstruction(OpCodes.Ldarg_S, 12)
                );
                return matcher.InstructionEnumeration();
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(draw_Transpiler)}:\n{ex}", LogLevel.Error);
                return code;
            }
        }
    }
}