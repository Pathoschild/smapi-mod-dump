/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Patches.Integrations.Automate;

#region using directives

using DaLion.Common;
using DaLion.Common.Data;
using DaLion.Common.Extensions.Reflection;
using DaLion.Common.Harmony;
using Extensions;
using HarmonyLib;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

#endregion using directives

[UsedImplicitly]
internal sealed class BushMachineGetOutputPatch : DaLion.Common.Harmony.HarmonyPatch
{
    private static Func<object, Bush>? _GetMachine;

    /// <summary>Construct an instance.</summary>
    internal BushMachineGetOutputPatch()
    {
        try
        {
            Target = "Pathoschild.Stardew.Automate.Framework.Machines.TerrainFeatures.BushMachine".ToType()
                .RequireMethod("GetOutput");
        }
        catch
        {
            // ignored
        }
    }

    #region harmony patches

    /// <summary>Patch for automated Berry Bush forage increment.</summary>
    [HarmonyPostfix]
    private static void BushMachineGetOutputPostfix(object __instance)
    {
        if (!ModEntry.Config.ShouldCountAutomatedHarvests) return;

        _GetMachine ??= __instance.GetType().RequirePropertyGetter("Machine")
            .CompileUnboundDelegate<Func<object, Bush>>();
        var machine = _GetMachine(__instance);
        if (machine.size.Value >= Bush.greenTeaBush) return;

        if (!Context.IsMainPlayer || !Game1.player.HasProfession(Profession.Ecologist)) return;

        ModDataIO.IncrementData<uint>(Game1.player, ModData.EcologistItemsForaged.ToString());
    }

    /// <summary>Patch for automated Berry Bush quality.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? BushMachineGetOutputTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// From: int quality = Game1.player.professions.Contains(<ecologist_id>) ? 4 : 0);
        /// To: int quality = Game1.player.professions.Contains(<ecologist_id>) ? GetEcologistForageQuality : 0);

        try
        {
            helper
                .FindProfessionCheck(Profession.Ecologist.Value) // find index of ecologist check
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Ldc_I4_4) // quality = 4
                )
                .GetLabels(out var labels) // backup branch labels
                .ReplaceWith( // replace with custom quality
                    new(OpCodes.Call,
                        typeof(FarmerExtensions).RequireMethod(nameof(FarmerExtensions.GetEcologistForageQuality)))
                )
                .InsertWithLabels(
                    labels: labels, // restore backed-up labels
                    new CodeInstruction(OpCodes.Call, typeof(Game1).RequirePropertyGetter(nameof(Game1.player)))
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while patching automated Berry Bush quality.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}