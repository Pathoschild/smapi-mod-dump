using StardewValley;
using System;

namespace NpcAdventure.AI
{
    public class EventArgsLocationChanged : EventArgs
    {
        public GameLocation PreviousLocation { get; set; }
        public GameLocation CurrentLocation { get; set; }
    }
}