/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
**
*************************************************/

namespace DaLion.Overhaul.Modules.Combat.Integrations;

#region using directives

using System.Collections.Generic;
using DaLion.Shared.Attributes;
using DaLion.Shared.Integrations;
using DaLion.Shared.Integrations.BetterCrafting;

#endregion using directives

[ModRequirement("leclair.bettercrafting", "Better Crafting", "1.0.0")]
internal sealed class BetterCraftingIntegration : ModIntegration<BetterCraftingIntegration, IBetterCraftingApi>
{
    /// <summary>Initializes a new instance of the <see cref="BetterCraftingIntegration"/> class.</summary>
    internal BetterCraftingIntegration()
        : base(ModHelper.ModRegistry)
    {
    }

    /// <inheritdoc />
    protected override bool RegisterImpl()
    {
        if (!this.IsLoaded)
        {
            return false;
        }

        var recipes = new List<string>();
        if (CombatModule.Config.RingsEnchantments.CraftableGemstoneRings)
        {
            recipes.AddRange(new[]
            {
                "Amethyst Ring", "Topaz Ring", "Aquamarine Ring", "Jade Ring", "Emerald Ring", "Ruby Ring",
                "Garnet Ring",
            });
        }

        this.ModApi.AddRecipesToDefaultCategory(false, "combat_rings", recipes);
        Log.D("[CMBT]: Registered the Better Crafting integration.");
        return true;
    }
}
