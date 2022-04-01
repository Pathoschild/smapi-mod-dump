/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StopRugRemoval
**
*************************************************/

using System.Reflection;
using System.Reflection.Emit;
using AtraBase.Toolkit.Reflection;
using AtraShared.Utils.HarmonyHelper;
using HarmonyLib;
using StardewValley.Locations;
using StardewValley.Objects;

namespace StopRugRemoval.HarmonyPatches;

/// <summary>
/// Class that holds patch to allow jukeboxes to be played anywhere.
/// </summary>
[HarmonyPatch(typeof(MiniJukebox))]
internal static class JukeboxesEverywhere
{
    /// <summary>
    /// Whether or not jukeboxes should be playable at this location.
    /// </summary>
    /// <param name="location">Gamelocation.</param>
    /// <returns>whether the jukebox should be playable.</returns>
    public static bool ShouldPlayJukeBoxHere(GameLocation location)
        => ModEntry.Config.JukeboxesEverywhere
            || !string.IsNullOrWhiteSpace(location.miniJukeboxTrack.Value) // always allow turning the bloody thing off.
            || location is Cellar || location.IsFarm || location.IsGreenhouse;

    [HarmonyPatch(nameof(MiniJukebox.checkForAction))]
    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);
            helper.FindNext(new CodeInstructionWrapper[]
                {
                    new (SpecialCodeInstructionCases.LdArg),
                    new (OpCodes.Callvirt, typeof(Character).InstancePropertyNamed(nameof(Character.currentLocation)).GetGetMethod()),
                    new (OpCodes.Callvirt, typeof(GameLocation).InstancePropertyNamed(nameof(GameLocation.IsFarm)).GetGetMethod()),
                    new (OpCodes.Brtrue_S),
                })
                .Advance(2)
                .RemoveUntil(new CodeInstructionWrapper[]
                {
                    new (OpCodes.Brtrue_S),
                    new (OpCodes.Ldsfld),
                    new (OpCodes.Ldstr, "Strings\\UI:Mini_JukeBox_NotFarmPlay"),
                })
                .Insert(new CodeInstruction[]
                {
                    new (OpCodes.Call, typeof(JukeboxesEverywhere).StaticMethodNamed(nameof(JukeboxesEverywhere.ShouldPlayJukeBoxHere))),
                });
            return helper.Render();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Ran into error transpiling jukeboxes to play everywhere!\n\n{ex}", LogLevel.Error);
        }
        return null;
    }
}