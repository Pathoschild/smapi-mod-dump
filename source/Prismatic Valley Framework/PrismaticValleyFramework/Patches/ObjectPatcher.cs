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
    internal class ObjectPatcher
    {
        private static IMonitor Monitor;

        /// <summary>
        /// Apply the harmony patches to Object.cs
        /// </summary>
        /// <param name="monitor">The Monitor instance for the PrismaticValleyFramework module</param>
        /// <param name="harmony">The Harmony instance for the PrismaticvalleyFramework module</param>
        internal static void Apply(IMonitor monitor, Harmony harmony)
        {
            Monitor = monitor;
            harmony.Patch(
                // Use DeclaredMethod as Object.cs has only one implementation of the drawWhenHeld method
                original: AccessTools.DeclaredMethod(typeof(StardewValley.Object), nameof(StardewValley.Object.drawWhenHeld)),
                transpiler: new HarmonyMethod(typeof(ObjectPatcher), nameof(drawWhenHeld_Transpiler))
            );
            harmony.Patch(
                original: AccessTools.DeclaredMethod(typeof(StardewValley.Object), nameof(StardewValley.Object.drawInMenu)),
                transpiler: new HarmonyMethod(typeof(ObjectPatcher), nameof(drawInMenu_Transpiler))
            );
            harmony.Patch(
                original: AccessTools.DeclaredMethod(typeof(StardewValley.Object), nameof(StardewValley.Object.drawAsProp)),
                transpiler: new HarmonyMethod(typeof(ObjectPatcher), nameof(drawAsProp_Transpiler))
            );
            harmony.Patch(
                // Include list of parameters to target specific overload - Avoids the ambigous method call error
                original: AccessTools.Method(typeof(StardewValley.Object), nameof(StardewValley.Object.draw), new Type[] {typeof(SpriteBatch), typeof(int), typeof(int), typeof(float)}),
                transpiler: new HarmonyMethod(typeof(ObjectPatcher), nameof(draw_Transpiler))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Object), nameof(StardewValley.Object.draw), new Type[] {typeof(SpriteBatch), typeof(int), typeof(int), typeof(float), typeof(float)}),
                transpiler: new HarmonyMethod(typeof(ObjectPatcher), nameof(draw_Transpiler2))
            );
        }

        /// <summary>
        /// Transpiler instructions to patch the drawWhenHeld method in Object.cs. 
        /// Overwrites the call to Color.White in the call to SpriteBatch.draw to call a custom method instead.
        /// </summary>
        /// <param name="instructions">The IL instructions</param>
        /// <param name="il">The IL generator</param>
        /// <returns>The patched IL instructions</returns>
        internal static IEnumerable<CodeInstruction> drawWhenHeld_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            var code = instructions.ToList();
            try
            {
                var matcher = new CodeMatcher(code, il);
                // Find the call to Color.White
                matcher.MatchStartForward(
                    new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(Color), "get_White"))
                ).ThrowIfNotMatch("Could not find proper entry point for drawWhenHeld_Transpiler");
                
                // Load the ParsedItemData (dataOrErrorItem) as the parameter for the custom get color method
                matcher.InsertAndAdvance(
                    new CodeInstruction(OpCodes.Ldloc_0)
                );
                // Replace the call to Color.White with a call to the custom get color method
                matcher.Set(OpCodes.Call, AccessTools.Method(typeof(ParseCustomFields), nameof(ParseCustomFields.getCustomColorFromParsedItemData)));
                return matcher.InstructionEnumeration();
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(drawWhenHeld_Transpiler)}:\n{ex}", LogLevel.Error);
                return code;
            }
        }

        /// <summary>
        /// Transpiler instructions to patch the drawInMenu method in Object.cs. 
        /// Overwrites the call to Color.White in the call to SpriteBatch.draw to call a custom method instead.
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
                ).ThrowIfNotMatch("Could not find proper entry point for drawInMenu_Transpiler");
                
                // Replace the color variable (as applicable) before it is multiplied by transparency
                // Load the ParsedItemData (dataOrErrorItem) as the parameter for the custom get color method
                matcher.InsertAndAdvance(
                    new CodeInstruction(OpCodes.Ldloc_0)
                );
                // Advance over the load of the color arg to include it as a parameter for the custom get color method
                matcher.Advance(1);
                // Insert a call to the custom get color method with color check (only overrides where color arg is Color.White)
                matcher.Insert(
                    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ParseCustomFields), nameof(ParseCustomFields.getCustomColorFromParsedItemDataWithColor), new Type[] {typeof(ParsedItemData), typeof(Color)}))
                );
                return matcher.InstructionEnumeration();
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(drawInMenu_Transpiler)}:\n{ex}", LogLevel.Error);
                return code;
            }
        }

        /// <summary>
        /// Transpiler instructions to patch the drawAsProp method in Object.cs. 
        /// Overwrites the call to Color.White in the first and fourth calls to SpriteBatch.draw to call a custom method instead.
        /// </summary>
        /// <remarks><para>Patch untested: Do not have a test case as this is only called from event.cs</para>
        /// Review of calls to SpriteBatch.draw with Color.White in drawAsProp
        ///     <list type="bullet">
        ///         <item>
        ///             <term>Modified</term>
        ///             <description>The first call to Color.White is in the the draw for big craftables.</description>
        ///         </item>
        ///         <item>
        ///             <term>Unmodified</term>
        ///             <description>The second call only applies for vanilla looms and is inside the draw for the wheel part with thread from the object spritesheet. No current use case.</description>
        ///         </item>
        ///         <item>
        ///             <term>Unmodified</term>
        ///             <description>The third call is in the draw for shadows of objects (not big craftables) that have one.</description>
        ///         </item>
        ///         <item>
        ///             <term>Modified</term>
        ///             <description>The fourth and final call is in the draw for objects (not big craftables).</description>
        ///         </item>
        ///     </list>
        /// </remarks>
        /// <param name="instructions">The IL instructions</param>
        /// <param name="il">The IL generator</param>
        /// <returns>The patched IL instructions</returns>
        internal static IEnumerable<CodeInstruction> drawAsProp_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            var code = instructions.ToList();
            try
            {
                var matcher = new CodeMatcher(code, il);
                // Find the first call to Color.White
                matcher.MatchStartForward(
                    new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(Color), "get_White"))
                ).ThrowIfNotMatch("Could not find proper entry point for drawAsProp_Transpiler");
                
                // Load the ParsedItemData (dataOrErrorItem) as the parameter for the custom get color method
                matcher.InsertAndAdvance(
                    new CodeInstruction(OpCodes.Ldloc_3)
                );
                // Replace the call to Color.White with a call to the custom get color method
                matcher.Set(OpCodes.Call, AccessTools.Method(typeof(ParseCustomFields), nameof(ParseCustomFields.getCustomColorFromParsedItemData)));
                
                // Find the final call to Color.White
                // Begin searching backwards from the end of the instructions
                matcher.End();
                matcher.MatchStartBackwards(
                    new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(Color), "get_White"))
                ).ThrowIfNotMatch("Could not find proper entry point for drawAsProp_Transpiler backwards");
                
                // Load the ParsedItemData (dataOrErrorItem2) as the parameter for the custom get color method
                matcher.InsertAndAdvance(
                    new CodeInstruction(OpCodes.Ldloc_S, 9)
                );
                // Replace the call to Color.White with a call to the custom get color method
                matcher.Set(OpCodes.Call, AccessTools.Method(typeof(ParseCustomFields), nameof(ParseCustomFields.getCustomColorFromParsedItemData)));
                return matcher.InstructionEnumeration();
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(drawAsProp_Transpiler)}:\n{ex}", LogLevel.Error);
                return code;
            }
        }

        /// <summary>
        /// Transpiler instructions to patch the draw(SpriteBatch, int, int, float) method in Object.cs. 
        /// Overwrites the color parameter passed to SpriteBatch.draw in several instances to call a custom method instead.
        /// </summary>
        /// <remarks>Analysis of calls to SpriteBatch.draw in draw(SpriteBatch, int, int, float)
        ///     <list type="bullet">
        ///         <item>
        ///             <term>Unmodified</term>
        ///             <description>The first draw methods are called for the vanilla Auto Petter object.</description>
        ///         </item>
        ///         <item>
        ///             <term>Modified</term>
        ///             <description>The third draw method is for all big craftables.</description>
        ///         </item>
        ///         <item>
        ///             <term>Unmodified</term>
        ///             <description>The fourth draw method is for the vanilla loom to draw the wheel (see drawAsProp_Transpiler).</description>
        ///         </item>
        ///         <item>
        ///             <term>Unmodified</term>
        ///             <description>The fifth draw method is for BC marked as lamps and does not use the BC texture.</description>
        ///         </item>
        ///         <item>
        ///             <term>Unmodified</term>
        ///             <description>The sixth draw method draws hats (presumably scarecrows). This may need to be modified to support static color overrides, but does natively support prismatic.</description>
        ///         </item>
        ///         <item>
        ///             <term>Unmodified</term>
        ///             <description>The next three draw methods do not draw the object texture.</description>
        ///         </item>
        ///         <item>
        ///             <term>Modified</term>
        ///             <description>The tenth draw method is for all objects (not big craftables).</description>
        ///         </item>
        ///         <item>
        ///             <term>Modified</term>
        ///             <description>The eleventh draw method is for sprinkler attachments.</description>
        ///         </item>
        ///         <item>
        ///             <term>Unmodified</term>
        ///             <description>The thirteenth draw method does not draw an object texture.</description>
        ///         </item>
        ///         <item>
        ///             <term>Modified</term>
        ///             <description>The fourteenth draw method is for held objects within the object.</description>
        ///         </item>
        ///     </list>
        /// </remarks>
        /// <param name="instructions">The IL instructions</param>
        /// <param name="il">The IL generator</param>
        /// <returns>The patched IL instructions</returns>
        internal static IEnumerable<CodeInstruction> draw_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            var code = instructions.ToList();
            try
            {
                var matcher = new CodeMatcher(code, il);
                // Find the start of the draw method for big craftables
                matcher.MatchStartForward(
                    new CodeMatch(OpCodes.Ldarg_1), // Load SpriteBatch
                    new CodeMatch(OpCodes.Ldloc_S), // Load local variable 7: dataOrErrorItem
                    new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(ParsedItemData), nameof(ParsedItemData.GetTexture))) // Call virtual method ParsedItemData.GetTexture
                ).ThrowIfNotMatch("Could not find proper entry point for big craftables in draw_Transpiler in Object");
                // Advance from match to the call to Color.White in the draw method for big craftables
                matcher.MatchStartForward(
                    new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(Color), "get_White"))
                ).ThrowIfNotMatch("Could not find proper entry point for Color.White for big craftables in draw_Transpiler in Object");
                
                // Load the ParsedItemData (dataOrErrorItem) as the parameter for the custom get color method
                matcher.InsertAndAdvance(
                    new CodeInstruction(OpCodes.Ldloc_S, 7)
                );
                // Replace the call to Color.White with a call to the custom get color method
                matcher.Set(OpCodes.Call, AccessTools.Method(typeof(ParseCustomFields), nameof(ParseCustomFields.getCustomColorFromParsedItemData)));
                
                // Find the start of the draw method for non big craftable objects
                matcher.MatchStartForward(
                    new CodeMatch(OpCodes.Ldarg_1), // Load SpriteBatch
                    new CodeMatch(OpCodes.Ldloc_S), // Load local variable 17: dataOrErrorItem3
                    new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(ParsedItemData), nameof(ParsedItemData.GetTexture))) // Call virtual method ParsedItemData.GetTexture
                ).ThrowIfNotMatch("Could not find proper entry point for objects in draw_Transpiler in Object");
                // Advance from match to the call to Color.White in the draw method for non big craftable objects
                matcher.MatchStartForward(
                    new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(Color), "get_White"))
                ).ThrowIfNotMatch("Could not find proper entry point for Color.White for objects in draw_Transpiler in Object");
                
                // Load the ParsedItemData (dataOrErrorItem3) as the parameter for the custom get color method
                matcher.InsertAndAdvance(
                    new CodeInstruction(OpCodes.Ldloc_S, 17)
                );
                // Replace the call to Color.White with a call to the custom get color method
                matcher.Set(OpCodes.Call, AccessTools.Method(typeof(ParseCustomFields), nameof(ParseCustomFields.getCustomColorFromParsedItemData)));

                // Find the start of the draw method for sprinkler attachments
                matcher.MatchStartForward(
                    new CodeMatch(OpCodes.Ldarg_1), // Load SpriteBatch
                    new CodeMatch(OpCodes.Ldloc_S), // Load local variable 21: dataOrErrorItem4
                    new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(ParsedItemData), nameof(ParsedItemData.GetTexture)))
                ).ThrowIfNotMatch("Could not find proper entry point for sprinkler attachments in draw_Transpiler in Object");
                // Advance from match to the call to Color.White in the draw method for sprinkler attachments
                matcher.MatchStartForward(
                    new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(Color), "get_White"))
                ).ThrowIfNotMatch("Could not find proper entry point for Color.White for sprinkler attachments in draw_Transpiler in Object");
                
                // Load the ParsedItemData (dataOrErrorItem4) as the parameter for the custom get color method
                matcher.InsertAndAdvance(
                    new CodeInstruction(OpCodes.Ldloc_S, 21)
                );
                // Replace the call to Color.White with a call to the custom get color method
                matcher.Set(OpCodes.Call, AccessTools.Method(typeof(ParseCustomFields), nameof(ParseCustomFields.getCustomColorFromParsedItemData)));

                // Find the start of the draw method for held objects within the object
                matcher.MatchStartForward(
                    new CodeMatch(OpCodes.Ldarg_1), // Load SpriteBatch
                    new CodeMatch(OpCodes.Ldloc_S) // Load local variable 25: texture3
                ).ThrowIfNotMatch("Could not find proper entry point for held objects within the object in draw_Transpiler in Object");
                // Advance from match to the call to Color.White in the draw method for held objects within the object
                matcher.MatchStartForward(
                    new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(Color), "get_White"))
                ).ThrowIfNotMatch("Could not find proper entry point for Color.White for held objects within the object in draw_Transpiler in Object");
                
                // Load the ParsedItemData (dataOrErrorItem5) as the parameter for the custom get color method
                matcher.InsertAndAdvance(
                    new CodeInstruction(OpCodes.Ldloc_S, 24)
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

        /// <summary>
        /// Transpiler instructions to patch the draw(SpriteBatch, int, int, float, float) method in Object.cs. 
        /// Overwrites the color parameter passed to SpriteBatch.draw in several instances to call a custom method instead.
        /// </summary>
        /// <remarks>Analysis of calls to SpriteBatch.draw in draw(SpriteBatch, int, int, float, float)
        ///     <list type="bullet">
        ///         <item>
        ///             <term>Modified</term>
        ///             <description>The first draw method is for all big craftables.</description>
        ///         </item>
        ///         <item>
        ///             <term>Unmodified</term>
        ///             <description>The second draw method is for the vanilla loom to draw the wheel (see drawAsProp_Transpiler).</description>
        ///         </item>
        ///         <item>
        ///             <term>Unmodified</term>
        ///             <description>The third draw method is for BC marked as lamps and does not use the BC texture.</description>
        ///         </item>
        ///         <item>
        ///             <term>Unmodified</term>
        ///             <description>The fourth draw method is for a shadow.</description>
        ///         </item>
        ///         <item>
        ///             <term>Modified</term>
        ///             <description>The fifth draw method is for all non big craftable objects.</description>
        ///         </item>
        ///     </list>
        /// </remarks>
        /// <param name="instructions">The IL instructions</param>
        /// <param name="il">The IL generator</param>
        /// <returns>The patched IL instructions</returns>
        internal static IEnumerable<CodeInstruction> draw_Transpiler2(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            var code = instructions.ToList();
            try
            {
                var matcher = new CodeMatcher(code, il);
                // Find the first call to Color.White
                matcher.MatchStartForward(
                    new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(Color), "get_White"))
                ).ThrowIfNotMatch("Could not find proper entry point for big craftables in draw_Transpiler2 in Object");
                
                // Load the ParsedItemData (dataOrErrorItem) as the parameter for the custom get color method
                matcher.InsertAndAdvance(
                    new CodeInstruction(OpCodes.Ldloc_S, 4)
                );
                // Replace the call to Color.White with a call to the custom get color method
                matcher.Set(OpCodes.Call, AccessTools.Method(typeof(ParseCustomFields), nameof(ParseCustomFields.getCustomColorFromParsedItemData)));

                // Find the final call to Color.White
                // Begin searching backwards from the end of the instructions
                matcher.End();
                matcher.MatchStartBackwards(
                    new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(Color), "get_White"))
                ).ThrowIfNotMatch("Could not find proper entry point for objects in draw_Transpiler2");
                
                // Load the ParsedItemData (dataOrErrorItem2) as the parameter for the custom get color method
                matcher.InsertAndAdvance(
                    new CodeInstruction(OpCodes.Ldloc_S, 5)
                );
                // Replace the call to Color.White with a call to the custom get color method
                matcher.Set(OpCodes.Call, AccessTools.Method(typeof(ParseCustomFields), nameof(ParseCustomFields.getCustomColorFromParsedItemData)));
                return matcher.InstructionEnumeration();
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(draw_Transpiler2)}:\n{ex}", LogLevel.Error);
                return code;
            }
        }
    }
}