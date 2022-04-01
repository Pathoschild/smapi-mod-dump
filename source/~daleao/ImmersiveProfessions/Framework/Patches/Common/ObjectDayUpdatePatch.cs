/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Patches.Common;

#region using directives

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;

using Stardew.Common.Extensions;
using Stardew.Common.Harmony;
using Extensions;

using SObject = StardewValley.Object;

#endregion using directives

[UsedImplicitly]
internal class ObjectDayUpdatePatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal ObjectDayUpdatePatch()
    {
        Original = RequireMethod<SObject>(nameof(SObject.DayUpdate));
        Postfix.priority = Priority.LowerThanNormal;
    }

    #region harmony patches

    /// <summary>Patch to increment object age + add quality to Ecologist Mushroom Boxes.</summary>
    [HarmonyPostfix]
    private static void ObjectDayUpdatePostfix(SObject __instance)
    {
        if (__instance.IsMushroomBox() && __instance.heldObject.Value is not null && Game1.MasterPlayer.HasProfession(Profession.Ecologist))
            __instance.heldObject.Value.Quality = Game1.MasterPlayer.GetEcologistForageQuality();
    }

    /// <summary>Patch to increase production frequency of Producer Bee House.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> ObjectDayUpdateTranspiler(IEnumerable<CodeInstruction> instructions,
        ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// From: minutesUntilReady.Value = Utility.CalculateMinutesUntilMorning(Game1.timeOfDay, 4);
        /// To: minutesUntilReady.Value = Utility.CalculateMinutesUntilMorning(Game1.timeOfDay, this.DoesOwnerHaveProfession(<producer_id>)
        ///     ? this.DoesOwnerHaveProfession(100 + <producer_id>
        ///         ? 1
        ///         : 2
        ///     : 4);

        var isNotProducer = generator.DefineLabel();
        var isNotPrestiged = generator.DefineLabel();
        var resumeExecution = generator.DefineLabel();
        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldc_I4_4),
                    new CodeInstruction(OpCodes.Call,
                        typeof(Utility).MethodNamed(nameof(Utility.CalculateMinutesUntilMorning),
                            new[] {typeof(int), typeof(int)}))
                )
                .AddLabels(isNotProducer)
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldc_I4_3), // 3 = Profession.Producer
                    new CodeInstruction(OpCodes.Ldc_I4_0), // false for not prestiged
                    new CodeInstruction(OpCodes.Call,
                        typeof(SObjectExtensions).MethodNamed(nameof(SObjectExtensions.DoesOwnerHaveProfession))),
                    new CodeInstruction(OpCodes.Brfalse_S, isNotProducer),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldc_I4_3),
                    new CodeInstruction(OpCodes.Ldc_I4_1), // true for prestiged
                    new CodeInstruction(OpCodes.Call,
                        typeof(SObjectExtensions).MethodNamed(nameof(SObjectExtensions.DoesOwnerHaveProfession))),
                    new CodeInstruction(OpCodes.Brfalse_S, isNotPrestiged),
                    new CodeInstruction(OpCodes.Ldc_I4_1),
                    new CodeInstruction(OpCodes.Br_S, resumeExecution),
                    new CodeInstruction(OpCodes.Ldc_I4_2),
                    new CodeInstruction(OpCodes.Br_S, resumeExecution)
                )
                .Retreat(2)
                .AddLabels(isNotPrestiged)
                .Return()
                .Advance()
                .AddLabels(resumeExecution);
        }
        catch (Exception ex)
        {
            Log.E($"Failed while patching Bee House production speed for Producers.\nHelper returned {ex}");
            transpilationFailed = true;
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}