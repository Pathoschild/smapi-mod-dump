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
using SpaceCore;
using StardewModdingAPI.Events;
using StardewValley;

namespace SocializingSkill;

[SEvent]
internal class Events
{
    [SEvent.GameLaunchedLate]
    private void GameLaunched(object sender, GameLaunchedEventArgs e)
    {
        BirbSkill.Register("drbirbdev.Socializing", ModEntry.Assets.SkillTexture, ModEntry.Instance.Helper, new Dictionary<string, object>()
        {
            {"Friendly", null},
            {"Helpful", null },
            {"SmoothTalker", null },
            {"Gifter", null },
            {"Haggler", null },
            {"Beloved", null }
        }, PerkText, HoverText);

        SpaceCore.Events.SpaceEvents.AfterGiftGiven += this.SpaceEvents_AfterGiftGiven;
    }

    private static List<string> PerkText(int level)
    {
        List<string> result = new()
        {
            ModEntry.Instance.I18n.Get("skill.perk", new { bonus = ModEntry.Config.ChanceNoFriendshipDecayPerLevel })
        };

        return result;
    }

    private static string HoverText(int level)
    {
        return ModEntry.Instance.I18n.Get("skill.perk", new { bonus = level * ModEntry.Config.ChanceNoFriendshipDecayPerLevel });
    }

    // Beloved Profession
    //  - reset which villagers have been checked for bonus gifts today for each player.
    [SEvent.DayStarted]
    private void DayStarted(object sender, DayStartedEventArgs e)
    {
        ModEntry.BelovedCheckedToday.Value = new List<string>();
    }

    // Grant XP
    // Gifter Profession
    //  - Give extra friendship
    private void SpaceEvents_AfterGiftGiven(object sender, SpaceCore.Events.EventArgsGiftGiven e)
    {
        int taste = e.Npc.getGiftTasteForThisItem(e.Gift);
        if (Game1.player.HasProfession("Gifter"))
        {
            int extraFriendship = 0;
            if (Game1.player.HasProfession("Gifter", true))
            {
                extraFriendship += 20;
            }
            switch (taste)
            {
                case 0:
                    extraFriendship += ModEntry.Config.GifterLovedGiftExtraFriendship;
                    break;
                case 2:
                    extraFriendship += ModEntry.Config.GifterLikedGiftExtraFriendship;
                    break;
                case 8:
                    extraFriendship += ModEntry.Config.GifterNeutralGiftExtraFriendship;
                    break;
            }
            Game1.player.changeFriendship(extraFriendship, e.Npc);
        }

        if (taste <= 2)
        {
            float exp = ModEntry.Config.ExperienceFromGifts;
            if (taste == 0)
            {
                exp *= ModEntry.Config.LovedGiftExpMultiplier;
            }
            if (e.Npc.isBirthday())
            {
                exp *= ModEntry.Config.BirthdayGiftExpMultiplier;
            }
            Skills.AddExperience(Game1.player, "drbirbdev.Socializing", (int)exp);
        }
    }
}
