
using Microsoft.Xna.Framework;
using StardewValley;
using System;

namespace MapPings.Framework.Events {

    public class ReceivedMapPingEventArgs : EventArgs {
        public Farmer SourceFarmer { get; set; }
        public Vector2 MapCoords { get; set; }
    }

}
