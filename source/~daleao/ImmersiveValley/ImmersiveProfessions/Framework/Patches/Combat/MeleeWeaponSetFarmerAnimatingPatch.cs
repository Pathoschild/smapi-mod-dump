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

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.Tools;

using DaLion.Common.Extensions.Reflection;
using DaLion.Common.Harmony;
using Extensions;
using Ultimate;

#endregion using directives

[UsedImplicitly]
internal class MeleeWeaponSetFarmerAnimatingPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal MeleeWeaponSetFarmerAnimatingPatch()
    {
        Original = RequireMethod<MeleeWeapon>(nameof(MeleeWeapon.setFarmerAnimating));
    }

    #region harmony patches

    /// <summary>Patch to increase prestiged Brute attack speed with rage.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> MeleeWeaponSetFarmerAnimatingTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// Injected: if (who.professions.Contains(100 + <brute_id>) swipeSpeed *= 1f - ModEntry.PlayerState.BruteRageCounter * 0.005f;
        /// After: if (who.IsLocalPlayer)

        var skipRageBonus = generator.DefineLabel();
        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Callvirt, typeof(Farmer).RequirePropertyGetter(nameof(Farmer.IsLocalPlayer)))
                )
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Ldarg_0)
                )
                .AddLabels(skipRageBonus)
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_1) // arg 1 = Farmer who
                )
                .InsertProfessionCheck((int) Profession.Brute + 100, forLocalPlayer: false)
                .Insert(
                    new CodeInstruction(OpCodes.Brfalse_S, skipRageBonus),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, typeof(MeleeWeapon).RequireField("swipeSpeed")),
                    new CodeInstruction(OpCodes.Ldc_R4, 1f),
                    new CodeInstruction(OpCodes.Call,
                        typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.PlayerState))),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(PlayerState).RequirePropertyGetter(nameof(PlayerState.BruteRageCounter))),
                    new CodeInstruction(OpCodes.Conv_R4),
                    new CodeInstruction(OpCodes.Ldc_R4, Frenzy.PCT_INCREMENT_PER_RAGE_F / 2f),
                    new CodeInstruction(OpCodes.Mul),
                    new CodeInstruction(OpCodes.Sub),
                    new CodeInstruction(OpCodes.Mul),
                    new CodeInstruction(OpCodes.Stfld, typeof(MeleeWeapon).RequireField("swipeSpeed"))
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding attack speed to prestiged Brute.\nHelper returned {ex}");
            transpilationFailed = true;
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}