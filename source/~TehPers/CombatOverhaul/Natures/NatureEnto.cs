using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using SFarmer = StardewValley.Farmer;

namespace TehPers.Stardew.CombatOverhaul.Natures {
    public class NatureEnto : Nature {
        public override bool activate(GameLocation location, int x, int y, int power, SFarmer who) {
            // TODO: If in a specific randomized tile, spawn a cake!
            return true;
        }

        public override string getName() {
            return "Ento";
        }

        public override string getDescription() {
            return "For some reason, smells... delicious?";
        }

        protected override string getTextureName() {
            return "EntoScepter";
        }
    }
}
