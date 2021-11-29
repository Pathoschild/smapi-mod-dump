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
using ItemPipes.Framework.Objects;
using StardewValley;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using Microsoft.Xna.Framework;

namespace ItemPipes.Framework
{
    public static class NetworkManager
    {
        public static void UpdateLocationNetworks(GameLocation location)
        {
            DataAccess DataAccess = DataAccess.GetDataAccess();
            List<Network> networkList;
            if (DataAccess.LocationNetworks.TryGetValue(location, out networkList))
            {
                foreach (Network network in networkList)
                {
                    network.Update();
                }
            }
        }


        public static void LoadNodeToNetwork(GameLocation location, int x, int y, Network network)
        {
            DataAccess DataAccess = DataAccess.GetDataAccess();
            Node[,] matrix;
            if (DataAccess.LocationMatrix.TryGetValue(location, out matrix))
            {
                network.AddNode(matrix[x, y]);
                if (matrix[x, y] is ExtractorPipe)
                {
                    network.AddOutput((ExtractorPipe)matrix[x, y]);
                }
                else if (matrix[x, y] is InserterPipe)
                {
                    network.AddInput((InserterPipe)matrix[x, y]);
                }
                else if (matrix[x, y] is PolymorphicPipe)
                {
                    network.AddInput((PolymorphicPipe)matrix[x, y]);
                }
                else if (matrix[x, y] is FilterPipe)
                {
                    network.AddInput((FilterPipe)matrix[x, y]);
                }
                else if (matrix[x, y] is ConnectorPipe)
                {
                    network.AddConnector((ConnectorPipe)matrix[x, y]);
                }
            }
        }

        public static void AddNewElement(Node newNode, Network network)
        {
            DataAccess DataAccess = DataAccess.GetDataAccess();
            Node[,] matrix;
            if (DataAccess.LocationMatrix.TryGetValue(Game1.currentLocation, out matrix))
            {
                newNode.ParentNetwork = network;
                matrix[(int)newNode.Position.X, (int)newNode.Position.Y] = newNode;
                LoadNodeToNetwork(newNode.Location, (int)newNode.Position.X, (int)newNode.Position.Y, network);
            }
        }



