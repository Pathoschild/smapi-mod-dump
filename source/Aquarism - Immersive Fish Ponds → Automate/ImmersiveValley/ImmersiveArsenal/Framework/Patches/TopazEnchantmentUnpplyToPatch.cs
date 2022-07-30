/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Arsenal.Framework.Patches;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class TopazEnchantmentUnpplyToPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal TopazEnchantmentUnpplyToPatch()
    {
        Target = RequireMethod<TopazEnchantment>("_UnapplyTo");
    }

    #region harmony patches

    /// <summary>Rebalances Topaz enchant.</summary>
    [HarmonyPostfix]
    private static void TopazEnchantmentUnpplyToPostfix(TopazEnchantment __instance, Item item)
    {
        switch (item)
        {
            case MeleeWeapon weapon when ModEntry.Config.RebalancedEnchants:
                weapon.addedDefense.Value -= 4 * __instance.GetLevel();
                break;
            case Slingshot:
                Game1.player.resilience -= ModEntry.Config.RebalancedEnchants ? 5 : 1;
                break;
        }
    }

    #endregion harmony patches
}