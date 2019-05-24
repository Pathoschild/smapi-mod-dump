using Microsoft.Xna.Framework;
using sdv_helper.Config;
using StardewValley;
using System;
using System.Collections;
using System.Collections.Generic;

namespace sdv_helper.Detectors
{
    class NPCDetector : IDetector
    {
        private GameLocation location;
        private readonly Settings settings;

        public NPCDetector(Settings settings)
        {
            this.settings = settings;
        }

        public EntityList Detect()
        {
            EntityList e = new EntityList();
            if (location != null)
            {
                foreach (var c in location.getCharacters())
                {
                    if (!(c is NPC))
                        throw new Exception("Invalid object type provided to NPC detection list");
                    if (location.isTileOnMap(c.getTileLocation()))
                        e.Add(new KeyValuePair<Vector2, object>(c.Position / Game1.tileSize, c));
                }
                IEnumerable farmers = Game1.getAllFarmers();
                foreach (Farmer farmer in farmers)
                    if (farmer.currentLocation == location && location.isTileOnMap(farmer.Position / Game1.tileSize))
                        e.Add(new KeyValuePair<Vector2, object>(farmer.Position / Game1.tileSize, farmer));
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
