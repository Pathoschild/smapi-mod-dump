/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Professions.Framework.Patchers.Foraging;

#region using directives

using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Data;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.TerrainFeatures;

#endregion using directives

[UsedImplicitly]
internal sealed class TreeDayUpdatePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="TreeDayUpdatePatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal TreeDayUpdatePatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<Tree>(nameof(Tree.dayUpdate));
    }

    #region harmony patches

    /// <summary>Patch to increase Arborist non-fruit tree growth odds.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? TreeDayUpdateTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            var readMethod = typeof(ModDataManager).GetMethods()
                                 .FirstOrDefault(mi =>
                                     mi.Name == nameof(ModDataManager.ReadAs) &&
                                     mi.GetParameters()[0].ParameterType == typeof(TerrainFeature) &&
                                     mi.GetGenericArguments().Length > 0)
                                 ?.MakeGenericMethod(typeof(bool)) ??
                             ThrowHelper.ThrowMissingMethodException<MethodInfo>("Read method not found.");
            var resumeExecution = generator.DefineLabel();
            helper
                .PatternMatch([new CodeInstruction(OpCodes.Stloc_S, helper.Locals[7])])
                .Move()
                .AddLabels(resumeExecution)
                .Insert([
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(ProfessionsMod).RequirePropertyGetter(nameof(Data))),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldstr, DataKeys.PlantedByArborist),
                    new CodeInstruction(OpCodes.Ldc_I4_0),
                    new CodeInstruction(OpCodes.Ldstr, string.Empty),
                    new CodeInstruction(OpCodes.Callvirt, readMethod),
                    new CodeInstruction(OpCodes.Brfalse_S, resumeExecution),
                    new CodeInstruction(OpCodes.Ldloc_S, helper.Locals[6]),
                    new CodeInstruction(OpCodes.Ldc_R4, 0.25f),
                    new CodeInstruction(OpCodes.Add),
                    new CodeInstruction(OpCodes.Stloc_S, helper.Locals[6]),
                    new CodeInstruction(OpCodes.Ldloc_S, helper.Locals[7]),
                    new CodeInstruction(OpCodes.Ldc_R4, 0.25f),
                    new CodeInstruction(OpCodes.Add),
                    new CodeInstruction(OpCodes.Stloc_S, helper.Locals[7])

                ]);
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding Arborist bonus tree growth chance.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
