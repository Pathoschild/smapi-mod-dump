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
using StardewValley.Objects;
using StardewValley.TerrainFeatures;

namespace StopRugRemoval.HarmonyPatches.OutdoorRugsMostly;

/// <summary>
/// Patches against fruit trees to allow for growth near rugs.
/// </summary>
[HarmonyPatch(typeof(FruitTree))]
internal static class FruitTreeGrowthPatch
{
    /***************************************************
    * Removing rugs from the possible check list.
    * Original method: if (o == null) { return true;}
    * New methods: if (o == null || (o is Furniture f && f.furniture_type.Value == Furniture.rug)) { return true;}
    ****************************************************/

    [HarmonyPatch(nameof(FruitTree.IsGrowthBlocked))]
    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);
            helper.FindLast(new CodeInstructionWrapper[]
                    {
                    new (SpecialCodeInstructionCases.LdArg),
                    new (SpecialCodeInstructionCases.LdLoc),
                    new (OpCodes.Ldfld),
                    new (OpCodes.Conv_I4),
                    new (SpecialCodeInstructionCases.LdLoc),
                    new (OpCodes.Ldfld),
                    new (OpCodes.Conv_I4),
                    new (OpCodes.Callvirt, typeof(GameLocation).GetCachedMethod(nameof(GameLocation.getObjectAtTile), ReflectionCache.FlagTypes.InstanceFlags, new Type[] { typeof(int), typeof(int) })),
                    })
                .FindNext(new CodeInstructionWrapper[]
                    {
                    new (SpecialCodeInstructionCases.StLoc, typeof(SObject)),
                    new (SpecialCodeInstructionCases.LdLoc, typeof(SObject)),
                    new (OpCodes.Brfalse_S),
                    })
                .Advance(2)
                .Insert(new CodeInstruction[]
                    {
                    new (OpCodes.Call, typeof(FruitTreeGrowthPatch).StaticMethodNamed(nameof(IsNotRugOrNull))),
                    });
            return helper.Render();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Transpiler for {nameof(FruitTree.IsGrowthBlocked)} failed with error {ex}", LogLevel.Error);
            original?.Snitch(ModEntry.ModMonitor);
        }
        return null;
    }

    private static bool IsNotRugOrNull(SObject? obj)
        => !(obj is null || (obj is Furniture f && f.furniture_type.Value == Furniture.rug));
}