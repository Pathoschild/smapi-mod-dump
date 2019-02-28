using bwdyworks.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bwdyworks.API
{
    public class Events
    {
        //NPC Check Action - called when a player activates an NPC. Cancellable.
        public event NPCCheckActionHandler NPCCheckAction;
        public delegate void NPCCheckActionHandler(object sender, NPCCheckActionEventArgs args);
        internal NPCCheckActionEventArgs NPCCheckActionEvent(Farmer who, NPC npc)
        {
            NPCCheckActionEventArgs args = new NPCCheckActionEventArgs
            {
                NPC = npc,
                Farmer = who,
                Cancelled = false
            };
            NPCCheckAction?.Invoke(this, args);
            return args;
        }
    }
}
