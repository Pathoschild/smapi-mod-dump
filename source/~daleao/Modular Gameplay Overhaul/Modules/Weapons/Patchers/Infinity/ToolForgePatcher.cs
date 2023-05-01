/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Weapons.Patchers.Infinity;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Weapons.Enchantments;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class ToolForgePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ToolForgePatcher"/> class.</summary>
    internal ToolForgePatcher()
    {
        this.Target = this.RequireMethod<Tool>(nameof(Tool.Forge));
    }

    #region harmony patches

    /// <summary>Require Hero Soul to transform Galaxy into Infinity.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? ToolForgeTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // From: if (enchantment is GalaxySoulEnchantment)
        // To: if (enchantment is (Config.InfinityPlusOne ? InfinityEnchantment : GalaxySoulEnchantment))
        try
        {
            var checkForGalaxy = generator.DefineLabel();
            var resumeExecution = generator.DefineLabel();
            helper
                .Match(new[] { new CodeInstruction(OpCodes.Isinst, typeof(GalaxySoulEnchantment)) })
                .AddLabels(checkForGalaxy)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Call, typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.Config))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.Weapons))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(Config).RequirePropertyGetter(nameof(Config.InfinityPlusOne))),
                        new CodeInstruction(OpCodes.Brfalse_S, checkForGalaxy),
                        new CodeInstruction(OpCodes.Isinst, typeof(InfinityEnchantment)),
                        new CodeInstruction(OpCodes.Br_S, resumeExecution),
                    })
                .Move()
                .AddLabels(resumeExecution)
                .Match(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(Tool)
                                .RequireMethod(nameof(Tool.GetEnchantmentOfType))
                                .MakeGenericMethod(typeof(GalaxySoulEnchantment))),
                    })
                .StripLabels(out var labels)
                .Match(new[] { new CodeInstruction(OpCodes.Brfalse_S) })
                .GetOperand(out var toRemove)
                .Return()
                .Count(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(Tool).RequireMethod(nameof(Tool.RemoveEnchantment))),
                    },
                    out var count)
                .Remove(count)
                .RemoveLabels((Label)toRemove)
                .AddLabels(labels);
        }
        catch (Exception ex)
        {
            Log.E($"Failed injecting Hero Soul condition for Infinity Blade.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
