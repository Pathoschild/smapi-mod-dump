/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Professions.Patchers.Farming;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class ObjectPerformDropDownActionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ObjectPerformDropDownActionPatcher"/> class.</summary>
    internal ObjectPerformDropDownActionPatcher()
    {
        this.Target = this.RequireMethod<SObject>(nameof(SObject.performDropDownAction));
    }

    #region harmony patches

    /// <summary>Patch to increase production frequency of Producer Bee House.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? ObjectDayUpdateTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // From: minutesUntilReady.Value = Utility.CalculateMinutesUntilMorning(Game1.timeOfDay, 4);
        // To: minutesUntilReady.Value = Utility.CalculateMinutesUntilMorning(Game1.timeOfDay, this.DoesOwnerHaveProfession(<producer_id>)
        //     ? this.DoesOwnerHaveProfession(100 + <producer_id>
        //         ? 1
        //         : 2
        //     : 4);
        try
        {
            var isNotProducer = generator.DefineLabel();
            var isNotPrestiged = generator.DefineLabel();
            var resumeExecution = generator.DefineLabel();
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldc_I4_4),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(Utility).RequireMethod(
                                nameof(Utility.CalculateMinutesUntilMorning),
                                new[] { typeof(int), typeof(int) })),
                    })
                .AddLabels(isNotProducer)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldc_I4_3), // 3 = Profession.Producer
                        new CodeInstruction(OpCodes.Ldc_I4_0), // false for not prestiged
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(SObjectExtensions).RequireMethod(
                                nameof(SObjectExtensions.DoesOwnerHaveProfession),
                                new[] { typeof(SObject), typeof(int), typeof(bool) })),
                        new CodeInstruction(OpCodes.Brfalse_S, isNotProducer),
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldc_I4_3),
                        new CodeInstruction(OpCodes.Ldc_I4_1), // true for prestiged
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(SObjectExtensions).RequireMethod(
                                nameof(SObjectExtensions.DoesOwnerHaveProfession),
                                new[] { typeof(SObject), typeof(int), typeof(bool) })),
                        new CodeInstruction(OpCodes.Brfalse_S, isNotPrestiged),
                        new CodeInstruction(OpCodes.Ldc_I4_1),
                        new CodeInstruction(OpCodes.Br_S, resumeExecution),
                        new CodeInstruction(OpCodes.Ldc_I4_2),
                        new CodeInstruction(OpCodes.Br_S, resumeExecution),
                    })
                .Move(-2)
                .AddLabels(isNotPrestiged)
                .Return()
                .Move()
                .AddLabels(resumeExecution);
        }
        catch (Exception ex)
        {
            Log.E($"Failed patching bee house production speed for Producers.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
