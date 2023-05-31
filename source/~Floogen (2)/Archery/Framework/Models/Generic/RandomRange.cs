/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/Archery
**
*************************************************/

using Archery.Framework.Interfaces.Internal;
using System;

namespace Archery.Framework.Models.Generic
{
    public class RandomRange : IRandomRange
    {
        public int Min { get; set; }
        public int Max { get; set; }

        public int Get(Random random, int minOffset = 0, int maxOffset = 0)
        {
            return random.Next(Min + minOffset, Max + maxOffset + 1);
        }
    }
}
