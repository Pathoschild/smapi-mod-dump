/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/QuestFramework
**
*************************************************/

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
