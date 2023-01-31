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
using Microsoft.Xna.Framework;
using StardewValley.Monsters;

namespace PrismaticSlime.HarmonyPatches.EggPatches;

/// <summary>
/// Makes a prismatic slime egg hatch into a prismatic slime.
/// </summary>
[HarmonyPatch(typeof(SObject))]
internal static class AdjustSlimeHatching
{
    private static bool IsPrismaticSlimeEgg(SObject obj)
        => ModEntry.PrismaticSlimeEgg != -1 && obj.ParentSheetIndex == ModEntry.PrismaticSlimeEgg;

    private static GreenSlime MakePrismaticSlime(Vector2 position)
    {
        GreenSlime slime = new(position, 80);
        slime.makePrismatic();
        return slime;
    }

    [HarmonyPatch(nameof(SObject.DayUpdate))]
    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);
            helper.FindNext(new CodeInstructionWrapper[]
            {
                SpecialCodeInstructionCases.LdLoc,
                OpCodes.Ldfld,
                (OpCodes.Callvirt, typeof(GameLocation).GetCachedMethod(nameof(GameLocation.canSlimeHatchHere), ReflectionCache.FlagTypes.InstanceFlags)),
                OpCodes.Brfalse,
            })
            .FindNext(new CodeInstructionWrapper[]
            {
                (OpCodes.Call, typeof(Vector2).GetCachedMethod<Vector2, float>("op_Multiply", ReflectionCache.FlagTypes.StaticFlags)),
                SpecialCodeInstructionCases.StLoc,
            })
            .Advance(1);

            CodeInstruction? pos = helper.CurrentInstruction.ToLdLoc();

            helper.FindNext(new CodeInstructionWrapper[]
            {
                OpCodes.Ldarg_0,
                (OpCodes.Ldfld, typeof(SObject).GetCachedField(nameof(SObject.heldObject), ReflectionCache.FlagTypes.InstanceFlags)),
                OpCodes.Callvirt,
            })
            .Copy(3, out IEnumerable<CodeInstruction>? heldobj);

            helper.FindNext(new CodeInstructionWrapper[]
            {
                SpecialCodeInstructionCases.LdLoc,
                OpCodes.Ldc_I4_0,
                (OpCodes.Newobj, typeof(GreenSlime).GetCachedConstructor<Vector2, int>(ReflectionCache.FlagTypes.InstanceFlags)),
                SpecialCodeInstructionCases.StLoc,
                OpCodes.Br_S,
            })
            .Advance(3);

            CodeInstruction? ldSlime = helper.CurrentInstruction.ToLdLoc();
            CodeInstruction? stSlime = helper.CurrentInstruction.Clone();

            helper.Advance(1)
            .StoreBranchDest()
            .AdvanceToStoredLabel()
            .GetLabels(out IList<Label>? labelsToMove)
            .DefineAndAttachLabel(out Label notnull);

            List<CodeInstruction> instr = new()
            {
                ldSlime, // a slime has been created, jump past.
                new CodeInstruction(OpCodes.Brtrue, notnull),
            };
            instr.AddRange(heldobj);
            instr.Add(new CodeInstruction(OpCodes.Call, typeof(AdjustSlimeHatching).GetCachedMethod(nameof(IsPrismaticSlimeEgg), ReflectionCache.FlagTypes.StaticFlags)));
            instr.Add(new CodeInstruction(OpCodes.Brfalse_S, notnull)); // not a prismatic slime egg, skip.
            instr.Add(pos);
            instr.Add(new CodeInstruction(OpCodes.Call, typeof(AdjustSlimeHatching).GetCachedMethod(nameof(MakePrismaticSlime), ReflectionCache.FlagTypes.StaticFlags)));
            instr.Add(stSlime);

            helper.Insert(instr.ToArray(), withLabels: labelsToMove);

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
