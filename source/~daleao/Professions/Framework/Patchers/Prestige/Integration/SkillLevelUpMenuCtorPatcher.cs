/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Professions.Framework.Patchers.Prestige.Integration;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using SpaceCore.Interface;

#endregion using directives

[UsedImplicitly]
[ModRequirement("spacechase0.SpaceCore")]
internal sealed class SkillLevelUpMenuCtorPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="SkillLevelUpMenuCtorPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal SkillLevelUpMenuCtorPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireConstructor<SkillLevelUpMenu>(typeof(string), typeof(int));
    }

    #region harmony patches

    /// <summary>Patch to prevent duplicate profession acquisition + display end of level up dialogues.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? LevelUpMenuCtorTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // This injection chooses the correct 2nd-tier profession choices based on the last selected level 5 profession.
        // From: profPair = null; foreach ( ... )
        // To: profPair = ChooseProfessionPair(skill);
        try
        {
            helper
                .PatternMatch([
                    // find index of initializing profPair to null
                    new CodeInstruction(OpCodes.Ldnull),
                    new CodeInstruction(OpCodes.Stfld, typeof(SkillLevelUpMenu).RequireField("profPair")),
                ])
                .ReplaceWith(
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(SkillLevelUpMenuUpdatePatcher).RequireMethod(nameof(SkillLevelUpMenuUpdatePatcher
                            .ChooseProfessionPair))))
                .Insert([
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(
                        OpCodes.Ldfld,
                        typeof(SkillLevelUpMenu).RequireField("currentSkill")),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(
                        OpCodes.Ldfld,
                        typeof(SkillLevelUpMenu).RequireField("currentLevel")),
                ])
                .Move(2)
                .CountUntil([new CodeInstruction(OpCodes.Endfinally)], out var count)
                .Remove(count); // remove the entire loop
        }
        catch (Exception ex)
        {
            Log.E(
                "Professions mod failed patching 2nd-tier profession choices to reflect last chosen 1st-tier profession." +
                $"\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
