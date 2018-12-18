using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnifiedSaveCore.Framework
{
    public class ObjectHandler : IInformationHandler
    {
        public Dictionary<GameLocation, List<StardewValley.Object>> objects;

        public ObjectHandler()
        {
            objects = new Dictionary<GameLocation, List<StardewValley.Object>>();
        }

        public void afterLoad()
        {
           
        }

        /// <summary>
        /// After save restore all custom objects.
        /// </summary>
        public void afterSave()
        {
            foreach(KeyValuePair<GameLocation,List<StardewValley.Object>> pair in objects)
            {
                foreach(var obj in pair.Value)
                {
                    pair.Key.objects.Add(obj.tileLocation,obj);
                }
            }
            objects.Clear();
        }

        /// <summary>
        /// Before save iterate across all locations and remove all custom objects.
        /// </summary>
        public void beforeSave()
        {
            List<GameLocation> locations = new List<GameLocation>();
            foreach(Building b in (Game1.getLocationFromName("Farm") as Farm).buildings)
            {
                locations.Add(b.indoors);
            }
            foreach(GameLocation loc in Game1.locations)
            {
                locations.Add(loc);
            }

            foreach(GameLocation loc in locations)
            {
                objects.Add(loc, new List<StardewValley.Object>());
                foreach(var something in loc.objects)
                {
                    foreach(var obj in something.Values)
                    {
                        foreach(var type in UnifiedSaveCore.modTypes)
                        {
                            if (obj.GetType().ToString() == type.ToString())
                            {
                                objects[loc].Add(obj);
                            }
                        }
                    }
                }
            }
        }
    }
}
