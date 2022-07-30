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
using StardewValley.TerrainFeatures;
using StardewValley.Tools;

namespace StopRugRemoval.HarmonyPatches.Niceties;

/// <summary>
/// Fruit trees aren't damage by hoes.
/// </summary>
[HarmonyPatch(typeof(FruitTree))]
internal static class FruitTreesAvoidHoe
{
    /// <summary>
    /// Applies late patches.
    /// </summary>
    /// <param name="harmony">Harmony instance.</param>
    /// <param name="registry">Mod registry.</param>
    internal static void ApplyPatches(Harmony harmony, IModRegistry registry)
    {
        if (registry.Get("spacechase0.DynamicGameAssets") is null)
        {
            return;
        }
        try
        {
            if (AccessTools.TypeByName("DynamicGameAssets.Game.CustomFruitTree") is Type dgaTree)
            {
                ModEntry.ModMonitor.Log("Transpiling DGA to remove damage to fruit trees from hoes", LogLevel.Info);
                harmony.Patch(
                    original: dgaTree.GetCachedMethod("performToolAction", ReflectionCache.FlagTypes.InstanceFlags),
                    transpiler: new HarmonyMethod(typeof(FruitTreesAvoidHoe), nameof(FruitTreesAvoidHoe.Transpiler)));
            }
            else
            {
                ModEntry.ModMonitor.Log("Cannot find dga fruit trees; they will still be affected by hoes.", LogLevel.Info);
            }
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Ran into issue transpiling DGA fruit trees to remove damage from hoes\n\n{ex}.", LogLevel.Error);
        }
    }

    [HarmonyPatch(nameof(FruitTree.performToolAction))]
    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);
            helper.FindNext(new CodeInstructionWrapper[]
            {
                new(OpCodes.Brtrue_S),
                new(OpCodes.Ldarg_1),
                new(OpCodes.Isinst, typeof(Hoe)),
                new(OpCodes.Brtrue_S),
                new(OpCodes.Ldarg_1),
                new(OpCodes.Isinst, typeof(MeleeWeapon)),
            })
            .Remove(6)
            .FindNext(new CodeInstructionWrapper[]
            {
                new(OpCodes.Brtrue_S),
                new(OpCodes.Ldarg_1),
                new(OpCodes.Callvirt, typeof(Tool).GetCachedProperty(nameof(Tool.BaseName), ReflectionCache.FlagTypes.InstanceFlags).GetGetMethod()),
                new(OpCodes.Ldstr, "Hoe"),
                new(OpCodes.Callvirt, typeof(string).GetCachedMethod(nameof(string.Contains), ReflectionCache.FlagTypes.InstanceFlags, new Type[] { typeof(string) })),
                new(OpCodes.Brfalse_S),
            })
            .Remove(5);
            return helper.Render();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Ran into error transpiling fruit trees to avoid hoe damage.\n\n{ex}", LogLevel.Error);
            original?.Snitch(ModEntry.ModMonitor);
        }
        return null;
    }
}