/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sergiomadd/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItemPipes.Framework.Model;
using ItemPipes.Framework.Nodes;
using StardewValley;
using StardewValley.Objects;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using Microsoft.Xna.Framework;
using ItemPipes.Framework.Util;
using ItemPipes.Framework.Factories;
using ItemPipes.Framework.Items;
using ItemPipes.Framework.Nodes.ObjectNodes;


namespace ItemPipes.Framework
{
    public static class NetworkManager
    {
        public static void UpdateLocationNetworks(GameLocation location)
        {
            DataAccess DataAccess = DataAccess.GetDataAccess();
            List<Network> networkList = DataAccess.LocationNetworks[location];
            foreach (Network network in networkList)
            {
                if(network != null)
                {
                    network.Update();
                }
            }
        }

        public static bool LoadNodeToNetwork(Vector2 postition, GameLocation location, Network network)
        {
            DataAccess DataAccess = DataAccess.GetDataAccess();
            List<Node> nodes = DataAccess.LocationNodes[location];
            Node node = nodes.Find(n => n.Position.Equals(postition));
            bool added = network.AddNode(node);
            return added;
        }

        public static void AddNewElement(Node newNode, Network network)
        {
            DataAccess DataAccess = DataAccess.GetDataAccess();
            newNode.ParentNetwork = network;
            LoadNodeToNetwork(newNode.Position, newNode.Location, network);
        }

        public static void AddObject(KeyValuePair<Vector2, StardewValley.Object> obj, GameLocation location)
        {
            DataAccess DataAccess = DataAccess.GetDataAccess();
            if (Globals.UltraDebug) { Printer.Info("Adding new object: " + obj.Key.ToString() + obj.Value.Name); }

            List<Node> nodes = DataAccess.LocationNodes[location];
            Node newNode = NodeFactory.CreateElement(obj.Key, location, obj.Value);
            if (Globals.UltraDebug) { Printer.Info("New node created: " + newNode.Print()); }
            int x = (int)newNode.Position.X;
            int y = (int)newNode.Position.Y;

            nodes.Add(newNode);
            Vector2 north = new Vector2(x, y - 1);
            Node northNode = nodes.Find(n => n.Position.Equals(north));
            if (northNode != null)
            {
                newNode.AddAdjacent(SideStruct.GetSides().North, northNode);
            }
            Vector2 south = new Vector2(x, y + 1);
            Node southNode = nodes.Find(n => n.Position.Equals(south));
            if (southNode != null)
            {
                newNode.AddAdjacent(SideStruct.GetSides().South, southNode);
            }
            Vector2 east = new Vector2(x + 1, y);
            Node eastNode = nodes.Find(n => n.Position.Equals(east));
            if (eastNode != null)
            {
                newNode.AddAdjacent(SideStruct.GetSides().East, eastNode);
            }
            Vector2 west = new Vector2(x - 1, y);
            Node westNode = nodes.Find(n => n.Position.Equals(west));
            if (westNode != null)
            {
                newNode.AddAdjacent(SideStruct.GetSides().West, westNode);
            }
            if (Globals.UltraDebug) { newNode.Print(); }

            if (obj.Value is CustomObjectItem)
            {
                if (Globals.UltraDebug) { Printer.Info("Assigning network to new node"); }
                List<Network> uncheckedAdjNetworks = newNode.Scan();
                List<Network> adjNetworks = new List<Network>();
                foreach (Network network in uncheckedAdjNetworks)
                {
                    if (network != null)
                    {
                        adjNetworks.Add(network);
                    }
                }
                if (Globals.UltraDebug) { Printer.Info("Adjacent network amount: " + adjNetworks.Count.ToString()); }
                if (adjNetworks.Count == 0)
                {
                    if (Globals.UltraDebug) { Printer.Info("No adjacent networks, creating new one... "); }
                    Network network = CreateLocationNetwork(location);
                    AddNewElement(newNode, network);
                }
                else
                {
                    List<Network> orderedAdjNetworks = adjNetworks.OrderByDescending(s => s.Nodes.Count).ToList();
                    if (Globals.UltraDebug) { Printer.Info($"Biggest network = {orderedAdjNetworks[0].ID}"); }
                    foreach(Network network in orderedAdjNetworks)
                    {
                        if (Globals.UltraDebug) { Printer.Info(network.Print()); }
                    }
                    newNode.ParentNetwork = orderedAdjNetworks[0];
                    AddNewElement(newNode, orderedAdjNetworks[0]);
                    MergeNetworks(orderedAdjNetworks, location);
                }
                if (Globals.UltraDebug){Printer.Info($"Assigned network: [N{newNode.ParentNetwork.ID}]");}
                //Another check for missmatching networks
                north = new Vector2(x, y - 1);
                northNode = nodes.Find(n => n.Position.Equals(north));
                if (northNode != null)
                {
                    newNode.AddAdjacent(SideStruct.GetSides().North, northNode);
                }
                south = new Vector2(x, y + 1);
                southNode = nodes.Find(n => n.Position.Equals(south));
                if (southNode != null)
                {
                    newNode.AddAdjacent(SideStruct.GetSides().South, southNode);
                }
                east = new Vector2(x + 1, y);
                eastNode = nodes.Find(n => n.Position.Equals(east));
                if (eastNode != null)
                {
                    newNode.AddAdjacent(SideStruct.GetSides().East, eastNode);
                }
                west = new Vector2(x - 1, y);
                westNode = nodes.Find(n => n.Position.Equals(west));
                if (westNode != null)
                {
                    newNode.AddAdjacent(SideStruct.GetSides().West, westNode);
                }
            }
            Node node = nodes.Find(n => n.Position.Equals(obj.Key));
            List<Network> networks = DataAccess.LocationNetworks[node.Location];
            if (node.ParentNetwork != null && !networks.Contains(node.ParentNetwork))
            {
                networks.Add(node.ParentNetwork);
            }
            foreach(KeyValuePair<Side, Node> pair in node.Adjacents)
            {
                if(pair.Value is PPMNode)
                {
                    PPMNode invisibilizerNode = (PPMNode)pair.Value;
                    invisibilizerNode.AdjNetworks.Add(node.ParentNetwork);
                    newNode.ParentNetwork.AddNode(invisibilizerNode);
                }
            }
        }

