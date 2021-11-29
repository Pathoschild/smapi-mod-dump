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
using StardewValley.Network;
using StardewValley.Buildings;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using Microsoft.Xna.Framework;

namespace ItemPipes.Framework
{
    public static class NetworkBuilder
    {
        public static void BuildLocationNetworks(GameLocation location)
        {
            DataAccess DataAccess = DataAccess.GetDataAccess();
            if(location.Name.Equals(Game1.getFarm().Name))
            {
                if (Globals.Debug) { Printer.Info("LOADING FARM BUILDINGS"); }
                foreach (Building building in Game1.getFarm().buildings)
                {
                    if (building != null)
                    {
                        if (DataAccess.Buildings.Contains(building.buildingType.ToString()))
                        {
                            for (int i = 0; i < building.tilesWide; i++)
                            {
                                for (int j = 0; j < building.tilesHigh; j++)
                                {
                                    int x = building.tileX + i;
                                    int y = building.tileY + j;
                                    BuildBuildings(location, null, x, y);
                                }
                            }
                        }
                    }
                }
            }
            if (Globals.Debug) { Printer.Info("LOADING FARM OBJECTS"); }
            foreach (KeyValuePair<Vector2, StardewValley.Object> obj in location.Objects.Pairs)
            {
                if (obj.Value != null)
                {
                    if (DataAccess.NetworkItems.Contains(obj.Value.Name))
                    {

                        BuildNetworkRecursive(location, null, (int)obj.Key.X, (int)obj.Key.Y);
                    }
                }
            }

        }

        public static void BuildBuildings(GameLocation location, Network inNetwork, int x, int y)
        {
            DataAccess DataAccess = DataAccess.GetDataAccess();
            Node[,] matrix;
            if (DataAccess.LocationMatrix.TryGetValue(location, out matrix))
            {
                if ((Game1.getFarm().getBuildingAt(new Vector2(x, y)) != null) && DataAccess.Buildings.Contains(Game1.getFarm().getBuildingAt(new Vector2(x, y)).buildingType.ToString()))
                {
                    matrix[x, y] = NodeFactory.CreateElement(new Vector2(x, y), location, Game1.getFarm().getBuildingAt(new Vector2(x, y)));
                }
            }
        }

