/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Tools.Patchers;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Tools.Extensions;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Netcode;
using StardewValley.TerrainFeatures;

#endregion using directives

[UsedImplicitly]
internal sealed class HoeDirtPerformToolActionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="HoeDirtPerformToolActionPatcher"/> class.</summary>
    internal HoeDirtPerformToolActionPatcher()
    {
        this.Target = this.RequireMethod<HoeDirt>(nameof(HoeDirt.performToolAction));
    }

    #region harmony patches

    /// <summary>Patch to allow harvest with scythe.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? HoeDirtPerformToolActionTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        var tempHarvestMethod = generator.DeclareLocal(typeof(int));

        // Injected:
        //      var temp = this.crop.harvestMethod.Value;
        //      if ((Game1.player.CanSickleHarvest(this.crop) { this.crop.harvestMethod.Value = 1; }
        // Before: if ((int)crop.harvestMethod == 1)
        try
        {
            var resumeExecution = generator.DefineLabel();
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldfld, typeof(Crop).RequireField(nameof(Crop.harvestMethod))),
                    })
                .Match(new[] { new CodeInstruction(OpCodes.Ldarg_0) }, ILHelper.SearchOption.Previous)
                .AddLabels(resumeExecution)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Call, typeof(HoeDirt).RequirePropertyGetter(nameof(HoeDirt.crop))),
                        new CodeInstruction(OpCodes.Ldfld, typeof(Crop).RequireField(nameof(Crop.harvestMethod))),
                        new CodeInstruction(OpCodes.Call, typeof(NetFieldBase<int, NetInt>).RequireMethod("op_Implicit")),
                        new CodeInstruction(OpCodes.Stloc_S, tempHarvestMethod),
                        new CodeInstruction(OpCodes.Call, typeof(Game1).RequirePropertyGetter(nameof(Game1.player))),
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Call, typeof(HoeDirt).RequirePropertyGetter(nameof(HoeDirt.crop))),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(FarmerExtensions).RequireMethod(nameof(FarmerExtensions.CanSickleHarvest))),
                        new CodeInstruction(OpCodes.Brfalse_S, resumeExecution),
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Call, typeof(HoeDirt).RequirePropertyGetter(nameof(HoeDirt.crop))),
                        new CodeInstruction(OpCodes.Ldfld, typeof(Crop).RequireField(nameof(Crop.harvestMethod))),
                        new CodeInstruction(OpCodes.Ldc_I4_1), // 1 is for Crop.sickleHarvest
                        new CodeInstruction(OpCodes.Call, typeof(NetInt).RequireMethod(nameof(NetInt.Set))),
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding check for sickle harvest.\nHelper returned {ex}");
            return null;
        }

        // Injected: this.crop.harvestMethod.Value = temp;
        // Before: if ((bool)crop.dead)
        try
        {
            helper
                .Match(new[]
                {
                    new CodeInstruction(OpCodes.Ldfld, typeof(Crop).RequireField(nameof(Crop.dead))),
                })
                .Match(new[] { new CodeInstruction(OpCodes.Ldarg_0) }, ILHelper.SearchOption.Previous)
                .Insert(new[]
                {
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call, typeof(HoeDirt).RequirePropertyGetter(nameof(HoeDirt.crop))),
                    new CodeInstruction(OpCodes.Ldfld, typeof(Crop).RequireField(nameof(Crop.harvestMethod))),
                    new CodeInstruction(OpCodes.Ldloc_S, tempHarvestMethod),
                    new CodeInstruction(OpCodes.Call, typeof(NetInt).RequireMethod(nameof(NetInt.Set))),
                });
        }
        catch (Exception ex)
        {
            Log.E($"Failed restoring original harvest method.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
