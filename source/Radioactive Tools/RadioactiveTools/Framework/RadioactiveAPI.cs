/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/kakashigr/stardew-radioactivetools
**
*************************************************/

using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace RadioactiveTools.Framework {
    public class RadioactiveAPI {
        public int SprinklerRange { get; } = ModEntry.Config.SprinklerRange;
        public int SprinklerIndex { get; } = RadioactiveSprinklerItem.INDEX;
        public int BarIndex { get; } = 910;
        public bool AreRadioactiveSprinklersScarecrows { get; } = ModEntry.Config.UseSprinklersAsScarecrows;

        public IEnumerable<Vector2> GetSprinklerCoverage(Vector2 origin) {
            for (int x = -this.SprinklerRange; x <= this.SprinklerRange; x++) {
                for (int y = -this.SprinklerRange; y <= this.SprinklerRange; y++)
                    yield return new Vector2(x, y) + origin;
            }
        }
    }
}
