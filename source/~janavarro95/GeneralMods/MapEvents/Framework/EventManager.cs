using System.Collections.Generic;
using StardewValley;

namespace EventSystem.Framework
{
    public class EventManager
    {
        public Dictionary<GameLocation, List<MapEvent>> mapEvents;

        /// <summary>Construct an instance.</summary>
        public EventManager()
        {
            this.mapEvents = new Dictionary<GameLocation, List<MapEvent>>();
            foreach (var v in Game1.locations)
                this.addLocation(v.Name, false);
        }

        /// <summary>Adds an event to the map given the name of the map.</summary>
        public virtual void addEvent(string mapName, MapEvent mapEvent)
        {
            foreach (var pair in this.mapEvents)
            {
                if (pair.Key.Name == mapName)
                    pair.Value.Add(mapEvent);
            }
        }

        /// <summary>Adds an event to a map.</summary>
        public virtual void addEvent(GameLocation location, MapEvent mapEvent)
        {
            foreach (var pair in this.mapEvents)
            {
                if (pair.Key == location)
                    pair.Value.Add(mapEvent);
            }
        }

        /// <summary>Adds a location to have events handled.</summary>
        /// <param name="location">The location to handle events.</param>
        public virtual void addLocation(GameLocation location)
        {
            EventSystem.ModMonitor.Log($"Adding event processing for location: {location.Name}");
            this.mapEvents.Add(location, new List<MapEvent>());
        }

        /// <summary>Adds a location to have events handled.</summary>
        public virtual void addLocation(GameLocation location, List<MapEvent> events)
        {
            EventSystem.ModMonitor.Log($"Adding event processing for location: {location.Name}");
            this.mapEvents.Add(location, events);
        }

        /// <summary>Adds a location to handle events.</summary>
        /// <param name="location">The name of the location. Can include farm buildings.</param>
        /// <param name="isStructure">Used if the building is a stucture. True=building.</param>
        public virtual void addLocation(string location, bool isStructure)
        {
            EventSystem.ModMonitor.Log($"Adding event processing for location: {location}");
            this.mapEvents.Add(Game1.getLocationFromName(location, isStructure), new List<MapEvent>());
        }

        /// <summary>Adds a location to have events handled.</summary>
        /// <param name="location">The name of the location. Can include farm buildings.</param>
        /// <param name="isStructure">Used if the building is a stucture. True=building.</param>
        /// <param name="events">A list of pre-initialized events.</param>
        public virtual void addLocation(string location, bool isStructure, List<MapEvent> events)
        {
            EventSystem.ModMonitor.Log($"Adding event processing for location: {location}");
            this.mapEvents.Add(Game1.getLocationFromName(location, isStructure), events);
        }

        /// <summary>Updates all events associated with the event manager.</summary>
        public virtual void update()
        {
            if (Game1.player == null)
                return;
            if (!Game1.hasLoadedGame)
                return;

            if (this.mapEvents.TryGetValue(Game1.player.currentLocation, out List<MapEvent> events))
            {
                foreach (var v in events)
                    v.update();
            }
        }
    }
}
