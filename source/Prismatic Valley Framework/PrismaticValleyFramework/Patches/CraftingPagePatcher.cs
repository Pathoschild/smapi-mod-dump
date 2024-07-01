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
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;

namespace PrismaticValleyFramework.Patches
{
    internal class CraftingPagePatcher
    {
        private static IMonitor Monitor;

        /// <summary>
        /// Apply the harmony patches to CraftingPage.cs
        /// </summary>
        /// <param name="monitor">The Monitor instance for the PrismaticValleyFramework module</param>
        /// <param name="harmony">The Harmony instance for the PrismaticvalleyFramework module</param>
        internal static void Apply(IMonitor monitor, Harmony harmony)
        {
            Monitor = monitor;
            harmony.Patch(
                // Use DeclaredMethod as CraftingPage.cs has only one implementation of the draw method
                original: AccessTools.DeclaredMethod(typeof(CraftingPage), nameof(CraftingPage.draw)),
                transpiler: new HarmonyMethod(typeof(CraftingPagePatcher), nameof(draw_Transpiler))
            );
        }

        /// <summary>
        /// Transpiler instructions to patch the draw method in CraftingPage.cs. 
        /// Overwrites the calls to ClickableTextureComponent.draw that draw the crafting recipes' output to call a custom draw method instead.
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
                // Patch the draw function for grayed out recipes
                // Find where Color.DimGray is loaded
                matcher.MatchStartForward(
                    new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(Color), "get_DimGray"))
                ).ThrowIfNotMatch("Could not find proper entry point for inactive recipes for draw_Transpiler in CraftingPage");
                
                // Move to start of parameter loading for call to ClickableTextureComponent.draw passing Color.DimGray as the color parameter
                matcher.Advance(-2); 
                // Load the CraftingPage __instance
                matcher.InsertAndAdvance( 
                    new CodeInstruction(OpCodes.Ldarg_0) 
                );
                // Keep the existing parameter loads for ClickableTextureComponent: Spritebatch, Color.DimGray, and color multiplier
                matcher.Advance(4); 
                // Remove call to Color multiply (the * between the color and float) to pass color and colorModifier in as separate parameters to the override method
                matcher.RemoveInstruction(); 
                // Keep the existing parameter load for layer depth
                matcher.Advance(1); 
                // Remove the call to ClickableTextureComponent.draw and the default loads for its 3 optional parameters 
                // *Unknown where the final parameter, yOffset, is defined, but the il instructions include a load for it
                matcher.RemoveInstructions(4); 
                // Insert a call to the custom draw method
                matcher.InsertAndAdvance( 
                    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ParseCustomFields), "DrawCustomColorForCraftingPage", new Type[] {typeof(CraftingPage), typeof(ClickableTextureComponent), typeof(SpriteBatch), typeof(Color), typeof(float), typeof(float)}))
                );

                // Patch the draw function for craftable recipes
                // Find the next call to ClickableTextureComponent.draw(SpriteBatch b)
                matcher.MatchStartForward(
                    new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(ClickableTextureComponent), nameof(ClickableTextureComponent.draw), new Type[] {typeof(SpriteBatch)}))
                ).ThrowIfNotMatch("Could not find proper entry point for craftable recipes for draw_Transpiler in CraftingPage");

                // Move to start of parameter loading for call to ClickableTextureComponent.draw
                matcher.Advance(-2); 
                // Replace the original ldloc.2 with the load for CraftingPage __instance, then readd ldloc.2 after 
                // This is done instead of inserting the load for CraftingPage __instance before ldloc.2 because the original instruction for ldloc.2 has a label.
                // Using set preserves the label and insures CraftingPage __instance is loading in the correct place (as a parameter for the custom draw function).
                // Labels identify locations in the instructions the code jumps to depending on the evaluation of certain conditions (ex. if statements),
                matcher.SetAndAdvance(OpCodes.Ldarg_0, null); 
                matcher.InsertAndAdvance(
                    new CodeInstruction(OpCodes.Ldloc_2) 
                );
                // Return to call to ClickableTextureComponent.draw(SpriteBatch b) found in the MatchStartForward
                matcher.Advance(1); 
                // Remove the call to ClickableTextureComponent.draw
                matcher.RemoveInstruction(); 
                // Insert a call to the custom draw method
                matcher.InsertAndAdvance( 
                    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ParseCustomFields), "DrawCustomColorForCraftingPage", new Type[] {typeof(CraftingPage), typeof(ClickableTextureComponent), typeof(SpriteBatch)}))
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