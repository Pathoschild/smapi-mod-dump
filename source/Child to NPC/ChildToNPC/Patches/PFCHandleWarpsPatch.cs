/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/loe2run/ChildToNPC
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Network;
using System;

namespace ChildToNPC.Patches
{
    /* Postfix for handleWarps
     * Most of this code is directly taken from the original method.
     * It handles the issue where non-married NPCs would be kicked out.
     */
    class PFCHandleWarpsPatch
    {
        public static bool Prefix(Rectangle position, PathFindController __instance, Character ___character)
        {
            if (!ModEntry.IsChildNPC(___character))
                return true;

            Warp warp = __instance.location.isCollidingWithWarpOrDoor(position);
            if (warp == null)
                return false;

            if (warp.TargetName == "Trailer" && Game1.MasterPlayer.mailReceived.Contains("pamHouseUpgrade"))
                warp = new Warp(warp.X, warp.Y, "Trailer_Big", 13, 24, false);

            //This is normally only for married NPCs
            if (___character is NPC && (___character as NPC).followSchedule)
            {
                NPC character = ___character as NPC;
                if (__instance.location is FarmHouse)
                    warp = new Warp(warp.X, warp.Y, "BusStop", 0, 23, false);
                if (__instance.location is BusStop && warp.X <= 0)
                    warp = new Warp(warp.X, warp.Y, character.getHome().Name, (character.getHome() as FarmHouse).getEntryLocation().X, (character.getHome() as FarmHouse).getEntryLocation().Y, false);
                if (character.temporaryController != null && character.controller != null)
                    character.controller.location = Game1.getLocationFromName(warp.TargetName);
            }

            __instance.location = Game1.getLocationFromName(warp.TargetName);
            //This is normally only for married NPCs
            if (___character is NPC && (warp.TargetName == "FarmHouse" || warp.TargetName == "Cabin"))
            {
                __instance.location = Utility.getHomeOfFarmer(Game1.getFarmer(ModEntry.GetFarmerParentId(___character)));
                warp = new Warp(warp.X, warp.Y, __instance.location.Name, (__instance.location as FarmHouse).getEntryLocation().X, (__instance.location as FarmHouse).getEntryLocation().Y, false);
                if ((___character as NPC).temporaryController != null && (___character as NPC).controller != null)
                    (___character as NPC).controller.location = __instance.location;
            }
            Game1.warpCharacter(___character as NPC, __instance.location, new Vector2(warp.TargetX, warp.TargetY));

            if (__instance.isPlayerPresent() && __instance.location.doors.ContainsKey(new Point(warp.X, warp.Y)))
                __instance.location.playSoundAt("doorClose", new Vector2(warp.X, warp.Y), NetAudio.SoundContext.NPC);
            if (__instance.isPlayerPresent() && __instance.location.doors.ContainsKey(new Point(warp.TargetX, warp.TargetY - 1)))
                __instance.location.playSoundAt("doorClose", new Vector2(warp.TargetX, warp.TargetY), NetAudio.SoundContext.NPC);
            if (__instance.pathToEndPoint.Count > 0)
                __instance.pathToEndPoint.Pop();
            while (__instance.pathToEndPoint.Count > 0 && (Math.Abs(__instance.pathToEndPoint.Peek().X - ___character.getTileX()) > 1 || Math.Abs(__instance.pathToEndPoint.Peek().Y - ___character.getTileY()) > 1))
                __instance.pathToEndPoint.Pop();

            return false;
        }
    }
}