        public static Node BuildNetworkRecursive(GameLocation location, Network inNetwork, int x, int y)
        {
            DataAccess DataAccess = DataAccess.GetDataAccess();
            Node node = null;
            Node[,] matrix;
            string inType = "";
            if((location.getObjectAtTile(x, y) != null) && (DataAccess.ModItems.Contains(location.getObjectAtTile(x, y).name)))
            {
                inType = "object";
            }
            else if ((Game1.getFarm().getBuildingAt(new Vector2(x, y)) != null) && DataAccess.Buildings.Contains(Game1.getFarm().getBuildingAt(new Vector2(x, y)).buildingType.ToString()))
            {
                inType = "building";
            }
            if (inType.Equals("object") || inType.Equals("building"))
            {
                if (DataAccess.LocationMatrix.TryGetValue(location, out matrix) )
                {
                    if (matrix[x, y] == null)
                    {
                        if(inType.Equals("object"))
                        {
                            matrix[x, y] = NodeFactory.CreateElement(new Vector2(x, y), location, location.getObjectAtTile(x, y));
                        }
                        else if(inType.Equals("building"))
                        {
                            matrix[x, y] = NodeFactory.CreateElement(new Vector2(x, y), location, Game1.getFarm().getBuildingAt(new Vector2(x, y)));
                        }
                        
                    }
                    if (inType.Equals("object"))
                    {
                        node = matrix[x, y];
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
                            NetworkManager.LoadNodeToNetwork(location, x, y, node.ParentNetwork);
                            //North
                            if (location.getObjectAtTile(x, y - 1) != null && y - 1 >= 0)
                            {
                                if (DataAccess.NetworkItems.Contains(location.getObjectAtTile(x, y - 1).Name))
                                {
                                    if (!node.ParentNetwork.Nodes.Contains(matrix[x, y - 1]))
                                    {
                                        Node adj = BuildNetworkRecursive(location, node.ParentNetwork, x, y - 1);
                                        node.AddAdjacent(SideStruct.GetSides().North, adj);
                                    }
                                }
                                else if (DataAccess.ExtraNames.Contains(location.getObjectAtTile(x, y - 1).Name))
                                {
                                    Node adj = NodeFactory.CreateElement(new Vector2(x, y - 1), location, location.getObjectAtTile(x, y - 1));
                                    matrix[x, y - 1] = adj;
                                    node.AddAdjacent(SideStruct.GetSides().North, adj);
                                }
                            }
                            else if (Game1.getFarm().getBuildingAt(new Vector2(x, y - 1)) != null && y - 1 >= 0)
                            {
                                if (matrix[x, y - 1] == null)
                                {
                                    Node adj = NodeFactory.CreateElement(new Vector2(x, y - 1), location, Game1.getFarm().getBuildingAt(new Vector2(x, y - 1)));
                                    matrix[x, y - 1] = adj;
                                    node.AddAdjacent(SideStruct.GetSides().North, adj);
                                }
                                else
                                {
                                    Node adj = matrix[x, y - 1];
                                    node.AddAdjacent(SideStruct.GetSides().North, adj);
                                }
                                
                            }

                            //South
                            if (location.getObjectAtTile(x, y + 1) != null && y + 1 < location.map.DisplayHeight)
                            {
                                if (DataAccess.NetworkItems.Contains(location.getObjectAtTile(x, y + 1).Name))
                                {
                                    if (!node.ParentNetwork.Nodes.Contains(matrix[x, y + 1]))
                                    {
                                        Node adj = BuildNetworkRecursive(location, node.ParentNetwork, x, y + 1);
                                        node.AddAdjacent(SideStruct.GetSides().South, adj);
                                    }
                                }
                                else if (DataAccess.ExtraNames.Contains(location.getObjectAtTile(x, y + 1).Name))
                                {
                                    Node adj = NodeFactory.CreateElement(new Vector2(x, y + 1), location, location.getObjectAtTile(x, y + 1));
                                    matrix[x, y + 1] = adj;
                                    node.AddAdjacent(SideStruct.GetSides().South, adj);
                                }
                            }
                            else if (Game1.getFarm().getBuildingAt(new Vector2(x, y + 1)) != null && y + 1 < location.map.DisplayHeight)
                            {
                                if (matrix[x, y + 1] == null)
                                {
                                    Node adj = NodeFactory.CreateElement(new Vector2(x, y + 1), location, Game1.getFarm().getBuildingAt(new Vector2(x, y + 1)));
                                    matrix[x, y + 1] = adj;
                                    node.AddAdjacent(SideStruct.GetSides().South, adj);
                                }
                                else
                                {
                                    Node adj = matrix[x, y + 1];
                                    node.AddAdjacent(SideStruct.GetSides().South, adj);
                                }
                                
                            }
                            //West
                            if (location.getObjectAtTile(x + 1, y) != null && x + 1 < location.map.DisplayWidth)
                            {
                                if (DataAccess.NetworkItems.Contains(location.getObjectAtTile(x + 1, y).Name))
                                {
                                    if (!node.ParentNetwork.Nodes.Contains(matrix[x + 1, y]))
                                    {
                                        Node adj = BuildNetworkRecursive(location, node.ParentNetwork, x + 1, y);
                                        node.AddAdjacent(SideStruct.GetSides().West, adj);
                                    }
                                }
                                else if (DataAccess.ExtraNames.Contains(location.getObjectAtTile(x + 1, y).Name))
                                {
                                    Node adj = NodeFactory.CreateElement(new Vector2(x + 1, y), location, location.getObjectAtTile(x + 1, y));
                                    matrix[x + 1, y] = adj;
                                    node.AddAdjacent(SideStruct.GetSides().West, adj);
                                }
                            }
                            else if (Game1.getFarm().getBuildingAt(new Vector2(x + 1, y)) != null && x + 1 < location.map.DisplayWidth)
                            {
                                if (matrix[x + 1, y] == null)
                                {
                                    Node adj = NodeFactory.CreateElement(new Vector2(x + 1, y), location, Game1.getFarm().getBuildingAt(new Vector2(x + 1, y)));
                                    matrix[x + 1, y] = adj;
                                    node.AddAdjacent(SideStruct.GetSides().West, adj);
                                }
                                else
                                {
                                    Node adj = matrix[x + 1, y];
                                    node.AddAdjacent(SideStruct.GetSides().West, adj);
                                }
                                
                            }
                            //East
                            if (location.getObjectAtTile(x - 1, y) != null && x - 1 >= 0)
                            {
                                if (DataAccess.NetworkItems.Contains(location.getObjectAtTile(x - 1, y).Name))
                                {
                                    if (!node.ParentNetwork.Nodes.Contains(matrix[x - 1, y]))
                                    {
                                        Node adj = BuildNetworkRecursive(location, node.ParentNetwork, x - 1, y);
                                        node.AddAdjacent(SideStruct.GetSides().East, adj);
                                    }
                                }
                                else if (DataAccess.ExtraNames.Contains(location.getObjectAtTile(x - 1, y).Name))
                                {
                                    Node adj = NodeFactory.CreateElement(new Vector2(x - 1, y), location, location.getObjectAtTile(x - 1, y));
                                    matrix[x - 1, y] = adj;
                                    node.AddAdjacent(SideStruct.GetSides().East, adj);
                                }
                            }
                            else if (Game1.getFarm().getBuildingAt(new Vector2(x - 1, y)) != null && x - 1 >= 0)
                            {
                                if (DataAccess.Buildings.Contains(Game1.getFarm().getBuildingAt(new Vector2(x - 1, y)).buildingType.ToString()))
                                {
                                    if (matrix[x - 1, y] == null)
                                    {
                                        Node adj = NodeFactory.CreateElement(new Vector2(x - 1, y), location, Game1.getFarm().getBuildingAt(new Vector2(x - 1, y)));
                                        matrix[x - 1, y] = adj;
                                        node.AddAdjacent(SideStruct.GetSides().East, adj);
                                    }
                                    else
                                    {
                                        Node adj = matrix[x - 1, y];
                                        node.AddAdjacent(SideStruct.GetSides().East, adj);
                                    }
                                    
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
