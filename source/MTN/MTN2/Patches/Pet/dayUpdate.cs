using Harmony;
using Microsoft.Xna.Framework;
using MTN2.MapData;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTN2.Patches.PetPatches
{
    public class dayUpdatePatch
    {
        private static CustomFarmManager farmManager;

        public dayUpdatePatch(CustomFarmManager farmManager) {
            dayUpdatePatch.farmManager = farmManager;
        }

        public static bool Prefix(Pet __instance) {
            return (farmManager.Canon) ? true : false;
        }

        public static void Postfix(Pet __instance) {
            if (farmManager.Canon) return;

            Interaction PetBowl = farmManager.PetWaterBowl;

            __instance.DefaultPosition = new Vector2(PetBowl.X, PetBowl.Y) * 64f;
            __instance.Sprite.loop = false;
            __instance.Breather = false;
            if (Game1.isRaining) {
                __instance.CurrentBehavior = 2;
                if (__instance.currentLocation is Farm) {
                    __instance.warpToFarmHouse(Game1.player);
                }
            } else if (__instance.currentLocation is FarmHouse) {
                setAtFarmPosition(__instance, PetBowl);
                //__instance.setAtFarmPosition();
            }
            if (__instance.currentLocation is Farm) {
                if (__instance.currentLocation.getTileIndexAt(PetBowl.X, PetBowl.Y, "Buildings") == 1939) {
                    __instance.friendshipTowardFarmer = Math.Min(1000, __instance.friendshipTowardFarmer + 6);
                }
                __instance.currentLocation.setMapTileIndex(PetBowl.X, PetBowl.Y, 1938, "Buildings", 0);
                __instance.setTilePosition(PetBowl.X, PetBowl.Y + 1);
                __instance.position.X -= 64f;
            }
            __instance.Halt();
            __instance.CurrentBehavior = 1;
            //__instance.wasPetToday = false;
            Traverse.Create(__instance).Field("wasPetToday").SetValue(false);
        }

        public static void setAtFarmPosition(Pet pet, Interaction PetBowl) {
            if (!Game1.isRaining) {
                pet.faceDirection(2);
                Game1.warpCharacter(pet, "Farm", new Vector2(PetBowl.X, PetBowl.Y - 1));
                pet.position.X -= 64f;
            }
        }
    }
}
