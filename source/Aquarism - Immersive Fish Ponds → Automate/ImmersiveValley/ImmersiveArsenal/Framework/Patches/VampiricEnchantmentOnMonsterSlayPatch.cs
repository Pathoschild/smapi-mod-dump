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
using Microsoft.Xna.Framework;
using StardewValley.Monsters;
using System;

#endregion using directives

[UsedImplicitly]
internal sealed class VampiricEnchantmentOnMonsterSlayPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal VampiricEnchantmentOnMonsterSlayPatch()
    {
        Target = RequireMethod<VampiricEnchantment>("_OnMonsterSlay");
    }

    #region harmony patches

    /// <summary>Rebalances Vampiric enchant.</summary>
    [HarmonyPrefix]
    private static bool VampiricEnchantmentOnMonsterSlayPrefix(Monster m, GameLocation location, Farmer who)
    {
        if (!ModEntry.Config.NewWeaponEnchants) return true; // run original logic

        var amount = Math.Max((int)((m.MaxHealth + Game1.random.Next(-m.MaxHealth / 10, m.MaxHealth / 15)) * 0.05f),
            1);
        who.health = Math.Min(who.health + amount, (int)(who.maxHealth * 1.1));
        location.debris.Add(new(amount, new(who.getStandingX(), who.getStandingY()), Color.Lime, 1f, who));
        Game1.playSound("healSound");
        return false; // don't run original logic
    }

    #endregion harmony patches
}