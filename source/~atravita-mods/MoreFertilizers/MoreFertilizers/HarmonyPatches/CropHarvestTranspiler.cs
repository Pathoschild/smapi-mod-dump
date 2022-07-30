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
using System.Runtime.CompilerServices;
using AtraBase.Toolkit;
using AtraBase.Toolkit.Reflection;
using AtraCore.Framework.ReflectionManager;
using AtraShared.Utils.Extensions;
using AtraShared.Utils.HarmonyHelper;
using HarmonyLib;
using Microsoft.Xna.Framework;
using MoreFertilizers.Framework;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;

namespace MoreFertilizers.HarmonyPatches;

/// <summary>
/// Holds the transpiler against Crop.harvest.
/// Covers organic, bountiful, and joja fertilizers.
/// </summary>
[HarmonyPatch(typeof(Crop))]
internal static class CropHarvestTranspiler
{
    /// <summary>
    /// Applies a matching patch to DGA's crop.Harvest.
    /// </summary>
    /// <param name="harmony">Harmony instance.</param>
    internal static void ApplyDGAPatch(Harmony harmony)
    {
        try
        {
            Type dgaCrop = AccessTools.TypeByName("DynamicGameAssets.Game.CustomCrop")
                ?? ReflectionThrowHelper.ThrowMethodNotFoundException<Type>("DGA Crop");
            harmony.Patch(
                original: dgaCrop.GetCachedMethod("Harvest", ReflectionCache.FlagTypes.InstanceFlags),
                transpiler: new HarmonyMethod(typeof(CropHarvestTranspiler), nameof(TranspileDGA)));
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Mod crashed while transpiling DGA. Integration may not work correctly.\n\n{ex}", LogLevel.Error);
        }
    }

    [MethodImpl(TKConstants.Hot)]
    private static int GetQualityForJojaFert(int prevQual, HoeDirt? dirt)
    {
        if(dirt is not null && dirt.fertilizer.Value != -1)
        {
            if (dirt.fertilizer.Value == ModEntry.JojaFertilizerID)
            {
                return 1;
            }
            else if (dirt.fertilizer.Value == ModEntry.DeluxeJojaFertilizerID)
            {
                return Game1.random.NextDouble() < 0.2 ? 2 : 1;
            }
        }
        return prevQual;
    }

    [MethodImpl(TKConstants.Hot)]
    private static Item? MakeItemOrganic(Item? item, HoeDirt? dirt)
        => item is SObject obj ? MakeObjectOrganic(obj, dirt) : item;

    [MethodImpl(TKConstants.Hot)]
    private static SObject MakeObjectOrganic(SObject obj, HoeDirt? dirt)
    {
        if (dirt is not null && dirt.fertilizer.Value != -1)
        {
            if (dirt.fertilizer.Value == ModEntry.OrganicFertilizerID && !obj.Name.Contains("Joja", StringComparison.OrdinalIgnoreCase))
            {
                obj.modData?.SetBool(CanPlaceHandler.Organic, true);
                obj.Price = (int)(obj.Price * 1.1);
                obj.Name += " (Organic)";
                obj.MarkContextTagsDirty();
            }
            else if (dirt.fertilizer.Value == ModEntry.DeluxeJojaFertilizerID || dirt.fertilizer.Value == ModEntry.JojaFertilizerID)
            {
                obj.modData?.SetBool(CanPlaceHandler.Joja, true);
                obj.MarkContextTagsDirty();
            }
        }
        return obj;
    }

