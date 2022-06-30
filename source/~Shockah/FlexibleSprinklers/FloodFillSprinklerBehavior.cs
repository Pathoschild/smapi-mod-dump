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
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shockah.FlexibleSprinklers
{
	internal enum FlexibleSprinklerBehaviorTileWaterBalanceMode { Relaxed, Exact, Restrictive }

	internal class FloodFillSprinklerBehavior: ISprinklerBehavior.Independent
	{
		private readonly FlexibleSprinklerBehaviorTileWaterBalanceMode TileWaterBalanceMode;
		private readonly ISprinklerBehavior.Independent? PriorityBehavior;

		private readonly IDictionary<IMap, IDictionary<(IntPoint position, SprinklerInfo info), IList<(ISet<IntPoint>, float)>>> Cache
			= new Dictionary<IMap, IDictionary<(IntPoint position, SprinklerInfo info), IList<(ISet<IntPoint>, float)>>>();

		public FloodFillSprinklerBehavior(FlexibleSprinklerBehaviorTileWaterBalanceMode tileWaterBalanceMode, ISprinklerBehavior.Independent? priorityBehavior)
		{
			this.TileWaterBalanceMode = tileWaterBalanceMode;
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

		public IList<(ISet<IntPoint>, float)> GetSprinklerTilesWithSteps(IMap map, IntPoint sprinklerPosition, SprinklerInfo info)
		{
			if (!Cache.TryGetValue(map, out var sprinklerCache))
				return GetUncachedSprinklerTilesWithSteps(map, sprinklerPosition, info);
			if (!sprinklerCache.TryGetValue((sprinklerPosition, info), out var cachedTiles))
				return GetUncachedSprinklerTilesWithSteps(map, sprinklerPosition, info);
			return cachedTiles;
		}

		private IList<(ISet<IntPoint>, float)> GetUncachedSprinklerTilesWithSteps(IMap map, IntPoint sprinklerPosition, SprinklerInfo info)
		{
			IList<(ISet<IntPoint>, float)> priorityWateredTilesSteps = new List<(ISet<IntPoint>, float)>();
			IList<ISet<IntPoint>> wateredTilesSteps = new List<ISet<IntPoint>>();
			ISet<IntPoint> currentWateredTiles = new HashSet<IntPoint>();
			ISet<IntPoint> wateredTiles = new HashSet<IntPoint>();
			var unwateredTileCount = info.Power;

			void FinishWateringStep()
			{
				if (currentWateredTiles.Count == 0)
					return;
				wateredTilesSteps.Add(currentWateredTiles.ToHashSet());
				currentWateredTiles.Clear();
			}

			void WaterTile(IntPoint tilePosition)
			{
				unwateredTileCount--;
				currentWateredTiles.Add(tilePosition);
				wateredTiles.Add(tilePosition);
			}

			void WaterTiles(IEnumerable<IntPoint> tilePositions)
			{
				foreach (var tilePosition in tilePositions)
					WaterTile(tilePosition);
			}

			if (PriorityBehavior is not null)
			{
				if (PriorityBehavior is not null)
				{
					foreach (var step in PriorityBehavior.GetSprinklerTilesWithSteps(map, sprinklerPosition, info))
					{
						var actuallyWaterableStepTiles = step.Item1.Where(t => map[t] == SoilType.Waterable).ToHashSet();
						priorityWateredTilesSteps.Add((actuallyWaterableStepTiles, step.Item2));
						unwateredTileCount -= actuallyWaterableStepTiles.Count;
					}
				}
			}
			if (unwateredTileCount <= 0)
				goto finish;

			int sprinklerRange, sprinkler1dRange;
			if (FlexibleSprinklers.Instance.Config.IgnoreRange)
			{
				sprinklerRange = int.MaxValue;
				sprinkler1dRange = (int)info.Layout.Max(t => Math.Max(Math.Abs(t.X), Math.Abs(t.Y)));
			}
			else
			{
				sprinklerRange = FlexibleSprinklers.Instance.GetSprinklerMaxRange(info);
				sprinkler1dRange = sprinklerRange * 2;
			}

			var waterableTiles = new HashSet<IntPoint>();
			ISet<IntPoint> otherSprinklers = new HashSet<IntPoint>();
			var @checked = new HashSet<IntPoint>();
			var toCheck = new Queue<IntPoint>();
			var maxCost = 0;

			var maxDX = Math.Max(wateredTiles.Count > 0 ? wateredTiles.Max(t => Math.Abs(t.X - sprinklerPosition.X)) : 0, sprinkler1dRange);
			var maxDY = Math.Max(wateredTiles.Count > 0 ? wateredTiles.Max(t => Math.Abs(t.Y - sprinklerPosition.Y)) : 0, sprinkler1dRange);

			var costArray = new int[maxDX * 2 + 1, maxDY * 2 + 1];
			var costArrayBaseXIndex = maxDX;
			var costArrayBaseYIndex = maxDY;

			for (int y = 0; y < costArray.GetLength(1); y++)
				for (int x = 0; x < costArray.GetLength(0); x++)
					costArray[x, y] = int.MaxValue;

			int GetCost(IntPoint point)
				=> costArray[costArrayBaseXIndex + point.X - sprinklerPosition.X, costArrayBaseYIndex + point.Y - sprinklerPosition.Y];

			void SetCost(IntPoint point, int cost)
				=> costArray[costArrayBaseXIndex + point.X - sprinklerPosition.X, costArrayBaseYIndex + point.Y - sprinklerPosition.Y] = cost;

			@checked.Add(sprinklerPosition);
			SetCost(sprinklerPosition, 0);
			foreach (var wateredTile in wateredTiles)
			{
				toCheck.Enqueue(wateredTile);
				SetCost(wateredTile, 0);
			}
			foreach (var neighbor in sprinklerPosition.Neighbors)
			{
				toCheck.Enqueue(neighbor);
				SetCost(neighbor, 1);
			}

			while (toCheck.Count > 0)
			{
				var tilePosition = toCheck.Dequeue();
				@checked.Add(tilePosition);

				var tilePathLength = GetCost(tilePosition);
				var newTilePathLength = tilePathLength + 1;

				if (waterableTiles.Count >= unwateredTileCount && newTilePathLength > maxCost)
					continue;

				switch (map[tilePosition])
				{
					case SoilType.Waterable:
						if (!wateredTiles.Contains(tilePosition))
							waterableTiles.Add(tilePosition);
						break;
					case SoilType.Sprinkler:
						otherSprinklers.Add(tilePosition);
						continue;
					case SoilType.NonWaterable:
						continue;
					default:
						throw new ArgumentException($"{nameof(SoilType)} has an invalid value.");
				}

				if (tilePathLength == sprinklerRange)
					continue;

				foreach (var neighbor in tilePosition.Neighbors)
				{
					if (@checked.Contains(neighbor) || Math.Abs(neighbor.X - sprinklerPosition.X) > maxDX || Math.Abs(neighbor.Y - sprinklerPosition.Y) > maxDY)
						continue;
					toCheck.Enqueue(neighbor);
					var newCost = Math.Min(GetCost(neighbor), newTilePathLength);
					SetCost(neighbor, newCost);
					maxCost = Math.Max(newCost, maxCost);
				}
			}

			var otherSprinklerDetectionRange = (int)Math.Sqrt(sprinklerRange - 1);

			IEnumerable<IntPoint> DetectSprinklers(IntPoint singleDirection)
			{
				int[] directions = { -1, 1 };
				
				for (int i = 1; i <= otherSprinklerDetectionRange; i++)
				{
					foreach (var direction in directions)
					{
						var position = sprinklerPosition + singleDirection * direction * i;
						if (!otherSprinklers.Contains(position))
							continue;
						if (map[position] == SoilType.Sprinkler)
							yield return position;
					}
				}
			}

			int? ClosestSprinkler(IntPoint singleDirection)
			{
				return DetectSprinklers(singleDirection)
					.Select(p => (Math.Abs(p.X - sprinklerPosition.X) + Math.Abs(p.Y - sprinklerPosition.Y)) as int?)
					.FirstOrDefault();
			}

			var sprinklerNeighbors = sprinklerPosition.Neighbors;
			var horizontalSprinklerDistance = ClosestSprinkler(IntPoint.Right);
			var verticalSprinklerDistance = ClosestSprinkler(IntPoint.Bottom);

			var sortedWaterableTiles = waterableTiles
				.Select(e => {
					var dx = Math.Abs(e.X - sprinklerPosition.X) * ((horizontalSprinklerDistance ?? 0) * sprinkler1dRange + 1);
					var dy = Math.Abs(e.Y - sprinklerPosition.Y) * ((verticalSprinklerDistance ?? 0) * sprinkler1dRange + 1);
					return (
						tilePosition: e,
						pathLength: GetCost(e),
						distance: Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2))
					);
				})
				.OrderBy(e => e.pathLength)
				.ThenBy(e => e.distance)
				.ToList();

			while (unwateredTileCount > 0 && sortedWaterableTiles.Count > 0)
			{
				var reachable = sortedWaterableTiles
					.Where(e => sprinklerNeighbors.Contains(e.tilePosition) || wateredTiles.SelectMany(t => t.Neighbors).Contains(e.tilePosition))
					.ToList();
				var currentDistance = reachable.First().distance;
				var tileEntries = reachable.TakeWhile(e => e.distance == currentDistance).ToList();
				if (tileEntries.Count == 0)
				{
					FlexibleSprinklers.Instance.Monitor.Log($"Could not find all tiles to water for sprinkler at {sprinklerPosition}.", LogLevel.Warn);
					break;
				}

				foreach (var tileEntry in tileEntries)
				{
					sortedWaterableTiles.Remove(tileEntry);
				}

				if (unwateredTileCount >= tileEntries.Count)
				{
					WaterTiles(tileEntries.Select(e => e.tilePosition));
					FinishWateringStep();
				}
				else
				{
					switch (TileWaterBalanceMode)
					{
						case FlexibleSprinklerBehaviorTileWaterBalanceMode.Relaxed:
							WaterTiles(tileEntries.Select(e => e.tilePosition));
							FinishWateringStep();
							break;
						case FlexibleSprinklerBehaviorTileWaterBalanceMode.Restrictive:
							unwateredTileCount = 0;
							break;
						case FlexibleSprinklerBehaviorTileWaterBalanceMode.Exact:
							var minD = tileEntries.Min(e => Math.Max(Math.Abs(e.tilePosition.X - sprinklerPosition.X), Math.Abs(e.tilePosition.Y - sprinklerPosition.Y)));
							var maxD = tileEntries.Max(e => Math.Max(Math.Abs(e.tilePosition.X - sprinklerPosition.X), Math.Abs(e.tilePosition.Y - sprinklerPosition.Y)));
							foreach (var spiralingTile in sprinklerPosition.GetSpiralingTiles(minD, maxD))
							{
								foreach (var tileEntry in tileEntries)
								{
									if (tileEntry.tilePosition == spiralingTile)
									{
										WaterTile(tileEntry.tilePosition);
										FinishWateringStep();
										tileEntries.Remove(tileEntry);
										if (unwateredTileCount <= 0)
											goto done;
										break;
									}
								}
							}
							done:;
							break;
						default:
							throw new ArgumentException($"{nameof(FlexibleSprinklerBehaviorTileWaterBalanceMode)} has an invalid value.");
					}
				}
			}

			finish:;
			if (!Cache.TryGetValue(map, out var sprinklerCache))
			{
				sprinklerCache = new Dictionary<(IntPoint position, SprinklerInfo info), IList<(ISet<IntPoint>, float)>>();
				Cache[map] = sprinklerCache;
			}

			FinishWateringStep();
			var results = priorityWateredTilesSteps
				.Union(wateredTilesSteps.Select((step, index) => (step, (priorityWateredTilesSteps.Count == 0 ? 0f : 1f) + 1f * index / (wateredTilesSteps.Count - 1))))
				.Select(step => priorityWateredTilesSteps.Count == 0 ? step : (step.Item1, step.Item2 / 2f))
				.ToList();
			sprinklerCache[(sprinklerPosition, info)] = results;
			return results;
		}
	}
}