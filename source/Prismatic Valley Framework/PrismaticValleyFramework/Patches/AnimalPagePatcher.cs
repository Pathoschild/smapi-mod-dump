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
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;

namespace PrismaticValleyFramework.Patches
{
    internal class AnimalPagePatcher
    {
        private static IMonitor Monitor;

        /// <summary>
        /// Apply the harmony patches to AnimalPage.cs
        /// </summary>
        /// <param name="monitor">The Monitor instance for the PrismaticValleyFramework module</param>
        /// <param name="harmony">The Harmony instance for the PrismaticvalleyFramework module</param>
        internal static void Apply(IMonitor monitor, Harmony harmony)
        {
            Monitor = monitor;
            harmony.Patch(
                // Method name passed as string to access/patch private methods (nameof cannot be used for private methods)
                original: AccessTools.Method(typeof(AnimalPage), "drawNPCSlot"),
                transpiler: new HarmonyMethod(typeof(AnimalPagePatcher), nameof(drawNPCSlot_Transpiler))
            );
        }

        /// <summary>
        /// Transpiler instructions to patch the drawNPCSlot method in AnimalPage.cs. 
        /// Overwrites the call to ClickableTextureComponent.draw(SpriteBatch) to call a custom method instead.
        /// </summary>
        /// <param name="instructions">The IL instructions</param>
        /// <param name="il">The IL generator</param>
        /// <returns>The patched IL instructions</returns>
        internal static IEnumerable<CodeInstruction> drawNPCSlot_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            var code = instructions.ToList();
            try
            {
                var matcher = new CodeMatcher(code, il);
                // Find the call to ClickableTextureComponents.draw(SpriteBatch)
                matcher.MatchStartForward(
                    new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(ClickableTextureComponent), nameof(ClickableTextureComponent.draw), new Type[] {typeof(SpriteBatch)}))
                ).ThrowIfNotMatch("Could not find proper entry point for drawNPCSlot_Transpiler");
                
                // Remove the call to ClickableTextureComponent and the load for its parameter (Ldarg_1: SpriteBatch b)
                matcher.Advance(-1);
                matcher.RemoveInstructions(2);
                // Insert a call to the custom draw method and load values for its last two parameters (ClickableTextureComponent is already loaded)
                matcher.InsertAndAdvance(
                    new CodeInstruction(OpCodes.Ldloc_0), // Load the AnimalPage instance
                    new CodeInstruction(OpCodes.Ldarg_1), // Reload the SpriteBatch
                    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ParseCustomFields), nameof(ParseCustomFields.DrawCustomColorForAnimalEntry)))
                );
                return matcher.InstructionEnumeration();
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(drawNPCSlot_Transpiler)}:\n{ex}", LogLevel.Error);
                return code;
            }
        }
    }
}