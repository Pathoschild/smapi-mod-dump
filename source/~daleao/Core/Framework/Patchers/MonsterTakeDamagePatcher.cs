/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Core.Framework.Patchers;

#region using directives

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DaLion.Core.Framework.Events;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal sealed class MonsterTakeDamagePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MonsterTakeDamagePatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal MonsterTakeDamagePatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<Monster>(
            nameof(Monster.takeDamage),
            [typeof(int), typeof(int), typeof(int), typeof(bool), typeof(double), typeof(Farmer)]);
    }

    /// <inheritdoc />
    protected override bool ApplyImpl(Harmony harmony)
    {
        if (!base.ApplyImpl(harmony))
        {
            return false;
        }

        foreach (var target in TargetMethods())
        {
            this.Target = target;
            if (!base.ApplyImpl(harmony))
            {
                return false;
            }
        }

        return true;
    }

    /// <inheritdoc />
    protected override bool UnapplyImpl(Harmony harmony)
    {
        if (!base.UnapplyImpl(harmony))
        {
            return false;
        }

        foreach (var target in TargetMethods())
        {
            this.Target = target;
            if (!base.UnapplyImpl(harmony))
            {
                return false;
            }
        }

        return true;
    }

    [HarmonyTargetMethods]
    private static IEnumerable<MethodBase> TargetMethods()
    {
        return new[]
        {
            typeof(AngryRoger), typeof(Bat), typeof(BigSlime), typeof(BlueSquid), typeof(Bug), typeof(Duggy),
            typeof(DwarvishSentry), typeof(Fly), typeof(Ghost), typeof(GreenSlime), typeof(Grub), typeof(Mummy),
            typeof(RockCrab), typeof(RockGolem), typeof(ShadowGirl), typeof(ShadowGuy), typeof(ShadowShaman),
            typeof(Spiker), typeof(SquidKid), typeof(Serpent),
        }.Select(t => t.RequireMethod(
            "takeDamage",
            [typeof(int), typeof(int), typeof(int), typeof(bool), typeof(double), typeof(Farmer)]));
    }

    #region harmony patches

    /// <summary>Frozen effect.</summary>
    [HarmonyPrefix]
    private static void MonsterTakeDamagePrefix(Monster __instance, ref int damage)
    {
        if (!__instance.IsFrozen())
        {
            return;
        }

        damage *= 2;
        __instance.Defrost();
    }

    /// <summary>Reset seconds out of combat.</summary>
    [HarmonyPostfix]
    private static void MonsterTakeDamagePostfix(Farmer who)
    {
        if (who.IsLocalPlayer)
        {
            EventManager.Enable<OutOfCombatOneSecondUpdateTickedEvent>();
        }
    }

    #endregion harmony patches
}
