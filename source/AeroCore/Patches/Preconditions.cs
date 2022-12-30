/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Tlitookilakin/AeroCore
**
*************************************************/

using AeroCore.Utils;
using HarmonyLib;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace AeroCore.Patches
{
    [HarmonyPatch(typeof(GameLocation), nameof(GameLocation.checkEventPrecondition))]
    internal class Preconditions
    {
        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codes, ILGenerator gen)
            => patch.Run(codes, gen);

        private static readonly ILHelper patch = new ILHelper(ModEntry.monitor, "preconditions")
            .SkipTo(new CodeInstruction[]
            {
                new(OpCodes.Ldloc_0),
                new(OpCodes.Ldloc_2),
                new(OpCodes.Ldelem_Ref),
                new(OpCodes.Ldc_I4_0)
            })
            .Add(new CodeInstruction[]
            {
                new(OpCodes.Ldloc_0),
                new(OpCodes.Ldloc_2),
                new(OpCodes.Ldelem_Ref),
                new(OpCodes.Ldc_I4_0),
                new(OpCodes.Callvirt, typeof(string).MethodNamed("get_Chars")),
                new(OpCodes.Ldc_I4_S, (int)(byte)'G')
            })
            .AddJump(OpCodes.Bne_Un_S, "skip")
            .Add(new CodeInstruction[]
            {
                new(OpCodes.Ldloc_0),
                new(OpCodes.Ldloc_2),
                new(OpCodes.Ldelem_Ref),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, typeof(Preconditions).MethodNamed(nameof(Check)))
            })
            .AddJump(OpCodes.Brtrue, "continue")
            .Add(new CodeInstruction[]
            {
                new(OpCodes.Ldc_I4_M1),
                new(OpCodes.Ret)
            })
            .AddLabel("skip")
            .SkipTo(new CodeInstruction[]
            {
                new(OpCodes.Ldloc_2),
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Add),
                new(OpCodes.Stloc_2)
            })
            .AddLabel("continue")
            .Finish();

        private static bool Check(string split, GameLocation loc)
            => split.Length >= 2 && Backport.GameStateQuery.CheckConditions(split[1..], game_location: loc);
    }
}
