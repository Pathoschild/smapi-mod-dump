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

using AtraShared.Utils.Extensions;
using AtraShared.Utils.HarmonyHelper;

using GrowableGiantCrops.Framework;

using HarmonyLib;

using StardewValley.TerrainFeatures;
using StardewValley.Tools;

namespace GrowableGiantCrops.HarmonyPatches.ItemPatches;

/// <summary>
/// Patches <see cref="HoeDirt.performToolAction(Tool, int, Vector2, GameLocation)"/>
/// so the shovel digs up ginger.
/// </summary>
[HarmonyPatch(typeof(HoeDirt))]
internal static class GingerTranspiler
{
    [HarmonyPatch(nameof(HoeDirt.performToolAction))]
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:Split parameters should start on line after declaration", Justification = "Reviewed.")]
    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);
            helper.FindNext(new CodeInstructionWrapper[]
            { // t is Hoe
                OpCodes.Ldarg_1,
                (OpCodes.Isinst, typeof(Hoe)),
                OpCodes.Brfalse_S,
            })
            .Push()
            .Advance(3)
            .DefineAndAttachLabel(out Label jumpPoint)
            .Pop()
            .GetLabels(out IList<Label>? labelsToMove)
            .Insert(new CodeInstruction[]
            { // insert t is ShovelTool -> final is if (t is ShovelTool || t is Hoe);
                new(OpCodes.Ldarg_1),
                new(OpCodes.Isinst, typeof(ShovelTool)),
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
