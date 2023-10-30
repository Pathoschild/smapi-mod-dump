/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
**
*************************************************/

namespace DaLion.Overhaul.Modules.Professions.Patchers.Integration.LoveOfCooking;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Professions.Ultimates;
using DaLion.Overhaul.Modules.Professions.VirtualProperties;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;

#endregion using directives

[UsedImplicitly]
[ModRequirement("blueberry.LoveOfCooking", "Love Of Cooking")]
[ImplicitIgnore]
internal sealed class ModEntryEventDrawRegenBarPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ModEntryEventDrawRegenBarPatcher"/> class.</summary>
    internal ModEntryEventDrawRegenBarPatcher()
    {
        this.Target = "LoveOfCooking.ModEntry"
            .ToType()
            .RequireMethod("Event_DrawRegenBar");
    }

    #region harmony patches

    /// <summary>Patch to displace food bar.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? ModEntryEvent_DrawRegenBarTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator ilGenerator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // Inject: if (Game1.player.get_Ultimate()?.Hud.IsVisible) topOfBar.X -= 56f;
        // Before: e.SpriteBatch.Draw( ... )
        try
        {
            var resumeExecution = ilGenerator.DefineLabel();
            helper
                .Match(new[] { new CodeInstruction(OpCodes.Ldarg_2) }) // arg 2 = RenderingHudEventArgs e
                .StripLabels(out var labels)
                .AddLabels(resumeExecution)
                .Insert(
                    new[]
                    {
                        // check if Ultimate is null
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(Game1).RequirePropertyGetter(nameof(Game1.player))),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(Farmer_Ultimate).RequireMethod(nameof(Farmer_Ultimate.Get_Ultimate))),
                        new CodeInstruction(OpCodes.Brfalse_S, resumeExecution),
                        // check if Ultimate.Hud.IsVisible
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(Game1).RequirePropertyGetter(nameof(Game1.player))),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(Farmer_Ultimate).RequireMethod(nameof(Farmer_Ultimate.Get_Ultimate))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(Ultimate).RequirePropertyGetter(nameof(Ultimate.Hud))),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(UltimateHud).RequirePropertyGetter(nameof(UltimateHud.IsVisible))),
                        new CodeInstruction(OpCodes.Brfalse_S, resumeExecution),
                        // load and displace topOfBar.X
                        new CodeInstruction(OpCodes.Ldloca_S, helper.Locals[7]),
                        new CodeInstruction(OpCodes.Ldloc_S, helper.Locals[7]),
                        new CodeInstruction(OpCodes.Ldfld, typeof(Vector2).RequireField(nameof(Vector2.X))),
                        new CodeInstruction(OpCodes.Ldc_R4, 56f), // displace by 56 pixels
                        new CodeInstruction(OpCodes.Sub),
                        new CodeInstruction(OpCodes.Stfld, typeof(Vector2).RequireField(nameof(Vector2.X))),
                    },
                    labels);
        }
        catch (Exception ex)
        {
            Log.E($"Failed moving Love Of Cooking's food regen bar.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
