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
using StardewValley;
using StardewValley.Objects;
using StardewValley.Buildings;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using Microsoft.Xna.Framework;
using ItemPipes.Framework.Util;
using ItemPipes.Framework.Factories;

namespace ItemPipes.Framework
{
    public static class NetworkBuilder
    {
        public static void BuildLocationNetworks(GameLocation location)
        {
            DataAccess DataAccess = DataAccess.GetDataAccess();
            if(location.Name.Equals(Game1.getFarm().Name))
            {
                if (Globals.UltraDebug) { Printer.Info("LOADING FARM BUILDINGS"); }
                foreach (Building building in Game1.getFarm().buildings)
                {
                    if (building != null)
                    {
                        if (DataAccess.Buildings.Contains(building.buildingType.ToString()))
                        {
                            for (int i = 0; i < building.tilesWide.Value; i++)
                            {
                                for (int j = 0; j < building.tilesHigh.Value; j++)
                                {
                                    int x = building.tileX.Value + i;
                                    int y = building.tileY.Value + j;
                                    BuildBuildings(new Vector2(x, y), location, null);
                                }
                            }
                        }
                    }
                }
            }
            if(location.Objects.Count() > 0)
            {
                foreach (KeyValuePair<Vector2, StardewValley.Object> obj in location.Objects.Pairs)
                {
                    if (obj.Value != null)
                    {
                        if (DataAccess.ModItems.Contains(obj.Value.Name))
                        {
                            Printer.Info(obj.Value.Name);
                            BuildNetworkRecursive(new Vector2(obj.Key.X, obj.Key.Y), location, null);
                        }
                    }
                }
            }
        }

        public static void BuildBuildings(Vector2 position, GameLocation location, Network inNetwork)
        {
            DataAccess DataAccess = DataAccess.GetDataAccess();
            List<Node> nodes = DataAccess.LocationNodes[location];
            if ((Game1.getFarm().getBuildingAt(position) != null) && DataAccess.Buildings.Contains(Game1.getFarm().getBuildingAt(position).buildingType.ToString()))
            {
                nodes.Add(NodeFactory.CreateElement(position, location, Game1.getFarm().getBuildingAt(position)));
            }
        }

