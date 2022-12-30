/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Tools.Patchers;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class SkillsPageDrawPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="SkillsPageDrawPatcher"/> class.</summary>
    internal SkillsPageDrawPatcher()
    {
        this.Target = this.RequireMethod<SkillsPage>(nameof(SkillsPage.draw), new[] { typeof(SpriteBatch) });
    }

    /// <summary>Allows new Master Enchantments to draw as green levels in the skills page.</summary>
    private static IEnumerable<CodeInstruction>? SkillsPageDrawTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);
        var currentTool = generator.DeclareLocal(typeof(Tool));

        try
        {
            var checkForMasterEnchantment = generator.DefineLabel();
            var setFalse = generator.DefineLabel();
            var setTrue = generator.DefineLabel();
            var resumeExecution = generator.DefineLabel();
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Cgt),
                        new CodeInstruction(OpCodes.Stloc_S, helper.Locals[6]),
                    })
                .ReplaceWith(new CodeInstruction(OpCodes.Bgt_S, setTrue))
                .Move()
                .AddLabels(resumeExecution)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Call, typeof(Game1).RequirePropertyGetter(nameof(Game1.player))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(Farmer).RequirePropertyGetter(nameof(Farmer.CurrentTool))),
                        new CodeInstruction(OpCodes.Stloc_S, currentTool),
                        new CodeInstruction(OpCodes.Ldloc_S, currentTool),
                        new CodeInstruction(OpCodes.Brfalse_S, setFalse),
                        new CodeInstruction(OpCodes.Ldloc_S, currentTool),
                        new CodeInstruction(OpCodes.Isinst, typeof(Hoe)),
                        new CodeInstruction(OpCodes.Brtrue_S, checkForMasterEnchantment),
                        new CodeInstruction(OpCodes.Ldloc_S, currentTool),
                        new CodeInstruction(OpCodes.Isinst, typeof(WateringCan)),
                        new CodeInstruction(OpCodes.Brfalse_S, setFalse),
                    })
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldloc_S, currentTool),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(Tool)
                                .RequireMethod(nameof(Tool.hasEnchantmentOfType))
                                .MakeGenericMethod(typeof(MasterEnchantment))),
                        new CodeInstruction(OpCodes.Brfalse_S, setFalse),
                    },
                    new[] { checkForMasterEnchantment })
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldc_I4_1),
                        new CodeInstruction(OpCodes.Br_S, resumeExecution),
                    },
                    new[] { setTrue })
                .Insert(
                    new[] { new CodeInstruction(OpCodes.Ldc_I4_0) },
                    new[] { setFalse });
        }
        catch (Exception ex)
        {
            Log.E($"Failed drawing bonus Farming Level for new Master enchantments.\nHelper returned {ex}");
            return null;
        }

        try
        {
            var setFalse = generator.DefineLabel();
            var setTrue = generator.DefineLabel();
            var resumeExecution = generator.DefineLabel();
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Cgt),
                        new CodeInstruction(OpCodes.Stloc_S, helper.Locals[6]),
                    })
                .ReplaceWith(new CodeInstruction(OpCodes.Bgt_S, setTrue))
                .Move()
                .AddLabels(resumeExecution)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Call, typeof(Game1).RequirePropertyGetter(nameof(Game1.player))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(Farmer).RequirePropertyGetter(nameof(Farmer.CurrentTool))),
                        new CodeInstruction(OpCodes.Stloc_S, currentTool),
                        new CodeInstruction(OpCodes.Ldloc_S, currentTool),
                        new CodeInstruction(OpCodes.Brfalse_S, setFalse),
                        new CodeInstruction(OpCodes.Ldloc_S, currentTool),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(Tool)
                                .RequireMethod(nameof(Tool.hasEnchantmentOfType))
                                .MakeGenericMethod(typeof(MasterEnchantment))),
                        new CodeInstruction(OpCodes.Brfalse_S, setFalse),
                    })
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldc_I4_1),
                        new CodeInstruction(OpCodes.Br_S, resumeExecution),
                    },
                    new[] { setTrue })
                .Insert(
                    new[] { new CodeInstruction(OpCodes.Ldc_I4_0) },
                    new[] { setFalse });
        }
        catch (Exception ex)
        {
            Log.E($"Failed drawing bonus Mining Level for new Master enchantment.\nHelper returned {ex}");
            return null;
        }

        try
        {
            var setFalse = generator.DefineLabel();
            var setTrue = generator.DefineLabel();
            var resumeExecution = generator.DefineLabel();
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Cgt),
                        new CodeInstruction(OpCodes.Stloc_S, helper.Locals[6]),
                    })
                .ReplaceWith(new CodeInstruction(OpCodes.Bgt_S, setTrue))
                .Move()
                .AddLabels(resumeExecution)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Call, typeof(Game1).RequirePropertyGetter(nameof(Game1.player))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(Farmer).RequirePropertyGetter(nameof(Farmer.CurrentTool))),
                        new CodeInstruction(OpCodes.Stloc_S, currentTool),
                        new CodeInstruction(OpCodes.Ldloc_S, currentTool),
                        new CodeInstruction(OpCodes.Brfalse_S, setFalse),
                        new CodeInstruction(OpCodes.Ldloc_S, currentTool),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(Tool)
                                .RequireMethod(nameof(Tool.hasEnchantmentOfType))
                                .MakeGenericMethod(typeof(MasterEnchantment))),
                        new CodeInstruction(OpCodes.Brfalse_S, setFalse),
                    })
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldc_I4_1),
                        new CodeInstruction(OpCodes.Br_S, resumeExecution),
                    },
                    new[] { setTrue })
                .Insert(
                    new[] { new CodeInstruction(OpCodes.Ldc_I4_0) },
                    new[] { setFalse });
        }
        catch (Exception ex)
        {
            Log.E($"Failed drawing bonus Foraging Level for new Master enchantment.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }
}
