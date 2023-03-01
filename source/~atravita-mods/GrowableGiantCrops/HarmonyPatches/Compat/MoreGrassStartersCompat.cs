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
using GrowableGiantCrops.HarmonyPatches.GrassPatches;

using HarmonyLib;

using Microsoft.Xna.Framework;

using StardewValley.TerrainFeatures;

namespace GrowableGiantCrops.HarmonyPatches.Compat;

/// <summary>
/// Patches MoreGrassStarters.
/// </summary>
internal static class MoreGrassStartersCompat
{
    /// <summary>
    /// Applies the patches for this class.
    /// </summary>
    /// <param name="harmony">My harmony instance.</param>
    internal static void ApplyPatch(Harmony harmony)
    {
        try
        {
            if (AccessTools.TypeByName("MoreGrassStarters.GrassStarterItem") is Type grassStarter)
            {
                harmony.Patch(
                    original: grassStarter.GetCachedMethod("placementAction", ReflectionCache.FlagTypes.InstanceFlags),
                    prefix: new HarmonyMethod(typeof(MoreGrassStartersCompat).StaticMethodNamed(nameof(Postfix))));
            }
            else
            {
                ModEntry.ModMonitor.Log($"MoreGrassStarter's GrassStarter item could not be found?.", LogLevel.Error);
            }

            if (AccessTools.TypeByName("MoreGrassStarters.Mod") is Type moreGrassStarters)
            {
                harmony.Patch(
                   original: moreGrassStarters.GetCachedMethod("OnDayStarted", ReflectionCache.FlagTypes.InstanceFlags),
                   transpiler: new HarmonyMethod(typeof(MoreGrassStartersCompat).StaticMethodNamed(nameof(Transpiler))));
            }
            else
            {
                ModEntry.ModMonitor.Log($"MoreGrassStarter's modentry class could not be found?.", LogLevel.Error);
            }
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Failed to patch More Grass Starters.\n\n{ex}", LogLevel.Error);
        }
    }

    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony Convention.")]
    private static void Postfix(SObject __instance, GameLocation location, int x, int y, bool __result)
    {
        if (!__result || __instance?.modData?.GetBool(SObjectPatches.ModDataKey) != true)
        {
            return;
        }

        try
        {
            Vector2 tile = new(x / Game1.tileSize, y / Game1.tileSize);
            if (location.terrainFeatures?.TryGetValue(tile, out TerrainFeature? terrain) == true
                && terrain is Grass grass)
            {
                grass.numberOfWeeds.Value = 1;
            }
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Failed while trying to override health of MGS grass:\n\n{ex}", LogLevel.Error);
        }
    }

    private static bool ShouldSkipThisGrass(Grass? grass) => grass?.modData?.ContainsKey(SObjectPatches.ModDataKey) == true;

    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:Split parameters should start on line after declaration", Justification = "Reviewed.")]
    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);
            helper.FindNext(new CodeInstructionWrapper[]
            { // terrainFeature is Grass grass
                SpecialCodeInstructionCases.LdLoc,
                (OpCodes.Isinst, typeof(Grass)),
                SpecialCodeInstructionCases.StLoc,
                SpecialCodeInstructionCases.LdLoc,
                OpCodes.Brfalse_S,
            })
            .Advance(3);

            var ldloc = helper.CurrentInstruction.Clone();
            helper.Push()
            .Advance(1)
            .StoreBranchDest()
            .AdvanceToStoredLabel()
            .DefineAndAttachLabel(out Label jumpPoint)
            .Pop()
            .GetLabels(out IList<Label>? labelsToMove)
            .Insert(new CodeInstruction[]
            {
                ldloc,
                new(OpCodes.Call, typeof(MoreGrassStartersCompat).GetCachedMethod(nameof(ShouldSkipThisGrass), ReflectionCache.FlagTypes.StaticFlags)),
                new(OpCodes.Brtrue, jumpPoint),
            }, withLabels: labelsToMove);

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
