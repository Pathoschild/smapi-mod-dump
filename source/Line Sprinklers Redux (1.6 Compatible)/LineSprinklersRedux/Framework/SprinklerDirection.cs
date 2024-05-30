/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/rtrox/LineSprinklersRedux
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LineSprinklersRedux.Framework
{
    public enum SprinklerDirection
    {
        Right,
        Down,
        Left,
        Up,
    }

    static class SprinkerDirectionMethods
    {
        public static SprinklerDirection Cycle(this SprinklerDirection direction)
        {
            if (direction == SprinklerDirection.Up)
            {
                return SprinklerDirection.Right;
            }
            return ++direction;
        }
    }
}
