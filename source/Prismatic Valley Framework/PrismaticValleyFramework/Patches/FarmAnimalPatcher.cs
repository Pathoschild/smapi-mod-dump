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
    internal class FarmAnimalPatcher
    {
        private static IMonitor Monitor;

        /// <summary>
        /// Apply the harmony patches to FarmAnimal.cs
        /// </summary>
        /// <param name="monitor">The Monitor instance for the PrismaticValleyFramework module</param>
        /// <param name="harmony">The Harmony instance for the PrismaticvalleyFramework module</param>
        internal static void Apply(IMonitor monitor, Harmony harmony)
        {
            Monitor = monitor;
            harmony.Patch(
                // Use DeclaredMethod as FarmAnimal.cs has only one implementation of the draw method
                original: AccessTools.DeclaredMethod(typeof(StardewValley.FarmAnimal), nameof(StardewValley.FarmAnimal.draw)),
                transpiler: new HarmonyMethod(typeof(FarmAnimalPatcher), nameof(draw_Transpiler))
            );
        }

        /// <summary>
        /// Transpiler instructions to patch the draw method in FarmAnimal.cs. 
        /// Overwrites the call to Color.White in the call to AnimatedSprite.draw to call a custom method instead.
        /// </summary>
        /// <param name="instructions">The IL instructions</param>
        /// <param name="il">The IL generator</param>
        /// <returns>The patched IL instructions</returns>
        internal static IEnumerable<CodeInstruction> draw_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            var code = instructions.ToList();
            try
            {
                // Find where Color.White is loaded
                var matcher = new CodeMatcher(code, il);
                matcher.MatchStartForward(
                    new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(Color), "get_White"))
                ).ThrowIfNotMatch("Could not find proper entry point for draw_Transpiler");
                
                // Load the FarmAnimalData as the parameter for the custom get color method
                matcher.InsertAndAdvance(
                    new CodeInstruction(OpCodes.Ldloc_2)
                );
                // Replace the call to Color.White with a call to the custom get color method
                matcher.Set(OpCodes.Call, AccessTools.Method(typeof(ParseCustomFields), nameof(ParseCustomFields.getCustomColorFromFarmAnimalData)));
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