/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Patches.Fishing;

#region using directives

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.Tools;

using DaLion.Common.Extensions.Reflection;
using DaLion.Common.Harmony;

#endregion using directives

[UsedImplicitly]
internal class FishingRodPlayerCaughtFishEndFunctionPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal FishingRodPlayerCaughtFishEndFunctionPatch()
    {
        Original = RequireMethod<FishingRod>(nameof(FishingRod.playerCaughtFishEndFunction));
    }

    #region harmony patches

    /// <summary>Patch for remove annoying repeated message for recatching legendary fish.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> FishingRodPlayerCaughtFishEndFunctionTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// From: if (isFishBossFish(whichFish))
        /// To: if (isFishBossFish(whichFish) && !this.getLastFarmerToUse().fishCount.ContainsKey(whichFish)

        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Call, typeof(FishingRod).RequireMethod(nameof(FishingRod.isFishBossFish)))
                )
                .Advance()
                .GetOperand(out var dontShowMessage)
                .Advance()
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call,
                        typeof(FishingRod).RequireMethod(nameof(FishingRod.getLastFarmerToUse))),
                    new CodeInstruction(OpCodes.Ldfld, typeof(Farmer).RequireField(nameof(Farmer.fishCaught))),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, typeof(FishingRod).RequireField("whichFish")),
                    new CodeInstruction(OpCodes.Call,
                        typeof(NetIntIntArrayDictionary).RequireMethod(nameof(NetIntIntArrayDictionary.ContainsKey))),
                    new CodeInstruction(OpCodes.Brtrue_S, dontShowMessage)
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while removing annoying legendary fish caught notification.\nHelper returned {ex}");
            transpilationFailed = true;
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}