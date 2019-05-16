using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using sdv_helper.Config;
using StardewValley;

// basically single blocks you can break
namespace sdv_helper.Detectors
{
    class ObjectDetector : IDetector
    {
        public GameLocation location;
        private Settings settings;
        public ObjectDetector(Settings settings)
        {
            this.settings = settings;
        }

        public EntityList Detect()
        {
            EntityList e = new EntityList();
            if (location != null)
                foreach (var c in location.Objects.Pairs)
                {
                    // if (!(c is KeyValuePair<Vector2, StardewValley.Object>))
                    // throw new Exception("Invalid object type provided to Object detection list");
                    e.Add(new KeyValuePair<Vector2, object>(c.Key, c.Value));
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
