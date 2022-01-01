/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using TehPers.Core.Api.Setup;
using TehPers.PowerGrid.World;

namespace TehPers.PowerGrid.Services.Setup
{
    internal class NetworkWatcher : ISetup, IDisposable
    {
        private readonly IModHelper helper;
        private readonly IMonitor monitor;

        private readonly Dictionary<GameLocation, List<EnergyNetwork>> networks;

        public NetworkWatcher(IModHelper helper, IMonitor monitor)
        {
            this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
            this.monitor = monitor ?? throw new ArgumentNullException(nameof(monitor));

            this.networks = new();
        }

        public void Setup()
        {
            this.helper.Events.World.LocationListChanged += this.WorldOnLocationListChanged;
            this.helper.Events.World.ObjectListChanged += this.WorldOnObjectListChanged;

            foreach (var location in Game1.locations)
            {
                this.networks.Add(location, new());
            }
        }

        public void Dispose()
        {
            this.helper.Events.World.LocationListChanged -= this.WorldOnLocationListChanged;
            this.helper.Events.World.ObjectListChanged -= this.WorldOnObjectListChanged;

            this.networks.Clear();
        }

        private void WorldOnLocationListChanged(object? sender, LocationListChangedEventArgs e)
        {
            // TODO: search for all networks
        }

        private void WorldOnObjectListChanged(object? sender, ObjectListChangedEventArgs e)
        {
            if (!this.networks.TryGetValue(e.Location, out var networks))
            {
                networks = new();
                this.networks.Add(e.Location, networks);
            }

            foreach (var (position, obj) in e.Removed)
            {
                if (obj is not IEnergyConductor conductor)
                {
                    continue;
                }

                // Find the network the object used to be a part of (if any) and remove it
                var network = networks.FirstOrDefault(network => network.Contains(conductor));
                if (network is null)
                {
                    // This should not happen, but conductor was already not in a network.
                    continue;
                }

                network.Remove(conductor);

                // Split the network if needed
            }

            foreach (var (position, obj) in e.Added)
            {
                if (obj is not IEnergyConductor conductor)
                {
                    continue;
                }

                // TODO: find the network the object is now part of (if any) and add it
                // TODO: if no network is found, create one
                // TODO: if two or more networks are found, merge them
            }
        }
    }
}