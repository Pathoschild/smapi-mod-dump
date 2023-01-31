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
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:Split parameters should start on line after declaration", Justification = "Reviewed.")]
    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);
            helper.FindNext(new CodeInstructionWrapper[]
                {
                    new (SpecialCodeInstructionCases.LdArg),
                    new (OpCodes.Callvirt, typeof(Character).GetCachedProperty(nameof(Character.currentLocation), ReflectionCache.FlagTypes.InstanceFlags).GetGetMethod()),
                    new (OpCodes.Callvirt, typeof(GameLocation).GetCachedProperty(nameof(GameLocation.IsFarm), ReflectionCache.FlagTypes.InstanceFlags).GetGetMethod()),
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
                })
                .FindNext(new CodeInstructionWrapper[]
                {
                    new(OpCodes.Ldsfld, typeof(Game1).GetCachedField(nameof(Game1.isRaining), ReflectionCache.FlagTypes.StaticFlags)),
                })
                .GetLabels(out IList<Label>? labels)
                .Remove(1)
                .Insert(new CodeInstruction[]
                {
                    new(OpCodes.Ldarg_1), // Farmer who
                    new(OpCodes.Callvirt, typeof(Character).GetCachedProperty(nameof(Character.currentLocation), ReflectionCache.FlagTypes.InstanceFlags).GetGetMethod()),
                    new(OpCodes.Call, typeof(Game1).GetCachedMethod(nameof(Game1.IsRainingHere), ReflectionCache.FlagTypes.StaticFlags)),
                }, withLabels: labels);
            return helper.Render();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Ran into error transpiling jukeboxes to play everywhere!\n\n{ex}", LogLevel.Error);
        }
        return null;
    }
}