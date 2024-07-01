/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Professions.Framework.Patchers.Integration.UiInfoSuite;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
[ModRequirement("Annosz.UiInfoSuite2", version: "2.3.3")]
internal sealed class ExperienceBarDrawExperienceBarPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ExperienceBarDrawExperienceBarPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal ExperienceBarDrawExperienceBarPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = "UIInfoSuite2.UIElements.ExperienceBar"
            .ToType()
            .RequireMethod("DrawExperienceBar");
    }

    #region harmony patches

    /// <summary>Patch to move skill icon to the right.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? ExperienceBarDrawExperienceBarTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // From: Game1.spriteBatch.Draw(Game1.mouseCursors, new Vector2(num + 54f, ...
        // To: Game1.spriteBatch.Draw(Game1.mouseCursors, new Vector2(num + 174f, ...
        try
        {
            helper
                .PatternMatch([new CodeInstruction(OpCodes.Ldc_R4, 54f)])
                .SetOperand(174f);
        }
        catch (Exception ex)
        {
            Log.E("Failed budging Ui Info Suite experience bar skill icon." + $"\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
