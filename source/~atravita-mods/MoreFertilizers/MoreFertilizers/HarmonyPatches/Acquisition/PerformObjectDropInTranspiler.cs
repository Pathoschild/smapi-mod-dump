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
using AtraBase.Toolkit.Reflection;
using AtraCore.Framework.ReflectionManager;
using AtraShared.Utils.HarmonyHelper;
using HarmonyLib;

namespace MoreFertilizers.HarmonyPatches.Acquisition;

/// <summary>
/// Transpiler to add more fertilizers to the bone mill.
/// </summary>
internal static class PerformObjectDropInTranspiler
{
    /// <summary>
    /// Applies the patches against SObject.performObjectDropInAction.
    /// </summary>
    /// <param name="harmony">Harmony instance.</param>
    internal static void ApplyPatches(Harmony harmony)
    {
        harmony.Patch(
            original: typeof(SObject).GetCachedMethod(nameof(SObject.performObjectDropInAction), ReflectionCache.FlagTypes.InstanceFlags),
            transpiler: new HarmonyMethod(typeof(PerformObjectDropInTranspiler), nameof(Transpiler)));
    }

    /// <summary>
    /// Applise a patch against Automate's bone mill.
    /// </summary>
    /// <param name="harmony">Harmony instance.</param>
    internal static void ApplyAutomateTranspiler(Harmony harmony)
    {
        Type bonemill = AccessTools.TypeByName("Pathoschild.Stardew.Automate.Framework.Machines.Objects.BoneMillMachine")
            ?? ReflectionThrowHelper.ThrowMethodNotFoundException<Type>("Automate bonemill");

        harmony.Patch(
            original: bonemill.GetCachedMethod("GetRecipeOutput", ReflectionCache.FlagTypes.StaticFlags),
            transpiler: new HarmonyMethod(typeof(PerformObjectDropInTranspiler), nameof(AutomateTranspiler)));
    }

    private static int GetOrganicFertilizer()
        => ModEntry.OrganicFertilizerID != -1 ? ModEntry.OrganicFertilizerID : 466;

    private static int GetFruitTreeFertilizer()
        => ModEntry.FruitTreeFertilizerID != -1 ? ModEntry.FruitTreeFertilizerID : 369;

    private static int GetBountifulFertilizer()
        => ModEntry.BountifulFertilizerID != -1 ? ModEntry.BountifulFertilizerID : 465;

