using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTN2.Patches.PetPatches
{
    public class setAtFarmPositionPatch
    {
        private static CustomManager customManager;

        public setAtFarmPositionPatch(CustomManager customManager) {
            setAtFarmPositionPatch.customManager = customManager;
        }

        public static bool Prefix(Pet __instance) {
            return (customManager.Canon) ? true : false;
        }

        public static void Postfix(Pet __instance) {
            if (customManager.Canon) return;

            if (!Game1.isRaining) {
                __instance.faceDirection(2);
                Game1.warpCharacter(__instance, "Farm", new Vector2(customManager.PetWaterBowl.X, customManager.PetWaterBowl.Y - 1));
                __instance.position.X -= 64f;
            }
            return;
        }
    }
}
