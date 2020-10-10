/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

namespace HarderMines.Framework
{
    internal class Treasure
    {
        public int Id;
        public int Count;

        /// <summary>
        /// Iniate the Treasure
        /// </summary>
        /// <param name="id">Treasure ID</param>
        /// <param name="count">The Number of id to give.</param>
        public Treasure(int id, int count)
        {
            Id = id;
            Count = count;
        }
    }
}
