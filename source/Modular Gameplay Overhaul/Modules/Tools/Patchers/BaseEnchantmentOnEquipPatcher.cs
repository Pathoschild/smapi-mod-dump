/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
**
*************************************************/

namespace DaLion.Overhaul.Modules.Tools.Patchers;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class BaseEnchantmentOnEquipPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="BaseEnchantmentOnEquipPatcher"/> class.</summary>
    internal BaseEnchantmentOnEquipPatcher()
    {
        this.Target = this.RequireMethod<BaseEnchantment>("_OnEquip");
    }

    #region harmony patches

    /// <summary>Universal Master enchantment.</summary>
    [HarmonyPrefix]
    private static bool BaseEnchantmentOnEquipPrefix(BaseEnchantment __instance, Farmer who)
    {
        if (__instance is not MasterEnchantment)
        {
            return true; // run original logic
        }

        switch (who.CurrentTool)
        {
            case Axe:
                who.addedForagingLevel.Value++;
                break;
            case Pickaxe:
                who.addedMiningLevel.Value++;
                break;
            case Hoe:
            case WateringCan:
                who.addedFarmingLevel.Value++;
                break;
            case FishingRod:
                who.addedFishingLevel.Value++;
                break;
        }

        return false; // don't run original logic
    }

    #endregion harmony patches
}
