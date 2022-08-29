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
using Common.Harmony;
using HarmonyLib;
using Netcode;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponDoDamagePatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal MeleeWeaponDoDamagePatch()
    {
        Target = RequireMethod<MeleeWeapon>(nameof(MeleeWeapon.DoDamage));
    }

    #region harmony patches

    /// <summary>Override `special = false` for stabby sword.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? MeleeWeaponDoDamageTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// From: isOnSpecial = false;
        /// To: isOnSpecial = (type.Value == MeleeWeapon.stabbingSword && isOnSpecial);

        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Stfld,
                        typeof(MeleeWeapon).RequireField(nameof(MeleeWeapon.isOnSpecial)))
                )
                .Retreat()
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, typeof(MeleeWeapon).RequireField(nameof(MeleeWeapon.type))),
                    new CodeInstruction(OpCodes.Call, typeof(NetFieldBase<int, NetInt>).RequireMethod("op_Implicit")),
                    new CodeInstruction(OpCodes.Ldc_I4_0),
                    new CodeInstruction(OpCodes.Ceq),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld,
                        typeof(MeleeWeapon).RequireField(nameof(MeleeWeapon.isOnSpecial))),
                    new CodeInstruction(OpCodes.And)
                )
                .Remove();
        }
        catch (Exception ex)
        {
            Log.E($"Failed prevent special stabby sword override.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}