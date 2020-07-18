using Microsoft.Xna.Framework;
using NpcAdventure.Utils;
using StardewValley;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile.Dimensions;

namespace NpcAdventure.AI.Controller
{
    public class PathFinder
    {

        #region Constructor & Members
        public PathFinder(GameLocation location, Character thisCharacter, Character goalCharacter = null)
        {
            this.GameLocation = location;
            this.thisCharacter = thisCharacter;
            this.goalCharacter = goalCharacter;
            this.relevantCharacters = new List<Character>(2) { thisCharacter, goalCharacter };
        }

        public GameLocation GameLocation
        {
            get { return this.gl; }
            set
            {
                this.gl = value;
                this.dimensions = new Vector2(value.map.Layers[0].LayerWidth, value.map.Layers[0].LayerHeight);
                this.tileCache = new Dictionary<Vector2, bool>();
                this.f = value as Farm;
                this.w = value as Woods;
                this.ah = value as AnimalHouse;
                this.ms = value as MineShaft;
            }
        }

        public Character GoalCharacter
        {
            get { return this.goalCharacter; }
            set
            {
                this.goalCharacter = value;
                this.relevantCharacters[1] = value;
            }
        }

        private GameLocation gl;
        private Dictionary<Vector2, bool> tileCache;
        private Farm f;
        private Woods w;
        private AnimalHouse ah;
        private MineShaft ms;
        private Character thisCharacter;
        private Character goalCharacter;
        private List<Character> relevantCharacters;
        private Vector2 dimensions;
        private Vector2 negativeOne = new Vector2(-1, -1);
        private int attempts;

        public Queue<Vector2> consolidatedPath;
        #endregion

