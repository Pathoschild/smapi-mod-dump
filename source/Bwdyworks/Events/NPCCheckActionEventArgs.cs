using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bwdyworks.Events
{
    public class NPCCheckActionEventArgs : CancelableEventArgs
    {
        public NPC NPC { set; get; }
        public Farmer Farmer { set; get; }
    }
}
