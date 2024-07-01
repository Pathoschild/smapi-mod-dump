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
using Microsoft.Xna.Framework.Graphics;

namespace PrismaticValleyFramework.Patches
{
    internal class CharacterPatcher
    {
        private static IMonitor Monitor;

        /// <summary>
        /// Apply the harmony patches to Character.cs
        /// </summary>
        /// <param name="monitor">The Monitor instance for the PrismaticValleyFramework module</param>
        /// <param name="harmony">The Harmony instance for the PrismaticvalleyFramework module</param>
        internal static void Apply(IMonitor monitor, Harmony harmony)
        {
            Monitor = monitor;
            harmony.Patch(
                // Character contains multiple implentations of the draw method: parameter type definitions required to target the correct implementation
                original: AccessTools.Method(typeof(StardewValley.Character), nameof(StardewValley.Character.draw), new Type[] {typeof(SpriteBatch), typeof(int), typeof(float)}),
                transpiler: new HarmonyMethod(typeof(CharacterPatcher), nameof(draw_Transpiler))
            );
        }

        /// <summary>
        /// Transpiler instructions to patch the draw(Spritebatch, int, float) method in Character.cs. 
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
                // Find first call to Color.White. This is the tint passed to the AnimatedSprite.draw method.
                matcher.MatchStartForward(
                    // Color.get_White appears in il instructions, but cannot be targeted directly using nameof
                    new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(Color), "get_White")) 
                ).ThrowIfNotMatch("Could not find proper entry point for draw_Transpiler");
                
                // Load the Character instance as the parameter for the custom get color method
                matcher.InsertAndAdvance(
                    new CodeInstruction(OpCodes.Ldarg_0)
                );

                // Replace the call to Color.White with a call to the custom get color method
                matcher.Set(OpCodes.Call, AccessTools.Method(typeof(ParseCustomFields), nameof(ParseCustomFields.getCustomColorFromCharacter)));
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