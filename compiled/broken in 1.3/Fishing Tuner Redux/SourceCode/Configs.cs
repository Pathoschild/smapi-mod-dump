using StardewValley;
using StardewModdingAPI;
using Microsoft.Xna.Framework;

namespace FishingTunerRedux {
	public class GeneralSettingsConfig {
		public string RefreshConfigKey { get; set; }
		public bool VerboseLogging { get; set; }
		public bool RecatchLegendaries { get; set; }
		public bool RecatchLegendariesOnceDaily { get; set; }

		public bool AlwaysPerfect { get; set; }
		public bool AlwaysFindTreasure { get; set; }
		public bool InfiniteTackle { get; set; }
		public bool InfiniteBait { get; set; }

		public float BaseDifficultyMultiplier { get; set; }
		public int BaseDifficultyAdditive { get; set; }
		public int BaseBobberSize { get; set; }
		public int BaseFishSizeReductionTimer { get; set; }
		public float BaseTreasureChance { get; set; }
		public float BaseFishCatchPercentage { get; set; }
		public float BaseNewFishTypeCatchPercentage { get; set; }
		public float BossFishInitialCatchPercentageAdditive { get; set; }
		public float BossFishInitialCatchPercentageMultiplier { get; set; }
		public float BaseFishCatchProgressGainRate { get; set; }
		public float BaseFishCatchProgressLossRate { get; set; }
		public float BaseTreasureCatchProgressGainRate { get; set; }
		public float BaseTreasureCatchProgressLossRate { get; set; }
		public float BaseFishAccelerationMultiplier { get; set; }
		public float BaseBobberBarAccelerationMultiplier { get; set; }
		public float BaseGravityFactor { get; set; }
		public float BaseHookStickiness { get; set; }
		public float BaseBounciness { get; set; }
		public int MinTreasureAppearanceTime { get; set; }
		public int MaxTreasureAppearanceTime { get; set; }
		public int MinTimeUntilBite { get; set; }
		public int MaxTimeUntilBite { get; set; }
		public int MinNibbleTime { get; set; }
		public int MaxNibbleTime { get; set; }

		public GeneralSettingsConfig() {
			RefreshConfigKey = "F5";
			VerboseLogging = false;
			RecatchLegendaries = false;
			RecatchLegendariesOnceDaily = true;
			AlwaysPerfect = false;
			AlwaysFindTreasure = false;
			InfiniteTackle = false;
			InfiniteBait = false;

			BaseDifficultyMultiplier = 1.0f;
			BaseDifficultyAdditive = 0;
			BaseBobberSize = 96;
			BaseFishSizeReductionTimer = 800;
			BaseTreasureChance = 0.15f;
			BaseFishCatchPercentage = 0.3f;
			BaseNewFishTypeCatchPercentage = 0.1f;
			BossFishInitialCatchPercentageAdditive = 0;
			BossFishInitialCatchPercentageMultiplier = 1.0f;
			BaseFishCatchProgressGainRate = 1f / 500f;
			BaseFishCatchProgressLossRate = 3f / 1000f;
			BaseTreasureCatchProgressGainRate = 0.0135f;
			BaseTreasureCatchProgressLossRate = 0.01f;
			BaseFishAccelerationMultiplier = 1.0f;
			BaseBobberBarAccelerationMultiplier = 0.6f;
			BaseGravityFactor = 0;
			BaseHookStickiness = 0;
			BaseBounciness = 2f / 3f;
			MinTreasureAppearanceTime = 1000;
			MaxTreasureAppearanceTime = 3000;
			MinTimeUntilBite = 600;
			MaxTimeUntilBite = 30000;
			MinNibbleTime = 340;
			MaxNibbleTime = 800;
		}
	}

