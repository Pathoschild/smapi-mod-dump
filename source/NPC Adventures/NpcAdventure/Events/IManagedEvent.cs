using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NpcAdventure.Events
{
    internal interface IManagedEvent
    {
        /// <summary>A human-readable name for the event.</summary>
        string EventName { get; }
    }
}
