/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Professions.Framework.Patchers.Fishing;

#region using directives

using System;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Extensions;
using StardewValley.GameData.Locations;
using StardewValley.Internal;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationGetFishFromLocationDataPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationGetFishFromLocationDataPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal GameLocationGetFishFromLocationDataPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<GameLocation>(nameof(GameLocation.GetFishFromLocationData),
        [
            typeof(string), typeof(Vector2), typeof(int), typeof(Farmer), typeof(bool), typeof(bool),
            typeof(GameLocation), typeof(ItemQueryContext),
        ]);
    }

    #region harmony patches

    /// <summary>Patch for Prestiged Angler boss fish recatch.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? GameLocationGetFishFromLocationDataTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            helper
                .PatternMatch(
                    [
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(SpawnFishData).RequirePropertyGetter(nameof(SpawnFishData.CatchLimit))),
                    ],
                    ILHelper.SearchOption.Last)
                .PatternMatch([new CodeInstruction(OpCodes.Brfalse_S)], ILHelper.SearchOption.Previous)
                .GetOperand(out var allowCatch)
                .Return()
                .Move()
                .GetOperand(out var disallowCatch)
                .ReplaceWith(new CodeInstruction(OpCodes.Blt_S, allowCatch))
                .Move()
                .Insert([
                    new CodeInstruction(OpCodes.Ldloc_S, helper.Locals[22]),
                    new CodeInstruction(OpCodes.Ldfld, "<>c__DisplayClass495_1".ToType().RequireField("spawn")),
                    new CodeInstruction(OpCodes.Ldarg_3),
                    new CodeInstruction(OpCodes.Ldloc_S, helper.Locals[9]),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(GameLocationGetFishFromLocationDataPatcher).RequireMethod(
                            nameof(PassesPrestigedAnglerCheck))),
                    new CodeInstruction(OpCodes.Brfalse_S, disallowCatch),
                ]);
        }
        catch (Exception ex)
        {
            Log.E($"Failed patching Prestiged Angler boss fish multiple catch.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injections

    private static bool PassesPrestigedAnglerCheck(SpawnFishData spawn, Farmer who, FishingRod? rod)
    {
        if (!spawn.IsBossFish || !who.HasProfession(Profession.Angler, true) || rod is null)
        {
            return false;
        }

        var chance = 0.01 * (1 + Utility.getStringCountInList(
            rod.GetTackleQualifiedItemIDs(),
            QualifiedObjectIds.CuriosityLure));
        return Game1.random.NextBool(chance);
    }

    #endregion injections
}
