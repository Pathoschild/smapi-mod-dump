/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using System.Reflection;
using System.Reflection.Emit;

using AtraCore.Framework.ReflectionManager;

using AtraShared.Utils.Extensions;
using AtraShared.Utils.HarmonyHelper;

using HarmonyLib;

namespace StopRugRemoval.HarmonyPatches.Niceties.CrashHandling;

[HarmonyPatch]
internal static class DummySpringSchedule
{
    private static string? GetDummySpringSchedule(NPC npc)
    {
        ModEntry.ModMonitor.Log($"NPC {npc.Name} found without the spring schedule, attempting to mock one", LogLevel.Warn);
        if (npc.DefaultMap is not null)
        {
            return $"{npc.DefaultMap} {npc.DefaultPosition.X / 64} {npc.DefaultPosition.Y / 64}";
        }
        return null;
    }

    [HarmonyPatch(typeof(NPC), nameof(NPC.getMasterScheduleEntry))]
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:Split parameters should start on line after declaration", Justification = "Reviewed.")]
    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);

            Label label = helper.Generator.DefineLabel();
            helper.FindLast(new CodeInstructionWrapper[]
            {
                OpCodes.Ldarg_0,
                OpCodes.Ldarg_1,
                (OpCodes.Stfld, typeof(NPC).GetCachedField("_lastLoadedScheduleKey", ReflectionCache.FlagTypes.InstanceFlags)),
                SpecialCodeInstructionCases.LdLoc,
                OpCodes.Ret,
            })
            .Advance(5)
            .GetLabels(out var labelsToMove)
            .DefineAndAttachLabel(out var skip);
            Label isnull = helper.Generator.DefineLabel();

            /* Injecting:
             * if (arg1 == "spring" && GetDummpSpringSchedule(npc) is string ret)
             * {
             *     return ret;
             * }
             * */

            helper.Insert(new CodeInstruction[]
            {
                new (OpCodes.Ldarg_1),
                new (OpCodes.Ldstr, "spring"),
                new (OpCodes.Call, typeof(string).GetCachedMethod<string, string>("op_Equality", ReflectionCache.FlagTypes.StaticFlags)),
                new (OpCodes.Brfalse, skip),
                new (OpCodes.Ldarg_0),
                new (OpCodes.Call, typeof(DummySpringSchedule).GetCachedMethod(nameof(GetDummySpringSchedule), ReflectionCache.FlagTypes.StaticFlags)),
                new (OpCodes.Dup),
                new (OpCodes.Brfalse, isnull),
                new (OpCodes.Ret),
                new CodeInstruction(OpCodes.Pop).WithLabels(isnull),
            }, withLabels: labelsToMove);

            // helper.Print();
            return helper.Render();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Ran into error transpiling {original.FullDescription()}.\n\n{ex}", LogLevel.Error);
            original.Snitch(ModEntry.ModMonitor);
        }
        return null;
    }
}
