/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/stokastic/PrismaticTools
**
*************************************************/

using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace PrismaticTools.Framework {
    public class PrismaticAPI {
        public int SprinklerRange { get; } = ModEntry.Config.SprinklerRange;
        public int SprinklerIndex { get; } = PrismaticSprinklerItem.INDEX;
        public int BarIndex { get; } = PrismaticBarItem.INDEX;
        public bool ArePrismaticSprinklersScarecrows { get; } = ModEntry.Config.UseSprinklersAsScarecrows;

        public IEnumerable<Vector2> GetSprinklerCoverage(Vector2 origin) {
            for (int x = -SprinklerRange; x <= SprinklerRange; x++) {
                for (int y = -SprinklerRange; y <= SprinklerRange; y++) {
                    yield return new Vector2(x, y) + origin;
                }
            }
        }
    }
}
