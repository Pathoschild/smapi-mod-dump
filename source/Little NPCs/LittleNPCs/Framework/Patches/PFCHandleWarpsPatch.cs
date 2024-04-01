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
using StardewValley.Pathfinding;
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
                warp = new Warp(warp.X, warp.Y, "Trailer_Big", 13, 24, flipFarmer: false);
            }

            // This is normally only for married NPCs.
            if (littleNPC is not null && littleNPC.followSchedule) {
                GameLocation gameLocation = __instance.location;
                if (!(gameLocation is FarmHouse)) {
                    if (gameLocation is BusStop && warp.X <= 9) {
                        GameLocation home = littleNPC.getHome();
                        Point entryLocation = (home as FarmHouse).getEntryLocation();
                        warp = new Warp(warp.X, warp.Y, home.Name, entryLocation.X, entryLocation.Y, flipFarmer: false);
                    }
                }
                else {
                    warp = new Warp(warp.X, warp.Y, "BusStop", 10, 23, flipFarmer: false);
                }

                if (littleNPC.temporaryController is not null && littleNPC.controller is not null) {
                    littleNPC.controller.location = Game1.RequireLocation(warp.TargetName);
                }
            }

            string text = warp.TargetName;
            foreach (string activePassiveFestival in Game1.netWorldState.Value.ActivePassiveFestivals) {
                if (Utility.TryGetPassiveFestivalData(activePassiveFestival, out var data) && data.MapReplacements is not null && data.MapReplacements.TryGetValue(text, out var value)) {
                    text = value;

                    break;
                }
            }

            if (littleNPC is not null && (warp.TargetName == "FarmHouse" || warp.TargetName == "Cabin")) {
                __instance.location = littleNPC.getHome();
                Point entryLocation2 = (__instance.location as FarmHouse).getEntryLocation();
                warp = new Warp(warp.X, warp.Y, __instance.location.Name, entryLocation2.X, entryLocation2.Y, flipFarmer: false);
                if (littleNPC.temporaryController is not null && littleNPC.controller is not null) {
                    littleNPC.controller.location = __instance.location;
                }
                Game1.warpCharacter(littleNPC, __instance.location, new Vector2(warp.TargetX, warp.TargetY));
            }
            else {
                __instance.location = Game1.RequireLocation(text);
                Game1.warpCharacter(littleNPC, warp.TargetName, new Vector2(warp.TargetX, warp.TargetY));
            }

            if (__instance.isPlayerPresent() && __instance.location.doors.ContainsKey(new Point(warp.X, warp.Y))) {
                __instance.location.playSound("doorClose", new Vector2(warp.X, warp.Y), null, StardewValley.Audio.SoundContext.NPC);
            }
            if (__instance.isPlayerPresent() && __instance.location.doors.ContainsKey(new Point(warp.TargetX, warp.TargetY - 1))) {
                __instance.location.playSound("doorClose", new Vector2(warp.TargetX, warp.TargetY), null, StardewValley.Audio.SoundContext.NPC);
            }
            if (__instance.pathToEndPoint.Count > 0) {
                __instance.pathToEndPoint.Pop();
            }
            Point tilePoint = littleNPC.TilePoint;
            while (__instance.pathToEndPoint.Count > 0 && (Math.Abs(__instance.pathToEndPoint.Peek().X - tilePoint.X) > 1 || Math.Abs(__instance.pathToEndPoint.Peek().Y - tilePoint.Y) > 1)) {
                __instance.pathToEndPoint.Pop();
            }

            // Disable original method.
            return false;
        }
    }
}
