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
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using StardewModdingAPI.Utilities;
using StardewValley;

using Stardew.Common.Harmony;
using SuperMode;

#endregion using directives

[UsedImplicitly]
internal class FarmerTakeDamagePatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal FarmerTakeDamagePatch()
    {
        Original = RequireMethod<Farmer>(nameof(Farmer.takeDamage));
    }

    #region harmony patches

    /// <summary>
    ///     Patch to make Poacher untargetable during Super Mode + increment Brute Fury for damage taken + add Brute super
    ///     mode immortality.
    /// </summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> FarmerTakeDamageTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// Injected: else if (this.IsLocalPlayer && SuperMode.IsActive && SuperMode.Index == <poacher_id>) monsterDamageCapable = false;

        var alreadyUndamageableOrNotAmbuscade = iLGenerator.DefineLabel();
        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Stloc_0)
                )
                .Advance()
                .AddLabels(alreadyUndamageableOrNotAmbuscade)
                .Insert(
                    // check if monsterDamageCapable is already false
                    new CodeInstruction(OpCodes.Ldloc_0),
                    new CodeInstruction(OpCodes.Brfalse_S, alreadyUndamageableOrNotAmbuscade),
                    // check if this.IsLocalPlayer
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call,
                        typeof(Farmer).PropertyGetter(nameof(Farmer.IsLocalPlayer))),
                    new CodeInstruction(OpCodes.Brfalse_S, alreadyUndamageableOrNotAmbuscade),
                    // check if SuperMode is null
                    new CodeInstruction(OpCodes.Call,
                        typeof(ModEntry).PropertyGetter(nameof(ModEntry.State))),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(PerScreen<ModState>).PropertyGetter(nameof(PerScreen<ModState>.Value))),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(ModState).PropertyGetter(nameof(ModState.SuperMode))),
                    new CodeInstruction(OpCodes.Brfalse_S, alreadyUndamageableOrNotAmbuscade),
                    // check if SuperMode.Index == <poacher_id>
                    new CodeInstruction(OpCodes.Call,
                        typeof(ModEntry).PropertyGetter(nameof(ModEntry.State))),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(PerScreen<ModState>).PropertyGetter(nameof(PerScreen<ModState>.Value))),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(ModState).PropertyGetter(nameof(ModState.SuperMode))),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(SuperMode).PropertyGetter(nameof(SuperMode.Index))),
                    new CodeInstruction(OpCodes.Ldc_I4_S, (int) SuperModeIndex.Poacher),
                    new CodeInstruction(OpCodes.Bne_Un_S, alreadyUndamageableOrNotAmbuscade),
                    // check if SuperMode.IsActive
                    new CodeInstruction(OpCodes.Call,
                        typeof(ModEntry).PropertyGetter(nameof(ModEntry.State))),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(PerScreen<ModState>).PropertyGetter(nameof(PerScreen<ModState>.Value))),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(ModState).PropertyGetter(nameof(ModState.SuperMode))),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(SuperMode).PropertyGetter(nameof(SuperMode.IsActive))),
                    new CodeInstruction(OpCodes.Brfalse_S, alreadyUndamageableOrNotAmbuscade),
                    // set monsterDamageCapable = false
                    new CodeInstruction(OpCodes.Ldc_I4_0),
                    new CodeInstruction(OpCodes.Stloc_0)
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while adding Poacher untargetability during Super Mode.\nHelper returned {ex}");
            return null;
        }

        /// Injected: if (IsSuperModeActive && SuperModeIndex == <brute_id>) health = 1;
        /// After: if (health <= 0)
        /// Before: GetEffectsOfRingMultiplier(863)

        var isNotUndyingButMayHaveDailyRevive = iLGenerator.DefineLabel();
        try
        {
            helper
                .FindNext( // find index of health <= 0 (start of revive ring effect)
                    new CodeInstruction(OpCodes.Ldarg_0), // arg 0 = Farmer this
                    new CodeInstruction(OpCodes.Ldfld,
                        typeof(Farmer).Field(nameof(Farmer.health))),
                    new CodeInstruction(OpCodes.Ldc_I4_0),
                    new CodeInstruction(OpCodes.Bgt)
                )
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Bgt)
                )
                .GetOperand(out var resumeExecution) // copy branch label to resume normal execution
                .Advance()
                .AddLabels(isNotUndyingButMayHaveDailyRevive)
                .Insert(
                    // check if SuperMode is null
                    new CodeInstruction(OpCodes.Call,
                        typeof(ModEntry).PropertyGetter(nameof(ModEntry.State))),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(PerScreen<ModState>).PropertyGetter(nameof(PerScreen<ModState>.Value))),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(ModState).PropertyGetter(nameof(ModState.SuperMode))),
                    new CodeInstruction(OpCodes.Brfalse_S, isNotUndyingButMayHaveDailyRevive),
                    // check if SuperMode.Index == <brute_id>
                    new CodeInstruction(OpCodes.Call,
                        typeof(ModEntry).PropertyGetter(nameof(ModEntry.State))),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(PerScreen<ModState>).PropertyGetter(nameof(PerScreen<ModState>.Value))),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(ModState).PropertyGetter(nameof(ModState.SuperMode))),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(SuperMode).PropertyGetter(nameof(SuperMode.Index))),
                    new CodeInstruction(OpCodes.Ldc_I4_S, (int) SuperModeIndex.Brute),
                    new CodeInstruction(OpCodes.Bne_Un_S, isNotUndyingButMayHaveDailyRevive),
                    // check if SuperMode.IsActive
                    new CodeInstruction(OpCodes.Call,
                        typeof(ModEntry).PropertyGetter(nameof(ModEntry.State))),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(PerScreen<ModState>).PropertyGetter(nameof(PerScreen<ModState>.Value))),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(ModState).PropertyGetter(nameof(ModState.SuperMode))),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(SuperMode).PropertyGetter(nameof(SuperMode.IsActive))),
                    new CodeInstruction(OpCodes.Brfalse_S, isNotUndyingButMayHaveDailyRevive),
                    // set health back to 1
                    new CodeInstruction(OpCodes.Ldarg_0), // arg 0 = Farmer this
                    new CodeInstruction(OpCodes.Ldc_I4_1),
                    new CodeInstruction(OpCodes.Stfld,
                        typeof(Farmer).Field(nameof(Farmer.health))),
                    // resume execution (skip revive ring effect)
                    new CodeInstruction(OpCodes.Br, resumeExecution)
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while adding Brute Super Mode immortality.\nHelper returned {ex}");
            return null;
        }

        /// Injected: if (SuperModeIndex == <brute_id> && damage > 0) SuperModeGaugeValue += 2;
        /// At: end of method (before return)

        var dontIncreaseBruteCounterForDamage = iLGenerator.DefineLabel();
        try
        {
            helper
                .FindLast( // find index of final return
                    new CodeInstruction(OpCodes.Ret)
                )
                .AddLabels(dontIncreaseBruteCounterForDamage) // branch here to skip gauge increment
                .Insert(
                    // check if SuperMode is null
                    new CodeInstruction(OpCodes.Call,
                        typeof(ModEntry).PropertyGetter(nameof(ModEntry.State))),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(PerScreen<ModState>).PropertyGetter(nameof(PerScreen<ModState>.Value))),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(ModState).PropertyGetter(nameof(ModState.SuperMode))),
                    new CodeInstruction(OpCodes.Brfalse_S, dontIncreaseBruteCounterForDamage),
                    // check if SuperMode.Index == <brute_id>
                    new CodeInstruction(OpCodes.Call,
                        typeof(ModEntry).PropertyGetter(nameof(ModEntry.State))),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(PerScreen<ModState>).PropertyGetter(nameof(PerScreen<ModState>.Value))),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(ModState).PropertyGetter(nameof(ModState.SuperMode))),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(SuperMode).PropertyGetter(nameof(SuperMode.Index))),
                    new CodeInstruction(OpCodes.Ldc_I4_S, (int) SuperModeIndex.Brute),
                    new CodeInstruction(OpCodes.Bne_Un_S, dontIncreaseBruteCounterForDamage),
                    // check if farmer received any damage
                    new CodeInstruction(OpCodes.Ldarg_1), // arg 1 = int damage
                    new CodeInstruction(OpCodes.Ldc_I4_0),
                    new CodeInstruction(OpCodes.Ble_S, dontIncreaseBruteCounterForDamage),
                    // if so, increment gauge by 2
                    new CodeInstruction(OpCodes.Call,
                        typeof(ModEntry).PropertyGetter(nameof(ModEntry.State))),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(PerScreen<ModState>).PropertyGetter(nameof(PerScreen<ModState>.Value))),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(ModState).PropertyGetter(nameof(ModState.SuperMode))),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(SuperMode).PropertyGetter(nameof(SuperMode.Gauge))),
                    new CodeInstruction(OpCodes.Dup),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(SuperModeGauge).PropertyGetter(nameof(SuperModeGauge.CurrentValue))),
                    new CodeInstruction(OpCodes.Ldc_R8, 2.0), // <-- increment amount
                    // increment by config factor
                    new CodeInstruction(OpCodes.Call, typeof(ModEntry).PropertyGetter(nameof(ModEntry.Config))),
                    new CodeInstruction(OpCodes.Callvirt, typeof(ModConfig).PropertyGetter(nameof(ModConfig.SuperModeGainFactor))),
                    new CodeInstruction(OpCodes.Conv_R8),
                    new CodeInstruction(OpCodes.Mul),
                    // scale for extended levels
                    new CodeInstruction(OpCodes.Call,
                        typeof(SuperModeGauge).PropertyGetter(nameof(SuperModeGauge.MaxValue))),
                    new CodeInstruction(OpCodes.Conv_R8),
                    new CodeInstruction(OpCodes.Ldc_R8, 500.0),
                    new CodeInstruction(OpCodes.Div),
                    new CodeInstruction(OpCodes.Mul),
                    new CodeInstruction(OpCodes.Add),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(SuperModeGauge).PropertySetter(nameof(SuperModeGauge.CurrentValue)))
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while adding Brute Fury gauge for damage taken.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}