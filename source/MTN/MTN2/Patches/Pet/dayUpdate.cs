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
    /// <summary>
    /// REASON FOR PATCHING: The relocation of the pet water bowl.
    /// 
    /// Patches the Pet.dayUpdate method to accomidate for the relocation
    /// of the pet water bowl.
    /// </summary>
    public class dayUpdatePatch
    {
        private static ICustomManager customManager;

        /// <summary>
        /// Constructor. Awkward method of setting references needed. However, Harmony patches
        /// are required to be static. Thus we must break good Object Orientated practices.
        /// </summary>
        /// <param name="CustomManager">The class controlling information pertaining to the customs (and the loaded customs).</param>
        public dayUpdatePatch(ICustomManager customManager) {
            dayUpdatePatch.customManager = customManager;
        }

        /// <summary>
        /// Prefix Method. Occurs before the original method is executed.
        /// 
        /// Checks to see if a custom farm map is loaded. Skips original method if so.
        /// </summary>
        /// <param name="__instance">The instance of <see cref="Pet"/> that called dayUpdate.</param>
        /// <returns></returns>
        public static bool Prefix(Pet __instance) {
            return (customManager.Canon) ? true : false;
        }

        /// <summary>
        /// Postfix Method. Occurs after the original method is executed.
        /// 
        /// Checks to see if a custom farm map is loaded. If so, reimplements
        /// the logic of dayUpdate with new coordinates.
        /// </summary>
        /// <param name="__instance">The instance of <see cref="Pet"/> that called dayUpdate.</param>
        public static void Postfix(Pet __instance) {
            if (customManager.Canon) return;

            Interaction PetBowl = customManager.PetWaterBowl;

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
                    __instance.friendshipTowardFarmer = new Netcode.NetInt(Math.Min(1000, __instance.friendshipTowardFarmer + 6));
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

        /// <summary>
        /// Internal Method calling.
        /// </summary>
        /// <param name="pet"></param>
        /// <param name="PetBowl"></param>
        public static void setAtFarmPosition(Pet pet, Interaction PetBowl) {
            if (!Game1.isRaining) {
                pet.faceDirection(2);
                Game1.warpCharacter(pet, "Farm", new Vector2(PetBowl.X, PetBowl.Y - 1));
                pet.position.X -= 64f;
            }
        }
    }
}