	public abstract class SettingsConfig {
		public float BobberBarSizeMultiplier { get; set; }
		public int BobberBarSizeAdditive { get; set; }
		public float TreasureChanceMultiplier { get; set; }
		public float TreasureChanceAdditive { get; set; }
		public float InitialFishCatchPercentageMultiplier { get; set; }
		public float InitialFishCatchPercentageAdditive { get; set; }
		public float InitialNewFishTypeCatchPercentageMultiplier { get; set; }
		public float InitialNewFishTypeCatchPercentageAdditive { get; set; }
		public float InitialTreasureCatchPercentageMultiplier { get; set; }
		public float InitialTreasureCatchPercentageAdditive { get; set; }
		public float FishCatchProgressGainRateMultiplier { get; set; }
		public float FishCatchProgressGainRateAdditive { get; set; }
		public float FishCatchProgressLossRateMultiplier { get; set; }
		public float FishCatchProgressLossRateAdditive { get; set; }
		public float TreasureCatchProgressGainRateMultiplier { get; set; }
		public float TreasureCatchProgressGainRateAdditive { get; set; }
		public float TreasureCatchProgressLossRateMultiplier { get; set; }
		public float TreasureCatchProgressLossRateAdditive { get; set; }
		public float FishEscapeDuringTreasureMultiplier { get; set; }
		public float FishEscapeDuringTreasureAdditive { get; set; }
		public float FishAccelerationMultiplier { get; set; }
		public float FishSizeReductionTimerMultiplier { get; set; }
		public int FishSizeReductionTimerAdditive { get; set; }
		public float GravityFactorMultiplier { get; set; }
		public float GravityFactorAdditive { get; set; }
		public float HookStickinessMultiplier { get; set; }
		public float HookStickinessAdditive { get; set; }
		public float HookAccelerationMultiplier { get; set; }
		public float BobberBarBottomBounciness { get; set; }
		public float BobberBarTopBounciness { get; set; }

		public SettingsConfig() {
			BobberBarSizeMultiplier = 1;
			BobberBarSizeAdditive = 0;
			TreasureChanceMultiplier = 1;
			TreasureChanceAdditive = 0;
			InitialFishCatchPercentageMultiplier = 1;
			InitialFishCatchPercentageAdditive = 0;
			InitialNewFishTypeCatchPercentageMultiplier = 1;
			InitialNewFishTypeCatchPercentageAdditive = 0;
			InitialTreasureCatchPercentageMultiplier = 1;
			InitialTreasureCatchPercentageAdditive = 0;
			FishCatchProgressGainRateMultiplier = 1;
			FishCatchProgressGainRateAdditive = 0;
			FishCatchProgressLossRateMultiplier = 1;
			FishCatchProgressLossRateAdditive = 0;
			TreasureCatchProgressGainRateMultiplier = 1;
			TreasureCatchProgressGainRateAdditive = 0;
			TreasureCatchProgressLossRateMultiplier = 1;
			TreasureCatchProgressLossRateAdditive = 0;
			FishEscapeDuringTreasureMultiplier = 1;
			FishEscapeDuringTreasureAdditive = 0;
			FishAccelerationMultiplier = 1;
			FishSizeReductionTimerMultiplier = 1;
			FishSizeReductionTimerAdditive = 0;
			GravityFactorMultiplier = 1;
			GravityFactorAdditive = 0;
			HookStickinessMultiplier = 1;
			HookStickinessAdditive = 0;
			HookAccelerationMultiplier = 1;
			BobberBarBottomBounciness = 1;
			BobberBarTopBounciness = 1;
		}
	}

	public class FishDifficultyConfig : SettingsConfig {
		public FishDifficultyConfig() : base() {
			BobberBarSizeMultiplier = 0;
			TreasureChanceMultiplier = 0;
			InitialFishCatchPercentageMultiplier = 0;
			InitialNewFishTypeCatchPercentageMultiplier = 0;
			InitialTreasureCatchPercentageMultiplier = 0;
			FishCatchProgressGainRateMultiplier = 0;
			FishCatchProgressLossRateMultiplier = 0;
			TreasureCatchProgressGainRateMultiplier = 0;
			TreasureCatchProgressLossRateMultiplier = 0;
			FishEscapeDuringTreasureMultiplier = 0;
			FishAccelerationMultiplier = 0;
			FishSizeReductionTimerMultiplier = 0;
			GravityFactorMultiplier = 0;
			HookStickinessMultiplier = 0;
			HookAccelerationMultiplier = 0;
			BobberBarBottomBounciness = 0;
			BobberBarTopBounciness = 0;
		}
	}

	public abstract class CustomizedSettingsConfig : SettingsConfig {
		public float DifficultyMultiplier { get; set; }
		public int DifficultyAdditive { get; set; }
		public float MinTimeUntilBiteAdditive { get; set; }
		public float MinTimeUntilBiteMultiplier { get; set; }
		public float MaxTimeUntilBiteAdditive { get; set; }
		public float MaxTimeUntilBiteMultiplier { get; set; }
		public float MinNibbleTimeAdditive { get; set; }
		public float MinNibbleTimeMultiplier { get; set; }
		public float MaxNibbleTimeAdditive { get; set; }
		public float MaxNibbleTimeMultiplier { get; set; }

