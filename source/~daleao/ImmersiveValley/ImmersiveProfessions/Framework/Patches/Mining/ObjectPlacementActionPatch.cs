/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Patches.Mining;

#region using directives

using DaLion.Common;
using DaLion.Common.Extensions.Reflection;
using DaLion.Common.Harmony;
using Extensions;
using HarmonyLib;
using StardewModdingAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

#endregion using directives

[UsedImplicitly]
internal sealed class ObjectPlacementActionPatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal ObjectPlacementActionPatch()
    {
        Target = RequireMethod<SObject>(nameof(SObject.placementAction));
    }

    #region harmony patches

    /// <summary>Patch to prevent quantum bombs when detonating manually.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? ObjectPlacementActionTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// Injected: if (who is not null && who.professions.Contains(<demolitionist_id>) && ModEntry.Config.ModKey.IsDown()) skipIntensity ...
        /// After: new TemporaryAnimatedSprite( ... )

        var i = 0;
    repeat:
        try
        {
            var skipIntensity = generator.DefineLabel();
            var resumeExecution = generator.DefineLabel();
            helper
                .FindNext(
                    new CodeInstruction(OpCodes.Dup),
                    new CodeInstruction(OpCodes.Ldc_R4, 0.5f),
                    new CodeInstruction(OpCodes.Stfld,
                        typeof(TemporaryAnimatedSprite).RequireField(nameof(TemporaryAnimatedSprite.shakeIntensity)))
                )
                .AddLabels(resumeExecution)
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_S, (byte)4), // arg 4 = Farmer who
                    new CodeInstruction(OpCodes.Brfalse_S, resumeExecution),
                    new CodeInstruction(OpCodes.Ldarg_S, (byte)4)
                )
                .InsertProfessionCheck(Profession.Demolitionist.Value, forLocalPlayer: false)
                .Insert(
                    new CodeInstruction(OpCodes.Brfalse_S, resumeExecution),
                    new CodeInstruction(OpCodes.Call,
                        typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.Config))),
                    new CodeInstruction(OpCodes.Call,
                        typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.ModKey))),
                    new CodeInstruction(OpCodes.Call,
                        typeof(KeybindList).RequireMethod(nameof(KeybindList.IsDown))),
                    new CodeInstruction(OpCodes.Brtrue_S, skipIntensity)
                )
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Dup),
                    new CodeInstruction(OpCodes.Ldloc_1),
                    new CodeInstruction(OpCodes.Stfld,
                        typeof(TemporaryAnimatedSprite).RequireField(nameof(TemporaryAnimatedSprite
                            .extraInfoForEndBehavior)))
                )
                .AddLabels(skipIntensity);
        }
        catch (Exception ex)
        {
            Log.E($"Failed injecting intensity skip for manually-detonated bombs.\nHelper returned {ex}");
            return null;
        }

        // repeat injection three times
        if (++i < 3) goto repeat;

        return helper.Flush();
    }

    #endregion harmony patches
}