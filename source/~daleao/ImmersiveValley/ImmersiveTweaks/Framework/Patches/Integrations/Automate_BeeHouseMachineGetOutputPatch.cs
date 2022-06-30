/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Stardew.Tweex.Framework.Patches;

#region using directives

using Common;
using Common.Extensions.Reflection;
using Common.Harmony;
using Extensions;
using HarmonyLib;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using SObject = StardewValley.Object;

#endregion using directives

[UsedImplicitly]
internal sealed class BeeHouseMachineGetOutputPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal BeeHouseMachineGetOutputPatch()
    {
        try
        {
            Target = "Pathoschild.Stardew.Automate.Framework.Machines.Objects.BeeHouseMachine".ToType()
                .RequireMethod("GetOutput");
        }
        catch
        {
            // ignored
        }
    }

    #region harmony patches

    /// <summary>Adds aging quality to automated bee houses.</summary>
    private static IEnumerable<CodeInstruction>? BeeHouseMachineGetOutputTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// Injected: @object.Quality = @object.GetQualityFromAge();
        /// After: @object.preservedParentSheetIndex.Value = flowerId;

        try
        {
            helper
                .FindLast(
                    new CodeInstruction(OpCodes.Stloc_S, helper.Locals[4])
                )
                .Insert(
                    new CodeInstruction(OpCodes.Dup),
                    new CodeInstruction(OpCodes.Dup),
                    new CodeInstruction(OpCodes.Call,
                        typeof(SObjectExtensions).RequireMethod(nameof(SObjectExtensions.GetQualityFromAge))),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(SObject).RequirePropertySetter(nameof(SObject.Quality)))
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed improving automated honey quality with age.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}