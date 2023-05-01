/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/FishingTrawler
**
*************************************************/

using FishingTrawler.Framework.Utilities;
using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FishingTrawler.Objects
{
    internal class Trawler
    {
        internal Vector2 _boatPosition;
        internal int _boatDirection;
        internal int _boatOffset;
        internal Event _boatEvent;
        internal int nonBlockingPause;
        internal float _nextBubble;
        internal float _nextSlosh;
        internal bool _boatAnimating;
        internal bool _closeGate;
        internal readonly GameLocation location;

        public Trawler()
        {

        }

        public Trawler(GameLocation location)
        {
            this.location = location;
            _boatPosition = GetStartingPosition();
        }

        internal Vector2 GetStartingPosition()
        {
            if (location is null || location is Beach)
            {
                return new Vector2(80f, 41f) * 64f;
            }
            else
            {
                return new Vector2(3f, 42f) * 64f;
            }
        }

        internal Vector2 GetTrawlerPosition()
        {
            // Enable this line to test moving to the right:
            //_boatOffset++;
            return _boatPosition + new Vector2(_boatOffset, 0f);
        }

        internal void Reset()
        {
            _nextBubble = 0f;
            _boatAnimating = false;
            _boatPosition = GetStartingPosition();
            _boatOffset = 0;
            _boatDirection = 0;
            _closeGate = false;
        }

        internal void TriggerDepartureEvent()
        {
            if (FishingTrawler.murphyNPC != null)
            {
                FishingTrawler.murphyNPC = null;
            }

            // Reset the plank tile for departure
            if (location is Beach)
            {
                int index = location.Map.TileSheets.ToList().FindIndex(t => t.Id == "z_beachPatch");
                if (index != -1)
                {
                    location.setMapTileIndex(87, 40, -1, "Back");
                    location.setMapTileIndex(87, 40, 14, "Back", index);
                }
            }
            else if (location is IslandSouthEast)
            {
                int index = location.Map.TileSheets.ToList().FindIndex(t => t.Id == "z_beachPatch");
                if (index != -1)
                {
                    location.setMapTileIndex(10, 41, -1, "Back");
                    location.setMapTileIndex(10, 41, 14, "Back", index);
                }
            }

            string id = location.currentEvent is null ? "Empty" : location.currentEvent.id.ToString();
            FishingTrawler.monitor.Log($"Starting event for {Game1.player.Name}: {location.currentEvent is null} | {id}", LogLevel.Trace);

            string eventString = "/-1000 -1000/farmer 0 0 0/playMusic none/fade/viewport -5000 -5000/warp farmer -100 -100/locationSpecificCommand despawn_murphy/locationSpecificCommand close_gate/changeMapTile Buildings 87 41 19/changeMapTile Buildings 87 42 24/changeMapTile Buildings 87 43 4/fade/viewport 83 38/locationSpecificCommand non_blocking_pause 1000/playSound furnace/locationSpecificCommand animate_boat_start/locationSpecificCommand non_blocking_pause 1000/locationSpecificCommand boat_depart/fade/viewport -5000 -5000/changeMapTile Buildings 87 41 14/changeMapTile Buildings 87 42 19/changeMapTile Buildings 87 43 24/locationSpecificCommand warp_to_cabin/end warpOut";
            if (location is IslandSouthEast)
            {
                eventString = "/-1000 -1000/farmer 0 0 0/playMusic none/fade/viewport -5000 -5000/warp farmer -100 -100/locationSpecificCommand despawn_murphy/locationSpecificCommand close_gate/changeMapTile Buildings 10 42 19/changeMapTile Buildings 10 43 24/changeMapTile Buildings 10 44 4/fade/viewport 22 39/locationSpecificCommand non_blocking_pause 1000/playSound furnace/locationSpecificCommand animate_boat_start/locationSpecificCommand non_blocking_pause 1000/locationSpecificCommand boat_depart/fade/viewport -5000 -5000/changeMapTile Buildings 10 42 14/changeMapTile Buildings 10 43 19/changeMapTile Buildings 10 44 24/locationSpecificCommand warp_to_cabin/end warpOut";
            }

            if (Context.IsMultiplayer)
            {
                // Force close menu
                if (Game1.player.hasMenuOpen.Value)
                {
                    Game1.activeClickableMenu = null;
                }

                Game1.player.locationBeforeForcedEvent.Value = "Custom_TrawlerCabin";
                Farmer farmerActor = (Game1.player.NetFields.Root as NetRoot<Farmer>).Clone().Value;
                Action performForcedEvent = delegate
                {
                    Game1.warpingForForcedRemoteEvent = true;
                    Game1.player.completelyStopAnimatingOrDoingAction();

                    farmerActor.currentLocation = location;
                    farmerActor.completelyStopAnimatingOrDoingAction();
                    farmerActor.UsingTool = false;
                    farmerActor.Items.Clear();
                    farmerActor.hidden.Value = false;
                    Event @event = new Event(eventString, FishingTrawler.BOAT_DEPART_EVENT_ID, farmerActor);
                    @event.showWorldCharacters = false;
                    @event.showGroundObjects = true;
                    @event.ignoreObjectCollisions = false;
                    Game1.currentLocation.startEvent(@event);
                    Game1.warpingForForcedRemoteEvent = false;
                    string value = Game1.player.locationBeforeForcedEvent.Value;
                    Game1.player.locationBeforeForcedEvent.Value = null;
                    @event.setExitLocation("Custom_TrawlerCabin", 8, 5);
                    Game1.player.locationBeforeForcedEvent.Value = value;
                    Game1.player.orientationBeforeEvent = 0;
                };
                Game1.remoteEventQueue.Add(performForcedEvent);

                return;
            }

            _boatEvent = new Event(eventString, FishingTrawler.BOAT_DEPART_EVENT_ID, Game1.player);
            _boatEvent.showWorldCharacters = false;
            _boatEvent.showGroundObjects = true;
            _boatEvent.ignoreObjectCollisions = false;
            _boatEvent.setExitLocation("Custom_TrawlerCabin", 8, 5);
            Game1.player.locationBeforeForcedEvent.Value = "Custom_TrawlerCabin";

            Event boatEvent = _boatEvent;
            boatEvent.onEventFinished = (Action)Delegate.Combine(boatEvent.onEventFinished, new Action(OnBoatEventEnd));
            location.currentEvent = _boatEvent;
            _boatEvent.checkForNextCommand(location, Game1.currentGameTime);

            Game1.eventUp = true;
        }

        internal void StartDeparture(Farmer who)
        {
            List<Farmer> farmersToDepart = GetFarmersToDepart();

            FishingTrawler.mainDeckhand = who;
            FishingTrawler.numberOfDeckhands = farmersToDepart.Count();
            FishingTrawler.monitor.Log($"There are {farmersToDepart.Count()} farm hands departing!", LogLevel.Trace);

            location.modData[ModDataKeys.MURPHY_ON_TRIP] = "true";

            TriggerDepartureEvent();

            if (Context.IsMultiplayer)
            {
                // Send out trigger event to relevant players
                FishingTrawler.AlertPlayersOfDeparture(who.UniqueMultiplayerID, farmersToDepart);
            }
        }

        internal void OnBoatEventEnd()
        {
            if (_boatEvent == null)
            {
                return;
            }
            foreach (NPC actor in _boatEvent.actors)
            {
                actor.shouldShadowBeOffset = false;
                actor.drawOffset.X = 0f;
            }
            foreach (Farmer farmerActor in _boatEvent.farmerActors)
            {
                farmerActor.shouldShadowBeOffset = false;
                farmerActor.drawOffset.X = 0f;
            }

            // Reset the plank tile for departure
            if (Game1.currentLocation is Beach)
            {
                Game1.currentLocation.setMapTileIndex(87, 40, -1, "Back");
                Game1.currentLocation.setMapTileIndex(87, 40, 504, "Back", 1);
            }
            else if (Game1.currentLocation is IslandSouthEast)
            {
                int index = location.Map.TileSheets.ToList().FindIndex(t => t.Id == "z_beachPatch");
                if (index != -1)
                {
                    Game1.currentLocation.setMapTileIndex(10, 41, -1, "Back");
                    Game1.currentLocation.setMapTileIndex(10, 41, 18, "Back", index);
                }
            }

            Reset();
            _boatEvent = null;
        }

        internal List<Farmer> GetFarmersToDepart()
        {
            Rectangle zoneOfDeparture = new Rectangle(82, 26, 10, 16);
            if (location is IslandSouthEast)
            {
                zoneOfDeparture = new Rectangle(5, 31, 10, 16);
            }
            return location.farmers.Where(f => zoneOfDeparture.Contains(f.getTileX(), f.getTileY()) && !FishingTrawler.HasFarmerGoneSailing(f)).ToList();
        }
    }
}
