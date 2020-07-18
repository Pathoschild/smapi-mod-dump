using QuestFramework.Quests;
using StardewValley;
using StardewValley.Quests;
using System;

namespace QuestFramework.Framework.Quests
{
    internal class QuestInfo : IQuestInfo
    {
        public QuestInfo(Quest sdvQuest, Farmer player)
        {
            this.VanillaQuest = sdvQuest ?? throw new ArgumentNullException(nameof(sdvQuest));
            this.Farmer = player ?? throw new ArgumentNullException(nameof(player));
        }

        public Quest VanillaQuest { get; }

        public Farmer Farmer { get; }
    }
}
