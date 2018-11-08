using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewValley;
using xTile.Dimensions;

namespace FollowerNPC
{
    public class aStar
    {

        #region Constructor & Members
        public aStar(GameLocation location, string character)
        {
            gameLocation = location;
            this.character = character;
        }

        public GameLocation gameLocation
        {
            get { return gl; }
            set
            {
                gl = value;
                dimensions = new Vector2(gameLocation.map.Layers[0].LayerWidth, gameLocation.map.Layers[0].LayerHeight);
            }
        }
        private GameLocation gl;
        private string character;
        private Vector2 dimensions;
        private Vector2 negativeOne = new Vector2(-1, -1);
        private int fullTile = Game1.tileSize;
        private int halfTile = (int) (Game1.tileSize * 0.5f);

        public Queue<Vector2> consolidatedPath;
        public List<Node> fullPath;
        #endregion

        #region Public Methods
        public Queue<Vector2> Pathfind(Vector2 start, Vector2 goal)
        {
            // Setup
            PriorityQueue open = new PriorityQueue(new Node(null, start));
            Dictionary<Vector2, float> closed = new Dictionary<Vector2, float>();
            Vector2 mapDimensions = new Vector2(100, 100);
            List<Node> path = new List<Node>();

            // Cache
            float maxH = mapDimensions.Length();

            while (open.Count != 0)
            {
                Node q = open.Dequeue();
                closed[q.position] = q.f;
                //ModEntry.monitor.Log(q.position.ToString());
                Node[] successors = GetSuccessors(q);
                foreach (Node successor in successors)
                {
                    if (!(successor != null))
                        continue;

                    if (successor.position == goal)
                    {
                        path = Reconstruct(successor);
                        fullPath = path;
                        goto NodeConsolidation;
                    }

                    successor.g = q.g + EuclideanDistance(q.position, successor.position);
                    successor.h = EuclideanDistance(successor.position, goal);
                    successor.f = successor.g + successor.h;

                    if (open.CheckValue(successor.position) <= successor.f)
                        continue;

                    if (closed.TryGetValue(successor.position, out float closedNodeCost) && closedNodeCost <= successor.f)
                        continue;

                    open.Enqueue(successor);
                }
            }
            return null;
            NodeConsolidation:
            Queue<Vector2> consolidatedPath = new Queue<Vector2>();
            for (int i = path.Count-2; i >=0; i--)
            {
                if (IsCorner(path[i].position))
                    consolidatedPath.Enqueue(path[i].position);
            }
            if (!IsCorner(goal))
                consolidatedPath.Enqueue(goal);
            this.consolidatedPath = consolidatedPath;
            return consolidatedPath;
        }
        #endregion

        #region Helper Methods
        private Vector2[] GetNeighbors(Vector2 tile)
        {
            float ppx = tile.X;
            float ppy = tile.Y;
            float mdx = dimensions.X;
            float mdy = dimensions.Y;
            Vector2[] successors = new Vector2[8];

            successors[0] = IsWalkableTile(new Vector2(ppx, ppy - 1)) ? new Vector2(ppx, ppy - 1) : negativeOne;
            successors[1] = IsWalkableTile(new Vector2(ppx + 1, ppy - 1)) ? new Vector2(ppx + 1, ppy - 1) : negativeOne;
            successors[2] = IsWalkableTile(new Vector2(ppx + 1, ppy)) ? new Vector2(ppx + 1, ppy) : negativeOne;
            successors[3] = IsWalkableTile(new Vector2(ppx + 1, ppy + 1)) ? new Vector2(ppx + 1, ppy + 1) : negativeOne;
            successors[4] = IsWalkableTile(new Vector2(ppx, ppy + 1)) ? new Vector2(ppx, ppy + 1) : negativeOne;
            successors[5] = IsWalkableTile(new Vector2(ppx - 1, ppy + 1)) ? new Vector2(ppx - 1, ppy + 1) : negativeOne;
            successors[6] = IsWalkableTile(new Vector2(ppx - 1, ppy)) ? new Vector2(ppx - 1, ppy) : negativeOne;
            successors[7] = IsWalkableTile(new Vector2(ppx - 1, ppy - 1)) ? new Vector2(ppx - 1, ppy - 1) : negativeOne;
            return successors;
        }

        private Node[] GetSuccessors(Node parent)
        {
            float ppx = parent.position.X;
            float ppy = parent.position.Y;
            float mdx = dimensions.X;
            float mdy = dimensions.Y;
            Node[] successors = new Node[4];
            successors[0] = IsWalkableTile(new Vector2(ppx, ppy - 1)) ? new Node(parent, new Vector2(ppx, ppy - 1)) : null;
            successors[1] = IsWalkableTile(new Vector2(ppx + 1, ppy)) ? new Node(parent, new Vector2(ppx + 1, ppy)) : null;
            successors[2] = IsWalkableTile(new Vector2(ppx, ppy + 1)) ? new Node(parent, new Vector2(ppx, ppy + 1)) : null;
            successors[3] = IsWalkableTile(new Vector2(ppx - 1, ppy)) ? new Node(parent, new Vector2(ppx - 1, ppy)) : null;
            return successors;
        }

