using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTN.Patches.PetPatch {
    class setAtFarmPositionPatch {
        public static bool Prefix(Pet __instance) {
            return (Memory.isWaterBowlRelocated) ? false : true;
        }

        public static void Postfix(Pet __instance) {
            if (!Memory.isWaterBowlRelocated) return;

            if (!Game1.isRaining) {
                __instance.faceDirection(2);
                Game1.warpCharacter(__instance, "Farm", new Vector2(Memory.loadedFarm.petWaterBowl.pointOfInteraction.x, Memory.loadedFarm.petWaterBowl.pointOfInteraction.y - 1));
                __instance.position.X -= 64f;
            }
            return;
        }
    }
}