        private static void MergeNetworks(List<Network> network, GameLocation location)
        {
            if (Globals.UltraDebug) { Printer.Info("Merging networks... "); }
            DataAccess DataAccess = DataAccess.GetDataAccess();
            for (int i = 1; i < network.Count; i++)
            {
                if (Globals.UltraDebug) { Printer.Info($"Network [{network[i].ID}] size: " + network[i].Nodes.Count.ToString()); }
                foreach (Node elem in network[i].Nodes.ToList())
                {
                    elem.ParentNetwork = network[0];
                    LoadNodeToNetwork(elem.Position, location, network[0]);
                }
                DataAccess.LocationNetworks[location].Remove(network[i]);
            }
        }

        public static void RemoveObject(KeyValuePair<Vector2, StardewValley.Object> obj, GameLocation location)
        {
            DataAccess DataAccess = DataAccess.GetDataAccess();
            if (Globals.UltraDebug) { Printer.Info("Removing object: " + obj.Key.ToString() + obj.Value.Name); }
            List<Node> nodes = DataAccess.LocationNodes[location];
            Node node = nodes.Find(n => n.Position.Equals(obj.Key));
            if(node != null)
            {
                nodes.Remove(node);

                if (node is IOPipeNode)
                {
                    IOPipeNode IOPipeNode = (IOPipeNode)node;
                    if (IOPipeNode.ConnectedContainer != null)
                    {
                        IOPipeNode.ConnectedContainer.RemoveIOPipe(IOPipeNode);
                    }
                }
                Printer.Info((obj.Value is CustomObjectItem).ToString());
                if (obj.Value is CustomObjectItem)
                {
                    if (node.ParentNetwork != null)
                    {
                        List<Network> adjNetworks = node.Scan();
                        node.ParentNetwork.RemoveNode(node);
                        if (adjNetworks.Count > 0)
                        {
                            RemakeNetwork(node, location);
                        }
                    }
                }
                node.RemoveAllAdjacents();
            }
        }

        public static void RemakeNetwork(Node node, GameLocation location)
        {
            DataAccess DataAccess = DataAccess.GetDataAccess();
            if (Globals.UltraDebug) { Printer.Info("Remaking networks..."); }
            List<Node> dict = node.Adjacents.Values.ToList();
            node.ParentNetwork.RemoveAllAdjacents();
            node.ParentNetwork.Delete();
            node.ParentNetwork = null;
            foreach (Node adj in dict)
            {
                if (adj != null)
                {
                    DataAccess.LocationNodes[location].Remove(node);
                    if (DataAccess.NetworkItems.Contains(adj.ID))
                    {
                        if (adj.ParentNetwork != null)
                        {
                            if(DataAccess.LocationNetworks[location].Contains(adj.ParentNetwork))
                            {
                                Printer.Info($"Contains network {adj.ParentNetwork.ID}");
                                /*Printer.Info($"{DataAccess.LocationNetworks[Game1.currentLocation].Count}");
                                DataAccess.LocationNetworks[Game1.currentLocation].Remove(adj.ParentNetwork);
                                Printer.Info("Deleted network: " + adj.ParentNetwork.ID);
                                Printer.Info($"{DataAccess.LocationNetworks[Game1.currentLocation].Count}");
                                adj.ParentNetwork.Delete();
                                adj.ParentNetwork = null;*/
                            }
                            else
                            {
                                Printer.Info($"Does not contain network {adj.ParentNetwork.ID}");
                                Printer.Info(adj.ParentNetwork.Print());
                            }

                        }
                        NetworkBuilder.BuildNetworkRecursive(adj.Position, location, null);
                    }
                }
            }
            //PrintLocationNetworks(location);
        }

        public static Network CreateLocationNetwork(GameLocation location)
        {
            DataAccess DataAccess = DataAccess.GetDataAccess();
            Network newNetwork = new Network(DataAccess.GetNewNetworkID(location));
            List<Network> networkList = DataAccess.LocationNetworks[location];
            if(networkList != null)
            {
                if (!networkList.Contains(newNetwork))
                {
                    networkList.Add(newNetwork);
                }
            }
            return newNetwork;
        }

        public static void PrintLocationNetworks(GameLocation location)
        {
            DataAccess DataAccess = DataAccess.GetDataAccess();
            List<Network> networkList = DataAccess.LocationNetworks[location];
            if (Globals.UltraDebug) { Printer.Info($"NUMBER OF GROUPS: {networkList.Count}"); }
            foreach (Network network in networkList)
            {
                if(network != null)
                {
                    Printer.Info(network.Print());
                }
            }
        }
    }
}
