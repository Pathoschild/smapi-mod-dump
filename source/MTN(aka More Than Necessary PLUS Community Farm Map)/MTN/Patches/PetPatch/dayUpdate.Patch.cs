using Harmony;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace MTN.Patches.PetPatch
{
    class dayUpdatePatch
    {
        public static bool Prefix(Pet __instance)
        {
            return (Memory.isWaterBowlRelocated) ? false : true;
        }

        /*
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            for (int i = 0; i < 7; i++)
            {
                codes[i].opcode = OpCodes.Nop;
            }

            return codes.AsEnumerable();
        }
        */

        public static void Postfix(Pet __instance)
        {
            if (!Memory.isWaterBowlRelocated) return;

            __instance.DefaultPosition = new Vector2(Memory.loadedFarm.petWaterBowl.pointOfInteraction.x, Memory.loadedFarm.petWaterBowl.pointOfInteraction.y) * 64f;
            __instance.Sprite.loop = false;
            __instance.Breather = false;
            if (Game1.isRaining)
            {
                __instance.CurrentBehavior = 2;
                if (__instance.currentLocation is Farm)
                {
                    __instance.warpToFarmHouse(Game1.player);
                }
            }
            else if (__instance.currentLocation is FarmHouse)
            {
                setAtFarmPosition(__instance);
                //__instance.setAtFarmPosition();
            }
            if (__instance.currentLocation is Farm)
            {
                if (__instance.currentLocation.getTileIndexAt(Memory.loadedFarm.petWaterBowl.pointOfInteraction.x, Memory.loadedFarm.petWaterBowl.pointOfInteraction.y, "Buildings") == 1939)
                {
                    __instance.friendshipTowardFarmer = Math.Min(1000, __instance.friendshipTowardFarmer + 6);
                }
                __instance.currentLocation.setMapTileIndex(Memory.loadedFarm.petWaterBowl.pointOfInteraction.x, Memory.loadedFarm.petWaterBowl.pointOfInteraction.y, 1938, "Buildings", 0);
                __instance.setTilePosition(Memory.loadedFarm.petWaterBowl.pointOfInteraction.x, Memory.loadedFarm.petWaterBowl.pointOfInteraction.y + 1);
                __instance.position.X -= 64f;
            }
            __instance.Halt();
            __instance.CurrentBehavior = 1;
            //__instance.wasPetToday = false;
            Traverse.Create(__instance).Field("wasPetToday").SetValue(false);
        }

        public static void setAtFarmPosition(Pet pet) {
            if (!Game1.isRaining) {
                pet.faceDirection(2);
                Game1.warpCharacter(pet, "Farm", new Vector2(Memory.loadedFarm.petWaterBowl.pointOfInteraction.x, Memory.loadedFarm.petWaterBowl.pointOfInteraction.y - 1));
                pet.position.X -= 64f;
            }
        }
    }
}
