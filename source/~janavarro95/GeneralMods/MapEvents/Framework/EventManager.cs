using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventSystem.Framework
{
    public class EventManager
    {
        public Dictionary<GameLocation, List<MapEvent>> mapEvents;

        /// <summary>
        /// Constructor.
        /// </summary>
        public EventManager()
        {
            this.mapEvents = new Dictionary<GameLocation, List<MapEvent>>();
            foreach(var v in Game1.locations)
            {
                addLocation(v.Name, false);
            }
        }

        /// <summary>
        /// Adds an event to the map given the name of the map.
        /// </summary>
        /// <param name="mapName"></param>
        /// <param name="mapEvent"></param>
        public virtual void addEvent(string mapName,MapEvent mapEvent)
        {
            foreach(var pair in this.mapEvents)
            {
                if (pair.Key.Name == mapName)
                {
                    pair.Value.Add(mapEvent);
                }
            }
        }

        /// <summary>
        /// Adds an event to a map.
        /// </summary>
        /// <param name="Location"></param>
        /// <param name="mapEvent"></param>
        public virtual void addEvent(GameLocation Location, MapEvent mapEvent)
        {
            foreach (var pair in this.mapEvents)
            {
                if (pair.Key == Location)
                {

                    pair.Value.Add(mapEvent);
                }
            }
        }

        /// <summary>
        /// Adds a location to have events handled.
        /// </summary>
        /// <param name="Location">The location to handle events.</param>
        public virtual void addLocation(GameLocation Location)
        {
            EventSystem.ModMonitor.Log("Adding event processing for location: " + Location.Name);
            this.mapEvents.Add(Location, new List<MapEvent>());
        }

        /// <summary>
        /// Adds a location to have events handled.
        /// </summary>
        /// <param name="Location"></param>
        /// <param name="Events"></param>
        public virtual void addLocation(GameLocation Location,List<MapEvent> Events)
        {
            EventSystem.ModMonitor.Log("Adding event processing for location: " + Location.Name);
            this.mapEvents.Add(Location, Events);
        }

        /// <summary>
        /// Adds a location to handle events.
        /// </summary>
        /// <param name="Location">The name of the location. Can include farm buildings.</param>
        /// <param name="isStructure">Used if the building is a stucture. True=building.</param>
        public virtual void addLocation(string Location,bool isStructure)
        {
            EventSystem.ModMonitor.Log("Adding event processing for location: " + Location);
            this.mapEvents.Add(Game1.getLocationFromName(Location,isStructure), new List<MapEvent>());
        }

        /// <summary>
        /// Adds a location to have events handled.
        /// </summary>
        /// <param name="Location">The name of the location. Can include farm buildings.</param>
        /// <param name="isStructure">Used if the building is a stucture. True=building.</param>
        /// <param name="Events">A list of pre-initialized events.</param>
        public virtual void addLocation(string Location, bool isStructure, List<MapEvent> Events)
        {
            EventSystem.ModMonitor.Log("Adding event processing for location: " + Location);
            this.mapEvents.Add(Game1.getLocationFromName(Location,isStructure), Events);
        }

        /// <summary>
        /// Updates all events associated with the event manager.
        /// </summary>
        public virtual void update()
        {
            List<MapEvent> events = new List<MapEvent>();
            if (Game1.player == null) return;
            if (Game1.hasLoadedGame == false) return;
            bool ok=this.mapEvents.TryGetValue(Game1.player.currentLocation, out events);
            if (ok == false) return;
            else
            {
                foreach(var v in events)
                {
                    v.update();
                }
            }
        }
    }
}
