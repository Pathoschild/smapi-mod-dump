/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewMods
**
*************************************************/

using AchtuurCore.Patches;
using HarmonyLib;
using StardewValley.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace JunimoBeacon.Patches;
internal class MaxJunimoCountPatcher : GenericPatcher
{

    private static MethodInfo Method_GetMaxJunimo = AccessTools.Method(
            typeof(MaxJunimoCountPatcher),
            nameof(GetMaxJunimos),
            new Type[] { typeof(JunimoHut) }
        );

    public override void Patch(Harmony harmony)
    {

        harmony.Patch(
            original: GetOriginalMethod<JunimoHut>(nameof(JunimoHut.getUnusedJunimoNumber)),
            transpiler: GetHarmonyMethod(nameof(transpile_GetUnusedJunimoNumber))
        );

        harmony.Patch(
            original: GetOriginalMethod<JunimoHut>(nameof(JunimoHut.updateWhenFarmNotCurrentLocation)),
            transpiler: GetHarmonyMethod(nameof(transpile_updateWhenFarmNotCurrentLocation))
        );
    }


    public static IEnumerable<CodeInstruction> transpile_GetUnusedJunimoNumber(IEnumerable<CodeInstruction> instructions)
    {
        /// In getUnusedJunimoNumber the following should be done:
        /// 1. for (int i = 0; i < 3; i++) -> for (int i = 0 i < maxJunimo; i++) (line 132)
        /// 2. return 2 -> return maxJunimo - 1; (line 155)
        /// 

        return new CodeMatcher(instructions)
            // For loop range check part
            .MatchStartForward(
                new CodeMatch(i => i.opcode == OpCodes.Ldloc_0),
                new CodeMatch(i => i.opcode == OpCodes.Ldc_I4_3),
                new CodeMatch(i => i.opcode == OpCodes.Blt_S)
            )
            // Replace Ldc_I4_3 function call to GetMaxJunimos
            .Advance(1) // Don't replace LdLoc_0
            .SetAndAdvance(OpCodes.Ldarg_0, null) // Replaces Ldc_I4_3
            .Insert(new CodeInstruction(OpCodes.Call, Method_GetMaxJunimo))
            
            // 'Return 2' part
            .MatchStartForward(
                new CodeMatch(i => i.opcode == OpCodes.Ldc_I4_2),
                new CodeMatch(i => i.opcode == OpCodes.Ret)
            )
            .SetAndAdvance(OpCodes.Ldarg_0, null) // replaces 'Ldc_I4_2'
            .Insert(new[] {
                new CodeInstruction(OpCodes.Call, Method_GetMaxJunimo),
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Sub)
            })
            .InstructionEnumeration();
    }

    public static IEnumerable<CodeInstruction> transpile_updateWhenFarmNotCurrentLocation(IEnumerable<CodeInstruction> instructions)
    {
        return new CodeMatcher(instructions)
            .MatchStartForward(
                new CodeMatch(i => i.opcode == OpCodes.Callvirt),
                new CodeMatch(i => i.opcode == OpCodes.Ldc_I4_3),
                new CodeMatch(i => i.opcode == OpCodes.Bge || i.opcode == OpCodes.Bge_S)
            )
            .Advance(1) // now points to 'Ldc_I4_3'
            .SetAndAdvance(OpCodes.Ldarg_0, null)
            .Insert(new CodeInstruction(OpCodes.Call, Method_GetMaxJunimo))
            .InstructionEnumeration();
    }

    public static IEnumerable<CodeInstruction> transpile_performTenMinuteAction(IEnumerable<CodeInstruction> instructions)
    {
        // this just happens to be the exact same as updateWhenFarmNotCurrentLocation :)
        return new CodeMatcher(instructions)
            .MatchStartForward(
                new CodeMatch(i => i.opcode == OpCodes.Callvirt),
                new CodeMatch(i => i.opcode == OpCodes.Ldc_I4_3),
                new CodeMatch(i => i.opcode == OpCodes.Bge || i.opcode == OpCodes.Bge_S)
            )
            .Advance(1) // now points to 'Ldc_I4_3'
            .SetAndAdvance(OpCodes.Ldarg_0, null)
            .Insert(new CodeInstruction(OpCodes.Call, Method_GetMaxJunimo))
            .InstructionEnumeration();
    }


    private static int GetMaxJunimos(JunimoHut hut)
    {
        JunimoGroup group = ModEntry.Instance.GetHutGroup(hut);

        if (group is null)
            return 3;

        return 3 + group.NumConnectedBeacons * ModEntry.Instance.Config.ExtraJunimoPerBeacon;
    }
}
