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
