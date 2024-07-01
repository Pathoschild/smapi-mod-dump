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
using StardewModdingAPI;
using PrismaticValleyFramework.Framework;
using Microsoft.Xna.Framework;

namespace PrismaticValleyFramework.Patches
{
    internal class CollectionsPagePatcher
    {
        private static IMonitor Monitor;

        /// <summary>
        /// Apply the harmony patches to CollectionsPage.cs
        /// </summary>
        /// <param name="monitor">The Monitor instance for the PrismaticValleyFramework module</param>
        /// <param name="harmony">The Harmony instance for the PrismaticvalleyFramework module</param>
        internal static void Apply(IMonitor monitor, Harmony harmony)
        {
            Monitor = monitor;
            harmony.Patch(
                // Use DeclaredMethod as CollectionsPage.cs has only one implementation of the draw method
                original: AccessTools.DeclaredMethod(typeof(StardewValley.Menus.CollectionsPage), nameof(StardewValley.Menus.CollectionsPage.draw)),
                transpiler: new HarmonyMethod(typeof(CollectionsPagePatcher), nameof(draw_Transpiler))
            );
        }

        /// <summary>
        /// Transpiler instructions to patch the draw method in CollectionsPage.cs. 
        /// Overwrites the call to ClickableTextureComponent.draw drawing the collection items to call a custom draw method instead.
        /// </summary>
        /// <param name="instructions">The IL instructions</param>
        /// <param name="il">The IL generator</param>
        /// <returns>The patched IL instructions</returns>
        internal static IEnumerable<CodeInstruction> draw_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            var code = instructions.ToList();
            try
            {
                var matcher = new CodeMatcher(code, il);
                // Find the first call to Color multiply (This is a * between the color and float)
                // This moves to the first condition statement for the color being passed to ClickableTextureComponent.draw
                // Not using nameof since this is an unusual method call and I'd prefer not to end up breaking it
                matcher.MatchStartForward(
                    new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(Color), "op_Multiply", new Type[] {typeof(Color), typeof(float)}))
                ).ThrowIfNotMatch("Could not find first proper entry point for draw_Transpiler in CollectionsPage");
                
                // Remove first call to Color multiply to pass color and colorModifier in as separate variables to the override method
                matcher.RemoveInstruction(); 
                
                // Find where Color.White is loaded 
                // This moves to the second condition statement for the color being passed to ClickableTextureComponent.draw
                matcher.MatchStartForward(
                    new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(Color), "get_White"))
                ).ThrowIfNotMatch("Could not find second proper entry point for draw_Transpiler in CollectionsPage");
                
                matcher.Advance(1);
                // Load float of 1 as the colorModifier when color passed to ClickableTextureComponent.draw is Color.White
                matcher.InsertAndAdvance(
                    new CodeInstruction(OpCodes.Ldc_R4, 1.0f) 
                );
                
                // Find the second call to Color multiply 
                // This moves to the third condition statement for the color being passed to ClickableTextureComponent.draw)
                matcher.MatchStartForward(
                    new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(Color), "op_Multiply", new Type[] {typeof(Color), typeof(float)}))
                ).ThrowIfNotMatch("Could not find third proper entry point for draw_Transpiler in CollectionsPage");

                // Remove second call to Color multiply
                matcher.RemoveInstruction(); 
                matcher.Advance(1);
                // Remove the call to ClickableTextureComponent.draw and the default loads for its 3 optional parameters 
                // *Unknown where the final parameter, yOffset, is defined, but the il instructions include a load for it
                matcher.RemoveInstructions(4); 
                // Insert a call to the custom draw method
                matcher.InsertAndAdvance( 
                    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ParseCustomFields), nameof(ParseCustomFields.DrawCustomColorForCollectionsPage)))
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