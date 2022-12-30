/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Professions.Patchers.Prestige;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
internal sealed class SkillsPageCtorPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="SkillsPageCtorPatcher"/> class.</summary>
    internal SkillsPageCtorPatcher()
    {
        this.Target = this.RequireConstructor<SkillsPage>(typeof(int), typeof(int), typeof(int), typeof(int));
    }

    #region harmony patches

    /// <summary>
    ///     Patch to increase the width of the skills page in the game menu to fit prestige ribbons + color yellow skill
    ///     bars to green for level >10.
    /// </summary>
    [HarmonyPostfix]
    private static void SkillsPageCtorPostfix(SkillsPage __instance)
    {
        if (!ProfessionsModule.Config.EnablePrestige)
        {
            return;
        }

        __instance.width += 48;
        if (ProfessionsModule.Config.PrestigeProgressionStyle == Config.ProgressionStyle.StackedStars)
        {
            __instance.width += 24;
        }

        var srcRect = new Rectangle(16, 0, 14, 9);
        foreach (var component in __instance.skillBars)
        {
            int skillIndex;
            switch (component.myID / 100)
            {
                case 1:
                    skillIndex = component.myID % 100;

                    // need to do this bullshit switch because mining and fishing are inverted in the skills page
                    skillIndex = skillIndex switch
                    {
                        1 => 3,
                        3 => 1,
                        _ => skillIndex,
                    };

                    if (Game1.player.GetUnmodifiedSkillLevel(skillIndex) >= 15)
                    {
                        component.texture = Textures.SkillBarsTx;
                        component.sourceRect = srcRect;
                    }

                    break;

                case 2:
                    skillIndex = component.myID % 200;

                    // need to do this bullshit switch because mining and fishing are inverted in the skills page
                    skillIndex = skillIndex switch
                    {
                        1 => 3,
                        3 => 1,
                        _ => skillIndex,
                    };

                    if (Game1.player.GetUnmodifiedSkillLevel(skillIndex) >= 20)
                    {
                        component.texture = Textures.SkillBarsTx;
                        component.sourceRect = srcRect;
                    }

                    break;
            }
        }
    }

    #endregion harmony patches
}
