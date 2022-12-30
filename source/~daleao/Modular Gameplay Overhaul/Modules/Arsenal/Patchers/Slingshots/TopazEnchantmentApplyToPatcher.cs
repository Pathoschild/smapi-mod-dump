/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Arsenal.Patchers.Slingshots;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class TopazEnchantmentApplyToPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="TopazEnchantmentApplyToPatcher"/> class.</summary>
    internal TopazEnchantmentApplyToPatcher()
    {
        this.Target = this.RequireMethod<TopazEnchantment>("_ApplyTo");
        this.Prefix!.before = new[] { "DaLion.Arsenal" };
    }

    #region harmony patches

    /// <summary>Rebalances Topaz enchant.</summary>
    [HarmonyPrefix]
    [HarmonyBefore("DaLion.Arsenal")]
    private static bool TopazEnchantmentApplyToPrefix(TopazEnchantment __instance, Item item)
    {
        if (item is not Slingshot)
        {
            return true; // run original logic
        }

        Game1.player.resilience += __instance.GetLevel();
        return false; // don't run original logic
    }

    #endregion harmony patches
}
