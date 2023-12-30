/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
**
*************************************************/

namespace DaLion.Overhaul.Modules.Combat.Patchers.Enchantments;

#region using directives

using System.Collections.Generic;
using DaLion.Overhaul.Modules.Combat.VirtualProperties;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class RubyEnchantmentUnapplyToPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="RubyEnchantmentUnapplyToPatcher"/> class.</summary>
    internal RubyEnchantmentUnapplyToPatcher()
    {
        this.Target = this.RequireMethod<RubyEnchantment>("_UnapplyTo");
    }

    #region harmony patches

    /// <summary>Adjust Ruby enchant for randomized weapons.</summary>
    [HarmonyPrefix]
    private static bool RubyEnchantmentUnapplyToPrefix(RubyEnchantment __instance, Item item)
    {
        if (item is not MeleeWeapon weapon || !CombatModule.Config.RingsEnchantments.RebalancedGemstones)
        {
            return true; // run original logic
        }

        var data = ModHelper.GameContent
            .Load<Dictionary<int, string>>("Data/weapons")[weapon.InitialParentTileIndex]
            .SplitWithoutAllocation('/');
        weapon.minDamage.Value -=
            (int)(weapon.Read(DataKeys.BaseMinDamage, int.Parse(data[2])) * __instance.GetLevel() * 0.1f);
        weapon.maxDamage.Value -=
            (int)(weapon.Read(DataKeys.BaseMaxDamage, int.Parse(data[3])) * __instance.GetLevel() * 0.1f);
        return false; // don't run original logic
    }

    /// <summary>Reset cached stats.</summary>
    [HarmonyPostfix]
    private static void RubyEnchantmentUnapplyPostfix(Item item)
    {
        switch (item)
        {
            case MeleeWeapon weapon:
                weapon.Invalidate();
                break;
            case Slingshot slingshot:
                slingshot.Invalidate();
                break;
        }
    }

    #endregion harmony patches
}