    private static int GetDeluxeFruitTreeFertilizer()
        => ModEntry.DeluxeFruitTreeFertilizerID != -1 ? ModEntry.DeluxeFruitTreeFertilizerID : 805;

    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);
            helper.FindNext(new CodeInstructionWrapper[]
            { // Find the bone mill section.
                new(OpCodes.Call, typeof(SObject).GetCachedProperty(nameof(SObject.name), ReflectionCache.FlagTypes.InstanceFlags).GetGetMethod()),
                new(OpCodes.Ldstr, "Bone Mill"),
            })
            .FindNext(new CodeInstructionWrapper[]
            { // find switch(Game1.random.Next(4)) and add four new cases.
                new(OpCodes.Ldsfld, typeof(Game1).GetCachedField(nameof(Game1.random), ReflectionCache.FlagTypes.StaticFlags)),
                new(OpCodes.Ldc_I4_4),
                new(OpCodes.Callvirt, typeof(Random).InstanceMethodNamed(nameof(Random.Next), new[] { typeof(int) } )),
                new(SpecialCodeInstructionCases.StLoc),
            }).Advance(1)
            .ReplaceInstruction(OpCodes.Ldc_I4, 8)
            .FindNext(new CodeInstructionWrapper[]
            {
                new(OpCodes.Switch),
                new(OpCodes.Br_S),
            })
            .Push()
            .Advance(1)
            .StoreBranchDest()
            .FindNext(new CodeInstructionWrapper[]
            {
                new(OpCodes.Ldc_I4),
                new(SpecialCodeInstructionCases.StLoc),
                new(OpCodes.Ldc_I4_3),
                new(SpecialCodeInstructionCases.StLoc),
            }).Advance(1);

            CodeInstruction? storeindex = helper.CurrentInstruction.Clone();
            helper.Advance(2);
            CodeInstruction? storecount = helper.CurrentInstruction.Clone();

            helper.AdvanceToStoredLabel()
            .DefineAndAttachLabel(out Label exitPoint);

            helper.Insert(new CodeInstruction[]
            {
                new(OpCodes.Br_S, exitPoint),
                new(OpCodes.Call, typeof(PerformObjectDropInTranspiler).StaticMethodNamed(nameof(GetOrganicFertilizer))),
                storeindex,
                new(OpCodes.Ldc_I4_3),
                storecount,
                new(OpCodes.Br_S, exitPoint),
                new(OpCodes.Call, typeof(PerformObjectDropInTranspiler).StaticMethodNamed(nameof(GetDeluxeFruitTreeFertilizer))),
                storeindex,
                new(OpCodes.Ldc_I4_3),
                storecount,
                new(OpCodes.Br_S, exitPoint),
                new(OpCodes.Call, typeof(PerformObjectDropInTranspiler).StaticMethodNamed(nameof(GetFruitTreeFertilizer))),
                storeindex,
                new(OpCodes.Ldc_I4, 10),
                storecount,
                new(OpCodes.Br_S, exitPoint),
                new(OpCodes.Call, typeof(PerformObjectDropInTranspiler).StaticMethodNamed(nameof(GetBountifulFertilizer))),
                storeindex,
                new(OpCodes.Ldc_I4_5),
                storecount,
            })
            .Advance(-4)
            .DefineAndAttachLabel(out Label jumpPoint5)
            .Advance(-5)
            .DefineAndAttachLabel(out Label jumpPoint6)
            .Advance(-5)
            .DefineAndAttachLabel(out Label jumpPoint7)
            .Advance(-5)
            .DefineAndAttachLabel(out Label jumpPoint8)
            .Pop();

            Label[]? operand = (Label[])helper.CurrentInstruction.operand;
            List<Label> jumpPoints = new(operand);
            jumpPoints.AddRange(new[] { jumpPoint8, jumpPoint7, jumpPoint6, jumpPoint5 });
            helper.ReplaceOperand(jumpPoints.ToArray());

            return helper.Render();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Mod crashed while transpiling SObject.performObjectDropInAction:\n\n{ex}", LogLevel.Error);
        }
        return null;
    }

    private static IEnumerable<CodeInstruction>? AutomateTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);

            helper.FindNext(new CodeInstructionWrapper[]
            { // find switch(Game1.random.Next(4)) and add four new cases.
                new(OpCodes.Ldsfld, typeof(Game1).StaticFieldNamed(nameof(Game1.random))),
                new(OpCodes.Ldc_I4_4),
                new(OpCodes.Callvirt, typeof(Random).InstanceMethodNamed(nameof(Random.Next), new[] { typeof(int) } )),
                new(SpecialCodeInstructionCases.StLoc),
            }).Advance(1)
            .ReplaceInstruction(OpCodes.Ldc_I4, 8)
            .FindNext(new CodeInstructionWrapper[]
            {
                new(OpCodes.Switch),
                new(OpCodes.Br_S),
            })
            .Push()
            .Advance(1)
            .StoreBranchDest()
            .FindNext(new CodeInstructionWrapper[]
            {
                new(OpCodes.Ldc_I4),
                new(SpecialCodeInstructionCases.StLoc),
                new(OpCodes.Ldc_I4_3),
                new(SpecialCodeInstructionCases.StLoc),
            }).Advance(1);

            CodeInstruction? storeindex = helper.CurrentInstruction.Clone();
            helper.Advance(2);
            CodeInstruction? storecount = helper.CurrentInstruction.Clone();

            helper.AdvanceToStoredLabel()
            .DefineAndAttachLabel(out Label exitPoint);

            helper.Insert(new CodeInstruction[]
            {
                new(OpCodes.Br_S, exitPoint),
                new(OpCodes.Call, typeof(PerformObjectDropInTranspiler).StaticMethodNamed(nameof(GetOrganicFertilizer))),
                storeindex,
                new(OpCodes.Ldc_I4_3),
                storecount,
                new(OpCodes.Br_S, exitPoint),
                new(OpCodes.Call, typeof(PerformObjectDropInTranspiler).StaticMethodNamed(nameof(GetDeluxeFruitTreeFertilizer))),
                storeindex,
                new(OpCodes.Ldc_I4_3),
                storecount,
                new(OpCodes.Br_S, exitPoint),
                new(OpCodes.Call, typeof(PerformObjectDropInTranspiler).StaticMethodNamed(nameof(GetFruitTreeFertilizer))),
                storeindex,
                new(OpCodes.Ldc_I4, 10),
                storecount,
                new(OpCodes.Br_S, exitPoint),
                new(OpCodes.Call, typeof(PerformObjectDropInTranspiler).StaticMethodNamed(nameof(GetBountifulFertilizer))),
                storeindex,
                new(OpCodes.Ldc_I4_5),
                storecount,
            })
            .Advance(-4)
            .DefineAndAttachLabel(out Label jumpPoint5)
            .Advance(-5)
            .DefineAndAttachLabel(out Label jumpPoint6)
            .Advance(-5)
            .DefineAndAttachLabel(out Label jumpPoint7)
            .Advance(-5)
            .DefineAndAttachLabel(out Label jumpPoint8)
            .Pop();

            Label[]? operand = (Label[])helper.CurrentInstruction.operand;
            List<Label> jumpPoints = new(operand);
            jumpPoints.AddRange(new[] { jumpPoint8, jumpPoint7, jumpPoint6, jumpPoint5 });
            helper.ReplaceOperand(jumpPoints.ToArray());

            // helper.Print();
            return helper.Render();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Mod crashed while transpiling Automate's bone mill:\n\n{ex}", LogLevel.Error);
        }
        return null;
    }
}