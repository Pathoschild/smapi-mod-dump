using Microsoft.Xna.Framework;
using sdv_helper.Config;
using StardewValley;
using System.Collections.Generic;

namespace sdv_helper.Detectors
{
    class WaterEntityDetector : IDetector
    {
        class WaterEntity
        {
            public string Name { get; set; }
        }

        private GameLocation location;
        private readonly Settings settings;

        public WaterEntityDetector(Settings settings)
        {
            this.settings = settings;
        }

        public EntityList Detect()
        {
            EntityList e = new EntityList();
            if (location != null)
            {
                if (location.fishSplashAnimation != null)
                    e.Add(new KeyValuePair<Vector2, object>(this.location.fishSplashAnimation.Position / Game1.tileSize, new WaterEntity() { Name = "Fishing Hotspot" }));
                if (location.orePanAnimation != null)
                    e.Add(new KeyValuePair<Vector2, object>(this.location.orePanAnimation.Position / Game1.tileSize, new WaterEntity() { Name = "Ore Panning" }));
            }
            return e;
        }

        public IDetector SetLocation(GameLocation loc)
        {
            location = loc;
            return this;
        }
    }
}
