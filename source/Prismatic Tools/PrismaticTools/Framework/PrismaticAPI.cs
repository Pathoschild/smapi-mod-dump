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
