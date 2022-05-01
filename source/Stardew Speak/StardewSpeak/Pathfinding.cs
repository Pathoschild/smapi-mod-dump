/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/evfredericksen/StardewSpeak
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using xTile.ObjectModel;
using xTile.Tiles;

namespace StardewSpeak.Pathfinder
{
	public class Location
	{
		public int X;
		public int Y;
		public int F;
		public int G;
		public int H;
		public Location Parent;
		public bool Preferable = false;
	}

	public class Point2
	{
		public int X;
		public int Y;

		public Point2(int x, int y)
		{
			this.X = x;
			this.Y = y;
		}
	}

	public class Pathfinder
	{

		private static readonly sbyte[,] Directions = new sbyte[4, 2]
			{
				{
					-1,
					0
				},
				{
					1,
					0
				},
				{
					0,
					1
				},
				{
					0,
					-1
				}
			};
		private static PriorityQueue<float> _openList = new PriorityQueue<float>();
		private static HashSet<int> _closedList = new HashSet<int>();
		private static int _counter = 0;
		public delegate bool isAtEnd(PathNode currentNode, Point endPoint, GameLocation location, Character c);
		public delegate bool adjustTileScore(PathNode currentNode, Point startPoint, GameLocation location, int currentScore);
		public static dynamic FindPath(GameLocation location, int startX, int startY, int targetX, int targetY, int limit = -1)
		{
			var startPoint = new Point(startX, startY);
			var endPoint = new Point(targetX, targetY);
			var path = findPath(startPoint, endPoint, PathFindController.isAtEndPoint, location, limit);
			return path?.Select(p => new { p.X, p.Y }).ToList();
		}

		public static Stack<Point> findPath(Point startPoint, Point endPoint, isAtEnd endPointFunction, GameLocation location, int limit = -1)
		{
			if (Interlocked.Increment(ref _counter) != 1)
			{
				throw new Exception();
			}
			try
			{
				bool endPointIsFarmer = endPoint.X == Game1.player.getTileX() && endPoint.Y == Game1.player.getTileY();
				_openList.Clear();
				_closedList.Clear();
				var openList = _openList;
				HashSet<int> closedList = _closedList;
				int iterations = 0;
				openList.Enqueue(new PathNode(startPoint.X, startPoint.Y, 0, null), Math.Abs(endPoint.X - startPoint.X) + Math.Abs(endPoint.Y - startPoint.Y));
				int layerWidth = location.map.Layers[0].LayerWidth;
				int layerHeight = location.map.Layers[0].LayerHeight;
				Character character = Game1.player;
				List<int> searchDirections = SearchDirections(startPoint, endPoint);
				while (!openList.IsEmpty())
				{
					PathNode currentNode = openList.Dequeue();
					if (endPointFunction(currentNode, endPoint, location, character))
					{
						return reconstructPath(currentNode);
					}
					closedList.Add(currentNode.id);
					int ng = (byte)(currentNode.g + 1);
					foreach (int i in searchDirections)
					{
						int nx = currentNode.x + Directions[i, 0];
						int ny = currentNode.y + Directions[i, 1];
						int nid = PathNode.ComputeHash(nx, ny);
						if (!closedList.Contains(nid))
						{
							bool isWalkable = isTileWalkable(location, nx, ny);
							bool isEndPoint = nx == endPoint.X && ny == endPoint.Y;
							bool isOffMap = nx < 0 || ny < 0 || nx >= layerWidth || ny >= layerHeight;
							if ((!isEndPoint && isOffMap) || !isWalkable)
							{
								closedList.Add(nid);
							}
							else
							{
								PathNode neighbor = new(nx, ny, currentNode);
								neighbor.g = (byte)(currentNode.g + 1);
								float f = ng + (Math.Abs(endPoint.X - nx) + Math.Abs(endPoint.Y - ny));
								closedList.Add(nid);
								openList.Enqueue(neighbor, f);
							}
						}
					}
					iterations++;
					if (limit >= 0)
					{
						if (iterations >= limit)
						{
							return null;
						}
					}
				}
				return null;
			}
			finally
			{
				if (Interlocked.Decrement(ref _counter) != 0)
				{
					throw new Exception();
				}
			}
		}

		public static Stack<Point> reconstructPath(PathNode finalNode)
		{
			Stack<Point> path = new Stack<Point>();
			path.Push(new Point(finalNode.x, finalNode.y));
			for (PathNode walk = finalNode.parent; walk != null; walk = walk.parent)
			{
				path.Push(new Point(walk.x, walk.y));
			}
			return path;
		}

		public static bool isTileWalkable(GameLocation location, int x, int y) 
		{
			//var layer = location.map.Layers[0];
			//if (x < 0 || y < 0 || x >= layer.LayerWidth || y >= layer.LayerHeight) return false;
			var rect = new Rectangle(x * 64 + 1, y * 64 + 1, 62, 62);
			return !location.isCollidingPosition(rect, Game1.viewport, true, 0, glider: false, Game1.player, pathfinding: true);
		}

		public static List<int> SearchDirections(Point start, Point end)
		{
			int xDiff = end.X - start.X;
			int yDiff = end.Y - start.Y;
			int first;
			int second;
			if (Math.Abs(yDiff) > Math.Abs(xDiff)) // north or south
			{
				first = yDiff > 0 ? 2 : 0;
				second = xDiff > 0 ? 1 : 3;
			}
			else // east or west
			{
				first = xDiff > 0 ? 1 : 3;
				second = yDiff > 0 ? 2 : 0;
			}
			int third = second < 2 ? second + 2 : second - 2;
			int fourth = first < 2 ? first + 2 : first - 2;
			return new List<int>() { first, second, third, fourth };
		}
	}

	class PriorityQueue<T>
	{
		private int total_size;

		private SortedDictionary<T, Queue<PathNode>> nodes;

		public PriorityQueue()
		{
			nodes = new SortedDictionary<T, Queue<PathNode>>();
			total_size = 0;
		}

		public bool IsEmpty()
		{
			return total_size == 0;
		}

		public void Clear()
		{
			total_size = 0;
			foreach (KeyValuePair<T, Queue<PathNode>> node in nodes)
			{
				node.Value.Clear();
			}
		}

		public bool Contains(PathNode p, T priority)
		{
			if (!nodes.TryGetValue(priority, out Queue<PathNode> v))
			{
				return false;
			}
			return v.Contains(p);
		}

		public PathNode Dequeue()
		{
			if (!IsEmpty())
			{
				foreach (Queue<PathNode> q in nodes.Values)
				{
					if (q.Count > 0)
					{
						total_size--;
						return q.Dequeue();
					}
				}
			}
			return null;
		}

		public object Peek()
		{
			if (!IsEmpty())
			{
				foreach (Queue<PathNode> q in nodes.Values)
				{
					if (q.Count > 0)
					{
						return q.Peek();
					}
				}
			}
			return null;
		}

		public object Dequeue(T priority)
		{
			total_size--;
			return nodes[priority].Dequeue();
		}

		public void Enqueue(PathNode item, T priority)
		{
			if (!nodes.ContainsKey(priority))
			{
				nodes.Add(priority, new Queue<PathNode>());
				Enqueue(item, priority);
			}
			else
			{
				nodes[priority].Enqueue(item);
				total_size++;
			}
		}
	}
}