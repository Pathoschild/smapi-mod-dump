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
using JetBrains.Annotations;
using StardewValley;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

#endregion using directives

[UsedImplicitly]
internal sealed class EventCommandItemAboveHeadPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal EventCommandItemAboveHeadPatch()
    {
        Target = RequireMethod<Event>(nameof(Event.command_itemAboveHead));
    }

    #region harmony patches

    /// <summary>Replaces rusty sword with wooden blade in Marlon's intro event.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? EventCommandItemAboveHeadTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        var rusty = generator.DefineLabel();
        var resumeExecution = generator.DefineLabel();
        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldc_I4_0),
                    new CodeInstruction(OpCodes.Newobj, typeof(MeleeWeapon).RequireConstructor(new[] { typeof(int) }))
                )
                .AddLabels(rusty)
                .Insert(
                    new CodeInstruction(OpCodes.Call,
                        typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.Config))),
                    new CodeInstruction(OpCodes.Call,
                        typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.WoodyReplacesRusty))),
                    new CodeInstruction(OpCodes.Brfalse_S, rusty),
                    new CodeInstruction(OpCodes.Ldc_I4_S, Constants.WOODEN_BLADE_INDEX_I),
                    new CodeInstruction(OpCodes.Br_S, resumeExecution)
                )
                .Advance()
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