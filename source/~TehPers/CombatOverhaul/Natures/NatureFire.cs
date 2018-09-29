using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewValley.Projectiles;
using SFarmer = StardewValley.Farmer;
using Microsoft.Xna.Framework;

namespace TehPers.Stardew.CombatOverhaul.Natures {
    public class NatureFire : Nature {
        public override bool activate(GameLocation location, int x, int y, int power, SFarmer who) {
            Vector2 direction = Game1.currentCursorTile - who.getTileLocation();
            direction.Normalize();
            direction *= 10;
            Game1.currentLocation.projectiles.Add(new BasicProjectile(15, 10, 3, 4, 0.0f, direction.X, direction.Y, who.position, "", "", true, false, who, false, null));
            return true;
        }

        public override string getDescription() {
            return "Warm to the touch";
        }

        protected override string getTextureName() {
            return "Fire";
        }

        public override string getName() {
            return "Fire";
        }
    }
}
