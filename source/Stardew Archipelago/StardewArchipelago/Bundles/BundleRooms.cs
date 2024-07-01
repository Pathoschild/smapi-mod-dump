/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System.Collections.Generic;
using StardewArchipelago.Constants;
using StardewArchipelago.Stardew;

namespace StardewArchipelago.Bundles
{
    public class BundleRooms
    {
        public Dictionary<string, BundleRoom> Rooms { get; set; }
        public Dictionary<string, Bundle> BundlesByName { get; set; }

        public BundleRooms(StardewItemManager itemManager, Dictionary<string, Dictionary<string, Dictionary<string, string>>> bundlesDictionary)
        {
            Rooms = new Dictionary<string, BundleRoom>();
            BundlesByName = new Dictionary<string, Bundle>();

            foreach (var (roomName, roomBundles) in bundlesDictionary)
            {
                var room = new BundleRoom(itemManager, roomName, roomBundles);
                Rooms.Add(roomName, room);
                foreach (var (name, bundle) in room.Bundles)
                {
                    BundlesByName.Add(name, bundle);
                }
            }
        }

        public Dictionary<string, string> ToStardewStrings()
        {
            var stardewStrings = new Dictionary<string, string>();
            foreach (var (roomName, bundleRoom) in Rooms)
            {
                if (roomName == APName.RACCOON_REQUESTS_ROOM)
                {
                    continue;
                }

                foreach (var (key, value) in bundleRoom.ToStardewStrings())
                {
                    stardewStrings.Add(key, value);
                }
            }

            return stardewStrings;
        }
    }
}
