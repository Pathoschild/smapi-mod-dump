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
using AtraShared.Utils.HarmonyHelper;
using HarmonyLib;

namespace PamTries.HarmonyPatches;

/// <summary>
/// Patches on events to hide Pam while she's at rehab.
/// </summary>
[HarmonyPatch(typeof(Event))]
internal static class EventPatches
{
    private static bool ShouldHidePam() => Game1.player.activeDialogueEvents.ContainsKey("PamTriesRehab");

    [HarmonyPostfix]
    [HarmonyPatch(nameof(Event.tryToLoadFestival))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony Convention.")]
    private static void PostfixSetup(Event __instance)
    {
        if (ShouldHidePam())
        {
            __instance.actors.Remove(__instance.getActorByName("Pam"));
        }
    }

    // transpile the festival update method to account for the fact that I've removed Pam from the festival, lol.
#pragma warning disable SA1116 // Split parameters should start on line after declaration. Reviewed.
    [HarmonyTranspiler]
    [HarmonyPatch(nameof(Event.festivalUpdate))]
    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);
            helper.FindNext(new CodeInstructionWrapper[]
            {
                new(OpCodes.Ldstr, "iceFishing"),
            })
            .FindNext(new CodeInstructionWrapper[]
            { // this.getActorByName("Pam");
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldstr, "Pam"),
            })
            .Push()
            .FindNext(new CodeInstructionWrapper[]
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldstr, "Elliott"),
            })
            .DefineAndAttachLabel(out var jumppoint)
            .Pop()
            .GetLabels(out var labelsToMove)
            .Insert(new CodeInstruction[]
            {
                new(OpCodes.Call, typeof(EventPatches).StaticMethodNamed(nameof(ShouldHidePam))),
                new(OpCodes.Brtrue, jumppoint),
            }, withLabels: labelsToMove)
            .FindNext(new CodeInstructionWrapper[]
            { // the other pam block lol
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, typeof(Event).InstanceFieldNamed("oldTime")),
                new(OpCodes.Ldc_I4, 45900),
            })
            .FindNext(new CodeInstructionWrapper[]
            {
                new(OpCodes.Bge),
            })
            .StoreBranchDest()
            .Push()
            .AdvanceToStoredLabel()
            .DefineAndAttachLabel(out var jumppass)
            .Pop()
            .Advance(1)
            .Insert(new CodeInstruction[]
            {
                new(OpCodes.Call, typeof(EventPatches).StaticMethodNamed(nameof(ShouldHidePam))),
                new(OpCodes.Brtrue, jumppass),
            });

            // helper.Print();
            return helper.Render();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Ran into error transpiliing GameLocation.UpdateWhenCurrentLocation when substituting new driver:\n{ex}", LogLevel.Error);
        }
        return null;
    }
#pragma warning restore SA1116 // Split parameters should start on line after declaration
}
