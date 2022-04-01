/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Patches.Integrations.LoveOfCooking;

#region using directives

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Utilities;

using Stardew.Common.Extensions;
using Stardew.Common.Harmony;
using Ultimate;

#endregion using directives

[UsedImplicitly]
internal class ModEntryEvent_DrawRegenBarPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal ModEntryEvent_DrawRegenBarPatch()
    {
        try
        {
            Original = "LoveOfCooking.ModEntry".ToType()
                .MethodNamed("Event_DrawRegenBar");
        }
        catch
        {
            // ignored
        }
    }

    #region harmony patches

    /// <summary>Patch to displace food bar.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> PropagatorPopExtraHeldMushroomsTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator ilGenerator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// Inject: if (ModEntry.PlayerState.RegisteredUltimate?.Meter.IsVisible) topOfBar.X -= 56f;
        /// Before: e.SpriteBatch.Draw( ... )

        var resumeExecution = ilGenerator.DefineLabel();
        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Nop),
                    new CodeInstruction(OpCodes.Ldloca_S, $"{typeof(Vector2)} (7)")
                )
                .Advance()
                .GetInstructions(out var got, 3)
                .FindFirst(
                    new CodeInstruction(OpCodes.Nop),
                    new CodeInstruction(OpCodes.Ldarg_2)
                )
                .Advance()
                .StripLabels(out var labels)
                .AddLabels(resumeExecution)
                .InsertWithLabels(
                    labels,
                    // check if RegisteredUltimate is null
                    new CodeInstruction(OpCodes.Call,
                        typeof(ModEntry).PropertyGetter(nameof(ModEntry.PlayerState))),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(PlayerState).PropertyGetter(nameof(PlayerState.RegisteredUltimate))),
                    new CodeInstruction(OpCodes.Brfalse_S, resumeExecution),
                    // check if RegisteredUltimate.Meter.IsVisible
                    new CodeInstruction(OpCodes.Call,
                        typeof(ModEntry).PropertyGetter(nameof(ModEntry.PlayerState))),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(PlayerState).PropertyGetter(nameof(PlayerState.RegisteredUltimate))),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(Ultimate).PropertyGetter(nameof(Ultimate.Meter))),
                    new CodeInstruction(OpCodes.Call,
                        typeof(UltimateMeter).PropertyGetter(nameof(UltimateMeter.IsVisible))),
                    new CodeInstruction(OpCodes.Brfalse_S, resumeExecution)
                )
                .Insert(got) // loads topOfBar.X
                .Insert(
                    new CodeInstruction(OpCodes.Ldc_R4, 56f),
                    new CodeInstruction(OpCodes.Sub),
                    new CodeInstruction(OpCodes.Stfld, typeof(Vector2).Field(nameof(Vector2.X)))
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while moving Love Of Cooking's food regen bar.\nHelper returned {ex}");
            transpilationFailed = true;
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}