		public CustomizedSettingsConfig() : base() {
			DifficultyMultiplier = 1;
			DifficultyAdditive = 0;
			MinTimeUntilBiteAdditive = 0;
			MinTimeUntilBiteMultiplier = 1;
			MaxTimeUntilBiteAdditive = 0;
			MaxTimeUntilBiteMultiplier = 1;
			MinNibbleTimeAdditive = 0;
			MinNibbleTimeMultiplier = 1;
			MaxNibbleTimeAdditive = 0;
			MaxNibbleTimeMultiplier = 1;
		}
	}

	public class FishingPointConfig : CustomizedSettingsConfig {

		public FishingPointConfig() : base() {
			MinTimeUntilBiteMultiplier = 0.25f;
			MaxTimeUntilBiteMultiplier = 0.25f;
		}
	}

	public class CastDistanceConfig : CustomizedSettingsConfig {
		public CastDistanceConfig() : base() {
			BobberBarSizeMultiplier = 0;
			TreasureChanceMultiplier = 0;
			InitialFishCatchPercentageMultiplier = 0;
			InitialNewFishTypeCatchPercentageMultiplier = 0;
			InitialTreasureCatchPercentageMultiplier = 0;
			FishCatchProgressGainRateMultiplier = 0;
			FishCatchProgressLossRateMultiplier = 0;
			TreasureCatchProgressGainRateMultiplier = 0;
			TreasureCatchProgressLossRateMultiplier = 0;
			FishEscapeDuringTreasureMultiplier = 0;
			FishAccelerationMultiplier = 0;
			FishSizeReductionTimerMultiplier = 0;
			GravityFactorMultiplier = 0;
			HookStickinessMultiplier = 0;
			HookAccelerationMultiplier = 0;
			BobberBarBottomBounciness = 0;
			BobberBarTopBounciness = 0;
			DifficultyAdditive = 0;
			DifficultyMultiplier = 0;
			MinTimeUntilBiteAdditive = 0;
			MinTimeUntilBiteMultiplier = 0;
			MaxTimeUntilBiteAdditive = 0;
			MaxTimeUntilBiteMultiplier = 0;
			MinNibbleTimeAdditive = 0;
			MinNibbleTimeMultiplier = 0;
			MaxNibbleTimeAdditive = 0;
			MaxNibbleTimeMultiplier = 0;
		}
	}

	public class DailyLuckConfig : CustomizedSettingsConfig {
		public DailyLuckConfig() : base() {
			BobberBarSizeMultiplier = 0;
			TreasureChanceMultiplier = 0;
			InitialFishCatchPercentageMultiplier = 0;
			InitialNewFishTypeCatchPercentageMultiplier = 0;
			InitialTreasureCatchPercentageMultiplier = 0;
			FishCatchProgressGainRateMultiplier = 0;
			FishCatchProgressLossRateMultiplier = 0;
			TreasureCatchProgressGainRateMultiplier = 0;
			TreasureCatchProgressLossRateMultiplier = 0;
			FishEscapeDuringTreasureMultiplier = 0;
			FishAccelerationMultiplier = 0;
			FishSizeReductionTimerMultiplier = 0;
			GravityFactorMultiplier = 0;
			HookStickinessMultiplier = 0;
			HookAccelerationMultiplier = 0;
			BobberBarBottomBounciness = 0;
			BobberBarTopBounciness = 0;
			DifficultyMultiplier = 0;
			MinTimeUntilBiteMultiplier = 0;
			MaxTimeUntilBiteMultiplier = 0;
			MinNibbleTimeMultiplier = 0;
			MaxNibbleTimeMultiplier = 0;

			TreasureChanceAdditive = 0.5f;
		}
	}

	public class PlayerLuckConfig : CustomizedSettingsConfig {
		public PlayerLuckConfig() : base() {
			BobberBarSizeMultiplier = 0;
			TreasureChanceMultiplier = 0;
			InitialFishCatchPercentageMultiplier = 0;
			InitialNewFishTypeCatchPercentageMultiplier = 0;
			InitialTreasureCatchPercentageMultiplier = 0;
			FishCatchProgressGainRateMultiplier = 0;
			FishCatchProgressLossRateMultiplier = 0;
			TreasureCatchProgressGainRateMultiplier = 0;
			TreasureCatchProgressLossRateMultiplier = 0;
			FishEscapeDuringTreasureMultiplier = 0;
			FishAccelerationMultiplier = 0;
			FishSizeReductionTimerMultiplier = 0;
			GravityFactorMultiplier = 0;
			HookStickinessMultiplier = 0;
			HookAccelerationMultiplier = 0;
			BobberBarBottomBounciness = 0;
			BobberBarTopBounciness = 0;
			DifficultyMultiplier = 0;
			MinTimeUntilBiteMultiplier = 0;
			MaxTimeUntilBiteMultiplier = 0;
			MinNibbleTimeMultiplier = 0;
			MaxNibbleTimeMultiplier = 0;

			TreasureChanceAdditive = 0.005f;
		}
	}

