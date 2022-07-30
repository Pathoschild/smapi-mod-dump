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
using AtraCore.Framework.ReflectionManager;
using AtraShared.Utils.Extensions;
using AtraShared.Utils.HarmonyHelper;
using HarmonyLib;
using StardewValley.Tools;

namespace MoreFertilizers.HarmonyPatches.Acquisition;

/// <summary>
/// Transpiler to insert additional items into fishing treasure chests.
/// </summary>
[HarmonyPatch(typeof(FishingRod))]
internal static class FishingTreasureTranspiler
{
    private static SObject? GetPossibleRandomFertilizer()
    {
        if (Game1.random.NextDouble() < 0.10)
        {
            int fertilizerToDrop = Game1.player.fishingLevel.Value.GetRandomFertilizerFromLevel();
            if (fertilizerToDrop != -1)
            {
                return new SObject(fertilizerToDrop, Game1.random.Next(1, 4 + (int)(Game1.player.DailyLuck * 20)));
            }
        }
        return null;
    }

#pragma warning disable SA1116 // Split parameters should start on line after declaration
    [HarmonyPatch(nameof(FishingRod.openTreasureMenuEndFunction))]
    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);
            helper.FindNext(new CodeInstructionWrapper[]
            { // find this.doneFishing
                new(OpCodes.Call, typeof(FishingRod).GetCachedMethod(nameof(FishingRod.doneFishing), ReflectionCache.FlagTypes.InstanceFlags)),
            })
            .FindNext(new CodeInstructionWrapper[]
            { // find the constructor for List<Item>, which is used to hold the treasure.
                new(OpCodes.Newobj, typeof(List<Item>).GetCachedConstructor(ReflectionCache.FlagTypes.InstanceFlags)),
                new(SpecialCodeInstructionCases.StLoc),
            })
            .Advance(1);

            // Grab it's local.
            CodeInstruction ldloc = helper.CurrentInstruction.ToLdLoc();
            Label noObject = helper.Generator.DefineLabel();

            helper.Advance(1)
            .GetLabels(out IList<Label>? labels, clear: true)
            .DefineAndAttachLabel(out Label finish)
            .Insert(new CodeInstruction[]
            { // insert if(GetPossibleRandomFertilizer() is SObject obj) treasureList.Add(obj);
                ldloc,
                new(OpCodes.Call, typeof(FishingTreasureTranspiler).GetCachedMethod(nameof(GetPossibleRandomFertilizer), ReflectionCache.FlagTypes.StaticFlags)),
                new(OpCodes.Dup),
                new(OpCodes.Brfalse_S, noObject),
                new(OpCodes.Call, typeof(List<Item>).GetCachedMethod("Add", ReflectionCache.FlagTypes.InstanceFlags)),
                new(OpCodes.Br_S, finish),
                new CodeInstruction(OpCodes.Pop).WithLabels(noObject),
                new(OpCodes.Pop),
            }, withLabels: labels);

            return helper.Render();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Mod crashed while transpiling FishingRod.openTreasureMenuEndFunction:\n\n{ex}", LogLevel.Error);
            original?.Snitch(ModEntry.ModMonitor);
        }
        return null;
    }
#pragma warning restore SA1116 // Split parameters should start on line after declaration
}