using System;
using System.Linq;
using System.Collections.Generic;
using Harmony;
using CIL = Harmony.CodeInstruction;
using StardewModdingAPI;
using StardewValley;
using System.Reflection.Emit;
using System.Reflection;

namespace NPCJasperFix
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        internal static ModEntry Instance { get; private set; }
        internal HarmonyInstance Harmony { get; private set; }
        public static IMonitor ModMonitor { get; private set; }

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            // Make resources available.
            Instance = this;
            ModMonitor = this.Monitor;
            Harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);

            // Apply the patch to stop Jas from going silent
            Harmony.Patch(
                original: helper.Reflection.GetMethod(new NPC(), "loadCurrentDialogue").MethodInfo,
                transpiler: new HarmonyMethod(AccessTools.Method(typeof(NPCPatch), nameof(NPCPatch.spouseContainsEquals_Transpiler)))
            );
            
            // Apply the related patch to jealousy code
            Harmony.Patch(
                original: helper.Reflection.GetMethod(new NPC(), "tryToReceiveActiveObject").MethodInfo,
                transpiler: new HarmonyMethod(AccessTools.Method(typeof(NPCPatch), nameof(NPCPatch.spouseContainsEquals_Transpiler)))
            );
        }

        /*********
        ** Harmony patches
        *********/
        /// <summary>Contains patches for patching game code in the StardewValley.NPC class.</summary>
        internal class NPCPatch
        {
            /// <summary>Changes the spouse.Contains check to use spouse.Equals instead.</summary>
            public static IEnumerable<CIL> spouseContainsEquals_Transpiler(IEnumerable<CIL> instructions, MethodBase original)
            {
                try
                {
                    var codes = new List<CodeInstruction>(instructions);
                    int patchCount = 0;

                    for (int i = 0; i < codes.Count - 4; i++)
                    {
                        // This is the snippet of code we want to find and change:
                        //     OLD Game1.player.spouse.Contains(this.Name) or who.spouse.Contains(this.Name) (who is Farmer)
                        //     NEW Game1.player.spouse.Equals(this.Name) or who.spouse.Equals(this.Name)
                        // It can be done with a single opcode call change from string.Contains to string.Equals

                        if (//callvirt instance string StardewValley.Farmer::get_spouse()
                            codes[i].opcode == OpCodes.Callvirt &&
                            (MethodInfo)codes[i].operand == typeof(Farmer).GetProperty("spouse").GetGetMethod() &&
                            //ldarg.0
                            codes[i + 1].opcode == OpCodes.Ldarg_0 &&
                            //call instance string StardewValley.Character::get_Name()
                            codes[i + 2].opcode == OpCodes.Call &&
                            (MethodInfo)codes[i + 2].operand == typeof(Character).GetProperty("Name").GetGetMethod() &&
                            //callvirt instance bool[mscorlib] System.String::Contains(string)
                            codes[i + 3].opcode == OpCodes.Callvirt &&
                            (MethodInfo)codes[i + 3].operand == typeof(string).GetMethod("Contains", new Type[] { typeof(string) }) )
                        {
                            // Insert the new replacement instruction
                            codes[i + 3] = new CIL(OpCodes.Callvirt, typeof(string).GetMethod("Equals", new Type[] { typeof(string) }) );
                            patchCount += 1;
                        }
                    }
                    // Log results of the patch attempt
                    if (patchCount == 0)
                    {
                        ModMonitor.LogOnce($"Couldn't find a code location to apply {nameof(spouseContainsEquals_Transpiler)} patch to {original.DeclaringType.Name}.{original.Name}\n" +
                            $"This is probably fine. Maybe a game update or another harmony mod already patched it?", LogLevel.Info);
                    }
                    else //patched at least once
                    {
                        //do stuff
                        ModMonitor.LogOnce($"Applied bugfix patch to {original.DeclaringType.Name}.{original.Name}: {nameof(spouseContainsEquals_Transpiler)}", LogLevel.Trace);

                        if (patchCount > 1)
                        {
                            ModMonitor.LogOnce($"Found an unusual number of patch locations for {nameof(spouseContainsEquals_Transpiler)} in {original.DeclaringType.Name}.{original.Name}\n" +
                            $"This might cause unexpected behaviour. Please share your SMAPI log with the creator of this mod.", LogLevel.Warn);
                        }
                    }

                    return codes.AsEnumerable();
                }
                catch (Exception ex)
                {
                    ModMonitor.Log($"Failed in {nameof(spouseContainsEquals_Transpiler)}:\n{ex}", LogLevel.Error);
                    return instructions; // use original code
                }
            }
        }
    }
}