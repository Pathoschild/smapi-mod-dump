using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using SFarmer = StardewValley.Farmer;
using Microsoft.Xna.Framework;
using StardewValley.TerrainFeatures;

namespace TehPers.Stardew.CombatOverhaul.Natures {
    public class NatureLife : Nature {
        public override bool activate(GameLocation location, int x, int y, int power, SFarmer who) {
            for (int relX = -2; relX <= 2; relX++) {
                for (int relY = -2; relY <= 2; relY++) {
                    Vector2 loc = new Vector2(who.getTileX() + relX, who.getTileY() + relY);
                    if (location.terrainFeatures.ContainsKey(loc)) {
                        HoeDirt feature = location.terrainFeatures[loc] as HoeDirt;
                        if (feature != null && feature.crop != null) {
                            feature.crop.growCompletely();
                        }
                    }
                }
            }

            return true;
        }

        public override string getDescription() {
            return "You feel tingly when you touch this";
        }

        public override string getName() {
            return "Life";
        }

        protected override string getTextureName() {
            return "LifeScepter";
        }
    }
}
