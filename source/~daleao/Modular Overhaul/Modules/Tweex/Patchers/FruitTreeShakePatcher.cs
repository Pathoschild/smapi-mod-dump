/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Tweex.Patchers;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Tweex.Extensions;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.TerrainFeatures;

#endregion using directives

[UsedImplicitly]
[ModConflict("aedenthorn.FruitTreeTweaks")]
internal sealed class FruitTreeShakePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FruitTreeShakePatcher"/> class.</summary>
    internal FruitTreeShakePatcher()
    {
        this.Target = this.RequireMethod<FruitTree>(nameof(FruitTree.shake));
    }

    #region harmony patches

    /// <summary>Customize Fruit Tree age quality.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? FruitTreeShakeTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // From: int fruitQuality = 0;
        // To: int fruitQuality = this.GetQualityFromAge();
        // Removed all remaining age checks for quality
        try
        {
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldc_I4_0),
                        new CodeInstruction(OpCodes.Stloc_0),
                    })
                .StripLabels(out var labels)
                .ReplaceWith(new CodeInstruction(
                    OpCodes.Call,
                    typeof(FruitTreeExtensions).RequireMethod(nameof(FruitTreeExtensions.GetQualityFromAge))))
                .Insert(
                    new[] { new CodeInstruction(OpCodes.Ldarg_0) },
                    labels)
                .Match(new[] { new CodeInstruction(OpCodes.Ldarg_0) })
                .CountUntil(new[] { new CodeInstruction(OpCodes.Stloc_0) }, out var count)
                .Remove(count)
                .CountUntil(new[] { new CodeInstruction(OpCodes.Stloc_0) }, out count)
                .Remove(count)
                .CountUntil(new[] { new CodeInstruction(OpCodes.Stloc_0) }, out count)
                .Remove(count)
                .StripLabels();
        }
        catch (Exception ex)
        {
            Log.E($"Failed customizing fruit tree age quality factor.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
