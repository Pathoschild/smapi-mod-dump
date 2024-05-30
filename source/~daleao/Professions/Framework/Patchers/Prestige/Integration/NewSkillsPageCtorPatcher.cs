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

using DaLion.Shared.Attributes;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using SpaceCore;
using SpaceCore.Interface;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
[ModRequirement("spacechase0.SpaceCore")]
internal sealed class NewSkillsPageCtorPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="NewSkillsPageCtorPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal NewSkillsPageCtorPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireConstructor<NewSkillsPage>(typeof(int), typeof(int), typeof(int), typeof(int));
    }

    #region harmony patches

    /// <summary>
    ///     Patch to increase the width of the skills page in the game menu to fit prestige ribbons + color yellow skill
    ///     bars to green for level >10.
    /// </summary>
    [HarmonyPostfix]
    private static void NewSkillsPageCtorPostfix(
        NewSkillsPage __instance,
        ClickableTextureComponent ___upButton,
        ClickableTextureComponent ___downButton,
        ClickableTextureComponent ___scrollBar,
        ref Rectangle ___scrollBarRunner)
    {
        if (ShouldEnableSkillReset)
        {
            ISkill? maxSkill = null;
            var maxLength = 0;
            foreach (var skill in Skill.List)
            {
                if (maxSkill is null)
                {
                    maxSkill = skill;
                    continue;
                }

                if (Game1.player.GetProfessionsForSkill(skill, true).Length is var length &&
                    (length > maxLength || (length == maxLength && skill.CurrentLevel > maxSkill.CurrentLevel)))
                {
                    maxSkill = skill;
                    maxLength = length;
                }
            }

            foreach (var skill in CustomSkill.Loaded.Values)
            {
                if (Game1.player.GetProfessionsForSkill(skill, true).Length is var length &&
                    (length > maxLength || (length == maxLength && skill.CurrentLevel > maxSkill!.CurrentLevel)))
                {
                    maxSkill = skill;
                    maxLength = length;
                }
            }

            if (maxLength > 0)
            {
                var addedWidth = ((maxLength + (maxSkill!.CurrentLevel >= 10 ? 2 : 1)) * 4 * (int)Textures.STARS_SCALE) + 12;
                __instance.width += addedWidth;
                ___upButton.bounds.X += addedWidth;
                ___downButton.bounds.X += addedWidth;
                ___scrollBar.bounds.X += addedWidth;
                ___scrollBarRunner.X += addedWidth;
                NewSkillsPageDrawPatcher.RibbonXOffset = 48 - (maxLength * 12);
            }
        }

        if (!ShouldEnablePrestigeLevels)
        {
            return;
        }

        var sourceRect = new Rectangle(16, 0, 14, 9);
        var skills = SCSkills.GetSkillList();
        foreach (var component in __instance.skillBars)
        {
            int skillIndex, skillLevel;
            switch (component.myID / 100)
            {
                case 1:
                    skillIndex = component.myID % 100;

                    // need to do this bullshit switch because mining and fishing are inverted in the skills page
                    skillIndex = skillIndex switch
                    {
                        1 => Farmer.miningSkill,
                        3 => Farmer.fishingSkill,
                        _ => skillIndex,
                    };

                    skillLevel = skillIndex switch
                    {
                        < Farmer.luckSkill => Game1.player.GetUnmodifiedSkillLevel(skillIndex),
                        > Farmer.luckSkill => SCSkills.GetSkillLevel(Game1.player, skills[skillIndex - Farmer.luckSkill - 1]), // -5 for vanilla skills and -1 for zero-index
                        _ => 0,
                    };

                    if (skillLevel >= 15)
                    {
                        component.texture = Textures.SkillBars;
                        component.sourceRect = sourceRect;
                    }

                    break;

                case 2:
                    skillIndex = component.myID % 200;

                    // need to do this bullshit switch because mining and fishing are inverted in the skills page
                    skillIndex = skillIndex switch
                    {
                        1 => Farmer.miningSkill,
                        3 => Farmer.fishingSkill,
                        _ => skillIndex,
                    };

                    skillLevel = skillIndex switch
                    {
                        < 5 => Game1.player.GetUnmodifiedSkillLevel(skillIndex),
                        > Farmer.luckSkill => SCSkills.GetSkillLevel(Game1.player, skills[skillIndex - Farmer.luckSkill - 1]), // -5 for vanilla skills and -1 for zero-index
                        _ => 0,
                    };

                    if (skillLevel >= 20)
                    {
                        component.texture = Textures.SkillBars;
                        component.sourceRect = sourceRect;
                    }

                    break;
            }
        }
    }

    #endregion harmony patches
}
