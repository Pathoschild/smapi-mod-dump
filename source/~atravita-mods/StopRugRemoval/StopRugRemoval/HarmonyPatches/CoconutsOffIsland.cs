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
using StardewValley.Locations;
using StardewValley.TerrainFeatures;

namespace StopRugRemoval.HarmonyPatches;

/// <summary>
/// Class that holds patch to allow jukeboxes to be played anywhere.
/// </summary>
[HarmonyPatch(typeof(Tree))]
internal static class CoconutsOffIsland
{
    /// <summary>
    /// Whether or not jukeboxes should be playable at this location.
    /// </summary>
    /// <param name="location">Gamelocation.</param>
    /// <returns>whether the jukebox should be playable.</returns>
    public static bool AllowCoconutShakingHere(GameLocation location)
        => location is IslandLocation
            || (ModEntry.Config.Enabled && ModEntry.Config.GoldenCoconutsOffIsland && Game1.netWorldState.Value.GoldenCoconutCracked.Value);

    [HarmonyPatch("shake")]
    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);
            helper.FindNext(new CodeInstructionWrapper[]
                {
                    new (SpecialCodeInstructionCases.LdArg),
                    new (OpCodes.Brfalse_S),
                    new (SpecialCodeInstructionCases.LdArg),
                    new (OpCodes.Isinst),
                    new (OpCodes.Brfalse_S),
                })
                .Advance(3)
                .ReplaceInstruction(OpCodes.Call, typeof(CoconutsOffIsland).StaticMethodNamed(nameof(CoconutsOffIsland.AllowCoconutShakingHere)), keepLabels: true);
            return helper.Render();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Ran into error transpiling coconut trees to drop coconuts everywhere!\n\n{ex}", LogLevel.Error);
        }
        return null;
    }
}