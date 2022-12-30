/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Arsenal.Patchers.Weapons;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponDrawInMenuPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MeleeWeaponDrawInMenuPatcher"/> class.</summary>
    internal MeleeWeaponDrawInMenuPatcher()
    {
        this.Target = this.RequireMethod<MeleeWeapon>(
            nameof(MeleeWeapon.drawInMenu),
            new[]
            {
                typeof(SpriteBatch), typeof(Vector2), typeof(float), typeof(float), typeof(float),
                typeof(StackDrawType), typeof(Color), typeof(bool),
            });
    }

    #region harmony patches

    /// <summary>Draw Stabbing Sword cooldown.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? MeleeWeaponDrawInMenuTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // Injected: else if (attackSwordCooldown > 0)
        //     { coolDownLevel = (float)defenseCooldown / 1500f; }
        // Before: addedScale = addedSwordScale;
        try
        {
            var tryAttackSwordCooldown = generator.DefineLabel();
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Ldsfld,
                            typeof(MeleeWeapon).RequireField(nameof(MeleeWeapon.defenseCooldown))),
                        new CodeInstruction(OpCodes.Ldc_I4_0),
                        new CodeInstruction(OpCodes.Ble_S),
                    })
                .Move(2)
                .GetOperand(out var resumeExecution)
                .SetOperand(tryAttackSwordCooldown)
                .Match(
                    new[] { new CodeInstruction(OpCodes.Ldsfld, typeof(MeleeWeapon).RequireField("addedSwordScale")) })
                .Insert(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Ldsfld,
                            typeof(MeleeWeapon).RequireField(nameof(MeleeWeapon.attackSwordCooldown))),
                        new CodeInstruction(OpCodes.Ldc_I4_0),
                        new CodeInstruction(OpCodes.Ble_S, resumeExecution),
                        new CodeInstruction(
                            OpCodes.Ldsfld,
                            typeof(MeleeWeapon).RequireField(nameof(MeleeWeapon.attackSwordCooldown))),
                        new CodeInstruction(OpCodes.Conv_R4),
                        new CodeInstruction(OpCodes.Ldc_R4, 2000f),
                        new CodeInstruction(OpCodes.Div),
                        new CodeInstruction(OpCodes.Stloc_0),
                    },
                    new[] { tryAttackSwordCooldown });
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding attack sword cooldown in menu.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
