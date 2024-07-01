/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using DeluxeJournal.Events;

namespace DeluxeJournal.Framework.Events
{
    internal class BuildingConstructedEvent : ManagedNetEvent<BuildingConstructedEventArgs, BuildingConstructedEvent.EventMessage>
    {
        public class EventMessage
        {
            /// <summary>Building tile X position.</summary>
            public int TileX { get; set; }

            /// <summary>Building tile Y position.</summary>
            public int TileY { get; set; }

            /// <summary>Building location name.</summary>
            public string LocationName { get; set; } = string.Empty;

            /// <summary>Value of <see cref="BuildingConstructedEventArgs.IsUpgrade"/></summary>
            public bool IsUpgrade { get; set; }
        }

        public BuildingConstructedEvent(string name, IMultiplayerHelper multiplayer)
            : base(name, multiplayer)
        {
        }

        protected override EventMessage EventArgsToMessage(BuildingConstructedEventArgs args)
        {
            return new EventMessage()
            {
                TileX = args.Building.tileX.Value,
                TileY = args.Building.tileY.Value,
                LocationName = args.Location.NameOrUniqueName,
                IsUpgrade = args.IsUpgrade
            };
        }

        protected override BuildingConstructedEventArgs MessageToEventArgs(EventMessage message)
        {
            Vector2 tile = new Vector2(message.TileX, message.TileY);

            if (Game1.getLocationFromName(message.LocationName) is not GameLocation location)
            {
                throw new InvalidOperationException(string.Format("No GameLocation with name '{0}'.", message.LocationName));
            }
            
            if (location.getBuildingAt(tile) is not Building building)
            {
                throw new InvalidOperationException(string.Format("No building found at location '{0}' on tile ({1},{2}).", message.LocationName, tile.X, tile.Y));
            }

            return new BuildingConstructedEventArgs(location, building, message.IsUpgrade);
        }
    }
}
