/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Jolly-Alpaca/PrismaticDinosaur
**
*************************************************/

using System.Reflection.Emit;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Extensions;
using StardewModdingAPI;

namespace PrismaticDinosaur.Patches
{
    internal class DinoMonsterPatcher
    {
        private static IMonitor Monitor;

        /// <summary>
        /// Apply the harmony patches to DinoMonster.cs
        /// </summary>
        /// <param name="monitor">The Monitor instance for the PrismaticDinosaurMonster module</param>
        /// <param name="harmony">The Harmony instance for the PrismaticDinosaurMonster module</param>
        internal static void Apply(IMonitor monitor, Harmony harmony)
        {
            Monitor = monitor;
            harmony.Patch(
                original: AccessTools.DeclaredMethod(typeof(StardewValley.Monsters.DinoMonster), nameof(StardewValley.Monsters.DinoMonster.getExtraDropItems)),
                prefix: new HarmonyMethod(typeof(DinoMonsterPatcher), nameof(getExtraDropItems_Prefix))
            );
            harmony.Patch(
                original: AccessTools.DeclaredMethod(typeof(StardewValley.Monsters.DinoMonster), nameof(StardewValley.Monsters.DinoMonster.draw)),
                transpiler: new HarmonyMethod(typeof(DinoMonsterPatcher), nameof(DinoMonsterPatcher.draw_Transpiler))
            );
        }

        /// <summary>
        /// Prefix to patch the getExtraDropItems in DinoMonster.cs
        /// Runs different drop logic if the DinoMonster instance is a Prismatic Pepper Rex
        /// </summary>
        /// <remarks>The logic is the same as the default Pepper Rex except the type of dinosaur egg it drops.</remarks>
        /// <param name="__instance">The calling DinoMonster instance</param>
        /// <param name="__result">The list of item drops</param>
        /// <returns>Returns false if the instance is a Prismatic Pepper Rex. Default: true</returns>
        internal static bool getExtraDropItems_Prefix(DinoMonster __instance, ref List<Item> __result)
        {
            try
            {
                if (__instance.Name == "JollyLlama.PrismaticDinosaur.Prismatic Pepper Rex")
                {
                    List<Item> extra_items = new List<Item>();
                    if (Game1.random.NextDouble() < 0.10000000149011612)
                    {
                        // Drop the prismatic dinosaur egg instead of the regular dinosaur egg
                        extra_items.Add(ItemRegistry.Create("(O)JollyLlama.PrismaticDinosaur.PrismaticDinosaurEgg"));
                    }
                    else
                    {
                        // Logic the same as vanilla
                        List<Item> non_egg_items = new List<Item>();
                        non_egg_items.Add(ItemRegistry.Create("(O)580"));
                        non_egg_items.Add(ItemRegistry.Create("(O)583"));
                        non_egg_items.Add(ItemRegistry.Create("(O)584"));
                        extra_items.Add(Game1.random.ChooseFrom(non_egg_items));
                    }
                    __result = extra_items;
                    return false;
                }  
                return true;
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(getExtraDropItems_Prefix)}:\n{ex}", LogLevel.Error);
                return true;
            }
        }

        /// <summary>
        /// Transpiler instructions to patch the draw method in DinoMonster.cs 
        /// Overwrites the color parameter passed to SpriteBatch.draw
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
                // Find the call to Color.White
                matcher.MatchStartForward(
                    new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(Color), "get_White"))
                ).ThrowIfNotMatch("Could not find proper entry point for draw_Transpiler in DinoMonster");
                
                // Load the DinoMonster instance as the parameter for the custom get color method
                matcher.InsertAndAdvance(
                    new CodeInstruction(OpCodes.Ldarg_0)
                );
                 // Replace the call to Color.White with a call to the custom get color method
                matcher.Set(OpCodes.Call, AccessTools.Method(typeof(DinoMonsterPatcher), "GetPrismaticColorforPrismaticDinoMonster"));
                return matcher.InstructionEnumeration();
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(draw_Transpiler)}:\n{ex}", LogLevel.Error);
                return code;
            }
        }

        /// <summary>
        /// Get the next MonoGame color in the prismatic effect for Prismatic Pepper Rex
        /// </summary>
        /// <param name="__instance">The calling DinoMonster instance</param>
        /// <returns>The next color in the prismatic effect. Default: Color.White</returns>
        internal static Color GetPrismaticColorforPrismaticDinoMonster(DinoMonster __instance)
        {
            if (__instance.Name == "JollyLlama.PrismaticDinosaur.Prismatic Pepper Rex") return Utility.GetPrismaticColor();
            return Color.White;
        }
    }
}