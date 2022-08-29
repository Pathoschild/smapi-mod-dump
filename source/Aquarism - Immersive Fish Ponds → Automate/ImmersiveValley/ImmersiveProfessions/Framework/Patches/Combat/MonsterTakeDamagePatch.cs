/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

#region using directives

using Extensions;
using HarmonyLib;
using StardewValley.Monsters;
using System.Linq;
using VirtualProperties;

#endregion using directives

[UsedImplicitly]
internal sealed class MonsterTakeDamagePatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal MonsterTakeDamagePatch()
    {
        Target = RequireMethod<Monster>(nameof(Monster.takeDamage),
            new[] { typeof(int), typeof(int), typeof(int), typeof(bool), typeof(double), typeof(string) });
    }

    #region harmony patches

    /// <summary>Patch to reset monster aggro.</summary>
    [HarmonyPostfix]
    private static void MonsterTakeDamagePostfix(Monster __instance)
    {
        if (__instance is not GreenSlime slime || slime.get_Piper() is null ||
            slime.Health > 0) return;

        foreach (var monster in slime.currentLocation.characters.OfType<Monster>()
                     .Where(m => !m.IsSlime() && m.get_Taunter().Get(m.currentLocation) == slime))
            monster.set_Taunter(null);
    }

    #endregion harmony patches
}