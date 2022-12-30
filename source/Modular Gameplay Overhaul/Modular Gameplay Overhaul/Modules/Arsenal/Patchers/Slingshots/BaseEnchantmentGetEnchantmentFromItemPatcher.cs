/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Arsenal.Patchers.Slingshots;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class BaseEnchantmentGetEnchantmentFromItemPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="BaseEnchantmentGetEnchantmentFromItemPatcher"/> class.</summary>
    internal BaseEnchantmentGetEnchantmentFromItemPatcher()
    {
        this.Target = this.RequireMethod<BaseEnchantment>(nameof(BaseEnchantment.GetEnchantmentFromItem));
    }

    #region harmony patches

    /// <summary>Allow Slingshot forges.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? BaseEnchantmentGetEnchantmentFromItemTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // From: if (base_item == null || (base_item is MeleeWeapon && !(base_item as MeleeWeapon).isScythe()))
        // To: if (base_item == null || (base_item is MeleeWeapon && !(base_item as MeleeWeapon).isScythe()) || base_item is Slingshot)
        try
        {
            var isNotMeleeWeaponButMaybeSlingshot = generator.DefineLabel();
            var canForge = generator.DefineLabel();
            helper
                .Match(new[] { new CodeInstruction(OpCodes.Brfalse_S) })
                .Match(new[] { new CodeInstruction(OpCodes.Brfalse) })
                .GetOperand(out var cannotForge)
                .SetOperand(isNotMeleeWeaponButMaybeSlingshot)
                .Match(new[] { new CodeInstruction(OpCodes.Brtrue) })
                .Move()
                .AddLabels(canForge)
                .Insert(new[] { new CodeInstruction(OpCodes.Br_S, canForge) })
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Isinst, typeof(Slingshot)),
                        new CodeInstruction(OpCodes.Brfalse, cannotForge),
                    },
                    new[] { isNotMeleeWeaponButMaybeSlingshot });
        }
        catch (Exception ex)
        {
            Log.E($"Failed allowing slingshot forges.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
