/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Professions.Patchers.Foraging;

#region using directives

using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.TerrainFeatures;

#endregion using directives

[UsedImplicitly]
internal sealed class TreeUpdateTapperProductPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="TreeUpdateTapperProductPatcher"/> class.</summary>
    internal TreeUpdateTapperProductPatcher()
    {
        this.Target = this.RequireMethod<Tree>(nameof(Tree.UpdateTapperProduct));
    }

    #region harmony patches

    /// <summary>Patch to decrease syrup production time for Tapper.</summary>
    [HarmonyPostfix]
    // ReSharper disable once InconsistentNaming
    private static void TreeUpdateTapperProductPostfix(Tree __instance, SObject? tapper_instance)
    {
        if (tapper_instance is null || (__instance.treeType.Value == Tree.mushroomTree && Game1.currentSeason == "winter"))
        {
            return;
        }

        var owner = ProfessionsModule.Config.LaxOwnershipRequirements ? Game1.player : tapper_instance.GetOwner();
        if (!owner.HasProfession(Profession.Tapper))
        {
            return;
        }

        if (tapper_instance.MinutesUntilReady > 0)
        {
            tapper_instance.MinutesUntilReady = (int)(tapper_instance.MinutesUntilReady *
                                                      (owner.HasProfession(Profession.Tapper, true) ? 0.5 : 0.75));
        }
    }

    #endregion harmony patches
}
