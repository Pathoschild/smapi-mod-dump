/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mus-candidus/LittleNPCs
**
*************************************************/

using System;

using Microsoft.Xna.Framework;

using StardewValley;
using StardewValley.Locations;
using StardewValley.Network;


namespace LittleNPCs.Framework.Patches {
    /// <summary>
    /// Prefix for <code>PathFindController.handleWarps</code>.
    /// Most of this code is directly taken from the original method.
    /// It handles the issue where non-married NPCs would be kicked out.
    /// </summary>
    class PFCHandleWarpsPatch {
        public static bool Prefix(Rectangle position, PathFindController __instance, Character ___character) {
            if (___character is not LittleNPC littleNPC) {
                return true;
            }

            Warp warp = __instance.location.isCollidingWithWarpOrDoor(position, littleNPC);
            if (warp is null) {
                return false;
            }

            if (warp.TargetName == "Trailer" && Game1.MasterPlayer.mailReceived.Contains("pamHouseUpgrade")) {
                warp = new Warp(warp.X, warp.Y, "Trailer_Big", 13, 24, false);
            }

            // This is normally only for married NPCs.
            if (littleNPC.followSchedule) {
                if (__instance.location is FarmHouse) {
                    warp = new Warp(warp.X, warp.Y, "BusStop", 0, 23, false);
                }
                if (__instance.location is BusStop && warp.X <= 0) {
                    warp = new Warp(warp.X, warp.Y, littleNPC.getHome().Name, (littleNPC.getHome() as FarmHouse).getEntryLocation().X, (littleNPC.getHome() as FarmHouse).getEntryLocation().Y, false);
                }
                if (littleNPC.temporaryController is not null && littleNPC.controller is not null) {
                    littleNPC.controller.location = Game1.getLocationFromName(warp.TargetName);
                }
            }

            __instance.location = Game1.getLocationFromName(warp.TargetName);
            // This is normally only for married NPCs.
            if (warp.TargetName == "FarmHouse" || warp.TargetName == "Cabin") {
                __instance.location = Utility.getHomeOfFarmer(Game1.getFarmer(littleNPC.WrappedChild.idOfParent.Value));
                warp = new Warp(warp.X, warp.Y, __instance.location.Name, (__instance.location as FarmHouse).getEntryLocation().X, (__instance.location as FarmHouse).getEntryLocation().Y, false);
                if (littleNPC.temporaryController is not null && littleNPC.controller is not null) {
                    littleNPC.controller.location = __instance.location;
                }
                Game1.warpCharacter(littleNPC, __instance.location, new Vector2(warp.TargetX, warp.TargetY));
            }
            else {
                Game1.warpCharacter(littleNPC, warp.TargetName, new Vector2(warp.TargetX, warp.TargetY));
            }

            if (__instance.isPlayerPresent() && __instance.location.doors.ContainsKey(new Point(warp.X, warp.Y))) {
                __instance.location.playSoundAt("doorClose", new Vector2(warp.X, warp.Y), NetAudio.SoundContext.NPC);
            }
            if (__instance.isPlayerPresent() && __instance.location.doors.ContainsKey(new Point(warp.TargetX, warp.TargetY - 1))) {
                __instance.location.playSoundAt("doorClose", new Vector2(warp.TargetX, warp.TargetY), NetAudio.SoundContext.NPC);
            }
            if (__instance.pathToEndPoint.Count > 0) {
                __instance.pathToEndPoint.Pop();
            }
            while (__instance.pathToEndPoint.Count > 0 && (Math.Abs(__instance.pathToEndPoint.Peek().X - littleNPC.getTileX()) > 1 || Math.Abs(__instance.pathToEndPoint.Peek().Y - littleNPC.getTileY()) > 1)) {
                __instance.pathToEndPoint.Pop();
            }

            // Disable original method.
            return false;
        }
    }
}
