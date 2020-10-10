/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/loe2run/ChildToNPC
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using Harmony;
using StardewValley;
using StardewValley.Locations;

namespace ChildToNPC.Patches
{
    /* Prefix for PathFindController.moveCharacter
     * Most of this code is directly translated from the original method
     * because the section with the marriage requirement is in the middle.
     * So I fully execute the necessary code, only modifying the spot I need to.
     */
    [HarmonyPatch(typeof(PathFindController))]
    [HarmonyPatch("moveCharacter")]
    class PFCMoveCharacterPatch
    {
        public static bool Prefix(ref PathFindController __instance, Character ___character, GameTime time)
        {
            if (!ModEntry.IsChildNPC(___character))
                return true;
            
            Rectangle rectangle = new Rectangle(__instance.pathToEndPoint.Peek().X * 64, __instance.pathToEndPoint.Peek().Y * 64, 64, 64);
            rectangle.Inflate(-2, 0);
            Rectangle boundingBox = ___character.GetBoundingBox();

            if ((rectangle.Contains(boundingBox) || boundingBox.Width > rectangle.Width && rectangle.Contains(boundingBox.Center)) && rectangle.Bottom - boundingBox.Bottom >= 2)
            {
                __instance.timerSinceLastCheckPoint = 0;
                __instance.pathToEndPoint.Pop();
                ___character.stopWithoutChangingFrame();
                if (__instance.pathToEndPoint.Count != 0)
                    return false;
                ___character.Halt();
                if (__instance.finalFacingDirection != -1)
                    ___character.faceDirection(__instance.finalFacingDirection);
                if (__instance.NPCSchedule)
                {
                    (___character as NPC).DirectionsToNewLocation = null;
                    (___character as NPC).endOfRouteMessage.Value = (___character as NPC).nextEndOfRouteMessage;
                }
                if (__instance.endBehaviorFunction == null)
                    return false;
                __instance.endBehaviorFunction(___character, __instance.location);
                return false;
            }
            else
            {
                foreach (NPC character in __instance.location.characters)
                {
                    if (!character.Equals(___character) && character.GetBoundingBox().Intersects(boundingBox) && character.isMoving() && string.Compare(character.Name, ___character.Name) < 0)
                    {
                        ___character.Halt();
                        return false;
                    }
                }
                if (boundingBox.Left < rectangle.Left && boundingBox.Right < rectangle.Right)
                    ___character.SetMovingRight(true);
                else if (boundingBox.Right > rectangle.Right && boundingBox.Left > rectangle.Left)
                    ___character.SetMovingLeft(true);
                else if (boundingBox.Top <= rectangle.Top)
                    ___character.SetMovingDown(true);
                else if (boundingBox.Bottom >= rectangle.Bottom - 2)
                    ___character.SetMovingUp(true);
                ___character.MovePosition(time, Game1.viewport, __instance.location);
                if (!__instance.NPCSchedule)
                    return false;
                Warp warp = __instance.location.isCollidingWithWarpOrDoor(___character.nextPosition(___character.getDirection()));
                if (warp == null)
                    return false;

                //This is the point where character needs to be married, I'm fixing.
                if (___character is NPC /*&& (___character as NPC).isMarried() */ && (___character as NPC).followSchedule)
                {
                    NPC character = ___character as NPC;
                    if (__instance.location is FarmHouse)
                        warp = new Warp(warp.X, warp.Y, "BusStop", 0, 23, false);

                    if (__instance.location is BusStop && warp.X <= 0)
                        warp = new Warp(warp.X, warp.Y, character.getHome().Name, (character.getHome() as FarmHouse).getEntryLocation().X, (character.getHome() as FarmHouse).getEntryLocation().Y, false);
                    if (character.temporaryController != null && character.controller != null)
                        character.controller.location = Game1.getLocationFromName(warp.TargetName);
                }
                
                Game1.warpCharacter(___character as NPC, warp.TargetName, new Vector2((float)warp.TargetX, (float)warp.TargetY));
                if (__instance.isPlayerPresent() && __instance.location.doors.ContainsKey(new Point(warp.X, warp.Y)))
                    __instance.location.playSoundAt("doorClose", new Vector2((float)warp.X, (float)warp.Y));
                __instance.location = Game1.getLocationFromName(warp.TargetName);
                if (__instance.isPlayerPresent() && __instance.location.doors.ContainsKey(new Point(warp.TargetX, warp.TargetY - 1)))
                    __instance.location.playSoundAt("doorClose", new Vector2((float)warp.TargetX, (float)warp.TargetY));
                if (__instance.pathToEndPoint.Count > 0)
                    __instance.pathToEndPoint.Pop();
                while (__instance.pathToEndPoint.Count > 0 && (Math.Abs(__instance.pathToEndPoint.Peek().X - ___character.getTileX()) > 1 || Math.Abs(__instance.pathToEndPoint.Peek().Y - ___character.getTileY()) > 1))
                    __instance.pathToEndPoint.Pop();

                return false;
            }
        }
    }
}