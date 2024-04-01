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
using StardewValley.Constants;

namespace SocializingSkill;

[SEvent]
internal class Events
{
    [SEvent.GameLaunchedLate]
    private void GameLaunched(object sender, GameLaunchedEventArgs e)
    {
        BirbSkill.Register("drbirbdev.Socializing", ModEntry.Assets.SkillTexture, ModEntry.Instance.Helper,
            new Dictionary<string, object>
            {
                { "Friendly", null },
                { "Helpful", null },
                { "SmoothTalker", null },
                { "Gifter", null },
                { "Haggler", null },
                { "Beloved", null }
            }, PerkText, HoverText);

        SpaceCore.Events.SpaceEvents.AfterGiftGiven += SpaceEvents_AfterGiftGiven;
    }

    private static List<string> PerkText(int level)
    {
        List<string> result =
            [ModEntry.Instance.I18N.Get("skill.perk", new { bonus = ModEntry.Config.ChanceNoFriendshipDecayPerLevel })];

        return result;
    }

    private static string HoverText(int level)
    {
        return ModEntry.Instance.I18N.Get("skill.perk",
            new { bonus = level * ModEntry.Config.ChanceNoFriendshipDecayPerLevel });
    }

    // Beloved Profession
    //  - reset which villagers have been checked for bonus gifts today for each player.
    [SEvent.DayStarted]
    private void DayStarted(object sender, DayStartedEventArgs e)
    {
        ModEntry.BELOVED_CHECKED_TODAY.Value = [];
    }

    // Grant XP from quest completion
    [SEvent.StatChanged(StatKeys.QuestsCompleted)]
    private void QuestCompleted(object sender, SEvent.StatChanged.EventArgs e)
    {
        Skills.AddExperience(Game1.player, "drbirbdev.Socializing", ModEntry.Config.ExperienceFromQuests * e.Delta);
    }

    // Grant XP from giving gifts
    // Gifter Profession
    //  - Give extra friendship
    private static void SpaceEvents_AfterGiftGiven(object sender, SpaceCore.Events.EventArgsGiftGiven e)
    {
        int taste = e.Npc.getGiftTasteForThisItem(e.Gift);

        if (taste is 4 or 6)
        {
            return;
        }

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

        if (!Game1.player.HasProfession("Gifter"))
        {
            return;
        }

        int extraFriendship = 0;
        if (Game1.player.HasProfession("Gifter", true))
        {
            extraFriendship += ModEntry.Config.GifterPrestigeExtraFriendship;
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
}
