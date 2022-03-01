/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

namespace Shockah.FlexibleSprinklers
{
	internal class ModConfig
	{
		internal enum SprinklerBehaviorEnum { Cluster, ClusterWithoutVanilla, Flexible, FlexibleWithoutVanilla, Vanilla }

		public SprinklerBehaviorEnum SprinklerBehavior { get; set; } = SprinklerBehaviorEnum.Cluster;
		public FlexibleSprinklerBehaviorTileWaterBalanceMode TileWaterBalanceMode { get; set; } = FlexibleSprinklerBehaviorTileWaterBalanceMode.Relaxed;
		public bool SplitDisconnectedClusters { get; set; } = true;
		public ClusterSprinklerBehaviorClusterOrdering ClusterBehaviorClusterOrdering { get; set; } = ClusterSprinklerBehaviorClusterOrdering.BiggerFirst;
		public ClusterSprinklerBehaviorBetweenClusterBalanceMode ClusterBehaviorBetweenClusterBalanceMode { get; set; } = ClusterSprinklerBehaviorBetweenClusterBalanceMode.Relaxed;
		public ClusterSprinklerBehaviorInClusterBalanceMode ClusterBehaviorInClusterBalanceMode { get; set; } = ClusterSprinklerBehaviorInClusterBalanceMode.Relaxed;
		public bool ActivateOnPlacement { get; set; } = false;
		public bool ActivateOnAction { get; set; } = false;
		public bool ActivateBeforeSleep { get; set; } = true;
		public float CoverageAlpha { get; set; } = 1;
		public float CoverageTimeInSeconds { get; set; } = 5;
		public float CoverageAnimationInSeconds { get; set; } = 1;
		public bool CoverageOverlayDuplicates { get; set; } = true;
		public bool ShowCoverageOnPlacement { get; set; } = true;
		public bool ShowCoverageOnAction { get; set; } = true;
		public int Tier1Power { get; set; } = 4;
		public int Tier2Power { get; set; } = 3 * 3 - 1;
		public int Tier3Power { get; set; } = 5 * 5 - 1;
		public int Tier4Power { get; set; } = 7 * 7 - 1;
		public int Tier5Power { get; set; } = 9 * 9 - 1;
		public int Tier6Power { get; set; } = 11 * 11 - 1;
		public int Tier7Power { get; set; } = 13 * 13 - 1;
		public int Tier8Power { get; set; } = 15 * 15 - 1;
		public bool CompatibilityMode { get; set; } = true;
	}
}
