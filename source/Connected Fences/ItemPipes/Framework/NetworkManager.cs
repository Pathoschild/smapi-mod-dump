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
                network.Update();
            }
        }

        public static void LoadNodeToNetwork(Vector2 postition, GameLocation location, Network network)
        {
            DataAccess DataAccess = DataAccess.GetDataAccess();
            List<Node> nodes = DataAccess.LocationNodes[location];
            Node node = nodes.Find(n => n.Position.Equals(postition));
            network.AddNode(node);
        }

        public static void AddNewElement(Node newNode, Network network)
        {
            DataAccess DataAccess = DataAccess.GetDataAccess();
            List<Node> nodes = DataAccess.LocationNodes[Game1.currentLocation];
            newNode.ParentNetwork = network;
            LoadNodeToNetwork(newNode.Position, newNode.Location, network);
        }

        public static void AddObject(KeyValuePair<Vector2, StardewValley.Object> obj)
        {
            DataAccess DataAccess = DataAccess.GetDataAccess();
            if (Globals.UltraDebug) { Printer.Info("Adding new object: " + obj.Key.ToString() + obj.Value.Name); }
            if (obj.Value is PipeItem)
            {
                List<Node> nodes = DataAccess.LocationNodes[Game1.currentLocation];
                Node newNode = NodeFactory.CreateElement(obj.Key, Game1.currentLocation, obj.Value);
                int x = (int)newNode.Position.X;
                int y = (int)newNode.Position.Y;
                nodes.Add(newNode);
                Vector2 north = new Vector2(x, y - 1);
                Node northNode = nodes.Find(n => n.Position.Equals(north));
                if (northNode != null)
                {
                    if(northNode is not ContainerNode)
                    {
                        newNode.AddAdjacent(SideStruct.GetSides().North, northNode);
                    }
                    else if(northNode is ContainerNode && newNode is IOPipeNode)
                    {
                        IOPipeNode IOPipeNode = (IOPipeNode)newNode;
                        IOPipeNode.AddConnectedContainer(northNode);
                    }
                }
                Vector2 south = new Vector2(x, y + 1);
                Node southNode = nodes.Find(n => n.Position.Equals(south));
                if (southNode != null)
                {
                    if (southNode is not ContainerNode)
                    {
                        newNode.AddAdjacent(SideStruct.GetSides().South, southNode);
                    }
                    else if (southNode is ContainerNode && newNode is IOPipeNode)
                    {
                        IOPipeNode IOPipeNode = (IOPipeNode)newNode;
                        IOPipeNode.AddConnectedContainer(southNode);
                    }
                }
                Vector2 east = new Vector2(x + 1, y);
                Node eastNode = nodes.Find(n => n.Position.Equals(east));
                if (eastNode != null)
                {
                    if (eastNode is not ContainerNode)
                    {
                        newNode.AddAdjacent(SideStruct.GetSides().East, eastNode);
                    }
                    else if (eastNode is ContainerNode && newNode is IOPipeNode)
                    {
                        IOPipeNode IOPipeNode = (IOPipeNode)newNode;
                        IOPipeNode.AddConnectedContainer(eastNode);
                    }
                }
                Vector2 west = new Vector2(x - 1, y);
                Node westNode = nodes.Find(n => n.Position.Equals(west));
                if (westNode != null)
                {
                    if (westNode is not ContainerNode)
                    {
                        newNode.AddAdjacent(SideStruct.GetSides().West, westNode);
                    }
                    else if (westNode is ContainerNode && newNode is IOPipeNode)
                    {
                        IOPipeNode IOPipeNode = (IOPipeNode)newNode;
                        IOPipeNode.AddConnectedContainer(westNode);
                    }
                }
                if (Globals.UltraDebug) { newNode.Print(); }
                if (DataAccess.NetworkItems.Contains(Game1.currentLocation.getObjectAtTile(x, y).Name))
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
                        Network network = CreateLocationNetwork(Game1.currentLocation);
                        AddNewElement(newNode, network);
                    }
                    else
                    {
                        List<Network> orderedAdjNetworks = adjNetworks.OrderByDescending(s => s.Nodes.Count).ToList();
                        newNode.ParentNetwork = orderedAdjNetworks[0];
                        AddNewElement(newNode, orderedAdjNetworks[0]);
                        MergeNetworks(orderedAdjNetworks);
                    }
                }
            }
            else if (obj.Value is Chest)
            {
                List<Node> nodes = DataAccess.LocationNodes[Game1.currentLocation];
                Node newNode = NodeFactory.CreateElement(obj.Key, Game1.currentLocation, obj.Value);
                int x = (int)newNode.Position.X;
                int y = (int)newNode.Position.Y;
                nodes.Add(newNode);
                Vector2 north = new Vector2(x, y - 1);
                Node northNode = nodes.Find(n => n.Position.Equals(north));
                if (northNode != null && northNode is IOPipeNode)
                {
                    IOPipeNode northIOPipeNode = (IOPipeNode)northNode;
                    northIOPipeNode.AddConnectedContainer(newNode);
                }
                Vector2 south = new Vector2(x, y + 1);
                Node southNode = nodes.Find(n => n.Position.Equals(south));
                if (southNode != null && southNode is IOPipeNode)
                {
                    IOPipeNode southIOPipeNode = (IOPipeNode)southNode;
                    southIOPipeNode.AddConnectedContainer(newNode);
                }
                Vector2 west = new Vector2(x + 1, y);
                Node westNode = nodes.Find(n => n.Position.Equals(west));
                if (westNode != null && westNode is IOPipeNode)
                {
                    IOPipeNode westIOPipeNode = (IOPipeNode)westNode;
                    westIOPipeNode.AddConnectedContainer(newNode);
                }
                Vector2 east = new Vector2(x - 1, y);
                Node eastNode = nodes.Find(n => n.Position.Equals(east));
                if (eastNode != null && eastNode is IOPipeNode)
                {
                    IOPipeNode eastIOPipeNode = (IOPipeNode)eastNode;
                    eastIOPipeNode.AddConnectedContainer(newNode);
                }
            }

        }

        private static void MergeNetworks(List<Network> network)
        {
            if (Globals.UltraDebug) { Printer.Info("Merging networks... "); }
            DataAccess DataAccess = DataAccess.GetDataAccess();
            for (int i = 1; i < network.Count; i++)
            {
                if (Globals.UltraDebug) { Printer.Info($"Network [{network[i].ID}] size: " + network[i].Nodes.Count.ToString()); }
                foreach (Node elem in network[i].Nodes.ToList())
                {
                    elem.ParentNetwork = network[0];
                    LoadNodeToNetwork(elem.Position, Game1.currentLocation, network[0]);
                }
                DataAccess.LocationNetworks[Game1.currentLocation].Remove(network[i]);
            }
        }

        public static void RemoveObject(KeyValuePair<Vector2, StardewValley.Object> obj)
        {
            DataAccess DataAccess = DataAccess.GetDataAccess();
            if (Globals.UltraDebug) { Printer.Info("Removing object: " + obj.Key.ToString() + obj.Value.Name); }
            if (obj.Value is PipeItem)
            {
                List<Node> nodes = DataAccess.LocationNodes[Game1.currentLocation];
                Node node = nodes.Find(n => n.Position.Equals(obj.Key));
                nodes.Remove(node); //Doesn't update
                if (node is IOPipeNode)
                {
                    IOPipeNode IOPipeNode = (IOPipeNode)node;
                    if(IOPipeNode.ConnectedContainer != null)
                    {
                        IOPipeNode.ConnectedContainer.RemoveIOPipe(IOPipeNode);
                    }
                }
                if (DataAccess.NetworkItems.Contains(obj.Value.Name))
                {
                    if (node.ParentNetwork != null)
                    {
                        List<Network> adjNetworks = node.Scan();
                        node.ParentNetwork.RemoveNode(node);
                        DataAccess.LocationNetworks[Game1.currentLocation].Remove(node.ParentNetwork);
                        if (adjNetworks.Count > 0)
                        {
                            RemakeNetwork(node);
                        }
                    }
                node.RemoveAllAdjacents();
                }
            }
            else if (obj.Value is Chest)
            {
                List<Node> nodes = DataAccess.LocationNodes[Game1.currentLocation];
                Node node = nodes.Find(n => n.Position.Equals(obj.Key));
                int x = (int)node.Position.X;
                int y = (int)node.Position.Y;
                Vector2 north = new Vector2(x, y - 1);
                Node northNode = nodes.Find(n => n.Position.Equals(north));
                if (northNode != null && northNode is IOPipeNode)
                {
                    IOPipeNode northIOPipeNode = (IOPipeNode)northNode;
                    northIOPipeNode.RemoveConnectedContainer(node);
                }
                Vector2 south = new Vector2(x, y + 1);
                Node southNode = nodes.Find(n => n.Position.Equals(south));
                if (southNode != null)
                {
                    IOPipeNode southIOPipeNode = (IOPipeNode)southNode;
                    southIOPipeNode.RemoveConnectedContainer(node);
                }
                Vector2 west = new Vector2(x + 1, y);
                Node westNode = nodes.Find(n => n.Position.Equals(west));
                if (westNode != null)
                {
                    IOPipeNode westIOPipeNode = (IOPipeNode)westNode;
                    westIOPipeNode.RemoveConnectedContainer(node);
                }
                Vector2 east = new Vector2(x - 1, y);
                Node eastNode = nodes.Find(n => n.Position.Equals(east));
                if (eastNode != null)
                {
                    IOPipeNode eastIOPipeNode = (IOPipeNode)eastNode;
                    eastIOPipeNode.RemoveConnectedContainer(node);
                }
            }
        }

        public static void RemakeNetwork(Node node)
        {
            DataAccess DataAccess = DataAccess.GetDataAccess();
            if (Globals.UltraDebug) { Printer.Info("Remaking networks..."); }
            List<Network> networkList;
            foreach (KeyValuePair<Side, Node> adj in node.Adjacents)
            {
                if (adj.Value != null)
                {
                    DataAccess.LocationNodes[Game1.currentLocation].Remove(node);
                    if (DataAccess.NetworkItems.Contains(adj.Value.Name))
                    {
                        DataAccess.LocationNetworks[Game1.currentLocation].Remove(adj.Value.ParentNetwork);
                        if (adj.Value.ParentNetwork != null)
                        {
                            adj.Value.ParentNetwork.Delete();
                        }
                        Node newNode = NetworkBuilder.BuildNetworkRecursive(adj.Value.Position, Game1.currentLocation, null);
                    }
                }
            }
            networkList = DataAccess.LocationNetworks[Game1.currentLocation];
            if (Globals.UltraDebug) 
            { 
                Printer.Info("NUMBER OF GRAPGHS: " + networkList.Count.ToString()); 
                foreach (Network network in networkList)
                {
                    Printer.Info(network.Print());
                }
            }
        }

        public static Network CreateLocationNetwork(GameLocation location)
        {
            DataAccess DataAccess = DataAccess.GetDataAccess();
            Network newNetwork = new Network(DataAccess.GetNewNetworkID());
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
                Printer.Info(network.Print());
            }
        }
    }
}
