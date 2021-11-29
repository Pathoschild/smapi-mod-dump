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

namespace CustomSpouseRooms
{
    public class SpouseRoomData
    {
        public string name;
        public int upgradeLevel;
        public int templateIndex = -1;
        public string templateName;
        public Point startPos = new Point(-1,-1);
        public string shellType;
        public Point spousePosOffset = new Point(4, 5);
    }
}