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

using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Buildings;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
internal sealed class PondQueryMenuCtorPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="PondQueryMenuCtorPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal PondQueryMenuCtorPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireConstructor<PondQueryMenu>(typeof(FishPond));
    }

    #region harmony patches

    /// <summary>Patch to adjust fish pond query menu for Aquarist increased max capacity.</summary>
    [HarmonyPostfix]
    private static void PondQueryMenuDrawPostfix(FishPond fish_pond)
    {
        if (fish_pond.maxOccupants.Value > 10)
        {
            PondQueryMenu.height += 32;
        }
    }

    #endregion harmony patches
}