        public static void AddObject(KeyValuePair<Vector2, StardewValley.Object> obj)
        {
            DataAccess DataAccess = DataAccess.GetDataAccess();
            if (Globals.Debug) { Printer.Info("ADDING: " + obj.Key.ToString() + obj.Value.Name); }
            if (DataAccess.ModItems.Contains(obj.Value.Name))
            {
                Node[,] matrix;
                if (DataAccess.LocationMatrix.TryGetValue(Game1.currentLocation, out matrix))
                {
                    Node newNode = NodeFactory.CreateElement(obj.Key, Game1.currentLocation, obj.Value);
                    int x = (int)newNode.Position.X;
                    int y = (int)newNode.Position.Y;
                    matrix[x, y] = newNode;
                    if (matrix[x, y - 1] != null)
                    {
                        newNode.AddAdjacent(SideStruct.GetSides().North, matrix[x, y - 1]);
                    }
                    if (matrix[x, y + 1] != null)
                    {
                        newNode.AddAdjacent(SideStruct.GetSides().South, matrix[x, y + 1]);
                    }
                    if (matrix[x + 1, y] != null)
                    {
                        newNode.AddAdjacent(SideStruct.GetSides().West, matrix[x + 1, y]);
                    }
                    if (matrix[x - 1, y] != null)
                    {
                        newNode.AddAdjacent(SideStruct.GetSides().East, matrix[x - 1, y]);
                    }
                    if (Globals.Debug) { newNode.Print(); }
                    if (DataAccess.NetworkItems.Contains(Game1.currentLocation.getObjectAtTile(x, y).Name))
                    {
                        if (Globals.Debug) { Printer.Info("ADDING GRAPH"); }
                        if (newNode is Input)
                        {
                            Input input = (Input)newNode;
                        }
                        List<Network> uncheckedAdjNetworks= newNode.Scan();
                        List<Network> adjNetworks = new List<Network>();
                        foreach(Network network in uncheckedAdjNetworks)
                        {
                            if(network != null)
                            {
                                adjNetworks.Add(network);
                            }
                        }
                        if (Globals.Debug) { Printer.Info("Adj graphs: " + adjNetworks.Count.ToString()); }
                        if (adjNetworks.Count == 0)
                        {
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
            }
        }

        private static void MergeNetworks(List<Network> network)
        {
            DataAccess DataAccess = DataAccess.GetDataAccess();

            if (Globals.Debug) { Printer.Info(network.Count.ToString()); }
            for (int i = 1; i < network.Count; i++)
            {
                if (Globals.Debug) { Printer.Info("G size:" + network[i].Nodes.Count.ToString()); }
                foreach (Node elem in network[i].Nodes)
                {
                    elem.ParentNetwork = network[0];
                    LoadNodeToNetwork(Game1.currentLocation, (int)elem.Position.X, (int)elem.Position.Y, network[0]);
                
                }
                List<Network> networkList;
                if (DataAccess.LocationNetworks.TryGetValue(Game1.currentLocation, out networkList))
                {
                    networkList.Remove(network[i]);
                }
            }
        }

        public static void RemoveObject(KeyValuePair<Vector2, StardewValley.Object> obj)
        {
            DataAccess DataAccess = DataAccess.GetDataAccess();
            if (Globals.Debug) { Printer.Info("REMOVE: " + obj.Key.ToString() + obj.Value.Name); }
            if (DataAccess.ModItems.Contains(obj.Value.Name))
            {
                Node[,] matrix;
                if (DataAccess.LocationMatrix.TryGetValue(Game1.currentLocation, out matrix))
                {
                    Node node = matrix[(int)obj.Key.X, (int)obj.Key.Y];
                    matrix[(int)node.Position.X, (int)node.Position.Y] = null;
                    if (DataAccess.NetworkItems.Contains(obj.Value.Name))
                    {
                        if (node.ParentNetwork != null)
                        {
                            List<Network> adjNetwork = node.Scan();
                            node.ParentNetwork.RemoveNode(node);
                            List<Network> networkList;
                            if (DataAccess.LocationNetworks.TryGetValue(Game1.currentLocation, out networkList))
                            {
                                networkList.Remove(node.ParentNetwork);
                            }
                            if (adjNetwork.Count > 0)
                            {
                                RemakeNetwork(node);
                            }
                        }
                    }
                    node.RemoveAllAdjacents();
                }
            }
        }

        public static void RemakeNetwork(Node node)
        {
            DataAccess DataAccess = DataAccess.GetDataAccess();
            if (Globals.Debug) { Printer.Info("Remaking"); }
            List<Network> networkList;
            foreach (KeyValuePair<Side, Node> adj in node.Adjacents)
            {
                if (adj.Value != null)
                {
                    if (DataAccess.NetworkItems.Contains(adj.Value.Name))
                    {
                        if (DataAccess.LocationNetworks.TryGetValue(Game1.currentLocation, out networkList))
                        {
                            networkList.Remove(adj.Value.ParentNetwork);
                        }
                        if (adj.Value.ParentNetwork != null)
                        {
                            adj.Value.ParentNetwork.Delete();
                        }
                        Node newNode = NetworkBuilder.BuildNetworkRecursive(Game1.currentLocation, null, (int)adj.Value.Position.X, (int)adj.Value.Position.Y);
                    }
                }
            }

            if (DataAccess.LocationNetworks.TryGetValue(Game1.currentLocation, out networkList))
            {
                if (Globals.Debug) 
                { 
                    Printer.Info("NUMBER OF GRAPGHS: " + networkList.Count.ToString()); 
                    foreach (Network network in networkList)
                    {
                        Printer.Info(network.Print());
                    }
                }
            }
        }

        public static Network CreateLocationNetwork(GameLocation location)
        {
            DataAccess DataAccess = DataAccess.GetDataAccess();
            Network newNetwork = new Network(DataAccess.GetNewNetworkID());
            List<Network> networkList;
            if (DataAccess.LocationNetworks.TryGetValue(location, out networkList))
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
            List<Network> networkList;
            if (DataAccess.LocationNetworks.TryGetValue(location, out networkList))
            {
                if (Globals.Debug) { Printer.Info($"NUMBER OF GROUPS: {networkList.Count}"); }
                foreach (Network network in networkList)
                {
                    Printer.Info(network.Print());
                }
            }
        }
    }
}
