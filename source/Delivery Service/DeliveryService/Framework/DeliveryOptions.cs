using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryService.Framework
{
    public class DeliveryOptions
    {
        public int[] Send = new int[Enum.GetValues(typeof(DeliveryCategories)).Length];
        public int[] Receive = new int[Enum.GetValues(typeof(DeliveryCategories)).Length];
        public bool MatchColor = false;
        public DeliveryOptions() { }
        public void Set(int[]send, int[] receive, bool match)
        {
            MatchColor = match;
            for(int i = 0; i < Send.Length; i++)
            {
                Send[i] = send[i];
                Receive[i] = receive[i];
            }
        }
        public void Set(DeliveryOptions options)
        {
            this.Set(options.Send, options.Receive, options.MatchColor);
        }
    }
}
