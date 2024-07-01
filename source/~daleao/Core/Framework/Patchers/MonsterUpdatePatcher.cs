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

using System.Reflection;
using DaLion.Core.Framework.Debuffs;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal sealed class MonsterUpdatePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MonsterUpdatePatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal MonsterUpdatePatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target =
            this.RequireMethod<Monster>(nameof(Monster.update), [typeof(GameTime), typeof(GameLocation)]);
        this.Prefix!.priority = Priority.First;
    }

    #region harmony patches

    /// <summary>Slow and damage-over-time effects.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.First)]
    private static bool MonsterUpdatePrefix(Monster __instance, GameTime time)
    {
        try
        {
            var ticks = time.TotalGameTime.Ticks;
            Farmer? killer = null;
            if (ticks % 30 == 0)
            {
                var bleedHolder = __instance.Get_BleedHolder();
                if (bleedHolder.BleedTimer.Value > 0)
                {
                    var (bleedTimer, bleedStacks, bleeder) = bleedHolder;
                    bleedTimer.Value -= time.ElapsedGameTime.Milliseconds;
                    if (bleedTimer.Value <= 0)
                    {
                        bleedTimer.Value = -1;
                        bleedStacks.Value = 0;
                        bleedHolder.Bleeder = null;
                        __instance.stopGlowing();
                    }
                    else
                    {
                        if (ticks % 60 == 0)
                        {
                            var bleed = (int)Math.Pow(2.5, bleedStacks.Value);
                            __instance.Health -= bleed;
                            Log.D($"{__instance.Name} suffered {bleed} bleed damage. HP Left: {__instance.Health}");
                            if (__instance.Health <= 0)
                            {
                                killer = bleeder;
                            }
                        }

                        __instance.startGlowing(Color.Maroon, true, 0.05f);
                    }
                }

                var burnHolder = __instance.Get_BurnHolder();
                if (burnHolder.BurnTimer.Value > 0)
                {
                    var (burnTimer, burner) = burnHolder;
                    burnTimer.Value -= time.ElapsedGameTime.Milliseconds;
                    if (burnTimer.Value <= 0)
                    {
                        __instance.jitteriness.Value /= 2;
                        burnTimer.Value = -1;
                        burnHolder.Burner = null;
                        __instance.stopGlowing();
                    }
                    else
                    {
                        if ((__instance is Bug or Fly && ticks % 30 == 0) || ticks % 180 == 0)
                        {
                            var burn = (int)(1d / 16d * __instance.MaxHealth);
                            __instance.Health -= burn;
                            Log.D($"{__instance.Name} suffered {burn} burn damage. HP Left: {__instance.Health}");
                            if (__instance.Health <= 0)
                            {
                                killer = burner;
                            }
                        }

                        __instance.startGlowing(Color.Yellow, true, 0.05f);
                    }
                }

                var poisonHolder = __instance.Get_PoisonHolder();
                if (poisonHolder.PoisonTimer.Value > 0)
                {
                    var (poisonTimer, poisonStacks, poisoner) = poisonHolder;
                    poisonTimer.Value -= time.ElapsedGameTime.Milliseconds;
                    if (poisonTimer.Value <= 0)
                    {
                        poisonTimer.Value = -1;
                        poisonStacks.Value = 0;
                        poisonHolder.Poisoner = null;
                        __instance.stopGlowing();
                    }
                    else
                    {
                        var pow = (int)Math.Pow(2, poisonStacks.Value - 1);
                        if (ticks % (180 / pow) == 0)
                        {
                            var poison = (int)(pow * __instance.MaxHealth / 16d);
                            __instance.Health -= poison;
                            Log.D($"{__instance.Name} suffered {poison} poison damage. HP Left: {__instance.Health}");
                            if (__instance.Health <= 0)
                            {
                                killer = poisoner;
                            }
                        }

                        __instance.startGlowing(Color.LimeGreen, true, 0.05f);
                    }
                }

                if (__instance.Health <= 0)
                {
                    __instance.Die(killer);
                    return false; // run original logic
                }
            }

            var slowHolder = __instance.Get_SlowHolder();
            if (slowHolder.SlowTimer.Value <= 0)
            {
                return true; // run original logic
            }

            var (slowTimer, slowIntensity) = slowHolder;
            slowTimer.Value -= time.ElapsedGameTime.Milliseconds;
            if (slowTimer.Value <= 0)
            {
                var chilled = __instance.Get_Chilled();
                if (chilled.Value)
                {
                    chilled.Value = false;
                    var frozen = __instance.Get_Frozen();
                    if (frozen.Value)
                    {
                        frozen.Value = false;
                    }
                }

                __instance.stopGlowing();
                slowTimer.Value = -1;
                return true; // run original logic
            }

            if (__instance.IsChilled())
            {
                __instance.startGlowing(Color.PowderBlue, true, 0.05f);
                if (__instance.IsFrozen())
                {
                    __instance.glowingTransparency = 1f;
                }
            }

            if (slowIntensity.Value < 1f && ticks % (int)(1f / slowIntensity.Value) == 0f)
            {
                return true; // run original logic
            }

            if (Reflector.GetUnboundFieldGetter<Monster, int>("invincibleCountdown")
                    .Invoke(__instance) is not (var invincibility and > 0))
            {
                return false; // don't run original logic
            }

            invincibility -= time.ElapsedGameTime.Milliseconds;
            Reflector.GetUnboundFieldSetter<Monster, int>("invincibleCountdown")
                .Invoke(__instance, invincibility);
            return false; // don't run original logic
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    #endregion harmony patches
}
