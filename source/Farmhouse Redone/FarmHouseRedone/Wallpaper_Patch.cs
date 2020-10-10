/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mjSurber/FarmHouseRedone
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Network;

namespace FarmHouseRedone
{
    class Wallpaper_placementAction_Patch
    {
        internal static bool Prefix(GameLocation location, int x, int y, Farmer who, ref bool __result, Wallpaper __instance)
        {
            if (who == null)
                who = Game1.player;
            if (who.currentLocation is DecoratableLocation)
                return true;
            DecoratableLocation host = OtherLocations.FakeDecor.FakeDecorHandler.getHost(who.currentLocation);
            if (host == null)
                return true;

            //We have our host now, so we will instead be using it on that host.

            Point point = new Point(x / 64, y / 64);

            if ((bool)((NetFieldBase<bool, NetBool>)__instance.isFloor))
            {
                List<Rectangle> floors = host.getFloors();
                for (int whichRoom = 0; whichRoom < floors.Count; ++whichRoom)
                {
                    if (floors[whichRoom].Contains(point))
                    {
                        host.setFloor((int)((NetFieldBase<int, NetInt>)__instance.parentSheetIndex), whichRoom, true);
                        host.setFloors();
                        location.playSound("coin", NetAudio.SoundContext.Default);
                        __result = true;
                        return false;
                    }
                }
            }
            else
            {
                List<Rectangle> walls = host.getWalls();
                for (int whichRoom = 0; whichRoom < walls.Count; ++whichRoom)
                {
                    if (walls[whichRoom].Contains(point))
                    {
                        host.setWallpaper((int)((NetFieldBase<int, NetInt>)__instance.parentSheetIndex), whichRoom, true);
                        host.setWallpapers();
                        location.playSound("coin", NetAudio.SoundContext.Default);
                        __result = true;
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
