/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Arsenal.Patchers.Common;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Arsenal.Configs;
using DaLion.Overhaul.Modules.Arsenal.Enchantments;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class BaseEnchantmentGetAvailableEnchantmentsPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="BaseEnchantmentGetAvailableEnchantmentsPatcher"/> class.</summary>
    internal BaseEnchantmentGetAvailableEnchantmentsPatcher()
    {
        this.Target = this.RequireMethod<BaseEnchantment>(nameof(BaseEnchantment.GetAvailableEnchantments));
    }

    #region harmony patches

    /// <summary>Out with the old and in with the new enchants.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? BaseEnchantmentGetAvailableEnchantmentsTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            var newWeaponEnchants = generator.DefineLabel();
            var resumeExecution = generator.DefineLabel();
            helper
                .GoTo(4)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Call, typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.Config))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.Arsenal))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(Config).RequirePropertyGetter(nameof(Config.Weapons))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(WeaponConfig).RequirePropertyGetter(nameof(WeaponConfig.EnableEnchants))),
                        new CodeInstruction(OpCodes.Brtrue_S, newWeaponEnchants),
                    })
                .Move(12)
                .Insert(new[] { new CodeInstruction(OpCodes.Br_S, resumeExecution) })
                .Insert(
                    new[]
                    {
                        // add redux artful enchant
                        new CodeInstruction(OpCodes.Ldsfld, typeof(BaseEnchantment).RequireField("_enchantments")),
                        new CodeInstruction(OpCodes.Newobj, typeof(ReduxArtfulEnchantment).RequireConstructor()),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(List<BaseEnchantment>).RequireMethod(nameof(List<BaseEnchantment>.Add))),
                        // add carving enchant
                        new CodeInstruction(OpCodes.Ldsfld, typeof(BaseEnchantment).RequireField("_enchantments")),
                        new CodeInstruction(OpCodes.Newobj, typeof(CarvingEnchantment).RequireConstructor()),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(List<BaseEnchantment>).RequireMethod(nameof(List<BaseEnchantment>.Add))),
                        // add cleaving enchant
                        new CodeInstruction(OpCodes.Ldsfld, typeof(BaseEnchantment).RequireField("_enchantments")),
                        new CodeInstruction(OpCodes.Newobj, typeof(CleavingEnchantment).RequireConstructor()),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(List<BaseEnchantment>).RequireMethod(nameof(List<BaseEnchantment>.Add))),
                        // add energized enchant
                        new CodeInstruction(OpCodes.Ldsfld, typeof(BaseEnchantment).RequireField("_enchantments")),
                        new CodeInstruction(OpCodes.Newobj, typeof(EnergizedEnchantment).RequireConstructor()),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(List<BaseEnchantment>).RequireMethod(nameof(List<BaseEnchantment>.Add))),
                        // add tribute enchant
                        new CodeInstruction(OpCodes.Ldsfld, typeof(BaseEnchantment).RequireField("_enchantments")),
                        new CodeInstruction(OpCodes.Newobj, typeof(TributeEnchantment).RequireConstructor()),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(List<BaseEnchantment>).RequireMethod(nameof(List<BaseEnchantment>.Add))),
                        // add bloodthirsty enchant
                        new CodeInstruction(OpCodes.Ldsfld, typeof(BaseEnchantment).RequireField("_enchantments")),
                        new CodeInstruction(OpCodes.Newobj, typeof(BloodthirstyEnchantment).RequireConstructor()),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(List<BaseEnchantment>).RequireMethod(nameof(List<BaseEnchantment>.Add))),
                        // add engorging enchant
                        new(OpCodes.Ldsfld, typeof(BaseEnchantment).RequireField("_enchantments")),
                        new(OpCodes.Newobj, typeof(EngorgingEnchantment).RequireConstructor()), new(
                            OpCodes.Callvirt,
                            typeof(List<BaseEnchantment>).RequireMethod(nameof(List<BaseEnchantment>.Add))),
                        // add gatling enchant
                        new(OpCodes.Ldsfld, typeof(BaseEnchantment).RequireField("_enchantments")),
                        new(OpCodes.Newobj, typeof(GatlingEnchantment).RequireConstructor()), new(
                            OpCodes.Callvirt,
                            typeof(List<BaseEnchantment>).RequireMethod(nameof(List<BaseEnchantment>.Add))),
                        // add preserving enchant
                        new(OpCodes.Ldsfld, typeof(BaseEnchantment).RequireField("_enchantments")),
                        new(OpCodes.Newobj, typeof(PreservingEnchantment).RequireConstructor()), new(
                            OpCodes.Callvirt,
                            typeof(List<BaseEnchantment>).RequireMethod(nameof(List<BaseEnchantment>.Add))),
                        // add quincy enchant
                        new(OpCodes.Ldsfld, typeof(BaseEnchantment).RequireField("_enchantments")),
                        new(OpCodes.Newobj, typeof(QuincyEnchantment).RequireConstructor()), new(
                            OpCodes.Callvirt,
                            typeof(List<BaseEnchantment>).RequireMethod(nameof(List<BaseEnchantment>.Add))),
                        // add spreading enchant
                        new(OpCodes.Ldsfld, typeof(BaseEnchantment).RequireField("_enchantments")),
                        new(OpCodes.Newobj, typeof(SpreadingEnchantment).RequireConstructor()), new(
                            OpCodes.Callvirt,
                            typeof(List<BaseEnchantment>).RequireMethod(nameof(List<BaseEnchantment>.Add))),
                    },
                    new[] { newWeaponEnchants })
                .AddLabels(resumeExecution);
        }
        catch (Exception ex)
        {
            Log.E($"Failed injecting new weapon enchants.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
