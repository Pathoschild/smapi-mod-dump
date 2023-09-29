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

using DaLion.Shared.Constants;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class CraftingRecipeCtorPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="CraftingRecipeCtorPatcher"/> class.</summary>
    internal CraftingRecipeCtorPatcher()
    {
        this.Target = this.RequireConstructor<CraftingRecipe>(typeof(string), typeof(bool));
    }

    #region harmony patches

    /// <summary>Fix localized display name for custom ring recipes.</summary>
    [HarmonyPostfix]
    private static void CraftingRecipeCtorPrefix(CraftingRecipe __instance, string name, bool isCookingRecipe)
    {
        if (isCookingRecipe || !__instance.name.Contains("Ring") || LocalizedContentManager.CurrentLanguageCode ==
            LocalizedContentManager.LanguageCode.en)
        {
            return;
        }

        __instance.DisplayName = name switch
        {
            "Glow Ring" => new Ring(ObjectIds.GlowRing).DisplayName,
            "Magnet Ring" => new Ring(ObjectIds.MagnetRing).DisplayName,
            _ => __instance.DisplayName,
        };
    }

    #endregion harmony patches
}
