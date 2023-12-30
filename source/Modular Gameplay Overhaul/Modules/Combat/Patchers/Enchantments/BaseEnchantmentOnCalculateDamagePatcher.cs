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

using System.Reflection;
using DaLion.Overhaul.Modules.Combat.Enchantments;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal sealed class BaseEnchantmentOnCalculateDamagePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="BaseEnchantmentOnCalculateDamagePatcher"/> class.</summary>
    internal BaseEnchantmentOnCalculateDamagePatcher()
    {
        this.Target = this.RequireMethod<BaseEnchantment>(nameof(BaseEnchantment.OnCalculateDamage));
    }

    #region harmony patches

    /// <summary>Redirect from _OnDealDamage.</summary>
    [HarmonyPrefix]
    private static bool BaseEnchantmentOnCalculateDamagePrefix(
        BaseEnchantment __instance, Monster monster, GameLocation location, Farmer who, ref int amount)
    {
        if (!CombatModule.Config.RingsEnchantments.NewPrismaticEnchantments)
        {
            return true; // run original logic
        }

        try
        {
            switch (__instance)
            {
                case ObsidianEnchantment obsidian:
                    obsidian.OnCalculateDamage(monster, location, who, ref amount);
                    goto default;
                default:
                    return false; // don't run original logic
            }
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    #endregion harmony patches
}
