/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/maxvollmer/DeepWoodsMod
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepWoodsMod
{
    public class XY
    {
        public int X { get; set; }
        public int Y { get; set; }
        public XY() { }
        public XY(int x, int y)
        {
            X = x;
            Y = y;
        }
        public override bool Equals(Object o)
        {
            return o is XY xy && xy.X == X && xy.Y == Y;
        }
        public override int GetHashCode()
        {
            return X ^ Y;
        }
    }
}
