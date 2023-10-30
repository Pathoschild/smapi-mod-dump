/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
**
*************************************************/

namespace DaLion.Overhaul.Modules.Tweex.Patchers.Integration;

#region using directives

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Tweex.Extensions;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
[ModRequirement("Pathoschild.Automate", "Automate")]
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "Integration patch specifies the mod in file name but not class to avoid breaking pattern.")]
internal sealed class BeeHouseMachineGetOutputPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="BeeHouseMachineGetOutputPatcher"/> class.</summary>
    internal BeeHouseMachineGetOutputPatcher()
    {
        this.Target = "Pathoschild.Stardew.Automate.Framework.Machines.Objects.BeeHouseMachine"
            .ToType()
            .RequireMethod("GetOutput");
    }

    #region harmony patches

    /// <summary>Adds aging quality to automated bee houses.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? BeeHouseMachineGetOutputTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // Injected: object.Quality = object.GetQualityFromAge();
        // Before: result = object;
        try
        {
            helper
                .Match(
                    new[] { new CodeInstruction(OpCodes.Stloc_S, helper.Locals[4]) },
                    ILHelper.SearchOption.Last)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Dup),
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(
                            OpCodes.Call,
                            "Pathoschild.Stardew.Automate.Framework.BaseMachine`1"
                                .ToType()
                                .MakeGenericType(typeof(SObject))
                                .RequirePropertyGetter("Machine")),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(SObjectExtensions).RequireMethod(nameof(SObjectExtensions.GetQualityFromAge))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(SObject).RequirePropertySetter(nameof(SObject.Quality))),
                    });
        }
        catch (Exception ex)
        {
            Log.E("Tweex module failed improving automated honey quality with age." +
                  $"\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
