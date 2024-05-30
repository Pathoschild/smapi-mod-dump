/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Professions.Framework.Patchers.Mining;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Locations;

#endregion using directives

[UsedImplicitly]
internal sealed class MineShaftLoadLevelPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MineShaftLoadLevelPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal MineShaftLoadLevelPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<MineShaft>(nameof(MineShaft.loadLevel));
    }

    #region harmony patches

    /// <summary>
    ///     Patch for Spelunker ladder down chance bonus + remove Geologist paired gem chance + remove Excavator double
    ///     geode chance + remove Prospector double coal chance.
    /// </summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? MineShaftCheckStoneForItemsTranspiler(IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            helper
                .PatternMatch([new CodeInstruction(OpCodes.Stloc_S, helper.Locals[9])], nth: 2)
                .Insert([
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(MineShaftLoadLevelPatcher).RequireMethod(
                            nameof(GetPrestigedSpelunkerTreasureRoomMultiplier))),
                    new CodeInstruction(OpCodes.Mul),
                ]);
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding Prestiged Spelunker bonus treasure room chance.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injections

    private static double GetPrestigedSpelunkerTreasureRoomMultiplier()
    {
        return !Game1.game1.DoesAnyPlayerHaveProfession(Profession.Spelunker, out var spelunkers, prestiged: true)
            ? 1d
            : 1d + (spelunkers.Count(spelunker => spelunker.currentLocation is MineShaft) * 2d);
    }

    #endregion injections
}
