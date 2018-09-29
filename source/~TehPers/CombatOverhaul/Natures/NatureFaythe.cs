using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Characters;
using System;
using SFarmer = StardewValley.Farmer;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TehPers.Stardew.CombatOverhaul.Natures {
    public class NatureFaythe : Nature {

        public override bool activate(GameLocation location, int x, int y, int power, SFarmer who) {
            ExplodingJunimo j = new ExplodingJunimo(who.position, Game1.random.Next(0, 6));
            j.forcedController = new PathFindController(j, location, new Point((int) Game1.currentCursorTile.X, (int) Game1.currentCursorTile.Y), 0, new PathFindController.endBehavior((c, loc) => {
                Game1.playSound("explosion");
                location.explode(c.getTileLocation(), 3, who);
                loc.characters.Remove(j);
            }));
            Game1.currentLocation.characters.Add(j);
            return true;
        }

        public override string getName() {
            return "Faythe";
        }

        protected override string getTextureName() {
            return "FaytheScepter";
        }

        public override string getDescription() {
            return "Seems to be unnaturally warm";
        }

        public override bool playWandSound() {
            Game1.playSound("junimoMeep1");
            return false;
        }
    }
}
