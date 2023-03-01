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

using GrowableGiantCrops.HarmonyPatches.Niceties;

using HarmonyLib;

using Microsoft.Xna.Framework;

using StardewValley.TerrainFeatures;

namespace GrowableGiantCrops.HarmonyPatches.GrassPatches;

/// <summary>
/// Prevents grass from spreading.
/// </summary>
[HarmonyPatch(typeof(GameLocation))]
internal static class PreventGrassSpreadTranspiler
{
    private static bool ShouldSkipThisGrass(Grass grass)
        => !ModEntry.Config.ShouldPlacedGrassSpread && grass.modData?.GetBool(PatchesForSObject.ModDataMiscObject) == true;

    [HarmonyPatch(nameof(GameLocation.growWeedGrass))]
    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);
            helper.FindNext(new CodeInstructionWrapper[]
            { // find if( (int)((Grass)kvp.Value).numberOfWeeds >= 4
                OpCodes.Ldloca_S,
                (OpCodes.Call, typeof(KeyValuePair<Vector2, TerrainFeature>).GetCachedProperty("Value", ReflectionCache.FlagTypes.InstanceFlags).GetGetMethod()),
                (OpCodes.Castclass, typeof(Grass)),
                (OpCodes.Ldfld, typeof(Grass).GetCachedField(nameof(Grass.numberOfWeeds), ReflectionCache.FlagTypes.InstanceFlags)),
                OpCodes.Call, //op_implicit
                OpCodes.Ldc_I4_4,
                OpCodes.Blt,
            })
            .Advance(3)
            .Push()
            .Advance(3);

            // we're gonna just straight up replace it with our function, since the original else if was pointless.
            Label jumpPoint = (Label)helper.CurrentInstruction.operand;
            helper.Pop()
            .Remove(4)
            .Insert(new CodeInstruction[]
            {
                new (OpCodes.Call, typeof(PreventGrassSpreadTranspiler).GetCachedMethod(nameof(ShouldSkipThisGrass), ReflectionCache.FlagTypes.StaticFlags)),
                new(OpCodes.Brtrue, jumpPoint),
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