        public bool IsWalkableTile(Vector2 tile)
        {
            StardewValley.Object o = gl.getObjectAtTile((int) tile.X, (int) tile.Y);
            return gl.isTileOnMap(tile) && !gl.isTileOccupiedIgnoreFloors(tile, character) &&
                   isTilePassableOverride(new Location((int) tile.X, (int) tile.Y), Game1.viewport) &&
                   (!(gameLocation is Farm)  || !((gameLocation as Farm).getBuildingAt(tile) != null)) &&
                   (!(o != null) || (o != null &&
                                     (o as StardewValley.Objects.Furniture)
                                     .furniture_type.Value == 12));
        }

        //public bool IsWalkableTilePrint(Vector2 tile)
        //{
        //    StardewValley.Object o = gl.getObjectAtTile((int)tile.X, (int)tile.Y);
        //    bool isTileOnMap = gl.isTileOnMap(tile);
        //    ModEntry.monitor.Log("is tile on map?: " + isTileOnMap.ToString());
        //    bool isTileUnoccupied = !gl.isTileOccupiedIgnoreFloors(tile, character);
        //    ModEntry.monitor.Log("is unnocupied?: " + isTileUnoccupied.ToString());
        //    bool isTilePassable = isTilePassableOverride(new Location((int)tile.X, (int)tile.Y), Game1.viewport);
        //    ModEntry.monitor.Log("is passable?: " + isTilePassable.ToString());
        //    bool isNotFarmBuilding = !gameLocation.IsFarm && !((gameLocation as Farm).getBuildingAt(tile) != null);
        //    //bool isTilePlaceable = gl.isTilePlaceable(tile, null);
        //    //ModEntry.monitor.Log("is placeable?: " + isTilePlaceable.ToString());
        //    bool noUnpassableObjects = (!(o != null) || (o != null &&
        //                                                 (o as StardewValley.Objects.Furniture)
        //                                                 .furniture_type.Value == 12));
        //    ModEntry.monitor.Log("is clear of unpassable objects?: " + noUnpassableObjects.ToString());

        //    return isTileOnMap && isTileUnoccupied && isTilePassable && noUnpassableObjects;
        //}

        public bool isTilePassableOverride(Location tileLocation, xTile.Dimensions.Rectangle viewport)
        {
            xTile.ObjectModel.PropertyValue passable = null;
            xTile.Tiles.Tile tmp = gameLocation.map.GetLayer("Back").PickTile(new Location(tileLocation.X * Game1.tileSize, tileLocation.Y * Game1.tileSize), viewport.Size);
            if (tmp != null)
            {
                tmp.TileIndexProperties.TryGetValue("Passable", out passable);
            }
            xTile.ObjectModel.PropertyValue passableBuilding = null;
            xTile.Tiles.Tile tile = gameLocation.map.GetLayer("Buildings").PickTile(new Location(tileLocation.X * Game1.tileSize, tileLocation.Y * Game1.tileSize), viewport.Size);
            if (tile != null)
            {
                passableBuilding = tile.TileIndexProperties.TryGetValue("Passable", out passableBuilding);
            }
            return passable == null && (tile == null || (passableBuilding != null && passableBuilding)) && tmp != null;
        }

        //public bool isTilePassableOverridePrint(Location tileLocation, xTile.Dimensions.Rectangle viewport)
        //{
        //    xTile.ObjectModel.PropertyValue passable = null;
        //    xTile.Tiles.Tile tmp = gameLocation.map.GetLayer("Back").PickTile(new Location(tileLocation.X * Game1.tileSize, tileLocation.Y * Game1.tileSize), viewport.Size);
        //    if (tmp != null)
        //    {
        //        tmp.TileIndexProperties.TryGetValue("Passable", out passable);
        //    }
        //    xTile.ObjectModel.PropertyValue passableBuilding = null;
        //    xTile.Tiles.Tile tile = gameLocation.map.GetLayer("Buildings").PickTile(new Location(tileLocation.X * Game1.tileSize, tileLocation.Y * Game1.tileSize), viewport.Size);
        //    if (tile != null)
        //    {
        //        passableBuilding = tile.TileIndexProperties.TryGetValue("Passable", out passableBuilding);
        //        ModEntry.monitor.Log("Passable Building property value: " + passableBuilding);
        //    }
        //    ModEntry.monitor.Log("Is Tile Passable Override:");
        //    ModEntry.monitor.Log("Tile Exists on Back Layer: " + (tmp != null).ToString());
        //    ModEntry.monitor.Log("Back Tile Passable Property: " + (passable == null).ToString());
        //    ModEntry.monitor.Log("Tile Doesn't Exist on Building Layer: " + (tile == null).ToString());
        //    ModEntry.monitor.Log("Building Tile is Passable " + (passableBuilding != null).ToString());
        //    //ModEntry.monitor.Log("Building Tile has 'T' property for 'Passable': " + (passableBuilding == "T").ToString());
        //    return passable == null && (tile == null || (passableBuilding != null && passableBuilding)) && tmp != null;
        //}

