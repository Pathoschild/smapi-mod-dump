/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
**
*************************************************/

namespace DaLion.Overhaul.Modules.Combat.Patchers.Quests.Woody;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Combat.Configs;
using DaLion.Shared.Constants;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class EventCommandItemAboveHeadPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="EventCommandItemAboveHeadPatcher"/> class.</summary>
    internal EventCommandItemAboveHeadPatcher()
    {
        this.Target = this.RequireMethod<Event>(nameof(Event.command_itemAboveHead));
    }

    #region harmony patches

    /// <summary>Replaces rusty sword with wooden blade in Marlon's intro event.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? EventCommandItemAboveHeadTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            var rusty = generator.DefineLabel();
            var resumeExecution = generator.DefineLabel();
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldc_I4_0),
                        new CodeInstruction(OpCodes.Newobj, typeof(MeleeWeapon).RequireConstructor(typeof(int))),
                    })
                .AddLabels(rusty)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.Config))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.Combat))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(CombatConfig).RequirePropertyGetter(nameof(CombatConfig.Quests))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(QuestsConfig).RequirePropertyGetter(nameof(QuestsConfig.WoodyReplacesRusty))),
                        new CodeInstruction(OpCodes.Brfalse_S, rusty),
                        new CodeInstruction(OpCodes.Ldc_I4_S, WeaponIds.WoodenBlade),
                        new CodeInstruction(OpCodes.Br_S, resumeExecution),
                    })
                .Move()
                .AddLabels(resumeExecution);
        }
        catch (Exception ex)
        {
            Log.E($"Failed replacing rusty sword above head with wooden blade.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
