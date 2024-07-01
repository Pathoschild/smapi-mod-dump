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
using StardewModdingAPI;
using PrismaticValleyFramework.Framework;

namespace PrismaticValleyFramework.Patches
{
    internal class BootsPatcher
    {
        private static IMonitor Monitor;

        /// <summary>
        /// Apply the harmony patches to Boots.cs
        /// </summary>
        /// <param name="monitor">The Monitor instance for the PrismaticValleyFramework module</param>
        /// <param name="harmony">The Harmony instance for the PrismaticvalleyFramework module</param>
        internal static void Apply(IMonitor monitor, Harmony harmony)
        {
            Monitor = monitor;
            harmony.Patch(
                // Use DeclaredMethod as Boots.cs has only one implementation of the drawInMenu method
                original: AccessTools.DeclaredMethod(typeof(StardewValley.Objects.Boots), nameof(StardewValley.Objects.Boots.drawInMenu)),
                transpiler: new HarmonyMethod(typeof(BootsPatcher), nameof(drawInMenu_Transpiler))
            );
        }

        /// <summary>
        /// Transpiler instructions to patch the drawInMenu method in Boots.cs. 
        /// Overwrites the color parameter passed to SpriteBatch.draw drawing the texture to call a custom method instead.
        /// </summary>
        /// <param name="instructions">The IL instructions</param>
        /// <param name="il">The IL generator</param>
        /// <returns>The patched IL instructions</returns>
        internal static IEnumerable<CodeInstruction> drawInMenu_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            var code = instructions.ToList();
            try
            {
                // Find color * transparency passed as the color argument in the draw method in drawInMenu 
                var matcher = new CodeMatcher(code, il);
                matcher.MatchStartForward(
                    new CodeMatch(OpCodes.Ldarg_S, null, "color"), // Load color arg for multiply
                    new CodeMatch(OpCodes.Ldarg_S, null, "transparency") // Load transparency for multiply
                ).ThrowIfNotMatch("Could not find proper entry point for drawInMenu_Transpiler in BootsPatcher");
                
                // Replace the color variable (as applicable) before it is multiplied by transparency
                // Load the unqualified itemId as the parameter for the custom get color method
                matcher.InsertAndAdvance(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(StardewValley.Item), "get_ItemId"))
                );
                // Advance over the load of the color arg to include it as a parameter for the custom get color method
                matcher.Advance(1);
                // Insert a call to the custom get color method with color check
                matcher.Insert(
                    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ParseCustomFields), nameof(ParseCustomFields.getCustomColorFromStringDictItemWithColor), new Type[] {typeof(string), typeof(Color)}))
                );
                return matcher.InstructionEnumeration();
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(drawInMenu_Transpiler)}:\n{ex}", LogLevel.Error);
                return code;
            }
        }
    }
}