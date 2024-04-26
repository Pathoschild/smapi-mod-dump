/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewTravelSkill
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using AchtuurCore.Patches;
using HarmonyLib;
using StardewValley;
using SObject = StardewValley.Object;


namespace FasterMiniObelisk.Patches;
internal class ObeliskWarpPatch : GenericPatcher
{
    public override void Patch(Harmony harmony)
    {
        harmony.Patch(
            original: GetOriginalMethod<SObject>(nameof(SObject.checkForAction)),
            transpiler: GetHarmonyMethod(nameof(this.transpile))
        );
    }

    /// <summary>
    /// Transpiler for <see cref="Game1.checkForAction"/>. 
    /// 
    /// Replaces part where farmer is paused and screen fades to black with a call to a method
    /// </summary>
    /// <param name="instructions"></param>
    /// <returns></returns>
    private static IEnumerable<CodeInstruction> transpile(IEnumerable<CodeInstruction> instructions, ILGenerator gen)
    {
        Label jumpToLabel = gen.DefineLabel();
        CodeMatcher matcher = new CodeMatcher(instructions);

        List<CodeInstruction> loadTargetInstr = matcher
            .Start()
            .MatchStartForward(
                new CodeMatch(i => i.opcode == OpCodes.Ldloc_S),
                new CodeMatch(i => i.opcode == OpCodes.Ldloca_S),
                new CodeMatch(i => i.opcode == OpCodes.Call)
            )
            .InstructionsInRange(matcher.Pos+1, matcher.Pos + 2);

        // Add branch label
        matcher.MatchStartForward(
                new CodeMatch(i => i.opcode == OpCodes.Ldloca || i.opcode == OpCodes.Ldloca_S),
                new CodeMatch(i => i.opcode == OpCodes.Ldloc || i.opcode == OpCodes.Ldloc_S)
            )
            .AddLabels(new List<Label> { jumpToLabel });


        return matcher
            .Start() // go back to start since this part comes before the branch dest
            .MatchStartForward(
                new CodeMatch(i => i.opcode == OpCodes.Callvirt),
                new CodeMatch(i => i.opcode == OpCodes.Ldc_I4_0),
                new CodeMatch(i => i.opcode == OpCodes.Stsfld)
            )
            .Advance(1)
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_1)) //load farmer
            .InsertAndAdvance(loadTargetInstr)
            .Insert(
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ModEntry), nameof(ModEntry.DoMiniObeliskWarp))),
                new CodeInstruction(OpCodes.Br, jumpToLabel)
            )
            .InstructionEnumeration();
    }
}
