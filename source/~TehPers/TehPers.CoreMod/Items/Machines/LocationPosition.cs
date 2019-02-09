using Microsoft.Xna.Framework;
using StardewValley;

namespace TehPers.CoreMod.Items.Machines {
    internal readonly struct LocationPosition {
        public GameLocation Location { get; }
        public Vector2 Position { get; }

        public LocationPosition(GameLocation location, Vector2 position) {
            this.Location = location;
            this.Position = position;
        }
    }
}