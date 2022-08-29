/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Patches.Foraging;

#region using directives

using DaLion.Common.Extensions.Stardew;
using Extensions;
using HarmonyLib;
using StardewValley.TerrainFeatures;

#endregion using directives

[UsedImplicitly]
internal sealed class TreeUpdateTapperProductPatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal TreeUpdateTapperProductPatch()
    {
        Target = RequireMethod<Tree>(nameof(Tree.UpdateTapperProduct));
    }

    #region harmony patches

    /// <summary>Patch to decrease syrup production time for Tapper.</summary>
    [HarmonyPostfix]
    private static void TreeUpdateTapperProductPostfix(SObject? tapper_instance)
    {
        if (tapper_instance is null) return;

        var owner = ModEntry.Config.LaxOwnershipRequirements ? Game1.player : tapper_instance.GetOwner();
        if (!owner.HasProfession(Profession.Tapper)) return;

        if (tapper_instance.MinutesUntilReady > 0)
            tapper_instance.MinutesUntilReady = (int)(tapper_instance.MinutesUntilReady *
                                                       (owner.HasProfession(Profession.Tapper, true) ? 0.5 : 0.75));
    }

    #endregion harmony patches
}