    [MethodImpl(TKConstants.Hot)]
    private static int IncrementForBountiful(int prevValue, HoeDirt? dirt)
    {
        if (ModEntry.BountifulFertilizerID != -1 && dirt?.fertilizer?.Value == ModEntry.BountifulFertilizerID
            && Game1.random.NextDouble() < 0.1)
        {
            ModEntry.ModMonitor.DebugOnlyLog("IncrementedOnceForBountiful", LogLevel.Info);
            return prevValue * 2;
        }
        return prevValue;
    }

#pragma warning disable SA1116 // Split parameters should start on line after declaration
    [HarmonyPatch(nameof(Crop.harvest))]
    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);
            helper.FindNext(new CodeInstructionWrapper[]
            {// if (this.forageCrop) and advance past this
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, typeof(Crop).GetCachedField(nameof(Crop.forageCrop), ReflectionCache.FlagTypes.InstanceFlags)),
                new(OpCodes.Call),
                new(OpCodes.Brfalse),
            })
            .Advance(3)
            .StoreBranchDest()
            .AdvanceToStoredLabel()
            .FindNext(new CodeInstructionWrapper[]
            { // find if (this.minHarvest > 1 || this.maxHarvest < 1)
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, typeof(Crop).GetCachedField(nameof(Crop.maxHarvest), ReflectionCache.FlagTypes.InstanceFlags)),
                new(OpCodes.Call),
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Ble_S),
            })
            .Advance(4)
            .StoreBranchDest() // We'll be sticking our incrementer at the end of this block, so store the branch dest so we can go there later.
            .FindNext(new CodeInstructionWrapper[]
            {// Advance into that block, find num = random2.Next(this.minHarvest + ...). This lets us grab the right local.
                new(OpCodes.Add),
                new(OpCodes.Call, typeof(Math).GetCachedMethod(nameof(Math.Max), ReflectionCache.FlagTypes.StaticFlags, new[] { typeof(int), typeof(int) } )),
                new(OpCodes.Callvirt, typeof(Random).GetCachedMethod(nameof(Random.Next), ReflectionCache.FlagTypes.InstanceFlags, new[] { typeof(int), typeof(int) })),
                new(SpecialCodeInstructionCases.StLoc),
            })
            .FindNext(new CodeInstructionWrapper[]
            {
                new(SpecialCodeInstructionCases.StLoc),
            });

            CodeInstruction numberToHarvestLdLoc = helper.CurrentInstruction.ToLdLoc();
            CodeInstruction numberToHarvestStLoc = helper.CurrentInstruction.Clone();

            // Advance out of that block.
            helper.AdvanceToStoredLabel()
            .GetLabels(out IList<Label>? numAdjustLabels, clear: true)
            .Insert(new CodeInstruction[]
            { // and insert our incrementer.
                numberToHarvestLdLoc,
                new(OpCodes.Ldarg_3),
                new(OpCodes.Call, typeof(CropHarvestTranspiler).GetCachedMethod(nameof(IncrementForBountiful), ReflectionCache.FlagTypes.StaticFlags)),
                numberToHarvestStLoc,
            }, withLabels: numAdjustLabels)
            .FindNext(new CodeInstructionWrapper[]
            { // find if(this.indexOfHarvest == )
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld),
                new(OpCodes.Call),
                new(OpCodes.Ldc_I4, 771),
            })
            .Push() // we'll be inserting the quality just before this, so temporarily save it.
            .FindNext(new CodeInstructionWrapper[]
            {
                new(OpCodes.Ldc_I4_0),
                new(SpecialCodeInstructionCases.StLoc),
            })
            .Advance(1);

            CodeInstruction qualityStLoc = helper.CurrentInstruction.Clone();
            CodeInstruction qualityLdLoc = helper.CurrentInstruction.ToLdLoc();

            helper.Pop()
            .GetLabels(out IList<Label>? jojaLabels, clear: true)
            .Insert(new CodeInstruction[]
            {
                qualityLdLoc,
                new(OpCodes.Ldarg_3), // HoeDirt soil
                new(OpCodes.Call, typeof(CropHarvestTranspiler).GetCachedMethod(nameof(GetQualityForJojaFert), ReflectionCache.FlagTypes.StaticFlags)),
                qualityStLoc,
            }, withLabels: jojaLabels)
            .FindNext(new CodeInstructionWrapper[]
            { // if (this.programColored)
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, typeof(Crop).GetCachedField(nameof(Crop.programColored), ReflectionCache.FlagTypes.InstanceFlags)),
                new(OpCodes.Call),
            })
            .FindNext(new CodeInstructionWrapper[]
            {
                new(SpecialCodeInstructionCases.LdLoc),
                new(OpCodes.Callvirt, typeof(SObject).GetCachedProperty(nameof(SObject.Quality), ReflectionCache.FlagTypes.InstanceFlags).GetSetMethod()),
                new(SpecialCodeInstructionCases.StLoc, typeof(SObject)),
            })
            .Advance(2)
            .GetLabels(out IList<Label>? firstSObjectCreationLabels, clear: true)
            .Insert(new CodeInstruction[]
            { // Insert function to make the object organic if needed.
                new(OpCodes.Ldarg_3),
                new (OpCodes.Call, typeof(CropHarvestTranspiler).GetCachedMethod(nameof(MakeObjectOrganic), ReflectionCache.FlagTypes.StaticFlags)),
            }, withLabels: firstSObjectCreationLabels)
            .FindNext(new CodeInstructionWrapper[]
            {// if (this.programColored), the second instance.
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, typeof(Crop).GetCachedField(nameof(Crop.programColored), ReflectionCache.FlagTypes.InstanceFlags)),
                new(OpCodes.Call),
            })
            .FindNext(new CodeInstructionWrapper[]
            { // Find the place where the second creation of an SObject/ColoredSObject is saved.
                new (OpCodes.Newobj, typeof(ColoredObject).GetCachedConstructor(ReflectionCache.FlagTypes.InstanceFlags, new[] { typeof(int), typeof(int), typeof(Color) } )),
                new (SpecialCodeInstructionCases.StLoc, typeof(SObject)),
            })
            .Advance(1);

            CodeInstruction secondSObjectLdLoc = helper.CurrentInstruction.ToLdLoc();
            CodeInstruction secondSObjectStLoc = helper.CurrentInstruction.Clone();

            helper.GetLabels(out IList<Label>? secondSObjectCreationLabels, clear: true)
            .Insert(new CodeInstruction[]
            { // Insert function to make the object organic if needed.
                new(OpCodes.Ldarg_3),
                new(OpCodes.Call, typeof(CropHarvestTranspiler).GetCachedMethod(nameof(MakeObjectOrganic), ReflectionCache.FlagTypes.StaticFlags)),
            }, withLabels: secondSObjectCreationLabels)
            .Advance(1)
            .Insert(new CodeInstruction[]
            {// Insert instructions to adjust the quality here too.
                secondSObjectLdLoc,
                new(OpCodes.Ldc_I4_0),
                new(OpCodes.Ldarg_3),
                new(OpCodes.Call, typeof(CropHarvestTranspiler).GetCachedMethod(nameof(GetQualityForJojaFert), ReflectionCache.FlagTypes.StaticFlags)),
                new(OpCodes.Callvirt, typeof(SObject).GetCachedProperty(nameof(SObject.Quality), ReflectionCache.FlagTypes.InstanceFlags).GetSetMethod()),
            });

            // helper.Print();
            return helper.Render();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Mod crashed while transpiling Crop.harvest:\n\n{ex}", LogLevel.Error);
            original?.Snitch(ModEntry.ModMonitor);
        }
        return null;
    }

    private static IEnumerable<CodeInstruction>? TranspileDGA(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);
            helper.FindNext(new CodeInstructionWrapper[]
            { // match the second half of if (fertilizerQualityLevel >= 3 && r.NextDobule() < chanceForGoldQuality/2
              // DaLion might take out the first half. Dunno.
                new(SpecialCodeInstructionCases.LdLoc),
                new(OpCodes.Callvirt, typeof(Random).GetCachedMethod(nameof(Random.NextDouble), ReflectionCache.FlagTypes.InstanceFlags)),
                new(SpecialCodeInstructionCases.LdLoc),
                new(OpCodes.Ldc_R8, 2d),
                new(OpCodes.Div),
                new(OpCodes.Bge_Un_S),
            })
            .FindNext(new CodeInstructionWrapper[]
            { // quality = 4
                new(OpCodes.Ldc_I4_4),
                new(SpecialCodeInstructionCases.StLoc),
                new(OpCodes.Br_S),
            })
            .Advance(1);

            // and grab local
            CodeInstruction qualityStLoc = helper.CurrentInstruction.Clone();
            CodeInstruction qualityLdLoc = helper.CurrentInstruction.ToLdLoc();

            // Grab the branch point so we can exit this block entirely
            helper.Advance(1)
            .StoreBranchDest()
            .AdvanceToStoredLabel()
            .GetLabels(out IList<Label>? qualityLabels, clear: true)
            .Insert(new CodeInstruction[]
            {
                qualityLdLoc,
                new(OpCodes.Ldarg_3), // HoeDirt soil
                new(OpCodes.Call, typeof(CropHarvestTranspiler).GetCachedMethod(nameof(GetQualityForJojaFert), ReflectionCache.FlagTypes.StaticFlags)),
                qualityStLoc,
            }, withLabels: qualityLabels);

            Type cropPackData = AccessTools.TypeByName("DynamicGameAssets.PackData.CropPackData")
                ?? ReflectionThrowHelper.ThrowMethodNotFoundException<Type>("DGA crop data");
            Type harvestData = cropPackData.GetNestedType("HarvestedDropData")
                ?? ReflectionThrowHelper.ThrowMethodNotFoundException<Type>("Harvested data!");

            helper.FindNext(new CodeInstructionWrapper[]
            { // data.MaximumHarvestedQuantity > 1
                new(SpecialCodeInstructionCases.LdLoc),
                new(OpCodes.Callvirt, harvestData.GetCachedProperty("MaximumHarvestedQuantity", ReflectionCache.FlagTypes.InstanceFlags).GetGetMethod()),
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Ble_S),
            })
            .Advance(3)
            .StoreBranchDest() // Incrementer will be placed at the end of this block, store the destination to get there next.
            .FindNext(new CodeInstructionWrapper[]
            {// Advance into that block, find num = random2.Next(data.MininumHarvestedQuality + ...). This lets us grab the right local.
                new(OpCodes.Add),
                new(OpCodes.Call, typeof(Math).GetCachedMethod(nameof(Math.Max), ReflectionCache.FlagTypes.StaticFlags, new[] { typeof(int), typeof(int) } )),
                new(OpCodes.Callvirt, typeof(Random).GetCachedMethod(nameof(Random.Next), ReflectionCache.FlagTypes.InstanceFlags, new[] { typeof(int), typeof(int) })),
                new(SpecialCodeInstructionCases.StLoc),
            })
            .FindNext(new CodeInstructionWrapper[]
            {
                new(SpecialCodeInstructionCases.StLoc),
            });

            CodeInstruction numberToHarvestLdLoc = helper.CurrentInstruction.ToLdLoc();
            CodeInstruction numberToHarvestStLoc = helper.CurrentInstruction.Clone();

            // Advance out of that block.
            helper.AdvanceToStoredLabel()
            .GetLabels(out IList<Label>? numAdjustLabels, clear: true)
            .Insert(new CodeInstruction[]
            { // and insert our incrementer.
                numberToHarvestLdLoc,
                new(OpCodes.Ldarg_3),
                new(OpCodes.Call, typeof(CropHarvestTranspiler).GetCachedMethod(nameof(IncrementForBountiful), ReflectionCache.FlagTypes.StaticFlags)),
                numberToHarvestStLoc,
            }, withLabels: numAdjustLabels);

            Type itemAbstraction = AccessTools.TypeByName("DynamicGameAssets.ItemAbstraction")
                ?? ReflectionThrowHelper.ThrowMethodNotFoundException<Type>("Item Abstraction");

            helper.FindNext(new CodeInstructionWrapper[]
            {
                new(OpCodes.Call, itemAbstraction.GetCachedMethod("Create", ReflectionCache.FlagTypes.InstanceFlags)),
                new(SpecialCodeInstructionCases.StLoc, typeof(Item)),
            })
            .Advance(1)
            .Insert(new CodeInstruction[]
            {
                new(OpCodes.Ldarg_3),
                new(OpCodes.Call, typeof(CropHarvestTranspiler).GetCachedMethod(nameof(MakeItemOrganic), ReflectionCache.FlagTypes.StaticFlags)),
            });

            // helper.Print();
            return helper.Render();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Mod crashed while transpiling Crop.harvest:\n\n{ex}", LogLevel.Error);
            original?.Snitch(ModEntry.ModMonitor);
        }
        return null;
    }
#pragma warning restore SA1116 // Split parameters should start on line after declaration
}