/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Stardew.Rings.Integrations;

#region using directives

using Common.Integrations;
using StardewModdingAPI;
using System.Collections.Generic;

#endregion using directives

internal sealed class BetterCraftingIntegration : BaseIntegration<IBetterCraftingAPI>
{
    /// <summary>Construct an instance.</summary>
    /// <param name="modRegistry">An API for fetching metadata about loaded mods.</param>
    public BetterCraftingIntegration(IModRegistry modRegistry)
        : base("Better Crafting", "leclair.bettercrafting", "1.0.0", modRegistry) { }

    /// <summary>Register the ring recipe provider.</summary>
    public void Register()
    {
        AssertLoaded();
        ModApi!.AddRecipeProvider(new RingRecipeProvider(ModApi));

        var newRingRecipes = new List<string>
        {
            "Glow Ring", "Magnet Ring", "Amethyst Ring", "Topaz Ring", "Aquamarine Ring", "Jade Ring", "Emerald Ring",
            "Ruby Ring"
        };
        ModApi!.AddRecipesToDefaultCategory(false, "combat_rings", newRingRecipes);
    }
}