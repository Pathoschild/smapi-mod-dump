using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bwdyworks.Events
{
    public class ItemEatenEventArgs : EventArgs
    {
        public StardewValley.Item Item { get; set; }
        public StardewValley.Farmer Farmer { get; set; }
    }
}
