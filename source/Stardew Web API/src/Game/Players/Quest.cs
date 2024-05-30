/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/zunderscore/StardewWebApi
**
*************************************************/

namespace StardewWebApi.Game.Players;

public class Quest
{
    private readonly StardewValley.Quests.Quest _quest;

    public Quest(StardewValley.Quests.Quest quest)
    {
        _quest = quest;
    }

    public string Title => _quest.GetName();
    public string Description => _quest.GetDescription();
    public bool HasReward => _quest.HasReward();
    public string? RewardDescription => _quest.HasReward() ? _quest.rewardDescription.Value : null;
    public bool HasMoneyReward => _quest.HasMoneyReward();
    public int? MoneyReward => _quest.HasMoneyReward() ? _quest.moneyReward.Value : null;
    public string CurrentObjective => _quest.currentObjective;
    public bool IsTimedQuest => _quest.IsTimedQuest();
    public int? DaysLeft => _quest.IsTimedQuest() ? _quest.GetDaysLeft() : null;
}