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
using StardewValley.ItemTypeDefinitions;
using StardewModdingAPI;
using PrismaticValleyFramework.Framework;

namespace PrismaticValleyFramework.Patches
{
    internal class FurniturePatcher
    {
        private static IMonitor Monitor;

        /// <summary>
        /// Apply the harmony patches to Furniture.cs
        /// </summary>
        /// <param name="monitor">The Monitor instance for the PrismaticValleyFramework module</param>
        /// <param name="harmony">The Harmony instance for the PrismaticvalleyFramework module</param>
        internal static void Apply(IMonitor monitor, Harmony harmony)
        {
            Monitor = monitor;
            harmony.Patch(
                // Use DeclaredMethod as Furniture.cs has only one implementation of the draw method
                original: AccessTools.DeclaredMethod(typeof(StardewValley.Objects.Furniture), nameof(StardewValley.Objects.Furniture.draw), new Type[] {typeof(SpriteBatch), typeof(int), typeof(int), typeof(float)}),
                transpiler: new HarmonyMethod(typeof(FurniturePatcher), nameof(draw_Transpiler))
            );
        }

        /// <summary>
        /// Transpiler instructions to patch the draw method in Furniture.cs. 
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
                var matcher = new CodeMatcher(code, il);
                // Only modifies the draw for held object
                // Find the start of the draw method for held object
                matcher.MatchStartForward(
                    new CodeMatch(OpCodes.Ldarg_1), // Load SpriteBatch
                    new CodeMatch(OpCodes.Ldloc_S), // Load local variable 12: heldItemData (dataOrErrorItem2)
                    new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(ParsedItemData), nameof(ParsedItemData.GetTexture))) // Call the method ParsedItemData.GetTexture
                ).ThrowIfNotMatch("Could not find proper entry point for draw_Transpiler in Furniture");
                // Advance to the call to Color.White in the draw method for held object
                matcher.MatchStartForward(
                    new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(Color), "get_White"))
                ).ThrowIfNotMatch("Could not find proper entry point for Color.White in draw_Transpiler in Furniture");
            
                // Load the ParsedItemData as the parameter for the custom get color method
                matcher.InsertAndAdvance(
                    new CodeInstruction(OpCodes.Ldloc_S, 12)
                );
                // Replace the call to Color.White with a call to the custom get color method
                matcher.Set(OpCodes.Call, AccessTools.Method(typeof(ParseCustomFields), nameof(ParseCustomFields.getCustomColorFromParsedItemData)));
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