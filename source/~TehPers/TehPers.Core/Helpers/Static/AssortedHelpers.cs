using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using StardewValley;

namespace TehPers.Core.Helpers.Static {
    public static class AssortedHelpers {
        private static readonly MethodInfo _genericCast = typeof(AssortedHelpers).GetMethod(nameof(AssortedHelpers.GenericCast), BindingFlags.NonPublic | BindingFlags.Static);
        private static readonly Type[] _primitiveTypes = { typeof(bool), typeof(byte), typeof(char), typeof(DateTime), typeof(decimal), typeof(double), typeof(short), typeof(int), typeof(long), typeof(sbyte), typeof(float), typeof(string), typeof(ushort), typeof(uint), typeof(ulong) };
        private static readonly Assembly _sdv = (Type.GetType("StardewValley.Game1, StardewValley") ?? Type.GetType("StardewValley.Game1, Stardew Valley"))?.Assembly;

        /// <summary>Safely casts an object to another type with a fallback if the cast fails</summary>
        /// <typeparam name="T">The type to cast to</typeparam>
        /// <param name="o">The object to cast</param>
        /// <param name="fallback">The fallback value if the cast fails</param>
        /// <returns>If the cast succeeds, <see cref="o"/> as <see cref="T"/>, otherwise <see cref="fallback"/></returns>
        public static T As<T>(this object o, T fallback = default(T)) => o is T t ? t : fallback;

        /// <summary>Super slow way of casting things to other types. Target type only needs to be known at runtime.</summary>
        /// <param name="obj">The object to cast.</param>
        /// <param name="target">The type to cast to.</param>
        /// <returns>The casted object, or null if it couldn't be cast.</returns>
        public static object DynamicCast(this object obj, Type target) {
            Type objType = obj.GetType();

            // Check if already the target type
            if (objType == target)
                return obj;

            // Check if it can be converted
            if (obj is IConvertible && AssortedHelpers._primitiveTypes.Contains(target))
                return Convert.ChangeType(obj, target);

            // Check if they can be directly assigned
            if (target.IsAssignableFrom(objType))
                return AssortedHelpers._genericCast.MakeGenericMethod(target).Invoke(null, new[] { obj });

            return null;

        }

        private static TDest GenericCast<TSource, TDest>(TSource obj) where TSource : TDest {
            return obj;
        }

        /// <summary>Gets a type from the SDV assembly.</summary>
        /// <param name="typeName">The name of the type. You may use <code>nameof</code> (which returns <see cref="Type.Name"/>) or a namespace qualified name (equivalent to <see cref="Type.FullName"/>).</param>
        /// <returns>The <see cref="Type"/> with the given name, or <code>null</code> if not found.</returns>
        /// <remarks>Try not to call this method too often. Cache the result somewhere if possible.</remarks>
        public static Type GetSDVType(string typeName) {
            // Search for the type
            Type[] types = AssortedHelpers._sdv.GetTypes();
            return types.FirstOrDefault(t => t.Name == typeName || t.FullName == typeName);
        }

        public static IEnumerable<Point> GetFloodedTiles(this GameLocation location, int? maxDistance) {
            if (location.waterTiles == null)
                return Enumerable.Empty<Point>();

            // Get a list of all the water tile coordinates
            int width = location.waterTiles.GetLength(0);
            int height = location.waterTiles.GetLength(1);
            List<Point> waterTiles = new List<Point>();
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    if (location.waterTiles[x, y]) {
                        waterTiles.Add(new Point(x, y));
                    }
                }
            }

            // Traverse the graph
            return waterTiles.Traverse(GetNeighbors).Select(node => node.Value);

            // Neighbors function
            IEnumerable<Point> GetNeighbors(GraphNode<Point> cur) {
                // Check if this is at the max possible distance before adding neighbors
                if (maxDistance != null && cur.TotalCost >= maxDistance)
                    return Enumerable.Empty<Point>();

                // Filter out each neighboring point
                return (new[] {
                    new Point(cur.Value.X, cur.Value.Y + 1),
                    new Point(cur.Value.X, cur.Value.Y - 1),
                    new Point(cur.Value.X + 1, cur.Value.Y),
                    new Point(cur.Value.X - 1, cur.Value.Y)
                }).Where(neighbor => {
                    // Make sure it's a valid tile
                    if (neighbor.X < 0 || neighbor.Y < 0 || neighbor.X >= width || neighbor.Y >= height)
                        return false;

                    // Make sure there's no fence there
                    if (location.Objects.ContainsKey(new Vector2(neighbor.X, neighbor.Y)))
                        return false;

                    // It's a valid tile
                    return true;
                });
            }
        }

        /// <summary>Performs any number of simultaneous breadth-first traversals over a graph, returning each vertex that was reached.</summary>
        /// <typeparam name="TVertex">The type of the vertex in the graph.</typeparam>
        /// <param name="starts">All vertices that the traversals should begin at.</param>
        /// <param name="getNeighbors">A function returning all vertices the given vertex is connected to.</param>
        /// <param name="equalityComparer">Equality comparer for vertices to avoid the same vertex being checked multiple times.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> containing each vertex that was reached.</returns>
        public static IEnumerable<GraphNode<TVertex>> Traverse<TVertex>(this IEnumerable<TVertex> starts, Func<GraphNode<TVertex>, IEnumerable<TVertex>> getNeighbors, IEqualityComparer<TVertex> equalityComparer = null) {
            Queue<GraphNode<TVertex>> open = new Queue<GraphNode<TVertex>>(starts.Select(s => new GraphNode<TVertex>(s, 0)));
            HashSet<TVertex> closed = equalityComparer != null ? new HashSet<TVertex>(equalityComparer) : new HashSet<TVertex>();

            // As long as there's any open nodes
            while (open.Any()) {
                // Take the closest one (the first in the queue will always be closest for breadth-first)
                // As a side note, since this is just a traversal, it doesn't matter which one we take since we're going to visit them all
                GraphNode<TVertex> cur = open.Dequeue();
                if (!closed.Add(cur.Value))
                    continue;

                // Return this node
                yield return cur;

                // Enqueue all the neighbors of this node
                foreach (TVertex neighbor in getNeighbors(cur)) {
                    open.Enqueue(new GraphNode<TVertex>(neighbor, cur.TotalCost + 1));
                }
            }
        }

        /// <summary>Wrapper class for vertices which represents a node in a graph.</summary>
        /// <typeparam name="TVertex">The type of the vertex in the graph.</typeparam>
        public class GraphNode<TVertex> {
            public TVertex Value { get; set; }
            public int TotalCost { get; set; }

            public GraphNode(TVertex value, int totalCost) {
                this.Value = value;
                this.TotalCost = totalCost;
            }
        }
    }
}
