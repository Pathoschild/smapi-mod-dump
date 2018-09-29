using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Characters;

namespace TehPers.Stardew.CombatOverhaul {
    public class ExplodingJunimo : Junimo {
        public PathFindController forcedController;

        public ExplodingJunimo(Vector2 position, int whichArea) : base(position, whichArea) {

        }

        public override void update(GameTime time, GameLocation location, long id, bool move) {
            this.controller = forcedController;
            base.update(time, location, id, move);
        }
    }
}
