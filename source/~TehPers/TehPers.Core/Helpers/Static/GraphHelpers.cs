using System.Collections.Generic;
using System.Linq;
using TehPers.Core.Collections;

namespace TehPers.Core.Helpers.Static {
    public static class GraphHelpers {
        /// <summary>Performs an A* pathfind on a graph from any number of starting nodes to any number of target nodes.</summary>
        /// <typeparam name="T">The type of node in the graph.</typeparam>
        /// <param name="starts">The starting nodes.</param>
        /// <param name="targets">The target nodes.</param>
        /// <param name="getNeighbors">A function which returns all the neighbors and their costs of a given node.</param>
        /// <param name="getHeuristic">A function which returns the estimated distance to a target, or null for no heuristic.</param>
        /// <param name="comparer">The equality comparer for <see cref="T"/>, or null to use the default one.</param>
        /// <returns>A stack containing the shortest path from a start to a target (including the start and target), or null if no path was found.</returns>
        public static Stack<T> FindPath<T>(IEnumerable<T> starts, IEnumerable<T> targets, GetNeighbors<T> getNeighbors, GetHeuristic<T> getHeuristic = null, IEqualityComparer<T> comparer = null) {
            // Turn the targets into a hash set to speed up checking if things are in it later
            targets = targets.ToHashSet();

            // Create a min heap and closed set
            MinHeap<GraphNode<T>> open = new MinHeap<GraphNode<T>>(starts.Select(node => new GraphNode<T>(node, null, 0)), (a, b) => a.F.CompareTo(b.F));
            HashSet<T> closed = comparer == null ? new HashSet<T>() : new HashSet<T>(comparer);

            // Loop as long as there are open nodes
            while (open.Any()) {
                // Grab the first node from the heap
                GraphNode<T> cur = open.RemoveFirst();

                // Make sure this node isn't closed
                if (closed.Contains(cur.Value))
                    continue;

                // Check if this node is a target
                if (targets.Contains(cur.Value)) {
                    Stack<T> path = new Stack<T>();
                    while (cur != null) {
                        path.Push(cur.Value);
                        cur = cur.Parent;
                    }

                    return path;
                }

                // Close this node
                closed.Add(cur.Value);

                // Add all of its neighbors
                foreach (KeyValuePair<T, double> neighbor in getNeighbors(cur)) {
                    open.Add(new GraphNode<T>(neighbor.Key, cur, cur.G + neighbor.Value, getHeuristic?.Invoke(neighbor.Key) ?? 0));
                }
            }

            return null;
        }

        public delegate IEnumerable<KeyValuePair<T, double>> GetNeighbors<T>(GraphNode<T> node);

        public delegate double GetHeuristic<in T>(T node);

        public class GraphNode<T> {
            public GraphNode<T> Parent { get; }
            public double F => this.G + this.H;
            public double G { get; }
            public double H { get; }
            public T Value { get; }

            public GraphNode(T value, GraphNode<T> parent, double g) {
                this.Value = value;
                this.Parent = parent;
                this.G = g;
                this.H = 0;
            }

            public GraphNode(T value, GraphNode<T> parent, double g, double h) {
                this.Value = value;
                this.Parent = parent;
                this.G = g;
                this.H = h;
            }

            public override string ToString() {
                return $"{this.Value} (F: {this.F}, G: {this.G}, H: {this.H})";
            }
        }
    }
}
