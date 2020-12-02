/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AngelaRanna/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using Harmony;
using StardewModdingAPI;
using System.Reflection.Emit;
using System.Linq;

namespace LeadRodFixer
{
    public class LeadRodFixer : Mod
    {
        public override void Entry(IModHelper helper)
        {
            // Make sure to get the monitor set up for debugging prints
            GetSpecialItemTranspilerPatch.Initialize(this.Monitor);

            var harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);

            // Transpiler patch to fix the level drop range of lead rods
            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Locations.MineShaft), nameof(StardewValley.Locations.MineShaft.getSpecialItemForThisMineLevel)),
                transpiler: new HarmonyMethod(typeof(GetSpecialItemTranspilerPatch), nameof(GetSpecialItemTranspilerPatch.getSpecialItem_transpiler))
                );
        }
    }

    public class GetSpecialItemTranspilerPatch
    {
        private static IMonitor Monitor;

        public static void Initialize(IMonitor monitor)
        {
            Monitor = monitor;
        }

        public static IEnumerable<CodeInstruction> getSpecialItem_transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            bool foundLevelCode = false;
            bool foundDropTableCode = false;

            for (int i = 0; i < codes.Count; i++)
            {
                /*
                 * The broken code for the lead rod drops looks like this:
                 *     IL_0174: ldarg.0      // level
                 *     IL_0175: ldc.i4       160 // 0x000000a0
                 *     IL_017a: bge.s        IL_01ef
                 *     
                 * Then check for one more line after that, the randomizer:
                 *     IL_017c: ldloc.0      // random
                 *     IL_017d: ldc.i4.7
                 *     IL_017e: callvirt     instance int32 [mscorlib]System.Random::Next(int32)
                 *     IL_0183: stloc.1      // V_1
                */
                if (!foundLevelCode && codes[i].opcode == OpCodes.Ldarg_0)
                {
                    if (codes[i + 1].opcode == OpCodes.Ldc_I4 && ((System.Int32)(codes[i + 1].operand)) == 160)
                    {
                        if (codes[i + 2].opcode == OpCodes.Bge
                            && codes[i + 3].opcode == OpCodes.Ldloc_0
                            && codes[i + 4].opcode == OpCodes.Ldc_I4_7
                            && codes[i + 5].opcode == OpCodes.Callvirt)
                        {
                            codes[i + 1].operand = (System.Int32)80;
                            foundLevelCode = true;
                            Monitor.Log($"Found lead rod level code to replace.", LogLevel.Info);
                        }
                    }
                }

                /*
                 * The code for the lead rod drop table looks like this:
                 *       IL_01ab: ldc.i4.s     26 // 0x1a
                 *       IL_01ad: newobj       instance void StardewValley.Tools.MeleeWeapon::.ctor(int32)
                 *       IL_01b2: ret
                 *
                 *       // [7622 13 - 7622 47]
                 *       IL_01b3: ldc.i4.s     26 // 0x1a
                 *       IL_01b5: newobj       instance void StardewValley.Tools.MeleeWeapon::.ctor(int32)
                 *       IL_01ba: ret
                 *
                 *       // [7624 13 - 7624 42]
                 *       IL_01bb: ldc.i4       508 // 0x000001fc
                 *       IL_01c0: newobj       instance void StardewValley.Objects.Boots::.ctor(int32)
                 *       IL_01c5: ret
                 *
                 *       // [7626 13 - 7626 42]
                 *       IL_01c6: ldc.i4       510 // 0x000001fe
                 *       IL_01cb: newobj       instance void StardewValley.Objects.Boots::.ctor(int32)
                 *       IL_01d0: ret
                 *
                 *       // [7628 13 - 7628 41]
                 *       IL_01d1: ldc.i4       517 // 0x00000205
                 *       IL_01d6: newobj       instance void StardewValley.Objects.Ring::.ctor(int32)
                 *       IL_01db: ret
                 *
                 *       // [7630 13 - 7630 41]
                 *       IL_01dc: ldc.i4       519 // 0x00000207
                 *       IL_01e1: newobj       instance void StardewValley.Objects.Ring::.ctor(int32)
                 *       IL_01e6: ret
                 *
                 *       // [7632 13 - 7632 47]
                 *       IL_01e7: ldc.i4.s     26 // 0x1a
                 *       IL_01e9: newobj       instance void StardewValley.Tools.MeleeWeapon::.ctor(int32)
                 *       IL_01ee: ret
                 */
                if (!foundDropTableCode && codes[i].opcode == OpCodes.Ldc_I4_S && ((sbyte)(codes[i].operand)) == 26)
                {
                    if (codes[i + 1].opcode == OpCodes.Newobj)
                    {
                        //Monitor.Log($"OPCODES: {codes[i].opcode.ToString()} : {codes[i].operand.ToString()}\n{codes[i + 1].opcode.ToString()}\n{codes[i + 2].opcode.ToString()}\n{codes[i + 3].opcode.ToString()} : {codes[i + 3].operand.ToString()}\n{codes[i + 4].opcode.ToString()}\n{codes[i + 5].opcode.ToString()}\n", LogLevel.Debug);
                        if (codes[i + 2].opcode == OpCodes.Ret
                            && codes[i + 3].opcode == OpCodes.Ldc_I4_S && ((sbyte)(codes[i + 3].operand)) == 26
                            && codes[i + 4].opcode == OpCodes.Newobj
                            && codes[i + 5].opcode == OpCodes.Ret)
                        {
                            codes[i].operand = (System.Int32)3;
                            codes[i + 3].operand = (System.Int32)3;
                            foundDropTableCode = true;
                            Monitor.Log($"Found lead rod drop table code to replace.", LogLevel.Info);
                        }
                    }
                }

                if (foundLevelCode && foundDropTableCode) break;
            }
            if (!foundLevelCode) Monitor.Log($"DID NOT FIND lead rod code to replace.\n", LogLevel.Error);
            if (!foundDropTableCode) Monitor.Log($"DID NOT FIND lead rod drop table code to replace.\n", LogLevel.Error);
            return codes.AsEnumerable();
        }
    }
}