        #region Public Methods
        public Queue<Vector2> Pathfind(Vector2 start, Vector2 goal)
        {
            if (this.GoalCharacter == null)
                return null;

            // Setup
            PriorityQueue open = new PriorityQueue(new Node(null, start));
            Dictionary<Vector2, float> closed = new Dictionary<Vector2, float>();
            Vector2 mapDimensions = new Vector2(100, 100);
            List<Node> path = new List<Node>();
            this.attempts = 0;

            // Cache
            float maxH = mapDimensions.Length();

            while (open.Count != 0 && ++this.attempts <= 150)
            {
                Node q = open.Dequeue();
                closed[q.position] = q.f;
                Node[] successors = this.GetSuccessors(q);
                foreach (Node successor in successors)
                {
                    if (!(successor != null))
                        continue;

                    if (successor.position == goal)
                    {
                        path = this.Reconstruct(successor);
                        goto NodeConsolidation;
                    }

                    successor.g = q.g + this.EuclideanDistance(q.position, successor.position);
                    successor.h = this.EuclideanDistance(successor.position, goal);
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
            for (int i = path.Count - 2; i >= 0; i--)
            {
                if (this.IsCorner(path[i].position))
                    consolidatedPath.Enqueue(path[i].position);
            }
            if (!this.IsCorner(goal))
                consolidatedPath.Enqueue(goal);
            this.consolidatedPath = consolidatedPath;
            return consolidatedPath;
        }
        #endregion

        #region Helper Methods

        public Vector2[] GetWalkableNeighbors(Vector2 tile)
        {
            float ppx = tile.X;
            float ppy = tile.Y;
            float mdx = this.dimensions.X;
            float mdy = this.dimensions.Y;
            Vector2[] successors = new Vector2[8];

            successors[0] = this.IsWalkableTile(new Vector2(ppx, ppy - 1)) ? new Vector2(ppx, ppy - 1) : this.negativeOne;
            successors[1] = this.IsWalkableTile(new Vector2(ppx + 1, ppy - 1)) ? new Vector2(ppx + 1, ppy - 1) : this.negativeOne;
            successors[2] = this.IsWalkableTile(new Vector2(ppx + 1, ppy)) ? new Vector2(ppx + 1, ppy) : this.negativeOne;
            successors[3] = this.IsWalkableTile(new Vector2(ppx + 1, ppy + 1)) ? new Vector2(ppx + 1, ppy + 1) : this.negativeOne;
            successors[4] = this.IsWalkableTile(new Vector2(ppx, ppy + 1)) ? new Vector2(ppx, ppy + 1) : this.negativeOne;
            successors[5] = this.IsWalkableTile(new Vector2(ppx - 1, ppy + 1)) ? new Vector2(ppx - 1, ppy + 1) : this.negativeOne;
            successors[6] = this.IsWalkableTile(new Vector2(ppx - 1, ppy)) ? new Vector2(ppx - 1, ppy) : this.negativeOne;
            successors[7] = this.IsWalkableTile(new Vector2(ppx - 1, ppy - 1)) ? new Vector2(ppx - 1, ppy - 1) : this.negativeOne;
            return successors;
        }

        public Vector3[] GetDirectWalkableNeighbors(Vector3 tile)
        {
            float ppx = tile.X;
            float ppy = tile.Y;
            float mdx = this.dimensions.X;
            float mdy = this.dimensions.Y;
            Vector3[] successors = new Vector3[4];

            successors[0] = this.IsWalkableTile(new Vector2(ppx, ppy - 1)) ? new Vector3(ppx, ppy - 1, 1) : new Vector3(ppx, ppy - 1, 0);
            successors[1] = this.IsWalkableTile(new Vector2(ppx + 1, ppy)) ? new Vector3(ppx + 1, ppy, 1) : new Vector3(ppx + 1, ppy, 0);
            successors[2] = this.IsWalkableTile(new Vector2(ppx, ppy + 1)) ? new Vector3(ppx, ppy + 1, 1) : new Vector3(ppx, ppy + 1, 0);
            successors[3] = this.IsWalkableTile(new Vector2(ppx - 1, ppy)) ? new Vector3(ppx - 1, ppy, 1) : new Vector3(ppx - 1, ppy, 0);
            return successors;
        }

        private Node[] GetSuccessors(Node parent)
        {
            float ppx = parent.position.X;
            float ppy = parent.position.Y;
            float mdx = this.dimensions.X;
            float mdy = this.dimensions.Y;
            Node[] successors = new Node[4];
            successors[0] = this.IsWalkableTile(new Vector2(ppx, ppy - 1)) ? new Node(parent, new Vector2(ppx, ppy - 1)) : null;
            successors[1] = this.IsWalkableTile(new Vector2(ppx + 1, ppy)) ? new Node(parent, new Vector2(ppx + 1, ppy)) : null;
            successors[2] = this.IsWalkableTile(new Vector2(ppx, ppy + 1)) ? new Node(parent, new Vector2(ppx, ppy + 1)) : null;
            successors[3] = this.IsWalkableTile(new Vector2(ppx - 1, ppy)) ? new Node(parent, new Vector2(ppx - 1, ppy)) : null;
            return successors;
        }

        public bool IsWalkableTile(Vector2 tile)
        {
            StardewValley.Object o = this.gl.getObjectAtTile((int)tile.X, (int)tile.Y);
            StardewValley.Objects.Furniture furn = o as StardewValley.Objects.Furniture;
            Fence fence = o as Fence;
            Torch torch = o as Torch;

            return this.gl.isTileOnMap(tile) &&
                       !this.isTileOccupiedIgnoreFloorsOverride(tile) &&
                       this.isTilePassableOverride(new Location((int)tile.X, (int)tile.Y), Game1.viewport) &&
                       (!(this.GameLocation is Farm) || !((this.GameLocation as Farm).getBuildingAt(tile) != null)) &&
                       ((o == null) ||
                        (furn != null && furn.furniture_type.Value == 12) ||
                        (fence != null && fence.isGate.Value) ||
                        (torch != null) ||
                        (o.ParentSheetIndex == 590));

        }

        public bool isTileOccupiedIgnoreFloorsOverride(Vector2 tileLocation)
        {
            Microsoft.Xna.Framework.Rectangle tileLocationRect = new Microsoft.Xna.Framework.Rectangle((int)tileLocation.X * 64 + 1, (int)tileLocation.Y * 64 + 1, 62, 62);
            for (int i = 0; i < this.GameLocation.characters.Count; i++)
            {
                if (this.GameLocation.characters[i] != null 
                    && !this.GameLocation.characters[i].IsMonster 
                    && !this.thisCharacter.Equals(this.GameLocation.characters[i]) 
                    && this.goalCharacter != null 
                    && !this.goalCharacter.Equals(this.GameLocation.characters[i]) 
                    && this.GameLocation.characters[i].GetBoundingBox().Intersects(tileLocationRect))
                {
                    return true;
                }
            }

            if (this.f != null)
            {
                foreach (FarmAnimal animal in this.f.animals.Values)
                {
                    if (animal.GetBoundingBox().Intersects(tileLocationRect))
                        return true;
                }

                foreach (ResourceClump clump in this.f.resourceClumps)
                {
                    if (clump.occupiesTile((int)tileLocation.X, (int)tileLocation.Y))
                        return true;
                }
            }
            else if (this.w != null)
            {
                foreach (ResourceClump stump in this.w.stumps)
                {
                    if (stump.occupiesTile((int)tileLocation.X, (int)tileLocation.Y))
                        return true;
                }
            }
            else if (this.ah != null)
            {
                foreach (FarmAnimal animal in this.ah.animals.Values)
                {
                    if (animal.GetBoundingBox().Intersects(tileLocationRect))
                        return true;
                }
            }
            else if (this.ms != null)
            {
                foreach (ResourceClump clump in this.ms.resourceClumps)
                {
                    if (clump.occupiesTile((int)tileLocation.X, (int)tileLocation.Y))
                        return true;
                }
            }


            if (this.GameLocation.terrainFeatures.ContainsKey(tileLocation) && tileLocationRect.Intersects(this.GameLocation.terrainFeatures[tileLocation].getBoundingBox(tileLocation)) && !this.GameLocation.terrainFeatures[tileLocation].isPassable(null))
            {
                return true;
            }
            if (this.GameLocation.largeTerrainFeatures.Count > 0)
            {
                foreach (StardewValley.TerrainFeatures.LargeTerrainFeature tf in this.GameLocation.largeTerrainFeatures)
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
            xTile.Tiles.Tile tmp = this.GameLocation.map.GetLayer("Back").PickTile(new Location(tileLocation.X * Game1.tileSize, tileLocation.Y * Game1.tileSize), viewport.Size);
            if (tmp != null)
            {
                tmp.TileIndexProperties.TryGetValue("Passable", out passable);
            }
            xTile.ObjectModel.PropertyValue passableBuilding = null;
            xTile.Tiles.Tile tile = this.GameLocation.map.GetLayer("Buildings").PickTile(new Location(tileLocation.X * Game1.tileSize, tileLocation.Y * Game1.tileSize), viewport.Size);
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
            Vector2[] neighbors = this.GetWalkableNeighbors(tile);
            for (int i = 1; i < 8; i += 2)
            {
                if (neighbors[i] == this.negativeOne)
                {
                    if ((neighbors[i - 1] != this.negativeOne) &&
                        (neighbors[i + 1 > 7 ? 0 : i + 1] != this.negativeOne))
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


    class Node
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
            this.items = new Node[100];
            this.items[0] = null;
            this.items[1] = head;
            this.Count = 1;

            this.positions = new Dictionary<Vector2, int>();
            this.positions.Add(head.position, 1);
        }

        public void Enqueue(Node item)
        {
            int position;
            if (!this.positions.TryGetValue(item.position, out position))
            {
                this.Count++;
                if (this.Count >= this.items.Length / 2)
                    this.ResizeItems();
                this.items[this.Count] = item;
                this.positions.Add(item.position, this.Count);
                this.PercolateUp(this.Count);
            }
            else if (this.items[position].f > item.f)
            {
                this.items[position] = item;
                this.PercolateUp(position);
            }
        }

        public Node Dequeue()
        {
            Node ret = this.items[1];
            this.items[1] = this.items[this.Count];
            this.items[this.Count] = null;
            this.positions.Remove(ret.position);
            if (this.Count > 1)
                this.positions[this.items[1].position] = 1;
            this.PercolateDown();
            this.Count--;
            return ret;
        }

        public Node Peek()
        {
            return this.items[1];
        }

        public float CheckValue(Vector2 nodeValue)
        {
            if (this.positions.TryGetValue(nodeValue, out int itemPosition))
                return this.items[itemPosition].f;
            return float.PositiveInfinity;
        }

        private void PercolateUp(int index)
        {
            while (this.items[index / 2] != null)
            {
                Node child = this.items[index];
                Node parent = this.items[index / 2];
                if (child.f < parent.f)
                {
                    this.items[index] = parent;
                    this.positions[parent.position] = index;
                    this.items[index / 2] = child;
                    this.positions[child.position] = index / 2;
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
                Node child_L = this.items[(index * 2)];
                Node child_R = this.items[(index * 2) + 1];
                Node parent = this.items[index];
                if (child_L != null && child_R != null)
                    replaceIndex = child_L.f < child_R.f ? (index * 2) : (index * 2) + 1;
                else if (child_L != null)
                    replaceIndex = index * 2;
                else
                    return;
                Node child = this.items[replaceIndex];
                if (child.f < parent.f)
                {
                    this.items[replaceIndex] = parent;
                    this.positions[parent.position] = replaceIndex;
                    this.items[index] = child;
                    this.positions[child.position] = index;
                    index = replaceIndex;
                }
            }
        }

        private void ResizeItems()
        {
            Node[] newItems = new Node[this.items.Length * 2];
            for (int i = 0; i < this.Count; i++)
                newItems[i] = this.items[i];
            this.items = newItems;
        }
    }
}
