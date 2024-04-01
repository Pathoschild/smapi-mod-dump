/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/csandwith/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;

namespace CustomSpouseRooms
{
    public interface ICustomSpouseRoomsAPI
    {
        public Point GetSpouseTileOffset(NPC spouse);
        public Point GetSpouseTile(NPC spouse);

        public Point GetSpouseRoomCornerTile(NPC spouse);
    }
}