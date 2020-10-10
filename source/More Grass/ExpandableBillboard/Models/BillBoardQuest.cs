/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AndyCrocker/StardewMods
**
*************************************************/

using System.Collections.Generic;
using static ExpandableBillboard.Enums;

namespace ExpandableBillboard.Models
{
    public class BillBoardQuest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Objective { get; set; }
        public int MoneyReward { get; set; }
        public int FriendshipReward { get; set; }
        public int DeliveryItem { get; set; }
        public string Requester { get; set; }
        public QuestType Type { get; set; }
        public int DaysToComplete { get; set; }
    }
}
