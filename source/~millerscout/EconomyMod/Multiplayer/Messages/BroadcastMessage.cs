using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomyMod.Multiplayer.Messages
{
    class BroadcastMessage
    {
        public BroadcastType Type { get; set; }
        public string DisplayName { get; set; }
        public int Tax { get; set; }
        public bool IsMale { get; set; }
    }
    public enum BroadcastType
    {
        Taxation_Paid,
        Taxation_Postpone
    }
}
