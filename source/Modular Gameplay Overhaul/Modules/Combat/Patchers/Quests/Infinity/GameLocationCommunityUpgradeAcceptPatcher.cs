/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
**
*************************************************/

namespace DaLion.Overhaul.Modules.Combat.Patchers.Quests.Infinity;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Combat.Enums;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationCommunityUpgradeAcceptPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationCommunityUpgradeAcceptPatcher"/> class.</summary>
    internal GameLocationCommunityUpgradeAcceptPatcher()
    {
        this.Target = this.RequireMethod<GameLocation>("communityUpgradeAccept");
    }

    #region harmony patches

    /// <summary>Complete Generosity quest.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? CommunityUpgradeAcceptTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            helper
                .Match(new[] { new CodeInstruction(OpCodes.Ldc_I4, 500000) })
                .Match(new[] { new CodeInstruction(OpCodes.Ret) })
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldc_I4, 500000),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(GameLocationCommunityUpgradeAcceptPatcher).RequireMethod(nameof(IncrementGenerosity))),
                    })
                .Match(new[] { new CodeInstruction(OpCodes.Ldc_I4, 300000) })
                .Match(new[] { new CodeInstruction(OpCodes.Ret) })
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldc_I4, 300000),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(GameLocationCommunityUpgradeAcceptPatcher).RequireMethod(nameof(IncrementGenerosity))),
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed injecting community upgrade generosity.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static void IncrementGenerosity(int amount)
    {
        foreach (var farmer in Game1.getAllFarmers())
        {
            farmer.Increment(Virtue.Generosity.Name, amount);
        }

        CombatModule.State.HeroQuest?.UpdateTrialProgress(Virtue.Generosity);
        Game1.showGlobalMessage(I18n.Virtues_Recognize_Robin());
    }

    #endregion injected subroutines
}
