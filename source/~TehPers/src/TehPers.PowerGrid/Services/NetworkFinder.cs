/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using StardewValley;

namespace TehPers.PowerGrid.World
{
    /// <summary>
    /// A helper class for finding the energy networks in a given location.
    /// </summary>
    internal class NetworkFinder
    {
        /// <summary>
        /// Finds all the energy networks in the given location. This is done by finding all the
        /// producers in the location and building a network for each one. The networks are then
        /// merged together if they are connected.
        /// </summary>
        /// <param name="location">The location to search.</param>
        /// <returns>A list of energy networks.</returns>
        public IEnumerable<EnergyNetwork> FindNetworks(GameLocation location)
        {
            // Find all the producers in the location
            var producers = location.Objects.Values.OfType<IEnergyProducer>().ToList();

            // Build a network for each producer
            var networks = new List<EnergyNetwork>();
            foreach (var producer in producers)
            {
                // Use breadth-first search to find all the conductors that are reachable from the producer
                var network = new EnergyNetwork();
                var queue = new Queue<IEnergyConductor>();
                queue.Enqueue(producer);
                while (queue.TryDequeue(out var conductor))
                {
                    // Add the conductor to the network
                    network.Add(conductor);

                    // Add all the conductors that are connected to the conductor to the queue
                    foreach (var neighborPosition in conductor.OutgoingConnections)
                    {
                        // Try to get the conductor from the location
                        if (location.Objects.TryGetValue(neighborPosition, out var neighbor) && neighbor is IEnergyConductor neighborConductor)
                        {
                            // Add the conductor to the queue
                            queue.Enqueue(neighborConductor);
                        }
                    }
                }

                // Merge the network with any other networks that are connected to it
                var overlappingNetworks = networks.Where(otherNetwork => otherNetwork.Overlaps(network)).ToHashSet();
                if (overlappingNetworks.Any())
                {
                    // Remove all the overlapping networks
                    networks.RemoveAll(n => overlappingNetworks.Contains(n));

                    // Merge them into this one
                    foreach (var overlappingNetwork in overlappingNetworks)
                    {
                        network.UnionWith(overlappingNetwork);
                    }
                }

                // Add the network to the list
                networks.Add(network);
            }

            // Return the networks
            return networks;
        }
    }    
}