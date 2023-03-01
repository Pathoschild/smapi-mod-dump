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
using AtraBase.Toolkit.Extensions;
using AtraBase.Toolkit.Reflection;

using AtraCore.Framework.ReflectionManager;

using AtraShared.Utils.Extensions;
using AtraShared.Utils.HarmonyHelper;
using AtraShared.Wrappers;

using HarmonyLib;

using Microsoft.Xna.Framework;

using MoreFertilizers.Framework;

using Netcode;

using StardewValley.Characters;
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
    private const string DGAModDataKey = "atravita.MoreFertilizers/DGASeedID";

    private static bool hasQualityMod = false;

    internal static void Initialize(IModRegistry registry)
    {
        hasQualityMod = registry.IsLoaded("spacechase0.AQualityMod");
    }

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

    #region helpers
    [MethodImpl(TKConstants.Hot)]
    private static int GetQualityForJojaFert(int prevQual, HoeDirt? dirt)
    {
        // the -1 is the JA not-found number.
        if (dirt is not null && dirt.fertilizer.Value != -1)
        {
            if (dirt.fertilizer.Value == ModEntry.JojaFertilizerID)
            {
                return 1;
            }
            else if (dirt.fertilizer.Value == ModEntry.DeluxeJojaFertilizerID)
            {
                return Game1.random.Next(5) == 0 ? 2 : 1;
            }
            else if (dirt.fertilizer.Value == ModEntry.SecretJojaFertilizerID)
            {
                return hasQualityMod
                    ? ((Game1.random.Next(4) != 0 || dirt.HasJojaCrop()) ? -2 : 1)
                    : ((Game1.random.Next(2) == 0 && !dirt.HasJojaCrop()) ? 1 : 0);
            }
        }
        return prevQual;
    }

    // Handles organic/beverage fertilizer for DGA.
    [MethodImpl(TKConstants.Hot)]
    private static Item? HandleOrganicAndBeverageItem(Item? item, HoeDirt? dirt, JunimoHarvester? junimo)
    {
        HandleBeverageFertilizer(item, dirt, junimo);
        return item is SObject obj ? HandleOrganicAndBeverageItem(obj, dirt, junimo) : item;
    }

    [MethodImpl(TKConstants.Hot)]
    private static SObject HandleOrganicAndBeverage(SObject obj, HoeDirt? dirt, JunimoHarvester? junimo)
    {
        HandleBeverageFertilizer(obj, dirt, junimo);
        return MakeObjectOrganic(obj, dirt);
    }

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
            else if (dirt.fertilizer.Value == ModEntry.DeluxeJojaFertilizerID
                || dirt.fertilizer.Value == ModEntry.JojaFertilizerID
                || dirt.fertilizer.Value == ModEntry.SecretJojaFertilizerID)
            {
                obj.modData?.SetBool(CanPlaceHandler.Joja, true);
                obj.MarkContextTagsDirty();
            }
        }
        return obj;
    }

    // Drops the beverage if needed.
    [MethodImpl(TKConstants.Hot)]
    private static void HandleBeverageFertilizer(Item? item, HoeDirt? dirt, JunimoHarvester? junimo)
    {
        if (dirt is not null && dirt.fertilizer.Value != -1 && item is not null)
        {
            if (dirt.fertilizer.Value == ModEntry.MiraculousBeveragesID
                && MiraculousFertilizerHandler.GetBeverage(item) is SObject beverage)
            {
                if (junimo is not null)
                {
                    junimo.tryToAddItemToHut(beverage);
                }
                else
                {
                    Game1.createItemDebris(beverage, dirt.currentTileLocation * 64f, -1);
                }
            }
        }
    }

    [MethodImpl(TKConstants.Hot)]
    private static int IncrementForBountiful(int prevValue, HoeDirt? dirt)
    {
        if (ModEntry.BountifulFertilizerID != -1 && dirt?.fertilizer?.Value == ModEntry.BountifulFertilizerID
            && Game1.random.Next(10) == 0)
        {
            ModEntry.ModMonitor.DebugOnlyLog("IncrementedOnceForBountiful", LogLevel.Info);
            return prevValue * 2;
        }
        return prevValue;
    }

    [MethodImpl(TKConstants.Hot)]
    private static int AdjustExperience(int prevValue, HoeDirt? dirt)
    {
        if (ModEntry.WisdomFertilizerID != -1 && dirt?.fertilizer?.Value == ModEntry.WisdomFertilizerID)
        {
            return (1.5 * prevValue).RandomRoundProportional();
        }
        return prevValue;
    }

    [MethodImpl(TKConstants.Hot)]
    private static int AdjustRegrow(int prevValue, HoeDirt? dirt)
    {
        if (ModEntry.SecretJojaFertilizerID != -1 && dirt?.fertilizer?.Value == ModEntry.SecretJojaFertilizerID
            && (Game1.random.Next(2) == 0 || dirt.HasJojaCrop()))
        {
            return Math.Max(1, ((hasQualityMod ? 0.65 : 0.8) * prevValue).RandomRoundProportional());
        }
        return prevValue;
    }

    [MethodImpl(TKConstants.Hot)]
    private static void DropSeedsForSeedyFertilizer(int x, int y, HoeDirt? dirt, JunimoHarvester? jumino)
    {
        if (dirt?.crop is not null && Game1.random.Next(10) == 0
            && ModEntry.SeedyFertilizerID != -1 && dirt.fertilizer?.Value == ModEntry.SeedyFertilizerID)
        {
            dirt.crop.InferSeedIndex();
            int seedIndex = dirt.crop.rowInSpriteSheet.Value != Crop.rowOfWildSeeds ? dirt.crop.netSeedIndex.Value : dirt.crop.whichForageCrop.Value;

            if (Game1Wrappers.ObjectInfo.ContainsKey(seedIndex))
            {
                SObject seeds = new(seedIndex, Game1.random.Next(3));
                if (jumino is null)
                {
                    Game1.createItemDebris(seeds, new Vector2((x * Game1.tileSize) + 32, (y * Game1.tileSize) + 32), -1, dirt.currentLocation);
                }
                else
                {
                    jumino.tryToAddItemToHut(seeds);
                }
            }
        }
    }

    #endregion

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
                new(OpCodes.Ldarg, 4),
                new (OpCodes.Call, typeof(CropHarvestTranspiler).GetCachedMethod(nameof(HandleOrganicAndBeverage), ReflectionCache.FlagTypes.StaticFlags)),
            }, withLabels: firstSObjectCreationLabels)
            .FindNext(new CodeInstructionWrapper[]
            { // find the sunflower seeds block (421)
                new (OpCodes.Ldarg_0),
                new (OpCodes.Ldfld, typeof(Crop).GetCachedField(nameof(Crop.indexOfHarvest), ReflectionCache.FlagTypes.InstanceFlags)),
                new (OpCodes.Call), // this is a op_Implicit
                new (OpCodes.Ldc_I4, 421),
            })
            .GetLabels(out IList<Label>? seedyLabels)
            .Insert(new CodeInstruction[]
            { // and just insert the seed fertilizer just before it.
                new(OpCodes.Ldarg_1),
                new(OpCodes.Ldarg_2),
                new(OpCodes.Ldarg_3),
                new(OpCodes.Ldarg, 4),
                new(OpCodes.Call, typeof(CropHarvestTranspiler).GetCachedMethod(nameof(DropSeedsForSeedyFertilizer), ReflectionCache.FlagTypes.StaticFlags)),
            }, withLabels: seedyLabels)
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
            })
            .FindNext(new CodeInstructionWrapper[]
            { // Find the block where the player is given XP
                new(OpCodes.Call, typeof(Game1).GetCachedProperty(nameof(Game1.player), ReflectionCache.FlagTypes.StaticFlags).GetGetMethod()),
                new(OpCodes.Ldc_I4_0),
                new(SpecialCodeInstructionCases.LdLoc),
            })
            .FindNext(new CodeInstructionWrapper[]
            {
                new(OpCodes.Conv_I4),
                new(OpCodes.Callvirt, typeof(Farmer).GetCachedMethod(nameof(Farmer.gainExperience), ReflectionCache.FlagTypes.InstanceFlags)),
            })
            .Advance(1)
            .Insert(new CodeInstruction[]
            { // insert a call to a function that changes the experience gained.
                new(OpCodes.Ldarg_3),
                new(OpCodes.Call, typeof(CropHarvestTranspiler).GetCachedMethod(nameof(AdjustExperience), ReflectionCache.FlagTypes.StaticFlags)),
            })
            .FindNext(new CodeInstructionWrapper[]
            { // find this.dayOfCurrentPhase.Value = this.regrowAfterHarvest
                new(OpCodes.Ldfld, typeof(Crop).GetCachedField(nameof(Crop.regrowAfterHarvest), ReflectionCache.FlagTypes.InstanceFlags)),
                new(OpCodes.Call),
                new(OpCodes.Callvirt, typeof(NetFieldBase<int, NetInt>).GetCachedProperty("Value", ReflectionCache.FlagTypes.InstanceFlags).GetSetMethod()),
            })
            .Advance(2)
            .Insert(new CodeInstruction[]
            {
                new(OpCodes.Ldarg_3),
                new(OpCodes.Call, typeof(CropHarvestTranspiler).GetCachedMethod(nameof(AdjustRegrow), ReflectionCache.FlagTypes.StaticFlags)),
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

#warning - todo: transpile DGA for the seedy fertilizer.
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
                new(OpCodes.Ldarg, 4),
                new(OpCodes.Call, typeof(CropHarvestTranspiler).GetCachedMethod(nameof(HandleOrganicAndBeverageItem), ReflectionCache.FlagTypes.StaticFlags)),
            })
            .FindNext(new CodeInstructionWrapper[]
            { // Find the block where the player is given XP
                new(OpCodes.Call, typeof(Game1).GetCachedProperty(nameof(Game1.player), ReflectionCache.FlagTypes.StaticFlags).GetGetMethod()),
                new(OpCodes.Ldc_I4_0),
                new(SpecialCodeInstructionCases.LdLoc),
            })
            .FindNext(new CodeInstructionWrapper[]
            {
                new(OpCodes.Conv_I4),
                new(OpCodes.Callvirt, typeof(Farmer).GetCachedMethod(nameof(Farmer.gainExperience), ReflectionCache.FlagTypes.InstanceFlags)),
            })
            .Advance(1)
            .Insert(new CodeInstruction[]
            { // insert a call to a function that changes the experience gained.
                new(OpCodes.Ldarg_3),
                new(OpCodes.Call, typeof(CropHarvestTranspiler).GetCachedMethod(nameof(AdjustExperience), ReflectionCache.FlagTypes.StaticFlags)),
            });

            Type phasedata = cropPackData.GetNestedType("PhaseData")
                ?? ReflectionThrowHelper.ThrowMethodNotFoundException<Type>("DGA phase data");

            helper.FindNext(new CodeInstructionWrapper[]
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, typeof(Crop).GetCachedField(nameof(Crop.currentPhase), ReflectionCache.FlagTypes.InstanceFlags)),
                new(SpecialCodeInstructionCases.LdLoc),
                new(OpCodes.Callvirt, phasedata.GetCachedProperty("HarvestedNewPhase", ReflectionCache.FlagTypes.InstanceFlags).GetGetMethod()),
                new(OpCodes.Callvirt, typeof(NetFieldBase<int, NetInt>).GetCachedProperty("Value", ReflectionCache.FlagTypes.InstanceFlags).GetSetMethod()),
            })
            .Advance(4)
            .Insert(new CodeInstruction[]
            {
                new(OpCodes.Ldarg_3),
                new(OpCodes.Call, typeof(CropHarvestTranspiler).GetCachedMethod(nameof(AdjustRegrow), ReflectionCache.FlagTypes.StaticFlags)),
            });

            // helper.Print();
            return helper.Render();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Mod crashed while transpiling DGA's Crop.harvest:\n\n{ex}", LogLevel.Error);
            original?.Snitch(ModEntry.ModMonitor);
        }
        return null;
    }
#pragma warning restore SA1116 // Split parameters should start on line after declaration
}
