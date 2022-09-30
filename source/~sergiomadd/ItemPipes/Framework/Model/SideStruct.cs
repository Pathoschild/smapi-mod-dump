/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sergiomadd/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemPipes.Framework.Model
{
    public class SideStruct
    {
        private static SideStruct mySides;
        public Side North { get; }
        public Side South { get; }
        public Side West { get; }
        public Side East { get; }

        public SideStruct()
        {
            North = new Side("North");
            South = new Side("South");
            West = new Side("West");
            East = new Side("East");
        }
        public static SideStruct GetSides()
        {
            if(mySides == null)
            {
                mySides = new SideStruct();
            }
            return mySides;
        }

        public Side GetInverse(Side side)
        {
            Side inverse = null;
            if(side.Name.Equals("North"))
            {
                inverse = South;
            }
            else if(side.Name.Equals("South"))
            {
                inverse = North;
            }
            else if (side.Name.Equals("West"))
            {
                inverse = East;
            }
            else if (side.Name.Equals("East"))
            {
                inverse = West;
            }
            return inverse;
        }
    }
}
