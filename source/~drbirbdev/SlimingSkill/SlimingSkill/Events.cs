/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using System.Collections.Generic;
using BirbCore.Attributes;
using BirbShared;

namespace SlimingSkill;
internal class Events
{

    [SEvent.GameLaunchedLate]
    private void GameLaunched(object sender, StardewModdingAPI.Events.GameLaunchedEventArgs e)
    {
        BirbSkill.Register("drbirbdev.Sliming", ModEntry.Assets.SkillTexture, ModEntry.Instance.Helper, new Dictionary<string, object>()
        {
            {"Rancher", null },
            {"Hunter", null },
            {"Breeder", null },
            {"Hatcher", null },
            {"Poacher", null },
            {"Tamer", null }
        }, PerkText, HoverText);
    }

    private static List<string> PerkText(int level)
    {
        List<string> result = new()
        {
            ModEntry.Instance.I18N.Get("skill.perk", new { bonus = 0 })
        };

        return result;
    }

    private static string HoverText(int level)
    {
        return ModEntry.Instance.I18N.Get("skill.perk", new { bonus = level * 0 });
    }

    [SEvent.SaveLoaded]
    private void GameLoop_SaveLoaded(object sender, StardewModdingAPI.Events.SaveLoadedEventArgs e)
    {

    }
}
