/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;

namespace CustomSpouseRooms
{
    public class CustomSpouseRoomsAPI
    {
        public Point GetSpouseTileOffset(NPC spouse)
        {
            if (ModEntry.currentRoomData.ContainsKey(spouse.Name))
                return ModEntry.currentRoomData[spouse.Name].spousePosOffset;
            return new Point(-1,-1);
        }

        public Point GetSpouseTile(NPC spouse)
        {
            if (ModEntry.currentRoomData.ContainsKey(spouse.Name))
            {
                Point point = ModEntry.currentRoomData[spouse.Name].startPos + ModEntry.currentRoomData[spouse.Name].spousePosOffset;
                ModEntry.PMonitor.Log($"Sending spouse tile for {spouse.Name}: {point}");
                return point;
            }
            ModEntry.PMonitor.Log($"Couldn't get spouse tile for {spouse.Name}! Rooms: {ModEntry.currentRoomData.Count}");
            foreach(var s in ModEntry.currentRoomData.Keys)
            {
                ModEntry.PMonitor.Log($"Have: {s}!");
            }
            return new Point(-1,-1);
        }

        public Point GetSpouseRoomCornerTile(NPC spouse)
        {
            if (ModEntry.currentRoomData.ContainsKey(spouse.Name))
                return ModEntry.currentRoomData[spouse.Name].startPos;
            return new Point(-1, -1);
        }
    }
}