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
        private static CustomFarmManager farmManager;

        public setAtFarmPositionPatch(CustomFarmManager farmManager) {
            setAtFarmPositionPatch.farmManager = farmManager;
        }

        public static bool Prefix(Pet __instance) {
            return (farmManager.Canon) ? true : false;
        }

        public static void Postfix(Pet __instance) {
            if (farmManager.Canon) return;

            if (!Game1.isRaining) {
                __instance.faceDirection(2);
                Game1.warpCharacter(__instance, "Farm", new Vector2(farmManager.PetWaterBowl.X, farmManager.PetWaterBowl.Y - 1));
                __instance.position.X -= 64f;
            }
            return;
        }
    }
}
