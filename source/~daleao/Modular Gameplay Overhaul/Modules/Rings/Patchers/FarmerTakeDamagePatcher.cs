/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Rings.Patchers;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Combat.Extensions;
using DaLion.Overhaul.Modules.Rings.Events;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Menus;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerTakeDamagePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FarmerTakeDamagePatcher"/> class.</summary>
    internal FarmerTakeDamagePatcher()
    {
        this.Target = this.RequireMethod<Farmer>(nameof(Farmer.takeDamage));
    }

    #region harmony patches

    /// <summary>Ring of Thorns and Ring of Yoba rebalance.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? FarmerTakeDamageTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            helper
                .Match(new[] { new CodeInstruction(OpCodes.Pop) })
                .Move()
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_0), // arg 0 = Farmer this
                        new CodeInstruction(OpCodes.Ldarg_3), // arg 3 = Monster damager
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(FarmerTakeDamagePatcher).RequireMethod(nameof(TryBleed))),
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed injecting Thorns Ring bleed chance.\nHelper returned {ex}");
            return null;
        }

        try
        {
            var doVanillaYoba = generator.DefineLabel();
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldc_I4_S, 21),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(BuffsDisplay).RequireMethod(nameof(BuffsDisplay.hasBuff))),
                    })
                .Move(2)
                .GetOperand(out var resumeExecution)
                .Move()
                .AddLabels(doVanillaYoba)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.Config))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.Rings))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(Config).RequirePropertyGetter(nameof(Config.RebalancedRings))),
                        new CodeInstruction(OpCodes.Brfalse_S, doVanillaYoba),
                        new CodeInstruction(OpCodes.Ldarg_0), // arg 0 = Farmer this
                        new CodeInstruction(OpCodes.Ldarg_1), // arg 1 = int damage
                        new CodeInstruction(OpCodes.Ldloc_3), // loc 3 = int effectiveResilience
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(FarmerTakeDamagePatcher).RequireMethod(nameof(TryGiveYobaShield))),
                        new CodeInstruction(OpCodes.Br_S, resumeExecution),
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed injecting Ring of Yoba overhaul (grant buff).\nHelper returned {ex}");
            return null;
        }

        try
        {
            var resumeExecution = generator.DefineLabel();
            helper
                .Match(
                    new[] { new CodeInstruction(OpCodes.Stfld, typeof(Farmer).RequireField(nameof(Farmer.health))) })
                .Match(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldc_I4_0),
                    },
                    ILHelper.SearchOption.Previous)
                .AddLabels(resumeExecution)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_0), // arg 0 = Farmer this
                        new CodeInstruction(OpCodes.Ldarg_1), // arg 1 = int damage
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(FarmerTakeDamagePatcher).RequireMethod(nameof(TryShieldDamage))),
                        new CodeInstruction(OpCodes.Starg_S, (byte)1),
                        new CodeInstruction(OpCodes.Ldarg_1),
                        new CodeInstruction(OpCodes.Ldc_I4_0),
                        new CodeInstruction(OpCodes.Bgt_Un_S, resumeExecution),
                        new CodeInstruction(OpCodes.Ret),
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed injecting Ring of Yoba overhaul (grant shield).\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static void TryBleed(Farmer who, Monster monster)
    {
        if (RingsModule.Config.RebalancedRings && CombatModule.ShouldEnable && Game1.random.NextDouble() < 0.25)
        {
            monster.Bleed(who);
        }
    }

    private static void TryGiveYobaShield(Farmer who, int rawDamage, int vanillaResistance)
    {
        if (RingsModule.State.CanReceiveYobaShield)
        {
            return;
        }

        var postMitigationDamage = CombatModule.Config.OverhauledDefense
            ? (int)(rawDamage * who.GetOverhauledResilience())
            : Math.Max(1, rawDamage - vanillaResistance);
        if (who.health - postMitigationDamage >= who.maxHealth * 0.3)
        {
            return;
        }

        who.currentLocation.playSound("yoba");
        RingsModule.State.YobaShieldHealth = (int)(who.maxHealth * 0.5);
        RingsModule.State.CanReceiveYobaShield = true;
        Game1.buffsDisplay.addOtherBuff(
            new Buff(21)
            {
                description = I18n.Get("buffs.yoba.desc"),
                millisecondsDuration = 30000,
            });

        EventManager.Enable<YobaRemoveUpdateTickedEvent>();
        EventManager.Enable<YobaCanReceiveUpdateTickedEvent>();
    }

    private static int TryShieldDamage(Farmer who, int actualDamage)
    {
        if (!who.IsLocalPlayer || RingsModule.State.YobaShieldHealth <= 0)
        {
            return actualDamage;
        }

        if (RingsModule.State.YobaShieldHealth >= actualDamage)
        {
            RingsModule.State.YobaShieldHealth -= actualDamage;
            Rumble.rumble(0.75f, 150f);
            who.temporarilyInvincible = true;
            who.temporaryInvincibilityTimer = 0;
            who.currentTemporaryInvincibilityDuration = 1200 + (who.GetEffectsOfRingMultiplier(861) * 400);
            who.currentLocation.debris.Add(new Debris(
                actualDamage,
                new Vector2(who.getStandingX() + 8, who.getStandingY()),
                Color.White,
                1f,
                who));
            who.currentLocation.playSound("clank");
            Game1.hitShakeTimer = 100 * actualDamage;
            return 0;
        }

        var shieldHealth = RingsModule.State.YobaShieldHealth;
        RingsModule.State.YobaShieldHealth = 0;
        Game1.buffsDisplay.removeOtherBuff(21);
        return actualDamage - shieldHealth;
    }

    #endregion injected subroutines
}
