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
using StardewValley.Monsters;

namespace PrismaticSlime.HarmonyPatches.RingPatches;

/// <summary>
/// Adjusts the chances of the prismatic slime spawning in the MineShaft.
/// </summary>
[HarmonyPatch(typeof(MineShaft))]
internal static class AdjustSlimeChances
{
    private static double AdjustChanceForPrismaticRing(double chance, Farmer player)
    {
        if (ModEntry.PrismaticSlimeRing == -1 || player is null)
        {
            return chance;
        }
        else if (ModEntry.WearMoreRingsAPI?.CountEquippedRings(player, ModEntry.PrismaticSlimeRing) is > 0
            || player.isWearingRing(ModEntry.PrismaticSlimeRing))
        {
            return Math.Clamp(chance * 5, 0, 1);
        }
        return chance;
    }

    [HarmonyPatch("populateLevel")]
    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);

            helper.FindNext(new CodeInstructionWrapper[]
            { // (monster as GreenSlime).makePrismatic(),
                new(SpecialCodeInstructionCases.LdLoc),
                new(OpCodes.Isinst, typeof(GreenSlime)),
                new(OpCodes.Callvirt, typeof(GreenSlime).GetCachedMethod(nameof(GreenSlime.makePrismatic), ReflectionCache.FlagTypes.InstanceFlags)),
            })
            .FindPrev(new CodeInstructionWrapper[]
            {
                new(OpCodes.Ldsfld, typeof(Game1).GetCachedField(nameof(Game1.random), ReflectionCache.FlagTypes.StaticFlags)),
                new(OpCodes.Callvirt, typeof(Random).GetCachedMethod(nameof(Random.NextDouble), ReflectionCache.FlagTypes.InstanceFlags, Type.EmptyTypes)),
                new(OpCodes.Ldc_R8, 0.012),
            })
            .Advance(3)
            .Insert(new CodeInstruction[]
            {
                new(OpCodes.Call, typeof(Game1).GetCachedProperty(nameof(Game1.player), ReflectionCache.FlagTypes.StaticFlags).GetGetMethod()),
                new(OpCodes.Call, typeof(AdjustSlimeChances).GetCachedMethod(nameof(AdjustChanceForPrismaticRing), ReflectionCache.FlagTypes.StaticFlags)),
            });

            // helper.Print();
            return helper.Render();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Ran into error transpiling {original.FullDescription()}.\n\n{ex}", LogLevel.Error);
            original?.Snitch(ModEntry.ModMonitor);
        }
        return null;
    }
}