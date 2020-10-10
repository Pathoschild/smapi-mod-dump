/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/rei2hu/stardewvalley-esp
**
*************************************************/

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValleyEsp.Config;

namespace StardewValleyEsp.Detectors
{
    class ObjectDetector : IDetector
    {
        public GameLocation location;
        private readonly Settings settings;

        public ObjectDetector(Settings settings)
        {
            this.settings = settings;
        }

        public EntityList Detect()
        {
            EntityList e = new EntityList();
            if (location != null)
                foreach (var c in location.Objects.Pairs)
                    if (location.isTileOnMap(c.Key))
                        e.Add(new KeyValuePair<Vector2, object>(c.Key, c.Value));
            return e;
        }

        public IDetector SetLocation(GameLocation loc)
        {
            location = loc;
            return this;
        }
    }
}
