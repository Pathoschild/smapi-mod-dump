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

namespace MoreFertilizers.HarmonyPatches.Acquisition;

/// <summary>
/// Transpiles my fertilizers into the fair stock.
/// </summary>
[HarmonyPatch(typeof(Event))]
internal static class FairShopTranspiler
{
    private static Dictionary<ISalable, int[]> AppendToShopStock(Dictionary<ISalable, int[]> shopStock)
    {
        try
        {
            if (Utility.hasFinishedJojaRoute())
            {
                if (ModEntry.JojaFertilizerID != -1)
                {
                    shopStock.TryAdd(new SObject(ModEntry.JojaFertilizerID, 1), new[] { 200, 20 });
                }
                if (ModEntry.DeluxeJojaFertilizerID != -1)
                {
                    shopStock.TryAdd(new SObject(ModEntry.DeluxeJojaFertilizerID, 1), new[] { 400, 20 });
                }
                if (ModEntry.SecretJojaFertilizerID != -1)
                {
                    shopStock.TryAdd(new SObject(ModEntry.SecretJojaFertilizerID, 1), new[] { 1000, 4 });
                }
            }
            else
            {
                if (ModEntry.BountifulFertilizerID != -1)
                {
                    shopStock.TryAdd(new SObject(ModEntry.BountifulFertilizerID, 1), new[] { 200, 20 });
                }
                if (ModEntry.OrganicFertilizerID != -1)
                {
                    shopStock.TryAdd(new SObject(ModEntry.OrganicFertilizerID, 1), new[] { 200, 20 });
                }
            }
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Mod somehow failed while adding to shop stock?\n\n{ex}", LogLevel.Error);
        }
        return shopStock;
    }

#pragma warning disable SA1116 // Split parameters should start on line after declaration
    [HarmonyPatch(nameof(Event.checkAction))]
    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);
            helper.FindNext(new CodeInstructionWrapper[]
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, typeof(Event).GetCachedField("festivalShops", ReflectionCache.FlagTypes.InstanceFlags)),
                new(OpCodes.Ldstr, "starTokenShop"),
            })
            .FindNext(new CodeInstructionWrapper[]
            {
                new(OpCodes.Newobj, typeof(Dictionary<ISalable, int[]>).GetCachedConstructor(ReflectionCache.FlagTypes.InstanceFlags)),
                new(SpecialCodeInstructionCases.StLoc),
            })
            .Advance(1);

            CodeInstruction ldloc = helper.CurrentInstruction.ToLdLoc();
            CodeInstruction stloc = helper.CurrentInstruction.ToStLoc();

            helper.FindNext(new CodeInstructionWrapper[]
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, typeof(Event).GetCachedField("festivalShops", ReflectionCache.FlagTypes.InstanceFlags)),
                new(OpCodes.Ldstr, "starTokenShop"),
            })
            .GetLabels(out IList<Label>? labelsToMove, clear: true)
            .Insert(new CodeInstruction[]
            {
                ldloc,
                new(OpCodes.Call, typeof(FairShopTranspiler).GetCachedMethod(nameof(AppendToShopStock), ReflectionCache.FlagTypes.StaticFlags)),
                stloc,
            }, withLabels: labelsToMove);

            return helper.Render();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Mod crashed while transpiling Event.checkaction:\n\n{ex}", LogLevel.Error);
            original?.Snitch(ModEntry.ModMonitor);
        }
        return null;
    }
#pragma warning restore SA1116 // Split parameters should start on line after declaration
}