        public static Node BuildNetworkRecursive(Vector2 position, GameLocation location, Network inNetwork)
        {
            DataAccess DataAccess = DataAccess.GetDataAccess();
            Node node = null;
            string inType = "";
            int x = (int)position.X;
            int y = (int)position.Y;
            if ((location.getObjectAtTile(x, y) != null) && DataAccess.ModItems.Contains(location.getObjectAtTile(x, y).Name))
            {
                inType = "object";
            }
            else if ((Game1.getFarm().getBuildingAt(new Vector2(x, y)) != null) && DataAccess.Buildings.Contains(Game1.getFarm().getBuildingAt(new Vector2(x, y)).buildingType.ToString()))
            {
                inType = "building";
            }
            if (inType.Equals("object") || inType.Equals("building"))
            {
                List<Node> nodes = DataAccess.LocationNodes[location];
                if (nodes.Find(n => n.Position.Equals(position)) == null)
                {
                    if (inType.Equals("object"))
                    {
                        nodes.Add(NodeFactory.CreateElement(new Vector2(x, y), location, location.getObjectAtTile(x, y)));
                    }
                    else if (inType.Equals("building"))
                    {
                        nodes.Add(NodeFactory.CreateElement(new Vector2(x, y), location, Game1.getFarm().getBuildingAt(new Vector2(x, y))));
                    }
                }
                if (inType.Equals("object"))
                {
                    node = nodes.Find(n => n.Position.Equals(position));
                    if (node.ParentNetwork == null)
                    {
                        if (inNetwork == null)
                        {
                            node.ParentNetwork = NetworkManager.CreateLocationNetwork(location);
                        }
                        else
                        {
                            node.ParentNetwork = inNetwork;
                        }
                        NetworkManager.LoadNodeToNetwork(node.Position, location, node.ParentNetwork);
                        //North
                        Vector2 north = new Vector2(x, y - 1);
                        if (location.getObjectAtTile(x, y - 1) != null && y - 1 >= 0)
                        {
                            if (DataAccess.NetworkItems.Contains(location.getObjectAtTile(x, y - 1).Name))
                            {
                                if (!node.ParentNetwork.Nodes.Contains(nodes.Find(n => n.Position.Equals(north))))
                                {
                                    Node adj = BuildNetworkRecursive(north, location, node.ParentNetwork);
                                    node.AddAdjacent(SideStruct.GetSides().North, adj);
                                }
                            }
                            else if (DataAccess.ExtraNames.Contains(location.getObjectAtTile(x, y - 1).Name))
                            {
                                Node adj = NodeFactory.CreateElement(north, location, location.getObjectAtTile(x, y - 1));
                                nodes.Add(adj);
                                if (node is IOPipeNode)
                                {
                                    IOPipeNode IOPipeNode = (IOPipeNode)node;
                                    IOPipeNode.AddConnectedContainer(adj);
                                }
                            }
                        }
                        else if (Game1.getFarm().getBuildingAt(north) != null && y - 1 >= 0)
                        {
                            if (DataAccess.Buildings.Contains(Game1.getFarm().getBuildingAt(north).buildingType.ToString()))
                            {
                                Node adj;
                                if (nodes.Find(n => n.Position.Equals(north)) == null)
                                {
                                    adj = NodeFactory.CreateElement(north, location, Game1.getFarm().getBuildingAt(north));
                                    nodes.Add(adj);
                                }
                                else
                                {
                                    adj = nodes.Find(n => n.Position.Equals(north));
                                }
                                if (node is IOPipeNode)
                                {
                                    IOPipeNode IOPipeNode = (IOPipeNode)node;
                                    IOPipeNode.AddConnectedContainer(adj);
                                }
                            }
                        }

                        //South
                        Vector2 south = new Vector2(x, y + 1);
                        if (location.getObjectAtTile(x, y + 1) != null && y + 1 < location.map.DisplayHeight)
                        {
                            if (DataAccess.NetworkItems.Contains(location.getObjectAtTile(x, y + 1).Name))
                            {
                                if (!node.ParentNetwork.Nodes.Contains(nodes.Find(n => n.Position.Equals(south))))
                                {
                                    Node adj = BuildNetworkRecursive(south, location, node.ParentNetwork);
                                    node.AddAdjacent(SideStruct.GetSides().South, adj);
                                }
                            }
                            else if (DataAccess.ExtraNames.Contains(location.getObjectAtTile(x, y + 1).Name))
                            {
                                Node adj = NodeFactory.CreateElement(south, location, location.getObjectAtTile(x, y + 1));
                                nodes.Add(adj);
                                if (node is IOPipeNode)
                                {
                                    IOPipeNode IOPipeNode = (IOPipeNode)node;
                                    IOPipeNode.AddConnectedContainer(adj);
                                }
                            }
                        }
                        else if (Game1.getFarm().getBuildingAt(south) != null && y + 1 < location.map.DisplayHeight)
                        {
                            if (DataAccess.Buildings.Contains(Game1.getFarm().getBuildingAt(south).buildingType.ToString()))
                            {
                                Node adj;
                                if (nodes.Find(n => n.Position.Equals(south)) == null)
                                {
                                    adj = NodeFactory.CreateElement(south, location, Game1.getFarm().getBuildingAt(south));
                                    nodes.Add(adj);
                                }
                                else
                                {
                                    adj = nodes.Find(n => n.Position.Equals(south));
                                }
                                if (node is IOPipeNode)
                                {
                                    IOPipeNode IOPipeNode = (IOPipeNode)node;
                                    IOPipeNode.AddConnectedContainer(adj);
                                }
                            }
                        }
                        //East
                        Vector2 east = new Vector2(x + 1, y);
                        if (location.getObjectAtTile(x + 1, y) != null && x + 1 >= 0)
                        {
                            if (DataAccess.NetworkItems.Contains(location.getObjectAtTile(x + 1, y).Name))
                            {
                                if (!node.ParentNetwork.Nodes.Contains(nodes.Find(n => n.Position.Equals(east))))
                                {
                                    Node adj = BuildNetworkRecursive(east, location, node.ParentNetwork);
                                    node.AddAdjacent(SideStruct.GetSides().East, adj);
                                }
                            }
                            else if (DataAccess.ExtraNames.Contains(location.getObjectAtTile(x + 1, y).Name))
                            {
                                Node adj = NodeFactory.CreateElement(east, location, location.getObjectAtTile(x + 1, y));
                                nodes.Add(adj);
                                if (node is IOPipeNode)
                                {
                                    IOPipeNode IOPipeNode = (IOPipeNode)node;
                                    IOPipeNode.AddConnectedContainer(adj);
                                }
                            }
                        }
                        else if (Game1.getFarm().getBuildingAt(east) != null && x + 1 >= 0)
                        {
                            if (DataAccess.Buildings.Contains(Game1.getFarm().getBuildingAt(east).buildingType.ToString()))
                            {
                                Node adj;
                                if (nodes.Find(n => n.Position.Equals(east)) == null)
                                {
                                    adj = NodeFactory.CreateElement(east, location, Game1.getFarm().getBuildingAt(east));
                                    nodes.Add(adj);
                                }
                                else
                                {
                                    adj = nodes.Find(n => n.Position.Equals(east));
                                }
                                if (node is IOPipeNode)
                                {
                                    IOPipeNode IOPipeNode = (IOPipeNode)node;
                                    IOPipeNode.AddConnectedContainer(adj);
                                }
                            }
                        }
                        //West
                        Vector2 west = new Vector2(x - 1, y);
                        if (location.getObjectAtTile(x - 1, y) != null && x + 1 < location.map.DisplayWidth)
                        {

                            if (DataAccess.NetworkItems.Contains(location.getObjectAtTile(x - 1, y).Name))
                            {
                                if (!node.ParentNetwork.Nodes.Contains(nodes.Find(n => n.Position.Equals(west))))
                                {
                                    Node adj = BuildNetworkRecursive(west, location, node.ParentNetwork);
                                    node.AddAdjacent(SideStruct.GetSides().West, adj);
                                }
                            }
                            else if (DataAccess.ExtraNames.Contains(location.getObjectAtTile(x - 1, y).Name))
                            {
                                Node adj = NodeFactory.CreateElement(west, location, location.getObjectAtTile(x - 1, y));
                                nodes.Add(adj);
                                if (node is IOPipeNode)
                                {
                                    IOPipeNode IOPipeNode = (IOPipeNode)node;
                                    IOPipeNode.AddConnectedContainer(adj);
                                }
                            }
                        }
                        else if (Game1.getFarm().getBuildingAt(west) != null && x - 1 < location.map.DisplayWidth)
                        {
                            if (DataAccess.Buildings.Contains(Game1.getFarm().getBuildingAt(west).buildingType.ToString()))
                            {
                                Node adj;
                                if (nodes.Find(n => n.Position.Equals(west)) == null)
                                {
                                    adj = NodeFactory.CreateElement(west, location, Game1.getFarm().getBuildingAt(west));
                                    nodes.Add(adj);
                                }
                                else
                                {
                                    adj = nodes.Find(n => n.Position.Equals(west));
                                }
                                if (node is IOPipeNode)
                                {
                                    IOPipeNode IOPipeNode = (IOPipeNode)node;
                                    IOPipeNode.AddConnectedContainer(adj);
                                }
                            }
                        }
                    }
                }
            }
            return node;
        }
    }
}
