/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardustCore.Events
{
    public class ConcurrentEventInformation
    {
        public string id;
        public string commandInfo;
        public Action<EventManager, string> functionToRun;
        public bool finished;
        public EventManager eventManager;

        public ConcurrentEventInformation()
        {

        }

        public ConcurrentEventInformation(string ID, string CommandInfo,EventManager EventManager ,Action<EventManager,string> Function)
        {
            this.id = ID;
            this.commandInfo = CommandInfo;
            this.eventManager = EventManager;
            this.functionToRun = Function;
        }

        public void finish()
        {
            this.finished = true;
        }


        public void invokeIfNotFinished()
        {
            if (this.finished) return;
            this.functionToRun.Invoke(this.eventManager, this.commandInfo);
        }

        public override int GetHashCode()
        {
            return this.id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return (obj as ConcurrentEventInformation).id.Equals(this.id);
        }

    }
}
