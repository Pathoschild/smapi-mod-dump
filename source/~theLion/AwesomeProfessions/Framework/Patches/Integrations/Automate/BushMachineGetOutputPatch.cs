/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

using StardewModdingAPI;
using StardewValley.TerrainFeatures;

namespace DaLion.Stardew.Professions.Framework.Patches.Integrations;

#region using directives

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;

using Stardew.Common.Harmony;
using Extensions;

using Object = StardewValley.Object;

#endregion using directives

[UsedImplicitly]
internal class BushMachineGetOutputPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal BushMachineGetOutputPatch()
    {
        try
        {
            Original = "Pathoschild.Stardew.Automate.Framework.Machines.TerrainFeatures.BushMachine".ToType()
                .MethodNamed("GetOutput");
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
        if (__instance is null || !ModEntry.Config.ShouldCountAutomatedHarvests) return;

        var machine = ModEntry.ModHelper.Reflection.GetProperty<Bush>(__instance, "Machine").GetValue();
        if (machine is null || machine.size.Value == 3) return;

        if (!Context.IsMainPlayer || !Game1.player.HasProfession(Profession.Ecologist)) return;

        ModData.Increment<uint>(DataField.EcologistItemsForaged);
    }

    /// <summary>Patch for automated Berry Bush quality.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> BushMachineGetOutputTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// From: int quality = Game1.player.professions.Contains(<ecologist_id>) ? 4 : 0);
        /// To: int quality = Game1.player.professions.Contains(<ecologist_id>) ? GetEcologistForageQuality : 0);

        try
        {
            helper
                .FindProfessionCheck((int) Profession.Ecologist) // find index of ecologist check
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Ldc_I4_4) // quality = 4
                )
                .GetLabels(out var labels) // backup branch labels
                .ReplaceWith( // replace with custom quality
                    new(OpCodes.Call,
                        typeof(FarmerExtensions).MethodNamed(
                            nameof(FarmerExtensions.GetEcologistForageQuality)))
                )
                .Insert(
                    labels: labels, // restore backed-up labels
                    new CodeInstruction(OpCodes.Call, typeof(Game1).PropertyGetter(nameof(Game1.player)))
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