	public class FishingLevelConfig : CustomizedSettingsConfig {
		public FishingLevelConfig() : base() {
			BobberBarSizeMultiplier = 0;
			TreasureChanceMultiplier = 0;
			InitialFishCatchPercentageMultiplier = 0;
			InitialNewFishTypeCatchPercentageMultiplier = 0;
			InitialTreasureCatchPercentageMultiplier = 0;
			FishCatchProgressGainRateMultiplier = 0;
			FishCatchProgressLossRateMultiplier = 0;
			TreasureCatchProgressGainRateMultiplier = 0;
			TreasureCatchProgressLossRateMultiplier = 0;
			FishEscapeDuringTreasureMultiplier = 0;
			FishAccelerationMultiplier = 0;
			FishSizeReductionTimerMultiplier = 0;
			GravityFactorMultiplier = 0;
			HookStickinessMultiplier = 0;
			HookAccelerationMultiplier = 0;
			BobberBarBottomBounciness = 0;
			BobberBarTopBounciness = 0;
			DifficultyMultiplier = 0;
			MinTimeUntilBiteMultiplier = 0;
			MaxTimeUntilBiteMultiplier = 0;
			MinNibbleTimeMultiplier = 0;
			MaxNibbleTimeMultiplier = 0;

			MaxTimeUntilBiteAdditive = -250;
		}
	}

	public class BaitConfig : CustomizedSettingsConfig {
		public BaitConfig() : base() {
			MaxTimeUntilBiteMultiplier = 0.5f;
		}
	}

	public class MagnetConfig : CustomizedSettingsConfig {
		public MagnetConfig() : base() {
			TreasureChanceAdditive = 0.15f;
		}
	}

	public class WildBaitConfig : CustomizedSettingsConfig {
		public WildBaitConfig() : base() {
			MaxTimeUntilBiteMultiplier = 0.375f;
		}
	}

	public class SpinnerConfig : CustomizedSettingsConfig {
		public SpinnerConfig() : base() {
			MaxTimeUntilBiteAdditive = -5000;
		}
	}

	public class DressedSpinnerConfig : CustomizedSettingsConfig {
		public DressedSpinnerConfig() : base() {
			MaxTimeUntilBiteAdditive = -10000;
		}
	}

	public class BarbedHookConfig : CustomizedSettingsConfig {
		public BarbedHookConfig() : base() {
			HookStickinessAdditive = 0.2f;
			HookAccelerationMultiplier = 0.5f;
		}
	}

	public class LeadBobberConfig : CustomizedSettingsConfig {
		public LeadBobberConfig() : base() {
			BobberBarBottomBounciness = 0.100000001490116f;
		}
	}

	public class TreasureHunterConfig : CustomizedSettingsConfig {
		public TreasureHunterConfig() : base() {
			TreasureChanceAdditive = 0.05f;
			FishEscapeDuringTreasureMultiplier = 0;
		}
	}

	public class TrapBobberConfig : CustomizedSettingsConfig {
		public TrapBobberConfig() : base() {
			FishCatchProgressLossRateMultiplier = 1.0f / 3.0f;
		}
	}

	public class CorkBobberConfig : CustomizedSettingsConfig {
		public CorkBobberConfig() : base() {
			BobberBarSizeAdditive = 24;
		}
	}

	public abstract class MotionConfig : SettingsConfig {
		public float DifficultyMultiplier { get; set; }
		public int DifficultyAdditive { get; set; }
		public float SinkAccelerationBonus { get; set; }
		public float SinkAccelerationBonusLimit { get; set; }
		public float FloatAccelerationBonus { get; set; }
		public float FloatAccelerationBonusLimit { get; set; }
		public float SmoothMovementChance { get; set; }
		public float DartChance { get; set; }

		public MotionConfig() : base() {
			DifficultyMultiplier = 1;
			DifficultyAdditive = 0;
			SinkAccelerationBonus = 0;
			SinkAccelerationBonusLimit = 0;
			FloatAccelerationBonus = 0;
			FloatAccelerationBonusLimit = 0;
			SmoothMovementChance = 0.0005f;
			DartChance = 0;
		}
	}

	public class MixedMotionConfig : MotionConfig {}

