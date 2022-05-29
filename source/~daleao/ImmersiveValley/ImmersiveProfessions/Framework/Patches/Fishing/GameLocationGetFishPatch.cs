/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Patches.Fishing;

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.Tools;

using DaLion.Common.Extensions.Reflection;
using DaLion.Common.Harmony;
using Extensions;

using SObject = StardewValley.Object;
using SUtility = StardewValley.Utility;

#endregion using directives

[UsedImplicitly]
internal class GameLocationGetFishPatch : BasePatch
{
    private const int MAGNET_INDEX_I = 703;

    /// <summary>Construct an instance.</summary>
    internal GameLocationGetFishPatch()
    {
        Original = RequireMethod<GameLocation>(nameof(GameLocation.getFish));
    }

    #region harmony patches

    /// <summary>Patch for Fisher to reroll reeled fish if first roll resulted in trash.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> GameLocationGetFishTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// Injected: if (ShouldRerollFish(who, whichFish, hasRerolled)) goto <choose_fish>
        ///	Before: caught = new Object(whichFish, 1);

        var startOfFishRoll = generator.DefineLabel();
        var shouldntReroll = generator.DefineLabel();
        var hasRerolled = generator.DeclareLocal(typeof(bool));
        var shuffleMethod = typeof(SUtility).GetMethods().Where(mi => mi.Name == "Shuffle").ElementAtOrDefault(1);
        if (shuffleMethod is null)
        {
            Log.E($"Failed to acquire {typeof(SUtility)}::Shuffle method.");
            transpilationFailed = true;
            return null;
        }

        try
        {
            helper
                .Insert( // set hasRerolled to false
                    new CodeInstruction(OpCodes.Ldc_I4_0),
                    new CodeInstruction(OpCodes.Stloc_S, hasRerolled)
                )
                .FindLast( // find index of caught = new Object(whichFish, 1)
                    new CodeInstruction(OpCodes.Newobj,
                        typeof(SObject).GetConstructor(new[]
                            {typeof(int), typeof(int), typeof(bool), typeof(int), typeof(int)}))
                )
                .RetreatUntil(
                    new CodeInstruction(OpCodes.Ldloc_1)
                )
                .AddLabels(shouldntReroll) // branch here if shouldn't reroll
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_S, (byte) 4), // arg 4 = Farmer who
                    new CodeInstruction(OpCodes.Ldloc_1), // local 1 = whichFish
                    new CodeInstruction(OpCodes.Ldloc_S, hasRerolled),
                    new CodeInstruction(OpCodes.Call,
                        typeof(GameLocationGetFishPatch).RequireMethod(nameof(ShouldRerollFish))),
                    new CodeInstruction(OpCodes.Brfalse_S, shouldntReroll),
                    new CodeInstruction(OpCodes.Ldc_I4_1),
                    new CodeInstruction(OpCodes.Stloc_S, hasRerolled), // set hasRerolled to true
                    new CodeInstruction(OpCodes.Br, startOfFishRoll)
                )
                .RetreatUntil( // start of choose fish
                    new CodeInstruction(OpCodes.Call, shuffleMethod.MakeGenericMethod(typeof(string)))
                )
                .Retreat(2)
                .AddLabels(startOfFishRoll); // branch here to reroll
        }
        catch (Exception ex)
        {
            Log.E($"Failed while adding modded Fisher fish reroll.\nHelper returned {ex}");
            transpilationFailed = true;
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region private methods

    private static bool ShouldRerollFish(Farmer who, int currentFish, bool hasRerolled)
    {
        return (currentFish is > 166 and < 173 || ModEntry.Config.SeaweedIsJunk && currentFish.IsAlgae())
               && who.CurrentTool is FishingRod rod
               && rod.getBaitAttachmentIndex() != MAGNET_INDEX_I
               && who.HasProfession(Profession.Fisher) && !hasRerolled;
    }

    #endregion private methods
}