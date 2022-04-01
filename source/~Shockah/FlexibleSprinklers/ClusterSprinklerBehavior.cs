/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using Shockah.CommonModCode;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shockah.FlexibleSprinklers
{
	internal enum ClusterSprinklerBehaviorClusterOrdering { SmallerFirst, BiggerFirst, Equally }
	internal enum ClusterSprinklerBehaviorBetweenClusterBalanceMode { Relaxed, Restrictive }
	internal enum ClusterSprinklerBehaviorInClusterBalanceMode { Relaxed, Exact, Restrictive }

	internal class ClusterSprinklerBehavior: ISprinklerBehavior
	{
		private class Cluster
		{
			public readonly ISet<IntPoint> Tiles = new HashSet<IntPoint>();
			public readonly ISet<(IntPoint position, SprinklerInfo info)> Sprinklers = new HashSet<(IntPoint position, SprinklerInfo info)>();

			public Cluster()
			{
			}

			public override string ToString()
				=> $"Cluster{{Tiles = {Tiles.Count}, Sprinklers = {Sprinklers.Count}}}";
		}

		private readonly ClusterSprinklerBehaviorClusterOrdering ClusterOrdering;
		private readonly ClusterSprinklerBehaviorBetweenClusterBalanceMode BetweenClusterBalanceMode;
		private readonly ClusterSprinklerBehaviorInClusterBalanceMode InClusterBalanceMode;
		private readonly ISprinklerBehavior.Independent? PriorityBehavior;

		private readonly IDictionary<IMap, (ISet<(IntPoint position, SprinklerInfo info)> sprinklers, IList<(ISet<IntPoint>, float)> tilesToWater)> Cache
			= new Dictionary<IMap, (ISet<(IntPoint position, SprinklerInfo info)> sprinklers, IList<(ISet<IntPoint>, float)> tilesToWater)>();

		public ClusterSprinklerBehavior(
			ClusterSprinklerBehaviorClusterOrdering clusterOrdering,
			ClusterSprinklerBehaviorBetweenClusterBalanceMode betweenClusterBalanceMode,
			ClusterSprinklerBehaviorInClusterBalanceMode inClusterBalanceMode,
			ISprinklerBehavior.Independent? priorityBehavior
		)
		{
			this.ClusterOrdering = clusterOrdering;
			this.BetweenClusterBalanceMode = betweenClusterBalanceMode;
			this.InClusterBalanceMode = inClusterBalanceMode;
			this.PriorityBehavior = priorityBehavior;
		}

		void ISprinklerBehavior.ClearCache()
		{
			Cache.Clear();
		}

		void ISprinklerBehavior.ClearCacheForMap(IMap map)
		{
			Cache.Remove(map);
		}

		public IList<(ISet<IntPoint>, float)> GetSprinklerTilesWithSteps(IMap map)
		{
			var sprinklersSet = map.GetAllSprinklers().ToHashSet();
			if (!Cache.TryGetValue(map, out var cachedInfo))
				return GetUncachedSprinklerTilesWithSteps(map, sprinklersSet);
			if (!cachedInfo.sprinklers.SetEquals(sprinklersSet))
				return GetUncachedSprinklerTilesWithSteps(map, sprinklersSet);
			return cachedInfo.tilesToWater;
		}

		private IList<(ISet<IntPoint>, float)> GetUncachedSprinklerTilesWithSteps(IMap map, ISet<(IntPoint position, SprinklerInfo info)> sprinklers)
		{
			IMap cachingMap;
			if (map is IMap.WithKnownSize mapWithKnownSize)
			{
				cachingMap = new KnownSizeCachingMap(mapWithKnownSize);
			}
			else
			{
				var maxSprinklerPower = sprinklers.Max(s => s.info.Power);
				var averageSprinklerX = sprinklers.Average(s => s.position.X);
				var averageSprinklerY = sprinklers.Average(s => s.position.Y);
				var averageSprinklerPosition = new IntPoint((int)Math.Round(averageSprinklerX), (int)Math.Round(averageSprinklerY));
				cachingMap = new CachingMap(map, averageSprinklerPosition, maxSprinklerPower * 2 + 1);
			}

			ICollection<(IntPoint position, SprinklerInfo info)>?[,] GetTileSprinklersGridForCluster(Cluster cluster, IEnumerable<Cluster> allClusters)
			{
				int minX = cluster.Tiles.Min(p => p.X);
				int maxX = cluster.Tiles.Max(p => p.X);
				int minY = cluster.Tiles.Min(p => p.Y);
				int maxY = cluster.Tiles.Max(p => p.Y);
				var grid = new ICollection<(IntPoint position, SprinklerInfo info)>?[maxX - minX + 1, maxY - minY + 1];

				foreach (var (sprinklerPosition, info) in cluster.Sprinklers)
				{
					var sprinklerClusterCount = GetClustersForSprinkler(sprinklerPosition, allClusters).Count();
					var sprinklerRange = FlexibleSprinklers.Instance.GetFloodFillSprinklerRange((int)Math.Ceiling(1.0 * info.Power / sprinklerClusterCount));

					ISet<IntPoint> @checked = new HashSet<IntPoint>();
					var toCheck = new LinkedList<(IntPoint point, int pathLength)>();

					foreach (var layoutPoint in info.Layout)
					{
						if ((Math.Abs((int)layoutPoint.X) == 1 && (int)layoutPoint.Y == 0) || (Math.Abs((int)layoutPoint.Y) == 1 && (int)layoutPoint.X == 0))
						{
							var point = sprinklerPosition + new IntPoint((int)layoutPoint.X, (int)layoutPoint.Y);
							if (cluster.Tiles.Contains(point))
								toCheck.AddLast((point, 1));
						}
					}

					while (toCheck.Count != 0)
					{
						var (point, pathLength) = toCheck.First!.Value;
						toCheck.RemoveFirst();
						if (@checked.Contains(point))
							continue;
						@checked.Add(point);

						var tileSprinklers = grid[point.X - minX, point.Y - minY] ?? new List<(IntPoint position, SprinklerInfo info)>();
						tileSprinklers.Add((sprinklerPosition, info));
						grid[point.X - minX, point.Y - minY] = tileSprinklers;

						if (pathLength >= sprinklerRange)
							continue;
						foreach (var neighbor in point.Neighbors)
						{
							if (!cluster.Tiles.Contains(neighbor))
								continue;
							toCheck.AddLast((neighbor, pathLength + 1));
						}
					}
				}

				return grid;
			}

			ICollection<Cluster> GetClusters()
			{
				IList<Cluster> clusters = new List<Cluster>();
				ISet<IntPoint> @checked = new HashSet<IntPoint>();
				var toCheck = new LinkedList<(IntPoint point, Cluster cluster)>();

				Cluster? GetClusterContainingTile(IntPoint point)
				{
					foreach (var cluster in clusters)
						if (cluster.Tiles.Contains(point))
							return cluster;
					return null;
				}

				void CombineClusters(Cluster clusterToRemove, Cluster clusterToMergeWith)
				{
					clusters.Remove(clusterToRemove);
					clusterToMergeWith.Tiles.UnionWith(clusterToRemove.Tiles);
					clusterToMergeWith.Sprinklers.UnionWith(clusterToRemove.Sprinklers);

					var current = toCheck.First;
					while (current is not null)
					{
						if (ReferenceEquals(current.Value.cluster, clusterToRemove))
							current.Value = (current.Value.point, clusterToMergeWith);
						current = current.Next;
					}
				}

				IDictionary<IntPoint, SprinklerInfo> sprinklerDictionary = new Dictionary<IntPoint, SprinklerInfo>();
				IDictionary<IntPoint, ISet<IntPoint>> sprinklerStartingPoints = new Dictionary<IntPoint, ISet<IntPoint>>();
				foreach (var (sprinklerPosition, info) in sprinklers)
				{
					sprinklerDictionary[sprinklerPosition] = info;
					var thisSprinklerStartingPoints = info.Layout
					   .Select(p => new IntPoint((int)p.X, (int)p.Y))
					   .Where(p => (Math.Abs(p.X) == 1 && p.Y == 0) || (Math.Abs(p.Y) == 1 && p.X == 0))
					   .Select(p => sprinklerPosition + p)
					   .Where(p => cachingMap[p] == SoilType.Waterable)
					   .ToHashSet();
					sprinklerStartingPoints[sprinklerPosition] = thisSprinklerStartingPoints;

					foreach (var sprinklerStartingPoint in thisSprinklerStartingPoints)
					{
						var cluster = GetClusterContainingTile(sprinklerStartingPoint);
						if (cluster is null)
						{
							cluster = new Cluster();
							cluster.Tiles.Add(sprinklerStartingPoint);
							clusters.Add(cluster);
							toCheck.AddLast((sprinklerStartingPoint, cluster));
						}
					}
				}

				while (toCheck.Count != 0)
				{
					var (point, cluster) = toCheck.First!.Value;
					toCheck.RemoveFirst();
					if (@checked.Contains(point))
					{
						var existingCluster = GetClusterContainingTile(point);
						if (existingCluster is not null && !ReferenceEquals(cluster, existingCluster))
							CombineClusters(cluster, existingCluster);
					}
					else
					{
						if (cachingMap[point] == SoilType.Waterable)
						{
							cluster.Tiles.Add(point);
							foreach (var neighbor in point.Neighbors)
								toCheck.AddLast((neighbor, cluster));
						}
						@checked.Add(point);
					}
				}

				void UpdateClusterSprinklers()
				{
					foreach (var cluster in clusters)
						cluster.Sprinklers.Clear();
					foreach (var (sprinklerPosition, thisSprinklerStartingPoints) in sprinklerStartingPoints)
					{
						foreach (var sprinklerStartingPoint in thisSprinklerStartingPoints)
						{
							var cluster = GetClusterContainingTile(sprinklerStartingPoint);
							cluster?.Sprinklers.Add((sprinklerPosition, sprinklerDictionary[sprinklerPosition]));
						}
					}
				}

				if (FlexibleSprinklers.Instance.Config.SplitDisconnectedClusters)
				{
					UpdateClusterSprinklers();
					IList<Cluster> clustersToAdd = new List<Cluster>();
					foreach (var cluster in clusters)
					{
						if (cluster.Tiles.Count == 0)
							continue;
						int minX = cluster.Tiles.Min(p => p.X);
						int minY = cluster.Tiles.Min(p => p.Y);
						var grid = GetTileSprinklersGridForCluster(cluster, clusters);

						var reachableTiles = cluster.Tiles.Where(t => (grid[t.X - minX, t.Y - minY]?.Count ?? 0) != 0).ToHashSet();
						if (reachableTiles.Count == cluster.Tiles.Count)
							continue;
						cluster.Tiles.Clear();
						cluster.Tiles.UnionWith(reachableTiles);

						while (reachableTiles.Count != 0)
						{
							var splitToCheck = new LinkedList<IntPoint>();
							splitToCheck.AddLast(reachableTiles.First());
							var thisClusterReachableTiles = new HashSet<IntPoint>();

							while (splitToCheck.Count != 0)
							{
								var point = splitToCheck.First!.Value;
								splitToCheck.RemoveFirst();
								thisClusterReachableTiles.Add(point);
								reachableTiles.Remove(point);

								foreach (var neighbor in point.Neighbors)
								{
									if (reachableTiles.Contains(neighbor) && !thisClusterReachableTiles.Contains(neighbor))
									{
										thisClusterReachableTiles.Add(neighbor);
										splitToCheck.AddLast(neighbor);
									}
								}
							}

							if (thisClusterReachableTiles.Count != 0 && thisClusterReachableTiles.Count != cluster.Tiles.Count)
							{
								var newCluster = new Cluster();
								newCluster.Tiles.UnionWith(thisClusterReachableTiles);
								clustersToAdd.Add(newCluster);
								cluster.Tiles.ExceptWith(thisClusterReachableTiles);
							}
						}
					}

					if (clustersToAdd.Count != 0)
						clusters = clusters.Where(c => c.Tiles.Count != 0).Union(clustersToAdd).ToList();
				}

				UpdateClusterSprinklers();
				return clusters;
			}

			IEnumerable<Cluster> GetClustersForSprinkler(IntPoint sprinklerPosition, IEnumerable<Cluster> clusters)
			{
				foreach (var cluster in clusters)
					foreach (var (clusterSprinklerPosition, _) in cluster.Sprinklers)
						if (clusterSprinklerPosition == sprinklerPosition)
							yield return cluster;
			}

			var clusters = GetClusters();
			IList<(ISet<IntPoint>, float)> priorityTilesToWaterSteps = new List<(ISet<IntPoint>, float)>();
			IList<ISet<IntPoint>> tilesToWaterSteps = new List<ISet<IntPoint>>();
			ISet<IntPoint> currentTilesToWater = new HashSet<IntPoint>();
			ISet<IntPoint> tilesToWater = new HashSet<IntPoint>();

			void WaterTile(IntPoint tilePosition)
			{
				tilesToWater.Add(tilePosition);
				currentTilesToWater.Add(tilePosition);
			}

			IDictionary<Cluster, IDictionary<IntPoint, int>> sprinklerTileCountToWaterPerCluster = new Dictionary<Cluster, IDictionary<IntPoint, int>>();
			foreach (var (sprinklerPosition, info) in sprinklers)
			{
				int tileCountToWaterLeft = info.Power;
				if (PriorityBehavior is not null)
				{
					foreach (var step in PriorityBehavior.GetSprinklerTilesWithSteps(cachingMap, sprinklerPosition, info))
					{
						var actuallyWaterableStepTiles = step.Item1.Where(t => cachingMap[t] == SoilType.Waterable).ToHashSet();
						priorityTilesToWaterSteps.Add((actuallyWaterableStepTiles, step.Item2));
						tileCountToWaterLeft -= actuallyWaterableStepTiles.Count;
					}
				}

				IList<Cluster> sprinklerClusters = clusters.Where(c => c.Sprinklers.Where(s => s.position == sprinklerPosition).Any()).ToList();
				if (sprinklerClusters.Count == 0)
					continue;

				void AddTileCountToWaterToCluster(int tileCount, Cluster cluster)
				{
					if (!sprinklerTileCountToWaterPerCluster.TryGetValue(cluster, out var sprinklerTileCountsToWater))
					{
						sprinklerTileCountsToWater = new Dictionary<IntPoint, int>();
						sprinklerTileCountToWaterPerCluster[cluster] = sprinklerTileCountsToWater;
					}

					if (!sprinklerTileCountsToWater.TryGetValue(sprinklerPosition, out int existingTileCountToWater))
						existingTileCountToWater = 0;
					sprinklerTileCountsToWater[sprinklerPosition] = existingTileCountToWater + tileCount;
					tileCountToWaterLeft -= tileCount;
				}

				int addEquallyPerCluster = tileCountToWaterLeft / sprinklerClusters.Count;
				if (addEquallyPerCluster > 0)
					foreach (var cluster in sprinklerClusters)
						AddTileCountToWaterToCluster(addEquallyPerCluster, cluster);

				while (tileCountToWaterLeft > 0)
				{
					IEnumerable<Cluster> nextClustersEnumerable = sprinklerClusters;
					switch (ClusterOrdering)
					{
						case ClusterSprinklerBehaviorClusterOrdering.SmallerFirst:
							nextClustersEnumerable = nextClustersEnumerable.OrderBy(c => c.Tiles.Count);
							break;
						case ClusterSprinklerBehaviorClusterOrdering.BiggerFirst:
							nextClustersEnumerable = nextClustersEnumerable.OrderByDescending(c => c.Tiles.Count);
							break;
						case ClusterSprinklerBehaviorClusterOrdering.Equally:
							break;
						default:
							throw new ArgumentException($"{nameof(ClusterSprinklerBehaviorClusterOrdering)} has an invalid value.");
					}

					var nextClusters = nextClustersEnumerable.ToList();
					addEquallyPerCluster = BetweenClusterBalanceMode == ClusterSprinklerBehaviorBetweenClusterBalanceMode.Relaxed
						? (int)Math.Ceiling(1.0 * tileCountToWaterLeft / nextClusters.Count)
						: (int)Math.Floor(1.0 * tileCountToWaterLeft / nextClusters.Count);
					if (addEquallyPerCluster > 0)
						foreach (var cluster in nextClusters)
							AddTileCountToWaterToCluster(addEquallyPerCluster, cluster);
					tileCountToWaterLeft = 0;
				}
			}

			var results = priorityTilesToWaterSteps.ToList();
			foreach (var cluster in clusters)
			{
				IList<ISet<IntPoint>> clusterSteps = new List<ISet<IntPoint>>();

				void FinishClusterWateringStep()
				{
					if (currentTilesToWater.Count == 0)
						return;
					clusterSteps.Add(currentTilesToWater.ToHashSet());
					currentTilesToWater.Clear();
				}

				int minX = cluster.Tiles.Min(p => p.X);
				int minY = cluster.Tiles.Min(p => p.Y);
				var grid = GetTileSprinklersGridForCluster(cluster, clusters);
				var totalTileCountToWater = sprinklerTileCountToWaterPerCluster.ContainsKey(cluster) ? sprinklerTileCountToWaterPerCluster[cluster].Values.Sum() : 0;
				if (totalTileCountToWater == 0)
					continue;
				var totalTileCount = cluster.Tiles.Count;
				var totalReachableTileCount = cluster.Tiles.Where(p => (grid[p.X - minX, p.Y - minY]?.Count ?? 0) != 0).Count();

				var averageSprinklerX = cluster.Sprinklers.Average(s => s.position.X);
				var averageSprinklerY = cluster.Sprinklers.Average(s => s.position.Y);
				var averageSprinklerPosition = new IntPoint((int)Math.Round(averageSprinklerX), (int)Math.Round(averageSprinklerY));
				var sortedReachableTiles = cluster.Tiles
					.Select(p => (
						tilePosition: p,
						sprinklerCount: grid[p.X - minX, p.Y - minY]?.Count ?? 0,
						distanceFromSprinklerCenter: Math.Sqrt(Math.Pow(p.X - averageSprinklerX, 2) + Math.Pow(p.Y - averageSprinklerY, 2)),
						distanceFromClosestSprinkler: cluster.Sprinklers.Select(s => s.position).Min(sp => Math.Sqrt(Math.Pow(p.X - sp.X, 2) + Math.Pow(p.Y - sp.Y, 2)))
					))
					.Where(e => e.sprinklerCount != 0)
					.OrderByDescending(e => e.sprinklerCount)
					.ThenBy(e => e.distanceFromClosestSprinkler + e.distanceFromSprinklerCenter)
					.ThenBy(e => e.distanceFromSprinklerCenter)
					.ToList();
				while (totalTileCountToWater > 0 && sortedReachableTiles.Count > 0)
				{
					var actuallyReachableTiles = sortedReachableTiles
						.Where(e => e.tilePosition.Neighbors.Where(neighbor => tilesToWater.Contains(neighbor) || cachingMap[neighbor] == SoilType.Sprinkler).Any())
						.ToList();
					if (actuallyReachableTiles.Count == 0)
						break; // TODO: log

					var (_, firstSprinklerCount, firstDistanceFromSprinklerCenter, firstDistanceFromClosestSprinkler) = actuallyReachableTiles.First();
					var stepTiles = actuallyReachableTiles
						.TakeWhile(e => e.sprinklerCount == firstSprinklerCount && e.distanceFromSprinklerCenter == firstDistanceFromSprinklerCenter && e.distanceFromClosestSprinkler == firstDistanceFromClosestSprinkler)
						.ToList();

					foreach (var stepTile in stepTiles)
						sortedReachableTiles.Remove(stepTile);

					if (totalTileCountToWater >= stepTiles.Count)
					{
						foreach (var (tilePosition, _, _, _) in stepTiles)
							WaterTile(tilePosition);
						totalTileCountToWater -= stepTiles.Count;
						FinishClusterWateringStep();
					}
					else
					{
						switch (InClusterBalanceMode)
						{
							case ClusterSprinklerBehaviorInClusterBalanceMode.Relaxed:
								foreach (var (tilePosition, _, _, _) in stepTiles)
									WaterTile(tilePosition);
								totalTileCountToWater -= stepTiles.Count;
								FinishClusterWateringStep();
								break;
							case ClusterSprinklerBehaviorInClusterBalanceMode.Restrictive:
								totalTileCountToWater = 0;
								break;
							case ClusterSprinklerBehaviorInClusterBalanceMode.Exact:
								var minD = stepTiles.Min(e => Math.Max(Math.Abs(e.tilePosition.X - averageSprinklerPosition.X), Math.Abs(e.tilePosition.Y - averageSprinklerPosition.Y)));
								var maxD = stepTiles.Max(e => Math.Max(Math.Abs(e.tilePosition.X - averageSprinklerPosition.X), Math.Abs(e.tilePosition.Y - averageSprinklerPosition.Y)));
								foreach (var spiralingTile in averageSprinklerPosition.GetSpiralingTiles(minD, maxD))
								{
									foreach (var (tilePosition, _, _, _) in stepTiles)
									{
										if (tilePosition == spiralingTile)
										{
											WaterTile(tilePosition);
											totalTileCountToWater--;
											FinishClusterWateringStep();
											if (totalTileCountToWater <= 0)
												goto done;
											break;
										}
									}
								}
								done:;
								break;
							default:
								throw new ArgumentException($"{nameof(ClusterSprinklerBehaviorInClusterBalanceMode)} has an invalid value.");
						}
					}
				}

				FinishClusterWateringStep();
				results = results
					.Union(clusterSteps.Select((step, index) => (step, (priorityTilesToWaterSteps.Count == 0 ? 0f : 1f) + 1f * index / (clusterSteps.Count - 1))))
					.ToList();
			}

			results = results
				.Select(step => priorityTilesToWaterSteps.Count == 0 ? step : (step.Item1, step.Item2 / 2f))
				.OrderBy(step => step.Item2)
				.ToList();
			Cache[map] = (sprinklers, results);
			return results;
		}
	}
}
