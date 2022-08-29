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

using Common;
using Common.Extensions.Reflection;
using Common.Extensions.Stardew;
using Common.Harmony;
using Enchantments;
using HarmonyLib;
using Netcode;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponCtorPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal MeleeWeaponCtorPatch()
    {
        Target = RequireConstructor<MeleeWeapon>(typeof(int));
    }

    #region harmony patches

    /// <summary>Add intrinsic weapon enchantments.</summary>
    [HarmonyPostfix]
    private static void MeleeWeaponCtorPostfix(MeleeWeapon __instance)
    {
        if (!ModEntry.Config.InfinityPlusOneWeapons || __instance.isScythe()) return;

        switch (__instance.InitialParentTileIndex)
        {
            case Constants.DARK_SWORD_INDEX_I:
                __instance.enchantments.Add(new DemonicEnchantment());
                __instance.specialItem = true;
                __instance.Write("EnemiesSlain", 0.ToString());
                break;
            case Constants.HOLY_BLADE_INDEX_I:
                __instance.enchantments.Add(new HolyEnchantment());
                __instance.specialItem = true;
                break;
            case Constants.INFINITY_BLADE_INDEX_I:
            case Constants.INFINITY_DAGGER_INDEX_I:
            case Constants.INFINITY_CLUB_INDEX_I:
                __instance.enchantments.Add(new InfinityEnchantment());
                __instance.specialItem = true;
                break;
        }
    }

    /// <summary>Prevent the game from overriding stabby swords.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? MeleeWeaponCtorTranspiler(IEnumerable<CodeInstruction> instructions,
        MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// Removed: if ((int)type == 0) { type.Set(3); }

        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldfld, typeof(MeleeWeapon).RequireField(nameof(MeleeWeapon.type)))
                )
                .FindNext(
                    new CodeInstruction(OpCodes.Ldfld, typeof(MeleeWeapon).RequireField(nameof(MeleeWeapon.type)))
                )
                .Retreat()
                .RemoveUntil(
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(NetFieldBase<int, NetInt>).RequireMethod(nameof(NetFieldBase<int, NetInt>.Set)))
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed removing stabby sword override.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}