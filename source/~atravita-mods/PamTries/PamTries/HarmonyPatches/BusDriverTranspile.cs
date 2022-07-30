/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

#if DEBUG

using System.Reflection;
using System.Reflection.Emit;
using AtraBase.Toolkit.Reflection;
using AtraShared.Utils.HarmonyHelper;
using HarmonyLib;

namespace PamTries.HarmonyPatches;

internal static class BusDriverTranspile
{
    internal static void ApplyPatch(Harmony harmony)
    {
        try
        {
            harmony.Patch(
            original: typeof(GameLocation).InstanceMethodNamed(nameof(GameLocation.UpdateWhenCurrentLocation)),
            transpiler: new HarmonyMethod(typeof(BusDriverTranspile).StaticMethodNamed(nameof(BusDriverTranspile.Transpiler))));
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Failed in transpiling GameLocation::UpdateWhenCurrentLocation to replace bus driver\n{ex}", LogLevel.Error);
        }
    }

    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);
            helper.FindLast(new CodeInstructionWrapper[]
                {
                    new (OpCodes.Ldarg_0),
                    new (OpCodes.Ldstr, "Pam"),
                    new (OpCodes.Call, typeof(GameLocation).InstanceMethodNamed(nameof(GameLocation.getCharacterFromName))),
                    new (SpecialCodeInstructionCases.StLoc),
                })
                .Advance(1)
                .ReplaceInstruction(OpCodes.Call, typeof(BusDriverSchedulePatch).StaticMethodNamed(nameof(BusDriverSchedulePatch.GetCurrentDriver)), keepLabels: true);
            return helper.Render();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Ran into error transpiliing GameLocation.UpdateWhenCurrentLocation when substituting new driver:\n{ex}", LogLevel.Error);
        }
        return null;
    }
}

#endif