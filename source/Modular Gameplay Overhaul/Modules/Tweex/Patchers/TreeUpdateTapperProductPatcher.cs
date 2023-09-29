/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
**
*************************************************/

namespace DaLion.Overhaul.Modules.Tweex.Patchers;

#region using directives

using DaLion.Overhaul.Modules.Tweex.Extensions;
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

    /// <summary>Adds age quality to tapper product.</summary>
    [HarmonyPostfix]
    private static void TreeUpdateTapperProductPostfix(Tree __instance, SObject? tapper_instance)
    {
        if (tapper_instance is not null)
        {
            tapper_instance.heldObject.Value.Quality = __instance.GetQualityFromAge();
        }
    }

    #endregion harmony patches
}
