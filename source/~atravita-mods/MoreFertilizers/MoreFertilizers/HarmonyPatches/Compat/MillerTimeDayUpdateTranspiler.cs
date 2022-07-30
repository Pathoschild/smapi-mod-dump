/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using System.Reflection;
using System.Reflection.Emit;
using AtraBase.Toolkit.Reflection;
using AtraCore.Framework.ReflectionManager;
using AtraShared.Utils.Extensions;
using AtraShared.Utils.HarmonyHelper;
using HarmonyLib;
using MoreFertilizers.HarmonyPatches.OrganicFertilizer;
using Netcode;
using StardewValley.Objects;

namespace MoreFertilizers.HarmonyPatches.Compat;

/// <summary>
/// Holds transpiler patches against Miller Time to handle organic crops.
/// </summary>
internal static class MillerTimeDayUpdateTranspiler
{
    /// <summary>
    /// Applies patches against Miller Time.
    /// </summary>
    /// <param name="harmony">Harmony reference.</param>
    /// <exception cref="MethodNotFoundException">Some type or something wasn't found.</exception>
    internal static void ApplyPatches(Harmony harmony)
    {
        try
        {
            Type millerpatches = AccessTools.TypeByName("MillerTime.Patches.MillPatch")
                ?? ReflectionThrowHelper.ThrowMethodNotFoundException<Type>("MillerTime Patches");
            harmony.Patch(
                original: millerpatches.StaticMethodNamed("DayUpdatePrefix"),
                transpiler: new HarmonyMethod(typeof(MillerTimeDayUpdateTranspiler), nameof(Transpiler)));
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Mod failed while transpiling Miller Time. Integration may not work.\n\n{ex}", LogLevel.Error);
        }
    }

#pragma warning disable SA1116 // Split parameters should start on line after declaration
    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);
            helper.DeclareLocal(typeof(Item), out LocalBuilder? inputlocal)
            .FindNext(new CodeInstructionWrapper[]
            { // Find the call to get_Item.
                new(OpCodes.Callvirt, typeof(NetFieldBase<Chest, NetRef<Chest>>).GetCachedProperty("Value", ReflectionCache.FlagTypes.InstanceFlags).GetGetMethod()),
                new(OpCodes.Ldfld, typeof(Chest).GetCachedField(nameof(Chest.items), ReflectionCache.FlagTypes.InstanceFlags)),
                new(SpecialCodeInstructionCases.LdLoc),
                new(OpCodes.Callvirt, typeof(NetList<Item, NetRef<Item>>).GetCachedProperty("Item", ReflectionCache.FlagTypes.InstanceFlags).GetGetMethod()),
            })
            .Advance(4)
            .Insert(new CodeInstruction[]
            { // This is sufficiently annoying we're just going to create a local to store it.
              // Bloody hoisted fields.
                new(OpCodes.Stloc, inputlocal),
                new(OpCodes.Ldloc, inputlocal),
            })
            .FindNext(new CodeInstructionWrapper[]
            { // find and store the output's local.
                new(OpCodes.Newobj, typeof(SObject).GetCachedConstructor(ReflectionCache.FlagTypes.InstanceFlags, new[] { typeof(int), typeof(int), typeof(bool), typeof(int), typeof(int) })),
                new(SpecialCodeInstructionCases.StLoc),
            }).Advance(1);

            CodeInstruction? stoutput = helper.CurrentInstruction.Clone();
            CodeInstruction? ldoutput = helper.CurrentInstruction.ToLdLoc();

            helper.Advance(1)
            .GetLabels(out IList<Label> labelsToMove)
            .Insert(new CodeInstruction[]
            { // Place our function call here.
                new(OpCodes.Ldloc, inputlocal),
                ldoutput,
                new(OpCodes.Call, typeof(MillDayUpdateTranspiler).GetCachedMethod(nameof(MillDayUpdateTranspiler.MakeMillOutputOrganic), ReflectionCache.FlagTypes.StaticFlags)),
                stoutput,
            }, withLabels: labelsToMove);

            // helper.Print();
            return helper.Render();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Mod crashed while transpiling Miller Time:\n\n{ex}", LogLevel.Error);
            original?.Snitch(ModEntry.ModMonitor);
        }
        return null;
    }
#pragma warning restore SA1116 // Split parameters should start on line after declaration
}