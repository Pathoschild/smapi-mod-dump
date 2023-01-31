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

using StardewValley.Locations;
using StardewValley.Minigames;

namespace MoreFertilizers.HarmonyPatches.Acquisition;

/// <summary>
/// Handles patches against the Crane Game.
/// </summary>
[HarmonyPatch(typeof(CraneGame.GameLogic))]
internal static class CraneGameTranspiler
{
    private static void AddFertilizerToRewardsListTwo(List<Item> items)
    {
        if (Utility.hasFinishedJojaRoute())
        {
            if (ModEntry.JojaFertilizerID != -1)
            {
                items.Add(new SObject(ModEntry.JojaFertilizerID, 5));
            }
            if (ModEntry.DeluxeJojaFertilizerID != -1)
            {
                items.Add(new SObject(ModEntry.DeluxeJojaFertilizerID, 5));
            }
            if (ModEntry.FruitTreeFertilizerID != -1)
            {
                items.Add(new SObject(ModEntry.FruitTreeFertilizerID, 5));
            }
            items.Add(new SObject(71, 1)); // Lewis's shorts.
        }
        else
        {
            if (ModEntry.SecretJojaFertilizerID != -1)
            {
                items.Add(new SObject(ModEntry.SecretJojaFertilizerID, 1));
            }
        }
        if (ModEntry.MiraculousBeveragesID != -1)
        {
            items.Add(new SObject(ModEntry.MiraculousBeveragesID, 5));
        }
    }

    private static void AddFertilizerToRewardsListThree(List<Item> items)
    {
        if (Utility.hasFinishedJojaRoute() && ModEntry.EverlastingFertilizerID != -1)
        {
            items.Add(new SObject(ModEntry.EverlastingFertilizerID, 5));
        }
        if (ModEntry.SecretJojaFertilizerID != -1)
        {
            items.Add(new SObject(ModEntry.SecretJojaFertilizerID, 1));
        }
        if (ModEntry.SeedyFertilizerID != -1)
        {
            items.Add(new SObject(ModEntry.SeedyFertilizerID, 5));
        }
    }

    [HarmonyPatch(MethodType.Constructor, new[] { typeof(CraneGame) })]
    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);
            helper.FindNext(new CodeInstructionWrapper[]
            {
                new(OpCodes.Call, typeof(Game1).GetCachedProperty(nameof(Game1.Date), ReflectionCache.FlagTypes.StaticFlags).GetGetMethod()),
                new(OpCodes.Call, typeof(MovieTheater).GetCachedMethod(nameof(MovieTheater.GetMovieForDate), ReflectionCache.FlagTypes.StaticFlags)),
            })
            .FindPrev(new CodeInstructionWrapper[]
            {
                new(OpCodes.Newobj, typeof(List<Item>).GetCachedConstructor(ReflectionCache.FlagTypes.InstanceFlags)),
                SpecialCodeInstructionCases.StLoc,
            })
            .Advance(1);

            CodeInstruction? ldloc = helper.CurrentInstruction.ToLdLoc();
            helper.Advance(1)
            .Insert(new CodeInstruction[]
            {
                ldloc,
                new(OpCodes.Call, typeof(CraneGameTranspiler).GetCachedMethod(nameof(AddFertilizerToRewardsListTwo), ReflectionCache.FlagTypes.StaticFlags)),
            })
            .FindNext(new CodeInstructionWrapper[]
            {
                new(OpCodes.Newobj, typeof(List<Item>).GetCachedConstructor(ReflectionCache.FlagTypes.InstanceFlags)),
                SpecialCodeInstructionCases.StLoc,
            })
            .Advance(1);

            CodeInstruction? ldloc2 = helper.CurrentInstruction.ToLdLoc();
            helper.Advance(1)
            .Insert(new CodeInstruction[]
            {
                ldloc2,
                new(OpCodes.Call, typeof(CraneGameTranspiler).GetCachedMethod(nameof(AddFertilizerToRewardsListThree), ReflectionCache.FlagTypes.StaticFlags)),
            });

            // helper.Print();
            return helper.Render();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Mod crashed while transpiling {original.FullDescription()}:\n\n{ex}", LogLevel.Error);
            original.Snitch(ModEntry.ModMonitor);
        }
        return null;
    }
}