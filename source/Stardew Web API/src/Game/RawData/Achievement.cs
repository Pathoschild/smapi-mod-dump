/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/zunderscore/StardewWebApi
**
*************************************************/

using StardewValley;
using StardewWebApi.Game.Items;

namespace StardewWebApi.Game.Players;

public class Achievement
{
    public Achievement(int achievementId)
    {
        RawAchievementData = Game1.achievements[achievementId];

        var parts = RawAchievementData.Split('^');

        Id = achievementId;
        Name = parts[0];
        Description = parts[1];
        ShowIfPrerequisiteMet = Boolean.Parse(parts[2]);

        var prerequesiteAchievement = Int32.Parse(parts[3]);
        HasPrerequisiteAchievement = prerequesiteAchievement > -1;
        if (HasPrerequisiteAchievement)
        {
            PrerequisiteAchievement = prerequesiteAchievement;
        }

        var hatRewardId = parts[4];
        Reward = Id switch
        {
            35 => BasicItem.FromItem(ItemUtilities.GetItemByTypeAndId("H", "PageboyCap")),
            36 => BasicItem.FromItem(ItemUtilities.GetItemByTypeAndId("H", "JesterHat")),
            37 => BasicItem.FromItem(ItemUtilities.GetItemByTypeAndId("H", "BlueRibbon")),
            38 => BasicItem.FromItem(ItemUtilities.GetItemByTypeAndId("H", "GovernorsHat")),
            39 => BasicItem.FromItem(ItemUtilities.GetItemByTypeAndId("H", "WhiteBow")),
            40 => BasicItem.FromItem(ItemUtilities.GetItemByTypeAndId("H", "PaperHat")),
            41 => BasicItem.FromItem(ItemUtilities.GetItemByTypeAndId("H", "SpaceHelmet")),
            42 => BasicItem.FromItem(ItemUtilities.GetItemByTypeAndId("H", "InfinityCrown")),
            44 => BasicItem.FromItem(ItemUtilities.GetItemByTypeAndId("H", "JunimoHat")),
            _ => BasicItem.FromItem(ItemUtilities.GetItemByTypeAndId("H", hatRewardId)),
        };
    }

    public int Id { get; }
    public string RawAchievementData { get; }
    public string Name { get; }
    public string Description { get; }
    public bool ShowIfPrerequisiteMet { get; }
    public bool HasPrerequisiteAchievement { get; }
    public int? PrerequisiteAchievement { get; }
    public BasicItem? Reward { get; }

    public Achievement? GetPrerequisiteAchievement()
    {
        return HasPrerequisiteAchievement
            ? new Achievement(PrerequisiteAchievement!.Value)
            : null;
    }
}