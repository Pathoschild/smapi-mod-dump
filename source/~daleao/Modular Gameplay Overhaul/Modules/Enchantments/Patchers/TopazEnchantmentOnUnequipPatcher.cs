/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Enchantments.Patchers;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class TopazEnchantmentOnUnequipPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="TopazEnchantmentOnUnequipPatcher"/> class.</summary>
    internal TopazEnchantmentOnUnequipPatcher()
    {
        this.Target = this.RequireMethod<TopazEnchantment>("_OnUnequip");
    }

    #region harmony patches

    /// <summary>Rebalances Topaz enchantment for Slingshots.</summary>
    [HarmonyPrefix]
    private static bool TopazEnchantmentOnUnequipPrefix(TopazEnchantment __instance, Item item)
    {
        if (item is not Slingshot || !EnchantmentsModule.Config.RebalancedForges || CombatModule.IsEnabled)
        {
            return true; // run original logic
        }

        Game1.player.resilience -= __instance.GetLevel();
        return false; // don't run original logic
    }

    #endregion harmony patches
}
