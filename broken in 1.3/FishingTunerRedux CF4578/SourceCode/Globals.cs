using System.Collections.Generic;

namespace FishingTunerRedux {
	public static class Motions {
		public const int Mixed = 0;
		public const int Dart = 1;
		public const int Smooth = 2;
		public const int Sinker = 3;
		public const int Floater = 4;
	}

	public static class Baits {
		public const int Bait = 685;
		public const int Magnet = 703;
		public const int WildBait = 774;
	}

	public static class Tackles {
		public const int Spinner = 686;
		public const int DressedSpinner = 687;
		public const int BarbedHook = 691;
		public const int LeadBobber = 692;
		public const int TreasureHunter = 693;
		public const int TrapBobber = 694;
		public const int CorkBobber = 695;
	}

	public static class LegendaryFish {
		public const int Crimsonfish = 159;
		public const int Angler = 160;
		public const int Legend = 163;
		public const int MutantCarp = 682;
		public const int Glacierfish = 775;
		public static int[] ToArray() {
			return new int[] { Crimsonfish, Angler, Legend, MutantCarp, Glacierfish };
		}
	}

	public static class Configs {
		public static GeneralSettingsConfig GeneralSettings { get; set; }
		public static FishingPointConfig FishingPoint { get; set; }
		public static CastDistanceConfig CastDistance { get; set; }
		public static DailyLuckConfig DailyLuck { get; set; }
		public static PlayerLuckConfig PlayerLuck { get; set; }
		public static FishDifficultyConfig FishDifficulty { get; set; }
		public static FishingLevelConfig FishingLevel { get; set; }
		public static CustomizedSettingsConfig Bait { get; set; }
		public static CustomizedSettingsConfig Tackle { get; set; }
		public static MotionConfig FishMotion { get; set; }
		public static LocationSettingsConfig LocationSettings { get; set; }
		public static List<LocationSettingsData> AppliedLocationEffects = new List<LocationSettingsData>();
	}

	public static class ClampExtensions {
		public static int Clamp(this int value, int min, int max) {
			return (value <= min) ? min : ((value >= max) ? max : value);
		}

		public static float Clamp(this float value, float min, float max) {
			return (value <= min) ? min : ((value >= max) ? max : value);
		}
	}
}