/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Professions.Framework.Patchers.Mining;

#region using directives

using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.GameData.Machines;
using StardewValley.Inventories;

#endregion using directives

[UsedImplicitly]
internal sealed class ObjectPlaceInMachinePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ObjectPlaceInMachinePatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal ObjectPlaceInMachinePatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<SObject>(nameof(SObject.PlaceInMachine));
    }

    #region harmony patches

    /// <summary>Patch to add Prestiged Demolitionist efficient coal consumption.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? ObjectPlaceInMachineTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            var skipConsumption = generator.DefineLabel();
            helper
                .PatternMatch([
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(IInventory).RequireMethod(nameof(IInventory.ReduceId))),
                ])
                .Move(2)
                .AddLabels(skipConsumption)
                .PatternMatch(
                    [
                        new CodeInstruction(OpCodes.Ldloc_S, helper.Locals[7]),
                    ],
                    ILHelper.SearchOption.Previous)
                .Insert([
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldloc_S, helper.Locals[9]),
                    new CodeInstruction(OpCodes.Ldarg_S, (byte)4),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(ObjectPlaceInMachinePatcher).RequireMethod(nameof(ShouldSkipFuelConsumption))),
                    new CodeInstruction(OpCodes.Brtrue_S, skipConsumption),
                ]);
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding Prestiged Demolitionist efficient coal consumption.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injections

    private static bool ShouldSkipFuelConsumption(
        SObject machine,
        MachineItemAdditionalConsumedItems additionalRequirement,
        Farmer who)
    {
        if (additionalRequirement.ItemId == SObject.coalQID && who.HasProfession(Profession.Demolitionist, true))
        {
            var required = additionalRequirement.RequiredCount;
            if (Data.ReadAs<int>(machine, DataKeys.PersistedCoals) >= required)
            {
                Data.Increment(machine, DataKeys.PersistedCoals, -required);
                return true;
            }

            Data.Increment(machine, DataKeys.PersistedCoals, required);
            return false;
        }

        return false;
    }

    #endregion injections
}
