using StardewValley;
using StardewValley.Characters;
using Harmony;
using StardewValley.Locations;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Collections.Generic;

namespace FamilyPlanning.Patches
{
    [HarmonyPatch(typeof(Child))]
    [HarmonyPatch("tenMinuteUpdate")]
    class ChildTenMinuteUpdatePatch
    {
        public static void Postfix(Child __instance)
        {
            if (!Game1.IsMasterGame || __instance.Age != 3 || Game1.timeOfDay != 1900)
                return;

            __instance.IsWalkingInSquare = false;
            __instance.Halt();
            FarmHouse currentLocation = __instance.currentLocation as FarmHouse;
            if (!currentLocation.characters.Contains(__instance))
                return;

            /* 
             * Stardew Valley would normally use getChildBed(this.gender),
             * but that always puts children on one Tile by gender and will stack children.
             * 
             * Currently, children will choose their bed by order of birth and gender.
             * Oldest siblings get priority, so the first child always sleeps in the same spot.
             * Otherwise, younger siblings sort themselves to try and pair like genders.
             * (The youngest child will always get stuck in the last open spot, though.)
             * 
             * If I expand this mod to edit the house, then I will need to revisit this.
             */

            int i = 1;
            int birthNumber = 0;
            int boys = 0;
            int girls = 0;
            List<Child> children = Game1.player.getChildren();
            foreach (Child child in children)
            {
                if (child.Gender == 0)
                    boys++;
                else
                    girls++;

                if (child.Equals(__instance))
                {
                    birthNumber = i;
                    break;
                }
                i++;
            }

            Point childBed = new Point(23, 5);

            if(birthNumber != 1 && boys + girls <= 2)
            {
                childBed = new Point(27, 5);
            }
            else if (birthNumber != 1 && boys + girls > 2)
            {
                if (children[0].Gender == children[1].Gender)
                {
                    if (birthNumber == 2)
                        childBed = new Point(22, 5);
                    else if (birthNumber == 3)
                        childBed = new Point(27, 5);
                    else if (birthNumber == 4)
                        childBed = new Point(26, 5);
                }
                else
                {
                    if (birthNumber == 2)
                        childBed = new Point(27, 5);

                    if (children[2].Gender == children[3].Gender)
                    {
                        if (birthNumber == 3)
                            childBed = new Point(26, 5);
                        else
                            childBed = new Point(22, 5);
                    }
                    else
                    {
                        if (birthNumber == 3)
                            childBed = new Point(22, 5);
                        else
                            childBed = new Point(26, 5);
                    }
                }
            }

            __instance.controller = new PathFindController(__instance, currentLocation, childBed, -1, new PathFindController.endBehavior(__instance.toddlerReachedDestination));
            if (__instance.controller.pathToEndPoint != null && currentLocation.isTileOnMap(__instance.controller.pathToEndPoint.Last().X, __instance.controller.pathToEndPoint.Last().Y))
                return;
            __instance.controller = null;
        }
    }
}
