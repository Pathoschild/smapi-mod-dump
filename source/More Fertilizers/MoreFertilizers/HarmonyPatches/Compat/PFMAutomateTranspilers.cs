/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/MoreFertilizers
**
*************************************************/

using System.Reflection;
using System.Reflection.Emit;
using AtraBase.Toolkit.Reflection;
using AtraShared.Utils.HarmonyHelper;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;

namespace MoreFertilizers.HarmonyPatches.Compat;

/// <summary>
/// Holds transpilers against PFMAutomate.
/// </summary>
internal static class PFMAutomateTranspilers
{
    /// <summary>
    /// Applies patches against PFM for PFM Automate.
    /// </summary>
    /// <param name="harmony">Harmony instance.</param>
    /// <exception cref="MethodNotFoundException">Something wasn't found.</exception>
    internal static void ApplyPatches(Harmony harmony)
    {
        try
        {
            Type pfmController = AccessTools.TypeByName("ProducerFrameworkMod.Controllers.ProducerRuleController")
                ?? throw new MethodNotFoundException("PFM Controller");
            harmony.Patch(
                original: pfmController.StaticMethodNamed("SearchInput"),
                transpiler: new HarmonyMethod(typeof(PFMAutomateTranspilers), nameof(SearchInputTranspiler)));
            harmony.Patch(
                original: pfmController.StaticMethodNamed("ProduceOutput"),
                transpiler: new HarmonyMethod(typeof(PFMAutomateTranspilers), nameof(ProduceOutputTranspiler)));
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Mod failed while transpiling automate. Integration may not work.\n\n{ex}", LogLevel.Error);
        }
    }

#pragma warning disable SA1116 // Split parameters should start on line after declaration
    private static IEnumerable<CodeInstruction>? SearchInputTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);
            helper.DeclareLocal(typeof(int), out LocalBuilder fertilizer)
            .FindNext(new CodeInstructionWrapper[]
            { // Find the first hoedirt, get the local for it.
                new(OpCodes.Isinst, typeof(HoeDirt)),
                new(SpecialCodeInstructionCases.StLoc, typeof(HoeDirt)),
                new(SpecialCodeInstructionCases.LdLoc, typeof(HoeDirt)),
            })
            .Advance(1);

            CodeInstruction hoedirt = helper.CurrentInstruction.ToLdLoc();

            helper.FindNext(new CodeInstructionWrapper[]
            { // find where the crop is stored.
                new(SpecialCodeInstructionCases.LdLoc, (LocalBuilder)hoedirt.operand),
                new(OpCodes.Callvirt, typeof(HoeDirt).InstancePropertyNamed(nameof(HoeDirt.crop)).GetGetMethod()),
                new(SpecialCodeInstructionCases.StLoc),
            })
            .GetLabels(out IList<Label>? firstLabelsToMove)
            .Insert(new CodeInstruction[]
            { // Insert codes that save the fertilizer into our local.
                hoedirt,
                new(OpCodes.Ldfld, typeof(HoeDirt).InstanceFieldNamed(nameof(HoeDirt.fertilizer))),
                new(OpCodes.Callvirt, typeof(NetFieldBase<int, NetInt>).InstancePropertyNamed("Value").GetGetMethod()),
                new(OpCodes.Stloc_S, fertilizer),
            }, withLabels: firstLabelsToMove)
            .FindNext(new CodeInstructionWrapper[]
            { // Find the other possibility of a crop, an Indoor pot.
                new(OpCodes.Isinst, typeof(IndoorPot)),
            })
            .FindNext(new CodeInstructionWrapper[]
            { // And find its hoedirt.
                new(SpecialCodeInstructionCases.LdLoc),
                new(OpCodes.Ldfld, typeof(IndoorPot).InstanceFieldNamed(nameof(IndoorPot.hoeDirt))),
                new(OpCodes.Callvirt, typeof(NetFieldBase<HoeDirt, NetRef<HoeDirt>>).InstancePropertyNamed("Value").GetGetMethod()),
                new(SpecialCodeInstructionCases.StLoc),
            })
            .Advance(3);

            CodeInstruction secondHoeDirt = helper.CurrentInstruction.ToLdLoc();

            helper.FindNext(new CodeInstructionWrapper[]
            { // Find where the second crop is stored.
                new(SpecialCodeInstructionCases.LdLoc, (LocalBuilder)secondHoeDirt.operand),
                new(OpCodes.Callvirt, typeof(HoeDirt).InstancePropertyNamed(nameof(HoeDirt.crop)).GetGetMethod()),
                new(SpecialCodeInstructionCases.StLoc),
            })
            .GetLabels(out IList<Label>? secondLabelsToMove)
            .Insert(new CodeInstruction[]
            { // Save the fertilizer into the local. (Either this path or the previous will be taken, but not both.)
                secondHoeDirt,
                new(OpCodes.Ldfld, typeof(HoeDirt).InstanceFieldNamed(nameof(HoeDirt.fertilizer))),
                new(OpCodes.Callvirt, typeof(NetFieldBase<int, NetInt>).InstancePropertyNamed("Value").GetGetMethod()),
                new(OpCodes.Stloc_S, fertilizer),
            }, withLabels: secondLabelsToMove)
            .FindNext(new CodeInstructionWrapper[]
            { // The next thing we care about is where the crop product is created, so look for that.
                new(SpecialCodeInstructionCases.LdLoc, typeof(Crop)),
                new(OpCodes.Ldfld, typeof(Crop).InstanceFieldNamed(nameof(Crop.programColored))),
            })
            .FindNext(new CodeInstructionWrapper[]
            {
                new(OpCodes.Newobj, typeof(SObject).Constructor(new[] { typeof(int), typeof(int), typeof(bool), typeof(int), typeof(int)})),
                new(SpecialCodeInstructionCases.StLoc),
            })
            .Advance(1)
            .Insert(new CodeInstruction[]
            {
                new(OpCodes.Ldloc_S, fertilizer),
                new(OpCodes.Call, typeof(MultiYieldCropsCompat).StaticMethodNamed(nameof(MultiYieldCropsCompat.AdjustItem))),
            })
            .FindNext(new CodeInstructionWrapper[]
            {
                new(OpCodes.Newobj, typeof(ColoredObject).Constructor(new[] { typeof(int), typeof(int), typeof(Color) })),
                new(SpecialCodeInstructionCases.StLoc),
            })
            .Advance(1)
            .Insert(new CodeInstruction[]
            {
                new(OpCodes.Ldloc_S, fertilizer),
                new(OpCodes.Call, typeof(MultiYieldCropsCompat).StaticMethodNamed(nameof(MultiYieldCropsCompat.AdjustItem))),
            });

            // helper.Print();
            return helper.Render();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Mod crashed while transpiling PFM for PFMAutomate:\n\n{ex}", LogLevel.Error);
        }
        return null;
    }

    private static IEnumerable<CodeInstruction>? ProduceOutputTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);
            helper.FindNext(new CodeInstructionWrapper[]
            {
                new(OpCodes.Ldarg_1),
                new(OpCodes.Ldfld, typeof(SObject).InstanceFieldNamed(nameof(SObject.heldObject))),
                new(SpecialCodeInstructionCases.LdLoc),
                new(OpCodes.Callvirt, typeof(NetFieldBase<SObject, NetRef<SObject>>).InstancePropertyNamed("Value").GetSetMethod()),
            })
            .Advance(3)
            .Insert(new CodeInstruction[]
            {
                new(OpCodes.Ldarg_S, 6), // Input SObject
                new(OpCodes.Call, typeof(AutomateTranspiler).StaticMethodNamed(nameof(AutomateTranspiler.MakeOrganic))),
            });

            // helper.Print();
            return helper.Render();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Mod crashed while transpiling PFM for PFMAutomate:\n\n{ex}", LogLevel.Error);
        }
        return null;
    }
#pragma warning restore SA1116 // Split parameters should start on line after declaration
}