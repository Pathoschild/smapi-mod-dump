using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValleyEsp.Config;

namespace StardewValleyEsp.Detectors
{
    struct BareCrop
    {
        public string name;
        public string Name { get { return name; } }
    }

    class CropDetector : IDetector
    {
        private GameLocation location;
        private readonly Settings settings;

        public CropDetector(Settings settings)
        {
            this.settings = settings;
        }

        public EntityList Detect()
        {
            EntityList e = new EntityList();
            if (location != null)
                foreach (var c in location.terrainFeatures.Pairs)
                    if (location.isTileOnMap(c.Key) && c.Value is StardewValley.TerrainFeatures.HoeDirt hoeDirt && hoeDirt.crop != null)
                        if (hoeDirt.crop.forageCrop.Value)
                            // currently the only thing that should pass is spring onions it seems
                            e.Add(new KeyValuePair<Vector2, object>(c.Key, new BareCrop { name = "Spring Onion" }));
                        else
                            // example string is Parsnip/35/10/Basic -75/Parsnip/A spring tuber...
                            e.Add(new KeyValuePair<Vector2, object>(c.Key, new BareCrop { name = Game1.objectInformation[hoeDirt.crop.indexOfHarvest.Value].Split('/')[0] }));
            return e;
        }

        public IDetector SetLocation(GameLocation loc)
        {
            location = loc;
            return this;
        }
    }
}
