/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StopRugRemoval
**
*************************************************/

#if DEBUG

using System.Reflection;
using System.Reflection.Emit;
using AtraBase.Toolkit.Reflection;
using AtraShared.Utils.HarmonyHelper;
using HarmonyLib;
using StardewValley.Locations;

namespace StopRugRemoval.HarmonyPatches.Beverages;

/// <summary>
/// Replaces the coffee at the night market with a random beverage.
/// </summary>
[HarmonyPatch(typeof(BeachNightMarket))]
internal static class ReplaceBeverage
{
    private static readonly Lazy<List<int>> LazyBeverages = new(GetBeverageIDs);

    /// <summary>
    /// Gets the item ID of a random beverage.
    /// </summary>
    /// <returns>int ID of beverage.</returns>
    public static int GetRandomBeverageId()
    {
        return Utility.GetRandom(LazyBeverages.Value);
    }

    [HarmonyPatch(nameof(BeachNightMarket.getFreeGift))]
    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);
            helper.FindNext(new CodeInstructionWrapper[]
                    {
                        new(OpCodes.Ldc_I4, 395),
                        new(OpCodes.Ldc_I4_1),
                        new(OpCodes.Ldc_I4_0),
                        new(OpCodes.Ldc_I4_M1),
                        new(OpCodes.Ldc_I4_0),
                    })
                .ReplaceInstruction(new(OpCodes.Call, typeof(ReplaceBeverage).StaticMethodNamed(nameof(ReplaceBeverage.GetRandomBeverageId))), keepLabels: true);
            return helper.Render();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Transpiler for Night Market Beverages failed with error {ex}", LogLevel.Error);
        }
        return null;
    }

    private static List<int> GetBeverageIDs()
    {
        List<int> beverageIds = new();
        foreach ((int key, string value) in Game1.objectInformation)
        {
            string[] splitvals = value.Split('/');
            if (splitvals.Length > 6 && splitvals[6].Contains("drink", StringComparison.OrdinalIgnoreCase))
            {
                beverageIds.Add(key);
            }
        }
        return beverageIds;
    }
}
#endif