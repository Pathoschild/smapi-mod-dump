/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Tools;

using Stardew.Common.Extensions;
using Stardew.Common.Harmony;
using AssetLoaders;
using Extensions;
using SuperMode;

using SObject = StardewValley.Object;

#endregion using directives

internal class GameLocationDamageMonsterPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal GameLocationDamageMonsterPatch()
    {
        Original = RequireMethod<GameLocation>(nameof(GameLocation.damageMonster),
            new[]
            {
                typeof(Rectangle), typeof(int), typeof(int), typeof(bool), typeof(float), typeof(int),
                typeof(float), typeof(float), typeof(bool), typeof(Farmer)
            });
    }

    #region harmony patches

    /// <summary>
    ///     Patch to move critical chance bonus from Scout to Poacher + patch Brute damage bonus + move critical damage
    ///     bonus from Desperado to Poacher + increment Brute Fury and Poacher Cold Blood gauges + perform Poacher steal.
    /// </summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> GameLocationDamageMonsterTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// From: if (who.professions.Contains(<scout_id>) critChance += critChance * 0.5f;
        /// To: if (who.professions.Contains(<poacher_id>) critChance *= 2f;

        try
        {
            helper
                .FindProfessionCheck(Farmer.scout) // find index of scout check
                .Advance()
                .SetOperand((int) Profession.Poacher) // replace with Poacher check
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Ldarg_S) // start of critChance += critChance * 0.5f
                )
                .Advance()
                .Remove() // was Ldarg_S critChance
                .SetOperand(2f) // was 0.5f
                .Advance(2)
                .Remove(); // was Add
        }
        catch (Exception ex)
        {
            Log.E($"Failed while moving modded bonus crit chance from Scout to Poacher.\nHelper returned {ex}");
            transpilationFailed = true;
            return null;
        }

        /// From: if (who is not null && who.professions.Contains(<fighter_id>) ... *= 1.1f;
        /// To: if (who is not null && who.professions.Contains(<fighter_id>) ... *= who.professions.Contains(100 + <fighter_id>) ? 1.2f : 1.1f;

        var notPrestigedFighter = generator.DefineLabel();
        var resumeExecution = generator.DefineLabel();
        try
        {
            helper
                .FindProfessionCheck((int) Profession.Fighter,
                    true) // find index of brute check
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Ldc_R4, 1.1f) // brute damage multiplier
                )
                .AddLabels(notPrestigedFighter)
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_S, (byte) 10) // arg 10 = Farmer who
                )
                .InsertProfessionCheckForPlayerOnStack((int) Profession.Fighter + 100,
                    notPrestigedFighter)
                .Insert(
                    new CodeInstruction(OpCodes.Ldc_R4, 1.2f),
                    new CodeInstruction(OpCodes.Br_S, resumeExecution)
                )
                .Advance()
                .AddLabels(resumeExecution);
        }
        catch (Exception ex)
        {
            Log.E($"Failed while patching prestiged Fighter bonus damage.\nHelper returned {ex}");
            transpilationFailed = true;
            return null;
        }

        /// Injected: if (who.IsLocalPlayer && ModEntry.PlayerState.Value.SuperMode is BruteFury fury) ... += fury.GetBonusDamageMultiplier(who)
        /// After: if (who is not null && who.professions.Contains(<brute_id>) ... *= 1.15f;

        resumeExecution = generator.DefineLabel();
        var bruteFury = generator.DeclareLocal(typeof(BruteFury));
        try
        {
            helper
                .FindProfessionCheck((int) Profession.Brute,
                    true) // find index of brute check
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Ldc_R4, 1.15f) // brute damage multiplier
                )
                .Advance()
                .AddLabels(resumeExecution)
                .Insert(
                    // check for local player
                    new CodeInstruction(OpCodes.Ldarg_S, (byte) 10), // arg 10 = Farmer who
                    new CodeInstruction(OpCodes.Call, typeof(Farmer).PropertyGetter(nameof(Farmer.IsLocalPlayer))),
                    new CodeInstruction(OpCodes.Brfalse_S, resumeExecution),
                    // check for Brute Fury
                    new CodeInstruction(OpCodes.Call,
                        typeof(ModEntry).PropertyGetter(nameof(ModEntry.PlayerState))),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(PerScreen<PlayerState>).PropertyGetter(nameof(PerScreen<PlayerState>.Value))),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(PlayerState).PropertyGetter(nameof(PlayerState.SuperMode))),
                    new CodeInstruction(OpCodes.Isinst, typeof(BruteFury)),
                    new CodeInstruction(OpCodes.Stloc_S, bruteFury),
                    new CodeInstruction(OpCodes.Ldloc_S, bruteFury),
                    new CodeInstruction(OpCodes.Brfalse_S, resumeExecution),
                    // increase damage bonus
                    new CodeInstruction(OpCodes.Ldloc_S, bruteFury),
                    new CodeInstruction(OpCodes.Ldarg_S, (byte) 10),
                    new CodeInstruction(OpCodes.Call, typeof(BruteFury).MethodNamed(nameof(BruteFury.GetBonusDamageMultiplier))),
                    new CodeInstruction(OpCodes.Add)
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while patching modded Brute bonus damage.\nHelper returned {ex}");
            transpilationFailed = true;
            return null;
        }

        /// From: if (who is not null && crit && who.professions.Contains(<desperado_id>) ... *= 2f;
        /// To: if (who is not null && crit && who.IsLocalPlayer && SuperMode is PoacherColdBlood poacherColdBlood) ... *= poacherColdBlood.GetCritDamageMultiplier();

        var poacherColdBlood = generator.DeclareLocal(typeof(PoacherColdBlood));
        try
        {
            helper
                .FindProfessionCheck(Farmer.desperado, true) // find index of desperado check
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Brfalse_S)
                )
                .GetOperand(out var dontIncreaseCritPow)
                .Return()
                .ReplaceWith(
                    new(OpCodes.Callvirt,
                        typeof(Farmer).PropertyGetter(nameof(Farmer.IsLocalPlayer))) // was Ldfld Farmer.professions
                )
                .Advance()
                .ReplaceWith(
                    new(OpCodes.Brfalse_S, dontIncreaseCritPow) // was Ldc_I4_S <desperado id>
                )
                .Advance()
                .Remove(2) // was Callvirt NetList.Contains
                .Insert(
                    // check for Poacher Cold Blood
                    new CodeInstruction(OpCodes.Call, typeof(ModEntry).PropertyGetter(nameof(ModEntry.PlayerState))),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(PerScreen<PlayerState>).PropertyGetter(nameof(PerScreen<PlayerState>.Value))),
                    new CodeInstruction(OpCodes.Callvirt, typeof(PlayerState).PropertyGetter(nameof(PlayerState.SuperMode))),
                    new CodeInstruction(OpCodes.Isinst, typeof(PoacherColdBlood)),
                    new CodeInstruction(OpCodes.Stloc_S, poacherColdBlood),
                    new CodeInstruction(OpCodes.Ldloc_S, poacherColdBlood),
                    new CodeInstruction(OpCodes.Brfalse_S, dontIncreaseCritPow)
                )
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Ldc_R4, 2f) // desperado critical damage multiplier
                )
                .ReplaceWith(
                    new(OpCodes.Call,
                        typeof(PoacherColdBlood).MethodNamed(nameof(PoacherColdBlood.GetCritDamageMultiplier)))
                )
                .Insert(
                    new CodeInstruction(OpCodes.Ldloc_S, poacherColdBlood)
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while moving modded bonus crit damage from Desperado to Poacher.\nHelper returned {ex}");
            transpilationFailed = true;
            return null;
        }

        /// Injected: DamageMonsterSubroutine(damageAmount, isBomb, crit, critMultiplier, monster, who);
        ///	Before: if (monster.Health <= 0)

        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldloc_S, $"{typeof(bool)} (7)")
                )
                .GetOperand(out var didCrit) // copy reference to local 7 = Crit (whether player performed a crit)
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldloc_S, $"{typeof(int)} (8)")
                )
                .GetOperand(out var damageAmount)
                .FindFirst( // monster.Health <= 0
                    new CodeInstruction(OpCodes.Ldloc_2),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(Monster).PropertyGetter(nameof(Monster.Health))),
                    new CodeInstruction(OpCodes.Ldc_I4_0),
                    new CodeInstruction(OpCodes.Bgt)
                )
                .StripLabels(out var labels) // backup and remove branch labels
                .Insert(
                    // restore backed-up labels
                    labels,
                    // prepare arguments
                    new CodeInstruction(OpCodes.Ldloc_S, damageAmount),
                    new CodeInstruction(OpCodes.Ldarg_S, (byte) 4), // arg 4 = bool isBomb
                    new CodeInstruction(OpCodes.Ldloc_S, didCrit),
                    new CodeInstruction(OpCodes.Ldarg_S, (byte) 8), // arg 8 = float critMultiplier
                    new CodeInstruction(OpCodes.Ldloc_2), // local 2 = Monster monster
                    new CodeInstruction(OpCodes.Ldarg_S, (byte) 10), // arg 10 = Farmer who
                    new CodeInstruction(OpCodes.Call,
                        typeof(GameLocationDamageMonsterPatch).MethodNamed(nameof(DamageMonsterSubroutine)))
                )
                .Return()
                .AddLabels(labels);
        }
        catch (Exception ex)
        {
            Log.E($"Failed while injecting modded Poacher snatch attempt plus Brute Fury and Poacher Cold Blood gauges.\nHelper returned {ex}");
            transpilationFailed = true;
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static void DamageMonsterSubroutine(int damageAmount, bool isBomb, bool didCrit, float critMultiplier,
        Monster monster, Farmer who)
    {
        if (damageAmount <= 0 || isBomb || who is not {IsLocalPlayer: true, CurrentTool: MeleeWeapon weapon} ||
            ModEntry.PlayerState.Value.SuperMode is not { } superMode) return;

        // try to steal
        if (didCrit && superMode is PoacherColdBlood)
        {
            var alreadyStoleFromThisMonster = monster.ReadDataAs<bool>("Stolen");
            if (!alreadyStoleFromThisMonster &&
                Game1.random.NextDouble() < (weapon.type.Value == MeleeWeapon.dagger ? 0.5 : 0.25))
            {
                var drops = monster.objectsToDrop.Select(o => new SObject(o, 1) as Item)
                    .Concat(monster.getExtraDropItems()).ToList();
                var itemToSteal = drops.ElementAtOrDefault(Game1.random.Next(drops.Count))?.getOne();
                if (itemToSteal is not null && !itemToSteal.Name.Contains("Error") &&
                    who.addItemToInventoryBool(itemToSteal))
                {
                    monster.WriteData("Stolen", bool.TrueString);

                    // play sound effect
                    SoundBank.Play(SFX.PoacherSteal);
                }
            }
        }

        // try to increment Super Mode gauges
        if (superMode.IsActive) return;

        var increment = 0;
        // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
        switch (superMode)
        {
            case BruteFury:
            {
                increment = 2;
                if (monster.Health <= 0) increment *= 2;
                if (weapon.type.Value == MeleeWeapon.club) increment *= 2;
                break;
            }
            case PoacherColdBlood:
            {
                increment = 2;
                if (didCrit) increment *= (int) critMultiplier;
                if (weapon.type.Value == MeleeWeapon.dagger) increment *= 2;
                break;
            }
            case PiperEubstance when monster.IsSlime() && monster.currentLocation.characters
                .OfType<Monster>().Any(m => !m.IsSlime()):
            {
#pragma warning disable CS8509
                increment = monster switch
#pragma warning restore CS8509
                {
                    GreenSlime => 4,
                    BigSlime => 8,
                };

                if (monster.Health <= 0)
                {
                    increment *= 2;
                    if (who.HasProfession(Profession.Piper, true))
                    {
                        var healed = (int) (who.maxHealth * 0.025f);
                        who.health = Math.Min(who.health + healed, who.maxHealth);
                        monster.currentLocation.debris.Add(new(healed,
                            new(who.getStandingX() + 8, who.getStandingY()), Color.Lime, 1f, who));

                        who.Stamina = Math.Min(who.Stamina + who.Stamina * 0.01f, who.MaxStamina);
                    }
                }

                break;
            }
        }

        superMode.ChargeValue += increment * ModEntry.Config.SuperModeGainFactor *
            SuperMode.MaxValue / SuperMode.INITIAL_MAX_VALUE_I;
    }

    #endregion injected subroutines
}