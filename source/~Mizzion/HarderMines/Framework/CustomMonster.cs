/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

using StardewValley.Monsters;

namespace HarderMines.Framework
{
    internal class CustomMonster : Monster
    {


        public CustomMonster(Monster m) : base(m.Name, m.Position)
        {

        }
    }
}
