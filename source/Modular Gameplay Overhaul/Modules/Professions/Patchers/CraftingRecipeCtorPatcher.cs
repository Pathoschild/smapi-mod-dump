/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
**
*************************************************/

namespace DaLion.Overhaul.Modules.Professions.Patchers;

#region using directives

using System.Collections.Generic;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Constants;
using DaLion.Shared.Extensions;
using DaLion.Shared.Harmony;
using HarmonyLib;

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

    /// <summary>Patch for cheaper crafting recipes for Blaster and Tapper.</summary>
    [HarmonyPostfix]
    private static void CraftingRecipeCtorPostfix(CraftingRecipe __instance)
    {
        switch (__instance.name)
        {
            case "Tapper" when Game1.player.HasProfession(Profession.Tapper):
                __instance.recipeList = new Dictionary<int, int>
                {
                    { ObjectIds.Wood, 25 },
                    { ObjectIds.CopperBar, 1 },
                };
                break;
            case "Heavy Tapper" when Game1.player.HasProfession(Profession.Tapper):
                __instance.recipeList = new Dictionary<int, int>
                {
                    { ObjectIds.Hardwood, 18 },
                    { ObjectIds.RadioactiveBar, 1 },
                };
                break;
            default:
                {
                    if (__instance.name.ContainsAnyOf("Bomb", "Explosive") && Game1.player.HasProfession(Profession.Blaster))
                    {
                        __instance.numberProducedPerCraft *= 2;
                    }

                    break;
                }
        }
    }

    #endregion harmony patches
}
