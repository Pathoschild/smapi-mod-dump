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
        private int attempts;

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
            attempts = 0;

            // Cache
            float maxH = mapDimensions.Length();

            while (open.Count != 0 && ++attempts <= 150)
            {
                Node q = open.Dequeue();
                closed[q.position] = q.f;
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
            StardewValley.Objects.Furniture furn = o as StardewValley.Objects.Furniture;
            Fence fence = o as Fence;

            //bool a = gl.isTileOnMap(tile);
            //bool b = !gl.isTileOccupiedIgnoreFloors(tile, character);
            //bool c = isTilePassableOverride(new Location((int)tile.X, (int)tile.Y), Game1.viewport);
            //bool d = (!(gameLocation is Farm) || !((gameLocation as Farm).getBuildingAt(tile) != null));
            //bool ea = !(o != null);
            //bool eba = (o != null);
            //bool ebba = (o as StardewValley.Objects.Furniture).furniture_type.Value == 12;
            //bool ebbba = ((o as Fence).isGate.Value);
            //bool ebbbb = (o as Fence).gatePosition.Value == 88;
            //bool ebbb = ebbba && ebbbb;
            //bool ebb = ebba || ebbb;
            //bool eb = eba && ebb;
            //bool full = (!(o != null) || (o != null && ((o as StardewValley.Objects.Furniture).furniture_type.Value == 12 || (((o as Fence).isGate.Value) && (o as Fence).gatePosition.Value == 88))));

            //bool one = gl.isTileOnMap(tile);
            //bool two = !isTileOccupiedIgnoreFloorsOverride(tile, character);
            //bool three = isTilePassableOverride(new Location((int) tile.X, (int) tile.Y), Game1.viewport);
            //bool four = (!(gameLocation is Farm) || !((gameLocation as Farm).getBuildingAt(tile) != null));
            //bool five = (!(o != null) || (furn != null && furn.furniture_type.Value == 12) ||
            //             (fence != null && fence.isGate.Value && (o as Fence).gatePosition.Value == 88));

            return gl.isTileOnMap(tile) && 
                   !isTileOccupiedIgnoreFloorsOverride(tile, character) &&
                   isTilePassableOverride(new Location((int) tile.X, (int) tile.Y), Game1.viewport) &&
                   (!(gameLocation is Farm)  || !((gameLocation as Farm).getBuildingAt(tile) != null)) &&
                   (!(o != null) || (furn != null && furn.furniture_type.Value == 12) || (fence != null && fence.isGate.Value && (o as Fence).gatePosition.Value == 88));
        }

        public bool isTileOccupiedIgnoreFloorsOverride(Vector2 tileLocation, string characterToIgnore = "")
        {
            Microsoft.Xna.Framework.Rectangle tileLocationRect = new Microsoft.Xna.Framework.Rectangle((int)tileLocation.X * 64 + 1, (int)tileLocation.Y * 64 + 1, 62, 62);
            for (int i = 0; i < gameLocation.characters.Count; i++)
            {
                if (gameLocation.characters[i] != null && !gameLocation.characters[i].Name.Equals(characterToIgnore) && gameLocation.characters[i].GetBoundingBox().Intersects(tileLocationRect))
                {
                    return true;
                }
            }

            AnimalHouse ah = gameLocation as AnimalHouse;
            if (ah != null)
            {
                foreach (FarmAnimal animal in ah.animals.Values)
                {
                    if (animal.GetBoundingBox().Intersects(tileLocationRect))
                        return true;
                }
            }

            Farm f = gameLocation as Farm;
            if (f != null)
            {
                foreach (FarmAnimal animal in f.animals.Values)
                {
                    if (animal.GetBoundingBox().Intersects(tileLocationRect))
                        return true;
                }
            }

            if (gameLocation.terrainFeatures.ContainsKey(tileLocation) && tileLocationRect.Intersects(gameLocation.terrainFeatures[tileLocation].getBoundingBox(tileLocation)) && !gameLocation.terrainFeatures[tileLocation].isPassable(null))
            {
                return true;
            }
            if (gameLocation.largeTerrainFeatures != null)
            {
                foreach (StardewValley.TerrainFeatures.LargeTerrainFeature tf in gameLocation.largeTerrainFeatures)
                {
                    if (tf.getBoundingBox().Intersects(tileLocationRect))
                        return true;
                }
            }
            return false;
        }

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