	public class DartMotionConfig : MotionConfig {
		public DartMotionConfig() : base() {
			DartChance = 0.001f;
		}
	}

	public class SmoothMotionConfig : MotionConfig {
		public SmoothMotionConfig() : base() {
			SmoothMovementChance = 0;
		}
	}

	public class SinkerMotionConfig : MotionConfig {
		public SinkerMotionConfig() : base() {
			SinkAccelerationBonus = 0.01f;
			SinkAccelerationBonusLimit = 1.5f;
		}
	}

	public class FloaterMotionConfig : MotionConfig {
		public FloaterMotionConfig() : base() {
			FloatAccelerationBonus = 0.01f;
			FloatAccelerationBonusLimit = 1.5f;
		}
	}

	public class LocationSettingsConfig {
		public LocationSettingsBoundary[] LocationSettings { get; set; }

		public LocationSettingsConfig() {
			LocationSettings = new LocationSettingsBoundary[] { new LocationSettingsBoundary() };
		}
	}

	public class LocationSettingsBoundary {
		public string[] LocationNames { get; set; }
		public string[] BoundingBoxes { get; set; }
		public string[] ExclusionBoxes { get; set; }
		public LocationSettingsData Settings { get; set; }

		public LocationSettingsBoundary() {
			LocationNames = new string[] { "Farm" };
			BoundingBoxes = new string[] { "1,1|-1,-1" };
			ExclusionBoxes = new string[] { "1,1|3,3" };
			Settings = new LocationSettingsData();
		}

		public bool WithinBoundary(GameLocation Loc, Vector2 BobberLocation) {
			bool ValidLocation = false;
			if (LocationNames != null) {
				foreach (string Name in LocationNames) {
					if (Name.StartsWith("UndergroundMine_")) {
						int Level = int.Parse(Name.Split(new char[] { '_' })[1]);

						if (Game1.inMine && Game1.mine.mineLevel == Level) {
							ValidLocation = true;
							break;
						}
					}
					if (Name.Equals(Loc.Name)) {
						ValidLocation = true;
						break;
					}
				}
			}

			if (ValidLocation && BoundingBoxes != null) {
				Rectangle BobberRect = new Rectangle((int) BobberLocation.X - Game1.tileSize * 5 / 4, (int) BobberLocation.Y - Game1.tileSize * 5 / 4, Game1.tileSize, Game1.tileSize);

				foreach (string BoundingBox in BoundingBoxes) {
					string[] BoundCoordStrings = BoundingBox.Split(new char[] { ',', '|', '/', '\\' });
					int[] BoundCoordList = new int[] {
						int.Parse(BoundCoordStrings[0]),
						int.Parse(BoundCoordStrings[1]),
						int.Parse(BoundCoordStrings[2]),
						int.Parse(BoundCoordStrings[3]),
					};

					if (BoundCoordList[2] == -1) { BoundCoordList[2] = Game1.currentLocation.map.DisplayWidth; }
					if (BoundCoordList[3] == -1) { BoundCoordList[3] = Game1.currentLocation.map.DisplayHeight; }

					BoundCoordList[2] -= BoundCoordList[0];
					BoundCoordList[3] -= BoundCoordList[1];

					for (int x = 0; x < 4; x++) {
						BoundCoordList[x] *= Game1.tileSize;
					}

					Rectangle Bounds = new Rectangle(BoundCoordList[0], BoundCoordList[1], BoundCoordList[2], BoundCoordList[3]);
					if (Bounds.Intersects(BobberRect)) {
						if (ExclusionBoxes != null) {
							foreach (string ExclusionBox in ExclusionBoxes) {
								string[] ExcludeCoordStrings = ExclusionBox.Split(new char[] { ',', '|', '/', '\\' });
								int[] ExcludeCoordList = new int[] {
								int.Parse(ExcludeCoordStrings[0]) * Game1.tileSize,
								int.Parse(ExcludeCoordStrings[1]) * Game1.tileSize,
								(int.Parse(ExcludeCoordStrings[2]) - int.Parse(BoundCoordStrings[0])) * Game1.tileSize,
								(int.Parse(ExcludeCoordStrings[3]) - int.Parse(BoundCoordStrings[1])) * Game1.tileSize,
							};

								Rectangle Exclude = new Rectangle(ExcludeCoordList[0], ExcludeCoordList[1], ExcludeCoordList[2], ExcludeCoordList[3]);
								if (Exclude.Intersects(BobberRect)) {
									return false;
								}
							}
						}

						return true;
					}
				}
			}

			return false;
		}
	}

	public class LocationSettingsData : CustomizedSettingsConfig {}
}