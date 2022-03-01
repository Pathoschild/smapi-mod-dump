/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Patches.Common;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;

using Stardew.Common.Extensions;
using Extensions;

#endregion using directives

[UsedImplicitly]
internal class CraftingRecipeCtorPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal CraftingRecipeCtorPatch()
    {
        Original = RequireConstructor<CraftingRecipe>(typeof(string), typeof(bool));
    }

    #region harmony patches

    /// <summary>Patch for cheaper crafting recipes for Blaster and Tapper.</summary>
    [HarmonyPostfix]
    private static void CraftingRecipeCtorPostfix(CraftingRecipe __instance)
    {
        if (__instance.name == "Tapper" && Game1.player.HasProfession(Profession.Tapper))
            __instance.recipeList = new()
            {
                { 388, 25 }, // wood
                { 334, 1 } // copper bar
            };
        else if (__instance.name == "Heavy Tapper" && Game1.player.HasProfession(Profession.Tapper))
            __instance.recipeList = new()
            {
                { 709, 20 }, // hardwood
                { 337, 1 }, // iridium bar
                { 909, 1 } // radioactive ore
            };
        else if (__instance.name.ContainsAnyOf("Bomb", "Explosive") && Game1.player.HasProfession(Profession.Blaster))
            __instance.numberProducedPerCraft *= 2;
    }

    #endregion harmony patches
}