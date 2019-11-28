using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley.Monsters;

namespace HarderMines.Framework
{
    internal class HarderMonster
    {
        public bool AttributesIniated;
        public Monster Monster;

        public HarderMonster(Monster monster)
        {
            Monster = monster;
            AttributesIniated = false;
        }
    }
}
