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
    internal class SpouseRoomTokenY : SpouseRoomTokenX
    {
        protected override string[] GetResult(int x, int y)
        {
            return new[] { y.ToString() };
        }
    }
}
