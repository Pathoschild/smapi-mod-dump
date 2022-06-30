/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Stardew.Arsenal.Framework.Patches;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal sealed class MonsterParriedPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal MonsterParriedPatch()
    {
        Target = RequireMethod<Monster>(nameof(Monster.parried));
    }

    #region harmony patches

    /// <summary>Adds stamina cost to sword parry.</summary>
    [HarmonyPostfix]
    private static void MonsterParriedPostfix(Farmer who)
    {
        if (ModEntry.Config.WeaponsCostStamina)
            who.Stamina -= 2 - who.CombatLevel * 0.1f;
    }

    #endregion harmony patches
}