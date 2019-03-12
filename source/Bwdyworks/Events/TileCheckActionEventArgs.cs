using Microsoft.Xna.Framework;
using StardewValley;

namespace bwdyworks.Events
{
    public class TileCheckActionEventArgs : CancelableEventArgs
    {
        public GameLocation GameLocation { set; get; }
        public Vector2 TileLocation { set; get; }
        public string Action { get; set; }
        public Farmer Farmer { set; get; }
    }
}
