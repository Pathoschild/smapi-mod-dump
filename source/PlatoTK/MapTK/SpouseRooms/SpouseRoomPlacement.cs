/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

namespace MapTK.SpouseRooms
{
    internal class SpouseRoomPlacement
    {
        public string Name { get; }

        public int Spot { get; } = -1;

        public SpouseRoomPlacement(string name, int index)
        {
            Name = name;
            Spot = index;
        }
    }
}
