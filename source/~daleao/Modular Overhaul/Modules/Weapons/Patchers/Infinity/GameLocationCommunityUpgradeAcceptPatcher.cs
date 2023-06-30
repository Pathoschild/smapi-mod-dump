/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Weapons.Patchers.Infinity;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
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
                .Match(new[] { new CodeInstruction(OpCodes.Ret) }, ILHelper.SearchOption.First)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(GameLocationCommunityUpgradeAcceptPatcher).RequireMethod(nameof(CheckForGenerosityCompletion))),
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed injecting Generosity completion.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static void CheckForGenerosityCompletion()
    {
        Game1.player.Increment(DataKeys.ProvenGenerosity, (int)5e5);
        WeaponsModule.State.VirtuesQuest?.UpdateVirtueProgress(Virtue.Generosity);
    }

    #endregion injected subroutines
}