        private int NodeArrayCount(Node[] a)
        {
            int count = 0;
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != null)
                    count++;
            }
            return count;
        }

        private List<Node> Reconstruct(Node goal)
        {
            Node iterator = goal;
            List<Node> ret = new List<Node>();
            ret.Add(goal);
            while (iterator.parent != null)
            {
                ret.Add(iterator.parent);
                iterator = iterator.parent;
            }
            return ret;
        }

        private bool IsCorner(Vector2 tile)
        {
            Vector2[] neighbors = GetNeighbors(tile);
            for (int i = 1; i < 8; i+=2)
            {
                if (neighbors[i] == negativeOne)
                {
                    if ((neighbors[i - 1] != negativeOne) &&
                        (neighbors[i + 1 > 7 ? 0 : i + 1] != negativeOne))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private float ManhattanDistance(Vector2 a, Vector2 b)
        {
            return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
        }

        private float EuclideanDistance(Vector2 a, Vector2 b)
        {
            return (a - b).Length();
            //return (float)Math.Sqrt(((a.X - b.X) * (a.X - b.X)) + ((a.Y - b.Y) * (a.Y - b.Y)));
        }
        #endregion
    }

    public class Node
    {
        public Node parent;
        public Vector2 position;
        public float f { get; set; }
        public float g { get; set; }
        public float h { get; set; }

        public Node(Node parent, Vector2 position)
        {
            this.parent = parent;
            this.position = position;
        }
    }

    class PriorityQueue
    {
        Node[] items;
        Dictionary<Vector2, int> positions;
        public int Count { get; private set; }

        public PriorityQueue(Node head)
        {
            items = new Node[100];
            items[0] = null;
            items[1] = head;
            Count = 1;

            positions = new Dictionary<Vector2, int>();
            positions.Add(head.position, 1);
        }

        public void Enqueue(Node item)
        {
            int position;
            if (!positions.TryGetValue(item.position, out position))
            {
                Count++;
                if (Count >= items.Length / 2)
                    ResizeItems();
                items[Count] = item;
                positions.Add(item.position, Count);
                PercolateUp(Count);
            }
            else if (items[position].f > item.f)
            {
                items[position] = item;
                PercolateUp(position);
            }
        }

        public Node Dequeue()
        {
            Node ret = items[1];
            items[1] = items[Count];
            items[Count] = null;
            positions.Remove(ret.position);
            if (Count > 1)
                positions[items[1].position] = 1;
            PercolateDown();
            Count--;
            return ret;
        }

        public Node Peek()
        {
            return items[1];
        }

        public float CheckValue(Vector2 nodeValue)
        {
            if (positions.TryGetValue(nodeValue, out int itemPosition))
                return items[itemPosition].f;
            return float.PositiveInfinity;
        }

        private void PercolateUp(int index)
        {
            while (items[index / 2] != null)
            {
                Node child = items[index];
                Node parent = items[index / 2];
                if (child.f < parent.f)
                {
                    items[index] = parent;
                    positions[parent.position] = index;
                    items[index / 2] = child;
                    positions[child.position] = index / 2;
                    index = index / 2;
                }
                else
                    return;
            }
        }

        private void PercolateDown()
        {
            int index = 1;
            int replaceIndex = 1;
            while (index == replaceIndex)
            {
                Node child_L = items[(index * 2)];
                Node child_R = items[(index * 2) + 1];
                Node parent = items[index];
                if (child_L != null && child_R != null)
                    replaceIndex = child_L.f < child_R.f ? (index * 2) : (index * 2) + 1;
                else if (child_L != null)
                    replaceIndex = index * 2;
                else
                    return;
                Node child = items[replaceIndex];
                if (child.f < parent.f)
                {
                    items[replaceIndex] = parent;
                    positions[parent.position] = replaceIndex;
                    items[index] = child;
                    positions[child.position] = index;
                    index = replaceIndex;
                }
            }
        }

        private void ResizeItems()
        {
            Node[] newItems = new Node[items.Length * 2];
            for (int i = 0; i < Count; i++)
                newItems[i] = items[i];
            items = newItems;
        }
    }
}
