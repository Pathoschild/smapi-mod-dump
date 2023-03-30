/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Weapons.Patchers.Infinity;

#region using directives

using DaLion.Overhaul.Modules.Weapons.Enchantments;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class BaseEnchantmentGetEnchantmentFromItemPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="BaseEnchantmentGetEnchantmentFromItemPatcher"/> class.</summary>
    internal BaseEnchantmentGetEnchantmentFromItemPatcher()
    {
        this.Target = this.RequireMethod<BaseEnchantment>(nameof(BaseEnchantment.GetEnchantmentFromItem));
    }

    #region harmony patches

    /// <summary>Allow Hero Soul forge.</summary>
    [HarmonyPostfix]
    private static void BaseEnchantmentGetEnchantmentFromItemPostfix(ref BaseEnchantment? __result, Item item)
    {
        if (__result is null && Globals.HeroSoulIndex.HasValue && item.ParentSheetIndex == Globals.HeroSoulIndex.Value)
        {
            __result = new InfinityEnchantment();
        }
    }

    #endregion harmony patches
}
